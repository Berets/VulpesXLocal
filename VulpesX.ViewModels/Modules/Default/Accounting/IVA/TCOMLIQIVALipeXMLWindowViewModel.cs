using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.IVA
{
    public class TCOMLIQIVALipeXMLWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public TCOMLIQIVALipeXMLWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required int Year { get; set; }
        public required int Month { get; set; }
        public string? LIPEType { get; set; }
        
        public string? VATID { get; set; }
        public string? FISCALID { get; set; }

        public string? FiscalIDSender { get; set; }
        
        public ObservableCollection<GenericIDDescription>? Titles { get; set; }
        public string? TitleID { get; set; }

        // broker 
        public string? BrokerFiscalID { get; set; }
        public ObservableCollection<GenericIDDescription>? Presentations { get; set; }
        public string? PresentationID { get; set; }

        public AZIENDA? GetAZIENDA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);
        }

        public string? GenerateXML()
        {
            var tcomLiqIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<ITCOMLIQIVARepository>();

            var items = tcomLiqIvaRepo.GetListDetails(CompanyID, Year, Month);

            return tcomLiqIvaRepo.GenerateXML(new Models.Models.Accounting.LIPEXMLViewModel
            {
                CompanyID = CompanyID,
                BrokerFiscalID = BrokerFiscalID,
                TitleID = TitleID,
                FiscalIDSender = FiscalIDSender,
                LIPEType = LIPEType,
                Month = Month,
                PresentationID = PresentationID,
                Presentations = Presentations,
                Titles = Titles,
                VATID = VATID,
                FISCALID = FISCALID,
                Year = Year,
            });
        }
    }
}
