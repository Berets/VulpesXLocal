
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Text;
using VulpesX.DAL.Auth;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Production;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.General;
using VulpesX.DAL.Tables.Productions;
using VulpesX.Models.Models;
using VulpesX.Models.Reports.Shipping;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.DAL.Shipping;

public interface IBOLLT00FRepository
{
    ObservableCollection<BOLLT00F>? GetList(string CompanyID, int Year, string? Status, string EntityType, string? FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount);

    BOLLT00F? Get(string CompanyID, int Year, int ID);

    List<BOLLT00F>? GetFromList(string CompanyID, string[] IDList);

    ObservableCollection<BOLLD00F>? GetListByOrder(string OrderCompanyID, int OrderYear, int OrderNumber);

    BOLLT00F? GetFull(string CompanyID, int Year, int ID, bool PrintProductNote, bool PrintAgentsInDetails);

    decimal GetDDTAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate);

    bool Exists(string bolsoc, int BTANNO, int BTBOLL);

    bool ExistsDefinitive(string bolsoc, int BTANNO, int BTNUBD, int BTCODC, string BTCAUS);

    #region Printing
    BOLLT00F? GetPrintFull(string CompanyID, int Year, int ID, bool PrintProductNote, bool PrintAgentsInDetails, bool UseCustomerCodes);

    DDTReport? PrintDDT(BOLLT00F DDT);
    #endregion

    #region CRM
    bool GenerateByOrder(ORDIT00F OrderHead, List<ORDID00F> SelectedRows, string UserID, string HeadNotes, string FootNotes);

    bool CancelAndFreeLinkedOrders(BOLLT00F DDT, string UserID, string CancelReason);

    bool UpdateDefinitiveDDTAndUnloadEngages(BOLLT00F DDT, store_causals StoreCausal, string UserID);
    #endregion

    #region Import
    bool ImportGILAT(List<string> Rows, string CausalID, string RateCode, string RateValue, string ExternCode, string CompanyID, string UserID);

    bool ImportBANCOLAT(List<string> Rows, string CausalID, string ExternCode, string CompanyID, string UserID);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(BOLLT00F Model);

    bool Update(BOLLT00F Model);

    bool Delete(BOLLT00F Model);

    string? Validate(BOLLT00F Model, bool IsInsert);
    #endregion
}

