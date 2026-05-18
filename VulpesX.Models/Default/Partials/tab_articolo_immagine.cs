using System.IO;

namespace VulpesX.Models.Default
{
    public partial class tab_articolo_immagine
    {
        public string Estensione
        {
            get
            {
                return !string.IsNullOrEmpty(Url) ? new FileInfo(Url).Extension : string.Empty;
            }
        }
        public string File
        {
            get
            {
                return !string.IsNullOrEmpty(Url) ? new FileInfo(Url).Name : string.Empty;
            }
        }
        public string? Cartella
        {
            get
            {
                return !string.IsNullOrEmpty(Url) ? new FileInfo(Url).Directory?.FullName : string.Empty;
            }
        }
    }
}
