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
    /// <summary>
    /// Interaction logic for ToolHasProductsWindow.xaml
    /// </summary>
    public partial class ToolHasProductsWindow : Window
    {
        public bool Confirmed { get; private set; } = false;

        public ToolHasProductsWindow(List<Product> relatedProducts)
        {
            InitializeComponent();
            ProductList.ItemsSource = relatedProducts.Select(p => $"{p.ProductNumber} - {p.Type}");
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
