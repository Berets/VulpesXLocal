using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Models.Ufp;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Ufp.Tables.Accounting
{
    public class INCASSOWindowViewModel
    {
        public INCASSOWindowViewModel()
        {
        }

        public required INCASSO Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IINCASSORepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IINCASSORepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IINCASSORepository>().Update(Data);
            }
        }
    }
}
