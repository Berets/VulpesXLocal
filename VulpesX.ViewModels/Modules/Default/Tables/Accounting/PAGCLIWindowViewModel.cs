using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class PAGCLIWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public PAGCLIWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            PaymentTypes = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().GetList();
        }

        public required PAGCLI Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<TAB_ACC_TIPINC>? PaymentTypes { get; set; }
        public TAB_ACC_TIPINC? SelectedPaymentType { get; set; }

        public ObservableCollection<LINGUA>? Lingue { get; set; }
        public LINGUA? SelectedLingua { get; set; }

        private PAGCLI_LINGUA? selectedTraduzione;
        public PAGCLI_LINGUA? SelectedTraduzione { get { return selectedTraduzione; } set { selectedTraduzione = value; NotifyPropertyChanged("SelectedTraduzione"); } }

        public ObservableCollection<LINGUA>? GetLINGUA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList(CompanyID);
        }

        public PAGCLI_LINGUA? GetLINGUA(string LanguageID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPAGCLI_LINGUARepository>().Get(Data.pclcod, LanguageID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Validate(Data, IsInsert);
        }

        public bool InsertOrUpdateLanguage()
        {
            if (SelectedTraduzione != null)
            {
                SelectedTraduzione.pclcod = Data.pclcod;
                return VulpesServiceProvider.Provider.GetRequiredService<IPAGCLI_LINGUARepository>().InsertOrUpdate(SelectedTraduzione);
            }

            return false;
        }

        public bool Save()
        {
            InsertOrUpdateLanguage();

            if (IsInsert)
            {
                Data.addedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Update(Data);
            }
        }
    }
}
