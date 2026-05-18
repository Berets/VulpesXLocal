namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class FullDocumentItemPayment
    {
        public string? Condizioni { get; set; }
        public List<FullDocumentItemPaymentDetail>? DettagliPagamento { get; set; }
    }
    public class FullDocumentItemPaymentDetail
    {
        public string? Beneficiario { get; set; }
        public string? Modalita { get; set; }
        public string? DataRiferimentoTerminiPagamento { get; set; }
        public int? GiorniTerminiPagamento { get; set; }
        public string? DataScadenza { get; set; }
        public decimal? Importo { get; set; }
        public string? CodiceUfficioPostale { get; set; }
        public string? CognomeQuietanzante { get; set; }
        public string? NomeQuietanzante { get; set; }
        public string? CFQuietanzante { get; set; }
        public string? TitoloQuietanzante { get; set; }
        public string? IstitutoFinanziario { get; set; }
        public string? IBAN { get; set; }
        public string? ABI { get; set; }
        public string? CAB { get; set; }
        public string? BIC { get; set; }
        public decimal? ScontoPagamentoAnticipato { get; set; }
        public string? DataLimitePagamentoAnticipato { get; set; }
        public decimal? PenalitaPagamentiRitardati { get; set; }
        public string? DataDecorrenzaPenale { get; set; }
        public string? CodicePagamento { get; set; }
    }
}
