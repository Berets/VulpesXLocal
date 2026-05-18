using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Assets;
using VulpesX.DAL.Tables.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Assets
{
    public class ASSETCO00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public ASSETCO00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required ASSET00F Head { get; set; }
        public ObservableCollection<TAB_GEN_CONTACTS_ROLES>? ContactRoles { get; set; }
        public ObservableCollection<TAB_GEN_CONTACTS_TYPES>? ContactTypes { get; set; }
        public bool IsReadonly => Head.canceled != null ? false : true;
        public bool IsEnabled => !IsReadonly;

        public void LoadDetails()
        {
            ContactRoles = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_CONTACTS_ROLESRepository>().GetList();
            ContactTypes = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_CONTACTS_TYPESRepository>().GetList();
        }

        public string? Validate(ASSETCO00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETCO00FRepository>().Validate(Item, true);
        }

        public string? Validate(ASSETCODET00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETCODET00FRepository>().Validate(Item, true);
        }

        public bool UpdateAll()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETCO00FRepository>().UpdateAll(Head);

        }
    }
}
