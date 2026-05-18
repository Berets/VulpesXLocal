using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Accounting.Assets
{
    public class AssetHistoryItem
    {
        public int Year { get; set; }
        public ACC_ASSETS_DEP_CIV_HISTORY? Civilistic { get; set; }
        public ACC_ASSETS_DEP_HISTORY? Fiscal { get; set; }
    }
}
