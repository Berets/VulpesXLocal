using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.General
{
    public class TAB_GEN_CONTACTS_TYPESWindowViewModel : Base
    {
        public required TAB_GEN_CONTACTS_TYPES Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_CONTACTS_TYPESRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_CONTACTS_TYPESRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_CONTACTS_TYPESRepository>().Update(Data);
        }
    }
}
