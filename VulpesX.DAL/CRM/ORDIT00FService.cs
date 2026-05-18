using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Production;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Reports.CRM;
using VulpesX.Services.Tables.CRM;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.CRM;

public interface IORDIT00FRepository
{
    ObservableCollection<ORDIT00F>? GetList(string CompanyID, int Year, string Status);

    ObservableCollection<ORDIT00F>? GetList(string CompanyID, int Year, string Status, int CustomerID);

    ObservableCollection<ORDID00F>? GetFulfillmentList(string CompanyID);

    void FlagFulfillment(string otsoci, int OTANNO, int OTNUOR, string UserID);

    ORDIT00F? Get(string otsoci, int OTANNO, int OTNUOR);

    ORDIT00F? GetFull(string otsoci, int OTANNO, int OTNUOR);

    bool Exists(string otsoci, int OTANNO, int OTNUOR);

    long GetTotalOrdersCount(string CompanyID, int? Year, int CustomerID);

    decimal GetOrdersAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate, bool OnlyNew);

    List<int>? GetDistinctYearsExisting(string CompanyID, int CustomerID);

    #region CRM
    bool GenerateByOffer(OFFET00F OfferHead, List<OFFED00F> SelectedRows, List<OFFETAL00F> SelectedAttachments, string UserID, bool HasAllSigns, Guid CompanyUID);
    #endregion

    #region Production
    bool GenerateProductionOrder(ORDIT00F Order, string UserID, long? ExistingOrderID, string? RevisionID, bool IsLinked);

    void UnlockOrders(string SocietaID, ObservableCollection<acq_orders_rows_customer_orders> CustomerOrders, string UserID);
    #endregion

    #region Printing
    ORDIT00F? GetPrintFull(string otsoci, int OTANNO, int OTNUOR, bool UseCustomerCodes, bool PrintProductNote, bool PrintAgentsInDetails);
    OrderReport? PrintOrder(ORDIT00F Order);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ORDIT00F Model);

    bool Update(ORDIT00F Model);

    bool Delete(ORDIT00F Model);

    string? Validate(ORDIT00F Model, bool IsInsert);
    #endregion
}

public class ORDIT00FRepository : RepositoryBase, IORDIT00FRepository
{
    public ORDIT00FRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public const string ERR_CANNOT_DELETE = "Impossibile eliminare un ordine chiuso, trasformato in ordini di produzione o gia' annullato";

