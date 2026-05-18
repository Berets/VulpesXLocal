using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports
{
    public class LotLabelReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }

        public tab_articolo? Articolo { get; set; }
        public store_stocks_lots? Lotto { get; set; }
    }
}
