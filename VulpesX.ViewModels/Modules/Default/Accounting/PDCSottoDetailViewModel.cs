using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Auth;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class PDCSottoDetailViewModel : Base
    {
        public abstract required string CompanyID { get; set; }

        public PDCSottoDetailViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public PDCSOTTO? Data { get; set; }

        public abstract ObservableCollection<GenericIDDescription> CFTypes { get; }

        public abstract ObservableCollection<GenericIDDescription> OBCPTypes { get; }

        public abstract ObservableCollection<GenericIDDescription> BLTypes { get; }

        public ObservableCollection<SOCBASE>? Companies { get; set; }
        public ObservableCollection<PNCEEBIL>? CEEs { get; set; }
        public ObservableCollection<FILIALI>? Branches { get; set; }

        private ObservableCollection<BANAZIEN>? banks;
        public ObservableCollection<BANAZIEN>? Banks
        {
            get { return banks; }
            set
            {
                banks = value;
                NotifyPropertyChanged("Banks");
            }
        }

        private PNCEEBIL? selectedCEEItem;
        public PNCEEBIL? SelectedCEEItem
        {
            get
            { return selectedCEEItem; }
            set
            {
                selectedCEEItem = value;
                NotifyPropertyChanged("SelectedCEEItem");
            }
        }

        private PNCEEBIL? selectedCEEItemAlt;
        public PNCEEBIL? SelectedCEEItemAlt
        {
            get
            { return selectedCEEItemAlt; }
            set
            {
                selectedCEEItemAlt = value;
                NotifyPropertyChanged("SelectedCEEItemAlt");
            }
        }

        private FILIALI? selectedBranch;
        public FILIALI? SelectedBranch
        {
            get
            { return selectedBranch; }
            set
            {
                selectedBranch = value;
                NotifyPropertyChanged("SelectedBranch");
            }
        }

        private BANAZIEN? selectedBank;
        public BANAZIEN? SelectedBank
        {
            get
            { return selectedBank; }
            set
            {
                selectedBank = value;
                NotifyPropertyChanged("SelectedBank");
            }
        }

        private bool isBankEnabled;
        public bool IsBankEnabled
        {
            get { return isBankEnabled; }
            set
            {
                isBankEnabled = value;
                NotifyPropertyChanged("IsBankEnabled");
                NotifyPropertyChanged("Banks");
            }
        }

        public bool IsInsert { get; set; }


        public abstract ObservableCollection<BANAZIEN>? LoadBanks(string ID);

        public abstract string? Validate();

        public abstract bool Insert();

        public abstract bool Update();

        public abstract int? GetLastExerciseYear();

        public abstract bool InsertPDCANNI(int LastYear);
    }

    public class PDCSottoDetailViewModelDefault : PDCSottoDetailViewModel
    {
        public override required string CompanyID { get; set; }

        public PDCSottoDetailViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;

            Companies = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().GetList(true);
            CEEs = VulpesServiceProvider.Provider.GetRequiredService<IPNCEEBILRepository>().GetList();
            Branches = VulpesServiceProvider.Provider.GetRequiredService<IFILIALIRepository>().GetList(CompanyID);
        }

        public override ObservableCollection<GenericIDDescription> CFTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
            new GenericIDDescription(){ ID = "C", Description = "Cliente" },
            new GenericIDDescription(){ ID = "F", Description = "Fornitore" }
        };

        public override ObservableCollection<GenericIDDescription> OBCPTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "N", Description = "Nessuna" },
            new GenericIDDescription(){ ID = "S", Description = "Sintetica" },
            new GenericIDDescription(){ ID = "A", Description = "Analitica" }
        };

        public override ObservableCollection<GenericIDDescription> BLTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuna" },
            new GenericIDDescription(){ ID = "B", Description = "Beni" },
            new GenericIDDescription(){ ID = "S", Description = "Servizi" }
        };


        public override ObservableCollection<BANAZIEN>? LoadBanks(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(ID);
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Validate(Data!, IsInsert);
        }

        public override bool Insert()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Insert(Data!);
        }

        public override bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Update(Data!);
        }

        public override int? GetLastExerciseYear()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID)?.FirstOrDefault()?.eseann;
        }

        public override bool InsertPDCANNI(int LastYear)
        {
            var group = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Get(Data!.P1GRUP);

            var newYear = new PDCANNI()
            {
                P1SOCI = CompanyID,
                P1GRUP = Data!.P1GRUP,
                P2CONT = Data!.P2CONT,
                P3SOTC = Data!.P3SOTC,
                P1CCHI = group?.p1chco,
                P4ANNO = LastYear
            };

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Insert(newYear);
        }
    }

    public class PDCSottoDetailViewModelUfp : PDCSottoDetailViewModel
    {
        public override required string CompanyID { get; set; }

        public PDCSottoDetailViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;

            Companies = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().GetList(true);
            Branches = VulpesServiceProvider.Provider.GetRequiredService<IFILIALIRepository>().GetList(CompanyID);
        }

        public override ObservableCollection<GenericIDDescription> CFTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuno" },
            new GenericIDDescription(){ ID = "C", Description = "Cliente" },
            new GenericIDDescription(){ ID = "F", Description = "Fornitore" }
        };

        public override ObservableCollection<GenericIDDescription> OBCPTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "N", Description = "Nessuna" },
            new GenericIDDescription(){ ID = "S", Description = "Sintetica" },
            new GenericIDDescription(){ ID = "A", Description = "Analitica" }
        };

        public override ObservableCollection<GenericIDDescription> BLTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = null, Description = "Nessuna" },
            new GenericIDDescription(){ ID = "B", Description = "Beni" },
            new GenericIDDescription(){ ID = "S", Description = "Servizi" }
        };

        public override ObservableCollection<BANAZIEN>? LoadBanks(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(ID);
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Validate(Data!, IsInsert);
        }

        public override bool Insert()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Insert(Data!);
        }

        public override bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Update(Data!);
        }

        public override int? GetLastExerciseYear()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID)?.FirstOrDefault()?.eseann;
        }

        public override bool InsertPDCANNI(int LastYear)
        {
            var group = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Get(Data!.P1GRUP);

            var newYear = new PDCANNI()
            {
                P1SOCI = CompanyID,
                P1GRUP = Data!.P1GRUP,
                P2CONT = Data!.P2CONT,
                P3SOTC = Data!.P3SOTC,
                P1CCHI = group?.p1chco,
                P4ANNO = LastYear
            };

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Insert(newYear);
        }
    }

}
