using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class PNIVA
    {
        public decimal? Amount
        {
            get { return N4IMEU; }
            set
            {
                N4IMEU = value;
                N4IVEU = ComputeIVAAmount();
                N4IIEU = ComputeIVANonDeduct();
                NotifyPropertyChanged("N4IVEU");
                NotifyPropertyChanged("N4IIEU");
            }
        }
        public ABE? Customer { get; set; }
        public string? CausalFullDescription { get; set; }
        public string? EntityFullDescription { get; set; }
        public string? IVABookType { get; set; }
        public string? IVABookFullDescription { get; set; }
        public bool IsSplitPayment { get; set; }
        public string RateText => $"{N4ASSF} {n4assa}";
        public int RowNumber { get; set; }
        public string? VATID { get; set; }
        public int CustomerID { get; set; }
        public string? CompanyDescription { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyCity { get; set; }
        public bool IsSubTotal { get; set; } = false;
        public DateTime? ExpireDate { get; set; }
        public int CustomerCount { get; set; }
        public string CustomerCountText => $"{CustomerCount} documenti diversi | Totale IVA indetraibile: {(N4IIEU.HasValue ? N4IIEU.Value.ToString("N2") ?? 0.ToString("N2") : "0,00")}";
        public decimal ReportAmount { get; set; }
        public decimal ReportVATAmount { get; set; }
        public decimal? DocumentTotal { get; set; }
        public bool CanEdit => N4FLGS != "*" && !N4DAST.HasValue;
        public string RateFullID => $"{N4ASSF} {n4assa}";
        #region IVA book
        private ObservableCollection<LIBRIIVA>? iVABooksList;
        public ObservableCollection<LIBRIIVA>? IVABooksList
        {
            get
            {
                return iVABooksList;
            }

            set
            {
                iVABooksList = value;
                SelectedIVABook = iVABooksList?.Where(w => w.livcod == N4LIBR).FirstOrDefault();
                NotifyPropertyChanged("SelectedIVABook");
                NotifyPropertyChanged("IVABookDescription");
            }
        }

        public string? IVABookDescription { get; set; }
        private LIBRIIVA? selectedIVABook;
        public LIBRIIVA? SelectedIVABook
        {
            get => selectedIVABook;
            set
            {
                selectedIVABook = value;
                IVABookDescription = selectedIVABook?.FullDescriptionSearchable;
                NotifyPropertyChanged("IVABookDescription");
            }
        }
        #endregion

        #region Rates
        private ObservableCollection<ASSOGGETAMENTI>? ratesList;
        public ObservableCollection<ASSOGGETAMENTI>? RatesList
        {
            get { return ratesList; }
            set
            {
                ratesList = value;
                if (!string.IsNullOrWhiteSpace(n4assa) && !string.IsNullOrWhiteSpace(N4ASSF))
                    SelectedRate = ratesList?.Where(w => w.assali == n4assa && w.asscod == N4ASSF).FirstOrDefault();
                else
                    SelectedRate = null;
                NotifyPropertyChanged("RatesList");
            }
        }

        public string? RateDescription { get; set; }
        public string? RateFullDescription { get; set; }
        private ASSOGGETAMENTI? selectedRate;

        public ASSOGGETAMENTI? SelectedRate
        {
            get => selectedRate;
            set
            {
                if (selectedRate?.asscod != value?.asscod || selectedRate?.assali != value?.assali)
                {
                    selectedRate = value;
                    RateDescription = selectedRate?.FullDescriptionSearchable;
                    N4INDP = selectedRate?.asspin ?? 0;
                    NotifyPropertyChanged("N4INDP");
                    NotifyPropertyChanged("SelectedRate");
                    NotifyPropertyChanged("RateDescription");
                }
            }
        }

        #endregion

        #region Utilities
        public decimal ComputeIVAAmount()
        {
            if (selectedRate == null || !N4IMEU.HasValue)
                return 0;
            decimal assali = 0;
            decimal.TryParse(selectedRate?.assali, out assali);
            return Math.Round(N4IMEU.Value / 100 * assali, 2);
        }
        public decimal ComputeIVANonDeduct()
        {
            if (!N4IVEU.HasValue || !N4IMEU.HasValue)
                return 0;
            return Math.Round(N4IVEU.Value * (N4INDP ?? 0) / 100, 2);
        }
        #endregion
    }
}
