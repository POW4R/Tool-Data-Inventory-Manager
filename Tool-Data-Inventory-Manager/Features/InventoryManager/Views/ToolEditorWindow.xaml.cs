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
    public partial class ToolEditorWindow : Window
    {
        public Tool Tool { get; private set; }

        public ToolEditorWindow(Tool tool)
        {
            InitializeComponent();
            Tool = tool;

            NameTextBox.Text = tool.Name;
            MaterialNumberTextBox.Text = tool.MaterialNumber.ToString();
            MagPlaceTextBox.Text = tool.MagPlace?.ToString() ?? string.Empty;
            PriceTextBox.Text = tool.Price.ToString("F2");

            CheckNagyolomaro.IsChecked = tool.IsNagyolomaro;
            CheckSorjazomaro.IsChecked = tool.IsSorjazomaro;
            CheckSimitomaro.IsChecked = tool.IsSimitomaro;
            CheckNagyoloI.IsChecked = tool.IsNagyoloI;
            CheckNagyoloII.IsChecked = tool.IsNagyoloII;
            CheckSimito2.IsChecked = tool.IsSimito2;
            CheckElofuro.IsChecked = tool.IsElofuro;
            CheckCsigafuro.IsChecked = tool.IsCsigafuro;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MaterialNumberTextBox.Text, out int materialNumber) &&
                decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                Tool.Name = NameTextBox.Text;
                Tool.MaterialNumber = materialNumber;
                Tool.MagPlace = int.TryParse(MagPlaceTextBox.Text, out int mag) ? mag : null;
                Tool.Price = price;

                Tool.IsNagyolomaro = CheckNagyolomaro.IsChecked == true;
                Tool.IsSorjazomaro = CheckSorjazomaro.IsChecked == true;
                Tool.IsSimitomaro = CheckSimitomaro.IsChecked == true;
                Tool.IsNagyoloI = CheckNagyoloI.IsChecked == true;
                Tool.IsNagyoloII = CheckNagyoloII.IsChecked == true;
                Tool.IsSimito2 = CheckSimito2.IsChecked == true;
                Tool.IsElofuro = CheckElofuro.IsChecked == true;
                Tool.IsCsigafuro = CheckCsigafuro.IsChecked == true;

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Hibás adatformátum.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
