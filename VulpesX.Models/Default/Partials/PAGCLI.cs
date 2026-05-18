using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class PAGCLI
    {
        public TAB_ACC_TIPINC? Incasso { get; set; }

        public string FullDescriptionSearchable => $"{pclcod} {pcldes?.Trim()}";
        public string? pclppaDescription => StartingTypes.Where(w => w.ID == pclppa).FirstOrDefault()?.Description;
        public bool pcivacasBool
        {
            get
            {
                return pcivacas == "S";
            }
            set
            {
                if (value)
                    pcivacas = "S";
                else
                    pcivacas = "N";
            }
        }

        public int MaxPayement
        {
            get
            {
                int retValue = 0;

                if ((pclgs1 ?? 0) >= retValue)
                    retValue = pclgs1 ?? 0;
                if ((pclgs2 ?? 0) >= retValue)
                    retValue = pclgs2 ?? 0;
                if ((pclgs3 ?? 0) >= retValue)
                    retValue = pclgs3 ?? 0;
                if ((pclgs4 ?? 0) >= retValue)
                    retValue = pclgs4 ?? 0;
                if ((pclgs5 ?? 0) >= retValue)
                    retValue = pclgs5 ?? 0;
                if ((pclgs6 ?? 0) >= retValue)
                    retValue = pclgs6 ?? 0;
                if ((pclgs7 ?? 0) >= retValue)
                    retValue = pclgs7 ?? 0;
                if ((pclgs8 ?? 0) >= retValue)
                    retValue = pclgs8 ?? 0;
                if ((pclgs9 ?? 0) >= retValue)
                    retValue = pclgs9 ?? 0;

                return retValue;
            }
        }
        public int CountPayement
        {
            get
            {
                int retValue = 0;

                if ((pclgs1 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs2 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs3 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs4 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs5 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs6 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs7 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs8 ?? 0) > 0)
                    retValue += 1;
                if ((pclgs9 ?? 0) > 0)
                    retValue += 1;

                return retValue;
            }
        }
        public int DaysPayment
        {
            get
            {
                int retValue = 0;

                if ((pclgs1 ?? 0) >= 0)
                    retValue += pclgs1 ?? 0;
                if ((pclgs2 ?? 0) >= 0)
                    retValue += pclgs2 ?? 0;
                if ((pclgs3 ?? 0) >= 0)
                    retValue += pclgs3 ?? 0;
                if ((pclgs4 ?? 0) >= 0)
                    retValue += pclgs4 ?? 0;
                if ((pclgs5 ?? 0) >= 0)
                    retValue += pclgs5 ?? 0;
                if ((pclgs6 ?? 0) >= 0)
                    retValue += pclgs6 ?? 0;
                if ((pclgs7 ?? 0) >= 0)
                    retValue += pclgs7 ?? 0;
                if ((pclgs8 ?? 0) >= 0)
                    retValue += pclgs8 ?? 0;
                if ((pclgs9 ?? 0) >= 0)
                    retValue += pclgs9 ?? 0;

                return retValue;
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
