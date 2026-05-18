using DocumentFormat.OpenXml.EMMA;
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
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class FE_TIPODOCWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public FE_TIPODOCWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public required FE_TIPODOC Data { get; set; }
        public bool IsInsert { get; set; }

        public CAUCONT? SelectedCausal { get; set; }
        public ObservableCollection<CAUCONT>? Causals { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (!IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Update(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Insert(Data);
            }
        }
    }
}
