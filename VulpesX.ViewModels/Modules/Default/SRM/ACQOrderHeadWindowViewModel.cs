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
using VulpesX.DAL.SRM;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Services.Tables.General;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.SRM
{
    public class ACQOrderHeadWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ACQOrderHeadWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required acq_orders_heads Data { get; set; }
        public bool IsInsert { get; set; }
        public bool IsReadonly { get; set; }
        public bool HasMergedSigns { get; set; }
        public ObservableCollection<TAB_GEN_TEXTS>? Texts { get; set; }

        private TAB_GEN_TEXTS? selectedHeadText;
        public TAB_GEN_TEXTS? SelectedHeadText { get => selectedHeadText; set { selectedHeadText = value; NotifyPropertyChanged("SelectedHeadText"); } }

        public void LoadDetails()
        {
            Data.Payments = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().GetList();
            Data.Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
            Data.Shipments = VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().GetList();
            Data.Deliveries = VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().GetList();
            Data.use_supplier_codes = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.AZSUPORD ?? false;
            Texts = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_TEXTSRepository>().GetList(CompanyID, TAB_GEN_TEXTS.PURCHASE_ORDERS);
        }

        public ObservableCollection<BankItem>? GetSimpleList()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
        }

        public ObservableCollection<BankItem>? GetSimpleListSuppliers(string PaymentType)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleListSuppliers(CompanyID, PaymentType);
        }

        public FORNAMMI? GetSupplier(int SupplierID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, SupplierID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                Data.addedUserID = UserID;
                Data.id  = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(CompanyID, now.Year, Constants.BUY_ORDERS, true));

                return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().Update(Data);
            }
        }
    }
}
