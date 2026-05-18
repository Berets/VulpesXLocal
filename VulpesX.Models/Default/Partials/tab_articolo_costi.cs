using System.ComponentModel;

namespace VulpesX.Models.Default
{
    public partial class tab_articolo_costi
    {
        public tab_articolo_costi()
        {
            this.PropertyChanged += Tab_articolo_costi_PropertyChanged;
        }

        private void Tab_articolo_costi_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "total_load" || e.PropertyName == "last_cost")
            {
                NotifyPropertyChanged("TotalValue");
                NotifyPropertyChanged("AverageCost");
            }
        }

        public tab_articolo? Product { get; set; }
        public decimal AverageCost => total_load > 0 ? total_value / total_load : last_cost;
        public decimal TotalValue => total_load * last_cost;

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion
    }
}
