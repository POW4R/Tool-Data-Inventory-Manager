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
    /// Interaction logic for AddMachineWindow.xaml
    /// </summary>
    public partial class AddMachineWindow : Window
    {
        private readonly AppDbContext _context;

        public AddMachineWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            ProductListBox.ItemsSource = _context.Products.ToList();
        }

        public Machine CreatedMachine { get; private set; }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var name = MachineNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Warning("Save failed: The machine name is empty.");
                MessageBox.Show("A gép neve kötelező.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProducts = ProductListBox.SelectedItems.Cast<Product>().ToList();

            CreatedMachine = new Machine
            {
                Name = name,
                Products = selectedProducts
            };

            _context.Machines.Add(CreatedMachine);
            _context.SaveChanges();

            Log.Information("New machine saved: {MachineName}, number of assigned products: {ProductCount}", name, selectedProducts.Count);

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
