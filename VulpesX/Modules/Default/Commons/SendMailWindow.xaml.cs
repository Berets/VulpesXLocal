using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VulpesX.Models;
using VulpesX.Models.Models;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using static VulpesX.Shared.Utilities.NotifierHelper;

namespace VulpesX.Modules.Default.Commons
{
    /// <summary>
    /// Interaction logic for SendMailWindow.xaml
    /// </summary>
    public partial class SendMailWindow : FluentDefaultWindow
    {
        private SendMailWindowViewModel _dataContext;
        private ISendEmailLogs? logEntry;

        public SendMailWindow(SendMailWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            cmdSave.IsEnabled = _dataContext.Settings != null;

            SetLogEntry(_dataContext.SendClass);

            string? to = null;

            if (logEntry != null && _dataContext.Settings != null)
            {
                switch (_dataContext.SendClass)
                {
                    case NotifierHelper.SendClasses.Generic:
                        this.Title = "Invio email interna";
                        logEntry.document_type = "###";
                        ((log_gen_send)logEntry).tag = "###";
                        break;
                    case NotifierHelper.SendClasses.CRM_Offers:
                        this.Title = "Invio email con offerta commerciale";
                        ((log_crm_send)logEntry).document_year = _dataContext.DocumentYear;
                        ((log_crm_send)logEntry).document_number = _dataContext.DocumentNumber;
                        _dataContext.Subject = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azoffogg) ? _dataContext.SettingsLanguage.azoffogg : _dataContext.Settings.azoffogg) : _dataContext.Settings.azoffogg;
                        _dataContext.Body = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azofftex) ? _dataContext.SettingsLanguage.azofftex : _dataContext.Settings.azofftex) : _dataContext.Settings.azofftex;
                        to = _dataContext.CustomerID.HasValue && _dataContext.CustomerID.Value > 0 ? string.Join(",", _dataContext.GetCRMEmailList(_dataContext.CustomerID.Value, _dataContext.SendClass) ?? new()) : _dataContext.To;
                        _dataContext.Cc = _dataContext.Settings.AZCCOFF ? _dataContext.UserID : null;
                        logEntry.document_type = "OFF";
                        break;
                    case NotifierHelper.SendClasses.CRM_Orders:
                        this.Title = "Invio email con ordine cliente";
                        ((log_crm_send)logEntry).document_year = _dataContext.DocumentYear;
                        ((log_crm_send)logEntry).document_number = _dataContext.DocumentNumber;
                        _dataContext.Subject = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azordogg) ? _dataContext.SettingsLanguage.azordogg : _dataContext.Settings.azordogg) : _dataContext.Settings.azordogg;
                        _dataContext.Body = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azordtex) ? _dataContext.SettingsLanguage.azordtex : _dataContext.Settings.azordtex) : _dataContext.Settings.azordtex;
                        to = string.Join(",", _dataContext.GetCRMEmailList(_dataContext.CustomerID!.Value, _dataContext.SendClass) ?? new());
                        _dataContext.Cc = _dataContext.Settings.AZCCORD ? _dataContext.UserID : null;
                        logEntry.document_type = "ORD";
                        break;
                    case NotifierHelper.SendClasses.CRM_DDT:
                        this.Title = "Invio email con DDT";
                        ((log_crm_send)logEntry).document_year = _dataContext.DocumentYear;
                        ((log_crm_send)logEntry).document_number = _dataContext.DocumentNumber;
                        _dataContext.Subject = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azddtogg) ? _dataContext.SettingsLanguage.azddtogg : _dataContext.Settings.azddtogg) : _dataContext.Settings.azddtogg;
                        _dataContext.Body = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azddttex) ? _dataContext.SettingsLanguage.azddttex : _dataContext.Settings.azddttex) : _dataContext.Settings.azddttex;
                        to = string.Join(",", _dataContext.GetCRMEmailList(_dataContext.CustomerID!.Value, _dataContext.SendClass) ?? new());
                        _dataContext.Cc = _dataContext.Settings.AZCCDDT ? _dataContext.UserID : null;
                        logEntry.document_type = "DDT";
                        break;
                    case NotifierHelper.SendClasses.CRM_Invoices:
                        this.Title = "Invio email con fattura";
                        ((log_crm_send)logEntry).document_year = _dataContext.DocumentYear;
                        ((log_crm_send)logEntry).document_number = _dataContext.DocumentNumber;
                        _dataContext.Subject = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azinvogg) ? _dataContext.SettingsLanguage.azinvogg : _dataContext.Settings.azinvogg) : _dataContext.Settings.azinvogg;
                        _dataContext.Body = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azinvtex) ? _dataContext.SettingsLanguage.azinvtex : _dataContext.Settings.azinvtex) : _dataContext.Settings.azinvtex;
                        to = string.Join(",", _dataContext.GetCRMEmailList(_dataContext.CustomerID!.Value, _dataContext.SendClass) ?? new());
                        _dataContext.Cc = _dataContext.Settings.AZCCINV ? _dataContext.UserID : null;
                        logEntry.document_type = "INV";
                        break;
                    case NotifierHelper.SendClasses.SRM_Purchase_Orders:
                        this.Title = "Invio email con ordine di acquisto";
                        ((log_srm_send)logEntry).document_number = _dataContext.DocumentID;
                        _dataContext.Subject = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azbuyogg) ? _dataContext.SettingsLanguage.azbuyogg : _dataContext.Settings.azbuyogg) : _dataContext.Settings.azbuyogg;
                        _dataContext.Body = (_dataContext.SettingsLanguage != null) ? (!string.IsNullOrEmpty(_dataContext.SettingsLanguage.azbuytex) ? _dataContext.SettingsLanguage.azbuytex : _dataContext.Settings.azbuytex) : _dataContext.Settings.azbuytex;
                        to = string.Join(",", _dataContext.GetSRMEmailList(_dataContext.SupplierID!.Value, _dataContext.SendClass) ?? new());
                        _dataContext.Cc = _dataContext.Settings.AZCCBUY ? _dataContext.UserID : null;
                        logEntry.document_type = "BUY";
                        break;
                }
            }
            if (string.IsNullOrWhiteSpace(to))
            {
                // add generic email from clienti/fornitori if exists
                to = _dataContext.GetReceiver();
            }
            _dataContext.To = to;

        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Settings != null)
            {
                if (NotifierHelper.CheckEmailAddresses(_dataContext.To))
                {
                    if (string.IsNullOrWhiteSpace(_dataContext.Cc) || (!string.IsNullOrWhiteSpace(_dataContext.Cc) && NotifierHelper.CheckEmailAddresses(_dataContext.Cc)))
                    {
                        if (!string.IsNullOrWhiteSpace(_dataContext.Subject))
                        {
                            if (!string.IsNullOrWhiteSpace(_dataContext.Body))
                            {
                                if (!string.IsNullOrEmpty(_dataContext.Settings.azusrsrm) && !string.IsNullOrEmpty(_dataContext.Settings.azusrsrmpsw) && !string.IsNullOrEmpty(_dataContext.Settings.azsmtp))
                                {
                                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                                    string? attachment = null;
                                    // zip attachments
                                    if (_dataContext.Attachments != null && _dataContext.Attachments.Count >= 1)
                                    {
                                        attachment = FileHelper.GenerateZIPFilename(_dataContext.OriginalFilename);

                                        if (!string.IsNullOrEmpty(attachment))
                                        {
                                            var resultZipCreation = FileHelper.CreateZipStream(attachment, _dataContext.Attachments.ToArray());
                                            if (!string.IsNullOrWhiteSpace(resultZipCreation))
                                            {
                                                Mouse.OverrideCursor = null;
                                                ErrorHandler.Validation(resultZipCreation);
                                                e.Handled = true;
                                            }
                                        }
                                        else
                                        {
                                            Mouse.OverrideCursor = null;
                                            ErrorHandler.Validation("Impossibile creare il file .zip");
                                            e.Handled = true;
                                        }
                                    }

                                    string selectedSender = "---";
                                    var result = NotifierHelper.SendEmail(
                                        _dataContext.Settings.azusrsrm,
                                        _dataContext.Settings.azusrsrmpsw,
                                        string.IsNullOrWhiteSpace(_dataContext.Settings.azusrsrmname) ? $"{UserContext.Instance!.ACCESS!.SelectedCompany!.SOMDES} - Acquisti" : _dataContext.Settings.azusrsrmname,
                                        _dataContext.Settings.azsmtp,
                                        _dataContext.Settings.azsmtpport ?? 0,
                                        _dataContext.Settings.azusetls ?? false,
                                         Encoding.UTF8.GetBytes(UserContext.Instance!.PK + UserContext.Instance!.KP),
                                         UserContext.Instance!.Domain!,
                                        _dataContext.SendClass!,
                                        _dataContext.To!,
                                        _dataContext.Cc,
                                        _dataContext.Subject,
                                        _dataContext.Body,
                                        !string.IsNullOrWhiteSpace(attachment) ? new string[] { attachment } : null,
                                        out selectedSender);
                                    // send log

                                    bool isOK = string.IsNullOrWhiteSpace(result);
                                    string entryResult = string.IsNullOrWhiteSpace(result) ? $"#OK" : $"#KO >>> {result}";

                                    if (logEntry != null)
                                    {
                                        logEntry.sent_to = _dataContext.To;
                                        logEntry.sent_cc = _dataContext.Cc;
                                        logEntry.sent_object = _dataContext.Subject;
                                        logEntry.sent_from = selectedSender;
                                        logEntry.sent_attachments = attachment;
                                        logEntry.client_time = DateTime.Now;
                                        logEntry.sendUserID = _dataContext.UserID;
                                        logEntry.result = entryResult;

                                        Mouse.OverrideCursor = null;

                                        if (logEntry is log_gen_send)
                                        {
                                            _dataContext.InsertGenLog((log_gen_send)logEntry);
                                        }
                                        else
                                        {
                                            if (logEntry is log_crm_send)
                                                _dataContext.InsertCrmLog((log_crm_send)logEntry);
                                            else
                                                _dataContext.InsertSrmLog((log_srm_send)logEntry);
                                        }
                                    }

                                    if (isOK)
                                        InfoHandler.Show("Email inviata correttamente");
                                    else
                                        ErrorHandler.Validation(entryResult);

                                    this.DialogResult = isOK;
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation($"Il testo dell'email e' obbligatorio");
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation($"L'oggetto della email e' obbligatorio");
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation($"Uno o piu' indirizzi in copia conoscenza non sono validi");
                    }
                }
                else
                {
                    ErrorHandler.Validation($"Uno o piu' indirizzi destinatari non sono validi");
                }
            }

            e.Handled = true;

        }

        private void cmdAddAttachment_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog() { Multiselect = true };
            if (fileDialog.ShowDialog() ?? false)
            {
                if (_dataContext.Attachments == null)
                    _dataContext.Attachments = new System.Collections.ObjectModel.ObservableCollection<string>();

                foreach (var file in fileDialog.FileNames)
                {
                    _dataContext.Attachments.Add(file);
                }

            }
        }

        private void lbAttachments_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (_dataContext.Attachments == null)
                    _dataContext.Attachments = new System.Collections.ObjectModel.ObservableCollection<string>();

                if (lbAttachments.SelectedItem != null)
                    _dataContext.Attachments.Remove((string)lbAttachments.SelectedItem);
            }
        }

        private void SetLogEntry(SendClasses Class)
        {
            switch (Class)
            {
                case NotifierHelper.SendClasses.Generic:
                    logEntry = new log_gen_send
                    {
                        company_id = _dataContext.CompanyID,
                        client_name = Environment.MachineName,
                        document_type = "Generic",
                        sendUserID = _dataContext.UserID,
                    };
                    break;
                case NotifierHelper.SendClasses.CRM_Offers:
                case NotifierHelper.SendClasses.CRM_Orders:
                case NotifierHelper.SendClasses.CRM_DDT:
                case NotifierHelper.SendClasses.CRM_Invoices:
                    logEntry = new log_crm_send
                    {
                        company_id = _dataContext.CompanyID,
                        client_name = Environment.MachineName,
                        document_type = "CRM",
                        sendUserID = _dataContext.UserID,
                    };
                    break;
                case NotifierHelper.SendClasses.SRM_Purchase_Orders:
                    logEntry = new log_srm_send
                    {
                        company_id = _dataContext.CompanyID,
                        client_name = Environment.MachineName,
                        document_type = "SRM",
                        sendUserID = _dataContext.UserID,
                    };
                    break;
            }
        }
    }
}
