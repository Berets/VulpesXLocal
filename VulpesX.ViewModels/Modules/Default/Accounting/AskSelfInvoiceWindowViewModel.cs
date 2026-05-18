using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class AskSelfInvoiceWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public AskSelfInvoiceWindowViewModel()
        {
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private DateTime? selectedDate;
        public DateTime? SelectedDate { get { return selectedDate; } set { selectedDate = value; NotifyPropertyChanged(); } }

        public required PNTESTATA Head { get; set; }
        public ESERCIZIO? AccountingYear { get; set; }

        private CAUFAT00F? causal;
        public CAUFAT00F? Causal { get { return causal; } set { causal = value; NotifyPropertyChanged(); } }

        private ObservableCollection<CAUFAT00F>? causals;
        public ObservableCollection<CAUFAT00F>? Causals { get { return causals; } set { causals = value; NotifyPropertyChanged(); } }

        private ABE? customer;
        public ABE? Customer { get { return customer; } set { customer = value; NotifyPropertyChanged(); } }

        private ObservableCollection<ABE>? customers;
        public ObservableCollection<ABE>? Customers { get { return customers; } set { customers = value; NotifyPropertyChanged(); } }

        private tab_articolo? product;
        public tab_articolo? Product { get { return product; } set { product = value; NotifyPropertyChanged(); } }

        private ObservableCollection<tab_articolo>? products;
        public ObservableCollection<tab_articolo>? Products { get { return products; } set { products = value; NotifyPropertyChanged(); } }

        public abstract Task LoadDetails();

        public abstract ObservableCollection<ESERCIZIO>? GetESERCIZIO();

        public abstract string? Generate();
    }

    public class AskSelfInvoiceWindowViewModelDefault : AskSelfInvoiceWindowViewModel
    {
        public AskSelfInvoiceWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override async Task LoadDetails()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var selectedDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();
                    var customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
                    var products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
                    var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

                    return new { selectedDate, causals, customers, products, azienda };
                });

                SelectedDate = result.selectedDate;
                Causals = result.causals;
                Customers = result.customers;
                Products = result.products;

                Causal = Causals?.Where(o => o.fatcod == Head.AccountingCausal?.cauaut).FirstOrDefault();
                Customer = Customers?.Where(o => o.abecod == result.azienda?.azcanu).FirstOrDefault();
                Product = Products?.Where(o => o.ID?.Trim() == result.azienda?.azartautfat).FirstOrDefault();
            }
            finally
            {
                IsBusy = false;
            }
        }


        public override ObservableCollection<ESERCIZIO>? GetESERCIZIO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public override string? Generate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().GenerateSelfInvoice(Head, AccountingYear!, Customer!, Causal!, Product!, SelectedDate!.Value, UserID);
        }
    }

    public class AskSelfInvoiceWindowViewModelUfp : AskSelfInvoiceWindowViewModel
    {
        public AskSelfInvoiceWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }


        public override async Task LoadDetails()
        {
            IsBusy = true;

            try
            {
               var result =  await Task.Run(() =>
                {
                    var selectedDate = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();
                    var customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
                    var products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList();
                    var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

                    return new { selectedDate, causals, customers, products, azienda };
                });

                SelectedDate = result.selectedDate;
                Causals = result.causals;
                Customers = result.customers;
                Products = result.products;

                Causal = Causals?.Where(o => o.fatcod == Head.AccountingCausal?.cauaut).FirstOrDefault();
                Customer = Customers?.Where(o => o.abecod == result.azienda?.azcanu).FirstOrDefault();
                Product = Products?.Where(o => o.ID?.Trim() == result.azienda?.azartautfat).FirstOrDefault();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override ObservableCollection<ESERCIZIO>? GetESERCIZIO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public override string? Generate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().GenerateSelfInvoice(Head, AccountingYear!, Customer!, Causal!, Product!, SelectedDate!.Value, UserID);
        }
    }

}
