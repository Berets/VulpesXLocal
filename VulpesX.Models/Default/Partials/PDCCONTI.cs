using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class PDCCONTI
    {
        public string FullDescriptionSearchable => $"{P2CONT} {P2DES1?.Trim()}";
        public ObservableCollection<PDCSOTTO>? Subaccounts { get; set; }

        public PDCCONTI? Clones()
        {
            return MemberwiseClone() as PDCCONTI;
        }

        public string? p2ragmas { get; set; }
    }
}
