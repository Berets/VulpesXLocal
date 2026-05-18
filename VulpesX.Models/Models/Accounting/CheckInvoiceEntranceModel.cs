using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting
{
    public class CheckInvoiceEntranceModel
    {
        public class DettaglioModel
        {
            public required string SocietaID { get; set; }
            public required string FatturaID { get; set; }
            public DateTime Data { get; set; }
            public required string PartitaIVA { get; set; }
            public int Riga { get; set; }
            public DateTime DDTData { get; set; }
            public string? DDTID { get; set; }

            public string? ArticoloFull { get; set; }
            public decimal Prezzo { get; set; }
            public decimal Quantita { get; set; }
            public decimal Totale { get; set; }
            public EntrataModel? Entrata { get; set; }
        }

        public class EntrataModel
        {
            public required string SocietaID { get; set; }
            public int FornitoreID { get; set; }
            public required string FornitoreDescrizione { get; set; }
            public required string FornitoreIva { get; set; }
            public DateTime Data { get; set; }
            public string? DDTID { get; set; }
            public string? FatturaID { get; set; }
            public int Riga { get; set; }

            public decimal Prezzo { get; set; }
            public decimal Quantita { get; set; }

            public string? OrdineSocietaID { get; set; }
            public int OrdineAnno { get; set; }
            public int OrdineID { get; set; }
            public int OrdineRiga { get; set; }

            public string? ArticoloID { get; set; }
            public string? ArticoloDisegno { get; set; }
        }
    }
}
