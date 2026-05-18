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
    public class CAMBIWindowViewModel : Base
    {
        public required CAMBI Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAMBIRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ICAMBIRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ICAMBIRepository>().Update(Data);
        }
    }
}
