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

namespace Tool_Data_Inventory_Manager
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string ResponseText { get; private set; }
        public InputDialog(string question)
        {
            InitializeComponent();
            this.Title = question;
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.ResponseText = txtResponse.Text;
            this.DialogResult = true;
        }
    }
}
