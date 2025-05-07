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
using Serilog;
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Views
{
    /// <summary>
    /// Interaction logic for MachineAddWindow.xaml
    /// </summary>
    public partial class MachineAddWindow : Window
    {
        private readonly AppDbContext _context;

        public MachineAddWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadProducts();
        }

        private void LoadProducts()
        {
            ProductListBox.ItemsSource = _context.Products.ToList();
        }

        private void AddMachineButton_Click(object sender, RoutedEventArgs e)
        {
            var name = MachineNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                Log.Warning("Machine addition failed: name is missing.");
                MessageBox.Show("Kérlek adj meg egy gépnevet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProducts = ProductListBox.SelectedItems.Cast<Product>().ToList();

            var newMachine = new Machine
            {
                Name = name,
                Products = selectedProducts
            };

            _context.Machines.Add(newMachine);
            _context.SaveChanges();
            Log.Information("New machine added: {MachineName}, assigned products: {ProductCount}", name, selectedProducts.Count);

            MessageBox.Show("Gép sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}
