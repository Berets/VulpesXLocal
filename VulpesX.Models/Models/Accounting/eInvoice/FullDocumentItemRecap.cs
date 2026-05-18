namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class FullDocumentItemRecap
    {
        public decimal Aliquota { get; set; }
        public decimal? Arrotondamento { get; set; }
        public decimal Imponibile { get; set; }
        public decimal Imposta { get; set; }
        public string? Natura { get; set; }
        public string? Esigibilita { get; set; }
        public string? RiferimentoNormativo { get; set; }
    }
}
