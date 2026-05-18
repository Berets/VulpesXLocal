

using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared.Generics;
using static Dapper.SqlMapper;

namespace VulpesX.DAL.Accounting
{
    public interface IPNFORNITORIRepository
    {
        Tuple<bool, decimal>? GetSaldoRiferimento(string CompanyID, int CustomerID, string ReferenceID, DateTime ReferenceDate);

        ObservableCollection<PNFORNITORI>? GetList(string CompanyID, int Year, int Number, int RowID, int SupplierID);

        ObservableCollection<PNFORNITORI>? GetListByReg(string CompanyID, int Year, int Number);

        ObservableCollection<MastrinoECReportItem>? GetECList(string CompanyID, int SupplierID, DateTime ToDate, bool HasDrawn, DateTime SinceDrawn, bool ExcludeInPaymentExecution = false);

        bool SyncPartita(string CompanyID, int SourceYear, int SourceNumber, int SourceRow, int TargetYear, int TargetNumber, int TargetRow);

        PNFORNITORI? Get(string CompanyID, int Year, int Number, int Row);

        List<DateTime>? ComputeExpires(DateTime DocumentDate, string PaymentTypeID);

        bool Lock(string CompanyID, int Year, int Number, string UserID, string Reason);

        bool Unlock(string CompanyID, int Year, int Number, string UserID);

        #region Reports
        ExpiresReport? ExpiresReport(string CompanyID, ABE Entity, GenericIDDescription PaymentType, DateTime? ExpireDate, string GroupType);

        ObservableCollection<BatchReportModel.EntityModel>? GetBatch(string CompanyID, DateTime From, DateTime To);
        #endregion

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(PNFORNITORI Model);

        bool Update(PNFORNITORI Model);

        bool DeleteSingle(PNFORNITORI Model);

        bool DeleteByReg(string CompanyID, int Year, int Number);

        string? Validate(PNFORNITORI Model, bool IsInsert);
        #endregion
    }

