namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_RIT
    {
        public FE_TIPORIT? Ritenuta { get; set; }
        public ACC_EINVOICE_HEADS? Head { get; set; }

        public decimal? EnasarcoCaricoAzienda { get; set; }
    }
}
