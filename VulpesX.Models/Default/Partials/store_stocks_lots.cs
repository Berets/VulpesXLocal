namespace VulpesX.Models.Default
{
    public partial class store_stocks_lots
    {
        public string? StoreDescription { get; set; }
        public string? ProductDescription { get; set; }
        public tab_articolo? Product { get; set; }
        public store_stores? Store { get; set; }
        public string FullDescriptionSearchable => $"{lot} Disponibile: {(AvailableQuantity != decimal.MaxValue ? AvailableQuantity.ToString("N6") : "∞")} {(!lot.StartsWith("#") ? expire.HasValue ? $"Scadenza {expire.Value.Date.ToShortDateString()}" : $"Nessuna scadenza" : null)}";
        public decimal? EngagedQuantity { get; set; }
        public decimal AvailableQuantity => (quantity_stock ?? 0) + (quantity_ordered.HasValue ? quantity_ordered.Value : 0) - (EngagedQuantity.HasValue ? EngagedQuantity.Value : 0);
    }
}
