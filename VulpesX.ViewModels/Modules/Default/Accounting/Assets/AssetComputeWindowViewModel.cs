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
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class AssetComputeWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AssetComputeWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ObservableCollection<GenericIDDescription> ComputeTypes => CommonsService.AccountingAssetsLaunchComputeTypes;
        public string? SelectedComputeType { get; set; }

        public ESERCIZIO? GetESERCIZIO(int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, Year);
        }

        public bool ComputeDepreciation(int Year, DateTime DateTo, ESERCIZIO Esercizio)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().ComputeDepreciation(CompanyID, UserID, Year, DateTo, SelectedComputeType!, Esercizio);

        }
    }
}
