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
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;

namespace Tool_Data_Inventory_Manager
{
    /// <summary>
    /// Interaction logic for SelectMachineWindow.xaml
    /// </summary>
    public partial class SelectMachineWindow : Window
    {
        private readonly MainWindow _mainWindow;
        private readonly AppDbContext _dbContext = new AppDbContext();
        public SelectMachineWindow()
        {
            InitializeComponent();
            LoadMachines();
        }
        private void cb_Machine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void btn_add_product_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_del_product_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_del_machine_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadMachines()
        {
            var machines = _dbContext.Machine_Products
                                      .Select(m => m.Machine_Number)
                                      .Distinct()
                                      .ToList();

            cb_Machine.ItemsSource = machines;
        }

        private void btn_add_machine_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("Add new machine number:");
            if (inputDialog.ShowDialog() == true)
            {
                string newMachine = inputDialog.ResponseText;

                if (!string.IsNullOrWhiteSpace(newMachine))
                {
                    var newMachineProduct = new Machine_Product
                    {
                        Machine_Number = newMachine,
                        SAP_Product_Number = "" // vagy kérdezzük be később
                    };

                    _dbContext.Machine_Products.Add(newMachineProduct);
                    _dbContext.SaveChanges();

                    MessageBox.Show("Machine added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadMachines(); // frissítjük a listát
                }
            }
        }
    }
}
