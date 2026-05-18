using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
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
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.IVA
{
    public class PNIVAViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public PNIVAViewModel()
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

        private ObservableCollection<PNIVA>? items;
        public ObservableCollection<PNIVA>? Items { get { return items; } set { items = value; NotifyPropertyChanged("Items"); } }

        private LIBRIIVA? selectedIVABook;
        public LIBRIIVA? SelectedIVABook
        {
            get => selectedIVABook;
            set
            {
                selectedIVABook = value;
                NotifyPropertyChanged("SelectedIVABook");
            }
        }

        public ObservableCollection<LIBRIIVA>? IVABooks { get; set; }

        private GenericIDDescription? selectedPrintedStatus;
        public GenericIDDescription? SelectedPrintedStatus
        {
            get => selectedPrintedStatus;
            set
            {
                selectedPrintedStatus = value;
                NotifyPropertyChanged("PrintedStatus");
            }
        }
        public ObservableCollection<GenericIDDescription> PrintedStatuses => CommonsService.PrintedStatusTypes;

        private DateTime? sinceDate;
        private DateTime? untilDate;
        private DateTime? expireDate;
        public DateTime? SinceDate
        {
            get => sinceDate;
            set
            {
                sinceDate = value;
                NotifyPropertyChanged("SinceDate");
            }
        }
        public DateTime? UntilDate
        {
            get => untilDate;
            set
            {
                untilDate = value;
                NotifyPropertyChanged("UntilDate");
            }
        }
        public DateTime? ExpireDate
        {
            get => expireDate;
            set
            {
                expireDate = value;
                NotifyPropertyChanged("ExpireDate");
            }
        }

        private bool onlyUnpaid;
        public bool OnlyUnpaid
        {
            get => onlyUnpaid;
            set
            {
                onlyUnpaid = value;
                NotifyPropertyChanged("OnlyUnpaid");
            }
        }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                if (SelectedIVABook != null && SelectedPrintedStatus != null)
                {
                    var accountingRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>();

                    var result = await Task.Run(() =>
                    {
                        var items = accountingRepo.GetList(CompanyID, SelectedIVABook!.livcod, SelectedPrintedStatus!.ID!, SinceDate, UntilDate, ExpireDate, OnlyUnpaid);

                        return new { items };
                    });

                    Items = result.items;
                }
                else
                {
                    Items = new ObservableCollection<PNIVA>();
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool Update(PNIVA Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().Update(Item);
        }

        public ObservableCollection<LIBRIIVA>? GetLIBRIIVAs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetList();
        }
    }
}
