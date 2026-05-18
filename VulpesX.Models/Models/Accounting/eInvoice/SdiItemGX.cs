using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting.eInvoice
{
    public class SdIItemGX
    {
        public string? SdIID { get; set; }
        public string? FileName { get; set; }
        public string? DataRicezione { get; set; }
        public FullDocument? FullDocumentDecoded { get; set; }
    }
}
