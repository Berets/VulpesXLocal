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
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class CLAZIONEWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public CLAZIONEWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required CLAZIONE Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLAZIONERepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ICLAZIONERepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ICLAZIONERepository>().Update(Data);
            }
        }
    }
}
