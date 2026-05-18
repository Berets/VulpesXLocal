 

namespace VulpesX.Models.Default
{
    public partial class ACC_EINVOICE_ROWS_SM
    {
        public string? SMTypeDescription => CommonsService.FESMTypes.Where(w => w.ID == sctipo).FirstOrDefault()?.FullDescriptionNotSearchable;
    }
}
