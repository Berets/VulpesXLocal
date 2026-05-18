using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class TIPMATPRI
    {
        public string? FullDescription { get { return $"{tmpcod?.TrimEnd()} - {tmpdes?.TrimEnd()}"; } }
    }
}
