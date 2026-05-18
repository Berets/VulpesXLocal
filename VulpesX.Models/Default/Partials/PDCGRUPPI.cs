using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class PDCGRUPPI
    {
        public string FullDescriptionSearchable => $"{P1GRUP} {P1DES1?.Trim()}";

        public ObservableCollection<PDCCONTI>? Accounts { get; set; }

        public bool P1OBCPBool
        {
            get
            {
                return P1OBCP == "S";
            }
            set
            {
                if (value)
                    P1OBCP = "S";
                else
                    P1OBCP = "N";
            }
        }

        public PDCGRUPPI? Clones()
        {
            return MemberwiseClone() as PDCGRUPPI;
        }
    }
}
