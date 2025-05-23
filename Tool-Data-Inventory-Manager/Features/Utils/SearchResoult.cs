using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager.Features.Utils
{
    class SearchResoult
    {
        public string MachineName { get; set; }
        public string ProductNumber { get; set; }
        public string ToolName { get; set; }
        public string MaterialNumber { get; set; }
        public string MagPlace { get; set; }

        public SearchResoult() { }
    }
}
