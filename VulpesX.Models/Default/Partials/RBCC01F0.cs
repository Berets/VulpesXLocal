namespace VulpesX.Models.Default
{
    public partial class RBCC01F0
    {
        public BANAZIEN? Bank { get; set; }
        public PDCSOTTO? Subaccount { get; set; }
        public decimal Castelletto { get; set; }
        public decimal ImportoProvvisorio { get; set; }
        public decimal ImportoScadenza { get; set; }
        public decimal ImportoProvvisorioSuccessivo { get; set; }
        public decimal ImportoScadenzaSuccessiva { get; set; }
        public decimal Fondi { get; set; }
        public decimal Differenza => (Bank != null) ? (Bank.abifidcas ?? 0m) + (cnsl17 ?? 0m) : 0m;
        public decimal DisponibilitaFutura => Differenza + ImportoProvvisorio + ImportoScadenza;
        public decimal DisponibilitaPortafoglio => (Bank != null) ? (Bank.abivalport ?? 0) - Castelletto : 0;
        public decimal SaldoAlGiorno => Differenza + ImportoProvvisorio;
        public decimal DisponibilitaMesiSuccessivi => DisponibilitaFutura + ImportoScadenzaSuccessiva;

        #region Styles
        public string ImportoProvvisorioBrush => ImportoProvvisorio >= 0 ? "X" : "O";
        public string ImportoScadenzaBrush => ImportoScadenza >= 0 ? "X" : "O";
        public string ImportoProvvisorioSuccessivoBrush => ImportoProvvisorioSuccessivo >= 0 ? "X" : "O";
        public string ImportoScadenzaSuccessivaBrush => ImportoScadenzaSuccessiva >= 0 ? "X" : "O";
        public string DisponibilitaMesiSuccessiviBrush => DisponibilitaMesiSuccessivi >= 0 ? "X" : "O";
        #endregion
    }
}
