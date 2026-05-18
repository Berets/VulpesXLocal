using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Ufp.Tables.Accounting
{
    public class COMTIPREGWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public COMTIPREGWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Signs = CommonsService.StandardAccountingSigns;
        }

        public required COMTIPREG Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription>? Signs { get; set; }

        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }
        public ObservableCollection<CAUCONT>? CausalsList { get; set; }
        public CAUCONT? SelectedCausal { get; set; }

        public ObservableCollection<COMTIPREGLEVEL1>? Details { get; set; }


        public ObservableCollection<COMTIPREGLEVEL1>? GetCOMTIPREGLEVEL1()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICOMTIPREGUfpRepository>().GetDetails(CompanyID, Data.causcon ?? string.Empty, Data.cauprco ?? 0);
        }

        public ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public List<PDCCONTI>? GetPDCCONTI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public List<PDCSOTTO>? GetPDCSOTTO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public ObservableCollection<CAUCONT>? GetCAUCONT()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList("*");
        }


        public string? ValidateRow(COMTIPREGLEVEL1 Detail)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICOMTIPREGUfpRepository>().ValidateRow(Detail, Detail.IsInsert);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICOMTIPREGUfpRepository>().Validate(Data, Details, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ICOMTIPREGUfpRepository>().Insert(Data, Details, CompanyID);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ICOMTIPREGUfpRepository>().Update(Data, Details, CompanyID);
            }
        }
    }
}
