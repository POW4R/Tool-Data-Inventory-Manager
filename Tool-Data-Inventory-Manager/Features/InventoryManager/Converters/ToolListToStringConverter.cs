using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;
using System.Windows.Data;
using System.Windows;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Converters
{
    public class ToolListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<Tool> tools)
                return string.Join(", ", tools.Select(t => t.Name));
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
