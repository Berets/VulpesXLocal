

using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Services.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using static VulpesX.Models.Models.Reports.Accounting.IVAReportYearly;

namespace VulpesX.DAL.Accounting;

public interface IPNIVARepository
{

    ObservableCollection<PNIVA>? GetList(string CompanyID);

    ObservableCollection<PNIVA>? GetList(string CompanyID, string IVABookID, string ShowType, DateTime? SinceDate, DateTime? UntilDate, DateTime? ExpireDate, bool OnlyUnpaid);

    ObservableCollection<PNIVA>? GetList(string CompanyID, int Year, int HeadNumber);

    ObservableCollection<PNIVA>? GetListFull(string CompanyID, int Year, int HeadNumber);

    PNIVA? Get(string CompanyID, int Year, int Number, int Row);

    bool Exists(string N4SOCI, int N4ANNO, int N4REGI, int N4RIGA);

    bool CheckProtocolAlreadyUsed(string N4SOCI, int N4ANNO, int N4REGI, string ProtocolID, string IVABook);

    Tuple<string, DateTime>? GetLastProtocolUsed(string N4SOCI, DateTime N4DARE, int N4REGI, string IVABook);

    #region Reports
    IVABookReport? PrintIVABook(string CompanyID, int AccountingYear, LIBRIIVA IVABook, DateTime PrintSince, DateTime PrintUntil, bool IsDefinitive);

    IVAReport? PrintIVARecap(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive, DateTime Now);

    bool UpdatePrintedDefinitives(string CompanyID, string IVABookID, string NumeratorID, int NumeratorYear, List<PNIVA> Rows, int LastPagePrinted, string UserID, DateTime ToDate);

    IVAReport? PrintIVARecap(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive, LIBRIIVA DefaultIVABookNumerator, DateTime Now);

    IVAReportYearly? PrintIVARecapYearly(string CompanyID, int Year);

    bool UpdateLiquidationDefinitive(string CompanyID, string NumeratorID, int NumeratorYear, int LastPagePrinted, string UserID, DateTime Until, decimal Total, decimal CompensationAmount);

    IVAReportDetails? PrintIVARecapDetails(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(PNIVA Model);

    bool Update(PNIVA Model);

    bool DeleteSingle(PNIVA Model);

    bool DeleteByReg(string CompanyID, int Year, int Number);

    string? Validate(PNIVA Model, bool IsInsert);
    #endregion

    #region Utilities
    List<VATRecap>? GetVATRecap(List<PNIVA>? Rows, string? IVABookType);
    #endregion
}

public class PNIVARepository : RepositoryBase, IPNIVARepository
{
    public PNIVARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNIVA>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PNIVA>(
                "SELECT * FROM PNIVA WHERE N4SOCI = @n4soci",
                new { n4soci = CompanyID });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNIVA>? GetList(string CompanyID, string IVABookID, string ShowType, DateTime? SinceDate, DateTime? UntilDate, DateTime? ExpireDate, bool OnlyUnpaid)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query;
            if (!OnlyUnpaid)
            {
                query = $@"SELECT i.*, CONCAT(c.caucod, ' ' , TRIM(c.caudes)) AS CausalFullDescription, CONCAT(e.abecod, ' ' , TRIM(e.abers1)) AS EntityFullDescription FROM PNIVA AS i
                        INNER JOIN LIBRIIVA AS l ON l.livcod=i.N4LIBR
                        LEFT JOIN CAUCONT AS c ON c.caucod=i.N4CAUS
                        INNER JOIN ABE AS e ON e.abecod=i.N4SOTT
                        WHERE i.N4SOCI = @n4soci 
                        {(!string.IsNullOrWhiteSpace(IVABookID) ? " AND i.N4LIBR=@n4libr" : null)}
                        {(ShowType == "*" ? null : (ShowType == "P" ? " AND i.N4DAST IS NOT NULL AND i.N4FLGS = '*'" : " AND i.N4DAST IS NULL AND i.N4FLGS IS NULL"))}
                        {(SinceDate.HasValue ? " AND i.N4DARE >= @since" : null)}
                        {(UntilDate.HasValue ? " AND i.N4DARE <= @until" : null)}
                        {(ExpireDate.HasValue ? " AND i.N4DTSCPG = @n4dtscpg" : null)}
                        ORDER BY CAST(i.N4DOCU AS INT)";
            }
            else
            {
                query = $@"SELECT i.*, CONCAT(c.caucod, ' ' , TRIM(c.caudes)) AS CausalFullDescription, CONCAT(e.abecod, ' ' , TRIM(e.abers1)) AS EntityFullDescription FROM PNIVA AS i
                        INNER JOIN LIBRIIVA AS l ON l.livcod=i.N4LIBR
                        LEFT JOIN CAUCONT AS c ON c.caucod=i.N4CAUS
                        INNER JOIN ABE AS e ON e.abecod=i.N4SOTT
                        WHERE i.N4SOCI = @n4soci AND i.N4DTPGEF IS NULL
                        {(!string.IsNullOrWhiteSpace(IVABookID) ? " AND i.N4LIBR=@n4libr" : null)}
                        {(ShowType == "*" ? null : (ShowType == "P" ? " AND i.N4DAST IS NOT NULL AND i.N4FLGS = '*'" : " AND i.N4DAST IS NULL AND i.N4FLGS IS NULL"))}
                        {(UntilDate.HasValue ? " AND i.N4DARE <= @until" : null)}
                        {(ExpireDate.HasValue ? " AND i.N4DTSCPG = @n4dtscpg" : null)}
                        ORDER BY i.N4DTSCPG, CAST(i.N4DOCU AS INT)";
            }
            var list = connection.Query<PNIVA>(query,
                new { n4soci = CompanyID, n4libr = IVABookID, n4dtscpg = ExpireDate?.Date ?? null, since = SinceDate?.Date ?? null, until = UntilDate?.Date ?? null });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNIVA>? GetList(string CompanyID, int Year, int HeadNumber)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNIVA>(
                "SELECT * FROM PNIVA WHERE N4SOCI = @cid AND N4ANNO = @yea AND N4REGI = @numb ORDER BY N4RIGA",
                new { cid = CompanyID, yea = Year, numb = HeadNumber });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNIVA>? GetListFull(string CompanyID, int Year, int HeadNumber)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNIVA>(
                @"SELECT i.*, CONCAT(l.livcod, ' ', TRIM(l.livdes)) AS IVABookDescription, CONCAT(a.asscod, ' ', a.assali , ' ', TRIM(a.assdes)) AS RateDescription FROM PNIVA AS i
                        INNER JOIN LIBRIIVA AS l ON l.livcod=i.N4LIBR
                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=i.N4ASSF AND a.assali=i.n4assa
                        WHERE i.N4SOCI = @cid AND i.N4ANNO = @yea AND i.N4REGI = @numb 
                        ORDER BY i.N4RIGA",
                new { cid = CompanyID, yea = Year, numb = HeadNumber });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNIVA? Get(string CompanyID, int Year, int Number, int Row)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNIVA>(
                "SELECT * FROM PNIVA WHERE N4SOCI = @n4soci AND N4ANNO = @n4anno AND N4REGI = @n4regi AND N4RIGA = @n4riga",
                new { n4soci = CompanyID, n4anno = Year, n4regi = Number, n4riga = Row })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string N4SOCI, int N4ANNO, int N4REGI, int N4RIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNIVA WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4REGI = @N4REGI AND N4RIGA = @N4RIGA",
                new { N4SOCI = N4SOCI, N4ANNO = N4ANNO, N4REGI = N4REGI, N4RIGA = N4RIGA }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool CheckProtocolAlreadyUsed(string N4SOCI, int N4ANNO, int N4REGI, string ProtocolID, string IVABook)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM PNIVA 
                        WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4DOCU = @prot AND N4LIBR = @ivab AND N4REGI <> @n4regi",
                new { N4SOCI = N4SOCI, N4ANNO = N4ANNO, prot = ProtocolID, ivab = IVABook, n4regi = N4REGI }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public Tuple<string, DateTime>? GetLastProtocolUsed(string N4SOCI, DateTime N4DARE, int N4REGI, string IVABook)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<PNIVA>(
                @"SELECT TOP(1) N4DOCU, N4DADO, N4DARE FROM PNIVA 
                        WHERE N4SOCI = @N4SOCI AND N4LIBR = @ivab AND N4REGI <> @n4regi
                        ORDER BY N4DARE DESC, CAST(N4DOCU AS INT) DESC",
                new { N4SOCI = N4SOCI, ivab = IVABook, n4regi = N4REGI }).FirstOrDefault();

