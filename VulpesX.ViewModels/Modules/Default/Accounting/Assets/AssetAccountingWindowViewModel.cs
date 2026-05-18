using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.Assets;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class AssetAccountingWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AssetAccountingWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public CAUCONT? SelectedCausal { get; set; }
        public ESERCIZIO? AccountingYear { get; set; }

        public  ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ObservableCollection<CAUCONT>? GetCAUCONTs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetList();
        }

        public bool Accounting(DateTime DateTo)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().Accounting(CompanyID,UserID,AccountingYear!,DateTo, SelectedCausal!);

        }
    }
}
