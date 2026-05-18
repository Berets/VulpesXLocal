namespace VulpesX.Models.Default
{
    public partial class FE_IVADOC
    {
        public string FullDescriptionSearchable => $"{FETICod} {FETIDes?.Trim()}";
        public string FullDescriptionNotSearchable => $"[{FETICod}] {FETIDes?.Trim()}";

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
