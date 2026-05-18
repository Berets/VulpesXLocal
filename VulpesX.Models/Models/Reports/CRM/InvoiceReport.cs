using VulpesX.Models.Default;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.Models.Reports.CRM
{
    public class InvoiceReport
    {
        public AZIENDA? CompanyInfo { get; set; }
        public FATTT00F? Invoice { get; set; }
        public byte[]? LogoData { get; set; }
        public byte[]? CertificationsLogoData { get; set; }
        public string? PaymentDescription { get; set; }
        public string? BankData { get; set; }
        public string? FixedText { get; set; }
        public List<Tuple<string, string>>? Expires { get; set; }
        // first 3
        public List<Tuple<string, string, string, string>>? Rates { get; set; }
        // others
        public List<Tuple<string, string, string, string>>? Rates2 { get; set; }

        public string? Expire1
        {
            get
            {
                if (Expires == null || Expires.Count == 0)
                    return null;

                return Expires[0]?.Item1;
            }
        }

        public string? Expire2
        {
            get
            {
                if (Expires == null || Expires.Count == 0)
                    return null;

                return Expires[0]?.Item2;
            }
        }

        public string? Expire3
        {
            get
            {
                if (Expires == null || Expires.Count < 2)
                    return null;

                return Expires[1]?.Item1;
            }
        }

        public string? Expire4
        {
            get
            {
                if (Expires == null || Expires.Count < 2)
                    return null;

                return Expires[1]?.Item2;
            }
        }

        public string? Expire5
        {
            get
            {
                if (Expires == null || Expires.Count < 3)
                    return null;

                return Expires[2]?.Item1;
            }
        }

        public string? Expire6
        {
            get
            {
                if (Expires == null || Expires.Count < 3)
                    return null;

                return Expires[2]?.Item2;
            }
        }

        public string? Expire7
        {
            get
            {
                if (Expires == null || Expires.Count < 4)
                    return null;

                return Expires[3]?.Item1;
            }
        }

        public string? Expire8
        {
            get
            {
                if (Expires == null || Expires.Count < 4)
                    return null;

                return Expires[3]?.Item2;
            }
        }

        public string? Expire9
        {
            get
            {
                if (Expires == null || Expires.Count < 5)
                    return null;

                return Expires[4]?.Item1;
            }
        }

        public string? Expire10
        {
            get
            {
                if (Expires == null || Expires.Count < 5)
                    return null;

                return Expires[4]?.Item2;
            }
        }

        public string? Expire11
        {
            get
            {
                if (Expires == null || Expires.Count < 6)
                    return null;

                return Expires[5]?.Item1;
            }
        }

        public string? Expire12
        {
            get
            {
                if (Expires == null || Expires.Count < 6)
                    return null;

                return Expires[5]?.Item2;
            }
        }

        public object? LinguaDictionary { get; set; }
    }
}
