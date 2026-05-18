using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default;
using VulpesX.Shared.Utilities;

namespace VulpesX.Models
{
    public class UserContext
    {
        private static UserContext? _instance;
        public static UserContext Instance => _instance ??= new UserContext { UserName = string.Empty };

        public required string UserName { get; set; }
        public string? Password { get; set; }
        public string? Domain
        {
            get
            {
                return !string.IsNullOrEmpty(UserName) ? UserName.Substring(UserName.IndexOf("@"), UserName.Length - UserName.IndexOf("@")) : null;
            }
        }
        public string? PK
        {
            get { return !string.IsNullOrEmpty(Password) ? Password.Substring(0, 4) : null; }
        }
        public string? KP
        {
            get
            {
                return !string.IsNullOrEmpty(PK) ? TextHelper.ReverseAndScrumble(PK) : null;
            }
        }

        public byte[] PKKP => Encoding.UTF8.GetBytes(PK + KP);

        public string? ConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(PK))
                {
                    byte[] PKData = Encoding.UTF8.GetBytes(PK + KP);

                    switch (Domain)
                    {
                        case "@cerbertech.com":
                            return CryptoHelper.CSDecrypt(CS.CSCT, PKData, Domain);
                        case "@gxitalia.com":
                            return CryptoHelper.CSDecrypt(CS.CSGX, PKData, Domain);
                        case "@groupchemie.it":
                            return CryptoHelper.CSDecrypt(CS.CSGC, PKData, Domain);
                        case "@baruffaldisrl.it":
                            return CryptoHelper.CSDecrypt(CS.CSBAL, PKData, Domain);
                        case "@cascinaboscogerolo.it":
                            return CryptoHelper.CSDecrypt(CS.CSBG, PKData, Domain);
                        case "@demo.it":
                            return CryptoHelper.CSDecrypt(CS.CSDM, PKData, Domain);
                        case "@ufp.it":
                            return "Server=VULPES;Database=Vulpes;User ID=sa;Password=ufp@SQL23;Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";
                    }
                }

                return null;
            }
        }
        public string? Schema { get; set; }

        public ACCESS? ACCESS { get; set; }
    }
}
