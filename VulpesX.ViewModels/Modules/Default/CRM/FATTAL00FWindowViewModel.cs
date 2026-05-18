using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTAL00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public FATTAL00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required FATTT00F Head { get; set; }

        public string? Validate(FATTAL00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTAL00FRepository>().Validate(Item, true);
        }

        public ObservableCollection<FATTAL00F>? GetFATTAL00Fs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTAL00FRepository>().GetList(CompanyID, Head.FTANNO, Head.FTNUOR);
        }

        public bool UpdateAll()
        {
            if (CompanyUID.HasValue)
                return VulpesServiceProvider.Provider.GetRequiredService<IFATTAL00FRepository>().UpdateAll(Head, CompanyUID!.Value);

            return false;
        }
    }
}
