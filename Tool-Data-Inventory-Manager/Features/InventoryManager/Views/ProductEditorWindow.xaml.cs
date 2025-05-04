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
    public partial class ProductEditorWindow : Window
    {
        private readonly AppDbContext _context;
        private readonly Product _product;

        public ProductEditorWindow(Product product, AppDbContext context)
        {
            InitializeComponent();
            _context = context;
            _product = product;

            ProductNumberTextBox.Text = product.ProductNumber.ToString();
            TypeTextBox.Text = product.Type;

            ToolsListBox.ItemsSource = _context.Tools.ToList();

            foreach (var tool in product.Tools)
            {
                ToolsListBox.SelectedItems.Add(tool);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ProductNumberTextBox.Text, out int productNumber))
            {
                _product.ProductNumber = productNumber;
                _product.Type = TypeTextBox.Text;

                _product.Tools.Clear();
                foreach (Tool selectedTool in ToolsListBox.SelectedItems)
                {
                    _product.Tools.Add(selectedTool);
                }
                Log.Information("Product saved: ProductNumber = {ProductNumber}, Type = {Type}, Tools = [{Tools}]", _product.ProductNumber, _product.Type, string.Join(", ", _product.Tools.Select(t => t.Name)));

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Hibás 'ProductNumber' mező!");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
