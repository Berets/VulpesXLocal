namespace VulpesX.Models.Default
{
    public partial class ACC_PLAFOND
    {
        public string FullID => $"{Cliacod}/{cliannosol}/{cliprog}";
        public ABE? Customer { get; set; }
        public decimal AmountAvailable => (cliimpesefino ?? 0) - (cliimpfattprog ?? 0) - (cliimpfattprovv ?? 0);

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
