using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class ASSETAL00F
    {

        public string SizeText => FileHelper.FilesizeGetText(document_size ?? 0);
        public string? FullPath { get; set; }
        public bool IsNotModified { get; set; } = true;
        public bool DownloadsVisibility => IsNotModified ? true : false;
    }
}
