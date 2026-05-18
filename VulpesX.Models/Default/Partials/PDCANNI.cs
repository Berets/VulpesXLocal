namespace VulpesX.Models.Default
{
    public partial class PDCANNI
    {
        public string? CurrencyID { get; set; }
        public PDCGRUPPI? Group { get; set; }
        public PDCCONTI? Account { get; set; }
        public PDCSOTTO? Subaccount { get; set; }
        public string? YearBalanceSign { get; set; }
        public decimal YearBalance
        {
            get
            {
                if ((P4DAES ?? 0) > (P4AVES ?? 0))
                {
                    YearBalanceSign = "D";
                    return (P4DAES ?? 0) - (P4AVES ?? 0);
                }
                else
                {
                    if ((P4AVES ?? 0) > (P4DAES ?? 0))
                    {
                        YearBalanceSign = "A";
                        return (P4AVES ?? 0) - (P4DAES ?? 0);
                    }
                    else
                    {
                        YearBalanceSign = "-";
                        return 0;
                    }
                }
            }
        }
        public string? PeriodBalanceSign { get; set; }
        public decimal PeriodBalance
        {
            get
            {
                if ((P4DAPE ?? 0) > (P4AVPE ?? 0))
                {
                    PeriodBalanceSign = "D";
                    return (P4DAPE ?? 0) - (P4AVPE ?? 0);
                }
                else
                {
                    if ((P4AVPE ?? 0) > (P4DAPE ?? 0))
                    {
                        PeriodBalanceSign = "A";
                        return (P4AVPE ?? 0) - (P4DAPE ?? 0);
                    }
                    else
                    {
                        PeriodBalanceSign = "-";
                        return 0;
                    }
                }
            }
        }

        public string? TotalBalanceSign { get; set; }
        public decimal TotalBalance
        {
            get
            {
                if (YearBalanceSign == PeriodBalanceSign)
                {
                    TotalBalanceSign = YearBalanceSign;
                    return YearBalance + PeriodBalance;
                }
                else
                {
                    if (YearBalance > PeriodBalance)
                    {
                        TotalBalanceSign = YearBalanceSign;
                        return YearBalance - PeriodBalance;
                    }
                    else
                    {
                        if (PeriodBalance > YearBalance)
                        {
                            TotalBalanceSign = PeriodBalanceSign;
                            return PeriodBalance - YearBalance;
                        }
                        else
                        {
                            TotalBalanceSign = "-";
                            return 0;
                        }
                    }
                }
            }
        }

        public string? SubaccountDescription { get; set; }

        public decimal? p4deva { get; set; }
        public decimal? p4aeva { get; set; }

        public PDCANNI? Clones()
        {
            return MemberwiseClone() as PDCANNI;
        }
    }
}
