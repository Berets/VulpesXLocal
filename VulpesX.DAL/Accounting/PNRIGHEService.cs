
using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Treasury;
using VulpesX.Models.Models.Reports.Accounting;
using static VulpesX.Models.Models.Reports.Accounting.MastrinoReport;
using static VulpesX.Models.Models.Reports.Accounting.MastrinoReport.MastrinoReportItem;

namespace VulpesX.DAL.Accounting
{
    public interface IPNRIGHERepository
    {
        #region Treasury
        Tuple<decimal, decimal> GetProvvisorioEScadenzaMeseCorrente(string CompanyID, DateTime LimitDate, string Group, string Account, string Subaccount, bool OnlyAtBank);

        Tuple<decimal, decimal> GetProvvisorioEScadenzaMeseSuccessivo(string CompanyID, DateTime LimitDate, string Group, string Account, string Subaccount, bool OnlyAtBank);

        ObservableCollection<BankFluxItem>? GetBankFluxes(string CompanyID, int Year, string Group, string Account, string Subaccount);

        ObservableCollection<BankFluxMonthItem>? GetBankFluxesMonth(string CompanyID, int Year, string Group, string Account, string Subaccount);

        ObservableCollection<BankFluxMonthItem>? GetBankFluxesMonthTemporary(string CompanyID, int Year, string Group, string Account, string Subaccount);

        bool ChangeBankFlag(string CompanyID, int Year, int ID, int RowID, bool FlagValue);
        #endregion

        #region Mastrini
        ObservableCollection<PNRIGHE>? GetMastrinoAtDate(string CompanyID, int Year, string Group, string Account, string Subaccount, DateTime LimitDate);

        bool MastrinoFlagDefinitive(string CompanyID, string GroupID, string AccountID, string SubaccountID, int Year, decimal Dare, decimal Avere, DateTime FromDate, DateTime LimitDate, int? EntityID);
        #endregion

        ObservableCollection<PNRIGHE>? GetList(string CompanyID);

        ObservableCollection<PNRIGHE>? GetList(string CompanyID, int Year, int HeadNumber);

        ObservableCollection<PNRIGHE>? GetListPrint(string CompanyID, int Year, int HeadNumber);

        PNRIGHE? Get(string CompanyID, int Year, int ID);

        bool CheckPrinted(string CompanyID, int Year, int ID);

        List<Tuple<int, decimal, decimal>> GetWithSaldo(string CompanyID, string GroupID, string AccountID, DateTime From, DateTime To);

        #region Reports
        bool PrintedOnGeneralJournal(string CompanyID, int Year, int HeadNumber);

        PDCBalanceReportOpposed? PrintPDCBalanceOpposed(string CompanyID, int Year, DateTime LimitDate, bool IncludeTemp, string? CostCenterID);

        PDCBalanceReportOpposed? PrintPDCBalanceOpposedDIVA(string CompanyID, int Year, DateTime LimitDate, bool IncludeTemp, string? CostCenterID);

        MastrinoReport? PrintMastrino(string CompanyID, int Year, string? Group, string? Account, string? Subaccount, DateTime FromDate, DateTime LimitDate, ABE? Entity, bool MonthlyGroup, bool IsDefinitive);

        MastrinoReport? ReprintMastrino(string CompanyID, int Year, string? Group, string? Account, string? Subaccount, DateTime FromDate, DateTime LimitDate, ABE? Entity, bool MonthlyGroup);
        #endregion

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(PNRIGHE Model);

        bool Update(PNRIGHE Model);

        bool RemoveTemporaryFlag(PNRIGHE Model);

        bool DeleteSingle(PNRIGHE Model);

        bool DeleteByReg(string CompanyID, int Year, int Number);

        string? Validate(PNRIGHE Model, CAUCONT Causal);

        string? ValidateModel(CAUCONT SelectedCausal, ObservableCollection<PNIVA>? IVARows, ObservableCollection<PNRIGHE>? Rows, decimal Balance, CAUCONT AccountingCausal, bool IsInsert);
        #endregion
    }

    public class PNRIGHERepository : RepositoryBase, IPNRIGHERepository
    {
        public PNRIGHERepository(IConnectionFactory factory) : base(factory)
        {
        }

        #region Treasury
        public Tuple<decimal, decimal> GetProvvisorioEScadenzaMeseCorrente(string CompanyID, DateTime LimitDate, string Group, string Account, string Subaccount, bool OnlyAtBank)
        {
            try
            {
                using var connection = GetOpenConnection();

                var multi = connection.QueryMultiple(
                    @"SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE > @limitdate AND month(l.N1DARE) = month(sysdatetime())" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE > @limitdate AND month(l.N1DARE) = month(sysdatetime())" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";"),
                    new { cid = CompanyID, limitdate = LimitDate, group = Group, account = Account, subaccount = Subaccount });

                return new Tuple<decimal, decimal>((multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m), (multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new Tuple<decimal, decimal>(-1, -1);
            }
        }

        public Tuple<decimal, decimal> GetProvvisorioEScadenzaMeseSuccessivo(string CompanyID, DateTime LimitDate, string Group, string Account, string Subaccount, bool OnlyAtBank)
        {
            try
            {
                using var connection = GetOpenConnection();


                var multi = connection.QueryMultiple(
                    @"SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE > @limitdate AND ((year(l.N1DARE) = year(sysdatetime()) AND  month(l.N1DARE) > month(sysdatetime())) OR (year(l.N1DARE) > year(sysdatetime())))" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE > @limitdate AND ((year(l.N1DARE) = year(sysdatetime()) AND  month(l.N1DARE) > month(sysdatetime())) OR (year(l.N1DARE) > year(sysdatetime())))" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";"),
                    new { cid = CompanyID, limitdate = LimitDate, group = Group, account = Account, subaccount = Subaccount });

                return new Tuple<decimal, decimal>((multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m), (multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new Tuple<decimal, decimal>(-1, -1);
            }
        }

        public ObservableCollection<BankFluxItem>? GetBankFluxes(string CompanyID, int Year, string Group, string Account, string Subaccount)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<dynamic>(
                    @"SELECT l.pncaus AS CausalID, TRIM(c.caudes) AS CausalDescription, DATEPART(MONTH, l.N1DARE) AS SyntMonth, SUM(N1IMEU) As Amount FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            inner join CAUCONT AS c ON c.caucod = l.pncaus
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount
                            GROUP BY l.pncaus, c.caudes , DATEPART(MONTH, l.N1DARE)
                            ORDER BY CausalID",
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount });

                var result = new ObservableCollection<BankFluxItem>();
                foreach (var item in list)
                {
                    if (!result.Where(w => w.CausalID == item.CausalID).Any())
                    {
                        var newResult = new BankFluxItem()
                        {
                            CausalID = item.CausalID,
                            CausalDescription = item.CausalDescription
                        };
                        newResult.Amounts[item.SyntMonth - 1] = item.Amount;
                        result.Add(newResult);
                    }
                    else
                    {
                        result.Where(w => w.CausalID == item.CausalID).First().Amounts[item.SyntMonth - 1] = item.Amount;
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<BankFluxMonthItem>? GetBankFluxesMonth(string CompanyID, int Year, string Group, string Account, string Subaccount)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<dynamic>(
                    @"SELECT N1DARE AS RegDate, SUM(N1IMEU) AS Amount FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount
                            GROUP BY l.N1DARE
                            ORDER BY l.N1DARE",
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount });

                var result = new ObservableCollection<BankFluxMonthItem>() {
                        new BankFluxMonthItem(){ Month = 1, MonthDescription = "Gennaio" },
                        new BankFluxMonthItem(){ Month = 2, MonthDescription = "Febbraio" },
                        new BankFluxMonthItem(){ Month = 3, MonthDescription = "Marzo" },
                        new BankFluxMonthItem(){ Month = 4, MonthDescription = "Aprile" },
                        new BankFluxMonthItem(){ Month = 5, MonthDescription = "Maggio" },
                        new BankFluxMonthItem(){ Month = 6, MonthDescription = "Giugno" },
                        new BankFluxMonthItem(){ Month = 7, MonthDescription = "Luglio" },
                        new BankFluxMonthItem(){ Month = 8, MonthDescription = "Agosto" },
                        new BankFluxMonthItem(){ Month = 9, MonthDescription = "Settembre" },
                        new BankFluxMonthItem(){ Month = 10, MonthDescription = "Ottobre" },
                        new BankFluxMonthItem(){ Month = 11, MonthDescription = "Novembre" },
                        new BankFluxMonthItem(){ Month = 12, MonthDescription = "Dicembre" }
                    };
                foreach (var item in list)
                {
                    var existing = result.Where(w => w.Month == item.RegDate.Month).First();
                    if (item.RegDate.Day <= 10)
                    {
                        existing.Amounts[0] += item.Amount;
                    }
                    else
                    {
                        if (item.RegDate.Day > 10 && item.RegDate.Day <= 20)
                        {
                            existing.Amounts[1] += item.Amount;
                        }
                        else
                        {
                            existing.Amounts[2] += item.Amount;
                        }
                    }
                }
                // add temporary regs
                foreach (var item in GetBankFluxesMonthTemporary(CompanyID, Year, Group, Account, Subaccount) ?? new ObservableCollection<BankFluxMonthItem>())
                {
                    result.Add(item);
                }
                return new ObservableCollection<BankFluxMonthItem>(result.OrderBy(o => o.Month));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<BankFluxMonthItem>? GetBankFluxesMonthTemporary(string CompanyID, int Year, string Group, string Account, string Subaccount)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<dynamic>(
                    @"SELECT N1DARE AS RegDate, SUM(N1IMEU) AS Amount FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND l.N1TmpPN = 'S'
                            GROUP BY l.N1DARE
                            ORDER BY l.N1DARE",
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount });

                var result = new ObservableCollection<BankFluxMonthItem>() {
                        new BankFluxMonthItem(){ Month = 1, MonthDescription = "Gennaio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 2, MonthDescription = "Febbraio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 3, MonthDescription = "Marzo" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 4, MonthDescription = "Aprile" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 5, MonthDescription = "Maggio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 6, MonthDescription = "Giugno" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 7, MonthDescription = "Luglio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 8, MonthDescription = "Agosto" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 9, MonthDescription = "Settembre" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 10, MonthDescription = "Ottobre" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 11, MonthDescription = "Novembre" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 12, MonthDescription = "Dicembre" , IsTemporary = true }
                    };
                foreach (var item in list)
                {
                    var existing = result.Where(w => w.Month == item.RegDate.Month).First();
                    if (item.RegDate.Day <= 10)
                    {
                        existing.Amounts[0] += item.Amount;
                    }
                    else
                    {
                        if (item.RegDate.Day > 10 && item.RegDate.Day <= 20)
                        {
                            existing.Amounts[1] += item.Amount;
                        }
                        else
                        {
                            existing.Amounts[2] += item.Amount;
                        }
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool ChangeBankFlag(string CompanyID, int Year, int ID, int RowID, bool FlagValue)
        {
            try
            {
                using var connection = GetOpenConnection();


                connection.Query<PNRIGHE>(
                    @"UPDATE PNRIGHE 
                            SET N1FLTE = @n1flte
                            WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @regi AND N1RIGA = @rid",
                    new { cid = CompanyID, yea = Year, regi = ID, rid = RowID, n1flte = FlagValue })
                    .FirstOrDefault();
                return true;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #region Mastrini
        public ObservableCollection<PNRIGHE>? GetMastrinoAtDate(string CompanyID, int Year, string Group, string Account, string Subaccount, DateTime LimitDate)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNTESTATA, PNRIGHE, CAUCONT, string, PNRIGHE>(
                    @"SELECT t.n1dare AS N1DARE, r.*, c.caudes, cc.cedesc AS ccde
                            FROM PNRIGHE as r INNER JOIN
                            dbo.PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
				            INNER JOIN CAUCONT as c ON t.pncaus = c.caucod
                            LEFT JOIN TCECO00F AS cc ON cc.cecodc=r.N1CCCC
                            WHERE  (r.N1SOCI = @cid) AND (r.N1ANNO = @yea) AND (r.pngrup = @group) AND (r.pncont = @account) AND (r.pnsott = @subaccount) AND (r.N1IMEU IS NOT NULL) AND 
                            (r.N1IMEU <> 0) AND (t.N1DARE <= @limitdate)
				            order by t.n1dare, t.n1regi",
                    (testa, riga, caus, cc) => { testa.AccountingCausal = caus; riga.Testata = testa; riga.CostCenterDescription = cc; return riga; },
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount, limitdate = LimitDate },
                    splitOn: "N1SOCI,caudes,ccde");

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool MastrinoFlagDefinitive(string CompanyID, string GroupID, string AccountID, string SubaccountID, int Year, decimal Dare, decimal Avere, DateTime FromDate, DateTime LimitDate, int? EntityID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // update PDCANNI
                        connection.Execute(@"UPDATE PDCANNI SET P4DAPE=@dare, P4AVPE=@avere
                                                    WHERE P1SOCI = @cid AND P1GRUP=@group AND P2CONT=@account AND P3SOTC=@subaccount AND P4ANNO = @yea",
                            new { dare = Dare, avere = Avere, cid = CompanyID, group = GroupID, account = AccountID, subaccount = SubaccountID, yea = Year }, transaction);

                        // update ROWS
                        connection.Execute($@"UPDATE r 
                                                SET r.N1STMA = '*' 
                                                FROM PNRIGHE AS r
                                                INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                WHERE r.N1SOCI = @cid AND r.N1ANNO = @yea AND t.N1DARE >= @fromdate AND t.N1DARE <= @limitdate
                                                AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount
                                                {(EntityID != null ? " AND r.N1CLIE=@N1CLIE" : null)}"
                        ,
                        new { cid = CompanyID, yea = Year, group = GroupID, account = AccountID, subaccount = SubaccountID, fromdate = FromDate, limitdate = LimitDate, N1CLIE = EntityID },
                        transaction);

                        // update esercizio
                        connection.Execute("UPDATE ESERCIZIO SET eseusm = @limitdate WHERE esesoc=@cid AND eseann=@yea",
                            new { cid = CompanyID, yea = Year, limitdate = LimitDate }, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
        #endregion

        public ObservableCollection<PNRIGHE>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNRIGHE>(
                    "SELECT * FROM PNRIGHE WHERE N1SOCI = @cid ORDER BY N1RIGA",
                    new { cid = CompanyID });

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNRIGHE>? GetList(string CompanyID, int Year, int HeadNumber)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNRIGHE>(
                    @"SELECT * FROM PNRIGHE 
                        WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @numb 
                        ORDER BY N1RIGA",
                    new { cid = CompanyID, yea = Year, numb = HeadNumber });

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNRIGHE>? GetListPrint(string CompanyID, int Year, int HeadNumber)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNRIGHE, PDCSOTTO, PNRIGHE>(
                    @"SELECT r.*, s.* FROM PNRIGHE AS r
                        LEFT JOIN PDCSOTTO AS s ON s.P1GRUP = r.pngrup AND s.P2CONT = r.pncont AND s.P3SOTC = r.pnsott
                        WHERE r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1REGI = @numb 
                        ORDER BY r.N1RIGA",
                    (reg, sot) => { reg.SelectedSubaccount = sot; return reg; },
                    new { cid = CompanyID, yea = Year, numb = HeadNumber }, splitOn: "P1GRUP");

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public PNRIGHE? Get(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PNRIGHE>(
                    "SELECT * FROM PNRIGHE WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @regi",
                    new { cid = CompanyID, yea = Year, regi = ID })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool CheckPrinted(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var count = connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM PNRIGHE WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @regi AND N1STBO = '*'",
                    new { cid = CompanyID, yea = Year, regi = ID });

                return int.Parse(count?.ToString() ?? "0") > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        public List<Tuple<int, decimal, decimal>> GetWithSaldo(string CompanyID, string GroupID, string AccountID, DateTime From, DateTime To)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<int>(
                  @"select DISTINCT(r.n1clie) FROM PNRIGHE as r 
                        JOIN PNTESTATA as t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                        WHERE t.N1soci = @CompanyID AND t.N1DARE >= @From AND t.N1DARE <= @To AND r.pngrup = @GroupID AND r.pncont = @AccountID AND r.n1clie is not null order by r.n1clie",
                  new { CompanyID = CompanyID, From = From, To = To, GroupID = GroupID, AccountID = AccountID })
                  .ToList();

                var retValue = new List<Tuple<int, decimal, decimal>>();

                foreach (var lst in list)
                {
                    var dare = connection.Query<decimal?>(
                          @"select SUM(r.N1IMEU) FROM PNRIGHE as r 
                                JOIN PNTESTATA as t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                WHERE t.N1soci = @CompanyID AND t.N1DARE >= @From AND t.N1DARE <= @To AND r.pngrup = @GroupID AND r.pncont = @AccountID AND r.n1clie = @RefID AND r.N1SEGN = 'D'",
                          new { CompanyID = CompanyID, From = From, To = To, GroupID = GroupID, AccountID = AccountID, RefID = lst })
                          .FirstOrDefault();

                    var avere = connection.Query<decimal?>(
                  @"select SUM(r.N1IMEU) FROM PNRIGHE as r 
                                JOIN PNTESTATA as t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                WHERE t.N1soci = @CompanyID AND t.N1DARE >= @From AND t.N1DARE <= @To AND r.pngrup = @GroupID AND r.pncont = @AccountID AND r.n1clie = @RefID AND r.N1SEGN = 'A'",
                  new { CompanyID = CompanyID, From = From, To = To, GroupID = GroupID, AccountID = AccountID, RefID = lst })
                  .FirstOrDefault();

                    if (((dare ?? 0) - (avere ?? 0)) != 0)
                    {
                        retValue.Add(new Tuple<int, decimal, decimal>(lst, (dare ?? 0), (avere ?? 0)));
                    }
                }

                return retValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new List<Tuple<int, decimal, decimal>>();
            }
        }

        #region Reports
        public bool PrintedOnGeneralJournal(string CompanyID, int Year, int HeadNumber)
        {
            try
            {
                using var connection = GetOpenConnection();


                return ((int?)connection.ExecuteScalar(
                    @"SELECT COUNT(*) FROM PNRIGHE 
                        WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @numb AND N1STBO = '*'",
                    new { cid = CompanyID, yea = Year, numb = HeadNumber }) > 0);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        public PDCBalanceReportOpposed? PrintPDCBalanceOpposed(string CompanyID, int Year, DateTime LimitDate, bool IncludeTemp, string? CostCenterID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new PDCBalanceReportOpposed()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    DateLimit = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59),
                    SubreportAP = new PDCBalanceSubReport(),
                    SubreportCR = new PDCBalanceSubReport(),
                };

                var esercizio = connection.Query<ESERCIZIO>($@"SELECT e.* FROM ESERCIZIO as e
                                                                    WHERE e.esesoc = @cid AND e.eseann = @yea
                                                                        ORDER BY e.eseann desc", new { cid = CompanyID, yea = Year - 1 }).Select(s => s.eseest).FirstOrDefault() ?? "X";

                #region ATTIVITA'

                IEnumerable<PNRIGHE>? list = null;

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                     $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDCCONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                     new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                       $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDCCONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                       new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }

                result.SubreportAP.LeftGroup = new PDCBalanceReport()
                {
                    Description = "ATTIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                string? lastGroupID = null;
                string? lastGroupDescription = null;
                string? lastAccountID = null;
                string? lastAccountDescription = null;
                string? lastSubaccountID = null;
                string? lastSubaccountDescription = null;
                string? lastExternalCode = null;
                string? lastAlternativeCode = null;
                decimal groupProgAmount = 0;
                decimal accountProgAmount = 0;
                decimal subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                                });
                            }

                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                    });
                                }

                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "D")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "D" : "A",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }

                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                    if (item.N1SEGN == "D")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last
                if (subaccountProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                    });
                }
                #endregion

