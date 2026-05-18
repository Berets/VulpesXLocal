 

namespace VulpesX.Models.Default
{
    public partial class TCODLIQIVA
    {
        public string FullDescriptionSearchable => $"{CVICod} {CVIDes}";

        public string? CVITipoDescription => CommonsService.LIPEComputeTypes.Where(w => w.ID == CVITipo).First().Description;

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion

        public string? CVIAttiva { get; set; }

        public bool CVIAttivaBool
        {
            get
            {
                return CVIAttiva == "A";
            }
            set
            {
                if (value)
                    CVIAttiva = "A";
                else
                    CVIAttiva = "D";
            }
        }

        public string? CVITot1 { get; set; }
        public string? CVITot2 { get; set; }
    }
}
