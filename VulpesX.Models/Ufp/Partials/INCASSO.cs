using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class INCASSO
    {
        public ObservableCollection<GenericIDDescription> SupportTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Assegno" },
            new GenericIDDescription(){ ID = "B", Description = "Bonifico cartaceo" },
            new GenericIDDescription(){ ID = "C", Description = "Carta" },
            new GenericIDDescription(){ ID = "E", Description = "Bonifico elettronico" },
        };

        public string? icltipDescription => SupportTypes.Where(w => w.ID == icltip).FirstOrDefault()?.Description;
    }
}