    public class PNFORNITORIRepository : RepositoryBase, IPNFORNITORIRepository
    {
        public PNFORNITORIRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public Tuple<bool, decimal>? GetSaldoRiferimento(string CompanyID, int CustomerID, string ReferenceID, DateTime ReferenceDate)
        {
            try
            {
                using var connection = GetOpenConnection();

                var multi = connection.QueryMultiple(
                    @"SELECT SUM(N3IMEU) AS Dare FROM PNFORNITORI WHERE N3SOCI=@cid AND N3SEGN = 'D' AND N3SOTT = @cust AND N3RIFE = @rifid AND N3DARI = @rifdate;
                          SELECT SUM(N3IMEU) AS Avere FROM PNFORNITORI WHERE N3SOCI=@cid AND N3SEGN = 'A' AND N3SOTT = @cust AND N3RIFE = @rifid AND N3DARI = @rifdate;",
                    new { cid = CompanyID, cust = CustomerID, rifid = ReferenceID, rifdate = ReferenceDate });

                var dare = multi.Read<decimal>().Single();
                var avere = multi.Read<decimal>().Single();

                return dare >= avere ? new Tuple<bool, decimal>(false, dare - avere) : new Tuple<bool, decimal>(true, avere - dare);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNFORNITORI>? GetList(string CompanyID, int Year, int Number, int RowID, int SupplierID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNFORNITORI>(
                    @"SELECT * FROM PNFORNITORI 
                        WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi AND N3SOTT = @n3sott AND n3rior = @n3rior
                        ORDER BY N3RIGA",
                    new { n3soci = CompanyID, n3anno = Year, n3regi = Number, n3sott = SupplierID, n3rior = RowID });

                return new ObservableCollection<PNFORNITORI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNFORNITORI>? GetListByReg(string CompanyID, int Year, int Number)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNFORNITORI>(
                    @"SELECT p.N3SCAD, p.N3RIFE, p.N3DARI, p.N3IMEU, p.N3SEGN, CONCAT(a.abecod , ' ' , TRIM(a.abers1)) AS SupplierDescription FROM PNFORNITORI AS p
                        INNER JOIN ABE AS a ON a.abecod = p.N3SOTT
                        WHERE p.N3SOCI = @cid AND p.N3ANNO = @yea AND p.N3REGI = @reg",
                    new { cid = CompanyID, yea = Year, reg = Number });

                return new ObservableCollection<PNFORNITORI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<MastrinoECReportItem>? GetECList(string CompanyID, int SupplierID, DateTime ToDate, bool HasDrawn, DateTime SinceDrawn, bool ExcludeInPaymentExecution = false)
        {
            try
            {
                using var connection = GetOpenConnection();
                ObservableCollection<MastrinoECReportItem> result = new ObservableCollection<MastrinoECReportItem>();

                string queryExcludePaymentExecution = @"AND NOT EXISTS (
                                                              SELECT 1
                                                              FROM MPDETTAGLIO e
                                                              WHERE e.M3SOCI = m.N3SOCI AND e.M3ANNO = m.N3ANNO AND e.m3REGI = m.N3REGI AND e.M3RIGA = m.N3RIGA
                                                          )";

                string? query = null;
                if (!HasDrawn)
                    query = $@"SELECT m.*, c.caucod, c.caudes FROM PNFORNITORI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.N3caus
                        WHERE m.N3SOCI = @N3soci AND m.N3SOTT = @N3sott AND m.N3DARE <= @N3dare AND m.N3PARE IS NULL {(ExcludeInPaymentExecution ? queryExcludePaymentExecution : null)}
                        ORDER BY m.N3DARI, m.N3RIFE";
                else
                    query = $@"SELECT m.*, c.caucod, c.caudes FROM PNFORNITORI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.N3caus
                        WHERE m.N3SOCI = @N3soci AND m.N3SOTT = @N3sott AND m.N3DARE <= @N3dare AND (m.N3PARE IS NULL OR (m.N3PARE = '*' AND m.N3DARE >= @dapare)) {(ExcludeInPaymentExecution ? queryExcludePaymentExecution : null)}
                        ORDER BY m.N3DARI, m.N3RIFE";

                var list = connection.Query<PNFORNITORI, CAUCONT, PNFORNITORI>(
                    query,
                    (pn, cau) => { pn.Causal = cau; return pn; },
                    new { N3soci = CompanyID, N3sott = SupplierID, N3dare = ToDate, dapare = SinceDrawn },
                    splitOn: "caucod");

                foreach (var item in list)
                {
                    // OPTIMIZE
                    var solls = (item.N3SOTT.HasValue && !string.IsNullOrEmpty(item.N3RIFE)) ? VulpesServiceProvider.Provider.GetRequiredService<ISOLLE0FRepository>().GetList(item.N3SOCI, item.N3SOTT.Value, item.N3RIFE) : null;
                    foreach (var soll in solls ?? new ObservableCollection<SOLLE0F>())
                    {
                        soll.DocumentID = item.N3DOCU;
                        soll.DocumentDate = item.N3DADO;
                    }
                    result.Add(new MastrinoECReportItem()
                    {
                        CompanyID = item.N3SOCI,
                        Year = item.N3ANNO,
                        RowID = item.N3RIGA,
                        DocumentDate = item.N3DADO,
                        DocumentID = item.N3DOCU,
                        RegistrationDate = item.N3DARE,
                        ReferenceDate = item.N3DARI,
                        ReferenceID = item.N3RIFE,
                        Number = item.N3REGI,
                        ExpireDate = item.N3SCAD,
                        CurrencyID = item.N3DIVI,
                        Dare = item.N3SEGN == "D" ? (item.N3IMEU ?? 0) : 0,
                        Avere = item.N3SEGN == "A" ? (item.N3IMEU ?? 0) : 0,
                        CausalFullDescription = item.Causal?.FullDescriptionSearchable,
                        EntityType = "F",
                        EntityID = SupplierID,
                        Reminders = solls,
                        PaymentID = item.N3PAGA,
                        PaymentTypeID = item.n3tipp,
                        LockedInfoText = item.locked.HasValue ? $"Pagamento bloccato da {item.lockedUserID} in data {item.locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.lockedReason?.Trim()}" : null,
                        PNFORNITORI = item,
                        SaldoRiga = item.N3SEGN == "D" ? result.Sum(s => s.Saldo) - (item.N3IMEU ?? 0) : result.Sum(s => s.Saldo) + (item.N3IMEU ?? 0)
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

        public bool SyncPartita(string CompanyID, int SourceYear, int SourceNumber, int SourceRow, int TargetYear, int TargetNumber, int TargetRow)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    var multi = connection.QueryMultiple(
                    @"SELECT * FROM PNFORNITORI WHERE N3SOCI=@cid AND N3ANNO = @sy AND N3REGI = @sn AND N3RIGA = @sr;
                                SELECT * FROM PNFORNITORI WHERE N3SOCI=@cid AND N3ANNO = @ty AND N3REGI = @tn AND N3RIGA = @tr;",
                    new { cid = CompanyID, sy = SourceYear, sn = SourceNumber, sr = SourceRow, ty = TargetYear, tn = TargetNumber, tr = TargetRow }, transaction);
                    var source = multi.Read<PNFORNITORI>().SingleOrDefault();
                    var target = multi.Read<PNFORNITORI>().SingleOrDefault();
                    if (source != null && target != null)
                    {
                        source.N3RIFE = target.N3RIFE;
                        source.N3DARI = target.N3DARI;
                        source.N3SCAD = target.N3SCAD;
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

        public PNFORNITORI? Get(string CompanyID, int Year, int Number, int Row)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PNFORNITORI>(
                    "SELECT * FROM PNFORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi AND N3RIGA = @n3riga",
                    new { n3soci = CompanyID, n3anno = Year, n3regi = Number, n3riga = Row })
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
                var payment = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(PaymentTypeID);

                List<DateTime> result = new List<DateTime> { DocumentDate };

                if (payment != null)
                {
                    if (payment.pfogs1.HasValue && payment.pfogs1.Value > 0)
                    {
                        var newExpire1 = result.First().AddDays(payment.pfogs1.Value);
                        if (payment.pfoppa == "0")
                            result[0] = newExpire1;
                        else
                            result[0] = new DateTime(newExpire1.Year, newExpire1.Month, DateTime.DaysInMonth(newExpire1.Year, newExpire1.Month));

                        if (payment.pfogs2.HasValue && payment.pfogs2.Value > 0)
                        {
                            var newExpire2 = DocumentDate.AddDays(payment.pfogs2.Value);
                            if (payment.pfoppa == "0")
                                result.Add(newExpire2);
                            else
                                result.Add(new DateTime(newExpire2.Year, newExpire2.Month, DateTime.DaysInMonth(newExpire2.Year, newExpire2.Month)));

                            if (payment.pfogs3.HasValue && payment.pfogs3.Value > 0)
                            {
                                var newExpire3 = DocumentDate.AddDays(payment.pfogs3.Value);
                                if (payment.pfoppa == "0")
                                    result.Add(newExpire3);
                                else
                                    result.Add(new DateTime(newExpire3.Year, newExpire3.Month, DateTime.DaysInMonth(newExpire3.Year, newExpire3.Month)));

                                if (payment.pfogs4.HasValue && payment.pfogs4.Value > 0)
                                {
                                    var newExpire4 = DocumentDate.AddDays(payment.pfogs4.Value);
                                    if (payment.pfoppa == "0")
                                        result.Add(newExpire4);
                                    else
                                        result.Add(new DateTime(newExpire4.Year, newExpire4.Month, DateTime.DaysInMonth(newExpire4.Year, newExpire4.Month)));

                                    if (payment.pfogs5.HasValue && payment.pfogs5.Value > 0)
                                    {
                                        var newExpire5 = DocumentDate.AddDays(payment.pfogs5.Value);
                                        if (payment.pfoppa == "0")
                                            result.Add(newExpire5);
                                        else
                                            result.Add(new DateTime(newExpire5.Year, newExpire5.Month, DateTime.DaysInMonth(newExpire5.Year, newExpire5.Month)));

                                        if (payment.pfogs6.HasValue && payment.pfogs6.Value > 0)
                                        {
                                            var newExpire6 = DocumentDate.AddDays(payment.pfogs6.Value);
                                            if (payment.pfoppa == "0")
                                                result.Add(newExpire6);
                                            else
                                                result.Add(new DateTime(newExpire6.Year, newExpire6.Month, DateTime.DaysInMonth(newExpire6.Year, newExpire6.Month)));

                                            if (payment.pfogs7.HasValue && payment.pfogs7.Value > 0)
                                            {
                                                var newExpire7 = DocumentDate.AddDays(payment.pfogs7.Value);
                                                if (payment.pfoppa == "0")
                                                    result.Add(newExpire7);
                                                else
                                                    result.Add(new DateTime(newExpire7.Year, newExpire7.Month, DateTime.DaysInMonth(newExpire7.Year, newExpire7.Month)));

                                                if (payment.pfogs8.HasValue && payment.pfogs8.Value > 0)
                                                {
                                                    var newExpire8 = DocumentDate.AddDays(payment.pfogs8.Value);
                                                    if (payment.pfoppa == "0")
                                                        result.Add(newExpire8);
                                                    else
                                                        result.Add(new DateTime(newExpire8.Year, newExpire8.Month, DateTime.DaysInMonth(newExpire8.Year, newExpire8.Month)));

                                                    if (payment.pfogs9.HasValue && payment.pfogs9.Value > 0)
                                                    {
                                                        var newExpire9 = DocumentDate.AddDays(payment.pfogs9.Value);
                                                        if (payment.pfoppa == "0")
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
                        if (payment.pfoppa != "0")
                            result[0] = new DateTime(DocumentDate.Year, DocumentDate.Month, DateTime.DaysInMonth(DocumentDate.Year, DocumentDate.Month));
                    }
                }

                return result;
            }
            catch { return null; }
        }

        public bool Lock(string CompanyID, int Year, int Number, string UserID, string Reason)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(@"UPDATE PNFORNITORI SET lockedUserID=@uid, lockedReason=@reason, locked=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                        WHERE N3SOCI=@cid AND N3ANNO=@yea AND N3REGI=@id",
                    new { cid = CompanyID, yea = Year, id = Number, uid = UserID, reason = Reason });
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

        public bool Unlock(string CompanyID, int Year, int Number, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(@"UPDATE PNFORNITORI SET lockedUserID=NULL, locked=NULL, unlockedUserID=@uid, unlocked=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                        WHERE N3SOCI=@cid AND N3ANNO=@yea AND N3REGI=@id",
                    new { cid = CompanyID, yea = Year, id = Number, uid = UserID });
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

                string query = $@"SELECT abe.abecod AS EntityID, 'F' AS EntityTypeID, pn.N3SEGN AS Sign, TRIM(abe.abers1) AS EntityDescription, pn.N3SCAD AS ExpireDate, TRIM(pn.N3RIFE) AS ReferenceID,pn.N3DARI AS ReferenceDate,tip.inccod AS PaymentTypeID,TRIM(tip.incdes) AS PaymentTypeDescription, pn.N3IMEU AS Amount, TRIM(cau.caudes) AS CausalDescription, locked AS Locked, lockedUserID AS LockedUserID, lockedReason AS LockedReason FROM PNFORNITORI AS pn
                                        INNER JOIN PNTESTATA AS hea ON hea.N1SOCI=pn.N3SOCI AND hea.N1ANNO=pn.N3ANNO AND hea.N1REGI=pn.N3REGI
                                        INNER JOIN ABE AS abe ON abe.abecod = pn.N3SOTT
                                        INNER JOIN CAUCONT AS cau ON cau.caucod = hea.pncaus
                                        LEFT JOIN PAGFOR AS pag ON pag.pfocod = pn.N3PAGA
                                        LEFT JOIN TAB_ACC_TIPPAG AS tip ON tip.inccod = pag.pfotip
                                        WHERE pn.N3SOCI=@N3soci AND pn.N3PARE IS NULL AND hea.N1TmpPN = 'N'
                                            {(Entity != null ? " AND pn.n3sott=@n3sott" : null)}
                                            {(!string.IsNullOrWhiteSpace(PaymentType.ID) ? " AND pag.pfotip=@pfotip" : null)}
                                            {(ExpireDate.HasValue ? " AND pn.N3SCAD<=@n3scad" : null)}
                                        order by pn.n3sott,pag.pfotip,pn.N3SCAD";


                var list = connection.Query<ExpiresReportItem>(
                    query,
                    new { n3soci = CompanyID, n3sott = Entity?.abecod, n3scad = ExpireDate, pfotip = PaymentType?.ID });

                result.Rows = new List<ExpiresReportMasterItem>();

                if (GroupType == "C")
                {
                    // supplier
                    foreach (var row in list.GroupBy(g => g.EntityID, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = avere - dare;
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
                            var diffI = avereI - dareI;
                            var item = det.details.Where(w => w.PaymentTypeID == det.detkey.PaymentTypeID && w.ReferenceID == det.detkey.ReferenceID && w.ReferenceDate == det.detkey.ReferenceDate && w.ExpireDate == det.detkey.ExpireDate).First();
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                PaymentTypeID = det.details.First().PaymentTypeID,
                                PaymentTypeDescription = det.details.First().PaymentTypeDescription,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : "",
                                LockedInfoText = item.Locked.HasValue ? $"Pagamento bloccato da {item.LockedUserID} in data {item.Locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.LockedReason?.Trim()}" : null
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
                        var diff = avere - dare;
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
                            var diffI = avereI - dareI;
                            var item = det.details.Where(w => w.PaymentTypeID == det.detkey.PaymentTypeID && w.ReferenceID == det.detkey.ReferenceID && w.ReferenceDate == det.detkey.ReferenceDate && w.EntityID == det.detkey.EntityID).First();
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
                                PrintSign = diffI < 0 ? "-" : "",
                                LockedInfoText = item.Locked.HasValue ? $"Pagamento bloccato da {item.LockedUserID} in data {item.Locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.LockedReason?.Trim()}" : null
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
                        var diff = avere - dare;
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
                            var diffI = avereI - dareI;
                            var item = det.details.Where(w => w.EntityID == det.detkey.EntityID && w.ReferenceID == det.detkey.ReferenceID && w.ReferenceDate == det.detkey.ReferenceDate && w.ExpireDate == det.detkey.ExpireDate).First();
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                EntityID = det.details.First().EntityID,
                                EntityDescription = det.details.First().EntityDescription,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : "",
                                LockedInfoText = item.Locked.HasValue ? $"Pagamento bloccato da {item.LockedUserID} in data {item.Locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.LockedReason?.Trim()}" : null
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
                string initial = @$"sselect 
                                    DISTINCT(n3sott) as EntityID, 
                                    MIN(a.abers1) as EntityDescription,
                                    SUM(
                                            CASE 
                                                WHEN  N3SEGN = 'A'
                                                THEN N3IMEU
                                                ELSE 0
                                            END
                                        ) - SUM(
                                            CASE 
                                                WHEN N3SEGN = 'D'
                                                THEN N3IMEU
                                                ELSE 0
                                            END
                                        ) as InitialValue
                                    FROM PNFORNITORI as f
                                    LEFT OUTER JOIN ABE as a ON f.N3SOTT = a.abecod
                                    WHERE f.N3SOCI = @cid AND f.N3DARE < @From
                                    GROUP BY f.n3sott
                                    order by f.n3sott";
                var initialList = connection.Query<BatchReportModel.EntityModel>(
                   initial,
                   new { cid = CompanyID, From = From });

                retValue.AddRange(initialList.Where(o => o.InitialValue != 0));

                // Get the one with movements
                string movement = $@"select 
                                    DISTINCT(n3sott) EntityID, 
                                    MIN(a.abers1) EntityDescription
                                    FROM PNFORNITORI as f
                                    LEFT OUTER JOIN ABE as a ON f.N3SOTT = a.abecod
                                    WHERE N3SOCI = @cid AND N3DARE >= @From AND N3DARE <= @To
                                    GROUP BY n3sott
                                    order by n3sott";
                var movementList = connection.Query<BatchReportModel.EntityModel>(
                  movement,
                  new { cid = CompanyID, From = From, To = To });


                foreach (var ent in movementList.Where(o => retValue.Any(oo => oo.EntityID == o.EntityID)))
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
                                    n3anno as Year,
                                    n3regi as ID,
                                    n3riga as Row,
                                    n3imeu as Import,
                                    n3segn as Sign
                                    FROM PNFORNITORI as f
                                    WHERE N3SOCI = @cid AND N3DARE >= @From AND N3DARE <= @To
                                    order by n3regi";

                    var movementEntitiesList = connection.Query<BatchReportModel.MovementModel>(
                      movementEntities,
                      new { cid = CompanyID, From = From, To = To });

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
        public string INSERT_QUERY => "INSERT INTO PNFORNITORI (N3SOCI,N3ANNO,N3REGI,N3RIGA,N3DARI,N3RIFE,N3DOCU,N3DARE,N3DADO,N3CAUS,N3GRUP,N3CONT,N3SSOC,N3SOTT,N3IMPO,N3DESC,N3SEGN,N3RATA,N3SCAD,N3PAGA,N3PARE,N3PRAT,N3DEST,N3PAXI,N3INIZ,N3CAMB,N3VALU,N3DIVI,n3vcod,N3FLPA,N3DVAL,N3ABIF,N3CABF,N3IMEU,N3TIDO,n3rior,N3FL01,n3tipp,N3VADO,N3DIDO,n3comm,N3CCNF,N3SOCA,N3ABIA,N3CABA,N3CCNA,locked,lockedUserID,lockedReason,unlocked,unlockedUserID) OUTPUT INSERTED.rv VALUES(@N3SOCI,@N3ANNO,@N3REGI,@N3RIGA,@N3DARI,@N3RIFE,@N3DOCU,@N3DARE,@N3DADO,@N3CAUS,@N3GRUP,@N3CONT,@N3SSOC,@N3SOTT,@N3IMPO,@N3DESC,@N3SEGN,@N3RATA,@N3SCAD,@N3PAGA,@N3PARE,@N3PRAT,@N3DEST,@N3PAXI,@N3INIZ,@N3CAMB,@N3VALU,@N3DIVI,@n3vcod,@N3FLPA,@N3DVAL,@N3ABIF,@N3CABF,@N3IMEU,@N3TIDO,@n3rior,@N3FL01,@n3tipp,@N3VADO,@N3DIDO,@n3comm,@N3CCNF,@N3SOCA,@N3ABIA,@N3CABA,@N3CCNA,@locked,@lockedUserID,@lockedReason,@unlocked,@unlockedUserID)";
        public string UPDATE_QUERY => "UPDATE PNFORNITORI SET N3SOCI = @N3SOCI,N3ANNO = @N3ANNO,N3REGI = @N3REGI,N3RIGA = @N3RIGA,N3DARI = @N3DARI,N3RIFE = @N3RIFE,N3DOCU = @N3DOCU,N3DARE = @N3DARE,N3DADO = @N3DADO,N3CAUS = @N3CAUS,N3GRUP = @N3GRUP,N3CONT = @N3CONT,N3SSOC = @N3SSOC,N3SOTT = @N3SOTT,N3IMPO = @N3IMPO,N3DESC = @N3DESC,N3SEGN = @N3SEGN,N3RATA = @N3RATA,N3SCAD = @N3SCAD,N3PAGA = @N3PAGA,N3PARE = @N3PARE,N3PRAT = @N3PRAT,N3DEST = @N3DEST,N3PAXI = @N3PAXI,N3INIZ = @N3INIZ,N3CAMB = @N3CAMB,N3VALU = @N3VALU,N3DIVI = @N3DIVI,n3vcod = @n3vcod,N3FLPA = @N3FLPA,N3DVAL = @N3DVAL,N3ABIF = @N3ABIF,N3CABF = @N3CABF,N3IMEU = @N3IMEU,N3TIDO = @N3TIDO,n3rior = @n3rior,N3FL01 = @N3FL01,n3tipp = @n3tipp,N3VADO = @N3VADO,N3DIDO = @N3DIDO,n3comm = @n3comm,N3CCNF = @N3CCNF,N3SOCA = @N3SOCA,N3ABIA = @N3ABIA,N3CABA = @N3CABA,N3CCNA = @N3CCNA,locked = @locked,lockedUserID = @lockedUserID,lockedReason = @lockedReason,unlocked = @unlocked,unlockedUserID = @unlockedUserID OUTPUT INSERTED.rv WHERE N3SOCI = @N3SOCI AND N3ANNO = @N3ANNO AND N3REGI = @N3REGI AND N3RIGA = @N3RIGA AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PNFORNITORI OUTPUT DELETED.rv WHERE N3SOCI = @N3SOCI AND N3ANNO = @N3ANNO AND N3REGI = @N3REGI AND N3RIGA = @N3RIGA AND rv = @rv";
        public bool Insert(PNFORNITORI Model)
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

        public bool Update(PNFORNITORI Model)
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

        public bool DeleteSingle(PNFORNITORI Model)
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
                    "DELETE FROM PNFORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi",
                    new { n3soci = CompanyID, n3anno = Year, n3regi = Number });
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

        public string? Validate(PNFORNITORI Model, bool IsInsert)
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

    public class PNFORNITORIUfpRepository : RepositoryBase, IPNFORNITORIRepository
    {
        public PNFORNITORIUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public Tuple<bool, decimal>? GetSaldoRiferimento(string CompanyID, int CustomerID, string ReferenceID, DateTime ReferenceDate)
        {
            try
            {
                using var connection = GetOpenConnection();

                var multi = connection.QueryMultiple(
                    @"SELECT SUM(N3IMEU) AS Dare FROM PN_FORNITORI WHERE N3SOCI=@cid AND N3SEGN = 'D' AND N3SOTT = @cust AND N3RIFE = @rifid AND N3DARI = @rifdate;
                          SELECT SUM(N3IMEU) AS Avere FROM PN_FORNITORI WHERE N3SOCI=@cid AND N3SEGN = 'A' AND N3SOTT = @cust AND N3RIFE = @rifid AND N3DARI = @rifdate;",
                    new { cid = CompanyID, cust = CustomerID, rifid = ReferenceID, rifdate = ReferenceDate });

                var dare = multi.Read<decimal>().Single();
                var avere = multi.Read<decimal>().Single();

                return dare >= avere ? new Tuple<bool, decimal>(false, dare - avere) : new Tuple<bool, decimal>(true, avere - dare);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNFORNITORI>? GetList(string CompanyID, int Year, int Number, int RowID, int SupplierID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNFORNITORI>(
                    @"SELECT * FROM PN_FORNITORI 
                        WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi AND N3SOTT = @n3sott AND (n3rior = @n3rior OR n3rior is null OR n3rior = 0)
                        ORDER BY N3RIGA",
                    new { n3soci = CompanyID, n3anno = Year, n3regi = Number, n3sott = SupplierID, n3rior = RowID });

                return new ObservableCollection<PNFORNITORI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNFORNITORI>? GetListByReg(string CompanyID, int Year, int Number)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PNFORNITORI>(
                    @"SELECT p.N3SCAD, p.N3RIFE, p.N3DARI, p.N3IMEU, p.N3SEGN, CONCAT(a.abecod , ' ' , TRIM(a.abers1)) AS SupplierDescription FROM PN_FORNITORI AS p
                        INNER JOIN ABE AS a ON a.abecod = p.N3SOTT
                        WHERE p.N3SOCI = @cid AND p.N3ANNO = @yea AND p.N3REGI = @reg",
                    new { cid = CompanyID, yea = Year, reg = Number });

                return new ObservableCollection<PNFORNITORI>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<MastrinoECReportItem>? GetECList(string CompanyID, int SupplierID, DateTime ToDate, bool HasDrawn, DateTime SinceDrawn, bool ExcludeInPaymentExecution = false)
        {
            try
            {
                using var connection = GetOpenConnection();
                ObservableCollection<MastrinoECReportItem> result = new ObservableCollection<MastrinoECReportItem>();

                string queryExcludePaymentExecution = @"AND NOT EXISTS (
                                                              SELECT 1
                                                              FROM MPDETTAGLIO e
                                                              WHERE e.M3SOCI = m.N3SOCI AND e.M3ANNO = m.N3ANNO AND e.m3REGI = m.N3REGI AND e.M3RIGA = m.N3RIGA
                                                          )";

                string? query = null;
                if (!HasDrawn)
                    query = $@"SELECT m.*, c.caucod, c.caudes FROM PN_FORNITORI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.N3caus
                        WHERE m.N3SOCI = @N3soci AND m.N3SOTT = @N3sott AND m.N3DARE <= @N3dare AND (m.N3PARE IS NULL OR m.n3PARE = '') {(ExcludeInPaymentExecution ? queryExcludePaymentExecution : null)}
                        ORDER BY m.N3DARI, m.N3RIFE";
                else
                    query = $@"SELECT m.*, c.caucod, c.caudes FROM PN_FORNITORI AS m
                        INNER JOIN CAUCONT AS c ON c.caucod = m.N3caus
                        WHERE m.N3SOCI = @N3soci AND m.N3SOTT = @N3sott AND m.N3DARE <= @N3dare AND ((m.N3PARE IS NULL OR m.n3PARE = '') OR (m.N3PARE = '*' AND m.N3DARE >= @dapare))  {(ExcludeInPaymentExecution ? queryExcludePaymentExecution : null)}
                        ORDER BY m.N3DARI, m.N3RIFE";

                var list = connection.Query<PNFORNITORI, CAUCONT, PNFORNITORI>(
                    query,
                    (pn, cau) => { pn.Causal = cau; return pn; },
                    new { N3soci = CompanyID, N3sott = SupplierID, N3dare = ToDate, dapare = SinceDrawn },
                    splitOn: "caucod");

                foreach (var item in list)
                {
                    // OPTIMIZE
                    //var solls = (item.N3SOTT.HasValue && !string.IsNullOrEmpty(item.N3RIFE)) ? VulpesServiceProvider.Provider.GetRequiredService<ISOLLE0FRepository>().GetList(item.N3SOCI, item.N3SOTT.Value, item.N3RIFE) : null;
                    //foreach (var soll in solls ?? new ObservableCollection<SOLLE0F>())
                    //{
                    //    soll.DocumentID = item.N3DOCU;
                    //    soll.DocumentDate = item.N3DADO;
                    //}
                    result.Add(new MastrinoECReportItem()
                    {
                        CompanyID = item.N3SOCI,
                        Year = item.N3ANNO,
                        RowID = item.N3RIGA,
                        DocumentDate = item.N3DADO,
                        DocumentID = item.N3DOCU,
                        RegistrationDate = item.N3DARE,
                        ReferenceDate = item.N3DARI,
                        ReferenceID = item.N3RIFE,
                        Number = item.N3REGI,
                        ExpireDate = item.N3SCAD,
                        CurrencyID = item.N3DIVI,
                        Dare = item.N3SEGN == "D" ? (item.N3IMEU ?? 0) : 0,
                        Avere = item.N3SEGN == "A" ? (item.N3IMEU ?? 0) : 0,
                        CausalFullDescription = item.Causal?.FullDescriptionSearchable,
                        EntityType = "F",
                        EntityID = SupplierID,
                        Reminders = null,
                        PaymentID = item.N3PAGA,
                        PaymentTypeID = item.n3tipp,
                        LockedInfoText = item.locked.HasValue ? $"Pagamento bloccato da {item.lockedUserID} in data {item.locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.lockedReason?.Trim()}" : null,
                        PNFORNITORI = item,
                        Cambio = item.N3CAMB,
                        ValoreValuta = item.N3VALU,
                        Valuta = item.N3DIDO,
                        SaldoRiga = item.N3SEGN == "D" ? result.Sum(s=>s.Saldo) - (item.N3IMEU ?? 0) : result.Sum(s => s.Saldo) + (item.N3IMEU ?? 0)
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

        public bool SyncPartita(string CompanyID, int SourceYear, int SourceNumber, int SourceRow, int TargetYear, int TargetNumber, int TargetRow)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    var multi = connection.QueryMultiple(
                    @"SELECT * FROM PN_FORNITORI WHERE N3SOCI=@cid AND N3ANNO = @sy AND N3REGI = @sn AND N3RIGA = @sr;
                                SELECT * FROM PN_FORNITORI WHERE N3SOCI=@cid AND N3ANNO = @ty AND N3REGI = @tn AND N3RIGA = @tr;",
                    new { cid = CompanyID, sy = SourceYear, sn = SourceNumber, sr = SourceRow, ty = TargetYear, tn = TargetNumber, tr = TargetRow }, transaction);
                    var source = multi.Read<PNFORNITORI>().SingleOrDefault();
                    var target = multi.Read<PNFORNITORI>().SingleOrDefault();
                    if (source != null && target != null)
                    {
                        source.N3RIFE = target.N3RIFE;
                        source.N3DARI = target.N3DARI;
                        source.N3SCAD = target.N3SCAD;
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

        public PNFORNITORI? Get(string CompanyID, int Year, int Number, int Row)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<PNFORNITORI>(
                    "SELECT * FROM PN_FORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi AND N3RIGA = @n3riga",
                    new { n3soci = CompanyID, n3anno = Year, n3regi = Number, n3riga = Row })
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
                var payment = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(PaymentTypeID);

                List<DateTime> result = new List<DateTime> { DocumentDate };

                if (payment != null)
                {
                    if (payment.pfogs1.HasValue && payment.pfogs1.Value > 0)
                    {
                        var newExpire1 = result.First().AddDays(payment.pfogs1.Value);
                        if (payment.pfoppa == "0")
                            result[0] = newExpire1;
                        else
                            result[0] = new DateTime(newExpire1.Year, newExpire1.Month, DateTime.DaysInMonth(newExpire1.Year, newExpire1.Month));

                        if (payment.pfogs2.HasValue && payment.pfogs2.Value > 0)
                        {
                            var newExpire2 = DocumentDate.AddDays(payment.pfogs2.Value);
                            if (payment.pfoppa == "0")
                                result.Add(newExpire2);
                            else
                                result.Add(new DateTime(newExpire2.Year, newExpire2.Month, DateTime.DaysInMonth(newExpire2.Year, newExpire2.Month)));

                            if (payment.pfogs3.HasValue && payment.pfogs3.Value > 0)
                            {
                                var newExpire3 = DocumentDate.AddDays(payment.pfogs3.Value);
                                if (payment.pfoppa == "0")
                                    result.Add(newExpire3);
                                else
                                    result.Add(new DateTime(newExpire3.Year, newExpire3.Month, DateTime.DaysInMonth(newExpire3.Year, newExpire3.Month)));

                                if (payment.pfogs4.HasValue && payment.pfogs4.Value > 0)
                                {
                                    var newExpire4 = DocumentDate.AddDays(payment.pfogs4.Value);
                                    if (payment.pfoppa == "0")
                                        result.Add(newExpire4);
                                    else
                                        result.Add(new DateTime(newExpire4.Year, newExpire4.Month, DateTime.DaysInMonth(newExpire4.Year, newExpire4.Month)));

                                    if (payment.pfogs5.HasValue && payment.pfogs5.Value > 0)
                                    {
                                        var newExpire5 = DocumentDate.AddDays(payment.pfogs5.Value);
                                        if (payment.pfoppa == "0")
                                            result.Add(newExpire5);
                                        else
                                            result.Add(new DateTime(newExpire5.Year, newExpire5.Month, DateTime.DaysInMonth(newExpire5.Year, newExpire5.Month)));

                                        if (payment.pfogs6.HasValue && payment.pfogs6.Value > 0)
                                        {
                                            var newExpire6 = DocumentDate.AddDays(payment.pfogs6.Value);
                                            if (payment.pfoppa == "0")
                                                result.Add(newExpire6);
                                            else
                                                result.Add(new DateTime(newExpire6.Year, newExpire6.Month, DateTime.DaysInMonth(newExpire6.Year, newExpire6.Month)));

                                            if (payment.pfogs7.HasValue && payment.pfogs7.Value > 0)
                                            {
                                                var newExpire7 = DocumentDate.AddDays(payment.pfogs7.Value);
                                                if (payment.pfoppa == "0")
                                                    result.Add(newExpire7);
                                                else
                                                    result.Add(new DateTime(newExpire7.Year, newExpire7.Month, DateTime.DaysInMonth(newExpire7.Year, newExpire7.Month)));

                                                if (payment.pfogs8.HasValue && payment.pfogs8.Value > 0)
                                                {
                                                    var newExpire8 = DocumentDate.AddDays(payment.pfogs8.Value);
                                                    if (payment.pfoppa == "0")
                                                        result.Add(newExpire8);
                                                    else
                                                        result.Add(new DateTime(newExpire8.Year, newExpire8.Month, DateTime.DaysInMonth(newExpire8.Year, newExpire8.Month)));

                                                    if (payment.pfogs9.HasValue && payment.pfogs9.Value > 0)
                                                    {
                                                        var newExpire9 = DocumentDate.AddDays(payment.pfogs9.Value);
                                                        if (payment.pfoppa == "0")
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
                        if (payment.pfoppa != "0")
                            result[0] = new DateTime(DocumentDate.Year, DocumentDate.Month, DateTime.DaysInMonth(DocumentDate.Year, DocumentDate.Month));
                    }
                }

                return result;
            }
            catch { return null; }
        }

        public bool Lock(string CompanyID, int Year, int Number, string UserID, string Reason)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(@"UPDATE PN_FORNITORI SET lockedUserID=@uid, lockedReason=@reason, locked=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                        WHERE N3SOCI=@cid AND N3ANNO=@yea AND N3REGI=@id",
                    new { cid = CompanyID, yea = Year, id = Number, uid = UserID, reason = Reason });
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

        public bool Unlock(string CompanyID, int Year, int Number, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = connection.Execute(@"UPDATE PN_FORNITORI SET lockedUserID=NULL, locked=NULL, unlockedUserID=@uid, unlocked=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                        WHERE N3SOCI=@cid AND N3ANNO=@yea AND N3REGI=@id",
                    new { cid = CompanyID, yea = Year, id = Number, uid = UserID });
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

                string query = $@"SELECT abe.abecod AS EntityID, 'F' AS EntityTypeID, pn.N3SEGN AS Sign, TRIM(abe.abers1) AS EntityDescription, pn.N3SCAD AS ExpireDate, TRIM(pn.N3RIFE) AS ReferenceID,pn.N3DARI AS ReferenceDate,tip.inccod AS PaymentTypeID,TRIM(tip.incdes) AS PaymentTypeDescription, pn.N3IMEU AS Amount, TRIM(cau.caudes) AS CausalDescription, locked AS Locked, lockedUserID AS LockedUserID, lockedReason AS LockedReason FROM PN_FORNITORI AS pn
                                        INNER JOIN PN_TESTATA AS hea ON hea.N1SOCI=pn.N3SOCI AND hea.N1ANNO=pn.N3ANNO AND hea.N1REGI=pn.N3REGI
                                        INNER JOIN ANAG_BASE AS abe ON abe.abecod = pn.N3SOTT
                                        INNER JOIN CAUCONT AS cau ON cau.caucod = hea.pncaus
                                        LEFT JOIN PAGFOR AS pag ON pag.pfocod = pn.N3PAGA
                                        LEFT JOIN PAGAMENTI AS tip ON tip.inccod = pag.pfotip
                                        WHERE pn.N3SOCI=@N3soci AND pn.N3PARE IS NULL AND hea.N1TmpPN = 'N'
                                            {(Entity != null ? " AND pn.n3sott=@n3sott" : null)}
                                            {(!string.IsNullOrWhiteSpace(PaymentType.ID) ? " AND pag.pfotip=@pfotip" : null)}
                                            {(ExpireDate.HasValue ? " AND pn.N3SCAD<=@n3scad" : null)}
                                        order by pn.n3sott,pag.pfotip,pn.N3SCAD";


                var list = connection.Query<ExpiresReportItem>(
                    query,
                    new { n3soci = CompanyID, n3sott = Entity?.abecod, n3scad = ExpireDate, pfotip = PaymentType?.ID });

                result.Rows = new List<ExpiresReportMasterItem>();

                if (GroupType == "C")
                {
                    // supplier
                    foreach (var row in list.GroupBy(g => g.EntityID, (key, items) => new { key, items }))
                    {
                        var dare = row.items.Where(w => w.Sign == "D").Sum(sum => sum.Amount);
                        var avere = row.items.Where(w => w.Sign == "A").Sum(sum => sum.Amount);
                        var diff = avere - dare;
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
                            var diffI = avereI - dareI;
                            var item = det.details.Where(w => w.PaymentTypeID == det.detkey.PaymentTypeID && w.ReferenceID == det.detkey.ReferenceID && w.ReferenceDate == det.detkey.ReferenceDate && w.ExpireDate == det.detkey.ExpireDate).First();
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                PaymentTypeID = det.details.First().PaymentTypeID,
                                PaymentTypeDescription = det.details.First().PaymentTypeDescription,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : "",
                                LockedInfoText = item.Locked.HasValue ? $"Pagamento bloccato da {item.LockedUserID} in data {item.Locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.LockedReason?.Trim()}" : null
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
                        var diff = avere - dare;
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
                            var diffI = avereI - dareI;
                            var item = det.details.Where(w => w.PaymentTypeID == det.detkey.PaymentTypeID && w.ReferenceID == det.detkey.ReferenceID && w.ReferenceDate == det.detkey.ReferenceDate && w.EntityID == det.detkey.EntityID).First();
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
                                PrintSign = diffI < 0 ? "-" : "",
                                LockedInfoText = item.Locked.HasValue ? $"Pagamento bloccato da {item.LockedUserID} in data {item.Locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.LockedReason?.Trim()}" : null
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
                        var diff = avere - dare;
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
                            var diffI = avereI - dareI;
                            var item = det.details.Where(w => w.EntityID == det.detkey.EntityID && w.ReferenceID == det.detkey.ReferenceID && w.ReferenceDate == det.detkey.ReferenceDate && w.ExpireDate == det.detkey.ExpireDate).First();
                            newMaster.Rows.Add(new ExpiresReportItem()
                            {
                                ExpireDate = det.details.First().ExpireDate,
                                EntityID = det.details.First().EntityID,
                                EntityDescription = det.details.First().EntityDescription,
                                ReferenceID = det.details.First().ReferenceID,
                                ReferenceDate = det.details.First().ReferenceDate,
                                CausalDescription = det.details.All(all => all.CausalDescription == det.details.First().CausalDescription) ? det.details.First().CausalDescription : null,
                                Amount = diffI < 0 ? diffI * -1 : diffI,
                                PrintSign = diffI < 0 ? "-" : "",
                                LockedInfoText = item.Locked.HasValue ? $"Pagamento bloccato da {item.LockedUserID} in data {item.Locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {item.LockedReason?.Trim()}" : null
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
                                    DISTINCT(n3sott) as EntityID, 
                                    MIN(a.abers1) as EntityDescription,
                                    SUM(
                                            CASE 
                                                WHEN  N3SEGN = 'A'
                                                THEN N3IMEU
                                                ELSE 0
                                            END
                                        ) - SUM(
                                            CASE 
                                                WHEN N3SEGN = 'D'
                                                THEN N3IMEU
                                                ELSE 0
                                            END
                                        ) as InitialValue
                                    FROM PN_FORNITORI as f
                                    LEFT OUTER JOIN ANAG_BASE as a ON f.N3SOTT = a.abecod
                                    WHERE f.N3SOCI = @cid AND f.N3DARE < @From
                                    GROUP BY f.n3sott
                                    order by f.n3sott";
                var initialList = connection.Query<BatchReportModel.EntityModel>(
                   initial,
                   new { cid = CompanyID, From = From });

                retValue.AddRange(initialList.Where(o => o.InitialValue != 0));

                // Get the one with movements
                string movement = $@"select 
                                    DISTINCT(n3sott) EntityID, 
                                    MIN(a.abers1) EntityDescription
                                    FROM PN_FORNITORI as f
                                    LEFT OUTER JOIN ANAG_BASE as a ON f.N3SOTT = a.abecod
                                    WHERE N3SOCI = @cid AND N3DARE >= @From AND N3DARE <= @To
                                    GROUP BY n3sott
                                    order by n3sott";
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
                                    n3anno as Year,
                                    n3regi as ID,
                                    n3riga as Row,
                                    n3imeu as Import,
                                    n3segn as Sign,
                                    n3dare as Date,
                                    n3dari as ReferenceDate,
                                    n3rife as ReferenceID,
                                    n3docu as DocumentID,
                                    n3dado as DocumentDate
                                    FROM PN_FORNITORI as f
                                    WHERE N3SOCI = @cid AND N3DARE >= @From AND N3DARE <= @To AND N3SOTT = @eid
                                    order by n3dare";

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
        public string INSERT_QUERY => "INSERT INTO PN_FORNITORI (N3SOCI,N3ANNO,N3REGI,N3RIGA,N3DARI,N3RIFE,N3DOCU,N3DARE,N3DADO,N3CAUS,N3GRUP,N3CONT,N3SSOC,N3SOTT,N3IMPO,N3DESC,N3SEGN,N3RATA,N3SCAD,N3PAGA,N3PARE,N3PRAT,N3DEST,N3PAXI,N3INIZ,N3CAMB,N3VALU,N3DIVI,n3vcod,N3FLPA,N3DVAL,N3ABIF,N3CABF,N3IMEU,N3TIDO,n3rior,N3FL01,n3tipp,N3VADO,N3DIDO,n3comm,N3CCNF,N3SOCA,N3ABIA,N3CABA,N3CCNA,locked,lockedUserID,lockedReason,unlocked,unlockedUserID) OUTPUT INSERTED.rv VALUES(@N3SOCI,@N3ANNO,@N3REGI,@N3RIGA,@N3DARI,@N3RIFE,@N3DOCU,@N3DARE,@N3DADO,@N3CAUS,@N3GRUP,@N3CONT,@N3SSOC,@N3SOTT,@N3IMPO,@N3DESC,@N3SEGN,@N3RATA,@N3SCAD,@N3PAGA,@N3PARE,@N3PRAT,@N3DEST,@N3PAXI,@N3INIZ,@N3CAMB,@N3VALU,@N3DIVI,@n3vcod,@N3FLPA,@N3DVAL,@N3ABIF,@N3CABF,@N3IMEU,@N3TIDO,@n3rior,@N3FL01,@n3tipp,@N3VADO,@N3DIDO,@n3comm,@N3CCNF,@N3SOCA,@N3ABIA,@N3CABA,@N3CCNA,@locked,@lockedUserID,@lockedReason,@unlocked,@unlockedUserID)";
        public string UPDATE_QUERY => "UPDATE PN_FORNITORI SET N3SOCI = @N3SOCI,N3ANNO = @N3ANNO,N3REGI = @N3REGI,N3RIGA = @N3RIGA,N3DARI = @N3DARI,N3RIFE = @N3RIFE,N3DOCU = @N3DOCU,N3DARE = @N3DARE,N3DADO = @N3DADO,N3CAUS = @N3CAUS,N3GRUP = @N3GRUP,N3CONT = @N3CONT,N3SSOC = @N3SSOC,N3SOTT = @N3SOTT,N3IMPO = @N3IMPO,N3DESC = @N3DESC,N3SEGN = @N3SEGN,N3RATA = @N3RATA,N3SCAD = @N3SCAD,N3PAGA = @N3PAGA,N3PARE = @N3PARE,N3PRAT = @N3PRAT,N3DEST = @N3DEST,N3PAXI = @N3PAXI,N3INIZ = @N3INIZ,N3CAMB = @N3CAMB,N3VALU = @N3VALU,N3DIVI = @N3DIVI,n3vcod = @n3vcod,N3FLPA = @N3FLPA,N3DVAL = @N3DVAL,N3ABIF = @N3ABIF,N3CABF = @N3CABF,N3IMEU = @N3IMEU,N3TIDO = @N3TIDO,n3rior = @n3rior,N3FL01 = @N3FL01,n3tipp = @n3tipp,N3VADO = @N3VADO,N3DIDO = @N3DIDO,n3comm = @n3comm,N3CCNF = @N3CCNF,N3SOCA = @N3SOCA,N3ABIA = @N3ABIA,N3CABA = @N3CABA,N3CCNA = @N3CCNA,locked = @locked,lockedUserID = @lockedUserID,lockedReason = @lockedReason,unlocked = @unlocked,unlockedUserID = @unlockedUserID OUTPUT INSERTED.rv WHERE N3SOCI = @N3SOCI AND N3ANNO = @N3ANNO AND N3REGI = @N3REGI AND N3RIGA = @N3RIGA AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM PN_FORNITORI OUTPUT DELETED.rv WHERE N3SOCI = @N3SOCI AND N3ANNO = @N3ANNO AND N3REGI = @N3REGI AND N3RIGA = @N3RIGA AND rv = @rv";
        public bool Insert(PNFORNITORI Model)
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

        public bool Update(PNFORNITORI Model)
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

        public bool DeleteSingle(PNFORNITORI Model)
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
                    "DELETE FROM PNFORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi",
                    new { n3soci = CompanyID, n3anno = Year, n3regi = Number });
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

        public string? Validate(PNFORNITORI Model, bool IsInsert)
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

}
