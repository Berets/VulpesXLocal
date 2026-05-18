using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class TAB_GEN_TEXTS
    {
        #region Types
        public static readonly string OFFERS = "OFF";
        public static readonly string ORDERS = "ORD";
        public static readonly string DDT = "DDT";
        public static readonly string INVOICES = "INV";
        public static readonly string REMINDERS = "SOL";
        public static readonly string FEASABILITY = "ANF";
        public static readonly string PURCHASE_ORDERS = "BUY";

        public static readonly ObservableCollection<GenericIDDescription> TextTypes = new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription("OFF","Offerte"),
            new GenericIDDescription("ORD","Ordini"),
            new GenericIDDescription("DDT","DDT"),
            new GenericIDDescription("INV","Fatture"),
            new GenericIDDescription("ANF","Analisi di fattibilita'"),
            new GenericIDDescription("SOL","Solleciti"),
            new GenericIDDescription("BUY","Ordini di acquisto")
        };
        #endregion

        public string FullDescriptionSearchable => $"{TTxcod} {TTxdes?.Trim()}";

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
