using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class LIBRIIVA
    {
        public LIBRIIVA()
        { }
        public string? CompanyID { get; set; }
        public string FullDescriptionSearchable => $"{livcod} {livdes?.Trim()}";

        public string? BookTypeFullDescription => CommonsService.IVABookTypes.Where(w => w.ID == livtip).Select(s => $"{s?.ID} {s?.Description}").FirstOrDefault();
        public string? BookTypeFullDescriptionUfp => CommonsService.IVABookTypesUfp.Where(w => w.ID == livtip).Select(s => $"{s?.ID} {s?.Description}").FirstOrDefault();

        public bool livautBool
        {
            get
            {
                return livaut == "S";
            }
            set
            {
                if (value)
                    livaut = "S";
                else
                    livaut = "N";
            }
        }

        #region PDC
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }
        private ObservableCollection<PDCGRUPPI>? groupsList;
        public ObservableCollection<PDCGRUPPI>? GroupsList
        {
            get { return groupsList; }
            set
            {
                groupsList = value;
                // IVA
                if (!string.IsNullOrWhiteSpace(livgci))
                    SelectedIVAGroup = groupsList?.Where(w => w.P1GRUP == livgci).FirstOrDefault();
                else
                    SelectedIVAGroup = null;
                // Erario
                if (!string.IsNullOrWhiteSpace(livgce))
                    SelectedErarioGroup = groupsList?.Where(w => w.P1GRUP == livgce).FirstOrDefault();
                else
                    SelectedErarioGroup = null;
                NotifyPropertyChanged("GroupsList");
            }
        }
        // IVA
        private ObservableCollection<PDCCONTI>? ivaAccountList;
        public ObservableCollection<PDCCONTI>? IVAAccountList
        {
            get { return ivaAccountList; }
            set
            {
                ivaAccountList = value;
                if (!string.IsNullOrWhiteSpace(livgci) && !string.IsNullOrWhiteSpace(livcci))
                    SelectedIVAAccount = ivaAccountList?.Where(w => w.P1GRUP == livgci && w.P2CONT == livcci).FirstOrDefault();
                else
                    SelectedIVAAccount = null;
                NotifyPropertyChanged("IVAAccountList");
            }
        }
        private ObservableCollection<PDCSOTTO>? ivaSubaccountList;
        public ObservableCollection<PDCSOTTO>? IVASubaccountList
        {
            get { return ivaSubaccountList; }
            set
            {
                ivaSubaccountList = value;
                if (!string.IsNullOrWhiteSpace(livgci) && !string.IsNullOrWhiteSpace(livcci) && !string.IsNullOrWhiteSpace(livsci))
                    SelectedIVASubaccount = ivaSubaccountList?.Where(w => w.P1GRUP == livgci && w.P2CONT == livcci && w.P3SOTC == livsci).FirstOrDefault();
                else
                    SelectedIVASubaccount = null;
                NotifyPropertyChanged("IVASubaccountList");
            }
        }

        private PDCGRUPPI? selectedIVAGroup;
        public PDCGRUPPI? SelectedIVAGroup
        {
            get
            {
                return selectedIVAGroup;
            }
            set
            {
                if (selectedIVAGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        IVAAccountList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        IVAAccountList = null;
                    }
                    if (selectedIVAGroup != null)
                    {
                        SelectedIVAAccount = null;
                        SelectedIVASubaccount = null;
                    }
                    livgci = value?.P1GRUP;
                    selectedIVAGroup = value;
                    NotifyPropertyChanged("SelectedIVAGroup");
                }
            }
        }

        private PDCCONTI? selectedIVAAccount;
        public PDCCONTI? SelectedIVAAccount
        {
            get
            {
                return selectedIVAAccount;
            }
            set
            {
                if (selectedIVAAccount?.P1GRUP != value?.P1GRUP || selectedIVAAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        IVASubaccountList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        IVASubaccountList = null;
                    }
                    if (selectedIVAAccount != null)
                    {
                        SelectedIVASubaccount = null;
                    }
                    livcci = value?.P2CONT;
                    selectedIVAAccount = value;
                    NotifyPropertyChanged("SelectedIVAAccount");
                }
            }
        }

        private PDCSOTTO? selectedIVASubaccount;

        public PDCSOTTO? SelectedIVASubaccount
        {
            get
            {
                return selectedIVASubaccount;
            }
            set
            {
                if (selectedIVASubaccount?.P1GRUP != value?.P1GRUP || selectedIVASubaccount?.P2CONT != value?.P2CONT || selectedIVASubaccount?.P3SOTC != value?.P3SOTC)
                {
                    livsci = value?.P3SOTC;
                    selectedIVASubaccount = value;
                    NotifyPropertyChanged("SelectedIVASubaccount");
                }
            }
        }

        // Erario
        private ObservableCollection<PDCCONTI>? erarioAccountsList;
        public ObservableCollection<PDCCONTI>? ErarioAccountsList
        {
            get { return erarioAccountsList; }
            set
            {
                erarioAccountsList = value;
                if (!string.IsNullOrWhiteSpace(livgce) && !string.IsNullOrWhiteSpace(livcce))
                    SelectedErarioAccount = erarioAccountsList?.Where(w => w.P1GRUP == livgce && w.P2CONT == livcce).FirstOrDefault();
                else
                    SelectedErarioAccount = null;
                NotifyPropertyChanged("ErarioAccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? erarioSubaccountList;
        public ObservableCollection<PDCSOTTO>? ErarioSubaccountList
        {
            get { return erarioSubaccountList; }
            set
            {
                erarioSubaccountList = value;
                if (!string.IsNullOrWhiteSpace(livgce) && !string.IsNullOrWhiteSpace(livcce) && !string.IsNullOrWhiteSpace(livsce))
                    SelectedErarioSubaccount = erarioSubaccountList?.Where(w => w.P1GRUP == livgce && w.P2CONT == livcce && w.P3SOTC == livsce).FirstOrDefault();
                else
                    SelectedErarioSubaccount = null;
                NotifyPropertyChanged("ErarioSubaccountList");
            }
        }

        private PDCGRUPPI? selectedErarioGroup;
        public PDCGRUPPI? SelectedErarioGroup
        {
            get
            {
                return selectedErarioGroup;
            }
            set
            {
                if (selectedErarioGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        ErarioAccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        ErarioAccountsList = null;
                    }
                    if (selectedErarioGroup != null)
                    {
                        SelectedErarioAccount = null;
                        SelectedErarioSubaccount = null;
                    }
                    livgce = value?.P1GRUP;
                    selectedErarioGroup = value;
                    NotifyPropertyChanged("SelectedErarioGroup");
                }
            }
        }
        private PDCCONTI? selectedErarioAccount;
        public PDCCONTI? SelectedErarioAccount
        {
            get
            {
                return selectedErarioAccount;
            }
            set
            {
                if (selectedErarioAccount?.P1GRUP != value?.P1GRUP || selectedErarioAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        ErarioSubaccountList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        ErarioSubaccountList = null;
                    }
                    if (selectedErarioAccount != null)
                    {
                        SelectedErarioSubaccount = null;
                    }
                    livcce = value?.P2CONT;
                    selectedErarioAccount = value;
                    NotifyPropertyChanged("SelectedErarioAccount");
                }
            }
        }

        private PDCSOTTO? selectedErarioSubaccount;

        public PDCSOTTO? SelectedErarioSubaccount
        {
            get
            {
                return selectedErarioSubaccount;
            }
            set
            {
                if (selectedErarioSubaccount?.P1GRUP != value?.P1GRUP || selectedErarioSubaccount?.P2CONT != value?.P2CONT || selectedErarioSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    livsce = value?.P3SOTC;
                    selectedErarioSubaccount = value;
                    NotifyPropertyChanged("SelectedErarioSubaccount");
                }
            }
        }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        #endregion

        public string? livpro { get; set; }
        public string? livagg { get; set; }
        public bool livaggBool
        {
            get
            {
                return livagg == "S";
            }
            set
            {
                if (value)
                    livagg = "S";
                else
                    livagg = "N";
            }
        }

    }
}
