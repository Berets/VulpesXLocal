using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class RFFTB00F
    {
        public string? rffspa { get; set; }
        public bool rffspaBool
        {
            get
            {
                return rffspa == "S";
            }
            set
            {
                if (value)
                    rffspa = "S";
                else
                    rffspa = "N";
            }
        }

        public string? rffSpedoffacq { get; set; }
        public bool rffSpedoffacqBool
        {
            get
            {
                return rffSpedoffacq == "S";
            }
            set
            {
                if (value)
                    rffSpedoffacq = "S";
                else
                    rffSpedoffacq = "N";
            }
        }

        public string? rffSpedOffVend { get; set; }
        public bool rffSpedOffVendlBool
        {
            get
            {
                return rffSpedOffVend == "S";
            }
            set
            {
                if (value)
                    rffSpedOffVend = "S";
                else
                    rffSpedOffVend = "N";
            }
        }

        public string? rffSpedInfo { get; set; }
        public bool rffSpedInfoBool
        {
            get
            {
                return rffSpedInfo == "S";
            }
            set
            {
                if (value)
                    rffSpedInfo = "S";
                else
                    rffSpedInfo = "N";
            }
        }


    }
}
