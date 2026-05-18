using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default;

public partial class ORDITAL00F
{
    public string OTASIZEText => FileHelper.FilesizeGetText(OTASIZE ?? 0);
    public string? FullPath { get; set; }
    public bool IsNotModified { get; set; } = true;
    public bool DownloadsVisibility => IsNotModified ? true : false;
}
