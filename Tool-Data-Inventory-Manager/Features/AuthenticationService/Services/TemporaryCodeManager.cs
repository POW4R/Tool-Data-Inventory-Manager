using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager
{
    public static class TemporaryCodeManager
    {

        public static int GenerateCode(string email)
        {
            int code = new Random().Next(100000, 999999);
            return code;
        }
    }
}
