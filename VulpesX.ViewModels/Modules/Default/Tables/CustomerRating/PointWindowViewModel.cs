using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CustomerRating;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.CustomerRating
{
    public class PointWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public PointWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetList();
        }

        public required cr_tab_points_financial Data { get; set; }
        public bool IsInsert { get; set; }

        private ObservableCollection<CAUCONT>? _causals;
        public ObservableCollection<CAUCONT>? Causals { get { return _causals; } set { _causals = value; } }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Icr_tab_points_financialRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Icr_tab_points_financialRepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<Icr_tab_points_financialRepository>().Update(Data);
            }
        }
    }
}
