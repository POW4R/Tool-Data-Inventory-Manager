using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool_Data_Inventory_Manager.Features.InventoryManager.Models;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Views
{
    public class ProductViewModel
    {
        public Product Product { get; }

        public ProductViewModel(Product product)
        {
            Product = product;
        }

        public int ProductNumber => Product.ProductNumber;
        public string Type => Product.Type;

        public string Tools => string.Join(", ", Product.Tools.Select(t => t.Name));

        public string Nagyolomaro => string.Join(", ", Product.Tools.Where(t => t.IsNagyolomaro).Select(t => t.Name));
        public string Sorjazomaro => string.Join(", ", Product.Tools.Where(t => t.IsSorjazomaro).Select(t => t.Name));
        public string Simitomaro => string.Join(", ", Product.Tools.Where(t => t.IsSimitomaro).Select(t => t.Name));
        public string NagyoloI => string.Join(", ", Product.Tools.Where(t => t.IsNagyoloI).Select(t => t.Name));
        public string NagyoloII => string.Join(", ", Product.Tools.Where(t => t.IsNagyoloII).Select(t => t.Name));
        public string Simito2 => string.Join(", ", Product.Tools.Where(t => t.IsSimito2).Select(t => t.Name));
        public string Elofuro => string.Join(", ", Product.Tools.Where(t => t.IsElofuro).Select(t => t.Name));
        public string Csigafuro => string.Join(", ", Product.Tools.Where(t => t.IsCsigafuro).Select(t => t.Name));
    }
}
