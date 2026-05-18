using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class AREEWindowViewModel : Base
    {
        public AREEWindowViewModel()
        {
        }

        public required AREE Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<ABE>? Suppliers { get; set; }

        public ObservableCollection<AGENTI>? Agents { get; set; }
        public AGENTI? SelectedAgent { get; set; }

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class AREEWindowViewModelDefault : AREEWindowViewModel
    {
        public AREEWindowViewModelDefault()
        {
            Agents = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetAreaManager();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().Update(Data);
        }
    }

    public class AREEWindowViewModelUfp : AREEWindowViewModel
    {
        public AREEWindowViewModelUfp()
        {
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().Update(Data);
        }
    }
}
