using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager
{
    internal class Machine_Product
    {
        public int Id { get; set; }

        [Required]
        public string Machine_Number { get; set; }

        [Required]
        public string SAP_Product_Number { get; set; }

    }
}