            if (result != null)
            {
                if (result.N4DARE.HasValue)
                {
                    if (N4DARE.Year <= result.N4DARE.Value.Year)
                        return (!string.IsNullOrEmpty(result.N4DOCU) && result.N4DADO.HasValue) ? new Tuple<string, DateTime>(result.N4DOCU, result.N4DADO.Value) : new Tuple<string, DateTime>("0", N4DARE);
                    else
                        return new Tuple<string, DateTime>("0", N4DARE);
                }
                else
                {
                    return new Tuple<string, DateTime>("0", N4DARE);
                }
            }
            else
            {
                return new Tuple<string, DateTime>("0", N4DARE);
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Reports
    public IVABookReport? PrintIVABook(string CompanyID, int AccountingYear, LIBRIIVA IVABook, DateTime PrintSince, DateTime PrintUntil, bool IsDefinitive)
    {
        try
        {
            var accounting = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, AccountingYear);
            int startingPage = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, PrintSince.Year, new GenericIDDescription() { ID = $"R{IVABook.livcod.Trim()}", Description = $"Numeratore pagine {IVABook.FullDescriptionSearchable}" }, false, 0);
            using var connection = GetOpenConnection();
            PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
            var rows = connection.Query<PNIVA>(
                @"SELECT p.*, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, p.N4SOTT AS CustomerID, c.abecod, c.abers1 AS CompanyDescription, c.abeind AS CompanyAddress, CONCAT(FORMAT(c.abecap,'00000'), ' ',TRIM(c.abeloc), ' (' , TRIM(c.abepro), ')') AS CompanyCity,c.abepiv AS VATID FROM PNIVA AS p
                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=p.N4ASSF AND a.assali=p.N4assa
                        INNER JOIN ABE AS c ON c.abecod = p.N4SOTT
                        where p.n4soci = @cid and p.N4LIBR=@liv and p.N4DARE >= @since and p.n4dare <= @until AND
                            p.N4DAST IS NULL AND p.N4FLGS IS NULL
                        ORDER BY CAST(p.N4DOCU AS INT)",
                new { cid = CompanyID, liv = IVABook.livcod, since = PrintSince.Date, until = PrintUntil });

            var result = new IVABookReport()
            {
                AccountingYear = AccountingYear,
                IVABook = IVABook,
                StartingPage = startingPage,
                PrintSince = PrintSince,
                PrintUntil = PrintUntil,
                TemporaryText = IsDefinitive ? "STAMPA DEFINITIVA" : "STAMPA PROVVISORIA",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                Rows = new List<PNIVA>()
            };

            // clear expire date if not IVA per cassa
            if (accounting != null && !accounting.eseivavenBool)
            {
                Parallel.ForEach(rows, row =>
                {
                    row.N4DTSCPG = null;
                });
            }

            decimal? totalDocument = null;
            foreach (var item in rows)
            {
                var existing = result.Rows.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT).FirstOrDefault();
                if (existing == null)
                {
                    result.Rows.Add(new PNIVA { N4SOCI = CompanyID, DocumentTotal = totalDocument });

                    var totPlus = rows.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT && w.N4SEGN == "+").Sum(sum => sum.N4IMEU + sum.N4IVEU);
                    var totMinus = rows.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT && w.N4SEGN == "-").Sum(sum => sum.N4IMEU + sum.N4IVEU);
                    totalDocument = totPlus - totMinus;
                    result.Rows.Add(item);
                }
                else
                {
                    result.Rows.Add(new PNIVA
                    {
                        N4SOCI = CompanyID,
                        N4ANNO = item.N4ANNO,
                        N4REGI = item.N4REGI,
                        N4RIGA = item.N4RIGA,
                        N4IMEU = (item.N4IMEU ?? 0),
                        N4IVEU = (item.N4IVEU ?? 0),
                        N4IIEU = item.N4IIEU.HasValue && item.N4IIEU.Value > 0 ? item.N4IIEU.Value : null,
                        N4SEGN = item.N4SEGN,
                        RateFullDescription = item.RateFullDescription,
                        CustomerID = item.N4SOTT ?? 0,
                        N4DTSCPG = item.N4DTSCPG
                    });
                }
            }
            result.Rows.Add(new PNIVA { N4SOCI = CompanyID, DocumentTotal = totalDocument, });

            result.VATs = GetVATRecap(result.Rows, null);
            return result;

        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public bool UpdatePrintedDefinitives(string CompanyID, string IVABookID, string NumeratorID, int NumeratorYear, List<PNIVA> Rows, int LastPagePrinted, string UserID, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                // update numerator
                connection.Execute("UPDATE NUMREG SET PERNUM=@lastpage WHERE PERSOC=@cid AND PERCOD=@numcod AND PERANN=@numann",
                    new { lastpage = LastPagePrinted, cid = CompanyID, numcod = NumeratorID, numann = NumeratorYear }, transaction);
                // update IVA book last print
                connection.Execute("UPDATE LIBRIIVA SET livulst=@tod, updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time', updatedUserID=@uid WHERE livcod=@id",
                    new { id = IVABookID, uid = UserID, tod = ToDate }, transaction);
                // update rows printed
                foreach (var row in Rows)
                {
                    connection.Execute("UPDATE PNIVA SET N4DAST=@tod, N4FLGS='*' WHERE N4SOCI=@cid AND N4ANNO=@yea AND N4REGI=@regi AND N4RIGA=@riga",
                        new { cid = CompanyID, yea = row.N4ANNO, regi = row.N4REGI, riga = row.N4RIGA, tod = ToDate }, transaction);
                }
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public IVAReport? PrintIVARecap(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive, LIBRIIVA DefaultIVABookNumerator, DateTime Now)
    {
        try
        {
            int startingPage = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, PrintSince.Year, new GenericIDDescription() { ID = $"R{DefaultIVABookNumerator.livcod.Trim()}", Description = $"Numeratore pagine {DefaultIVABookNumerator.FullDescriptionSearchable}" }, false);
            using var connection = GetOpenConnection();

          
                PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
                string definitiveWhere = IsDefinitive ? " AND N4DAST IS NOT NULL AND N4FLGS IS NOT NULL" : "AND N4DAST IS NULL AND N4FLGS IS NULL";
                string query
                    = IsCash ? $@"SELECT *, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CAST(CASE WHEN a.asssplpay = 'S' THEN 1 ELSE 0 END AS bit) AS IsSplitPayment FROM PNIVA 
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=N4ASSF AND a.assali=N4assa
                                INNER JOIN LIBRIIVA AS l ON l.livcod=N4LIBR AND l.livili = 1
                                WHERE N4SOCI = @cid AND N4DTPGEF >= @since AND N4DTPGEF <= @until" :
                                $@"SELECT *, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CAST(CASE WHEN a.asssplpay = 'S' THEN 1 ELSE 0 END AS bit) AS IsSplitPayment FROM PNIVA 
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=N4ASSF AND a.assali=N4assa                                    
                                INNER JOIN LIBRIIVA AS l ON l.livcod=N4LIBR AND l.livili = 1
                                WHERE N4SOCI = @cid AND N4DARE >= @since AND N4DARE <= @until {definitiveWhere}";

                var rows = connection.Query<PNIVA>($"{query}",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });
                var saldo = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetLastByPreviousMonth(CompanyID, PrintSince.Year, PrintSince.Month);

                var result = new IVAReport()
                {
                    Year = Now.Year,
                    StartingPage = startingPage,
                    PrintSince = PrintSince,
                    PrintUntil = PrintUntil,
                    PreviousMonthAmount = saldo != null && (saldo.salsal ?? 0) != 0 && saldo.saldeb == 0 ? (saldo.salsal ?? 0) * -1 : 0,
                    TemporaryText = IsDefinitive ? "RIEPILOGO LIQUIDAZIONE IVA" : "SIMULAZIONE LIQUIDAZIONE IVA",
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    Rows = new List<PNIVA>(rows),
                    SalesVATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                    PurchasesVATs = GetVATRecap(new List<PNIVA>(rows), "A")
                };

                return result;
           
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public IVAReport? PrintIVARecap(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive, DateTime Now)
    {
        throw new NotImplementedException();
    }

    public IVAReportYearly? PrintIVARecapYearly(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = new IVAReportYearly()
            {
                PrintSince = new DateTime(Year, 1, 1),
                PrintUntil = new DateTime(Year, 12, 31, 23, 59, 59),
                TemporaryText = "RIEPILOGO LIQUIDAZIONE IVA ANNUALE",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                Rows = new List<PNIVA>()
            };

            var rows = connection.Query<PNIVA>($@"SELECT p.*, CONCAT(TRIM(l.livcod), ' ', TRIM(l.livdes)) IVABookFullDescription, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription,p.N4SOTT AS CustomerID, c.abecod, c.abers1 AS CompanyDescription, c.abeind AS CompanyAddress, CONCAT(FORMAT(c.abecap,'00000'), ' ',TRIM(c.abeloc), ' (' , TRIM(c.abepro), ')') AS CompanyCity,c.abepiv AS VATID FROM PNIVA AS p
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=p.N4ASSF AND a.assali=p.N4assa                                    
                                INNER JOIN LIBRIIVA AS l ON l.livcod=p.N4LIBR
                                INNER JOIN ABE AS c ON c.abecod = p.N4SOTT
                                WHERE p.N4SOCI = @cid AND p.N4FLGS='*' AND p.N4DAST >= @since AND p.N4DAST <= @until AND p.n4tmppn <> 'S'
                                ORDER BY IVABookType DESC, IVABookFullDescription",
                    new { cid = CompanyID, since = result.PrintSince, until = result.PrintUntil });

            #region Recap
            result.IVABookRecaps = new List<IVAReportYearly.DetailSection>();
            foreach (var row in rows)
            {
                result.Rows.Add(row);
                var existing = result.IVABookRecaps.Where(w => w.IVABookFullDescription == row.IVABookFullDescription).FirstOrDefault();
                if (existing != null)
                {
                    existing.Rows?.Add(row);
                }
                else
                {
                    result.IVABookRecaps.Add(new IVAReportYearly.DetailSection()
                    {
                        IVABookFullDescription = row.IVABookFullDescription,
                        IVABookType = row.IVABookType,
                        Rows = new List<PNIVA>() { row }
                    });
                }
            }

            foreach (var rec in result.IVABookRecaps)
            {
                rec.VATs = GetVATRecap(rec.Rows, null);
            }
            #endregion

            #region TABSALDI1
            result.PaymentsInfo = new IVAReportYearly.PaymentsRecap()
            {
                VATDebit = result.SalesTotal,
                VATCredit = result.PurchasesTotal,
                Payments = new List<IVAReportYearly.VATPayment>()
            };
            var lastPreviousYear = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetLastByMonth(CompanyID, Year - 1, 12);
            if (lastPreviousYear != null && lastPreviousYear.salsal.HasValue && lastPreviousYear.salsal.Value != 0)
                result.PaymentsInfo.InitialCredit = lastPreviousYear.salsal.Value;
            var saldi = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetListByYear(CompanyID, Year);

            decimal previousCredit = 0;
            foreach (var item in (saldi ?? new ObservableCollection<TABSALDI1>()).OrderBy(o => o.salpag))
            {
                result.PaymentsInfo.Payments.Add(new IVAReportYearly.VATPayment()
                {
                    PaymentDate = item.salpag,
                    DebitAmount = (item.saldeb ?? 0) - previousCredit,
                    CreditAmount = item.salsal ?? 0
                });

                previousCredit = (item.salsal ?? 0) < 0 ? (item.salsal ?? 0) * -1 : (item.salsal ?? 0);
            }
            #endregion

            #region Unpaid recap
            var unpaidRows = connection.Query<PNIVA>($@"SELECT p.*, CONCAT(TRIM(l.livcod), ' ', TRIM(l.livdes)) IVABookFullDescription, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription,p.N4SOTT AS CustomerID, c.abecod, c.abers1 AS CompanyDescription, c.abeind AS CompanyAddress, CONCAT(FORMAT(c.abecap,'00000'), ' ',TRIM(c.abeloc), ' (' , TRIM(c.abepro), ')') AS CompanyCity,c.abepiv AS VATID FROM PNIVA AS p
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=p.N4ASSF AND a.assali=p.N4assa                                    
                                INNER JOIN LIBRIIVA AS l ON l.livcod=p.N4LIBR
                                INNER JOIN ABE AS c ON c.abecod = p.N4SOTT
                                WHERE p.N4SOCI = @cid AND p.N4DARE <= @until AND p.N4FLGS='*' AND p.N4FLIVCA = 'S' AND p.n4tmppn <> 'S' AND (p.N4DTPGEF IS NULL OR p.N4DTPGEF > @until)
                                ORDER BY p.N4DTSCPG",
                            new { cid = CompanyID, until = result.PrintUntil });
            result.UnpaidInfo = new IVAReportYearly.UnpaidRecap()
            {
                Rows = new List<PNIVA>(),
                IVABookRecaps = new List<IVAReportYearly.DetailSection>()
            };

            foreach (var row in unpaidRows)
            {
                result.UnpaidInfo.Rows.Add(row);
                var existing = result.UnpaidInfo.IVABookRecaps.Where(w => w.IVABookFullDescription == row.IVABookFullDescription).FirstOrDefault();
                if (existing != null)
                {
                    existing.Rows?.Add(row);
                }
                else
                {
                    result.UnpaidInfo.IVABookRecaps.Add(new IVAReportYearly.DetailSection()
                    {
                        IVABookFullDescription = row.IVABookFullDescription,
                        IVABookType = row.IVABookType,
                        Rows = new List<PNIVA>() { row }
                    });
                }
            }
            #endregion

            #region Unpaid expire recap
            result.UnpaidExpireInfo = new IVAReportYearly.UnpaidExpireRecap() { Expires = new List<IVAReportYearly.UnpaidExpireRecapItem>() };

            foreach (var row in unpaidRows)
            {
                var existing = result.UnpaidExpireInfo.Expires.Where(w => w.ExpireDate == row.N4DTSCPG).FirstOrDefault();
                if (existing == null)
                {
                    var newItem = new UnpaidExpireRecapItem()
                    {
                        ExpireDate = row.N4DTSCPG.HasValue ? row.N4DTSCPG.Value : DateTime.MinValue
                    };
                    if (row.IVABookType == "V")
                    {
                        if (row.N4SEGN == "+")
                        {
                            newItem.SalesAmount += row.N4IMEU ?? 0;
                            newItem.SalesVATAmount += row.N4IVEU ?? 0;
                            newItem.SalesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            newItem.SalesAmount -= row.N4IMEU ?? 0;
                            newItem.SalesVATAmount -= row.N4IVEU ?? 0;
                            newItem.SalesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                    else
                    {
                        if (row.N4SEGN == "+")
                        {
                            newItem.PurchasesAmount += row.N4IMEU ?? 0;
                            newItem.PurchasesVATAmount += row.N4IVEU ?? 0;
                            newItem.PurchasesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            newItem.PurchasesAmount -= row.N4IMEU ?? 0;
                            newItem.PurchasesVATAmount -= row.N4IVEU ?? 0;
                            newItem.PurchasesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                    result.UnpaidExpireInfo.Expires.Add(newItem);
                }
                else
                {
                    if (row.IVABookType == "V")
                    {
                        if (row.N4SEGN == "+")
                        {
                            existing.SalesAmount += row.N4IMEU ?? 0;
                            existing.SalesVATAmount += row.N4IVEU ?? 0;
                            existing.SalesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            existing.SalesAmount -= row.N4IMEU ?? 0;
                            existing.SalesVATAmount -= row.N4IVEU ?? 0;
                            existing.SalesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                    else
                    {
                        if (row.N4SEGN == "+")
                        {
                            existing.PurchasesAmount += row.N4IMEU ?? 0;
                            existing.PurchasesVATAmount += row.N4IVEU ?? 0;
                            existing.PurchasesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            existing.PurchasesAmount -= row.N4IMEU ?? 0;
                            existing.PurchasesVATAmount -= row.N4IVEU ?? 0;
                            existing.PurchasesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                }
            }
            #endregion

            return result;
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public bool UpdateLiquidationDefinitive(string CompanyID, string NumeratorID, int NumeratorYear, int LastPagePrinted, string UserID, DateTime Until, decimal Total, decimal CompensationAmount)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var tabSaldiRepository = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDIRepository>();
                var tabSaldi1Repository = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>();
                // update numerator
                connection.Execute("UPDATE NUMREG SET PERNUM=@lastpage WHERE PERSOC=@cid AND PERCOD=@numcod AND PERANN=@numann",
                    new { lastpage = LastPagePrinted, cid = CompanyID, numcod = NumeratorID, numann = NumeratorYear }, transaction);
                // update TABSALDI
                var tabsaldi = tabSaldiRepository.Get(CompanyID, Until.Year);
                if (tabsaldi == null)
                {
                    // create TABSALDI
                    connection.Execute(tabSaldiRepository.INSERT_QUERY, new TABSALDI() { salsoc = CompanyID, salann = Until.Year }, transaction);
                }
                var tabsaldi1 = tabSaldi1Repository.Get(CompanyID, Until.Year, Until.Month, Until);
                if (tabsaldi1 == null)
                {
                    // add TABSALDI1
                    connection.Execute(tabSaldi1Repository.INSERT_QUERY, new TABSALDI1()
                    {
                        salsoc = CompanyID,
                        salann = Until.Year,
                        salmes = Until.Month,
                        salpag = Until.Date,
                        saldeb = Total > 0 ? Total : 0,
                        salsal = Total < 0 ? Total : 0,
                        salcom = CompensationAmount,
                    },
                        transaction);
                }
                else
                {
                    tabsaldi1.saldeb = Total > 0 ? Total : 0;
                    tabsaldi1.salsal = Total < 0 ? Total : 0;
                    connection.Execute(tabSaldi1Repository.UPDATE_QUERY, tabsaldi1, transaction);
                }
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public IVAReportDetails? PrintIVARecapDetails(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                connection.Open();
                PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
                var result = new IVAReportDetails()
                {
                    PrintSince = PrintSince,
                    PrintUntil = PrintUntil,
                    TemporaryText = IsDefinitive ? "RIEPILOGO LIQUIDAZIONE IVA" : "SIMULAZIONE LIQUIDAZIONE IVA",
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                };

                #region Sales previous period unpaid
                var rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF IS NULL AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.SalesPrevious = new IVAReportDetails.DetailSection()
                {
                    Text = $"VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                    RecapText = $"RIEPILOGO VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                    IVABookType = "V",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                };

                #endregion

                #region Sales current period unpaid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND (pi.N4DTPGEF IS NULL OR pi.N4DTPGEF > @until) AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.SalesCurrent = new IVAReportDetails.DetailSection()
                {
                    Text = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                    RecapText = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                    IVABookType = "V",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                };

                #endregion

                #region Sales previous period paid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.SalesPreviousPaid = new IVAReportDetails.DetailSection()
                {
                    Text = $"VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                    RecapText = $"RIEPILOGO VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                    IVABookType = "V",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                };

                #endregion

                #region Sales current period paid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.SalesCurrentPaid = new IVAReportDetails.DetailSection()
                {
                    Text = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                    RecapText = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                    IVABookType = "V",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                };

                #endregion

                #region Purchases previous period unpaid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF IS NULL AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.PurchasesPrevious = new IVAReportDetails.DetailSection()
                {
                    Text = $"ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                    RecapText = $"RIEPILOGO ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                    IVABookType = "A",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
                };

                #endregion

                #region Purchases current period unpaid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND (pi.N4DTPGEF IS NULL OR pi.N4DTPGEF > @until) AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.PurchasesCurrent = new IVAReportDetails.DetailSection()
                {
                    Text = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                    RecapText = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                    IVABookType = "A",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
                };

                #endregion

                #region Purchases previous period paid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.PurchasesPreviousPaid = new IVAReportDetails.DetailSection()
                {
                    Text = $"ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                    RecapText = $"RIEPILOGO ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                    IVABookType = "A",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
                };

                #endregion

                #region Purchases current period paid
                rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                        new { cid = CompanyID, since = PrintSince, until = PrintUntil });

                result.PurchasesCurrentPaid = new IVAReportDetails.DetailSection()
                {
                    Text = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                    RecapText = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                    IVABookType = "A",
                    Rows = new List<PNIVA>(rows),
                    VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
                };

                #endregion

                return result;
            }
            else
            {
                ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                return null;
            }
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PNIVA (N4SOCI,N4ANNO,N4REGI,N4RIGA,N4DOCU,N4RIFE,N4DARE,N4DADO,N4DARI,N4CAUS,N4SOTT,N4TCLF,N4IMPO,N4SEGN,N4LIBR,N4ASSF,n4assa,N4IIVA,N4INDP,N4INDV,N4DAST,N4FLGS,N4IMEU,N4IVEU,N4IIEU,N4TIDO,n4donu,n4tmppn,N4FLSPESO,N4LIBALT,N4FLIVCA,N4IMPPAG,N4DTSCAD,N4DTPGEF,N4DTSCPG) OUTPUT INSERTED.rv VALUES(@N4SOCI,@N4ANNO,@N4REGI,@N4RIGA,@N4DOCU,@N4RIFE,@N4DARE,@N4DADO,@N4DARI,@N4CAUS,@N4SOTT,@N4TCLF,@N4IMPO,@N4SEGN,@N4LIBR,@N4ASSF,@n4assa,@N4IIVA,@N4INDP,@N4INDV,@N4DAST,@N4FLGS,@N4IMEU,@N4IVEU,@N4IIEU,@N4TIDO,@n4donu,@n4tmppn,@N4FLSPESO,@N4LIBALT,@N4FLIVCA,@N4IMPPAG,@N4DTSCAD,@N4DTPGEF,@N4DTSCPG)";
    public string UPDATE_QUERY => "UPDATE PNIVA SET N4SOCI = @N4SOCI,N4ANNO = @N4ANNO,N4REGI = @N4REGI,N4RIGA = @N4RIGA,N4DOCU = @N4DOCU,N4RIFE = @N4RIFE,N4DARE = @N4DARE,N4DADO = @N4DADO,N4DARI = @N4DARI,N4CAUS = @N4CAUS,N4SOTT = @N4SOTT,N4TCLF = @N4TCLF,N4IMPO = @N4IMPO,N4SEGN = @N4SEGN,N4LIBR = @N4LIBR,N4ASSF = @N4ASSF,n4assa = @n4assa,N4IIVA = @N4IIVA,N4INDP = @N4INDP,N4INDV = @N4INDV,N4DAST = @N4DAST,N4FLGS = @N4FLGS,N4IMEU = @N4IMEU,N4IVEU = @N4IVEU,N4IIEU = @N4IIEU,N4TIDO = @N4TIDO,n4donu = @n4donu,n4tmppn = @n4tmppn,N4FLSPESO = @N4FLSPESO,N4LIBALT = @N4LIBALT,N4FLIVCA = @N4FLIVCA,N4IMPPAG = @N4IMPPAG,N4DTSCAD = @N4DTSCAD,N4DTPGEF = @N4DTPGEF,N4DTSCPG = @N4DTSCPG OUTPUT INSERTED.rv WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4REGI = @N4REGI AND N4RIGA = @N4RIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PNIVA OUTPUT DELETED.rv WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4REGI = @N4REGI AND N4RIGA = @N4RIGA AND rv = @rv";

    public bool Insert(PNIVA Model)
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

    public bool Update(PNIVA Model)
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

    public bool DeleteSingle(PNIVA Model)
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
                "DELETE FROM PNIVA OUTPUT DELETED.rv WHERE N4SOCI = @n4soci AND N4ANNO = @n4anno AND N4REGI = @n4regi",
                new { n4soci = CompanyID, n4anno = Year, n4regi = Number });
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

    public string? Validate(PNIVA Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.N4SEGN))
            {
                if (!string.IsNullOrWhiteSpace(Model.N4ASSF) && !string.IsNullOrWhiteSpace(Model.n4assa))
                {
                    if (!string.IsNullOrWhiteSpace(Model.N4LIBR))
                    {
                        return null;
                    }
                    else
                    {
                        return "Il libro IVA è obbligatorio";
                    }
                }
                else
                {
                    return "L'aliquota è obbligatoria";
                }
            }
            else
            {
                return "Il segno è obbligatorio";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

    #region Utilities
    public List<VATRecap>? GetVATRecap(List<PNIVA>? Rows, string? IVABookType)
    {
        if (Rows == null)
            return null;

        var result = new List<VATRecap>();
        List<PNIVA> rowsData;
        if (string.IsNullOrWhiteSpace(IVABookType))
            rowsData = Rows.OrderBy(o => o.RateFullDescription).ToList();
        else
            rowsData = Rows.Where(w => w.IVABookType == IVABookType).OrderBy(o => o.RateFullDescription).ToList();
        foreach (var item in rowsData.Where(w => !string.IsNullOrWhiteSpace(w.RateFullDescription)))
        {
            var existing = result.Where(w => w.RateFullDescription == item.RateFullDescription).FirstOrDefault();
            if (existing == null)
            {
                var newRecap = new VATRecap()
                {
                    RateFullDescription = item.RateFullDescription ?? string.Empty,
                    IsSplitPayment = item.IsSplitPayment
                };
                if (item.N4SEGN == "+")
                {
                    newRecap.TotalAmount += (item.N4IMEU ?? 0);
                    newRecap.TotalVATAmount += (item.N4IVEU ?? 0);
                    newRecap.TotalNoVATAmount += (item.N4IIEU ?? 0);
                }
                else
                {
                    newRecap.TotalAmount -= (item.N4IMEU ?? 0);
                    newRecap.TotalVATAmount -= (item.N4IVEU ?? 0);
                    newRecap.TotalNoVATAmount -= (item.N4IIEU ?? 0);
                }
                result.Add(newRecap);
            }
            else
            {
                if (item.N4SEGN == "+")
                {
                    existing.TotalAmount += (item.N4IMEU ?? 0);
                    existing.TotalVATAmount += (item.N4IVEU ?? 0);
                    existing.TotalNoVATAmount += (item.N4IIEU ?? 0);
                }
                else
                {
                    existing.TotalAmount -= (item.N4IMEU ?? 0);
                    existing.TotalVATAmount -= (item.N4IVEU ?? 0);
                    existing.TotalNoVATAmount -= (item.N4IIEU ?? 0);
                }
            }
        }

        return result;
    }
    #endregion
}

public class PNIVAUfpRepository : RepositoryBase, IPNIVARepository
{
    public PNIVAUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNIVA>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PNIVA>(
                "SELECT * FROM PN_IVA WHERE N4SOCI = @n4soci",
                new { n4soci = CompanyID });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNIVA>? GetList(string CompanyID, string IVABookID, string ShowType, DateTime? SinceDate, DateTime? UntilDate, DateTime? ExpireDate, bool OnlyUnpaid)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query;
            if (!OnlyUnpaid)
            {
                query = $@"SELECT i.*, CONCAT(c.caucod, ' ' , TRIM(c.caudes)) AS CausalFullDescription, CONCAT(e.abecod, ' ' , TRIM(e.abers1)) AS EntityFullDescription FROM PN_IVA AS i
                        INNER JOIN LIBRIIVA AS l ON l.livcod=i.N4LIBR
                        LEFT JOIN CAUCONT AS c ON c.caucod=i.N4CAUS
                        INNER JOIN ANAG_BASE AS e ON e.abecod=i.N4SOTT
                        WHERE i.N4SOCI = @n4soci 
                        {(!string.IsNullOrWhiteSpace(IVABookID) ? " AND i.N4LIBR=@n4libr" : null)}
                        {(ShowType == "*" ? null : (ShowType == "P" ? " AND (i.N4DAST IS NOT NULL AND i.N4DAST <> '1753-01-01') AND i.N4FLGS = '*'" : " AND (i.N4DAST IS NULL OR i.N4DAST = '1753-01-01') AND (i.N4FLGS IS NULL OR i.N4FLGS = '')"))}
                        {(SinceDate.HasValue ? " AND i.N4DARE >= @since" : null)}
                        {(UntilDate.HasValue ? " AND i.N4DARE <= @until" : null)}
                        {(ExpireDate.HasValue ? " AND i.N4DTSCPG = @n4dtscpg" : null)}
                        ORDER BY CAST(i.N4DOCU AS INT)";
            }
            else
            {
                query = $@"SELECT i.*, CONCAT(c.caucod, ' ' , TRIM(c.caudes)) AS CausalFullDescription, CONCAT(e.abecod, ' ' , TRIM(e.abers1)) AS EntityFullDescription FROM PN_IVA AS i
                        INNER JOIN LIBRIIVA AS l ON l.livcod=i.N4LIBR
                        LEFT JOIN CAUCONT AS c ON c.caucod=i.N4CAUS
                        INNER JOIN ANAG_BASE AS e ON e.abecod=i.N4SOTT
                        WHERE i.N4SOCI = @n4soci AND i.N4DTPGEF IS NULL
                        {(!string.IsNullOrWhiteSpace(IVABookID) ? " AND i.N4LIBR=@n4libr" : null)}
                        {(ShowType == "*" ? null : (ShowType == "P" ? " AND (i.N4DAST IS NOT NULL AND i.N4DAST <> '1753-01-01') AND i.N4FLGS = '*'" : " AND (i.N4DAST IS NULL OR i.N4DAST = '1753-01-01') AND (i.N4FLGS IS NULL OR i.N4FLGS = '')"))}
                        {(UntilDate.HasValue ? " AND i.N4DARE <= @until" : null)}
                        {(ExpireDate.HasValue ? " AND i.N4DTSCPG = @n4dtscpg" : null)}
                        ORDER BY i.N4DTSCPG, CAST(i.N4DOCU AS INT)";
            }
            var list = connection.Query<PNIVA>(query,
                new { n4soci = CompanyID, n4libr = IVABookID, n4dtscpg = ExpireDate?.Date ?? null, since = SinceDate?.Date ?? null, until = UntilDate?.Date ?? null });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNIVA>? GetList(string CompanyID, int Year, int HeadNumber)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNIVA>(
                "SELECT * FROM PN_IVA WHERE N4SOCI = @cid AND N4ANNO = @yea AND N4REGI = @numb ORDER BY N4RIGA",
                new { cid = CompanyID, yea = Year, numb = HeadNumber });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNIVA>? GetListFull(string CompanyID, int Year, int HeadNumber)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNIVA>(
                @"SELECT i.*, CONCAT(l.livcod, ' ', TRIM(l.livdes)) AS IVABookDescription, CONCAT(a.asscod, ' ', a.assali , ' ', TRIM(a.assdes)) AS RateDescription FROM PN_IVA AS i
                        INNER JOIN LIBRIIVA AS l ON l.livcod=i.N4LIBR
                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=i.N4ASSF AND a.assali=i.n4assa
                        WHERE i.N4SOCI = @cid AND i.N4ANNO = @yea AND i.N4REGI = @numb 
                        ORDER BY i.N4RIGA",
                new { cid = CompanyID, yea = Year, numb = HeadNumber });

            return new ObservableCollection<PNIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNIVA? Get(string CompanyID, int Year, int Number, int Row)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNIVA>(
                "SELECT * FROM PN_IVA WHERE N4SOCI = @n4soci AND N4ANNO = @n4anno AND N4REGI = @n4regi AND N4RIGA = @n4riga",
                new { n4soci = CompanyID, n4anno = Year, n4regi = Number, n4riga = Row })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string N4SOCI, int N4ANNO, int N4REGI, int N4RIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PN_IVA WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4REGI = @N4REGI AND N4RIGA = @N4RIGA",
                new { N4SOCI = N4SOCI, N4ANNO = N4ANNO, N4REGI = N4REGI, N4RIGA = N4RIGA }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool CheckProtocolAlreadyUsed(string N4SOCI, int N4ANNO, int N4REGI, string ProtocolID, string IVABook)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM PN_IVA 
                        WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4DOCU = @prot AND N4LIBR = @ivab AND N4REGI <> @n4regi",
                new { N4SOCI = N4SOCI, N4ANNO = N4ANNO, prot = ProtocolID, ivab = IVABook, n4regi = N4REGI }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public Tuple<string, DateTime>? GetLastProtocolUsed(string N4SOCI, DateTime N4DARE, int N4REGI, string IVABook)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<PNIVA>(
                @"SELECT TOP(1) N4DOCU, N4DADO, N4DARE FROM PN_IVA 
                        WHERE N4SOCI = @N4SOCI AND N4LIBR = @ivab AND N4REGI <> @n4regi
                        ORDER BY N4DARE DESC, CAST(N4DOCU AS INT) DESC",
                new { N4SOCI = N4SOCI, ivab = IVABook, n4regi = N4REGI }).FirstOrDefault();

            if (result != null)
            {
                if (result.N4DARE.HasValue)
                {
                    if (N4DARE.Year <= result.N4DARE.Value.Year)
                        return (!string.IsNullOrEmpty(result.N4DOCU) && result.N4DADO.HasValue) ? new Tuple<string, DateTime>(result.N4DOCU, result.N4DADO.Value) : new Tuple<string, DateTime>("0", N4DARE);
                    else
                        return new Tuple<string, DateTime>("0", N4DARE);
                }
                else
                {
                    return new Tuple<string, DateTime>("0", N4DARE);
                }
            }
            else
            {
                return new Tuple<string, DateTime>("0", N4DARE);
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Reports
    public IVABookReport? PrintIVABook(string CompanyID, int AccountingYear, LIBRIIVA IVABook, DateTime PrintSince, DateTime PrintUntil, bool IsDefinitive)
    {
        try
        {
            var accounting = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, AccountingYear);
            int startingPage = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, PrintSince.Year, new GenericIDDescription() { ID = $"R{IVABook.livcod.Trim()}", Description = $"Numeratore pagine {IVABook.FullDescriptionSearchable}" }, false, 0);
            using var connection = GetOpenConnection();
            PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
            var rows = connection.Query<PNIVA>(
                @"SELECT p.*, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, p.N4SOTT AS CustomerID, c.abecod, c.abers1 AS CompanyDescription, c.abeind AS CompanyAddress, CONCAT(FORMAT(c.abecap,'00000'), ' ',TRIM(c.abeloc), ' (' , TRIM(c.abepro), ')') AS CompanyCity,c.abepiv AS VATID FROM PN_IVA AS p
                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=p.N4ASSF AND a.assali=p.N4assa
                        INNER JOIN ANAG_BASE AS c ON c.abecod = p.N4SOTT
                        where p.n4soci = @cid and p.N4LIBR=@liv and p.N4DARE >= @since and p.n4dare <= @until AND
                            ((p.N4DAST IS NULL AND p.N4FLGS IS NULL) OR (p.N4DAST = '1753-01-01' AND p.N4FLGS = ''))
                        ORDER BY CAST(p.N4DOCU AS INT)",
                new { cid = CompanyID, liv = IVABook.livcod, since = PrintSince.Date, until = PrintUntil });

            var result = new IVABookReport()
            {
                AccountingYear = AccountingYear,
                IVABook = IVABook,
                StartingPage = startingPage,
                PrintSince = PrintSince,
                PrintUntil = PrintUntil,
                TemporaryText = IsDefinitive ? "STAMPA DEFINITIVA" : "STAMPA PROVVISORIA",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                Rows = new List<PNIVA>()
            };

            // clear expire date if not IVA per cassa
            if (accounting != null && !accounting.eseivavenBool)
            {
                Parallel.ForEach(rows, row =>
                {
                    row.N4DTSCPG = null;
                });
            }

            decimal? totalDocument = null;
            foreach (var item in rows)
            {
                var existing = result.Rows.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT).FirstOrDefault();
                if (existing == null)
                {
                    result.Rows.Add(new PNIVA { N4SOCI = CompanyID, DocumentTotal = totalDocument });

                    var totPlus = rows.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT && w.N4SEGN == "+").Sum(sum => sum.N4IMEU + sum.N4IVEU);
                    var totMinus = rows.Where(w => w.N4DOCU == item.N4DOCU && w.N4SOTT == item.N4SOTT && w.N4SEGN == "-").Sum(sum => sum.N4IMEU + sum.N4IVEU);
                    totalDocument = totPlus - totMinus;
                    result.Rows.Add(item);
                }
                else
                {
                    result.Rows.Add(new PNIVA
                    {
                        N4SOCI = CompanyID,
                        N4ANNO = item.N4ANNO,
                        N4REGI = item.N4REGI,
                        N4RIGA = item.N4RIGA,
                        N4IMEU = (item.N4IMEU ?? 0),
                        N4IVEU = (item.N4IVEU ?? 0),
                        N4IIEU = item.N4IIEU.HasValue && item.N4IIEU.Value > 0 ? item.N4IIEU.Value : null,
                        N4SEGN = item.N4SEGN,
                        RateFullDescription = item.RateFullDescription,
                        CustomerID = item.N4SOTT ?? 0,
                        N4DTSCPG = item.N4DTSCPG
                    });
                }
            }
            result.Rows.Add(new PNIVA { N4SOCI = CompanyID, DocumentTotal = totalDocument, });

            result.VATs = GetVATRecap(result.Rows, null);
            return result;

        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public bool UpdatePrintedDefinitives(string CompanyID, string IVABookID, string NumeratorID, int NumeratorYear, List<PNIVA> Rows, int LastPagePrinted, string UserID, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                // update numerator
                connection.Execute("UPDATE TAB_NUMERATORI SET PERNUM=@lastpage WHERE PERSOC=@cid AND PERCOD=@numcod AND PERANN=@numann",
                    new { lastpage = LastPagePrinted, cid = CompanyID, numcod = NumeratorID, numann = NumeratorYear }, transaction);
                // update IVA book last print
                connection.Execute("UPDATE LIBRIIVA SET livulst=@tod, updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time', updatedUserID=@uid WHERE livcod=@id",
                    new { id = IVABookID, uid = UserID, tod = ToDate }, transaction);
                // update rows printed
                foreach (var row in Rows)
                {
                    connection.Execute("UPDATE PN_IVA SET N4DAST=@tod, N4FLGS='*' WHERE N4SOCI=@cid AND N4ANNO=@yea AND N4REGI=@regi AND N4RIGA=@riga",
                        new { cid = CompanyID, yea = row.N4ANNO, regi = row.N4REGI, riga = row.N4RIGA, tod = ToDate }, transaction);
                }
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public IVAReport? PrintIVARecap(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive, LIBRIIVA DefaultIVABookNumerator, DateTime Now)
    {
        try
        {
            int startingPage = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, PrintSince.Year, new GenericIDDescription() { ID = $"R{DefaultIVABookNumerator.livcod.Trim()}", Description = $"Numeratore pagine {DefaultIVABookNumerator.FullDescriptionSearchable}" }, false);
            using var connection = GetOpenConnection();


            PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
            string definitiveWhere = IsDefinitive ? " AND N4DAST IS NOT NULL AND N4FLGS IS NOT NULL" : "AND N4DAST IS NULL AND N4FLGS IS NULL";
            string query
                = IsCash ? $@"SELECT *, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CAST(CASE WHEN a.asssplpay = 'S' THEN 1 ELSE 0 END AS bit) AS IsSplitPayment FROM PN_IVA 
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=N4ASSF AND a.assali=N4assa
                                INNER JOIN LIBRIIVA AS l ON l.livcod=N4LIBR AND l.livili = 1
                                WHERE N4SOCI = @cid AND N4DTPGEF >= @since AND N4DTPGEF <= @until" :
                            $@"SELECT *, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CAST(CASE WHEN a.asssplpay = 'S' THEN 1 ELSE 0 END AS bit) AS IsSplitPayment FROM PN_IVA 
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=N4ASSF AND a.assali=N4assa                                    
                                INNER JOIN LIBRIIVA AS l ON l.livcod=N4LIBR AND l.livili = 1
                                WHERE N4SOCI = @cid AND N4DARE >= @since AND N4DARE <= @until {definitiveWhere}";

            var rows = connection.Query<PNIVA>($"{query}",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });
            var saldo = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetLastByPreviousMonth(CompanyID, PrintSince.Year, PrintSince.Month);

            var result = new IVAReport()
            {
                Year = Now.Year,
                StartingPage = startingPage,
                PrintSince = PrintSince,
                PrintUntil = PrintUntil,
                PreviousMonthAmount = saldo != null && (saldo.salsal ?? 0) != 0 && saldo.saldeb == 0 ? (saldo.salsal ?? 0) * -1 : 0,
                TemporaryText = IsDefinitive ? "RIEPILOGO LIQUIDAZIONE IVA" : "SIMULAZIONE LIQUIDAZIONE IVA",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                Rows = new List<PNIVA>(rows),
                SalesVATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                PurchasesVATs = GetVATRecap(new List<PNIVA>(rows), "A")
            };

            return result;

        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public IVAReport? PrintIVARecap(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive, DateTime Now)
    {
        try
        {
            int startingPage = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, PrintSince.Year, new GenericIDDescription() { ID = "RRI", Description = $"Numeratore pagine riepilogo IVA" }, false);
            using var connection = GetOpenConnection();


            PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
            string definitiveWhere = IsDefinitive ? "AND (N4DAST IS NOT NULL AND N4DAST <> '1753-01-01') AND (N4FLGS = '*')" : "AND (N4DAST IS NULL OR N4DAST = '1753-01-01') AND (N4FLGS IS NULL OR N4FLGS = '')";
            string query
                = IsCash ? $@"SELECT *, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CAST(CASE WHEN a.asssplpay = 'S' THEN 1 ELSE 0 END AS bit) AS IsSplitPayment FROM PN_IVA 
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=N4ASSF AND a.assali=N4assa
                                INNER JOIN LIBRIIVA AS l ON l.livcod=N4LIBR
                                WHERE N4SOCI = @cid AND N4DTPGEF >= @since AND N4DTPGEF <= @until" :
                            $@"SELECT *, l.livtip AS IVABookType,  CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CAST(CASE WHEN a.asssplpay = 'S' THEN 1 ELSE 0 END AS bit) AS IsSplitPayment FROM PN_IVA 
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=N4ASSF AND a.assali=N4assa 
                                INNER JOIN LIBRIIVA AS l ON l.livcod=N4LIBR
                                WHERE N4SOCI = @cid AND N4DARE >= @since AND N4DARE <= @until {definitiveWhere}";

            var rows = connection.Query<PNIVA>($"{query}",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });
            var saldo = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetLastByPreviousMonth(CompanyID, PrintSince.Year, PrintSince.Month);

            var result = new IVAReport()
            {
                Year = Now.Year,
                StartingPage = startingPage,
                PrintSince = PrintSince,
                PrintUntil = PrintUntil,
                PreviousMonthAmount = saldo != null && (saldo.salsal ?? 0) != 0 && saldo.saldeb == 0 ? (saldo.salsal ?? 0) * -1 : 0,
                TemporaryText = IsDefinitive ? "RIEPILOGO LIQUIDAZIONE IVA" : "SIMULAZIONE LIQUIDAZIONE IVA",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                Rows = new List<PNIVA>(rows),
                SalesVATs = GetVATRecap(new List<PNIVA>(rows), "V"),
                PurchasesVATs = GetVATRecap(new List<PNIVA>(rows), "A")
            };

            return result;

        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public IVAReportYearly? PrintIVARecapYearly(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = new IVAReportYearly()
            {
                PrintSince = new DateTime(Year, 1, 1),
                PrintUntil = new DateTime(Year, 12, 31, 23, 59, 59),
                TemporaryText = "RIEPILOGO LIQUIDAZIONE IVA ANNUALE",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                Rows = new List<PNIVA>()
            };

            var rows = connection.Query<PNIVA>($@"SELECT p.*, CONCAT(TRIM(l.livcod), ' ', TRIM(l.livdes)) IVABookFullDescription, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription,p.N4SOTT AS CustomerID, c.abecod, c.abers1 AS CompanyDescription, c.abeind AS CompanyAddress, CONCAT(FORMAT(c.abecap,'00000'), ' ',TRIM(c.abeloc), ' (' , TRIM(c.abepro), ')') AS CompanyCity,c.abepiv AS VATID FROM PN_IVA AS p
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=p.N4ASSF AND a.assali=p.N4assa                                    
                                INNER JOIN LIBRIIVA AS l ON l.livcod=p.N4LIBR
                                INNER JOIN ANAG_BASE AS c ON c.abecod = p.N4SOTT
                                WHERE p.N4SOCI = @cid AND p.N4FLGS='*' AND p.N4DAST >= @since AND p.N4DAST <= @until AND p.n4tmppn <> 'S'
                                ORDER BY IVABookType DESC, IVABookFullDescription",
                    new { cid = CompanyID, since = result.PrintSince, until = result.PrintUntil });

            #region Recap
            result.IVABookRecaps = new List<IVAReportYearly.DetailSection>();
            foreach (var row in rows)
            {
                result.Rows.Add(row);
                var existing = result.IVABookRecaps.Where(w => w.IVABookFullDescription == row.IVABookFullDescription).FirstOrDefault();
                if (existing != null)
                {
                    existing.Rows?.Add(row);
                }
                else
                {
                    result.IVABookRecaps.Add(new IVAReportYearly.DetailSection()
                    {
                        IVABookFullDescription = row.IVABookFullDescription,
                        IVABookType = row.IVABookType,
                        Rows = new List<PNIVA>() { row }
                    });
                }
            }

            foreach (var rec in result.IVABookRecaps)
            {
                rec.VATs = GetVATRecap(rec.Rows, null);
            }
            #endregion

            #region TABSALDI1
            result.PaymentsInfo = new IVAReportYearly.PaymentsRecap()
            {
                VATDebit = result.SalesTotal,
                VATCredit = result.PurchasesTotal,
                Payments = new List<IVAReportYearly.VATPayment>()
            };
            var lastPreviousYear = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetLastByMonth(CompanyID, Year - 1, 12);

            if (lastPreviousYear != null && lastPreviousYear.salsal.HasValue && lastPreviousYear.salsal.Value != 0)
                result.PaymentsInfo.InitialCredit = lastPreviousYear.salsal.Value;

            var saldi = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>().GetListByYear(CompanyID, Year);

            foreach (var item in (saldi ?? new ObservableCollection<TABSALDI1>()).OrderBy(o => o.salpag))
            {
                result.PaymentsInfo.Payments.Add(new IVAReportYearly.VATPayment()
                {
                    PaymentDate = item.salpag,
                    DebitAmount = (item.saldeb ?? 0),
                    CreditAmount = item.salsal ?? 0,
                    CompensationAmount = item.salcom ?? 0,
                });

                //result.PaymentsInfo.Payments.Add(new IVAReportYearly.VATPayment()
                //{
                //    PaymentDate = item.salpag,
                //    DebitAmount = (item.saldeb ?? 0) - previousCredit,
                //    CreditAmount = item.salsal ?? 0,
                //    CompensationAmount = item.salcom ?? 0,
                //});

                //previousCredit = (item.salsal ?? 0) < 0 ? (item.salsal ?? 0) * -1 : (item.salsal ?? 0);
            }
            #endregion

            #region Unpaid recap
            var unpaidRows = connection.Query<PNIVA>($@"SELECT p.*, CONCAT(TRIM(l.livcod), ' ', TRIM(l.livdes)) IVABookFullDescription, l.livtip AS IVABookType, CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription,p.N4SOTT AS CustomerID, c.abecod, c.abers1 AS CompanyDescription, c.abeind AS CompanyAddress, CONCAT(FORMAT(c.abecap,'00000'), ' ',TRIM(c.abeloc), ' (' , TRIM(c.abepro), ')') AS CompanyCity,c.abepiv AS VATID FROM PN_IVA AS p
                                INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=p.N4ASSF AND a.assali=p.N4assa                                    
                                INNER JOIN LIBRIIVA AS l ON l.livcod=p.N4LIBR
                                INNER JOIN ANAG_BASE AS c ON c.abecod = p.N4SOTT
                                WHERE p.N4SOCI = @cid AND p.N4DARE <= @until AND p.N4FLGS='*' AND p.N4FLIVCA = 'S' AND p.n4tmppn <> 'S' AND (p.N4DTPGEF IS NULL OR p.N4DTPGEF > @until)
                                ORDER BY p.N4DTSCPG",
                            new { cid = CompanyID, until = result.PrintUntil });
            result.UnpaidInfo = new IVAReportYearly.UnpaidRecap()
            {
                Rows = new List<PNIVA>(),
                IVABookRecaps = new List<IVAReportYearly.DetailSection>()
            };

            foreach (var row in unpaidRows)
            {
                result.UnpaidInfo.Rows.Add(row);
                var existing = result.UnpaidInfo.IVABookRecaps.Where(w => w.IVABookFullDescription == row.IVABookFullDescription).FirstOrDefault();
                if (existing != null)
                {
                    existing.Rows?.Add(row);
                }
                else
                {
                    result.UnpaidInfo.IVABookRecaps.Add(new IVAReportYearly.DetailSection()
                    {
                        IVABookFullDescription = row.IVABookFullDescription,
                        IVABookType = row.IVABookType,
                        Rows = new List<PNIVA>() { row }
                    });
                }
            }
            #endregion

            #region Unpaid expire recap
            result.UnpaidExpireInfo = new IVAReportYearly.UnpaidExpireRecap() { Expires = new List<IVAReportYearly.UnpaidExpireRecapItem>() };

            foreach (var row in unpaidRows)
            {
                var existing = result.UnpaidExpireInfo.Expires.Where(w => w.ExpireDate == row.N4DTSCPG).FirstOrDefault();
                if (existing == null)
                {
                    var newItem = new UnpaidExpireRecapItem()
                    {
                        ExpireDate = row.N4DTSCPG.HasValue ? row.N4DTSCPG.Value : DateTime.MinValue
                    };
                    if (row.IVABookType == "V")
                    {
                        if (row.N4SEGN == "+")
                        {
                            newItem.SalesAmount += row.N4IMEU ?? 0;
                            newItem.SalesVATAmount += row.N4IVEU ?? 0;
                            newItem.SalesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            newItem.SalesAmount -= row.N4IMEU ?? 0;
                            newItem.SalesVATAmount -= row.N4IVEU ?? 0;
                            newItem.SalesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                    else
                    {
                        if (row.N4SEGN == "+")
                        {
                            newItem.PurchasesAmount += row.N4IMEU ?? 0;
                            newItem.PurchasesVATAmount += row.N4IVEU ?? 0;
                            newItem.PurchasesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            newItem.PurchasesAmount -= row.N4IMEU ?? 0;
                            newItem.PurchasesVATAmount -= row.N4IVEU ?? 0;
                            newItem.PurchasesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                    result.UnpaidExpireInfo.Expires.Add(newItem);
                }
                else
                {
                    if (row.IVABookType == "V")
                    {
                        if (row.N4SEGN == "+")
                        {
                            existing.SalesAmount += row.N4IMEU ?? 0;
                            existing.SalesVATAmount += row.N4IVEU ?? 0;
                            existing.SalesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            existing.SalesAmount -= row.N4IMEU ?? 0;
                            existing.SalesVATAmount -= row.N4IVEU ?? 0;
                            existing.SalesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                    else
                    {
                        if (row.N4SEGN == "+")
                        {
                            existing.PurchasesAmount += row.N4IMEU ?? 0;
                            existing.PurchasesVATAmount += row.N4IVEU ?? 0;
                            existing.PurchasesNoVATAmount += row.N4IIEU ?? 0;
                        }
                        else
                        {
                            existing.PurchasesAmount -= row.N4IMEU ?? 0;
                            existing.PurchasesVATAmount -= row.N4IVEU ?? 0;
                            existing.PurchasesNoVATAmount -= row.N4IIEU ?? 0;
                        }
                    }
                }
            }
            #endregion

            return result;
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }

    public bool UpdateLiquidationDefinitive(string CompanyID, string NumeratorID, int NumeratorYear, int LastPagePrinted, string UserID, DateTime Until, decimal Total, decimal CompensationAmount)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var tabSaldiRepository = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDIRepository>();
                var tabSaldi1Repository = VulpesServiceProvider.Provider.GetRequiredService<ITABSALDI1Repository>();
                // update numerator
                connection.Execute("UPDATE TAB_NUMERATORI SET PERNUM=@lastpage WHERE PERSOC=@cid AND PERCOD=@numcod AND PERANN=@numann",
                    new { lastpage = LastPagePrinted, cid = CompanyID, numcod = NumeratorID, numann = NumeratorYear }, transaction);
                // update TABSALDI
                var tabsaldi = tabSaldiRepository.Get(CompanyID, Until.Year);
                if (tabsaldi == null)
                {
                    // create TABSALDI
                    connection.Execute(tabSaldiRepository.INSERT_QUERY, new TABSALDI() { salsoc = CompanyID, salann = Until.Year }, transaction);
                }
                var tabsaldi1 = tabSaldi1Repository.Get(CompanyID, Until.Year, Until.Month, Until);
                if (tabsaldi1 == null)
                {
                    // add TABSALDI1
                    connection.Execute(tabSaldi1Repository.INSERT_QUERY, new TABSALDI1()
                    {
                        salsoc = CompanyID,
                        salann = Until.Year,
                        salmes = Until.Month,
                        salpag = Until.Date,
                        saldeb = Total > 0 ? Total : 0,
                        salsal = Total < 0 ? Total : 0,
                        salcom = CompensationAmount,
                    },
                        transaction);
                }
                else
                {
                    tabsaldi1.saldeb = Total > 0 ? Total : 0;
                    tabsaldi1.salsal = Total < 0 ? Total : 0;
                    connection.Execute(tabSaldi1Repository.UPDATE_QUERY, tabsaldi1, transaction);
                }
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public IVAReportDetails? PrintIVARecapDetails(string CompanyID, DateTime PrintSince, DateTime PrintUntil, bool IsCash, bool IsDefinitive)
    {
        try
        {
            using var connection = GetOpenConnection();


            PrintUntil = new DateTime(PrintUntil.Year, PrintUntil.Month, PrintUntil.Day, 23, 59, 59);
            var result = new IVAReportDetails()
            {
                PrintSince = PrintSince,
                PrintUntil = PrintUntil,
                TemporaryText = IsDefinitive ? "RIEPILOGO LIQUIDAZIONE IVA" : "SIMULAZIONE LIQUIDAZIONE IVA",
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
            };

            #region Sales previous period unpaid
            var rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF IS NULL AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.SalesPrevious = new IVAReportDetails.DetailSection()
            {
                Text = $"VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                RecapText = $"RIEPILOGO VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                IVABookType = "V",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
            };

            #endregion

            #region Sales current period unpaid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND (pi.N4DTPGEF IS NULL OR pi.N4DTPGEF > @until) AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.SalesCurrent = new IVAReportDetails.DetailSection()
            {
                Text = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                RecapText = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                IVABookType = "V",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
            };

            #endregion

            #region Sales previous period paid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.SalesPreviousPaid = new IVAReportDetails.DetailSection()
            {
                Text = $"VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                RecapText = $"RIEPILOGO VENDITE - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                IVABookType = "V",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
            };

            #endregion

            #region Sales current period paid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'V'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.SalesCurrentPaid = new IVAReportDetails.DetailSection()
            {
                Text = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                RecapText = $"VENDITE - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                IVABookType = "V",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "V"),
            };

            #endregion

            #region Purchases previous period unpaid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF IS NULL AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.PurchasesPrevious = new IVAReportDetails.DetailSection()
            {
                Text = $"ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                RecapText = $"RIEPILOGO ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - NON PAGATE",
                IVABookType = "A",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
            };

            #endregion

            #region Purchases current period unpaid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND (pi.N4DTPGEF IS NULL OR pi.N4DTPGEF > @until) AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.PurchasesCurrent = new IVAReportDetails.DetailSection()
            {
                Text = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                RecapText = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - NON PAGATE",
                IVABookType = "A",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
            };

            #endregion

            #region Purchases previous period paid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE < @since AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.PurchasesPreviousPaid = new IVAReportDetails.DetailSection()
            {
                Text = $"ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                RecapText = $"RIEPILOGO ACQUISTI - Periodo precedente il {PrintSince.ToString("dd/MM/yyyy")} - PAGATE",
                IVABookType = "A",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
            };

            #endregion

            #region Purchases current period paid
            rows = connection.Query<PNIVA>($@"SELECT row_number() OVER(ORDER BY pi.N4DADO, pi.N4DOCU) AS RowNumber,pi.*,l.livtip AS IVABookType,CONCAT(TRIM(a.asscod), ' ' , TRIM(a.assali), ' ', TRIM(a.assdes)) AS RateFullDescription, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS EntityFullDescription FROM PNIVA AS pi
                                                        INNER JOIN ASSOGGETAMENTI AS a ON a.asscod=pi.N4ASSF AND a.assali=pi.N4assa
                                                        INNER JOIN ABE AS c ON c.abecod = pi.N4SOTT   
                                                        INNER JOIN LIBRIIVA AS l ON l.livcod=pi.N4LIBR AND l.livili = 1
                                                        WHERE pi.N4SOCI=@cid AND pi.N4DARE >= @since AND pi.N4DARE <= @until AND pi.N4DTPGEF >= @since AND pi.N4DTPGEF <= @until AND l.livtip = 'A'
                                                        ORDER BY pi.N4DADO, pi.N4DOCU",
                    new { cid = CompanyID, since = PrintSince, until = PrintUntil });

            result.PurchasesCurrentPaid = new IVAReportDetails.DetailSection()
            {
                Text = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                RecapText = $"ACQUISTI - Periodo dal {PrintSince.ToString("dd/MM/yyyy")} al {PrintUntil.ToString("dd/MM/yyyy")} - PAGATE",
                IVABookType = "A",
                Rows = new List<PNIVA>(rows),
                VATs = GetVATRecap(new List<PNIVA>(rows), "A"),
            };

            #endregion

            return result;

        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PN_IVA (N4SOCI,N4ANNO,N4REGI,N4RIGA,N4DOCU,N4RIFE,N4DARE,N4DADO,N4DARI,N4CAUS,N4SOTT,N4TCLF,N4IMPO,N4SEGN,N4LIBR,N4ASSF,n4assa,N4IIVA,N4INDP,N4INDV,N4DAST,N4FLGS,N4IMEU,N4IVEU,N4IIEU,N4TIDO,n4donu,n4tmppn,N4FLSPESO,N4LIBALT,N4FLIVCA,N4IMPPAG,N4DTSCAD,N4DTPGEF,N4DTSCPG) OUTPUT INSERTED.rv VALUES(@N4SOCI,@N4ANNO,@N4REGI,@N4RIGA,@N4DOCU,@N4RIFE,@N4DARE,@N4DADO,@N4DARI,@N4CAUS,@N4SOTT,@N4TCLF,@N4IMPO,@N4SEGN,@N4LIBR,@N4ASSF,@n4assa,@N4IIVA,@N4INDP,@N4INDV, @N4DAST, @N4FLGS,@N4IMEU,@N4IVEU,@N4IIEU,@N4TIDO,@n4donu,@n4tmppn,@N4FLSPESO,@N4LIBALT,@N4FLIVCA,@N4IMPPAG,@N4DTSCAD,@N4DTPGEF,@N4DTSCPG)";
    public string UPDATE_QUERY => "UPDATE PN_IVA SET N4SOCI = @N4SOCI,N4ANNO = @N4ANNO,N4REGI = @N4REGI,N4RIGA = @N4RIGA,N4DOCU = @N4DOCU,N4RIFE = @N4RIFE,N4DARE = @N4DARE,N4DADO = @N4DADO,N4DARI = @N4DARI,N4CAUS = @N4CAUS,N4SOTT = @N4SOTT,N4TCLF = @N4TCLF,N4IMPO = @N4IMPO,N4SEGN = @N4SEGN,N4LIBR = @N4LIBR,N4ASSF = @N4ASSF,n4assa = @n4assa,N4IIVA = @N4IIVA,N4INDP = @N4INDP,N4INDV = @N4INDV,N4DAST = @N4DAST,N4FLGS = @N4FLGS,N4IMEU = @N4IMEU,N4IVEU = @N4IVEU,N4IIEU = @N4IIEU,N4TIDO = @N4TIDO,n4donu = @n4donu,n4tmppn = @n4tmppn,N4FLSPESO = @N4FLSPESO,N4LIBALT = @N4LIBALT,N4FLIVCA = @N4FLIVCA,N4IMPPAG = @N4IMPPAG,N4DTSCAD = @N4DTSCAD,N4DTPGEF = @N4DTPGEF,N4DTSCPG = @N4DTSCPG OUTPUT INSERTED.rv WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4REGI = @N4REGI AND N4RIGA = @N4RIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PN_IVA OUTPUT DELETED.rv WHERE N4SOCI = @N4SOCI AND N4ANNO = @N4ANNO AND N4REGI = @N4REGI AND N4RIGA = @N4RIGA AND rv = @rv";

    public bool Insert(PNIVA Model)
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

    public bool Update(PNIVA Model)
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

    public bool DeleteSingle(PNIVA Model)
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
                "DELETE FROM PNIVA OUTPUT DELETED.rv WHERE N4SOCI = @n4soci AND N4ANNO = @n4anno AND N4REGI = @n4regi",
                new { n4soci = CompanyID, n4anno = Year, n4regi = Number });
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

    public string? Validate(PNIVA Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.N4SEGN))
            {
                if (!string.IsNullOrWhiteSpace(Model.N4ASSF) && !string.IsNullOrWhiteSpace(Model.n4assa))
                {
                    if (!string.IsNullOrWhiteSpace(Model.N4LIBR))
                    {
                        return null;
                    }
                    else
                    {
                        return "Il libro IVA è obbligatorio";
                    }
                }
                else
                {
                    return "L'aliquota è obbligatoria";
                }
            }
            else
            {
                return "Il segno è obbligatorio";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

    #region Utilities
    public List<VATRecap>? GetVATRecap(List<PNIVA>? Rows, string? IVABookType)
    {
        if (Rows == null)
            return null;

        var result = new List<VATRecap>();
        List<PNIVA> rowsData;
        if (string.IsNullOrWhiteSpace(IVABookType))
            rowsData = Rows.OrderBy(o => o.RateFullDescription).ToList();
        else
            rowsData = Rows.Where(w => (w.IVABookType ?? string.Empty).StartsWith(IVABookType)).OrderBy(o => o.RateFullDescription).ToList();
        foreach (var item in rowsData.Where(w => !string.IsNullOrWhiteSpace(w.RateFullDescription)))
        {
            var existing = result.Where(w => w.RateFullDescription == item.RateFullDescription).FirstOrDefault();
            if (existing == null)
            {
                var newRecap = new VATRecap()
                {
                    RateFullDescription = item.RateFullDescription ?? string.Empty,
                    IsSplitPayment = item.IsSplitPayment
                };
                if (item.N4SEGN == "+")
                {
                    newRecap.TotalAmount += (item.N4IMEU ?? 0);
                    newRecap.TotalVATAmount += (item.N4IVEU ?? 0);
                    newRecap.TotalNoVATAmount += (item.N4IIEU ?? 0);
                }
                else
                {
                    newRecap.TotalAmount -= (item.N4IMEU ?? 0);
                    newRecap.TotalVATAmount -= (item.N4IVEU ?? 0);
                    newRecap.TotalNoVATAmount -= (item.N4IIEU ?? 0);
                }
                result.Add(newRecap);
            }
            else
            {
                if (item.N4SEGN == "+")
                {
                    existing.TotalAmount += (item.N4IMEU ?? 0);
                    existing.TotalVATAmount += (item.N4IVEU ?? 0);
                    existing.TotalNoVATAmount += (item.N4IIEU ?? 0);
                }
                else
                {
                    existing.TotalAmount -= (item.N4IMEU ?? 0);
                    existing.TotalVATAmount -= (item.N4IVEU ?? 0);
                    existing.TotalNoVATAmount -= (item.N4IIEU ?? 0);
                }
            }
        }

        return result;
    }
    #endregion
}