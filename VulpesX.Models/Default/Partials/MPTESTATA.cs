using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class MPTESTATA
    {
        public bool MPFLSTBool
        {
            get
            {
                return MPFLST == "S";
            }
            set
            {
                if (value)
                    MPFLST = "S";
                else
                    MPFLST = "N";
            }
        }

        public bool MPFLCOBool
        {
            get
            {
                return MPFLCO == "S";
            }
            set
            {
                if (value)
                    MPFLCO = "S";
                else
                    MPFLCO = "N";
            }
        }

        public bool MPFLESBool
        {
            get
            {
                return MPFLES == "S";
            }
            set
            {
                if (value)
                    MPFLES = "S";
                else
                    MPFLES = "N";
            }
        }

        public bool MPFIRMABool
        {
            get
            {
                return !string.IsNullOrEmpty(mpfirma?.TrimEnd());
            }
        }

        public string? BankDescription { get; set; }

        public string? TypeDescription { get; set; }

        public bool SignVisibility { get { return !MPFIRMABool; } }

        public bool FileVisibility { get { return MPFIRMABool; } }

        public bool IsEnabled { get { return !MPFIRMABool; } }

        public bool IsReadOnly { get { return MPFIRMABool; } }
    }
}
