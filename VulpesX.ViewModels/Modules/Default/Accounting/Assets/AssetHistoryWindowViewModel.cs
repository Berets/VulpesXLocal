using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.Assets;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class AssetHistoryWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AssetHistoryWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ObservableCollection<GenericIDDescription> ComputeTypes => CommonsService.AccountingAssetsLaunchComputeTypes;
        public string? SelectedComputeType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public bool UpdateHistory(int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_CARDSRepository>().UpdateHistory(CompanyID, UserID, Year, SelectedComputeType!, DateFrom!.Value, DateTo!.Value);

        }
    }
}
