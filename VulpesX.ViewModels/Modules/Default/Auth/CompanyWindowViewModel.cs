using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.General;
using VulpesX.DAL.Tables.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Auth
{
    public abstract class CompanyWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public CompanyWindowViewModel()
        {
        }

        public required SOCBASE Data { get; set; }
        public AZIENDA? Details { get; set; }
        public bool IsDetailsInsert { get; set; }

        public ObservableCollection<COMUNI>? Cities { get; set; }
        public ObservableCollection<TAB_STATES>? States { get; set; }

        private ObservableCollection<ACCESS>? users;
        public ObservableCollection<ACCESS>? Users
        {
            get { return users; }
            set
            {
                users = value;
                NotifyPropertyChanged("Users");
            }
        }

        private ACCESS? selectedUser;
        public ACCESS? SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                NotifyPropertyChanged("SelectedUser");
            }
        }

        private ObservableCollection<MenuModel>? menus;
        public ObservableCollection<MenuModel>? Menus
        {
            get { return menus; }
            set
            {
                menus = value;
                NotifyPropertyChanged("Menus");
            }
        }

        private  AUTH_ACCESS_ROLES userRoles = null!;
        public required AUTH_ACCESS_ROLES UserRoles
        {
            get { return userRoles; }
            set
            {
                userRoles = value;
                NotifyPropertyChanged("UserRoles");
            }
        }

        public COMUNI? SelectedLegalCity { get; set; }
        public TAB_STATES? SelectedLegalState { get; set; }
        public COMUNI? SelectedWorkingCity { get; set; }
        public TAB_STATES? SelectedWorkingState { get; set; }

        private string? lotPreview;
        public string? LotPreview
        {
            get => lotPreview;
            set
            {
                lotPreview = value;
                NotifyPropertyChanged("LotPreview");
                NotifyPropertyChanged("LotPreviewColor");
            }
        }
        public string? LotPreviewColor
        {
            get
            {
                return lotPreview != null && lotPreview.Length <= 50 ? "G" : "R";
            }
        }

        public ObservableCollection<GenericIDDescription> ShareholderTypes => CommonsService.ShareholderTypes;
        public ObservableCollection<GenericIDDescription> LiquidationStatuses => CommonsService.LiquidationStatuses;
        public ObservableCollection<ISO>? ISOs { get; set; }
        public ISO? SelectedISOExtraCEE { get; set; }
        public ObservableCollection<FE_RFIDOC>? TaxRegimeList { get; set; }
        public FE_RFIDOC? SelectedTaxRegime { get; set; }

        private byte[]? logoData;
        public byte[]? LogoData { get => logoData; set { logoData = value; NotifyPropertyChanged("LogoData"); } }
        private byte[]? certsLogoData;
        public byte[]? CertsLogoData { get => certsLogoData; set { certsLogoData = value; NotifyPropertyChanged("CertsLogoData"); } }

        public ObservableCollection<LINGUA>? LingueDefault { get; set; }
        private LINGUA? selectedLinguaDefault;
        public LINGUA? SelectedLinguaDefault { get { return selectedLinguaDefault; } set { selectedLinguaDefault = value; NotifyPropertyChanged("SelectedLinguaDefault"); } }

        private ObservableCollection<LINGUA>? lingue;
        public ObservableCollection<LINGUA>? Lingue { get { return lingue; } set { lingue = value; NotifyPropertyChanged("Lingue"); } }
        private LINGUA? selectedLingua;
        public LINGUA? SelectedLingua { get { return selectedLingua; } set { selectedLingua = value; NotifyPropertyChanged("SelectedLingua"); } }

        private AZIENDA_LINGUA? selectedAziendaLingua;

        public AZIENDA_LINGUA? SelectedAziendaLingua
        {
            get { return selectedAziendaLingua; }
            set
            {
                selectedAziendaLingua = value;
                NotifyPropertyChanged("SelectedAziendaLingua");
            }
        }

        public abstract void LoadDetails();

        public abstract string? Validate();

        public abstract string? ValidateDetails();

        public abstract bool Update();

        public abstract bool InsertAzienda();

        public abstract bool UpdateAzienda();

        public abstract string? GenerateLotID(string Template);

        public abstract bool UpdateRole(AUTH_ACCESS_ROLES Item);

        public abstract void SaveLingua();

        public abstract AZIENDA_LINGUA? GetAZIENDA_LINGUA(string ID);

        public abstract ObservableCollection<LINGUA>? GetLINGUAs();

        public abstract void LoadUserMenu();

        public abstract void UpdateUserMenu(string MenuJSON);
    }

    public class CompanyWindowViewModelDefault : CompanyWindowViewModel
    {
        public CompanyWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public override void LoadDetails()
        {
            Data = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(CompanyID) ?? Data;
            Details = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            if (Details == null)
            {
                IsDetailsInsert = true;
                Details = new AZIENDA() { AZCode = CompanyID, azrssl = string.Empty, };
            }

            Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
            ISOs = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
            TaxRegimeList = VulpesServiceProvider.Provider.GetRequiredService<IFE_RFIDOCRepository>().GetList();

            Details.DDTCausals = VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().GetList("C");
            Details.Rates = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            Details.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Details.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Details.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();

            Users = VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().GetUsers(CompanyID);
            LingueDefault = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Validate(Data, false);
        }

        public override string? ValidateDetails()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Validate(Details!, false);
        }

        public override bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Update(Data);
        }

        public override bool InsertAzienda()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Insert(Details!);
        }

        public override bool UpdateAzienda()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Update(Details!);
        }

        public override string? GenerateLotID(string Template)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GenerateLotID(Template, new Random().Next(999999), new Random().Next(999999), 30);
        }

        public override bool UpdateRole(AUTH_ACCESS_ROLES Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>().Update(Item);
        }

        public override void SaveLingua()
        {

            if (SelectedAziendaLingua != null)
            {
                var linguaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>();

                if (linguaRepo.Get(CompanyID, SelectedAziendaLingua.lincod) == null)
                    linguaRepo.Insert(SelectedAziendaLingua);
                else
                    linguaRepo.Update(SelectedAziendaLingua);
            }
        }

        public override AZIENDA_LINGUA? GetAZIENDA_LINGUA(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(CompanyID, ID);
        }

        public override ObservableCollection<LINGUA>? GetLINGUAs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList();
        }

        public override void LoadUserMenu()
        {
            Menus = VulpesServiceProvider.Provider.GetRequiredService<IMenuRepository>().GetFull(CompanyID, SelectedUser!.USROLD, true);
            UserRoles = VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>().Get(CompanyID, SelectedUser!.USRID) ?? new AUTH_ACCESS_ROLES
            {
                companyID = CompanyID,
                userID = SelectedUser!.USRID,
            };
        }

        public override void UpdateUserMenu(string MenuJSON)
        {
            VulpesServiceProvider.Provider.GetRequiredService<IMenuRepository>().UpdateMenuRole(CompanyID, SelectedUser!, MenuJSON, UserRoles);
        }
    }

    public class CompanyWindowViewModelUfp : CompanyWindowViewModel
    {
        public CompanyWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public override void LoadDetails()
        {
            Data = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(CompanyID) ?? Data;
            Details = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            if (Details == null)
            {
                IsDetailsInsert = true;
                Details = new AZIENDA() { AZCode = CompanyID, azrssl = string.Empty, };
            }

            Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
            ISOs = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
            TaxRegimeList = VulpesServiceProvider.Provider.GetRequiredService<IFE_RFIDOCRepository>().GetList();

            Details.DDTCausals = VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().GetList("C");
            Details.Rates = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            Details.AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            Details.SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            Details.GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();

            Users = VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().GetUsers(CompanyID);
            LingueDefault = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Validate(Data, false);
        }

        public override string? ValidateDetails()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Validate(Details!, false);
        }

        public override bool Update()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Update(Data);
        }

        public override bool InsertAzienda()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Insert(Details!);
        }

        public override bool UpdateAzienda()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Update(Details!);
        }

        public override string? GenerateLotID(string Template)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GenerateLotID(Template, new Random().Next(999999), new Random().Next(999999), 30);
        }

        public override bool UpdateRole(AUTH_ACCESS_ROLES Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>().Update(Item);
        }

        public override void SaveLingua()
        {

            if (SelectedAziendaLingua != null)
            {
                var linguaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>();

                if (linguaRepo.Get(CompanyID, SelectedAziendaLingua.lincod) == null)
                    linguaRepo.Insert(SelectedAziendaLingua);
                else
                    linguaRepo.Update(SelectedAziendaLingua);
            }
        }

        public override AZIENDA_LINGUA? GetAZIENDA_LINGUA(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(CompanyID, ID);
        }

        public override ObservableCollection<LINGUA>? GetLINGUAs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList();
        }

        public override void LoadUserMenu()
        {
            Menus = VulpesServiceProvider.Provider.GetRequiredService<IMenuRepository>().GetFull(CompanyID, SelectedUser!.USRID, true);
            UserRoles = VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>().Get(CompanyID, SelectedUser!.USRID) ?? new AUTH_ACCESS_ROLES
            {
                companyID = CompanyID,
                userID = SelectedUser!.USRID,
            };
        }

        public override void UpdateUserMenu(string MenuJSON)
        {
            VulpesServiceProvider.Provider.GetRequiredService<IMenuRepository>().UpdateMenuRole(CompanyID, SelectedUser!, MenuJSON, UserRoles);
        }
    }
}
