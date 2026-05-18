using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.CRM
{
    public class AccountingAccount
    {
        public string? GroupID { get; set; }
        public string? AccountID { get; set; }
        public string? SubaccountID { get; set; }
        public string? CostCenter { get; set; }
        public string? ProductCostCenter { get; set; }

        public override bool Equals(object? obj)
        {
            if (!(obj is AccountingAccount))
                return false;

            var tObj = obj as AccountingAccount;

            return tObj!.GroupID == GroupID && tObj.AccountID == AccountID && tObj.SubaccountID == SubaccountID && tObj.CostCenter == CostCenter;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(GroupID);
            hash.Add(AccountID);
            hash.Add(SubaccountID);
            hash.Add(CostCenter);
            return hash.ToHashCode();
        }
    }
}
