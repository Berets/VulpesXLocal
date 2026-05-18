using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.DWH;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.DWH
{
    public class DWHQueryWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public DWHQueryWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required DWH_Query Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if(IsInsert)
            {
                Data.LogAdded = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                Data.LogAddedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Insert(Data);
            }
            else
            {
                Data.LogUpdated = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                Data.LogUpdatedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Update(Data);
            }
        }
    }
}
