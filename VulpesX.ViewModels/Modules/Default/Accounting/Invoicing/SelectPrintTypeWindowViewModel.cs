using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public class SelectPrintTypeWindowViewModel  : Base
    {
        public required string SendFormat { get; set; }
        public required string Filename { get; set; }
        public required byte[] Data { get; set; }
    }
}
