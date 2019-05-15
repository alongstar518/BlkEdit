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
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (SuitBinTextBox.Text.Length > 0)
            {
                this.IsEnabled = false;
                SelectedItems.Instance.SuitBintoChange = SuitBinTextBox.Text;
                progressBar.Visibility = Visibility.Visible;
                this.Title = "Processing...";
                
                new ChangeSuitBin(UpgradeProcessBar);
                this.Close();
            }
            else
            {
                errorMessageLable.Content = "Please input suit bin string";
            }
        }

        private void UpgradeProcessBar(int total,int current)
        {
            
            progressBar.Maximum = total;
            progressBar.Minimum = 1;
            progressBar.Value = current;
            if(current == total)
            {
                MessageBox.Show("Done! " + current.ToString() + " items updated.", "Informaiton");
            }
        }

    }
}
