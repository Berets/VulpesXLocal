namespace VulpesX.Models.Default
{
    public partial class TAB_CRM_CAUOFF
    {
        public string? CompanyID { get; set; }
        public string FullDescriptionSearchable => $"{offcod} {offede?.Trim()}";
        public TAB_CRM_CAUORD? OrderCausal { get; set; }
        public TAB_GEN_TEXTS? Text { get; set; }

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
