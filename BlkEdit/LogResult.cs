using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace MR.BlkEdit
{
    public class LogResult
    {
        public static string ResultTemplatePath = null;

        public static string[] SelectedTemplates = null;

        public static string TEST_PASS_STRING = "pass";

        public static string TEST_Fail_STRING = "fail";

        public static bool GlobalTemplateConfigLoaded = false;

        private TFS tfs;

        public LogResult(string passString)
        {
            tfs = new TFS();
            ProcessLogResult(SelectedItems.Instance.WorkItemIDs,passString);
        }


        private Dictionary<string, string> GetResultTemplateFromFile(string templateFilePath)
        {
            if (!File.Exists(templateFilePath))
            {
                if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), templateFilePath)))
                {
                    throw new FileNotFoundException(string.Format("file : {0} doesn't exist, pls. check its location ", templateFilePath));
                }
                templateFilePath = Path.Combine(Directory.GetCurrentDirectory(), templateFilePath);
            }

            Dictionary<string, string> templateItems = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(templateFilePath);
            XmlNode projectName = doc.SelectSingleNode("//Template/TeamProjectName");
            XmlNode workItemTypeName = doc.SelectSingleNode("//Template/WorkItemTypeName");

            if (workItemTypeName.InnerText != "Test Result")
                return null;

            templateItems["TeamProjectName"] = projectName.InnerText;
            templateItems["WorkItemTypeName"] = workItemTypeName.InnerText;
            // Extract all fields from xml template
            XmlNodeList fieldNodes = doc.SelectNodes("//Template/FieldValues/FieldValue");
            foreach (XmlNode node in fieldNodes)
            {
                string id = node.SelectSingleNode("ReferenceName").InnerXml;

                string value = node.SelectSingleNode("Value").InnerText;
                templateItems[id] = value;
            }


            return templateItems;
        }

        private WorkItem CreateTestResultWorkItem(string resultValue,WorkItem testCase,Dictionary<String, String> resultTemplate = null)
        {
            var testResultType = tfs.GetWorkItemType(resultTemplate["WorkItemTypeName"], resultTemplate["TeamProjectName"]);
            var testResultWorkItem = new WorkItem(testResultType);
            testResultWorkItem.PartialOpen();

            if (resultTemplate != null)
            {
                foreach (string key in resultTemplate.Keys)
                {
                    if(key != "TeamProjectName" && key !="WorkItemTypeName")
                        testResultWorkItem.Fields[key].Value = resultTemplate[key];
                }
            }
            testResultWorkItem["Test Case ID"] = testCase.Id;
            if (String.IsNullOrEmpty((String)testResultWorkItem["Title"])) testResultWorkItem["Title"] = testCase.Title;
            testResultWorkItem["Microsoft.VSTS.Common.TestResultValue"] = (resultValue.ToLower() == "pass") ? "Pass" : "Fail";
            return testResultWorkItem;
        }

        private void LogResultForTestCase(WorkItem testCase,string passString)
        {
            foreach(string t in SelectedTemplates)
            {
                var resultTemplate = GetResultTemplateFromFile(t);
                if (resultTemplate == null)
                    continue;

                var resultWorkItem = CreateTestResultWorkItem(passString, testCase, resultTemplate);
                resultWorkItem.Save();
                var linkType =  tfs.workItemStore.WorkItemLinkTypes.LinkTypeEnds[TFS.TFSLINKTYPE_CHILD];
                var relatedLink = new RelatedLink(linkType, resultWorkItem.Id);
                              
                testCase.Links.Add(relatedLink);
                
                testCase.Save();
                testCase.Close();
                resultWorkItem.Close();
                
            }
        }

        public int ProcessLogResult(int[] workItemIDs, string passString)
        {
            int finishedCount = 0; 

                foreach(int workItemID in workItemIDs)
                {
                    try
                    {
                        var wi = tfs.GetWorkItemById(workItemID.ToString());
                        LogResultForTestCase(wi, passString);
                        finishedCount++;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("Error, Log result for work Item {0} Failed, Error Message:\r\n {1}", workItemID.ToString(),e.Message),"Error");
                        break;
                    }
                }
            MessageBox.Show(string.Format("{0} test cases processed,",workItemIDs.Length),"Information");
            return finishedCount;
        }

    }
}
