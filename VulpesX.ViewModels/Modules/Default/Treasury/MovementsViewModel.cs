using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.DAL.Treasury;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class MovementsViewModel : Base
    {
        public required string CompanyID { get; set; }
        public MovementsViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
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

        private ObservableCollection<RBCC01F0>? items;
        public ObservableCollection<RBCC01F0>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load(int Year, bool OnlyAtBank)
        {
            IsBusy = true;

            var result = await Task.Run(() =>
            {
                var items = VulpesServiceProvider.Provider.GetRequiredService<ITreasuryRepository>().LoadMovements(CompanyID, Year, Today, OnlyAtBank);
                var trend = VulpesServiceProvider.Provider.GetRequiredService<IRBCC01F0Repository>().GetTotaleDisponibilita(CompanyID);

                return new { items, trend };
            });

            Items = result.items;
            TotaleDisponibilita = result.trend;

            IsBusy = false;

        }

        public DateTime Today => VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
        public string TodayText => Today.Date.ToShortDateString();
        public decimal TotaleDisponibilita { get; set; }
        public decimal TotaleDisponibilitaFutura => Items != null ? Items.Sum(sum => sum.DisponibilitaFutura) : 0;
        public decimal TotaleDisponibilitaPortafoglio => Items != null ? Items.Sum(sum => sum.DisponibilitaPortafoglio) : 0;
        public decimal TotaleImpegniFornitoriNonAssegnati { get; set; }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public bool Update(RBCC01F0 Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IRBCC01F0Repository>().Update(Item);
        }
    }
}
