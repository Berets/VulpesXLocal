using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class PNPORTAFOGLIO_DIST
    {
        public string? BankDescription { get; set; }

        private ObservableCollection<PNPORTAFOGLIO>? items;
        public ObservableCollection<PNPORTAFOGLIO>? Items
        {
            get => items;
            set
            {
                items = value;
                NotifyPropertyChanged("Items");
                NotifyPropertyChanged("TotalAmount");
            }
        }
        public decimal TotalAmount => Items?.Sum(sum => sum.N6IMEU) ?? 0;

        #region Icon management
        // delete
        public bool DeleteIconVisibility => accounting_date.HasValue ? false : true;
        //public Cursor DeleteCursor => accounting_date.HasValue ? Cursors.Arrow : Cursors.Hand;
        public string DeleteToolTip => accounting_date.HasValue ? "Impossibile eliminare una distinta contabilizzata" : "Elimina la distinta, tutte le disposizioni contenute verranno ripristinate";
        // accounting
        public bool AccountingIconVisibility => accounting_date.HasValue ? false : true;
        //public Cursor AccountingCursor => accounting_date.HasValue ? Cursors.Arrow : Cursors.Hand;
        public string AccountingToolTip => accounting_date.HasValue ? "Impossibile contabilizzare una distinta già contabilizzata" : "Contabilizza definitivamente la distinta";
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        #endregion

        public int Year { get; set; }
    }
}
