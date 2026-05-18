using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting
{
    public class ImportPNModel
    {
        public required string SocietaID { get; set; }
        public short Anno { get; set; }
        public int ID { get; set; }
        public string? CausaleID { get; set; }
        public DateTime? Data { get; set; }
        public string? DocumentoID { get; set; }
        public DateTime? DocumentoData { get; set; }
        public string? RiferimentoID { get; set; }
        public DateTime? RiferimentoData { get; set; }
        public string? GruppoID { get; set; }
        public string? ContoID { get; set; }
        public string? SottocontoID { get; set; }
        public string? Note { get; set; }
        public string? Segno { get; set; }
        public decimal? Importo { get; set; }
        public string? Valuta { get; set; }
    }
}
