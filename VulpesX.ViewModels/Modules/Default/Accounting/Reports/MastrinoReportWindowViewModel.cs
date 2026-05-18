using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Reports
{
    public class MastrinoReportWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public MastrinoReportWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public int? AccountingYear { get; set; }


        private DateTime? printFrom;
        public DateTime? PrintFrom { get => printFrom; set { printFrom = value; NotifyPropertyChanged("PrintFrom"); } }
        public DateTime? PrintUntil { get; set; }
        public bool MonthlyGroup { get; set; }
        public bool IsDefinitive { get; set; }

        #region PDC
        private ObservableCollection<PDCGRUPPI>? groupsList;
        public ObservableCollection<PDCGRUPPI>? GroupsList
        {
            get { return groupsList; }
            set
            {
                groupsList = value;
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
                   
                    if (selectedGroup != null)
                    {
                        SelectedAccount = null;
                        SelectedSubaccount = null;
                        SubaccountsList = null;
                    }
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

                    
                    if (selectedAccount != null)
                    {
                        SelectedSubaccount = null;
                        SubaccountsList = null;
                    }
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
                    selectedSubaccount = value;
                    NotifyPropertyChanged("SelectedSubaccount");
                }
            }
        }
        #endregion

        private ABE? selectedEntity;
        public ABE? SelectedEntity { get => selectedEntity; set { selectedEntity = value; NotifyPropertyChanged("SelectedEntity"); } }

        public ObservableCollection<ABE>? Entities { get; set; }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetList(CompanyID);
        }

        public ESERCIZIO? GetESERCIZIO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID , AccountingYear!.Value);
        }

        public ObservableCollection<ABE>? GetABEs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList();
        }


        public ObservableCollection<PDCGRUPPI>? GetPDCGRUPPIs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public ObservableCollection<PDCCONTI>? GetPDCCONTIs(string GroupID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList(GroupID);
        }

        public ObservableCollection<PDCSOTTO>? GetPDCSOTTOs(string GroupID,string AccountID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(GroupID,AccountID, CompanyID);
        }

        public MastrinoReport? PrintMastrino()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintMastrino(CompanyID, AccountingYear!.Value, SelectedGroup?.P1GRUP, SelectedAccount?.P2CONT, SelectedSubaccount?.P3SOTC, PrintFrom!.Value, PrintUntil!.Value, SelectedEntity, MonthlyGroup, IsDefinitive);
        }

        public MastrinoReport? ReprintMastrino()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().ReprintMastrino(CompanyID, AccountingYear!.Value, SelectedGroup?.P1GRUP, SelectedAccount?.P2CONT, SelectedSubaccount?.P3SOTC, PrintFrom!.Value, PrintUntil!.Value, SelectedEntity, MonthlyGroup);
        }
    }
}
