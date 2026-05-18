using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Assets;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Assets
{
    public class ASSETAL00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public ASSETAL00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        public required ASSET00F Head { get; set; }
        public bool IsReadonly => Head.canceled != null ? false : true;
        public bool IsEnabled => !IsReadonly;

        public string? Validate(ASSETAL00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETAL00FRepository>().Validate(Item, true);
        }

        public bool UpdateAll()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IASSETAL00FRepository>().UpdateAll(Head, CompanyUID!.Value);

        }
    }
}
