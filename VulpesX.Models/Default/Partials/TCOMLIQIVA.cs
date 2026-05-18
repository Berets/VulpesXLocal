 

namespace VulpesX.Models.Default
{
    public partial class TCOMLIQIVA
    {
        public string? CLITipLiqDescription => CommonsService.LIPELiquidationTypes.Where(w => w.ID == CLITipLiq).First().Description;

        public TCODLIQIVA? ItemInfo { get; set; }

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
