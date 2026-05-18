using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class tab_produzione_operatore
    {
        public string FullDescriptionSearchable => $"{ID.Trim()} {Descrizione.Trim()}";
        public pro_ordine_composizione? UltimaComposizione { get; set; }
        public ObservableCollection<tab_produzione_risorsa>? OperatoreRisorse { get; set; }
    }
}
