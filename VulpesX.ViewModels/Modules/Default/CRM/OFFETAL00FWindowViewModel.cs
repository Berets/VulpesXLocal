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
    public class OFFETAL00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public OFFETAL00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required OFFET00F Head { get; set; }
        public bool HasMergedSigns { get; set; }
        public bool IsReadonly => Head.canceled.HasValue || Head.oflgchi == "C" || Head.oflgchi == "O";
        public bool IsEnabled => !IsReadonly;

        public string? Validate(OFFETAL00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFETAL00FRepository>().Validate(Item, true);
        }

        public ObservableCollection<OFFETAL00F>? GetOFFETAL00Fs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFETAL00FRepository>().GetList(CompanyID, Head.OFTANNO, Head.OFTNUOR);
        }

        public bool UpdateAll()
        {
            if (CompanyUID.HasValue)
                return VulpesServiceProvider.Provider.GetRequiredService<IOFFETAL00FRepository>().UpdateAll(Head, CompanyUID!.Value);

            return false;
        }
    }
}
