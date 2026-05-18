using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Logs;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Logs;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Commons
{
    public class SendMailWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public required NotifierHelper.SendClasses SendClass { get; set; }
        public AZIENDA? Settings { get; set; }

        public SendMailWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public int DocumentYear { get; set; }
        public int DocumentNumber { get; set; }
        public long DocumentID { get; set; }
        public int? CustomerID { get; set; }
        public int? SupplierID { get; set; }
        public string? OriginalFilename { get; set; }

        public AZIENDA_LINGUA? SettingsLanguage { get; set; }


        private ObservableCollection<string>? attachments;
        public ObservableCollection<string>? Attachments { get => attachments; set { attachments = value; NotifyPropertyChanged("Attachments"); } }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? To { get; set; }
        public string? Cc { get; set; }
        public string? Language { get; set; }

        public void LoadDetails()
        {
            Settings = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            if (!string.IsNullOrEmpty(Language))
                SettingsLanguage = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(CompanyID, Language);
        }

        public bool InsertGenLog(log_gen_send Log)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ilog_gen_sendRepository>().Insert(Log);
        }

        public bool InsertCrmLog(log_crm_send Log)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ilog_crm_sendRepository>().Insert(Log);
        }

        public bool InsertSrmLog(log_srm_send Log)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ilog_srm_sendRepository>().Insert(Log);
        }

        public ObservableCollection<string>? GetCRMEmailList(int ID, NotifierHelper.SendClasses SendClass)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>().GetCRMEmailList(ID, SendClass);
        }

        public ObservableCollection<string>? GetSRMEmailList(int ID, NotifierHelper.SendClasses SendClass)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>().GetSRMEmailList(ID, SendClass);
        }

        public string? GetReceiver()
        {
            if (CustomerID.HasValue)
                return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(CustomerID.Value)?.climail?.Trim();
            if (SupplierID.HasValue)
                return VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>().Get(SupplierID.Value)?.Formail?.Trim();

            return null;
        }
    }
}
