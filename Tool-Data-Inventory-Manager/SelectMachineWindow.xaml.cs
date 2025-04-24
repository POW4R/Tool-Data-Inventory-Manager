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
    /// Interaction logic for SelectMachineWindow.xaml
    /// </summary>
    public partial class SelectMachineWindow : Window
    {
        public SelectMachineWindow()
        {
            InitializeComponent();
        }
        private void cb_Machine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cb_Machine.Items.Add("7350_4");
        }

        private void btn_add_product_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_add_machine_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("");
        }

        private void btn_del_product_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_del_machine_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
