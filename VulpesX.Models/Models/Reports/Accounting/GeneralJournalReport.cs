using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class GeneralJournalReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public int AccountingYear { get; set; }
        public int StartingPage { get; set; }
        public DateTime PrintLimit { get; set; }
        public decimal DareTotal { get; set; }
        public decimal AvereTotal { get; set; }
        public List<RowInfo>? Rows { get; set; }

        public class RowInfo
        {
            public int? RowNumber { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public int? RegistrationNumber { get; set; }
            public int? RegistrationRow { get; set; }
            public DateTime? DocumentDate { get; set; }
            public string? DocumentNumber { get; set; }
            public string? GroupID { get; set; }
            public string? AccountID { get; set; }
            public string? SubaccountID { get; set; }
            public string? PDCDescription { get; set; }
            public string? EntityDescription { get; set; }
            public string? RowDescription => !string.IsNullOrWhiteSpace(EntityDescription) ? EntityDescription.Trim() : PDCDescription?.Trim();
       
            private string? note;
            public string? Note
            {
                get => note;
                set
                {
                    note = !string.IsNullOrWhiteSpace(value) ? value : null;
                }
            }
            public string? CausalFullDescription { get; set; }
            public decimal? DareAmount => string.IsNullOrWhiteSpace(Sign) ? null : Sign == "D" ? Amount : 0;
            public decimal? AvereAmount => string.IsNullOrWhiteSpace(Sign) ? null : Sign == "A" ? Amount : 0;
            public decimal? Amount { get; set; }
            public string? Sign { get; set; }
            // day break
            public bool IsDayTotal { get; set; } = false;
            public string? DayBreakText { get; set; }
            public decimal? DareDayTotal { get; set; }
            public decimal? AvereDayTotal { get; set; }
            // page top
            public bool IsPageTop { get; set; } = false;
            public decimal? DareTop { get; set; }
            public decimal? AvereTop { get; set; }
            // page bottom
            public bool IsPageBottom { get; set; } = false;
            public decimal? DareBottom { get; set; }
            public decimal? AvereBottom { get; set; }
        }
    }
}