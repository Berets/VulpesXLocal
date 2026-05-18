using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class SOCBASE
    {
        public string Description { get { return SOMDES.TrimEnd(); } }

        public ACCESOCE? AccessCompany { get; set; }
    }
}
