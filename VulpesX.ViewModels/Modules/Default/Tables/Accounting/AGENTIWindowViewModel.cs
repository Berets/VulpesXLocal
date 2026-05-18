using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Ufp;
using VulpesX.Services.Tables.CRM;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class AGENTIWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AGENTIWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required AGENTI Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<ABE>? Suppliers { get; set; }

        private ABE? selectedSupplier;
        public ABE? SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; NotifyPropertyChanged(); } }

        public abstract ObservableCollection<GenericIDDescription> CommissionTypes { get; }

        public abstract string? Validate();

        public abstract bool Save();

        //UFP 
        public ObservableCollection<AGEPROVPER>? ExceptionsCausal { get; set; }
        public ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle { get; set; }

        public ObservableCollection<ABE>? CustomersCache { get; set; }
        public ObservableCollection<ABE>? SuppliersCache { get; set; }
        public ObservableCollection<TAB_CRM_CAUORD>? OrderCausalsCache { get; set; }
        public ObservableCollection<tab_articolo>? ArticlesCache { get; set; }
        public ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarcos { get; set; }

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

        public abstract Task LoadArticles();

        public abstract ObservableCollection<AGEPROVPER>? GetAGEPROVPERs();

        public abstract ObservableCollection<AGENTI_SOTTOLIVELLO>? GetAGENTI_SOTTOLIVELLOs();

        public abstract ObservableCollection<TAB_AGENTI_ENASARCO>? GetTAB_AGENTI_ENASARCOs();

        public abstract ObservableCollection<ACC_EINVOICE_HEADS>? GetACC_EINVOICE_HEADs(int Year, int SupplierID);

        public abstract ACC_EINVOICE_HEADS? GetFull(long ID);

        public abstract PNTESTATA? GetPNTESTATA(int Year, int ID);

        public abstract ObservableCollection<ABE>? GetABE(string N1FLCF);

        public abstract ObservableCollection<CAUCONT>? GetCAUCONT(string StateID);

        public abstract string? ValidateCausals(AGEPROVPER Causal);

        public abstract string? ValidateArticles(AGENTI_SOTTOLIVELLO Article);

        public abstract string? ValidateEnasarco(TAB_AGENTI_ENASARCO Enasarco);
    }

    public class AGENTIWindowViewModelDefault : AGENTIWindowViewModel
    {
        public AGENTIWindowViewModelDefault()
        {
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public override ObservableCollection<GenericIDDescription> CommissionTypes => CommonsService.StandardValueTypes;

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Update(Data);
        }

        public override Task LoadArticles()
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<AGEPROVPER>? GetAGEPROVPERs()
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<AGENTI_SOTTOLIVELLO>? GetAGENTI_SOTTOLIVELLOs()
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<TAB_AGENTI_ENASARCO>? GetTAB_AGENTI_ENASARCOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetTAB_AGENTI_ENASARCOs(CompanyID, Data.agecod);
        }

        public override ObservableCollection<ACC_EINVOICE_HEADS>? GetACC_EINVOICE_HEADs(int Year, int SupplierID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetList(CompanyID, Year, SupplierID);
        }

        public override ACC_EINVOICE_HEADS? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetFull(ID);
        }

        public override PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public override ObservableCollection<ABE>? GetABE(string N1FLCF)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(N1FLCF);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONT(string StateID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList(StateID);
        }

        public override string? ValidateCausals(AGEPROVPER Causal)
        {
            throw new NotImplementedException();
        }

        public override string? ValidateArticles(AGENTI_SOTTOLIVELLO Article)
        {
            throw new NotImplementedException();
        }

        public override string? ValidateEnasarco(TAB_AGENTI_ENASARCO Enasarco)
        {
            throw new NotImplementedException();
        }
    }

    public class AGENTIWindowViewModelUfp : AGENTIWindowViewModel
    {
        public AGENTIWindowViewModelUfp()
        {
            SuppliersCache = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
            CustomersCache = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
            OrderCausalsCache = VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUORDRepository>().GetList();

            Suppliers = SuppliersCache;
        }

        public override ObservableCollection<GenericIDDescription> CommissionTypes => CommonsService.StandardValueTypes;

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Insert(CompanyID, Data, ExceptionsCausal, ExceptionsArticle, Enasarcos);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Update(CompanyID, Data, ExceptionsCausal, ExceptionsArticle, Enasarcos);
        }

        public override async Task LoadArticles()
        {
            IsBusy = true;

            await Task.Run(() =>
                       ArticlesCache = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList());

            IsBusy = false;

        }

        public override ObservableCollection<AGEPROVPER>? GetAGEPROVPERs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetAGEPROVPERs(Data.agecod);
        }

        public override ObservableCollection<AGENTI_SOTTOLIVELLO>? GetAGENTI_SOTTOLIVELLOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetAGENTI_SOTTOLIVELLOs(Data.agecod);
        }

        public override ObservableCollection<TAB_AGENTI_ENASARCO>? GetTAB_AGENTI_ENASARCOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetTAB_AGENTI_ENASARCOs(CompanyID, Data.agecod);
        }

        public override ObservableCollection<ACC_EINVOICE_HEADS>? GetACC_EINVOICE_HEADs(int Year, int SupplierID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetList(CompanyID, Year, SupplierID);
        }

        public override ACC_EINVOICE_HEADS? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetFull(ID);
        }

        public override PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public override ObservableCollection<ABE>? GetABE(string N1FLCF)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(N1FLCF);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONT(string StateID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList(StateID);
        }

        public override string? ValidateCausals(AGEPROVPER Causal)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().ValidateCausals((ExceptionsCausal ?? new ObservableCollection<AGEPROVPER>()).ToList(), Causal);
        }

        public override string? ValidateArticles(AGENTI_SOTTOLIVELLO Article)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().ValidateArticles((ExceptionsArticle ?? new ObservableCollection<AGENTI_SOTTOLIVELLO>()).ToList(), Article);
        }

        public override string? ValidateEnasarco(TAB_AGENTI_ENASARCO Enasarco)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().ValidateEnasarco((Enasarcos ?? new ObservableCollection<TAB_AGENTI_ENASARCO>()).ToList(), Enasarco);
        }
    }
}
