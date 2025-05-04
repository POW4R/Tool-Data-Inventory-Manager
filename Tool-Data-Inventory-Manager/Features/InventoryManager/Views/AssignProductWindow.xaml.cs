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
    /// Interaction logic for AssignProductWindow.xaml
    /// </summary>
    public partial class AssignProductWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Machine _machine;

        public AssignProductWindow(AppDbContext context, Machine machine)
        {
            InitializeComponent();
            _context = context;
            _machine = machine;

            LoadAvailableProducts();
        }

        private void LoadAvailableProducts()
        {
            // Csak azokat a termékeket jelenítjük meg, amik még nincsenek ehhez a géphez rendelve
            var assignedIds = _machine.Products.Select(p => p.Id).ToList();
            var products = _context.Products.Where(p => !assignedIds.Contains(p.Id)).ToList();
            AvailableProductsListBox.ItemsSource = products;
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (AvailableProductsListBox.SelectedItem is Product selectedProduct)
            {
                _machine.Products.Add(selectedProduct);
                Log.Information("Product assigned: {ProductName} to machine: {MachineName}", selectedProduct.ProductNumber, _machine.Name);

                DialogResult = true;
            }
            else
            {
                Log.Warning("Product assignment failed: no product selected.");
                MessageBox.Show("Válassz ki egy terméket.");
            }
        }
    }
}
