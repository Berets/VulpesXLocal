using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Production
{
    public class pro_ordine_composizioneDependency
    {
        public required pro_ordine_composizione FromTask { get; set; }
    }
}
