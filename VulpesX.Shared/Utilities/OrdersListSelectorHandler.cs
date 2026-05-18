using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class OrdersListSelectorHandler
    {
        public static Func<string, int, int, int, string>? CallAction { get; set; }

        public static string Call(string CompanyID, int Year, int ID, int IsProductionNeeded)
        {
            if (CallAction == null)
                return string.Empty;

            return CallAction.Invoke(
                CompanyID,
                Year,
                ID,
                IsProductionNeeded);
        }
    }
}
