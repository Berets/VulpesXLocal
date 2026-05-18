using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class TIPTA00F
    {
        public string? FullDescription { get { return $"{tipcod?.TrimEnd()} - {tipdes?.TrimEnd()}"; } }
    }
}
