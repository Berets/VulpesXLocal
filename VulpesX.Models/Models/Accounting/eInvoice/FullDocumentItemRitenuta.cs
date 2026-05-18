namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class FullDocumentItemRitenuta
    {
        public string? Tipo { get; set; }
        public decimal Importo { get; set; }
        public decimal Aliquota { get; set; }
        public string? CausalePagamento { get; set; }
    }
}
