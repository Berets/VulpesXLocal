namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_SM
    {
        public string? SMTypeDescription => CommonsService.FESMTypes.Where(w => w.ID == scttipo).FirstOrDefault()?.FullDescriptionNotSearchable;
    }
}
