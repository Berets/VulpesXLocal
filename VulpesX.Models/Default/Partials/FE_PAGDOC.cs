using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class FE_PAGDOC
    {
        public string FullDescriptionSearchable => $"{FEPACOD} {FEPADES?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{FEPACOD}] {FEPADES?.Trim()}";
        public string? FEPATVALDescription => ValidityTypes.Where(w => w.ID == FEPATVAL).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIDDescription> ValidityTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Valido" },
            new GenericIDDescription(){ ID = "A", Description = "Disabilitato" }
        };
    }
}
