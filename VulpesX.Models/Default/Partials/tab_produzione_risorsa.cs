namespace VulpesX.Models.Default
{
    public partial class tab_produzione_risorsa
    {
        public bool EGanttAperto { get; set; }
        public pro_ordine_composizione? UltimaComposizione { get; set; }
        public string FullDescriptionSearchable => $"{ID} {Descrizione}";
    }
}
