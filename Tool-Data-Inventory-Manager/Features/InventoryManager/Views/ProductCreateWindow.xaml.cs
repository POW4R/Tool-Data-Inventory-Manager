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
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Views
{
    public partial class ProductCreateWindow : Window
    {
        private readonly AppDbContext _context;
        public Product NewProduct { get; private set; }

        public ProductCreateWindow(AppDbContext context)
        {
            InitializeComponent();
            _context = context;

            ToolsListBox.ItemsSource = _context.Tools.ToList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ProductNumberTextBox.Text, out int productNumber))
            {
                MessageBox.Show("Hibás Product Number.");
                return;
            }

            var selectedTools = ToolsListBox.SelectedItems.Cast<Tool>().ToList();

            NewProduct = new Product
            {
                ProductNumber = productNumber,
                Type = TypeTextBox.Text,
                Tools = new List<Tool>(selectedTools)
            };

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
