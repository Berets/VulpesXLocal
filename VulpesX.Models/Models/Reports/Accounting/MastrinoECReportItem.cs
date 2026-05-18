using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class MastrinoECReportItem
    {
        public required string CompanyID { get; set; }
        public int Year { get; set; }
        public int Number { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int RowID { get; set; }
        public string? DocumentID { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string? ReferenceID { get; set; }
        public DateTime? ReferenceDate { get; set; }
        public string Partita => $"{ReferenceID?.Trim().PadLeft(10, ' ')} {(ReferenceDate.HasValue ? ReferenceDate.Value.ToString("d") : "---")}";
        public string? PartitaPrint { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string? CausalFullDescription { get; set; }
        public string? CurrencyID { get; set; }
        public decimal Dare { get; set; }
        public decimal Avere { get; set; }
        public required string EntityType { get; set; }
        public int EntityID { get; set; }
        public ObservableCollection<SOLLE0F>? Reminders { get; set; }
        public string? PaymentID { get; set; }
        public string? PaymentTypeID { get; set; }
        public List<MastrinoECReportItem>? Details { get; set; }
        public decimal SaldoDARE => Dare > Avere ? Math.Round(Dare - Avere, 2, MidpointRounding.AwayFromZero) : 0;
        public decimal SaldoAVERE => Avere > Dare ? Math.Round(Avere - Dare, 2, MidpointRounding.AwayFromZero) : 0;
        public string? LockedInfoText { get; set; }
        public DateTime? Scadenza2 { get; set; }

        public decimal Saldo
        {
            get
            {
                return EntityType == "F" ? (Avere - Dare) : (Dare - Avere);
            }
        }
        public decimal Valore { get; set; }
        public decimal SaldoRiga { get; set; }
        public string Segno
        {
            get
            {
                if (Dare > Avere)
                {
                    return "D";
                }
                else
                {
                    if (Avere > Dare)
                    {
                        return "A";
                    }
                    else
                    {
                        return "-";
                    }
                }
            }
        }

        public string? TypeID { get; set; }
        public string? Note { get; set; }

        public decimal? Cambio { get; set; }
        public decimal? ValoreValuta { get; set; }
        public string? Valuta { get; set; }

        public string? ValutaText { get { return (ValoreValuta ?? 0) > 0 ? $"{Valuta}: {Cambio?.ToString("N4")} -> {ValoreValuta?.ToString("N2")}" : string.Empty; } }

        public string? ReportText { get { return LockedInfoText?.TrimEnd() + " - " + ValutaText?.TrimEnd(); } }

        public string? InvoiceFile { get; set; }

        public PNFORNITORI? PNFORNITORI { get; set; }
        public PNCLIENTI? PNCLIENTI { get; set; }
    }
}
