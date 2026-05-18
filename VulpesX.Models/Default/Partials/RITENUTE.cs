using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class RITENUTE
    {
        public RITENUTE()
        { }

        public string? CompanyID { get; set; }
        public string FullDescriptionSearchable => $"{ritcod} {ritdes?.Trim()}";
        public bool ritfl1Bool
        {
            get
            {
                return ritfl1 == "S";
            }
            set
            {
                if (value)
                    ritfl1 = "S";
                else
                    ritfl1 = "N";
            }
        }
        public bool ritfl2Bool
        {
            get
            {
                return ritfl2 == "S";
            }
            set
            {
                if (value)
                    ritfl2 = "S";
                else
                    ritfl2 = "N";
            }
        }
        public string? rtsezDescription => Sections.Where(w => w.ID == rtsez).FirstOrDefault()?.Description;
        public string? rttipoDescription => ElementTypes.Where(w => w.ID == rttipo).FirstOrDefault()?.Description;
        public string? rtTipRedDescription => RedditTypes.Where(w => w.ID == rtTipRed).FirstOrDefault()?.Description;
        public string? rtmeseDescription => CommonsService.MonthsNamesWithNone.Where(w => w.ID == rtmese).FirstOrDefault()?.Description;
        public ObservableCollection<GenericIntIDDescription> Months => CommonsService.MonthsNamesWithNone;
        public ObservableCollection<GenericIDDescription> Sections => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "E", Description = "Erario" },
            new GenericIDDescription(){ ID = "I", Description = "I.N.P.S." },
            new GenericIDDescription(){ ID = "R", Description = "Regioni" },
            new GenericIDDescription(){ ID = "T", Description = "I.C.I. ed altri tributi locali" },
            new GenericIDDescription(){ ID = "P", Description = "Altri enti previdenziali ed assicurativi" }
        };

        public ObservableCollection<GenericIDDescription> ElementTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
            new GenericIDDescription(){ ID = "D", Description = "Imposte dirette" },
            new GenericIDDescription(){ ID = "I", Description = "IVA" },
            new GenericIDDescription(){ ID = "R", Description = "Ritenute alla fonte" },
            new GenericIDDescription(){ ID = "T", Description = "Altri tributi ed interessi" }
        };

        public ObservableCollection<GenericIDDescription> RedditTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Prestazioni di lavoro autonomo (professionisti, notai avvocati ecc..)" },
            new GenericIDDescription(){ ID = "O", Description = "Prestazioni occasionali" },
            new GenericIDDescription(){ ID = "R", Description = "Provvigioni plurimandatari" },
            new GenericIDDescription(){ ID = "N", Description = "Indennita' di trasferta, rimborso forfettario di spese, premi e compensi erogati" }
        };

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
                if (!string.IsNullOrWhiteSpace(ritgr1))
                    SelectedAssessmentGroup = groupsList?.Where(w => w.P1GRUP == ritgr1).FirstOrDefault();
                else
                    SelectedAssessmentGroup = null;
                if (!string.IsNullOrWhiteSpace(ritgr2))
                    SelectedPaymentGroup = groupsList?.Where(w => w.P1GRUP == ritgr2).FirstOrDefault();
                else
                    SelectedPaymentGroup = null;
                NotifyPropertyChanged("GroupsList");
            }
        }
        // accertamento
        private ObservableCollection<PDCCONTI>? assessmentAccountsList;
        public ObservableCollection<PDCCONTI>? AssessmentAccountsList
        {
            get { return assessmentAccountsList; }
            set
            {
                assessmentAccountsList = value;
                if (!string.IsNullOrWhiteSpace(ritgr1) && !string.IsNullOrWhiteSpace(ritco1))
                    SelectedAssessmentAccount = assessmentAccountsList?.Where(w => w.P1GRUP == ritgr1 && w.P2CONT == ritco1).FirstOrDefault();
                else
                    SelectedAssessmentAccount = null;
                NotifyPropertyChanged("AssessmentAccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? assessmentSubaccountsList;
        public ObservableCollection<PDCSOTTO>? AssessmentSubaccountsList
        {
            get { return assessmentSubaccountsList; }
            set
            {
                assessmentSubaccountsList = value;
                if (!string.IsNullOrWhiteSpace(ritgr1) && !string.IsNullOrWhiteSpace(ritco1) && !string.IsNullOrWhiteSpace(ritso1))
                    SelectedAssessmentSubaccount = assessmentSubaccountsList?.Where(w => w.P1GRUP == ritgr1 && w.P2CONT == ritco1 && w.P3SOTC == ritso1).FirstOrDefault();
                else
                    SelectedAssessmentSubaccount = null;
                NotifyPropertyChanged("AssessmentSubaccountsList");
            }
        }
        private PDCGRUPPI? selectedAssessmentGroup;
        public PDCGRUPPI? SelectedAssessmentGroup
        {
            get
            {
                return selectedAssessmentGroup;
            }
            set
            {
                if (selectedAssessmentGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AssessmentAccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AssessmentAccountsList = null;
                    }
                    if (selectedAssessmentGroup != null)
                    {
                        SelectedAssessmentAccount = null;
                        SelectedAssessmentSubaccount = null;
                    }
                    ritgr1 = value?.P1GRUP;
                    selectedAssessmentGroup = value;
                    NotifyPropertyChanged("SelectedAssessmentGroup");
                }
            }
        }
        private PDCCONTI? selectedAssessmentAccount;
        public PDCCONTI? SelectedAssessmentAccount
        {
            get
            {
                return selectedAssessmentAccount;
            }
            set
            {
                if (selectedAssessmentAccount?.P1GRUP != value?.P1GRUP || selectedAssessmentAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        AssessmentSubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        AssessmentSubaccountsList = null;
                    }
                    if (selectedAssessmentAccount != null)
                    {
                        SelectedAssessmentSubaccount = null;
                    }
                    ritco1 = value?.P2CONT;
                    selectedAssessmentAccount = value;
                    NotifyPropertyChanged("SelectedAssessmentAccount");
                }
            }
        }
        private PDCSOTTO? selectedAssessmentSubaccount;
        public PDCSOTTO? SelectedAssessmentSubaccount
        {
            get
            {
                return selectedAssessmentSubaccount;
            }
            set
            {
                if (selectedAssessmentSubaccount?.P1GRUP != value?.P1GRUP || selectedAssessmentSubaccount?.P2CONT != value?.P2CONT || selectedAssessmentSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    ritso1 = value?.P3SOTC;
                    selectedAssessmentSubaccount = value;
                    NotifyPropertyChanged("SelectedAssessmentSubaccount");
                }
            }
        }

        // pagamento
        private ObservableCollection<PDCCONTI>? paymentAccountsList;
        public ObservableCollection<PDCCONTI>? PaymentAccountsList
        {
            get { return paymentAccountsList; }
            set
            {
                paymentAccountsList = value;
                if (!string.IsNullOrWhiteSpace(ritgr2) && !string.IsNullOrWhiteSpace(ritco2))
                    SelectedPaymentAccount = paymentAccountsList?.Where(w => w.P1GRUP == ritgr2 && w.P2CONT == ritco2).FirstOrDefault();
                else
                    SelectedPaymentAccount = null;
                NotifyPropertyChanged("PaymentAccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? paymentSubaccountsList;
        public ObservableCollection<PDCSOTTO>? PaymentSubaccountsList
        {
            get { return paymentSubaccountsList; }
            set
            {
                paymentSubaccountsList = value;
                if (!string.IsNullOrWhiteSpace(ritgr2) && !string.IsNullOrWhiteSpace(ritco2) && !string.IsNullOrWhiteSpace(ritso2))
                    SelectedPaymentSubaccount = paymentSubaccountsList?.Where(w => w.P1GRUP == ritgr2 && w.P2CONT == ritco2 && w.P3SOTC == ritso2).FirstOrDefault();
                else
                    SelectedPaymentSubaccount = null;
                NotifyPropertyChanged("PaymentSubaccountsList");
            }
        }
        private PDCGRUPPI? selectedPaymentGroup;
        public PDCGRUPPI? SelectedPaymentGroup
        {
            get
            {
                return selectedPaymentGroup;
            }
            set
            {
                if (selectedPaymentGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        PaymentAccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        PaymentAccountsList = null;
                    }
                    if (selectedPaymentGroup != null)
                    {
                        SelectedPaymentAccount = null;
                        SelectedPaymentSubaccount = null;
                    }
                    ritgr2 = value?.P1GRUP;
                    selectedPaymentGroup = value;
                    NotifyPropertyChanged("SelectedPaymentGroup");
                }
            }
        }
        private PDCCONTI? selectedPaymentAccount;
        public PDCCONTI? SelectedPaymentAccount
        {
            get
            {
                return selectedPaymentAccount;
            }
            set
            {
                if (selectedPaymentAccount?.P1GRUP != value?.P1GRUP || selectedPaymentAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        PaymentSubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        PaymentSubaccountsList = null;
                    }
                    if (selectedPaymentAccount != null)
                    {
                        SelectedPaymentSubaccount = null;
                    }
                    ritco2 = value?.P2CONT;
                    selectedPaymentAccount = value;
                    NotifyPropertyChanged("SelectedPaymentAccount");
                }

            }
        }
        private PDCSOTTO? selectedPaymentSubaccount;
        public PDCSOTTO? SelectedPaymentSubaccount
        {
            get
            {
                return selectedPaymentSubaccount;
            }
            set
            {
                if (selectedPaymentSubaccount?.P1GRUP != value?.P1GRUP || selectedPaymentSubaccount?.P2CONT != value?.P2CONT || selectedPaymentSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    ritso2 = value?.P3SOTC;
                    selectedPaymentSubaccount = value;
                    NotifyPropertyChanged("SelectedPaymentSubaccount");
                }
            }
        }
        #endregion
    }
}
