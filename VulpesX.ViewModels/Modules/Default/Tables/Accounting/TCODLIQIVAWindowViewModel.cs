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
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class TCODLIQIVAWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public TCODLIQIVAWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required TCODLIQIVA Data { get; set; }
        public bool IsInsert { get; set; }
        public abstract ObservableCollection<GenericIDDescription> ComputeTypes { get; }

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class TCODLIQIVAWindowViewModelDefault : TCODLIQIVAWindowViewModel
    {
        public TCODLIQIVAWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override ObservableCollection<GenericIDDescription> ComputeTypes => CommonsService.LIPEComputeTypes;

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().Update(Data);
            }
        }
    }

    public class TCODLIQIVAWindowViewModelUfp : TCODLIQIVAWindowViewModel
    {
        public TCODLIQIVAWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override ObservableCollection<GenericIDDescription> ComputeTypes => CommonsService.LIPEComputeTypes;

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().Update(Data);
            }
        }
    }
}
