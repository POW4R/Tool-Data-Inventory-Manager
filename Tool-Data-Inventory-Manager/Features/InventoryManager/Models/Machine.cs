using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Models
{
    public class Machine
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        public List<Product> Products { get; set; } = new();
    }
}
