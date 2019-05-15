using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MR.BlkEdit
{
    /// <summary>
    /// Interaction logic for LogResultWindow.xaml
    /// </summary>
    public partial class LogResultWindow : Window
    {
        public LogResultWindow()
        {
            InitializeComponent();
            AddTemplateToListBox();
            AddSelectedTemplatedToListBox();
        }

        private void BrowseButton_Click_BrowseButton(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            
            if (dialog.SelectedPath.Length > 0)
            {
                GlobalConfig.SaveConfig(new[] { dialog.SelectedPath }, System.IO.Path.Combine(GlobalConfig.ConfigFilePath, "TemplatePath.txt"));
                LogResult.ResultTemplatePath = dialog.SelectedPath;
                FolderPathTestBlock.Text = dialog.SelectedPath;
                AddTemplateToListBox();
            }
            //else
            //{
            //    MessageBox.Show("Please Select Template Directory", "Informartion");
            //}

        }

        private void AddTemplateToListBox()
        {
            
            if (LogResult.ResultTemplatePath != null && System.IO.Directory.Exists(LogResult.ResultTemplatePath))
            {
                FolderPathTestBlock.Text = LogResult.ResultTemplatePath;
                foreach (var t in GlobalConfig.GetAllAvalialetemplates())
                {
                    if (t.Length >= 0 && !CheckIfTemplateAlreadyinSaveList(t))
                        AvaliableTemplateBox.Items.Add(t);
                }
            }
            else
            {
                MessageBox.Show("Please Specify Template folder.", "Information");
            }
        }

        private void AddSelectedTemplatedToListBox()
        {
            if (LogResult.SelectedTemplates != null)
            {
                foreach (var t in LogResult.SelectedTemplates)
                {
                    if (t.Length > 0)
                        SelectedItemListBox.Items.Add(t);
                }
            }
        }
        private bool CheckIfTemplateAlreadyinSaveList(string template)
        {
            if (LogResult.SelectedTemplates == null)
                return false;
            foreach(var t in LogResult.SelectedTemplates)
            {
                if (t == template)
                    return true;
            }
            return false;
        }

        private void SwapItems(ListBox From, ListBox To,bool Save)
        {
            object o = From.SelectedItem;
            From.Items.Remove(o);
            To.Items.Add(o);
            if(Save)
            {
                List<string> newSaveItems = new List<string>();
                foreach(var item in To.Items)
                {
                    newSaveItems.Add((string)item);
                }
                GlobalConfig.SaveConfig(newSaveItems.ToArray(),System.IO.Path.Combine(GlobalConfig.ConfigFilePath, "SelectedTemplates.txt"));
                LogResult.SelectedTemplates = newSaveItems.ToArray();
            }
            else
            {
                List<string> newSaveItems = new List<string>();
                foreach (var item in From.Items)
                {
                    newSaveItems.Add((string)item);
                }
                GlobalConfig.SaveConfig(newSaveItems.ToArray(), System.IO.Path.Combine(GlobalConfig.ConfigFilePath, "SelectedTemplates.txt"));
                LogResult.SelectedTemplates = newSaveItems.ToArray();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if(AvaliableTemplateBox.SelectedIndex != -1)
            {
                SwapItems(AvaliableTemplateBox, SelectedItemListBox, true);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItemListBox.SelectedIndex != -1)
            {
                SwapItems(SelectedItemListBox,AvaliableTemplateBox, false);
            }
        }

        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            DealWithPassFailButton(LogResult.TEST_PASS_STRING);
        }

        private void FailButton_Click(object sender, RoutedEventArgs e)
        {
            DealWithPassFailButton(LogResult.TEST_Fail_STRING);
        }

        private void changeLogResultWindowStatus()
        {
            this.Title = "Processing...";
            this.IsEnabled = false;
            PassButton.IsEnabled = false;
            FailButton.IsEnabled = false;
            BrowseButton.IsEnabled = false;
        }

        private void DealWithPassFailButton(string passOrFail)
        {
            if (SelectedItemListBox.Items.Count > 0)
            {
                changeLogResultWindowStatus();
                new LogResult(passOrFail);
                this.Close();
            }
            else
            {
                MessageBox.Show("No Test Result Template Selected", "Error");
            }
        }
    }
}
