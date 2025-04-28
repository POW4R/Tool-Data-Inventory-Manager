using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager
{
    public static class TemporaryCodeManager
    {
        private static Dictionary<string, (string code, DateTime expiry)> _codes = new();

        public static string GenerateCode(string email)
        {
            var code = new Random().Next(100000, 999999).ToString();
            _codes[email] = (code, DateTime.Now.AddMinutes(10));
            return code;
        }

        public static bool ValidateCode(string email, string inputCode)
        {
            if (_codes.TryGetValue(email, out var codeInfo))
            {
                if (codeInfo.code == inputCode && DateTime.Now <= codeInfo.expiry)
                {
                    _codes.Remove(email);
                    return true;
                }
            }
            return false;
        }
    }
}