    public ObservableCollection<ORDIT00F>? GetList(string CompanyID, int Year, string Status)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ORDIT00F, TAB_CRM_CAUORD, ORDIT00F>(
                $@"SELECT o.otsoci,o.OTANNO,o.OTNUOR,o.OTDAOR,o.OTCAUS,o.OTCLIE,o.DESTIN,o.flgchi,o.OTDARI,o.OTDATC,o.OTDAPA,o.canceled, o.OTINFI, o.OTINFIUSR, o.OTFICO, o.OTFICOUSR, o.OTFITE, o.OTFITEUSR, 
                            (SELECT COUNT(*) FROM (SELECT DISTINCT ftsoci, ftanno, ftnuor, otann1, otnuo1 FROM FATTD00F WHERE ftsoci = o.otsoci AND OTANN1 = o.OTANNO AND OTNUO1 = o.OTNUOR) AS tt) AS InvoicesCount,
                            (SELECT COUNT(*) FROM (SELECT DISTINCT bolsoc, BTANNO, BTBOLL, BOANNO, BONUOR FROM BOLLD00F WHERE bolsoc = o.otsoci AND BOANNO = o.OTANNO AND BONUOR = o.OTNUOR) AS tt) AS DDTCount,
                            (SELECT COUNT(*) FROM ORDITALE WHERE otsoci=o.otsoci AND otanno=o.OTANNO AND otnuor=o.OTNUOR AND OTAATTI=1) AS ActiveAlerts,
                            (SELECT COUNT(*) FROM ORDITALE WHERE otsoci=o.otsoci AND otanno=o.OTANNO AND otnuor=o.OTNUOR) AS TotalAlerts,
                            (SELECT COUNT(*) FROM pro_ordine WHERE SocietaID=o.otsoci AND OrdineClienteAnno=o.OTANNO AND OrdineClienteID=o.OTNUOR AND LogCanceled IS NULL) AS ProductionOrdersGenerated,
                            CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS CustomerFullDescriptionSearchable,
                            CONCAT(d.codesti, ' ', TRIM(d.ragisoc)) AS RecipientFullDescriptionSearchable,
                        ca.* FROM ORDIT00F AS o
                        INNER JOIN ABE AS c ON c.abecod = o.OTCLIE
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OTCLIE AND d.codesti = o.DESTIN
                        INNER JOIN TAB_CRM_CAUORD AS ca ON ca.cauacqsoc = o.otsoci AND ca.cauacq = o.OTCAUS
                        WHERE o.otsoci = @otsoci AND o.OTANNO = @yea AND
                        {(Status == "X" ? " o.canceled IS NOT NULL " : $" flgchi {(Status == "*" ? " <> @flgchi" : " = @flgchi AND o.canceled IS NULL")}")}
                        ORDER BY o.OTNUOR DESC",

                (ord, cau) => { ord.Causal = cau; return ord; },
                new { otsoci = CompanyID, yea = Year, flgchi = Status }, splitOn: "cauacq");
            var rows = new ObservableCollection<ORDID00F>(connection.Query<ORDID00F, tab_articolo, ORDID00F>(
                    @"SELECT otsoci, OTNUOR,ODTPRE,ODPREZ,ODQTAV,ODCODA, ODSCO1, ODSCO2, ODSCO3, ODMAGG, ODTSC1,ODTSC2,ODTSC3,ODTMAG,ODSTA,ODTQTA, odunit, art.ID, art.UnitaID, art.QuantitaDefault FROM ORDID00F
                            INNER JOIN tab_articolo AS art ON art.SocietaID=otsoci AND art.ID = ODCODA
                            WHERE otsoci = @otsoci AND OTANNO = @OTANNO",
                    (row, prd) => { row.Product = new tab_articolo() { SocietaID = CompanyID, Descrizione = string.Empty, ID = string.Empty, TipoID = string.Empty, UnitaID = prd.UnitaID, QuantitaDefault = prd.QuantitaDefault }; return row; },
                    new { otsoci = CompanyID, OTANNO = Year }, splitOn: "ID").ToList());
            var attachments = new ObservableCollection<ORDITAL00F>(connection.Query<ORDITAL00F>(
                    @"SELECT * FROM ORDITAL00F
                            WHERE otasoci = @OTASOCI AND OTAANNO = @OTAANNO
                            ORDER BY OTANAME",
                    new { OTASOCI = CompanyID, OTAANNO = Year }).ToList());
            Parallel.ForEach(list, (head) =>
            {
                head.Rows = new ObservableCollection<ORDID00F>(rows.Where(w => w.OTNUOR == head.OTNUOR).ToList());
                head.Attachments = new ObservableCollection<ORDITAL00F>(attachments.Where(w => w.OTANUOR == head.OTNUOR).ToList());
                // compute production orders needed
                head.ProductionOrdersNeeded = head.Rows.Where(w => w.ODSTA == "*" || string.IsNullOrWhiteSpace(w.ODSTA)).Count();
                head.ProductionOrdersGenerated = (int?)connection.ExecuteScalar("SELECT COUNT(*) FROM pro_ordine WHERE SocietaID=@cid AND OrdineClienteAnno=@yea AND OrdineClienteID=@num", new { cid = CompanyID, yea = head.OTANNO, num = head.OTNUOR }) ?? 0;
            });
            return new ObservableCollection<ORDIT00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ORDIT00F>? GetList(string CompanyID, int Year, string Status, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ORDIT00F, TAB_CRM_CAUORD, ORDIT00F>(
                $@"SELECT o.otsoci,o.OTANNO,o.OTNUOR,o.OTDAOR,o.OTCAUS,o.OTCLIE,o.DESTIN,o.flgchi,o.OTDARI,o.OTDATC,o.OTDAPA,o.canceled, o.OTINFI, o.OTINFIUSR, o.OTFICO, o.OTFICOUSR, o.OTFITE, o.OTFITEUSR, 
                            (SELECT COUNT(*) FROM (SELECT DISTINCT ftanno, ftnuor, otann1, otnuo1 FROM FATTD00F WHERE ftsoci = o.otsoci AND OTANN1 = o.OTANNO AND OTNUO1 = o.OTNUOR) AS tt) AS InvoicesCount,
                            (SELECT COUNT(*) FROM (SELECT DISTINCT BTANNO, BTBOLL, BOANNO, BONUOR FROM BOLLD00F WHERE bolsoc = o.otsoci AND BOANNO = o.OTANNO AND BONUOR = o.OTNUOR) AS tt) AS DDTCount,
                            (SELECT COUNT(*) FROM ORDITALE WHERE otsoci=o.otsoci AND otanno=o.OTANNO AND otnuor=o.OTNUOR AND OTAATTI=1) AS ActiveAlerts,
                            (SELECT COUNT(*) FROM ORDITALE WHERE otsoci=o.otsoci AND otanno=o.OTANNO AND otnuor=o.OTNUOR) AS TotalAlerts,
                            (SELECT COUNT(*) FROM pro_ordine WHERE SocietaID=o.otsoci AND OrdineClienteAnno=o.OTANNO AND OrdineClienteID=o.OTNUOR AND LogCanceled IS NULL) AS ProductionOrdersGenerated,
                            CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS CustomerFullDescriptionSearchable,
                            CONCAT(d.codesti, ' ', TRIM(d.ragisoc)) AS RecipientFullDescriptionSearchable,
                        ca.* FROM ORDIT00F AS o
                        INNER JOIN ABE AS c ON c.abecod = o.OTCLIE
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OTCLIE AND d.codesti = o.DESTIN
                        INNER JOIN TAB_CRM_CAUORD AS ca ON ca.cauacqsoc = o.otsoci AND ca.cauacq = o.OTCAUS
                        WHERE o.otsoci = @otsoci AND o.OTANNO = @yea AND o.OTCLIE = @cid AND
                        {(Status == "X" ? " o.canceled IS NOT NULL " : $" flgchi {(Status == "*" ? " <> @flgchi" : " = @flgchi AND o.canceled IS NULL")}")}
                        ORDER BY o.OTNUOR DESC",

                (ord, cau) => { ord.Causal = cau; return ord; },
                new { otsoci = CompanyID, yea = Year, flgchi = Status, cid = CustomerID }, splitOn: "cauacq");
            var rows = new ObservableCollection<ORDID00F>(connection.Query<ORDID00F, tab_articolo, ORDID00F>(
                    @"SELECT otsoci, OTNUOR,ODTPRE,ODPREZ,ODQTAV,ODCODA, ODSCO1, ODSCO2, ODSCO3, ODMAGG, ODTSC1,ODTSC2,ODTSC3,ODTMAG,ODSTA,ODTQTA, odunit, art.ID, art.UnitaID, art.QuantitaDefault FROM ORDID00F
                            INNER JOIN tab_articolo AS art ON art.SocietaID=otsoci AND art.ID = ODCODA
                            WHERE otsoci = @otsoci AND OTANNO = @OTANNO",
                    (row, prd) => { row.Product = new tab_articolo() { SocietaID = CompanyID, Descrizione = string.Empty, ID = string.Empty, TipoID = string.Empty, UnitaID = prd.UnitaID, QuantitaDefault = prd.QuantitaDefault }; return row; },
                    new { otsoci = CompanyID, OTANNO = Year }, splitOn: "ID").ToList());
            var attachments = new ObservableCollection<ORDITAL00F>(connection.Query<ORDITAL00F>(
                    @"SELECT * FROM ORDITAL00F
                            WHERE otasoci = @OTASOCI AND OTAANNO = @OTAANNO
                            ORDER BY OTANAME",
                    new { OTASOCI = CompanyID, OTAANNO = Year }).ToList());
            Parallel.ForEach(list, (head) =>
            {
                head.Rows = new ObservableCollection<ORDID00F>(rows.Where(w => w.OTNUOR == head.OTNUOR).ToList());
                head.Attachments = new ObservableCollection<ORDITAL00F>(attachments.Where(w => w.OTANUOR == head.OTNUOR).ToList());
                // compute production orders needed
                head.ProductionOrdersNeeded = head.Rows.Where(w => w.ODSTA == "*" || string.IsNullOrWhiteSpace(w.ODSTA)).Count();
                head.ProductionOrdersGenerated = (int?)connection.ExecuteScalar("SELECT COUNT(*) FROM pro_ordine WHERE SocietaID=@cid AND OrdineClienteAnno=@yea AND OrdineClienteID=@num", new { cid = CompanyID, yea = head.OTANNO, num = head.OTNUOR }) ?? 0;
            });
            return new ObservableCollection<ORDIT00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ORDID00F>? GetFulfillmentList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var umscache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            var list = new ObservableCollection<ORDID00F>(connection.Query<ORDID00F, tab_articolo, ABE, DESTINATARI, TAB_CRM_CAUORD, ORDIT00F, pro_ordine, ORDID00F>(
                    @"SELECT d.*,art.ID, art.Descrizione, art.UnitaID, art.UnitaIDAlt, art.QuantitaDefault,c.*, de.*, ca.*, t.flgchi, t.OTDAOR, t.OTPAGA, t.abiabi, t.abicab, t.OTBCON, prod.ID, prod.Stato, prod.LogCanceled FROM ORDID00F AS d
                            INNER JOIN ORDIT00F AS t ON t.otsoci=d.otsoci AND t.OTANNO=d.OTANNO AND t.OTNUOR=d.OTNUOR
                            INNER JOIN tab_articolo AS art ON art.SocietaID=d.otsoci AND art.ID = d.ODCODA
                            INNER JOIN ABE AS c ON c.abecod = t.OTCLIE
                            LEFT JOIN DESTINATARI AS de ON de.cliecod = t.OTCLIE AND de.codesti = t.DESTIN
                            LEFT JOIN pro_ordine AS prod ON prod.SocietaID=d.otsoci AND prod.OrdineClienteAnno=d.OTANNO AND prod.OrdineClienteID=d.OTNUOR AND prod.OrdineClienteRiga=d.ODRIGA
                            INNER JOIN TAB_CRM_CAUORD AS ca ON ca.cauacqsoc = t.otsoci AND ca.cauacq = t.OTCAUS
                            WHERE t.canceled IS NULL AND d.otsoci = @otsoci AND (ODSTA = '*' OR ODSTA = '#' OR ODSTA = '?') AND t.flgchi = 'F' AND d.ODSTATO IS NULL
                            ORDER BY d.ODDACO, t.OTCLIE, d.ODCODA",
                    (row, prd, abe, des, cau, hea, prod) =>
                    {
                        row.UMsCache = umscache;
                        row.Product = prd;
                        row.SelectedProduct = prd;
                        row.Customer = abe;
                        row.Recipient = des;
                        row.PaymentID = hea.OTPAGA;
                        row.BankFullID = $"{hea.abiabi}{hea.abicab}{hea.OTBCON}";
                        row.Causal = cau;
                        row.OrderDate = hea.OTDAOR ?? DateTime.MinValue;
                        row.HeadStatusDescription = CommonsService.OrderStatuses.Where(w => w.ID == hea.flgchi).FirstOrDefault()?.Description;
                        row.LinkedProductionOrder = prod;
                        return row;
                    },
                    new { otsoci = CompanyID }, splitOn: "ID, abecod, cliecod, cauacq, flgchi, ID").ToList());
            return new ObservableCollection<ORDID00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public void FlagFulfillment(string otsoci, int OTANNO, int OTNUOR, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
            var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();

            var multi = connection.QueryMultiple(
                    @"SELECT COUNT(*) FROM ORDID00F WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR;
                            SELECT COUNT(*) FROM ORDID00F WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND ODSTATO = '*';",
                    new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR });
            var total = multi.Read<int>().Single();
            var fulfilled = multi.Read<int>().Single();
            var head = Get(otsoci, OTANNO, OTNUOR);

            if (head != null)
            {
                if (total == fulfilled)
                {
                    head.updatedUserID = UserID;
                    head.flgchi = "E";
                    Update(head);
                }
                else
                {
                    head.updatedUserID = UserID;
                    head.flgchi = "F"; // reset to SIGNED
                    Update(head);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
        }
    }

    public ORDIT00F? Get(string otsoci, int OTANNO, int OTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<ORDIT00F, ABE, DESTINATARI, TAB_CRM_CAUORD, ORDIT00F>(
                @"SELECT o.*, c.*, d.*, ca.* FROM ORDIT00F AS o
                        INNER JOIN ABE AS c ON c.abecod = o.OTCLIE
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OTCLIE AND d.codesti = o.DESTIN
                        INNER JOIN TAB_CRM_CAUORD AS ca ON ca.cauacqsoc = o.otsoci AND ca.cauacq = o.OTCAUS
                        WHERE o.otsoci = @otsoci AND o.OTANNO = @OTANNO AND o.OTNUOR = @OTNUOR",
                (ord, abe, des, cau) => { ord.Customer = abe; ord.Recipient = des; ord.Causal = cau; return ord; },
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR }, splitOn: "abecod, cliecod, cauacq")
                .FirstOrDefault();
            var umscache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(otsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<ORDID00F>(connection.Query<ORDID00F>(
                        @"SELECT OTNUOR,ODTPRE,ODPREZ,ODQTAV, ODSCO1, ODSCO2, ODSCO3, ODMAGG, ODTSC1,ODTSC2,ODTSC3,ODTMAG FROM ORDID00F
                            WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR",
                        new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR }).ToList());
                result.Attachments = new ObservableCollection<ORDITAL00F>(connection.Query<ORDITAL00F>(
                        @"SELECT * FROM ORDITAL00F
                            WHERE otasoci = @OTASOCI AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR
                            ORDER BY OTANAME",
                        new { OTASOCI = otsoci, OTAANNO = OTANNO, OTANUOR = OTNUOR }).ToList());
                Parallel.ForEach(result.Rows, (row) => { row.UMsCache = umscache; });
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ORDIT00F? GetFull(string otsoci, int OTANNO, int OTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();
            var fatttRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>();

            var result = connection.Query<ORDIT00F, ABE, DESTINATARI, TAB_CRM_CAUORD, ORDIT00F>(
                @"SELECT o.*, c.*, d.*, ca.* FROM ORDIT00F AS o
                        INNER JOIN ABE AS c ON c.abecod = o.OTCLIE
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OTCLIE AND d.codesti = o.DESTIN
                        INNER JOIN TAB_CRM_CAUORD AS ca ON ca.cauacqsoc = o.otsoci AND ca.cauacq = o.OTCAUS
                        WHERE o.otsoci = @otsoci AND o.OTANNO = @OTANNO AND o.OTNUOR = @OTNUOR",
                (ord, abe, des, cau) => { ord.Customer = abe; ord.Recipient = des; ord.Causal = cau; return ord; },
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR }, splitOn: "abecod, cliecod, cauacq")
                .FirstOrDefault();
            var umscache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(otsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<ORDID00F>(connection.Query<ORDID00F, tab_articolo, ORDID00F>(
                        @"SELECT r.*,p.SocietaID, p.ID, p.Descrizione, p.UnitaID FROM ORDID00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.otsoci AND p.ID = r.ODCODA
                            WHERE r.otsoci = @otsoci AND r.OTANNO = @OTANNO AND r.OTNUOR = @OTNUOR",
                        (off, prd) =>
                        {
                            off.UMsCache = umscache;
                            off.SelectedProduct = prd;
                            off.CustomerDiscount = result.OTSCCL ?? 0;
                            return off;
                        },
                        new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR }, splitOn: "SocietaID").ToList());
                result.Invoices = fatttRepo.GetListByOrder(otsoci, OTANNO, OTNUOR);
                result.DDTs = bolltRepo.GetListByOrder(otsoci, OTANNO, OTNUOR);
                result.Attachments = new ObservableCollection<ORDITAL00F>(connection.Query<ORDITAL00F>(
                        @"SELECT * FROM ORDITAL00F
                            WHERE otasoci = @OTASOCI AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR
                            ORDER BY OTANAME",
                        new { OTASOCI = otsoci, OTAANNO = OTANNO, OTANUOR = OTNUOR }).ToList());
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string otsoci, int OTANNO, int OTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM ORDIT00F 
                        WHERE canceled IS NULL AND otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR",
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public long GetTotalOrdersCount(string CompanyID, int? Year, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query;
            if (Year.HasValue)
                query = @"SELECT COUNT_BIG(*) FROM ORDIT00F WHERE canceled IS NULL AND otsoci=@otsoci AND OTCLIE=@otclie AND OTANNO = @otanno";
            else
                query = @"SELECT COUNT_BIG(*) FROM ORDIT00F WHERE canceled IS NULL AND otsoci=@otsoci AND OTCLIE=@otclie";
            return (long)(connection.ExecuteScalar(
                query,
                new { otsoci = CompanyID, otanno = Year.HasValue ? Year.Value : 0, otclie = CustomerID }) ?? 0L);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public decimal GetOrdersAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate, bool OnlyNew)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query = @$"SELECT d.*, t.* FROM ORDID00F AS d
                       INNER JOIN ORDIT00F AS t ON t.otsoci = d.otsoci AND t.OTANNO = d.OTANNO AND t.OTNUOR = d.OTNUOR
                       WHERE t.canceled IS NULL AND t.otsoci=@otsoci AND t.OTCLIE=@otclie {(Year.HasValue ? "AND t.OTANNO = @otanno" : null)} {(ToDate.HasValue ? "AND t.OTDAOR<=@todate" : null)} {(OnlyNew ? "AND d.ODFL10 = ' '" : null)}";

            var list = connection.Query<ORDID00F, ORDIT00F, ORDID00F>(
                query,
                (dett, head) => { return dett; },
                new { otsoci = CompanyID, otanno = Year.HasValue ? Year.Value : 0, otclie = CustomerID, todate = ToDate },
                splitOn: "otsoci");

            return list.Sum(sum => sum.NetPrice);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public List<int>? GetDistinctYearsExisting(string CompanyID, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<int>(
                @"SELECT DISTINCT OTANNO from ORDIT00F WHERE otsoci=@otsoci AND OTCLIE=@otclie ORDER BY OTANNO",
                new { otsoci = CompanyID, otclie = CustomerID }).ToList();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRM
    public bool GenerateByOffer(OFFET00F OfferHead, List<OFFED00F> SelectedRows, List<OFFETAL00F> SelectedAttachments, string UserID, bool HasAllSigns, Guid CompanyUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var offedRepo = VulpesServiceProvider.Provider.GetRequiredService<IOFFED00FRepository>();
                var orditalRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDITAL00FRepository>();

                int sequence = 1;
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var newOrderID = numRegRepo.GetNumber(OfferHead.oftsoci, now.Year, Constants.CUSTOMER_ORDERS, true);

                // generate order head
                var newHead = new ORDIT00F()
                {
                    otsoci = OfferHead.oftsoci,
                    OTANNO = now.Year,
                    OTNUOR = newOrderID,
                    OTDAOR = now.Date,
                    OTCAUS = OfferHead.Causal?.offord,
                    OTCLIE = OfferHead.OFTCOCL,
                    DESTIN = OfferHead.OFTDEST,
                    OTCONZ = OfferHead.OFTCONZ,
                    OTPAGA = OfferHead.OFTPAGA,
                    abiabi = OfferHead.abiabi,
                    abicab = OfferHead.abicab,
                    OTCONS = OfferHead.OFTCONS,
                    OTSPED = OfferHead.OFTSPED,
                    OTCORR = OfferHead.OFTCORR,
                    OTCORR2 = OfferHead.OFTCORR2,
                    OTDATC = OfferHead.OFTDACO,
                    OTAREA = OfferHead.OFTAREA,
                    OTZONA = OfferHead.OFTZONE,
                    OTFILI = OfferHead.OFTFILI,
                    OTSETM = OfferHead.OFTSETM,
                    OTCIDI = OfferHead.OFTCIDI,
                    otdivi = OfferHead.oftdivi,
                    OTDE25 = OfferHead.OFTDE25,
                    OTIMBA = OfferHead.OFTIMBA,
                    otling = OfferHead.OFTLING,
                    flgchi = HasAllSigns ? "F" : "A",
                    added = now,
                    addedUserID = UserID,
                    OTBCON = OfferHead.OFTBCON,
                    OTINFI = HasAllSigns ? now : null,
                    OTINFIUSR = HasAllSigns ? UserID : null,
                    OTFICO = HasAllSigns ? now : null,
                    OTFICOUSR = HasAllSigns ? UserID : null,
                    OTFITE = HasAllSigns ? now : null,
                    OTFITEUSR = HasAllSigns ? UserID : null,
                    OTDARI = OfferHead.OFTDARI,
                    OTREGI = OfferHead.OFTREGI,
                    OTRIVE = OfferHead.OFTRIVE,
                    OTSCCL = OfferHead.OFTSCCL
                };
                connection.Execute(INSERT_QUERY, newHead, transaction);
                // generate rows and update origins
                foreach (var row in SelectedRows)
                {
                    var newRow = new ORDID00F()
                    {
                        otsoci = OfferHead.oftsoci,
                        OTANNO = now.Year,
                        OTNUOR = newOrderID,
                        ODRIGA = sequence,
                        ODCODA = row.OFDCODA,
                        ODQTAV = row.OFDQTAV,
                        ODTQTA = row.OFDTQTA,
                        ODPREZ = row.OFDPREZ,
                        ODTPRE = row.OFDTPRE,
                        ODSCO1 = row.OFDSCO1,
                        ODSCO2 = row.OFDSCO2,
                        ODSCO3 = row.OFDSCO3,
                        ODMAGG = row.OFDMAGG,
                        ODTSC1 = row.OFDTSC1,
                        ODTSC2 = row.OFDTSC2,
                        ODTSC3 = row.OFDTSC3,
                        ODTMAG = row.OFDTMAG,
                        ODASSF = row.OFDASSF,
                        ODALIV = row.OFDALIV,
                        ODDACO = row.OFDDACO,
                        ODRIFC = row.OFDRIFC,
                        ODGRUP = row.OFDGRUP,
                        ODCONT = row.OFDCONT,
                        ODSCTO = row.OFDSCTO,
                        odunit = row.ofdunim,
                        ODOFTAN = row.OFTANNO,
                        ODOFTNUM = row.OFTNUOR,
                        ODOFDRIG = row.OFDRIGA,
                        ODNOTE = row.OFDNOTE,
                        ODSHOW = row.OFDSHOW,
                        ODCOA1 = row.OFDCOA1,
                        ODCOA1P = row.OFDCOA1P,
                        ODCOA1PT = row.OFDCOA1PT,
                        ODCOA2 = row.OFDCOA2,
                        ODCOA2P = row.OFDCOA2P,
                        ODCOA2PT = row.OFDCOA2PT,
                        ODDARI = row.OFDDARI,
                        ODQTAEV = row.OFDQTAEV
                    };
                    connection.Execute(ordidRepo.INSERT_QUERY, newRow, transaction);
                    row.transformed = now;
                    row.transform_user = UserID;
                    row.ofdanf = now.Year;
                    row.ofdnuf = newOrderID;
                    row.ofdrif = sequence++;
                    connection.Execute(offedRepo.UPDATE_QUERY, row, transaction);
                }
                // add attachments 
                foreach (var att in SelectedAttachments)
                {
                    var newGuid = Guid.NewGuid();
                    // download blob
                    var fileData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.OFFERS_ATTACHMENTS_FOLDER}{att.OFTAUID}");

                    if (fileData != null)
                    {
                        if (StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{newGuid}", fileData) == null)
                        {
                            // add record
                            var newAtt = new ORDITAL00F()
                            {
                                otasoci = OfferHead.oftsoci,
                                OTAANNO = now.Year,
                                OTANUOR = newOrderID,
                                OTANAME = att.OFTANAME,
                                OTAUID = newGuid,
                                OTASIZE = att.OFTASIZE,
                                add_user = UserID
                            };
                            connection.Execute(orditalRepo.INSERT_QUERY, newAtt, transaction);
                        }
                    }
                }

                transaction.Commit();
                InfoHandler.Show($"Generato correttamente l'ordine cliente {now.Year}/{newOrderID}");
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
    #endregion

    #region Production
    public bool GenerateProductionOrder(ORDIT00F Order, string UserID, long? ExistingOrderID, string? RevisionID, bool IsLinked)
    {
        try
        {
            using var connection = GetOpenConnection();

            try
            {
                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();
                var tabArticoloComposizioneRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>();
                var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                var storeStockRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>();
                var srmrdaRepo = VulpesServiceProvider.Provider.GetRequiredService<ISRM_RDARepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var proOrdineRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>();


                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                foreach (var row in (Order.Rows ?? new ObservableCollection<ORDID00F>()).Where(w => w.ODSTA == null))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // if product has cycle create production order and flag the row with *, else
                            // flag it with # as not  to produce
                            if (tabArticoloComposizioneRepo.HasCycle(row.otsoci, row.ODCODA))
                            {
                                // retrieve ID
                                var orderID = ((ExistingOrderID.HasValue && ExistingOrderID.Value > 0) ? ExistingOrderID : numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(Order.otsoci, now.Year, Constants.PRODUCTION_ORDERS, true))) ?? 0;
                                var defaultRevisionID = string.IsNullOrWhiteSpace(RevisionID) ? tabArticoloRepo.GetDefaultRevision(Order.otsoci, row.ODCODA) : RevisionID;
                                // get product
                                var product = tabArticoloRepo.GetSingle(row.otsoci, row.ODCODA);
                                // create head
                                pro_ordine prodOrder = new pro_ordine()
                                {
                                    SocietaID = Order.otsoci,
                                    ID = orderID.ToString(),
                                    LogAdded = now,
                                    LogAddedUserID = UserID,
                                    ClienteID = Order.OTCLIE,
                                    ArticoloID = row.ODCODA,
                                    RevisioneID = !string.IsNullOrWhiteSpace(defaultRevisionID) ? defaultRevisionID : null,
                                    DataOrdine = Order.OTDAOR,
                                    DataConsegna = row.ODDACO,
                                    Quantita = tabArticoloRepo.ComputeRealQuantity(row.otsoci, row.ODCODA, row.odunit ?? string.Empty, product?.UnitaID ?? string.Empty, product?.QuantitaDefault, row.QuantityToProduce),
                                    Stato = "A",
                                    Commessa = orderID.ToString(),
                                    Note = row.ODNOTE,
                                    OrdineClienteAnno = IsLinked ? Order.OTANNO : null,
                                    OrdineClienteID = IsLinked ? Order.OTNUOR : null,
                                    OrdineClienteRiga = IsLinked ? row.ODRIGA : null
                                };
                                if (proOrdineRepo.Insert(prodOrder))
                                {
                                    row.ODSTA = "*"; // flag transformed to production order
                                }
                            }
                            else
                            {
                                #region Resale
                                // check if available
                                var availabilities = storeStockRepo.CheckAvailabilityByProduct(row.otsoci, row.ODCODA, null);
                                // add engage on customer order ordering by expire date
                                decimal engaged = 0;
                                if (availabilities != null && availabilities.Count > 0)
                                {
                                    foreach (var ava in availabilities.OrderBy(o => o.Expire).ThenBy(tb => tb.QuantityAvailable))
                                    {
                                        if (ava.QuantityAvailable >= row.ODQTAV - engaged)
                                        {
                                            var engage = new store_stocks_engage()
                                            {
                                                company_id = row.otsoci,
                                                id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(row.otsoci, now.Year, Constants.STORE_ENGAGES, true)),
                                                store_id = ava.StoreID ?? "X",
                                                product_id = row.ODCODA,
                                                quantity = row.ODQTAV - engaged,
                                                date_engaged = now,
                                                add_user = UserID,
                                                lot = ava.Lot,
                                                order_year = row.OTANNO,
                                                order_number = row.OTNUOR,
                                                order_row = row.ODRIGA
                                            };
                                            connection.Execute(storeStockEngageRepo.INSERT_QUERY, engage, transaction);
                                            engaged += row.ODQTAV - engaged;
                                            break;
                                        }
                                        else
                                        {
                                            var engage = new store_stocks_engage()
                                            {
                                                company_id = row.otsoci,
                                                id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(row.otsoci, now.Year, Constants.STORE_ENGAGES, true)),
                                                store_id = ava.StoreID ?? "X",
                                                product_id = row.ODCODA,
                                                quantity = ava.QuantityAvailable,
                                                date_engaged = now,
                                                add_user = UserID,
                                                lot = ava.Lot,
                                                order_year = row.OTANNO,
                                                order_number = row.OTNUOR,
                                                order_row = row.ODRIGA
                                            };
                                            connection.Execute(storeStockEngageRepo.INSERT_QUERY, engage, transaction);
                                            engaged += ava.QuantityAvailable;
                                        }
                                    }
                                }

                                if (row.ODQTAV == engaged)
                                {
                                    // flag not to produce and available
                                    row.ODSTA = "#";
                                }
                                else
                                {
                                    // not enough
                                    // generate RDA for difference
                                    var diffQuantity = row.ODQTAV - engaged;
                                    decimal quantity = tabArticoloRepo.ComputeQuantityToOrder(row.otsoci, row.ODCODA, diffQuantity);
                                    var defaultSupplierID = tabArticoloRepo.GetDefaultSupplier(row.otsoci, row.ODCODA);
                                    var roles = VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>().Get(row.otsoci, UserID);
                                    srmrdaRepo.Insert(new SRM_RDA()
                                    {
                                        companyID = row.otsoci,
                                        id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(row.otsoci, now.Year, Constants.RDA, true)),
                                        product_id = row.ODCODA,
                                        note = quantity > 0 ? $"RDA generata automaticamente dall'ordine cliente {row.OTANNO}/{row.OTNUOR}/{row.ODRIGA}" : $"RDA generata automaticamente dall'ordine cliente {row.OTANNO}/{row.OTNUOR}/{row.ODRIGA} con errori nel calcolo della quantità da ordinare, verificare anagrafica articolo",
                                        original_needed = diffQuantity,
                                        quantity = quantity,
                                        is_customer_material = false,
                                        requested_delivery = now,
                                        addedUserID = UserID,
                                        approval_date = roles != null && roles.canApproveRDA ? now : null,
                                        approval_user = roles != null && roles.canApproveRDA ? UserID : null,
                                        supplier_id = defaultSupplierID,
                                        order_year = row.OTANNO,
                                        order_number = row.OTNUOR,
                                        order_row = row.ODRIGA
                                    });
                                    // flag not to produce but waiting material
                                    row.ODSTA = "?";
                                }
                                #endregion
                            }
                            // reset original quantity
                            row.ODQTAV = row.OriginalQuantity;
                            connection.Execute(ordidRepo.UPDATE_QUERY, row, transaction);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Show(ex.Message);
                            transaction.Rollback();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
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

    public void UnlockOrders(string SocietaID, ObservableCollection<acq_orders_rows_customer_orders> CustomerOrders, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
            var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();

            foreach (var co in CustomerOrders)
            {
                var orderRow = ordidRepo.Get(SocietaID, co.customer_order_year, co.customer_order_number, co.customer_order_row);
                var engagedQuantity = storeStockEngageRepo.GetListByCustomerOrderID(co.company_id, co.customer_order_year, co.customer_order_number, co.customer_order_row)?.Sum(sum => sum.quantity) ?? 0;

                if (orderRow != null)
                {
                    if (engagedQuantity == orderRow.ODQTAV)
                    {
                        orderRow.ODSTA = "#";
                        ordidRepo.Update(orderRow);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
        }
    }
    #endregion

    #region Printing
    public ORDIT00F? GetPrintFull(string otsoci, int OTANNO, int OTNUOR, bool UseCustomerCodes, bool PrintProductNote, bool PrintAgentsInDetails)
    {
        try
        {
            using var connection = GetOpenConnection();

            var unitaRepo = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>();
            var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
            var agentiRepo = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>();
            var crmLisCliRepo = VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>();
            var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

            var result = connection.Query<ORDIT00F>(
                @"SELECT i.*, c.*, v1.*, v2.*,s.*, co.*, d.*,iso.isolin,sl.spedes,cl.condes FROM ORDIT00F AS i
                        INNER JOIN ABE AS c ON c.abecod = i.OTCLIE
                        LEFT OUTER JOIN VETTORI AS v1 ON v1.vetcod = i.OTCORR 
                        LEFT OUTER JOIN VETTORI AS v2 ON v2.vetcod = i.OTCORR2 
                        LEFT OUTER JOIN SPEDIZIONE AS s ON s.specod = i.OTSPED
                        LEFT OUTER JOIN CONSEGNA AS co ON co.concod = i.OTCONS
                        LEFT OUTER JOIN DESTINATARI AS d ON d.cliecod = i.OTCLIE AND d.codesti = i.DESTIN
                        LEFT OUTER JOIN ISO AS iso ON iso.isocod = c.isocod
                        LEFT OUTER JOIN SPEDIZIONE_LINGUA AS sl ON sl.specod = i.OTSPED AND sl.lincod = iso.isolin
                        LEFT OUTER JOIN CONSEGNA_LINGUA AS cl ON cl.concod = i.OTCONS AND cl.lincod = iso.isolin
                        WHERE i.otsoci = @otsoci AND i.OTANNO = @OTANNO AND i.OTNUOR = @OTNUOR",
              new[] { typeof(ORDIT00F), typeof(ABE), typeof(VETTORI), typeof(VETTORI), typeof(SPEDIZIONE), typeof(CONSEGNA), typeof(DESTINATARI), typeof(string), typeof(string), typeof(string) }, (objects) =>
              {
                  var ordit00f = (ORDIT00F)objects[0];
                  ordit00f.Customer = objects[1] as ABE;
                  ordit00f.FirstCarrier = objects[2] as VETTORI;
                  ordit00f.SecondCarrier = objects[3] as VETTORI;
                  ordit00f.Shipping = objects[4] as SPEDIZIONE;
                  if (ordit00f.Shipping != null)
                      ordit00f.Shipping.spedes = (!string.IsNullOrEmpty(objects[8]?.ToString()) ? objects[8].ToString() : ordit00f.Shipping.spedes) ?? string.Empty;
                  ordit00f.Delivery = objects[5] as CONSEGNA;
                  if (ordit00f.Delivery != null)
                      ordit00f.Delivery.condes = (!string.IsNullOrEmpty(objects[9]?.ToString()) ? objects[9].ToString() : ordit00f.Delivery.condes) ?? string.Empty;
                  ordit00f.Recipient = objects[6] as DESTINATARI;
                  ordit00f.Language = objects[7] as string;

                  return ordit00f;
              },
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR }, splitOn: "abecod,vetcod,vetcod,specod,concod,cliecod,isolin,spedes,condes")
                .FirstOrDefault();

            var umscache = unitaRepo.GetSimpleList(otsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<ORDID00F>(connection.Query<ORDID00F, tab_articolo, tab_articolo_unita, AGENTI, AGENTI, tab_articolo_lingua, ORDID00F>(
                        $@"SELECT r.*, {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes, l.* FROM ORDID00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.otsoci AND p.ID = r.ODCODA
                            LEFT OUTER JOIN tab_articolo_lingua AS l ON p.SocietaID = l.SocietaID AND p.ID = l.ArticoloID AND l.lincod = @Lingua
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.otsoci AND u.ID = r.odunit
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.ODCOA1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.ODCOA2
                            WHERE r.otsoci = @otsoci AND r.OTANNO = @OTANNO AND r.OTNUOR = @OTNUOR",
                        (row, prd, um, age1, age2, lingua) =>
                        {
                            row.UMsCache = umscache;
                            row.Product = prd;
                            row.Product.Descrizione = (lingua != null && !string.IsNullOrEmpty(lingua.Descrizione) ? lingua.Descrizione : prd.Descrizione);
                            row.Product.Note = (lingua != null && !string.IsNullOrEmpty(lingua.Note) ? lingua.Note : prd.Note);
                            row.UM = um;
                            row.OriginalQuantity = row.QuantityToProduce;
                            row.SelectedFirstAgent = age1;
                            row.SelectedSecondAgent = age2;
                            row.PrintAgentsInDetails = PrintAgentsInDetails && (age1 != null || age2 != null);
                            row.CustomerDiscount = result.OTSCCL ?? 0;
                            return row;
                        },
                        new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR, Lingua = result.Language }, splitOn: "SocietaID,SocietaID,agecod,agecod,SocietaID").ToList() ?? new List<ORDID00F>());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent
                    var agecod1 = result.Rows.OrderBy(o => o.ODRIGA).FirstOrDefault()?.ODCOA1;
                    var agecod2 = result.Rows.OrderBy(o => o.ODRIGA).FirstOrDefault()?.ODCOA2;

                    result.DefaultFirstAgent = !string.IsNullOrEmpty(agecod1) ? agentiRepo.Get(agecod1) : null;
                    result.DefaultSecondAgent = !string.IsNullOrEmpty(agecod2) ? agentiRepo.Get(agecod2) : null;
                }

                // check if custom code and UMs recap for each row
                result.UMsRecap = new ObservableCollection<GenericStringDecimal>();
                foreach (var row in result.Rows.OrderBy(o => o.odunit))
                {
                    #region Customer code
                    var lisCli = crmLisCliRepo.GetCurrent(otsoci, row.ODCODA, result.OTCLIE ?? 0, result.OTDAOR ?? DateTime.Now, row.odunit ?? string.Empty);
                    if (lisCli != null)
                    {
                        if (!UseCustomerCodes)
                        {
                            row.CustomerCode = $"Codifica cliente: {(!string.IsNullOrWhiteSpace(lisCli.CustomerProductID) ? lisCli.CustomerProductID : null)} {(!string.IsNullOrWhiteSpace(lisCli.CustomerProductDescription) ? lisCli.CustomerProductDescription : null)}";
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(lisCli.CustomerProductID))
                                row.ODCODA = lisCli.CustomerProductID;
                            if (!string.IsNullOrWhiteSpace(lisCli.CustomerProductDescription) && row.Product != null)
                                row.Product.Descrizione = lisCli.CustomerProductDescription;
                        }
                    }
                    else
                    {
                        row.CustomerCode = null;
                    }
                    #endregion
                    #region UMs recap
                    var existingUM = result.UMsRecap.Where(w => w.Description == row.odunit).FirstOrDefault();
                    if (existingUM != null)
                    {
                        existingUM.Value += row.ODQTAV;
                    }
                    else
                    {
                        result.UMsRecap.Add(new GenericStringDecimal()
                        {
                            Description = row.odunit,
                            Value = row.ODQTAV
                        });
                    }
                    #endregion
                }
                #region Rates recap
                int i = 1;
                List<string> rates = new List<string>();
                var ratesList = new List<Tuple<string, string, string, string>>();
                var ratesList2 = new List<Tuple<string, string, string, string>>();
                foreach (var row in result.Rows.OrderBy(o => o.ODASSF).ThenBy(o => o.ODALIV))
                {
                    if (!rates.Contains(row.ODASSF + row.ODALIV?.Trim()))
                    {
                        var rate = aliquotaRepo.Get(row.ODASSF ?? string.Empty, row.ODALIV ?? string.Empty);
                        rates.Add(row.ODASSF + row.ODALIV?.Trim());
                        var imponibile = Math.Round(result.Rows
                        .Where(w => w.ODASSF == row.ODASSF && w.ODALIV == row.ODALIV)
                        .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                        // compute customer discount
                        imponibile = imponibile - (imponibile * (result.OTSCCL ?? 0) / 100);
                        decimal rateValue = 0;
                        decimal.TryParse(rate?.assali, out rateValue);
                        var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                        if (i % 2 == 0)
                            ratesList2.Add(new Tuple<string, string, string, string>(row.ODASSF + " " + row.ODALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                        else
                            ratesList.Add(new Tuple<string, string, string, string>(row.ODASSF + " " + row.ODALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                        i++;
                    }
                }
                result.RatesRecap = ratesList;
                result.RatesRecap2 = ratesList2;
                #endregion
                if (result.HasGifts)
                {
                    #region Gifts rates recap
                    i = 1;
                    List<string> giftsRates = new List<string>();
                    var giftsRatesList = new List<Tuple<string, string, string, string>>();
                    foreach (var row in result.Rows.Where(w => w.ODTQTA == "O").OrderBy(o => o.ODASSF).ThenBy(o => o.ODALIV))
                    {
                        if (!giftsRates.Contains(row.ODASSF + row.ODALIV?.Trim()))
                        {
                            var rate = aliquotaRepo.Get(row.ODASSF ?? string.Empty, row.ODALIV ?? string.Empty);
                            giftsRates.Add(row.ODASSF + row.ODALIV?.Trim());
                            var imponibile = Math.Round(result.Rows
                            .Where(w => w.ODTQTA == "O" && w.ODASSF == row.ODASSF && w.ODALIV == row.ODALIV)
                            .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                            // compute customer discount
                            imponibile = imponibile - (imponibile * (result.OTSCCL ?? 0) / 100);
                            decimal rateValue = 0;
                            decimal.TryParse(rate?.assali, out rateValue);
                            var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                            giftsRatesList.Add(new Tuple<string, string, string, string>(row.ODASSF + " " + row.ODALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                            i++;
                        }
                    }
                    result.GiftsRatesRecap = giftsRatesList;
                    #endregion
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
    public OrderReport? PrintOrder(ORDIT00F Order)
    {
        try
        {
            var aziendaRepository = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();

            var azienda = aziendaRepository.Get(Order.otsoci);

            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(Order.otsoci);

            var causal = !string.IsNullOrEmpty(Order.OTCAUS) ? VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUORDRepository>().GetFull(Order.otsoci, Order.OTCAUS) : null;

            foreach (var row in Order.Rows ?? new ObservableCollection<ORDID00F>())
            {
                row.CausalNotes = causal != null && causal.Text != null ? causal.Text.TTXNote : null;
            }

            var bank = (Order.abiabi.HasValue && Order.abicab.HasValue) ? VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(Order.abiabi.Value, Order.abicab.Value) : null;

            var companyBank = (Order.abiabi.HasValue && Order.abicab.HasValue && !string.IsNullOrEmpty(Order.OTBCON)) ? VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Order.otsoci, Order.abiabi.Value, Order.abicab.Value, Order.OTBCON) : null;

            var customerData = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Order.otsoci, Order.OTCLIE!.Value);

            var pagcli = !string.IsNullOrEmpty(Order.OTPAGA) ? VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(Order.OTPAGA) : null;

            // get customizations
            var aziendaLingua = !string.IsNullOrEmpty(Order.Language) ? VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(Order.otsoci, Order.Language) : null;

            var pagcliLingua = (!string.IsNullOrEmpty(Order.OTPAGA) && !string.IsNullOrEmpty(Order.Language)) ? VulpesServiceProvider.Provider.GetRequiredService<IPAGCLI_LINGUARepository>().Get(Order.OTPAGA, Order.Language) : null;

            // get customizations
            object? dictionary = null;

            if (!string.IsNullOrEmpty(Order.Language))
            {
                var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(Order.Language);

                if (languageDictionary != null)
                    dictionary = new LocalizationHelper().CreateClassFromDictionary(languageDictionary);
            }

            return new OrderReport()
            {
                Order = Order,
                PaymentDescription = (pagcliLingua != null) ? (!string.IsNullOrEmpty(pagcliLingua.pcldes) ? pagcliLingua.pcldes : pagcli?.pcldes) : pagcli?.pcldes,
                CompanyInfo = aziendaRepository.Get(Order.otsoci),
                BankData = $"{bank?.FullDescriptionSearchable} c/c nr.{(companyBank != null ? Order.OTBCON : customerData?.CLNUCC)} IBAN: {(companyBank != null ? companyBank.abibiba : customerData?.cliban)}",
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase!.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                CertificationsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}certs.png"),
                HeaderNote = Order.OTSHOWT ? Order.OTNOTET : null,
                HeaderFootNote = Order.OTSHOWP ? Order.OTNOTEP : null,
                FixedText = (aziendaLingua != null) ? (!string.IsNullOrEmpty(aziendaLingua.azordgtex) ? aziendaLingua.azordgtex : azienda!.azordgtex) : azienda!.azordgtex,
                LinguaDictionary = dictionary,
            };
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ORDIT00F (otsoci,OTANNO,OTNUOR,OTDAOR,OTCAUS,OTCLIE,DESTIN,OTCONZ,OTPAGA,abicab,OTCONS,OTSPED,OTCORR,OTDATC,OTAREA,OTFILI,OTZONA,OTSETM,OTCIDI,OTDE25,OTIMBA,abiabi,otdivi,flgchi,otling,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,OTNOTET,OTNOTEP,OTSHOWT,OTSHOWP,OTBCON,OTINFI,OTINFIUSR,OTFICO,OTFICOUSR,OTFITE,OTFITEUSR,OTDARI,OTCORR2,OTREGI,OTRIVE,OTSCCL,OTDAPA,OTCUNO,OTCUDO) OUTPUT INSERTED.rv VALUES(@otsoci,@OTANNO,@OTNUOR,@OTDAOR,@OTCAUS,@OTCLIE,@DESTIN,@OTCONZ,@OTPAGA,@abicab,@OTCONS,@OTSPED,@OTCORR,@OTDATC,@OTAREA,@OTFILI,@OTZONA,@OTSETM,@OTCIDI,@OTDE25,@OTIMBA,@abiabi,@otdivi,@flgchi,@otling,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@OTNOTET,@OTNOTEP,@OTSHOWT,@OTSHOWP,@OTBCON,@OTINFI,@OTINFIUSR,@OTFICO,@OTFICOUSR,@OTFITE,@OTFITEUSR,@OTDARI,@OTCORR2,@OTREGI,@OTRIVE,@OTSCCL,@OTDAPA,@OTCUNO,@OTCUDO)";
    public string UPDATE_QUERY => "UPDATE ORDIT00F SET otsoci = @otsoci,OTANNO = @OTANNO,OTNUOR = @OTNUOR,OTDAOR = @OTDAOR,OTCAUS = @OTCAUS,OTCLIE = @OTCLIE,DESTIN = @DESTIN,OTCONZ = @OTCONZ,OTPAGA = @OTPAGA,abicab = @abicab,OTCONS = @OTCONS,OTSPED = @OTSPED,OTCORR = @OTCORR,OTDATC = @OTDATC,OTAREA = @OTAREA,OTFILI = @OTFILI,OTZONA = @OTZONA,OTSETM = @OTSETM,OTCIDI = @OTCIDI,OTDE25 = @OTDE25,OTIMBA = @OTIMBA,abiabi = @abiabi,otdivi = @otdivi,flgchi = @flgchi,otling = @otling,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,OTNOTET = @OTNOTET,OTNOTEP = @OTNOTEP,OTSHOWT = @OTSHOWT,OTSHOWP = @OTSHOWP,OTBCON = @OTBCON,OTINFI = @OTINFI,OTINFIUSR = @OTINFIUSR,OTFICO = @OTFICO,OTFICOUSR = @OTFICOUSR,OTFITE = @OTFITE,OTFITEUSR = @OTFITEUSR,OTDARI = @OTDARI,OTCORR2 = @OTCORR2,OTREGI = @OTREGI,OTRIVE = @OTRIVE,OTSCCL = @OTSCCL,OTDAPA = @OTDAPA,OTCUNO = @OTCUNO,OTCUDO = @OTCUDO OUTPUT INSERTED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ORDIT00F OUTPUT DELETED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND rv = @rv";
    public bool Insert(ORDIT00F Model)
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

    public bool Update(ORDIT00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (string.IsNullOrWhiteSpace(Model.OTNOTET))
                Model.OTNOTET = null;
            if (string.IsNullOrWhiteSpace(Model.OTNOTEP))
                Model.OTNOTEP = null;
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

    public bool Delete(ORDIT00F Model)
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

    public string? Validate(ORDIT00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.otsoci) && Model.OTANNO > 0 && IsInsert && !Exists(Model.otsoci, Model.OTANNO, Model.OTNUOR)) || !IsInsert)
            {
                if (Model.OTDAOR.HasValue && Model.OTDAOR.Value.Year == Model.OTANNO)
                {
                    if (!string.IsNullOrWhiteSpace(Model.OTCAUS))
                    {
                        if (Model.OTCLIE.HasValue && Model.OTCLIE.Value > 0)
                        {
                            if (Model.abiabi.HasValue && Model.abiabi.Value > 0 &&
                                Model.abicab.HasValue && Model.abicab.Value > 0)
                            {
                                return null;
                            }
                            else
                            { return "La banca di riferimento è obbligatoria"; }
                        }
                        else
                        { return "Il codice cliente è obbligatorio"; }
                    }
                    else
                    { return "La causale è obbligatoria"; }
                }
                else
                { return $"La data dell'ordine è obbligatoria e deve essere nell'anno scelto : {Model.OTANNO}"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}