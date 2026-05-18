using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.Models;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class AskInvoiceFinalDateWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public AskInvoiceFinalDateWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public int Year { get; set; }
        public required string NumeratorID { get; set; }

        public DateTime? SelectedDate { get; set; }

        public DateTime? CheckLastFinalDate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().CheckLastFinalDate(CompanyID, Year, NumeratorID);
        }
    }
}
