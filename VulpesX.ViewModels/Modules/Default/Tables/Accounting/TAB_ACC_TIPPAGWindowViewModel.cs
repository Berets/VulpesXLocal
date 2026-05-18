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
    public class TAB_ACC_TIPPAGWindowViewModel
    {
        public TAB_ACC_TIPPAGWindowViewModel()
        {
        }

        public required TAB_ACC_TIPPAG Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().Update(Data);
            }
        }
    }
}
