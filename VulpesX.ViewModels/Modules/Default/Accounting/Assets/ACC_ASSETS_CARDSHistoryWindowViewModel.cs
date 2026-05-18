using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Models.Models.Accounting.Assets;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Assets
{
    public class ACC_ASSETS_CARDSHistoryWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_ASSETS_CARDSHistoryWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public string? Title { get; set; }
        public ObservableCollection<AssetHistoryItem>? Items { get; set; }
    }
}
