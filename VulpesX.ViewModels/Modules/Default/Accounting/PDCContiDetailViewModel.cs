using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class PDCContiDetailViewModel : Base
    {
        public abstract required string CompanyID { get; set; }

        public PDCContiDetailViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public PDCCONTI? Data { get; set; }

        public abstract ObservableCollection<GenericIDDescription> CFTypes { get; }
        public abstract ObservableCollection<GenericIDDescription> Sectors { get; }
        public bool IsInsert { get; set; }

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class PDCContiDetailViewModelDefault : PDCContiDetailViewModel
    {
        public override required string CompanyID { get; set; }


        public PDCContiDetailViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override ObservableCollection<GenericIDDescription> CFTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
            new GenericIDDescription(){ ID = "C", Description = "Cliente" },
            new GenericIDDescription(){ ID = "F", Description = "Fornitore" }
        };
        public override ObservableCollection<GenericIDDescription> Sectors => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "BT", Description = "Crediti B.T." },
            new GenericIDDescription(){ ID = "LT", Description = "Crediti L.T." },
            new GenericIDDescription(){ ID = "RI", Description = "Rimanenze" },
            new GenericIDDescription(){ ID = "IM", Description = "Immobilizzazioni" },
            new GenericIDDescription(){ ID = "FO", Description = "Fondi" },
            new GenericIDDescription(){ ID = "TF", Description = "TFR" },
            new GenericIDDescription(){ ID = "PN", Description = "Patrimonio" }
        };

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Validate(Data!, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Insert(Data!);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Update(Data!);
        }
    }

    public class PDCContiDetailViewModelUfp : PDCContiDetailViewModel
    {
        public override required string CompanyID { get; set; }


        public PDCContiDetailViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override ObservableCollection<GenericIDDescription> CFTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
            new GenericIDDescription(){ ID = "C", Description = "Cliente" },
            new GenericIDDescription(){ ID = "F", Description = "Fornitore" }
        };
        public override ObservableCollection<GenericIDDescription> Sectors => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "BT", Description = "Crediti B.T." },
            new GenericIDDescription(){ ID = "LT", Description = "Crediti L.T." },
            new GenericIDDescription(){ ID = "RI", Description = "Rimanenze" },
            new GenericIDDescription(){ ID = "IM", Description = "Immobilizzazioni" },
            new GenericIDDescription(){ ID = "FO", Description = "Fondi" },
            new GenericIDDescription(){ ID = "TF", Description = "TFR" },
            new GenericIDDescription(){ ID = "PN", Description = "Patrimonio" }
        };

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Validate(Data!, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Insert(Data!);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Update(Data!);
        }
    }

}
