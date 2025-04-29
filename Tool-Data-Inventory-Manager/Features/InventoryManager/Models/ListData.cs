using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Models
{
    internal class ListData
    {
        public int Id { get; set; }

        [Required]
        public string Tool { get; set; }

        [Required]
        public string Matrial_Number { get; set; }

        [Required]
        public string MagPlace { get; set; }
    }
}
