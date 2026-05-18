using CerberoSendAPI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTT00FSentInfoWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public FATTT00FSentInfoWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public required FATTT00F InvoiceHead { get; set; }

        private RetrieveFileResult? result;
        public RetrieveFileResult? Result
        {
            get => result;
            set
            {
                result = value;
                NotifyPropertyChanged("Result");
            }
        }

        public AZIENDA? GetAZIENDA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);
        }

        public string? GenerateInvoiceXML(int Year, int ID, bool IsRegenerate)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GenerateInvoiceXML(CompanyID, Year, ID, IsRegenerate);
        }
    }
}
