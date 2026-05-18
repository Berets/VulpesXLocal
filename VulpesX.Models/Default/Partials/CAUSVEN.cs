using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class CAUSVEN
    {
        public string FullDescriptionSearchable => $"{cauven} {caudev?.Trim()}";
    }
}
