using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class OFFETAL00F
    {
        public string OFTASIZEText => FileHelper.FilesizeGetText(OFTASIZE ?? 0);
        public string? FullPath { get; set; }
        public bool IsNotModified { get; set; } = true;
        public bool DownloadsVisibility => IsNotModified ? true : false;
    }
}
