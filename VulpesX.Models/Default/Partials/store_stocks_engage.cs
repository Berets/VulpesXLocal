namespace VulpesX.Models.Default
{
    public partial class store_stocks_engage
    {
        public string? FullCustomerOrderID => order_year.HasValue && order_number.HasValue && order_row.HasValue ? $"{order_year}/{order_number}/{order_row}" : null;
        public string? FullDDTID => ddt_year.HasValue && ddt_number.HasValue && ddt_row.HasValue ? $"{ddt_year}/{ddt_number}/{ddt_row}" : null;
        public string? StoreFullDescription { get; set; }
        public string? ProductFullDescription { get; set; }
        public string? UM { get; set; }
        public string? RowUM { get; set; }
        public decimal RowQuantity { get; set; }
        public string? GoodsLocation { get; set; }

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
