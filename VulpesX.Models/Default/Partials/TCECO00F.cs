 

namespace VulpesX.Models.Default
{
    public partial class TCECO00F
    {
        public string FullDescriptionSearchable => $"{cecodc} {cedesc?.Trim()}";
        public string? TypeDescription => CommonsService.CostCentersTypes.Where(w => w.ID == cetipo).FirstOrDefault()?.Description;
    }
}