                #region PASSIVITA'

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDCCONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDCCONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }
                result.SubreportAP.RightGroup = new PDCBalanceReport()
                {
                    Description = "PASSIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastGroupID = null;
                lastGroupDescription = null;
                lastAccountID = null;
                lastAccountDescription = null;
                lastSubaccountID = null;
                lastSubaccountDescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                groupProgAmount = 0;
                accountProgAmount = 0;
                subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                                });
                            }
                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                    });
                                }
                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "A")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "A" : "D",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }
                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                    if (item.N1SEGN == "A")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last
                if (subaccountProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                    });
                }
                #endregion

                #region Compute balance A/P
                // A sign
                if (result.SubreportAP.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "A";
                    result.SubreportAP.LeftGroup.TotalAmount = result.SubreportAP.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportAP.RightGroup.TotalAmount > 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportAP.RightGroup.TotalAmount < 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "D";
                    result.SubreportAP.RightGroup.TotalAmount = result.SubreportAP.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportAP.LeftGroup.TotalAmount > result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.LeftGroup.BalanceAmount = result.SubreportAP.LeftGroup.TotalAmount - result.SubreportAP.RightGroup.TotalAmount;
                    result.SubreportAP.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.RightGroup.BalanceAmount = result.SubreportAP.RightGroup.TotalAmount - result.SubreportAP.LeftGroup.TotalAmount;
                    result.SubreportAP.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                #region COSTI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDCCONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'C' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.LeftGroup = new PDCBalanceReport()
                {
                    Description = "COSTI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastGroupID = null;
                lastGroupDescription = null;
                lastAccountID = null;
                lastAccountDescription = null;
                lastSubaccountID = null;
                lastSubaccountDescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                groupProgAmount = 0;
                accountProgAmount = 0;
                subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                                });
                            }

                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                    });
                                }

                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "D")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "D" : "A",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }

                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                    if (item.N1SEGN == "D")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last
                if (subaccountProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                    });
                }
                #endregion

                #region RICAVI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDCCONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'R' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.RightGroup = new PDCBalanceReport()
                {
                    Description = "RICAVI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastGroupID = null;
                lastGroupDescription = null;
                lastAccountID = null;
                lastAccountDescription = null;
                lastSubaccountID = null;
                lastSubaccountDescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                groupProgAmount = 0;
                accountProgAmount = 0;
                subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                                });
                            }
                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                    });
                                }
                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "A")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "A" : "D",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }
                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    if (item.N1SEGN == "A")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last 
                if (subaccountProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                    });
                }
                #endregion

                #region Compute balance C/R
                // A sign
                if (result.SubreportCR.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "A";
                    result.SubreportCR.LeftGroup.TotalAmount = result.SubreportCR.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportCR.RightGroup.TotalAmount > 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportCR.RightGroup.TotalAmount < 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "D";
                    result.SubreportCR.RightGroup.TotalAmount = result.SubreportCR.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportCR.LeftGroup.TotalAmount > result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.LeftGroup.BalanceAmount = result.SubreportCR.LeftGroup.TotalAmount - result.SubreportCR.RightGroup.TotalAmount;
                    result.SubreportCR.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.RightGroup.BalanceAmount = result.SubreportCR.RightGroup.TotalAmount - result.SubreportCR.LeftGroup.TotalAmount;
                    result.SubreportCR.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public PDCBalanceReportOpposed? PrintPDCBalanceOpposedDIVA(string CompanyID, int Year, DateTime LimitDate, bool IncludeTemp, string? CostCenterID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new PDCBalanceReportOpposed()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    DateLimit = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59),
                    SubreportAP = new PDCBalanceSubReport(),
                    SubreportCR = new PDCBalanceSubReport(),
                };

                var esercizio = connection.Query<ESERCIZIO>($@"SELECT e.* FROM ESERCIZIO as e
                                                                    WHERE e.esesoc = @cid AND e.eseann = @yea
                                                                        ORDER BY e.eseann desc", new { cid = CompanyID, yea = Year - 1 }).Select(s => s.eseest).FirstOrDefault() ?? "X";

                #region ATTIVITA'

                IEnumerable<PNRIGHE>? list = null;

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                     $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                     new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                       $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                       new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }

                result.SubreportAP.LeftGroup = new PDCBalanceReport()
                {
                    Description = "ATTIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };

                string? lastDIVADescription = null;
                string? lastExternalCode = null;
                string? lastAlternativeCode = null;
                decimal divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.DIVADescription)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }

                            lastDIVADescription = item.DIVADescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }

                // add last
                if (divaProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                #endregion

                #region PASSIVITA'

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }
                result.SubreportAP.RightGroup = new PDCBalanceReport()
                {
                    Description = "PASSIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastDIVADescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.DIVADescription)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }

                            lastDIVADescription = item.DIVADescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }
                // add last
                if (divaProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }

                #endregion

                #region Compute balance A/P
                // A sign
                if (result.SubreportAP.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "A";
                    result.SubreportAP.LeftGroup.TotalAmount = result.SubreportAP.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportAP.RightGroup.TotalAmount > 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportAP.RightGroup.TotalAmount < 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "D";
                    result.SubreportAP.RightGroup.TotalAmount = result.SubreportAP.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportAP.LeftGroup.TotalAmount > result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.LeftGroup.BalanceAmount = result.SubreportAP.LeftGroup.TotalAmount - result.SubreportAP.RightGroup.TotalAmount;
                    result.SubreportAP.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.RightGroup.BalanceAmount = result.SubreportAP.RightGroup.TotalAmount - result.SubreportAP.LeftGroup.TotalAmount;
                    result.SubreportAP.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                #region COSTI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'C' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.LeftGroup = new PDCBalanceReport()
                {
                    Description = "COSTI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastDIVADescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.pngrup)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }


                            lastDIVADescription = item.DIVADescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }
                // add last
                if (divaProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                #endregion

                #region RICAVI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'R' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.RightGroup = new PDCBalanceReport()
                {
                    Description = "RICAVI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastDIVADescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.pngrup)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode
                                });
                            }

                            lastDIVADescription = item.DIVADescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }
                // add last 
                if (divaProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                #endregion

                #region Compute balance C/R
                // A sign
                if (result.SubreportCR.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "A";
                    result.SubreportCR.LeftGroup.TotalAmount = result.SubreportCR.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportCR.RightGroup.TotalAmount > 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportCR.RightGroup.TotalAmount < 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "D";
                    result.SubreportCR.RightGroup.TotalAmount = result.SubreportCR.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportCR.LeftGroup.TotalAmount > result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.LeftGroup.BalanceAmount = result.SubreportCR.LeftGroup.TotalAmount - result.SubreportCR.RightGroup.TotalAmount;
                    result.SubreportCR.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.RightGroup.BalanceAmount = result.SubreportCR.RightGroup.TotalAmount - result.SubreportCR.LeftGroup.TotalAmount;
                    result.SubreportCR.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MastrinoReport? PrintMastrino(string CompanyID, int Year, string? Group, string? Account, string? Subaccount, DateTime FromDate, DateTime LimitDate, ABE? Entity, bool MonthlyGroup, bool IsDefinitive)
        {
            try
            {
                using var connection = GetOpenConnection();
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
                LimitDate = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59);
                var result = new MastrinoReport()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    IstantText = $"Stampato il {now.ToString("dd/MM/yyyy HH:mm:ss")}",
                    ReportTitle = $"STAMPA MASTRINI DI SOTTOCONTO ESERCIZIO {Year} DAL {FromDate.ToString("dd/MM/yyyy")} AL {LimitDate.ToString("dd/MM/yyyy")}",
                    PDCDescription = string.IsNullOrWhiteSpace(Group) ? "ALL" : $"{Group}_{Account}_{Subaccount}",
                    FromDate = FromDate,
                    ToDate = LimitDate.Date,
                    Rows = new List<MastrinoReportItem>()
                };

                var query = $@"SELECT t.n1dare AS N1DARE, r.*, c.caucod,c.caudes, g.P1GRUP, g.P1DES1, g.P1TICO, a.P2CONT, a.P2DES1, s.P3SOTC, s.P3DES1, abe.abecod, abe.abers1, abe.abers2
                            FROM PNRIGHE as r 
                            INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
				            INNER JOIN CAUCONT as c ON t.pncaus = c.caucod
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup 
                            INNER JOIN PDCCONTI AS a ON a.P1GRUP=r.pngrup AND a.P2CONT=pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=pncont AND s.P3SOTC=pnsott
                            LEFT JOIN ABE AS abe ON abe.abecod=r.N1CLIE
                            WHERE  r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND 
                            r.N1IMEU <> 0 AND t.N1DARE >= @fromdate AND t.N1DARE <= @limitdate AND (r.N1STMA <> '*' OR r.N1STMA IS NULL)
                            {(!string.IsNullOrWhiteSpace(Group) ? " AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount" : null)}
                            {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)}
				            order by r.pngrup,r.pncont,r.pnsott, t.n1dare, t.n1regi";
                var list = connection.Query<PNTESTATA, PNRIGHE, CAUCONT, PDCGRUPPI, PDCCONTI, PDCSOTTO, ABE, PNRIGHE>(
                    query,
                    (testa, riga, caus, grp, acc, sot, abe) =>
                    {
                        testa.AccountingCausal = caus;
                        riga.Testata = testa;
                        riga.Group = grp;
                        riga.Account = acc;
                        riga.Subaccount = sot;
                        riga.BasicRegistry = abe;
                        return riga;
                    },
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount, fromdate = FromDate, limitdate = LimitDate, N1CLIE = Entity?.abecod },
                    splitOn: "N1SOCI,caucod,P1GRUP,P2CONT,P3SOTC,abecod");

                var esercizioRepository = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>();
                var esercizio = esercizioRepository.Get(CompanyID, Year);
                var previousEsercizio = esercizioRepository.Get(CompanyID, Year - 1);

                foreach (var item in list.GroupBy(g => new { g.pngrup, g.pncont, g.pnsott }, (key, items) => new { key, items }))
                {
                    var newPDC = new MastrinoReportItem()
                    {
                        GroupDescription = item.items.First().Group?.FullDescriptionSearchable,
                        AccountDescription = item.items.First().Account?.FullDescriptionSearchable,
                        SubaccountDescription = item.items.First().Subaccount?.FullDescriptionSearchable,
                        Details = new List<MastrinoReportItemDetails>()
                    };

                    decimal? dareE = 0;
                    decimal? avereE = 0;
                    decimal progressive = 0;

                    #region Compute previous esercizio balance
                    if (previousEsercizio?.eseest == "A" && (item.items.First().Group?.P1TICO == "A" || item.items.First().Group?.P1TICO == "P"))
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU),0) FROM PNRIGHE AS r 
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year - 1, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, N1CLIE = Entity?.abecod });
                        dareE = multiple.Read<decimal?>().Single();
                        avereE = multiple.Read<decimal?>().Single();

                        progressive = (dareE ?? 0) - (avereE ?? 0);

                        if (progressive != 0)
                        {
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                Description = $"SALDO ESERCIZIO PRECEDENTE ({(Year - 1)})",
                                Debit = dareE ?? 0,
                                Credit = avereE ?? 0,
                                Progressive = progressive
                            });
                        }
                    }
                    #endregion

                    #region Compute previous balance

                    var startDate = new DateTime(Year, (esercizio?.eseini ?? 1), 1);
                    decimal? dareP = 0;
                    decimal? avereP = 0;
                    if (startDate != FromDate)
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, startdate = startDate, fromdate = FromDate, N1CLIE = Entity?.abecod });
                        dareP = multiple.Read<decimal?>().Single();
                        avereP = multiple.Read<decimal?>().Single();
                    }
                    progressive = (dareP ?? 0) - (avereP ?? 0);

                    if (progressive != 0)
                    {
                        newPDC.Details.Add(new MastrinoReportItemDetails()
                        {
                            Description = $"SALDO PRECEDENTE (dal {new DateTime(Year, (esercizio?.eseini ?? 1), 1).ToString("dd/MM/yyyy")} al {FromDate.ToString("dd/MM/yyyy")})",
                            Debit = dareP ?? 0,
                            Credit = avereP ?? 0,
                            Progressive = progressive
                        });
                    }
                    #endregion

                    progressive = ((dareE ?? 0) + (dareP ?? 0)) - ((avereE ?? 0) + (avereP ?? 0));

                    if (MonthlyGroup)
                    {
                        foreach (var det in item.items.OrderBy(o => o.Testata?.N1DARE).GroupBy(g => new { g.Testata?.N1DARE?.Year, g.Testata?.N1DARE?.Month }, (ikey, details) => new { ikey, details }))
                        {
                            decimal monthlyDebit = 0;
                            decimal monthlyCredit = 0;
                            foreach (var row in det.details)
                            {
                                if (row.N1SEGN == "D")
                                    progressive += row.N1IMEU ?? 0;
                                else
                                    progressive -= row.N1IMEU ?? 0;

                                newPDC.Details.Add(new MastrinoReportItemDetails()
                                {
                                    IsDefinitive = row.N1STMA,
                                    JournalRow = row.N1RIGB ?? 0,
                                    JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                    RegistrationNumber = row.N1REGI,
                                    RegistrationDateText = row.Testata?.N1DARE?.ToString("dd/MM/yyyy"),
                                    DocumentID = row.N1DOCU,
                                    ReferenceID = row.N1RIFE,
                                    Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                    Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                    Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                    Progressive = progressive
                                });
                                monthlyDebit += row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0;
                                monthlyCredit += row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0;
                            }
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit,
                                CreditMonth = monthlyCredit,
                                ProgressiveMonth = progressive,
                                MonthText = "TOTALI MESE"
                            });
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit > monthlyCredit ? monthlyDebit - monthlyCredit : 0,
                                CreditMonth = monthlyDebit < monthlyCredit ? monthlyCredit - monthlyDebit : 0,
                                ProgressiveMonth = progressive,
                                MonthText = "SALDI MESE"
                            });
                        }
                    }
                    else
                    {
                        foreach (var row in item.items.OrderBy(o => o.Testata?.N1DARE))
                        {
                            if (row.N1SEGN == "D")
                                progressive += row.N1IMEU ?? 0;
                            else
                                progressive -= row.N1IMEU ?? 0;

                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                IsDefinitive = row.N1STMA,
                                JournalRow = row.N1RIGB ?? 0,
                                JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                RegistrationNumber = row.N1REGI,
                                RegistrationDateText = row.Testata?.N1DARE?.ToString("dd/MM/yyyy"),
                                DocumentID = row.N1DOCU,
                                ReferenceID = row.N1RIFE,
                                Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                Progressive = progressive,
                                EntityDescription = row.BasicRegistry?.FullDescriptionSearchable
                            });
                        }
                    }

                    result.Rows.Add(newPDC);
                    var totalDebit = item.items.Where(w => w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    var totalCredit = item.items.Where(w => w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                    var totMast = new MastrinoReportItem()
                    {
                        MastrinoTotalText = "TOTALE MASTRINO",
                        Debit = totalDebit + dareE + dareP,
                        Credit = totalCredit + avereE + avereP,
                        Progressive = progressive
                    };
                    result.Rows.Add(totMast);
                    result.Rows.Add(new MastrinoReportItem()
                    {
                        MastrinoTotalText = "SALDO MASTRINO",
                        Debit = totMast.Debit > totMast.Credit ? totMast.Debit - totMast.Credit : 0,
                        Credit = totMast.Debit < totMast.Credit ? totMast.Credit - totMast.Debit : 0,
                        Progressive = progressive
                    });
                    if (IsDefinitive)
                    {
                        if (!MastrinoFlagDefinitive(CompanyID, item.items.First().Group?.P1GRUP ?? string.Empty, item.items.First().Account?.P2CONT ?? string.Empty, item.items.First()?.Subaccount?.P3SOTC ?? string.Empty, Year, totalDebit, totalCredit, FromDate, LimitDate, Entity?.abecod))
                        {
                            ErrorHandler.Show("Errore durante l'aggiornamento dei dati per la stampa DEFINITIVA, ripetere l'operazione");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MastrinoReport? ReprintMastrino(string CompanyID, int Year, string? Group, string? Account, string? Subaccount, DateTime FromDate, DateTime LimitDate, ABE? Entity, bool MonthlyGroup)
        {
            try
            {
                using var connection = GetOpenConnection();
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
                LimitDate = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59);
                var result = new MastrinoReport()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    IstantText = $"Stampato il {now.ToString("dd/MM/yyyy HH:mm:ss")}",
                    ReportTitle = $"STAMPA MASTRINI DI SOTTOCONTO ESERCIZIO {Year} DAL {FromDate.ToString("dd/MM/yyyy")} AL {LimitDate.ToString("dd/MM/yyyy")}",
                    PDCDescription = string.IsNullOrWhiteSpace(Group) ? "ALL" : $"{Group}_{Account}_{Subaccount}",
                    FromDate = FromDate,
                    ToDate = LimitDate.Date,
                    Rows = new List<MastrinoReportItem>()
                };


                var query = $@"SELECT t.n1dare AS N1DARE, r.*, c.caucod,c.caudes, g.P1GRUP, g.P1DES1, g.P1TICO, a.P2CONT, a.P2DES1, s.P3SOTC, s.P3DES1, abe.abecod, abe.abers1, abe.abers2
                            FROM PNRIGHE as r 
                            INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
				            INNER JOIN CAUCONT as c ON t.pncaus = c.caucod
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=r.pngrup 
                            INNER JOIN PDCCONTI AS a ON a.P1GRUP=r.pngrup AND a.P2CONT=pncont
                            INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=pncont AND s.P3SOTC=pnsott
                            LEFT JOIN ABE AS abe ON abe.abecod=r.N1CLIE
                            WHERE  r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND 
                            r.N1IMEU <> 0 AND t.N1DARE >= @fromdate AND t.N1DARE <= @limitdate
                            {(!string.IsNullOrWhiteSpace(Group) ? " AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount" : null)}
                            {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)}
				            order by r.pngrup,r.pncont,r.pnsott, t.n1dare, t.n1regi";
                var list = connection.Query<PNTESTATA, PNRIGHE, CAUCONT, PDCGRUPPI, PDCCONTI, PDCSOTTO, ABE, PNRIGHE>(
                    query,
                    (testa, riga, caus, grp, acc, sot, abe) =>
                    {
                        testa.AccountingCausal = caus;
                        riga.Testata = testa;
                        riga.Group = grp;
                        riga.Account = acc;
                        riga.Subaccount = sot;
                        riga.BasicRegistry = abe;
                        return riga;
                    },
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount, fromdate = FromDate, limitdate = LimitDate, N1CLIE = Entity?.abecod },
                    splitOn: "N1SOCI,caucod,P1GRUP,P2CONT,P3SOTC,abecod");

                var esercizioRepository = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>();
                var esercizio = esercizioRepository.Get(CompanyID, Year)!;
                var previousEsercizio = esercizioRepository.Get(CompanyID, Year - 1);

                foreach (var item in list.GroupBy(g => new { g.pngrup, g.pncont, g.pnsott, g.n1clie }, (key, items) => new { key, items }))
                {
                    var newPDC = new MastrinoReportItem()
                    {
                        GroupDescription = item.items.First().Group?.FullDescriptionSearchable,
                        AccountDescription = item.items.First().Account?.FullDescriptionSearchable,
                        SubaccountDescription = item.items.First().Subaccount?.FullDescriptionSearchable,
                        Details = new List<MastrinoReportItemDetails>()
                    };

                    decimal? dareE = 0;
                    decimal? avereE = 0;
                    decimal progressive = 0;

                    #region Compute previous esercizio balance
                    if (previousEsercizio?.eseest == "A" && (item.items.First().Group?.P1TICO == "A" || item.items.First().Group?.P1TICO == "P"))
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU),0) FROM PNRIGHE AS r 
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year - 1, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, N1CLIE = Entity?.abecod });
                        dareE = multiple.Read<decimal?>().Single();
                        avereE = multiple.Read<decimal?>().Single();

                        progressive = (dareE ?? 0) - (avereE ?? 0);

                        if (progressive != 0)
                        {
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                Description = $"SALDO ESERCIZIO PRECEDENTE ({(Year - 1)})",
                                Debit = dareE ?? 0,
                                Credit = avereE ?? 0,
                                Progressive = progressive
                            });
                        }
                    }
                    #endregion

                    #region Compute previous balance

                    var startDate = new DateTime(Year, (esercizio.eseini ?? 1), 1);
                    decimal? dareP = 0;
                    decimal? avereP = 0;
                    if (startDate != FromDate)
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, startdate = startDate, fromdate = FromDate, N1CLIE = Entity?.abecod });
                        dareP = multiple.Read<decimal?>().Single();
                        avereP = multiple.Read<decimal?>().Single();
                    }
                    progressive = (dareP ?? 0) - (avereP ?? 0);

                    if (progressive != 0)
                    {
                        newPDC.Details.Add(new MastrinoReportItemDetails()
                        {
                            Description = $"SALDO PRECEDENTE (dal {new DateTime(Year, (esercizio.eseini ?? 1), 1).ToString("dd/MM/yyyy")} al {FromDate.ToString("dd/MM/yyyy")})",
                            Debit = dareP ?? 0,
                            Credit = avereP ?? 0,
                            Progressive = progressive
                        });
                    }
                    #endregion

                    progressive = ((dareE ?? 0) + (dareP ?? 0)) - ((avereE ?? 0) + (avereP ?? 0));

                    if (MonthlyGroup)
                    {
                        foreach (var det in item.items.OrderBy(o => o.Testata?.N1DARE).GroupBy(g => new { (g.Testata?.N1DARE ?? DateTime.MinValue).Year, (g.Testata?.N1DARE ?? DateTime.MinValue).Month }, (ikey, details) => new { ikey, details }))
                        {
                            decimal monthlyDebit = 0;
                            decimal monthlyCredit = 0;
                            foreach (var row in det.details)
                            {
                                if (row.N1SEGN == "D")
                                    progressive += row.N1IMEU ?? 0;
                                else
                                    progressive -= row.N1IMEU ?? 0;

                                newPDC.Details.Add(new MastrinoReportItemDetails()
                                {
                                    IsDefinitive = row.N1STMA,
                                    JournalRow = row.N1RIGB ?? 0,
                                    JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                    RegistrationNumber = row.N1REGI,
                                    RegistrationDateText = (row.Testata?.N1DARE ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                                    DocumentID = row.N1DOCU,
                                    ReferenceID = row.N1RIFE,
                                    Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                    Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                    Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                    Progressive = progressive
                                });
                                monthlyDebit += row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0;
                                monthlyCredit += row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0;
                            }
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit,
                                CreditMonth = monthlyCredit,
                                ProgressiveMonth = progressive,
                                MonthText = "TOTALI MESE"
                            });
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit > monthlyCredit ? monthlyDebit - monthlyCredit : 0,
                                CreditMonth = monthlyDebit < monthlyCredit ? monthlyCredit - monthlyDebit : 0,
                                ProgressiveMonth = progressive,
                                MonthText = "SALDI MESE"
                            });
                        }
                    }
                    else
                    {
                        foreach (var row in item.items.OrderBy(o => o.Testata?.N1DARE))
                        {
                            if (row.N1SEGN == "D")
                                progressive += row.N1IMEU ?? 0;
                            else
                                progressive -= row.N1IMEU ?? 0;

                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                IsDefinitive = row.N1STMA,
                                JournalRow = row.N1RIGB ?? 0,
                                JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                RegistrationNumber = row.N1REGI,
                                RegistrationDateText = (row.Testata?.N1DARE ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                                DocumentID = row.N1DOCU,
                                ReferenceID = row.N1RIFE,
                                Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                Progressive = progressive,
                                EntityDescription = row.BasicRegistry?.FullDescriptionSearchable
                            });
                        }
                    }

                    result.Rows.Add(newPDC);
                    var totalDebit = item.items.Where(w => w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    var totalCredit = item.items.Where(w => w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                    var totMast = new MastrinoReportItem()
                    {
                        MastrinoTotalText = "TOTALE MASTRINO",
                        Debit = totalDebit + dareE + dareP,
                        Credit = totalCredit + avereE + avereP,
                        Progressive = progressive
                    };
                    result.Rows.Add(totMast);
                    result.Rows.Add(new MastrinoReportItem()
                    {
                        MastrinoTotalText = "SALDO MASTRINO",
                        Debit = totMast.Debit > totMast.Credit ? totMast.Debit - totMast.Credit : 0,
                        Credit = totMast.Debit < totMast.Credit ? totMast.Credit - totMast.Debit : 0,
                        Progressive = progressive
                    });
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO PNRIGHE (N1SOCI,N1ANNO,N1REGI,N1RIGA,N1DOCU,N1RIFE,N1DADO,N1DARI,pngrup,pncont,pnsott,N1IMPO,N1DESC,N1SEGN,n1clie,N1CCCC,N1CCCS,N1CONP,N1COMM,N1DEST,N1GIRO,N1STNO,N1STBO,N1STMA,N1CHIU,N1RIGB,N1DABB,N1DASM,N1DACC,N1FIVA,N1BC01,N1BC02,N1BC03,N1BC04,N1BC05,N1IMEU,N1TIDO,n1scad,n1paga,N1DRri,N1SCON,N1DIVI,N1CAMB,N1IMVA,N1FLCO,N1tmpPNR,n1mese,N1FLTE) OUTPUT INSERTED.rv VALUES(@N1SOCI,@N1ANNO,@N1REGI,@N1RIGA,@N1DOCU,@N1RIFE,@N1DADO,@N1DARI,@pngrup,@pncont,@pnsott,@N1IMPO,@N1DESC,@N1SEGN,@n1clie,@N1CCCC,@N1CCCS,@N1CONP,@N1COMM,@N1DEST,@N1GIRO,@N1STNO,@N1STBO,@N1STMA,@N1CHIU,@N1RIGB,@N1DABB,@N1DASM,@N1DACC,@N1FIVA,@N1BC01,@N1BC02,@N1BC03,@N1BC04,@N1BC05,@N1IMEU,@N1TIDO,@n1scad,@n1paga,@N1DRri,@N1SCON,@N1DIVI,@N1CAMB,@N1IMVA,@N1FLCO,@N1tmpPNR,@n1mese,@N1FLTE)";
        public string UPDATE_QUERY => "UPDATE PNRIGHE SET N1SOCI = @N1SOCI,N1ANNO = @N1ANNO,N1REGI = @N1REGI,N1RIGA = @N1RIGA,N1DOCU = @N1DOCU,N1RIFE = @N1RIFE,N1DADO = @N1DADO,N1DARI = @N1DARI,pngrup = @pngrup,pncont = @pncont,pnsott = @pnsott,N1IMPO = @N1IMPO,N1DESC = @N1DESC,N1SEGN = @N1SEGN,n1clie = @n1clie,N1CCCC = @N1CCCC,N1CCCS = @N1CCCS,N1CONP = @N1CONP,N1COMM = @N1COMM,N1DEST = @N1DEST,N1GIRO = @N1GIRO,N1STNO = @N1STNO,N1STBO = @N1STBO,N1STMA = @N1STMA,N1CHIU = @N1CHIU,N1RIGB = @N1RIGB,N1DABB = @N1DABB,N1DASM = @N1DASM,N1DACC = @N1DACC,N1FIVA = @N1FIVA,N1BC01 = @N1BC01,N1BC02 = @N1BC02,N1BC03 = @N1BC03,N1BC04 = @N1BC04,N1BC05 = @N1BC05,N1IMEU = @N1IMEU,N1TIDO = @N1TIDO,n1scad = @n1scad,n1paga = @n1paga,N1DRri = @N1DRri,N1SCON = @N1SCON,N1DIVI = @N1DIVI,N1CAMB = @N1CAMB,N1IMVA = @N1IMVA,N1FLCO = @N1FLCO,N1tmpPNR = @N1tmpPNR,n1mese = @n1mese,N1FLTE = @N1FLTE OUTPUT INSERTED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND N1RIGA = @N1RIGA AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PNRIGHE OUTPUT DELETED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND N1RIGA = @N1RIGA AND rv = @rv";
        public bool Insert(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(INSERT_QUERY, Model);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
                if (result != null)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool RemoveTemporaryFlag(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    var resultUpdate = connection.Execute(@"UPDATE PNRIGHE SET N1tmpPNR = 'N'  
                                                                WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi",
                        Model,
                        transaction: transaction);

                    if (resultUpdate > 0)
                    {
                        var resultUpdateHead = connection.ExecuteScalar(@"UPDATE PNTESTATA SET N1tmpPN = 'N' OUTPUT INSERTED.rv 
                                                                WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi AND rv = @rv",
                        Model.Testata,
                        transaction: transaction);

                        if (resultUpdateHead != null)
                        {
                            decimal amount = 0;
                            foreach (var row in connection.Query<PNRIGHE>(@"SELECT * FROM PNRIGHE
                                                                                WHERE N1SOCI=@N1SOCI AND N1ANNO=@N1ANNO AND N1REGI=@N1REGI AND pngrup=@pngrup AND pncont=@pncont AND pnsott=@pnsott",
                                                                            Model, transaction))
                            {
                                if (row.N1SEGN == "D")
                                    amount += (row.N1IMEU ?? 0);
                                else
                                    amount -= (row.N1IMEU ?? 0);
                            }

                            var rbccItem = VulpesServiceProvider.Provider.GetRequiredService<IRBCC01F0Repository>().GetByPDC(Model.N1SOCI, Model.pngrup ?? string.Empty, Model.pncont ?? string.Empty, Model.pnsott ?? string.Empty);
                            var resultBalance = connection.ExecuteScalar(@"UPDATE RBCC01F0 SET cnsl17 = cnsl17 + @amount OUTPUT INSERTED.rv 
                                                                WHERE cnsl34 = @cnsl34 AND cnsl01 = @cnsl01 AND cnsl02 = @cnsl02 AND cnsl05 = @cnsl05 AND rv = @rv",
                            new { cnsl34 = rbccItem?.cnsl34, cnsl01 = rbccItem?.cnsl01, cnsl02 = rbccItem?.cnsl02, cnsl05 = rbccItem?.cnsl05, rv = rbccItem?.rv, amount = amount },
                            transaction: transaction);

                            if (resultBalance != null)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                                return false;
                            }
                        }
                        else
                        {
                            ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                            return false;
                        }
                    }
                    else
                    {
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }



            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteSingle(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(DELETE_QUERY, Model);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteByReg(string CompanyID, int Year, int Number)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(
                    "DELETE FROM PNRIGHE WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi",
                    new { n1soci = CompanyID, n1anno = Year, n1regi = Number });
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(PNRIGHE Model, CAUCONT Causal)
        {
            if (Model != null)
            {
                if (Model.N1IMEU.HasValue && Model.N1IMEU.Value >= 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.N1SEGN))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.SelectedGroup?.P1GRUP))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.SelectedAccount?.P2CONT))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3SOTC))
                                {
                                    // check existing PDCANNI
                                    var pdcAnni = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Get(Model.N1SOCI, Model.SelectedGroup?.P1GRUP ?? string.Empty, Model.SelectedAccount?.P2CONT ?? string.Empty, Model.SelectedSubaccount?.P3SOTC ?? string.Empty, Model.N1ANNO);
                                    if (pdcAnni != null)
                                    {
                                        int testDoc = 0;
                                        if ((Causal.cauivaBool && int.TryParse(Model.N1DOCU, out testDoc)) ||
                                            !Causal.cauivaBool)
                                        {
                                            if (Model.SelectedSubaccount == null || (Model.SelectedSubaccount != null &&
                                                (string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3CLFO) || (!string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3CLFO) && Model.SelectedEntity != null && Model.SelectedEntity.abecod > 0))))
                                            {
                                                if (Model.SelectedEntity == null || (Model.SelectedEntity != null && Model.SelectedEntity.abecod > 0 && Model.SelectedPayment != null))
                                                {
                                                    return null;
                                                }
                                                else
                                                {
                                                    return "In caso di cliente/fornitore il codice pagamento è obbligatorio";
                                                }
                                            }
                                            else
                                            {
                                                return "Il sottoconto prevede obbligatoriamente un codice cliente/fornitore";
                                            }
                                        }
                                        else
                                        {
                                            return "La causale contabile prevede IVA quindi il numero documento deve contenere solo numeri e nessun carattere";
                                        }
                                    }
                                    else
                                    {
                                        return "Il sottoconto anagrafico non presenta un sottoconto contabile. verificare l'apertura dell'esercizio contabile";
                                    }
                                }
                                else
                                {
                                    return "Il sottoconto contabile è obbligatorio";
                                }
                            }
                            else
                            {
                                return "Il conto contabile è obbligatorio";
                            }
                        }
                        else
                        {
                            return "Il gruppo contabile è obbligatorio";
                        }
                    }
                    else
                    {
                        return "Il segno è obbligatorio";
                    }
                }
                else
                {
                    return "L'importo deve essere maggiore o uguale a 0";
                }
            }
            else
            {
                return "Errore sconosciuto, impossibile proseguire";
            }
        }

        public string? ValidateModel(CAUCONT SelectedCausal, ObservableCollection<PNIVA>? IVARows, ObservableCollection<PNRIGHE>? Rows, decimal Balance, CAUCONT AccountingCausal, bool IsInsert)
        {

            if ((SelectedCausal.cauivaBool && IVARows != null && IVARows.Count > 0) ||
                (!SelectedCausal.cauivaBool && (IVARows == null || IVARows.Count == 0)))
            {
                if ((!SelectedCausal.causolBool && Rows != null && Rows.Count > 0) ||
                    SelectedCausal.causolBool)
                {
                    // check 0 amounts
                    if (!(Rows ?? new ObservableCollection<PNRIGHE>()).Any(any => any.N1IMEU == 0) || SelectedCausal.cauzer)
                    {
                        // check PDC
                        if (!(Rows ?? new ObservableCollection<PNRIGHE>()).Any(any => string.IsNullOrWhiteSpace(any.pngrup) || string.IsNullOrWhiteSpace(any.pncont) || string.IsNullOrWhiteSpace(any.pnsott)))
                        {
                            if (Balance == 0)
                            {
                                var ivaFlaggedTotalDare = Rows?.Where(w => w.SelectedSubaccount?.p3coniBool ?? false && w.N1SEGN == "D").Sum(sum => sum.N1IMEU);
                                var ivaFlaggedTotalAvere = Rows?.Where(w => w.SelectedSubaccount?.p3coniBool ?? false && w.N1SEGN == "A").Sum(sum => sum.N1IMEU);
                                var ivaFlaggedTotal = ivaFlaggedTotalDare - ivaFlaggedTotalAvere;
                                if (ivaFlaggedTotal < 0)
                                    ivaFlaggedTotal *= -1;
                                var ivaTotalPlus = IVARows?.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IVEU);
                                var ivaTotalMinus = IVARows?.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IVEU);
                                var ivaTotal = ivaTotalPlus - ivaTotalMinus;
                                if (ivaTotal < 0)
                                    ivaTotal *= -1;
                                if (SelectedCausal.causolBool ||
                                    (AccountingCausal.cauivaBool && ivaFlaggedTotal == ivaTotal) ||
                                    !AccountingCausal.cauivaBool ||
                                    (AccountingCausal.cauivaBool && ivaFlaggedTotal != ivaTotal && ConfirmHandler.Confirm($"Il totale delle righe con conto IVA ({(ivaFlaggedTotal ?? 0).ToString("N2")}) e quello delle righe IVA ({(ivaTotal ?? 0).ToString("N2")}) non coincidono, proseguire comunque ?")))
                                {
                                    // check sectional amount
                                    var dareAmount = Rows?.Where(w => w.N1SEGN == "D" && (w.n1clie.HasValue && w.n1clie.Value > 0)).Sum(sum => sum.N1IMEU) ?? 0;
                                    var avereAmount = Rows?.Where(w => w.N1SEGN == "A" && (w.n1clie.HasValue && w.n1clie.Value > 0)).Sum(sum => sum.N1IMEU) ?? 0;
                                    decimal dareDetails = 0;
                                    foreach (var item in (Rows ?? new ObservableCollection<PNRIGHE>()).Where(w => w.N1SEGN == "D" && (w.n1clie.HasValue && w.n1clie.Value > 0)))
                                    {
                                        if (item.ExpireRows != null && item.ExpireRows.Count > 0)
                                            dareDetails += item.ExpireRows.Sum(sum => sum.Amount);
                                    }
                                    decimal avereDetails = 0;
                                    foreach (var item in (Rows ?? new ObservableCollection<PNRIGHE>()).Where(w => w.N1SEGN == "A" && (w.n1clie.HasValue && w.n1clie.Value > 0)))
                                    {
                                        if (item.ExpireRows != null && item.ExpireRows.Count > 0)
                                            avereDetails += item.ExpireRows.Sum(sum => sum.Amount);
                                    }
                                    if (IsInsert || (dareAmount == dareDetails && avereAmount == avereDetails))
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        return "Il totale delle righe cliente/fornitore e le relative scadenze nel sezionale non corrispondono";
                                    }
                                }
                                else
                                {
                                    return "Il totale delle righe con conto IVA e quello delle righe IVA non coincidono";
                                }
                            }
                            else
                            {
                                return "La registrazione è sbilanciata, impossibile proseguire";
                            }
                        }
                        else
                        {
                            return "Il riferimento del piano dei conti è obbligatorio, c'è almeno una riga senza gruppo, conto e/o sottoconto";
                        }
                    }
                    else
                    {
                        return "Ci sono delle righe con importo uguale a 0. Tutte le righe devono avere un importo superiore a 0.";
                    }
                }
                else
                {
                    return "E' necessario che siano presenti delle righe per confermare la registrazione";
                }
            }
            else
            {
                return "Se la causale prevede gestione IVA devono esserci delle righe valide, altrimenti non ci devono essere righe IVA";
            }
        }
        #endregion
    }

    public class PNRIGHEUfpRepository : RepositoryBase, IPNRIGHERepository
    {
        public PNRIGHEUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        #region Treasury
        public Tuple<decimal, decimal> GetProvvisorioEScadenzaMeseCorrente(string CompanyID, DateTime LimitDate, string Group, string Account, string Subaccount, bool OnlyAtBank)
        {
            try
            {
                using var connection = GetOpenConnection();

                var multi = connection.QueryMultiple(
                    @"SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE > @limitdate AND month(l.N1DARE) = month(sysdatetime())" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE > @limitdate AND month(l.N1DARE) = month(sysdatetime())" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";"),
                    new { cid = CompanyID, limitdate = LimitDate, group = Group, account = Account, subaccount = Subaccount });

                return new Tuple<decimal, decimal>((multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m), (multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new Tuple<decimal, decimal>(-1, -1);
            }
        }

        public Tuple<decimal, decimal> GetProvvisorioEScadenzaMeseSuccessivo(string CompanyID, DateTime LimitDate, string Group, string Account, string Subaccount, bool OnlyAtBank)
        {
            try
            {
                using var connection = GetOpenConnection();


                var multi = connection.QueryMultiple(
                    @"SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE <= @limitdate" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='D' AND l.N1DARE > @limitdate AND ((year(l.N1DARE) = year(sysdatetime()) AND  month(l.N1DARE) > month(sysdatetime())) OR (year(l.N1DARE) > year(sysdatetime())))" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";") +
                      "SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r INNER JOIN PNTESTATA AS l ON l.N1SOCI = r.N1SOCI AND l.N1ANNO = r.N1ANNO AND l.N1REGI = r.N1REGI WHERE r.N1SOCI=@cid AND r.pngrup=@group AND r.pncont=@account AND r.pnsott=@subaccount AND r.N1tmpPNR='S' AND r.N1SEGN='A' AND l.N1DARE > @limitdate AND ((year(l.N1DARE) = year(sysdatetime()) AND  month(l.N1DARE) > month(sysdatetime())) OR (year(l.N1DARE) > year(sysdatetime())))" + (OnlyAtBank ? " AND r.N1FLTE = 1;" : ";"),
                    new { cid = CompanyID, limitdate = LimitDate, group = Group, account = Account, subaccount = Subaccount });

                return new Tuple<decimal, decimal>((multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m), (multi.Read<decimal?>().Single() ?? 0m) - (multi.Read<decimal?>().Single() ?? 0m));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new Tuple<decimal, decimal>(-1, -1);
            }
        }

        public ObservableCollection<BankFluxItem>? GetBankFluxes(string CompanyID, int Year, string Group, string Account, string Subaccount)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<dynamic>(
                    @"SELECT l.pncaus AS CausalID, TRIM(c.caudes) AS CausalDescription, DATEPART(MONTH, l.N1DARE) AS SyntMonth, SUM(N1IMEU) As Amount FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            inner join CAUCONT AS c ON c.caucod = l.pncaus
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount
                            GROUP BY l.pncaus, c.caudes , DATEPART(MONTH, l.N1DARE)
                            ORDER BY CausalID",
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount });

                var result = new ObservableCollection<BankFluxItem>();
                foreach (var item in list)
                {
                    if (!result.Where(w => w.CausalID == item.CausalID).Any())
                    {
                        var newResult = new BankFluxItem()
                        {
                            CausalID = item.CausalID,
                            CausalDescription = item.CausalDescription
                        };
                        newResult.Amounts[item.SyntMonth - 1] = item.Amount;
                        result.Add(newResult);
                    }
                    else
                    {
                        result.Where(w => w.CausalID == item.CausalID).First().Amounts[item.SyntMonth - 1] = item.Amount;
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<BankFluxMonthItem>? GetBankFluxesMonth(string CompanyID, int Year, string Group, string Account, string Subaccount)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<dynamic>(
                    @"SELECT N1DARE AS RegDate, SUM(N1IMEU) AS Amount FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount
                            GROUP BY l.N1DARE
                            ORDER BY l.N1DARE",
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount });

                var result = new ObservableCollection<BankFluxMonthItem>() {
                        new BankFluxMonthItem(){ Month = 1, MonthDescription = "Gennaio" },
                        new BankFluxMonthItem(){ Month = 2, MonthDescription = "Febbraio" },
                        new BankFluxMonthItem(){ Month = 3, MonthDescription = "Marzo" },
                        new BankFluxMonthItem(){ Month = 4, MonthDescription = "Aprile" },
                        new BankFluxMonthItem(){ Month = 5, MonthDescription = "Maggio" },
                        new BankFluxMonthItem(){ Month = 6, MonthDescription = "Giugno" },
                        new BankFluxMonthItem(){ Month = 7, MonthDescription = "Luglio" },
                        new BankFluxMonthItem(){ Month = 8, MonthDescription = "Agosto" },
                        new BankFluxMonthItem(){ Month = 9, MonthDescription = "Settembre" },
                        new BankFluxMonthItem(){ Month = 10, MonthDescription = "Ottobre" },
                        new BankFluxMonthItem(){ Month = 11, MonthDescription = "Novembre" },
                        new BankFluxMonthItem(){ Month = 12, MonthDescription = "Dicembre" }
                    };
                foreach (var item in list)
                {
                    var existing = result.Where(w => w.Month == item.RegDate.Month).First();
                    if (item.RegDate.Day <= 10)
                    {
                        existing.Amounts[0] += item.Amount;
                    }
                    else
                    {
                        if (item.RegDate.Day > 10 && item.RegDate.Day <= 20)
                        {
                            existing.Amounts[1] += item.Amount;
                        }
                        else
                        {
                            existing.Amounts[2] += item.Amount;
                        }
                    }
                }
                // add temporary regs
                foreach (var item in GetBankFluxesMonthTemporary(CompanyID, Year, Group, Account, Subaccount) ?? new ObservableCollection<BankFluxMonthItem>())
                {
                    result.Add(item);
                }
                return new ObservableCollection<BankFluxMonthItem>(result.OrderBy(o => o.Month));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<BankFluxMonthItem>? GetBankFluxesMonthTemporary(string CompanyID, int Year, string Group, string Account, string Subaccount)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<dynamic>(
                    @"SELECT N1DARE AS RegDate, SUM(N1IMEU) AS Amount FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND l.N1TmpPN = 'S'
                            GROUP BY l.N1DARE
                            ORDER BY l.N1DARE",
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount });

                var result = new ObservableCollection<BankFluxMonthItem>() {
                        new BankFluxMonthItem(){ Month = 1, MonthDescription = "Gennaio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 2, MonthDescription = "Febbraio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 3, MonthDescription = "Marzo" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 4, MonthDescription = "Aprile" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 5, MonthDescription = "Maggio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 6, MonthDescription = "Giugno" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 7, MonthDescription = "Luglio" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 8, MonthDescription = "Agosto" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 9, MonthDescription = "Settembre" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 10, MonthDescription = "Ottobre" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 11, MonthDescription = "Novembre" , IsTemporary = true },
                        new BankFluxMonthItem(){ Month = 12, MonthDescription = "Dicembre" , IsTemporary = true }
                    };
                foreach (var item in list)
                {
                    var existing = result.Where(w => w.Month == item.RegDate.Month).First();
                    if (item.RegDate.Day <= 10)
                    {
                        existing.Amounts[0] += item.Amount;
                    }
                    else
                    {
                        if (item.RegDate.Day > 10 && item.RegDate.Day <= 20)
                        {
                            existing.Amounts[1] += item.Amount;
                        }
                        else
                        {
                            existing.Amounts[2] += item.Amount;
                        }
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool ChangeBankFlag(string CompanyID, int Year, int ID, int RowID, bool FlagValue)
        {
            try
            {
                using var connection = GetOpenConnection();


                connection.Query<PNRIGHE>(
                    @"UPDATE PN_RIGHE 
                            SET N1FLTE = @n1flte
                            WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @regi AND N1RIGA = @rid",
                    new { cid = CompanyID, yea = Year, regi = ID, rid = RowID, n1flte = FlagValue })
                    .FirstOrDefault();
                return true;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #region Mastrini
        public ObservableCollection<PNRIGHE>? GetMastrinoAtDate(string CompanyID, int Year, string Group, string Account, string Subaccount, DateTime LimitDate)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNTESTATA, PNRIGHE, CAUCONT, string, PNRIGHE>(
                    @"SELECT t.n1dare AS N1DARE, r.*, c.caudes, cc.cedesc AS ccde
                            FROM PN_RIGHE as r INNER JOIN
                            dbo.PN_TESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
				            INNER JOIN CAUCONT as c ON t.pncaus = c.caucod
                            LEFT JOIN TCECO00F AS cc ON cc.cecodc=r.N1CCCC
                            WHERE  (r.N1SOCI = @cid) AND (r.N1ANNO = @yea) AND (r.pngrup = @group) AND (r.pncont = @account) AND (r.pnsott = @subaccount) AND (r.N1IMEU IS NOT NULL) AND 
                            (r.N1IMEU <> 0) AND (t.N1DARE <= @limitdate)
				            order by t.n1dare, t.n1regi",
                    (testa, riga, caus, cc) => { testa.AccountingCausal = caus; riga.Testata = testa; riga.CostCenterDescription = cc; return riga; },
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount, limitdate = LimitDate },
                    splitOn: "N1SOCI,caudes,ccde");

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool MastrinoFlagDefinitive(string CompanyID, string GroupID, string AccountID, string SubaccountID, int Year, decimal Dare, decimal Avere, DateTime FromDate, DateTime LimitDate, int? EntityID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // update PDCANNI
                        connection.Execute(@"UPDATE PDC_ANNI SET P4DAPE=@dare, P4AVPE=@avere
                                                    WHERE P1SOCI = @cid AND P1GRUP=@group AND P2CONT=@account AND P3SOTC=@subaccount AND P4ANNO = @yea",
                            new { dare = Dare, avere = Avere, cid = CompanyID, group = GroupID, account = AccountID, subaccount = SubaccountID, yea = Year }, transaction);

                        // update ROWS
                        connection.Execute($@"UPDATE r 
                                                SET r.N1STMA = '*' 
                                                FROM PN_RIGHE AS r
                                                INNER JOIN PN_TESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                WHERE r.N1SOCI = @cid AND r.N1ANNO = @yea AND t.N1DARE >= @fromdate AND t.N1DARE <= @limitdate
                                                AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount
                                                {(EntityID != null ? " AND r.N1CLIE=@N1CLIE" : null)}"
                        ,
                        new { cid = CompanyID, yea = Year, group = GroupID, account = AccountID, subaccount = SubaccountID, fromdate = FromDate, limitdate = LimitDate, N1CLIE = EntityID },
                        transaction);

                        // update esercizio
                        connection.Execute("UPDATE ESERCIZIO SET eseusm = @limitdate WHERE esesoc=@cid AND eseann=@yea",
                            new { cid = CompanyID, yea = Year, limitdate = LimitDate }, transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
        #endregion

        public ObservableCollection<PNRIGHE>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNRIGHE>(
                    "SELECT * FROM PN_RIGHE WHERE N1SOCI = @cid ORDER BY N1RIGA",
                    new { cid = CompanyID });

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNRIGHE>? GetList(string CompanyID, int Year, int HeadNumber)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNRIGHE>(
                    @"SELECT * FROM PN_RIGHE 
                        WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @numb 
                        ORDER BY N1RIGA",
                    new { cid = CompanyID, yea = Year, numb = HeadNumber });

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNRIGHE>? GetListPrint(string CompanyID, int Year, int HeadNumber)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNRIGHE, PDCSOTTO, PNRIGHE>(
                    @"SELECT r.*, s.* FROM PN_RIGHE AS r
                        LEFT JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP = r.pngrup AND s.P2CONT = r.pncont AND s.P3SOTC = r.pnsott
                        WHERE r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1REGI = @numb 
                        ORDER BY r.N1RIGA",
                    (reg, sot) => { reg.SelectedSubaccount = sot; return reg; },
                    new { cid = CompanyID, yea = Year, numb = HeadNumber }, splitOn: "P1GRUP");

                return new ObservableCollection<PNRIGHE>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public PNRIGHE? Get(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PNRIGHE>(
                    "SELECT * FROM PN_RIGHE WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @regi",
                    new { cid = CompanyID, yea = Year, regi = ID })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool CheckPrinted(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var count = connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM PN_RIGHE WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @regi AND N1STBO = '*'",
                    new { cid = CompanyID, yea = Year, regi = ID });

                return int.Parse(count?.ToString() ?? "0") > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        public List<Tuple<int, decimal, decimal>> GetWithSaldo(string CompanyID, string GroupID, string AccountID, DateTime From, DateTime To)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<int>(
                  @"select DISTINCT(r.n1clie) FROM PN_RIGHE as r 
                        JOIN PN_TESTATA as t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                        WHERE t.N1soci = @CompanyID AND t.N1DARE >= @From AND t.N1DARE <= @To AND r.pngrup = @GroupID AND r.pncont = @AccountID AND r.n1clie is not null order by r.n1clie",
                  new { CompanyID = CompanyID, From = From, To = To, GroupID = GroupID, AccountID = AccountID })
                  .ToList();

                var retValue = new List<Tuple<int, decimal, decimal>>();

                foreach (var lst in list)
                {
                    var dare = connection.Query<decimal?>(
                          @"select SUM(r.N1IMEU) FROM PN_RIGHE as r 
                                JOIN PN_TESTATA as t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                WHERE t.N1soci = @CompanyID AND t.N1DARE >= @From AND t.N1DARE <= @To AND r.pngrup = @GroupID AND r.pncont = @AccountID AND r.n1clie = @RefID AND r.N1SEGN = 'D'",
                          new { CompanyID = CompanyID, From = From, To = To, GroupID = GroupID, AccountID = AccountID, RefID = lst })
                          .FirstOrDefault();

                    var avere = connection.Query<decimal?>(
                  @"select SUM(r.N1IMEU) FROM PN_RIGHE as r 
                                JOIN PN_TESTATA as t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                WHERE t.N1soci = @CompanyID AND t.N1DARE >= @From AND t.N1DARE <= @To AND r.pngrup = @GroupID AND r.pncont = @AccountID AND r.n1clie = @RefID AND r.N1SEGN = 'A'",
                  new { CompanyID = CompanyID, From = From, To = To, GroupID = GroupID, AccountID = AccountID, RefID = lst })
                  .FirstOrDefault();

                    if (((dare ?? 0) - (avere ?? 0)) != 0)
                    {
                        retValue.Add(new Tuple<int, decimal, decimal>(lst, (dare ?? 0), (avere ?? 0)));
                    }
                }

                return retValue;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new List<Tuple<int, decimal, decimal>>();
            }
        }

        #region Reports
        public bool PrintedOnGeneralJournal(string CompanyID, int Year, int HeadNumber)
        {
            try
            {
                using var connection = GetOpenConnection();


                return ((int?)connection.ExecuteScalar(
                    @"SELECT COUNT(*) FROM PN_RIGHE 
                        WHERE N1SOCI = @cid AND N1ANNO = @yea AND N1REGI = @numb AND N1STBO = '*'",
                    new { cid = CompanyID, yea = Year, numb = HeadNumber }) > 0);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        public PDCBalanceReportOpposed? PrintPDCBalanceOpposed(string CompanyID, int Year, DateTime LimitDate, bool IncludeTemp, string? CostCenterID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new PDCBalanceReportOpposed()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    DateLimit = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59),
                    SubreportAP = new PDCBalanceSubReport(),
                    SubreportCR = new PDCBalanceSubReport(),
                };

                var esercizio = connection.Query<ESERCIZIO>($@"SELECT e.* FROM ESERCIZIO as e
                                                                    WHERE e.esesoc = @cid AND e.eseann = @yea
                                                                        ORDER BY e.eseann desc", new { cid = CompanyID, yea = Year - 1 }).Select(s => s.eseest).FirstOrDefault() ?? "X";

                #region ATTIVITA'

                IEnumerable<PNRIGHE>? list = null;

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                     $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                     new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                       $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                       new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }

                result.SubreportAP.LeftGroup = new PDCBalanceReport()
                {
                    Description = "ATTIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                string? lastGroupID = null;
                string? lastGroupDescription = null;
                string? lastAccountID = null;
                string? lastAccountDescription = null;
                string? lastSubaccountID = null;
                string? lastSubaccountDescription = null;
                string? lastExternalCode = null;
                string? lastAlternativeCode = null;
                decimal groupProgAmount = 0;
                decimal accountProgAmount = 0;
                decimal subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = "999999",
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                                });
                            }

                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = "999999",
                                        SubaccountDescription = "999999",
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                    });
                                }

                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "D")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "D" : "A",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }

                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                    if (item.N1SEGN == "D")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last
                if (subaccountProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = "999999",
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                    });
                }
                #endregion

                #region PASSIVITA'

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }
                result.SubreportAP.RightGroup = new PDCBalanceReport()
                {
                    Description = "PASSIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastGroupID = null;
                lastGroupDescription = null;
                lastAccountID = null;
                lastAccountDescription = null;
                lastSubaccountID = null;
                lastSubaccountDescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                groupProgAmount = 0;
                accountProgAmount = 0;
                subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = "999999",
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                                });
                            }
                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = "999999",
                                        SubaccountDescription = "999999",
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                    });
                                }
                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "A")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "A" : "D",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }
                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                    if (item.N1SEGN == "A")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last
                if (subaccountProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = "999999",
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                    });
                }
                #endregion

                #region Compute balance A/P
                // A sign
                if (result.SubreportAP.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "A";
                    result.SubreportAP.LeftGroup.TotalAmount = result.SubreportAP.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportAP.RightGroup.TotalAmount > 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportAP.RightGroup.TotalAmount < 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "D";
                    result.SubreportAP.RightGroup.TotalAmount = result.SubreportAP.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportAP.LeftGroup.TotalAmount > result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.LeftGroup.BalanceAmount = result.SubreportAP.LeftGroup.TotalAmount - result.SubreportAP.RightGroup.TotalAmount;
                    result.SubreportAP.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.RightGroup.BalanceAmount = result.SubreportAP.RightGroup.TotalAmount - result.SubreportAP.LeftGroup.TotalAmount;
                    result.SubreportAP.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                #region COSTI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'C' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.LeftGroup = new PDCBalanceReport()
                {
                    Description = "COSTI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastGroupID = null;
                lastGroupDescription = null;
                lastAccountID = null;
                lastAccountDescription = null;
                lastSubaccountID = null;
                lastSubaccountDescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                groupProgAmount = 0;
                accountProgAmount = 0;
                subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = "999999",
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                                });
                            }

                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = "999999",
                                        SubaccountDescription = "999999",
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                                    });
                                }

                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "D")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "D" : "A",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }

                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                    if (item.N1SEGN == "D")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last
                if (subaccountProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "D" : "A"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = "999999",
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "D" : "A"
                    });
                }
                #endregion

                #region RICAVI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'R' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.RightGroup = new PDCBalanceReport()
                {
                    Description = "RICAVI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastGroupID = null;
                lastGroupDescription = null;
                lastAccountID = null;
                lastAccountDescription = null;
                lastSubaccountID = null;
                lastSubaccountDescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                groupProgAmount = 0;
                accountProgAmount = 0;
                subaccountProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastGroupID) && string.IsNullOrWhiteSpace(lastAccountID) && string.IsNullOrWhiteSpace(lastSubaccountID)) ||
                        (lastGroupID == item.pngrup && lastAccountID == item.pncont && lastSubaccountID == item.pnsott))
                    {
                        lastGroupID = item.pngrup;
                        lastGroupDescription = item.GroupDescription;
                        lastAccountID = item.pncont;
                        lastAccountDescription = item.AccountDescription;
                        lastSubaccountID = item.pnsott;
                        lastSubaccountDescription = item.SubaccountDescription;
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            groupProgAmount += (item.N1IMEU ?? 0);
                            accountProgAmount += (item.N1IMEU ?? 0);
                            subaccountProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            groupProgAmount -= (item.N1IMEU ?? 0);
                            accountProgAmount -= (item.N1IMEU ?? 0);
                            subaccountProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastGroupID != item.pngrup)
                        {
                            // subaccount subtotal
                            if (subaccountProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = lastSubaccountID,
                                    SubaccountDescription = lastSubaccountDescription,
                                    Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                    Sign = subaccountProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode
                                });
                            }
                            // account subtotal
                            if (accountProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = lastAccountID,
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    SubTotalMark = "TOTALE CONTO",
                                    SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                    SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                    SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                });
                            }
                            // group subtotal
                            if (groupProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    GroupID = lastGroupID,
                                    AccountID = "999999",
                                    SubaccountID = "999999",
                                    SubaccountDescription = "999999",
                                    IsSubTotal = true,
                                    IsGroupSubtotal = true,
                                    SubTotalMark = "TOTALE GRUPPO",
                                    SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                                    SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                                    SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                                });
                            }
                            lastGroupID = item.pngrup;
                            lastGroupDescription = item.GroupDescription;
                            lastAccountID = item.pncont;
                            lastAccountDescription = item.AccountDescription;
                            lastSubaccountID = item.pnsott;
                            lastSubaccountDescription = item.SubaccountDescription;
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                groupProgAmount = (item.N1IMEU ?? 0);
                                accountProgAmount = (item.N1IMEU ?? 0);
                                subaccountProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                groupProgAmount = (item.N1IMEU ?? 0) * -1;
                                accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                        else
                        {
                            if (lastAccountID != item.pncont)
                            {
                                // subaccount subtotal
                                if (subaccountProgAmount != 0)
                                {
                                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = lastSubaccountID,
                                        SubaccountDescription = lastSubaccountDescription,
                                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                                        ExternalCode = lastExternalCode,
                                        AlternativeCode = lastAlternativeCode,
                                    });
                                }
                                // account subtotal
                                if (accountProgAmount != 0)
                                {
                                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                    {
                                        GroupID = lastGroupID,
                                        AccountID = lastAccountID,
                                        SubaccountID = "999999",
                                        SubaccountDescription = "999999",
                                        IsSubTotal = true,
                                        SubTotalMark = "TOTALE CONTO",
                                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                                    });
                                }
                                lastAccountID = item.pncont;
                                lastAccountDescription = item.AccountDescription;
                                lastSubaccountID = item.pnsott;
                                lastSubaccountDescription = item.SubaccountDescription;
                                lastExternalCode = item.PDCExternalCode;
                                lastAlternativeCode = item.PDCAlternativeCode ?? "";

                                if (item.N1SEGN == "A")
                                {
                                    groupProgAmount += (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0);
                                    subaccountProgAmount = (item.N1IMEU ?? 0);
                                    result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                }
                                else
                                {
                                    groupProgAmount -= (item.N1IMEU ?? 0);
                                    accountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                    result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                }
                            }
                            else
                            {
                                if (lastSubaccountID != item.pnsott)
                                {
                                    // subaccount subtotal
                                    if (subaccountProgAmount != 0)
                                    {
                                        result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                        {
                                            GroupID = lastGroupID,
                                            AccountID = lastAccountID,
                                            SubaccountID = lastSubaccountID,
                                            SubaccountDescription = lastSubaccountDescription,
                                            Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                                            Sign = subaccountProgAmount > 0 ? "A" : "D",
                                            ExternalCode = lastExternalCode,
                                            AlternativeCode = lastAlternativeCode,
                                        });
                                    }
                                    lastSubaccountID = item.pnsott;
                                    lastSubaccountDescription = item.SubaccountDescription;
                                    lastExternalCode = item.PDCExternalCode;
                                    if (item.N1SEGN == "A")
                                    {
                                        groupProgAmount += (item.N1IMEU ?? 0);
                                        accountProgAmount += (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0);
                                        result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                                    }
                                    else
                                    {
                                        groupProgAmount -= (item.N1IMEU ?? 0);
                                        accountProgAmount -= (item.N1IMEU ?? 0);
                                        subaccountProgAmount = (item.N1IMEU ?? 0) * -1;
                                        result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                // add last 
                if (subaccountProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = lastSubaccountID,
                        SubaccountDescription = lastSubaccountDescription,
                        Amount = subaccountProgAmount > 0 ? subaccountProgAmount : subaccountProgAmount * -1,
                        Sign = subaccountProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                // account subtotal
                if (accountProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = lastAccountID,
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        SubTotalMark = "TOTALE CONTO",
                        SubtotalText = $"{lastAccountID} {lastAccountDescription}",
                        SubTotalAmount = accountProgAmount > 0 ? accountProgAmount : accountProgAmount * -1,
                        SubTotalAmountSign = accountProgAmount > 0 ? "A" : "D"
                    });
                }
                // group subtotal
                if (groupProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        GroupID = lastGroupID,
                        AccountID = "999999",
                        SubaccountID = "999999",
                        SubaccountDescription = "999999",
                        IsSubTotal = true,
                        IsGroupSubtotal = true,
                        SubTotalMark = "TOTALE GRUPPO",
                        SubtotalText = $"{lastGroupID} {lastGroupDescription}",
                        SubTotalAmount = groupProgAmount > 0 ? groupProgAmount : groupProgAmount * -1,
                        SubTotalAmountSign = groupProgAmount > 0 ? "A" : "D"
                    });
                }
                #endregion

                #region Compute balance C/R
                // A sign
                if (result.SubreportCR.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "A";
                    result.SubreportCR.LeftGroup.TotalAmount = result.SubreportCR.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportCR.RightGroup.TotalAmount > 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportCR.RightGroup.TotalAmount < 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "D";
                    result.SubreportCR.RightGroup.TotalAmount = result.SubreportCR.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportCR.LeftGroup.TotalAmount > result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.LeftGroup.BalanceAmount = result.SubreportCR.LeftGroup.TotalAmount - result.SubreportCR.RightGroup.TotalAmount;
                    result.SubreportCR.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.RightGroup.BalanceAmount = result.SubreportCR.RightGroup.TotalAmount - result.SubreportCR.LeftGroup.TotalAmount;
                    result.SubreportCR.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public PDCBalanceReportOpposed? PrintPDCBalanceOpposedDIVA(string CompanyID, int Year, DateTime LimitDate, bool IncludeTemp, string? CostCenterID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new PDCBalanceReportOpposed()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    DateLimit = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59),
                    SubreportAP = new PDCBalanceSubReport(),
                    SubreportCR = new PDCBalanceSubReport(),
                };

                var esercizio = connection.Query<ESERCIZIO>($@"SELECT e.* FROM ESERCIZIO as e
                                                                    WHERE e.esesoc = @cid AND e.eseann = @yea
                                                                        ORDER BY e.eseann desc", new { cid = CompanyID, yea = Year - 1 }).Select(s => s.eseest).FirstOrDefault() ?? "X";

                #region ATTIVITA'

                IEnumerable<PNRIGHE>? list = null;

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                     $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                     new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                       $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'A' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                       new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }

                result.SubreportAP.LeftGroup = new PDCBalanceReport()
                {
                    Description = "ATTIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };

                string? lastDIVADescription = null;
                string? lastExternalCode = null;
                string? lastAlternativeCode = null;
                decimal divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.DIVADescription)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }

                            lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }

                // add last
                if (divaProgAmount != 0)
                {
                    result.SubreportAP.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                #endregion

                #region PASSIVITA'

                if (esercizio == "A")
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO>=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year - 1, limitdate = LimitDate, cc = CostCenterID });
                }
                else
                {
                    list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'P' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });
                }
                result.SubreportAP.RightGroup = new PDCBalanceReport()
                {
                    Description = "PASSIVITA'",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastDIVADescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.DIVADescription)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }

                            lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportAP.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportAP.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }
                // add last
                if (divaProgAmount != 0)
                {
                    result.SubreportAP.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
               
                #endregion

                #region Compute balance A/P
                // A sign
                if (result.SubreportAP.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "A";
                    result.SubreportAP.LeftGroup.TotalAmount = result.SubreportAP.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportAP.RightGroup.TotalAmount > 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportAP.RightGroup.TotalAmount < 0)
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "D";
                    result.SubreportAP.RightGroup.TotalAmount = result.SubreportAP.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportAP.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportAP.LeftGroup.TotalAmount > result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.LeftGroup.BalanceAmount = result.SubreportAP.LeftGroup.TotalAmount - result.SubreportAP.RightGroup.TotalAmount;
                    result.SubreportAP.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportAP.LeftGroup.TotalAmount < result.SubreportAP.RightGroup.TotalAmount)
                {
                    result.SubreportAP.RightGroup.BalanceAmount = result.SubreportAP.RightGroup.TotalAmount - result.SubreportAP.LeftGroup.TotalAmount;
                    result.SubreportAP.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                #region COSTI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'C' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.LeftGroup = new PDCBalanceReport()
                {
                    Description = "COSTI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastDIVADescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription)) ||
                        (lastDIVADescription == item.DIVADescription))
                    {
                        lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "D")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.pngrup)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "D" : "A",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode,
                                });
                            }


                            lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "D")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.LeftGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.LeftGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }
                // add last
                if (divaProgAmount != 0)
                {
                    result.SubreportCR.LeftGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "D" : "A",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                #endregion

                #region RICAVI
                list = connection.Query<PNRIGHE>(
                    $@"SELECT r.*, TRIM(g.P1DES1) AS GroupDescription, TRIM(c.P2DES1) AS AccountDescription, TRIM(s.P3DES1) AS SubaccountDescription, s.p3este AS PDCExternalCode, s.p3est2 AS PDCAlternativeCode,TRIM(s.P3DES2) as DIVADescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup
                            INNER JOIN PDC_CONTI AS c ON c.P1GRUP=r.pngrup AND c.P2CONT=r.pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE <= @limitdate and g.P1TICO = 'R' AND r.N1IMEU > 0 {(IncludeTemp ? null : "AND r.N1tmpPNR <> 'S'")} {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND r.N1CCCC = @cc")}
                            ORDER BY pngrup, pncont, pnsott",
                    new { cid = CompanyID, yea = Year, limitdate = LimitDate, cc = CostCenterID });

                result.SubreportCR.RightGroup = new PDCBalanceReport()
                {
                    Description = "RICAVI",
                    Rows = new List<PDCBalanceReport.PDCBalanceReportItem>()
                };
                lastDIVADescription = null;
                lastExternalCode = null;
                lastAlternativeCode = null;

                divaProgAmount = 0;
                foreach (var item in list)
                {
                    if ((string.IsNullOrWhiteSpace(lastDIVADescription) ) ||
                        (lastDIVADescription == item.DIVADescription ))
                    {
                        lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                        lastExternalCode = item.PDCExternalCode;
                        lastAlternativeCode = item.PDCAlternativeCode ?? "";

                        if (item.N1SEGN == "A")
                        {
                            divaProgAmount += (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                        }
                        else
                        {
                            divaProgAmount -= (item.N1IMEU ?? 0);
                            result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                        }
                    }
                    else
                    {
                        if (lastDIVADescription != item.pngrup)
                        {
                            // subaccount subtotal
                            if (divaProgAmount != 0)
                            {
                                result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                                {
                                    SubaccountDescription = lastDIVADescription,
                                    Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                                    Sign = divaProgAmount > 0 ? "A" : "D",
                                    ExternalCode = lastExternalCode,
                                    AlternativeCode = lastAlternativeCode
                                });
                            }

                            lastDIVADescription = item.DIVADescription?.Replace("\r\n", "");
                            lastExternalCode = item.PDCExternalCode;
                            lastAlternativeCode = item.PDCAlternativeCode ?? "";

                            if (item.N1SEGN == "A")
                            {
                                divaProgAmount = (item.N1IMEU ?? 0);
                                result.SubreportCR.RightGroup.TotalAmount += (item.N1IMEU ?? 0);
                            }
                            else
                            {
                                divaProgAmount = (item.N1IMEU ?? 0) * -1;
                                result.SubreportCR.RightGroup.TotalAmount -= (item.N1IMEU ?? 0);
                            }
                        }
                    }
                }
                // add last 
                if (divaProgAmount != 0)
                {
                    result.SubreportCR.RightGroup.Rows.Add(new PDCBalanceReport.PDCBalanceReportItem()
                    {
                        SubaccountDescription = lastDIVADescription,
                        Amount = divaProgAmount > 0 ? divaProgAmount : divaProgAmount * -1,
                        Sign = divaProgAmount > 0 ? "A" : "D",
                        ExternalCode = lastExternalCode,
                        AlternativeCode = lastAlternativeCode,
                    });
                }
                #endregion

                #region Compute balance C/R
                // A sign
                if (result.SubreportCR.LeftGroup.TotalAmount > 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < 0)
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "A";
                    result.SubreportCR.LeftGroup.TotalAmount = result.SubreportCR.LeftGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.LeftGroup.TotalAmountSign = "-";
                }
                // P sign
                if (result.SubreportCR.RightGroup.TotalAmount > 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "A";
                }
                else if (result.SubreportCR.RightGroup.TotalAmount < 0)
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "D";
                    result.SubreportCR.RightGroup.TotalAmount = result.SubreportCR.RightGroup.TotalAmount * -1;
                }
                else
                {
                    result.SubreportCR.RightGroup.TotalAmountSign = "-";
                }

                if (result.SubreportCR.LeftGroup.TotalAmount > result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.LeftGroup.BalanceAmount = result.SubreportCR.LeftGroup.TotalAmount - result.SubreportCR.RightGroup.TotalAmount;
                    result.SubreportCR.LeftGroup.BalanceAmountSign = "D";
                }
                else if (result.SubreportCR.LeftGroup.TotalAmount < result.SubreportCR.RightGroup.TotalAmount)
                {
                    result.SubreportCR.RightGroup.BalanceAmount = result.SubreportCR.RightGroup.TotalAmount - result.SubreportCR.LeftGroup.TotalAmount;
                    result.SubreportCR.RightGroup.BalanceAmountSign = "A";
                }
                #endregion

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MastrinoReport? PrintMastrino(string CompanyID, int Year, string? Group, string? Account, string? Subaccount, DateTime FromDate, DateTime LimitDate, ABE? Entity, bool MonthlyGroup, bool IsDefinitive)
        {
            try
            {
                using var connection = GetOpenConnection();
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
                LimitDate = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59);
                var result = new MastrinoReport()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    IstantText = $"Stampato il {now.ToString("dd/MM/yyyy HH:mm:ss")}",
                    ReportTitle = $"STAMPA MASTRINI DI SOTTOCONTO ESERCIZIO {Year} DAL {FromDate.ToString("dd/MM/yyyy")} AL {LimitDate.ToString("dd/MM/yyyy")}",
                    PDCDescription = string.IsNullOrWhiteSpace(Group) ? "ALL" : $"{Group}_{Account}_{Subaccount}",
                    FromDate = FromDate,
                    ToDate = LimitDate.Date,
                    Rows = new List<MastrinoReportItem>()
                };

                var query = $@"SELECT t.n1dare AS N1DARE, r.*, c.caucod,c.caudes, g.P1GRUP, g.P1DES1, g.P1TICO, a.P2CONT, a.P2DES1, s.P3SOTC, s.P3DES1, abe.abecod, abe.abers1, abe.abers2
                            FROM PN_RIGHE as r 
                            INNER JOIN PN_TESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
				            INNER JOIN CAUCONT as c ON t.pncaus = c.caucod
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup 
                            INNER JOIN PDC_CONTI AS a ON a.P1GRUP=r.pngrup AND a.P2CONT=pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=pncont AND s.P3SOTC=pnsott
                            LEFT JOIN ANAG_BASE AS abe ON abe.abecod=r.N1CLIE
                            WHERE  r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND 
                            r.N1IMEU <> 0 AND t.N1DARE >= @fromdate AND t.N1DARE <= @limitdate AND (r.N1STMA <> '*' OR r.N1STMA IS NULL)
                            {(!string.IsNullOrWhiteSpace(Group) ? " AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount" : null)}
                            {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)}
				            order by r.pngrup,r.pncont,r.pnsott, t.n1dare, t.n1regi";
                var list = connection.Query<PNTESTATA, PNRIGHE, CAUCONT, PDCGRUPPI, PDCCONTI, PDCSOTTO, ABE, PNRIGHE>(
                    query,
                    (testa, riga, caus, grp, acc, sot, abe) =>
                    {
                        testa.AccountingCausal = caus;
                        riga.Testata = testa;
                        riga.Group = grp;
                        riga.Account = acc;
                        riga.Subaccount = sot;
                        riga.BasicRegistry = abe;
                        return riga;
                    },
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount, fromdate = FromDate, limitdate = LimitDate, N1CLIE = Entity?.abecod },
                    splitOn: "N1SOCI,caucod,P1GRUP,P2CONT,P3SOTC,abecod");

                var esercizioRepository = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>();
                var esercizio = esercizioRepository.Get(CompanyID, Year);
                var previousEsercizio = esercizioRepository.Get(CompanyID, Year - 1);

                foreach (var item in list.GroupBy(g => new { g.pngrup, g.pncont, g.pnsott }, (key, items) => new { key, items }))
                {
                    var newPDC = new MastrinoReportItem()
                    {
                        GroupDescription = item.items.First().Group?.FullDescriptionSearchable,
                        AccountDescription = item.items.First().Account?.FullDescriptionSearchable,
                        SubaccountDescription = item.items.First().Subaccount?.FullDescriptionSearchable,
                        Details = new List<MastrinoReportItemDetails>()
                    };

                    decimal? dareE = 0;
                    decimal? avereE = 0;
                    decimal progressive = 0;

                    #region Compute previous esercizio balance
                    if (previousEsercizio?.eseest == "A" && (item.items.First().Group?.P1TICO == "A" || item.items.First().Group?.P1TICO == "P"))
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU),0) FROM PN_RIGHE AS r 
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PN_RIGHE AS r 
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year - 1, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, N1CLIE = Entity?.abecod });
                        dareE = multiple.Read<decimal?>().Single();
                        avereE = multiple.Read<decimal?>().Single();

                        progressive = (dareE ?? 0) - (avereE ?? 0);

                        if (progressive != 0)
                        {
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                Description = $"SALDO ESERCIZIO PRECEDENTE ({(Year - 1)})",
                                Debit = dareE ?? 0,
                                Credit = avereE ?? 0,
                                Progressive = progressive
                            });
                        }
                    }
                    #endregion

                    #region Compute previous balance

                    var startDate = new DateTime(Year, (esercizio?.eseini ?? 1), 1);
                    decimal? dareP = 0;
                    decimal? avereP = 0;
                    if (startDate != FromDate)
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU), 0) FROM PN_RIGHE AS r 
                                                                    INNER JOIN PN_TESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PN_RIGHE AS r 
                                                                    INNER JOIN PN_TESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, startdate = startDate, fromdate = FromDate, N1CLIE = Entity?.abecod });
                        dareP = multiple.Read<decimal?>().Single();
                        avereP = multiple.Read<decimal?>().Single();
                    }
                    progressive = (dareP ?? 0) - (avereP ?? 0);

                    if (progressive != 0)
                    {
                        newPDC.Details.Add(new MastrinoReportItemDetails()
                        {
                            Description = $"SALDO PRECEDENTE (dal {new DateTime(Year, (esercizio?.eseini ?? 1), 1).ToString("dd/MM/yyyy")} al {FromDate.ToString("dd/MM/yyyy")})",
                            Debit = dareP ?? 0,
                            Credit = avereP ?? 0,
                            Progressive = progressive
                        });
                    }
                    #endregion

                    progressive = ((dareE ?? 0) + (dareP ?? 0)) - ((avereE ?? 0) + (avereP ?? 0));

                    if (MonthlyGroup)
                    {
                        foreach (var det in item.items.OrderBy(o => o.Testata?.N1DARE).GroupBy(g => new { g.Testata?.N1DARE?.Year, g.Testata?.N1DARE?.Month }, (ikey, details) => new { ikey, details }))
                        {
                            decimal monthlyDebit = 0;
                            decimal monthlyCredit = 0;
                            foreach (var row in det.details)
                            {
                                if (row.N1SEGN == "D")
                                    progressive += row.N1IMEU ?? 0;
                                else
                                    progressive -= row.N1IMEU ?? 0;

                                newPDC.Details.Add(new MastrinoReportItemDetails()
                                {
                                    IsDefinitive = row.N1STMA,
                                    JournalRow = row.N1RIGB ?? 0,
                                    JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                    RegistrationNumber = row.N1REGI,
                                    RegistrationDateText = row.Testata?.N1DARE?.ToString("dd/MM/yyyy"),
                                    DocumentID = row.N1DOCU,
                                    ReferenceID = row.N1RIFE,
                                    Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                    Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                    Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                    Progressive = progressive
                                });
                                monthlyDebit += row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0;
                                monthlyCredit += row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0;
                            }
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit,
                                CreditMonth = monthlyCredit,
                                ProgressiveMonth = progressive,
                                MonthText = "TOTALI MESE"
                            });
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit > monthlyCredit ? monthlyDebit - monthlyCredit : 0,
                                CreditMonth = monthlyDebit < monthlyCredit ? monthlyCredit - monthlyDebit : 0,
                                ProgressiveMonth = progressive,
                                MonthText = "SALDI MESE"
                            });
                        }
                    }
                    else
                    {
                        foreach (var row in item.items.OrderBy(o => o.Testata?.N1DARE))
                        {
                            if (row.N1SEGN == "D")
                                progressive += row.N1IMEU ?? 0;
                            else
                                progressive -= row.N1IMEU ?? 0;

                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                IsDefinitive = row.N1STMA,
                                JournalRow = row.N1RIGB ?? 0,
                                JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                RegistrationNumber = row.N1REGI,
                                RegistrationDateText = row.Testata?.N1DARE?.ToString("dd/MM/yyyy"),
                                DocumentID = row.N1DOCU,
                                ReferenceID = row.N1RIFE,
                                Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                Progressive = progressive,
                                EntityDescription = row.BasicRegistry?.FullDescriptionSearchable
                            });
                        }
                    }

                    result.Rows.Add(newPDC);
                    var totalDebit = item.items.Where(w => w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    var totalCredit = item.items.Where(w => w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                    var totMast = new MastrinoReportItem()
                    {
                        MastrinoTotalText = "TOTALE MASTRINO",
                        Debit = totalDebit + dareE + dareP,
                        Credit = totalCredit + avereE + avereP,
                        Progressive = progressive
                    };
                    result.Rows.Add(totMast);
                    result.Rows.Add(new MastrinoReportItem()
                    {
                        MastrinoTotalText = "SALDO MASTRINO",
                        Debit = totMast.Debit > totMast.Credit ? totMast.Debit - totMast.Credit : 0,
                        Credit = totMast.Debit < totMast.Credit ? totMast.Credit - totMast.Debit : 0,
                        Progressive = progressive
                    });
                    if (IsDefinitive)
                    {
                        if (!MastrinoFlagDefinitive(CompanyID, item.items.First().Group?.P1GRUP ?? string.Empty, item.items.First().Account?.P2CONT ?? string.Empty, item.items.First()?.Subaccount?.P3SOTC ?? string.Empty, Year, totalDebit, totalCredit, FromDate, LimitDate, Entity?.abecod))
                        {
                            ErrorHandler.Show("Errore durante l'aggiornamento dei dati per la stampa DEFINITIVA, ripetere l'operazione");
                        }
                    }
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MastrinoReport? ReprintMastrino(string CompanyID, int Year, string? Group, string? Account, string? Subaccount, DateTime FromDate, DateTime LimitDate, ABE? Entity, bool MonthlyGroup)
        {
            try
            {
                using var connection = GetOpenConnection();
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
                LimitDate = new DateTime(LimitDate.Year, LimitDate.Month, LimitDate.Day, 23, 59, 59);
                var result = new MastrinoReport()
                {
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    IstantText = $"Stampato il {now.ToString("dd/MM/yyyy HH:mm:ss")}",
                    ReportTitle = $"STAMPA MASTRINI DI SOTTOCONTO ESERCIZIO {Year} DAL {FromDate.ToString("dd/MM/yyyy")} AL {LimitDate.ToString("dd/MM/yyyy")}",
                    PDCDescription = string.IsNullOrWhiteSpace(Group) ? "ALL" : $"{Group}_{Account}_{Subaccount}",
                    FromDate = FromDate,
                    ToDate = LimitDate.Date,
                    Rows = new List<MastrinoReportItem>()
                };


                var query = $@"SELECT t.n1dare AS N1DARE, r.*, c.caucod,c.caudes, g.P1GRUP, g.P1DES1, g.P1TICO, a.P2CONT, a.P2DES1, s.P3SOTC, s.P3DES1, abe.abecod, abe.abers1, abe.abers2
                            FROM PN_RIGHE as r 
                            INNER JOIN PN_TESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
				            INNER JOIN CAUCONT as c ON t.pncaus = c.caucod
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=r.pngrup 
                            INNER JOIN PDC_CONTI AS a ON a.P1GRUP=r.pngrup AND a.P2CONT=pncont
                            INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=pncont AND s.P3SOTC=pnsott
                            LEFT JOIN ANAG_BASE AS abe ON abe.abecod=r.N1CLIE
                            WHERE  r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND 
                            r.N1IMEU <> 0 AND t.N1DARE >= @fromdate AND t.N1DARE <= @limitdate
                            {(!string.IsNullOrWhiteSpace(Group) ? " AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount" : null)}
                            {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)}
				            order by r.pngrup,r.pncont,r.pnsott, t.n1dare, t.n1regi";
                var list = connection.Query<PNTESTATA, PNRIGHE, CAUCONT, PDCGRUPPI, PDCCONTI, PDCSOTTO, ABE, PNRIGHE>(
                    query,
                    (testa, riga, caus, grp, acc, sot, abe) =>
                    {
                        testa.AccountingCausal = caus;
                        riga.Testata = testa;
                        riga.Group = grp;
                        riga.Account = acc;
                        riga.Subaccount = sot;
                        riga.BasicRegistry = abe;
                        return riga;
                    },
                    new { cid = CompanyID, yea = Year, group = Group, account = Account, subaccount = Subaccount, fromdate = FromDate, limitdate = LimitDate, N1CLIE = Entity?.abecod },
                    splitOn: "N1SOCI,caucod,P1GRUP,P2CONT,P3SOTC,abecod");

                var esercizioRepository = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>();
                var esercizio = esercizioRepository.Get(CompanyID, Year)!;
                var previousEsercizio = esercizioRepository.Get(CompanyID, Year - 1);

                foreach (var item in list.GroupBy(g => new { g.pngrup, g.pncont, g.pnsott, g.n1clie }, (key, items) => new { key, items }))
                {
                    var newPDC = new MastrinoReportItem()
                    {
                        GroupDescription = item.items.First().Group?.FullDescriptionSearchable,
                        AccountDescription = item.items.First().Account?.FullDescriptionSearchable,
                        SubaccountDescription = item.items.First().Subaccount?.FullDescriptionSearchable,
                        Details = new List<MastrinoReportItemDetails>()
                    };

                    decimal? dareE = 0;
                    decimal? avereE = 0;
                    decimal progressive = 0;

                    #region Compute previous esercizio balance
                    if (previousEsercizio?.eseest == "A" && (item.items.First().Group?.P1TICO == "A" || item.items.First().Group?.P1TICO == "P"))
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU),0) FROM PNRIGHE AS r 
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year - 1, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, N1CLIE = Entity?.abecod });
                        dareE = multiple.Read<decimal?>().Single();
                        avereE = multiple.Read<decimal?>().Single();

                        progressive = (dareE ?? 0) - (avereE ?? 0);

                        if (progressive != 0)
                        {
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                Description = $"SALDO ESERCIZIO PRECEDENTE ({(Year - 1)})",
                                Debit = dareE ?? 0,
                                Credit = avereE ?? 0,
                                Progressive = progressive
                            });
                        }
                    }
                    #endregion

                    #region Compute previous balance

                    var startDate = new DateTime(Year, (esercizio.eseini ?? 1), 1);
                    decimal? dareP = 0;
                    decimal? avereP = 0;
                    if (startDate != FromDate)
                    {
                        var multiple = connection.QueryMultiple($@"SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'D' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};
                                                                   SELECT ISNULL(SUM(N1IMEU), 0) FROM PNRIGHE AS r 
                                                                    INNER JOIN PNTESTATA AS t ON r.N1SOCI = t.N1SOCI AND r.N1ANNO = t.N1ANNO AND r.N1REGI = t.N1REGI
                                                                    WHERE r.N1SEGN = 'A' AND r.N1SOCI = @cid AND r.N1ANNO = @yea AND r.N1IMEU IS NOT NULL AND r.N1IMEU <> 0 AND t.N1DARE >= @startdate AND t.N1DARE < @fromdate AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount {(Entity != null ? " AND r.N1CLIE=@N1CLIE" : null)};",
                            new { cid = CompanyID, yea = Year, group = item.items.First().Group?.P1GRUP, account = item.items.First().Account?.P2CONT, subaccount = item.items.First().Subaccount?.P3SOTC, startdate = startDate, fromdate = FromDate, N1CLIE = Entity?.abecod });
                        dareP = multiple.Read<decimal?>().Single();
                        avereP = multiple.Read<decimal?>().Single();
                    }
                    progressive = (dareP ?? 0) - (avereP ?? 0);

                    if (progressive != 0)
                    {
                        newPDC.Details.Add(new MastrinoReportItemDetails()
                        {
                            Description = $"SALDO PRECEDENTE (dal {new DateTime(Year, (esercizio.eseini ?? 1), 1).ToString("dd/MM/yyyy")} al {FromDate.ToString("dd/MM/yyyy")})",
                            Debit = dareP ?? 0,
                            Credit = avereP ?? 0,
                            Progressive = progressive
                        });
                    }
                    #endregion

                    progressive = ((dareE ?? 0) + (dareP ?? 0)) - ((avereE ?? 0) + (avereP ?? 0));

                    if (MonthlyGroup)
                    {
                        foreach (var det in item.items.OrderBy(o => o.Testata?.N1DARE).GroupBy(g => new { (g.Testata?.N1DARE ?? DateTime.MinValue).Year, (g.Testata?.N1DARE ?? DateTime.MinValue).Month }, (ikey, details) => new { ikey, details }))
                        {
                            decimal monthlyDebit = 0;
                            decimal monthlyCredit = 0;
                            foreach (var row in det.details)
                            {
                                if (row.N1SEGN == "D")
                                    progressive += row.N1IMEU ?? 0;
                                else
                                    progressive -= row.N1IMEU ?? 0;

                                newPDC.Details.Add(new MastrinoReportItemDetails()
                                {
                                    IsDefinitive = row.N1STMA,
                                    JournalRow = row.N1RIGB ?? 0,
                                    JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                    RegistrationNumber = row.N1REGI,
                                    RegistrationDateText = (row.Testata?.N1DARE ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                                    DocumentID = row.N1DOCU,
                                    ReferenceID = row.N1RIFE,
                                    Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                    Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                    Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                    Progressive = progressive
                                });
                                monthlyDebit += row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0;
                                monthlyCredit += row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0;
                            }
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit,
                                CreditMonth = monthlyCredit,
                                ProgressiveMonth = progressive,
                                MonthText = "TOTALI MESE"
                            });
                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                DebitMonth = monthlyDebit > monthlyCredit ? monthlyDebit - monthlyCredit : 0,
                                CreditMonth = monthlyDebit < monthlyCredit ? monthlyCredit - monthlyDebit : 0,
                                ProgressiveMonth = progressive,
                                MonthText = "SALDI MESE"
                            });
                        }
                    }
                    else
                    {
                        foreach (var row in item.items.OrderBy(o => o.Testata?.N1DARE))
                        {
                            if (row.N1SEGN == "D")
                                progressive += row.N1IMEU ?? 0;
                            else
                                progressive -= row.N1IMEU ?? 0;

                            newPDC.Details.Add(new MastrinoReportItemDetails()
                            {
                                IsDefinitive = row.N1STMA,
                                JournalRow = row.N1RIGB ?? 0,
                                JournalDateText = row.N1DABB.HasValue ? row.N1DABB.Value.ToString("dd/MM/yyyy") : "---",
                                RegistrationNumber = row.N1REGI,
                                RegistrationDateText = (row.Testata?.N1DARE ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                                DocumentID = row.N1DOCU,
                                ReferenceID = row.N1RIFE,
                                Description = !string.IsNullOrWhiteSpace(row.N1DESC?.Trim()) ? row.N1DESC?.Trim() : row.Testata?.AccountingCausal?.FullDescriptionNotSearchable,
                                Debit = row.N1SEGN == "D" ? (row.N1IMEU ?? 0) : 0,
                                Credit = row.N1SEGN == "A" ? (row.N1IMEU ?? 0) : 0,
                                Progressive = progressive,
                                EntityDescription = row.BasicRegistry?.FullDescriptionSearchable
                            });
                        }
                    }

                    result.Rows.Add(newPDC);
                    var totalDebit = item.items.Where(w => w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0;
                    var totalCredit = item.items.Where(w => w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0;
                    var totMast = new MastrinoReportItem()
                    {
                        MastrinoTotalText = "TOTALE MASTRINO",
                        Debit = totalDebit + dareE + dareP,
                        Credit = totalCredit + avereE + avereP,
                        Progressive = progressive
                    };
                    result.Rows.Add(totMast);
                    result.Rows.Add(new MastrinoReportItem()
                    {
                        MastrinoTotalText = "SALDO MASTRINO",
                        Debit = totMast.Debit > totMast.Credit ? totMast.Debit - totMast.Credit : 0,
                        Credit = totMast.Debit < totMast.Credit ? totMast.Credit - totMast.Debit : 0,
                        Progressive = progressive
                    });
                }

                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO PN_RIGHE (N1SOCI,N1ANNO,N1REGI,N1RIGA,N1DOCU,N1RIFE,N1DADO,N1DARI,pngrup,pncont,pnsott,N1IMPO,N1DESC,N1SEGN,n1clie,N1CCCC,N1CCCS,N1CONP,N1COMM,N1DEST,N1GIRO,N1STNO,N1STBO,N1STMA,N1CHIU,N1RIGB,N1DABB,N1DASM,N1DACC,N1FIVA,N1BC01,N1BC02,N1BC03,N1BC04,N1BC05,N1IMEU,N1TIDO,n1scad,n1paga,N1DRri,N1SCON,N1DIVI,N1CAMB,N1IMVA,N1FLCO,N1tmpPNR,n1mese) OUTPUT INSERTED.rv VALUES(@N1SOCI,@N1ANNO,@N1REGI,@N1RIGA,@N1DOCU,@N1RIFE,@N1DADO,@N1DARI,@pngrup,@pncont,@pnsott,@N1IMPO,@N1DESC,@N1SEGN,@n1clie,@N1CCCC,@N1CCCS,@N1CONP,@N1COMM,@N1DEST,@N1GIRO,@N1STNO,@N1STBO,@N1STMA,@N1CHIU,@N1RIGB,@N1DABB,@N1DASM,@N1DACC,@N1FIVA,@N1BC01,@N1BC02,@N1BC03,@N1BC04,@N1BC05,@N1IMEU,@N1TIDO,@n1scad,@n1paga,@N1DRri,@N1SCON,@N1DIVI,@N1CAMB,@N1IMVA,@N1FLCO,@N1tmpPNR,@n1mese)";
        public string UPDATE_QUERY => "UPDATE PN_RIGHE SET N1SOCI = @N1SOCI,N1ANNO = @N1ANNO,N1REGI = @N1REGI,N1RIGA = @N1RIGA,N1DOCU = @N1DOCU,N1RIFE = @N1RIFE,N1DADO = @N1DADO,N1DARI = @N1DARI,pngrup = @pngrup,pncont = @pncont,pnsott = @pnsott,N1IMPO = @N1IMPO,N1DESC = @N1DESC,N1SEGN = @N1SEGN,n1clie = @n1clie,N1CCCC = @N1CCCC,N1CCCS = @N1CCCS,N1CONP = @N1CONP,N1COMM = @N1COMM,N1DEST = @N1DEST,N1GIRO = @N1GIRO,N1STNO = @N1STNO,N1STBO = @N1STBO,N1STMA = @N1STMA,N1CHIU = @N1CHIU,N1RIGB = @N1RIGB,N1DABB = @N1DABB,N1DASM = @N1DASM,N1DACC = @N1DACC,N1FIVA = @N1FIVA,N1BC01 = @N1BC01,N1BC02 = @N1BC02,N1BC03 = @N1BC03,N1BC04 = @N1BC04,N1BC05 = @N1BC05,N1IMEU = @N1IMEU,N1TIDO = @N1TIDO,n1scad = @n1scad,n1paga = @n1paga,N1DRri = @N1DRri,N1SCON = @N1SCON,N1DIVI = @N1DIVI,N1CAMB = @N1CAMB,N1IMVA = @N1IMVA,N1FLCO = @N1FLCO,N1tmpPNR = @N1tmpPNR,n1mese = @n1mese OUTPUT INSERTED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND N1RIGA = @N1RIGA AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PN_RIGHE OUTPUT DELETED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND N1RIGA = @N1RIGA AND rv = @rv";
        public bool Insert(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(INSERT_QUERY, Model);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.INSERT_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
                if (result != null)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool RemoveTemporaryFlag(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    var resultUpdate = connection.Execute(@"UPDATE PN_RIGHE SET N1tmpPNR = 'N'  
                                                                WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi",
                        Model,
                        transaction: transaction);

                    if (resultUpdate > 0)
                    {
                        var resultUpdateHead = connection.ExecuteScalar(@"UPDATE PN_TESTATA SET N1tmpPN = 'N' OUTPUT INSERTED.rv 
                                                                WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi AND rv = @rv",
                        Model.Testata,
                        transaction: transaction);

                        if (resultUpdateHead != null)
                        {
                            decimal amount = 0;
                            foreach (var row in connection.Query<PNRIGHE>(@"SELECT * FROM PN_RIGHE
                                                                                WHERE N1SOCI=@N1SOCI AND N1ANNO=@N1ANNO AND N1REGI=@N1REGI AND pngrup=@pngrup AND pncont=@pncont AND pnsott=@pnsott",
                                                                            Model, transaction))
                            {
                                if (row.N1SEGN == "D")
                                    amount += (row.N1IMEU ?? 0);
                                else
                                    amount -= (row.N1IMEU ?? 0);
                            }

                            var rbccItem = VulpesServiceProvider.Provider.GetRequiredService<IRBCC01F0Repository>().GetByPDC(Model.N1SOCI, Model.pngrup ?? string.Empty, Model.pncont ?? string.Empty, Model.pnsott ?? string.Empty);
                            var resultBalance = connection.ExecuteScalar(@"UPDATE RBCC01F0 SET cnsl17 = cnsl17 + @amount OUTPUT INSERTED.rv 
                                                                WHERE cnsl34 = @cnsl34 AND cnsl01 = @cnsl01 AND cnsl02 = @cnsl02 AND cnsl05 = @cnsl05 AND rv = @rv",
                            new { cnsl34 = rbccItem?.cnsl34, cnsl01 = rbccItem?.cnsl01, cnsl02 = rbccItem?.cnsl02, cnsl05 = rbccItem?.cnsl05, rv = rbccItem?.rv, amount = amount },
                            transaction: transaction);

                            if (resultBalance != null)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                                return false;
                            }
                        }
                        else
                        {
                            ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                            return false;
                        }
                    }
                    else
                    {
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }



            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteSingle(PNRIGHE Model)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(DELETE_QUERY, Model);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteByReg(string CompanyID, int Year, int Number)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(
                    "DELETE FROM PN_RIGHE WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi",
                    new { n1soci = CompanyID, n1anno = Year, n1regi = Number });
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(PNRIGHE Model, CAUCONT Causal)
        {
            if (Model != null)
            {
                if (Model.N1IMEU.HasValue && Model.N1IMEU.Value >= 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.N1SEGN))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.SelectedGroup?.P1GRUP))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.SelectedAccount?.P2CONT))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3SOTC))
                                {
                                    // check existing PDCANNI
                                    var pdcAnni = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Get(Model.N1SOCI, Model.SelectedGroup?.P1GRUP ?? string.Empty, Model.SelectedAccount?.P2CONT ?? string.Empty, Model.SelectedSubaccount?.P3SOTC ?? string.Empty, Model.N1ANNO);
                                    if (pdcAnni != null)
                                    {
                                        int testDoc = 0;
                                        if ((Causal.cauivaBool && int.TryParse(Model.N1DOCU, out testDoc)) ||
                                            !Causal.cauivaBool)
                                        {
                                            if (Model.SelectedSubaccount == null || (Model.SelectedSubaccount != null &&
                                                (string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3CLFO) || (!string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3CLFO) && Model.SelectedEntity != null && Model.SelectedEntity.abecod > 0))))
                                            {
                                                if (Model.SelectedEntity == null || (Model.SelectedEntity != null && Model.SelectedEntity.abecod > 0 && Model.SelectedPayment != null))
                                                {
                                                    return null;
                                                }
                                                else
                                                {
                                                    return "In caso di cliente/fornitore il codice pagamento è obbligatorio";
                                                }
                                            }
                                            else
                                            {
                                                return "Il sottoconto prevede obbligatoriamente un codice cliente/fornitore";
                                            }
                                        }
                                        else
                                        {
                                            return "La causale contabile prevede IVA quindi il numero documento deve contenere solo numeri e nessun carattere";
                                        }
                                    }
                                    else
                                    {
                                        return "Il sottoconto anagrafico non presenta un sottoconto contabile. verificare l'apertura dell'esercizio contabile";
                                    }
                                }
                                else
                                {
                                    return "Il sottoconto contabile è obbligatorio";
                                }
                            }
                            else
                            {
                                return "Il conto contabile è obbligatorio";
                            }
                        }
                        else
                        {
                            return "Il gruppo contabile è obbligatorio";
                        }
                    }
                    else
                    {
                        return "Il segno è obbligatorio";
                    }
                }
                else
                {
                    return "L'importo deve essere maggiore o uguale a 0";
                }
            }
            else
            {
                return "Errore sconosciuto, impossibile proseguire";
            }
        }

        public string? ValidateModel(CAUCONT SelectedCausal, ObservableCollection<PNIVA>? IVARows, ObservableCollection<PNRIGHE>? Rows, decimal Balance, CAUCONT AccountingCausal, bool IsInsert)
        {

            if ((SelectedCausal.cauivaBool && IVARows != null && IVARows.Count > 0) ||
                (!SelectedCausal.cauivaBool && (IVARows == null || IVARows.Count == 0)))
            {
                if ((!SelectedCausal.causolBool && Rows != null && Rows.Count > 0) ||
                    SelectedCausal.causolBool)
                {
                    // check 0 amounts
                    if (!(Rows ?? new ObservableCollection<PNRIGHE>()).Any(any => any.N1IMEU == 0) || SelectedCausal.cauzer)
                    {
                        // check PDC
                        if (!(Rows ?? new ObservableCollection<PNRIGHE>()).Any(any => string.IsNullOrWhiteSpace(any.pngrup) || string.IsNullOrWhiteSpace(any.pncont) || string.IsNullOrWhiteSpace(any.pnsott)))
                        {
                            if (Balance == 0)
                            {
                                var ivaFlaggedTotalDare = Rows?.Where(w => (w.SelectedSubaccount?.p3coniBool ?? false) && w.N1SEGN == "D").Sum(sum => sum.N1IMEU);
                                var ivaFlaggedTotalAvere = Rows?.Where(w => (w.SelectedSubaccount?.p3coniBool ?? false) && w.N1SEGN == "A").Sum(sum => sum.N1IMEU);

                                var ivaFlaggedTotal = ivaFlaggedTotalDare - ivaFlaggedTotalAvere;
                                if (ivaFlaggedTotal < 0)
                                    ivaFlaggedTotal *= -1;
                                var ivaTotalPlus = IVARows?.Where(w => w.N4SEGN == "+").Sum(sum => sum.N4IVEU);
                                var ivaTotalMinus = IVARows?.Where(w => w.N4SEGN == "-").Sum(sum => sum.N4IVEU);
                                var ivaTotal = ivaTotalPlus - ivaTotalMinus;
                                if (ivaTotal < 0)
                                    ivaTotal *= -1;
                                if (SelectedCausal.causolBool ||
                                    (AccountingCausal.cauivaBool && ivaFlaggedTotal == ivaTotal) ||
                                    !AccountingCausal.cauivaBool ||
                                    (AccountingCausal.cauivaBool && ivaFlaggedTotal != ivaTotal && ConfirmHandler.Confirm($"Il totale delle righe con conto IVA ({(ivaFlaggedTotal ?? 0).ToString("N2")}) e quello delle righe IVA ({(ivaTotal ?? 0).ToString("N2")}) non coincidono, proseguire comunque ?")))
                                {
                                    // check sectional amount
                                    var dareAmount = Rows?.Where(w => w.N1SEGN == "D" && (w.n1clie.HasValue && w.n1clie.Value > 0)).Sum(sum => sum.N1IMEU) ?? 0;
                                    var avereAmount = Rows?.Where(w => w.N1SEGN == "A" && (w.n1clie.HasValue && w.n1clie.Value > 0)).Sum(sum => sum.N1IMEU) ?? 0;
                                    decimal dareDetails = 0;
                                    foreach (var item in (Rows ?? new ObservableCollection<PNRIGHE>()).Where(w => w.N1SEGN == "D" && (w.n1clie.HasValue && w.n1clie.Value > 0)))
                                    {
                                        if (item.ExpireRows != null && item.ExpireRows.Count > 0)
                                            dareDetails += item.ExpireRows.Sum(sum => sum.Amount);
                                    }
                                    decimal avereDetails = 0;
                                    foreach (var item in (Rows ?? new ObservableCollection<PNRIGHE>()).Where(w => w.N1SEGN == "A" && (w.n1clie.HasValue && w.n1clie.Value > 0)))
                                    {
                                        if (item.ExpireRows != null && item.ExpireRows.Count > 0)
                                            avereDetails += item.ExpireRows.Sum(sum => sum.Amount);
                                    }
                                    if (IsInsert || (dareAmount == dareDetails && avereAmount == avereDetails))
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        return "Il totale delle righe cliente/fornitore e le relative scadenze nel sezionale non corrispondono";
                                    }
                                }
                                else
                                {
                                    return "Il totale delle righe con conto IVA e quello delle righe IVA non coincidono";
                                }
                            }
                            else
                            {
                                return "La registrazione è sbilanciata, impossibile proseguire";
                            }
                        }
                        else
                        {
                            return "Il riferimento del piano dei conti è obbligatorio, c'è almeno una riga senza gruppo, conto e/o sottoconto";
                        }
                    }
                    else
                    {
                        return "Ci sono delle righe con importo uguale a 0. Tutte le righe devono avere un importo superiore a 0.";
                    }
                }
                else
                {
                    return "E' necessario che siano presenti delle righe per confermare la registrazione";
                }
            }
            else
            {
                return "Se la causale prevede gestione IVA devono esserci delle righe valide, altrimenti non ci devono essere righe IVA";
            }
        }
        #endregion
    }
}