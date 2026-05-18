using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public static class AccountingHelper
    {
        public static (decimal DiscountValue, decimal SurchargeValue, decimal NetPrice) ComputePrice(decimal RowAmount, decimal? Discount1, string? DiscountType1, decimal? Discount2, string? DiscountType2, decimal? Discount3, string? DiscountType3, decimal? Surcharge, string? SurchargeType)
        {
            try
            {
                decimal Amount = RowAmount;
                decimal discount = 0;
                decimal surcharge = 0;
                if (Discount1.HasValue && Discount1.Value > 0)
                {
                    if (DiscountType1 == "V")
                    {
                        Amount -= Discount1.Value;
                        discount += Discount1.Value;
                    }
                    else
                    {
                        decimal tmp = Math.Round(Amount * Discount1.Value / 100, 2, MidpointRounding.AwayFromZero);
                        Amount -= tmp;
                        discount += tmp;
                    }
                }
                if (Discount2.HasValue && Discount2.Value > 0)
                {
                    if (DiscountType2 == "V")
                    {
                        Amount -= Discount2.Value;
                        discount += Discount2.Value;
                    }
                    else
                    {
                        decimal tmp = Math.Round(Amount * Discount2.Value / 100, 2, MidpointRounding.AwayFromZero);
                        Amount -= tmp;
                        discount += tmp;
                    }
                }
                if (Discount3.HasValue && Discount3.Value > 0)
                {
                    if (DiscountType3 == "V")
                    {
                        Amount -= Discount3.Value;
                        discount += Discount3.Value;
                    }
                    else
                    {
                        decimal tmp = Math.Round(Amount * Discount3.Value / 100, 2, MidpointRounding.AwayFromZero);
                        Amount -= tmp;
                        discount += tmp;
                    }
                }
                if (Surcharge.HasValue && Surcharge.Value > 0)
                {
                    if (SurchargeType == "V")
                    {
                        surcharge = Surcharge.Value;
                        Amount += Surcharge.Value;
                    }
                    else
                    {
                        surcharge = Math.Round(Amount * Surcharge.Value / 100, 2, MidpointRounding.AwayFromZero);
                        Amount += surcharge;
                    }
                }
                return (discount, surcharge, Math.Round(Amount, 2, MidpointRounding.AwayFromZero));
            }
            catch
            { return (0, 0, 0); }
        }

        public static bool IsDiscountPairValid(decimal? value, string? type)
        {
            bool hasValue = value.HasValue && value.Value > 0;
            bool hasType = !string.IsNullOrWhiteSpace(type);

            return (hasValue && hasType) || (!hasValue && !hasType);
        }
    }
}
