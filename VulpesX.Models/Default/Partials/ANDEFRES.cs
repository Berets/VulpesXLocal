using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class ANDEFRES
    {
        public string? SpedOffVend { get; set; }
        public bool SpedOffVendBool
        {
            get
            {
                return SpedOffVend == "S";
            }
            set
            {
                if (value)
                    SpedOffVend = "S";
                else
                    SpedOffVend = "N";
            }
        }

        public string? SpedOrdAcq { get; set; }
        public bool SpedOrdAcqBool
        {
            get
            {
                return SpedOrdAcq == "S";
            }
            set
            {
                if (value)
                    SpedOrdAcq = "S";
                else
                    SpedOrdAcq = "N";
            }
        }

        public string? SpedOffAcq { get; set; }
        public bool SpedOffAcqBool
        {
            get
            {
                return SpedOffAcq == "S";
            }
            set
            {
                if (value)
                    SpedOffAcq = "S";
                else
                    SpedOffAcq = "N";
            }
        }

        public string? SpedInfo { get; set; }
        public bool SpedInfoBool
        {
            get
            {
                return SpedInfo == "S";
            }
            set
            {
                if (value)
                    SpedInfo = "S";
                else
                    SpedInfo = "N";
            }
        }

        public string? rfcspe { get; set; }
        public bool rfcspeBool
        {
            get
            {
                return rfcspe == "S";
            }
            set
            {
                if (value)
                    rfcspe = "S";
                else
                    rfcspe = "N";
            }
        }

        public string rfspf { get; set; } = "N";
        public bool rfspfBool
        {
            get
            {
                return rfspf == "S";
            }
            set
            {
                if (value)
                    rfspf = "S";
                else
                    rfspf = "N";
            }
        }
    }
}
