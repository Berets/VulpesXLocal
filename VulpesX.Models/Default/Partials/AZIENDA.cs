using Org.BouncyCastle.Asn1.Crmf;
using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class AZIENDA
    {
        public string? SocialCapital => azcapsoc.HasValue ? $"{azcapsoc.Value.ToString("N2")}" : null;
        public string CoordText => !string.IsNullOrWhiteSpace(azdirez) ? $" |Direzione e coordinamento {azdirez.Trim()}" : string.Empty;
        public string LegalSiteFirst => $"{(!string.IsNullOrWhiteSpace(azinsa) && azinsl == azinsa && azcasl == azcasa && azlosl == azlosa && azprsl == azprsa ? "Sede legale ed operativa" : "Sede legale")}: {azinsl?.Trim()}";
        public string LegalSiteSecond => $"{azcasl} {azlosl?.Trim()} ({azprsl?.Trim()})";
        public string? OperationSiteFirst => !string.IsNullOrWhiteSpace(azinsa) && (azinsl != azinsa || azcasl != azcasa || azlosl != azlosa || azprsl != azprsa) ? $"Sede operativa: {azinsa.Trim()}" : null;
        public string? OperationSiteSecond => !string.IsNullOrWhiteSpace(azinsa) && (azinsl != azinsa || azcasl != azcasa || azlosl != azlosa || azprsl != azprsa) ? $"{azcasa} {azlosa?.Trim()} ({azprsa?.Trim()})" : null;

        #region GILATCausalDDT
        private ObservableCollection<CAUSBOLL>? ddtCausals;
        public ObservableCollection<CAUSBOLL>? DDTCausals
        {
            get => ddtCausals;
            set
            {
                ddtCausals = value;
                if (!string.IsNullOrWhiteSpace(azimpgilatcau))
                    GILATCausalDDT = ddtCausals?.Where(w => w.bolcod == azimpgilatcau).FirstOrDefault();
                else
                    GILATCausalDDT = null;
                if (!string.IsNullOrWhiteSpace(azimpbancolatcau))
                    BANCOLATCausalDDT = ddtCausals?.Where(w => w.bolcod == azimpbancolatcau).FirstOrDefault();
                else
                    BANCOLATCausalDDT = null;
                NotifyPropertyChanged("DDTCausals");
            }
        }

        private CAUSBOLL? gilatCausalDDT;
        public CAUSBOLL? GILATCausalDDT
        {
            get => gilatCausalDDT;
            set
            {
                azimpgilatcau = value?.bolcod;
                gilatCausalDDT = value;
                NotifyPropertyChanged("GILATCausalDDT");
            }
        }
        #endregion

        #region GILATRateDDT
        private ObservableCollection<ASSOGGETAMENTI>? rates;
        public ObservableCollection<ASSOGGETAMENTI>? Rates
        {
            get => rates;
            set
            {
                rates = value;
                if (!string.IsNullOrWhiteSpace(azimpgilatalc) && !string.IsNullOrWhiteSpace(azimpgilatala))
                    GILATRateDDT = rates?.Where(w => w.asscod == azimpgilatalc && w.assali == azimpgilatala).FirstOrDefault();
                else
                    GILATRateDDT = null;
                NotifyPropertyChanged("Rates");
            }
        }

        private ASSOGGETAMENTI? gilatRateDDT;
        public ASSOGGETAMENTI? GILATRateDDT
        {
            get => gilatRateDDT;
            set
            {
                azimpgilatalc = value?.asscod;
                azimpgilatala = value?.assali;
                gilatRateDDT = value;
                NotifyPropertyChanged("GILATRateDDT");
            }
        }
        #endregion

        #region BANCOLATCausalDDT

        private CAUSBOLL? bancolatCausalDDT;
        public CAUSBOLL? BANCOLATCausalDDT
        {
            get => bancolatCausalDDT;
            set
            {
                azimpbancolatcau = value?.bolcod;
                bancolatCausalDDT = value;
                NotifyPropertyChanged("BANCOLATCausalDDT");
            }
        }
        #endregion

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
                if (!string.IsNullOrWhiteSpace(azdisgrp))
                    SelectedGroup = groupsList?.Where(w => w.P1GRUP == azdisgrp).FirstOrDefault();
                else
                    SelectedGroup = null;
                NotifyPropertyChanged("GroupsList");
            }
        }
        private ObservableCollection<PDCCONTI>? accountsList;
        public ObservableCollection<PDCCONTI>? AccountsList
        {
            get { return accountsList; }
            set
            {
                accountsList = value;
                if (!string.IsNullOrWhiteSpace(azdisgrp) && !string.IsNullOrWhiteSpace(azdiscnt))
                    SelectedAccount = accountsList?.Where(w => w.P1GRUP == azdisgrp && w.P2CONT == azdiscnt).FirstOrDefault();
                else
                    SelectedAccount = null;
                NotifyPropertyChanged("AccountsList");
            }
        }
        private ObservableCollection<PDCSOTTO>? subaccountsList;
        public ObservableCollection<PDCSOTTO>? SubaccountsList
        {
            get { return subaccountsList; }
            set
            {
                subaccountsList = value;
                if (!string.IsNullOrWhiteSpace(azdisgrp) && !string.IsNullOrWhiteSpace(azdiscnt) && !string.IsNullOrWhiteSpace(azdissot))
                    SelectedSubaccount = subaccountsList?.Where(w => w.P1GRUP == azdisgrp && w.P2CONT == azdiscnt && w.P3SOTC == azdissot).FirstOrDefault();
                else
                    SelectedSubaccount = null;
                NotifyPropertyChanged("SubaccountsList");
            }
        }

        private PDCGRUPPI? selectedGroup;
        public PDCGRUPPI? SelectedGroup
        {
            get
            {
                return selectedGroup;
            }
            set
            {
                if (selectedGroup?.P1GRUP != value?.P1GRUP)
                {
                    if (value != null && AccountCache != null)
                    {
                        AccountsList = new ObservableCollection<PDCCONTI>(AccountCache.Where(w => w.P1GRUP == value.P1GRUP).ToList());
                    }
                    else
                    {
                        AccountsList = null;
                    }
                    if (selectedGroup != null)
                    {
                        SelectedAccount = null;
                        SelectedSubaccount = null;
                        SubaccountsList = null;
                    }
                    azdisgrp = value?.P1GRUP;
                    selectedGroup = value;
                    NotifyPropertyChanged("SelectedGroup");
                }
            }
        }

        private PDCCONTI? selectedAccount;
        public PDCCONTI? SelectedAccount
        {
            get
            {
                return selectedAccount;
            }
            set
            {
                if (selectedAccount?.P1GRUP != value?.P1GRUP && selectedAccount?.P2CONT != value?.P2CONT)
                {

                    if (value != null && SubaccountCache != null)
                    {
                        SubaccountsList = new ObservableCollection<PDCSOTTO>(SubaccountCache.Where(w => w.P1GRUP == value.P1GRUP && w.P2CONT == value.P2CONT).ToList());
                    }
                    else
                    {
                        SubaccountsList = null;
                    }
                    if (selectedAccount != null)
                    {
                        SelectedSubaccount = null;
                        SubaccountsList = null;
                    }
                    azdiscnt = value?.P2CONT;
                    selectedAccount = value;
                    NotifyPropertyChanged("SelectedAccount");
                }
            }
        }

        private PDCSOTTO? selectedSubaccount;

        public PDCSOTTO? SelectedSubaccount
        {
            get
            {
                return selectedSubaccount;
            }
            set
            {
                if (selectedSubaccount?.P1GRUP != value?.P1GRUP && selectedSubaccount?.P2CONT != value?.P2CONT && selectedSubaccount?.P3SOTC != value?.P3SOTC)
                {
                    azdissot = value?.P3SOTC;
                    selectedSubaccount = value;
                    NotifyPropertyChanged("SelectedSubaccount");
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

        public short? azca2z { get; set; }
        public short? azsa2c { get; set; }
        public string? azvalu { get; set; }
        public short? azp2lr { get; set; }
        public short? azprl2 { get; set; }
        public short? azpec2 { get; set; }
        public string? azieat { get; set; }
        public string? azdddr { get; set; }
        public string? azddf7 { get; set; }
        public string? azcfdi { get; set; }
        public string? azcodi { get; set; }
        public string? aznodi { get; set; }
        public string? azindi { get; set; }
        public int? azcadi { get; set; }
        public string? azlodi { get; set; }
        public string? azprdi { get; set; }
        public short? azdi2c { get; set; }
        public string? aztedi { get; set; }
        public short? azlast { get; set; }

        public string? azpth { get; set; }
        public string? azdoc { get; set; }

        public string? AZRagso { get; set; }
        public string? AZIndir2 { get; set; }

        public short? AZCap { get; set; }
        public string? Azban { get; set; }

        public string? azpca { get; set; }
        public string? azpcp { get; set; }
        public string? azplc { get; set; }

        public string? azindsito { get; set; }
        public string? azcapintver { get; set; }

        public string? azloccamcom { get; set; }
        public string? aznumcam { get; set; }
        public string? azlocreg { get; set; }
        public string? aznumreg { get; set; }
        public string? azcripto { get; set; }

    }
}
