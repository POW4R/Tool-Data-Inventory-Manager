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

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Views
{
    /// <summary>
    /// Interaction logic for ListMachineData.xaml
    /// </summary>
    public partial class ListMachineData : Window
    {
        public ListMachineData()
        {
            InitializeComponent();
        }

        private void btn_add_magPlace_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            SelectMachineWindow smw = new SelectMachineWindow();
            this.Close();
            smw.Show();
        }
    }
}
