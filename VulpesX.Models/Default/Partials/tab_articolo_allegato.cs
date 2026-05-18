using System.IO;

namespace VulpesX.Models.Default
{
    public partial class tab_articolo_allegato
    {
        public string Estensione
        {
            get
            {
                return !string.IsNullOrEmpty(Uri) ? new FileInfo(Uri).Extension : string.Empty;
            }
        }
        public string File
        {
            get
            {
                return !string.IsNullOrEmpty(Uri) ? new FileInfo(Uri).Name : string.Empty;
            }
        }
        public string? Cartella
        {
            get
            {
                return !string.IsNullOrEmpty(Uri) ? new FileInfo(Uri).Directory?.FullName : string.Empty;
            }
        }
    }
}
