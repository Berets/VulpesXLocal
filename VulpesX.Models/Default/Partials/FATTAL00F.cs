using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class FATTAL00F
    {
        public string FTASIZEText => FileHelper.FilesizeGetText(FTASIZE ?? 0);
        public string? FullPath { get; set; }
        public bool IsNotModified { get; set; } = true;
        public bool DownloadsVisibility => IsNotModified ? true : false;
    }
}
