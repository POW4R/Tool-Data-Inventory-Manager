using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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
            LoadMachines(true);
        }
        private void cb_Machine_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(cb_Machine.SelectedItem.ToString().Trim()))
            {
                btn_del_machine.IsEnabled = true;
                cb_SapProductId.IsEnabled = true;
                btn_add_product.IsEnabled = true;
                btn_del_product.IsEnabled = true;
                btn_next.IsEnabled = true;
            }
            else
            {
                btn_del_machine.IsEnabled = false;
                cb_SapProductId.IsEnabled = false;
                btn_add_product.IsEnabled = false;
                btn_del_product.IsEnabled = false;
                btn_next.IsEnabled = false;
            }
        }
        private void btn_add_product_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btn_del_product_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_del_machine_Click(object sender, RoutedEventArgs e)
        {
            if (cb_Machine.SelectedItem == null || string.IsNullOrWhiteSpace(cb_Machine.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a machine to delete.");
                return;
            }

            string selectedMachine = cb_Machine.SelectedItem.ToString();

            var machinesToDelete = _dbContext.Machine_Products
                                              .Where(m => m.Machine_Number == selectedMachine)
                                              .ToList();

            if (!machinesToDelete.Any())
            {
                MessageBox.Show("Selected machine not found in the database.");
                return;
            }

            _dbContext.Machine_Products.RemoveRange(machinesToDelete);

            try
            {
                _dbContext.SaveChangesAsync();
                MessageBox.Show("Machine deleted successfully.");

                LoadMachines(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting machine: " + ex.Message);
            }
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadMachines(bool loadWithEmptyField)
        {
            var machines = _dbContext.Machine_Products
                                     .Select(m => m.Machine_Number)
                                     .Distinct()
                                     .ToList();

            if (loadWithEmptyField)
            {
                machines.Insert(0, string.Empty);
                cb_Machine.SelectedIndex = 0;
            }
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
                    LoadMachines(true); // frissítjük a listát
                }
            }
        }
    }
}
