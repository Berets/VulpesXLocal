using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class FORILUBRIFICATI
    {
        public string? FullDescription { get { return $"{FLcod?.TrimEnd()} - {FLdes?.TrimEnd()}"; } }
    }
}
