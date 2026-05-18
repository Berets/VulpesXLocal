using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class tab_produzione_causale
    {
        public ObservableCollection<tab_produzione_risorsa>? CausaleRisorse { get; set; }
    }
}
