using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Store
{
    public class STORE_CAUSALSWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }


        public STORE_CAUSALSWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required store_causals Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription> StoreTypes => CommonsService.StoreTypes;

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().Update(Data);
            }
        }

        public void LoadDetails()
        {
            Data.CostCentersList = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
            Data.CausalsList = VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().GetList(CompanyID);
            Data.StoresList = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID);
        }
    }
}
