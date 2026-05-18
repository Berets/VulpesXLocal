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
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class AliquoteWindowViewModel : Base
    {
        public AliquoteWindowViewModel()
        {
            Classifications = CommonsService.RateClassifications;
            IVANatures = VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().GetList();
        }

        public required ASSOGGETAMENTI Data { get; set; }

        public ObservableCollection<GenericIDDescription>? Classifications { get; set; }
        public ObservableCollection<FE_IVADOC>? IVANatures { get; set; }
        public FE_IVADOC? SelectedIVANature { get; set; }

        public bool IsInsert { get; set; }
        
        public ObservableCollection<ASSOGGETAMENTI>? GetRatesList()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Update(Data);
        }
    }
}
