using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models
{
    public class LanguageModel
    {
        public required string Key { get; set; }
        public string? Value { get; set; }
        public string? ValueTranslated { get; set; }
    }
}
