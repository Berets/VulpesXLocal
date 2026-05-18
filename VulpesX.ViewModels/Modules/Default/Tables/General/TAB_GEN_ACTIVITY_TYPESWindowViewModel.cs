using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.General
{
    public class TAB_GEN_ACTIVITY_TYPESWindowViewModel : Base
    {
        public required TAB_GEN_ACTIVITY_TYPES Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_ACTIVITY_TYPESRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_ACTIVITY_TYPESRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_ACTIVITY_TYPESRepository>().Update(Data);
        }
    }
}
