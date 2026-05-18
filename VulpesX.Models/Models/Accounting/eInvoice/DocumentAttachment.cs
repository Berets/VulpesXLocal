using VulpesX.Shared.Utilities;

namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class DocumentAttachment
    {
        public string? Name { get; set; }
        public string? Compression { get; set; }
        public string? Format { get; set; }
        public string? Description { get; set; }
        public byte[]? Data { get; set; }
        public long Size { get; set; }
        public string SizeText => FileHelper.FilesizeGetText(Size);
    }
}
