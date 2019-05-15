using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.BlkEdit
{
    
    public delegate void ChangeProgressBarCallBack(int total, int current);
    class ChangeSuitBin
    {

        private TFS tfs = null;
        private string[] SuitBinsToProcess = null;

        public ChangeSuitBin(ChangeProgressBarCallBack changeProgressar)
        {
            tfs = new TFS();
            if (TFS.TFSReady)
            {
                SuitBinsToProcess = CheckSuitBinFormat();
                ChangeSuitBinInBulk(changeProgressar);
            }
        }
        private string GetCurrentSuitBinValue(WorkItem workItem)
        {
            if(workItem != null)
            {
                try
                {
                    return workItem.Fields["TCSuiteBin"].Value.ToString();
                }
                catch(Exception)
                {

                }
            }
            return null;
        }

        public void ChangeSuitBinInBulk(ChangeProgressBarCallBack changeProgressar)
        {
            int totalCount = SelectedItems.Instance.WorkItemIDs.Length;
            int finishedCouht = 0;
            foreach(int Id in SelectedItems.Instance.WorkItemIDs)
            {
                var wi = tfs.GetWorkItemById(Id.ToString());
                tfs.ChangeWorkItemValue(wi, "TCSuiteBin", CombinSuitBin(SuitBinsToProcess, GetCurrentSuitBinValue(wi)));
                finishedCouht++;
            }
            changeProgressar(totalCount, finishedCouht);
        }

        private string[] CheckSuitBinFormat()
        {
            if(SelectedItems.Instance.SuitBintoChange.Length >0)
            {
                string[] suitBins = SelectedItems.Instance.SuitBintoChange.Split(';');

                if (suitBins.Length > 0)
                    return suitBins;
                else
                {
                    if(SelectedItems.Instance.SuitBintoChange.EndsWith("_"))
                    {
                        suitBins = new [] {SelectedItems.Instance.SuitBintoChange};
                    }
                }
            }
            return null;
        }

        private string CombinSuitBin(string[] newSuitBins, string currentSuitBins)
        {
            string newSuitBinsString = string.Empty;
            foreach(string sb in newSuitBins)
            {
                if (currentSuitBins.Contains(sb))
                    continue;
                if(sb.StartsWith("-"))
                {
                    currentSuitBins = currentSuitBins.Replace((sb.Trim('-') + ";"),string.Empty);
                }
                else
                {
                    newSuitBinsString += sb + ";\r\n";
                }
                
            }
            newSuitBinsString += currentSuitBins;
            return newSuitBinsString;
        }
    }
}
