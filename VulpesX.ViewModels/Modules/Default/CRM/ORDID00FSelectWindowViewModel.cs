using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class ORDID00FSelectWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public ORDID00FSelectWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required string FlagTarget { get; set; } //  D = DDT | I = Invoice
        public required List<ORDIT00F> OrdersHeads { get; set; }
        public ORDIT00F FirstHead => OrdersHeads.First();
        public ObservableCollection<ORDID00F>? AvailableRows { get; set; }

        public bool GenerateByOrder(List<ORDID00F> SelectedRows, string HeadNotes, string FootNotes)
        {
            if (FlagTarget == "D")
            {
                // create DDT
                return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GenerateByOrder(FirstHead, SelectedRows, UserID, HeadNotes, FootNotes);
            }
            else if (FlagTarget == "I")
            {
                // create invoice
                return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GenerateByOrder(FirstHead, SelectedRows, UserID, HeadNotes, FootNotes, "F");
            }
            else
            {
                return false;
            }
        }

        public bool FlagFulfillment()
        {
            foreach (var ord in OrdersHeads)
            {
                VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().FlagFulfillment(ord.otsoci, ord.OTANNO, ord.OTNUOR, UserID);
            }

            return true;
        }
    }
}
