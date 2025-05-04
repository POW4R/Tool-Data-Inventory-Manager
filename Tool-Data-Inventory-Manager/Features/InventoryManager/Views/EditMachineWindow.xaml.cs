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
    /// Interaction logic for EditMachineWindow.xaml
    /// </summary>
    public partial class EditMachineWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Machine _machine;

        public EditMachineWindow(AppDbContext context, Machine machine)
        {
            InitializeComponent();
            _context = context;
            _machine = machine;

            MachineNameTextBox.Text = machine.Name;

            var products = _context.Products.ToList();
            ProductListBox.ItemsSource = products;

            foreach (var product in machine.Products)
            {
                var match = products.FirstOrDefault(p => p.Id == product.Id);
                if (match != null)
                    ProductListBox.SelectedItems.Add(match);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var name = MachineNameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Log.Warning("Machine save failed: the name was empty.");
                MessageBox.Show("A gép neve kötelező.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _machine.Name = name;
            _machine.Products.Clear();

            foreach (Product p in ProductListBox.SelectedItems)
            {
                _machine.Products.Add(p);
            }

            _context.SaveChanges(); 
            Log.Information("Machine saved: {MachineName}, number of assigned products: {ProductCount}", _machine.Name, _machine.Products.Count);

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
