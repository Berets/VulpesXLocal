using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default.Partials
{
    public class BankItem
    {
        public int ABI { get; set; }
        public int CAB { get; set; }
        public string? Description { get; set; }
        public string? Account { get; set; }
        public string? CIN { get; set; }
        public string? BIC { get; set; }
        public string? BBAN { get; set; }
        public string? IBAN { get; set; }
        public string FullDescriptionSearchable => $"{ABI} {CAB} {Description} {Account}";
    }
}
