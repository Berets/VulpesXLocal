

using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Accounting;

public interface IPNCLIENTIRepository
{
    ObservableCollection<PNCLIENTI>? GetList();

    ObservableCollection<PNCLIENTI>? GetListByReg(string CompanyID, int Year, int Number);

    ObservableCollection<PNCLIENTI>? GetList(string CompanyID, int Year, int Number, int OriginalRowID, int CustomerID);

    ObservableCollection<MastrinoECReportItem>? GetECList(string CompanyID, int CustomerID, DateTime ToDate, bool HasDrawn, DateTime SinceDrawn);

    ObservableCollection<PNCLIENTI>? GetChecks(string CompanyID, DateTime From);

    decimal GetExpired(string CompanyID, int CustomerID);

    bool SyncPartita(string CompanyID, int SourceYear, int SourceNumber, int SourceRow, int TargetYear, int TargetNumber, int TargetRow);

    PNCLIENTI? Get(string CompanyID, int Year, int Number, int Row);

    List<DateTime>? ComputeExpires(DateTime DocumentDate, string PaymentTypeID);

    #region Customer analysis
    Tuple<decimal, string> GetCABalance(string CompanyID, int CustomerID, DateTime ToDate);

    Tuple<decimal, string> GetCABalanceExpiring(string CompanyID, int CustomerID, DateTime SinceDate);

    Tuple<decimal, string> GetCAExpiredBalance(string CompanyID, int CustomerID, DateTime ToDate, DateTime SinceDate);

    decimal GetCATotalCirculating(string CompanyID, int CustomerID, DateTime ToDate);

    decimal GetCACirculating(string CompanyID, int CustomerID, DateTime FromDate, DateTime ToDate);
    #endregion

    #region Reports
    ExpiresReport? ExpiresReport(string CompanyID, ABE Entity, GenericIDDescription PaymentType, DateTime? ExpireDate, string GroupType);

    ObservableCollection<BatchReportModel.EntityModel>? GetBatch(string CompanyID, DateTime From, DateTime To);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(PNCLIENTI Model);

    bool Update(PNCLIENTI Model);

    bool DeleteSingle(PNCLIENTI Model);

    bool DeleteByReg(string CompanyID, int Year, int Number);

    string? Validate(PNCLIENTI Model, bool IsInsert);
    #endregion
}

