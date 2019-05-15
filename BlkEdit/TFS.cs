using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MR.BlkEdit
{
    public class TFS
    {

        public static bool TFSReady = false;

        public static TfsTeamProjectCollection currentTFSProjectCollection = null;

        public WorkItemStore workItemStore = null;

        public static readonly string TFSLINKTYPE_CHILD = "Child";

        public TFS()
        {
            if (currentTFSProjectCollection != null)
            {
                workItemStore = (WorkItemStore)currentTFSProjectCollection.GetService(typeof(WorkItemStore));
                if(workItemStore != null)
                 TFSReady = true;
            }

            else
            {
                MessageBox.Show("TFS connection is not ready!", "Error");
            }
            
        }

        public WorkItem GetWorkItemById(string Id)
        {
            string queryString = String.Format("SELECT * FROM WorkItems WHERE [System.Id] = {0} ORDER BY [System.Id]", Id);
            var query = new Query(workItemStore, queryString);
            var result = query.RunQuery();
            if (result.Count > 0)
                return result[0];
            return null;
        }
 

        public void ChangeWorkItemValue(WorkItem workItem,string ChangeItem, string NewValue)
        {
            try
            {
                workItem.Open();
                workItem.Fields[ChangeItem].Value = NewValue;
                workItem.Save();
                workItem.Close();
            }
            catch(Exception)
            {
                MessageBox.Show(string.Format("Error when change work item: {0}", workItem.Id), "Error");
            }
        }

        public void LinkToWorkItem(WorkItem LinkTo, WorkItem LinkFrom, string LinkType)
        {
            try
            {
                var linkType = workItemStore.WorkItemLinkTypes.LinkTypeEnds[LinkType];
                var relateLink = new RelatedLink(linkType, LinkFrom.Id);
                LinkTo.PartialOpen();
                LinkTo.Links.Add(relateLink);
                LinkTo.Save();
                LinkTo.Close();
                LinkFrom.Close();
                LinkFrom.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Error when Create item: {0}, error message:{1}", LinkTo.Id,e.Message), "Error");
            }
        }

        public WorkItemType GetWorkItemType(string workItemTypeString, string teamProjectName)
        {
            if(TFSReady && workItemTypeString.Length > 0 && teamProjectName.Length > 0)
            {
                return workItemStore.Projects[teamProjectName].WorkItemTypes[workItemTypeString];
            }
            return null;
        }
    }
}
