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
    public class ORDITALEWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public ORDITALEWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required ORDIT00F Head { get; set; }

        public ObservableCollection<ORDITALE>? Alerts { get; set; }
        public bool HasMergedSigns { get; set; }
        public bool IsReadonly => Head.canceled.HasValue || Head.flgchi == "E";
        public bool IsEnabled => !IsReadonly;

        public string? Validate(ORDITALE Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDITALERepository>().Validate(Item, true);
        }

        public ObservableCollection<ORDITALE>? GetList()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDITALERepository>().GetList(CompanyID, Head.OTANNO, Head.OTNUOR);
        }

        public bool UpdateAll()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDITALERepository>().UpdateAll(Head, Alerts ?? new());
        }
    }
}
