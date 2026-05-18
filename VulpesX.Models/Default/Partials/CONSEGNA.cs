using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class CONSEGNA
    {
        public string FullDescriptionSearchable => $"{concod} {condes?.Trim()}";

        public string? conintDescription => INTRATypes?.Where(w => w.ID == conint).FirstOrDefault()?.Description;

        public ObservableCollection<GenericIDDescription>? INTRATypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "C", Description = "Costi e noli" },
            new GenericIDDescription(){ ID = "D", Description = "Resi" },
            new GenericIDDescription(){ ID = "E", Description = "Franco fabbrica" },
            new GenericIDDescription(){ ID = "F", Description = "Franco trasportatore" }
        };
    }
}
