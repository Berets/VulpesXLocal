using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.CRM
{
    public class InvoicingTrend : Base
    {
        private ObservableCollection<InvoicingTrendMonth>? months;
        public ObservableCollection<InvoicingTrendMonth>? Months
        {
            get => months;
            set
            {
                months = value;
                NotifyPropertyChanged("Months");
            }
        }

        public decimal Average(int Year)
        {
            try
            {
                return Math.Round((Months ?? new ObservableCollection<InvoicingTrendMonth>()).Where(w => w.Year == Year).Sum(sum => sum.Amount) / 12, 2);
            }
            catch
            { return -1; }
        }

        public decimal Median(int year)
        {
            try
            {
                var ordered = (Months ?? new ObservableCollection<InvoicingTrendMonth>()).OrderBy(o => o.Amount).ToList();
                return Math.Round((ordered[5].Amount + ordered[6].Amount) / 2, 2);
            }
            catch
            { return -1; }
        }
    }

    public class InvoicingTrendMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string CategoryLabel => $"{Month.ToString().PadLeft(2, '0')}/{Year}";
        public decimal PreviousMonthAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance => Amount - CreditAmount;
        public decimal Weight { get; set; }
        public decimal TrendPercentage
        {
            get
            {
                if (PreviousMonthAmount == 0)
                    return 0;

                return Math.Round(((Amount - PreviousMonthAmount) / PreviousMonthAmount) * 100, 2);

            }
        }
    }
}
