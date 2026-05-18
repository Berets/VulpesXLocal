using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class ACC_PLAFONDWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACC_PLAFONDWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public bool IsReadonly => !Data.clidatchi.HasValue;
        public required ACC_PLAFOND Data { get; set; }
        public bool IsInsert { get; set; }

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

        public ObservableCollection<ABE>? Customers { get; set; }

        private ObservableCollection<ACC_PLAFOND_ROWS>? details;
        public ObservableCollection<ACC_PLAFOND_ROWS>? Details
        {
            get => details;
            set
            {
                details = value;
                NotifyPropertyChanged("Details");
            }
        }

        public abstract Task Load();

        public abstract string? Validate();

        public abstract string? Save();

        public abstract string? GetInvoiceArchived(short Year, int DefinitiveID);
    }

    public class ACC_PLAFONDWindowViewModelDefault : ACC_PLAFONDWindowViewModel
    {
        public ACC_PLAFONDWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;

            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");

        }

       
        public override async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>().GetList(Data.Cliasoc, Data.Cliacod, Data.cliannosol, Data.cliprog);

                });

                Details = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().Validate(Data, IsInsert);
        }

        public override string? Save()
        {
            var accPlafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
            if (!IsInsert)
            {
                Data.updatedUserID = UserID;
                if (accPlafondRepo.Update(Data))
                {
                    return null;
                }
                else
                {
                    return "Aggiornamento plafond fallito";
                }
            }
            else
            {
                var newProg = accPlafondRepo.GetLastProgressive(Data.Cliasoc, Data.Cliacod, Data.cliannosol) + 1;
                if (newProg > 0)
                {
                    Data.cliprog = newProg;
                    Data.addedUserID = UserID;
                    if (accPlafondRepo.Insert(Data))
                    {
                        return null;
                    }
                    else
                    {
                        return "Inserimento plafond fallito";
                    }
                }
                else
                {
                    return "Impossibile inserire una nuova dichiarazione d'intento, nessun progressivo trovato";
                }
            }
        }

        public override string? GetInvoiceArchived(short Year, int DefinitiveID)
        {
            throw new NotImplementedException();
        }
    }

    public class ACC_PLAFONDWindowViewModelUfp : ACC_PLAFONDWindowViewModel
    {
        public ACC_PLAFONDWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;

            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");

        }


        public override async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>().GetList(Data.Cliasoc, Data.Cliacod, Data.cliannosol, Data.cliprog);

                });

                Details = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().Validate(Data, IsInsert);
        }

        public override string? Save()
        {
            var accPlafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
            if (!IsInsert)
            {
                Data.updatedUserID = UserID;
                if (accPlafondRepo.Update(Data))
                {
                    return null;
                }
                else
                {
                    return "Aggiornamento plafond fallito";
                }
            }
            else
            {
                var newProg = accPlafondRepo.GetLastProgressive(Data.Cliasoc, Data.Cliacod, Data.cliannosol) + 1;
                if (newProg > 0)
                {
                    Data.cliprog = newProg;
                    Data.addedUserID = UserID;
                    if (accPlafondRepo.Insert(Data))
                    {
                        return null;
                    }
                    else
                    {
                        return "Inserimento plafond fallito";
                    }
                }
                else
                {
                    return "Impossibile inserire una nuova dichiarazione d'intento, nessun progressivo trovato";
                }
            }
        }

        public override string? GetInvoiceArchived(short Year, int DefinitiveID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>().GetInvoiceArchived(CompanyID, Data.Cliacod, Year, DefinitiveID);
        }
    }

}
