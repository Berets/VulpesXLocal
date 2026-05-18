using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Ufp
{
    public partial class ANAFAT_CONST : Base
    {
        public bool IsActive
        {
            get { return !canceled.HasValue; }
            set
            {
                if (value)
                    canceled = null;
                else
                    canceled = DateTime.Now;
            }
        }

        private ObservableCollection<ANAFAT_PIECES> pieces = new()!;
        public ObservableCollection<ANAFAT_PIECES> Pieces
        {
            get { return pieces; }
            set { pieces = value; NotifyPropertyChanged(); }
        }

        public int RowsCount { get; set; }
        public bool IsReadOnly { get { return RowsCount > 0; } }

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updateUserID) ? updateUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion

        public string afproductionpreTooltip => $"Maggiorazione costo tempi di produzione {afproductionpre?.ToString("N2")}%";
        public string afproductionautTooltip => $"Maggiorazione costo tempi di produzione {afproductionaut?.ToString("N2")}%";

        public string afcomplexitystaTooltip => $"Maggiorazione costo per difficoltà Standard {afcomplexitysta?.ToString("N2")}%";
        public string afcomplexitymedTooltip => $"Maggiorazione costo per difficoltà Media {afcomplexitymed?.ToString("N2")}%";
        public string afcomplexitycomTooltip => $"Maggiorazione costo per difficoltà Complessa {afcomplexitycom?.ToString("N2")}%";

        public string afcustomericoTooltip => $"Maggiorazione costo cliente ICO {afcliico?.ToString("N2")}%";
        public string afcustomerdirectTooltip => $"Maggiorazione costo cliente Diretto {afclidir?.ToString("N2")}%";
    }
}
