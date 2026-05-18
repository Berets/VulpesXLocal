using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class SRM_RDA
    {
        public string? FullCustomerOrderID => order_year.HasValue && order_number.HasValue && order_row.HasValue ? $"{order_year}/{order_number}/{order_row}" : null;

        private tab_articolo? product;
        public tab_articolo? Product
        {
            get => product;
            set
            {
                product = value;
                NotifyPropertyChanged("Product");
            }
        }
        public ObservableCollection<ABE>? Suppliers { get; set; }
        private ABE? supplier;
        public ABE? Supplier { get => supplier; set { supplier = value; NotifyPropertyChanged("Supplier"); } }
        public string? UM { get; set; }
        public bool CanDelete => !canceled.HasValue && !transformed.HasValue;
        public bool CanDeleteVisibility => CanDelete ? true : false;

        #region Approve properties
        public bool CanApprove => !canceled.HasValue && !approval_date.HasValue;
        public string ApproveToolTip
        {
            get
            {
                if (canceled.HasValue)
                    return "Questa RDA è stata annullata e non è più approvabile";

                if (approval_date.HasValue)
                {
                    return $"Questa RDA è già stata approvata il {approval_date.Value.ToString("dd/MM/yyyy HH:mm:ss")} da {approval_user}";
                }
                else
                {
                    return "Cliccare qui per approvare questa RDA";
                }
            }
        }
        public string ApproveColor
        {
            get
            {
                if (canceled.HasValue)
                    return "W";

                if (approval_date.HasValue)
                {
                    return "G";
                }
                else
                {
                    return "W";
                }
            }
        }
        //public Cursor ApproveCursor
        //{
        //    get
        //    {
        //        if (approval_date.HasValue || canceled.HasValue)
        //        {
        //            return null;
        //        }
        //        else
        //        { return Cursors.Hand; }
        //    }
        //}
        #endregion

        #region Info
        public string AddedText => added > DateTime.MinValue ? added.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion
    }
}
