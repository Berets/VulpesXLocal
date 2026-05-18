using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class RIVESTIMENTI
    {
        public string? FullDescription { get { return $"{rivecod?.TrimEnd()} - {rivedes?.TrimEnd()}"; } }
    }
}
