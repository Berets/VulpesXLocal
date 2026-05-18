using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class PAGFOR
    {
        public TAB_ACC_TIPPAG? PaymentType { get; set; }
        public string FullDescriptionSearchable => $"{pfocod} {pfodes?.Trim()}";
        public string? pfoppaDescription => StartingTypes.Where(w => w.ID == pfoppa).FirstOrDefault()?.Description;
        public bool pfivacasBool
        {
            get
            {
                return pfivacas == "S";
            }
            set
            {
                if (value)
                    pfivacas = "S";
                else
                    pfivacas = "N";
            }
        }

        public ObservableCollection<GenericIDDescription> StartingTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "0", Description = "Data fattura" },
            new GenericIDDescription(){ ID = "1", Description = "Fine mese" }
        };

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
