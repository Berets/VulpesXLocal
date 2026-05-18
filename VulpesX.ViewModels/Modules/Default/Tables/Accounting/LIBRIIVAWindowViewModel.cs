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
    public abstract class LIBRIIVAWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public LIBRIIVAWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required LIBRIIVA Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public ObservableCollection<GenericIDDescription> BookTypes => CommonsService.IVABookTypesUfp;
        public CAUCONT? SelectedGiroCausal { get; set; }
        public CAUCONT? SelectedStornoCausal { get; set; }

        public abstract void LoadData();

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class LIBRIIVAWindowViewModelDefault : LIBRIIVAWindowViewModel
    {
        public LIBRIIVAWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }
        
        public override void LoadData()
        {

            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Update(Data);
            }
        }
    }

    public class LIBRIIVAWindowViewModelUfp : LIBRIIVAWindowViewModel
    {
        public LIBRIIVAWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public override void LoadData()
        {

            Data.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Data.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Data.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Insert(Data);

            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Update(Data);
            }
        }
    }

}
