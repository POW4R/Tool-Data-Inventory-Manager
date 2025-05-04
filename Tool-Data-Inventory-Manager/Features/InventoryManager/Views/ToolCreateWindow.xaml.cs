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
    public partial class ToolCreateWindow : Window
    {
        public Tool NewTool { get; private set; }

        public ToolCreateWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(MaterialNumberTextBox.Text, out int materialNumber))
            {
                MessageBox.Show("Hibás formátum a Material Number vagy Ár mezőben.");
                return;
            }

            int? magPlace = null;
            if (int.TryParse(MagPlaceTextBox.Text, out int parsedMag))
            {
                magPlace = parsedMag;
            }

            NewTool = new Tool
            {
                Name = NameTextBox.Text,
                MaterialNumber = materialNumber,
                MagPlace = magPlace,

                IsNagyolomaro = CheckNagyolomaro.IsChecked == true,
                IsSorjazomaro = CheckSorjazomaro.IsChecked == true,
                IsSimitomaro = CheckSimitomaro.IsChecked == true,
                IsNagyoloI = CheckNagyoloI.IsChecked == true,
                IsNagyoloII = CheckNagyoloII.IsChecked == true,
                IsSimito2 = CheckSimito2.IsChecked == true,
                IsElofuro = CheckElofuro.IsChecked == true,
                IsCsigafuro = CheckCsigafuro.IsChecked == true
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
