using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class tab_produzione_reparto
    {
        public ObservableCollection<tab_produzione_risorsa>? RepartoRisorse { get; set; }
    }
}
