namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class FullDocumentItemRow
    {
        public long Linea { get; set; }
        public string? TipoCessionePrestazione { get; set; }
        public string? Descrizione { get; set; }
        public decimal? Quantita { get; set; }
        public decimal PrezzoUnitario { get; set; }
        public decimal PrezzoTotale { get; set; }
        public decimal AliquotaIVA { get; set; }
        public string? UnitaMisura { get; set; }
        public string? Natura { get; set; }
        public string? Ritenuta { get; set; }
        public string? RiferimentoAmministrazione { get; set; }
        public string? DataInizioPeriodo { get; set; }
        public string? DataFinePeriodo { get; set; }
        public List<FullDocumentItemDiscount>? ScontiMaggiorazioni { get; set; }
        public List<FullDocumentItemRowProduct>? CodiciArticolo { get; set; }
        public List<FullDocumentItemRowOther>? AltriDatiGestionali { get; set; }
        #region Semplificata
        public decimal? Importo { get; set; }
        public string? RiferimentoNormativo { get; set; }
        public decimal? DatiIVAImposta { get; set; }
        public decimal? DatiIVAAliquota { get; set; }
        #endregion
    }
}
