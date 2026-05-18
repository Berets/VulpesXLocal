using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting.eInvoice;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public abstract class AttachmentsWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyGuidID { get; set; }
        public required string UserID { get; set; }

        public AttachmentsWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyGuidID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.UserName;
        }

        public required ACC_EINVOICE_HEADS Head { get; set; }

        private ObservableCollection<DocumentAttachment>? attachments;
        public ObservableCollection<DocumentAttachment>? Attachments
        {
            get => attachments;
            set { attachments = value; NotifyPropertyChanged("Attachments"); }
        }

        public abstract string? GetApiKey();
    }

    public class AttachmentsWindowViewModelDefault : AttachmentsWindowViewModel
    {
        public AttachmentsWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyGuidID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.UserName;
        }

        public override string? GetApiKey()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.azapikey;
        }
    }

    public class AttachmentsWindowViewModelUfp : AttachmentsWindowViewModel
    {
        public AttachmentsWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyGuidID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.UserName;
        }

        public override string? GetApiKey()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPERSBOLLRepository>().Get(CompanyID)?.perapikey;
        }
    }
}
