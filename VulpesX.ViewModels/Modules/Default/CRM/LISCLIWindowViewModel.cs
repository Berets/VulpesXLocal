using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class LISCLIWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public LISCLIWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required CRM_LISCLI Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription> ValueTypes => CommonsService.StandardValueTypes;

        public ObservableCollection<ABE>? Customers { get; set; }
        public ABE? SelectedCustomer { get; set; }

        private ObservableCollection<GenericIDDescription>? recipients;
        public ObservableCollection<GenericIDDescription>? Recipients { get => recipients; set { recipients = value; NotifyPropertyChanged("Recipients"); } }

        private GenericIDDescription? selectedRecipient;
        public GenericIDDescription? SelectedRecipient { get => selectedRecipient; set { selectedRecipient = value; NotifyPropertyChanged("SelectedRecipient"); } }

        public void LoadDetails()
        {
            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
            Data.UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            Data.Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
        }

        public ObservableCollection<GenericIDDescription>? GetDESTINATARIs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetSimpleList(EntityID, true);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().Update(Data);
            }
        }
    }
}
