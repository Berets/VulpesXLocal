using System.Collections.ObjectModel;
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class ECReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public byte[]? LogoData { get; set; }
        public string PrintFilename => $"EC_{Entity?.FullDescriptionSearchable}_al_{ToDate.ToString("dd_MM_yyyy")}.pdf";
        public ABE? Entity { get; set; }
        public DateTime ToDate { get; set; }
        public string? ReportTitle { get; set; }
        public string? EntityDescription { get; set; }
        public decimal TotalDARE => Rows != null && Rows.Count > 0 ? Math.Round(Rows.Sum(sum => sum.Dare), 2, MidpointRounding.AwayFromZero) : 0;
        public decimal TotalAVERE => Rows != null && Rows.Count > 0 ? Math.Round(Rows.Sum(sum => sum.Avere), 2, MidpointRounding.AwayFromZero) : 0;
        public decimal SaldoDARE => TotalDARE > TotalAVERE ? Math.Round(TotalDARE - TotalAVERE, 2, MidpointRounding.AwayFromZero) : 0;
        public decimal SaldoAVERE => TotalAVERE > TotalDARE ? Math.Round(TotalAVERE - TotalDARE, 2, MidpointRounding.AwayFromZero) : 0;
        public string? IstantText { get; set; }
        public ObservableCollection<MastrinoECReportItem>? Rows { get; set; }
        public List<MastrinoECReportItem>? Entries => Rows?.GroupBy(g => g.Partita, (gKey, items) => new MastrinoECReportItem()
        {
            CompanyID = "X",
            EntityType = "X",
            PartitaPrint = gKey,
            Dare = items.Sum(sum => sum.Dare),
            Avere = items.Sum(sum => sum.Avere),
            Details = items.ToList()
        }).ToList();
    }
}
