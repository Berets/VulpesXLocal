using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM.AF;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Ufp.Tables.CRM.AF
{
    public class ANAFAT_CONSTWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public ANAFAT_CONSTWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required ANAFAT_CONST Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription> ProductionType => CommonsService.ANAFAT_PIECESProductionTypes;

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!Data.IsActive)
            {
                Data.canceledUserID = UserID;
                Data.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
            }

            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().Insert(Data);
            }
            else
            {
                Data.updateUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_CONSTRepository>().Update(Data);
            }
        }
    }
}
