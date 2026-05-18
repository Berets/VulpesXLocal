using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class PDCSOTTO
    {
        public string? p3sociTrimmed
        {
            get => p3soci == null ? "" : p3soci?.Trim();
            set => p3soci = value;
        }

        public PDCGRUPPI? Group { get; set; }
        public PDCCONTI? Account { get; set; }
        public string FullDescriptionSearchable => $"{P3SOTC} {P3DES1?.Trim()}";
        public string FullText => $"{P3DES1?.Trim()} {P3DES2?.Trim()}";

        public ObservableCollection<PDCANNI>? Years { get; set; }

        public bool P3OBCPBool
        {
            get
            {
                return P3OBCP == "S";
            }
            set
            {
                if (value)
                    P3OBCP = "S";
                else
                    P3OBCP = "N";
            }
        }

        public bool p3coniBool
        {
            get
            {
                return p3coni == "S";
            }
            set
            {
                if (value)
                    p3coni = "S";
                else
                    p3coni = "N";
            }
        }

        public bool p3utgrBool
        {
            get
            {
                return p3utgr == "S";
            }
            set
            {
                if (value)
                    p3utgr = "S";
                else
                    p3utgr = string.Empty;
            }
        }

        public bool p3flSpesoBool
        {
            get
            {
                return p3flSpeso == "S";
            }
            set
            {
                if (value)
                    p3flSpeso = "S";
                else
                    p3flSpeso = "N";
            }
        }

        public PDCSOTTO? Clones()
        {
            return MemberwiseClone() as PDCSOTTO;
        }

        public string? p3gval { get; set; }
        public bool p3gvalBool
        {
            get
            {
                return p3gval == "S";
            }
            set
            {
                if (value)
                    p3gval = "S";
                else
                    p3gval = "N";
            }
        }

        public bool p3est2Bool
        {
            get
            {
                return p3est2 == 1;
            }
            set
            {
                if (value)
                    p3est2 = 1;
                else
                    p3est2 = 0;
            }
        }

        public string? p3ragmas { get; set; }

    }
}
