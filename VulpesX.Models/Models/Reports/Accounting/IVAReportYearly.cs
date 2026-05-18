
using VulpesX.Models.Default;

namespace VulpesX.Models.Models.Reports.Accounting
{
    public class IVAReportYearly
    {
        public AZIENDA? CompanyInfo { get; set; }
        public DateTime PrintSince { get; set; }
        public DateTime PrintUntil { get; set; }
        public string? TemporaryText { get; set; }
        public string DetailsText => "DETTAGLI LIBRI IVA";
        public string IntervalText => $"Dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")}";

        #region Recap
        public List<DetailSection>? IVABookRecaps { get; set; }
        public List<PNIVA>? Rows { get; set; }
        public decimal SalesVATTotal => (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("V") && w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("V") && w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
        public decimal SalesNoVATTotal => (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("V") && w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("V") && w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
        public decimal SalesTotal => SalesVATTotal - SalesNoVATTotal;
        public decimal PurchasesVATTotal => (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("A") && w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("A") && w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
        public decimal PurchasesNoVATTotal => (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("A") && w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (Rows?.Where(w => (w.IVABookType ?? string.Empty).StartsWith("A") && w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
        public decimal PurchasesTotal => PurchasesVATTotal - PurchasesNoVATTotal;
        public decimal TotalBalance => SalesTotal - PurchasesTotal;
        public string TotalBalanceText => TotalBalance > 0 ? "TOTALE IVA A DEBITO" : TotalBalance < 0 ? "TOTALE IVA A CREDITO" : "TOTALE IVA";
        #endregion
        #region Payments
        public PaymentsRecap? PaymentsInfo { get; set; }
        #endregion
        #region Unpaid recap
        public UnpaidRecap? UnpaidInfo { get; set; }
        #endregion
        #region Unpaid expire recap
        public UnpaidExpireRecap? UnpaidExpireInfo { get; set; }
        #endregion
        #region Unpaid IVA details
        public List<PNIVA> SalesDetails
        {
            get
            {
                var result = new List<PNIVA>();
                DateTime? lastExpire = null;
                string? lastCustomerDoc = null;
                decimal totalAmount = 0;
                decimal totalVAT = 0;
                decimal totalNoVAT = 0;
                int customerCount = 0;
                bool afterSub = false;

                foreach (var item in (UnpaidInfo?.Rows ?? new List<PNIVA>()).Where(w => w.IVABookType == "V").OrderBy(o => o.N4DTSCPG).ThenBy(tb => tb.N4DOCU))
                {
                    if (lastExpire != item.N4DTSCPG && lastExpire.HasValue)
                    {
                        afterSub = true;
                        // add expire date subtotal row
                        result.Add(new PNIVA
                        {
                            N4SOCI = string.Empty,
                            IsSubTotal = true,
                            ExpireDate = lastExpire.Value,
                            ReportAmount = totalAmount,
                            ReportVATAmount = totalVAT,
                            N4IIEU = totalNoVAT,
                            CustomerCount = customerCount
                        });
                        lastExpire = item.N4DTSCPG;
                        customerCount = 1;
                        if (item.N4SEGN == "+")
                        {
                            totalAmount = (item.N4IMEU ?? 0);
                            totalVAT = (item.N4IVEU ?? 0);
                            totalNoVAT = (item.N4IIEU ?? 0);
                        }
                        else
                        {
                            totalAmount = (item.N4IMEU ?? 0) * -1;
                            totalVAT = (item.N4IVEU ?? 0) * -1;
                            totalNoVAT = (item.N4IIEU ?? 0) * -1;
                        }
                    }
                    else
                    {
                        if (lastCustomerDoc != item.N4DOCU)
                        {
                            customerCount++;
                            lastCustomerDoc = item.N4DOCU;
                        }

                        if (item.N4SEGN == "+")
                        {
                            totalAmount += (item.N4IMEU ?? 0);
                            totalVAT += (item.N4IVEU ?? 0);
                        }
                        else
                        {
                            totalAmount -= (item.N4IMEU ?? 0);
                            totalVAT -= (item.N4IVEU ?? 0);
                        }
                    }

                    var existing = result.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT).FirstOrDefault();
                    if (existing == null || afterSub)
                    {
                        result.Add(item);
                        afterSub = false;
                    }
                    else
                    {
                        result.Add(new PNIVA
                        {
                            N4SOCI = string.Empty,
                            N4ANNO = item.N4ANNO,
                            N4REGI = item.N4REGI,
                            N4RIGA = item.N4RIGA,
                            N4IMEU = (item.N4IMEU ?? 0),
                            N4IVEU = (item.N4IVEU ?? 0),
                            N4IIEU = item.N4IIEU.HasValue && item.N4IIEU.Value > 0 ? item.N4IIEU.Value : null,
                            N4SEGN = item.N4SEGN,
                            RateFullDescription = item.RateFullDescription,
                            CustomerID = (item.N4SOTT ?? 0)
                        });
                    }

                    if (lastExpire == null)
                        lastExpire = item.N4DTSCPG;
                }
                if (lastExpire != null)
                {
                    // add last subtotal
                    result.Add(new PNIVA
                    {
                        N4SOCI = string.Empty,
                        IsSubTotal = true,
                        ExpireDate = lastExpire.Value,
                        ReportAmount = totalAmount,
                        ReportVATAmount = totalVAT,
                        N4IIEU = totalNoVAT,
                        CustomerCount = customerCount
                    });
                }
                return result;
            }
        }
        public List<PNIVA> PurchasesDetails
        {
            get
            {
                var result = new List<PNIVA>();
                DateTime? lastExpire = null;
                string? lastCustomerDoc = null;
                decimal totalAmount = 0;
                decimal totalVAT = 0;
                decimal totalNoVAT = 0;
                int customerCount = 0;
                foreach (var item in (UnpaidInfo?.Rows ?? new List<PNIVA>()).Where(w => w.IVABookType == "A").OrderBy(o => o.N4DTSCPG).ThenBy(tb => tb.N4DOCU))
                {
                    if (lastExpire != item.N4DTSCPG && lastExpire.HasValue)
                    {
                        // add expire date subtotal row
                        result.Add(new PNIVA
                        {
                            N4SOCI = string.Empty,
                            IsSubTotal = true,
                            ExpireDate = lastExpire.Value,
                            ReportAmount = totalAmount,
                            ReportVATAmount = totalVAT,
                            N4IIEU = totalNoVAT,
                            CustomerCount = customerCount
                        });
                        lastExpire = item.N4DTSCPG;
                        customerCount = 1;
                        if (item.N4SEGN == "+")
                        {
                            totalAmount = (item.N4IMEU ?? 0);
                            totalVAT = (item.N4IVEU ?? 0);
                            totalNoVAT = (item.N4IIEU ?? 0);
                        }
                        else
                        {
                            totalAmount = (item.N4IMEU ?? 0) * -1;
                            totalVAT = (item.N4IVEU ?? 0) * -1;
                            totalNoVAT = (item.N4IIEU ?? 0) * -1;
                        }
                    }
                    else
                    {
                        if (lastCustomerDoc != item.N4DOCU)
                        {
                            customerCount++;
                            lastCustomerDoc = item.N4DOCU;
                        }

                        if (item.N4SEGN == "+")
                        {
                            totalAmount += (item.N4IMEU ?? 0);
                            totalVAT += (item.N4IVEU ?? 0);
                        }
                        else
                        {
                            totalAmount -= (item.N4IMEU ?? 0);
                            totalVAT -= (item.N4IVEU ?? 0);
                        }
                    }

                    var existing = result.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT).FirstOrDefault();
                    if (existing == null)
                    {
                        result.Add(item);
                    }
                    else
                    {
                        result.Add(new PNIVA
                        {
                            N4SOCI = string.Empty,
                            N4ANNO = item.N4ANNO,
                            N4REGI = item.N4REGI,
                            N4RIGA = item.N4RIGA,
                            N4IMEU = (item.N4IMEU ?? 0),
                            N4IVEU = (item.N4IVEU ?? 0),
                            N4IIEU = item.N4IIEU.HasValue && item.N4IIEU.Value > 0 ? item.N4IIEU.Value : null,
                            N4SEGN = item.N4SEGN,
                            RateFullDescription = item.RateFullDescription,
                            CustomerID = (item.N4SOTT ?? 0),
                            N4DTSCPG = item.N4DTSCPG
                        });
                    }

                    if (lastExpire == null)
                        lastExpire = item.N4DTSCPG;
                }
                if (lastExpire != null)
                {
                    // add last subtotal
                    result.Add(new PNIVA
                    {
                        N4SOCI = string.Empty,
                        IsSubTotal = true,
                        ExpireDate = lastExpire.Value,
                        ReportAmount = totalAmount,
                        ReportVATAmount = totalVAT,
                        N4IIEU = totalNoVAT,
                        CustomerCount = customerCount
                    });
                }
                return result;
            }
        }
        public decimal SalesDetailsAmountTotal => (SalesDetails.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IMEU) ?? 0) - (SalesDetails.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IMEU) ?? 0);
        public decimal SalesDetailsAmountVATTotal => (SalesDetails.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (SalesDetails.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
        public decimal SalesDetailsAmountNoVATTotal => (SalesDetails.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (SalesDetails.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
        public decimal PurchasesDetailsAmountTotal => (PurchasesDetails.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IMEU) ?? 0) - (PurchasesDetails.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IMEU) ?? 0);
        public decimal PurchasesDetailsAmountVATTotal => (PurchasesDetails.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (PurchasesDetails.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
        public decimal PurchasesDetailsAmountNoVATTotal => (PurchasesDetails.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (PurchasesDetails.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
        #endregion

        public class DetailSection
        {
            public string? IVABookFullDescription { get; set; }
            public string? IVABookType { get; set; }
            public string? IVABookTypeFullDescription => CommonsService.IVABookTypes.Where(w => w.ID == IVABookType).FirstOrDefault()?.FullDescription.ToUpper();
            public List<PNIVA>? Rows { get; set; }
            public List<VATRecap>? VATs { get; set; }
            public decimal VATTotal => VATs?.Sum(sum => sum.TotalVATAmount) ?? 0;
            public decimal NoVATTotal => VATs?.Sum(sum => sum.TotalNoVATAmount) ?? 0;
            public decimal Balance => VATTotal - NoVATTotal;

        }

        public class PaymentsRecap
        {
            public decimal InitialCredit { get; set; }
            public decimal VATDebit { get; set; }
            public decimal VATCredit { get; set; }
            public decimal VATBalance => VATDebit - (VATCredit + InitialCredit);
            public string VATBalanceText => VATBalance > 0 ? "TOTALE IVA A DEBITO" : VATBalance < 0 ? "TOTALE IVA A CREDITO" : "TOTALE IVA";
            public List<VATPayment>? Payments { get; set; }
            public decimal TotalDebit => Payments?.Sum(sum => sum.DebitAmount) ?? 0;
            public decimal TotalCredit => Payments?.Sum(sum => sum.CreditAmount) ?? 0;
            public decimal PaymentBalance => TotalDebit - VATBalance;
            public string PaymentBalanceText => PaymentBalance > 0 ? "SALDO TAB_ACC_TIPPAG A CREDITO" : PaymentBalance < 0 ? "SALDO TAB_ACC_TIPPAG A DEBITO" : "SALDO TAB_ACC_TIPPAG";
        }

        public class VATPayment
        {
            public DateTime PaymentDate { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
            public decimal CompensationAmount { get; set; }
        }

        public class UnpaidRecap
        {
            public List<DetailSection>? IVABookRecaps { get; set; }
            public List<PNIVA>? Rows { get; set; }
            public decimal SalesVATTotal => (Rows?.Where(w => w.IVABookType == "V" && w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (Rows?.Where(w => w.IVABookType == "V" && w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
            public decimal SalesNoVATTotal => (Rows?.Where(w => w.IVABookType == "V" && w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (Rows?.Where(w => w.IVABookType == "V" && w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
            public decimal SalesTotal => SalesVATTotal - SalesNoVATTotal;
            public decimal PurchasesVATTotal => (Rows?.Where(w => w.IVABookType == "A" && w.N4SEGN == "+").Sum(sum => sum.N4IVEU) ?? 0) - (Rows?.Where(w => w.IVABookType == "A" && w.N4SEGN == "-").Sum(sum => sum.N4IVEU) ?? 0);
            public decimal PurchasesNoVATTotal => (Rows?.Where(w => w.IVABookType == "A" && w.N4SEGN == "+").Sum(sum => sum.N4IIEU) ?? 0) - (Rows?.Where(w => w.IVABookType == "A" && w.N4SEGN == "-").Sum(sum => sum.N4IIEU) ?? 0);
            public decimal PurchasesTotal => PurchasesVATTotal - PurchasesNoVATTotal;
            public decimal TotalBalance => SalesTotal - PurchasesTotal;
            public string TotalBalanceText => TotalBalance > 0 ? "TOTALE IVA A DEBITO" : TotalBalance < 0 ? "TOTALE IVA A CREDITO" : "TOTALE IVA";
        }

        public class UnpaidExpireRecap
        {
            public List<UnpaidExpireRecapItem>? Expires { get; set; }
            public decimal SalesTotalAmount => Expires?.Sum(sum => sum.SalesAmount) ?? 0;
            public decimal SalesTotalVATAmount => Expires?.Sum(sum => sum.SalesVATAmount) ?? 0;
            public decimal SalesTotalNoVATAmount => Expires?.Sum(sum => sum.SalesNoVATAmount) ?? 0;
            public decimal PurchasesTotalAmount => Expires?.Sum(sum => sum.PurchasesAmount) ?? 0;
            public decimal PurchasesTotalVATAmount => Expires?.Sum(sum => sum.PurchasesVATAmount) ?? 0;
            public decimal PurchasesTotalNoVATAmount => Expires?.Sum(sum => sum.PurchasesNoVATAmount) ?? 0;
            public decimal TotalBalance => SalesTotalVATAmount - SalesTotalNoVATAmount - (PurchasesTotalVATAmount - PurchasesTotalNoVATAmount);
            public string TotalBalanceText => TotalBalance > 0 ? "TOTALE IVA A DEBITO" : TotalBalance < 0 ? "TOTALE IVA A CREDITO" : "TOTALE IVA";
        }

        public class UnpaidExpireRecapItem
        {
            public DateTime ExpireDate { get; set; }
            public decimal SalesAmount { get; set; }
            public decimal SalesVATAmount { get; set; }
            public decimal SalesNoVATAmount { get; set; }
            public decimal PurchasesAmount { get; set; }
            public decimal PurchasesVATAmount { get; set; }
            public decimal PurchasesNoVATAmount { get; set; }

            #region Overrides
            public override bool Equals(object? obj)
            {
                if (obj == null)
                    return false;
                if (!(obj is UnpaidExpireRecapItem))
                    return false;
                return (obj as UnpaidExpireRecapItem)?.ExpireDate == ExpireDate;
            }

            public override int GetHashCode()
            {
                return ExpireDate.GetHashCode();
            }
            #endregion
        }
    }
}
