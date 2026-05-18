using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreMovementsCheckWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreMovementsCheckWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        private bool _isBusyLots;
        public bool IsBusyLots
        {
            get { return _isBusyLots; }
            set
            {
                _isBusyLots = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusyNoLots;
        public bool IsBusyNoLots
        {
            get { return _isBusyNoLots; }
            set
            {
                _isBusyNoLots = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusyMovLots;
        public bool IsBusyMovLots
        {
            get { return _isBusyMovLots; }
            set
            {
                _isBusyMovLots = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusyMovNoLots;
        public bool IsBusyMovNoLots
        {
            get { return _isBusyMovNoLots; }
            set
            {
                _isBusyMovNoLots = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<StockCheckExistance.NoLots>? noLots;
        public ObservableCollection<StockCheckExistance.NoLots>? NoLots { get => noLots; set { noLots = value; NotifyPropertyChanged("NoLots"); } }


        private ObservableCollection<StockCheckExistance.Lots>? lots;
        public ObservableCollection<StockCheckExistance.Lots>? Lots { get => lots; set { lots = value; NotifyPropertyChanged("Lots"); } }


        private ObservableCollection<store_movements>? movementsLot;
        public ObservableCollection<store_movements>? MovementsLot { get => movementsLot; set { movementsLot = value; NotifyPropertyChanged("MovementsLot"); } }


        private ObservableCollection<store_movements>? movementsNoLot;
        public ObservableCollection<store_movements>? MovementsNoLot { get => movementsNoLot; set { movementsNoLot = value; NotifyPropertyChanged("MovementsNoLot"); } }

        public async Task LoadDataLots()
        {
            IsBusyLots = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().CheckExistanceLots(CompanyID);
                });

                Lots = result;
            }
            finally
            {
                IsBusyLots = false;
            }
        }

        public async Task LoadDataNoLots()
        {
            IsBusyNoLots = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().CheckExistanceNoLots(CompanyID);
                });

                NoLots = result;
            }
            finally
            {
                IsBusyNoLots = false;
            }
        }

        public async Task LoadMovementsLots(string ProductID, string StoreID, string Lot)
        {
            IsBusyMovLots = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GetList(CompanyID, ProductID, StoreID, Lot);
                });

                MovementsLot = result;
            }
            finally
            {
                IsBusyMovLots = false;
            }
        }

        public async Task LoadMovementsNoLots(string ProductID, string StoreID)
        {
            IsBusyMovNoLots = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GetList(CompanyID, ProductID, StoreID, string.Empty);
                });

                MovementsNoLot = result;
            }
            finally
            {
                IsBusyMovNoLots = false;
            }
        }

        public bool Align(string ProductID, string StoreID, string Lot)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>().Align(CompanyID, StoreID, ProductID, Lot);
        }

        public tab_articolo? GetArticolo(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, ProductID);
        }
    }
}
