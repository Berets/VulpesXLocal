using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models
{
    public class PaneModel
    {
        public required string Header { get; set; }
        public required object Content { get; set; }
        public required string Tag { get; set; }
        public bool IsSelected { get; set; }
        public bool IsVisible { get; set; }
    }
}
