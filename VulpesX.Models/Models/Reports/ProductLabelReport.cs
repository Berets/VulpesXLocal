using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports
{
    public class ProductLabelReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }
        public tab_articolo? Articolo { get; set; }
    }
}
