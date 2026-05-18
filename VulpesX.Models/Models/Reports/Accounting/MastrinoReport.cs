using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class MastrinoReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public string PrintFilename => $"MAS_{PDCDescription}_al_{ToDate.ToString("dd_MM_yyyy")}.pdf";
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? ReportTitle { get; set; }
        public string? PDCDescription { get; set; }
        public string? IstantText { get; set; }
        public List<MastrinoReportItem>? Rows { get; set; }

        public class MastrinoReportItem
        {
            public string? GroupDescription { get; set; }
            public string? AccountDescription { get; set; }
            public string? SubaccountDescription { get; set; }
            public bool HasDetails => Details != null && Details.Count > 0;
            public List<MastrinoReportItemDetails>? Details { get; set; }
            public string? MastrinoTotalText { get; set; }
            public decimal? Debit { get; set; }
            public decimal? Credit { get; set; }
            public decimal? Progressive { get; set; }
            public string ProgressiveText => $"{(Progressive.HasValue ? Progressive > 0 ? $"{Progressive.Value.ToString("N2")} D" : Progressive < 0 ? $"{(Progressive * -1).Value.ToString("N2")} A" : $" 0,00 -" : null)}";

            public class MastrinoReportItemDetails
            {
                public string? IsDefinitive { get; set; }
                public int? JournalRow { get; set; }
                public string? JournalDateText { get; set; }
                public int? RegistrationNumber { get; set; }
                public string? RegistrationDateText { get; set; }
                public string? EntityDescription { get; set; }
                public string? DocumentID { get; set; }
                public string? ReferenceID { get; set; }
                public string? Description { get; set; }
                public decimal? Debit { get; set; }
                public decimal? Credit { get; set; }
                public decimal? Progressive { get; set; }
                public decimal? ProgressiveMonth { get; set; }
                public string? MonthText { get; set; }
                public decimal? DebitMonth { get; set; }
                public decimal? CreditMonth { get; set; }
                public decimal? DebitTotal { get; set; }
                public decimal? CreditTotal { get; set; }
                public decimal? ProgressiveTotal { get; set; }

                public string ProgressiveText => $"{(Progressive.HasValue ? Progressive > 0 ? $"{Progressive.Value.ToString("N2")} D" : Progressive < 0 ? $"{(Progressive * -1).Value.ToString("N2")} A" : $" 0,00 -" : null)}";
                public string ProgressiveMonthText => $"{(ProgressiveMonth.HasValue ? ProgressiveMonth > 0 ? $"{ProgressiveMonth.Value.ToString("N2")} D" : ProgressiveMonth < 0 ? $"{(ProgressiveMonth * -1).Value.ToString("N2")} A" : $" 0,00 -" : null)}";
            }
        }
    }




}
