using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Default
{
    public partial class acq_orders_heads_attachments
    {
        public string document_sizeText => FileHelper.FilesizeGetText(document_size ?? 0);
        public string? FullPath { get; set; }
        public bool IsNotModified { get; set; } = true;
        public bool DownloadsVisibility => IsNotModified ? true : false;
    }
}
