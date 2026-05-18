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
using VulpesX.Models.Ufp;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class VETTORIWindowViewModel : Base
    {
        public VETTORIWindowViewModel()
        {
        }

        public required VETTORI Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<COMUNI>? Cities { get; set; }
        public COMUNI? SelectedCity { get; set; }
        public ObservableCollection<TAB_STATES>? States { get; set; }
        public TAB_STATES? SelectedState { get; set; }
        public ObservableCollection<ABE>? Suppliers { get; set; }
        public ABE? SelectedSupplier { get; set; }

        public ObservableCollection<VETTORISPESE1>? Expenses { get; set; }
        public ObservableCollection<ISO>? ISOCache { get; set; }

        public abstract ObservableCollection<VETTORISPESE1>? GetVETTORISPESE1();

        public abstract string? ValidateExpense(VETTORISPESE1 Expense);

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class VETTORIWindowViewModelDefault : VETTORIWindowViewModel
    {
        public VETTORIWindowViewModelDefault()
        {
            Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public override ObservableCollection<VETTORISPESE1>? GetVETTORISPESE1()
        {
            throw new NotImplementedException();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().Validate(Data, IsInsert);
        }

        public override string? ValidateExpense(VETTORISPESE1 Expense)
        {
            throw new NotImplementedException();
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().Update(Data);
        }
    }

    public class VETTORIWindowViewModelUfp : VETTORIWindowViewModel
    {
        public VETTORIWindowViewModelUfp()
        {
            Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");

            ISOCache = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
        }

        public override ObservableCollection<VETTORISPESE1>? GetVETTORISPESE1()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetExpenses(Data.vetcod);
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().Validate(Data, IsInsert);
        }

        public override string? ValidateExpense(VETTORISPESE1 Expense)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().ValidateExpenses(Expenses?.ToList(), Expense);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().Insert(Data, Expenses);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().Update(Data, Expenses);
        }
    }

}
