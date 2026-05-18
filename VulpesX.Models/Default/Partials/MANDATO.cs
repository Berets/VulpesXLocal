using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class MANDATO
    {
        public ObservableCollection<GenericIDDescription> SupportTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Assegno" },
            new GenericIDDescription(){ ID = "B", Description = "Bonifico cartaceo" },
            new GenericIDDescription(){ ID = "C", Description = "Carta" },
            new GenericIDDescription(){ ID = "E", Description = "Bonifico elettronico" },
            new GenericIDDescription(){ ID = "R", Description = "Ri.Ba" },
        };

        public string? mantipDescription => SupportTypes.Where(w => w.ID == mantip).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIDDescription> TrackTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "AP", Description = "Ritiro effetti manuale" },
            new GenericIDDescription(){ ID = "PC", Description = "Bonifico Italia" },
            new GenericIDDescription(){ ID = "PE", Description = "Bonifico Estero" },
            new GenericIDDescription(){ ID = "AE", Description = "Ritiro effetti CBI" },
        };

        public string? manpagDescription => TrackTypes.Where(w => w.ID == manpag).FirstOrDefault()?.Description;

        public string FullDescriptionSearchable => $"{mancod} {mandes?.TrimEnd()}";


    }
}
