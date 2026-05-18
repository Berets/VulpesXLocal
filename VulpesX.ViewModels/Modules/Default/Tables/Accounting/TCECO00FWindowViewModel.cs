using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Auth;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class TCECO00FWindowViewModel : Base
    {
        public required TCECO00F Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription> CostCentersTypes => CommonsService.CostCentersTypes;
        public ObservableCollection<SOCBASE>? Companies { get; set; }

        public ObservableCollection<SOCBASE>? GetSOCBASE()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().GetList(true);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().Update(Data);
        }
    }
}
