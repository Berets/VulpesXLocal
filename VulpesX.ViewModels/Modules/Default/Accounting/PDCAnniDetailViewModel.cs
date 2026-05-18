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
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class PDCAnniDetailViewModel : Base
    {
        public abstract required string CompanyID { get; set; }
        public ObservableCollection<TAB_ACC_CLOSING>? Closings { get; set; }
        public PDCAnniDetailViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public PDCANNI? Data { get; set; }

        public abstract ObservableCollection<GenericIDDescription> GroupTypes { get; }

        public bool IsInsert { get; set; }

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class PDCAnniDetailViewModelDefault : PDCAnniDetailViewModel
    {
        public override required string CompanyID { get; set; }
        public PDCAnniDetailViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            Closings = VulpesServiceProvider.Provider.GetRequiredService<ICODCHIUSURARepository>().GetList();
        }


        public override ObservableCollection<GenericIDDescription> GroupTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Attività" },
            new GenericIDDescription(){ ID = "P", Description = "Passività" },
            new GenericIDDescription(){ ID = "C", Description = "Costi" },
            new GenericIDDescription(){ ID = "R", Description = "Ricavi" }
        };


        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Validate(Data!, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Insert(Data!);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Update(Data!);
        }
    }

    public class PDCAnniDetailViewModelUfp : PDCAnniDetailViewModel
    {
        public override required string CompanyID { get; set; }
        public PDCAnniDetailViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            Closings = VulpesServiceProvider.Provider.GetRequiredService<ICODCHIUSURARepository>().GetList();
        }


        public override ObservableCollection<GenericIDDescription> GroupTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "A", Description = "Attività" },
            new GenericIDDescription(){ ID = "P", Description = "Passività" },
            new GenericIDDescription(){ ID = "C", Description = "Costi" },
            new GenericIDDescription(){ ID = "R", Description = "Ricavi" }
        };


        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Validate(Data!, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Insert(Data!);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Update(Data!);
        }
    }

}