public class BOLLT00FRepository : RepositoryBase, IBOLLT00FRepository
{
    public BOLLT00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BOLLT00F>? GetList(string CompanyID, int Year, string? Status, string EntityType, string? FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount)
    {
        TotalCount = 0;
        try
        {
            using var connection = GetOpenConnection();

            var aliasList = new List<GenericIDDescriptionType>() {
                    new GenericIDDescriptionType(){ ID = "BTBOLLText", Description="BTBOLL" },
                    new GenericIDDescriptionType(){ ID = "BTDATPText", Description="BTDATP", Type = "D" },
                    new GenericIDDescriptionType(){ ID = "BTDATAText", Description="BTDATA", Type = "D" },
                    new GenericIDDescriptionType(){ ID = "BTDASPText", Description="BTDASP", Type = "DT" },
                    new GenericIDDescriptionType(){ ID = "BTCOLLText", Description="BTCOLL" },
                    new GenericIDDescriptionType(){ ID = "Causal.bolcod", Description="bolcod" },
                    new GenericIDDescriptionType(){ ID = "Causal.boldes", Description="boldes" },
                    new GenericIDDescriptionType(){ ID = "CustomerID", Description="abecod" },
                    new GenericIDDescriptionType(){ ID = "CustomerDescription", Description="abers1" },
                    new GenericIDDescriptionType(){ ID = "RecipientID", Description="codesti" },
                    new GenericIDDescriptionType(){ ID = "RecipientDescription", Description="ragisoc" },
                    new GenericIDDescriptionType(){ ID = "BTDE25", Description="BTDE25" },
                    new GenericIDDescriptionType(){ ID = "BTDEBE", Description="BTDEBE" },
                    new GenericIDDescriptionType() { ID = "DefinitiveID", Description = "BTNUBD" },
                    new GenericIDDescriptionType() { ID = "FirstCarrier.vetdes", Description = "ve1.vetdes" },
                    new GenericIDDescriptionType() { ID = "SecondCarrier.vetdes", Description = "ve2.vetdes" }};
            #region Args
            FullTextSearch = FullTextSearch?.ToLower();
            var args = new DynamicParameters();
            args.Add("bolsoc", CompanyID);
            args.Add("yea", Year);
            args.Add("sts", Status);
            args.Add("et", EntityType);
            args.Add("skip", RequestedPage * PageSize);
            args.Add("ps", PageSize);
            args.Add("ft", $"%{FullTextSearch}%");
            #endregion
            #region Where
            var whereFilter = new StringBuilder($@"b.bolsoc = @bolsoc AND b.BTANNO = @yea AND BTFLCF=@et AND 
                                            {(Status == "X" ? " b.canceled IS NOT NULL " : $" {(Status == "*" ? "(b.BTSTATO <> @sts OR b.BTSTATO IS NULL)" : (!string.IsNullOrWhiteSpace(Status) ? "(b.BTSTATO = @sts AND b.canceled IS NULL)" : "(b.BTNUBD = 0 AND b.canceled IS NULL)"))}")}");
            // grid filters
            TelerikGridService.ComputeFilter(whereFilter, FilterList, aliasList, args);
            // full-text search
            if (!string.IsNullOrWhiteSpace(FullTextSearch))
            {
                whereFilter.Append($@" AND (LOWER(ve1.vetdes) LIKE @ft OR 
                                            LOWER(ve2.vetdes) LIKE @ft OR 
                                            LOWER(ca.bolcod) LIKE @ft OR 
                                            LOWER(ca.boldes) LIKE @ft OR 
                                            LOWER(CONVERT(nvarchar(10), b.BTBOLL)) LIKE @ft OR
                                            LOWER(b.BTDE25) LIKE @ft OR 
                                            LOWER(b.BTDEBE) LIKE @ft OR 
                                            LOWER(CONVERT(nvarchar(10),b.BTNUBD)) LIKE @ft OR
                                            CONVERT(nvarchar(10), LOWER(c.abecod)) LIKE @ft OR 
                                            LOWER(c.abers1) LIKE @ft OR 
                                            LOWER(CONVERT(nvarchar(10), descli.codesti)) LIKE @ft OR 
                                            LOWER(descli.ragisoc) LIKE @ft OR
                                            LOWER(CONVERT(nvarchar(10), desfor.fodesti)) LIKE @ft OR 
                                            LOWER(desfor.foragso) LIKE @ft OR
                                            LOWER(CONVERT(nvarchar(20), b.BTDATP, 103)) LIKE @ft OR 
                                            LOWER(CONVERT(nvarchar(20), b.BTDATA, 103)) LIKE @ft OR 
                                            LOWER(CONVERT(nvarchar(20), b.BTDASP, 131)) LIKE @ft OR
                                            LOWER(CONVERT(nvarchar(10), b.BTCOLL)) LIKE @ft)");
            }
            #endregion
            #region Sort
            var sort = TelerikGridService.ComputeSort(SortList, aliasList);
            #endregion
            TotalCount = ((int?)connection.ExecuteScalar($@"SELECT COUNT(*) FROM BOLLT00F AS b
                                                        INNER JOIN ABE AS c ON c.abecod = b.BTCODC
                                                        LEFT JOIN DESTINATARI AS descli ON descli.cliecod = b.BTCODC AND descli.codesti = b.BTCODD
                                                        LEFT JOIN DESFOR AS desfor ON desfor.fornicod = b.BTCODC AND desfor.fodesti = b.BTCODD
                                                        INNER JOIN CAUSBOLL AS ca ON ca.bolcod = b.BTCAUS
                                                        LEFT JOIN VETTORI as ve1 ON ve1.vetcod = b.BTCORR
                                                        LEFT JOIN VETTORI as ve2 ON ve2.vetcod = b.BTCORR2
                                                        WHERE {whereFilter.ToString()};", args)) ?? 0;

            var list = connection.Query<BOLLT00F, CAUSBOLL, VETTORI, VETTORI, DESTINATARI, DESFOR, BOLLT00F>(
                $@"SELECT b.bolsoc,b.BTANNO,b.BTBOLL,b.BTNUBD,b.BTDATP,b.BTDATA,b.BTCAUS,b.BTCODC,b.BTCODD,b.BTPAGA,b.BTCONS,b.BTSPED,b.BTCORR,b.BTDE25,b.BTPESO,b.BTDASP,b.BTCOLL,b.BTAREA,b.BTDEBE,b.BTPES2,b.BTIMBA,b.abiabi,b.abicab,b.BTCORR2,b.BTBCON,b.BTSTATO,b.BTDAFA,b.BTSCCL,
                        c.abecod AS CustomerID, TRIM(c.abers1) AS CustomerDescription, ca.*, ve1.*, ve2.*, descli.*, desfor.* FROM BOLLT00F AS b
                        INNER JOIN ABE AS c ON c.abecod = b.BTCODC
                        LEFT JOIN DESTINATARI AS descli ON descli.cliecod = b.BTCODC AND descli.codesti = b.BTCODD
                        LEFT JOIN DESFOR AS desfor ON desfor.fornicod = b.BTCODC AND desfor.fodesti = b.BTCODD
                        LEFT JOIN VETTORI as ve1 ON ve1.vetcod = b.BTCORR
                        LEFT JOIN VETTORI as ve2 ON ve2.vetcod = b.BTCORR2
                        INNER JOIN CAUSBOLL AS ca ON ca.bolcod = b.BTCAUS
                        WHERE {whereFilter.ToString()}
                        {(!string.IsNullOrWhiteSpace(sort) ? sort : "ORDER BY b.BTBOLL DESC ")}
                        OFFSET @skip ROWS 
                        FETCH NEXT @ps ROWS ONLY",
                (ddt, cau, ve1, ve2, descli, desfor) =>
                {
                    ddt.Causal = cau;
                    ddt.FirstCarrier = ve1;
                    ddt.SecondCarrier = ve2;

                    if (EntityType == "C")
                    {
                        ddt.RecipientID = (descli != null) ? descli.codesti : 0;
                        ddt.RecipientDescription = (descli != null) ? (descli.ragisoc ?? string.Empty).TrimEnd() : string.Empty;
                    }
                    else
                    {
                        ddt.RecipientID = (desfor != null) ? desfor.fodesti : 0;
                        ddt.RecipientDescription = (desfor != null) ? (desfor.foragso ?? string.Empty).TrimEnd() : string.Empty;
                    }
                    return ddt;
                },
                args, splitOn: "cliecod,bolcod,vetcod,vetcod,cliecod,fornicod");
            args.Add("hlist", list.Select(s => s.BTBOLL).Distinct().ToList().ToArray<int>());
            var rows = connection.Query<BOLLD00F>(
                $@"SELECT b.* FROM BOLLD00F AS b
                        WHERE b.bolsoc = @bolsoc AND b.BTANNO = @yea AND b.BTBOLL IN @hlist",
                args);

            Parallel.ForEach(list, item =>
            {
                item.Rows = new ObservableCollection<BOLLD00F>(rows.Where(w => w.bolsoc == item.bolsoc && w.BTANNO == item.BTANNO && w.BTBOLL == item.BTBOLL).ToList());
            });

            return new ObservableCollection<BOLLT00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BOLLT00F? Get(string CompanyID, int Year, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BOLLT00F, ABE, DESTINATARI, CAUSBOLL, BOLLT00F>(
                @"SELECT b.*,
                    c.abecod AS CustomerID, TRIM(c.abers1) AS CustomerDescription,d.codesti AS RecipientID, TRIM(d.ragisoc) AS RecipientDescription,
                        c.*, d.*, ca.* FROM BOLLT00F AS b
                        LEFT JOIN ABE AS c ON c.abecod = b.BTCODC
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = b.BTCODC AND d.codesti = b.BTCODD
                        INNER JOIN CAUSBOLL AS ca ON ca.bolcod = b.BTCAUS
                        WHERE b.bolsoc = @bolsoc AND b.BTANNO = @btanno AND b.BTBOLL = @btboll",
                (ddt, abe, des, cau) => { ddt.Customer = abe; ddt.Recipient = des; ddt.Causal = cau; return ddt; },
                new { bolsoc = CompanyID, btanno = Year, btboll = ID }, splitOn: "abecod,cliecod,bolcod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<BOLLT00F>? GetFromList(string CompanyID, string[] IDList)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BOLLT00F, CAUSBOLL, BOLLT00F>(
                @"SELECT b.BTNUBD, b.BTANNO, b.BTBOLL, b.BTDATA, ca.* FROM BOLLT00F AS b
                        INNER JOIN CAUSBOLL AS ca ON ca.bolcod = b.BTCAUS
                        WHERE b.bolsoc = @bolsoc AND CONCAT(b.BTANNO,b.BTBOLL) IN @idlist",
                (ddt, cau) => { ddt.Causal = cau; return ddt; },
                new { bolsoc = CompanyID, idlist = IDList }, splitOn: "bolcod")
                .ToList();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BOLLD00F>? GetListByOrder(string OrderCompanyID, int OrderYear, int OrderNumber)
    {
        try
        {
            using var connection = GetOpenConnection();


            var heads = connection.Query<BOLLD00F>(@"SELECT DISTINCT bolsoc, BTANNO, BTBOLL, BOANNO, BONUOR from BOLLD00F
                                                        WHERE bolsoc = @ordc AND BOANNO = @ordy AND BONUOR = @ordn
                                                        ORDER BY BTANNO DESC, BTBOLL DESC",
                                                    new { ordc = OrderCompanyID, ordy = OrderYear, ordn = OrderNumber });

            return new ObservableCollection<BOLLD00F>(heads);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BOLLT00F? GetFull(string CompanyID, int Year, int ID, bool PrintProductNote, bool PrintAgentsInDetails)
    {
        try
        {
            using var connection = GetOpenConnection();

            var bolld1Repo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1Repository>();

            var unitaRepo = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>();
            var storeStoreRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>();
            var destinatariRepo = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();
            var cliammiRepo = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();


            var result = connection.Query<BOLLT00F>(
                @"SELECT h.*,
                        ca.bolcod, ca.boldes, ca.bolfac,
                        c.abecod, c.abers1
                        FROM BOLLT00F AS h
                        INNER JOIN CAUSBOLL AS ca ON ca.bolcod = h.BTCAUS
                        INNER JOIN ABE AS c ON c.abecod = h.BTCODC
                        WHERE h.bolsoc = @bolsoc AND h.BTANNO = @btanno AND h.BTBOLL = @btboll",
                 new[] { typeof(BOLLT00F), typeof(CAUSBOLL), typeof(ABE) }
                , (objects) =>
                {
                    var ddt = (BOLLT00F)objects[0];
                    ddt.Causal = objects[1] as CAUSBOLL;
                    ddt.Customer = objects[2] as ABE;
                    return ddt;
                },
                new { bolsoc = CompanyID, btanno = Year, btboll = ID },
                splitOn: "bolcod,abecod")
                .FirstOrDefault();

            if (result != null)
            {
                // retrieve default agents
                if (result.BTFLCF == "C")
                {
                    var customerData = cliammiRepo.Get(CompanyID, result.BTCODC ?? 0);
                    if (result.BTCODD.HasValue && result.BTCODD.Value > 0)
                    {
                        // check recipient
                        var recipient = destinatariRepo.Get(result.BTCODC ?? 0, result.BTCODD ?? 0);
                        result.DefaultFirstAgent = !string.IsNullOrWhiteSpace(recipient?.decoag1) ?
                            new AGENTI() { agedes = string.Empty, ageflag = string.Empty, agecod = recipient?.decoag1 ?? string.Empty, agepvg = recipient?.deage1p, agepvgt = recipient?.deage1pt } :
                        (string.IsNullOrWhiteSpace(customerData?.CLAGEN) ? null : new AGENTI() { agedes = string.Empty, ageflag = string.Empty, agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT });
                        result.DefaultSecondAgent = !string.IsNullOrWhiteSpace(recipient?.decoag2) ?
                            new AGENTI() { agedes = string.Empty, ageflag = string.Empty, agecod = recipient?.decoag2 ?? string.Empty, agepvg = recipient?.deage2p, agepvgt = recipient?.deage2pt } :
                        (string.IsNullOrWhiteSpace(customerData?.CLAGEN2) ? null : new AGENTI() { agedes = string.Empty, ageflag = string.Empty, agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT });
                    }
                    else
                    {
                        result.DefaultFirstAgent = string.IsNullOrWhiteSpace(customerData?.CLAGEN) ? null : new AGENTI() { agedes = string.Empty, ageflag = string.Empty, agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT };
                        result.DefaultSecondAgent = string.IsNullOrWhiteSpace(customerData?.CLAGEN2) ? null : new AGENTI() { agedes = string.Empty, ageflag = string.Empty, agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT };
                    }
                }

                var infiniteStore = storeStoreRepo.GetDefaultInfinite(CompanyID);
                var umsCache = unitaRepo.GetSimpleList(CompanyID);
                result.Rows = new ObservableCollection<BOLLD00F>(connection.Query<BOLLD00F, tab_articolo, AGENTI, AGENTI, BOLLD00F>(
                        $@"SELECT r.*, 
                            {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, 
                            p.ID,  p.Descrizione, p.UnitaID, p.UnitaIDAlt, p.QuantitaDefault, p.GroupID, p. AccountID, p.SubaccountID, p.asscod, p.assali, p.IsInfinite, p.HasLots,
                            a1.agecod, a1.agedes, 
                            a2.agecod, a2.agedes 
                            FROM BOLLD00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.bolsoc AND p.ID = r.BOCODA
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.bolsoc AND u.ID = r.BOUNIM
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.BOCOA1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.BOCOA2
                            WHERE r.bolsoc = @bolsoc AND r.BTANNO = @BTANNO AND r.BTBOLL = @BTBOLL",
                        (row, prd, age1, age2) =>
                        {
                            row.UMsCache = umsCache;
                            row.Product = prd;
                            row.FirstAgent = age1;
                            row.SecondAgent = age2;
                            row.PrintAgentsInDetails = PrintAgentsInDetails && (age1 != null || age2 != null);
                            row.CustomerDiscount = result.BTSCCL ?? 0;
                            row.EngagesRows = bolld1Repo.GetListByDDTRow(row.bolsoc, row.BTANNO, row.BTBOLL, row.BORIGB, row.Product, infiniteStore);
                            return row;
                        },
                        new { bolsoc = CompanyID, BTANNO = Year, BTBOLL = ID }, splitOn: "ID,agecod,agecod").ToList());
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public decimal GetDDTAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query = @$"SELECT d.*, t.* FROM BOLLD00F AS d
                       INNER JOIN BOLLT00F AS t ON t.bolsoc = d.bolsoc AND t.BTANNO = d.BTANNO AND t.BTBOLL = d.BTBOLL
                       WHERE t.bolsoc=@bolsoc AND t.BTCODC=@btcodc {(Year.HasValue ? "AND t.BTANNO = @btanno" : null)} {(ToDate.HasValue ? "AND t.BTDATA<=@todate" : null)}";

            var list = connection.Query<BOLLD00F, BOLLT00F, BOLLD00F>(
                query,
                (dett, head) => { return dett; },
                new { bolsoc = CompanyID, btanno = Year.HasValue ? Year.Value : 0, btcodc = CustomerID, todate = ToDate },
                splitOn: "bolsoc");

            return list.Sum(sum => sum.NetPrice);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public bool Exists(string bolsoc, int BTANNO, int BTBOLL)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM BOLLT00F WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool ExistsDefinitive(string bolsoc, int BTANNO, int BTNUBD, int BTCODC, string BTCAUS)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM BOLLT00F 
                        WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTNUBD = @BTNUBD AND 
                        BTCODC = @BTCODC AND BTCAUS = @BTCAUS AND canceled IS NULL",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTNUBD = BTNUBD, BTCODC = BTCODC, BTCAUS = BTCAUS }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Printing
    public BOLLT00F? GetPrintFull(string CompanyID, int Year, int ID, bool PrintProductNote, bool PrintAgentsInDetails, bool UseCustomerCodes)
    {
        try
        {
            using var connection = GetOpenConnection();

            var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
            var agentiRepo = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>();
            var bolld1Repo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1Repository>();
            var storeStoreRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>();
            var uniteRepo = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>();
            var linguaRepo = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>();


            var result = connection.Query<BOLLT00F>(
                @"SELECT h.*,c.*, d.*, ca.*, s.*, de.*, df.*,iso.isolin,sl.spedes,cl.condes FROM BOLLT00F AS h
                        INNER JOIN ABE AS c ON c.abecod = h.BTCODC
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = h.BTCODC AND d.codesti = h.BTCODD
                        LEFT JOIN DESFOR AS df ON df.fornicod = h.BTCODC AND df.fodesti = h.BTCODD
                        INNER JOIN CAUSBOLL AS ca ON ca.bolcod = h.BTCAUS
                        LEFT JOIN SPEDIZIONE AS s ON s.specod = h.BTSPED
                        LEFT JOIN CONSEGNA AS de ON de.concod = h.BTCONS
                        LEFT OUTER JOIN ISO AS iso ON iso.isocod = c.isocod
                        LEFT OUTER JOIN SPEDIZIONE_LINGUA AS sl ON sl.specod = h.BTSPED AND sl.lincod = iso.isolin
                        LEFT OUTER JOIN CONSEGNA_LINGUA AS cl ON cl.concod = h.BTCONS AND cl.lincod = iso.isolin
                        WHERE h.bolsoc = @bolsoc AND h.BTANNO = @btanno AND h.BTBOLL = @btboll",
                 new[] { typeof(BOLLT00F), typeof(ABE), typeof(DESTINATARI), typeof(CAUSBOLL), typeof(SPEDIZIONE), typeof(CONSEGNA), typeof(DESFOR), typeof(string), typeof(string), typeof(string) }
                , (objects) =>
                {
                    var ddt = (BOLLT00F)objects[0];
                    ddt.Customer = objects[1] as ABE;
                    ddt.Recipient = objects[2] as DESTINATARI;
                    ddt.Causal = objects[3] as CAUSBOLL;
                    ddt.Shipment = objects[4] as SPEDIZIONE;
                    if (ddt.Shipment != null)
                        ddt.Shipment.spedes = (!string.IsNullOrEmpty(objects[8]?.ToString()) ? objects[8].ToString() : ddt.Shipment.spedes) ?? string.Empty;
                    ddt.Delivery = objects[5] as CONSEGNA;
                    if (ddt.Delivery != null)
                        ddt.Delivery.condes = (!string.IsNullOrEmpty(objects[9]?.ToString()) ? objects[9].ToString() : ddt.Delivery.condes) ?? string.Empty;
                    ddt.Language = objects[7] as string;

                    if (ddt.BTFLCF == "C")
                    {
                        ddt.Recipient = objects[2] as DESTINATARI;
                    }
                    else
                    {
                        var recipientSupplier = objects[6] as DESFOR;

                        if (recipientSupplier != null)
                        {
                            ddt.Recipient = new DESTINATARI
                            {
                                cliecod = recipientSupplier.fornicod,
                                codesti = recipientSupplier.fodesti,
                                ragisoc = recipientSupplier.foragso ?? string.Empty,
                                DEINDI = recipientSupplier.fodein,
                                DECAP = recipientSupplier.fodecap,
                                deloc = recipientSupplier.fodeloc,
                                depro = recipientSupplier.fodepro,
                                person = recipientSupplier.foperco,
                            };
                        }
                    }
                    return ddt;
                },
                new { bolsoc = CompanyID, btanno = Year, btboll = ID },
                splitOn: "abecod,cliecod,bolcod,specod,concod,fornicod,isolin,spedes,condes")
                .FirstOrDefault();

            if (result != null)
            {
                var languageDictionary = linguaRepo.GetDictionary(result.Language ?? string.Empty);

                var carrier1 = connection.Query<VETTORI>(
                    @"SELECT * FROM VETTORI
                       WHERE vetcod = @vetcod",
                    new { vetcod = result.BTCORR }).FirstOrDefault();
                if (carrier1 == null)
                {
                    carrier1 = new VETTORI()
                    {
                        vetdes = "CONDUCENTE",
                        vetind = "#"
                    };
                }
                result.FirstCarrier = carrier1;

                result.SecondCarrier = connection.Query<VETTORI>(
                    @"SELECT * FROM VETTORI
                       WHERE vetcod = @vetcod",
                    new { vetcod = result.BTCORR2 }).FirstOrDefault();

                var infiniteStore = storeStoreRepo.GetDefaultInfinite(CompanyID);
                var umsCache = uniteRepo.GetSimpleList(CompanyID);
                result.Rows = new ObservableCollection<BOLLD00F>(connection.Query<BOLLD00F, tab_articolo, tab_articolo_unita, AGENTI, AGENTI, ORDIT00F, tab_articolo_lingua, BOLLD00F>(
                        $@"SELECT r.*, 
                            (
                                SELECT TOP(1) TRIM(customerProductID) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.bolsoc AND l.productID = r.BOCODA AND l.customerID = h.BTCODC AND l.unit_id=r.BOUNIM AND
                                CAST(l.fromDate AS date) <= h.BTDATP AND CAST(l.toDate AS date) >= h.BTDATP AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductID,
                            (
                                SELECT TOP(1) TRIM(customerProductDescription) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.bolsoc AND l.productID = r.BOCODA AND l.customerID = h.BTCODC AND l.unit_id=r.BOUNIM AND
                                CAST(l.fromDate AS date) <= h.BTDATP AND CAST(l.toDate AS date) >= h.BTDATP AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductDescription,
                            (
                                SELECT TOP(1) TRIM(supplierProductID) FROM SRM_LISFOR AS l
                                WHERE l.companyID = r.bolsoc AND l.productID = r.BOCODA AND l.supplierID = h.BTCODC AND l.unit_id=r.BOUNIM AND
                                CAST(l.fromDate AS date) <= h.BTDATP AND CAST(l.toDate AS date) >= h.BTDATP AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS SupplierProductID,
                            (
                                SELECT TOP(1) TRIM(supplierProductDescription) FROM SRM_LISFOR AS l
                                WHERE l.companyID = r.bolsoc AND l.productID = r.BOCODA AND l.supplierID = h.BTCODC AND l.unit_id=r.BOUNIM AND
                                CAST(l.fromDate AS date) <= h.BTDATP AND CAST(l.toDate AS date) >= h.BTDATP AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS SupplierProductDescription,
                            {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes,ord.OTSOCI,ord.OTDAOR,ord.OTDE25,ord.OTCUNO,ord.OTCUDO, l.* FROM BOLLD00F AS r
                            INNER JOIN BOLLT00F AS h ON h.bolsoc=r.bolsoc AND h.BTANNO = r.BTANNO AND h.BTBOLL=r.BTBOLL
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.bolsoc AND p.ID = r.BOCODA
                            LEFT OUTER JOIN tab_articolo_lingua AS l ON p.SocietaID = l.SocietaID AND p.ID = l.ArticoloID AND l.lincod = @Lingua
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.bolsoc AND u.ID = r.BOUNIM
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.BOCOA1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.BOCOA2
                            LEFT JOIN ORDIT00F as ord ON ord.OTSOCI=r.bolsoc AND ord.OTANNO = r.BOANNO AND ord.OTNUOR = r.BONUOR
                            WHERE r.bolsoc = @bolsoc AND r.BTANNO = @BTANNO AND r.BTBOLL = @BTBOLL
                            ORDER BY r.BOANNO, r.BONUOR",
                        (row, prd, um, age1, age2, ord, lingua) =>
                        {
                            row.UMsCache = umsCache;
                            row.Product = prd;
                            row.Product.Descrizione = (lingua != null && !string.IsNullOrEmpty(lingua.Descrizione) ? lingua.Descrizione : prd.Descrizione);
                            row.Product.Note = (lingua != null && !string.IsNullOrEmpty(lingua.Note) ? lingua.Note : prd.Note);
                            row.UM = um;
                            row.FirstAgent = age1;
                            row.SecondAgent = age2;
                            row.PrintAgentsInDetails = PrintAgentsInDetails && (age1 != null || age2 != null);
                            row.EngagesRows = bolld1Repo.GetListByDDTRow(row.bolsoc, row.BTANNO, row.BTBOLL, row.BORIGB, prd, infiniteStore);
                            row.CustomerDiscount = result.BTSCCL ?? 0;
                            row.LinkedOrder = ord;
                            return row;
                        },
                        new { bolsoc = CompanyID, BTANNO = Year, BTBOLL = ID, Lingua = result.Language }, splitOn: "SocietaID,SocietaID,agecod,agecod,OTSOCI,SocietaID").ToList());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent
                    var agent1 = agentiRepo.Get(result.Rows.OrderBy(o => o.BORIGA).FirstOrDefault()?.BOCOA1 ?? string.Empty);
                    var agent2 = agentiRepo.Get(result.Rows.OrderBy(o => o.BORIGA).FirstOrDefault()?.BOCOA2 ?? string.Empty);
                    result.DefaultFirstAgent = agent1;
                    result.DefaultSecondAgent = agent2;
                }

                // check if custom code and UMs recap for each row
                int? lastYear = null;
                int? lastNumber = null;
                var customization = aziendaRepo.Get(CompanyID);
                foreach (var row in result.Rows)
                {
                    #region Order grouping
                    if (lastYear != row.BOANNO || lastNumber != row.BONUOR)
                    {
                        row.OrderReferenceText = $"{languageDictionary?["RiferimentoODA"].ToString()} {row.BOANNO}/{row.BONUOR} {languageDictionary?["Del"].ToString()} {(row.LinkedOrder?.OTDAOR ?? DateTime.MinValue).ToString("dd/MM/yyyy")}";
                        // check if customization need to print customer ref from order
                        if (customization?.azddtricl ?? false)
                        {
                            if (!string.IsNullOrWhiteSpace(row.LinkedOrder?.OTCUNO) && row.LinkedOrder.OTCUDO.HasValue)
                                row.OrderReferenceText += $" [{languageDictionary?["VostroOrdine"].ToString()}{row.LinkedOrder.OTCUNO.Trim()} {languageDictionary?["Del"].ToString()} {(row.LinkedOrder.OTCUDO ?? DateTime.MinValue).ToString("dd/MM/yyyy")}]";
                            else
                                row.OrderReferenceText += $" [{row.LinkedOrder?.OTDE25?.Trim()}]";
                        }
                        lastYear = row.BOANNO;
                        lastNumber = row.BONUOR;
                    }
                    #endregion
                    #region Customer code
                    if (!string.IsNullOrWhiteSpace(row.CustomerProductID) && !string.IsNullOrWhiteSpace(row.CustomerProductDescription))
                    {
                        if (!UseCustomerCodes)
                        {
                            row.CustomerCode = $"Codifica cliente: {(!string.IsNullOrWhiteSpace(row.CustomerProductID) ? row.CustomerProductID : null)} {(!string.IsNullOrWhiteSpace(row.CustomerProductDescription) ? row.CustomerProductDescription : null)}";
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(row.CustomerProductID))
                                row.BOCODA = row.CustomerProductID;
                            if (row.Product != null && !string.IsNullOrWhiteSpace(row.CustomerProductDescription))
                                row.Product.Descrizione = row.CustomerProductDescription;
                        }
                    }
                    else
                    {
                        row.CustomerCode = null;
                    }
                    #endregion

                    #region Supplier code
                    if (!string.IsNullOrWhiteSpace(row.SupplierProductID) && !string.IsNullOrWhiteSpace(row.SupplierProductDescription))
                    {
                        if (!string.IsNullOrEmpty(row.SupplierProductID))
                            row.BOCODA = row.SupplierProductID;
                        if (row.Product != null && !string.IsNullOrWhiteSpace(row.SupplierProductDescription))
                            row.Product.Descrizione = row.SupplierProductDescription;
                    }
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

    public DDTReport? PrintDDT(BOLLT00F DDT)
    {
        try
        {
            var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
            var aziendaLinguaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>();
            var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
            var companyRepo = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>();
            var linguaRepo = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>();
            foreach (var row in DDT.Rows ?? new ObservableCollection<BOLLD00F>())
            {
                // set foot notes on rows to print it
                if (DDT.BTSHOWP)
                    row.HeaderFootNote = DDT.BTNOTEP;
                // set quantities
                var orderRow = ordidRepo.Get(row.bolsoc, row.BOANNO ?? 0, row.BONUOR ?? 0, row.BORIGA ?? 0);
                if (orderRow != null)
                {
                    row.OrderedQuantity = orderRow.ODQTAV;
                    row.ResidualQuantity = orderRow.ODQTAV - (orderRow.ODQTAEV ?? 0);
                }
            }
            var socbase = companyRepo.Get(DDT.bolsoc)!;
            // get customizations
            var azienda = aziendaRepo.Get(DDT.bolsoc)!;
            var aziendaLingua = aziendaLinguaRepo.Get(DDT.bolsoc, DDT.Language ?? string.Empty);

            var languageDictionary = linguaRepo.GetDictionary(DDT.Language ?? string.Empty);

            object? objectDictionary = null;
            if (languageDictionary != null)
                objectDictionary = new LocalizationHelper().CreateClassFromDictionary(languageDictionary);

            return new DDTReport()
            {
                DDT = DDT,
                CompanyInfo = aziendaRepo.Get(DDT.bolsoc),
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                CertificationsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}certs.png"),
                FixedText = (aziendaLingua != null) ? (!string.IsNullOrEmpty(aziendaLingua.azddtgtex) ? aziendaLingua.azddtgtex : azienda.azddtgtex) : azienda.azddtgtex,
                LinguaDictionary = objectDictionary,
            };
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRM
    public bool GenerateByOrder(ORDIT00F OrderHead, List<ORDID00F> SelectedRows, string UserID, string HeadNotes, string FootNotes)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var aziendaLinguaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var companyRepo = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>();
                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                var bolldRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>();
                var bolld1Repo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1Repository>();

                var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();
                var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                var proOrdineRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>();


                int sequence = 1;
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var newDDTID = numRegRepo.GetNumber(OrderHead.otsoci, now.Year, Constants.DDT_TEMP, true);

                // generate rows and update origins
                decimal grossWeight = 0;
                decimal netWeight = 0;
                foreach (var row in SelectedRows)
                {
                    var newRow = new BOLLD00F()
                    {
                        bolsoc = OrderHead.otsoci,
                        BTANNO = now.Year,
                        BTBOLL = newDDTID,
                        BORIGB = sequence++,
                        BOCODA = row.ODCODA,
                        BOQTAV = row.QuantityToSend,
                        BOTQTA = row.ODTQTA,
                        boprez = row.ODPREZ,
                        botpre = row.ODTPRE,
                        bosco1 = row.ODSCO1,
                        bosco2 = row.ODSCO2,
                        bosco3 = row.ODSCO3,
                        bomagg = row.ODMAGG,
                        botsc1 = row.ODTSC1,
                        botsc2 = row.ODTSC2,
                        botsc3 = row.ODTSC3,
                        botmag = row.ODTMAG,
                        boasso = row.ODASSF,
                        boaliq = row.ODALIV,
                        BODACO = row.ODDACO,
                        BORIFC = row.ODRIFC,
                        bogrup = row.ODGRUP,
                        bocont = row.ODCONT,
                        bosotc = row.ODSCTO,
                        BOUNIM = row.odunit,
                        BOANNO = row.OTANNO,
                        BONUOR = row.OTNUOR,
                        BORIGA = row.ODRIGA,
                        BONOTE = row.ODNOTE,
                        BOSHOW = row.ODSHOW,
                        BOCOA1 = row.ODCOA1,
                        BOCOA2 = row.ODCOA2,
                        BOCOA1P = row.ODCOA1P,
                        BOCOA2P = row.ODCOA2P,
                        BOCOA1PT = row.ODCOA1PT,
                        BOCOA2PT = row.ODCOA2PT,
                        BOSERI = row.ODSERI
                    };
                    connection.Execute(bolldRepo.INSERT_QUERY, newRow, transaction);
                    row.ODSTATO = row.FulfillmentID == "S" ? "*" : null;
                    row.ODQTAEV = (row.ODQTAEV ?? 0) + row.QuantityToSend;
                    connection.Execute(ordidRepo.UPDATE_QUERY, row, transaction);
                    // compute weight
                    var weights = tabArticoloRepo.ComputeWeight(row.otsoci, row.ODCODA, row.ODQTAV, row.odunit ?? string.Empty);
                    grossWeight += weights.Item1;
                    netWeight += weights.Item2;

                    // if exists pro_ordine add engaged material
                    var prodOrder = proOrdineRepo.GetByOrder(row.otsoci, row.OTANNO, row.OTNUOR, row.ODRIGA);
                    if (prodOrder != null)
                    {
                        int boposc = 1;
                        foreach (var eng in storeStockEngageRepo.GetListByOrderID(row.otsoci, prodOrder.ID) ?? new ObservableCollection<store_stocks_engage>())
                        {
                            var newDDTEng = new BOLLD00F1()
                            {
                                bolsoc = eng.company_id,
                                BTANNO = now.Year,
                                BTBOLL = newDDTID,
                                BORIGB = newRow.BORIGB,
                                boposc = boposc++,
                                bolott = !string.IsNullOrWhiteSpace(eng.lot) ? eng.lot : Constants.NO_LOT_ID,
                                boqtlo = (eng.quantity ?? 0),
                                store_id = eng.store_id,
                                product_id = eng.product_id
                            };
                            connection.Execute(bolld1Repo.INSERT_QUERY, newDDTEng, transaction);
                            // move engage to DDT
                            eng.ddt_year = now.Year;
                            eng.ddt_number = newDDTID;
                            eng.ddt_row = newRow.BORIGB;
                            eng.update_user = UserID;
                            connection.Execute(storeStockEngageRepo.UPDATE_QUERY, eng, transaction);
                        }
                    }
                    else
                    {
                        // engage resale material
                        int boposc = 1;
                        foreach (var eng in storeStockEngageRepo.GetListByCustomerOrderID(row.otsoci, row.OTANNO, row.OTNUOR, row.ODRIGA) ?? new ObservableCollection<store_stocks_engage>())
                        {
                            var newDDTEng = new BOLLD00F1()
                            {
                                bolsoc = eng.company_id,
                                BTANNO = now.Year,
                                BTBOLL = newDDTID,
                                BORIGB = newRow.BORIGB,
                                boposc = boposc++,
                                bolott = !string.IsNullOrWhiteSpace(eng.lot) ? eng.lot : Constants.NO_LOT_ID,
                                boqtlo = (eng.quantity ?? 0),
                                store_id = eng.store_id,
                                product_id = eng.product_id
                            };
                            connection.Execute(bolld1Repo.INSERT_QUERY, newDDTEng, transaction);
                            // move engage to DDT
                            eng.ddt_year = now.Year;
                            eng.ddt_number = newDDTID;
                            eng.ddt_row = newRow.BORIGB;
                            eng.update_user = UserID;
                            connection.Execute(storeStockEngageRepo.UPDATE_QUERY, eng, transaction);
                        }
                    }
                }

                // generate DDT head
                var newHead = new BOLLT00F()
                {
                    bolsoc = OrderHead.otsoci,
                    BTANNO = now.Year,
                    BTBOLL = newDDTID,
                    BTDATP = now.Date,
                    BTCAUS = OrderHead.Causal?.caubol,
                    BTCODC = OrderHead.OTCLIE,
                    BTCODD = OrderHead.DESTIN,
                    BTPAGA = OrderHead.OTPAGA,
                    abiabi = OrderHead.abiabi,
                    abicab = OrderHead.abicab,
                    BTCONS = OrderHead.OTCONS,
                    BTSPED = OrderHead.OTSPED,
                    BTCORR = OrderHead.OTCORR,
                    BTCORR2 = OrderHead.OTCORR2,
                    BTDASP = now,
                    BTIMBA = OrderHead.OTIMBA,
                    BTLING = OrderHead.otling,
                    addedUserID = UserID,
                    BTNOTET = HeadNotes,
                    BTNOTEP = FootNotes,
                    BTSHOWT = !string.IsNullOrWhiteSpace(HeadNotes),
                    BTSHOWP = !string.IsNullOrWhiteSpace(FootNotes),
                    BTDE25 = OrderHead.OTDE25,
                    BTBCON = OrderHead.OTBCON,
                    BTSCCL = OrderHead.OTSCCL,
                    BTCOLL = SelectedRows.Count,
                    BTAREA = OrderHead.OTAREA,
                    BTRIVE = OrderHead.OTRIVE,
                    BTCONZ = OrderHead.OTCONZ,
                    BTFILI = OrderHead.OTFILI,
                    BTZONA = OrderHead.OTZONA,
                    BTSETM = OrderHead.OTSETM,
                    BTREGI = OrderHead.OTREGI,
                    BTPESO = grossWeight,
                    BTPES2 = netWeight,
                    BTFLCF = "C"
                };
                connection.Execute(INSERT_QUERY, newHead, transaction);

                transaction.Commit();
                InfoHandler.Show($"Generato correttamente il DDT {now.Year}/{newDDTID}");
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
            return true;
        }
    }

    public bool CancelAndFreeLinkedOrders(BOLLT00F DDT, string UserID, string CancelReason)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var orditRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>();

                // check linked orders
                foreach (var row in (DDT.Rows ?? new ObservableCollection<BOLLD00F>()).Where(w => w.BOANNO.HasValue && w.BONUOR.HasValue && w.BORIGA.HasValue))
                {
                    var orderRow = ordidRepo.Get(row.bolsoc, row.BOANNO!.Value, row.BONUOR!.Value, row.BORIGA!.Value);
                    if (orderRow != null)
                    {
                        orderRow.ODSTATO = null;
                        orderRow.ODQTAEV -= row.BOQTAV;
                        if (orderRow.ODQTAEV == 0)
                            orderRow.ODQTAEV = null;
                        connection.Execute(ordidRepo.UPDATE_QUERY, orderRow, transaction);
                    }
                }

                // update status
                DDT.BTSTATO = "X";
                DDT.canceledUserID = UserID;
                DDT.canceled = now;
                DDT.canceledNote = CancelReason;
                connection.Execute(UPDATE_QUERY, DDT, transaction);

                transaction.Commit();

                // check orders heads new status
                foreach (var row in (DDT.Rows ?? new ObservableCollection<BOLLD00F>()).Select(s => new { s.BOANNO, s.BONUOR })
                                            .Where(w => w.BOANNO.HasValue && w.BONUOR.HasValue)
                                            .Distinct())
                {
                    orditRepo.FlagFulfillment(DDT.bolsoc, row.BOANNO!.Value, row.BONUOR!.Value, UserID);
                }

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
            return true;
        }
    }

    public bool UpdateDefinitiveDDTAndUnloadEngages(BOLLT00F DDT, store_causals StoreCausal, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();

                // update head info
                connection.Execute(UPDATE_QUERY, DDT, transaction);
                if (StoreCausal != null)
                {
                    // unload engages
                    var engagesList = storeStockEngageRepo.GetListByDDT(DDT.bolsoc, DDT.BTANNO, DDT.BTBOLL);
                    foreach (var eng in engagesList ?? new ObservableCollection<store_stocks_engage>())
                    {
                        // unload engage
                        eng.date_unloaded = now;
                        storeStockEngageRepo.Unload(eng, StoreCausal, eng.company_id, UserID, connection, transaction);
                    }
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
            return true;
        }
    }
    #endregion

    #region Import
    public bool ImportGILAT(List<string> Rows, string CausalID, string RateCode, string RateValue, string ExternCode, string CompanyID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var abeRepo = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>();
                var cliammiRepo = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                var abeExternDestRepo = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();
                var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();
                var tabArticoloExternRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_externRepository>();

                var crmLisCliRepo = VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>();
                var crmLisGenRepo = VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>();

                var bolldRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>();
                var pagCliRepo = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>();

                var tipiIncRepo = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>();

                // extracted data
                var ddtID = Rows.First().Substring(26, 6);
                var ddtDate = Rows.First().Substring(18, 8);
                var year = int.Parse(Rows.First().Substring(18, 4));
                var ddtDatetime = new DateTime(int.Parse(ddtDate.Substring(0, 4)), int.Parse(ddtDate.Substring(4, 2)), int.Parse(ddtDate.Substring(6, 2)));
                var customerID = Rows.First().Substring(0, 12);
                var destinationID = Rows.First().Substring(12, 6);
                var extCustomerData = abeExternDestRepo.GetByExtern(ExternCode, customerID, destinationID);
                var abe = abeRepo.Get(extCustomerData?.abecod ?? 0);
                var cliammi = cliammiRepo.Get(CompanyID, extCustomerData?.abecod ?? 0);

                int sequence = 1;
                var newID = numRegRepo.GetNumber(CompanyID, year, Constants.DDT_TEMP, true);

                decimal grossWeight = 0;
                decimal netWeight = 0;
                foreach (var row in Rows)
                {
                    // extracted data
                    var productID = row.Substring(32, 5);
                    var extProduct = tabArticoloExternRepo.GetByExtern(CompanyID, ExternCode, productID);
                    var qty = row.Substring(38, 11);
                    var qtyDecimal = decimal.Parse(qty.Insert(8, "."), new CultureInfo("en-US"));
                    var um = row.Substring(49, 2);

                    var product = tabArticoloRepo.GetSingle(CompanyID, extProduct?.extpid ?? string.Empty);
                    // search for price
                    GenericPriceInfo? priceInfo = crmLisCliRepo.GetCurrent(CompanyID, product?.ID ?? string.Empty, abe?.abecod ?? 0, extCustomerData?.abedestid, ddtDatetime, qtyDecimal, product?.UnitaID ?? string.Empty);
                    if (priceInfo == null)
                    {
                        priceInfo = crmLisGenRepo.GetCurrent(CompanyID, product?.ID ?? string.Empty, ddtDatetime, qtyDecimal, product?.UnitaID ?? string.Empty);
                    }
                    var newRow = new BOLLD00F()
                    {
                        bolsoc = CompanyID,
                        BTANNO = year,
                        BTBOLL = newID,
                        BORIGB = sequence++,
                        BOCODA = extProduct?.extpid,
                        BOQTAV = qtyDecimal,
                        BOTQTA = row.Trim().Length == 52 ? "O" : "V",
                        boasso = row.Trim().Length == 52 ? (RateCode) : product?.asscod,
                        boaliq = row.Trim().Length == 52 ? (RateValue) : product?.assali,
                        BODACO = ddtDatetime,
                        BORIFC = "GILAT",
                        bogrup = product?.GroupID,
                        bocont = product?.AccountID,
                        bosotc = product?.SubaccountID,
                        boprez = priceInfo != null ? priceInfo.Price : 0,
                        botpre = "U",
                        bosco1 = priceInfo != null ? priceInfo.Discount1 : null,
                        bosco2 = priceInfo != null ? priceInfo.Discount2 : null,
                        bosco3 = priceInfo != null ? priceInfo.Discount3 : null,
                        botsc1 = priceInfo != null ? priceInfo.DiscountType1 : null,
                        botsc2 = priceInfo != null ? priceInfo.DiscountType2 : null,
                        botsc3 = priceInfo != null ? priceInfo.DiscountType3 : null,
                        bomagg = priceInfo != null ? priceInfo.Surcharge : null,
                        botmag = priceInfo != null ? priceInfo.SurchargeType : null,
                        BOUNIM = product?.UnitaID,
                        BONOTE = "Generazione automatica da importazione GILAT",
                        BOSHOW = false
                    };
                    connection.Execute(bolldRepo.INSERT_QUERY, newRow, transaction);
                    // compute weight
                    grossWeight += qtyDecimal;
                    netWeight += qtyDecimal;
                }

                // generate DDT head
                var payment = pagCliRepo.Get(cliammi?.pclcod ?? string.Empty);
                var paymentType = tipiIncRepo.Get(payment?.pcltip ?? string.Empty);
                var newHead = new BOLLT00F()
                {
                    BTSTATO = "R",
                    bolsoc = CompanyID,
                    BTANNO = year,
                    BTBOLL = newID,
                    BTNUBD = int.Parse(ddtID),
                    BTDATA = ddtDatetime,
                    BTDATP = ddtDatetime,
                    BTCAUS = CausalID,
                    BTCODC = extCustomerData?.abecod,
                    BTCODD = extCustomerData?.abedestid,
                    BTPAGA = cliammi?.pclcod,
                    abiabi = paymentType?.icssup == "R" ? cliammi?.CLABI : cliammi?.banabi,
                    abicab = paymentType?.icssup == "R" ? cliammi?.CLCAB : cliammi?.bancab,
                    BTBCON = paymentType?.icssup == "R" ? null : cliammi?.bancoc,
                    BTCONS = cliammi?.concod,
                    BTSPED = cliammi?.specod,
                    BTCORR = cliammi?.vetcod,
                    BTDASP = ddtDatetime,
                    BTIMBA = cliammi?.CLIMBA,
                    addedUserID = UserID,
                    BTNOTET = "Generazione automatica da importazione GILAT",
                    BTNOTEP = null,
                    BTSHOWT = false,
                    BTSHOWP = false,
                    BTCOLL = Rows.Count,
                    BTAREA = cliammi?.arecod,
                    BTRIVE = cliammi?.rivcod,
                    BTFILI = cliammi?.filcod,
                    BTZONA = cliammi?.CLZONE,
                    BTSETM = cliammi?.CLSETM,
                    BTREGI = cliammi?.CLREGI,
                    BTPESO = grossWeight,
                    BTPES2 = netWeight,
                    BTFLCF = "C"
                };
                connection.Execute(INSERT_QUERY, newHead, transaction);
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
            return true;
        }
    }

    public bool ImportBANCOLAT(List<string> Rows, string CausalID, string ExternCode, string CompanyID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var abeRepo = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>();
                var cliammiRepo = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                var abeExternDestRepo = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();
                var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();
                var tabArticoloExternRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_externRepository>();

                var crmLisCliRepo = VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>();
                var crmLisGenRepo = VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>();

                var bolldRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>();
                var pagCliRepo = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>();
                var aliquoteRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();
                var tipiIncRepo = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>();

                // extracted data
                var ddtID = Rows.First().Substring(26, 6);
                var ddtDate = Rows.First().Substring(18, 8);
                var year = int.Parse(Rows.First().Substring(18, 4));
                var ddtDatetime = new DateTime(int.Parse(ddtDate.Substring(0, 4)), int.Parse(ddtDate.Substring(4, 2)), int.Parse(ddtDate.Substring(6, 2)));
                var customerID = Rows.First().Substring(0, 12);
                var destinationID = Rows.First().Substring(12, 6);
                var extCustomerData = abeExternDestRepo.GetByExtern(ExternCode, customerID, destinationID);
                var abe = abeRepo.Get(extCustomerData?.abecod ?? 0);
                var cliammi = cliammiRepo.Get(CompanyID, extCustomerData?.abecod ?? 0);

                int sequence = 1;
                var newID = numRegRepo.GetNumber(CompanyID, year, Constants.DDT_TEMP, true);

                decimal grossWeight = 0;
                decimal netWeight = 0;
                foreach (var row in Rows)
                {
                    // extracted data
                    var productID = row.Substring(32, 5);
                    var extProduct = tabArticoloExternRepo.GetByExtern(CompanyID, ExternCode, productID);
                    var qty = row.Substring(38, 11);
                    var qtyDecimal = decimal.Parse(qty.Insert(8, "."), new CultureInfo("en-US"));
                    var um = row.Substring(49, 2);

                    var product = tabArticoloRepo.GetSingle(CompanyID, extProduct?.extpid ?? string.Empty);
                    var causal = aliquoteRepo.Get(product?.asscod ?? string.Empty, product?.assali ?? string.Empty);
                    // search for price
                    GenericPriceInfo? priceInfo = crmLisCliRepo.GetCurrent(CompanyID, product?.ID ?? string.Empty, abe?.abecod ?? 0, extCustomerData?.abedestid, ddtDatetime, qtyDecimal, product?.UnitaID ?? string.Empty);
                    if (priceInfo == null)
                    {
                        priceInfo = crmLisGenRepo.GetCurrent(CompanyID, product?.ID ?? string.Empty, ddtDatetime, qtyDecimal, product?.UnitaID ?? string.Empty);
                    }
                    var newRow = new BOLLD00F()
                    {
                        bolsoc = CompanyID,
                        BTANNO = year,
                        BTBOLL = newID,
                        BORIGB = sequence++,
                        BOCODA = extProduct?.extpid,
                        BOQTAV = qtyDecimal,
                        BOTQTA = row.Trim().Length == 52 ? "O" : "V",
                        boasso = causal?.assventcod,
                        boaliq = causal?.assventali,
                        BODACO = ddtDatetime,
                        BORIFC = "BANCOLAT",
                        bogrup = product?.GroupID,
                        bocont = product?.AccountID,
                        bosotc = product?.SubaccountID,
                        boprez = priceInfo != null ? priceInfo.Price : 0,
                        botpre = "U",
                        bosco1 = priceInfo != null ? priceInfo.Discount1 : null,
                        bosco2 = priceInfo != null ? priceInfo.Discount2 : null,
                        bosco3 = priceInfo != null ? priceInfo.Discount3 : null,
                        botsc1 = priceInfo != null ? priceInfo.DiscountType1 : null,
                        botsc2 = priceInfo != null ? priceInfo.DiscountType2 : null,
                        botsc3 = priceInfo != null ? priceInfo.DiscountType3 : null,
                        bomagg = priceInfo != null ? priceInfo.Surcharge : null,
                        botmag = priceInfo != null ? priceInfo.SurchargeType : null,
                        BOUNIM = product?.UnitaID,
                        BONOTE = "Generazione automatica da importazione BANCOLAT",
                        BOSHOW = false
                    };
                    connection.Execute(bolldRepo.INSERT_QUERY, newRow, transaction);
                    // compute weight
                    grossWeight += qtyDecimal;
                    netWeight += qtyDecimal;
                }

                // generate DDT head
                var payment = pagCliRepo.Get(cliammi?.pclcod ?? string.Empty);
                var paymentType = tipiIncRepo.Get(payment?.pcltip ?? string.Empty);
                var newHead = new BOLLT00F()
                {
                    BTSTATO = "R",
                    bolsoc = CompanyID,
                    BTANNO = year,
                    BTBOLL = newID,
                    BTNUBD = int.Parse(ddtID),
                    BTDATA = ddtDatetime,
                    BTDATP = ddtDatetime,
                    BTCAUS = CausalID,
                    BTCODC = extCustomerData?.abecod,
                    BTCODD = extCustomerData?.abedestid,
                    BTPAGA = cliammi?.pclcod,
                    abiabi = paymentType?.icssup == "R" ? cliammi?.CLABI : cliammi?.banabi,
                    abicab = paymentType?.icssup == "R" ? cliammi?.CLCAB : cliammi?.bancab,
                    BTBCON = paymentType?.icssup == "R" ? null : cliammi?.bancoc,
                    BTCONS = cliammi?.concod,
                    BTSPED = cliammi?.specod,
                    BTCORR = cliammi?.vetcod,
                    BTDASP = ddtDatetime,
                    BTIMBA = cliammi?.CLIMBA,
                    addedUserID = UserID,
                    BTNOTET = "Generazione automatica da importazione BANCOLAT",
                    BTNOTEP = null,
                    BTSHOWT = false,
                    BTSHOWP = false,
                    BTCOLL = Rows.Count,
                    BTAREA = cliammi?.arecod,
                    BTRIVE = cliammi?.rivcod,
                    BTFILI = cliammi?.filcod,
                    BTZONA = cliammi?.CLZONE,
                    BTSETM = cliammi?.CLSETM,
                    BTREGI = cliammi?.CLREGI,
                    BTPESO = grossWeight,
                    BTPES2 = netWeight,
                    BTFLCF = "C"
                };
                connection.Execute(INSERT_QUERY, newHead, transaction);
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
            return true;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO BOLLT00F (bolsoc,BTANNO,BTBOLL,BTNUBD,BTDATP,BTDATA,BTCAUS,BTCODC,BTCODD,BTPAGA,BTCONS,BTSPED,BTCORR,BTDE25,BTPESO,BTDASP,BTCOLL,BTAREA,BTDEBE,BTPES2,BTIMBA,abiabi,abicab,BTNOTET,BTNOTEP,BTSHOWT,BTSHOWP,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,BTCORR2,BTBCON,BTCONZ,BTSTATO,BTDAFA,BTFILI,BTSCCL,BTZONA,BTSETM,BTREGI,BTRIVE,BTLING,BTFLCF) OUTPUT INSERTED.rv VALUES(@bolsoc,@BTANNO,@BTBOLL,@BTNUBD,@BTDATP,@BTDATA,@BTCAUS,@BTCODC,@BTCODD,@BTPAGA,@BTCONS,@BTSPED,@BTCORR,@BTDE25,@BTPESO,@BTDASP,@BTCOLL,@BTAREA,@BTDEBE,@BTPES2,@BTIMBA,@abiabi,@abicab,@BTNOTET,@BTNOTEP,@BTSHOWT,@BTSHOWP,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@BTCORR2,@BTBCON,@BTCONZ,@BTSTATO,@BTDAFA,@BTFILI,@BTSCCL,@BTZONA,@BTSETM,@BTREGI,@BTRIVE,@BTLING,@BTFLCF)";
    public string UPDATE_QUERY => "UPDATE BOLLT00F SET bolsoc = @bolsoc,BTANNO = @BTANNO,BTBOLL = @BTBOLL,BTNUBD = @BTNUBD,BTDATP = @BTDATP,BTDATA = @BTDATA,BTCAUS = @BTCAUS,BTCODC = @BTCODC,BTCODD = @BTCODD,BTPAGA = @BTPAGA,BTCONS = @BTCONS,BTSPED = @BTSPED,BTCORR = @BTCORR,BTDE25 = @BTDE25,BTPESO = @BTPESO,BTDASP = @BTDASP,BTCOLL = @BTCOLL,BTAREA = @BTAREA,BTDEBE = @BTDEBE,BTPES2 = @BTPES2,BTIMBA = @BTIMBA,abiabi = @abiabi,abicab = @abicab,BTNOTET = @BTNOTET,BTNOTEP = @BTNOTEP,BTSHOWT = @BTSHOWT,BTSHOWP = @BTSHOWP,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,BTCORR2 = @BTCORR2,BTBCON = @BTBCON,BTCONZ = @BTCONZ,BTSTATO = @BTSTATO,BTDAFA = @BTDAFA,BTFILI = @BTFILI,BTSCCL = @BTSCCL,BTZONA = @BTZONA,BTSETM = @BTSETM,BTREGI = @BTREGI,BTRIVE = @BTRIVE,BTLING = @BTLING,BTFLCF = @BTFLCF OUTPUT INSERTED.rv WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM BOLLT00F OUTPUT DELETED.rv WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND rv = @rv";

    public bool Insert(BOLLT00F Model)
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

    public bool Update(BOLLT00F Model)
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

    public bool Delete(BOLLT00F Model)
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

    public string? Validate(BOLLT00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.bolsoc) && IsInsert && !Exists(Model.bolsoc, Model.BTANNO, Model.BTBOLL)) || !IsInsert)
            {
                if (Model.BTCODC.HasValue && Model.BTCODC.Value > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.BTCAUS))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.BTIMBA))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.BTSPED))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.BTCONS))
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.BTDEBE))
                                    {
                                        if (Model.BTFLCF != "C" || !string.IsNullOrWhiteSpace(Model.BTPAGA))
                                        {
                                            if (Model.BTCOLL > 0)
                                            {
                                                return null;
                                            }
                                            else
                                            {
                                                return "Il numero dei colli deve essere maggiore di 0";
                                            }
                                        }
                                        else
                                        {
                                            return "Il tipo pagamento è un dato obbligatorio";
                                        }
                                    }
                                    else
                                    { return "L'aspetto esteriore dei beni è un dato obbligatorio"; }
                                }
                                else
                                { return "La consegna è un dato obbligatorio"; }
                            }
                            else
                            { return "La spedizione è un dato obbligatorio"; }
                        }
                        else
                        { return "L'imballo è un dato obbligatorio"; }
                    }
                    else
                    { return "La causale e' obbligatoria"; }
                }
                else
                { return "Il cliente è obbligatorio"; }
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