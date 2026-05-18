using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class NotifierHelper
    {
        public enum SendClasses { Generic, CRM_Offers, CRM_Orders, CRM_DDT, CRM_Invoices, SRM_Purchase_Orders };

        public static string? SendEmail(string UserName, string Password, string SenderName, string SMTPAddress, int SMTPPort, bool SMTPUseTLS, byte[] PKData, string Domain, SendClasses ReportClass, string To, string? Cc, string Subject, string Body, string[]? Attachments, out string Sender)
        {
            try
            {
                var message = new MimeMessage();
                string? usr = UserName;
                string? psw = Password;
                string? senderName = SenderName;

                //switch (ReportClass)
                //{
                //    case SendClasses.Generic:
                //        usr = Username;
                //        psw = Settings.azpsw;
                //        senderName = string.IsNullOrWhiteSpace(Settings.azusrname) ? $"{CompanyName} - Info" : Settings.azusrname;
                //        break;
                //    case SendClasses.CRM_Offers:
                //    case SendClasses.CRM_Orders:
                //    case SendClasses.CRM_DDT:
                //    case SendClasses.CRM_Invoices:
                //        usr = Settings.azusrcrm;
                //        psw = Settings.azusrcrmpsw;
                //        senderName = string.IsNullOrWhiteSpace(Settings.azusrcrmname) ? $"{CompanyName} - Vendite" : Settings.azusrcrmname;
                //        break;
                //    case SendClasses.SRM_Purchase_Orders:
                //        usr = Settings.azusrsrm;
                //        psw = Settings.azusrsrmpsw;
                //        senderName = string.IsNullOrWhiteSpace(Settings.azusrsrmname) ? $"{CompanyName} - Acquisti" : Settings.azusrsrmname;
                //        break;
                //}

                Sender = $"{senderName} [{usr}]";
                message.From.Add(new MailboxAddress(senderName, usr));
                message.ReplyTo.Add(new MailboxAddress(senderName, usr));

                foreach (var itTo in To.Split(','))
                {
                    message.To.Add(new MailboxAddress(itTo, itTo));
                }

                if (!string.IsNullOrWhiteSpace(Cc))
                {
                    foreach (var itCc in Cc.Split(','))
                    {
                        message.Cc.Add(new MailboxAddress(itCc, itCc));
                    }
                }

                message.Subject = Subject;
                var builder = new BodyBuilder();
                builder.TextBody = Body;
                if (Attachments != null)
                {
                    foreach (var att in Attachments)
                    {
                        if (att != null)
                            builder.Attachments.Add(att);
                    }
                }
                message.Body = builder.ToMessageBody();

                var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(SMTPAddress, SMTPPort, SMTPUseTLS ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None);

                string? passwordDecrpyted = CryptoHelper.CSDecrypt(psw, PKData, Domain);

                if (string.IsNullOrEmpty(passwordDecrpyted))
                {
                    client.Disconnect(true);
                    return "Errore nel decriptare la password";    
                }

                client.Authenticate(usr,  passwordDecrpyted);
                client.Send(message);
                client.Disconnect(true);
                return null;
            }
            catch (Exception exc)
            {
                Sender = "ERROR";
                return exc.Message + "\n\n" + exc.InnerException?.Message + "\n\n" + exc.InnerException?.InnerException?.Message;
            }
        }

        public static bool CheckEmailAddress(string? EmailEddress)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailEddress))
                    return false;

                _ = new MailAddress(EmailEddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool CheckEmailAddresses(string? EmailEddresses)
        {
            if (!string.IsNullOrWhiteSpace(EmailEddresses))
            {
                foreach (var email in EmailEddresses.Split(','))
                {
                    if (!CheckEmailAddress(email))
                        return false;
                }
                return true;
            }
            else
            { return false; }
        }

        public static string? SendTestEmail(string SMTPServer, int Port, bool UseTLS, string Username, string Password, string To)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(Constants.APP_NAME, Username));
                message.ReplyTo.Add(new MailboxAddress(Constants.APP_NAME, Username));
                foreach (var itTo in To.Split(','))
                {
                    message.To.Add(new MailboxAddress(itTo, itTo));
                }
                message.Subject = $"{Constants.APP_NAME} test email";
                var builder = new BodyBuilder();
                builder.TextBody = $"Email di prova inviata da Vulpes X, nome PC: {Environment.MachineName}";
                message.Body = builder.ToMessageBody();
                var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(SMTPServer, Port, UseTLS ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None);
                client.Authenticate(Username, Password);
                client.Send(message);
                client.Disconnect(true);
                return null;
            }
            catch (Exception exc)
            {
                return exc.Message + "\n\n" + exc.InnerException?.Message + "\n\n" + exc.InnerException?.InnerException?.Message;
            }
        }

        public static bool CheckSMTPConnection(string SMTPServer, int Port, bool UseTLS, string Username, string Password)
        {
            try
            {
                var client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect(SMTPServer, Port, UseTLS ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None);
                client.Authenticate(Username, Password);
                client.Disconnect(true);
                return true;
            }
            catch (Exception exc)
            {
                ErrorHandler.Validation(exc.Message + "\n\n" + exc.InnerException?.Message + "\n\n" + exc.InnerException?.InnerException?.Message);
                return false;
            }
        }
    }
}
