using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.BlkEdit
{
    static class GlobalConfig
    {
        public static string ConfigFilePath = null;

        public static bool LoadConfigFile()
        {
            bool ret = false;
            string configFileFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            GlobalConfig.ConfigFilePath = Path.Combine(configFileFolder, "BlkEditConfig");
            if (Directory.Exists(GlobalConfig.ConfigFilePath))
            {
                string templatePathConfigFile = Path.Combine(GlobalConfig.ConfigFilePath, "TemplatePath.txt");
                if (File.Exists(templatePathConfigFile))
                {
                    try
                    {
                        LogResult.ResultTemplatePath = File.ReadAllLines(templatePathConfigFile)[0];
                        ret = true;
                    }
                    catch(Exception)
                    {

                    }
                }
                else
                {
                    var fs = File.Create(templatePathConfigFile);
                    fs.Close();
                    
                }
                    

                string selectedTemplateConfigFile = Path.Combine(GlobalConfig.ConfigFilePath, "SelectedTemplates.txt");

                if (File.Exists(selectedTemplateConfigFile) && ret)
                {
                    try
                    {
                        LogResult.SelectedTemplates = File.ReadAllLines(selectedTemplateConfigFile);
                        VerifyAndRemoveLostTemplate();
                        return ret;
                    }
                    catch(Exception)
                    {

                    }
                }
                else
                {
                    var fs = File.Create(selectedTemplateConfigFile);
                    fs.Close();
                    ret = false;
                }
                
            }
            else
            {
                Directory.CreateDirectory(ConfigFilePath);
                LoadConfigFile();
            }
            return ret;
        }

        public static void SaveConfig(string[] config, string configFile)
        {
            File.Delete(configFile);
            var fs = File.Create(configFile);
            var sw = new StreamWriter(fs);
            foreach(string line in config)
            {
                sw.WriteLine(line);
            }
            sw.Flush();
            sw.Close();
        }

        public static string[] GetAllAvalialetemplates()
        {
           return Directory.GetFiles(LogResult.ResultTemplatePath, "*.wt");
        }

        private static void VerifyAndRemoveLostTemplate()
        {
            List<string> selectedItems = new List<string>();
            foreach(string t in LogResult.SelectedTemplates)
            {
                if(File.Exists(t))
                {
                    selectedItems.Add(t);
                }
            }
            LogResult.SelectedTemplates = selectedItems.ToArray();
        }

    }


}
