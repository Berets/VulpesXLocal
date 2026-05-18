using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class DDTListSelectorHandler
    {
        public static Func<string, int, int, string>? CallAction { get; set; }

        public static string Call(string CompanyID, int Year, int ID)
        {
            if (CallAction == null)
                return string.Empty;

            return CallAction.Invoke(
                CompanyID,
                Year,
                ID);
        }
    }
}
