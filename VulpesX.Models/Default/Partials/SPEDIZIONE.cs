using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class SPEDIZIONE
    {
        public string FullDescriptionSearchable => $"{specod} {spedes?.Trim()}";

        public string? spetipDescription => TransportTypes.Where(w => w.ID == spetip).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIntIDDescription> TransportTypes => new ObservableCollection<GenericIntIDDescription>() {
            new GenericIntIDDescription(){ ID = 1, Description = "Trasporto marittimo" },
            new GenericIntIDDescription(){ ID = 2, Description = "Trasporto ferroviario" },
            new GenericIntIDDescription(){ ID = 3, Description = "Trasporto stradale" },
            new GenericIntIDDescription(){ ID = 4, Description = "Trasporto aereo" },
            new GenericIntIDDescription(){ ID = 5, Description = "Spedizioni postali" },
            new GenericIntIDDescription(){ ID = 6, Description = "Installazioni fisse di trasporto" },
            new GenericIntIDDescription(){ ID = 8, Description = "Trasporto per vie d'acqua" },
            new GenericIntIDDescription(){ ID = 9, Description = "Propulsione propria" }
        };
    }
}
