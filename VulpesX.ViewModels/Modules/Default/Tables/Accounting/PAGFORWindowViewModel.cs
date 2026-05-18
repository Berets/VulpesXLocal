using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class PAGFORWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public PAGFORWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            PaymentTypes = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().GetList();
        }

        public required PAGFOR Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<TAB_ACC_TIPPAG>? PaymentTypes { get; set; }
        public TAB_ACC_TIPPAG? SelectedPaymentType { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Update(Data);
            }
        }
    }
}
