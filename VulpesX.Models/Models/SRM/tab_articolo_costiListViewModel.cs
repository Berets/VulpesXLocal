using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.SRM
{
    public class tab_articolo_costiListViewModel
    {
        public string? ID { get; set; }
        public string? Description { get; set; }
        public string? UM { get; set; }
        public decimal GloballyValue { get; set; }
        public decimal GloballyLoad { get; set; }
        public decimal? GloballyAverageCost => GloballyLoad > 0 ? GloballyValue / GloballyLoad : 0;
    }
}
