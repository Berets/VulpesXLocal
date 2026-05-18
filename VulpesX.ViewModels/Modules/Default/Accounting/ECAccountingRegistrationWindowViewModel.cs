using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class ECAccountingRegistrationWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ECAccountingRegistrationWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }


        public required string EntityType { get; set; }
        public required ABE Entity { get; set; }

        public int? AccountingYear { get; set; }

        public int? BankABI { get; set; }
        public int? BankCAB { get; set; }
        public string? BankCC { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string? DocumentID { get; set; }

        public short? RegistrationYear { get; set; }
        public int? RegistrationNumber { get; set; }

        public ObservableCollection<CAUCONT>? Causals { get; set; }

        private CAUCONT? selectedCausal;
        public CAUCONT? SelectedCausal { get => selectedCausal; set { selectedCausal = value; NotifyPropertyChanged("SelectedCausal"); } }

        public ObservableCollection<BANAZIEN>? Banks { get; set; }

        private BANAZIEN? selectedBank;
        public BANAZIEN? SelectedBank { get => selectedBank; set { selectedBank = value; NotifyPropertyChanged("SelectedBank"); } }


        private ObservableCollection<MastrinoECReportItem>? items;
        public ObservableCollection<MastrinoECReportItem>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); } }

        public ObservableCollection<GenericIDDescription> Types { get; set; } = new ObservableCollection<GenericIDDescription>
        {
            new GenericIDDescription(){ ID = "S", Description = "Saldo" },
            new GenericIDDescription(){ ID = "A", Description = "Abbuono" },
            new GenericIDDescription(){ ID = "C", Description = "Acconto" }
        };

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ObservableCollection<CAUCONT>? GetCAUCONTs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetList();
        }

        public ObservableCollection<BANAZIEN>? GetBANAZIENs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(CompanyID);
        }

        public string? Validation()
        {
            if (!AccountingYear.HasValue)
                return "Selezionare un esercizio valido";
            if (!Date.HasValue)
                return "Inserire la data di registrazione";
            if (SelectedCausal == null)
                return "Selezionare una causale contabile";
            if (SelectedBank == null)
                return "Selezionare una banca interna";
            if (!(Items ?? new ObservableCollection<MastrinoECReportItem>()).Any())
                return "Almeno una riga di registrazione";

            return null;
        }

        public string? Save()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().RegistrationAccounting(CompanyID, AccountingYear!.Value, Date!.Value, Entity, EntityType, SelectedCausal!, SelectedBank!, DocumentID, DocumentDate, Items ?? new ObservableCollection<MastrinoECReportItem>(), UserID);
        }
    }
}
