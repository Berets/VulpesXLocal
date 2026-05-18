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
    public class ABICABWindowViewModel : Base
    {
        public ABICABWindowViewModel()
        {
            Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
        }

        public required ABICAB Data { get; set; }

        public bool IsInsert { get; set; }

        public bool IsInsertVisibility => IsInsert ? true : false;
        public bool IsUpdateVisibility => !IsInsert ? true : false;

        public ObservableCollection<COMUNI>? Cities { get; set; }
        public COMUNI? SelectedCity { get; set; }
        public ObservableCollection<TAB_STATES>? States { get; set; }
        public TAB_STATES? SelectedState { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Update(Data);
            }
        }
    }
}
