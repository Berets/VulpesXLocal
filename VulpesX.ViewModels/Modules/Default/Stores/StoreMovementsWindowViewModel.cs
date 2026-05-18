using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Stores
{
    public class StoreMovementsWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public StoreMovementsWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required store_movements Data { get; set; }
        public bool IsInsert { get; set; }
        public bool IsLotLocked { get; set; }
        public bool IsFixedProduct { get; set; }
        public bool IsProductEnabled => IsInsert && !IsFixedProduct;

        public ObservableCollection<store_stores>? Stores { get; set; }

        private store_stores? selectedStore;
        public store_stores? SelectedStore
        {
            get => selectedStore;
            set
            {
                selectedStore = value;
                NotifyPropertyChanged("SelectedStore");
            }
        }

        public ObservableCollection<store_causals>? Causals { get; set; }

        public ObservableCollection<tab_articolo>? Products { get; set; }

        public ObservableCollection<ABE>? Suppliers { get; set; }

        public ObservableCollection<string>? GoodsLocations { get; set; }

        public void LoadDetails()
        {
            IsLotLocked = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.lot_locked ?? true;

            Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID);

            Causals = VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().GetList(CompanyID);

            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);

            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");

            GoodsLocations = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GetGoodsLocationsList(CompanyID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().Validate(Data, IsInsert);
        }

        public string? GenerateLotID()
        {
            var lotTemplate = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().GetLotTemplate(CompanyID);
            var progressive = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime().Year, Constants.GOODS_LOTS, true);

            if (Data.Product != null)
                return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GenerateLotID(lotTemplate, Data.supplier_id ?? 0, progressive, Data.Product.ExpireDays ?? 0);

            return null;
        }

        public string? GetDefaultStoreID()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetDefaultStoreID(CompanyID, Data.product_id);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.add_user = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().Insert(Data);
            }
            else
            {
                Data.update_user = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().Update(Data);
            }
        }
    }
}
