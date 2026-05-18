using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class AFFIDABILITAWindowViewModel : Base
    {
        public required AFFIDABILITA Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAFFIDABILITARepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IAFFIDABILITARepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IAFFIDABILITARepository>().Update(Data);
        }
    }
}
