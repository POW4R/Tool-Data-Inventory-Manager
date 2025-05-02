using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int ProductNumber { get; set; }
        public string Type { get; set; }

        public List<Tool> Tools { get; set; } = new();
    }
}
