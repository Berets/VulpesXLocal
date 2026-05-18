using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class SOLLECITIWindowViewModel
    {
        public SOLLECITIWindowViewModel()
        {
        }

        public required SOLLECITI Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISOLLECITIRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ISOLLECITIRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ISOLLECITIRepository>().Update(Data);
            }
        }
    }
}
