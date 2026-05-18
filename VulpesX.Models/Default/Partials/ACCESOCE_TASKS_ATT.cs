using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class ACCESOCE_TASKS_ATT
    {
        public string fsizeText => FileHelper.FilesizeGetText(fsize ?? 0);
        public string? FullPath { get; set; }
    }
}
