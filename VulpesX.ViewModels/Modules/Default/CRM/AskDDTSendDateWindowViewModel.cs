using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.Shipping;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class AskDDTSendDateWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AskDDTSendDateWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required BOLLT00F Data { get; set; }
        public store_causals? StoreCausal { get; set; }

        private CONSEGNA? selectedDelivery;
        public CONSEGNA? SelectedDelivery
        {
            get { return selectedDelivery; }
            set
            {
                selectedDelivery = value;
                NotifyPropertyChanged("SelectedDelivery");
            }
        }

        public ObservableCollection<CONSEGNA>? Deliveries { get; set; }

        private SPEDIZIONE? selectedShipment;
        public SPEDIZIONE? SelectedShipment
        {
            get { return selectedShipment; }
            set
            {
                selectedShipment = value;
                NotifyPropertyChanged("SelectedShipment");
            }
        }

        public ObservableCollection<SPEDIZIONE>? Shipments { get; set; }

        public IMBALLI? SelectedPacking { get; set; }

        public ObservableCollection<IMBALLI>? Packings { get; set; }

        private VETTORI? selectedFirstCarrier;
        public VETTORI? SelectedFirstCarrier
        {
            get { return selectedFirstCarrier; }
            set
            {
                selectedFirstCarrier = value;
                NotifyPropertyChanged("SelectedFirstCarrier");
            }
        }

        private VETTORI? selectedSecondCarrier;
        public VETTORI? SelectedSecondCarrier
        {
            get { return selectedSecondCarrier; }
            set
            {
                selectedSecondCarrier = value;
                NotifyPropertyChanged("SelectedSecondCarrier");
            }
        }
        public ObservableCollection<VETTORI>? Carriers { get; set; }

        public void LoadDetails()
        {
            Deliveries = VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().GetList();
            Shipments = VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().GetList();
            Packings = VulpesServiceProvider.Provider.GetRequiredService<IIMBALLIRepository>().GetList();
            Carriers = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList();
        }

        public CAUSBOLL? GetCAUSBOLL(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().Get(ID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Validate(Data, false);
        }

        public int? GetNumerator(int Year, GenericIDDescription Type)
        {
            var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            return numRegRepo.GetNumber(CompanyID, Year, Type, true);
        }

        public Tuple<decimal, decimal, decimal>? GetRowsTotalQuantity()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().GetRowsTotalQuantity(CompanyID, Data.BTANNO, Data.BTBOLL);
        }

        public BOLLT00F? GetPrintFull(int Year, int ID)
        {
            var companySettings = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            if (companySettings == null)
                return null;

            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GetPrintFull(CompanyID, Year, ID, companySettings.azpnotddt, companySettings.azagedddt, companySettings.AZCUSDDT);
        }

        public DDTReport? PrintDDT(BOLLT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().PrintDDT(Item);

        }

        public bool UpdateDefinitiveDDTAndUnloadEngages()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().UpdateDefinitiveDDTAndUnloadEngages(Data, StoreCausal!, UserID);

        }
    }
}
