

using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.CRM;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Reports.CRM;
using VulpesX.Services.Tables.CRM;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.CRM;

public interface IOFFET00FRepository
{
    ObservableCollection<OFFET00F>? GetList(string CompanyID, int Year, string Status);

    ObservableCollection<OFFET00F>? GetList(string CompanyID, int Year, string Status, int CustomerID);

    OFFET00F? Get(string oftsoci, int OFTANNO, int OFTNUOR);

    OFFET00F? GetFull(string oftsoci, int OFTANNO, int OFTNUOR);

    bool CloseOrders(List<OFFET00F> SelectedOffers, string CausalID, string Note, string Username);

    bool Exists(string oftsoci, int OFTANNO, int OFTNUOR);

    #region Printing
    OFFET00F? GetPrintFull(string oftsoci, int OFTANNO, int OFTNUOR, bool UseCustomerCodes, bool PrintProductNote, bool PrintAgentsInDetails);
    OfferReport? PrintOffer(OFFET00F Offer);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(OFFET00F Model);
    bool Update(OFFET00F Model);

    bool Delete(OFFET00F Model);

    string? Validate(OFFET00F Model, bool IsInsert);
    #endregion
}

public class OFFET00FRepository : RepositoryBase, IOFFET00FRepository
{
    public OFFET00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public const string ERR_CANNOT_DELETE = "Impossibile eliminare un'offerta chiusa, con delle righe parzialmente trasformate in ordine e/o chiuse o gia' annullata";
    public ObservableCollection<OFFET00F>? GetList(string CompanyID, int Year, string Status)
    {
        try
        {
            using var connection = GetOpenConnection();


            var query = $@"SELECT o.OFTANNO, o.OFTNUOR, o.OFTDAOR, o.OFTCOCL, o.OFTCAUS, o.OFTOGG, o.OFTDEST, o.oflgchi, o.oftsoci, o.OFTINFI, o.OFTINFIUSR, o.OFTFITE, o.OFTFITEUSR, o.OFTFICO, o.OFTFICOUSR, o.OFTSCCL, o.canceled,
                                c.abecod, c.abers1, c.abers2, c.abecfe,
                                d.cliecod, d.codesti, d.ragisoc, 
                                ca.* 
                                FROM OFFET00F AS o
                        LEFT JOIN ABE AS c ON c.abecod = o.OFTCOCL
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OFTCOCL AND d.codesti = o.OFTDEST
                        INNER JOIN TAB_CRM_CAUOFF AS ca ON ca.offcod = o.OFTCAUS
                        WHERE o.oftsoci = @oftsoci AND o.OFTANNO = @yea AND
                        {(Status == "X" ? " o.canceled IS NOT NULL " : $" oflgchi {(Status == "*" ? " <> @oflgchi " : " = @oflgchi AND o.canceled IS NULL ")}")}
                        ORDER BY o.OFTNUOR DESC";

            var list = connection.Query<OFFET00F, ABE, DESTINATARI, TAB_CRM_CAUOFF, OFFET00F>(
                query,
                (ord, abe, des, cau) => { ord.Customer = abe; ord.Recipient = des; ord.Causal = cau; return ord; },
                new { oftsoci = CompanyID, yea = Year, oflgchi = Status }, splitOn: "abecod, cliecod, offcod");
            var rows = new ObservableCollection<OFFED00F>(connection.Query<OFFED00F, tab_articolo, OFFED00F>(
                    @"SELECT OFTNUOR,OFDTPRE,OFDPREZ,OFDQTAV, OFDSCO1, OFDSCO2, OFDSCO3, OFDMAGG, OFDTSC1,OFDTSC2,OFDTSC3,OFDTMAG,OFDSTA,OFDQTAEV,OFDTQTA, ofdunim, art.ID, art.UnitaID, art.QuantitaDefault FROM OFFED00F
                            INNER JOIN tab_articolo AS art ON art.SocietaID=oftsoci AND art.ID = OFDCODA
                            WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO",
                    (row, prd) => { row.Product = new tab_articolo { Descrizione = string.Empty, ID = string.Empty, SocietaID = CompanyID, TipoID = string.Empty, UnitaID = prd.UnitaID, QuantitaDefault = prd.QuantitaDefault }; return row; },
                    new { oftsoci = CompanyID, OFTANNO = Year }, splitOn: "ID").ToList());
            var attachments = new ObservableCollection<OFFETAL00F>(connection.Query<OFFETAL00F>(
                    @"SELECT * FROM OFFETAL00F
                            WHERE oftasoci = @OFTASOCI AND OFTAANNO = @OFTAANNO
                            ORDER BY OFTANAME",
                    new { OFTASOCI = CompanyID, OFTAANNO = Year }).ToList());
            foreach (var head in list)
            {
                head.Rows = new ObservableCollection<OFFED00F>(rows.Where(w => w.OFTNUOR == head.OFTNUOR).ToList());
                head.Attachments = new ObservableCollection<OFFETAL00F>(attachments.Where(w => w.OFTANUOR == head.OFTNUOR).ToList());
            }
            return new ObservableCollection<OFFET00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<OFFET00F>? GetList(string CompanyID, int Year, string Status, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var query = $@"SELECT o.OFTANNO, o.OFTNUOR, o.OFTDAOR, o.OFTCOCL, o.OFTCAUS, o.OFTOGG, o.OFTDEST, o.oflgchi, o.oftsoci, o.OFTINFI, o.OFTINFIUSR, o.OFTFITE, o.OFTFITEUSR, o.OFTFICO, o.OFTFICOUSR, o.OFTSCCL, o.canceled,
                                c.abecod, c.abers1, c.abers2, c.abecfe,
                                d.cliecod, d.codesti, d.ragisoc, 
                                ca.* 
                                FROM OFFET00F AS o
                        LEFT JOIN ABE AS c ON c.abecod = o.OFTCOCL
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OFTCOCL AND d.codesti = o.OFTDEST
                        INNER JOIN TAB_CRM_CAUOFF AS ca ON ca.offcod = o.OFTCAUS
                        WHERE o.oftsoci = @oftsoci AND o.OFTANNO = @yea AND o.OFTCOCL = @cid AND
                        {(Status == "X" ? " o.canceled IS NOT NULL " : $" oflgchi {(Status == "*" ? " <> @oflgchi " : " = @oflgchi AND o.canceled IS NULL ")}")}
                        ORDER BY o.OFTNUOR DESC";

            var list = connection.Query<OFFET00F, ABE, DESTINATARI, TAB_CRM_CAUOFF, OFFET00F>(
                query,
                (ord, abe, des, cau) => { ord.Customer = abe; ord.Recipient = des; ord.Causal = cau; return ord; },
                new { oftsoci = CompanyID, yea = Year, oflgchi = Status, cid = CustomerID }, splitOn: "abecod, cliecod, offcod");
            var rows = new ObservableCollection<OFFED00F>(connection.Query<OFFED00F, tab_articolo, OFFED00F>(
                    @"SELECT OFTNUOR,OFDTPRE,OFDPREZ,OFDQTAV, OFDSCO1, OFDSCO2, OFDSCO3, OFDMAGG, OFDTSC1,OFDTSC2,OFDTSC3,OFDTMAG,OFDSTA,OFDQTAEV,OFDTQTA, ofdunim, art.ID, art.UnitaID, art.QuantitaDefault FROM OFFED00F
                            INNER JOIN tab_articolo AS art ON art.SocietaID=oftsoci AND art.ID = OFDCODA
                            WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO",
                    (row, prd) => { row.Product = new tab_articolo() { Descrizione = string.Empty, ID = string.Empty, SocietaID = CompanyID, TipoID = string.Empty, UnitaID = prd.UnitaID, QuantitaDefault = prd.QuantitaDefault }; return row; },
                    new { oftsoci = CompanyID, OFTANNO = Year }, splitOn: "ID").ToList());
            var attachments = new ObservableCollection<OFFETAL00F>(connection.Query<OFFETAL00F>(
                    @"SELECT * FROM OFFETAL00F
                            WHERE oftasoci = @OFTASOCI AND OFTAANNO = @OFTAANNO
                            ORDER BY OFTANAME",
                    new { OFTASOCI = CompanyID, OFTAANNO = Year }).ToList());
            foreach (var head in list)
            {
                head.Rows = new ObservableCollection<OFFED00F>(rows.Where(w => w.OFTNUOR == head.OFTNUOR).ToList());
                head.Attachments = new ObservableCollection<OFFETAL00F>(attachments.Where(w => w.OFTANUOR == head.OFTNUOR).ToList());
            }
            return new ObservableCollection<OFFET00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public OFFET00F? Get(string oftsoci, int OFTANNO, int OFTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Query<OFFET00F, ABE, DESTINATARI, TAB_CRM_CAUOFF, OFFET00F>(
                @"SELECT o.*, c.*, d.*, ca.* FROM OFFET00F AS o
                        LEFT JOIN ABE AS c ON c.abecod = o.OFTCOCL
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OFTCOCL AND d.codesti = o.OFTDEST
                        INNER JOIN TAB_CRM_CAUOFF AS ca ON ca.offcod = o.OFTCAUS
                        WHERE o.oftsoci = @oftsoci AND o.OFTANNO = @OFTANNO AND o.OFTNUOR = @OFTNUOR",
                (ord, abe, des, cau) => { ord.Customer = abe; ord.Recipient = des; ord.Causal = cau; return ord; },
                new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR }, splitOn: "abecod, cliecod, offcod")
                .FirstOrDefault();
            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(oftsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<OFFED00F>(connection.Query<OFFED00F>(
                        @"SELECT OFTNUOR,OFDTPRE,OFDPREZ,OFDQTAV, OFDSCO1, OFDSCO2, OFDSCO3, OFDMAGG, OFDTSC1,OFDTSC2,OFDTSC3,OFDTMAG,OFDSTA,OFDQTAEV FROM OFFED00F
                            WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR",
                        new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR }).ToList());
                result.Attachments = new ObservableCollection<OFFETAL00F>(connection.Query<OFFETAL00F>(
                        @"SELECT * FROM OFFETAL00F
                            WHERE oftasoci = @OFTASOCI AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR
                            ORDER BY OFTANAME",
                        new { OFTASOCI = oftsoci, OFTAANNO = OFTANNO, OFTANUOR = OFTNUOR }).ToList());
                Parallel.ForEach(result.Rows, (row) => { row.UMsCache = umsCache; });
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public OFFET00F? GetFull(string oftsoci, int OFTANNO, int OFTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<OFFET00F, ABE, DESTINATARI, TAB_CRM_CAUOFF, OFFET00F>(
                @"SELECT o.*, c.*, d.*, ca.* FROM OFFET00F AS o
                        LEFT JOIN ABE AS c ON c.abecod = o.OFTCOCL
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = o.OFTCOCL AND d.codesti = o.OFTDEST
                        INNER JOIN TAB_CRM_CAUOFF AS ca ON ca.offcod = o.OFTCAUS
                        WHERE o.oftsoci = @oftsoci AND o.OFTANNO = @OFTANNO AND o.OFTNUOR = @OFTNUOR",
                (ord, abe, des, cau) => { ord.Customer = abe; ord.Recipient = des; ord.Causal = cau; return ord; },
                new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR }, splitOn: "abecod, cliecod, offcod")
                .FirstOrDefault();
            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(oftsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<OFFED00F>(connection.Query<OFFED00F, tab_articolo, OFFED00F>(
                        @"SELECT r.*,p.SocietaID, p.ID, p.Descrizione, p.UnitaID, p.UnitaIDAlt FROM OFFED00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.oftsoci AND p.ID = r.OFDCODA
                            WHERE r.oftsoci = @oftsoci AND r.OFTANNO = @OFTANNO AND r.OFTNUOR = @OFTNUOR",
                        (off, prd) =>
                        {
                            off.UMsCache = umsCache;
                            off.SelectedProduct = prd;
                            off.CustomerDiscount = result.OFTSCCL ?? 0;
                            return off;
                        },
                        new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR }, splitOn: "SocietaID").ToList());
                result.Attachments = new ObservableCollection<OFFETAL00F>(connection.Query<OFFETAL00F>(
                        @"SELECT * FROM OFFETAL00F
                            WHERE oftasoci = @OFTASOCI AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR
                            ORDER BY OFTANAME",
                        new { OFTASOCI = oftsoci, OFTAANNO = OFTANNO, OFTANUOR = OFTNUOR }).ToList());
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CloseOrders(List<OFFET00F> SelectedOffers, string CausalID, string Note, string Username)
    {
        try
        {
            using var connection = GetOpenConnection();



            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var offedRepo = VulpesServiceProvider.Provider.GetRequiredService<IOFFED00FRepository>();

                    foreach (var off in SelectedOffers)
                    {
                        // refresh
                        var refreshed = Get(off.oftsoci, off.OFTANNO, off.OFTNUOR);

                        if (refreshed != null)
                        {
                            refreshed.closed = now;
                            refreshed.close_user = Username;
                            refreshed.close_causal_id = CausalID;
                            refreshed.close_note = Note;

                            var rows = offedRepo.GetList(off.oftsoci, off.OFTANNO, off.OFTNUOR);
                            if (rows != null)
                                refreshed.oflgchi = rows.Any(any => any.transformed.HasValue) ? "O" : "C";
                            connection.Execute(UPDATE_QUERY, refreshed, transaction);
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return true;
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Exists(string oftsoci, int OFTANNO, int OFTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM OFFET00F WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR",
                new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Printing
    public OFFET00F? GetPrintFull(string oftsoci, int OFTANNO, int OFTNUOR, bool UseCustomerCodes, bool PrintProductNote, bool PrintAgentsInDetails)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<OFFET00F>($@"SELECT i.*, c.*, v1.*, v2.*,s.*, co.*, d.*,iso.isolin,sl.spedes,cl.condes FROM OFFET00F AS i
                        LEFT OUTER JOIN ABE AS c ON c.abecod = i.OFTCOCL
                        LEFT OUTER JOIN VETTORI AS v1 ON v1.vetcod = i.OFTCORR 
                        LEFT OUTER JOIN VETTORI AS v2 ON v2.vetcod = i.OFTCORR2 
                        LEFT OUTER JOIN SPEDIZIONE AS s ON s.specod = i.OFTSPED
                        LEFT OUTER JOIN CONSEGNA AS co ON co.concod = i.OFTCONS
                        LEFT OUTER JOIN ISO AS iso ON iso.isocod = c.isocod
                        LEFT OUTER JOIN SPEDIZIONE_LINGUA AS sl ON sl.specod = i.OFTSPED AND sl.lincod = iso.isolin
                        LEFT OUTER JOIN CONSEGNA_LINGUA AS cl ON cl.concod = i.OFTCONS AND cl.lincod = iso.isolin
                        LEFT OUTER JOIN DESTINATARI AS d ON d.cliecod = i.OFTCOCL AND d.codesti = i.OFTDEST
                        WHERE i.oftsoci = @oftsoci AND i.OFTANNO = @OFTANNO AND i.OFTNUOR = @OFTNUOR",
                    new[] { typeof(OFFET00F), typeof(ABE), typeof(VETTORI), typeof(VETTORI), typeof(SPEDIZIONE), typeof(CONSEGNA), typeof(DESTINATARI), typeof(string), typeof(string), typeof(string) }, (objects) =>
                    {
                        var offet00f = (OFFET00F)objects[0];
                        offet00f.Customer = objects[1] as ABE;
                        offet00f.FirstCarrier = objects[2] as VETTORI;
                        offet00f.SecondCarrier = objects[3] as VETTORI;
                        offet00f.Shipping = objects[4] as SPEDIZIONE;
                        if (offet00f.Shipping != null)
                            offet00f.Shipping.spedes = (!string.IsNullOrEmpty(objects[8]?.ToString()) ? objects[8].ToString() : offet00f.Shipping.spedes) ?? string.Empty;
                        offet00f.Delivery = objects[5] as CONSEGNA;
                        if (offet00f.Delivery != null)
                            offet00f.Delivery.condes = (!string.IsNullOrEmpty(objects[9]?.ToString()) ? objects[9].ToString() : offet00f.Delivery.condes) ?? string.Empty;
                        offet00f.Recipient = objects[6] as DESTINATARI;
                        offet00f.Language = objects[7] as string;

                        return offet00f;
                    }, new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR }, splitOn: "abecod,vetcod,vetcod,specod,concod,cliecod,isolin,spedes,condes").FirstOrDefault();

            if (result != null)
            {
                result.Rows = new ObservableCollection<OFFED00F>(connection.Query<OFFED00F, tab_articolo, tab_articolo_unita, AGENTI, AGENTI, tab_articolo_lingua, OFFED00F>(
                        $@"SELECT r.*, {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes, l.* FROM OFFED00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.oftsoci AND p.ID = r.OFDCODA
                            LEFT OUTER JOIN tab_articolo_lingua AS l ON p.SocietaID = l.SocietaID AND p.ID = l.ArticoloID AND l.lincod = @Lingua
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.oftsoci AND u.ID = r.ofdunim
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.OFDCOA1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.OFDCOA2
                            WHERE r.oftsoci = @oftsoci AND r.OFTANNO = @OFTANNO AND r.OFTNUOR = @OFTNUOR",
                        (row, prd, um, age1, age2, lingua) =>
                        {
                            row.Product = prd;
                            row.Product.Descrizione = (lingua != null && !string.IsNullOrEmpty(lingua.Descrizione) ? lingua.Descrizione : prd.Descrizione);
                            row.Product.Note = (lingua != null && !string.IsNullOrEmpty(lingua.Note) ? lingua.Note : prd.Note);
                            row.UM = um;
                            row.SelectedFirstAgent = age1;
                            row.SelectedSecondAgent = age2;
                            row.PrintAgentsInDetails = PrintAgentsInDetails && (age1 != null || age2 != null);
                            row.CustomerDiscount = result.OFTSCCL ?? 0;
                            return row;
                        },
                        new { oftsoci = oftsoci, OFTANNO = OFTANNO, OFTNUOR = OFTNUOR, Lingua = result.Language }, splitOn: "SocietaID,SocietaID,agecod,agecod,SocietaID").ToList());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent
                    var agent1 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.OFDRIGA).FirstOrDefault()?.OFDCOA1 ?? string.Empty);
                    var agent2 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.OFDRIGA).FirstOrDefault()?.OFDCOA2 ?? string.Empty);
                    result.DefaultFirstAgent = agent1;
                    result.DefaultSecondAgent = agent2;
                }


                result.Attachments = new ObservableCollection<OFFETAL00F>(connection.Query<OFFETAL00F>(
                        @"SELECT * FROM OFFETAL00F
                            WHERE oftasoci = @OFTASOCI AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR
                            ORDER BY OFTANAME",
                        new { OFTASOCI = oftsoci, OFTAANNO = OFTANNO, OFTANUOR = OFTNUOR }).ToList());

                // check if custom code and UMs recap for each row
                result.UMsRecap = new ObservableCollection<GenericStringDecimal>();
                foreach (var row in result.Rows.OrderBy(o => o.ofdunim))
                {
                    #region Customer code
                    var lisCli = VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetCurrent(oftsoci, row.OFDCODA, (result.OFTCOCL ?? 0), result.OFTDAOR ?? DateTime.MinValue, row.ofdunim ?? string.Empty);
                    if (lisCli != null)
                    {
                        if (!UseCustomerCodes)
                        {
                            row.CustomerCode = $"Codifica cliente: {(!string.IsNullOrWhiteSpace(lisCli.CustomerProductID) ? lisCli.CustomerProductID : null)} {(!string.IsNullOrWhiteSpace(lisCli.CustomerProductDescription) ? lisCli.CustomerProductDescription : null)}";
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(lisCli.CustomerProductID))
                                row.OFDCODA = lisCli.CustomerProductID;
                            if (row.Product != null && !string.IsNullOrWhiteSpace(lisCli.CustomerProductDescription))
                                row.Product.Descrizione = lisCli.CustomerProductDescription;
                        }
                    }
                    else
                    {
                        row.CustomerCode = null;
                    }
                    #endregion
                    #region UMs recap
                    var existingUM = result.UMsRecap.Where(w => w.Description == row.ofdunim).FirstOrDefault();
                    if (existingUM != null)
                    {
                        existingUM.Value += row.OFDQTAV;
                    }
                    else
                    {
                        result.UMsRecap.Add(new GenericStringDecimal()
                        {
                            Description = row.ofdunim,
                            Value = row.OFDQTAV
                        });
                    }
                    #endregion
                }
                #region Rates recap
                int i = 1;
                List<string> rates = new List<string>();
                var ratesList = new List<Tuple<string, string, string, string>>();
                var ratesList2 = new List<Tuple<string, string, string, string>>();
                foreach (var row in result.Rows.OrderBy(o => o.OFDASSF).ThenBy(o => o.OFDALIV))
                {
                    if (!rates.Contains(row.OFDASSF + row.OFDALIV?.Trim()))
                    {
                        var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.OFDASSF ?? string.Empty, row.OFDALIV ?? string.Empty);
                        rates.Add(row.OFDASSF + row.OFDALIV?.Trim());
                        var imponibile = Math.Round(result.Rows
                        .Where(w => w.OFDASSF == row.OFDASSF && w.OFDALIV == row.OFDALIV)
                        .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                        // compute customer discount
                        imponibile = imponibile - (imponibile * (result.OFTSCCL ?? 0) / 100);
                        decimal rateValue = 0;
                        decimal.TryParse(rate?.assali, out rateValue);
                        var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                        if (i % 2 == 0)
                            ratesList2.Add(new Tuple<string, string, string, string>(row.OFDASSF + " " + row.OFDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                        else
                            ratesList.Add(new Tuple<string, string, string, string>(row.OFDASSF + " " + row.OFDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
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
                    foreach (var row in result.Rows.Where(w => w.OFDTQTA == "O").OrderBy(o => o.OFDASSF).ThenBy(o => o.OFDALIV))
                    {
                        if (!giftsRates.Contains(row.OFDASSF + row.OFDALIV?.Trim()))
                        {
                            var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.OFDASSF ?? string.Empty, row.OFDALIV ?? string.Empty);
                            giftsRates.Add(row.OFDASSF + row.OFDALIV?.Trim());
                            var imponibile = Math.Round(result.Rows
                            .Where(w => w.OFDTQTA == "O" && w.OFDASSF == row.OFDASSF && w.OFDALIV == row.OFDALIV)
                            .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                            // compute customer discount
                            imponibile = imponibile - (imponibile * (result.OFTSCCL ?? 0) / 100);
                            decimal rateValue = 0;
                            decimal.TryParse(rate?.assali, out rateValue);
                            var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                            giftsRatesList.Add(new Tuple<string, string, string, string>(row.OFDASSF + " " + row.OFDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
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

    public OfferReport? PrintOffer(OFFET00F Offer)
    {
        try
        {
            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(Offer.oftsoci)!;
            var causal = VulpesServiceProvider.Provider.GetRequiredService<ITAB_CRM_CAUOFFRepository>().GetFull(Offer.oftsoci, Offer.OFTCAUS ?? string.Empty);

            foreach (var row in Offer.Rows ?? new ObservableCollection<OFFED00F>())
            {
                row.CausalNotes = causal != null && causal.Text != null ? causal.Text.TTXNote : null;
            }

            ABICAB? bank = null;
            BANAZIEN? companyBank = null;
            if (Offer.abiabi.HasValue)
            {
                bank = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(Offer.abiabi ?? 0, Offer.abicab ?? 0);
                companyBank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Offer.oftsoci, Offer.abiabi ?? 0, Offer.abicab ?? 0, Offer.OFTBCON ?? string.Empty);
            }

            var customerData = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Offer.oftsoci, (Offer.OFTCOCL ?? 0));

            var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Offer.oftsoci)!;
            var aziendaLingua = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(Offer.oftsoci, Offer.Language ?? string.Empty);

            var pagamento = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(Offer.OFTPAGA ?? string.Empty);
            var pagamentoLingua = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLI_LINGUARepository>().Get(Offer.OFTPAGA ?? string.Empty, Offer.Language ?? string.Empty);

            var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(Offer.Language ?? string.Empty);

            object? objectDictionary = null;
            if (languageDictionary != null)
                objectDictionary = new LocalizationHelper().CreateClassFromDictionary(languageDictionary);

            return new OfferReport()
            {
                Offer = Offer,
                PaymentDescription = !string.IsNullOrWhiteSpace(Offer.OFTPAGA) ? (pagamentoLingua != null && !string.IsNullOrEmpty(pagamentoLingua.pcldes) ? pagamentoLingua.pcldes : pagamento?.pcldes) : null,
                CompanyInfo = azienda,
                BankData = bank != null ? ($"{bank.FullDescriptionSearchable} c/c nr.{(companyBank != null ? Offer.OFTBCON : customerData?.CLNUCC)} IBAN: {(companyBank != null ? companyBank.abibiba : customerData?.cliban)}") : "Nessuna banca indicata",
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                CertificationsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}certs.png"),
                HeaderFootNote = Offer.OFTSHOWP ? Offer.OFTNOTEP : null,
                FixedText = (aziendaLingua != null) ? (!string.IsNullOrEmpty(aziendaLingua.azoffgtex) ? aziendaLingua.azoffgtex : azienda.azoffgtex) : azienda.azoffgtex,
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

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO OFFET00F (oftsoci,OFTANNO,OFTNUOR,OFTDAOR,OFTCAUS,OFTCOCL,OFTDEST,OFTCONZ,OFTPAGA,OFTCONS,OFTSPED,OFTCORR,OFTAREA,OFTCIDI,OFTDE25,OFTIMBA,oftdivi,OFTOGG,abicab,abiabi,oflgchi,oftscad,OFTDARI,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,OFTCORR2,OFTNOTET,OFTNOTEP,OFTSHOWT,OFTSHOWP,OFTREGI,OFTZONE,OFTSETM,OFTFILI,OFTRIVE,OFTBCON,OFTSCCL,OFTDACO,OFTINFI,OFTINFIUSR,OFTFICO,OFTFICOUSR,OFTFITE,OFTFITEUSR,OFTLING,close_causal_id,closed,close_user,close_note) OUTPUT INSERTED.rv VALUES(@oftsoci,@OFTANNO,@OFTNUOR,@OFTDAOR,@OFTCAUS,@OFTCOCL,@OFTDEST,@OFTCONZ,@OFTPAGA,@OFTCONS,@OFTSPED,@OFTCORR,@OFTAREA,@OFTCIDI,@OFTDE25,@OFTIMBA,@oftdivi,@OFTOGG,@abicab,@abiabi,@oflgchi,@oftscad,@OFTDARI,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@OFTCORR2,@OFTNOTET,@OFTNOTEP,@OFTSHOWT,@OFTSHOWP,@OFTREGI,@OFTZONE,@OFTSETM,@OFTFILI,@OFTRIVE,@OFTBCON,@OFTSCCL,@OFTDACO,@OFTINFI,@OFTINFIUSR,@OFTFICO,@OFTFICOUSR,@OFTFITE,@OFTFITEUSR,@OFTLING,@close_causal_id,@closed,@close_user,@close_note)";
    public string UPDATE_QUERY => "UPDATE OFFET00F SET oftsoci = @oftsoci,OFTANNO = @OFTANNO,OFTNUOR = @OFTNUOR,OFTDAOR = @OFTDAOR,OFTCAUS = @OFTCAUS,OFTCOCL = @OFTCOCL,OFTDEST = @OFTDEST,OFTCONZ = @OFTCONZ,OFTPAGA = @OFTPAGA,OFTCONS = @OFTCONS,OFTSPED = @OFTSPED,OFTCORR = @OFTCORR,OFTAREA = @OFTAREA,OFTCIDI = @OFTCIDI,OFTDE25 = @OFTDE25,OFTIMBA = @OFTIMBA,oftdivi = @oftdivi,OFTOGG = @OFTOGG,abicab = @abicab,abiabi = @abiabi,oflgchi = @oflgchi,oftscad = @oftscad,OFTDARI = @OFTDARI,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,OFTCORR2 = @OFTCORR2,OFTNOTET = @OFTNOTET,OFTNOTEP = @OFTNOTEP,OFTSHOWT = @OFTSHOWT,OFTSHOWP = @OFTSHOWP,OFTREGI = @OFTREGI,OFTZONE = @OFTZONE,OFTSETM = @OFTSETM,OFTFILI = @OFTFILI,OFTRIVE = @OFTRIVE,OFTBCON = @OFTBCON,OFTSCCL = @OFTSCCL,OFTDACO = @OFTDACO,OFTINFI = @OFTINFI,OFTINFIUSR = @OFTINFIUSR,OFTFICO = @OFTFICO,OFTFICOUSR = @OFTFICOUSR,OFTFITE = @OFTFITE,OFTFITEUSR = @OFTFITEUSR,OFTLING = @OFTLING,close_causal_id = @close_causal_id,closed = @closed,close_user = @close_user,close_note = @close_note OUTPUT INSERTED.rv WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM OFFET00F OUTPUT DELETED.rv WHERE oftsoci = @oftsoci AND OFTANNO = @OFTANNO AND OFTNUOR = @OFTNUOR AND rv = @rv";
    public bool Insert(OFFET00F Model)
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
    public bool Update(OFFET00F Model)
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

    public bool Delete(OFFET00F Model)
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

    public string? Validate(OFFET00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.oftsoci) && IsInsert && !Exists(Model.oftsoci, Model.OFTANNO, Model.OFTNUOR)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.OFTCAUS))
                {
                    if (!string.IsNullOrWhiteSpace(Model.OFTOGG))
                    {
                        if (Model.oftscad.HasValue && Model.oftscad.Value > (Model.OFTDAOR ?? DateTime.MaxValue))
                        {
                            if (Model.OFTCOCL.HasValue && Model.OFTCOCL.Value > 0)
                            {
                                if ((Model.OFTCOCL.HasValue && !string.IsNullOrWhiteSpace(Model.OFTPAGA)) || !Model.OFTCOCL.HasValue)
                                {
                                    if ((Model.OFTCOCL.HasValue && Model.abiabi.HasValue && Model.abicab.HasValue && Model.abiabi.Value > 0 && Model.abicab.Value > 0) || !Model.OFTCOCL.HasValue)
                                    {
                                        if ((Model.OFTCOCL.HasValue && !string.IsNullOrWhiteSpace(Model.OFTSPED)) || !Model.OFTCOCL.HasValue)
                                        {
                                            if ((Model.OFTCOCL.HasValue && !string.IsNullOrWhiteSpace(Model.OFTCONS)) || !Model.OFTCOCL.HasValue)
                                            {
                                                return null;
                                            }
                                            else
                                            { return "Il tipo di consegna è obbligatorio"; }
                                        }
                                        else
                                        { return "Il tipo spedizione è obbligatorio"; }
                                    }
                                    else
                                    { return "La banca per il pagamento è obbligatoria"; }
                                }
                                else
                                { return "Il codice pagamento è obbligatorio"; }
                            }
                            else
                            { return "Deve essere selezionato un cliente oppure un prospect"; }
                        }
                        else
                        { return "La data fine validità è obbligatoria è deve essere successiva alla data dell'offerta e alla data di richiesta del cliente"; }
                    }
                    else
                    { return "L'oggetto dell'offerta è obbligatorio"; }
                }
                else
                { return "La causale è obbligatoria"; }
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