

using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Linq;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.CRM;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Models.CRM;
using VulpesX.Models.Reports.CRM;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared.Generics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.DAL.CRM;

public interface IFATTT00FRepository
{
    ObservableCollection<FATTT00F>? GetList(string CompanyID, DateTime FromDate, DateTime ToDate);

    ObservableCollection<FATTD00F>? GetListByOrder(string OrderCompanyID, int OrderYear, int OrderNumber);

    ObservableCollection<FATTD00F>? GetListByOrderYear(string OrderCompanyID, int OrderYear);

    FATTT00F? GetHead(string ftsoci, int FTANNO, int FTNUOR);

    FATTT00F? GetSingle(string ftsoci, int FTANNO, int FTNUOR);

    FATTT00F? Get(string ftsoci, int FTANNO, int FTNUOR);

    InvoicingTrend? GetTrend(string CompanyID);

    FATTT00F? GetFull(string ftsoci, int FTANNO, int FTNUOR, bool PrintProductNote, bool PrintAgentsInDetails);

    DateTime? CheckLastFinalDate(string ftsoci, int FTANNO, string NumeratorID);

    bool Exists(string ftsoci, int FTANNO, int FTNUOR);

    decimal GetInvoicesAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate, bool OnlyUnaccounted);

    List<FATTT00F>? GetINTRA(string CompanyID, DateTime Period);

    #region eInvoice
    string? GenerateInvoiceXML(string CompanyID, int InvoiceYear, int InvoiceNumber, bool IsRegenerate);
    #endregion

    #region Accounting
    bool Accounting(FATTT00F Invoice, ESERCIZIO AccountingYear, DateTime AccountingDate, string UserID);
    bool AccountingReceivedInvoice(string CompanyID, ACC_EINVOICE_HEADS Invoice, ESERCIZIO AccountingYear, DateTime RegDate, DateTime ProtDate, CAUCONT Causal, List<PNRIGHE> Counterparts, string UserID);
    bool AccountingSentExternalInvoice(string CompanyID, ACC_EINVOICE_HEADS Invoice, ESERCIZIO AccountingYear, DateTime RegDate, DateTime ProtDate, CAUCONT Causal, string UserID);
    #endregion

    #region CRM
    bool GenerateByOrder(ORDIT00F OrderHead, List<ORDID00F> SelectedRows, string UserID, string HeadNotes, string FootNotes, string DocumentType);
    bool GenerateByDDT(List<BOLLT00F> DDTList, int Year, string UserID);
    #endregion

    #region Printing
    FATTT00F? GetPrintFull(string ftsoci, int FTANNO, int FTNUOR, bool PrintProductNote, bool PrintAgentsInDetails, bool UseCustomerCodes);
    InvoiceReport? PrintInvoice(FATTT00F Invoice);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FATTT00F Model);

    bool Update(FATTT00F Model);

    bool Delete(FATTT00F Model);

    string? Validate(FATTT00F Model, bool IsInsert);
    #endregion
}

public class FATTT00FRepository : RepositoryBase, IFATTT00FRepository
{
    public FATTT00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public const string ERR_CANNOT_DELETE = "Impossibile eliminare una fattura stampata in definitivo";

    public ObservableCollection<FATTT00F>? GetList(string CompanyID, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FATTT00F, FATTAUT, CAUFAT00F, FATTT00F>(
                @"SELECT i.ftsoci,i.FTANNO,i.FTNUOR,i.FTTIPO,i.FTNUFD,i.FTDAOR,i.FTCAUS,i.FTCODC,i.FTCODD,i.FTDATFEL,i.FTNUMFEL,i.ftsdiid,i.FTSCCL,i.FTFLA1,i.FTFLA2,i.rv, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS CustomerFullDescriptionSearchable, si.*, ca.fatcod,ca.fatdes,ca.fatpre FROM FATTT00F AS i
                        INNER JOIN ABE AS c ON c.abecod = i.FTCODC
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        WHERE i.ftsoci = @cid AND CONVERT(date, i.FTDAOR) >= @fd AND CONVERT(date, i.FTDAOR) <= @td
                        ORDER BY i.FTANNO DESC, i.FTNUOR DESC",
                (inv, sel, cau) => { inv.SelfInvoice = sel; inv.Causal = cau; return inv; },
                new { cid = CompanyID, fd = FromDate.Date, td = ToDate.Date }, splitOn: "FTAUSC,fatcod").ToList();
            var rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F, tab_articolo, FATTD00F>(
                    @"SELECT r.FTANNO,r.FTNUOR,r.FDTPRE,r.FDPREZ,r.FDQTAV, r.FDALIV, r.FDSCO1, r.FDSCO2, r.FDSCO3, r.FDMAGG, r.FDTSC1,r.FDTSC2,r.FDTSC3,r.FDTMAG,r.FDTQTA, r.FDSTAMP, r.fdunim, art.ID, art.UnitaID, art.QuantitaDefault FROM FATTD00F AS r
                            INNER JOIN tab_articolo AS art ON art.SocietaID=ftsoci AND art.ID = FDCODA
                            INNER JOIN FATTT00F AS i ON i.ftsoci=r.ftsoci AND i.FTANNO=r.FTANNO AND i.FTNUOR=r.FTNUOR
                            WHERE r.ftsoci = @ftsoci AND CONVERT(date, i.FTDAOR) >= @fd AND CONVERT(date, i.FTDAOR) <= @td",
                    (row, prd) => { row.Product = new tab_articolo() { Descrizione = string.Empty, ID = string.Empty, SocietaID = CompanyID, TipoID = string.Empty, UnitaID = prd.UnitaID, QuantitaDefault = prd.QuantitaDefault }; return row; },
                    new { ftsoci = CompanyID, fd = FromDate.Date, td = ToDate.Date }, splitOn: "ID").ToList());
            var attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                    @"SELECT a.* FROM FATTAL00F AS a
                            INNER JOIN FATTT00F AS i ON i.ftsoci=a.FTASOCI AND i.FTANNO=a.FTAANNO AND i.FTNUOR=a.FTANUOR
                            WHERE a.FTASOCI = @FTASOCI AND CONVERT(date, i.FTDAOR) >= @fd AND CONVERT(date, i.FTDAOR) <= @td
                            ORDER BY a.FTANAME",
                    new { FTASOCI = CompanyID, fd = FromDate.Date, td = ToDate.Date }).ToList());
            Parallel.ForEach(list, (head) =>
            {
                head.SDISentStatus = null;
                head.Rows = new ObservableCollection<FATTD00F>(rows.Where(w => w.FTANNO == head.FTANNO && w.FTNUOR == head.FTNUOR).ToList());
                head.Attachments = new ObservableCollection<FATTAL00F>(attachments.Where(w => w.FTAANNO == head.FTANNO && w.FTANUOR == head.FTNUOR).ToList());
            });

            #region Update sent status
            string? APIKey = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.azapikey?.Trim();
            if (APIKey != null)
            {
#pragma warning disable SYSLIB0014
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new CerberoSendAPI.SendInvoicesClient();
                if (client.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    try
                    {
                        var request = new CerberoSendAPI.RetrieveSentStatusRequest(APIKey, list.Where(w => !string.IsNullOrWhiteSpace(w.FTNUMFEL) && string.IsNullOrWhiteSpace(w.ftsdiid)).Select(s => s.FTNUMFEL).ToList().ToArray());
                        var result = client.RetrieveSentStatusAsync(request).Result;
                        if (!string.IsNullOrWhiteSpace(result.RetrieveSentStatusResult.ErrorCode))
                        {
                            ErrorHandler.Show($"{result.RetrieveSentStatusResult.ErrorCode} - {result.RetrieveSentStatusResult.ErrorMessage}");
                        }
                        else
                        {
                            Parallel.ForEach(result.RetrieveSentStatusResult.Items, (item) =>
                            {
                                var tmpExist = list.Where(w => w.FTNUMFEL == item.Filename).First();
                                var existing = GetSingle(tmpExist.ftsoci, tmpExist.FTANNO, tmpExist.FTNUOR);
                                if (item.Status == "D" && existing != null)
                                {
                                    existing.ftsdiid = item.SDIID;
                                    Update(existing);
                                }
                                else if (item.Status == "F" && existing != null)
                                {
                                    existing.ftsdiid = item.SDIID;
                                    Update(existing);
                                    tmpExist.FailureDescription = item.FailureDescription;
                                }
                                else if (item.Status == "R")
                                {
                                    if (item.Errors != null && item.Errors.Count() > 0)
                                    {
                                        tmpExist.SDIErrors = new List<Tuple<string, string, string>>();
                                        foreach (var err in item.Errors.OrderBy(o => o.ID))
                                        {
                                            tmpExist.SDIErrors.Add(new Tuple<string, string, string>(err.ID, err.Description, err.Suggestion));
                                        }
                                    }
                                }
                                tmpExist.SDISentStatus = item.Status;
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        Exception? exi = exc;
                        var sb = new StringBuilder();
                        do
                        {
                            sb.Append(exi.Message).Append("\n\n");
                            exi = exi.InnerException;
                        } while (exi != null);
                        ErrorHandler.Show(sb.ToString());
                        client.Close();
                    }
                    client.Close();
                }
            }
            #endregion

            return new ObservableCollection<FATTT00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<FATTD00F>? GetListByOrder(string OrderCompanyID, int OrderYear, int OrderNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            var heads = connection.Query<FATTD00F>(@"SELECT DISTINCT ftsoci, ftanno, ftnuor, otann1, otnuo1 from FATTD00F
                                                        WHERE ftsoci = @ordc AND OTANN1 = @ordy AND OTNUO1 = @ordn
                                                        ORDER BY ftanno DESC, ftnuor DESC",
                                                    new { ordc = OrderCompanyID, ordy = OrderYear, ordn = OrderNumber });

            return new ObservableCollection<FATTD00F>(heads);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<FATTD00F>? GetListByOrderYear(string OrderCompanyID, int OrderYear)
    {
        try
        {
            using var connection = GetOpenConnection();


            var heads = connection.Query<FATTD00F>(@"SELECT DISTINCT ftanno, ftnuor, otann1, otnuo1 from FATTD00F
                                                        WHERE ftsoci = @ordc AND OTANN1 = @ordy
                                                        ORDER BY ftanno DESC, ftnuor DESC",
                                                    new { ordc = OrderCompanyID, ordy = OrderYear });

            return new ObservableCollection<FATTD00F>(heads);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? GetHead(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FATTT00F, ABE, CAUFAT00F, FATTT00F>(
                @"SELECT i.*, c.abecod, c.abers1, c.abers2, ca.fatcod, ca.fatpre FROM FATTT00F AS i
                        INNER JOIN ABE AS c ON c.abecod = i.FTCODC
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                (inv, cus, cau) => { inv.Customer = cus; inv.Causal = cau; return inv; },
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,fatcod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? GetSingle(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FATTT00F>(
                @"SELECT i.* FROM FATTT00F AS i
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? Get(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            var unitaRepo = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>();


            var result = connection.Query<FATTT00F, ABE, FATTAUT, CAUFAT00F, FATTT00F>(
                @"SELECT i.*, c.abecod, c.abers1, c.abers2, si.*, ca.fatcod, ca.fatpre FROM FATTT00F AS i
                        INNER JOIN ABE AS c ON c.abecod = i.FTCODC
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                (inv, cus, sel, cau) => { inv.Customer = cus; inv.SelfInvoice = sel; inv.Causal = cau; return inv; },
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,FTAUSC,fatcod")
                .FirstOrDefault();
            var umsCache = unitaRepo.GetSimpleList(ftsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F>(
                        @"SELECT * FROM FATTD00F
                            WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR",
                        new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }).ToList());
                result.Attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                        @"SELECT * FROM FATTAL00F
                            WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                            ORDER BY FTANAME",
                        new { FTASOCI = ftsoci, FTAANNO = FTANNO, FTANUOR = FTNUOR }).ToList());
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

    public InvoicingTrend? GetTrend(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            InvoicingTrend result = new InvoicingTrend() { Months = new ObservableCollection<InvoicingTrendMonth>() };

            var heads = connection.Query<FATTT00F>(@"select ftsoci,ftanno,ftnuor,ftdaor,ftsccl,fttipo from fattt00f where ftsoci=@cid and canceled IS NULL order by ftanno,ftnuor;", new { cid = CompanyID });
            var years = heads.Select(s => s.FTANNO).Distinct().ToArray();
            var rows = connection.Query<FATTD00F>(@"select d.ftsoci,d.ftanno,d.ftnuor,d.FDSCO1, d.FDTSC1, d.FDSCO2, d.FDTSC2, d.FDSCO3, d.FDTSC3, d.FDMAGG, d.FDTMAG,d.fdprez, d.fdtpre,d.fdqtav,d.fdtqta from fattd00f AS d
                                                        INNER JOIN FATTT00F AS t ON t.ftsoci=d.ftsoci AND t.ftanno=d.ftanno AND t.ftnuor=d.ftnuor
                                                        where d.ftsoci=@cid and d.ftanno IN @years and t.canceled IS NULL", new { cid = CompanyID, years = years });
            Parallel.ForEach(heads, head =>
            {
                head.Rows = new ObservableCollection<FATTD00F>(rows.Where(w => w.ftsoci == head.ftsoci && w.FTANNO == head.FTANNO && w.FTNUOR == head.FTNUOR).ToList());
            });

            decimal previousMonthAmount = 0;
            foreach (var head in heads.GroupBy(g => new { g.FTANNO, (g.FTDAOR ?? DateTime.MinValue).Month }, (key, items) => new { key, items }))
            {
                var newMonth = new InvoicingTrendMonth()
                {
                    Year = head.key.FTANNO,
                    Month = head.key.Month,
                    Amount = head.items.Where(w => w.FTTIPO == "F" || w.FTTIPO == "B").Sum(sum => sum.Imponibile),
                    CreditAmount = head.items.Where(w => w.FTTIPO == "N").Sum(sum => sum.Imponibile),
                    Weight = 1,
                    PreviousMonthAmount = previousMonthAmount
                };
                result.Months.Add(newMonth);
                previousMonthAmount = newMonth.Amount;
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? GetFull(string ftsoci, int FTANNO, int FTNUOR, bool PrintProductNote, bool PrintAgentsInDetails)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<FATTT00F, ABE, FATTAUT, FATTT00F>(
                @"SELECT i.*, c.*, si.* FROM FATTT00F AS i
                        INNER JOIN ABE AS c ON c.abecod = i.FTCODC
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                (inv, cus, sel) => { inv.Customer = cus; inv.SelfInvoice = sel; return inv; },
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,FTAUSC")
                .FirstOrDefault();
            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(ftsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F, tab_articolo, tab_articolo_unita, AGENTI, AGENTI, FATTD00F>(
                        $@"SELECT r.*, (CASE WHEN asspla = 'S' THEN 1 ELSE 0 END) AS HasPlafond, {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes FROM FATTD00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.FDCOAG1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.FDCOAG2
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR",
                        (row, prd, um, age1, age2) =>
                        {
                            row.UMsCache = umsCache;
                            row.Product = prd;
                            row.UM = um;
                            row.FirstAgent = age1;
                            row.SecondAgent = age2;
                            row.PrintAgentsInDetails = PrintAgentsInDetails && (age1 != null || age2 != null);
                            row.CustomerDiscount = result.FTSCCL ?? 0;
                            return row;
                        },
                        new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "SocietaID,SocietaID,agecod,agecod").ToList());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent

                    var agent1 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG1 ?? string.Empty);
                    var agent2 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG2 ?? string.Empty);
                    result.DefaultFirstAgent = agent1;
                    result.DefaultSecondAgent = agent2;
                }

                result.Attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                        @"SELECT * FROM FATTAL00F
                            WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                            ORDER BY FTANAME",
                        new { FTASOCI = ftsoci, FTAANNO = FTANNO, FTANUOR = FTNUOR }).ToList());
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DateTime? CheckLastFinalDate(string ftsoci, int FTANNO, string NumeratorID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.ExecuteScalar<DateTime>(
                @"SELECT MAX(t.FTDAOR) FROM FATTT00F AS t
                        INNER JOIN CAUFAT00F AS c ON c.fatcod=t.FTCAUS
                        WHERE t.ftsoci = @ftsoci AND t.FTANNO = @FTANNO AND t.FTNUFD IS NOT NULL AND t.FTNUFD > 0 AND c.fatnmr=@nid",
                new { ftsoci = ftsoci, FTANNO = FTANNO, @nid = NumeratorID });

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FATTT00F WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR",
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public decimal GetInvoicesAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate, bool OnlyUnaccounted)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query = @$"SELECT d.*, t.* FROM FATTD00F AS d
                       INNER JOIN FATTT00F AS t ON t.ftsoci = d.ftsoci AND t.FTANNO = d.FTANNO AND t.FTNUOR = d.FTNUOR
                       WHERE t.ftsoci=@ftsoci AND t.FTCODC=@ftcodc {(Year.HasValue ? "AND t.FTANNO = @ftanno" : null)} {(ToDate.HasValue ? "AND t.FTDAOR<=@todate" : null)} {(OnlyUnaccounted ? "AND d.FDFLA2 = ' '" : null)}";

            var list = connection.Query<FATTD00F, FATTT00F, FATTD00F>(
                query,
                (dett, head) => { return dett; },
                new { ftsoci = CompanyID, ftanno = Year.HasValue ? Year.Value : 0, ftcodc = CustomerID, todate = ToDate },
                splitOn: "ftsoci");

            return list.Sum(sum => sum.NetPrice);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public List<FATTT00F>? GetINTRA(string CompanyID, DateTime Period)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<FATTT00F, ABE, CAUFAT00F, FATTT00F>(
                @"select h.*,ca.* from FATTT00F as h
                        INNER JOIN ABE AS c ON c.abecod = h.FTCODC
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = h.FTCAUS
                        WHERE h.ftsoci = @ftsoci AND YEAR(h.FTANNO) = YEAR(@period) AND MONTH(h.FTDAOR) = MONTH(@period) AND ca.fatCEE = 'S' AND ca.fatEXTCEE = 'N' AND ca.fatITA = 'N' AND (ca.fattipo = 'V' OR ca.fattipo ='C')",
                (inv, cus, sel) =>
                {
                    inv.Customer = cus;
                    inv.Causal = sel;
                    return inv;
                },
                new { ftsoci = CompanyID, period = Period }, splitOn: "abecod,fatcod")
                .ToList();

            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);

            foreach (var inv in result)
            {
                inv.Rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F, tab_articolo, tab_articolo_unita, AGENTI, AGENTI, FATTD00F>(
                        $@"SELECT r.*, (CASE WHEN asspla = 'S' THEN 1 ELSE 0 END) AS HasPlafond, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes FROM FATTD00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.FDCOAG1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.FDCOAG2
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR",
                        (row, prd, um, age1, age2) =>
                        {
                            row.UMsCache = umsCache;
                            row.Product = prd;
                            row.UM = um;
                            row.FirstAgent = age1;
                            row.SecondAgent = age2;
                            row.CustomerDiscount = inv.FTSCCL ?? 0;
                            return row;
                        },
                        new { ftsoci = inv.ftsoci, FTANNO = inv.FTANNO, FTNUOR = inv.FTNUOR }, splitOn: "SocietaID,SocietaID,agecod,agecod").ToList());
            }

            return result;


        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region eInvoice
    public string? GenerateInvoiceXML(string CompanyID, int InvoiceYear, int InvoiceNumber, bool IsRegenerate)
    {
        try
        {

            var orditRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>();
            var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();
            var fattalRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTAL00FRepository>();
            var notecliRepo = VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>();
            var abeExtDestRepo = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();

            var caufatRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>();
            var caucontRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>();
            var libriIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>();
            var fattautRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>();

            CultureInfo culture = new CultureInfo("en-US");
            XNamespace p = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2";
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            //if (false) // TODO semplificata non la invieremo mai
            //{
            //    XNamespace ps = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.0";
            //}
            // retrieve invoice
            var invoice = Get(CompanyID, InvoiceYear, InvoiceNumber);

            if (invoice != null)
            {
                invoice.Rows = new ObservableCollection<FATTD00F>(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().GetListWithRate(CompanyID, InvoiceYear, InvoiceNumber) ?? new List<FATTD00F>());

                #region Company infos

                var company = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)!;
                var companyBase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(CompanyID)!;

                string? senderVAT = !string.IsNullOrWhiteSpace(company.azcofi) ? company.azcofi.Trim() : company.azpaiv?.Trim();
                var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, InvoiceYear)!;
                #endregion

                #region Customer infos

                var customerABE = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(invoice.FTCODC ?? 0);
                if (customerABE == null)
                {
                    ErrorHandler.Show("Cliente non trovato");
                    return null;
                }
                if (!customerABE.abecap.HasValue || customerABE.abecap < 0)
                {
                    ErrorHandler.Show("Il c.a.p. del cliente contiene un valore non valido");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(customerABE.abeloc))
                {
                    ErrorHandler.Show("La località del cliente contiene un valore non valido");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(customerABE.abepro))
                {
                    ErrorHandler.Show("La provincia del cliente contiene un valore non valido");
                    return null;
                }
                var client = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(invoice.FTCODC ?? 0);

                if (client == null)
                {
                    ErrorHandler.Show("CLIENTI non trovata");
                    return null;
                }

                var customerISO = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Get(customerABE.isocod ?? string.Empty);
                string intraFlag = "N";
                if (customerABE.isocod != "IT")
                    intraFlag = customerISO?.isointr ?? "N";

                #endregion

                #region Other infos
                var causalInvoice = caufatRepo.Get(invoice.FTCAUS ?? string.Empty);

                var causalAccounting = caucontRepo.Get(causalInvoice?.fatcon ?? string.Empty);

                var ivaBook = libriIvaRepo.Get(causalAccounting?.cauliv ?? string.Empty);

                var customizationSelf = fattautRepo.Get(CompanyID, InvoiceYear, InvoiceNumber);
                #endregion

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var numerator = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, now.Year, Constants.E_INVOICE_GENERAL, true);

                #region Generate file name
                var unixTime = new DateTimeOffset(now).ToUnixTimeMilliseconds();
                var uniqueID = TextHelper.EncodeToBase62(unixTime);
                var neededPre = 10 - uniqueID.Length;
                string fullPath = $"{Path.GetTempPath()}IT{senderVAT}{uniqueID.Substring(0, neededPre).PadLeft(5, '0')}_{uniqueID.Substring(neededPre - 1, 5)}.xml";
                #endregion

                string transmissionFormat = client != null && client.clpaymBool ? "FPA12" : "FPR12";
                string numerazione = invoice.PrintFullID;

                XElement root = new XElement(p + "FatturaElettronica",
                    new XAttribute(XNamespace.Xmlns + "p", p.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "ds", ds.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                    new XAttribute("versione", transmissionFormat));

                #region Recipient IDs
                string? recipientID;
                string? recipientPEC = null;
                if (customerABE.isocod == "IT")
                {
                    if (client!.clpaymBool)
                    {
                        recipientID = client!.clcouff;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(client.clcoddest))
                        {
                            recipientID = client.clcoddest;
                        }
                        else
                        {
                            recipientID = "0000000";
                            recipientPEC = client.clpec;
                        }
                    }
                }
                else
                {
                    recipientID = company.azcodcee;
                }
                #endregion

                XElement header = new XElement("FatturaElettronicaHeader",
                    new XElement("DatiTrasmissione",
                    new XElement("IdTrasmittente",
                        new XElement("IdPaese", "IT"),
                        new XElement("IdCodice", senderVAT)),
                    new XElement("ProgressivoInvio", $"{now.Year}{numerator.ToString().PadLeft(6, '0')}"),
                    new XElement("FormatoTrasmissione", transmissionFormat),
                    new XElement("CodiceDestinatario", recipientID),
                    new XElement("ContattiTrasmittente", new XElement("Telefono", company.AZTel?.Trim())),
                        (!string.IsNullOrWhiteSpace(recipientPEC) ? new XElement("PECDestinatario", recipientPEC) : null))
                    );

                #region 1.2
                if (invoice.FTTIPO != "A" && invoice.FTTIPO != "C")
                {
                    XElement cedentePrestatore = new XElement("CedentePrestatore",
                        new XElement("DatiAnagrafici",
                            new XElement("IdFiscaleIVA",
                                new XElement("IdPaese", "IT"),
                                new XElement("IdCodice", senderVAT?.Trim())),
                            new XElement("Anagrafica",
                                new XElement("Denominazione", TextHelper.SanitizeFull(companyBase.SOMDES.Trim()))),
                            new XElement("RegimeFiscale", company.azregifatt)),
                        new XElement("Sede",
                            new XElement("Indirizzo", TextHelper.SanitizeFull(company.azinsl?.Trim())),
                            new XElement("CAP", company.azcasl.HasValue ? company.azcasl.Value.ToString().PadLeft(5, '0') : "00000"),
                            new XElement("Comune", TextHelper.SanitizeFull(company.azlosl)),
                            new XElement("Provincia", company.azprsl),
                            new XElement("Nazione", "IT")),
                        new XElement("IscrizioneREA",
                            new XElement("Ufficio", company.azprsl),
                            new XElement("NumeroREA", company.azcodrea?.Trim()),
                            new XElement("CapitaleSociale", company.azcapsoc?.ToString("F2", culture)),
                            new XElement("SocioUnico", company.azsocuni),
                            new XElement("StatoLiquidazione", company.azstaliq)));

                    header.Add(cedentePrestatore);
                }
                else
                {
                    // autofattura
                    var supplier = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(customizationSelf?.FTAUCOF ?? 0);
                    var supplierISO = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Get(supplier?.isocod ?? string.Empty);
                    XElement cedentePrestatore = new XElement("CedentePrestatore",
                        new XElement("DatiAnagrafici",
                            new XElement("IdFiscaleIVA",
                                new XElement("IdPaese", supplierISO?.isocod3166),
                                new XElement("IdCodice", supplier?.abepiv?.Trim())),
                            new XElement("Anagrafica",
                                new XElement("Denominazione", TextHelper.Truncate(TextHelper.SanitizeFull(supplier?.abers1?.Trim()), 80))),
                            new XElement("RegimeFiscale", company.azregifatt)),
                        new XElement("Sede",
                            new XElement("Indirizzo", TextHelper.Truncate(TextHelper.SanitizeFull(supplier?.abeind.Trim()), 60)),
                            new XElement("CAP", supplier != null && supplier.abecap.HasValue ? supplier.abecap.Value.ToString().PadLeft(5, '0') : "00000"),
                            new XElement("Comune", TextHelper.Truncate(TextHelper.SanitizeFull(supplier?.abeloc), 60)),
                            new XElement("Provincia", supplier?.abepro),
                            new XElement("Nazione", supplierISO?.isocod3166)));

                    header.Add(cedentePrestatore);
                }
                #endregion

                #region 1.4

                XElement? idFiscaleIVA = null;
                XElement? anagrafica = null;
                XElement? codiceFiscale = null;
                if (customerISO?.isocod3166 == "IT" || intraFlag == "S")
                {
                    if (customerABE.abetpo != "2")
                    {
                        idFiscaleIVA = new XElement("IdFiscaleIVA",
                        new XElement("IdPaese", customerISO?.isocod3166),
                        new XElement("IdCodice", customerABE.abepiv));
                        if (customerISO?.isocod3166 == "IT" && !string.IsNullOrWhiteSpace(customerABE.abecfi))
                            codiceFiscale = new XElement("CodiceFiscale", customerABE.abecfi.Trim());
                        anagrafica = new XElement("Anagrafica",
                        new XElement("Denominazione", TextHelper.Truncate($"{TextHelper.SanitizeFull(customerABE.abers1?.Trim())} {TextHelper.SanitizeFull(customerABE.abers2?.Trim())}", 80)));
                    }
                    else
                    {
                        codiceFiscale = new XElement("CodiceFiscale", customerABE.abecfi?.Trim());
                        anagrafica = new XElement("Anagrafica",
                            new XElement("Nome", !string.IsNullOrWhiteSpace(customerABE.abers2) ? TextHelper.SanitizeFull(customerABE.abers2?.Trim()) : TextHelper.SanitizeFull(customerABE.abers1?.Trim())),
                            new XElement("Cognome", TextHelper.SanitizeFull(customerABE.abers1?.Trim())));
                    }
                }
                else
                {
                    idFiscaleIVA = new XElement("IdFiscaleIVA",
                        new XElement("IdPaese", customerISO?.isocod3166),
                        new XElement("IdCodice", $"{company.azisoextracee?.Trim()} {company.azcodextracee?.Trim()}"));
                    anagrafica = new XElement("Anagrafica",
                        new XElement("Denominazione", TextHelper.Truncate($"{TextHelper.SanitizeFull(customerABE.abers1?.Trim())} {TextHelper.SanitizeFull(customerABE.abers2?.Trim())}", 80)));
                }

                XElement cessionarioCommittente = new XElement("CessionarioCommittente",
                    new XElement("DatiAnagrafici",
                        idFiscaleIVA,
                        codiceFiscale,
                        anagrafica),
                    new XElement("Sede",
                        new XElement("Indirizzo", TextHelper.Truncate(TextHelper.SanitizeFull(customerABE.abeind.Trim()), 60)),
                        new XElement("CAP", (customerABE.abecap.HasValue && customerABE.abecap.Value > 0 ? customerABE.abecap?.ToString().PadLeft(5, '0') : "00000")),
                        new XElement("Comune", TextHelper.Truncate(TextHelper.SanitizeFull(customerABE.abeloc.Trim()), 60)),
                        (!string.IsNullOrWhiteSpace(customerABE.abepro) ? new XElement("Provincia", customerABE.abepro.Trim()) : null),
                        new XElement("Nazione", customerISO?.isocod3166)));

                header.Add(cessionarioCommittente);
                #endregion

                if (company.azuseei)
                {
                    #region 1.5 Intermediario

                    XElement intermediario = new XElement("TerzoIntermediarioOSoggettoEmittente",
                        new XElement("DatiAnagrafici",
                            new XElement("IdFiscaleIVA",
                                new XElement("IdPaese", "IT"),
                                new XElement("IdCodice", "07837340962")),
                            new XElement("CodiceFiscale", "07837340962"),
                            new XElement("Anagrafica",
                                new XElement("Denominazione", "CERBER Tech S.r.l."))));

                    header.Add(intermediario);

                    #endregion
                }

                if (invoice.FTTIPO == "A")
                {
                    // se autofattura aggiunge CC
                    header.Add(new XElement("SoggettoEmittente", "CC"));
                }

                root.Add(header);

                DateTime data = invoice.FTDAOR ?? DateTime.MinValue;
                XElement body = new XElement("FatturaElettronicaBody");

                var dichiarazioneIntento = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().GetLast(CompanyID, invoice.FTCODC ?? 0, invoice.FTANNO, invoice.FTDAOR ?? DateTime.MinValue, IsRegenerate);
                string? causalDecription = null;
                if (dichiarazioneIntento != null && !string.IsNullOrWhiteSpace(dichiarazioneIntento.clinumprotuffiva))
                {
                    causalDecription = $"{TextHelper.SanitizeFull(causalInvoice?.fatdes.Trim())} Ricevuta dich. intento prot.n. {dichiarazioneIntento.clinumprotuffiva.Trim()}";
                }
                else
                {
                    causalDecription = causalInvoice?.fatdes.Trim();
                }

                // bollo
                XElement? bollo = null;
                var bolloProductRow = invoice.Rows.Where(w => w.FDRIGA == 999999).FirstOrDefault();
                if (bolloProductRow != null)
                {
                    bollo = new XElement("DatiBollo",
                    new XElement("BolloVirtuale", "SI"),
                    new XElement("ImportoBollo", Math.Round(bolloProductRow.Amount, 2).ToString("F2", culture)));
                }

                XElement datiGenerali = new XElement("DatiGenerali");
                XElement datiGeneraliDocumento = new XElement("DatiGeneraliDocumento",
                            new XElement("TipoDocumento", invoice.fttdoc?.Trim()),
                            new XElement("Divisa", invoice.ftciva?.Trim()),
                            new XElement("Data", data.ToString("yyyy-MM-dd")),
                            new XElement("Numero", numerazione),
                            bollo,
                            new XElement("ImportoTotaleDocumento", invoice.GrandTotal.ToString("F2", culture)),
                            new XElement("Causale", causalDecription));

                datiGenerali.Add(datiGeneraliDocumento);

                // 2.1.2 Dati ordine di acquisto
                foreach (var row in invoice.Rows.Where(w => w.OTANN1.HasValue && w.OTANN1.Value > 0 && w.OTNUO1.HasValue && w.OTNUO1.Value > 0 && w.ODRIG1.HasValue && w.ODRIG1.Value > 0)
                                                .GroupBy(g => new { g.OTANN1, g.OTNUO1, g.ODRIG1 }, (key, items) => new { key, items }))
                {
                    var refLines = new List<XElement>();
                    foreach (var inv in row.items)
                    {
                        refLines.Add(new XElement("RiferimentoNumeroLinea", inv.FDRIGA));
                    }
                    var order = orditRepo.Get(invoice.ftsoci, row.key.OTANN1 ?? 0, row.key.OTNUO1 ?? 0);
                    if (order != null && !string.IsNullOrWhiteSpace(order.OTCUNO) && order.OTCUDO.HasValue)
                    {
                        datiGenerali.Add(new XElement("DatiOrdineAcquisto",
                            refLines,
                            new XElement("IdDocumento", order.OTCUNO.Trim().Length > 20 ? order.OTCUNO.Trim().Substring(0, 20) : order.OTCUNO.Trim()),
                            new XElement("Data", order.OTCUDO.Value.ToString("yyyy-MM-dd")),
                            new XElement("NumItem", row.key.ODRIG1 ?? 0))); // questa è la riga del nostro ODA 
                    }
                }

                // 2.1.6 DatiFattureCollegate
                if (invoice.FTTIPO == "A")
                {
                    if (!string.IsNullOrWhiteSpace(customizationSelf?.FTAUINDSDI))
                    {
                        // add DatiFattureCollegate
                        datiGenerali.Add(new XElement("DatiFattureCollegate",
                            new XElement("IdDocumento", customizationSelf?.FTAUINDSDI),
                            new XElement("Data", (customizationSelf?.FTAUDATRIC ?? DateTime.MinValue).ToString("yyyy-MM-dd"))));
                    }
                }
                else
                {
                    if (invoice.ftannf.HasValue)
                    {
                        // add DatiFattureCollegate
                        datiGenerali.Add(new XElement("DatiFattureCollegate",
                            new XElement("IdDocumento", (invoice.ftnufa ?? 0).ToString().PadLeft(6, '0')),
                            new XElement("Data", (invoice.ftdafa ?? DateTime.MinValue).ToString("yyyy-MM-dd"))));
                    }
                }

                // 2.1.8 Dati DDT
                var ddtList = invoice.Rows.Where(w => w.FDBOLL.HasValue && w.FDBOLL.Value > 0).Select(s => $"{s.FDBONO}{s.FDBOLL}").Distinct().ToList().ToArray<string>();
                var neededDDTs = bolltRepo.GetFromList(CompanyID, ddtList);
                foreach (var row in invoice.Rows.Where(w => w.FDBOLL.HasValue && w.FDBOLL.Value > 0))
                {
                    var ddt = neededDDTs?.Where(w => w.BTANNO == row.FDBONO && w.BTBOLL == row.FDBOLL).First();
                    datiGenerali.Add(new XElement("DatiDDT",
                        new XElement("NumeroDDT", ddt?.DefinitiveID),
                        new XElement("DataDDT", (ddt?.BTDATA ?? DateTime.MinValue).ToString("yyyy-MM-dd")),
                        new XElement("RiferimentoNumeroLinea", row.FDRIGA)));
                }

                // 2.1.9 Dati trasporto
                if ((invoice.FTCODD ?? 0) > 0)
                {
                    var destinatario = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().Get(invoice.FTCODC ?? 0, invoice.FTCODD ?? 0);

                    if (destinatario != null)
                    {
                        XElement datiTrasporto = new XElement("DatiTrasporto");
                        XElement datiDestinatario = new XElement("IndirizzoResa",
                               new XElement("Indirizzo", destinatario.DEINDI?.Trim()),
                                 new XElement("NumeroCivico", "1"),
                                 new XElement("CAP", destinatario.DECAPText),
                                 new XElement("Comune", destinatario.deloc),
                                 new XElement("Provincia", destinatario.depro),
                                 new XElement("Nazione", destinatario.isocod));
                        datiTrasporto.Add(datiDestinatario);

                        datiGenerali.Add(datiTrasporto);
                    }
                }

                body.Add(datiGenerali);

                // righe
                var datiBeniServizi = new XElement("DatiBeniServizi");
                int rowsCount = 1;

                foreach (var row in invoice.Rows.Where(w => w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)).OrderBy(o => o.FDRIGA))
                {
                    #region Product description
                    string? productDescription = row.Product?.Descrizione;
                    // note testata fattura
                    if (rowsCount == 1 && !string.IsNullOrWhiteSpace(invoice.FTNOTET) && invoice.FTSHOWT)
                        productDescription += $" {invoice.FTNOTET.Trim()}";
                    // note articolo
                    if (company.azpnotinv)
                    {
                        productDescription += $" {row.Product?.Note}";
                    }
                    // note riga
                    if (!string.IsNullOrWhiteSpace(row.FDNOTE) && row.FDSHOW)
                        productDescription += $" {row.FDNOTE.Trim()}";
                    // note bolla cliente
                    if (row.FDBONO.HasValue && row.FDBOLL.HasValue)
                    {
                        var ddt = bolltRepo.Get(CompanyID, row.FDBONO.Value, row.FDBOLL.Value);
                        if (ddt != null)
                        {
                            productDescription += $" {ddt.BTNOTET?.Trim()}";
                        }
                    }
                    // note pie' di pagina fattura e note cliente
                    if (rowsCount == invoice.Rows.Count())
                    {
                        if (!string.IsNullOrWhiteSpace(invoice.FTNOTEP) && invoice.FTSHOWP)
                            productDescription += $" {invoice.FTNOTEP?.Trim()}";

                        var customerNotes = notecliRepo.GetListForInvoices(invoice.FTCODC ?? 0);
                        if (customerNotes != null && customerNotes.Count > 0)
                        {
                            foreach (var note in customerNotes)
                            {
                                productDescription += $" {note.notdes?.Trim()}";
                            }
                        }
                    }
                    // sanitize product description
                    productDescription = productDescription?.Replace("€", "euro");
                    if (productDescription?.Length > 1000)
                        productDescription = productDescription?.Substring(0, 1000);
                    productDescription = TextHelper.SanitizeFull(productDescription);
                    #endregion

                    decimal quantity = row.FDQTAV ?? 0;
                    XElement? um = null;
                    if (!string.IsNullOrWhiteSpace(row.FDUNIM))
                    {
                        um = new XElement("UnitaMisura", row.FDUNIM.Trim());
                    }

                    #region Sconti e maggiorazioni

                    XElement? discount1 = null;
                    if (row.FDSCO1.HasValue && row.FDSCO1.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDSCO1.Value;
                        if (row.FDTSC1 == "P")
                            ItemName = "Percentuale";
                        discount1 = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "SC"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    XElement? discount2 = null;
                    if (row.FDSCO2.HasValue && row.FDSCO2.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDSCO2.Value;
                        if (row.FDTSC2 == "P")
                            ItemName = "Percentuale";
                        discount2 = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "SC"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    XElement? discount3 = null;
                    if (row.FDSCO3.HasValue && row.FDSCO3.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDSCO3.Value;
                        if (row.FDTSC3 == "P")
                            ItemName = "Percentuale";
                        discount3 = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "SC"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    XElement? maggiorazione = null;
                    if (row.FDMAGG.HasValue && row.FDMAGG.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDMAGG.Value;
                        if (row.FDTMAG == "P")
                            ItemName = "Percentuale";
                        maggiorazione = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "MG"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    #endregion

                    XElement? natura = null;
                    decimal rateNatureValue = 0;
                    if (!decimal.TryParse(row.FDALIV?.Trim(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out rateNatureValue))
                        natura = new XElement("Natura", row.fdtiva?.Trim());

                    #region AltriDatiGestionali
                    List<XElement> altri = new List<XElement>();
                    // plafond info
                    if (row.Rate?.asspla == "S")
                    {
                        altri.Add(new XElement("AltriDatiGestionali",
                            new XElement("TipoDato", "INTENTO"),
                            new XElement("RiferimentoTesto", dichiarazioneIntento?.clinumprotuffiva?.Trim()),
                            new XElement("RiferimentoData", (dichiarazioneIntento?.clidatuffiva ?? DateTime.MinValue).ToString("yyyy-MM-dd"))));
                    }
                    // recipient decoding
                    if (invoice.FTCODD.HasValue)
                    {
                        var extRecipients = abeExtDestRepo.GeteInvoiceRecipientList(invoice.FTCODC ?? 0, invoice.FTCODD ?? 0);
                        if (extRecipients != null && extRecipients.Count > 0)
                        {
                            foreach (var ext in extRecipients)
                            {
                                altri.Add(new XElement("AltriDatiGestionali",
                                    new XElement("TipoDato", ext.abeextcode.Split('#')[1]),
                                    new XElement("RiferimentoTesto", ext.abeextdid)));
                            }
                        }
                    }
                    #endregion

                    decimal rateRowValue = 0;
                    if (!decimal.TryParse(row.FDALIV?.Trim(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out rateRowValue))
                        rateRowValue = 0;
                    var newLine = new XElement("DettaglioLinee",
                        new XElement("NumeroLinea", row.FDRIGA != 999999 ? row.FDRIGA : invoice.Rows.Where(w => w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)).Count()),
                        (row.FDTQTA == "O" ? new XElement("TipoCessionePrestazione", "SC") : null),
                            new XElement("CodiceArticolo",
                                new XElement("CodiceTipo", "CPV"),
                                new XElement("CodiceValore", row.FDCODA?.Trim())),
                                new XElement("Descrizione", productDescription),
                                new XElement("Quantita", quantity.ToString("F6", culture)),
                                um,
                                new XElement("PrezzoUnitario", row.FDTQTA == "V" || row.FDTQTA == "B" || row.FDTQTA == "O" ? (row.FDTPRE == "U" ? (row.FDPREZ ?? 0).ToString("F6", culture) : Math.Round((row.FDPREZ ?? 0) / (row.FDQTAV ?? 1), 2, MidpointRounding.AwayFromZero).ToString("F6", culture)) : (row.FDTPRE == "U" ? ((row.FDPREZ ?? 0) * -1).ToString("F6", culture) : Math.Round((((row.FDPREZ ?? 0) / (row.FDQTAV ?? 1)) * -1), 2, MidpointRounding.AwayFromZero).ToString("F6", culture))),
                                discount1,
                                discount2,
                                discount3,
                                maggiorazione,
                                new XElement("PrezzoTotale", row.NetPrice.ToString("F2", culture)),
                                new XElement("AliquotaIVA", rateRowValue.ToString("F2", culture)),
                                natura);
                    if (altri != null && altri.Count > 0)
                    {
                        newLine.Add(altri);
                    }
                    datiBeniServizi.Add(newLine);
                    rowsCount += 1;
                }

                #region Dati riepilogo

                foreach (var aliq in invoice.Rows.GroupBy(g => new { g.FDALIV, g.FDASSF }, (key, items) => new { key, items }))
                {
                    XElement? riepNatura = null;
                    XElement? riepRife = null;
                    XElement? riepEsig = null;

                    if (aliq.items.First().Rate?.asssplpayBool ?? false)
                    {
                        riepEsig = new XElement("EsigibilitaIVA", "S");
                    }
                    else
                    {
                        if (esercizio.eseivavenBool)
                            riepEsig = new XElement("EsigibilitaIVA", "D");
                        else
                            riepEsig = new XElement("EsigibilitaIVA", "I");
                    }

                    var taxable = aliq.items.Sum(sum => sum.NetPrice);
                    decimal taxes = 0;
                    decimal ratePerc = 0;
                    if (decimal.TryParse(aliq.key.FDALIV?.Trim(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out ratePerc))
                    {
                        taxes = Math.Round(taxable * ratePerc / 100, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        taxes = 0;
                        riepNatura = new XElement("Natura", aliq.items.First().Rate?.assnatufe?.Trim());
                        var nature = VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().Get(aliq.items.First().Rate?.assnatufe ?? string.Empty);
                        riepRife = new XElement("RiferimentoNormativo", $"{nature?.FETIDes.Trim()}");
                    }


                    var datiRiepilogo = new XElement("DatiRiepilogo",
                    new XElement("AliquotaIVA", ratePerc.ToString("F2", culture)),
                    riepNatura,
                    new XElement("ImponibileImporto", taxable.ToString("F2", culture)),
                    new XElement("Imposta", taxes.ToString("F2", culture)),
                    riepEsig,
                    riepRife);

                    datiBeniServizi.Add(datiRiepilogo);
                }
                #endregion

                body.Add(datiBeniServizi);

                #region Dati pagamento

                XElement datiPagamento = new XElement("DatiPagamento");
                var expires = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().ComputeExpires(CompanyID, invoice.FTPAGA ?? string.Empty, invoice.FTDAOR ?? DateTime.MinValue, invoice.FTCODC ?? 0);
                var condizioniPagamento = new XElement("CondizioniPagamento", expires?.Count == 1 ? "TP02" : "TP01");
                var paymentFull = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetFull(invoice.FTPAGA ?? string.Empty);
                decimal stepAmount = Math.Round(invoice.GrandTotal / (expires?.Count ?? 1), 2);
                decimal finalStepAmount = Math.Round((stepAmount + (invoice.GrandTotal - (stepAmount * (expires?.Count ?? 1)))), 2);
                ABICAB? bank = null;
                BANAZIEN? companyBank = null;
                if (invoice.FTABIB.HasValue && invoice.FTABIB.Value > 0 &&
                    invoice.FTCABB.HasValue && invoice.FTCABB.Value > 0)
                {
                    bank = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(invoice.FTABIB.Value, invoice.FTCABB.Value);
                    companyBank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(invoice.ftsoci, invoice.FTABIB.Value, invoice.FTCABB.Value, invoice.FTBCON ?? string.Empty);
                }
                datiPagamento.Add(condizioniPagamento);
                int currentRow = 1;
                foreach (var exp in expires ?? new List<DateTime>())
                {
                    datiPagamento.Add(new XElement("DettaglioPagamento",
                        new XElement("ModalitaPagamento", paymentFull?.Incasso?.icsfepacod),
                        new XElement("DataScadenzaPagamento", exp.ToString("yyyy-MM-dd")),
                        new XElement("ImportoPagamento", currentRow == expires?.Count ? finalStepAmount : stepAmount),
                        bank != null ? new XElement("IstitutoFinanziario", $"{TextHelper.SanitizeFull(bank.abiban.Trim())} {TextHelper.SanitizeFull(bank.abiage?.Trim())}") : null,
                        bank != null && companyBank != null ? new XElement("IBAN", companyBank.abibiba) : null,
                        bank != null ? new XElement("ABI", bank.abiabi.ToString().PadLeft(5, '0')) : null,
                        bank != null ? new XElement("CAB", bank.abicab.ToString().PadLeft(5, '0')) : null));

                    currentRow += 1;
                }

                body.Add(datiPagamento);
                #endregion

                #region Allegati

                List<string> compressedExtensions = new List<string>() {
                "zip", "rar", "br", "bz2", "gz", "lz", "lz4", "lzma", "lzo", "lz", "sfark", "sz",
                "xz", "z", "zst", "7z", "s7z", "ace", "afa", "apk", "arj", "cab", "dmg", "gca",
                "ice", "jar", "lza", "lzh", "lzx", "pak", "pim", "sfx", "shk", "sit", "sitx",
                "sqx", "tar.gz", "tgz", "tar.Z", "tar.bz2", "tbz2", "tar.lz", "tlz", "tar.xz",
                "txz", "tar.zst", "tar", "war", "wim", "xar", "xp3", "xz1", "zipx", "zoo", "zpaq", "zz"};

                foreach (var attach in fattalRepo.GetList(CompanyID, InvoiceYear, InvoiceNumber) ?? new ObservableCollection<FATTAL00F>())
                {
                    string attachmentName = attach.FTANAME.Trim();
                    if (attachmentName.Length > 60)
                        attachmentName = attachmentName.Substring(0, 60);
                    string attachmentDescription = $"{attachmentName} {attach.FTASIZE} bytes";
                    if (attachmentDescription.Length > 100)
                        attachmentDescription = attachmentDescription.Substring(0, 100);
                    string attachmentFormat = Path.GetExtension(attach.FTANAME.Trim()).Trim().ToUpper();
                    attachmentFormat = attachmentFormat.Substring(1, attachmentFormat.Length - 1);
                    if (attachmentFormat.Length > 10)
                        attachmentFormat = attachmentFormat.Substring(0, 10);

                    XElement? algoritmoCompressione = null;
                    if (compressedExtensions.Contains(attachmentFormat.ToLower()))
                    {
                        algoritmoCompressione = new XElement("AlgoritmoCompressione", attachmentFormat);
                    }

                    byte[]? attachmentData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER,
                        $"{companyBase.SOCUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{attach.FTAUID}");

                    if (attachmentData != null)
                    {
                        body.Add(new XElement("Allegati",
                            new XElement("NomeAttachment", TextHelper.SanitizeFull(attachmentName)),
                            new XElement("FormatoAttachment", attachmentFormat),
                            new XElement("DescrizioneAttachment", TextHelper.SanitizeFull(attachmentDescription)),
                            algoritmoCompressione,
                            new XElement("Attachment", System.Convert.ToBase64String(attachmentData))));
                    }
                }

                #endregion

                root.Add(body);
                root.Save(fullPath);
                return fullPath;
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    #region Accounting
    public bool Accounting(FATTT00F Invoice, ESERCIZIO AccountingYear, DateTime AccountingDate, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var pnClientiRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>();
                var pnPortafoglioRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>();
                var pnIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>();
                var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var plafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var plafondRowsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var plafondParmsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var tipiIncRepo = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>();
                // get registration number
                var accountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Invoice.ftsoci, AccountingYear.eseann, Constants.PN, true);
                // self invoice info
                FATTAUT? fattaut = null;
                FORNAMMI? fornammi = null;
                PDCSOTTO? pdcSottoSupplier = null;
                if (Invoice.FTTIPO == "A" || Invoice.FTTIPO == "C")
                {
                    fattaut = VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().Get(Invoice.ftsoci, Invoice.FTANNO, Invoice.FTNUOR);
                    fornammi = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(Invoice.ftsoci, fattaut?.FTAUCOF ?? 0);
                    pdcSottoSupplier = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(fornammi?.foGRUP ?? string.Empty, fornammi?.foCONT ?? string.Empty, "F", Invoice.ftsoci);
                }
                // caucont from caufat
                var caufatt = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Get(Invoice.FTCAUS ?? string.Empty);
                Invoice.Causal = caufatt;
                var caucont = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Get(caufatt?.fatcon ?? string.Empty);
                // CLIAMMI
                var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Invoice.ftsoci, Invoice.FTCODC ?? 0);
                if (cliammi == null)
                {
                    ErrorHandler.Show($"Non trovati i dati amministrativi del cliente");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(cliammi.clGRUP) || string.IsNullOrWhiteSpace(cliammi.clcont))
                {
                    ErrorHandler.Show($"Manca il conto contabile del cliente");
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(cliammi.clGRUP, cliammi.clcont, "C", Invoice.ftsoci);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show($"Non trovato il sottoconto cliente nel gruppo {cliammi.clGRUP} e conto {cliammi.clcont}");
                    return false;
                }
                // IVA book
                var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(caucont?.cauliv ?? string.Empty);
                // CAUCONT_GROUPS
                var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(Invoice.ftsoci, caucont?.caucod ?? string.Empty);
                // PAGCLI/TAB_ACC_TIPINC
                var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(Invoice.FTPAGA ?? string.Empty);
                if (pagcli == null)
                {
                    ErrorHandler.Show($"Non trovato il codice pagamento sulla fattura");
                    return false;
                }
                var incassi = tipiIncRepo.Get(pagcli.pcltip ?? string.Empty);
                if (incassi == null)
                {
                    ErrorHandler.Show($"Non trovato il tipo pagamento sul codice pagamento {pagcli.FullDescriptionSearchable}");
                    return false;
                }
                // Expires
                var expires = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().ComputeExpires(Invoice.ftsoci, Invoice.FTPAGA ?? string.Empty, Invoice.FTDAOR ?? DateTime.MinValue, Invoice.FTCODC ?? 0);
                // invoice vs credit parms
                string? customerRowSign = null;
                string? otherRowsSign = null;
                string? ivaSign = null;
                if (Invoice.FTTIPO == "F" || Invoice.FTTIPO == "A" || Invoice.FTTIPO == "B")
                {
                    customerRowSign = "D";
                    otherRowsSign = "A";
                    ivaSign = "+";
                }
                else
                {
                    customerRowSign = "A";
                    otherRowsSign = "D";
                    ivaSign = "-";
                }
                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = Invoice.ftsoci,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = accountingID,
                    pncaus = caucont?.caucod,
                    N1DARE = AccountingDate.Date,
                    N1docn = Invoice.FTNUFD.ToString(),
                    N1docd = Invoice.FTDAOR,
                    N1rifn = Invoice.PrintFullID,
                    N1rifd = Invoice.FTDAOR,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Invoice.FTCODC,
                    N1FLCF = "C",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                if (!caucont?.causolBool ?? true)
                {
                    #region Customer row
                    var customerRows = new List<PNRIGHE>();
                    PNRIGHE customerRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = head.N1rifn,
                        N1DARI = head.N1rifd,
                        n1clie = Invoice.FTCODC,
                        N1SEGN = customerRowSign,
                        pngrup = pdcSotto.P1GRUP,
                        pncont = pdcSotto.P2CONT,
                        pnsott = pdcSotto.P3SOTC,
                        N1IMEU = Invoice.GrandTotalWithGift,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = Invoice.FTPAGA,
                        n1scad = Invoice.FTSCAD,
                        N1DRri = head.N1docd
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    customerRows.Add(customerRow);
                    // GIFT
                    if ((Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Any(any => any.FDTQTA == "O"))
                    {
                        PNRIGHE customerGiftRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            n1clie = Invoice.FTCODC,
                            N1SEGN = otherRowsSign,
                            pngrup = pdcSotto.P1GRUP,
                            pncont = pdcSotto.P2CONT,
                            pnsott = pdcSotto.P3SOTC,
                            N1IMEU = Invoice.TotalGifts,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            n1paga = Invoice.FTPAGA,
                            n1scad = Invoice.FTSCAD,
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, customerGiftRow, transaction);
                        customerRows.Add(customerGiftRow);
                    }
                    #endregion

                    #region IVA row
                    if (Invoice.TotalVAT > 0)
                    {
                        foreach (var cg in grpcau ?? new ObservableCollection<CAUCONT_GROUPS>())
                        {
                            PNRIGHE ivaRow = new PNRIGHE()
                            {
                                N1SOCI = head.N1SOCI,
                                N1ANNO = head.N1ANNO,
                                N1REGI = head.N1REGI,
                                N1RIGA = rowsCounter++,
                                N1DOCU = head.N1docn,
                                N1DADO = head.N1docd,
                                N1RIFE = head.N1rifn,
                                N1DARI = head.N1rifd,
                                N1SEGN = cg.grpseg,
                                pngrup = cg.grpgrp,
                                pncont = cg.grpcto,
                                pnsott = cg.grpsct,
                                N1IMEU = grpcau?.Count == 1 ? Invoice.TotalVAT : 0,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DRri = head.N1docd,
                                N1tmpPNR = "N",
                                N1DESC = Invoice.Customer?.FullDescriptionSearchable
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, ivaRow, transaction);
                        }
                    }
                    #endregion

                    #region Ricavi rows
                    List<AccountingAccount> accounts = new List<AccountingAccount>();
                    foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1))
                        .OrderBy(o => o.FDGRUP).ThenBy(o => o.FDCONT).ThenBy(o => o.FDSCTO).ThenBy(o => o.FDCECO))
                    {
                        var newAccount = new AccountingAccount() { GroupID = row.FDGRUP, AccountID = row.FDCONT, SubaccountID = row.FDSCTO, CostCenter = row.FDCECO, ProductCostCenter = row.Product?.costcenter_id };
                        if (!accounts.Contains(newAccount))
                        {
                            accounts.Add(newAccount);
                        }
                    }
                    foreach (var acc in accounts)
                    {
                        var netAmount = (Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                            .Where(w => w.FDGRUP == acc.GroupID && w.FDCONT == acc.AccountID && w.FDSCTO == acc.SubaccountID && w.FDCECO == acc.CostCenter &&
                            (w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)))
                            .Sum(sum => sum.NetPrice);
                        PNRIGHE newRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = otherRowsSign,
                            pngrup = acc.GroupID,
                            pncont = acc.AccountID,
                            pnsott = acc.SubaccountID,
                            N1CCCC = !string.IsNullOrWhiteSpace(acc.CostCenter) ? acc.CostCenter : (!string.IsNullOrWhiteSpace(caucont?.cauceco) ? caucont.cauceco : acc.ProductCostCenter),
                            N1IMEU = netAmount,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                    }
                    // customer discount
                    if (Invoice.ScontiCliente > 0)
                    {
                        // get custom discount account
                        var discAccount = aziendaRepo.Get(Invoice.ftsoci);
                        bool daExists = !string.IsNullOrWhiteSpace(discAccount?.azdisgrp) && !string.IsNullOrWhiteSpace(discAccount.azdiscnt) && !string.IsNullOrWhiteSpace(discAccount.azdissot);
                        // add discount row
                        PNRIGHE discountRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = customerRowSign,
                            pngrup = daExists ? discAccount?.azdisgrp : accounts.First().GroupID,
                            pncont = daExists ? discAccount?.azdiscnt : accounts.First().AccountID,
                            pnsott = daExists ? discAccount?.azdissot : accounts.First().SubaccountID,
                            N1CCCC = Invoice.FTCECO,
                            N1IMEU = Invoice.ScontiCliente,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, discountRow, transaction);
                    }
                    // GIFT
                    if ((Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Any(any => any.FDTQTA == "O"))
                    {
                        // get custom discount account
                        var discAccount = aziendaRepo.Get(Invoice.ftsoci);
                        bool daExists = !string.IsNullOrWhiteSpace(discAccount?.azdisgrp) && !string.IsNullOrWhiteSpace(discAccount.azdiscnt) && !string.IsNullOrWhiteSpace(discAccount.azdissot);
                        // add discount row
                        PNRIGHE giftRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = customerRowSign,
                            pngrup = daExists ? discAccount?.azdisgrp : accounts.First().GroupID,
                            pncont = daExists ? discAccount?.azdiscnt : accounts.First().AccountID,
                            pnsott = daExists ? discAccount?.azdissot : accounts.First().SubaccountID,
                            N1CCCC = Invoice.FTCECO,
                            N1IMEU = Invoice.TotalGifts,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, giftRow, transaction);
                    }
                    #endregion

                    #region PNCLIENTI
                    int customerRowsCounter = 1;
                    foreach (var cliRow in customerRows)
                    {
                        decimal stepAmount = Math.Round((cliRow.N1IMEU ?? 0) / (expires?.Count ?? 1), 2);
                        decimal finalStepAmount = Math.Round((stepAmount + ((cliRow.N1IMEU ?? 0) - (stepAmount * (expires?.Count ?? 1)))), 2);
                        foreach (var exp in expires ?? new List<DateTime>())
                        {
                            PNCLIENTI customer = new PNCLIENTI()
                            {
                                N2SOCI = head.N1SOCI,
                                N2ANNO = head.N1ANNO,
                                N2REGI = head.N1REGI,
                                N2RIGA = customerRowsCounter++,
                                N2DARI = head.N1rifd,
                                N2RIFE = head.N1rifn,
                                N2DOCU = head.N1docn,
                                N2DADO = head.N1docd,
                                N2DARE = AccountingDate.Date,
                                N2CAUS = head.pncaus,
                                N2GRUP = pdcSotto.P1GRUP,
                                N2CONT = pdcSotto.P2CONT,
                                N2SOTT = Invoice.FTCODC,
                                N2SSOC = head.N1SOCI,
                                N2SEGN = cliRow.N1SEGN,
                                N2PAGA = Invoice.FTPAGA,
                                N2SCAD = exp,
                                N2DIVI = "EUR",
                                n2vcod = "UIC",
                                N2DIDO = "EUR",
                                N2VADO = "UIC",
                                N2TIDO = "E",
                                N2IMEU = customerRowsCounter == expires?.Count ? finalStepAmount : stepAmount,
                                n2rior = cliRow.N1RIGA,
                                n2tipi = pagcli?.pcltip
                            };
                            connection.Execute(pnClientiRepo.INSERT_QUERY, customer, transaction);
                        }
                    }
                    #endregion

                    #region PNPORTAFOGLIO
                    if (incassi != null && incassi.icssup == "R")
                    {
                        int portafRowCounter = 1;
                        int portafQuoteCounter = 1;
                        decimal stepAmountP = Math.Round(Invoice.GrandTotal / (expires?.Count ?? 1), 2);
                        decimal finalStepAmountP = Math.Round((stepAmountP + (Invoice.GrandTotal - (stepAmountP * (expires?.Count ?? 1)))), 2);
                        foreach (var exp in expires ?? new List<DateTime>())
                        {
                            PNPORTAFOGLIO portaf = new PNPORTAFOGLIO()
                            {
                                N6SOCI = head.N1SOCI,
                                N6ANNO = head.N1ANNO,
                                N6REGI = head.N1REGI,
                                N6RIGA = portafRowCounter++,
                                N6DARI = head.N1rifd,
                                N6RIFE = head.N1rifn,
                                N6DOCU = head.N1docn,
                                N6DADO = head.N1docd,
                                N6DARE = AccountingDate.Date,
                                N6CAUS = head.pncaus,
                                N6GRUP = pdcSotto.P1GRUP,
                                N6CONT = pdcSotto.P2CONT,
                                N6SOTT = Invoice.FTCODC,
                                N6SCAD = exp,
                                N6SEGN = customerRowSign,
                                N6RATA = portafQuoteCounter++,
                                N6CABI = Invoice.FTABIB.HasValue ? Invoice.FTABIB.Value : cliammi.CLABI,
                                N6CCAB = Invoice.FTCABB.HasValue ? Invoice.FTCABB.Value : cliammi.CLCAB,
                                N6COVA = "EUR",
                                N6IMEU = customerRowsCounter == expires?.Count ? finalStepAmountP : stepAmountP,
                                N6TIPODOC = string.Empty,
                                N6STATO = "N"
                            };
                            connection.Execute(pnPortafoglioRepo.INSERT_QUERY, portaf, transaction);
                        }
                    }
                    #endregion

                    #region Plafond
                    var plafondTotal = (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.HasPlafond).Sum(sum => sum.NetPrice);
                    if (plafondTotal > 0)
                    {
                        var plafondSettings = plafondParmsRepo.Get(Invoice.ftsoci);
                        if (plafondTotal > plafondSettings?.limit_amount)
                        {
                            var plafond = plafondRepo.GetLast(Invoice.ftsoci, Invoice.FTCODC ?? 0, Invoice.FTANNO, Invoice.FTDAOR ?? DateTime.MinValue, false);

                            if (plafond != null)
                            {
                                // update plafond counters
                                plafond.cliimpfattprovv -= plafondTotal;
                                plafond.cliimpfattprog += plafondTotal;
                                connection.Execute(plafondRepo.UPDATE_QUERY, plafond, transaction);

                                // add detail
                                var detail = new ACC_PLAFOND_ROWS()
                                {
                                    Cliasoc = Invoice.ftsoci,
                                    Cliacod = Invoice.FTCODC ?? 0,
                                    cliannosol = Invoice.FTANNO,
                                    cliprog = plafond.cliprog,
                                    clinumfat = Invoice.FTNUOR.ToString(),
                                    clidatfatt = Invoice.FTDAOR ?? DateTime.MinValue,
                                    cliimpplaf = plafondTotal,
                                    cliimpimpo = plafondTotal + (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDRIGA == 999999).FirstOrDefault()?.Amount
                                };
                                connection.Execute(plafondRowsRepo.INSERT_QUERY, detail, transaction);
                            }
                        }
                    }
                    #endregion


                    #region PNIVA
                    int ivaRowsCounter = 1;
                    List<string> rates = new List<string>();
                    var IVAs = new List<PNIVA>();
                    foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).OrderBy(o => o.FDASSF).ThenBy(o => o.FDALIV))
                    {
                        if (!rates.Contains(row.FDASSF + row.FDALIV?.Trim()))
                        {
                            rates.Add(row.FDASSF + row.FDALIV?.Trim());
                        }
                    }

                    if (!AccountingYear.eseivavenBool)
                    {
                        // NO IVA per cassa
                        foreach (var rate in rates)
                        {
                            var imponibile = (Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                                .Where(w => w.FDASSF == rate.Substring(0, 1) && w.FDALIV == rate.Substring(1, rate.Length - 1) &&
                                (w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)))
                                .Sum(sum => sum.NetPrice);
                            imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                            int rateValue = 0;
                            int.TryParse(rate.Substring(1, rate.Length - 1), out rateValue);
                            var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                            PNIVA pnivaRow = new PNIVA()
                            {
                                N4SOCI = Invoice.ftsoci,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = AccountingDate.Date,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = ivaSign,
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.FTCODC,
                                N4TCLF = "C",
                                N4ASSF = rate.Substring(0, 1),
                                n4assa = rate.Substring(1, rate.Length - 1),
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = AccountingDate.Date,
                                N4DTPGEF = null,
                                N4DTSCPG = AccountingDate.Date,
                                N4IMEU = imponibile,
                                N4IVEU = imposta,
                                N4IIEU = 0,
                                N4FLIVCA = "N",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                            IVAs.Add(pnivaRow);
                        }
                    }
                    else
                    {
                        // IVA per cassa, suddivido anche per scadenza
                        foreach (var rate in rates)
                        {
                            var imponibile = (Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                                .Where(w => w.FDASSF == rate.Substring(0, 1) && w.FDALIV == rate.Substring(1, rate.Length - 1) &&
                                (w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)))
                                .Sum(sum => sum.NetPrice);
                            int rateValue = 0;
                            int.TryParse(rate.Substring(1, rate.Length - 1), out rateValue);
                            var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                            decimal partialImponibileStepAmount = Math.Round(imponibile / (expires?.Count ?? 1), 2);
                            decimal partialImponibileFinalStepAmount = Math.Round((partialImponibileStepAmount + (imponibile - (partialImponibileStepAmount * (expires?.Count ?? 1)))), 2);
                            decimal partialImpostaStepAmount = Math.Round(imposta / (expires?.Count ?? 1), 2);
                            decimal partialImpostaFinalStepAmount = Math.Round((partialImpostaStepAmount + (imposta - (partialImpostaStepAmount * (expires?.Count ?? 1)))), 2);
                            foreach (var exp in expires ?? new List<DateTime>())
                            {
                                PNIVA pnivaRow = new PNIVA()
                                {
                                    N4SOCI = Invoice.ftsoci,
                                    N4ANNO = head.N1ANNO,
                                    N4REGI = accountingID,
                                    N4RIGA = ivaRowsCounter++,
                                    N4DOCU = head.N1docn,
                                    N4RIFE = head.N1rifn,
                                    N4DARE = AccountingDate.Date,
                                    N4DADO = head.N1docd,
                                    N4DARI = head.N1rifd,
                                    N4CAUS = head.pncaus,
                                    N4SEGN = ivaSign,
                                    N4LIBR = ivaBook?.livcod,
                                    N4SOTT = Invoice.FTCODC,
                                    N4TCLF = "C",
                                    N4ASSF = rate.Substring(0, 1),
                                    n4assa = rate.Substring(1, rate.Length - 1),
                                    N4TIDO = "E",
                                    n4donu = int.Parse(head.N1docn),
                                    N4DAST = null,
                                    n4tmppn = "N",
                                    N4DTSCAD = exp.AddMonths(12).AddDays(-1),
                                    N4DTPGEF = null,
                                    N4DTSCPG = exp,
                                    N4IMEU = ivaRowsCounter == expires?.Count ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                    N4IVEU = ivaRowsCounter == expires?.Count ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                    N4IIEU = 0,
                                    N4FLIVCA = "S",
                                    N4FLSPESO = string.Empty,
                                    N4IMPPAG = 0
                                };
                                connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                                IVAs.Add(pnivaRow);
                            }
                        }
                    }

                    // GIFT
                    if ((Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Any(any => any.FDTQTA == "O"))
                    {
                        foreach (var rat in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDTQTA == "O").GroupBy(g => new { g.FDASSF, g.FDALIV }))
                        {
                            var imponibile = (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDTQTA == "O" && w.FDASSF == rat.Key.FDASSF && w.FDALIV == rat.Key.FDALIV).Sum(sum => sum.NetPrice);
                            imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                            var dischargeRate = aliquotaRepo.Get(rat.Key.FDASSF ?? string.Empty, rat.Key.FDALIV ?? string.Empty);
                            PNIVA pnivaRowC = new PNIVA()
                            {
                                N4SOCI = Invoice.ftsoci,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = AccountingDate.Date,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = Invoice.FTTIPO == "N" ? "+" : "-",
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.FTCODC,
                                N4TCLF = "C",
                                N4ASSF = dischargeRate?.assomacod,
                                n4assa = dischargeRate?.assomaali,
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = AccountingDate.Date,
                                N4DTPGEF = null,
                                N4DTSCPG = AccountingDate.Date,
                                N4IMEU = imponibile,
                                N4IVEU = 0,
                                N4IIEU = 0,
                                N4FLIVCA = "N",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRowC, transaction);
                        }
                    }
                    #endregion

                    int selfAccountID = -1;
                    if (Invoice.FTTIPO == "A" || Invoice.FTTIPO == "C")
                    {
                        selfAccountID = numRegRepository.GetNumber(Invoice.ftsoci, AccountingYear.eseann, Constants.PN, true);
                        #region PNTESTATA
                        var headSelf = new PNTESTATA()
                        {
                            N1SOCI = Invoice.ftsoci,
                            N1ANNO = AccountingYear.eseann,
                            N1REGI = selfAccountID,
                            pncaus = caufatt?.fatcaut,
                            N1DARE = AccountingDate.Date,
                            N1docn = selfAccountID.ToString(),
                            N1docd = AccountingDate.Date,
                            N1rifn = Invoice.PrintFullID,
                            N1rifd = Invoice.FTDAOR,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1CLFO = Invoice.FTCODC,
                            N1FLCF = "C",
                            N1FL01 = string.Empty,
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headSelf, transaction);
                        #endregion
                        int selfRowsCounter = 1;
                        #region PNRIGHE
                        var newRow = new PNRIGHE()
                        {
                            N1SOCI = headSelf.N1SOCI,
                            N1ANNO = headSelf.N1ANNO,
                            N1REGI = headSelf.N1REGI,
                            N1RIGA = selfRowsCounter++,
                            N1DOCU = headSelf.N1docn,
                            N1DADO = headSelf.N1docd,
                            N1RIFE = fattaut?.FTAUNUMFAT,
                            N1DARI = fattaut?.FTAUDATFAT,
                            N1SEGN = Invoice.FTTIPO == "A" ? "D" : "A",
                            pngrup = pdcSottoSupplier?.P1GRUP,
                            pncont = pdcSottoSupplier?.P2CONT,
                            pnsott = pdcSottoSupplier?.P3SOTC,
                            N1FLCO = "F",
                            n1clie = fattaut?.FTAUCOF,
                            N1IMEU = IVAs.Sum(sum => sum.N4IVEU),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headSelf.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                        var newRow2 = new PNRIGHE()
                        {
                            N1SOCI = headSelf.N1SOCI,
                            N1ANNO = headSelf.N1ANNO,
                            N1REGI = headSelf.N1REGI,
                            N1RIGA = selfRowsCounter++,
                            N1DOCU = headSelf.N1docn,
                            N1DADO = headSelf.N1docd,
                            N1RIFE = fattaut?.FTAUNUMFAT,
                            N1DARI = fattaut?.FTAUDATFAT,
                            N1SEGN = Invoice.FTTIPO == "A" ? "D" : "A",
                            pngrup = Invoice.Rows?.First().FDGRUP,
                            pncont = Invoice.Rows?.First().FDCONT,
                            pnsott = Invoice.Rows?.First().FDSCTO,
                            N1IMEU = Invoice.Rows?.First().NetPrice,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headSelf.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow2, transaction);
                        var newRow3 = new PNRIGHE()
                        {
                            N1SOCI = headSelf.N1SOCI,
                            N1ANNO = headSelf.N1ANNO,
                            N1REGI = headSelf.N1REGI,
                            N1RIGA = selfRowsCounter++,
                            N1DOCU = headSelf.N1docn,
                            N1DADO = headSelf.N1docd,
                            N1RIFE = Invoice.PrintFullID,
                            N1DARI = Invoice.FTDAOR,
                            N1SEGN = Invoice.FTTIPO == "A" ? "A" : "D",
                            pngrup = pdcSotto.P1GRUP,
                            pncont = pdcSotto.P2CONT,
                            pnsott = pdcSotto.P3SOTC,
                            N1FLCO = "C",
                            n1clie = Invoice.FTCODC,
                            N1IMEU = Invoice.GrandTotalWithGift,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headSelf.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow3, transaction);
                        #endregion
                        #region PNFORNITORI
                        var supplier = new PNFORNITORI()
                        {
                            N3SOCI = headSelf.N1SOCI,
                            N3ANNO = headSelf.N1ANNO,
                            N3REGI = headSelf.N1REGI,
                            N3RIGA = 1,
                            N3DOCU = headSelf.N1docn,
                            N3DADO = headSelf.N1docd,
                            N3RIFE = fattaut?.FTAUNUMFAT,
                            N3DARI = fattaut?.FTAUDATFAT,
                            N3DARE = AccountingDate.Date,
                            N3CAUS = headSelf.pncaus,
                            N3GRUP = pdcSottoSupplier?.P1GRUP,
                            N3CONT = pdcSottoSupplier?.P2CONT,
                            N3SOTT = fattaut?.FTAUCOF,
                            N3SSOC = headSelf.N1SOCI,
                            N3SEGN = Invoice.FTTIPO == "A" ? "D" : "A",
                            N3PAGA = fornammi?.pfocod,
                            N3SCAD = AccountingDate.Date,
                            N3DIVI = "EUR",
                            n3vcod = "UIC",
                            N3DIDO = "EUR",
                            N3VADO = "UIC",
                            N3TIDO = "E",
                            N3IMEU = newRow.N1IMEU,
                            n3rior = 1,
                            n3tipp = "1"
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, supplier, transaction);
                        #endregion
                        #region PNCLIENTI
                        var customer = new PNCLIENTI()
                        {
                            N2SOCI = headSelf.N1SOCI,
                            N2ANNO = headSelf.N1ANNO,
                            N2REGI = headSelf.N1REGI,
                            N2RIGA = 1,
                            N2RIFE = Invoice.PrintFullID,
                            N2DARI = Invoice.FTDAOR,
                            N2DOCU = headSelf.N1docn,
                            N2DADO = headSelf.N1docd,
                            N2DARE = AccountingDate.Date,
                            N2CAUS = headSelf.pncaus,
                            N2GRUP = pdcSotto.P1GRUP,
                            N2CONT = pdcSotto.P2CONT,
                            N2SOTT = Invoice.FTCODC,
                            N2SSOC = headSelf.N1SOCI,
                            N2SEGN = Invoice.FTTIPO == "A" ? "A" : "D",
                            N2PAGA = Invoice.FTPAGA,
                            N2SCAD = AccountingDate.Date,
                            N2DIVI = "EUR",
                            n2vcod = "UIC",
                            N2DIDO = "EUR",
                            N2VADO = "UIC",
                            N2TIDO = "E",
                            N2IMEU = newRow3.N1IMEU,
                            n2rior = 3,
                            n2tipi = pagcli?.pcltip
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, customer, transaction);
                        #endregion
                    }

                    // flag invoice worked
                    Invoice.FTFLA2 = "1";
                    connection.Execute(UPDATE_QUERY, Invoice, transaction);

                    transaction.Commit();
                    InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID}{(Invoice.FTTIPO == "A" ? $" e n. {selfAccountID}" : null)}");
                    return true;
                }

                return false;
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
    public bool AccountingReceivedInvoice(string CompanyID, ACC_EINVOICE_HEADS Invoice, ESERCIZIO AccountingYear, DateTime RegDate, DateTime ProtDate, CAUCONT Causal, List<PNRIGHE> Counterparts, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var pnIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>();
                var einvoHead = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>();

                // get registration number
                var accountingID = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);
                // Supplier info
                var fornammi = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, Invoice.fattfor ?? 0);
                if (fornammi == null)
                {
                    ErrorHandler.Show($"Non trovati i dati amministrativi del fornitore");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(fornammi.foGRUP) || string.IsNullOrWhiteSpace(fornammi.foCONT))
                {
                    ErrorHandler.Show($"Manca il conto contabile del fornitore");
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(fornammi.foGRUP, fornammi.foCONT, "F", CompanyID);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show($"Non trovato il sottoconto fornitore nel gruppo {fornammi.foGRUP} e conto {fornammi.foCONT}");
                    return false;
                }
                // IVA book
                var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(Causal.cauliv ?? string.Empty);
                // CAUCONT_GROUPS
                var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(CompanyID, Causal.caucod);
                // PAGCLI/TAB_ACC_TIPINC
                var pagfor = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(fornammi.pfocod ?? string.Empty);
                if (pagfor == null)
                {
                    ErrorHandler.Show($"Non trovato il codice pagamento sul fornitore");
                    return false;
                }
                var payment = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().Get(pagfor.pfotip ?? string.Empty);
                if (payment == null)
                {
                    ErrorHandler.Show($"Non trovato il tipo pagamento sul codice pagamento {pagfor.FullDescriptionSearchable}");
                    return false;
                }
                // document
                var lastProtocolInfo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, RegDate.Year, new GenericIDDescription() { ID = Causal.cauliv ?? string.Empty, Description = string.Empty }, true);
                // fix total invoice
                decimal totalInvoice = Math.Round((Invoice.fatttot ?? 0), 2, MidpointRounding.AwayFromZero);
                if (totalInvoice < 0)
                    totalInvoice *= -1;
                decimal totalInvoiceImposta = Math.Round((Invoice.fattimposta ?? 0), 2, MidpointRounding.AwayFromZero);
                if (totalInvoiceImposta < 0)
                    totalInvoiceImposta *= -1;
                // invoice vs credit parms
                string? supplierRowSign = null;
                string? otherRowsSign = null;
                string? ivaSign = null;
                if (Causal.causeg == "+")
                {
                    supplierRowSign = "A";
                    otherRowsSign = "D";
                    ivaSign = "+";
                }
                else
                {
                    supplierRowSign = "D";
                    otherRowsSign = "A";
                    ivaSign = "-";
                }

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = CompanyID,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = accountingID,
                    pncaus = Causal.caucod,
                    N1DARE = RegDate,
                    N1docn = lastProtocolInfo.ToString(),
                    N1docd = ProtDate,
                    N1rifn = Invoice.fattnum,
                    N1rifd = Invoice.fattdata,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Invoice.fattfor,
                    N1FLCF = "F",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                #region Supplier row
                var supplierRows = new List<PNRIGHE>();
                PNRIGHE supplierRow = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = rowsCounter++,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1rifn,
                    N1DARI = head.N1rifd,
                    n1clie = Invoice.fattfor,
                    N1SEGN = supplierRowSign,
                    pngrup = pdcSotto.P1GRUP,
                    pncont = pdcSotto.P2CONT,
                    pnsott = pdcSotto.P3SOTC,
                    N1IMEU = totalInvoice + (Counterparts.Where(w => w.NotPair).Sum(sum => sum.N1IMEU) ?? 0),
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "N",
                    n1paga = fornammi.pfocod,
                    N1DRri = head.N1docd,
                    N1DESC = Invoice.Supplier?.FullDescriptionSearchable
                };
                connection.Execute(pnRigheRepository.INSERT_QUERY, supplierRow, transaction);
                supplierRows.Add(supplierRow);
                #endregion

                #region IVA row
                if (Invoice.fattimposta > 0)
                {
                    foreach (var cg in grpcau ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        PNRIGHE ivaRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = cg.grpseg,
                            pngrup = cg.grpgrp,
                            pncont = cg.grpcto,
                            pnsott = cg.grpsct,
                            N1IMEU = grpcau?.Count == 1 ? totalInvoiceImposta : 0,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1DESC = Invoice.Supplier?.FullDescriptionSearchable,
                            N1DRri = head.N1docd,
                            N1tmpPNR = "N"
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, ivaRow, transaction);
                    }
                }
                #endregion

                // compute expires count
                var expiresCount = Invoice.Expires?.Count > 0 ? Invoice.Expires.Count : 1;

                #region PNIVA
                int ivaRowsCounter = 1;
                bool hasIndetraibile = false;
                decimal indetraibileTotal = 0;
                if (!AccountingYear.eseivavenBool)
                {
                    // NO IVA per cassa
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        // fix amounts
                        var imponibileFix = Math.Round((rate.fattimpodett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (imponibileFix < 0)
                            imponibileFix *= -1;
                        var impostaFix = Math.Round((rate.fattimpostadett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (impostaFix < 0)
                            impostaFix *= -1;
                        if (impostaFix == 0)
                        {
                            decimal rateVal = 0;
                            decimal.TryParse(rate.RateValue, out rateVal);
                            impostaFix = Math.Round(imponibileFix * rateVal / 100, 2, MidpointRounding.AwayFromZero);
                        }
                        decimal indetraibileAmount = 0;
                        if (rate.SelectedRate != null && rate.SelectedRate.asspin.HasValue && rate.SelectedRate.asspin.Value > 0)
                        {
                            indetraibileAmount = Math.Round((rate.fattimpostadett ?? 0) * rate.SelectedRate.asspin.Value / 100, 2, MidpointRounding.AwayFromZero);
                            indetraibileTotal += indetraibileAmount;
                            hasIndetraibile = true;
                        }
                        PNIVA pnivaRow = new PNIVA()
                        {
                            N4SOCI = CompanyID,
                            N4ANNO = head.N1ANNO,
                            N4REGI = accountingID,
                            N4RIGA = ivaRowsCounter++,
                            N4DOCU = head.N1docn,
                            N4RIFE = head.N1rifn,
                            N4DARE = head.N1DARE,
                            N4DADO = head.N1docd,
                            N4DARI = head.N1rifd,
                            N4CAUS = head.pncaus,
                            N4SEGN = ivaSign,
                            N4LIBR = ivaBook?.livcod,
                            N4SOTT = Invoice.fattfor,
                            N4TCLF = "F",
                            N4ASSF = rate.SelectedRate?.asscod,
                            n4assa = rate.SelectedRate?.assali,
                            N4TIDO = "E",
                            n4donu = int.Parse(head.N1docn),
                            N4DAST = null,
                            n4tmppn = "N",
                            N4DTPGEF = null,
                            N4IMEU = imponibileFix,
                            N4IVEU = impostaFix,
                            N4IIEU = indetraibileAmount,
                            N4FLIVCA = "N",
                            N4FLSPESO = string.Empty,
                            N4IMPPAG = 0
                        };
                        connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                    }
                }
                else
                {
                    // IVA per cassa, suddivido anche per scadenza
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        // fix amounts
                        var imponibileFix = Math.Round((rate.fattimpodett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (imponibileFix < 0)
                            imponibileFix *= -1;
                        var impostaFix = Math.Round((rate.fattimpostadett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (impostaFix < 0)
                            impostaFix *= -1;
                        decimal partialImponibileStepAmount = Math.Round(imponibileFix / expiresCount, 2);
                        decimal partialImponibileFinalStepAmount = Math.Round((partialImponibileStepAmount + (imponibileFix - (partialImponibileStepAmount * expiresCount))), 2);
                        decimal partialImpostaStepAmount = Math.Round(impostaFix / expiresCount, 2);
                        decimal partialImpostaFinalStepAmount = Math.Round((partialImpostaStepAmount + (impostaFix - (partialImpostaStepAmount * expiresCount))), 2);
                        decimal partialImpostaIndetraibileStepAmount = 0;
                        decimal partialImpostaIndetraibileFinalStepAmount = 0;
                        if (rate.SelectedRate != null && rate.SelectedRate.asspin.HasValue && rate.SelectedRate.asspin.Value > 0)
                        {
                            var indetraibileAmount = Math.Round((rate.fattimpostadett ?? 0) * rate.SelectedRate.asspin.Value / 100, 2, MidpointRounding.AwayFromZero);
                            indetraibileTotal += indetraibileAmount;
                            partialImpostaIndetraibileStepAmount = Math.Round(indetraibileAmount / expiresCount, 2);
                            partialImpostaIndetraibileFinalStepAmount = Math.Round((partialImpostaIndetraibileStepAmount + (indetraibileAmount - (partialImpostaIndetraibileStepAmount * expiresCount))), 2);
                        }

                        if (Invoice.Expires != null && Invoice.Expires.Count > 0)
                        {
                            foreach (var exp in Invoice.Expires)
                            {
                                PNIVA pnivaRow = new PNIVA()
                                {
                                    N4SOCI = CompanyID,
                                    N4ANNO = head.N1ANNO,
                                    N4REGI = accountingID,
                                    N4RIGA = ivaRowsCounter++,
                                    N4DOCU = head.N1docn,
                                    N4RIFE = head.N1rifn,
                                    N4DARE = head.N1DARE,
                                    N4DADO = head.N1docd,
                                    N4DARI = head.N1rifd,
                                    N4CAUS = head.pncaus,
                                    N4SEGN = ivaSign,
                                    N4LIBR = ivaBook?.livcod,
                                    N4SOTT = Invoice.fattfor,
                                    N4TCLF = "F",
                                    N4ASSF = rate.SelectedRate?.asscod,
                                    n4assa = rate.SelectedRate?.assali,
                                    N4TIDO = "E",
                                    n4donu = int.Parse(head.N1docn),
                                    N4DAST = null,
                                    n4tmppn = "N",
                                    N4DTSCAD = exp.fattdatascad.Date.AddMonths(12).AddDays(-1),
                                    N4DTPGEF = null,
                                    N4DTSCPG = exp.fattdatascad.Date,
                                    N4IMEU = ivaRowsCounter == expiresCount ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                    N4IVEU = ivaRowsCounter == expiresCount ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                    N4IIEU = ivaRowsCounter == expiresCount ? partialImpostaIndetraibileFinalStepAmount : partialImpostaIndetraibileStepAmount,
                                    N4FLIVCA = "S",
                                    N4FLSPESO = string.Empty,
                                    N4IMPPAG = 0
                                };
                                connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                            }
                        }
                        else
                        {
                            // no scadenze
                            PNIVA pnivaRow = new PNIVA()
                            {
                                N4SOCI = CompanyID,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = head.N1DARE,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = ivaSign,
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.fattfor,
                                N4TCLF = "F",
                                N4ASSF = rate.SelectedRate?.asscod,
                                n4assa = rate.SelectedRate?.assali,
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = head.N1DARE.Value.AddMonths(12).AddDays(-1),
                                N4DTPGEF = null,
                                N4DTSCPG = head.N1DARE.Value,
                                N4IMEU = ivaRowsCounter == expiresCount ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                N4IVEU = ivaRowsCounter == expiresCount ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                N4IIEU = 0,
                                N4FLIVCA = "S",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                        }
                    }
                }
                #endregion

                #region Costi rows
                foreach (var cop in Counterparts)
                {
                    // fix amount
                    cop.N1IMEU = Math.Round((cop.N1IMEU ?? 0), 2, MidpointRounding.AwayFromZero);
                    if (cop.N1IMEU < 0)
                        cop.N1IMEU *= -1;
                    cop.N1RIGA = rowsCounter++;
                    cop.N1SOCI = head.N1SOCI;
                    cop.N1ANNO = head.N1ANNO;
                    cop.N1REGI = head.N1REGI;
                    cop.N1DOCU = head.N1docn;
                    cop.N1DADO = head.N1docd;
                    cop.N1RIFE = head.N1rifn;
                    cop.N1DARI = head.N1rifd;
                    cop.N1SEGN = otherRowsSign;
                    cop.N1CHIU = "A";
                    cop.N1TIDO = "E";
                    cop.N1DIVI = "EUR";
                    cop.N1tmpPNR = "N";
                    cop.N1DESC = Invoice.Supplier?.FullDescriptionSearchable;
                    cop.N1DRri = head.N1docd;
                    connection.Execute(pnRigheRepository.INSERT_QUERY, cop, transaction);
                }
                #endregion

                #region PNFORNITORI
                int supplierRowsCounter = 1;
                foreach (var supRow in supplierRows)
                {
                    decimal stepAmount = Math.Round((supRow.N1IMEU ?? 0) / expiresCount, 2);
                    decimal finalStepAmount = Math.Round((stepAmount + ((supRow.N1IMEU ?? 0) - (stepAmount * expiresCount))), 2);
                    if (Invoice.Expires != null && Invoice.Expires.Count > 0)
                    {
                        foreach (var exp in Invoice.Expires)
                        {
                            var supplier = new PNFORNITORI()
                            {
                                N3SOCI = head.N1SOCI,
                                N3ANNO = head.N1ANNO,
                                N3REGI = head.N1REGI,
                                N3RIGA = supplierRowsCounter++,
                                N3DARI = head.N1rifd,
                                N3RIFE = head.N1rifn,
                                N3DOCU = head.N1docn,
                                N3DADO = head.N1docd,
                                N3DARE = head.N1DARE,
                                N3CAUS = head.pncaus,
                                N3GRUP = pdcSotto.P1GRUP,
                                N3CONT = pdcSotto.P2CONT,
                                N3SOTT = Invoice.fattfor,
                                N3SSOC = head.N1SOCI,
                                N3SEGN = supRow.N1SEGN,
                                N3PAGA = pagfor?.pfocod,
                                N3SCAD = (exp.fattdatascad == DateTime.MinValue) ? null : exp.fattdatascad.Date,
                                N3DIVI = "EUR",
                                n3vcod = "UIC",
                                N3DIDO = "EUR",
                                N3VADO = "UIC",
                                N3TIDO = "E",
                                N3IMEU = supplierRowsCounter == expiresCount ? finalStepAmount : stepAmount,
                                n3rior = supRow.N1RIGA,
                                n3tipp = pagfor?.pfotip
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, supplier, transaction);
                        }
                    }
                    else
                    {
                        // nessuna scadenza
                        var supplier = new PNFORNITORI()
                        {
                            N3SOCI = head.N1SOCI,
                            N3ANNO = head.N1ANNO,
                            N3REGI = head.N1REGI,
                            N3RIGA = supplierRowsCounter++,
                            N3DARI = head.N1rifd,
                            N3RIFE = head.N1rifn,
                            N3DOCU = head.N1docn,
                            N3DADO = head.N1docd,
                            N3DARE = head.N1DARE,
                            N3CAUS = head.pncaus,
                            N3GRUP = pdcSotto.P1GRUP,
                            N3CONT = pdcSotto.P2CONT,
                            N3SOTT = Invoice.fattfor,
                            N3SSOC = head.N1SOCI,
                            N3SEGN = supRow.N1SEGN,
                            N3PAGA = pagfor?.pfocod,
                            N3SCAD = head.N1DARE,
                            N3DIVI = "EUR",
                            n3vcod = "UIC",
                            N3DIDO = "EUR",
                            N3VADO = "UIC",
                            N3TIDO = "E",
                            N3IMEU = supplierRowsCounter == expiresCount ? finalStepAmount : stepAmount,
                            n3rior = supRow.N1RIGA,
                            n3tipp = pagfor?.pfotip
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, supplier, transaction);
                    }
                }
                #endregion

                #region Giroconto IVA indetraibile
                if (hasIndetraibile)
                {
                    // get registration number
                    var girocontoIndetraibileRegID = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);

                    #region PNTESTATA
                    PNTESTATA girocontoHead = new PNTESTATA()
                    {
                        N1SOCI = CompanyID,
                        N1ANNO = AccountingYear.eseann,
                        N1REGI = girocontoIndetraibileRegID,
                        pncaus = ivaBook?.livcii,
                        N1DARE = RegDate,
                        N1docn = head.N1docn,
                        N1docd = head.N1docd,
                        N1rifn = head.N1rifn,
                        N1rifd = head.N1rifd,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1FL01 = string.Empty,
                        N1TmpPN = "N",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(pnTestataRepository.INSERT_QUERY, girocontoHead, transaction);
                    #endregion

                    PNRIGHE firstRow = new PNRIGHE()
                    {
                        N1SOCI = girocontoHead.N1SOCI,
                        N1ANNO = girocontoHead.N1ANNO,
                        N1REGI = girocontoHead.N1REGI,
                        N1RIGA = 1,
                        N1DOCU = girocontoHead.N1docn,
                        N1DADO = girocontoHead.N1docd,
                        N1RIFE = girocontoHead.N1rifn,
                        N1DARI = girocontoHead.N1rifd,
                        N1SEGN = ivaSign == "+" ? "A" : "D",
                        pngrup = ivaBook?.livgci,
                        pncont = ivaBook?.livcci,
                        pnsott = ivaBook?.livsci,
                        N1IMEU = indetraibileTotal,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1DESC = Invoice.Supplier?.FullDescriptionSearchable,
                        N1DRri = girocontoHead.N1docd,
                        N1tmpPNR = "N"
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, firstRow, transaction);
                    PNRIGHE secondRow = new PNRIGHE()
                    {
                        N1SOCI = girocontoHead.N1SOCI,
                        N1ANNO = girocontoHead.N1ANNO,
                        N1REGI = girocontoHead.N1REGI,
                        N1RIGA = 2,
                        N1DOCU = girocontoHead.N1docn,
                        N1DADO = girocontoHead.N1docd,
                        N1RIFE = girocontoHead.N1rifn,
                        N1DARI = girocontoHead.N1rifd,
                        N1SEGN = ivaSign == "+" ? "D" : "A",
                        pngrup = Counterparts.First().pngrup,
                        pncont = Counterparts.First().pncont,
                        pnsott = Counterparts.First().pnsott,
                        N1IMEU = indetraibileTotal,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1DESC = Invoice.Supplier?.FullDescriptionSearchable,
                        N1DRri = girocontoHead.N1docd,
                        N1tmpPNR = "N"
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, secondRow, transaction);
                }
                #endregion

                // flag invoice accounted and add year/number reg
                Invoice.fattannoreg = head.N1ANNO;
                Invoice.fattnumreg = head.N1REGI;
                Invoice.accounted = now;
                Invoice.accounted_UserID = UserID;
                connection.Execute(einvoHead.UPDATE_QUERY, Invoice, transaction);

                transaction.Commit();
                InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID} protocollo {head.N1docn}");
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
    public bool AccountingSentExternalInvoice(string CompanyID, ACC_EINVOICE_HEADS Invoice, ESERCIZIO AccountingYear, DateTime RegDate, DateTime ProtDate, CAUCONT Causal, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var pnPortafoglioRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                var accPlafondRepository = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var accPlafondRowsRepository = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var accPlafondParmsRepository = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var tipiIncRepo = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>();

                // get registration number
                var accountingID = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);
                // Customer info
                var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, Invoice.fattcli ?? 0);
                if (cliammi == null)
                {
                    ErrorHandler.Show($"Non trovati i dati amministrativi del cliente");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(cliammi.clGRUP) || string.IsNullOrWhiteSpace(cliammi.clcont))
                {
                    ErrorHandler.Show($"Manca il conto contabile del cliente");
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(cliammi.clGRUP, cliammi.clcont, "C", CompanyID);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show($"Non trovato il sottoconto cliente nel gruppo {cliammi.clGRUP} e conto {cliammi.clcont}");
                    return false;
                }
                // IVA book
                var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(Causal.cauliv ?? string.Empty);
                // IVA book numerator
                var customProtocol = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, new GenericIDDescription() { ID = ivaBook?.livcod, Description = ivaBook?.livdes }, true);
                // CAUCONT_GROUPS
                var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(CompanyID, Causal.caucod);
                // PAGCLI/TAB_ACC_TIPINC
                var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(cliammi.pclcod ?? string.Empty);
                if (pagcli == null)
                {
                    ErrorHandler.Show($"Non trovato il codice pagamento sul cliente");
                    return false;
                }
                var payment = tipiIncRepo.Get(pagcli.pcltip ?? string.Empty);
                if (payment == null)
                {
                    ErrorHandler.Show($"Non trovato il tipo pagamento sul codice pagamento {pagcli.FullDescriptionSearchable}");
                    return false;
                }
                // invoice vs credit parms
                string? customerRowSign = null;
                string? otherRowsSign = null;
                string? ivaSign = null;
                if (Causal.causeg == "+")
                {
                    customerRowSign = "D";
                    otherRowsSign = "A";
                    ivaSign = "+";
                }
                else
                {
                    customerRowSign = "A";
                    otherRowsSign = "D";
                    ivaSign = "-";
                }

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = CompanyID,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = accountingID,
                    pncaus = Causal.caucod,
                    N1DARE = RegDate,
                    N1docn = customProtocol.ToString(),
                    N1docd = ProtDate,
                    N1rifn = Invoice.fattnum,
                    N1rifd = ProtDate,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Invoice.fattcli,
                    N1FLCF = "C",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                if (!Causal.causolBool)
                {
                    #region Customer row
                    var customerRows = new List<PNRIGHE>();
                    PNRIGHE customerRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = head.N1rifn,
                        N1DARI = head.N1rifd,
                        n1clie = Invoice.fattcli,
                        N1SEGN = customerRowSign,
                        pngrup = pdcSotto.P1GRUP,
                        pncont = pdcSotto.P2CONT,
                        pnsott = pdcSotto.P3SOTC,
                        N1IMEU = Invoice.fatttot,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = cliammi.pclcod,
                        N1DRri = head.N1docd
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    customerRows.Add(customerRow);
                    #endregion

                    #region IVA row
                    if (Invoice.fattimposta > 0)
                    {
                        foreach (var cg in grpcau ?? new ObservableCollection<CAUCONT_GROUPS>())
                        {
                            PNRIGHE ivaRow = new PNRIGHE()
                            {
                                N1SOCI = head.N1SOCI,
                                N1ANNO = head.N1ANNO,
                                N1REGI = head.N1REGI,
                                N1RIGA = rowsCounter++,
                                N1DOCU = head.N1docn,
                                N1DADO = head.N1docd,
                                N1RIFE = head.N1rifn,
                                N1DARI = head.N1rifd,
                                N1SEGN = cg.grpseg,
                                pngrup = cg.grpgrp,
                                pncont = cg.grpcto,
                                pnsott = cg.grpsct,
                                N1IMEU = grpcau?.Count == 1 ? Invoice.fattimposta : 0,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DRri = head.N1docd,
                                N1tmpPNR = "N"
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, ivaRow, transaction);
                        }
                    }
                    #endregion

                    #region Ricavi rows
                    foreach (var cop in (Invoice.Rows ?? new ObservableCollection<ACC_EINVOICE_ROWS>()).GroupBy(g => new { g.Product?.GroupID, g.Product?.AccountID, g.Product?.SubaccountID, g.Product?.costcenter_id }))
                    {
                        var pnCop = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = otherRowsSign,
                            pngrup = cop.Key.GroupID,
                            pncont = cop.Key.AccountID,
                            pnsott = cop.Key.SubaccountID,
                            N1CCCC = cop.Key.costcenter_id,
                            N1IMEU = Math.Round((Invoice.Rows ?? new ObservableCollection<ACC_EINVOICE_ROWS>()).Where(w => w.Product?.GroupID == cop.Key.GroupID && w.Product?.AccountID == cop.Key.AccountID && w.Product?.SubaccountID == cop.Key.SubaccountID && w.Product?.costcenter_id == cop.Key.costcenter_id).Sum(sum => sum.fatttotriga) ?? 0, 2, MidpointRounding.AwayFromZero),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, pnCop, transaction);
                    }
                    #endregion

                    #region PNCLIENTI
                    int customerRowsCounter = 1;
                    foreach (var supRow in customerRows)
                    {
                        decimal stepAmount = Math.Round((supRow.N1IMEU ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal finalStepAmount = Math.Round((stepAmount + ((supRow.N1IMEU ?? 0) - (stepAmount * (Invoice.Expires?.Count ?? 1)))), 2);
                        foreach (var exp in Invoice.Expires ?? new ObservableCollection<ACC_EINVOICE_EXPIRES>())
                        {
                            var custom = new PNCLIENTI()
                            {
                                N2SOCI = head.N1SOCI,
                                N2ANNO = head.N1ANNO,
                                N2REGI = head.N1REGI,
                                N2RIGA = customerRowsCounter++,
                                N2DARI = head.N1rifd,
                                N2RIFE = head.N1rifn,
                                N2DOCU = head.N1docn,
                                N2DADO = head.N1docd,
                                N2DARE = head.N1DARE,
                                N2CAUS = head.pncaus,
                                N2GRUP = pdcSotto.P1GRUP,
                                N2CONT = pdcSotto.P2CONT,
                                N2SOTT = Invoice.fattcli,
                                N2SSOC = head.N1SOCI,
                                N2SEGN = supRow.N1SEGN,
                                N2PAGA = pagcli?.pclcod,
                                N2SCAD = exp.fattdatascad.Date,
                                N2DIVI = "EUR",
                                n2vcod = "UIC",
                                N2DIDO = "EUR",
                                N2VADO = "UIC",
                                N2TIDO = "E",
                                N2IMEU = customerRowsCounter == Invoice.Expires?.Count ? finalStepAmount : stepAmount,
                                n2rior = supRow.N1RIGA,
                                n2tipi = pagcli?.pcltip
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, custom, transaction);
                        }
                    }
                    #endregion

                    #region PNPORTAFOGLIO
                    if (payment != null && payment.icssup == "R")
                    {
                        int portafRowCounter = 1;
                        int portafQuoteCounter = 1;
                        decimal stepAmountP = Math.Round((Invoice.fattimpo ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal finalStepAmountP = Math.Round((stepAmountP + ((Invoice.fattimpo ?? 0) - (stepAmountP * (Invoice.Expires?.Count ?? 1)))), 2);
                        foreach (var exp in Invoice.Expires ?? new ObservableCollection<ACC_EINVOICE_EXPIRES>())
                        {
                            PNPORTAFOGLIO portaf = new PNPORTAFOGLIO()
                            {
                                N6SOCI = head.N1SOCI,
                                N6ANNO = head.N1ANNO,
                                N6REGI = head.N1REGI,
                                N6RIGA = portafRowCounter++,
                                N6DARI = head.N1rifd,
                                N6RIFE = head.N1rifn,
                                N6DOCU = head.N1docn,
                                N6DADO = head.N1docd,
                                N6DARE = head.N1DARE,
                                N6CAUS = head.pncaus,
                                N6GRUP = pdcSotto.P1GRUP,
                                N6CONT = pdcSotto.P2CONT,
                                N6SOTT = Invoice.fattcli,
                                N6SCAD = exp.fattdatascad.Date,
                                N6SEGN = customerRowSign,
                                N6RATA = portafQuoteCounter++,
                                N6CABI = !string.IsNullOrWhiteSpace(exp.fattabi) ? int.Parse(exp.fattabi) : cliammi.CLABI,
                                N6CCAB = !string.IsNullOrWhiteSpace(exp.fattcab) ? int.Parse(exp.fattcab) : cliammi.CLCAB,
                                N6COVA = "EUR",
                                N6IMEU = customerRowsCounter == Invoice.Expires?.Count ? finalStepAmountP : stepAmountP,
                                N6TIPODOC = string.Empty,
                                N6STATO = "N"
                            };
                            connection.Execute(pnPortafoglioRepo.INSERT_QUERY, portaf, transaction);
                        }
                    }
                    #endregion

                    #region Plafond
                    var plafondTotal = Invoice.Rows?.Where(w => w.Rate?.assplaBool ?? false).Sum(sum => sum.fatttotriga);
                    if (plafondTotal > 0)
                    {
                        var plafondSettings = accPlafondParmsRepository.Get(Invoice.fattsoc);
                        if (plafondTotal > plafondSettings?.limit_amount)
                        {
                            var plafond = accPlafondRepository.GetLast(Invoice.fattsoc, Invoice.fattcli ?? 0, AccountingYear.eseann, Invoice.fattdata, false);

                            if (plafond != null)
                            {
                                // update plafond counters
                                plafond.cliimpfattprovv -= plafondTotal;
                                plafond.cliimpfattprog += plafondTotal;
                                connection.Execute(accPlafondRepository.UPDATE_QUERY, plafond, transaction);

                                // add detail
                                var detail = new ACC_PLAFOND_ROWS()
                                {
                                    Cliasoc = Invoice.fattsoc,
                                    Cliacod = Invoice.fattcli ?? 0,
                                    cliannosol = AccountingYear.eseann,
                                    cliprog = plafond.cliprog,
                                    clinumfat = Invoice.fattnum,
                                    clidatfatt = Invoice.fattdata,
                                    cliimpplaf = plafondTotal,
                                    cliimpimpo = plafondTotal + Invoice.fattimpobol
                                };
                                connection.Execute(accPlafondRowsRepository.INSERT_QUERY, detail, transaction);
                            }
                        }
                    }
                    #endregion
                }

                #region PNIVA
                int ivaRowsCounter = 1;

                if (!AccountingYear.eseivavenBool)
                {
                    // NO IVA per cassa
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        var rateItem = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetFirstAliquota(rate.Fattaliq ?? string.Empty);
                        PNIVA pnivaRow = new PNIVA()
                        {
                            N4SOCI = CompanyID,
                            N4ANNO = head.N1ANNO,
                            N4REGI = accountingID,
                            N4RIGA = ivaRowsCounter++,
                            N4DOCU = head.N1docn,
                            N4RIFE = head.N1rifn,
                            N4DARE = head.N1DARE,
                            N4DADO = head.N1docd,
                            N4DARI = head.N1rifd,
                            N4CAUS = head.pncaus,
                            N4SEGN = ivaSign,
                            N4LIBR = ivaBook?.livcod,
                            N4SOTT = Invoice.fattcli,
                            N4TCLF = "F",
                            N4ASSF = rateItem?.asscod,
                            n4assa = rateItem?.assali,
                            N4TIDO = "E",
                            n4donu = 0,
                            N4DAST = null,
                            n4tmppn = "N",
                            N4DTPGEF = null,
                            N4IMEU = rate.fattimpodett,
                            N4IVEU = rate.fattimpostadett,
                            N4IIEU = 0,
                            N4FLIVCA = "N",
                            N4FLSPESO = string.Empty,
                            N4IMPPAG = 0
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, pnivaRow, transaction);
                    }
                }
                else
                {
                    // IVA per cassa, suddivido anche per scadenza
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        decimal partialImponibileStepAmount = Math.Round((rate.fattimpodett ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal partialImponibileFinalStepAmount = Math.Round((partialImponibileStepAmount + ((rate.fattimpodett ?? 0) - (partialImponibileStepAmount * (Invoice.Expires?.Count ?? 1)))), 2);
                        decimal partialImpostaStepAmount = Math.Round((rate.fattimpostadett ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal partialImpostaFinalStepAmount = Math.Round((partialImpostaStepAmount + ((rate.fattimpostadett ?? 0) - (partialImpostaStepAmount * (Invoice.Expires?.Count ?? 1)))), 2);
                        foreach (var exp in Invoice.Expires ?? new ObservableCollection<ACC_EINVOICE_EXPIRES>())
                        {
                            var rateItem = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetFirstAliquota(rate.Fattaliq ?? string.Empty);
                            PNIVA pnivaRow = new PNIVA()
                            {
                                N4SOCI = CompanyID,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = head.N1DARE,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = ivaSign,
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.fattcli,
                                N4TCLF = "F",
                                N4ASSF = rateItem?.asscod,
                                n4assa = rateItem?.assali,
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = exp.fattdatascad.Date.AddMonths(12).AddDays(-1),
                                N4DTPGEF = null,
                                N4DTSCPG = exp.fattdatascad.Date,
                                N4IMEU = ivaRowsCounter == Invoice.Expires?.Count ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                N4IVEU = ivaRowsCounter == Invoice.Expires?.Count ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                N4IIEU = 0,
                                N4FLIVCA = "S",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, pnivaRow, transaction);
                        }
                    }
                }
                #endregion

                // flag invoice accounted and add year/number reg
                Invoice.fattannoreg = head.N1ANNO;
                Invoice.fattnumreg = head.N1REGI;
                Invoice.accounted = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                Invoice.accounted_UserID = UserID;
                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().UPDATE_QUERY, Invoice, transaction);

                transaction.Commit();
                InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID}");
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

    #region CRM
    public bool GenerateByOrder(ORDIT00F OrderHead, List<ORDID00F> SelectedRows, string UserID, string HeadNotes, string FootNotes, string DocumentType)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {

                var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var plafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var plafondRowsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var plafondParmsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var caufatRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>();
                var fattdRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var cliammiRepo = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                int sequence = 1;
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var newInvoiceID = numRegRepository.GetNumber(OrderHead.otsoci, now.Year, Constants.INVOICE_TEMP, true);
                var invoiceCausal = caufatRepo.Get(OrderHead.Causal?.caufat ?? string.Empty);

                // generate invoice head
                var newHead = new FATTT00F()
                {
                    ftsoci = OrderHead.otsoci,
                    FTANNO = now.Year,
                    FTNUOR = newInvoiceID,
                    FTTIPO = DocumentType,
                    FTDAOR = now.Date,
                    FTCAUS = OrderHead.Causal?.caufat,
                    FTCODC = OrderHead.OTCLIE,
                    FTCODD = OrderHead.DESTIN,
                    FTPAGA = OrderHead.OTPAGA,
                    FTABIB = OrderHead.abiabi,
                    FTCABB = OrderHead.abicab,
                    FTCONS = OrderHead.OTCONS,
                    FTSPED = OrderHead.OTSPED,
                    FTCORR = OrderHead.OTCORR,
                    FTCORR2 = OrderHead.OTCORR2,
                    FTCIDI = "UIC",
                    ftciva = OrderHead.otdivi,
                    FTIMBL = OrderHead.OTIMBA,
                    ftling = OrderHead.otling,
                    addedUserID = UserID,
                    FTNOTET = HeadNotes,
                    FTNOTEP = FootNotes,
                    FTSHOWT = !string.IsNullOrWhiteSpace(HeadNotes),
                    FTSHOWP = !string.IsNullOrWhiteSpace(FootNotes),
                    FTDE25 = OrderHead.OTDE25,
                    FTBCON = OrderHead.OTBCON,
                    FTSCCL = OrderHead.OTSCCL,
                    FTAREA = OrderHead.OTAREA,
                    FTRIVE = OrderHead.OTRIVE,
                    FTCONZ = OrderHead.OTCONZ,
                    FTFILI = OrderHead.OTFILI,
                    FTZONA = OrderHead.OTZONA,
                    FTSETM = OrderHead.OTSETM,
                    FTREGI = OrderHead.OTREGI,
                    fttdoc = invoiceCausal?.fattido
                };
                connection.Execute(INSERT_QUERY, newHead, transaction);
                // generate rows and update origins
                decimal plafondTotal = 0;
                foreach (var row in SelectedRows)
                {
                    var rate = aliquotaRepo.Get(row.ODASSF ?? string.Empty, row.ODALIV ?? string.Empty);
                    if (rate?.assplaBool ?? false)
                        plafondTotal += row.Amount;
                    var newRow = new FATTD00F()
                    {
                        ftsoci = OrderHead.otsoci,
                        FTANNO = now.Year,
                        FTNUOR = newInvoiceID,
                        FDRIGA = sequence++,
                        FDCODA = row.ODCODA,
                        FDQTAV = row.QuantityToSend,
                        FDTQTA = row.ODTQTA,
                        FDPREZ = row.ODPREZ,
                        FDTPRE = row.ODTPRE,
                        FDSCO1 = row.ODSCO1,
                        FDSCO2 = row.ODSCO2,
                        FDSCO3 = row.ODSCO3,
                        FDMAGG = row.ODMAGG,
                        FDTSC1 = row.ODTSC1,
                        FDTSC2 = row.ODTSC2,
                        FDTSC3 = row.ODTSC3,
                        FDTMAG = row.ODTMAG,
                        FDASSF = row.ODASSF,
                        FDALIV = row.ODALIV,
                        FDDACO = row.ODDACO,
                        FDRIFC = row.ODRIFC,
                        FDGRUP = row.ODGRUP,
                        FDCONT = row.ODCONT,
                        FDSCTO = row.ODSCTO,
                        FDUNIM = row.odunit,
                        OTANN1 = row.OTANNO,
                        OTNUO1 = row.OTNUOR,
                        ODRIG1 = row.ODRIGA,
                        FDNOTE = row.ODNOTE,
                        FDSHOW = row.ODSHOW,
                        FDCOAG1 = row.ODCOA1,
                        FDCOAG2 = row.ODCOA2,
                        FDPROV = row.ODCOA1P,
                        fdpro2 = row.ODCOA2P,
                        FDCOAG1PT = row.ODCOA1PT,
                        FDCOAG2PT = row.ODCOA2PT,
                        FDSERI = row.ODSERI,
                        fdtiva = rate?.assnatufe
                    };
                    connection.Execute(fattdRepo.INSERT_QUERY, newRow, transaction);
                    row.ODSTATO = row.FulfillmentID == "S" ? "*" : null;
                    row.ODQTAEV = (row.ODQTAEV ?? 0) + row.QuantityToSend;
                    connection.Execute(ordidRepo.UPDATE_QUERY, row, transaction);
                }
                // check for plafond and stamp
                if (plafondTotal > 0)
                {
                    var plafondSettings = plafondParmsRepo.Get(OrderHead.otsoci);
                    if (plafondTotal > plafondSettings?.limit_amount)
                    {
                        // add stamp row
                        var cliammi = cliammiRepo.Get(OrderHead.otsoci, OrderHead.OTCLIE ?? 0);
                        var plafond = plafondRepo.GetLast(OrderHead.otsoci, OrderHead.OTCLIE ?? 0, now.Year, now.Date, false);
                        var plafondRate = aliquotaRepo.Get(plafondSettings.rate_code, plafondSettings.rate_value);
                        var stampRow = new FATTD00F()
                        {
                            ftsoci = OrderHead.otsoci,
                            FTANNO = now.Year,
                            FTNUOR = newInvoiceID,
                            FDRIGA = 999999,
                            FDCODA = plafondSettings.product_id,
                            FDQTAV = 1,
                            FDTQTA = "B",
                            FDPREZ = plafondSettings.stamp_amount,
                            FDTPRE = "U",
                            FDUNIM = plafondSettings.UM,
                            FDASSF = plafondSettings.rate_code,
                            FDALIV = plafondSettings.rate_value,
                            FDGRUP = plafondSettings.group_id,
                            FDCONT = plafondSettings.account_id,
                            FDSCTO = plafondSettings.subaccount_id,
                            FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                            FDNOTE = plafond?.clinote,
                            FDSHOW = true,
                            fdtiva = plafondRate?.assnatufe
                        };
                        connection.Execute(fattdRepo.INSERT_QUERY, stampRow, transaction);
                    }
                }

                transaction.Commit();
                InfoHandler.Show($"Generata correttamente la fattura {now.Year}/{newInvoiceID}");
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

    public bool GenerateByDDT(List<BOLLT00F> DDTList, int Year, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var plafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var plafondRowsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var plafondParmsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var caufatRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>();
                var fattdRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                foreach (var customersGroup in DDTList.GroupBy(g => new { g.BTCODC }).Select(s => s.ToList()))
                {
                    var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(customersGroup?.FirstOrDefault()?.bolsoc ?? string.Empty, (customersGroup?.FirstOrDefault()?.BTCODC ?? 0));

                    if (cliammi != null && cliammi.CLRAGF && !cliammi.CLRAGFD)
                    {
                        // group invoices by CUSTOMER
                        foreach (var group in DDTList.Where(w => w.BTCODC == (customersGroup?.FirstOrDefault()?.BTCODC ?? 0)).GroupBy(g => new { g.BTCODC, g.BTCAUS, g.BTPAGA, g.abiabi, g.abicab })
                                                    .Select(s => s.ToList()))
                        {
                            var ddt = group.FirstOrDefault();

                            if (ddt != null)
                            {
                                var newInvoiceID = numRegRepository.GetNumber(ddt!.bolsoc, Year, Constants.INVOICE_TEMP, true);
                                var invoiceCausal = caufatRepo.Get(ddt.Causal?.bolfac ?? string.Empty);
                                var rows = new List<BOLLD00F>();
                                foreach (var item in group)
                                {
                                    rows.AddRange(item.Rows ?? new ObservableCollection<BOLLD00F>());
                                    // update DDT status
                                    item.BTSTATO = "F";
                                    connection.Execute(bolltRepo.UPDATE_QUERY, item, transaction);
                                }

                                // generate invoice head
                                var newHead = new FATTT00F()
                                {
                                    ftsoci = ddt.bolsoc,
                                    FTANNO = Year,
                                    FTNUOR = newInvoiceID,
                                    FTTIPO = "F",
                                    FTDAOR = now.Date,
                                    FTCAUS = invoiceCausal?.fatcod,
                                    FTCODC = ddt.BTCODC,
                                    FTCODD = ddt.BTCODD,
                                    FTPAGA = ddt.BTPAGA,
                                    FTABIB = ddt.abiabi,
                                    FTCABB = ddt.abicab,
                                    FTCONS = ddt.BTCONS,
                                    FTSPED = ddt.BTSPED,
                                    FTCORR = ddt.BTCORR,
                                    FTCORR2 = ddt.BTCORR2,
                                    FTCIDI = "UIC",
                                    ftciva = "EUR",
                                    FTIMBL = ddt.BTIMBA,
                                    ftling = ddt.BTLING,
                                    addedUserID = UserID,
                                    FTNOTET = null,
                                    FTNOTEP = null,
                                    FTSHOWT = false,
                                    FTSHOWP = false,
                                    FTDE25 = ddt.BTDE25,
                                    FTBCON = ddt.BTBCON,
                                    FTSCCL = ddt.BTSCCL,
                                    FTAREA = ddt.BTAREA,
                                    FTRIVE = ddt.BTRIVE,
                                    FTCONZ = ddt.BTCONZ,
                                    FTFILI = ddt.BTFILI,
                                    FTZONA = ddt.BTZONA,
                                    FTSETM = ddt.BTSETM,
                                    FTREGI = ddt.BTREGI,
                                    fttdoc = invoiceCausal?.fattido
                                };
                                connection.Execute(INSERT_QUERY, newHead, transaction);

                                // generate rows
                                decimal plafondTotal = 0;
                                int sequence = 1;
                                foreach (var row in rows)
                                {
                                    var rate = aliquotaRepo.Get(row.boasso ?? string.Empty, row.boaliq ?? string.Empty);
                                    if (rate?.assplaBool ?? false)
                                        plafondTotal += row.Amount;
                                    var newRow = new FATTD00F()
                                    {
                                        ftsoci = ddt.bolsoc,
                                        FTANNO = Year,
                                        FTNUOR = newInvoiceID,
                                        FDRIGA = sequence++,
                                        FDCODA = row.BOCODA,
                                        FDQTAV = row.BOQTAV,
                                        FDTQTA = row.BOTQTA,
                                        FDPREZ = row.boprez,
                                        FDTPRE = row.botpre,
                                        FDSCO1 = row.bosco1,
                                        FDSCO2 = row.bosco2,
                                        FDSCO3 = row.bosco3,
                                        FDMAGG = row.bomagg,
                                        FDTSC1 = row.botsc1,
                                        FDTSC2 = row.botsc2,
                                        FDTSC3 = row.botsc3,
                                        FDTMAG = row.botmag,
                                        FDASSF = row.boasso,
                                        FDALIV = row.boaliq,
                                        FDDACO = row.BODACO,
                                        FDRIFC = row.BORIFC,
                                        FDGRUP = row.bogrup,
                                        FDCONT = row.bocont,
                                        FDSCTO = row.bosotc,
                                        FDUNIM = row.BOUNIM,
                                        FDBONO = row.BTANNO,
                                        FDBOLL = row.BTBOLL,
                                        FDBORI = row.BORIGB,
                                        OTANN1 = row.BOANNO,
                                        OTNUO1 = row.BONUOR,
                                        ODRIG1 = row.BORIGA,
                                        FDNOTE = row.BOSHOW ? row.BONOTE : null,
                                        FDSHOW = row.BOSHOW,
                                        FDCOAG1 = row.BOCOA1,
                                        FDCOAG2 = row.BOCOA2,
                                        FDPROV = row.BOCOA1P,
                                        fdpro2 = row.BOCOA2P,
                                        FDCOAG1PT = row.BOCOA1PT,
                                        FDCOAG2PT = row.BOCOA2PT,
                                        FDSERI = row.BOSERI,
                                        fdtiva = rate?.assnatufe
                                    };
                                    connection.Execute(fattdRepo.INSERT_QUERY, newRow, transaction);
                                }

                                // check for plafond and stamp
                                if (plafondTotal > 0)
                                {
                                    var plafondSettings = plafondParmsRepo.Get(ddt.bolsoc);
                                    if (plafondTotal > plafondSettings?.limit_amount)
                                    {
                                        // add stamp row
                                        var plafond = plafondRepo.GetLast(ddt.bolsoc, ddt.BTCODC ?? 0, Year, now.Date, false);
                                        var plafondRate = aliquotaRepo.Get(plafondSettings.rate_code, plafondSettings.rate_value);
                                        var stampRow = new FATTD00F()
                                        {
                                            ftsoci = ddt.bolsoc,
                                            FTANNO = Year,
                                            FTNUOR = newInvoiceID,
                                            FDRIGA = 999999,
                                            FDCODA = plafondSettings.product_id,
                                            FDQTAV = 1,
                                            FDTQTA = "B",
                                            FDPREZ = plafondSettings.stamp_amount,
                                            FDTPRE = "U",
                                            FDUNIM = plafondSettings.UM,
                                            FDASSF = plafondSettings.rate_code,
                                            FDALIV = plafondSettings.rate_value,
                                            FDGRUP = plafondSettings.group_id,
                                            FDCONT = plafondSettings.account_id,
                                            FDSCTO = plafondSettings.subaccount_id,
                                            FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                                            FDNOTE = plafond?.clinote,
                                            FDSHOW = true,
                                            fdtiva = plafondRate?.assnatufe
                                        };
                                        connection.Execute(fattdRepo.INSERT_QUERY, stampRow, transaction);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (cliammi != null && cliammi.CLRAGF && cliammi.CLRAGFD)
                        {
                            // group invoices by CUSTOMER AND RECIPIENT
                            foreach (var group in DDTList.Where(w => w.BTCODC == (customersGroup?.FirstOrDefault()?.BTCODC ?? 0)).GroupBy(g => new { g.BTCODC, g.BTCODD, g.BTCAUS, g.BTPAGA, g.abiabi, g.abicab })
                                                        .Select(s => s.ToList()))
                            {
                                var ddt = group.First();

                                if (ddt != null)
                                {
                                    var newInvoiceID = numRegRepository.GetNumber(ddt.bolsoc, Year, Constants.INVOICE_TEMP, true);
                                    var invoiceCausal = caufatRepo.Get(ddt.Causal?.bolfac ?? string.Empty);
                                    var rows = new List<BOLLD00F>();
                                    foreach (var item in group)
                                    {
                                        rows.AddRange(item.Rows ?? new ObservableCollection<BOLLD00F>());
                                        // update DDT status
                                        item.BTSTATO = "F";
                                        connection.Execute(bolltRepo.UPDATE_QUERY, item, transaction);
                                    }

                                    // compute head and footer notes
                                    var headNotes = string.Join("\n", group.Select(s => s.BTNOTET));
                                    var footNotes = string.Join("\n", group.Select(s => s.BTNOTEP));

                                    // generate invoice head
                                    var newHead = new FATTT00F()
                                    {
                                        ftsoci = ddt.bolsoc,
                                        FTANNO = Year,
                                        FTNUOR = newInvoiceID,
                                        FTTIPO = "F",
                                        FTDAOR = now.Date,
                                        FTCAUS = invoiceCausal?.fatcod,
                                        FTCODC = ddt.BTCODC,
                                        FTCODD = ddt.BTCODD,
                                        FTPAGA = ddt.BTPAGA,
                                        FTABIB = ddt.abiabi,
                                        FTCABB = ddt.abicab,
                                        FTCONS = ddt.BTCONS,
                                        FTSPED = ddt.BTSPED,
                                        FTCORR = ddt.BTCORR,
                                        FTCORR2 = ddt.BTCORR2,
                                        FTCIDI = "UIC",
                                        ftciva = "EUR",
                                        FTIMBL = ddt.BTIMBA,
                                        ftling = ddt.BTLING,
                                        addedUserID = UserID,
                                        FTNOTET = null,
                                        FTNOTEP = null,
                                        FTSHOWT = false,
                                        FTSHOWP = false,
                                        FTDE25 = ddt.BTDE25,
                                        FTBCON = ddt.BTBCON,
                                        FTSCCL = ddt.BTSCCL,
                                        FTAREA = ddt.BTAREA,
                                        FTRIVE = ddt.BTRIVE,
                                        FTCONZ = ddt.BTCONZ,
                                        FTFILI = ddt.BTFILI,
                                        FTZONA = ddt.BTZONA,
                                        FTSETM = ddt.BTSETM,
                                        FTREGI = ddt.BTREGI,
                                        fttdoc = invoiceCausal?.fattido
                                    };
                                    connection.Execute(INSERT_QUERY, newHead, transaction);

                                    // generate rows
                                    decimal plafondTotal = 0;
                                    int sequence = 1;
                                    foreach (var row in rows)
                                    {
                                        var rate = aliquotaRepo.Get(row.boasso ?? string.Empty, row.boaliq ?? string.Empty);
                                        if (rate?.assplaBool ?? false)
                                            plafondTotal += row.Amount;
                                        var newRow = new FATTD00F()
                                        {
                                            ftsoci = ddt.bolsoc,
                                            FTANNO = Year,
                                            FTNUOR = newInvoiceID,
                                            FDRIGA = sequence++,
                                            FDCODA = row.BOCODA,
                                            FDQTAV = row.BOQTAV,
                                            FDTQTA = row.BOTQTA,
                                            FDPREZ = row.boprez,
                                            FDTPRE = row.botpre,
                                            FDSCO1 = row.bosco1,
                                            FDSCO2 = row.bosco2,
                                            FDSCO3 = row.bosco3,
                                            FDMAGG = row.bomagg,
                                            FDTSC1 = row.botsc1,
                                            FDTSC2 = row.botsc2,
                                            FDTSC3 = row.botsc3,
                                            FDTMAG = row.botmag,
                                            FDASSF = row.boasso,
                                            FDALIV = row.boaliq,
                                            FDDACO = row.BODACO,
                                            FDRIFC = row.BORIFC,
                                            FDGRUP = row.bogrup,
                                            FDCONT = row.bocont,
                                            FDSCTO = row.bosotc,
                                            FDUNIM = row.BOUNIM,
                                            FDBONO = row.BTANNO,
                                            FDBOLL = row.BTBOLL,
                                            FDBORI = row.BORIGB,
                                            OTANN1 = row.BOANNO,
                                            OTNUO1 = row.BONUOR,
                                            ODRIG1 = row.BORIGA,
                                            FDNOTE = row.BOSHOW ? row.BONOTE : null,
                                            FDSHOW = row.BOSHOW,
                                            FDCOAG1 = row.BOCOA1,
                                            FDCOAG2 = row.BOCOA2,
                                            FDPROV = row.BOCOA1P,
                                            fdpro2 = row.BOCOA2P,
                                            FDCOAG1PT = row.BOCOA1PT,
                                            FDCOAG2PT = row.BOCOA2PT,
                                            FDSERI = row.BOSERI,
                                            fdtiva = rate?.assnatufe
                                        };
                                        connection.Execute(fattdRepo.INSERT_QUERY, newRow, transaction);
                                    }

                                    // check for plafond and stamp
                                    if (plafondTotal > 0)
                                    {
                                        var plafondSettings = plafondParmsRepo.Get(ddt.bolsoc);
                                        if (plafondTotal > plafondSettings?.limit_amount)
                                        {
                                            // add stamp row
                                            var plafond = plafondRepo.GetLast(ddt.bolsoc, ddt.BTCODC ?? 0, Year, now.Date, false);
                                            var plafondRate = aliquotaRepo.Get(plafondSettings.rate_code, plafondSettings.rate_value);
                                            var stampRow = new FATTD00F()
                                            {
                                                ftsoci = ddt.bolsoc,
                                                FTANNO = Year,
                                                FTNUOR = newInvoiceID,
                                                FDRIGA = 999999,
                                                FDCODA = plafondSettings.product_id,
                                                FDQTAV = 1,
                                                FDTQTA = "B",
                                                FDPREZ = plafondSettings.stamp_amount,
                                                FDTPRE = "U",
                                                FDUNIM = plafondSettings.UM,
                                                FDASSF = plafondSettings.rate_code,
                                                FDALIV = plafondSettings.rate_value,
                                                FDGRUP = plafondSettings.group_id,
                                                FDCONT = plafondSettings.account_id,
                                                FDSCTO = plafondSettings.subaccount_id,
                                                FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                                                FDNOTE = plafond?.clinote,
                                                FDSHOW = true,
                                                fdtiva = plafondRate?.assnatufe
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().INSERT_QUERY, stampRow, transaction);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // do not group invoices
                            foreach (var ddt in DDTList.Where(w => w.BTCODC == (customersGroup?.FirstOrDefault()?.BTCODC ?? 0)))
                            {
                                var newInvoiceID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(ddt.bolsoc, Year, Constants.INVOICE_TEMP, true);
                                var invoiceCausal = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Get(ddt.Causal?.bolfac ?? string.Empty);

                                // generate invoice head
                                var newHead = new FATTT00F()
                                {
                                    ftsoci = ddt.bolsoc,
                                    FTANNO = Year,
                                    FTNUOR = newInvoiceID,
                                    FTTIPO = "F",
                                    FTDAOR = now.Date,
                                    FTCAUS = invoiceCausal?.fatcod,
                                    FTCODC = ddt.BTCODC,
                                    FTCODD = ddt.BTCODD,
                                    FTPAGA = ddt.BTPAGA,
                                    FTABIB = ddt.abiabi,
                                    FTCABB = ddt.abicab,
                                    FTCONS = ddt.BTCONS,
                                    FTSPED = ddt.BTSPED,
                                    FTCORR = ddt.BTCORR,
                                    FTCORR2 = ddt.BTCORR2,
                                    FTCIDI = "UIC",
                                    ftciva = "EUR",
                                    FTIMBL = ddt.BTIMBA,
                                    ftling = ddt.BTLING,
                                    addedUserID = UserID,
                                    FTNOTET = ddt.BTSHOWT ? ddt.BTNOTET : null,
                                    FTNOTEP = ddt.BTSHOWP ? ddt.BTNOTEP : null,
                                    FTSHOWT = ddt.BTSHOWT,
                                    FTSHOWP = ddt.BTSHOWP,
                                    FTDE25 = ddt.BTDE25,
                                    FTBCON = ddt.BTBCON,
                                    FTSCCL = ddt.BTSCCL,
                                    FTAREA = ddt.BTAREA,
                                    FTRIVE = ddt.BTRIVE,
                                    FTCONZ = ddt.BTCONZ,
                                    FTFILI = ddt.BTFILI,
                                    FTZONA = ddt.BTZONA,
                                    FTSETM = ddt.BTSETM,
                                    FTREGI = ddt.BTREGI,
                                    fttdoc = invoiceCausal?.fattido
                                };
                                connection.Execute(INSERT_QUERY, newHead, transaction);

                                // generate rows
                                decimal plafondTotal = 0;
                                int sequence = 1;
                                foreach (var row in ddt.Rows ?? new ObservableCollection<BOLLD00F>())
                                {
                                    var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.boasso ?? string.Empty, row.boaliq ?? string.Empty);
                                    if (rate?.assplaBool ?? false)
                                        plafondTotal += row.Amount;
                                    var newRow = new FATTD00F()
                                    {
                                        ftsoci = ddt.bolsoc,
                                        FTANNO = Year,
                                        FTNUOR = newInvoiceID,
                                        FDRIGA = sequence++,
                                        FDCODA = row.BOCODA,
                                        FDQTAV = row.BOQTAV,
                                        FDTQTA = row.BOTQTA,
                                        FDPREZ = row.boprez,
                                        FDTPRE = row.botpre,
                                        FDSCO1 = row.bosco1,
                                        FDSCO2 = row.bosco2,
                                        FDSCO3 = row.bosco3,
                                        FDMAGG = row.bomagg,
                                        FDTSC1 = row.botsc1,
                                        FDTSC2 = row.botsc2,
                                        FDTSC3 = row.botsc3,
                                        FDTMAG = row.botmag,
                                        FDASSF = row.boasso,
                                        FDALIV = row.boaliq,
                                        FDDACO = row.BODACO,
                                        FDRIFC = row.BORIFC,
                                        FDGRUP = row.bogrup,
                                        FDCONT = row.bocont,
                                        FDSCTO = row.bosotc,
                                        FDUNIM = row.BOUNIM,
                                        FDBONO = row.BTANNO,
                                        FDBOLL = row.BTBOLL,
                                        FDBORI = row.BORIGB,
                                        OTANN1 = row.BOANNO,
                                        OTNUO1 = row.BONUOR,
                                        ODRIG1 = row.BORIGA,
                                        FDNOTE = row.BOSHOW ? row.BONOTE : null,
                                        FDSHOW = row.BOSHOW,
                                        FDCOAG1 = row.BOCOA1,
                                        FDCOAG2 = row.BOCOA2,
                                        FDPROV = row.BOCOA1P,
                                        fdpro2 = row.BOCOA2P,
                                        FDCOAG1PT = row.BOCOA1PT,
                                        FDCOAG2PT = row.BOCOA2PT,
                                        FDSERI = row.BOSERI,
                                        fdtiva = rate?.assnatufe
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().INSERT_QUERY, newRow, transaction);
                                }

                                // update DDT status
                                ddt.BTSTATO = "F";
                                connection.Execute(bolltRepo.UPDATE_QUERY, ddt, transaction);

                                // check for plafond and stamp
                                if (plafondTotal > 0)
                                {
                                    var plafondSettings = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Get(ddt.bolsoc);
                                    if (plafondTotal > plafondSettings?.limit_amount)
                                    {
                                        // add stamp row
                                        var plafond = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().GetLast(ddt.bolsoc, ddt.BTCODC ?? 0, Year, now.Date, false);

                                        if (plafond == null)
                                        {
                                            ErrorHandler.Show("Impossibile generare: plafond non trovato");
                                            return false;
                                        }

                                        var plafondRate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(plafondSettings.rate_code, plafondSettings.rate_value);
                                        var stampRow = new FATTD00F()
                                        {
                                            ftsoci = ddt.bolsoc,
                                            FTANNO = Year,
                                            FTNUOR = newInvoiceID,
                                            FDRIGA = 999999,
                                            FDCODA = plafondSettings.product_id,
                                            FDQTAV = 1,
                                            FDTQTA = "B",
                                            FDPREZ = plafondSettings.stamp_amount,
                                            FDTPRE = "U",
                                            FDUNIM = plafondSettings.UM,
                                            FDASSF = plafondSettings.rate_code,
                                            FDALIV = plafondSettings.rate_value,
                                            FDGRUP = plafondSettings.group_id,
                                            FDCONT = plafondSettings.account_id,
                                            FDSCTO = plafondSettings.subaccount_id,
                                            FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                                            FDNOTE = plafond.clinote,
                                            FDSHOW = true,
                                            fdtiva = plafondRate?.assnatufe
                                        };
                                        connection.Execute(fattdRepo.INSERT_QUERY, stampRow, transaction);
                                    }
                                }
                            }
                        }
                    }
                }
                transaction.Commit();
                InfoHandler.Show($"Fatture generate correttamente");
                return true;
            }
            catch (Exception exc)
            {
                transaction.Rollback();
                ErrorHandler.Show(exc.Message);
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

    #region Printing
    public FATTT00F? GetPrintFull(string ftsoci, int FTANNO, int FTNUOR, bool PrintProductNote, bool PrintAgentsInDetails, bool UseCustomerCodes)
    {
        try
        {
            using var connection = GetOpenConnection();




            var result = connection.Query<FATTT00F>(@"SELECT i.*, c.*, si.*, d.*, ca.fatcod,ca.fatpre,ca.fatnmr,ca.fatdes,iso.isolin FROM FATTT00F AS i
                        INNER JOIN ABE AS c ON c.abecod = i.FTCODC
                        LEFT OUTER JOIN ISO AS iso ON iso.isocod = c.isocod
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = i.FTCODC AND d.codesti = i.FTCODD
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR", new[]
            { typeof(FATTT00F), typeof(ABE), typeof(FATTAUT), typeof(DESTINATARI), typeof(CAUFAT00F), typeof(string), }, (objects) =>
            {
                var fattt00f = (FATTT00F)objects[0];
                fattt00f.Customer = objects[1] as ABE;
                fattt00f.SelfInvoice = objects[2] as FATTAUT;
                fattt00f.Recipient = objects[3] as DESTINATARI;
                fattt00f.Causal = objects[4] as CAUFAT00F;
                fattt00f.Language = objects[5] as string;

                return fattt00f;
            }, new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,FTAUSC,cliecod,fatcod,isolin").FirstOrDefault();

            if (result != null)
            {
                var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(result.Language ?? string.Empty);

                //Tipo fattura

                if (languageDictionary != null)
                {
                    switch (result.FTTIPO)
                    {
                        case ("N"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_NotaCredito"].ToString();
                            break;
                        case ("A"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Autofattura"].ToString();
                            break;
                        case ("C"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Autonotacredito"].ToString();
                            break;
                        case ("P"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Parcella"].ToString();
                            break;
                        case ("B"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Notadebito"].ToString();
                            break;
                        default:
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Fattura"].ToString();
                            break;
                    }
                }

                var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(ftsoci);
                result.Rows = new ObservableCollection<FATTD00F>(
                    connection.Query<FATTD00F>(
                        $@"SELECT r.*, 
                            (
                                SELECT TOP(1) TRIM(customerProductID) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND l.unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductID,
                            (
                                SELECT TOP(1) TRIM(customerProductDescription) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND l.unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductDescription,
                            (CASE WHEN asspla = 'S' THEN 1 ELSE 0 END) AS HasPlafond,
                            {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes, ddt.bolsoc,ddt.BTNUBD,ddt.BTDATA, ddt.BTCODD, dest.ragisoc, dest.DEINDI, dest.deloc, dest.depro,ord.OTSOCI,ord.OTDAOR,ord.OTDE25,ord.OTCUNO,ord.OTCUDO,l.*
                            FROM FATTD00F AS r
                            INNER JOIN FATTT00F AS h ON h.ftsoci=r.ftsoci AND h.FTANNO = r.FTANNO AND h.FTNUOR=r.FTNUOR
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_lingua AS l ON p.SocietaID = l.SocietaID AND p.ID = l.ArticoloID AND l.lincod = @Lingua
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.FDCOAG1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.FDCOAG2
                            LEFT JOIN BOLLT00F as ddt ON ddt.bolsoc=r.ftsoci AND ddt.BTANNO = r.FDBONO AND ddt.BTBOLL = r.FDBOLL
                            LEFT JOIN ORDIT00F as ord ON ord.OTSOCI=r.ftsoci AND ord.OTANNO = r.OTANN1 AND ord.OTNUOR = r.OTNUO1
                            LEFT JOIN DESTINATARI AS dest ON dest.cliecod=ddt.BTCODC AND dest.codesti=ddt.BTCODD
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR
                            ORDER BY r.FDBONO, r.FDBOLL, r.OTANN1, r.OTNUO1",
                        new[] { typeof(FATTD00F), typeof(tab_articolo), typeof(tab_articolo_unita), typeof(AGENTI), typeof(AGENTI), typeof(BOLLT00F), typeof(DESTINATARI), typeof(ORDIT00F), typeof(tab_articolo_lingua) },
                        (objs) =>
                        {
                            var obj = (FATTD00F)objs[0];
                            obj.UMsCache = umsCache;
                            obj.Product = (tab_articolo)objs[1];
                            obj.Product.Descrizione = (objs[8] as tab_articolo_lingua != null && !string.IsNullOrEmpty((objs[8] as tab_articolo_lingua)?.Descrizione) ? (objs[8] as tab_articolo_lingua)?.Descrizione : obj.Product.Descrizione) ?? string.Empty;
                            obj.Product.Note = (objs[8] as tab_articolo_lingua != null && !string.IsNullOrEmpty((objs[8] as tab_articolo_lingua)?.Note) ? (objs[8] as tab_articolo_lingua)?.Note : obj.Product.Note);
                            obj.UM = objs[2] as tab_articolo_unita;
                            obj.FirstAgent = objs[3] as AGENTI;
                            obj.SecondAgent = objs[4] as AGENTI;
                            obj.PrintAgentsInDetails = PrintAgentsInDetails && (obj.FirstAgent != null || obj.SecondAgent != null);
                            obj.LinkedDDT = objs[5] as BOLLT00F;
                            obj.RecipientDescription = (objs[6] as DESTINATARI)?.FullRecipientText;
                            obj.LinkedOrder = objs[7] as ORDIT00F;
                            obj.CustomerDiscount = result.FTSCCL ?? 0;
                            return obj;
                        },
                        new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR, Lingua = result.Language }, splitOn: "SocietaID,SocietaID,agecod,agecod,bolsoc,ragisoc,OTSOCI,SocietaID").ToList());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent
                    var agent1 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG1 ?? string.Empty);
                    var agent2 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG2 ?? string.Empty);
                    result.DefaultFirstAgent = agent1;
                    result.DefaultSecondAgent = agent2;
                }

                // check if custom code and UMs recap for each row
                int? lastYear = null;
                int? lastNumber = null;
                int? lastODAYear = null;
                int? lastODANumber = null;
                var customization = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(ftsoci);


                foreach (var row in result.Rows)
                {
                    #region DDT grouping
                    if (lastYear != row.FDBONO || lastNumber != row.FDBOLL)
                    {
                        row.DDTReferenceText = $"{languageDictionary?["RiferimentoDDT"].ToString()} {row.FDBONO}/{row.LinkedDDT?.BTNUBD} {languageDictionary?["Del"].ToString()} {(row.LinkedDDT?.BTDATA ?? DateTime.MinValue).ToString("dd/MM/yyyy")}{(customization?.azinvshde ?? false ? $" - {languageDictionary?["DestinatarioBreve"].ToString()} {row.RecipientDescription}" : null)}";
                        lastYear = row.FDBONO;
                        lastNumber = row.FDBOLL;
                        lastODAYear = null;
                        lastODANumber = null;
                    }
                    #endregion
                    #region ODA grouping
                    if (lastODAYear != row.OTANN1 || lastODANumber != row.OTNUO1)
                    {
                        row.ODAReferenceText = $"{languageDictionary?["RiferimentoODA"].ToString()} {row.OTANN1}/{row.OTNUO1} {languageDictionary?["Del"].ToString()} {(row.LinkedOrder?.OTDAOR ?? DateTime.MinValue).ToString("dd/MM/yyyy")}";
                        // check if customization need to print customer ref from order
                        if (customization?.azinvricl ?? false)
                        {
                            if (!string.IsNullOrWhiteSpace(row.LinkedOrder?.OTCUNO) && row.LinkedOrder.OTCUDO.HasValue)
                                row.ODAReferenceText += $" [{languageDictionary?["VostroOrdine"].ToString()}{row.LinkedOrder.OTCUNO.Trim()} {languageDictionary?["Del"].ToString()} {(row.LinkedOrder.OTCUDO ?? DateTime.MinValue).ToString("dd/MM/yyyy")}]";
                            else
                                row.ODAReferenceText += $" [{row.LinkedOrder?.OTDE25?.Trim()}]";
                        }
                        lastODAYear = row.OTANN1;
                        lastODANumber = row.OTNUO1;
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
                                row.FDCODA = row.CustomerProductID;
                            if (row.Product != null && !string.IsNullOrWhiteSpace(row.CustomerProductDescription))
                                row.Product.Descrizione = row.CustomerProductDescription;
                        }
                    }
                    else
                    {
                        row.CustomerCode = null;
                    }
                    #endregion
                }
                result.Attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                        @"SELECT * FROM FATTAL00F
                            WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                            ORDER BY FTANAME",
                        new { FTASOCI = ftsoci, FTAANNO = FTANNO, FTANUOR = FTNUOR }).ToList());

            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public InvoiceReport? PrintInvoice(FATTT00F Invoice)
    {
        try
        {
            // set foot notes on rows to print it
            if (Invoice.FTSHOWP)
            {
                foreach (var row in Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                { row.HeaderFootNote = Invoice.FTNOTEP; }
            }
            #region Expires
            var expires = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().ComputeExpires(Invoice.ftsoci, Invoice.FTPAGA ?? string.Empty, Invoice.FTDAOR ?? DateTime.MinValue, Invoice.FTCODC ?? 0);
            decimal stepAmount = Math.Round(Invoice.GrandTotal / (expires?.Count ?? 1), 2);
            decimal finalStepAmount = Math.Round((stepAmount + (Invoice.GrandTotal - (stepAmount * (expires?.Count ?? 1)))), 2);
            int i = 1;
            var expireList = new List<Tuple<string, string>>();
            foreach (var exp in expires ?? new List<DateTime>())
            {
                expireList.Add(new Tuple<string, string>(exp.Date.ToString("dd/MM/yyyy"), (i == (expires?.Count ?? 1) ? $"{finalStepAmount.ToString("N2")} €" : $"{stepAmount.ToString("N2")} €")));
                i++;
            }
            #endregion
            #region Rates
            i = 1;
            List<string> rates = new List<string>();
            var ratesList = new List<Tuple<string, string, string, string>>();
            var ratesList2 = new List<Tuple<string, string, string, string>>();
            foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).OrderBy(o => o.FDASSF).ThenBy(o => o.FDALIV))
            {
                if (!rates.Contains(row.FDASSF + row.FDALIV?.Trim()))
                {
                    var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.FDASSF ?? string.Empty, row.FDALIV ?? string.Empty);
                    rates.Add(row.FDASSF + row.FDALIV?.Trim());
                    var imponibile = Math.Round((Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                    .Where(w => w.FDASSF == row.FDASSF && w.FDALIV == row.FDALIV)
                    .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                    // compute customer discount
                    imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                    decimal rateValue = 0;
                    decimal.TryParse(rate?.assali, out rateValue);
                    var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                    if (i % 2 == 0)
                        ratesList2.Add(new Tuple<string, string, string, string>(row.FDASSF + " " + row.FDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                    else
                        ratesList.Add(new Tuple<string, string, string, string>(row.FDASSF + " " + row.FDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                    i++;
                }
            }
            #endregion

            if (Invoice.HasGifts)
            {
                #region Gifts rates recap
                i = 1;
                List<string> giftsRates = new List<string>();
                var giftsRatesList = new List<Tuple<string, string, string, string>>();
                foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDTQTA == "O").OrderBy(o => o.FDASSF).ThenBy(o => o.FDALIV))
                {
                    if (!giftsRates.Contains(row.FDASSF + row.FDALIV?.Trim()))
                    {
                        var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.FDASSF ?? string.Empty, row.FDALIV ?? string.Empty);
                        giftsRates.Add(row.FDASSF + row.FDALIV?.Trim());
                        var imponibile = Math.Round((Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                        .Where(w => w.FDTQTA == "O" && w.FDASSF == row.FDASSF && w.FDALIV == row.FDALIV)
                        .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                        // compute customer discount
                        imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                        decimal rateValue = 0;
                        decimal.TryParse(rate?.assali, out rateValue);
                        var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                        giftsRatesList.Add(new Tuple<string, string, string, string>(row.FDASSF + " " + row.FDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                        i++;
                    }
                }
                Invoice.GiftsRatesRecap = giftsRatesList;
                #endregion
            }

            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(Invoice.ftsoci)!;
            var bank = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(Invoice.FTABIB ?? 0, Invoice.FTCABB ?? 0);
            var companyBank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Invoice.ftsoci, Invoice.FTABIB ?? 0, Invoice.FTCABB ?? 00, Invoice.FTBCON ?? string.Empty);
            var customerData = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Invoice.ftsoci, Invoice.FTCODC ?? 0);
            // get customizations
            var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Invoice.ftsoci)!;
            var aziendaLingua = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(Invoice.ftsoci, Invoice.Language ?? string.Empty);

            var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(Invoice.FTPAGA ?? string.Empty);
            var pagcliLingua = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLI_LINGUARepository>().Get(Invoice.FTPAGA ?? string.Empty, Invoice.Language ?? string.Empty);

            var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(Invoice.Language ?? string.Empty);

            object? objectDictionary = null;

            if (languageDictionary != null)
                objectDictionary = new LocalizationHelper().CreateClassFromDictionary(languageDictionary);

            return new InvoiceReport()
            {
                Invoice = Invoice,
                PaymentDescription = (pagcliLingua != null) ? (!string.IsNullOrEmpty(pagcliLingua.pcldes) ? pagcliLingua.pcldes : pagcli?.pcldes) : pagcli?.pcldes,
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Invoice.ftsoci),
                BankData = $"{bank?.FullDescriptionSearchable} c/c nr.{(companyBank != null ? Invoice.FTBCON : customerData?.CLNUCC)} IBAN: {(companyBank != null ? companyBank.abibiba : customerData?.cliban)}",
                Expires = expireList,
                Rates = ratesList,
                Rates2 = ratesList2,
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                CertificationsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}certs.png"),
                FixedText = (aziendaLingua != null) ? (!string.IsNullOrEmpty(aziendaLingua.azinvgtex) ? aziendaLingua.azinvgtex : azienda.azinvgtex) : azienda.azinvgtex,
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
    public string INSERT_QUERY => "INSERT INTO FATTT00F (ftsoci,FTANNO,FTNUOR,FTTIPO,FTNUFD,FTDAOR,FTCAUS,FTCODC,FTCODD,FTCONZ,FTPAGA,FTABIB,FTCABB,FTCONS,FTSPED,FTCORR,FTAREA,FTFILI,FTZONA,FTSETM,FTCIDI,FTDE25,FTFLA1,FTFLA2,FTFLA3,FTNURI,FTDARI,FTBOLL,FTIMBL,ftciva,ftdao2,FTSCAD,ftling,FTDATFEL,FTNUMFEL,ftdafa,ftnufa,ftannf,fttdoc,FTNOTET,FTNOTEP,FTSHOWT,FTSHOWP,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,FTBCON,FTCORR2,FTREGI,FTRIVE,FTSCCL,FTPLAF,FTCECO,ftsdiid) OUTPUT INSERTED.rv VALUES(@ftsoci,@FTANNO,@FTNUOR,@FTTIPO,@FTNUFD,@FTDAOR,@FTCAUS,@FTCODC,@FTCODD,@FTCONZ,@FTPAGA,@FTABIB,@FTCABB,@FTCONS,@FTSPED,@FTCORR,@FTAREA,@FTFILI,@FTZONA,@FTSETM,@FTCIDI,@FTDE25,@FTFLA1,@FTFLA2,@FTFLA3,@FTNURI,@FTDARI,@FTBOLL,@FTIMBL,@ftciva,@ftdao2,@FTSCAD,@ftling,@FTDATFEL,@FTNUMFEL,@ftdafa,@ftnufa,@ftannf,@fttdoc,@FTNOTET,@FTNOTEP,@FTSHOWT,@FTSHOWP,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@FTBCON,@FTCORR2,@FTREGI,@FTRIVE,@FTSCCL,@FTPLAF,@FTCECO,@ftsdiid)";
    public string UPDATE_QUERY => "UPDATE FATTT00F SET ftsoci = @ftsoci,FTANNO = @FTANNO,FTNUOR = @FTNUOR,FTTIPO = @FTTIPO,FTNUFD = @FTNUFD,FTDAOR = @FTDAOR,FTCAUS = @FTCAUS,FTCODC = @FTCODC,FTCODD = @FTCODD,FTCONZ = @FTCONZ,FTPAGA = @FTPAGA,FTABIB = @FTABIB,FTCABB = @FTCABB,FTCONS = @FTCONS,FTSPED = @FTSPED,FTCORR = @FTCORR,FTAREA = @FTAREA,FTFILI = @FTFILI,FTZONA = @FTZONA,FTSETM = @FTSETM,FTCIDI = @FTCIDI,FTDE25 = @FTDE25,FTFLA1 = @FTFLA1,FTFLA2 = @FTFLA2,FTFLA3 = @FTFLA3,FTNURI = @FTNURI,FTDARI = @FTDARI,FTBOLL = @FTBOLL,FTIMBL = @FTIMBL,ftciva = @ftciva,ftdao2 = @ftdao2,FTSCAD = @FTSCAD,ftling = @ftling,FTDATFEL = @FTDATFEL,FTNUMFEL = @FTNUMFEL,ftdafa = @ftdafa,ftnufa = @ftnufa,ftannf = @ftannf,fttdoc = @fttdoc,FTNOTET = @FTNOTET,FTNOTEP = @FTNOTEP,FTSHOWT = @FTSHOWT,FTSHOWP = @FTSHOWP,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,FTBCON = @FTBCON,FTCORR2 = @FTCORR2,FTREGI = @FTREGI,FTRIVE = @FTRIVE,FTSCCL = @FTSCCL,FTPLAF = @FTPLAF,FTCECO = @FTCECO,ftsdiid = @ftsdiid OUTPUT INSERTED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTT00F OUTPUT DELETED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND rv = @rv";
    public bool Insert(FATTT00F Model)
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

    public bool Update(FATTT00F Model)
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

    public bool Delete(FATTT00F Model)
    {
        try
        {

            if (Model.FTNUFD <= 0)
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    var invoiceRows = connection.Query<FATTD00F>(@"SELECT * FROM FATTD00F
                                                                         WHERE ftsoci = @ftsoci AND FTANNO = @ftanno AND FTNUOR = @ftnuor",
                                                                    new { ftsoci = Model.ftsoci, ftanno = Model.FTANNO, ftnuor = Model.FTNUOR }, transaction);

                    bool haveDDT = false;

                    #region Free DDT
                    if (invoiceRows.Any(any => any.FDBONO.HasValue && any.FDBOLL.HasValue && any.FDBORI.HasValue))
                    {
                        foreach (var row in invoiceRows
                                            .Where(w => w.FDBONO.HasValue && w.FDBOLL.HasValue)
                                            .GroupBy(g => new { g.FDBONO, g.FDBOLL }))
                        {
                            haveDDT = true;
                            connection.Execute($"UPDATE BOLLT00F SET BTSTATO = 'R' WHERE bolsoc=@bolsoc AND BTANNO=@btanno AND BTBOLL=@btboll",
                                            new { bolsoc = Model.ftsoci, btanno = row.Key.FDBONO, btboll = row.Key.FDBOLL }, transaction);
                        }
                    }
                    #endregion

                    #region Free orders
                    if (invoiceRows.Any(any => any.OTANN1.HasValue && any.OTNUO1.HasValue && any.ODRIG1.HasValue) && !haveDDT)
                    {
                        // free rows
                        foreach (var row in invoiceRows.Where(w => w.OTANN1.HasValue && w.OTNUO1.HasValue && w.ODRIG1.HasValue))
                        {
                            var current = connection.Query<ORDID00F>($"SELECT * FROM ORDID00F WHERE otsoci=@otsoci AND OTANNO=@otanno AND OTNUOR=@otnuor AND ODRIGA=@odriga",
                                            new { otsoci = row.ftsoci, otanno = row.OTANN1, otnuor = row.OTNUO1, odriga = row.ODRIG1 }, transaction).FirstOrDefault();

                            if (current != null)
                            {
                                var diff = (current.ODQTAEV ?? 0) - (row.FDQTAV ?? 0);
                                connection.Execute($"UPDATE ORDID00F SET ODQTAEV = @diff, ODSTATO = NULL WHERE otsoci=@otsoci AND OTANNO=@otanno AND OTNUOR=@otnuor AND ODRIGA=@odriga",
                                                new { otsoci = row.ftsoci, otanno = row.OTANN1, otnuor = row.OTNUO1, odriga = row.ODRIG1, diff = diff }, transaction);
                            }
                        }
                        // update head
                        foreach (var row in invoiceRows
                                            .Where(w => w.OTANN1.HasValue && w.OTNUO1.HasValue)
                                            .GroupBy(g => new { g.OTANN1, g.OTNUO1 }))
                        {
                            connection.Execute($"UPDATE ORDIT00F SET flgchi = 'F' WHERE otsoci=@otsoci AND OTANNO=@otanno AND OTNUOR=@otnuor",
                                            new { otsoci = Model.ftsoci, otanno = row.Key.OTANN1, otnuor = row.Key.OTNUO1 }, transaction);
                        }
                    }
                    #endregion

                    #region Free accounting registration
                    var self = connection.Query<FATTAUT>(@"SELECT * FROM FATTAUT WHERE FTAUSC = @cid AND FTAUAN = @yea AND FTAUNUM = @id",
                        new { cid = Model.ftsoci, yea = Model.FTANNO, id = Model.FTNUOR },
                        transaction).FirstOrDefault();
                    if (self != null && self.FTAPNAN.HasValue)
                    {
                        connection.Execute("UPDATE PNTESTATA SET N1AUAN=NULL, N1AUNU=NULL, N1AUGE=NULL WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1REGI=@id",
                            new { cid = Model.ftsoci, yea = self.FTAPNAN, id = self.FTAPNRE }, transaction);
                    }
                    #endregion

                    // delete FATTAUT
                    connection.Execute("DELETE FROM FATTAUT WHERE FTAUSC = @cid AND FTAUAN = @yea AND FTAUNUM = @id",
                    new { cid = Model.ftsoci, yea = Model.FTANNO, id = Model.FTNUOR },
                    transaction);
                    // delete rows 
                    connection.Execute("DELETE FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @ftanno AND FTNUOR = @ftnuor",
                    new { ftsoci = Model.ftsoci, ftanno = Model.FTANNO, ftnuor = Model.FTNUOR },
                    transaction);
                    // delete head
                    connection.Execute(DELETE_QUERY, Model, transaction);

                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {

                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
            else
            {
                ErrorHandler.Show(ERR_CANNOT_DELETE);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(FATTT00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ftsoci) && Model.FTANNO > 0 && IsInsert && !Exists(Model.ftsoci, Model.FTANNO, Model.FTNUOR)) || !IsInsert)
            {
                if (Model.FTDAOR.HasValue && Model.FTDAOR.Value.Year == Model.FTANNO)
                {
                    if (Model.FTCODC.HasValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.FTCAUS))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.FTPAGA))
                            {
                                if (Model.FTABIB.HasValue && Model.FTCABB.HasValue)
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.FTNOTET) || (string.IsNullOrWhiteSpace(Model.FTNOTET) && !Model.FTSHOWT))
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.FTNOTEP) || (string.IsNullOrWhiteSpace(Model.FTNOTEP) && !Model.FTSHOWP))
                                        {
                                            return null;
                                        }
                                        else
                                        { return "Impossibile stampare una nota a pie' di pagina vuota"; }
                                    }
                                    else
                                    { return "Impossibile stampare una nota di testata vuota"; }
                                }
                                else
                                { return "La banca cliente è obbligatoria"; }
                            }
                            else
                            { return "Il tipo pagamento è obbligatorio"; }
                        }
                        else
                        { return "La causale fattura è obbligatoria"; }
                    }
                    else
                    { return "Il codice cliente è obbligatorio"; }
                }
                else
                { return "La data fattura non è coerente con l'anno della stessa"; }
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

public class FATTT00FUfpRepository : RepositoryBase, IFATTT00FRepository
{
    public FATTT00FUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public const string ERR_CANNOT_DELETE = "Impossibile eliminare una fattura stampata in definitivo";

    public ObservableCollection<FATTT00F>? GetList(string CompanyID, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FATTT00F, FATTAUT, CAUFAT00F, FATTT00F>(
                @"SELECT i.ftsoci,i.FTANNO,i.FTNUOR,i.FTTIPO,i.FTNUFD,i.FTDAOR,i.FTCAUS,i.FTCODC,i.FTCODD,i.FTDATFEL,i.FTNUMFEL,i.ftsdiid,i.FTSCCL,i.FTFLA1,i.FTFLA2,i.rv, CONCAT(c.abecod, ' ', TRIM(c.abers1)) AS CustomerFullDescriptionSearchable, si.*, ca.fatcod,ca.fatdes,ca.fatpre FROM FATTT00F AS i
                        INNER JOIN ANAG_BASE AS c ON c.abecod = i.FTCODC
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        WHERE i.ftsoci = @cid AND CONVERT(date, i.FTDAOR) >= @fd AND CONVERT(date, i.FTDAOR) <= @td
                        ORDER BY i.FTANNO DESC, i.FTNUOR DESC",
                (inv, sel, cau) => { inv.SelfInvoice = sel; inv.Causal = cau; return inv; },
                new { cid = CompanyID, fd = FromDate.Date, td = ToDate.Date }, splitOn: "FTAUSC,fatcod").ToList();
            var rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F, tab_articolo, FATTD00F>(
                    @"SELECT r.FTANNO,r.FTNUOR,r.FDTPRE,r.FDPREZ,r.FDQTAV, r.FDALIV, r.FDSCO1, r.FDSCO2, r.FDSCO3, r.FDMAGG, r.FDTSC1,r.FDTSC2,r.FDTSC3,r.FDTMAG,r.FDTQTA,  art.artcod, art.artumi, 0 FROM FATTD00F AS r
                            INNER JOIN anag_articoli AS art ON  art.artcod = FDCODA
                            INNER JOIN FATTT00F AS i ON i.ftsoci=r.ftsoci AND i.FTANNO=r.FTANNO AND i.FTNUOR=r.FTNUOR
                            WHERE r.ftsoci = @ftsoci AND CONVERT(date, i.FTDAOR) >= @fd AND CONVERT(date, i.FTDAOR) <= @td",
                    (row, prd) => { row.Product = new tab_articolo() { Descrizione = string.Empty, ID = string.Empty, SocietaID = CompanyID, TipoID = string.Empty, UnitaID = prd.UnitaID, QuantitaDefault = prd.QuantitaDefault }; return row; },
                    new { ftsoci = CompanyID, fd = FromDate.Date, td = ToDate.Date }, splitOn: "artcod").ToList());
            var attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                    @"SELECT a.* FROM FATTAL00F AS a
                            INNER JOIN FATTT00F AS i ON i.ftsoci=a.FTASOCI AND i.FTANNO=a.FTAANNO AND i.FTNUOR=a.FTANUOR
                            WHERE a.FTASOCI = @FTASOCI AND CONVERT(date, i.FTDAOR) >= @fd AND CONVERT(date, i.FTDAOR) <= @td
                            ORDER BY a.FTANAME",
                    new { FTASOCI = CompanyID, fd = FromDate.Date, td = ToDate.Date }).ToList());
            Parallel.ForEach(list, (head) =>
            {
                head.SDISentStatus = null;
                head.Rows = new ObservableCollection<FATTD00F>(rows.Where(w => w.FTANNO == head.FTANNO && w.FTNUOR == head.FTNUOR).ToList());
                head.Attachments = new ObservableCollection<FATTAL00F>(attachments.Where(w => w.FTAANNO == head.FTANNO && w.FTANUOR == head.FTNUOR).ToList());
            });

            #region Update sent status
            string? APIKey = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.azapikey?.Trim();
            if (APIKey != null)
            {
#pragma warning disable SYSLIB0014
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new CerberoSendAPI.SendInvoicesClient();
                if (client.State != System.ServiceModel.CommunicationState.Faulted)
                {
                    try
                    {
                        var request = new CerberoSendAPI.RetrieveSentStatusRequest(APIKey, list.Where(w => !string.IsNullOrWhiteSpace(w.FTNUMFEL) && string.IsNullOrWhiteSpace(w.ftsdiid)).Select(s => s.FTNUMFEL).ToList().ToArray());
                        var result = client.RetrieveSentStatusAsync(request).Result;
                        if (!string.IsNullOrWhiteSpace(result.RetrieveSentStatusResult.ErrorCode))
                        {
                            ErrorHandler.Show($"{result.RetrieveSentStatusResult.ErrorCode} - {result.RetrieveSentStatusResult.ErrorMessage}");
                        }
                        else
                        {
                            Parallel.ForEach(result.RetrieveSentStatusResult.Items, (item) =>
                            {
                                var tmpExist = list.Where(w => w.FTNUMFEL == item.Filename).First();
                                var existing = GetSingle(tmpExist.ftsoci, tmpExist.FTANNO, tmpExist.FTNUOR);
                                if (item.Status == "D" && existing != null)
                                {
                                    existing.ftsdiid = item.SDIID;
                                    Update(existing);
                                }
                                else if (item.Status == "F" && existing != null)
                                {
                                    existing.ftsdiid = item.SDIID;
                                    Update(existing);
                                    tmpExist.FailureDescription = item.FailureDescription;
                                }
                                else if (item.Status == "R")
                                {
                                    if (item.Errors != null && item.Errors.Count() > 0)
                                    {
                                        tmpExist.SDIErrors = new List<Tuple<string, string, string>>();
                                        foreach (var err in item.Errors.OrderBy(o => o.ID))
                                        {
                                            tmpExist.SDIErrors.Add(new Tuple<string, string, string>(err.ID, err.Description, err.Suggestion));
                                        }
                                    }
                                }
                                tmpExist.SDISentStatus = item.Status;
                            });
                        }
                    }
                    catch (Exception exc)
                    {
                        Exception? exi = exc;
                        var sb = new StringBuilder();
                        do
                        {
                            sb.Append(exi.Message).Append("\n\n");
                            exi = exi.InnerException;
                        } while (exi != null);
                        ErrorHandler.Show(sb.ToString());
                        client.Close();
                    }
                    client.Close();
                }
            }
            #endregion

            return new ObservableCollection<FATTT00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<FATTD00F>? GetListByOrder(string OrderCompanyID, int OrderYear, int OrderNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            var heads = connection.Query<FATTD00F>(@"SELECT DISTINCT ftsoci, ftanno, ftnuor, otann1, otnuo1 from FATTD00F
                                                        WHERE ftsoci = @ordc AND OTANN1 = @ordy AND OTNUO1 = @ordn
                                                        ORDER BY ftanno DESC, ftnuor DESC",
                                                    new { ordc = OrderCompanyID, ordy = OrderYear, ordn = OrderNumber });

            return new ObservableCollection<FATTD00F>(heads);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<FATTD00F>? GetListByOrderYear(string OrderCompanyID, int OrderYear)
    {
        try
        {
            using var connection = GetOpenConnection();


            var heads = connection.Query<FATTD00F>(@"SELECT DISTINCT ftanno, ftnuor, otann1, otnuo1 from FATTD00F
                                                        WHERE ftsoci = @ordc AND OTANN1 = @ordy
                                                        ORDER BY ftanno DESC, ftnuor DESC",
                                                    new { ordc = OrderCompanyID, ordy = OrderYear });

            return new ObservableCollection<FATTD00F>(heads);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? GetHead(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FATTT00F, ABE, CAUFAT00F, FATTT00F>(
                @"SELECT i.*, c.abecod, c.abers1, c.abers2, ca.fatcod, ca.fatpre FROM FATTT00F AS i
                        INNER JOIN ANAG_BASE AS c ON c.abecod = i.FTCODC
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                (inv, cus, cau) => { inv.Customer = cus; inv.Causal = cau; return inv; },
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,fatcod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? GetSingle(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FATTT00F>(
                @"SELECT i.* FROM FATTT00F AS i
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? Get(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            var unitaRepo = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>();


            var result = connection.Query<FATTT00F, ABE, FATTAUT, CAUFAT00F, FATTT00F>(
                @"SELECT i.*, c.abecod, c.abers1, c.abers2, si.*, ca.fatcod, ca.fatpre FROM FATTT00F AS i
                        INNER JOIN ANAG_BASE AS c ON c.abecod = i.FTCODC
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                (inv, cus, sel, cau) => { inv.Customer = cus; inv.SelfInvoice = sel; inv.Causal = cau; return inv; },
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,FTAUSC,fatcod")
                .FirstOrDefault();
            var umsCache = unitaRepo.GetSimpleList(ftsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F>(
                        @"SELECT * FROM FATTD00F
                            WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR",
                        new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }).ToList());
                result.Attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                        @"SELECT * FROM FATTAL00F
                            WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                            ORDER BY FTANAME",
                        new { FTASOCI = ftsoci, FTAANNO = FTANNO, FTANUOR = FTNUOR }).ToList());
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

    public InvoicingTrend? GetTrend(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            InvoicingTrend result = new InvoicingTrend() { Months = new ObservableCollection<InvoicingTrendMonth>() };

            var lastThreeYears = DateTime.Now.Year - 3;

            var heads = connection.Query<FATTT00F>(@"select ftsoci,ftanno,ftnuor,ftdaor,ftsccl,fttipo from fattt00f where ftsoci=@cid AND ftanno >= @year and canceled IS NULL order by ftanno,ftnuor;", new { cid = CompanyID , year = lastThreeYears});
            var years = heads.Select(s => s.FTANNO).Distinct().ToArray();
            var rows = connection.Query<FATTD00F>(@"select d.ftsoci,d.ftanno,d.ftnuor,d.FDSCO1, d.FDTSC1, d.FDSCO2, d.FDTSC2, d.FDSCO3, d.FDTSC3, d.FDMAGG, d.FDTMAG,d.fdprez, d.fdtpre,d.fdqtav,d.fdtqta from fattd00f AS d
                                                        INNER JOIN FATTT00F AS t ON t.ftsoci=d.ftsoci AND t.ftanno=d.ftanno AND t.ftnuor=d.ftnuor
                                                        where d.ftsoci=@cid and d.ftanno IN @years and t.canceled IS NULL", new { cid = CompanyID, years = years });
            //Parallel.ForEach(heads, head =>
            //{
            //    head.Rows = new ObservableCollection<FATTD00F>(rows.Where(w => w.ftsoci == head.ftsoci && w.FTANNO == head.FTANNO && w.FTNUOR == head.FTNUOR).ToList());
            //});

            var rowsLookup = rows
    .GroupBy(r => new { r.ftsoci, r.FTANNO, r.FTNUOR })
    .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var head in heads)
            {
                var key = new { head.ftsoci, head.FTANNO, head.FTNUOR };

                if (rowsLookup.TryGetValue(key, out var headRows))
                    head.Rows = new ObservableCollection<FATTD00F>(headRows);
                else
                    head.Rows = new ObservableCollection<FATTD00F>();
            }

            decimal previousMonthAmount = 0;
            foreach (var head in heads.GroupBy(g => new { g.FTANNO, (g.FTDAOR ?? DateTime.MinValue).Month }, (key, items) => new { key, items }))
            {
                var newMonth = new InvoicingTrendMonth()
                {
                    Year = head.key.FTANNO,
                    Month = head.key.Month,
                    Amount = head.items.Where(w => w.FTTIPO == "F" || w.FTTIPO == "B").Sum(sum => sum.Imponibile),
                    CreditAmount = head.items.Where(w => w.FTTIPO == "N").Sum(sum => sum.Imponibile),
                    Weight = 1,
                    PreviousMonthAmount = previousMonthAmount
                };
                result.Months.Add(newMonth);
                previousMonthAmount = newMonth.Amount;
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTT00F? GetFull(string ftsoci, int FTANNO, int FTNUOR, bool PrintProductNote, bool PrintAgentsInDetails)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<FATTT00F, ABE, FATTAUT, FATTT00F>(
                @"SELECT i.*, c.*, si.* ,
TRY_CAST(NULLIF(LTRIM(RTRIM(i.FTFILI)), '') AS INT) AS FTFILI,
FROM FATTT00F AS i
                        INNER JOIN ANAG_BASE AS c ON c.abecod = i.FTCODC
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR",
                (inv, cus, sel) => { inv.Customer = cus; inv.SelfInvoice = sel; return inv; },
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,FTAUSC")
                .FirstOrDefault();
            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(ftsoci);

            if (result != null)
            {
                result.Rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F, tab_articolo, tab_articolo_unita, AGENTI, AGENTI, FATTD00F>(
                        $@"SELECT r.*, (CASE WHEN asspla = 'S' THEN 1 ELSE 0 END) AS HasPlafond, {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes FROM FATTD00F AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.FDCOAG1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.FDCOAG2
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR",
                        (row, prd, um, age1, age2) =>
                        {
                            row.UMsCache = umsCache;
                            row.Product = prd;
                            row.UM = um;
                            row.FirstAgent = age1;
                            row.SecondAgent = age2;
                            row.PrintAgentsInDetails = PrintAgentsInDetails && (age1 != null || age2 != null);
                            row.CustomerDiscount = result.FTSCCL ?? 0;
                            return row;
                        },
                        new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "SocietaID,SocietaID,agecod,agecod").ToList());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent

                    var agent1 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG1 ?? string.Empty);
                    var agent2 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG2 ?? string.Empty);
                    result.DefaultFirstAgent = agent1;
                    result.DefaultSecondAgent = agent2;
                }

                result.Attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                        @"SELECT * FROM FATTAL00F
                            WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                            ORDER BY FTANAME",
                        new { FTASOCI = ftsoci, FTAANNO = FTANNO, FTANUOR = FTNUOR }).ToList());
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DateTime? CheckLastFinalDate(string ftsoci, int FTANNO, string NumeratorID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.ExecuteScalar<DateTime>(
                @"SELECT MAX(t.FTDAOR) FROM FATTT00F AS t
                        INNER JOIN CAUFAT00F AS c ON c.fatcod=t.FTCAUS
                        WHERE t.ftsoci = @ftsoci AND t.FTANNO = @FTANNO AND t.FTNUFD IS NOT NULL AND t.FTNUFD > 0 AND c.fatnmr=@nid",
                new { ftsoci = ftsoci, FTANNO = FTANNO, @nid = NumeratorID });

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ftsoci, int FTANNO, int FTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FATTT00F WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR",
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public decimal GetInvoicesAmount(string CompanyID, int CustomerID, int? Year, DateTime? ToDate, bool OnlyUnaccounted)
    {
        try
        {
            using var connection = GetOpenConnection();


            string query = @$"SELECT d.*, t.* FROM FATTD00F AS d
                       INNER JOIN FATTT00F AS t ON t.ftsoci = d.ftsoci AND t.FTANNO = d.FTANNO AND t.FTNUOR = d.FTNUOR
                       WHERE t.ftsoci=@ftsoci AND t.FTCODC=@ftcodc {(Year.HasValue ? "AND t.FTANNO = @ftanno" : null)} {(ToDate.HasValue ? "AND t.FTDAOR<=@todate" : null)} {(OnlyUnaccounted ? "AND d.FDFLA2 = ' '" : null)}";

            var list = connection.Query<FATTD00F, FATTT00F, FATTD00F>(
                query,
                (dett, head) => { return dett; },
                new { ftsoci = CompanyID, ftanno = Year.HasValue ? Year.Value : 0, ftcodc = CustomerID, todate = ToDate },
                splitOn: "ftsoci");

            return list.Sum(sum => sum.NetPrice);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public List<FATTT00F>? GetINTRA(string CompanyID, DateTime Period)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<FATTT00F, ABE, CAUFAT00F, FATTT00F>(
                @"select 
ftsoci,
FTANNO,
FTNUOR, 
FTTIPO, 
FTNUFD, 
FTDAOR, 
FTCAUS, 
FTCODC, 
FTCODD, 
FTCONZ, 
FTPAGA,
FTABIB, 
FTCABB, 
FTCONS, 
FTSPED, 
FTCORR, 
FTAREA, 
FTCOAG, 
TRY_CAST(NULLIF(LTRIM(RTRIM(h.FTFILI)), '') AS INT) AS FTFILI,
FTZONA, 
FTSETM, 
FTCIDI, 
FTDE25,  
FTSCMO, 
FTFLA1, 
FTFLA2, 
FTFLA3, 
FTFLA4,
FTFLA5, 
FTFLA6, 
FTFLA7,
FTFLA8,
FTFLA9, 
FTFLA0, 
FTSCOM, 
FTNURI, 
FTDARI, 
FTTRAS,
FTSC1A, FTSC1B, FTSC2A, FTSC2B, FTBOLL, FTTRAS, FTIMBA, FTPOST, FTPTRA, FTPIMB, FTIMBL, 
                         ftciva, FTCAMB, abers5, ftdao2, FTSCAD, FTCLSO, ftling, fteinfo, ftetal, ftannf, ftnufa, ftdafa, abiant, cabant, ccant, ftanbol, FTPortaf, ftagecod2, ftalib, ftassb, ftfirmsped, ftfirm, ftdatfel, ftnumfel,fattmpag,fttdoc
,

c.*,
ca.* from FATTT00F as h
                        INNER JOIN ANAG_BASE AS c ON c.abecod = h.FTCODC
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = h.FTCAUS
                        WHERE h.ftsoci = @ftsoci AND YEAR(h.FTDAOR) = YEAR(@period) AND MONTH(h.FTDAOR) = MONTH(@period) AND ca.fatCEE = 'S' AND ca.fatEXTCEE = 'N' AND ca.fatITA = 'N' AND (ca.fattipo = 'V' OR ca.fattipo ='C')",
                (inv, cus, sel) =>
                {
                    inv.Customer = cus;
                    inv.Causal = sel;
                    return inv;
                },
                new { ftsoci = CompanyID, period = Period }, splitOn: "abecod,fatcod")
                .ToList();

            foreach (var inv in result)
            {
                inv.Rows = new ObservableCollection<FATTD00F>(connection.Query<FATTD00F, tab_articolo, BOLLD00F, BOLLT00F, FATTD00F>(
                        $@"SELECT r.*, p.*, bd.*, bt.* FROM FATTD00F AS r
                            INNER JOIN ANAG_ARTICOLI AS p ON p.artcod = r.FDCODA
                            LEFT OUTER JOIN BOLLD00F AS bd ON r.ftsoci = bd.bolsoc and r.fdbono = bd.btanno and r.fdboll = bd.btboll and r.fdbori = bd.borigb
                            LEFT OUTER JOIN BOLLT00F AS bt ON bd.bolsoc = bt.bolsoc and bd.btanno = bt.btanno and bd.btboll = bt.btboll
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR",
                        (row, prd, bld, blt) =>
                        {
                            row.Product = prd;
                            row.CustomerDiscount = inv.FTSCCL ?? 0;
                            row.LinkedDDT = blt;
                            return row;
                        },
                        new { ftsoci = inv.ftsoci, FTANNO = inv.FTANNO, FTNUOR = inv.FTNUOR }, splitOn: "artcod,bolsoc,bolsoc").ToList());
            }

            return result;


        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region eInvoice
    public string? GenerateInvoiceXML(string CompanyID, int InvoiceYear, int InvoiceNumber, bool IsRegenerate)
    {
        try
        {

            var orditRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>();
            var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();
            var fattalRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTAL00FRepository>();
            var notecliRepo = VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>();
            var abeExtDestRepo = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();

            var caufatRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>();
            var caucontRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>();
            var libriIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>();
            var fattautRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>();

            CultureInfo culture = new CultureInfo("en-US");
            XNamespace p = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.2";
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            //if (false) // TODO semplificata non la invieremo mai
            //{
            //    XNamespace ps = "http://ivaservizi.agenziaentrate.gov.it/docs/xsd/fatture/v1.0";
            //}
            // retrieve invoice
            var invoice = Get(CompanyID, InvoiceYear, InvoiceNumber);

            if (invoice != null)
            {
                invoice.Rows = new ObservableCollection<FATTD00F>(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().GetListWithRate(CompanyID, InvoiceYear, InvoiceNumber) ?? new List<FATTD00F>());

                #region Company infos

                var company = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)!;
                var companyBase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(CompanyID)!;

                string? senderVAT = !string.IsNullOrWhiteSpace(company.azcofi) ? company.azcofi.Trim() : company.azpaiv?.Trim();
                var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, InvoiceYear)!;
                #endregion

                #region Customer infos

                var customerABE = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(invoice.FTCODC ?? 0);
                if (customerABE == null)
                {
                    ErrorHandler.Show("Cliente non trovato");
                    return null;
                }
                if (!customerABE.abecap.HasValue || customerABE.abecap < 0)
                {
                    ErrorHandler.Show("Il c.a.p. del cliente contiene un valore non valido");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(customerABE.abeloc))
                {
                    ErrorHandler.Show("La località del cliente contiene un valore non valido");
                    return null;
                }
                if (string.IsNullOrWhiteSpace(customerABE.abepro))
                {
                    ErrorHandler.Show("La provincia del cliente contiene un valore non valido");
                    return null;
                }
                var client = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(invoice.FTCODC ?? 0);

                if (client == null)
                {
                    ErrorHandler.Show("CLIENTI non trovata");
                    return null;
                }

                var customerISO = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Get(customerABE.isocod ?? string.Empty);
                string intraFlag = "N";
                if (customerABE.isocod != "IT")
                    intraFlag = customerISO?.isointr ?? "N";

                #endregion

                #region Other infos
                var causalInvoice = caufatRepo.Get(invoice.FTCAUS ?? string.Empty);

                var causalAccounting = caucontRepo.Get(causalInvoice?.fatcon ?? string.Empty);

                var ivaBook = libriIvaRepo.Get(causalAccounting?.cauliv ?? string.Empty);

                var customizationSelf = fattautRepo.Get(CompanyID, InvoiceYear, InvoiceNumber);
                #endregion

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var numerator = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, now.Year, Constants.E_INVOICE_GENERAL, true);

                #region Generate file name
                var unixTime = new DateTimeOffset(now).ToUnixTimeMilliseconds();
                var uniqueID = TextHelper.EncodeToBase62(unixTime);
                var neededPre = 10 - uniqueID.Length;
                string fullPath = $"{Path.GetTempPath()}IT{senderVAT}{uniqueID.Substring(0, neededPre).PadLeft(5, '0')}_{uniqueID.Substring(neededPre - 1, 5)}.xml";
                #endregion

                string transmissionFormat = client != null && client.clpaymBool ? "FPA12" : "FPR12";
                string numerazione = invoice.PrintFullID;

                XElement root = new XElement(p + "FatturaElettronica",
                    new XAttribute(XNamespace.Xmlns + "p", p.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "ds", ds.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
                    new XAttribute("versione", transmissionFormat));

                #region Recipient IDs
                string? recipientID;
                string? recipientPEC = null;
                if (customerABE.isocod == "IT")
                {
                    if (client!.clpaymBool)
                    {
                        recipientID = client!.clcouff;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(client.clcoddest))
                        {
                            recipientID = client.clcoddest;
                        }
                        else
                        {
                            recipientID = "0000000";
                            recipientPEC = client.clpec;
                        }
                    }
                }
                else
                {
                    recipientID = company.azcodcee;
                }
                #endregion

                XElement header = new XElement("FatturaElettronicaHeader",
                    new XElement("DatiTrasmissione",
                    new XElement("IdTrasmittente",
                        new XElement("IdPaese", "IT"),
                        new XElement("IdCodice", senderVAT)),
                    new XElement("ProgressivoInvio", $"{now.Year}{numerator.ToString().PadLeft(6, '0')}"),
                    new XElement("FormatoTrasmissione", transmissionFormat),
                    new XElement("CodiceDestinatario", recipientID),
                    new XElement("ContattiTrasmittente", new XElement("Telefono", company.AZTel?.Trim())),
                        (!string.IsNullOrWhiteSpace(recipientPEC) ? new XElement("PECDestinatario", recipientPEC) : null))
                    );

                #region 1.2
                if (invoice.FTTIPO != "A" && invoice.FTTIPO != "C")
                {
                    XElement cedentePrestatore = new XElement("CedentePrestatore",
                        new XElement("DatiAnagrafici",
                            new XElement("IdFiscaleIVA",
                                new XElement("IdPaese", "IT"),
                                new XElement("IdCodice", senderVAT?.Trim())),
                            new XElement("Anagrafica",
                                new XElement("Denominazione", TextHelper.SanitizeFull(companyBase.SOMDES.Trim()))),
                            new XElement("RegimeFiscale", company.azregifatt)),
                        new XElement("Sede",
                            new XElement("Indirizzo", TextHelper.SanitizeFull(company.azinsl?.Trim())),
                            new XElement("CAP", company.azcasl.HasValue ? company.azcasl.Value.ToString().PadLeft(5, '0') : "00000"),
                            new XElement("Comune", TextHelper.SanitizeFull(company.azlosl)),
                            new XElement("Provincia", company.azprsl),
                            new XElement("Nazione", "IT")),
                        new XElement("IscrizioneREA",
                            new XElement("Ufficio", company.azprsl),
                            new XElement("NumeroREA", company.azcodrea?.Trim()),
                            new XElement("CapitaleSociale", company.azcapsoc?.ToString("F2", culture)),
                            new XElement("SocioUnico", company.azsocuni),
                            new XElement("StatoLiquidazione", company.azstaliq)));

                    header.Add(cedentePrestatore);
                }
                else
                {
                    // autofattura
                    var supplier = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(customizationSelf?.FTAUCOF ?? 0);
                    var supplierISO = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().Get(supplier?.isocod ?? string.Empty);
                    XElement cedentePrestatore = new XElement("CedentePrestatore",
                        new XElement("DatiAnagrafici",
                            new XElement("IdFiscaleIVA",
                                new XElement("IdPaese", supplierISO?.isocod3166),
                                new XElement("IdCodice", supplier?.abepiv?.Trim())),
                            new XElement("Anagrafica",
                                new XElement("Denominazione", TextHelper.Truncate(TextHelper.SanitizeFull(supplier?.abers1?.Trim()), 80))),
                            new XElement("RegimeFiscale", company.azregifatt)),
                        new XElement("Sede",
                            new XElement("Indirizzo", TextHelper.Truncate(TextHelper.SanitizeFull(supplier?.abeind.Trim()), 60)),
                            new XElement("CAP", supplier != null && supplier.abecap.HasValue ? supplier.abecap.Value.ToString().PadLeft(5, '0') : "00000"),
                            new XElement("Comune", TextHelper.Truncate(TextHelper.SanitizeFull(supplier?.abeloc), 60)),
                            new XElement("Provincia", supplier?.abepro),
                            new XElement("Nazione", supplierISO?.isocod3166)));

                    header.Add(cedentePrestatore);
                }
                #endregion

                #region 1.4

                XElement? idFiscaleIVA = null;
                XElement? anagrafica = null;
                XElement? codiceFiscale = null;
                if (customerISO?.isocod3166 == "IT" || intraFlag == "S")
                {
                    if (customerABE.abetpo != "2")
                    {
                        idFiscaleIVA = new XElement("IdFiscaleIVA",
                        new XElement("IdPaese", customerISO?.isocod3166),
                        new XElement("IdCodice", customerABE.abepiv));
                        if (customerISO?.isocod3166 == "IT" && !string.IsNullOrWhiteSpace(customerABE.abecfi))
                            codiceFiscale = new XElement("CodiceFiscale", customerABE.abecfi.Trim());
                        anagrafica = new XElement("Anagrafica",
                        new XElement("Denominazione", TextHelper.Truncate($"{TextHelper.SanitizeFull(customerABE.abers1?.Trim())} {TextHelper.SanitizeFull(customerABE.abers2?.Trim())}", 80)));
                    }
                    else
                    {
                        codiceFiscale = new XElement("CodiceFiscale", customerABE.abecfi?.Trim());
                        anagrafica = new XElement("Anagrafica",
                            new XElement("Nome", !string.IsNullOrWhiteSpace(customerABE.abers2) ? TextHelper.SanitizeFull(customerABE.abers2?.Trim()) : TextHelper.SanitizeFull(customerABE.abers1?.Trim())),
                            new XElement("Cognome", TextHelper.SanitizeFull(customerABE.abers1?.Trim())));
                    }
                }
                else
                {
                    idFiscaleIVA = new XElement("IdFiscaleIVA",
                        new XElement("IdPaese", customerISO?.isocod3166),
                        new XElement("IdCodice", $"{company.azisoextracee?.Trim()} {company.azcodextracee?.Trim()}"));
                    anagrafica = new XElement("Anagrafica",
                        new XElement("Denominazione", TextHelper.Truncate($"{TextHelper.SanitizeFull(customerABE.abers1?.Trim())} {TextHelper.SanitizeFull(customerABE.abers2?.Trim())}", 80)));
                }

                XElement cessionarioCommittente = new XElement("CessionarioCommittente",
                    new XElement("DatiAnagrafici",
                        idFiscaleIVA,
                        codiceFiscale,
                        anagrafica),
                    new XElement("Sede",
                        new XElement("Indirizzo", TextHelper.Truncate(TextHelper.SanitizeFull(customerABE.abeind.Trim()), 60)),
                        new XElement("CAP", (customerABE.abecap.HasValue && customerABE.abecap.Value > 0 ? customerABE.abecap?.ToString().PadLeft(5, '0') : "00000")),
                        new XElement("Comune", TextHelper.Truncate(TextHelper.SanitizeFull(customerABE.abeloc.Trim()), 60)),
                        (!string.IsNullOrWhiteSpace(customerABE.abepro) ? new XElement("Provincia", customerABE.abepro.Trim()) : null),
                        new XElement("Nazione", customerISO?.isocod3166)));

                header.Add(cessionarioCommittente);
                #endregion

                if (company.azuseei)
                {
                    #region 1.5 Intermediario

                    XElement intermediario = new XElement("TerzoIntermediarioOSoggettoEmittente",
                        new XElement("DatiAnagrafici",
                            new XElement("IdFiscaleIVA",
                                new XElement("IdPaese", "IT"),
                                new XElement("IdCodice", "07837340962")),
                            new XElement("CodiceFiscale", "07837340962"),
                            new XElement("Anagrafica",
                                new XElement("Denominazione", "CERBER Tech S.r.l."))));

                    header.Add(intermediario);

                    #endregion
                }

                if (invoice.FTTIPO == "A")
                {
                    // se autofattura aggiunge CC
                    header.Add(new XElement("SoggettoEmittente", "CC"));
                }

                root.Add(header);

                DateTime data = invoice.FTDAOR ?? DateTime.MinValue;
                XElement body = new XElement("FatturaElettronicaBody");

                var dichiarazioneIntento = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().GetLast(CompanyID, invoice.FTCODC ?? 0, invoice.FTANNO, invoice.FTDAOR ?? DateTime.MinValue, IsRegenerate);
                string? causalDecription = null;
                if (dichiarazioneIntento != null && !string.IsNullOrWhiteSpace(dichiarazioneIntento.clinumprotuffiva))
                {
                    causalDecription = $"{TextHelper.SanitizeFull(causalInvoice?.fatdes.Trim())} Ricevuta dich. intento prot.n. {dichiarazioneIntento.clinumprotuffiva.Trim()}";
                }
                else
                {
                    causalDecription = causalInvoice?.fatdes.Trim();
                }

                // bollo
                XElement? bollo = null;
                var bolloProductRow = invoice.Rows.Where(w => w.FDRIGA == 999999).FirstOrDefault();
                if (bolloProductRow != null)
                {
                    bollo = new XElement("DatiBollo",
                    new XElement("BolloVirtuale", "SI"),
                    new XElement("ImportoBollo", Math.Round(bolloProductRow.Amount, 2).ToString("F2", culture)));
                }

                XElement datiGenerali = new XElement("DatiGenerali");
                XElement datiGeneraliDocumento = new XElement("DatiGeneraliDocumento",
                            new XElement("TipoDocumento", invoice.fttdoc?.Trim()),
                            new XElement("Divisa", invoice.ftciva?.Trim()),
                            new XElement("Data", data.ToString("yyyy-MM-dd")),
                            new XElement("Numero", numerazione),
                            bollo,
                            new XElement("ImportoTotaleDocumento", invoice.GrandTotal.ToString("F2", culture)),
                            new XElement("Causale", causalDecription));

                datiGenerali.Add(datiGeneraliDocumento);

                // 2.1.2 Dati ordine di acquisto
                foreach (var row in invoice.Rows.Where(w => w.OTANN1.HasValue && w.OTANN1.Value > 0 && w.OTNUO1.HasValue && w.OTNUO1.Value > 0 && w.ODRIG1.HasValue && w.ODRIG1.Value > 0)
                                                .GroupBy(g => new { g.OTANN1, g.OTNUO1, g.ODRIG1 }, (key, items) => new { key, items }))
                {
                    var refLines = new List<XElement>();
                    foreach (var inv in row.items)
                    {
                        refLines.Add(new XElement("RiferimentoNumeroLinea", inv.FDRIGA));
                    }
                    var order = orditRepo.Get(invoice.ftsoci, row.key.OTANN1 ?? 0, row.key.OTNUO1 ?? 0);
                    if (order != null && !string.IsNullOrWhiteSpace(order.OTCUNO) && order.OTCUDO.HasValue)
                    {
                        datiGenerali.Add(new XElement("DatiOrdineAcquisto",
                            refLines,
                            new XElement("IdDocumento", order.OTCUNO.Trim().Length > 20 ? order.OTCUNO.Trim().Substring(0, 20) : order.OTCUNO.Trim()),
                            new XElement("Data", order.OTCUDO.Value.ToString("yyyy-MM-dd")),
                            new XElement("NumItem", row.key.ODRIG1 ?? 0))); // questa è la riga del nostro ODA 
                    }
                }

                // 2.1.6 DatiFattureCollegate
                if (invoice.FTTIPO == "A")
                {
                    if (!string.IsNullOrWhiteSpace(customizationSelf?.FTAUINDSDI))
                    {
                        // add DatiFattureCollegate
                        datiGenerali.Add(new XElement("DatiFattureCollegate",
                            new XElement("IdDocumento", customizationSelf?.FTAUINDSDI),
                            new XElement("Data", (customizationSelf?.FTAUDATRIC ?? DateTime.MinValue).ToString("yyyy-MM-dd"))));
                    }
                }
                else
                {
                    if (invoice.ftannf.HasValue)
                    {
                        // add DatiFattureCollegate
                        datiGenerali.Add(new XElement("DatiFattureCollegate",
                            new XElement("IdDocumento", (invoice.ftnufa ?? 0).ToString().PadLeft(6, '0')),
                            new XElement("Data", (invoice.ftdafa ?? DateTime.MinValue).ToString("yyyy-MM-dd"))));
                    }
                }

                // 2.1.8 Dati DDT
                var ddtList = invoice.Rows.Where(w => w.FDBOLL.HasValue && w.FDBOLL.Value > 0).Select(s => $"{s.FDBONO}{s.FDBOLL}").Distinct().ToList().ToArray<string>();
                var neededDDTs = bolltRepo.GetFromList(CompanyID, ddtList);
                foreach (var row in invoice.Rows.Where(w => w.FDBOLL.HasValue && w.FDBOLL.Value > 0))
                {
                    var ddt = neededDDTs?.Where(w => w.BTANNO == row.FDBONO && w.BTBOLL == row.FDBOLL).First();
                    datiGenerali.Add(new XElement("DatiDDT",
                        new XElement("NumeroDDT", ddt?.DefinitiveID),
                        new XElement("DataDDT", (ddt?.BTDATA ?? DateTime.MinValue).ToString("yyyy-MM-dd")),
                        new XElement("RiferimentoNumeroLinea", row.FDRIGA)));
                }

                // 2.1.9 Dati trasporto
                if ((invoice.FTCODD ?? 0) > 0)
                {
                    var destinatario = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().Get(invoice.FTCODC ?? 0, invoice.FTCODD ?? 0);

                    if (destinatario != null)
                    {
                        XElement datiTrasporto = new XElement("DatiTrasporto");
                        XElement datiDestinatario = new XElement("IndirizzoResa",
                               new XElement("Indirizzo", destinatario.DEINDI?.Trim()),
                                 new XElement("NumeroCivico", "1"),
                                 new XElement("CAP", destinatario.DECAPText),
                                 new XElement("Comune", destinatario.deloc),
                                 new XElement("Provincia", destinatario.depro),
                                 new XElement("Nazione", destinatario.isocod));
                        datiTrasporto.Add(datiDestinatario);

                        datiGenerali.Add(datiTrasporto);
                    }
                }

                body.Add(datiGenerali);

                // righe
                var datiBeniServizi = new XElement("DatiBeniServizi");
                int rowsCount = 1;

                foreach (var row in invoice.Rows.Where(w => w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)).OrderBy(o => o.FDRIGA))
                {
                    #region Product description
                    string? productDescription = row.Product?.Descrizione;
                    // note testata fattura
                    if (rowsCount == 1 && !string.IsNullOrWhiteSpace(invoice.FTNOTET) && invoice.FTSHOWT)
                        productDescription += $" {invoice.FTNOTET.Trim()}";
                    // note articolo
                    if (company.azpnotinv)
                    {
                        productDescription += $" {row.Product?.Note}";
                    }
                    // note riga
                    if (!string.IsNullOrWhiteSpace(row.FDNOTE) && row.FDSHOW)
                        productDescription += $" {row.FDNOTE.Trim()}";
                    // note bolla cliente
                    if (row.FDBONO.HasValue && row.FDBOLL.HasValue)
                    {
                        var ddt = bolltRepo.Get(CompanyID, row.FDBONO.Value, row.FDBOLL.Value);
                        if (ddt != null)
                        {
                            productDescription += $" {ddt.BTNOTET?.Trim()}";
                        }
                    }
                    // note pie' di pagina fattura e note cliente
                    if (rowsCount == invoice.Rows.Count())
                    {
                        if (!string.IsNullOrWhiteSpace(invoice.FTNOTEP) && invoice.FTSHOWP)
                            productDescription += $" {invoice.FTNOTEP?.Trim()}";

                        var customerNotes = notecliRepo.GetListForInvoices(invoice.FTCODC ?? 0);
                        if (customerNotes != null && customerNotes.Count > 0)
                        {
                            foreach (var note in customerNotes)
                            {
                                productDescription += $" {note.notdes?.Trim()}";
                            }
                        }
                    }
                    // sanitize product description
                    productDescription = productDescription?.Replace("€", "euro");
                    if (productDescription?.Length > 1000)
                        productDescription = productDescription?.Substring(0, 1000);
                    productDescription = TextHelper.SanitizeFull(productDescription);
                    #endregion

                    decimal quantity = row.FDQTAV ?? 0;
                    XElement? um = null;
                    if (!string.IsNullOrWhiteSpace(row.FDUNIM))
                    {
                        um = new XElement("UnitaMisura", row.FDUNIM.Trim());
                    }

                    #region Sconti e maggiorazioni

                    XElement? discount1 = null;
                    if (row.FDSCO1.HasValue && row.FDSCO1.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDSCO1.Value;
                        if (row.FDTSC1 == "P")
                            ItemName = "Percentuale";
                        discount1 = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "SC"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    XElement? discount2 = null;
                    if (row.FDSCO2.HasValue && row.FDSCO2.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDSCO2.Value;
                        if (row.FDTSC2 == "P")
                            ItemName = "Percentuale";
                        discount2 = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "SC"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    XElement? discount3 = null;
                    if (row.FDSCO3.HasValue && row.FDSCO3.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDSCO3.Value;
                        if (row.FDTSC3 == "P")
                            ItemName = "Percentuale";
                        discount3 = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "SC"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    XElement? maggiorazione = null;
                    if (row.FDMAGG.HasValue && row.FDMAGG.Value > 0)
                    {
                        string ItemName = "Importo";
                        decimal ItemValue = row.FDMAGG.Value;
                        if (row.FDTMAG == "P")
                            ItemName = "Percentuale";
                        maggiorazione = new XElement("ScontoMaggiorazione",
                            new XElement("Tipo", "MG"),
                            new XElement(ItemName, ItemValue.ToString("F2", culture)));
                    }
                    #endregion

                    XElement? natura = null;
                    decimal rateNatureValue = 0;
                    if (!decimal.TryParse(row.FDALIV?.Trim(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out rateNatureValue))
                        natura = new XElement("Natura", row.fdtiva?.Trim());

                    #region AltriDatiGestionali
                    List<XElement> altri = new List<XElement>();
                    // plafond info
                    if (row.Rate?.asspla == "S")
                    {
                        altri.Add(new XElement("AltriDatiGestionali",
                            new XElement("TipoDato", "INTENTO"),
                            new XElement("RiferimentoTesto", dichiarazioneIntento?.clinumprotuffiva?.Trim()),
                            new XElement("RiferimentoData", (dichiarazioneIntento?.clidatuffiva ?? DateTime.MinValue).ToString("yyyy-MM-dd"))));
                    }
                    // recipient decoding
                    if (invoice.FTCODD.HasValue)
                    {
                        var extRecipients = abeExtDestRepo.GeteInvoiceRecipientList(invoice.FTCODC ?? 0, invoice.FTCODD ?? 0);
                        if (extRecipients != null && extRecipients.Count > 0)
                        {
                            foreach (var ext in extRecipients)
                            {
                                altri.Add(new XElement("AltriDatiGestionali",
                                    new XElement("TipoDato", ext.abeextcode.Split('#')[1]),
                                    new XElement("RiferimentoTesto", ext.abeextdid)));
                            }
                        }
                    }
                    #endregion

                    decimal rateRowValue = 0;
                    if (!decimal.TryParse(row.FDALIV?.Trim(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out rateRowValue))
                        rateRowValue = 0;
                    var newLine = new XElement("DettaglioLinee",
                        new XElement("NumeroLinea", row.FDRIGA != 999999 ? row.FDRIGA : invoice.Rows.Where(w => w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)).Count()),
                        (row.FDTQTA == "O" ? new XElement("TipoCessionePrestazione", "SC") : null),
                            new XElement("CodiceArticolo",
                                new XElement("CodiceTipo", "CPV"),
                                new XElement("CodiceValore", row.FDCODA?.Trim())),
                                new XElement("Descrizione", productDescription),
                                new XElement("Quantita", quantity.ToString("F6", culture)),
                                um,
                                new XElement("PrezzoUnitario", row.FDTQTA == "V" || row.FDTQTA == "B" || row.FDTQTA == "O" ? (row.FDTPRE == "U" ? (row.FDPREZ ?? 0).ToString("F6", culture) : Math.Round((row.FDPREZ ?? 0) / (row.FDQTAV ?? 1), 2, MidpointRounding.AwayFromZero).ToString("F6", culture)) : (row.FDTPRE == "U" ? ((row.FDPREZ ?? 0) * -1).ToString("F6", culture) : Math.Round((((row.FDPREZ ?? 0) / (row.FDQTAV ?? 1)) * -1), 2, MidpointRounding.AwayFromZero).ToString("F6", culture))),
                                discount1,
                                discount2,
                                discount3,
                                maggiorazione,
                                new XElement("PrezzoTotale", row.NetPrice.ToString("F2", culture)),
                                new XElement("AliquotaIVA", rateRowValue.ToString("F2", culture)),
                                natura);
                    if (altri != null && altri.Count > 0)
                    {
                        newLine.Add(altri);
                    }
                    datiBeniServizi.Add(newLine);
                    rowsCount += 1;
                }

                #region Dati riepilogo

                foreach (var aliq in invoice.Rows.GroupBy(g => new { g.FDALIV, g.FDASSF }, (key, items) => new { key, items }))
                {
                    XElement? riepNatura = null;
                    XElement? riepRife = null;
                    XElement? riepEsig = null;

                    if (aliq.items.First().Rate?.asssplpayBool ?? false)
                    {
                        riepEsig = new XElement("EsigibilitaIVA", "S");
                    }
                    else
                    {
                        if (esercizio.eseivavenBool)
                            riepEsig = new XElement("EsigibilitaIVA", "D");
                        else
                            riepEsig = new XElement("EsigibilitaIVA", "I");
                    }

                    var taxable = aliq.items.Sum(sum => sum.NetPrice);
                    decimal taxes = 0;
                    decimal ratePerc = 0;
                    if (decimal.TryParse(aliq.key.FDALIV?.Trim(), System.Globalization.NumberStyles.Number, CultureInfo.CurrentCulture, out ratePerc))
                    {
                        taxes = Math.Round(taxable * ratePerc / 100, 2, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        taxes = 0;
                        riepNatura = new XElement("Natura", aliq.items.First().Rate?.assnatufe?.Trim());
                        var nature = VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().Get(aliq.items.First().Rate?.assnatufe ?? string.Empty);
                        riepRife = new XElement("RiferimentoNormativo", $"{nature?.FETIDes.Trim()}");
                    }


                    var datiRiepilogo = new XElement("DatiRiepilogo",
                    new XElement("AliquotaIVA", ratePerc.ToString("F2", culture)),
                    riepNatura,
                    new XElement("ImponibileImporto", taxable.ToString("F2", culture)),
                    new XElement("Imposta", taxes.ToString("F2", culture)),
                    riepEsig,
                    riepRife);

                    datiBeniServizi.Add(datiRiepilogo);
                }
                #endregion

                body.Add(datiBeniServizi);

                #region Dati pagamento

                XElement datiPagamento = new XElement("DatiPagamento");
                var expires = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().ComputeExpires(CompanyID, invoice.FTPAGA ?? string.Empty, invoice.FTDAOR ?? DateTime.MinValue, invoice.FTCODC ?? 0);
                var condizioniPagamento = new XElement("CondizioniPagamento", expires?.Count == 1 ? "TP02" : "TP01");
                var paymentFull = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetFull(invoice.FTPAGA ?? string.Empty);
                decimal stepAmount = Math.Round(invoice.GrandTotal / (expires?.Count ?? 1), 2);
                decimal finalStepAmount = Math.Round((stepAmount + (invoice.GrandTotal - (stepAmount * (expires?.Count ?? 1)))), 2);
                ABICAB? bank = null;
                BANAZIEN? companyBank = null;
                if (invoice.FTABIB.HasValue && invoice.FTABIB.Value > 0 &&
                    invoice.FTCABB.HasValue && invoice.FTCABB.Value > 0)
                {
                    bank = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(invoice.FTABIB.Value, invoice.FTCABB.Value);
                    companyBank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(invoice.ftsoci, invoice.FTABIB.Value, invoice.FTCABB.Value, invoice.FTBCON ?? string.Empty);
                }
                datiPagamento.Add(condizioniPagamento);
                int currentRow = 1;
                foreach (var exp in expires ?? new List<DateTime>())
                {
                    datiPagamento.Add(new XElement("DettaglioPagamento",
                        new XElement("ModalitaPagamento", paymentFull?.Incasso?.icsfepacod),
                        new XElement("DataScadenzaPagamento", exp.ToString("yyyy-MM-dd")),
                        new XElement("ImportoPagamento", currentRow == expires?.Count ? finalStepAmount : stepAmount),
                        bank != null ? new XElement("IstitutoFinanziario", $"{TextHelper.SanitizeFull(bank.abiban.Trim())} {TextHelper.SanitizeFull(bank.abiage?.Trim())}") : null,
                        bank != null && companyBank != null ? new XElement("IBAN", companyBank.abibiba) : null,
                        bank != null ? new XElement("ABI", bank.abiabi.ToString().PadLeft(5, '0')) : null,
                        bank != null ? new XElement("CAB", bank.abicab.ToString().PadLeft(5, '0')) : null));

                    currentRow += 1;
                }

                body.Add(datiPagamento);
                #endregion

                #region Allegati

                List<string> compressedExtensions = new List<string>() {
                "zip", "rar", "br", "bz2", "gz", "lz", "lz4", "lzma", "lzo", "lz", "sfark", "sz",
                "xz", "z", "zst", "7z", "s7z", "ace", "afa", "apk", "arj", "cab", "dmg", "gca",
                "ice", "jar", "lza", "lzh", "lzx", "pak", "pim", "sfx", "shk", "sit", "sitx",
                "sqx", "tar.gz", "tgz", "tar.Z", "tar.bz2", "tbz2", "tar.lz", "tlz", "tar.xz",
                "txz", "tar.zst", "tar", "war", "wim", "xar", "xp3", "xz1", "zipx", "zoo", "zpaq", "zz"};

                foreach (var attach in fattalRepo.GetList(CompanyID, InvoiceYear, InvoiceNumber) ?? new ObservableCollection<FATTAL00F>())
                {
                    string attachmentName = attach.FTANAME.Trim();
                    if (attachmentName.Length > 60)
                        attachmentName = attachmentName.Substring(0, 60);
                    string attachmentDescription = $"{attachmentName} {attach.FTASIZE} bytes";
                    if (attachmentDescription.Length > 100)
                        attachmentDescription = attachmentDescription.Substring(0, 100);
                    string attachmentFormat = Path.GetExtension(attach.FTANAME.Trim()).Trim().ToUpper();
                    attachmentFormat = attachmentFormat.Substring(1, attachmentFormat.Length - 1);
                    if (attachmentFormat.Length > 10)
                        attachmentFormat = attachmentFormat.Substring(0, 10);

                    XElement? algoritmoCompressione = null;
                    if (compressedExtensions.Contains(attachmentFormat.ToLower()))
                    {
                        algoritmoCompressione = new XElement("AlgoritmoCompressione", attachmentFormat);
                    }

                    byte[]? attachmentData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER,
                        $"{companyBase.SOCUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{attach.FTAUID}");

                    if (attachmentData != null)
                    {
                        body.Add(new XElement("Allegati",
                            new XElement("NomeAttachment", TextHelper.SanitizeFull(attachmentName)),
                            new XElement("FormatoAttachment", attachmentFormat),
                            new XElement("DescrizioneAttachment", TextHelper.SanitizeFull(attachmentDescription)),
                            algoritmoCompressione,
                            new XElement("Attachment", System.Convert.ToBase64String(attachmentData))));
                    }
                }

                #endregion

                root.Add(body);
                root.Save(fullPath);
                return fullPath;
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    #region Accounting
    public bool Accounting(FATTT00F Invoice, ESERCIZIO AccountingYear, DateTime AccountingDate, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var pnClientiRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>();
                var pnPortafoglioRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>();
                var pnIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>();
                var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var plafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var plafondRowsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var plafondParmsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var tipiIncRepo = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>();
                // get registration number
                var accountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Invoice.ftsoci, AccountingYear.eseann, Constants.PN, true);
                // self invoice info
                FATTAUT? fattaut = null;
                FORNAMMI? fornammi = null;
                PDCSOTTO? pdcSottoSupplier = null;
                if (Invoice.FTTIPO == "A" || Invoice.FTTIPO == "C")
                {
                    fattaut = VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().Get(Invoice.ftsoci, Invoice.FTANNO, Invoice.FTNUOR);
                    fornammi = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(Invoice.ftsoci, fattaut?.FTAUCOF ?? 0);
                    pdcSottoSupplier = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(fornammi?.foGRUP ?? string.Empty, fornammi?.foCONT ?? string.Empty, "F", Invoice.ftsoci);
                }
                // caucont from caufat
                var caufatt = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Get(Invoice.FTCAUS ?? string.Empty);
                Invoice.Causal = caufatt;
                var caucont = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Get(caufatt?.fatcon ?? string.Empty);
                // CLIAMMI
                var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Invoice.ftsoci, Invoice.FTCODC ?? 0);
                if (cliammi == null)
                {
                    ErrorHandler.Show($"Non trovati i dati amministrativi del cliente");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(cliammi.clGRUP) || string.IsNullOrWhiteSpace(cliammi.clcont))
                {
                    ErrorHandler.Show($"Manca il conto contabile del cliente");
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(cliammi.clGRUP, cliammi.clcont, "C", Invoice.ftsoci);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show($"Non trovato il sottoconto cliente nel gruppo {cliammi.clGRUP} e conto {cliammi.clcont}");
                    return false;
                }
                // IVA book
                var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(caucont?.cauliv ?? string.Empty);
                // CAUCONT_GROUPS
                var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(Invoice.ftsoci, caucont?.caucod ?? string.Empty);
                // PAGCLI/TAB_ACC_TIPINC
                var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(Invoice.FTPAGA ?? string.Empty);
                if (pagcli == null)
                {
                    ErrorHandler.Show($"Non trovato il codice pagamento sulla fattura");
                    return false;
                }
                var incassi = tipiIncRepo.Get(pagcli.pcltip ?? string.Empty);
                if (incassi == null)
                {
                    ErrorHandler.Show($"Non trovato il tipo pagamento sul codice pagamento {pagcli.FullDescriptionSearchable}");
                    return false;
                }
                // Expires
                var expires = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().ComputeExpires(Invoice.ftsoci, Invoice.FTPAGA ?? string.Empty, Invoice.FTDAOR ?? DateTime.MinValue, Invoice.FTCODC ?? 0);
                // invoice vs credit parms
                string? customerRowSign = null;
                string? otherRowsSign = null;
                string? ivaSign = null;
                if (Invoice.FTTIPO == "F" || Invoice.FTTIPO == "A" || Invoice.FTTIPO == "B")
                {
                    customerRowSign = "D";
                    otherRowsSign = "A";
                    ivaSign = "+";
                }
                else
                {
                    customerRowSign = "A";
                    otherRowsSign = "D";
                    ivaSign = "-";
                }
                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = Invoice.ftsoci,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = accountingID,
                    pncaus = caucont?.caucod,
                    N1DARE = AccountingDate.Date,
                    N1docn = Invoice.FTNUFD.ToString(),
                    N1docd = Invoice.FTDAOR,
                    N1rifn = Invoice.PrintFullID,
                    N1rifd = Invoice.FTDAOR,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Invoice.FTCODC,
                    N1FLCF = "C",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                if (!caucont?.causolBool ?? true)
                {
                    #region Customer row
                    var customerRows = new List<PNRIGHE>();
                    PNRIGHE customerRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = head.N1rifn,
                        N1DARI = head.N1rifd,
                        n1clie = Invoice.FTCODC,
                        N1SEGN = customerRowSign,
                        pngrup = pdcSotto.P1GRUP,
                        pncont = pdcSotto.P2CONT,
                        pnsott = pdcSotto.P3SOTC,
                        N1IMEU = Invoice.GrandTotalWithGift,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = Invoice.FTPAGA,
                        n1scad = Invoice.FTSCAD,
                        N1DRri = head.N1docd
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    customerRows.Add(customerRow);
                    // GIFT
                    if ((Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Any(any => any.FDTQTA == "O"))
                    {
                        PNRIGHE customerGiftRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            n1clie = Invoice.FTCODC,
                            N1SEGN = otherRowsSign,
                            pngrup = pdcSotto.P1GRUP,
                            pncont = pdcSotto.P2CONT,
                            pnsott = pdcSotto.P3SOTC,
                            N1IMEU = Invoice.TotalGifts,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            n1paga = Invoice.FTPAGA,
                            n1scad = Invoice.FTSCAD,
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, customerGiftRow, transaction);
                        customerRows.Add(customerGiftRow);
                    }
                    #endregion

                    #region IVA row
                    if (Invoice.TotalVAT > 0)
                    {
                        foreach (var cg in grpcau ?? new ObservableCollection<CAUCONT_GROUPS>())
                        {
                            PNRIGHE ivaRow = new PNRIGHE()
                            {
                                N1SOCI = head.N1SOCI,
                                N1ANNO = head.N1ANNO,
                                N1REGI = head.N1REGI,
                                N1RIGA = rowsCounter++,
                                N1DOCU = head.N1docn,
                                N1DADO = head.N1docd,
                                N1RIFE = head.N1rifn,
                                N1DARI = head.N1rifd,
                                N1SEGN = cg.grpseg,
                                pngrup = cg.grpgrp,
                                pncont = cg.grpcto,
                                pnsott = cg.grpsct,
                                N1IMEU = grpcau?.Count == 1 ? Invoice.TotalVAT : 0,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DRri = head.N1docd,
                                N1tmpPNR = "N",
                                N1DESC = Invoice.Customer?.FullDescriptionSearchable
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, ivaRow, transaction);
                        }
                    }
                    #endregion

                    #region Ricavi rows
                    List<AccountingAccount> accounts = new List<AccountingAccount>();
                    foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1))
                        .OrderBy(o => o.FDGRUP).ThenBy(o => o.FDCONT).ThenBy(o => o.FDSCTO).ThenBy(o => o.FDCECO))
                    {
                        var newAccount = new AccountingAccount() { GroupID = row.FDGRUP, AccountID = row.FDCONT, SubaccountID = row.FDSCTO, CostCenter = row.FDCECO, ProductCostCenter = row.Product?.costcenter_id };
                        if (!accounts.Contains(newAccount))
                        {
                            accounts.Add(newAccount);
                        }
                    }
                    foreach (var acc in accounts)
                    {
                        var netAmount = (Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                            .Where(w => w.FDGRUP == acc.GroupID && w.FDCONT == acc.AccountID && w.FDSCTO == acc.SubaccountID && w.FDCECO == acc.CostCenter &&
                            (w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)))
                            .Sum(sum => sum.NetPrice);
                        PNRIGHE newRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = otherRowsSign,
                            pngrup = acc.GroupID,
                            pncont = acc.AccountID,
                            pnsott = acc.SubaccountID,
                            N1CCCC = !string.IsNullOrWhiteSpace(acc.CostCenter) ? acc.CostCenter : (!string.IsNullOrWhiteSpace(caucont?.cauceco) ? caucont.cauceco : acc.ProductCostCenter),
                            N1IMEU = netAmount,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                    }
                    // customer discount
                    if (Invoice.ScontiCliente > 0)
                    {
                        // get custom discount account
                        var discAccount = aziendaRepo.Get(Invoice.ftsoci);
                        bool daExists = !string.IsNullOrWhiteSpace(discAccount?.azdisgrp) && !string.IsNullOrWhiteSpace(discAccount.azdiscnt) && !string.IsNullOrWhiteSpace(discAccount.azdissot);
                        // add discount row
                        PNRIGHE discountRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = customerRowSign,
                            pngrup = daExists ? discAccount?.azdisgrp : accounts.First().GroupID,
                            pncont = daExists ? discAccount?.azdiscnt : accounts.First().AccountID,
                            pnsott = daExists ? discAccount?.azdissot : accounts.First().SubaccountID,
                            N1CCCC = Invoice.FTCECO,
                            N1IMEU = Invoice.ScontiCliente,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, discountRow, transaction);
                    }
                    // GIFT
                    if ((Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Any(any => any.FDTQTA == "O"))
                    {
                        // get custom discount account
                        var discAccount = aziendaRepo.Get(Invoice.ftsoci);
                        bool daExists = !string.IsNullOrWhiteSpace(discAccount?.azdisgrp) && !string.IsNullOrWhiteSpace(discAccount.azdiscnt) && !string.IsNullOrWhiteSpace(discAccount.azdissot);
                        // add discount row
                        PNRIGHE giftRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = customerRowSign,
                            pngrup = daExists ? discAccount?.azdisgrp : accounts.First().GroupID,
                            pncont = daExists ? discAccount?.azdiscnt : accounts.First().AccountID,
                            pnsott = daExists ? discAccount?.azdissot : accounts.First().SubaccountID,
                            N1CCCC = Invoice.FTCECO,
                            N1IMEU = Invoice.TotalGifts,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, giftRow, transaction);
                    }
                    #endregion

                    #region PNCLIENTI
                    int customerRowsCounter = 1;
                    foreach (var cliRow in customerRows)
                    {
                        decimal stepAmount = Math.Round((cliRow.N1IMEU ?? 0) / (expires?.Count ?? 1), 2);
                        decimal finalStepAmount = Math.Round((stepAmount + ((cliRow.N1IMEU ?? 0) - (stepAmount * (expires?.Count ?? 1)))), 2);
                        foreach (var exp in expires ?? new List<DateTime>())
                        {
                            PNCLIENTI customer = new PNCLIENTI()
                            {
                                N2SOCI = head.N1SOCI,
                                N2ANNO = head.N1ANNO,
                                N2REGI = head.N1REGI,
                                N2RIGA = customerRowsCounter++,
                                N2DARI = head.N1rifd,
                                N2RIFE = head.N1rifn,
                                N2DOCU = head.N1docn,
                                N2DADO = head.N1docd,
                                N2DARE = AccountingDate.Date,
                                N2CAUS = head.pncaus,
                                N2GRUP = pdcSotto.P1GRUP,
                                N2CONT = pdcSotto.P2CONT,
                                N2SOTT = Invoice.FTCODC,
                                N2SSOC = head.N1SOCI,
                                N2SEGN = cliRow.N1SEGN,
                                N2PAGA = Invoice.FTPAGA,
                                N2SCAD = exp,
                                N2DIVI = "EUR",
                                n2vcod = "UIC",
                                N2DIDO = "EUR",
                                N2VADO = "UIC",
                                N2TIDO = "E",
                                N2IMEU = customerRowsCounter == expires?.Count ? finalStepAmount : stepAmount,
                                n2rior = cliRow.N1RIGA,
                                n2tipi = pagcli?.pcltip
                            };
                            connection.Execute(pnClientiRepo.INSERT_QUERY, customer, transaction);
                        }
                    }
                    #endregion

                    #region PNPORTAFOGLIO
                    if (incassi != null && incassi.icssup == "R")
                    {
                        int portafRowCounter = 1;
                        int portafQuoteCounter = 1;
                        decimal stepAmountP = Math.Round(Invoice.GrandTotal / (expires?.Count ?? 1), 2);
                        decimal finalStepAmountP = Math.Round((stepAmountP + (Invoice.GrandTotal - (stepAmountP * (expires?.Count ?? 1)))), 2);
                        foreach (var exp in expires ?? new List<DateTime>())
                        {
                            PNPORTAFOGLIO portaf = new PNPORTAFOGLIO()
                            {
                                N6SOCI = head.N1SOCI,
                                N6ANNO = head.N1ANNO,
                                N6REGI = head.N1REGI,
                                N6RIGA = portafRowCounter++,
                                N6DARI = head.N1rifd,
                                N6RIFE = head.N1rifn,
                                N6DOCU = head.N1docn,
                                N6DADO = head.N1docd,
                                N6DARE = AccountingDate.Date,
                                N6CAUS = head.pncaus,
                                N6GRUP = pdcSotto.P1GRUP,
                                N6CONT = pdcSotto.P2CONT,
                                N6SOTT = Invoice.FTCODC,
                                N6SCAD = exp,
                                N6SEGN = customerRowSign,
                                N6RATA = portafQuoteCounter++,
                                N6CABI = Invoice.FTABIB.HasValue ? Invoice.FTABIB.Value : cliammi.CLABI,
                                N6CCAB = Invoice.FTCABB.HasValue ? Invoice.FTCABB.Value : cliammi.CLCAB,
                                N6COVA = "EUR",
                                N6IMEU = customerRowsCounter == expires?.Count ? finalStepAmountP : stepAmountP,
                                N6TIPODOC = string.Empty,
                                N6STATO = "N"
                            };
                            connection.Execute(pnPortafoglioRepo.INSERT_QUERY, portaf, transaction);
                        }
                    }
                    #endregion

                    #region Plafond
                    var plafondTotal = (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.HasPlafond).Sum(sum => sum.NetPrice);
                    if (plafondTotal > 0)
                    {
                        var plafondSettings = plafondParmsRepo.Get(Invoice.ftsoci);
                        if (plafondTotal > plafondSettings?.limit_amount)
                        {
                            var plafond = plafondRepo.GetLast(Invoice.ftsoci, Invoice.FTCODC ?? 0, Invoice.FTANNO, Invoice.FTDAOR ?? DateTime.MinValue, false);

                            if (plafond != null)
                            {
                                // update plafond counters
                                plafond.cliimpfattprovv -= plafondTotal;
                                plafond.cliimpfattprog += plafondTotal;
                                connection.Execute(plafondRepo.UPDATE_QUERY, plafond, transaction);

                                // add detail
                                var detail = new ACC_PLAFOND_ROWS()
                                {
                                    Cliasoc = Invoice.ftsoci,
                                    Cliacod = Invoice.FTCODC ?? 0,
                                    cliannosol = Invoice.FTANNO,
                                    cliprog = plafond.cliprog,
                                    clinumfat = Invoice.FTNUOR.ToString(),
                                    clidatfatt = Invoice.FTDAOR ?? DateTime.MinValue,
                                    cliimpplaf = plafondTotal,
                                    cliimpimpo = plafondTotal + (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDRIGA == 999999).FirstOrDefault()?.Amount
                                };
                                connection.Execute(plafondRowsRepo.INSERT_QUERY, detail, transaction);
                            }
                        }
                    }
                    #endregion


                    #region PNIVA
                    int ivaRowsCounter = 1;
                    List<string> rates = new List<string>();
                    var IVAs = new List<PNIVA>();
                    foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).OrderBy(o => o.FDASSF).ThenBy(o => o.FDALIV))
                    {
                        if (!rates.Contains(row.FDASSF + row.FDALIV?.Trim()))
                        {
                            rates.Add(row.FDASSF + row.FDALIV?.Trim());
                        }
                    }

                    if (!AccountingYear.eseivavenBool)
                    {
                        // NO IVA per cassa
                        foreach (var rate in rates)
                        {
                            var imponibile = (Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                                .Where(w => w.FDASSF == rate.Substring(0, 1) && w.FDALIV == rate.Substring(1, rate.Length - 1) &&
                                (w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)))
                                .Sum(sum => sum.NetPrice);
                            imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                            int rateValue = 0;
                            int.TryParse(rate.Substring(1, rate.Length - 1), out rateValue);
                            var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                            PNIVA pnivaRow = new PNIVA()
                            {
                                N4SOCI = Invoice.ftsoci,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = AccountingDate.Date,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = ivaSign,
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.FTCODC,
                                N4TCLF = "C",
                                N4ASSF = rate.Substring(0, 1),
                                n4assa = rate.Substring(1, rate.Length - 1),
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = AccountingDate.Date,
                                N4DTPGEF = null,
                                N4DTSCPG = AccountingDate.Date,
                                N4IMEU = imponibile,
                                N4IVEU = imposta,
                                N4IIEU = 0,
                                N4FLIVCA = "N",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                            IVAs.Add(pnivaRow);
                        }
                    }
                    else
                    {
                        // IVA per cassa, suddivido anche per scadenza
                        foreach (var rate in rates)
                        {
                            var imponibile = (Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                                .Where(w => w.FDASSF == rate.Substring(0, 1) && w.FDALIV == rate.Substring(1, rate.Length - 1) &&
                                (w.FDRIGA != 999999 || (w.FDRIGA == 999999 && w.FDSTAMP == 1)))
                                .Sum(sum => sum.NetPrice);
                            int rateValue = 0;
                            int.TryParse(rate.Substring(1, rate.Length - 1), out rateValue);
                            var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                            decimal partialImponibileStepAmount = Math.Round(imponibile / (expires?.Count ?? 1), 2);
                            decimal partialImponibileFinalStepAmount = Math.Round((partialImponibileStepAmount + (imponibile - (partialImponibileStepAmount * (expires?.Count ?? 1)))), 2);
                            decimal partialImpostaStepAmount = Math.Round(imposta / (expires?.Count ?? 1), 2);
                            decimal partialImpostaFinalStepAmount = Math.Round((partialImpostaStepAmount + (imposta - (partialImpostaStepAmount * (expires?.Count ?? 1)))), 2);
                            foreach (var exp in expires ?? new List<DateTime>())
                            {
                                PNIVA pnivaRow = new PNIVA()
                                {
                                    N4SOCI = Invoice.ftsoci,
                                    N4ANNO = head.N1ANNO,
                                    N4REGI = accountingID,
                                    N4RIGA = ivaRowsCounter++,
                                    N4DOCU = head.N1docn,
                                    N4RIFE = head.N1rifn,
                                    N4DARE = AccountingDate.Date,
                                    N4DADO = head.N1docd,
                                    N4DARI = head.N1rifd,
                                    N4CAUS = head.pncaus,
                                    N4SEGN = ivaSign,
                                    N4LIBR = ivaBook?.livcod,
                                    N4SOTT = Invoice.FTCODC,
                                    N4TCLF = "C",
                                    N4ASSF = rate.Substring(0, 1),
                                    n4assa = rate.Substring(1, rate.Length - 1),
                                    N4TIDO = "E",
                                    n4donu = int.Parse(head.N1docn),
                                    N4DAST = null,
                                    n4tmppn = "N",
                                    N4DTSCAD = exp.AddMonths(12).AddDays(-1),
                                    N4DTPGEF = null,
                                    N4DTSCPG = exp,
                                    N4IMEU = ivaRowsCounter == expires?.Count ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                    N4IVEU = ivaRowsCounter == expires?.Count ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                    N4IIEU = 0,
                                    N4FLIVCA = "S",
                                    N4FLSPESO = string.Empty,
                                    N4IMPPAG = 0
                                };
                                connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                                IVAs.Add(pnivaRow);
                            }
                        }
                    }

                    // GIFT
                    if ((Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Any(any => any.FDTQTA == "O"))
                    {
                        foreach (var rat in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDTQTA == "O").GroupBy(g => new { g.FDASSF, g.FDALIV }))
                        {
                            var imponibile = (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDTQTA == "O" && w.FDASSF == rat.Key.FDASSF && w.FDALIV == rat.Key.FDALIV).Sum(sum => sum.NetPrice);
                            imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                            var dischargeRate = aliquotaRepo.Get(rat.Key.FDASSF ?? string.Empty, rat.Key.FDALIV ?? string.Empty);
                            PNIVA pnivaRowC = new PNIVA()
                            {
                                N4SOCI = Invoice.ftsoci,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = AccountingDate.Date,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = Invoice.FTTIPO == "N" ? "+" : "-",
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.FTCODC,
                                N4TCLF = "C",
                                N4ASSF = dischargeRate?.assomacod,
                                n4assa = dischargeRate?.assomaali,
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = AccountingDate.Date,
                                N4DTPGEF = null,
                                N4DTSCPG = AccountingDate.Date,
                                N4IMEU = imponibile,
                                N4IVEU = 0,
                                N4IIEU = 0,
                                N4FLIVCA = "N",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRowC, transaction);
                        }
                    }
                    #endregion

                    int selfAccountID = -1;
                    if (Invoice.FTTIPO == "A" || Invoice.FTTIPO == "C")
                    {
                        selfAccountID = numRegRepository.GetNumber(Invoice.ftsoci, AccountingYear.eseann, Constants.PN, true);
                        #region PNTESTATA
                        var headSelf = new PNTESTATA()
                        {
                            N1SOCI = Invoice.ftsoci,
                            N1ANNO = AccountingYear.eseann,
                            N1REGI = selfAccountID,
                            pncaus = caufatt?.fatcaut,
                            N1DARE = AccountingDate.Date,
                            N1docn = selfAccountID.ToString(),
                            N1docd = AccountingDate.Date,
                            N1rifn = Invoice.PrintFullID,
                            N1rifd = Invoice.FTDAOR,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1CLFO = Invoice.FTCODC,
                            N1FLCF = "C",
                            N1FL01 = string.Empty,
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headSelf, transaction);
                        #endregion
                        int selfRowsCounter = 1;
                        #region PNRIGHE
                        var newRow = new PNRIGHE()
                        {
                            N1SOCI = headSelf.N1SOCI,
                            N1ANNO = headSelf.N1ANNO,
                            N1REGI = headSelf.N1REGI,
                            N1RIGA = selfRowsCounter++,
                            N1DOCU = headSelf.N1docn,
                            N1DADO = headSelf.N1docd,
                            N1RIFE = fattaut?.FTAUNUMFAT,
                            N1DARI = fattaut?.FTAUDATFAT,
                            N1SEGN = Invoice.FTTIPO == "A" ? "D" : "A",
                            pngrup = pdcSottoSupplier?.P1GRUP,
                            pncont = pdcSottoSupplier?.P2CONT,
                            pnsott = pdcSottoSupplier?.P3SOTC,
                            N1FLCO = "F",
                            n1clie = fattaut?.FTAUCOF,
                            N1IMEU = IVAs.Sum(sum => sum.N4IVEU),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headSelf.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                        var newRow2 = new PNRIGHE()
                        {
                            N1SOCI = headSelf.N1SOCI,
                            N1ANNO = headSelf.N1ANNO,
                            N1REGI = headSelf.N1REGI,
                            N1RIGA = selfRowsCounter++,
                            N1DOCU = headSelf.N1docn,
                            N1DADO = headSelf.N1docd,
                            N1RIFE = fattaut?.FTAUNUMFAT,
                            N1DARI = fattaut?.FTAUDATFAT,
                            N1SEGN = Invoice.FTTIPO == "A" ? "D" : "A",
                            pngrup = Invoice.Rows?.First().FDGRUP,
                            pncont = Invoice.Rows?.First().FDCONT,
                            pnsott = Invoice.Rows?.First().FDSCTO,
                            N1IMEU = Invoice.Rows?.First().NetPrice,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headSelf.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow2, transaction);
                        var newRow3 = new PNRIGHE()
                        {
                            N1SOCI = headSelf.N1SOCI,
                            N1ANNO = headSelf.N1ANNO,
                            N1REGI = headSelf.N1REGI,
                            N1RIGA = selfRowsCounter++,
                            N1DOCU = headSelf.N1docn,
                            N1DADO = headSelf.N1docd,
                            N1RIFE = Invoice.PrintFullID,
                            N1DARI = Invoice.FTDAOR,
                            N1SEGN = Invoice.FTTIPO == "A" ? "A" : "D",
                            pngrup = pdcSotto.P1GRUP,
                            pncont = pdcSotto.P2CONT,
                            pnsott = pdcSotto.P3SOTC,
                            N1FLCO = "C",
                            n1clie = Invoice.FTCODC,
                            N1IMEU = Invoice.GrandTotalWithGift,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headSelf.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow3, transaction);
                        #endregion
                        #region PNFORNITORI
                        var supplier = new PNFORNITORI()
                        {
                            N3SOCI = headSelf.N1SOCI,
                            N3ANNO = headSelf.N1ANNO,
                            N3REGI = headSelf.N1REGI,
                            N3RIGA = 1,
                            N3DOCU = headSelf.N1docn,
                            N3DADO = headSelf.N1docd,
                            N3RIFE = fattaut?.FTAUNUMFAT,
                            N3DARI = fattaut?.FTAUDATFAT,
                            N3DARE = AccountingDate.Date,
                            N3CAUS = headSelf.pncaus,
                            N3GRUP = pdcSottoSupplier?.P1GRUP,
                            N3CONT = pdcSottoSupplier?.P2CONT,
                            N3SOTT = fattaut?.FTAUCOF,
                            N3SSOC = headSelf.N1SOCI,
                            N3SEGN = Invoice.FTTIPO == "A" ? "D" : "A",
                            N3PAGA = fornammi?.pfocod,
                            N3SCAD = AccountingDate.Date,
                            N3DIVI = "EUR",
                            n3vcod = "UIC",
                            N3DIDO = "EUR",
                            N3VADO = "UIC",
                            N3TIDO = "E",
                            N3IMEU = newRow.N1IMEU,
                            n3rior = 1,
                            n3tipp = "1"
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, supplier, transaction);
                        #endregion
                        #region PNCLIENTI
                        var customer = new PNCLIENTI()
                        {
                            N2SOCI = headSelf.N1SOCI,
                            N2ANNO = headSelf.N1ANNO,
                            N2REGI = headSelf.N1REGI,
                            N2RIGA = 1,
                            N2RIFE = Invoice.PrintFullID,
                            N2DARI = Invoice.FTDAOR,
                            N2DOCU = headSelf.N1docn,
                            N2DADO = headSelf.N1docd,
                            N2DARE = AccountingDate.Date,
                            N2CAUS = headSelf.pncaus,
                            N2GRUP = pdcSotto.P1GRUP,
                            N2CONT = pdcSotto.P2CONT,
                            N2SOTT = Invoice.FTCODC,
                            N2SSOC = headSelf.N1SOCI,
                            N2SEGN = Invoice.FTTIPO == "A" ? "A" : "D",
                            N2PAGA = Invoice.FTPAGA,
                            N2SCAD = AccountingDate.Date,
                            N2DIVI = "EUR",
                            n2vcod = "UIC",
                            N2DIDO = "EUR",
                            N2VADO = "UIC",
                            N2TIDO = "E",
                            N2IMEU = newRow3.N1IMEU,
                            n2rior = 3,
                            n2tipi = pagcli?.pcltip
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, customer, transaction);
                        #endregion
                    }

                    // flag invoice worked
                    Invoice.FTFLA2 = "1";
                    connection.Execute(UPDATE_QUERY, Invoice, transaction);

                    transaction.Commit();
                    InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID}{(Invoice.FTTIPO == "A" ? $" e n. {selfAccountID}" : null)}");
                    return true;
                }

                return false;
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

    public bool AccountingReceivedInvoice(string CompanyID, ACC_EINVOICE_HEADS Invoice, ESERCIZIO AccountingYear, DateTime RegDate, DateTime ProtDate, CAUCONT Causal, List<PNRIGHE> Counterparts, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var pnIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>();
                var einvoHead = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>();

                // get registration number
                var accountingID = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);
                // Supplier info
                var fornammi = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, Invoice.fattfor ?? 0);
                if (fornammi == null)
                {
                    ErrorHandler.Show($"Non trovati i dati amministrativi del fornitore");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(fornammi.foGRUP) || string.IsNullOrWhiteSpace(fornammi.foCONT))
                {
                    ErrorHandler.Show($"Manca il conto contabile del fornitore");
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(fornammi.foGRUP, fornammi.foCONT, "F", CompanyID);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show($"Non trovato il sottoconto fornitore nel gruppo {fornammi.foGRUP} e conto {fornammi.foCONT}");
                    return false;
                }
                // IVA book
                var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(Causal.cauliv ?? string.Empty);
                // CAUCONT_GROUPS
                var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(CompanyID, Causal.caucod);
                // PAGCLI/TAB_ACC_TIPINC
                var pagfor = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(fornammi.pfocod ?? string.Empty);
                if (pagfor == null)
                {
                    ErrorHandler.Show($"Non trovato il codice pagamento sul fornitore");
                    return false;
                }
                var payment = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPPAGRepository>().Get(pagfor.pfotip ?? string.Empty);
                if (payment == null)
                {
                    ErrorHandler.Show($"Non trovato il tipo pagamento sul codice pagamento {pagfor.FullDescriptionSearchable}");
                    return false;
                }
                // document
                var lastProtocolInfo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, RegDate.Year, new GenericIDDescription() { ID = Causal.cauliv ?? string.Empty, Description = string.Empty }, true);
                // fix total invoice
                decimal totalInvoice = Math.Round((Invoice.fatttot ?? 0), 2, MidpointRounding.AwayFromZero);
                if (totalInvoice < 0)
                    totalInvoice *= -1;
                decimal totalInvoiceImposta = Math.Round((Invoice.fattimposta ?? 0), 2, MidpointRounding.AwayFromZero);
                if (totalInvoiceImposta < 0)
                    totalInvoiceImposta *= -1;
                // invoice vs credit parms
                string? supplierRowSign = null;
                string? otherRowsSign = null;
                string? ivaSign = null;
                if (Causal.causeg == "+")
                {
                    supplierRowSign = "A";
                    otherRowsSign = "D";
                    ivaSign = "+";
                }
                else
                {
                    supplierRowSign = "D";
                    otherRowsSign = "A";
                    ivaSign = "-";
                }

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = CompanyID,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = accountingID,
                    pncaus = Causal.caucod,
                    N1DARE = RegDate.Date,
                    N1docn = lastProtocolInfo.ToString(),
                    N1docd = ProtDate,
                    N1rifn = Invoice.fattnum,
                    N1rifd = Invoice.fattdata,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Invoice.fattfor,
                    N1FLCF = "F",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                #region Supplier row
                var supplierRows = new List<PNRIGHE>();
                PNRIGHE supplierRow = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = rowsCounter++,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1rifn,
                    N1DARI = head.N1rifd,
                    n1clie = Invoice.fattfor,
                    N1SEGN = supplierRowSign,
                    pngrup = pdcSotto.P1GRUP,
                    pncont = pdcSotto.P2CONT,
                    pnsott = pdcSotto.P3SOTC,
                    N1IMEU = totalInvoice + (Counterparts.Where(w => w.NotPair).Sum(sum => sum.N1IMEU) ?? 0),
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "N",
                    n1paga = fornammi.pfocod,
                    N1DRri = head.N1docd,
                    N1DESC = Invoice.Supplier?.FullDescriptionSearchable
                };
                connection.Execute(pnRigheRepository.INSERT_QUERY, supplierRow, transaction);
                supplierRows.Add(supplierRow);
                #endregion

                #region IVA row
                if (Invoice.fattimposta > 0)
                {
                    foreach (var cg in grpcau ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        PNRIGHE ivaRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = cg.grpseg,
                            pngrup = cg.grpgrp,
                            pncont = cg.grpcto,
                            pnsott = cg.grpsct,
                            N1IMEU = grpcau?.Count == 1 ? totalInvoiceImposta : 0,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1DESC = Invoice.Supplier?.FullDescriptionSearchable,
                            N1DRri = head.N1docd,
                            N1tmpPNR = "N"
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, ivaRow, transaction);
                    }
                }
                #endregion

                // compute expires count
                var expiresCount = Invoice.Expires?.Count > 0 ? Invoice.Expires.Count : 1;

                #region PNIVA
                int ivaRowsCounter = 1;
                bool hasIndetraibile = false;
                decimal indetraibileTotal = 0;
                if (!AccountingYear.eseivavenBool)
                {
                    // NO IVA per cassa
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        bool isVatNegative = false;
                        // fix amounts
                        var imponibileFix = Math.Round((rate.fattimpodett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (imponibileFix < 0)
                        {
                            imponibileFix *= -1;
                            isVatNegative = true;
                        }
                        var impostaFix = Math.Round((rate.fattimpostadett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (impostaFix < 0)
                            impostaFix *= -1;
                        if (impostaFix == 0)
                        {
                            decimal rateVal = 0;
                            decimal.TryParse(rate.RateValue, out rateVal);
                            impostaFix = Math.Round(imponibileFix * rateVal / 100, 2, MidpointRounding.AwayFromZero);
                        }
                        decimal indetraibileAmount = 0;
                        if (rate.SelectedRate != null && rate.SelectedRate.asspin.HasValue && rate.SelectedRate.asspin.Value > 0)
                        {
                            indetraibileAmount = Math.Round((rate.fattimpostadett ?? 0) * rate.SelectedRate.asspin.Value / 100, 2, MidpointRounding.AwayFromZero);
                            indetraibileTotal += indetraibileAmount;
                            hasIndetraibile = true;
                        }
                        PNIVA pnivaRow = new PNIVA()
                        {
                            N4SOCI = CompanyID,
                            N4ANNO = head.N1ANNO,
                            N4REGI = accountingID,
                            N4RIGA = ivaRowsCounter++,
                            N4DOCU = head.N1docn,
                            N4RIFE = head.N1rifn,
                            N4DARE = head.N1DARE,
                            N4DADO = head.N1docd,
                            N4DARI = head.N1rifd,
                            N4CAUS = head.pncaus,
                            N4SEGN = (isVatNegative) ? (ivaSign == "+") ? "-" : "+" : ivaSign,
                            N4LIBR = ivaBook?.livcod,
                            N4SOTT = Invoice.fattfor,
                            N4TCLF = "F",
                            N4ASSF = rate.SelectedRate?.asscod,
                            n4assa = rate.SelectedRate?.assali,
                            N4TIDO = "E",
                            n4donu = int.Parse(head.N1docn),
                            N4DAST = Constants._GX_MIN_DATE,
                            N4FLGS = "",
                            n4tmppn = "N",
                            N4DTPGEF = null,
                            N4IMEU = imponibileFix,
                            N4IVEU = impostaFix,
                            N4IIEU = indetraibileAmount,
                            N4FLIVCA = "N",
                            N4FLSPESO = string.Empty,
                            N4IMPPAG = 0,
                        };
                        connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                    }
                }
                else
                {
                    // IVA per cassa, suddivido anche per scadenza
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        // fix amounts
                        var imponibileFix = Math.Round((rate.fattimpodett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (imponibileFix < 0)
                            imponibileFix *= -1;
                        var impostaFix = Math.Round((rate.fattimpostadett ?? 0), 2, MidpointRounding.AwayFromZero);
                        if (impostaFix < 0)
                            impostaFix *= -1;
                        decimal partialImponibileStepAmount = Math.Round(imponibileFix / expiresCount, 2);
                        decimal partialImponibileFinalStepAmount = Math.Round((partialImponibileStepAmount + (imponibileFix - (partialImponibileStepAmount * expiresCount))), 2);
                        decimal partialImpostaStepAmount = Math.Round(impostaFix / expiresCount, 2);
                        decimal partialImpostaFinalStepAmount = Math.Round((partialImpostaStepAmount + (impostaFix - (partialImpostaStepAmount * expiresCount))), 2);
                        decimal partialImpostaIndetraibileStepAmount = 0;
                        decimal partialImpostaIndetraibileFinalStepAmount = 0;
                        if (rate.SelectedRate != null && rate.SelectedRate.asspin.HasValue && rate.SelectedRate.asspin.Value > 0)
                        {
                            var indetraibileAmount = Math.Round((rate.fattimpostadett ?? 0) * rate.SelectedRate.asspin.Value / 100, 2, MidpointRounding.AwayFromZero);
                            indetraibileTotal += indetraibileAmount;
                            partialImpostaIndetraibileStepAmount = Math.Round(indetraibileAmount / expiresCount, 2);
                            partialImpostaIndetraibileFinalStepAmount = Math.Round((partialImpostaIndetraibileStepAmount + (indetraibileAmount - (partialImpostaIndetraibileStepAmount * expiresCount))), 2);
                        }

                        if (Invoice.Expires != null && Invoice.Expires.Count > 0)
                        {
                            foreach (var exp in Invoice.Expires)
                            {
                                PNIVA pnivaRow = new PNIVA()
                                {
                                    N4SOCI = CompanyID,
                                    N4ANNO = head.N1ANNO,
                                    N4REGI = accountingID,
                                    N4RIGA = ivaRowsCounter++,
                                    N4DOCU = head.N1docn,
                                    N4RIFE = head.N1rifn,
                                    N4DARE = head.N1DARE,
                                    N4DADO = head.N1docd,
                                    N4DARI = head.N1rifd,
                                    N4CAUS = head.pncaus,
                                    N4SEGN = ivaSign,
                                    N4LIBR = ivaBook?.livcod,
                                    N4SOTT = Invoice.fattfor,
                                    N4TCLF = "F",
                                    N4ASSF = rate.SelectedRate?.asscod,
                                    n4assa = rate.SelectedRate?.assali,
                                    N4TIDO = "E",
                                    n4donu = int.Parse(head.N1docn),
                                    N4DAST = Constants._GX_MIN_DATE,
                                    N4FLGS = "",
                                    n4tmppn = "N",
                                    N4DTSCAD = exp.fattdatascad.Date.AddMonths(12).AddDays(-1),
                                    N4DTPGEF = null,
                                    N4DTSCPG = exp.fattdatascad.Date,
                                    N4IMEU = ivaRowsCounter == expiresCount ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                    N4IVEU = ivaRowsCounter == expiresCount ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                    N4IIEU = ivaRowsCounter == expiresCount ? partialImpostaIndetraibileFinalStepAmount : partialImpostaIndetraibileStepAmount,
                                    N4FLIVCA = "S",
                                    N4FLSPESO = string.Empty,
                                    N4IMPPAG = 0
                                };
                                connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                            }
                        }
                        else
                        {
                            // no scadenze
                            PNIVA pnivaRow = new PNIVA()
                            {
                                N4SOCI = CompanyID,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = head.N1DARE,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = ivaSign,
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.fattfor,
                                N4TCLF = "F",
                                N4ASSF = rate.SelectedRate?.asscod,
                                n4assa = rate.SelectedRate?.assali,
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = Constants._GX_MIN_DATE,
                                N4FLGS = "",
                                n4tmppn = "N",
                                N4DTSCAD = head.N1DARE.Value.AddMonths(12).AddDays(-1),
                                N4DTPGEF = null,
                                N4DTSCPG = head.N1DARE.Value,
                                N4IMEU = ivaRowsCounter == expiresCount ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                N4IVEU = ivaRowsCounter == expiresCount ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                N4IIEU = 0,
                                N4FLIVCA = "S",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(pnIvaRepo.INSERT_QUERY, pnivaRow, transaction);
                        }
                    }
                }
                #endregion

                #region Costi rows
                foreach (var cop in Counterparts)
                {
                    // fix amount
                    cop.N1IMEU = Math.Round((cop.N1IMEU ?? 0), 2, MidpointRounding.AwayFromZero);
                    if (cop.N1IMEU < 0)
                        cop.N1IMEU *= -1;
                    cop.N1RIGA = rowsCounter++;
                    cop.N1SOCI = head.N1SOCI;
                    cop.N1ANNO = head.N1ANNO;
                    cop.N1REGI = head.N1REGI;
                    cop.N1DOCU = head.N1docn;
                    cop.N1DADO = head.N1docd;
                    cop.N1RIFE = head.N1rifn;
                    cop.N1DARI = head.N1rifd;
                    cop.N1SEGN = otherRowsSign;
                    cop.N1CHIU = "A";
                    cop.N1TIDO = "E";
                    cop.N1DIVI = "EUR";
                    cop.N1tmpPNR = "N";
                    cop.N1DESC = Invoice.Supplier?.FullDescriptionSearchable;
                    cop.N1DRri = head.N1docd;
                    connection.Execute(pnRigheRepository.INSERT_QUERY, cop, transaction);
                }
                #endregion

                #region PNFORNITORI
                int supplierRowsCounter = 1;
                foreach (var supRow in supplierRows)
                {
                    decimal stepAmount = Math.Round((supRow.N1IMEU ?? 0) / expiresCount, 2);
                    decimal finalStepAmount = Math.Round((stepAmount + ((supRow.N1IMEU ?? 0) - (stepAmount * expiresCount))), 2);
                    if (Invoice.Expires != null && Invoice.Expires.Count > 0)
                    {
                        foreach (var exp in Invoice.Expires)
                        {
                            var supplier = new PNFORNITORI()
                            {
                                N3SOCI = head.N1SOCI,
                                N3ANNO = head.N1ANNO,
                                N3REGI = head.N1REGI,
                                N3RIGA = supplierRowsCounter++,
                                N3DARI = head.N1rifd,
                                N3RIFE = head.N1rifn,
                                N3DOCU = head.N1docn,
                                N3DADO = head.N1docd,
                                N3DARE = head.N1DARE,
                                N3CAUS = head.pncaus,
                                N3GRUP = pdcSotto.P1GRUP,
                                N3CONT = pdcSotto.P2CONT,
                                N3SOTT = Invoice.fattfor,
                                N3SSOC = head.N1SOCI,
                                N3SEGN = supRow.N1SEGN,
                                N3PAGA = pagfor?.pfocod,
                                N3SCAD = (exp.fattdatascad == DateTime.MinValue) ? null : exp.fattdatascad.Date,
                                N3DIVI = "EUR",
                                n3vcod = "UIC",
                                N3DIDO = "EUR",
                                N3VADO = "UIC",
                                N3TIDO = "E",
                                N3IMEU = supplierRowsCounter == expiresCount ? finalStepAmount : stepAmount,
                                n3rior = supRow.N1RIGA,
                                n3tipp = pagfor?.pfotip,
                                N3PARE = "",
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, supplier, transaction);
                        }
                    }
                    else
                    {
                        // nessuna scadenza
                        var supplier = new PNFORNITORI()
                        {
                            N3SOCI = head.N1SOCI,
                            N3ANNO = head.N1ANNO,
                            N3REGI = head.N1REGI,
                            N3RIGA = supplierRowsCounter++,
                            N3DARI = head.N1rifd,
                            N3RIFE = head.N1rifn,
                            N3DOCU = head.N1docn,
                            N3DADO = head.N1docd,
                            N3DARE = head.N1DARE,
                            N3CAUS = head.pncaus,
                            N3GRUP = pdcSotto.P1GRUP,
                            N3CONT = pdcSotto.P2CONT,
                            N3SOTT = Invoice.fattfor,
                            N3SSOC = head.N1SOCI,
                            N3SEGN = supRow.N1SEGN,
                            N3PAGA = pagfor?.pfocod,
                            N3SCAD = head.N1DARE,
                            N3DIVI = "EUR",
                            n3vcod = "UIC",
                            N3DIDO = "EUR",
                            N3VADO = "UIC",
                            N3TIDO = "E",
                            N3IMEU = supplierRowsCounter == expiresCount ? finalStepAmount : stepAmount,
                            n3rior = supRow.N1RIGA,
                            n3tipp = pagfor?.pfotip
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, supplier, transaction);
                    }
                }
                #endregion

                #region Giroconto IVA indetraibile
                if (hasIndetraibile)
                {
                    // get registration number
                    var girocontoIndetraibileRegID = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);

                    #region PNTESTATA
                    PNTESTATA girocontoHead = new PNTESTATA()
                    {
                        N1SOCI = CompanyID,
                        N1ANNO = AccountingYear.eseann,
                        N1REGI = girocontoIndetraibileRegID,
                        pncaus = ivaBook?.livcii,
                        N1DARE = RegDate,
                        N1docn = head.N1docn,
                        N1docd = head.N1docd,
                        N1rifn = head.N1rifn,
                        N1rifd = head.N1rifd,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1FL01 = string.Empty,
                        N1TmpPN = "N",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(pnTestataRepository.INSERT_QUERY, girocontoHead, transaction);
                    #endregion

                    PNRIGHE firstRow = new PNRIGHE()
                    {
                        N1SOCI = girocontoHead.N1SOCI,
                        N1ANNO = girocontoHead.N1ANNO,
                        N1REGI = girocontoHead.N1REGI,
                        N1RIGA = 1,
                        N1DOCU = girocontoHead.N1docn,
                        N1DADO = girocontoHead.N1docd,
                        N1RIFE = girocontoHead.N1rifn,
                        N1DARI = girocontoHead.N1rifd,
                        N1SEGN = ivaSign == "+" ? "A" : "D",
                        pngrup = ivaBook?.livgci,
                        pncont = ivaBook?.livcci,
                        pnsott = ivaBook?.livsci,
                        N1IMEU = indetraibileTotal,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1DESC = Invoice.Supplier?.FullDescriptionSearchable,
                        N1DRri = girocontoHead.N1docd,
                        N1tmpPNR = "N"
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, firstRow, transaction);
                    PNRIGHE secondRow = new PNRIGHE()
                    {
                        N1SOCI = girocontoHead.N1SOCI,
                        N1ANNO = girocontoHead.N1ANNO,
                        N1REGI = girocontoHead.N1REGI,
                        N1RIGA = 2,
                        N1DOCU = girocontoHead.N1docn,
                        N1DADO = girocontoHead.N1docd,
                        N1RIFE = girocontoHead.N1rifn,
                        N1DARI = girocontoHead.N1rifd,
                        N1SEGN = ivaSign == "+" ? "D" : "A",
                        pngrup = Counterparts.First().pngrup,
                        pncont = Counterparts.First().pncont,
                        pnsott = Counterparts.First().pnsott,
                        N1IMEU = indetraibileTotal,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1DESC = Invoice.Supplier?.FullDescriptionSearchable,
                        N1DRri = girocontoHead.N1docd,
                        N1tmpPNR = "N"
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, secondRow, transaction);
                }
                #endregion

                // flag invoice accounted and add year/number reg
                Invoice.fattannoreg = head.N1ANNO;
                Invoice.fattnumreg = head.N1REGI;
                Invoice.accounted = now;
                Invoice.accounted_UserID = UserID;
                connection.Execute(einvoHead.UPDATE_QUERY, Invoice, transaction);

                transaction.Commit();
                InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID} protocollo {head.N1docn}");
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
    public bool AccountingSentExternalInvoice(string CompanyID, ACC_EINVOICE_HEADS Invoice, ESERCIZIO AccountingYear, DateTime RegDate, DateTime ProtDate, CAUCONT Causal, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var pnPortafoglioRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                var accPlafondRepository = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var accPlafondRowsRepository = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var accPlafondParmsRepository = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var tipiIncRepo = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>();

                // get registration number
                var accountingID = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);
                // Customer info
                var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, Invoice.fattcli ?? 0);
                if (cliammi == null)
                {
                    ErrorHandler.Show($"Non trovati i dati amministrativi del cliente");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(cliammi.clGRUP) || string.IsNullOrWhiteSpace(cliammi.clcont))
                {
                    ErrorHandler.Show($"Manca il conto contabile del cliente");
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(cliammi.clGRUP, cliammi.clcont, "C", CompanyID);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show($"Non trovato il sottoconto cliente nel gruppo {cliammi.clGRUP} e conto {cliammi.clcont}");
                    return false;
                }
                // IVA book
                var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(Causal.cauliv ?? string.Empty);
                // IVA book numerator
                var customProtocol = numRegRepository.GetNumber(CompanyID, AccountingYear.eseann, new GenericIDDescription() { ID = ivaBook?.livcod, Description = ivaBook?.livdes }, true);
                // CAUCONT_GROUPS
                var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(CompanyID, Causal.caucod);
                // PAGCLI/TAB_ACC_TIPINC
                var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(cliammi.pclcod ?? string.Empty);
                if (pagcli == null)
                {
                    ErrorHandler.Show($"Non trovato il codice pagamento sul cliente");
                    return false;
                }
                var payment = tipiIncRepo.Get(pagcli.pcltip ?? string.Empty);
                if (payment == null)
                {
                    ErrorHandler.Show($"Non trovato il tipo pagamento sul codice pagamento {pagcli.FullDescriptionSearchable}");
                    return false;
                }
                // invoice vs credit parms
                string? customerRowSign = null;
                string? otherRowsSign = null;
                string? ivaSign = null;
                if (Causal.causeg == "+")
                {
                    customerRowSign = "D";
                    otherRowsSign = "A";
                    ivaSign = "+";
                }
                else
                {
                    customerRowSign = "A";
                    otherRowsSign = "D";
                    ivaSign = "-";
                }

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = CompanyID,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = accountingID,
                    pncaus = Causal.caucod,
                    N1DARE = RegDate,
                    N1docn = customProtocol.ToString(),
                    N1docd = ProtDate,
                    N1rifn = Invoice.fattnum,
                    N1rifd = ProtDate,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Invoice.fattcli,
                    N1FLCF = "C",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                if (!Causal.causolBool)
                {
                    #region Customer row
                    var customerRows = new List<PNRIGHE>();
                    PNRIGHE customerRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = head.N1rifn,
                        N1DARI = head.N1rifd,
                        n1clie = Invoice.fattcli,
                        N1SEGN = customerRowSign,
                        pngrup = pdcSotto.P1GRUP,
                        pncont = pdcSotto.P2CONT,
                        pnsott = pdcSotto.P3SOTC,
                        N1IMEU = Invoice.fatttot,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = cliammi.pclcod,
                        N1DRri = head.N1docd
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    customerRows.Add(customerRow);
                    #endregion

                    #region IVA row
                    if (Invoice.fattimposta > 0)
                    {
                        foreach (var cg in grpcau ?? new ObservableCollection<CAUCONT_GROUPS>())
                        {
                            PNRIGHE ivaRow = new PNRIGHE()
                            {
                                N1SOCI = head.N1SOCI,
                                N1ANNO = head.N1ANNO,
                                N1REGI = head.N1REGI,
                                N1RIGA = rowsCounter++,
                                N1DOCU = head.N1docn,
                                N1DADO = head.N1docd,
                                N1RIFE = head.N1rifn,
                                N1DARI = head.N1rifd,
                                N1SEGN = cg.grpseg,
                                pngrup = cg.grpgrp,
                                pncont = cg.grpcto,
                                pnsott = cg.grpsct,
                                N1IMEU = grpcau?.Count == 1 ? Invoice.fattimposta : 0,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DRri = head.N1docd,
                                N1tmpPNR = "N"
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, ivaRow, transaction);
                        }
                    }
                    #endregion

                    #region Ricavi rows
                    foreach (var cop in (Invoice.Rows ?? new ObservableCollection<ACC_EINVOICE_ROWS>()).GroupBy(g => new { g.Product?.GroupID, g.Product?.AccountID, g.Product?.SubaccountID, g.Product?.costcenter_id }))
                    {
                        var pnCop = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = otherRowsSign,
                            pngrup = cop.Key.GroupID,
                            pncont = cop.Key.AccountID,
                            pnsott = cop.Key.SubaccountID,
                            N1CCCC = cop.Key.costcenter_id,
                            N1IMEU = Math.Round((Invoice.Rows ?? new ObservableCollection<ACC_EINVOICE_ROWS>()).Where(w => w.Product?.GroupID == cop.Key.GroupID && w.Product?.AccountID == cop.Key.AccountID && w.Product?.SubaccountID == cop.Key.SubaccountID && w.Product?.costcenter_id == cop.Key.costcenter_id).Sum(sum => sum.fatttotriga) ?? 0, 2, MidpointRounding.AwayFromZero),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd,
                            N1DESC = Invoice.Customer?.FullDescriptionSearchable
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, pnCop, transaction);
                    }
                    #endregion

                    #region PNCLIENTI
                    int customerRowsCounter = 1;
                    foreach (var supRow in customerRows)
                    {
                        decimal stepAmount = Math.Round((supRow.N1IMEU ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal finalStepAmount = Math.Round((stepAmount + ((supRow.N1IMEU ?? 0) - (stepAmount * (Invoice.Expires?.Count ?? 1)))), 2);
                        foreach (var exp in Invoice.Expires ?? new ObservableCollection<ACC_EINVOICE_EXPIRES>())
                        {
                            var custom = new PNCLIENTI()
                            {
                                N2SOCI = head.N1SOCI,
                                N2ANNO = head.N1ANNO,
                                N2REGI = head.N1REGI,
                                N2RIGA = customerRowsCounter++,
                                N2DARI = head.N1rifd,
                                N2RIFE = head.N1rifn,
                                N2DOCU = head.N1docn,
                                N2DADO = head.N1docd,
                                N2DARE = head.N1DARE,
                                N2CAUS = head.pncaus,
                                N2GRUP = pdcSotto.P1GRUP,
                                N2CONT = pdcSotto.P2CONT,
                                N2SOTT = Invoice.fattcli,
                                N2SSOC = head.N1SOCI,
                                N2SEGN = supRow.N1SEGN,
                                N2PAGA = pagcli?.pclcod,
                                N2SCAD = exp.fattdatascad.Date,
                                N2DIVI = "EUR",
                                n2vcod = "UIC",
                                N2DIDO = "EUR",
                                N2VADO = "UIC",
                                N2TIDO = "E",
                                N2IMEU = customerRowsCounter == Invoice.Expires?.Count ? finalStepAmount : stepAmount,
                                n2rior = supRow.N1RIGA,
                                n2tipi = pagcli?.pcltip
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, custom, transaction);
                        }
                    }
                    #endregion

                    #region PNPORTAFOGLIO
                    if (payment != null && payment.icssup == "R")
                    {
                        int portafRowCounter = 1;
                        int portafQuoteCounter = 1;
                        decimal stepAmountP = Math.Round((Invoice.fattimpo ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal finalStepAmountP = Math.Round((stepAmountP + ((Invoice.fattimpo ?? 0) - (stepAmountP * (Invoice.Expires?.Count ?? 1)))), 2);
                        foreach (var exp in Invoice.Expires ?? new ObservableCollection<ACC_EINVOICE_EXPIRES>())
                        {
                            PNPORTAFOGLIO portaf = new PNPORTAFOGLIO()
                            {
                                N6SOCI = head.N1SOCI,
                                N6ANNO = head.N1ANNO,
                                N6REGI = head.N1REGI,
                                N6RIGA = portafRowCounter++,
                                N6DARI = head.N1rifd,
                                N6RIFE = head.N1rifn,
                                N6DOCU = head.N1docn,
                                N6DADO = head.N1docd,
                                N6DARE = head.N1DARE,
                                N6CAUS = head.pncaus,
                                N6GRUP = pdcSotto.P1GRUP,
                                N6CONT = pdcSotto.P2CONT,
                                N6SOTT = Invoice.fattcli,
                                N6SCAD = exp.fattdatascad.Date,
                                N6SEGN = customerRowSign,
                                N6RATA = portafQuoteCounter++,
                                N6CABI = !string.IsNullOrWhiteSpace(exp.fattabi) ? int.Parse(exp.fattabi) : cliammi.CLABI,
                                N6CCAB = !string.IsNullOrWhiteSpace(exp.fattcab) ? int.Parse(exp.fattcab) : cliammi.CLCAB,
                                N6COVA = "EUR",
                                N6IMEU = customerRowsCounter == Invoice.Expires?.Count ? finalStepAmountP : stepAmountP,
                                N6TIPODOC = string.Empty,
                                N6STATO = "N"
                            };
                            connection.Execute(pnPortafoglioRepo.INSERT_QUERY, portaf, transaction);
                        }
                    }
                    #endregion

                    #region Plafond
                    var plafondTotal = Invoice.Rows?.Where(w => w.Rate?.assplaBool ?? false).Sum(sum => sum.fatttotriga);
                    if (plafondTotal > 0)
                    {
                        var plafondSettings = accPlafondParmsRepository.Get(Invoice.fattsoc);
                        if (plafondTotal > plafondSettings?.limit_amount)
                        {
                            var plafond = accPlafondRepository.GetLast(Invoice.fattsoc, Invoice.fattcli ?? 0, AccountingYear.eseann, Invoice.fattdata, false);

                            if (plafond != null)
                            {
                                // update plafond counters
                                plafond.cliimpfattprovv -= plafondTotal;
                                plafond.cliimpfattprog += plafondTotal;
                                connection.Execute(accPlafondRepository.UPDATE_QUERY, plafond, transaction);

                                // add detail
                                var detail = new ACC_PLAFOND_ROWS()
                                {
                                    Cliasoc = Invoice.fattsoc,
                                    Cliacod = Invoice.fattcli ?? 0,
                                    cliannosol = AccountingYear.eseann,
                                    cliprog = plafond.cliprog,
                                    clinumfat = Invoice.fattnum,
                                    clidatfatt = Invoice.fattdata,
                                    cliimpplaf = plafondTotal,
                                    cliimpimpo = plafondTotal + Invoice.fattimpobol
                                };
                                connection.Execute(accPlafondRowsRepository.INSERT_QUERY, detail, transaction);
                            }
                        }
                    }
                    #endregion
                }

                #region PNIVA
                int ivaRowsCounter = 1;

                if (!AccountingYear.eseivavenBool)
                {
                    // NO IVA per cassa
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        var rateItem = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetFirstAliquota(rate.Fattaliq ?? string.Empty);
                        PNIVA pnivaRow = new PNIVA()
                        {
                            N4SOCI = CompanyID,
                            N4ANNO = head.N1ANNO,
                            N4REGI = accountingID,
                            N4RIGA = ivaRowsCounter++,
                            N4DOCU = head.N1docn,
                            N4RIFE = head.N1rifn,
                            N4DARE = head.N1DARE,
                            N4DADO = head.N1docd,
                            N4DARI = head.N1rifd,
                            N4CAUS = head.pncaus,
                            N4SEGN = ivaSign,
                            N4LIBR = ivaBook?.livcod,
                            N4SOTT = Invoice.fattcli,
                            N4TCLF = "F",
                            N4ASSF = rateItem?.asscod,
                            n4assa = rateItem?.assali,
                            N4TIDO = "E",
                            n4donu = 0,
                            N4DAST = null,
                            n4tmppn = "N",
                            N4DTPGEF = null,
                            N4IMEU = rate.fattimpodett,
                            N4IVEU = rate.fattimpostadett,
                            N4IIEU = 0,
                            N4FLIVCA = "N",
                            N4FLSPESO = string.Empty,
                            N4IMPPAG = 0
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, pnivaRow, transaction);
                    }
                }
                else
                {
                    // IVA per cassa, suddivido anche per scadenza
                    foreach (var rate in Invoice.VATs ?? new ObservableCollection<ACC_EINVOICE_VAT>())
                    {
                        decimal partialImponibileStepAmount = Math.Round((rate.fattimpodett ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal partialImponibileFinalStepAmount = Math.Round((partialImponibileStepAmount + ((rate.fattimpodett ?? 0) - (partialImponibileStepAmount * (Invoice.Expires?.Count ?? 1)))), 2);
                        decimal partialImpostaStepAmount = Math.Round((rate.fattimpostadett ?? 0) / (Invoice.Expires?.Count ?? 1), 2);
                        decimal partialImpostaFinalStepAmount = Math.Round((partialImpostaStepAmount + ((rate.fattimpostadett ?? 0) - (partialImpostaStepAmount * (Invoice.Expires?.Count ?? 1)))), 2);
                        foreach (var exp in Invoice.Expires ?? new ObservableCollection<ACC_EINVOICE_EXPIRES>())
                        {
                            var rateItem = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetFirstAliquota(rate.Fattaliq ?? string.Empty);
                            PNIVA pnivaRow = new PNIVA()
                            {
                                N4SOCI = CompanyID,
                                N4ANNO = head.N1ANNO,
                                N4REGI = accountingID,
                                N4RIGA = ivaRowsCounter++,
                                N4DOCU = head.N1docn,
                                N4RIFE = head.N1rifn,
                                N4DARE = head.N1DARE,
                                N4DADO = head.N1docd,
                                N4DARI = head.N1rifd,
                                N4CAUS = head.pncaus,
                                N4SEGN = ivaSign,
                                N4LIBR = ivaBook?.livcod,
                                N4SOTT = Invoice.fattcli,
                                N4TCLF = "F",
                                N4ASSF = rateItem?.asscod,
                                n4assa = rateItem?.assali,
                                N4TIDO = "E",
                                n4donu = int.Parse(head.N1docn),
                                N4DAST = null,
                                n4tmppn = "N",
                                N4DTSCAD = exp.fattdatascad.Date.AddMonths(12).AddDays(-1),
                                N4DTPGEF = null,
                                N4DTSCPG = exp.fattdatascad.Date,
                                N4IMEU = ivaRowsCounter == Invoice.Expires?.Count ? partialImponibileFinalStepAmount : partialImponibileStepAmount,
                                N4IVEU = ivaRowsCounter == Invoice.Expires?.Count ? partialImpostaFinalStepAmount : partialImpostaStepAmount,
                                N4IIEU = 0,
                                N4FLIVCA = "S",
                                N4FLSPESO = string.Empty,
                                N4IMPPAG = 0
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, pnivaRow, transaction);
                        }
                    }
                }
                #endregion

                // flag invoice accounted and add year/number reg
                Invoice.fattannoreg = head.N1ANNO;
                Invoice.fattnumreg = head.N1REGI;
                Invoice.accounted = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                Invoice.accounted_UserID = UserID;
                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().UPDATE_QUERY, Invoice, transaction);

                transaction.Commit();
                InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID}");
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

    #region CRM
    public bool GenerateByOrder(ORDIT00F OrderHead, List<ORDID00F> SelectedRows, string UserID, string HeadNotes, string FootNotes, string DocumentType)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {

                var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var plafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var plafondRowsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var plafondParmsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var caufatRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>();
                var fattdRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var cliammiRepo = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                int sequence = 1;
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var newInvoiceID = numRegRepository.GetNumber(OrderHead.otsoci, now.Year, Constants.INVOICE_TEMP, true);
                var invoiceCausal = caufatRepo.Get(OrderHead.Causal?.caufat ?? string.Empty);

                // generate invoice head
                var newHead = new FATTT00F()
                {
                    ftsoci = OrderHead.otsoci,
                    FTANNO = now.Year,
                    FTNUOR = newInvoiceID,
                    FTTIPO = DocumentType,
                    FTDAOR = now.Date,
                    FTCAUS = OrderHead.Causal?.caufat,
                    FTCODC = OrderHead.OTCLIE,
                    FTCODD = OrderHead.DESTIN,
                    FTPAGA = OrderHead.OTPAGA,
                    FTABIB = OrderHead.abiabi,
                    FTCABB = OrderHead.abicab,
                    FTCONS = OrderHead.OTCONS,
                    FTSPED = OrderHead.OTSPED,
                    FTCORR = OrderHead.OTCORR,
                    FTCORR2 = OrderHead.OTCORR2,
                    FTCIDI = "UIC",
                    ftciva = OrderHead.otdivi,
                    FTIMBL = OrderHead.OTIMBA,
                    ftling = OrderHead.otling,
                    addedUserID = UserID,
                    FTNOTET = HeadNotes,
                    FTNOTEP = FootNotes,
                    FTSHOWT = !string.IsNullOrWhiteSpace(HeadNotes),
                    FTSHOWP = !string.IsNullOrWhiteSpace(FootNotes),
                    FTDE25 = OrderHead.OTDE25,
                    FTBCON = OrderHead.OTBCON,
                    FTSCCL = OrderHead.OTSCCL,
                    FTAREA = OrderHead.OTAREA,
                    FTRIVE = OrderHead.OTRIVE,
                    FTCONZ = OrderHead.OTCONZ,
                    FTFILI = OrderHead.OTFILI,
                    FTZONA = OrderHead.OTZONA,
                    FTSETM = OrderHead.OTSETM,
                    FTREGI = OrderHead.OTREGI,
                    fttdoc = invoiceCausal?.fattido
                };
                connection.Execute(INSERT_QUERY, newHead, transaction);
                // generate rows and update origins
                decimal plafondTotal = 0;
                foreach (var row in SelectedRows)
                {
                    var rate = aliquotaRepo.Get(row.ODASSF ?? string.Empty, row.ODALIV ?? string.Empty);
                    if (rate?.assplaBool ?? false)
                        plafondTotal += row.Amount;
                    var newRow = new FATTD00F()
                    {
                        ftsoci = OrderHead.otsoci,
                        FTANNO = now.Year,
                        FTNUOR = newInvoiceID,
                        FDRIGA = sequence++,
                        FDCODA = row.ODCODA,
                        FDQTAV = row.QuantityToSend,
                        FDTQTA = row.ODTQTA,
                        FDPREZ = row.ODPREZ,
                        FDTPRE = row.ODTPRE,
                        FDSCO1 = row.ODSCO1,
                        FDSCO2 = row.ODSCO2,
                        FDSCO3 = row.ODSCO3,
                        FDMAGG = row.ODMAGG,
                        FDTSC1 = row.ODTSC1,
                        FDTSC2 = row.ODTSC2,
                        FDTSC3 = row.ODTSC3,
                        FDTMAG = row.ODTMAG,
                        FDASSF = row.ODASSF,
                        FDALIV = row.ODALIV,
                        FDDACO = row.ODDACO,
                        FDRIFC = row.ODRIFC,
                        FDGRUP = row.ODGRUP,
                        FDCONT = row.ODCONT,
                        FDSCTO = row.ODSCTO,
                        FDUNIM = row.odunit,
                        OTANN1 = row.OTANNO,
                        OTNUO1 = row.OTNUOR,
                        ODRIG1 = row.ODRIGA,
                        FDNOTE = row.ODNOTE,
                        FDSHOW = row.ODSHOW,
                        FDCOAG1 = row.ODCOA1,
                        FDCOAG2 = row.ODCOA2,
                        FDPROV = row.ODCOA1P,
                        fdpro2 = row.ODCOA2P,
                        FDCOAG1PT = row.ODCOA1PT,
                        FDCOAG2PT = row.ODCOA2PT,
                        FDSERI = row.ODSERI,
                        fdtiva = rate?.assnatufe
                    };
                    connection.Execute(fattdRepo.INSERT_QUERY, newRow, transaction);
                    row.ODSTATO = row.FulfillmentID == "S" ? "*" : null;
                    row.ODQTAEV = (row.ODQTAEV ?? 0) + row.QuantityToSend;
                    connection.Execute(ordidRepo.UPDATE_QUERY, row, transaction);
                }
                // check for plafond and stamp
                if (plafondTotal > 0)
                {
                    var plafondSettings = plafondParmsRepo.Get(OrderHead.otsoci);
                    if (plafondTotal > plafondSettings?.limit_amount)
                    {
                        // add stamp row
                        var cliammi = cliammiRepo.Get(OrderHead.otsoci, OrderHead.OTCLIE ?? 0);
                        var plafond = plafondRepo.GetLast(OrderHead.otsoci, OrderHead.OTCLIE ?? 0, now.Year, now.Date, false);
                        var plafondRate = aliquotaRepo.Get(plafondSettings.rate_code, plafondSettings.rate_value);
                        var stampRow = new FATTD00F()
                        {
                            ftsoci = OrderHead.otsoci,
                            FTANNO = now.Year,
                            FTNUOR = newInvoiceID,
                            FDRIGA = 999999,
                            FDCODA = plafondSettings.product_id,
                            FDQTAV = 1,
                            FDTQTA = "B",
                            FDPREZ = plafondSettings.stamp_amount,
                            FDTPRE = "U",
                            FDUNIM = plafondSettings.UM,
                            FDASSF = plafondSettings.rate_code,
                            FDALIV = plafondSettings.rate_value,
                            FDGRUP = plafondSettings.group_id,
                            FDCONT = plafondSettings.account_id,
                            FDSCTO = plafondSettings.subaccount_id,
                            FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                            FDNOTE = plafond?.clinote,
                            FDSHOW = true,
                            fdtiva = plafondRate?.assnatufe
                        };
                        connection.Execute(fattdRepo.INSERT_QUERY, stampRow, transaction);
                    }
                }

                transaction.Commit();
                InfoHandler.Show($"Generata correttamente la fattura {now.Year}/{newInvoiceID}");
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

    public bool GenerateByDDT(List<BOLLT00F> DDTList, int Year, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var aliquotaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();

                var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();
                var plafondRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>();
                var plafondRowsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_ROWSRepository>();
                var plafondParmsRepo = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>();

                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var caufatRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>();
                var fattdRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>();
                var ordidRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>();
                var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                foreach (var customersGroup in DDTList.GroupBy(g => new { g.BTCODC }).Select(s => s.ToList()))
                {
                    var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(customersGroup?.FirstOrDefault()?.bolsoc ?? string.Empty, (customersGroup?.FirstOrDefault()?.BTCODC ?? 0));

                    if (cliammi != null && cliammi.CLRAGF && !cliammi.CLRAGFD)
                    {
                        // group invoices by CUSTOMER
                        foreach (var group in DDTList.Where(w => w.BTCODC == (customersGroup?.FirstOrDefault()?.BTCODC ?? 0)).GroupBy(g => new { g.BTCODC, g.BTCAUS, g.BTPAGA, g.abiabi, g.abicab })
                                                    .Select(s => s.ToList()))
                        {
                            var ddt = group.FirstOrDefault();

                            if (ddt != null)
                            {
                                var newInvoiceID = numRegRepository.GetNumber(ddt!.bolsoc, Year, Constants.INVOICE_TEMP, true);
                                var invoiceCausal = caufatRepo.Get(ddt.Causal?.bolfac ?? string.Empty);
                                var rows = new List<BOLLD00F>();
                                foreach (var item in group)
                                {
                                    rows.AddRange(item.Rows ?? new ObservableCollection<BOLLD00F>());
                                    // update DDT status
                                    item.BTSTATO = "F";
                                    connection.Execute(bolltRepo.UPDATE_QUERY, item, transaction);
                                }

                                // generate invoice head
                                var newHead = new FATTT00F()
                                {
                                    ftsoci = ddt.bolsoc,
                                    FTANNO = Year,
                                    FTNUOR = newInvoiceID,
                                    FTTIPO = "F",
                                    FTDAOR = now.Date,
                                    FTCAUS = invoiceCausal?.fatcod,
                                    FTCODC = ddt.BTCODC,
                                    FTCODD = ddt.BTCODD,
                                    FTPAGA = ddt.BTPAGA,
                                    FTABIB = ddt.abiabi,
                                    FTCABB = ddt.abicab,
                                    FTCONS = ddt.BTCONS,
                                    FTSPED = ddt.BTSPED,
                                    FTCORR = ddt.BTCORR,
                                    FTCORR2 = ddt.BTCORR2,
                                    FTCIDI = "UIC",
                                    ftciva = "EUR",
                                    FTIMBL = ddt.BTIMBA,
                                    ftling = ddt.BTLING,
                                    addedUserID = UserID,
                                    FTNOTET = null,
                                    FTNOTEP = null,
                                    FTSHOWT = false,
                                    FTSHOWP = false,
                                    FTDE25 = ddt.BTDE25,
                                    FTBCON = ddt.BTBCON,
                                    FTSCCL = ddt.BTSCCL,
                                    FTAREA = ddt.BTAREA,
                                    FTRIVE = ddt.BTRIVE,
                                    FTCONZ = ddt.BTCONZ,
                                    FTFILI = ddt.BTFILI,
                                    FTZONA = ddt.BTZONA,
                                    FTSETM = ddt.BTSETM,
                                    FTREGI = ddt.BTREGI,
                                    fttdoc = invoiceCausal?.fattido
                                };
                                connection.Execute(INSERT_QUERY, newHead, transaction);

                                // generate rows
                                decimal plafondTotal = 0;
                                int sequence = 1;
                                foreach (var row in rows)
                                {
                                    var rate = aliquotaRepo.Get(row.boasso ?? string.Empty, row.boaliq ?? string.Empty);
                                    if (rate?.assplaBool ?? false)
                                        plafondTotal += row.Amount;
                                    var newRow = new FATTD00F()
                                    {
                                        ftsoci = ddt.bolsoc,
                                        FTANNO = Year,
                                        FTNUOR = newInvoiceID,
                                        FDRIGA = sequence++,
                                        FDCODA = row.BOCODA,
                                        FDQTAV = row.BOQTAV,
                                        FDTQTA = row.BOTQTA,
                                        FDPREZ = row.boprez,
                                        FDTPRE = row.botpre,
                                        FDSCO1 = row.bosco1,
                                        FDSCO2 = row.bosco2,
                                        FDSCO3 = row.bosco3,
                                        FDMAGG = row.bomagg,
                                        FDTSC1 = row.botsc1,
                                        FDTSC2 = row.botsc2,
                                        FDTSC3 = row.botsc3,
                                        FDTMAG = row.botmag,
                                        FDASSF = row.boasso,
                                        FDALIV = row.boaliq,
                                        FDDACO = row.BODACO,
                                        FDRIFC = row.BORIFC,
                                        FDGRUP = row.bogrup,
                                        FDCONT = row.bocont,
                                        FDSCTO = row.bosotc,
                                        FDUNIM = row.BOUNIM,
                                        FDBONO = row.BTANNO,
                                        FDBOLL = row.BTBOLL,
                                        FDBORI = row.BORIGB,
                                        OTANN1 = row.BOANNO,
                                        OTNUO1 = row.BONUOR,
                                        ODRIG1 = row.BORIGA,
                                        FDNOTE = row.BOSHOW ? row.BONOTE : null,
                                        FDSHOW = row.BOSHOW,
                                        FDCOAG1 = row.BOCOA1,
                                        FDCOAG2 = row.BOCOA2,
                                        FDPROV = row.BOCOA1P,
                                        fdpro2 = row.BOCOA2P,
                                        FDCOAG1PT = row.BOCOA1PT,
                                        FDCOAG2PT = row.BOCOA2PT,
                                        FDSERI = row.BOSERI,
                                        fdtiva = rate?.assnatufe
                                    };
                                    connection.Execute(fattdRepo.INSERT_QUERY, newRow, transaction);
                                }

                                // check for plafond and stamp
                                if (plafondTotal > 0)
                                {
                                    var plafondSettings = plafondParmsRepo.Get(ddt.bolsoc);
                                    if (plafondTotal > plafondSettings?.limit_amount)
                                    {
                                        // add stamp row
                                        var plafond = plafondRepo.GetLast(ddt.bolsoc, ddt.BTCODC ?? 0, Year, now.Date, false);
                                        var plafondRate = aliquotaRepo.Get(plafondSettings.rate_code, plafondSettings.rate_value);
                                        var stampRow = new FATTD00F()
                                        {
                                            ftsoci = ddt.bolsoc,
                                            FTANNO = Year,
                                            FTNUOR = newInvoiceID,
                                            FDRIGA = 999999,
                                            FDCODA = plafondSettings.product_id,
                                            FDQTAV = 1,
                                            FDTQTA = "B",
                                            FDPREZ = plafondSettings.stamp_amount,
                                            FDTPRE = "U",
                                            FDUNIM = plafondSettings.UM,
                                            FDASSF = plafondSettings.rate_code,
                                            FDALIV = plafondSettings.rate_value,
                                            FDGRUP = plafondSettings.group_id,
                                            FDCONT = plafondSettings.account_id,
                                            FDSCTO = plafondSettings.subaccount_id,
                                            FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                                            FDNOTE = plafond?.clinote,
                                            FDSHOW = true,
                                            fdtiva = plafondRate?.assnatufe
                                        };
                                        connection.Execute(fattdRepo.INSERT_QUERY, stampRow, transaction);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (cliammi != null && cliammi.CLRAGF && cliammi.CLRAGFD)
                        {
                            // group invoices by CUSTOMER AND RECIPIENT
                            foreach (var group in DDTList.Where(w => w.BTCODC == (customersGroup?.FirstOrDefault()?.BTCODC ?? 0)).GroupBy(g => new { g.BTCODC, g.BTCODD, g.BTCAUS, g.BTPAGA, g.abiabi, g.abicab })
                                                        .Select(s => s.ToList()))
                            {
                                var ddt = group.First();

                                if (ddt != null)
                                {
                                    var newInvoiceID = numRegRepository.GetNumber(ddt.bolsoc, Year, Constants.INVOICE_TEMP, true);
                                    var invoiceCausal = caufatRepo.Get(ddt.Causal?.bolfac ?? string.Empty);
                                    var rows = new List<BOLLD00F>();
                                    foreach (var item in group)
                                    {
                                        rows.AddRange(item.Rows ?? new ObservableCollection<BOLLD00F>());
                                        // update DDT status
                                        item.BTSTATO = "F";
                                        connection.Execute(bolltRepo.UPDATE_QUERY, item, transaction);
                                    }

                                    // compute head and footer notes
                                    var headNotes = string.Join("\n", group.Select(s => s.BTNOTET));
                                    var footNotes = string.Join("\n", group.Select(s => s.BTNOTEP));

                                    // generate invoice head
                                    var newHead = new FATTT00F()
                                    {
                                        ftsoci = ddt.bolsoc,
                                        FTANNO = Year,
                                        FTNUOR = newInvoiceID,
                                        FTTIPO = "F",
                                        FTDAOR = now.Date,
                                        FTCAUS = invoiceCausal?.fatcod,
                                        FTCODC = ddt.BTCODC,
                                        FTCODD = ddt.BTCODD,
                                        FTPAGA = ddt.BTPAGA,
                                        FTABIB = ddt.abiabi,
                                        FTCABB = ddt.abicab,
                                        FTCONS = ddt.BTCONS,
                                        FTSPED = ddt.BTSPED,
                                        FTCORR = ddt.BTCORR,
                                        FTCORR2 = ddt.BTCORR2,
                                        FTCIDI = "UIC",
                                        ftciva = "EUR",
                                        FTIMBL = ddt.BTIMBA,
                                        ftling = ddt.BTLING,
                                        addedUserID = UserID,
                                        FTNOTET = null,
                                        FTNOTEP = null,
                                        FTSHOWT = false,
                                        FTSHOWP = false,
                                        FTDE25 = ddt.BTDE25,
                                        FTBCON = ddt.BTBCON,
                                        FTSCCL = ddt.BTSCCL,
                                        FTAREA = ddt.BTAREA,
                                        FTRIVE = ddt.BTRIVE,
                                        FTCONZ = ddt.BTCONZ,
                                        FTFILI = ddt.BTFILI,
                                        FTZONA = ddt.BTZONA,
                                        FTSETM = ddt.BTSETM,
                                        FTREGI = ddt.BTREGI,
                                        fttdoc = invoiceCausal?.fattido
                                    };
                                    connection.Execute(INSERT_QUERY, newHead, transaction);

                                    // generate rows
                                    decimal plafondTotal = 0;
                                    int sequence = 1;
                                    foreach (var row in rows)
                                    {
                                        var rate = aliquotaRepo.Get(row.boasso ?? string.Empty, row.boaliq ?? string.Empty);
                                        if (rate?.assplaBool ?? false)
                                            plafondTotal += row.Amount;
                                        var newRow = new FATTD00F()
                                        {
                                            ftsoci = ddt.bolsoc,
                                            FTANNO = Year,
                                            FTNUOR = newInvoiceID,
                                            FDRIGA = sequence++,
                                            FDCODA = row.BOCODA,
                                            FDQTAV = row.BOQTAV,
                                            FDTQTA = row.BOTQTA,
                                            FDPREZ = row.boprez,
                                            FDTPRE = row.botpre,
                                            FDSCO1 = row.bosco1,
                                            FDSCO2 = row.bosco2,
                                            FDSCO3 = row.bosco3,
                                            FDMAGG = row.bomagg,
                                            FDTSC1 = row.botsc1,
                                            FDTSC2 = row.botsc2,
                                            FDTSC3 = row.botsc3,
                                            FDTMAG = row.botmag,
                                            FDASSF = row.boasso,
                                            FDALIV = row.boaliq,
                                            FDDACO = row.BODACO,
                                            FDRIFC = row.BORIFC,
                                            FDGRUP = row.bogrup,
                                            FDCONT = row.bocont,
                                            FDSCTO = row.bosotc,
                                            FDUNIM = row.BOUNIM,
                                            FDBONO = row.BTANNO,
                                            FDBOLL = row.BTBOLL,
                                            FDBORI = row.BORIGB,
                                            OTANN1 = row.BOANNO,
                                            OTNUO1 = row.BONUOR,
                                            ODRIG1 = row.BORIGA,
                                            FDNOTE = row.BOSHOW ? row.BONOTE : null,
                                            FDSHOW = row.BOSHOW,
                                            FDCOAG1 = row.BOCOA1,
                                            FDCOAG2 = row.BOCOA2,
                                            FDPROV = row.BOCOA1P,
                                            fdpro2 = row.BOCOA2P,
                                            FDCOAG1PT = row.BOCOA1PT,
                                            FDCOAG2PT = row.BOCOA2PT,
                                            FDSERI = row.BOSERI,
                                            fdtiva = rate?.assnatufe
                                        };
                                        connection.Execute(fattdRepo.INSERT_QUERY, newRow, transaction);
                                    }

                                    // check for plafond and stamp
                                    if (plafondTotal > 0)
                                    {
                                        var plafondSettings = plafondParmsRepo.Get(ddt.bolsoc);
                                        if (plafondTotal > plafondSettings?.limit_amount)
                                        {
                                            // add stamp row
                                            var plafond = plafondRepo.GetLast(ddt.bolsoc, ddt.BTCODC ?? 0, Year, now.Date, false);
                                            var plafondRate = aliquotaRepo.Get(plafondSettings.rate_code, plafondSettings.rate_value);
                                            var stampRow = new FATTD00F()
                                            {
                                                ftsoci = ddt.bolsoc,
                                                FTANNO = Year,
                                                FTNUOR = newInvoiceID,
                                                FDRIGA = 999999,
                                                FDCODA = plafondSettings.product_id,
                                                FDQTAV = 1,
                                                FDTQTA = "B",
                                                FDPREZ = plafondSettings.stamp_amount,
                                                FDTPRE = "U",
                                                FDUNIM = plafondSettings.UM,
                                                FDASSF = plafondSettings.rate_code,
                                                FDALIV = plafondSettings.rate_value,
                                                FDGRUP = plafondSettings.group_id,
                                                FDCONT = plafondSettings.account_id,
                                                FDSCTO = plafondSettings.subaccount_id,
                                                FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                                                FDNOTE = plafond?.clinote,
                                                FDSHOW = true,
                                                fdtiva = plafondRate?.assnatufe
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().INSERT_QUERY, stampRow, transaction);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // do not group invoices
                            foreach (var ddt in DDTList.Where(w => w.BTCODC == (customersGroup?.FirstOrDefault()?.BTCODC ?? 0)))
                            {
                                var newInvoiceID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(ddt.bolsoc, Year, Constants.INVOICE_TEMP, true);
                                var invoiceCausal = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Get(ddt.Causal?.bolfac ?? string.Empty);

                                // generate invoice head
                                var newHead = new FATTT00F()
                                {
                                    ftsoci = ddt.bolsoc,
                                    FTANNO = Year,
                                    FTNUOR = newInvoiceID,
                                    FTTIPO = "F",
                                    FTDAOR = now.Date,
                                    FTCAUS = invoiceCausal?.fatcod,
                                    FTCODC = ddt.BTCODC,
                                    FTCODD = ddt.BTCODD,
                                    FTPAGA = ddt.BTPAGA,
                                    FTABIB = ddt.abiabi,
                                    FTCABB = ddt.abicab,
                                    FTCONS = ddt.BTCONS,
                                    FTSPED = ddt.BTSPED,
                                    FTCORR = ddt.BTCORR,
                                    FTCORR2 = ddt.BTCORR2,
                                    FTCIDI = "UIC",
                                    ftciva = "EUR",
                                    FTIMBL = ddt.BTIMBA,
                                    ftling = ddt.BTLING,
                                    addedUserID = UserID,
                                    FTNOTET = ddt.BTSHOWT ? ddt.BTNOTET : null,
                                    FTNOTEP = ddt.BTSHOWP ? ddt.BTNOTEP : null,
                                    FTSHOWT = ddt.BTSHOWT,
                                    FTSHOWP = ddt.BTSHOWP,
                                    FTDE25 = ddt.BTDE25,
                                    FTBCON = ddt.BTBCON,
                                    FTSCCL = ddt.BTSCCL,
                                    FTAREA = ddt.BTAREA,
                                    FTRIVE = ddt.BTRIVE,
                                    FTCONZ = ddt.BTCONZ,
                                    FTFILI = ddt.BTFILI,
                                    FTZONA = ddt.BTZONA,
                                    FTSETM = ddt.BTSETM,
                                    FTREGI = ddt.BTREGI,
                                    fttdoc = invoiceCausal?.fattido
                                };
                                connection.Execute(INSERT_QUERY, newHead, transaction);

                                // generate rows
                                decimal plafondTotal = 0;
                                int sequence = 1;
                                foreach (var row in ddt.Rows ?? new ObservableCollection<BOLLD00F>())
                                {
                                    var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.boasso ?? string.Empty, row.boaliq ?? string.Empty);
                                    if (rate?.assplaBool ?? false)
                                        plafondTotal += row.Amount;
                                    var newRow = new FATTD00F()
                                    {
                                        ftsoci = ddt.bolsoc,
                                        FTANNO = Year,
                                        FTNUOR = newInvoiceID,
                                        FDRIGA = sequence++,
                                        FDCODA = row.BOCODA,
                                        FDQTAV = row.BOQTAV,
                                        FDTQTA = row.BOTQTA,
                                        FDPREZ = row.boprez,
                                        FDTPRE = row.botpre,
                                        FDSCO1 = row.bosco1,
                                        FDSCO2 = row.bosco2,
                                        FDSCO3 = row.bosco3,
                                        FDMAGG = row.bomagg,
                                        FDTSC1 = row.botsc1,
                                        FDTSC2 = row.botsc2,
                                        FDTSC3 = row.botsc3,
                                        FDTMAG = row.botmag,
                                        FDASSF = row.boasso,
                                        FDALIV = row.boaliq,
                                        FDDACO = row.BODACO,
                                        FDRIFC = row.BORIFC,
                                        FDGRUP = row.bogrup,
                                        FDCONT = row.bocont,
                                        FDSCTO = row.bosotc,
                                        FDUNIM = row.BOUNIM,
                                        FDBONO = row.BTANNO,
                                        FDBOLL = row.BTBOLL,
                                        FDBORI = row.BORIGB,
                                        OTANN1 = row.BOANNO,
                                        OTNUO1 = row.BONUOR,
                                        ODRIG1 = row.BORIGA,
                                        FDNOTE = row.BOSHOW ? row.BONOTE : null,
                                        FDSHOW = row.BOSHOW,
                                        FDCOAG1 = row.BOCOA1,
                                        FDCOAG2 = row.BOCOA2,
                                        FDPROV = row.BOCOA1P,
                                        fdpro2 = row.BOCOA2P,
                                        FDCOAG1PT = row.BOCOA1PT,
                                        FDCOAG2PT = row.BOCOA2PT,
                                        FDSERI = row.BOSERI,
                                        fdtiva = rate?.assnatufe
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().INSERT_QUERY, newRow, transaction);
                                }

                                // update DDT status
                                ddt.BTSTATO = "F";
                                connection.Execute(bolltRepo.UPDATE_QUERY, ddt, transaction);

                                // check for plafond and stamp
                                if (plafondTotal > 0)
                                {
                                    var plafondSettings = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Get(ddt.bolsoc);
                                    if (plafondTotal > plafondSettings?.limit_amount)
                                    {
                                        // add stamp row
                                        var plafond = VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().GetLast(ddt.bolsoc, ddt.BTCODC ?? 0, Year, now.Date, false);

                                        if (plafond == null)
                                        {
                                            ErrorHandler.Show("Impossibile generare: plafond non trovato");
                                            return false;
                                        }

                                        var plafondRate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(plafondSettings.rate_code, plafondSettings.rate_value);
                                        var stampRow = new FATTD00F()
                                        {
                                            ftsoci = ddt.bolsoc,
                                            FTANNO = Year,
                                            FTNUOR = newInvoiceID,
                                            FDRIGA = 999999,
                                            FDCODA = plafondSettings.product_id,
                                            FDQTAV = 1,
                                            FDTQTA = "B",
                                            FDPREZ = plafondSettings.stamp_amount,
                                            FDTPRE = "U",
                                            FDUNIM = plafondSettings.UM,
                                            FDASSF = plafondSettings.rate_code,
                                            FDALIV = plafondSettings.rate_value,
                                            FDGRUP = plafondSettings.group_id,
                                            FDCONT = plafondSettings.account_id,
                                            FDSCTO = plafondSettings.subaccount_id,
                                            FDSTAMP = cliammi != null ? (cliammi.CLASBOBool ? 1 : 2) : 1,
                                            FDNOTE = plafond.clinote,
                                            FDSHOW = true,
                                            fdtiva = plafondRate?.assnatufe
                                        };
                                        connection.Execute(fattdRepo.INSERT_QUERY, stampRow, transaction);
                                    }
                                }
                            }
                        }
                    }
                }
                transaction.Commit();
                InfoHandler.Show($"Fatture generate correttamente");
                return true;
            }
            catch (Exception exc)
            {
                transaction.Rollback();
                ErrorHandler.Show(exc.Message);
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

    #region Printing
    public FATTT00F? GetPrintFull(string ftsoci, int FTANNO, int FTNUOR, bool PrintProductNote, bool PrintAgentsInDetails, bool UseCustomerCodes)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Query<FATTT00F>(@"SELECT i.*, c.*, si.*, d.*, ca.fatcod,ca.fatpre,ca.fatnmr,ca.fatdes,iso.isolin FROM FATTT00F AS i
                        INNER JOIN ANAG_BASE AS c ON c.abecod = i.FTCODC
                        LEFT OUTER JOIN ISO AS iso ON iso.isocod = c.isocod
                        LEFT JOIN CAUFAT00F AS ca ON ca.fatcod = i.FTCAUS
                        LEFT JOIN DESTINATARI AS d ON d.cliecod = i.FTCODC AND d.codesti = i.FTCODD
                        LEFT OUTER JOIN FATTAUT AS si ON si.FTAUSC = i.ftsoci AND si.FTAUAN = i.FTANNO AND si.FTAUNUM = i.FTNUOR
                        WHERE i.ftsoci = @ftsoci AND i.FTANNO = @FTANNO AND i.FTNUOR = @FTNUOR", new[]
            { typeof(FATTT00F), typeof(ABE), typeof(FATTAUT), typeof(DESTINATARI), typeof(CAUFAT00F), typeof(string), }, (objects) =>
            {
                var fattt00f = (FATTT00F)objects[0];
                fattt00f.Customer = objects[1] as ABE;
                fattt00f.SelfInvoice = objects[2] as FATTAUT;
                fattt00f.Recipient = objects[3] as DESTINATARI;
                fattt00f.Causal = objects[4] as CAUFAT00F;
                fattt00f.Language = objects[5] as string;

                return fattt00f;
            }, new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR }, splitOn: "abecod,FTAUSC,cliecod,fatcod,isolin").FirstOrDefault();

            if (result != null)
            {
                var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(result.Language ?? string.Empty);

                //Tipo fattura

                if (languageDictionary != null)
                {
                    switch (result.FTTIPO)
                    {
                        case ("N"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_NotaCredito"].ToString();
                            break;
                        case ("A"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Autofattura"].ToString();
                            break;
                        case ("C"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Autonotacredito"].ToString();
                            break;
                        case ("P"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Parcella"].ToString();
                            break;
                        case ("B"):
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Notadebito"].ToString();
                            break;
                        default:
                            result.FTTIPODescriptionReport = languageDictionary["TipoFattura_Fattura"].ToString();
                            break;
                    }
                }

                var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(ftsoci);
                result.Rows = new ObservableCollection<FATTD00F>(
                    connection.Query<FATTD00F>(
                        $@"SELECT r.*, 
                            (
                                SELECT TOP(1) TRIM(customerProductID) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND l.unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductID,
                            (
                                SELECT TOP(1) TRIM(customerProductDescription) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND l.unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductDescription,
                            (CASE WHEN asspla = 'S' THEN 1 ELSE 0 END) AS HasPlafond,
                            {(PrintProductNote ? " 1 AS PrintProductNote" : " 0 AS PrintProductNote")}, p.*, u.*, a1.agecod, a1.agedes, a2.agecod, a2.agedes, ddt.bolsoc,ddt.BTNUBD,ddt.BTDATA, ddt.BTCODD, dest.ragisoc, dest.DEINDI, dest.deloc, dest.depro,ord.OTSOCI,ord.OTDAOR,ord.OTDE25,ord.OTCUNO,ord.OTCUDO,l.*
                            FROM FATTD00F AS r
                            INNER JOIN FATTT00F AS h ON h.ftsoci=r.ftsoci AND h.FTANNO = r.FTANNO AND h.FTNUOR=r.FTNUOR
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_lingua AS l ON p.SocietaID = l.SocietaID AND p.ID = l.ArticoloID AND l.lincod = @Lingua
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            LEFT OUTER JOIN AGENTI AS a1 ON a1.agecod=r.FDCOAG1
                            LEFT OUTER JOIN AGENTI AS a2 ON a2.agecod=r.FDCOAG2
                            LEFT JOIN BOLLT00F as ddt ON ddt.bolsoc=r.ftsoci AND ddt.BTANNO = r.FDBONO AND ddt.BTBOLL = r.FDBOLL
                            LEFT JOIN ORDIT00F as ord ON ord.OTSOCI=r.ftsoci AND ord.OTANNO = r.OTANN1 AND ord.OTNUOR = r.OTNUO1
                            LEFT JOIN DESTINATARI AS dest ON dest.cliecod=ddt.BTCODC AND dest.codesti=ddt.BTCODD
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR
                            ORDER BY r.FDBONO, r.FDBOLL, r.OTANN1, r.OTNUO1",
                        new[] { typeof(FATTD00F), typeof(tab_articolo), typeof(tab_articolo_unita), typeof(AGENTI), typeof(AGENTI), typeof(BOLLT00F), typeof(DESTINATARI), typeof(ORDIT00F), typeof(tab_articolo_lingua) },
                        (objs) =>
                        {
                            var obj = (FATTD00F)objs[0];
                            obj.UMsCache = umsCache;
                            obj.Product = (tab_articolo)objs[1];
                            obj.Product.Descrizione = (objs[8] as tab_articolo_lingua != null && !string.IsNullOrEmpty((objs[8] as tab_articolo_lingua)?.Descrizione) ? (objs[8] as tab_articolo_lingua)?.Descrizione : obj.Product.Descrizione) ?? string.Empty;
                            obj.Product.Note = (objs[8] as tab_articolo_lingua != null && !string.IsNullOrEmpty((objs[8] as tab_articolo_lingua)?.Note) ? (objs[8] as tab_articolo_lingua)?.Note : obj.Product.Note);
                            obj.UM = objs[2] as tab_articolo_unita;
                            obj.FirstAgent = objs[3] as AGENTI;
                            obj.SecondAgent = objs[4] as AGENTI;
                            obj.PrintAgentsInDetails = PrintAgentsInDetails && (obj.FirstAgent != null || obj.SecondAgent != null);
                            obj.LinkedDDT = objs[5] as BOLLT00F;
                            obj.RecipientDescription = (objs[6] as DESTINATARI)?.FullRecipientText;
                            obj.LinkedOrder = objs[7] as ORDIT00F;
                            obj.CustomerDiscount = result.FTSCCL ?? 0;
                            return obj;
                        },
                        new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR, Lingua = result.Language }, splitOn: "SocietaID,SocietaID,agecod,agecod,bolsoc,ragisoc,OTSOCI,SocietaID").ToList());

                // check where print agents
                if (PrintAgentsInDetails)
                {
                    // on details rows
                    result.PrintAgentsInDetails = true;
                }
                else
                {
                    // on header, take first row agent
                    var agent1 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG1 ?? string.Empty);
                    var agent2 = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().Get(result.Rows.OrderBy(o => o.FDRIGA).FirstOrDefault()?.FDCOAG2 ?? string.Empty);
                    result.DefaultFirstAgent = agent1;
                    result.DefaultSecondAgent = agent2;
                }

                // check if custom code and UMs recap for each row
                int? lastYear = null;
                int? lastNumber = null;
                int? lastODAYear = null;
                int? lastODANumber = null;
                var customization = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(ftsoci);


                foreach (var row in result.Rows)
                {
                    #region DDT grouping
                    if (lastYear != row.FDBONO || lastNumber != row.FDBOLL)
                    {
                        row.DDTReferenceText = $"{languageDictionary?["RiferimentoDDT"].ToString()} {row.FDBONO}/{row.LinkedDDT?.BTNUBD} {languageDictionary?["Del"].ToString()} {(row.LinkedDDT?.BTDATA ?? DateTime.MinValue).ToString("dd/MM/yyyy")}{(customization?.azinvshde ?? false ? $" - {languageDictionary?["DestinatarioBreve"].ToString()} {row.RecipientDescription}" : null)}";
                        lastYear = row.FDBONO;
                        lastNumber = row.FDBOLL;
                        lastODAYear = null;
                        lastODANumber = null;
                    }
                    #endregion
                    #region ODA grouping
                    if (lastODAYear != row.OTANN1 || lastODANumber != row.OTNUO1)
                    {
                        row.ODAReferenceText = $"{languageDictionary?["RiferimentoODA"].ToString()} {row.OTANN1}/{row.OTNUO1} {languageDictionary?["Del"].ToString()} {(row.LinkedOrder?.OTDAOR ?? DateTime.MinValue).ToString("dd/MM/yyyy")}";
                        // check if customization need to print customer ref from order
                        if (customization?.azinvricl ?? false)
                        {
                            if (!string.IsNullOrWhiteSpace(row.LinkedOrder?.OTCUNO) && row.LinkedOrder.OTCUDO.HasValue)
                                row.ODAReferenceText += $" [{languageDictionary?["VostroOrdine"].ToString()}{row.LinkedOrder.OTCUNO.Trim()} {languageDictionary?["Del"].ToString()} {(row.LinkedOrder.OTCUDO ?? DateTime.MinValue).ToString("dd/MM/yyyy")}]";
                            else
                                row.ODAReferenceText += $" [{row.LinkedOrder?.OTDE25?.Trim()}]";
                        }
                        lastODAYear = row.OTANN1;
                        lastODANumber = row.OTNUO1;
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
                                row.FDCODA = row.CustomerProductID;
                            if (row.Product != null && !string.IsNullOrWhiteSpace(row.CustomerProductDescription))
                                row.Product.Descrizione = row.CustomerProductDescription;
                        }
                    }
                    else
                    {
                        row.CustomerCode = null;
                    }
                    #endregion
                }
                result.Attachments = new ObservableCollection<FATTAL00F>(connection.Query<FATTAL00F>(
                        @"SELECT * FROM FATTAL00F
                            WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                            ORDER BY FTANAME",
                        new { FTASOCI = ftsoci, FTAANNO = FTANNO, FTANUOR = FTNUOR }).ToList());

            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
 
    public InvoiceReport? PrintInvoice(FATTT00F Invoice)
    {
        try
        {
            // set foot notes on rows to print it
            if (Invoice.FTSHOWP)
            {
                foreach (var row in Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                { row.HeaderFootNote = Invoice.FTNOTEP; }
            }
            #region Expires
            var expires = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().ComputeExpires(Invoice.ftsoci, Invoice.FTPAGA ?? string.Empty, Invoice.FTDAOR ?? DateTime.MinValue, Invoice.FTCODC ?? 0);
            decimal stepAmount = Math.Round(Invoice.GrandTotal / (expires?.Count ?? 1), 2);
            decimal finalStepAmount = Math.Round((stepAmount + (Invoice.GrandTotal - (stepAmount * (expires?.Count ?? 1)))), 2);
            int i = 1;
            var expireList = new List<Tuple<string, string>>();
            foreach (var exp in expires ?? new List<DateTime>())
            {
                expireList.Add(new Tuple<string, string>(exp.Date.ToString("dd/MM/yyyy"), (i == (expires?.Count ?? 1) ? $"{finalStepAmount.ToString("N2")} €" : $"{stepAmount.ToString("N2")} €")));
                i++;
            }
            #endregion
            #region Rates
            i = 1;
            List<string> rates = new List<string>();
            var ratesList = new List<Tuple<string, string, string, string>>();
            var ratesList2 = new List<Tuple<string, string, string, string>>();
            foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).OrderBy(o => o.FDASSF).ThenBy(o => o.FDALIV))
            {
                if (!rates.Contains(row.FDASSF + row.FDALIV?.Trim()))
                {
                    var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.FDASSF ?? string.Empty, row.FDALIV ?? string.Empty);
                    rates.Add(row.FDASSF + row.FDALIV?.Trim());
                    var imponibile = Math.Round((Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                    .Where(w => w.FDASSF == row.FDASSF && w.FDALIV == row.FDALIV)
                    .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                    // compute customer discount
                    imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                    decimal rateValue = 0;
                    decimal.TryParse(rate?.assali, out rateValue);
                    var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                    if (i % 2 == 0)
                        ratesList2.Add(new Tuple<string, string, string, string>(row.FDASSF + " " + row.FDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                    else
                        ratesList.Add(new Tuple<string, string, string, string>(row.FDASSF + " " + row.FDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                    i++;
                }
            }
            #endregion

            if (Invoice.HasGifts)
            {
                #region Gifts rates recap
                i = 1;
                List<string> giftsRates = new List<string>();
                var giftsRatesList = new List<Tuple<string, string, string, string>>();
                foreach (var row in (Invoice.Rows ?? new ObservableCollection<FATTD00F>()).Where(w => w.FDTQTA == "O").OrderBy(o => o.FDASSF).ThenBy(o => o.FDALIV))
                {
                    if (!giftsRates.Contains(row.FDASSF + row.FDALIV?.Trim()))
                    {
                        var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.FDASSF ?? string.Empty, row.FDALIV ?? string.Empty);
                        giftsRates.Add(row.FDASSF + row.FDALIV?.Trim());
                        var imponibile = Math.Round((Invoice.Rows ?? new ObservableCollection<FATTD00F>())
                        .Where(w => w.FDTQTA == "O" && w.FDASSF == row.FDASSF && w.FDALIV == row.FDALIV)
                        .Sum(sum => sum.NetPrice), 2, MidpointRounding.AwayFromZero);
                        // compute customer discount
                        imponibile = imponibile - (imponibile * (Invoice.FTSCCL ?? 0) / 100);
                        decimal rateValue = 0;
                        decimal.TryParse(rate?.assali, out rateValue);
                        var imposta = Math.Round(imponibile * rateValue / 100, 2, MidpointRounding.AwayFromZero);
                        giftsRatesList.Add(new Tuple<string, string, string, string>(row.FDASSF + " " + row.FDALIV, rate?.assdes ?? string.Empty, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                        i++;
                    }
                }
                Invoice.GiftsRatesRecap = giftsRatesList;
                #endregion
            }

            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(Invoice.ftsoci)!;
            var bank = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(Invoice.FTABIB ?? 0, Invoice.FTCABB ?? 0);
            var companyBank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Invoice.ftsoci, Invoice.FTABIB ?? 0, Invoice.FTCABB ?? 00, Invoice.FTBCON ?? string.Empty);
            var customerData = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Invoice.ftsoci, Invoice.FTCODC ?? 0);
            // get customizations
            var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Invoice.ftsoci)!;
            var aziendaLingua = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(Invoice.ftsoci, Invoice.Language ?? string.Empty);

            var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(Invoice.FTPAGA ?? string.Empty);
            var pagcliLingua = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLI_LINGUARepository>().Get(Invoice.FTPAGA ?? string.Empty, Invoice.Language ?? string.Empty);

            var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(Invoice.Language ?? string.Empty);

            object? objectDictionary = null;

            if (languageDictionary != null)
                objectDictionary = new LocalizationHelper().CreateClassFromDictionary(languageDictionary);

            return new InvoiceReport()
            {
                Invoice = Invoice,
                PaymentDescription = (pagcliLingua != null) ? (!string.IsNullOrEmpty(pagcliLingua.pcldes) ? pagcliLingua.pcldes : pagcli?.pcldes) : pagcli?.pcldes,
                CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Invoice.ftsoci),
                BankData = $"{bank?.FullDescriptionSearchable} c/c nr.{(companyBank != null ? Invoice.FTBCON : customerData?.CLNUCC)} IBAN: {(companyBank != null ? companyBank.abibiba : customerData?.cliban)}",
                Expires = expireList,
                Rates = ratesList,
                Rates2 = ratesList2,
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                CertificationsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}certs.png"),
                FixedText = (aziendaLingua != null) ? (!string.IsNullOrEmpty(aziendaLingua.azinvgtex) ? aziendaLingua.azinvgtex : azienda.azinvgtex) : azienda.azinvgtex,
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
    public string INSERT_QUERY => "INSERT INTO FATTT00F (ftsoci,FTANNO,FTNUOR,FTTIPO,FTNUFD,FTDAOR,FTCAUS,FTCODC,FTCODD,FTCONZ,FTPAGA,FTABIB,FTCABB,FTCONS,FTSPED,FTCORR,FTAREA,FTFILI,FTZONA,FTSETM,FTCIDI,FTDE25,FTFLA1,FTFLA2,FTFLA3,FTNURI,FTDARI,FTBOLL,FTIMBL,ftciva,ftdao2,FTSCAD,ftling,FTDATFEL,FTNUMFEL,ftdafa,ftnufa,ftannf,fttdoc,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote, ftcoag, ftagecod2,FTCLSO, fattmpag) OUTPUT INSERTED.rv VALUES(@ftsoci,@FTANNO,@FTNUOR,@FTTIPO,@FTNUFD,@FTDAOR,@FTCAUS,@FTCODC,@FTCODD,@FTCONZ,@FTPAGA,@FTABIB,@FTCABB,@FTCONS,@FTSPED,@FTCORR,@FTAREA,@FTFILI,@FTZONA,@FTSETM,@FTCIDI,@FTDE25,@FTFLA1,@FTFLA2,@FTFLA3,@FTNURI,@FTDARI,@FTBOLL,@FTIMBL,@ftciva,@ftdao2,@FTSCAD,@ftling,@FTDATFEL,@FTNUMFEL,@ftdafa,@ftnufa,@ftannf,@fttdoc,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote, @ftcoag, @ftagecod2, @FTCLSO,@fattmpag)";
    public string UPDATE_QUERY => "UPDATE FATTT00F SET ftsoci = @ftsoci,FTANNO = @FTANNO,FTNUOR = @FTNUOR,FTTIPO = @FTTIPO,FTNUFD = @FTNUFD,FTDAOR = @FTDAOR,FTCAUS = @FTCAUS,FTCODC = @FTCODC,FTCODD = @FTCODD,FTCONZ = @FTCONZ,FTPAGA = @FTPAGA,FTABIB = @FTABIB,FTCABB = @FTCABB,FTCONS = @FTCONS,FTSPED = @FTSPED,FTCORR = @FTCORR,FTAREA = @FTAREA,FTFILI = @FTFILI,FTZONA = @FTZONA,FTSETM = @FTSETM,FTCIDI = @FTCIDI,FTDE25 = @FTDE25,FTFLA1 = @FTFLA1,FTFLA2 = @FTFLA2,FTFLA3 = @FTFLA3,FTNURI = @FTNURI,FTDARI = @FTDARI,FTBOLL = @FTBOLL,FTIMBL = @FTIMBL,ftciva = @ftciva,ftdao2 = @ftdao2,FTSCAD = @FTSCAD,ftling = @ftling,FTDATFEL = @FTDATFEL,FTNUMFEL = @FTNUMFEL,ftdafa = @ftdafa,ftnufa = @ftnufa,ftannf = @ftannf,fttdoc = @fttdoc,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote, ftcoag= @ftcoag, ftagecod2= @ftagecod2, FTCLSO = @FTCLSO, fattmpag=@fattmpag OUTPUT INSERTED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTT00F OUTPUT DELETED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND rv = @rv";
    public bool Insert(FATTT00F Model)
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

    public bool Update(FATTT00F Model)
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

    public bool Delete(FATTT00F Model)
    {
        try
        {

            if (Model.FTNUFD <= 0)
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    var invoiceRows = connection.Query<FATTD00F>(@"SELECT * FROM FATTD00F
                                                                         WHERE ftsoci = @ftsoci AND FTANNO = @ftanno AND FTNUOR = @ftnuor",
                                                                    new { ftsoci = Model.ftsoci, ftanno = Model.FTANNO, ftnuor = Model.FTNUOR }, transaction);

                    bool haveDDT = false;

                    #region Free DDT
                    if (invoiceRows.Any(any => any.FDBONO.HasValue && any.FDBOLL.HasValue && any.FDBORI.HasValue))
                    {
                        foreach (var row in invoiceRows
                                            .Where(w => w.FDBONO.HasValue && w.FDBOLL.HasValue)
                                            .GroupBy(g => new { g.FDBONO, g.FDBOLL }))
                        {
                            haveDDT = true;
                            connection.Execute($"UPDATE BOLLT00F SET BTSTATO = 'R' WHERE bolsoc=@bolsoc AND BTANNO=@btanno AND BTBOLL=@btboll",
                                            new { bolsoc = Model.ftsoci, btanno = row.Key.FDBONO, btboll = row.Key.FDBOLL }, transaction);
                        }
                    }
                    #endregion

                    #region Free orders
                    if (invoiceRows.Any(any => any.OTANN1.HasValue && any.OTNUO1.HasValue && any.ODRIG1.HasValue) && !haveDDT)
                    {
                        // free rows
                        foreach (var row in invoiceRows.Where(w => w.OTANN1.HasValue && w.OTNUO1.HasValue && w.ODRIG1.HasValue))
                        {
                            var current = connection.Query<ORDID00F>($"SELECT * FROM ORDID00F WHERE otsoci=@otsoci AND OTANNO=@otanno AND OTNUOR=@otnuor AND ODRIGA=@odriga",
                                            new { otsoci = row.ftsoci, otanno = row.OTANN1, otnuor = row.OTNUO1, odriga = row.ODRIG1 }, transaction).FirstOrDefault();

                            if (current != null)
                            {
                                var diff = (current.ODQTAEV ?? 0) - (row.FDQTAV ?? 0);
                                connection.Execute($"UPDATE ORDID00F SET ODQTAEV = @diff, ODSTATO = NULL WHERE otsoci=@otsoci AND OTANNO=@otanno AND OTNUOR=@otnuor AND ODRIGA=@odriga",
                                                new { otsoci = row.ftsoci, otanno = row.OTANN1, otnuor = row.OTNUO1, odriga = row.ODRIG1, diff = diff }, transaction);
                            }
                        }
                        // update head
                        foreach (var row in invoiceRows
                                            .Where(w => w.OTANN1.HasValue && w.OTNUO1.HasValue)
                                            .GroupBy(g => new { g.OTANN1, g.OTNUO1 }))
                        {
                            connection.Execute($"UPDATE ORDIT00F SET flgchi = 'F' WHERE otsoci=@otsoci AND OTANNO=@otanno AND OTNUOR=@otnuor",
                                            new { otsoci = Model.ftsoci, otanno = row.Key.OTANN1, otnuor = row.Key.OTNUO1 }, transaction);
                        }
                    }
                    #endregion

                    #region Free accounting registration
                    var self = connection.Query<FATTAUT>(@"SELECT * FROM FATTAUT WHERE FTAUSC = @cid AND FTAUAN = @yea AND FTAUNUM = @id",
                        new { cid = Model.ftsoci, yea = Model.FTANNO, id = Model.FTNUOR },
                        transaction).FirstOrDefault();
                    if (self != null && self.FTAPNAN.HasValue)
                    {
                        connection.Execute("UPDATE PNTESTATA SET N1AUAN=NULL, N1AUNU=NULL, N1AUGE=NULL WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1REGI=@id",
                            new { cid = Model.ftsoci, yea = self.FTAPNAN, id = self.FTAPNRE }, transaction);
                    }
                    #endregion

                    // delete FATTAUT
                    connection.Execute("DELETE FROM FATTAUT WHERE FTAUSC = @cid AND FTAUAN = @yea AND FTAUNUM = @id",
                    new { cid = Model.ftsoci, yea = Model.FTANNO, id = Model.FTNUOR },
                    transaction);
                    // delete rows 
                    connection.Execute("DELETE FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @ftanno AND FTNUOR = @ftnuor",
                    new { ftsoci = Model.ftsoci, ftanno = Model.FTANNO, ftnuor = Model.FTNUOR },
                    transaction);
                    // delete head
                    connection.Execute(DELETE_QUERY, Model, transaction);

                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {

                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
            else
            {
                ErrorHandler.Show(ERR_CANNOT_DELETE);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(FATTT00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ftsoci) && Model.FTANNO > 0 && IsInsert && !Exists(Model.ftsoci, Model.FTANNO, Model.FTNUOR)) || !IsInsert)
            {
                if (Model.FTDAOR.HasValue && Model.FTDAOR.Value.Year == Model.FTANNO)
                {
                    if (Model.FTCODC.HasValue)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.FTCAUS))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.FTPAGA))
                            {
                                if (Model.FTABIB.HasValue && Model.FTCABB.HasValue)
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.FTNOTET) || (string.IsNullOrWhiteSpace(Model.FTNOTET) && !Model.FTSHOWT))
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.FTNOTEP) || (string.IsNullOrWhiteSpace(Model.FTNOTEP) && !Model.FTSHOWP))
                                        {
                                            return null;
                                        }
                                        else
                                        { return "Impossibile stampare una nota a pie' di pagina vuota"; }
                                    }
                                    else
                                    { return "Impossibile stampare una nota di testata vuota"; }
                                }
                                else
                                { return "La banca cliente è obbligatoria"; }
                            }
                            else
                            { return "Il tipo pagamento è obbligatorio"; }
                        }
                        else
                        { return "La causale fattura è obbligatoria"; }
                    }
                    else
                    { return "Il codice cliente è obbligatorio"; }
                }
                else
                { return "La data fattura non è coerente con l'anno della stessa"; }
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