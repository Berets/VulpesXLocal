namespace VulpesX.Models.Default
{
    public partial class store_movements
    {
        public string? dateText => (date.HasValue) ?  $"{date.Value.ToString("dd/MM/yyyy HH:mm:ss")}" : null;
        public string? expireText => expire.HasValue ? expire.Value.ToString("dd/MM/yyyy") : null;
        public string? document_dateText => document_date.HasValue ? document_date.Value.ToString("dd/MM/yyyy") : null;
        public string? addedText => added.HasValue ? added.Value.ToString("dd/MM/yyyy HH:mm:ss") : null;
        public store_causals? Causal { get; set; }
        public ABE? Supplier { get; set; }
        private tab_articolo? product;
        public tab_articolo? Product { get => product; set { product = value; NotifyPropertyChanged("Product"); NotifyPropertyChanged("UM"); } }
        public string? CausalDescription { get; set; }

        public string? StoreFullDescription { get; set; }
        public string? StoreDescription { get; set; }
        public string? ProductDescription { get; set; }
        public string? SupplierDescription { get; set; }
        public string? UM { get; set; }
        public string? Sign { get; set; }

        public store_movements_history? OldVersion { get; set; }

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(add_user) ? add_user : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(update_user) ? update_user : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(cancel_user) ? cancel_user : "---";
        #endregion
    }
}
