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
    public abstract class TAB_ACC_TIPINCWindowViewModel : Base
    {
        public TAB_ACC_TIPINCWindowViewModel()
        {
        }

        public required TAB_ACC_TIPINC Data { get; set; }
        public bool IsInsert { get; set; }
        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public CAUCONT? SelectedCausal { get; set; }
        public ObservableCollection<FE_PAGDOC>? FEs { get; set; }
        public FE_PAGDOC? SelectedFE { get; set; }

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class TAB_ACC_TIPINCWindowViewModelDefault : TAB_ACC_TIPINCWindowViewModel
    {
        public TAB_ACC_TIPINCWindowViewModelDefault()
        {
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
            FEs = VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Update(Data);
            }
        }
    }

    public class TAB_ACC_TIPINCWindowViewModelUfp : TAB_ACC_TIPINCWindowViewModel
    {
        public TAB_ACC_TIPINCWindowViewModelUfp()
        {
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
            FEs = VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Insert(Data);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Update(Data);
            }
        }
    }
}
