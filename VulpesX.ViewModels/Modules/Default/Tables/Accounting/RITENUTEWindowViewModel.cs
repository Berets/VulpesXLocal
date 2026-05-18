using CerberoRetrieveAPI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class RITENUTEWindowViewModel
    {
        public required string CompanyID { get; set; }

        public RITENUTEWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public required RITENUTE Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public CAUCONT? SelectedCausal { get; set; }
        public CAUCONT? SelectedAssessmentCausal { get; set; }
        public CAUCONT? SelectedPaymentCausal { get; set; }

        public void LoadData()
        {
            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IRITENUTERepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IRITENUTERepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IRITENUTERepository>().Update(Data);
            }
        }
    }
}
