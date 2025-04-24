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
    /// Interaction logic for AddMachineWindow.xaml
    /// </summary>
    public partial class AddMachineWindow : Window
    {
        private readonly AddMachineViewModel _viewModel;
        private readonly SelectMachineWindow _baseWindow;
        public AddMachineWindow(SelectMachineWindow baseWindow)
        {
            _viewModel = new AddMachineViewModel();
            _baseWindow = baseWindow;
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            string input = tb_MachineName.Text;
            
            _viewModel.AddMachine(int.Parse(input));
            _baseWindow.IsEnabled = true;
            this.Close();
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            _baseWindow.IsEnabled = true;
            this.Close();
        }

    }
}
