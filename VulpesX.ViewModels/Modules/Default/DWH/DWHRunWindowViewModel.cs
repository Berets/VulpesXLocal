using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Models.Default;

namespace VulpesX.ViewModels.Modules.Default.DWH
{
    public class DWHRunWindowViewModel
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public DWHRunWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ObservableCollection<DWH_QueryParameter> Parameters { get; set; } = new ObservableCollection<DWH_QueryParameter>();
    }
}
