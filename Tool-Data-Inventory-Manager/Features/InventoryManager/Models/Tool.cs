using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager.Features.InventoryManager.Models
{
    public class Tool
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // or Guid.NewSequentialId()
        public int MaterialNumber { get; set; }
        public string Name { get; set; }
        public int? MagPlace { get; set; }
        public decimal Price { get; set; }

        //Categories
        public bool IsNagyolomaro { get; set; }
        public bool IsSorjazomaro { get; set; }
        public bool IsSimitomaro { get; set; }
        public bool IsNagyoloI { get; set; }
        public bool IsNagyoloII { get; set; }
        public bool IsSimito2 { get; set; }
        public bool IsElofuro { get; set; }
        public bool IsCsigafuro { get; set; }
    }
}