public class PNCLIENTIRepository : RepositoryBase, IPNCLIENTIRepository
{
    public PNCLIENTIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNCLIENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                "SELECT * FROM PNCLIENTI");

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNCLIENTI>? GetListByReg(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                @"SELECT p.N2SCAD, p.N2RIFE, p.N2DARI, p.N2IMEU, p.N2SEGN, CONCAT(a.abecod , ' ' , TRIM(a.abers1)) AS CustomerDescription FROM PNCLIENTI AS p
                        INNER JOIN ABE AS a ON a.abecod = p.N2SOTT
                        WHERE p.N2SOCI = @cid AND p.N2ANNO = @yea AND p.N2REGI = @reg",
                new { cid = CompanyID, yea = Year, reg = Number });

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNCLIENTI>? GetList(string CompanyID, int Year, int Number, int OriginalRowID, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                @"SELECT * FROM PNCLIENTI 
                        WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi AND N2SOTT = @n2sott AND n2rior = @n2rior
                        ORDER BY N2RIGA",
                new { n2soci = CompanyID, n2anno = Year, n2regi = Number, n2sott = CustomerID, n2rior = OriginalRowID });

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<MastrinoECReportItem>? GetECList(string CompanyID, int CustomerID, DateTime ToDate, bool HasDrawn, DateTime SinceDrawn)
    {
        try
        {
            using var connection = GetOpenConnection();
            ObservableCollection<MastrinoECReportItem> result = new ObservableCollection<MastrinoECReportItem>();

            string? query = null;
            if (!HasDrawn)
                query = @"SELECT m.*, c.caucod, c.caudes FROM PNCLIENTI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.n2caus
                        WHERE m.N2SOCI = @n2soci AND m.N2SOTT = @n2sott AND m.N2DARE <= @n2dare AND m.N2PARE IS NULL
                        ORDER BY m.N2DARI, m.N2RIFE";
            else
                query = @"SELECT m.*, c.caucod, c.caudes FROM PNCLIENTI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.n2caus
                        WHERE m.N2SOCI = @n2soci AND m.N2SOTT = @n2sott AND m.N2DARE <= @n2dare AND 
                        (m.N2PARE IS NULL OR (m.N2PARE = '*' AND m.N2DARI >= @dapare))
                        ORDER BY m.N2DARI, m.N2RIFE";

            var list = connection.Query<PNCLIENTI, CAUCONT, PNCLIENTI>(
                query,
                (pn, cau) => { pn.Causal = cau; return pn; },
                new { n2soci = CompanyID, n2sott = CustomerID, n2dare = ToDate, dapare = SinceDrawn },
                splitOn: "caucod");

            foreach (var item in list)
            {
                var solls = (!string.IsNullOrEmpty(item.N2RIFE) && item.N2SOTT.HasValue) ? VulpesServiceProvider.Provider.GetRequiredService<ISOLLE0FRepository>().GetList(item.N2SOCI, item.N2SOTT.Value, item.N2RIFE) : null;
                foreach (var soll in solls ?? new ObservableCollection<SOLLE0F>())
                {
                    soll.DocumentID = item.N2DOCU;
                    soll.DocumentDate = item.N2DADO;
                }
                result.Add(new MastrinoECReportItem()
                {
                    CompanyID = item.N2SOCI,
                    Year = item.N2ANNO,
                    RowID = item.N2RIGA,
                    DocumentDate = item.N2DADO,
                    DocumentID = item.N2DOCU,
                    RegistrationDate = item.N2DARE,
                    ReferenceDate = item.N2DARI,
                    ReferenceID = item.N2RIFE,
                    Number = item.N2REGI,
                    ExpireDate = item.N2SCAD,
                    CurrencyID = item.N2DIVI,
                    Dare = item.N2SEGN == "D" ? (item.N2IMEU ?? 0) : 0,
                    Avere = item.N2SEGN == "A" ? (item.N2IMEU ?? 0) : 0,
                    CausalFullDescription = item.Causal?.FullDescriptionSearchable,
                    EntityType = "C",
                    EntityID = CustomerID,
                    Reminders = solls,
                    PaymentID = item.N2PAGA,
                    PaymentTypeID = item.n2tipi,
                    Scadenza2 = item.N2INIZ,
                    PNCLIENTI = item,
                    SaldoRiga = item.N2SEGN == "D" ? result.Sum(s => s.Saldo) + (item.N2IMEU ?? 0) : result.Sum(s => s.Saldo) - (item.N2IMEU ?? 0)
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

    public ObservableCollection<PNCLIENTI>? GetChecks(string CompanyID, DateTime From)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                @"SELECT p.N2SCAD, p.N2RIFE, p.N2DARI, p.N2IMEU, p.N2SEGN, p.N2INIZ, p.N2DOCU, p.N2DADO, CONCAT(a.abecod , ' ' , TRIM(a.abers1)) AS CustomerDescription FROM PNCLIENTI AS p
                        INNER JOIN ABE AS a ON a.abecod = p.N2SOTT
                        WHERE p.N2SOCI = @cid AND p.N2INIZ >= @from",
                new { cid = CompanyID, from = From });

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }


    public decimal GetExpired(string CompanyID, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();

            string query = @"SELECT m.*, c.caucod, c.caudes FROM PNCLIENTI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.n2caus
                        WHERE m.N2SOCI = @n2soci AND m.N2SOTT = @n2sott AND m.N2SCAD <= @n2Scad AND m.N2PARE IS NULL
                        ORDER BY m.N2DARI, m.N2RIFE";

            var list = connection.Query<PNCLIENTI, CAUCONT, PNCLIENTI>(
                query,
                (pn, cau) => { pn.Causal = cau; return pn; },
                new { n2soci = CompanyID, n2sott = CustomerID, n2Scad = DateTime.Now },
                splitOn: "caucod");

            if (list.Any())
                return (list.Where(o => o.N2SEGN == "D").Sum(s => s.N2IMEU) ?? 0) - (list.Where(o => o.N2SEGN == "A").Sum(s => s.N2IMEU) ?? 0);
            else
                return 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public bool SyncPartita(string CompanyID, int SourceYear, int SourceNumber, int SourceRow, int TargetYear, int TargetNumber, int TargetRow)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                var multi = connection.QueryMultiple(
                @"SELECT * FROM PNCLIENTI WHERE N2SOCI=@cid AND N2ANNO = @sy AND N2REGI = @sn AND N2RIGA = @sr;
                                SELECT * FROM PNCLIENTI WHERE N2SOCI=@cid AND N2ANNO = @ty AND N2REGI = @tn AND N2RIGA = @tr;",
                new { cid = CompanyID, sy = SourceYear, sn = SourceNumber, sr = SourceRow, ty = TargetYear, tn = TargetNumber, tr = TargetRow }, transaction);
                var source = multi.Read<PNCLIENTI>().SingleOrDefault();
                var target = multi.Read<PNCLIENTI>().SingleOrDefault();
                if (source != null && target != null)
                {
                    source.N2RIFE = target.N2RIFE;
                    source.N2DARI = target.N2DARI;
                    source.N2SCAD = target.N2SCAD;
                    var updated = connection.Execute(UPDATE_QUERY, source, transaction);
                    if (updated > 0)
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
                    ErrorHandler.Show("Righe di registrazione non trovate");
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

    public PNCLIENTI? Get(string CompanyID, int Year, int Number, int Row)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PNCLIENTI>(
                "SELECT * FROM PNCLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                new { n2soci = CompanyID, n2anno = Year, n2regi = Number, n2riga = Row })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<DateTime>? ComputeExpires(DateTime DocumentDate, string PaymentTypeID)
    {
        try
        {
            var payment = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(PaymentTypeID);

            List<DateTime> result = new List<DateTime> { DocumentDate };

            if (payment != null)
            {
                if (payment.pclgs1.HasValue && payment.pclgs1.Value > 0)
                {
                    var newExpire1 = result.First().AddDays(payment.pclgs1.Value);
                    if (payment.pclppa == "0")
                        result[0] = newExpire1;
                    else
                        result[0] = new DateTime(newExpire1.Year, newExpire1.Month, DateTime.DaysInMonth(newExpire1.Year, newExpire1.Month));

                    if (payment.pclgs2.HasValue && payment.pclgs2.Value > 0)
                    {
                        var newExpire2 = DocumentDate.AddDays(payment.pclgs2.Value);
                        if (payment.pclppa == "0")
                            result.Add(newExpire2);
                        else
                            result.Add(new DateTime(newExpire2.Year, newExpire2.Month, DateTime.DaysInMonth(newExpire2.Year, newExpire2.Month)));

                        if (payment.pclgs3.HasValue && payment.pclgs3.Value > 0)
                        {
                            var newExpire3 = DocumentDate.AddDays(payment.pclgs3.Value);
                            if (payment.pclppa == "0")
                                result.Add(newExpire3);
                            else
                                result.Add(new DateTime(newExpire3.Year, newExpire3.Month, DateTime.DaysInMonth(newExpire3.Year, newExpire3.Month)));

                            if (payment.pclgs4.HasValue && payment.pclgs4.Value > 0)
                            {
                                var newExpire4 = DocumentDate.AddDays(payment.pclgs4.Value);
                                if (payment.pclppa == "0")
                                    result.Add(newExpire4);
                                else
                                    result.Add(new DateTime(newExpire4.Year, newExpire4.Month, DateTime.DaysInMonth(newExpire4.Year, newExpire4.Month)));

                                if (payment.pclgs5.HasValue && payment.pclgs5.Value > 0)
                                {
                                    var newExpire5 = DocumentDate.AddDays(payment.pclgs5.Value);
                                    if (payment.pclppa == "0")
                                        result.Add(newExpire5);
                                    else
                                        result.Add(new DateTime(newExpire5.Year, newExpire5.Month, DateTime.DaysInMonth(newExpire5.Year, newExpire5.Month)));

                                    if (payment.pclgs6.HasValue && payment.pclgs6.Value > 0)
                                    {
                                        var newExpire6 = DocumentDate.AddDays(payment.pclgs6.Value);
                                        if (payment.pclppa == "0")
                                            result.Add(newExpire6);
                                        else
                                            result.Add(new DateTime(newExpire6.Year, newExpire6.Month, DateTime.DaysInMonth(newExpire6.Year, newExpire6.Month)));

                                        if (payment.pclgs7.HasValue && payment.pclgs7.Value > 0)
                                        {
                                            var newExpire7 = DocumentDate.AddDays(payment.pclgs7.Value);
                                            if (payment.pclppa == "0")
                                                result.Add(newExpire7);
                                            else
                                                result.Add(new DateTime(newExpire7.Year, newExpire7.Month, DateTime.DaysInMonth(newExpire7.Year, newExpire7.Month)));

                                            if (payment.pclgs8.HasValue && payment.pclgs8.Value > 0)
                                            {
                                                var newExpire8 = DocumentDate.AddDays(payment.pclgs8.Value);
                                                if (payment.pclppa == "0")
                                                    result.Add(newExpire8);
                                                else
                                                    result.Add(new DateTime(newExpire8.Year, newExpire8.Month, DateTime.DaysInMonth(newExpire8.Year, newExpire8.Month)));

                                                if (payment.pclgs9.HasValue && payment.pclgs9.Value > 0)
                                                {
                                                    var newExpire9 = DocumentDate.AddDays(payment.pclgs9.Value);
                                                    if (payment.pclppa == "0")
                                                        result.Add(newExpire9);
                                                    else
                                                        result.Add(new DateTime(newExpire9.Year, newExpire9.Month, DateTime.DaysInMonth(newExpire9.Year, newExpire9.Month)));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    if (payment.pclppa != "0")
                        result[0] = new DateTime(DocumentDate.Year, DocumentDate.Month, DateTime.DaysInMonth(DocumentDate.Year, DocumentDate.Month));
                }
            }

            return result;
        }
        catch { return null; }
    }

    #region Customer analysis
    public Tuple<decimal, string> GetCABalance(string CompanyID, int CustomerID, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multi = connection.QueryMultiple(
                @"SELECT SUM(N2IMEU) FROM PNCLIENTI WHERE N2SOCI = @cid AND N2SEGN='D' AND N2SOTT = @custid AND N2SCAD <= @todate;
                      SELECT SUM(N2IMEU) FROM PNCLIENTI WHERE N2SOCI = @cid AND N2SEGN='A' AND N2SOTT = @custid AND N2SCAD <= @todate;",
                new { cid = CompanyID, custid = CustomerID, todate = ToDate });
            var dare = multi.Read<decimal?>().Single() ?? 0m;
            var avere = multi.Read<decimal?>().Single() ?? 0m;
            decimal balance = 0;
            string balanceSign = "-";
            if (dare > avere)
            {
                balance = dare - avere;
                balanceSign = "D";
            }
            else if (dare < avere)
            {
                balance = avere - dare;
                balanceSign = "A";
            }

            return new Tuple<decimal, string>(balance, balanceSign);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, string>(-1, "ERR");
        }
    }

    public Tuple<decimal, string> GetCABalanceExpiring(string CompanyID, int CustomerID, DateTime SinceDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multi = connection.QueryMultiple(
                @"SELECT SUM(N2IMEU) FROM PNCLIENTI WHERE N2SOCI = @cid AND N2SEGN='D' AND N2SOTT = @custid AND N2SCAD > @sincedate;
                      SELECT SUM(N2IMEU) FROM PNCLIENTI WHERE N2SOCI = @cid AND N2SEGN='A' AND N2SOTT = @custid AND N2SCAD > @sincedate;",
                new { cid = CompanyID, custid = CustomerID, sincedate = SinceDate });
            var dare = multi.Read<decimal?>().Single() ?? 0m;
            var avere = multi.Read<decimal?>().Single() ?? 0m;
            decimal balance = 0;
            string balanceSign = "-";
            if (dare > avere)
            {
                balance = dare - avere;
                balanceSign = "D";
            }
            else if (dare < avere)
            {
                balance = avere - dare;
                balanceSign = "A";
            }

            return new Tuple<decimal, string>(balance, balanceSign);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, string>(-1, "ERR");
        }
    }

    public Tuple<decimal, string> GetCAExpiredBalance(string CompanyID, int CustomerID, DateTime ToDate, DateTime SinceDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multi = connection.QueryMultiple(
                @"SELECT SUM(N2IMEU) FROM PNCLIENTI WHERE N2SOCI = @cid AND N2SEGN='D' AND N2SOTT = @custid AND N2DARI <= @todate AND N2SCAD >= @sincedate AND N2SCAD <= @todate;
                      SELECT SUM(N2IMEU) FROM PNCLIENTI WHERE N2SOCI = @cid AND N2SEGN='A' AND N2SOTT = @custid AND N2DARI <= @todate AND N2SCAD >= @sincedate AND N2SCAD <= @todate;",
                new { cid = CompanyID, custid = CustomerID, todate = ToDate, sincedate = SinceDate });
            var dare = multi.Read<decimal?>().Single() ?? 0m;
            var avere = multi.Read<decimal?>().Single() ?? 0m;
            decimal balance = 0;
            string balanceSign = "-";
            if (dare > avere)
            {
                balance = dare - avere;
                balanceSign = "D";
            }
            else if (dare < avere)
            {
                balance = avere - dare;
                balanceSign = "A";
            }

            return new Tuple<decimal, string>(balance, balanceSign);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, string>(-1, "ERR");
        }
    }

    public decimal GetCATotalCirculating(string CompanyID, int CustomerID, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (decimal)(connection.ExecuteScalar(
                @"SELECT SUM(l.N2IMEU) FROM PNCLIENTI AS l
                        INNER JOIN CAUCONT AS r ON l.N2CAUS = r.caucod
                        WHERE l.N2SOCI = @cid AND r.caucir = 'S' AND l.N2SOTT = @custid AND l.N2DARE <= @todate AND l.N2SCAD > @todate",
                new { cid = CompanyID, custid = CustomerID, todate = ToDate }) ?? 0m);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public decimal GetCACirculating(string CompanyID, int CustomerID, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (decimal)(connection.ExecuteScalar(
                @"SELECT SUM(l.N2IMEU) FROM PNCLIENTI AS l
                        INNER JOIN CAUCONT AS r ON l.N2CAUS = r.caucod
                        WHERE l.N2SOCI = @cid AND r.caucir = 'S' AND l.N2SOTT = @custid AND l.N2DARE <= @todate AND (l.N2SCAD >= @fromdate AND l.N2SCAD <= @todate);",
                new { cid = CompanyID, custid = CustomerID, fromdate = FromDate, todate = ToDate }) ?? 0);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }
    #endregion

    #region Reports
    public ExpiresReport? ExpiresReport(string CompanyID, ABE Entity, GenericIDDescription PaymentType, DateTime? ExpireDate, string GroupType)
    {
        try
        {
            using var connection = GetOpenConnection();
            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(CompanyID);
            var result = new ExpiresReport()
            {
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase!.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png")
            };
      
                string query = $@"SELECT abe.abecod AS EntityID, 'F' AS EntityTypeID, pn.N2SEGN AS Sign, TRIM(abe.abers1) AS EntityDescription, pn.N2SCAD AS ExpireDate, TRIM(pn.N2RIFE) AS ReferenceID,pn.N2DARI AS ReferenceDate,tip.icscod AS PaymentTypeID,TRIM(tip.icsdes) AS PaymentTypeDescription, pn.N2IMEU AS Amount, TRIM(cau.caudes) AS CausalDescription FROM PNCLIENTI AS pn
                                        INNER JOIN PNTESTATA AS hea ON hea.N1SOCI=pn.N2SOCI AND hea.N1ANNO=pn.N2ANNO AND hea.N1REGI=pn.N2REGI
                                        INNER JOIN ABE AS abe ON abe.abecod = pn.N2SOTT
                                        INNER JOIN CAUCONT AS cau ON cau.caucod = hea.pncaus
                                        LEFT JOIN PAGCLI AS pag ON pag.pclcod = pn.N2PAGA
                                        LEFT JOIN TAB_ACC_TIPINC AS tip ON tip.icscod = pag.pcltip
                                        WHERE pn.N2SOCI=@N2soci AND pn.N2PARE IS NULL AND hea.N1TmpPN = 'N'
                                            {(Entity != null ? " AND pn.n2sott=@n2sott" : null)}
                                            {(!string.IsNullOrWhiteSpace(PaymentType.ID) ? " AND pag.pcltip=@pcltip" : null)}
                                            {(ExpireDate.HasValue ? " AND pn.N2SCAD<=@n2scad" : null)}
                                        order by pn.n2sott,pag.pcltip,pn.N2SCAD";

               
                var list = connection.Query<ExpiresReportItem>(
                    query,
                    new { n2soci = CompanyID, n2sott = Entity?.abecod, n2scad = ExpireDate, pcltip = PaymentType?.ID });

                result.Rows = new List<ExpiresReportMasterItem>();

                if (GroupType == "C")
                {
                    // customer
                    foreach (var row in list.GroupBy(g => g.EntityID, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = dare - avere;
                        var newMaster = new ExpiresReportMasterItem()
                        {
                            EntityID = row.key,
                            EntityDescription = row.items.First().EntityDescription,
                            EntityTotal = diff < 0 ? diff * -1 : diff,
                            PrintSign = diff < 0 ? "-" : "",
                            Rows = new List<ExpiresReportItem>()
                        };
                        foreach (var det in row.items.GroupBy(g => new { g.PaymentTypeID, g.ReferenceID, g.ReferenceDate, g.ExpireDate }, (detkey, details) => new { detkey, details }))
                        {
                            var dareI = det.details.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                            var avereI = det.details.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                            var diffI = dareI - avereI;
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                PaymentTypeID = det.details.First().PaymentTypeID,
                                PaymentTypeDescription = det.details.First().PaymentTypeDescription,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : ""
                            });
                        }
                        // reorder details
                        newMaster.Rows = newMaster.Rows.OrderBy(o => o.ExpireDate).ThenBy(tb => tb.PaymentTypeID).ToList();
                        result.Rows.Add(newMaster);
                        result.GrandTotal += diff;
                    }
                }
                else if (GroupType == "S")
                {
                    // expire date
                    foreach (var row in list.GroupBy(g => g.ExpireDate, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = dare - avere;
                        var newMaster = new ExpiresReportMasterItem()
                        {
                            ExpireDate = row.key,
                            EntityID = row.key.ToString("dd/MM/yyyy"),
                            EntityDescription = null,
                            EntityTotal = diff < 0 ? diff * -1 : diff,
                            PrintSign = diff < 0 ? "-" : "",
                            Rows = new List<ExpiresReportItem>()
                        };
                        foreach (var det in row.items.GroupBy(g => new { g.EntityID, g.PaymentTypeID, g.ReferenceID, g.ReferenceDate }, (detkey, details) => new { detkey, details }))
                        {
                            var dareI = det.details.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                            var avereI = det.details.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                            var diffI = dareI - avereI;
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                EntityID = det.details.First().EntityID,
                                EntityDescription = det.details.First().EntityDescription,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                PaymentTypeID = det.details.First().PaymentTypeID,
                                PaymentTypeDescription = det.details.First().PaymentTypeDescription,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : ""
                            });
                        }
                        // reorder details
                        newMaster.Rows = newMaster.Rows.OrderBy(o => o.EntityID).ThenBy(tb => tb.PaymentTypeID).ToList();
                        result.Rows.Add(newMaster);
                        // reorder master
                        result.Rows = result.Rows.OrderBy(o => o.ExpireDate).ToList();
                        result.GrandTotal += diff;
                    }
                }
                else
                {
                    // payment type
                    foreach (var row in list.GroupBy(g => g.PaymentTypeID, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = dare - avere;
                        var newMaster = new ExpiresReportMasterItem()
                        {
                            EntityID = row.key,
                            EntityDescription = row.items.First().PaymentTypeDescription,
                            EntityTotal = diff < 0 ? diff * -1 : diff,
                            PrintSign = diff < 0 ? "-" : "",
                            Rows = new List<ExpiresReportItem>()
                        };
                        foreach (var det in row.items.GroupBy(g => new { g.ExpireDate, g.EntityID, g.ReferenceID, g.ReferenceDate }, (detkey, details) => new { detkey, details }))
                        {
                            var dareI = det.details.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                            var avereI = det.details.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                            var diffI = dareI - avereI;
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                EntityID = det.details.First().EntityID,
                                EntityDescription = det.details.First().EntityDescription,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : ""
                            });
                        }
                        // reorder details
                        newMaster.Rows = newMaster.Rows.OrderBy(o => o.ExpireDate).ThenBy(tb => tb.EntityID).ToList();
                        result.Rows.Add(newMaster);
                        // reorder master
                        result.Rows = result.Rows.OrderBy(o => o.EntityID).ToList();
                        result.GrandTotal += diff;
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

    public ObservableCollection<BatchReportModel.EntityModel>? GetBatch(string CompanyID, DateTime From, DateTime To)
    {
        try
        {
            using var connection = GetOpenConnection();

            var retValue = new List<BatchReportModel.EntityModel>();

            // Get the one with initial value
            string initial = @$"select 
                                    DISTINCT(n2sott) as EntityID, 
                                    MIN(a.abers1) as EntityDescription,
                                    SUM(
                                            CASE 
                                                WHEN  N2SEGN = 'D'
                                                THEN N2IMEU
                                                ELSE 0
                                            END
                                        ) - SUM(
                                            CASE 
                                                WHEN N2SEGN = 'A'
                                                THEN N2IMEU
                                                ELSE 0
                                            END
                                        ) as InitialValue
                                    FROM PNCLIENTI as f
                                    LEFT OUTER JOIN ABE as a ON f.N2SOTT = a.abecod
                                    WHERE f.N2SOCI = @cid AND f.N2DARE < @From
                                    GROUP BY f.n2sott
                                    order by f.n2sott";
            var initialList = connection.Query<BatchReportModel.EntityModel>(
               initial,
               new { cid = CompanyID, From = From });

            retValue.AddRange(initialList.Where(o => o.InitialValue != 0));

            // Get the one with movements
            string movement = $@"select 
                                    DISTINCT(n2sott) EntityID, 
                                    MIN(a.abers1) EntityDescription
                                    FROM PNCLIENTI as f
                                    LEFT OUTER JOIN ABE as a ON f.N2SOTT = a.abecod
                                    WHERE N2SOCI = @cid AND N2DARE >= @From AND N2DARE <= @To
                                    GROUP BY n2sott
                                    order by n2sott";
            var movementList = connection.Query<BatchReportModel.EntityModel>(
              movement,
              new { cid = CompanyID, From = From, To = To });


            foreach (var ent in movementList.Where(o => !retValue.Any(oo => oo.EntityID == o.EntityID)))
            {
                retValue.Add(new BatchReportModel.EntityModel
                {
                    EntityID = ent.EntityID,
                    EntityDescription = ent.EntityDescription,
                    InitialValue = 0
                });
            }

            foreach (var ent in retValue)
            {
                string movementEntities = $@"select 
                                    n2anno as Year,
                                    n2regi as ID,
                                    n2riga as Row,
                                    n2imeu as Import,
                                    n2segn as Sign,
                                    n2dare as Date,
                                    n2dari as ReferenceDate,
                                    n2rife as ReferenceID,
                                    n2docu as DocumentID,
                                    n2dado as DocumentDate
                                    FROM PNCLIENTI as f
                                    WHERE N2SOCI = @cid AND N2DARE >= @From AND N2DARE <= @To AND N2SOTT = @eid
                                    order by n2dare";

                var movementEntitiesList = connection.Query<BatchReportModel.MovementModel>(
                  movementEntities,
                  new { cid = CompanyID, From = From, To = To, eid = ent.EntityID });

                ent.Movements = movementEntitiesList.ToList() ?? new List<BatchReportModel.MovementModel>();
            }

            return new ObservableCollection<BatchReportModel.EntityModel>(retValue);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PNCLIENTI (N2SOCI,N2ANNO,N2REGI,N2RIGA,N2DARI,N2RIFE,N2DOCU,N2DARE,N2DADO,N2CAUS,N2GRUP,N2CONT,N2SSOC,N2SOTT,N2IMPO,N2DESC,N2SEGN,N2RATA,N2SCAD,N2PAGA,N2PARE,N2PRAT,N2DEST,N2PAXI,N2INIZ,N2CAMB,N2VALU,N2DIVI,n2vcod,N2FLIN,N2DVAL,N2IMEU,N2TIDO,n2rior,N2FL01,n2tipi,N2VADO,N2DIDO,n2comm,N2ANTI) OUTPUT INSERTED.rv VALUES(@N2SOCI,@N2ANNO,@N2REGI,@N2RIGA,@N2DARI,@N2RIFE,@N2DOCU,@N2DARE,@N2DADO,@N2CAUS,@N2GRUP,@N2CONT,@N2SSOC,@N2SOTT,@N2IMPO,@N2DESC,@N2SEGN,@N2RATA,@N2SCAD,@N2PAGA,@N2PARE,@N2PRAT,@N2DEST,@N2PAXI,@N2INIZ,@N2CAMB,@N2VALU,@N2DIVI,@n2vcod,@N2FLIN,@N2DVAL,@N2IMEU,@N2TIDO,@n2rior,@N2FL01,@n2tipi,@N2VADO,@N2DIDO,@n2comm,@N2ANTI)";
    public string UPDATE_QUERY => "UPDATE PNCLIENTI SET N2SOCI = @N2SOCI,N2ANNO = @N2ANNO,N2REGI = @N2REGI,N2RIGA = @N2RIGA,N2DARI = @N2DARI,N2RIFE = @N2RIFE,N2DOCU = @N2DOCU,N2DARE = @N2DARE,N2DADO = @N2DADO,N2CAUS = @N2CAUS,N2GRUP = @N2GRUP,N2CONT = @N2CONT,N2SSOC = @N2SSOC,N2SOTT = @N2SOTT,N2IMPO = @N2IMPO,N2DESC = @N2DESC,N2SEGN = @N2SEGN,N2RATA = @N2RATA,N2SCAD = @N2SCAD,N2PAGA = @N2PAGA,N2PARE = @N2PARE,N2PRAT = @N2PRAT,N2DEST = @N2DEST,N2PAXI = @N2PAXI,N2INIZ = @N2INIZ,N2CAMB = @N2CAMB,N2VALU = @N2VALU,N2DIVI = @N2DIVI,n2vcod = @n2vcod,N2FLIN = @N2FLIN,N2DVAL = @N2DVAL,N2IMEU = @N2IMEU,N2TIDO = @N2TIDO,n2rior = @n2rior,N2FL01 = @N2FL01,n2tipi = @n2tipi,N2VADO = @N2VADO,N2DIDO = @N2DIDO,n2comm = @n2comm,N2ANTI = @N2ANTI OUTPUT INSERTED.rv WHERE N2SOCI = @N2SOCI AND N2ANNO = @N2ANNO AND N2REGI = @N2REGI AND N2RIGA = @N2RIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PNCLIENTI OUTPUT DELETED.rv WHERE N2SOCI = @N2SOCI AND N2ANNO = @N2ANNO AND N2REGI = @N2REGI AND N2RIGA = @N2RIGA AND rv = @rv";
    public bool Insert(PNCLIENTI Model)
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

    public bool Update(PNCLIENTI Model)
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

    public bool DeleteSingle(PNCLIENTI Model)
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
                "DELETE FROM PNCLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                new { n2soci = CompanyID, n2anno = Year, n2regi = Number });
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

    public string? Validate(PNCLIENTI Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PNCLIENTIUfpRepository : RepositoryBase, IPNCLIENTIRepository
{
    public PNCLIENTIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNCLIENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                "SELECT * FROM PN_CLIENTI");

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNCLIENTI>? GetListByReg(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                @"SELECT p.N2SCAD, p.N2RIFE, p.N2DARI, p.N2IMEU, p.N2SEGN, CONCAT(a.abecod , ' ' , TRIM(a.abers1)) AS CustomerDescription FROM PN_CLIENTI AS p
                        INNER JOIN ANAG_BASE AS a ON a.abecod = p.N2SOTT
                        WHERE p.N2SOCI = @cid AND p.N2ANNO = @yea AND p.N2REGI = @reg",
                new { cid = CompanyID, yea = Year, reg = Number });

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PNCLIENTI>? GetList(string CompanyID, int Year, int Number, int OriginalRowID, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                @"SELECT * FROM PN_CLIENTI 
                        WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi AND N2SOTT = @n2sott AND (n2rior = @n2rior OR n2rior is null OR n2rior = 0)
                        ORDER BY N2RIGA",
                new { n2soci = CompanyID, n2anno = Year, n2regi = Number, n2sott = CustomerID, n2rior = OriginalRowID });

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<MastrinoECReportItem>? GetECList(string CompanyID, int CustomerID, DateTime ToDate, bool HasDrawn, DateTime SinceDrawn)
    {
        try
        {
            using var connection = GetOpenConnection();
            ObservableCollection<MastrinoECReportItem> result = new ObservableCollection<MastrinoECReportItem>();

            string? query = null;
            if (!HasDrawn)
                query = @"SELECT m.*, invfile.fn as InvoiceFile , c.caucod, c.caudes FROM PN_CLIENTI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.n2caus
                        OUTER APPLY(SELECT TOP(1) d.arcfile as fn FROM ARCCLI as d WHERE d.arcsoc = m.N2SOCI AND d.arccli = m.n2sott AND d.arcanno = YEAR(m.n2DADO) AND d.arcrig = TRY_CAST(m.N2DOCU AS INT) AND d.arctip = 'F') AS invfile
                        WHERE m.N2SOCI = @n2soci AND m.N2SOTT = @n2sott AND m.N2DARE <= @n2dare AND (m.N2PARE IS NULL OR m.N2PARE <> '*')
                        ORDER BY m.N2DARI, m.N2RIFE";
            else
                query = @"SELECT m.*, invfile.fn as InvoiceFile, c.caucod, c.caudes FROM PN_CLIENTI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.n2caus
                        OUTER APPLY(SELECT TOP(1) d.arcfile as fn FROM ARCCLI as d WHERE d.arcsoc = m.N2SOCI AND d.arccli = m.n2sott AND d.arcanno = YEAR(m.n2DADO) AND d.arcrig = TRY_CAST(m.N2DOCU AS INT) AND d.arctip = 'F') AS invfile
                        WHERE m.N2SOCI = @n2soci AND m.N2SOTT = @n2sott AND m.N2DARE <= @n2dare AND 
                        ((m.N2PARE IS NULL OR m.N2PARE <> '*') OR (m.N2PARE = '*' AND m.N2DARI >= @dapare))
                        ORDER BY m.N2DARI, m.N2RIFE";

            var list = connection.Query<PNCLIENTI, CAUCONT, PNCLIENTI>(
                query,
                (pn, cau) => { pn.Causal = cau; return pn; },
                new { n2soci = CompanyID, n2sott = CustomerID, n2dare = ToDate, dapare = SinceDrawn },
                splitOn: "caucod");

            foreach (var item in list)
            {
                //var solls = (!string.IsNullOrEmpty(item.N2RIFE) && item.N2SOTT.HasValue) ? VulpesServiceProvider.Provider.GetRequiredService<ISOLLE0FRepository>().GetList(item.N2SOCI, item.N2SOTT.Value, item.N2RIFE) : null;
                //foreach (var soll in solls ?? new ObservableCollection<SOLLE0F>())
                //{
                //    soll.DocumentID = item.N2DOCU;
                //    soll.DocumentDate = item.N2DADO;
                //}
                result.Add(new MastrinoECReportItem()
                {
                    CompanyID = item.N2SOCI,
                    Year = item.N2ANNO,
                    RowID = item.N2RIGA,
                    DocumentDate = item.N2DADO,
                    DocumentID = item.N2DOCU,
                    RegistrationDate = item.N2DARE,
                    ReferenceDate = item.N2DARI,
                    ReferenceID = item.N2RIFE,
                    Number = item.N2REGI,
                    ExpireDate = item.N2SCAD,
                    CurrencyID = item.N2DIVI,
                    Dare = item.N2SEGN == "D" ? (item.N2IMEU ?? 0) : 0,
                    Avere = item.N2SEGN == "A" ? (item.N2IMEU ?? 0) : 0,
                    CausalFullDescription = item.Causal?.FullDescriptionSearchable,
                    EntityType = "C",
                    EntityID = CustomerID,
                    Reminders = null,
                    PaymentID = item.N2PAGA,
                    PaymentTypeID = item.n2tipi,
                    Scadenza2 = item.N2INIZ,
                    PNCLIENTI = item,
                    Cambio = item.N2CAMB,
                    ValoreValuta = item.N2VALU,
                    Valuta = item.N2DIDO,
                    SaldoRiga = item.N2SEGN == "D" ? result.Sum(s => s.Saldo) + (item.N2IMEU ?? 0) : result.Sum(s => s.Saldo) - (item.N2IMEU ?? 0),
                    InvoiceFile = item.InvoiceFile,
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

    public ObservableCollection<PNCLIENTI>? GetChecks(string CompanyID, DateTime From)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCLIENTI>(
                @"SELECT p.N2SCAD, p.N2RIFE, p.N2DARI, p.N2IMEU, p.N2SEGN, p.N2INIZ, p.N2DOCU, p.N2DADO, CONCAT(a.abecod , ' ' , TRIM(a.abers1)) AS CustomerDescription FROM PN_CLIENTI AS p
                        INNER JOIN ANAG_BASE AS a ON a.abecod = p.N2SOTT
                        WHERE p.N2SOCI = @cid AND p.N2INIZ >= @from",
                new { cid = CompanyID, from = From });

            return new ObservableCollection<PNCLIENTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }


    public decimal GetExpired(string CompanyID, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();

            string query = @"SELECT m.*, c.caucod, c.caudes FROM PN_CLIENTI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.n2caus
                        WHERE m.N2SOCI = @n2soci AND m.N2SOTT = @n2sott AND m.N2SCAD <= @n2Scad AND m.N2PARE IS NULL
                        ORDER BY m.N2DARI, m.N2RIFE";

            var list = connection.Query<PNCLIENTI, CAUCONT, PNCLIENTI>(
                query,
                (pn, cau) => { pn.Causal = cau; return pn; },
                new { n2soci = CompanyID, n2sott = CustomerID, n2Scad = DateTime.Now },
                splitOn: "caucod");

            if (list.Any())
                return (list.Where(o => o.N2SEGN == "D").Sum(s => s.N2IMEU) ?? 0) - (list.Where(o => o.N2SEGN == "A").Sum(s => s.N2IMEU) ?? 0);
            else
                return 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public bool SyncPartita(string CompanyID, int SourceYear, int SourceNumber, int SourceRow, int TargetYear, int TargetNumber, int TargetRow)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                var multi = connection.QueryMultiple(
                @"SELECT * FROM PN_CLIENTI WHERE N2SOCI=@cid AND N2ANNO = @sy AND N2REGI = @sn AND N2RIGA = @sr;
                                SELECT * FROM PN_CLIENTI WHERE N2SOCI=@cid AND N2ANNO = @ty AND N2REGI = @tn AND N2RIGA = @tr;",
                new { cid = CompanyID, sy = SourceYear, sn = SourceNumber, sr = SourceRow, ty = TargetYear, tn = TargetNumber, tr = TargetRow }, transaction);
                var source = multi.Read<PNCLIENTI>().SingleOrDefault();
                var target = multi.Read<PNCLIENTI>().SingleOrDefault();
                if (source != null && target != null)
                {
                    source.N2RIFE = target.N2RIFE;
                    source.N2DARI = target.N2DARI;
                    source.N2SCAD = target.N2SCAD;
                    var updated = connection.Execute(UPDATE_QUERY, source, transaction);
                    if (updated > 0)
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
                    ErrorHandler.Show("Righe di registrazione non trovate");
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

    public PNCLIENTI? Get(string CompanyID, int Year, int Number, int Row)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PNCLIENTI>(
                "SELECT * FROM PN_CLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                new { n2soci = CompanyID, n2anno = Year, n2regi = Number, n2riga = Row })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<DateTime>? ComputeExpires(DateTime DocumentDate, string PaymentTypeID)
    {
        try
        {
            var payment = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(PaymentTypeID);

            List<DateTime> result = new List<DateTime> { DocumentDate };

            if (payment != null)
            {
                if (payment.pclgs1.HasValue && payment.pclgs1.Value > 0)
                {
                    var newExpire1 = result.First().AddDays(payment.pclgs1.Value);
                    if (payment.pclppa == "0")
                        result[0] = newExpire1;
                    else
                        result[0] = new DateTime(newExpire1.Year, newExpire1.Month, DateTime.DaysInMonth(newExpire1.Year, newExpire1.Month));

                    if (payment.pclgs2.HasValue && payment.pclgs2.Value > 0)
                    {
                        var newExpire2 = DocumentDate.AddDays(payment.pclgs2.Value);
                        if (payment.pclppa == "0")
                            result.Add(newExpire2);
                        else
                            result.Add(new DateTime(newExpire2.Year, newExpire2.Month, DateTime.DaysInMonth(newExpire2.Year, newExpire2.Month)));

                        if (payment.pclgs3.HasValue && payment.pclgs3.Value > 0)
                        {
                            var newExpire3 = DocumentDate.AddDays(payment.pclgs3.Value);
                            if (payment.pclppa == "0")
                                result.Add(newExpire3);
                            else
                                result.Add(new DateTime(newExpire3.Year, newExpire3.Month, DateTime.DaysInMonth(newExpire3.Year, newExpire3.Month)));

                            if (payment.pclgs4.HasValue && payment.pclgs4.Value > 0)
                            {
                                var newExpire4 = DocumentDate.AddDays(payment.pclgs4.Value);
                                if (payment.pclppa == "0")
                                    result.Add(newExpire4);
                                else
                                    result.Add(new DateTime(newExpire4.Year, newExpire4.Month, DateTime.DaysInMonth(newExpire4.Year, newExpire4.Month)));

                                if (payment.pclgs5.HasValue && payment.pclgs5.Value > 0)
                                {
                                    var newExpire5 = DocumentDate.AddDays(payment.pclgs5.Value);
                                    if (payment.pclppa == "0")
                                        result.Add(newExpire5);
                                    else
                                        result.Add(new DateTime(newExpire5.Year, newExpire5.Month, DateTime.DaysInMonth(newExpire5.Year, newExpire5.Month)));

                                    if (payment.pclgs6.HasValue && payment.pclgs6.Value > 0)
                                    {
                                        var newExpire6 = DocumentDate.AddDays(payment.pclgs6.Value);
                                        if (payment.pclppa == "0")
                                            result.Add(newExpire6);
                                        else
                                            result.Add(new DateTime(newExpire6.Year, newExpire6.Month, DateTime.DaysInMonth(newExpire6.Year, newExpire6.Month)));

                                        if (payment.pclgs7.HasValue && payment.pclgs7.Value > 0)
                                        {
                                            var newExpire7 = DocumentDate.AddDays(payment.pclgs7.Value);
                                            if (payment.pclppa == "0")
                                                result.Add(newExpire7);
                                            else
                                                result.Add(new DateTime(newExpire7.Year, newExpire7.Month, DateTime.DaysInMonth(newExpire7.Year, newExpire7.Month)));

                                            if (payment.pclgs8.HasValue && payment.pclgs8.Value > 0)
                                            {
                                                var newExpire8 = DocumentDate.AddDays(payment.pclgs8.Value);
                                                if (payment.pclppa == "0")
                                                    result.Add(newExpire8);
                                                else
                                                    result.Add(new DateTime(newExpire8.Year, newExpire8.Month, DateTime.DaysInMonth(newExpire8.Year, newExpire8.Month)));

                                                if (payment.pclgs9.HasValue && payment.pclgs9.Value > 0)
                                                {
                                                    var newExpire9 = DocumentDate.AddDays(payment.pclgs9.Value);
                                                    if (payment.pclppa == "0")
                                                        result.Add(newExpire9);
                                                    else
                                                        result.Add(new DateTime(newExpire9.Year, newExpire9.Month, DateTime.DaysInMonth(newExpire9.Year, newExpire9.Month)));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    if (payment.pclppa != "0")
                        result[0] = new DateTime(DocumentDate.Year, DocumentDate.Month, DateTime.DaysInMonth(DocumentDate.Year, DocumentDate.Month));
                }
            }

            return result;
        }
        catch { return null; }
    }

    #region Customer analysis
    public Tuple<decimal, string> GetCABalance(string CompanyID, int CustomerID, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multi = connection.QueryMultiple(
                @"SELECT SUM(N2IMEU) FROM PN_CLIENTI WHERE N2SOCI = @cid AND N2SEGN='D' AND N2SOTT = @custid AND N2SCAD <= @todate;
                      SELECT SUM(N2IMEU) FROM PN_CLIENTI WHERE N2SOCI = @cid AND N2SEGN='A' AND N2SOTT = @custid AND N2SCAD <= @todate;",
                new { cid = CompanyID, custid = CustomerID, todate = ToDate });
            var dare = multi.Read<decimal?>().Single() ?? 0m;
            var avere = multi.Read<decimal?>().Single() ?? 0m;
            decimal balance = 0;
            string balanceSign = "-";
            if (dare > avere)
            {
                balance = dare - avere;
                balanceSign = "D";
            }
            else if (dare < avere)
            {
                balance = avere - dare;
                balanceSign = "A";
            }

            return new Tuple<decimal, string>(balance, balanceSign);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, string>(-1, "ERR");
        }
    }

    public Tuple<decimal, string> GetCABalanceExpiring(string CompanyID, int CustomerID, DateTime SinceDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multi = connection.QueryMultiple(
                @"SELECT SUM(N2IMEU) FROM PN_CLIENTI WHERE N2SOCI = @cid AND N2SEGN='D' AND N2SOTT = @custid AND N2SCAD > @sincedate;
                      SELECT SUM(N2IMEU) FROM PN_CLIENTI WHERE N2SOCI = @cid AND N2SEGN='A' AND N2SOTT = @custid AND N2SCAD > @sincedate;",
                new { cid = CompanyID, custid = CustomerID, sincedate = SinceDate });
            var dare = multi.Read<decimal?>().Single() ?? 0m;
            var avere = multi.Read<decimal?>().Single() ?? 0m;
            decimal balance = 0;
            string balanceSign = "-";
            if (dare > avere)
            {
                balance = dare - avere;
                balanceSign = "D";
            }
            else if (dare < avere)
            {
                balance = avere - dare;
                balanceSign = "A";
            }

            return new Tuple<decimal, string>(balance, balanceSign);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, string>(-1, "ERR");
        }
    }

    public Tuple<decimal, string> GetCAExpiredBalance(string CompanyID, int CustomerID, DateTime ToDate, DateTime SinceDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multi = connection.QueryMultiple(
                @"SELECT SUM(N2IMEU) FROM PN_CLIENTI WHERE N2SOCI = @cid AND N2SEGN='D' AND N2SOTT = @custid AND N2DARI <= @todate AND N2SCAD >= @sincedate AND N2SCAD <= @todate;
                      SELECT SUM(N2IMEU) FROM PN_CLIENTI WHERE N2SOCI = @cid AND N2SEGN='A' AND N2SOTT = @custid AND N2DARI <= @todate AND N2SCAD >= @sincedate AND N2SCAD <= @todate;",
                new { cid = CompanyID, custid = CustomerID, todate = ToDate, sincedate = SinceDate });
            var dare = multi.Read<decimal?>().Single() ?? 0m;
            var avere = multi.Read<decimal?>().Single() ?? 0m;
            decimal balance = 0;
            string balanceSign = "-";
            if (dare > avere)
            {
                balance = dare - avere;
                balanceSign = "D";
            }
            else if (dare < avere)
            {
                balance = avere - dare;
                balanceSign = "A";
            }

            return new Tuple<decimal, string>(balance, balanceSign);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, string>(-1, "ERR");
        }
    }

    public decimal GetCATotalCirculating(string CompanyID, int CustomerID, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (decimal)(connection.ExecuteScalar(
                @"SELECT SUM(l.N2IMEU) FROM PN_CLIENTI AS l
                        INNER JOIN CAUCONT AS r ON l.N2CAUS = r.caucod
                        WHERE l.N2SOCI = @cid AND r.caucir = 'S' AND l.N2SOTT = @custid AND l.N2DARE <= @todate AND l.N2SCAD > @todate",
                new { cid = CompanyID, custid = CustomerID, todate = ToDate }) ?? 0m);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public decimal GetCACirculating(string CompanyID, int CustomerID, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (decimal)(connection.ExecuteScalar(
                @"SELECT SUM(l.N2IMEU) FROM PN_CLIENTI AS l
                        INNER JOIN CAUCONT AS r ON l.N2CAUS = r.caucod
                        WHERE l.N2SOCI = @cid AND r.caucir = 'S' AND l.N2SOTT = @custid AND l.N2DARE <= @todate AND (l.N2SCAD >= @fromdate AND l.N2SCAD <= @todate);",
                new { cid = CompanyID, custid = CustomerID, fromdate = FromDate, todate = ToDate }) ?? 0);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }
    #endregion

    #region Reports
    public ExpiresReport? ExpiresReport(string CompanyID, ABE Entity, GenericIDDescription PaymentType, DateTime? ExpireDate, string GroupType)
    {
        try
        {
            using var connection = GetOpenConnection();
            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(CompanyID);
            var result = new ExpiresReport()
            {
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase!.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png")
            };
           
                string query = $@"SELECT abe.abecod AS EntityID, 'F' AS EntityTypeID, pn.N2SEGN AS Sign, TRIM(abe.abers1) AS EntityDescription, pn.N2SCAD AS ExpireDate, TRIM(pn.N2RIFE) AS ReferenceID,pn.N2DARI AS ReferenceDate,tip.icscod AS PaymentTypeID,TRIM(tip.icsdes) AS PaymentTypeDescription, pn.N2IMEU AS Amount, TRIM(cau.caudes) AS CausalDescription FROM PN_CLIENTI AS pn
                                        INNER JOIN PN_TESTATA AS hea ON hea.N1SOCI=pn.N2SOCI AND hea.N1ANNO=pn.N2ANNO AND hea.N1REGI=pn.N2REGI
                                        INNER JOIN ANAG_BASE AS abe ON abe.abecod = pn.N2SOTT
                                        INNER JOIN CAUCONT AS cau ON cau.caucod = hea.pncaus
                                        LEFT JOIN PAGCLI AS pag ON pag.pclcod = pn.N2PAGA
                                        LEFT JOIN INCASSI1 AS tip ON tip.icscod = pag.pcltip
                                        WHERE pn.N2SOCI=@N2soci AND pn.N2PARE IS NULL AND hea.N1TmpPN = 'N'
                                            {(Entity != null ? " AND pn.n2sott=@n2sott" : null)}
                                            {(!string.IsNullOrWhiteSpace(PaymentType.ID) ? " AND pag.pcltip=@pcltip" : null)}
                                            {(ExpireDate.HasValue ? " AND pn.N2SCAD<=@n2scad" : null)}
                                        order by pn.n2sott,pag.pcltip,pn.N2SCAD";

                var list = connection.Query<ExpiresReportItem>(
                    query,
                    new { n2soci = CompanyID, n2sott = Entity?.abecod, n2scad = ExpireDate, pcltip = PaymentType?.ID });

                result.Rows = new List<ExpiresReportMasterItem>();

                if (GroupType == "C")
                {
                    // customer
                    foreach (var row in list.GroupBy(g => g.EntityID, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = dare - avere;
                        var newMaster = new ExpiresReportMasterItem()
                        {
                            EntityID = row.key,
                            EntityDescription = row.items.First().EntityDescription,
                            EntityTotal = diff < 0 ? diff * -1 : diff,
                            PrintSign = diff < 0 ? "-" : "",
                            Rows = new List<ExpiresReportItem>()
                        };
                        foreach (var det in row.items.GroupBy(g => new { g.PaymentTypeID, g.ReferenceID, g.ReferenceDate, g.ExpireDate }, (detkey, details) => new { detkey, details }))
                        {
                            var dareI = det.details.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                            var avereI = det.details.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                            var diffI = dareI - avereI;
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                PaymentTypeID = det.details.First().PaymentTypeID,
                                PaymentTypeDescription = det.details.First().PaymentTypeDescription,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : ""
                            });
                        }
                        // reorder details
                        newMaster.Rows = newMaster.Rows.OrderBy(o => o.ExpireDate).ThenBy(tb => tb.PaymentTypeID).ToList();
                        result.Rows.Add(newMaster);
                        result.GrandTotal += diff;
                    }
                }
                else if (GroupType == "S")
                {
                    // expire date
                    foreach (var row in list.GroupBy(g => g.ExpireDate, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = dare - avere;
                        var newMaster = new ExpiresReportMasterItem()
                        {
                            ExpireDate = row.key,
                            EntityID = row.key.ToString("dd/MM/yyyy"),
                            EntityDescription = null,
                            EntityTotal = diff < 0 ? diff * -1 : diff,
                            PrintSign = diff < 0 ? "-" : "",
                            Rows = new List<ExpiresReportItem>()
                        };
                        foreach (var det in row.items.GroupBy(g => new { g.EntityID, g.PaymentTypeID, g.ReferenceID, g.ReferenceDate }, (detkey, details) => new { detkey, details }))
                        {
                            var dareI = det.details.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                            var avereI = det.details.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                            var diffI = dareI - avereI;
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                EntityID = det.details.First().EntityID,
                                EntityDescription = det.details.First().EntityDescription,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                PaymentTypeID = det.details.First().PaymentTypeID,
                                PaymentTypeDescription = det.details.First().PaymentTypeDescription,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : ""
                            });
                        }
                        // reorder details
                        newMaster.Rows = newMaster.Rows.OrderBy(o => o.EntityID).ThenBy(tb => tb.PaymentTypeID).ToList();
                        result.Rows.Add(newMaster);
                        // reorder master
                        result.Rows = result.Rows.OrderBy(o => o.ExpireDate).ToList();
                        result.GrandTotal += diff;
                    }
                }
                else
                {
                    // payment type
                    foreach (var row in list.GroupBy(g => g.PaymentTypeID, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = dare - avere;
                        var newMaster = new ExpiresReportMasterItem()
                        {
                            EntityID = row.key,
                            EntityDescription = row.items.First().PaymentTypeDescription,
                            EntityTotal = diff < 0 ? diff * -1 : diff,
                            PrintSign = diff < 0 ? "-" : "",
                            Rows = new List<ExpiresReportItem>()
                        };
                        foreach (var det in row.items.GroupBy(g => new { g.ExpireDate, g.EntityID, g.ReferenceID, g.ReferenceDate }, (detkey, details) => new { detkey, details }))
                        {
                            var dareI = det.details.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                            var avereI = det.details.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                            var diffI = dareI - avereI;
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                EntityID = det.details.First().EntityID,
                                EntityDescription = det.details.First().EntityDescription,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : ""
                            });
                        }
                        // reorder details
                        newMaster.Rows = newMaster.Rows.OrderBy(o => o.ExpireDate).ThenBy(tb => tb.EntityID).ToList();
                        result.Rows.Add(newMaster);
                        // reorder master
                        result.Rows = result.Rows.OrderBy(o => o.EntityID).ToList();
                        result.GrandTotal += diff;
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

    public ObservableCollection<BatchReportModel.EntityModel>? GetBatch(string CompanyID, DateTime From, DateTime To)
    {
        try
        {
            using var connection = GetOpenConnection();

            var retValue = new List<BatchReportModel.EntityModel>();

            // Get the one with initial value
            string initial = @$"select 
                                    DISTINCT(n2sott) as EntityID, 
                                    MIN(a.abers1) as EntityDescription,
                                    SUM(
                                            CASE 
                                                WHEN  N2SEGN = 'D'
                                                THEN N2IMEU
                                                ELSE 0
                                            END
                                        ) - SUM(
                                            CASE 
                                                WHEN N2SEGN = 'A'
                                                THEN N2IMEU
                                                ELSE 0
                                            END
                                        ) as InitialValue
                                    FROM PN_CLIENTI as f
                                    LEFT OUTER JOIN ANAG_BASE as a ON f.N2SOTT = a.abecod
                                    WHERE f.N2SOCI = @cid AND f.N2DARE < @From
                                    GROUP BY f.n2sott
                                    order by f.n2sott";
            var initialList = connection.Query<BatchReportModel.EntityModel>(
               initial,
               new { cid = CompanyID, From = From });

            retValue.AddRange(initialList.Where(o => o.InitialValue != 0));

            // Get the one with movements
            string movement = $@"select 
                                    DISTINCT(n2sott) EntityID, 
                                    MIN(a.abers1) EntityDescription
                                    FROM PN_CLIENTI as f
                                    LEFT OUTER JOIN ANAG_BASE as a ON f.N2SOTT = a.abecod
                                    WHERE N2SOCI = @cid AND N2DARE >= @From AND N2DARE <= @To
                                    GROUP BY n2sott
                                    order by n2sott";
            var movementList = connection.Query<BatchReportModel.EntityModel>(
              movement,
              new { cid = CompanyID, From = From, To = To });


            foreach (var ent in movementList.Where(o => !retValue.Any(oo => oo.EntityID == o.EntityID)))
            {
                retValue.Add(new BatchReportModel.EntityModel
                {
                    EntityID = ent.EntityID,
                    EntityDescription = ent.EntityDescription,
                    InitialValue = 0
                });
            }

            foreach (var ent in retValue)
            {
                string movementEntities = $@"select 
                                    n2anno as Year,
                                    n2regi as ID,
                                    n2riga as Row,
                                    n2imeu as Import,
                                    n2segn as Sign,
                                    n2dare as Date,
                                    n2dari as ReferenceDate,
                                    n2rife as ReferenceID,
                                    n2docu as DocumentID,
                                    n2dado as DocumentDate
                                    FROM PN_CLIENTI as f
                                    WHERE N2SOCI = @cid AND N2DARE >= @From AND N2DARE <= @To AND N2SOTT = @eid
                                    order by n2dare";

                var movementEntitiesList = connection.Query<BatchReportModel.MovementModel>(
                  movementEntities,
                  new { cid = CompanyID, From = From, To = To, eid = ent.EntityID });

                ent.Movements = movementEntitiesList.ToList() ?? new List<BatchReportModel.MovementModel>();
            }

            return new ObservableCollection<BatchReportModel.EntityModel>(retValue);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PN_CLIENTI (N2SOCI,N2ANNO,N2REGI,N2RIGA,N2DARI,N2RIFE,N2DOCU,N2DARE,N2DADO,N2CAUS,N2GRUP,N2CONT,N2SSOC,N2SOTT,N2IMPO,N2DESC,N2SEGN,N2RATA,N2SCAD,N2PAGA,N2PARE,N2PRAT,N2DEST,N2PAXI,N2INIZ,N2CAMB,N2VALU,N2DIVI,n2vcod,N2FLIN,N2DVAL,N2IMEU,N2TIDO,n2rior,N2FL01,n2tipi,N2VADO,N2DIDO,n2comm,N2ANTI) OUTPUT INSERTED.rv VALUES(@N2SOCI,@N2ANNO,@N2REGI,@N2RIGA,@N2DARI,@N2RIFE,@N2DOCU,@N2DARE,@N2DADO,@N2CAUS,@N2GRUP,@N2CONT,@N2SSOC,@N2SOTT,@N2IMPO,@N2DESC,@N2SEGN,@N2RATA,@N2SCAD,@N2PAGA,@N2PARE,@N2PRAT,@N2DEST,@N2PAXI,@N2INIZ,@N2CAMB,@N2VALU,@N2DIVI,@n2vcod,@N2FLIN,@N2DVAL,@N2IMEU,@N2TIDO,@n2rior,@N2FL01,@n2tipi,@N2VADO,@N2DIDO,@n2comm,@N2ANTI)";
    public string UPDATE_QUERY => "UPDATE PN_CLIENTI SET N2SOCI = @N2SOCI,N2ANNO = @N2ANNO,N2REGI = @N2REGI,N2RIGA = @N2RIGA,N2DARI = @N2DARI,N2RIFE = @N2RIFE,N2DOCU = @N2DOCU,N2DARE = @N2DARE,N2DADO = @N2DADO,N2CAUS = @N2CAUS,N2GRUP = @N2GRUP,N2CONT = @N2CONT,N2SSOC = @N2SSOC,N2SOTT = @N2SOTT,N2IMPO = @N2IMPO,N2DESC = @N2DESC,N2SEGN = @N2SEGN,N2RATA = @N2RATA,N2SCAD = @N2SCAD,N2PAGA = @N2PAGA,N2PARE = @N2PARE,N2PRAT = @N2PRAT,N2DEST = @N2DEST,N2PAXI = @N2PAXI,N2INIZ = @N2INIZ,N2CAMB = @N2CAMB,N2VALU = @N2VALU,N2DIVI = @N2DIVI,n2vcod = @n2vcod,N2FLIN = @N2FLIN,N2DVAL = @N2DVAL,N2IMEU = @N2IMEU,N2TIDO = @N2TIDO,n2rior = @n2rior,N2FL01 = @N2FL01,n2tipi = @n2tipi,N2VADO = @N2VADO,N2DIDO = @N2DIDO,n2comm = @n2comm,N2ANTI = @N2ANTI OUTPUT INSERTED.rv WHERE N2SOCI = @N2SOCI AND N2ANNO = @N2ANNO AND N2REGI = @N2REGI AND N2RIGA = @N2RIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PN_CLIENTI OUTPUT DELETED.rv WHERE N2SOCI = @N2SOCI AND N2ANNO = @N2ANNO AND N2REGI = @N2REGI AND N2RIGA = @N2RIGA AND rv = @rv";
    public bool Insert(PNCLIENTI Model)
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

    public bool Update(PNCLIENTI Model)
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

    public bool DeleteSingle(PNCLIENTI Model)
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
                "DELETE FROM PNCLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                new { n2soci = CompanyID, n2anno = Year, n2regi = Number });
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

    public string? Validate(PNCLIENTI Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}