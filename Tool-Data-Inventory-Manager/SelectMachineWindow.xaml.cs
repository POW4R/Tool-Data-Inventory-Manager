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
            bool isMachineSelected = cb_Machine.SelectedItem != null;
            cb_SapProductId.IsEnabled = isMachineSelected;
            btn_AddSapProductId.IsEnabled = isMachineSelected;
            btn_DeleteSapProductId.IsEnabled = isMachineSelected;
            btn_DeleteMachine.IsEnabled = isMachineSelected;
        }
    }
}
