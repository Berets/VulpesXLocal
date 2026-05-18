using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class DIAMETRO
    {
        public string? FullDescription { get { return $"{Diamcod?.TrimEnd()} - {diamdes?.TrimEnd()}"; } }
    }
}
