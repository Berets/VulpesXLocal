using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class ACC_ASSETS_TYPES
    {
        public string FullDescriptionSearchable => $"{JTICE} {TCDES}";

        public string? ComputeTypeDescription => CommonsService.AccountingAssetsComputeTypes.Where(w => w.ID == TCCALC).FirstOrDefault()?.Description;
        public ObservableCollection<GenericIDDescription>? ComputeTypes { get; set; }

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
