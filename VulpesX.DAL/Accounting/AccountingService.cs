using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.Auth;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models.Accounting;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared.Generics;
using static VulpesX.Models.Models.Accounting.AccountingSituationViewModel;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.DAL.Accounting
{
    public interface IAccountingRepository
    {
        ObservableCollection<PNTESTATA>? GetHeadList(string CompanyID, int Year, string? FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount);

        ObservableCollection<PNRIGHE>? GetHeadTemporaryPeriodList(string CompanyID, int Year, string? MonthFlag, DateTime? From, DateTime? To, string GroupID, string AccountID, string SubaccountID);

        bool RunEqualization(string CompanyID, string EntityType, DateTime UntilDate);

        #region Receipts registration
        string? AccountingReceipts(string CompanyID, DateTime RegistrationDate, ABE SelectedCustomer, CAUCONT SelectedCausal, string DocumentID, ObservableCollection<ReceiptInfo> Rows, decimal TotalTax, string UserID);

        string? RegistrationAccounting(string CompanyID, int AccountingYear, DateTime RegistrationDate, ABE Entity, string EntityType, CAUCONT SelectedCausal, BANAZIEN SelectedBank, string? DocumentID, DateTime? DocumentDate, ObservableCollection<MastrinoECReportItem> Rows, string UserID);
        #endregion

        #region Accounting situation

        AccountingSituationViewModel? GetAccountingSituation(string CompanyID, int Year, string? CostCenterID);

        ObservableCollection<ASItem>? GetAccountingSituationDetails(string CompanyID, int Year, string GroupID, bool IsDare, string AccountID);

        string? ExportCEEBalance(string CompanyID, int Year);
        #endregion

        #region Closing
        bool PeriodClosing(string CompanyID, int Year, DateTime LimitDate);

        bool YearClosing(string CompanyID, int Year, DateTime LimitDate, DateTime NewDate, string UserID);

        decimal ComputeLossProfit(string CompanyID, int Year);
        #endregion

        #region Self invoicing
        string? GenerateSelfInvoice(PNTESTATA Head, ESERCIZIO AccountingYear, ABE Customer, CAUFAT00F Causal, tab_articolo Product, DateTime SelectedDate, string UserID);
        #endregion

        #region Printing
        AccountingRecordReport? PrintAccountingRecord(PNTESTATA Head);


        #region General journal
        // page break vars
        GeneralJournalReport? PrintGeneralJournal(string CompanyID, int AccountingYear, DateTime PrintUntil);
        bool UpdateJournalDefinitives(GeneralJournalReport ReportData, int LastPagePrinted, string CompanyID, string UserID);
        void AddJournalRow(List<GeneralJournalReport.RowInfo> displayRows, GeneralJournalReport.RowInfo row, ref decimal dareTop, ref decimal avereTop, ref decimal spaceUsed);
        #endregion
        #endregion
    }

    public class AccountingRepository : RepositoryBase, IAccountingRepository
    {
        public AccountingRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<PNTESTATA>? GetHeadList(string CompanyID, int Year, string? FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount)
        {
            TotalCount = 0;
            try
            {
                using var connection = GetOpenConnection();


                var aliasList = new List<GenericIDDescriptionType>() {
                    new GenericIDDescriptionType(){ ID = "NotBalanced", Description="nb", Type = "#BF#sq.Amount <> sq.OtherAmount#sq.Amount = sq.OtherAmount#" },
                    new GenericIDDescriptionType(){ ID = "N1TmpPNBool", Description="N1TmpPN", Type = "#B#S#N#" },
                    new GenericIDDescriptionType(){ ID = "N1REGIText", Description="N1REGI" },
                    new GenericIDDescriptionType(){ ID = "N1CLFOText", Description="N1CLFO" },
                    new GenericIDDescriptionType(){ ID = "N1DAREText", Description="N1DARE", Type="D" },
                    new GenericIDDescriptionType(){ ID = "pncaus", Description="pncaus" },
                    new GenericIDDescriptionType(){ ID = "Amount", Description="Amount", Type = "#C#(SELECT SUM(N1IMEU) FROM PNRIGHE WHERE N1SEGN = 'D' AND N1SOCI = sq.N1SOCI AND N1ANNO = sq.N1ANNO AND N1REGI = sq.N1REGI)" },
                    new GenericIDDescriptionType(){ ID = "N1CLFO", Description="N1CLFO" },
                    new GenericIDDescriptionType(){ ID = "AccountingCausal.caudes", Description="caudes" },
                    new GenericIDDescriptionType(){ ID = "N1docn", Description="N1docn" },
                    new GenericIDDescriptionType(){ ID = "N1rifn", Description="N1rifn" },
                    new GenericIDDescriptionType(){ ID = "EntityFullDescription", Description = "EntityFullDescription", Type = "#C#CONCAT(TRIM(sq.abers1) , ' ', TRIM(sq.abers2))"} };

                #region Args
                FullTextSearch = FullTextSearch?.ToLower();
                var args = new DynamicParameters();
                args.Add("CompanyID", CompanyID);
                args.Add("Year", Year);
                args.Add("skip", RequestedPage * PageSize);
                args.Add("ps", PageSize);
                args.Add("ft", $"%{FullTextSearch}%");
                #endregion
                #region Where
                var whereFilter = new StringBuilder("sq.N1SOCI = @CompanyID AND sq.N1ANNO = @Year ");
                // grid filters
                TelerikGridService.ComputeFilter(whereFilter, FilterList, aliasList, args);
                // full-text search
                if (!string.IsNullOrWhiteSpace(FullTextSearch))
                {
                    whereFilter.Append($@"AND (LOWER(caudes) LIKE @ft OR LOWER(pncaus) LIKE @ft OR LOWER(CONVERT(nvarchar(20), N1REGI)) LIKE @ft OR
                                            LOWER(N1docn) LIKE @ft OR LOWER(N1rifn) LIKE @ft OR
                                            LOWER(abers1) LIKE @ft OR LOWER(abers2) LIKE @ft OR LOWER(CONVERT(nvarchar(10),N1CLFO)) LIKE @ft)");
                }
                #endregion
                #region Sort
                var sort = TelerikGridService.ComputeSort(SortList, aliasList);
                #endregion
                TotalCount = (int?)connection.ExecuteScalar($@"SELECT COUNT(*) FROM (SELECT t.*,
                                                        (SELECT SUM(N1IMEU) FROM PNRIGHE WHERE N1SEGN = 'D' AND N1SOCI = t.N1SOCI AND N1ANNO = t.N1ANNO AND N1REGI = t.N1REGI) as Amount, 
                                                        (SELECT SUM(N1IMEU) FROM PNRIGHE WHERE N1SEGN = 'A' AND N1SOCI = t.N1SOCI AND N1ANNO = t.N1ANNO AND N1REGI = t.N1REGI) as OtherAmount,
                                                        l.caudes, base.abers1, base.abers2, CONCAT(TRIM(base.abers1) , ' ', TRIM(base.abers2)) AS EntityFullDescription
                                                        FROM PNTESTATA AS t 
                                                        INNER JOIN CAUCONT AS l ON t.PNCAUS = l.CAUCOD 
                                                        LEFT JOIN ABE AS base ON base.abecod = t.N1CLFO) AS sq
                                                        WHERE {whereFilter.ToString()};", args) ?? 0;

                var list = connection.Query<PNTESTATA, CAUCONT, PNTESTATA>(
                    $@"SELECT *, ABS(Amount - OtherAmount) AS nb
                        FROM
                        (   
                            SELECT 
                                t.*,
                                CONCAT(TRIM(base.abers1), ' ', TRIM(base.abers2)) AS EntityFullDescription,

                                -- Aggregated sums instead of correlated subqueries
                                ISNULL(r.Amount, 0) AS Amount,
                                ISNULL(r.OtherAmount, 0) AS OtherAmount,

                                l.caudes,
                                l.caugen,
                                l.cauiva,
                                l.caucli,
                                l.caufor,
                                base.abers1,
                                base.abers2

                            FROM PNTESTATA AS t
                            INNER JOIN CAUCONT AS l 
                                ON t.PNCAUS = l.CAUCOD
                            LEFT JOIN ABE AS base 
                                ON base.abecod = t.N1CLFO

                            LEFT JOIN
                            (
                                SELECT 
                                    N1SOCI,
                                    N1ANNO,
                                    N1REGI,
                                    SUM(CASE WHEN N1SEGN = 'D' THEN N1IMEU ELSE 0 END) AS Amount,
                                    SUM(CASE WHEN N1SEGN = 'A' THEN N1IMEU ELSE 0 END) AS OtherAmount
                                FROM PNRIGHE
                                GROUP BY N1SOCI, N1ANNO, N1REGI
                            ) r
                                ON r.N1SOCI = t.N1SOCI
                               AND r.N1ANNO = t.N1ANNO
                               AND r.N1REGI = t.N1REGI

                        ) AS sq
                        WHERE {whereFilter.ToString()}
                        {(!string.IsNullOrWhiteSpace(sort) ? sort : "ORDER BY N1DARE DESC, N1REGI DESC ")}
                        OFFSET @skip ROWS 
                        FETCH NEXT @ps ROWS ONLY",
                    (pn, caus) => { pn.AccountingCausal = caus; return pn; },
                    args,
                    splitOn: "caudes");

                return new ObservableCollection<PNTESTATA>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNRIGHE>? GetHeadTemporaryPeriodList(string CompanyID, int Year, string? MonthFlag, DateTime? From, DateTime? To, string GroupID, string AccountID, string SubaccountID)
        {
            try
            {
                using var connection = GetOpenConnection();


                // generate month flag where
                string? monthFlagWhere = null;
                if (!string.IsNullOrWhiteSpace(MonthFlag))
                {
                    monthFlagWhere = $"AND DATEFROMPARTS(YEAR(l.N1DARE),MONTH(l.N1DARE),1) {MonthFlag} " +
                        $"DATEFROMPARTS(YEAR(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'), MONTH(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'),1)";
                }

                string queryCommand = string.Empty;
                if (From.HasValue && To.HasValue)
                {
                    queryCommand = $@"SELECT r.*, l.*, c.caudes, base.abecod, base.abers1, base.abers2 FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            INNER JOIN CAUCONT AS c ON l.PNCAUS = c.CAUCOD
                            LEFT OUTER JOIN ABE AS base ON base.abecod = l.N1CLFO
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea {monthFlagWhere} AND l.N1DARE >= @from AND l.N1DARE <= @to AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND r.N1tmpPNR = 'S'
                            ORDER BY l.N1DARE ASC";
                }
                else
                {
                    if (From.HasValue && !To.HasValue)
                    {
                        queryCommand = $@"SELECT r.*, l.*, c.caudes, base.abecod, base.abers1, base.abers2 FROM PNRIGHE AS r
                                                INNER JOIN PNTESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                                                INNER JOIN CAUCONT AS c ON l.PNCAUS = c.CAUCOD
                                                LEFT OUTER JOIN ABE AS base ON base.abecod = l.N1CLFO
                                                WHERE r.n1soci = @cid AND r.N1ANNO = @yea {monthFlagWhere} AND l.N1DARE <= @from AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND r.N1tmpPNR = 'S'
                                                ORDER BY l.N1DARE ASC";
                    }
                    else
                    {
                        if (!From.HasValue && To.HasValue)
                        {
                            queryCommand = $@"SELECT r.*, l.*, c.caudes, base.abecod, base.abers1, base.abers2 FROM PNRIGHE AS r
                                                    INNER JOIN PNTESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                                                    INNER JOIN CAUCONT AS c ON l.PNCAUS = c.CAUCOD
                                                    LEFT OUTER JOIN ABE AS base ON base.abecod = l.N1CLFO
                                                    WHERE r.n1soci = @cid AND r.N1ANNO = @yea {monthFlagWhere} AND l.N1DARE > @to AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND r.N1tmpPNR = 'S'
                                                    ORDER BY l.N1DARE ASC";
                        }
                    }
                }
                var list = connection.Query<PNRIGHE, PNTESTATA, CAUCONT, ABE, PNRIGHE>(
                    queryCommand,
                    (row, head, cau, bas) => { head.AccountingCausal = cau; head.BasicRegistry = bas; row.Testata = head; return row; },
                    new { cid = CompanyID, yea = Year, from = From, to = To, group = GroupID, account = AccountID, subaccount = SubaccountID },
                    splitOn: "N1SOCI,caudes,abecod");
                return new ObservableCollection<PNRIGHE>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool RunEqualization(string CompanyID, string EntityType, DateTime UntilDate)
        {
            try
            {
                using var connection = GetOpenConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@tipo", EntityType);
                parameters.Add("@societa", CompanyID);
                parameters.Add("@data_registrazione", UntilDate);

                connection.Execute(
                    "PareggioPartite",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        #region Receipts registration
        public string? AccountingReceipts(string CompanyID, DateTime RegistrationDate, ABE SelectedCustomer, CAUCONT SelectedCausal, string DocumentID, ObservableCollection<ReceiptInfo> Rows, decimal TotalTax, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    // get registration number
                    var newAccountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, RegistrationDate.Year, Constants.PN, true);
                    // CLIAMMI
                    var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, SelectedCustomer.abecod);
                    // IVA book
                    var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(SelectedCausal.cauliv ?? string.Empty);
                    // CAUCONT_GROUPS
                    var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetListNoCR(CompanyID, SelectedCausal.caucod);
                    var ricavo = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetFirstSign(CompanyID, SelectedCausal.caucod, "D");
                    // PAGCLI/TAB_ACC_TIPINC
                    var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(cliammi?.pclcod ?? string.Empty);
                    var incassi = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Get(pagcli?.pcltip ?? string.Empty);

                    #region PNTESTATA
                    PNTESTATA head = new PNTESTATA()
                    {
                        N1SOCI = CompanyID,
                        N1ANNO = RegistrationDate.Year,
                        N1REGI = newAccountingID,
                        pncaus = SelectedCausal.caucod,
                        N1DARE = RegistrationDate.Date,
                        N1docn = DocumentID,
                        N1docd = RegistrationDate.Date,
                        N1rifn = DocumentID,
                        N1rifd = RegistrationDate.Date,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1CLFO = SelectedCustomer.abecod,
                        N1FLCF = "C",
                        N1FL01 = string.Empty,
                        N1TmpPN = "N",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().INSERT_QUERY, head, transaction);
                    #endregion

                    int rowsCounter = 1;

                    #region Customer row
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
                        N1SEGN = "D",
                        pngrup = ricavo?.grpgrp,
                        pncont = ricavo?.grpcto,
                        pnsott = ricavo?.grpsct,
                        N1IMEU = Rows.Sum(sum => sum.Amount),
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = cliammi?.pclcod,
                        n1scad = head.N1DARE,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, customerRow, transaction);
                    #endregion

                    #region Details rows
                    foreach (var row in Rows)
                    {
                        var newRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = "A",
                            pngrup = row.SelectedGroup?.P1GRUP,
                            pncont = row.SelectedAccount?.P2CONT,
                            pnsott = row.SelectedSubaccount?.P3SOTC,
                            N1IMEU = row.Taxable,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            n1paga = cliammi?.pclcod,
                            n1scad = head.N1DARE,
                            N1DRri = head.N1docd,
                            N1STBO = string.Empty,
                            N1STMA = string.Empty,
                            N1STNO = string.Empty,
                            N1CCCC = row.CostCenter?.cecodc
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, newRow, transaction);
                    }
                    #endregion

                    #region IVA row
                    if (TotalTax > 0)
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
                            N1SEGN = grpcau?.grpseg,
                            pngrup = grpcau?.grpgrp,
                            pncont = grpcau?.grpcto,
                            pnsott = grpcau?.grpsct,
                            N1IMEU = Rows.Sum(sum => sum.Tax),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1STBO = string.Empty,
                            N1STMA = string.Empty,
                            N1STNO = string.Empty,
                            N1DRri = head.N1docd,
                            N1tmpPNR = "N"
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, ivaRow, transaction);
                    }
                    #endregion

                    #region PNIVA
                    int ivaRowsCounter = 1;
                    List<string> rates = new List<string>();
                    foreach (var row in Rows.OrderBy(o => o.Rate?.asscod).ThenBy(o => o.Rate?.assali))
                    {
                        if (!rates.Contains(row.Rate?.asscod + row.Rate?.assali.Trim()))
                        {
                            rates.Add(row.Rate?.asscod + row.Rate?.assali.Trim());
                        }
                    }
                    foreach (var rate in rates)
                    {
                        var imponibile = Rows
                            .Where(w => w.Rate?.asscod == rate.Substring(0, 1) && w.Rate?.assali == rate.Substring(1, rate.Length - 1))
                            .Sum(sum => sum.Taxable);
                        var imposta = Rows
                            .Where(w => w.Rate?.asscod == rate.Substring(0, 1) && w.Rate?.assali == rate.Substring(1, rate.Length - 1))
                            .Sum(sum => sum.Tax);
                        PNIVA pnivaRow = new PNIVA()
                        {
                            N4SOCI = CompanyID,
                            N4ANNO = head.N1ANNO,
                            N4REGI = newAccountingID,
                            N4RIGA = ivaRowsCounter++,
                            N4DOCU = head.N1docn,
                            N4RIFE = head.N1rifn,
                            N4DARE = head.N1DARE,
                            N4DADO = head.N1docd,
                            N4DARI = head.N1rifd,
                            N4CAUS = head.pncaus,
                            N4SEGN = SelectedCausal.causeg,
                            N4LIBR = ivaBook?.livcod,
                            N4SOTT = head.N1CLFO,
                            N4TCLF = "C",
                            N4ASSF = rate.Substring(0, 1),
                            n4assa = rate.Substring(1, rate.Length - 1),
                            N4TIDO = "E",
                            n4donu = int.Parse(head.N1docn),
                            N4DAST = null,
                            n4tmppn = "N",
                            N4DTSCAD = head.N1DARE,
                            N4DTPGEF = null,
                            N4DTSCPG = head.N1DARE,
                            N4IMEU = imponibile,
                            N4IVEU = imposta,
                            N4IIEU = 0,
                            N4FLIVCA = "N",
                            N4FLSPESO = string.Empty,
                            N4IMPPAG = 0
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, pnivaRow, transaction);
                    }
                    #endregion

                    transaction.Commit();
                    return $"{head.N1ANNO}/{head.N1REGI}";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public string? RegistrationAccounting(string CompanyID, int AccountingYear, DateTime RegistrationDate, ABE Entity, string EntityType, CAUCONT SelectedCausal, BANAZIEN SelectedBank, string? DocumentID, DateTime? DocumentDate, ObservableCollection<MastrinoECReportItem> Rows, string UserID)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Accounting situation

        public AccountingSituationViewModel? GetAccountingSituation(string CompanyID, int Year, string? CostCenterID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new AccountingSituationViewModel()
                {
                    AttivitaGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>(),
                    PassivitaGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>(),
                    RicaviGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>(),
                    CostiGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>()
                };
                Parallel.ForEach(new string[] { "A", "P", "R", "C" }, groupType =>
                {
                    var groupsList = connection.Query<PDCGRUPPI>(@"SELECT P1GRUP, TRIM(P1DES1) AS P1DES1 FROM PDCGRUPPI WHERE P1TICO = @groupType", new { groupType = groupType });
                    foreach (var grp in groupsList)
                    {
                        var multi = connection.QueryMultiple(
                            $@"SELECT SUM(N1IMEU) FROM PNRIGHE
                                  WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND N1SEGN = 'D' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};
                                  SELECT SUM(N1IMEU) FROM PNRIGHE
                                  WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND N1SEGN = 'A' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};",
                                new { cid = CompanyID, yea = Year, pngrup = grp.P1GRUP, cc = CostCenterID });
                        var newGroup = new AccountingSituationViewModel.ASItem()
                        {
                            GroupID = grp.P1GRUP,
                            Description = grp.FullDescriptionSearchable,
                            IsDare = groupType == "A" || groupType == "C",
                            Dare = multi.Read<decimal?>().Single() ?? 0m,
                            Avere = multi.Read<decimal?>().Single() ?? 0m,
                            Accounts = new ObservableCollection<AccountingSituationViewModel.ASItem>()
                        };

                        if (newGroup.Saldo != 0)
                        {
                            // accounts
                            foreach (var acc in VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList(grp.P1GRUP) ?? new ObservableCollection<PDCCONTI>())
                            {
                                var multiAccount = connection.QueryMultiple(
                                    $@"SELECT SUM(N1IMEU) FROM PNRIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND N1SEGN = 'D' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};
                                      SELECT SUM(N1IMEU) FROM PNRIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND N1SEGN = 'A' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};",
                                    new { cid = CompanyID, yea = Year, pngrup = grp.P1GRUP, pncont = acc.P2CONT, cc = CostCenterID });
                                var newAccount = new AccountingSituationViewModel.ASItem()
                                {
                                    GroupID = acc.P1GRUP,
                                    AccountID = acc.P2CONT,
                                    Description = acc.FullDescriptionSearchable,
                                    IsDare = groupType == "A" || groupType == "C",
                                    Dare = multiAccount.Read<decimal?>().Single() ?? 0m,
                                    Avere = multiAccount.Read<decimal?>().Single() ?? 0m
                                };
                                if (newAccount.Saldo != 0)
                                    newGroup.Accounts.Add(newAccount);
                            }
                            switch (groupType)
                            {
                                case "A":
                                    result.AttivitaGruppi.Add(newGroup);
                                    break;
                                case "P":
                                    result.PassivitaGruppi.Add(newGroup);
                                    break;
                                case "R":
                                    result.RicaviGruppi.Add(newGroup);
                                    break;
                                case "C":
                                    result.CostiGruppi.Add(newGroup);
                                    break;
                            }
                        }
                    }
                });
                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ASItem>? GetAccountingSituationDetails(string CompanyID, int Year, string GroupID, bool IsDare, string AccountID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new ObservableCollection<ASItem>();
                foreach (var sub in VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(GroupID, AccountID, CompanyID) ?? new ObservableCollection<PDCSOTTO>())
                {
                    var multi = connection.QueryMultiple(
                                @"SELECT SUM(N1IMEU) FROM PNRIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND pnsott = @pnsott AND N1SEGN = 'D';
                                      SELECT SUM(N1IMEU) FROM PNRIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND pnsott = @pnsott AND N1SEGN = 'A';",
                                new { cid = CompanyID, yea = Year, pngrup = GroupID, pncont = AccountID, pnsott = sub.P3SOTC });
                    var newSub = new AccountingSituationViewModel.ASItem()
                    {
                        GroupID = GroupID,
                        Description = sub.FullDescriptionSearchable,
                        IsDare = IsDare,
                        Dare = multi.Read<decimal?>().Single() ?? 0m,
                        Avere = multi.Read<decimal?>().Single() ?? 0m
                    };
                    if (newSub.Saldo != 0)
                    {
                        result.Add(newSub);
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

        public string? ExportCEEBalance(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PDCSOTTO, PDCGRUPPI, PDCSOTTO>(
                    @"SELECT s.*, g.P1TICO FROM PDCSOTTO AS s
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = s.P1GRUP
                            WHERE (s.p3soci = @CompanyID OR s.p3soci IS NULL)
                            ORDER BY s.P1GRUP, s.P2CONT, s.P3SOTC",
                    (sot, grp) => { sot.Group = grp; return sot; },
                    new { CompanyID = CompanyID }, splitOn: "P1TICO");
                ConcurrentQueue<GenericIntDecimal> codes = new ConcurrentQueue<GenericIntDecimal>();
                int unknownCount = -1;
                Parallel.ForEach(list.ToList(), sot =>
                {
                    var multi = connection.QueryMultiple(
                    @"SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1soci = r.N1soci AND t.n1anno = r.n1anno AND t.n1regi = r.n1regi
                            WHERE r.N1ANNO = @yea AND r.N1SOCI = @cid AND r.pngrup = @grp AND r.pncont = @cnt AND r.pnsott = @sot AND r.N1SEGN = 'D' AND t.canceled IS NULL;
                        SELECT SUM(r.N1IMEU) FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1soci = r.N1soci AND t.n1anno = r.n1anno AND t.n1regi = r.n1regi
                            WHERE r.N1ANNO = @yea AND r.N1SOCI = @cid AND r.pngrup = @grp AND r.pncont = @cnt AND r.pnsott = @sot AND r.N1SEGN = 'A' AND t.canceled IS NULL;",
                    new { cid = CompanyID, grp = sot.P1GRUP, cnt = sot.P2CONT, sot = sot.P3SOTC, yea = Year });
                    var dare = multi.Read<decimal?>().Single() ?? 0m;
                    var avere = multi.Read<decimal?>().Single() ?? 0m;
                    if (dare != avere)
                    {
                        if (sot.Group?.P1TICO == "A" || sot.Group?.P1TICO == "C")
                        {
                            if (dare > avere)
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3este ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += dare - avere;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3este ?? unknownCount--, Tag = (sot.p3este.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = dare - avere });
                            }
                            else
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3est2 ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += dare - avere;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3est2 ?? unknownCount--, Tag = (sot.p3est2.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = dare - avere });
                            }
                        }
                        else
                        {
                            if (dare > avere)
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3est2 ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += avere - dare;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3est2 ?? unknownCount--, Tag = (sot.p3est2.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = avere - dare });
                            }
                            else
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3este ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += avere - dare;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3este ?? unknownCount--, Tag = (sot.p3este.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = avere - dare });
                            }
                        }
                    }
                });
                StringBuilder result = new StringBuilder();
                foreach (var item in codes.OrderBy(o => o.IntItem))
                {
                    result.Append($"{(item.IntItem > 0 ? item.IntItem.ToString().PadLeft(8, '0') : $"#{item.Tag?.ToString()}")};{(item.DecimalItem > 0 ? "+" : "-")}{Math.Abs(item.DecimalItem).ToString("N2", new System.Globalization.CultureInfo("it-IT") { NumberFormat = new System.Globalization.NumberFormatInfo() { NumberGroupSeparator = string.Empty, NumberDecimalSeparator = "," } }).PadLeft(13, '0')}{Environment.NewLine}");
                }
                return result.ToString();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region Closing
        public bool PeriodClosing(string CompanyID, int Year, DateTime LimitDate)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // flag all rows
                        connection.Execute("UPDATE PNRIGHE SET N1CHIU='C' WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1STBO='*' AND N1STMA='*';",
                            new { cid = CompanyID, yea = Year }, transaction);

                        // move period into esercizio (sum) on pdcanni
                        connection.Execute(@"UPDATE pdc
                                            SET pdc.P4DAES = ISNULL(pdc.P4DAES, 0) + ISNULL(pdc.P4DAPE, 0), pdc.P4AVES = ISNULL(pdc.P4AVES, 0) + ISNULL(pdc.P4AVPE, 0), pdc.P4DAPE = 0, pdc.P4AVPE = 0
                                            FROM PDCANNI AS pdc INNER JOIN
                                            (SELECT N1SOCI,N1ANNO,pngrup,pncont,pnsott FROM PNRIGHE
                                            WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1STBO='*' AND N1STMA='*'
                                            GROUP BY N1SOCI,N1ANNO,pngrup,pncont,pnsott) AS grp
                                            ON grp.pngrup=P1GRUP AND grp.pncont=P2CONT AND grp.pnsott=P3SOTC AND grp.N1ANNO=P4ANNO AND grp.N1SOCI=P1SOCI",
                            new { cid = CompanyID, yea = Year }, transaction);

                        // update esercizio last closing
                        connection.Execute("UPDATE ESERCIZIO SET eseuch=@eseuch WHERE esesoc=@cid AND eseann=@yea",
                            new { cid = CompanyID, yea = Year, eseuch = LimitDate }, transaction);

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

        public bool YearClosing(string CompanyID, int Year, DateTime LimitDate, DateTime NewDate, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, Year)!;

                        var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                        var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                        var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                        var tabClosingRepositoty = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_CLOSINGRepository>();
                        var pdcAnniRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>();

                        LimitDate = LimitDate.Date;
                        NewDate = NewDate.Date;
                        // ECONOMICI
                        #region Registrazione chiusura conto economico (costi e ricavi)
                        #region PNTESTATA
                        var accountingID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                        var causals = tabClosingRepositoty.GetList();
                        var lossCausal = causals?.Where(w => w.cchppr == "S").First();
                        PNTESTATA head = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year,
                            N1REGI = accountingID,
                            pncaus = lossCausal?.cchchi,
                            N1DARE = LimitDate,
                            N1docn = accountingID.ToString(),
                            N1docd = LimitDate,
                            N1rifn = accountingID.ToString(),
                            N1rifd = LimitDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                        #endregion
                        int rowsCounter = 1;
                        #region Righe
                        decimal dareTotal = 0;
                        decimal avereTotal = 0;
                        foreach (var yea in pdcAnniRepository.GetListByYearTypes(CompanyID, Year, ["C", "R"]) ?? new ObservableCollection<PDCANNI>())
                        {
                            var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                            if (diff != 0)
                            {
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
                                    N1SEGN = diff > 0 ? "A" : "D",
                                    pngrup = yea.P1GRUP,
                                    pncont = yea.P2CONT,
                                    pnsott = yea.P3SOTC,
                                    N1IMEU = diff > 0 ? diff : diff * -1,
                                    N1CHIU = "A",
                                    N1TIDO = "E",
                                    N1DIVI = "EUR",
                                    N1tmpPNR = "N",
                                    N1DRri = head.N1docd
                                };
                                connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                if (newRow.N1SEGN == "D")
                                    dareTotal += newRow.N1IMEU ?? 0;
                                else
                                    avereTotal += newRow.N1IMEU ?? 0;
                            }
                        }
                        // totali conto profitti e perdite
                        var newDareRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = "A",
                            pngrup = lossCausal?.cchgrc,
                            pncont = lossCausal?.cchctc,
                            pnsott = lossCausal?.cchstc,
                            N1IMEU = dareTotal,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newDareRow, transaction);
                        var newAvereRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = "D",
                            pngrup = lossCausal?.cchgrc,
                            pncont = lossCausal?.cchctc,
                            pnsott = lossCausal?.cchstc,
                            N1IMEU = avereTotal,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereRow, transaction);

                        #endregion
                        #endregion

                        #region Registrazione utile/perdita
                        var diffLG = dareTotal - avereTotal;
                        #region PNTESTATA
                        var accountingLGID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                        var headLG = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year,
                            N1REGI = accountingLGID,
                            pncaus = diffLG >= 0 ? lossCausal?.cchues : lossCausal?.cchpes,
                            N1DARE = LimitDate,
                            N1docn = accountingLGID.ToString(),
                            N1docd = LimitDate,
                            N1rifn = accountingLGID.ToString(),
                            N1rifd = LimitDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headLG, transaction);
                        #endregion
                        int rowsLGCounter = 1;
                        #region Righe
                        PNRIGHE newDiffRow = new PNRIGHE()
                        {
                            N1SOCI = headLG.N1SOCI,
                            N1ANNO = headLG.N1ANNO,
                            N1REGI = headLG.N1REGI,
                            N1RIGA = rowsLGCounter++,
                            N1DOCU = headLG.N1docn,
                            N1DADO = headLG.N1docd,
                            N1RIFE = headLG.N1rifn,
                            N1DARI = headLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "A" : "D",
                            pngrup = diffLG >= 0 ? lossCausal?.cchgru : lossCausal?.cchgrp,
                            pncont = diffLG >= 0 ? lossCausal?.cchctu : lossCausal?.cchctp,
                            pnsott = diffLG >= 0 ? lossCausal?.cchstu : lossCausal?.cchstp,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newDiffRow, transaction);
                        PNRIGHE newCPRow = new PNRIGHE()
                        {
                            N1SOCI = headLG.N1SOCI,
                            N1ANNO = headLG.N1ANNO,
                            N1REGI = headLG.N1REGI,
                            N1RIGA = rowsLGCounter++,
                            N1DOCU = headLG.N1docn,
                            N1DADO = headLG.N1docd,
                            N1RIFE = headLG.N1rifn,
                            N1DARI = headLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "D" : "A",
                            pngrup = lossCausal?.cchgrc,
                            pncont = lossCausal?.cchctc,
                            pnsott = lossCausal?.cchstc,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newCPRow, transaction);
                        #endregion
                        #endregion

                        #region Registrazione chiusura risultato esercizio
                        var esercizioCausal = (causals ?? new ObservableCollection<TAB_ACC_CLOSING>()).Where(w => w.cchppr == "N").First();

                        #region PNTESTATA
                        var accountingRisID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                        var headRis = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year,
                            N1REGI = accountingRisID,
                            pncaus = esercizioCausal.cchchi,
                            N1DARE = LimitDate,
                            N1docn = accountingRisID.ToString(),
                            N1docd = LimitDate,
                            N1rifn = accountingRisID.ToString(),
                            N1rifd = LimitDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headRis, transaction);
                        #endregion
                        int rowsRisCounter = 1;
                        #region Righe
                        PNRIGHE newRis1Row = new PNRIGHE()
                        {
                            N1SOCI = headRis.N1SOCI,
                            N1ANNO = headRis.N1ANNO,
                            N1REGI = headRis.N1REGI,
                            N1RIGA = rowsRisCounter++,
                            N1DOCU = headRis.N1docn,
                            N1DADO = headRis.N1docd,
                            N1RIFE = headRis.N1rifn,
                            N1DARI = headRis.N1rifd,
                            N1SEGN = newDiffRow.N1SEGN == "D" ? "A" : "D",
                            pngrup = newDiffRow.pngrup,
                            pncont = newDiffRow.pncont,
                            pnsott = newDiffRow.pnsott,
                            N1IMEU = newDiffRow.N1IMEU,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headRis.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRis1Row, transaction);
                        PNRIGHE newRis2Row = new PNRIGHE()
                        {
                            N1SOCI = headRis.N1SOCI,
                            N1ANNO = headRis.N1ANNO,
                            N1REGI = headRis.N1REGI,
                            N1RIGA = rowsRisCounter++,
                            N1DOCU = headRis.N1docn,
                            N1DADO = headRis.N1docd,
                            N1RIFE = headRis.N1rifn,
                            N1DARI = headRis.N1rifd,
                            N1SEGN = newRis1Row.N1SEGN == "D" ? "A" : "D",
                            pngrup = esercizioCausal.cchgrc,
                            pncont = esercizioCausal.cchctc,
                            pnsott = esercizioCausal.cchstc,
                            N1IMEU = newRis1Row.N1IMEU,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headRis.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRis2Row, transaction);
                        #endregion
                        #endregion

                        var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)!;

                        var apYears = pdcAnniRepository.GetListByYearTypes(CompanyID, Year, ["A", "P"]);

                        if (azienda.aziconchiuclifor ?? false)
                        {
                            // PATRIMONIALI
                            #region Registrazione chiusura conti patrimoniali
                            #region PNTESTATA
                            var accountingPatID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA headPat = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingPatID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headPat, transaction);
                            #endregion
                            int rowsPatCounter = 1;
                            #region Righe
                            decimal darePatTotal = 0;
                            decimal averePatTotal = 0;
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                if (yea.Account?.p2flcf != "C" && yea.Account?.p2flcf != "F")
                                {
                                    var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                    if (diff != 0)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = headPat.N1SOCI,
                                            N1ANNO = headPat.N1ANNO,
                                            N1REGI = headPat.N1REGI,
                                            N1RIGA = rowsPatCounter++,
                                            N1DOCU = headPat.N1docn,
                                            N1DADO = headPat.N1docd,
                                            N1RIFE = headPat.N1rifn,
                                            N1DARI = headPat.N1rifd,
                                            N1SEGN = diff > 0 ? "A" : "D",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = diff > 0 ? diff : diff * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            N1DRri = headPat.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            darePatTotal += newRow.N1IMEU ?? 0;
                                        else
                                            averePatTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                            }

                            var newDarePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = darePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDarePatRow, transaction);
                            var newAverePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = averePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAverePatRow, transaction);

                            #endregion
                            #endregion

                            #region Registrazione chiusura Clienti/Fornitori
                            #region PNTESTATA
                            var accountingCusID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA accountingCus = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingCusID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, accountingCus, transaction);

                            var accountingSupID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA accountingSup = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingSupID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, accountingSup, transaction);
                            #endregion

                            int rowsCusCounter = 1;
                            int rowsSupCounter = 1;

                            #region Righe
                            decimal dareCusTotal = 0;
                            decimal avereCusTotal = 0;
                            decimal dareSupTotal = 0;
                            decimal avereSupTotal = 0;

                            foreach (var yea in (apYears ?? new ObservableCollection<PDCANNI>()).Where(o => o.Account?.p2flcf == "C" || o.Account?.p2flcf == "F"))
                            {
                                if (yea.Account?.p2flcf == "C")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = accountingCus.N1SOCI,
                                            N1ANNO = accountingCus.N1ANNO,
                                            N1REGI = accountingCus.N1REGI,
                                            N1RIGA = rowsCusCounter++,
                                            N1DOCU = accountingCus.N1docn,
                                            N1DADO = accountingCus.N1docd,
                                            N1RIFE = accountingCus.N1rifn,
                                            N1DARI = accountingCus.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "A" : "D",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = accountingCus.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareCusTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereCusTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                                if (yea.Account?.p2flcf == "F")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = accountingSup.N1SOCI,
                                            N1ANNO = accountingSup.N1ANNO,
                                            N1REGI = accountingSup.N1REGI,
                                            N1RIGA = rowsSupCounter++,
                                            N1DOCU = accountingSup.N1docn,
                                            N1DADO = accountingSup.N1docd,
                                            N1RIFE = accountingSup.N1rifn,
                                            N1DARI = accountingSup.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "A" : "D",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = accountingSup.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareSupTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereSupTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                            }

                            var newDareCusRow = new PNRIGHE()
                            {
                                N1SOCI = accountingCus.N1SOCI,
                                N1ANNO = accountingCus.N1ANNO,
                                N1REGI = accountingCus.N1REGI,
                                N1RIGA = rowsCusCounter++,
                                N1DOCU = accountingCus.N1docn,
                                N1DADO = accountingCus.N1docd,
                                N1RIFE = accountingCus.N1rifn,
                                N1DARI = accountingCus.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = avereCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingCus.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareCusRow, transaction);
                            var newAvereCusRow = new PNRIGHE()
                            {
                                N1SOCI = accountingCus.N1SOCI,
                                N1ANNO = accountingCus.N1ANNO,
                                N1REGI = accountingCus.N1REGI,
                                N1RIGA = rowsCusCounter++,
                                N1DOCU = accountingCus.N1docn,
                                N1DADO = accountingCus.N1docd,
                                N1RIFE = accountingCus.N1rifn,
                                N1DARI = accountingCus.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = dareCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingCus.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereCusRow, transaction);

                            var newDareSupRow = new PNRIGHE()
                            {
                                N1SOCI = accountingSup.N1SOCI,
                                N1ANNO = accountingSup.N1ANNO,
                                N1REGI = accountingSup.N1REGI,
                                N1RIGA = rowsSupCounter++,
                                N1DOCU = accountingSup.N1docn,
                                N1DADO = accountingSup.N1docd,
                                N1RIFE = accountingSup.N1rifn,
                                N1DARI = accountingSup.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = avereSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingSup.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareSupRow, transaction);
                            var newAvereSupRow = new PNRIGHE()
                            {
                                N1SOCI = accountingSup.N1SOCI,
                                N1ANNO = accountingSup.N1ANNO,
                                N1REGI = accountingSup.N1REGI,
                                N1RIGA = rowsSupCounter++,
                                N1DOCU = accountingSup.N1docn,
                                N1DADO = accountingSup.N1docd,
                                N1RIFE = accountingSup.N1rifn,
                                N1DARI = accountingSup.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = dareSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingSup.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereSupRow, transaction);
                            #endregion
                            #endregion

                            #region Registrazione riapertura conti patrimoniali
                            #region PNTESTATA
                            var accountingOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA headOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headOpen, transaction);
                            #endregion
                            int rowsOpenCounter = 1;
                            #region Righe
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                if (yea.Account?.p2flcf != "C" && yea.Account?.p2flcf != "F")
                                {
                                    var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                    if (diff != 0)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = headOpen.N1SOCI,
                                            N1ANNO = headOpen.N1ANNO,
                                            N1REGI = headOpen.N1REGI,
                                            N1RIGA = rowsOpenCounter++,
                                            N1DOCU = headOpen.N1docn,
                                            N1DADO = headOpen.N1docd,
                                            N1RIFE = headOpen.N1rifn,
                                            N1DARI = headOpen.N1rifd,
                                            N1SEGN = diff > 0 ? "D" : "A",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = diff > 0 ? diff : diff * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            N1DRri = headOpen.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                    }
                                }
                            }
                            var newDareOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = darePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareOpenRow, transaction);
                            var newAvereOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = averePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereOpenRow, transaction);

                            #endregion
                            #endregion

                            #region Registrazione riapertura Clienti/Fornitori
                            #region PNTESTATA
                            var accountingCusOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA cusOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingCusOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, cusOpen, transaction);

                            var accountingSupOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA supOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingSupOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, supOpen, transaction);
                            #endregion
                            int rowsCusOpenCounter = 1;
                            int rowsSupOpenCounter = 1;
                            #region Righe
                            decimal dareOpenCusTotal = 0;
                            decimal avereOpenCusTotal = 0;
                            decimal dareOpenSupTotal = 0;
                            decimal avereOpenSupTotal = 0;

                            foreach (var yea in (apYears ?? new ObservableCollection<PDCANNI>()).Where(o => o.Account?.p2flcf == "C" || o.Account?.p2flcf == "F"))
                            {
                                if (yea.Account?.p2flcf == "C")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = cusOpen.N1SOCI,
                                            N1ANNO = cusOpen.N1ANNO,
                                            N1REGI = cusOpen.N1REGI,
                                            N1RIGA = rowsCusOpenCounter++,
                                            N1DOCU = cusOpen.N1docn,
                                            N1DADO = cusOpen.N1docd,
                                            N1RIFE = cusOpen.N1rifn,
                                            N1DARI = cusOpen.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "D" : "A",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = cusOpen.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareOpenCusTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereOpenCusTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                                if (yea.Account?.p2flcf == "F")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = supOpen.N1SOCI,
                                            N1ANNO = supOpen.N1ANNO,
                                            N1REGI = supOpen.N1REGI,
                                            N1RIGA = rowsSupOpenCounter++,
                                            N1DOCU = supOpen.N1docn,
                                            N1DADO = supOpen.N1docd,
                                            N1RIFE = supOpen.N1rifn,
                                            N1DARI = supOpen.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "D" : "A",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = supOpen.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareOpenSupTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereOpenSupTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                            }
                            var newDareCusOpenRow = new PNRIGHE()
                            {
                                N1SOCI = cusOpen.N1SOCI,
                                N1ANNO = cusOpen.N1ANNO,
                                N1REGI = cusOpen.N1REGI,
                                N1RIGA = rowsCusOpenCounter++,
                                N1DOCU = cusOpen.N1docn,
                                N1DADO = cusOpen.N1docd,
                                N1RIFE = cusOpen.N1rifn,
                                N1DARI = cusOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = dareOpenCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = cusOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareCusOpenRow, transaction);
                            var newAvereCusOpenRow = new PNRIGHE()
                            {
                                N1SOCI = cusOpen.N1SOCI,
                                N1ANNO = cusOpen.N1ANNO,
                                N1REGI = cusOpen.N1REGI,
                                N1RIGA = rowsCusOpenCounter++,
                                N1DOCU = cusOpen.N1docn,
                                N1DADO = cusOpen.N1docd,
                                N1RIFE = cusOpen.N1rifn,
                                N1DARI = cusOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = avereOpenCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = cusOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereCusOpenRow, transaction);

                            var newDareSupOpenRow = new PNRIGHE()
                            {
                                N1SOCI = supOpen.N1SOCI,
                                N1ANNO = supOpen.N1ANNO,
                                N1REGI = supOpen.N1REGI,
                                N1RIGA = rowsSupOpenCounter++,
                                N1DOCU = supOpen.N1docn,
                                N1DADO = supOpen.N1docd,
                                N1RIFE = supOpen.N1rifn,
                                N1DARI = supOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = dareOpenSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = supOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareSupOpenRow, transaction);
                            var newAvereSupOpenRow = new PNRIGHE()
                            {
                                N1SOCI = supOpen.N1SOCI,
                                N1ANNO = supOpen.N1ANNO,
                                N1REGI = supOpen.N1REGI,
                                N1RIGA = rowsSupOpenCounter++,
                                N1DOCU = supOpen.N1docn,
                                N1DADO = supOpen.N1docd,
                                N1RIFE = supOpen.N1rifn,
                                N1DARI = supOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = avereOpenSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = supOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereSupOpenRow, transaction);
                            #endregion
                            #endregion
                        }
                        else
                        {
                            // PATRIMONIALI
                            #region Registrazione chiusura conti patrimoniali
                            #region PNTESTATA
                            var accountingPatID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA headPat = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingPatID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headPat, transaction);
                            #endregion
                            int rowsPatCounter = 1;
                            #region Righe
                            decimal darePatTotal = 0;
                            decimal averePatTotal = 0;
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                if (diff != 0)
                                {
                                    PNRIGHE newRow = new PNRIGHE()
                                    {
                                        N1SOCI = headPat.N1SOCI,
                                        N1ANNO = headPat.N1ANNO,
                                        N1REGI = headPat.N1REGI,
                                        N1RIGA = rowsPatCounter++,
                                        N1DOCU = headPat.N1docn,
                                        N1DADO = headPat.N1docd,
                                        N1RIFE = headPat.N1rifn,
                                        N1DARI = headPat.N1rifd,
                                        N1SEGN = diff > 0 ? "A" : "D",
                                        pngrup = yea.P1GRUP,
                                        pncont = yea.P2CONT,
                                        pnsott = yea.P3SOTC,
                                        N1IMEU = diff > 0 ? diff : diff * -1,
                                        N1CHIU = "A",
                                        N1TIDO = "E",
                                        N1DIVI = "EUR",
                                        N1tmpPNR = "N",
                                        N1DRri = headPat.N1docd
                                    };
                                    connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                    if (newRow.N1SEGN == "D")
                                        darePatTotal += newRow.N1IMEU ?? 0;
                                    else
                                        averePatTotal += newRow.N1IMEU ?? 0;
                                }
                            }
                            var newDarePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = dareTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDarePatRow, transaction);
                            var newAverePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = avereTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAverePatRow, transaction);

                            #endregion
                            #endregion

                            // RIAPERTURA
                            #region Registrazione riapertura
                            #region PNTESTATA
                            var accountingOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA headOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headOpen, transaction);
                            #endregion
                            int rowsOpenCounter = 1;
                            #region Righe
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                if (diff != 0)
                                {
                                    PNRIGHE newRow = new PNRIGHE()
                                    {
                                        N1SOCI = headOpen.N1SOCI,
                                        N1ANNO = headOpen.N1ANNO,
                                        N1REGI = headOpen.N1REGI,
                                        N1RIGA = rowsOpenCounter++,
                                        N1DOCU = headOpen.N1docn,
                                        N1DADO = headOpen.N1docd,
                                        N1RIFE = headOpen.N1rifn,
                                        N1DARI = headOpen.N1rifd,
                                        N1SEGN = diff > 0 ? "D" : "A",
                                        pngrup = yea.P1GRUP,
                                        pncont = yea.P2CONT,
                                        pnsott = yea.P3SOTC,
                                        N1IMEU = diff > 0 ? diff : diff * -1,
                                        N1CHIU = "A",
                                        N1TIDO = "E",
                                        N1DIVI = "EUR",
                                        N1tmpPNR = "N",
                                        N1DRri = headOpen.N1docd
                                    };
                                    connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                }
                            }
                            var newDareOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = darePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareOpenRow, transaction);
                            var newAvereOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = averePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereOpenRow, transaction);

                            #endregion
                            #endregion
                        }

                        #region Registrazione apertura utile/perdita
                        #region PNTESTATA
                        var accountingOpenLGID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                        var headOpenLG = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year + 1,
                            N1REGI = accountingOpenLGID,
                            pncaus = esercizioCausal.cchria,
                            N1DARE = NewDate,
                            N1docn = accountingOpenLGID.ToString(),
                            N1docd = NewDate,
                            N1rifn = accountingOpenLGID.ToString(),
                            N1rifd = NewDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headOpenLG, transaction);
                        #endregion
                        int rowsOpenLGCounter = 1;
                        #region Righe
                        PNRIGHE newDiffOpenRow = new PNRIGHE()
                        {
                            N1SOCI = headOpenLG.N1SOCI,
                            N1ANNO = headOpenLG.N1ANNO,
                            N1REGI = headOpenLG.N1REGI,
                            N1RIGA = rowsOpenLGCounter++,
                            N1DOCU = headOpenLG.N1docn,
                            N1DADO = headOpenLG.N1docd,
                            N1RIFE = headOpenLG.N1rifn,
                            N1DARI = headOpenLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "A" : "D",
                            pngrup = diffLG >= 0 ? esercizioCausal.cchgrp : esercizioCausal.cchgru,
                            pncont = diffLG >= 0 ? esercizioCausal.cchctp : esercizioCausal.cchctu,
                            pnsott = diffLG >= 0 ? esercizioCausal.cchstp : esercizioCausal.cchstu,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headOpenLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newDiffOpenRow, transaction);
                        PNRIGHE newCPOpenRow = new PNRIGHE()
                        {
                            N1SOCI = headOpenLG.N1SOCI,
                            N1ANNO = headOpenLG.N1ANNO,
                            N1REGI = headOpenLG.N1REGI,
                            N1RIGA = rowsOpenLGCounter++,
                            N1DOCU = headOpenLG.N1docn,
                            N1DADO = headOpenLG.N1docd,
                            N1RIFE = headOpenLG.N1rifn,
                            N1DARI = headOpenLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "D" : "A",
                            pngrup = esercizioCausal.cchgrr,
                            pncont = esercizioCausal.cchctr,
                            pnsott = esercizioCausal.cchstr,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headOpenLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newCPOpenRow, transaction);
                        #endregion
                        #endregion

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

        public decimal ComputeLossProfit(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();


                var multi = connection.QueryMultiple(
                    @"SELECT (SUM(ISNULL(a.P4AVES, 0)) - SUM(ISNULL(a.P4DAES, 0))) AS rbalance FROM PDCANNI AS a
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=a.P1GRUP
                            WHERE g.P1TICO = 'R' AND a.P4ANNO=@yea AND a.P1SOCI=@cid;
                            SELECT (SUM(ISNULL(a.P4DAES, 0)) - SUM(ISNULL(a.P4AVES, 0))) AS cbalance FROM PDCANNI AS a
                            INNER JOIN PDCGRUPPI AS g ON g.P1GRUP=a.P1GRUP
                            WHERE g.P1TICO = 'C' AND a.P4ANNO=@yea AND a.P1SOCI=@cid;",
                    new { cid = CompanyID, yea = Year });

                return multi.Read<decimal>().Single() - multi.Read<decimal>().Single();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return 0;
            }
        }
        #endregion

        #region Self invoicing
        public string? GenerateSelfInvoice(PNTESTATA Head, ESERCIZIO AccountingYear, ABE Customer, CAUFAT00F Causal, tab_articolo Product, DateTime SelectedDate, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    int sequence = 1;
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var newInvoiceID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Head.N1SOCI, AccountingYear.eseann, Constants.INVOICE_TEMP, true);
                    if (newInvoiceID <= 0)
                    {
                        ErrorHandler.Show("Impossibile recuperare un nuovo numero di fattura provvisiorio");
                        return null;
                    }
                    var customerData = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Head.N1SOCI, Customer.abecod);
                    var clientData = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(Customer.abecod);
                    if (customerData == null || clientData == null)
                    {
                        ErrorHandler.Show("Impossibile proseguire per la mancanza dei dati amministrativi del cliente selezionato");
                        return null;
                    }
                    var payment = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetFull(customerData?.pclcod ?? string.Empty);
                    if (payment == null)
                    {
                        ErrorHandler.Show("Impossibile recuperare il tipo pagamento dal cliente selezionato");
                        return null;
                    }
                    var sourceInvoice = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetByAccountingRegistration(Head.N1CLFO ?? 0, Head.N1rifn ?? string.Empty);

                    // generate invoice head
                    var newHead = new FATTT00F()
                    {
                        ftsoci = Head.N1SOCI,
                        FTANNO = AccountingYear.eseann,
                        FTNUOR = newInvoiceID,
                        FTTIPO = "A",
                        FTDAOR = SelectedDate,
                        FTCAUS = Causal.fatcod,
                        FTCODC = Customer.abecod,
                        FTPAGA = customerData?.pclcod,
                        FTABIB = payment.Incasso?.icssup == "R" ? customerData?.CLABI : customerData?.banabi,
                        FTCABB = payment.Incasso?.icssup == "R" ? customerData?.CLCAB : customerData?.bancab,
                        FTBCON = payment.Incasso?.icssup == "R" ? null : customerData?.bancoc,
                        FTCONS = customerData?.concod,
                        FTSPED = customerData?.specod,
                        FTCORR = customerData?.vetcod,
                        FTIMBL = customerData?.CLIMBA,
                        FTCIDI = Head.pnvcod,
                        ftciva = Head.pnvdiv,
                        addedUserID = UserID,
                        FTDE25 = clientData?.CLREAC,
                        FTSCCL = customerData?.CLSCON,
                        FTAREA = customerData?.arecod,
                        FTRIVE = customerData?.rivcod,
                        FTCONZ = customerData?.CLCCON,
                        FTFILI = customerData?.filcod,
                        FTZONA = customerData?.CLZONE,
                        FTSETM = customerData?.CLSETM,
                        FTREGI = customerData?.CLREGI,
                        fttdoc = Causal.fattido
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().INSERT_QUERY, newHead, transaction);

                    // generate rows and update origins
                    foreach (var row in VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetList(Head.N1SOCI, Head.N1ANNO, Head.N1REGI) ?? new ObservableCollection<PNIVA>())
                    {
                        var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.N4ASSF ?? string.Empty, row.n4assa ?? string.Empty);
                        var newRow = new FATTD00F()
                        {
                            ftsoci = newHead.ftsoci,
                            FTANNO = newHead.FTANNO,
                            FTNUOR = newHead.FTNUOR,
                            FDRIGA = sequence++,
                            FDCODA = Product.ID,
                            FDQTAV = 1,
                            FDTQTA = "V",
                            FDPREZ = row.N4IMEU,
                            FDTPRE = "U",
                            FDASSF = row.N4ASSF,
                            FDALIV = row.n4assa,
                            FDGRUP = Product.GroupID,
                            FDCONT = Product.AccountID,
                            FDSCTO = Product.SubaccountID,
                            FDUNIM = Product.UnitaID,
                            FDNOTE = $"Autofattura relativa a fatt. {Head.N1rifn} del {(Head.N1rifd ?? DateTime.Now).ToString("dd/MM/yyyy")} prot. {Head.N1docn}",
                            FDSHOW = true,
                            fdtiva = rate?.assnatufe
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().INSERT_QUERY, newRow, transaction);
                    }

                    // generate self invoice data
                    var self = new FATTAUT()
                    {
                        FTAUSC = newHead.ftsoci,
                        FTAUAN = newHead.FTANNO,
                        FTAUNUM = newHead.FTNUOR,
                        FTAUCOF = Head.N1CLFO ?? 0,
                        FTAUDATRIC = sourceInvoice != null ? sourceInvoice.fattdataric : (Head.N1docd ?? DateTime.Now),
                        FTAUINDSDI = sourceInvoice?.fattidsdi,
                        FTAUDATFAT = sourceInvoice != null ? sourceInvoice.fattdata : Head.N1rifd,
                        FTAUNUMFAT = sourceInvoice != null ? sourceInvoice.fattnum : Head.N1rifn,
                        FTAPNAN = Head.N1ANNO,
                        FTAPNRE = Head.N1REGI
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().INSERT_QUERY, self, transaction);

                    // flag accounting registration
                    Head.N1AUAN = AccountingYear.eseann;
                    Head.N1AUNU = newInvoiceID;
                    Head.N1AUGE = now;
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().UPDATE_QUERY, Head, transaction);

                    transaction.Commit();
                    return $"{Head.N1AUAN}/{Head.N1AUNU} del {SelectedDate.ToString("dd/MM/yyyy")}";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return null;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region Printing
        public AccountingRecordReport? PrintAccountingRecord(PNTESTATA Head)
        {
            try
            {
                var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(Head.N1SOCI)!;

                Head.Rows = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().GetListPrint(Head.N1SOCI, Head.N1ANNO, Head.N1REGI);
                if (Head.N1FLCF == "C")
                {
                    Head.PrintDetailRows = new ObservableCollection<PrintRegViewModel>();
                    foreach (var item in VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().GetListByReg(Head.N1SOCI, Head.N1ANNO, Head.N1REGI) ?? new ObservableCollection<PNCLIENTI>())
                    {
                        Head.PrintDetailRows.Add(new PrintRegViewModel()
                        {
                            EntityTypeDescription = "Cliente",
                            Customer = item.CustomerDescription,
                            Expire = (item.N2SCAD ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            ReferenceID = item.N2RIFE,
                            ReferenceDate = (item.N2DARI ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            Amount = (item.N2IMEU ?? 0).ToString("N2"),
                            Sign = item.N2SEGN
                        });
                    }
                }
                else if (Head.N1FLCF == "F")
                {
                    Head.PrintDetailRows = new ObservableCollection<PrintRegViewModel>();
                    foreach (var item in VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().GetListByReg(Head.N1SOCI, Head.N1ANNO, Head.N1REGI) ?? new ObservableCollection<PNFORNITORI>())
                    {
                        Head.PrintDetailRows.Add(new PrintRegViewModel()
                        {
                            EntityTypeDescription = "Fornitore",
                            Customer = item.SupplierDescription,
                            Expire = (item.N3SCAD ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            ReferenceID = item.N3RIFE,
                            ReferenceDate = (item.N3DARI ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            Amount = (item.N3IMEU ?? 0).ToString("N2"),
                            Sign = item.N3SEGN
                        });
                    }
                }

                Head.VATRows = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetListFull(Head.N1SOCI, Head.N1ANNO, Head.N1REGI);

                return new AccountingRecordReport()
                {
                    Head = Head,
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Head.N1SOCI),
                    LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png")
                };
            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return null;
            }
        }

        #region General journal
        // page break vars
        private static decimal rowSpace = .4m;
        private static decimal rowFullSpace = .6m;
        private static decimal breakSpace = .2m;
        private static decimal minSpaceToAddRow = .4m;
        private static decimal minSpaceToAddFullRow = .6m;
        private static decimal minSpaceToAddDayBreak = .2m;
        private static decimal maxSpaceAvailable = 25m; // is the space available for each page in cm
        public GeneralJournalReport? PrintGeneralJournal(string CompanyID, int AccountingYear, DateTime PrintUntil)
        {
            try
            {
                var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, AccountingYear)!;

                using var connection = GetOpenConnection();

                var rows = connection.Query<GeneralJournalReport.RowInfo>(
                    @"SELECT (@start + ROW_NUMBER() over (order by (select NULL))) AS RowNumber, t.pncaus + ' ' + c.caudes AS CausalFullDescription, t.N1DARE AS RegistrationDate, t.N1REGI AS RegistrationNumber, r.N1RIGA AS RegistrationRow, TRIM(t.n1docn) AS DocumentNumber, t.n1docd AS DocumentDate, r.pngrup AS GroupID, r.pncont AS AccountID, r.pnsott AS SubaccountID, TRIM(r.N1DESC) AS Note, r.N1IMEU AS Amount, r.N1SEGN AS Sign, TRIM(p.P3DES1) AS PDCDescription, TRIM(abe.abers1) AS EntityDescription FROM PNRIGHE AS r
                            INNER JOIN PNTESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN CAUCONT AS c ON c.caucod=t.pncaus
                            INNER JOIN PDCSOTTO AS p ON p.P1GRUP=r.pngrup AND p.P2CONT=r.pncont AND p.P3SOTC=r.pnsott
                            LEFT JOIN ABE AS abe ON abe.abecod = r.N1CLIE
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE<=@until AND (r.N1STBO IS NULL OR r.N1STBO = '')
                            ORDER BY t.N1DARE, t.N1REGI, r.N1RIGA",
                    new { cid = CompanyID, yea = AccountingYear, until = PrintUntil, start = esercizio.eseulr ?? 0 });

                var displayRows = new List<GeneralJournalReport.RowInfo>();

                #region Vars
                decimal dareTop = 0;
                decimal avereTop = 0;
                decimal spaceUsed = 0;
                DateTime? lastDate = null;
                decimal avereDay = 0;
                decimal dareDay = 0;
                dareTop = esercizio.esedar ?? 0;
                avereTop = esercizio.eseave ?? 0;
                bool first = true;
                #endregion

                if (rows != null && rows.Count() > 0)
                {
                    foreach (var row in rows)
                    {
                        if (first)
                        {
                            displayRows.Add(new GeneralJournalReport.RowInfo()
                            {
                                IsPageTop = true,
                                AvereTop = avereTop,
                                DareTop = dareTop
                            });
                            spaceUsed += breakSpace;
                            first = false;
                        }

                        if (lastDate != row.RegistrationDate || !lastDate.HasValue)
                        {
                            if (lastDate.HasValue)
                            {
                                if (maxSpaceAvailable - spaceUsed >= minSpaceToAddDayBreak + breakSpace)
                                {
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsDayTotal = true,
                                        DayBreakText = $"Progressivi del giorno {lastDate.Value.ToString("dd/MM/yyyy")}",
                                        AvereDayTotal = avereDay,
                                        DareDayTotal = dareDay
                                    });
                                    spaceUsed += breakSpace;
                                }
                                else
                                {
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsPageBottom = true,
                                        AvereBottom = avereTop,
                                        DareBottom = dareTop
                                    });
                                    spaceUsed += breakSpace;
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsPageTop = true,
                                        AvereTop = avereTop,
                                        DareTop = dareTop
                                    });
                                    spaceUsed = breakSpace;
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsDayTotal = true,
                                        DayBreakText = $"Progressivi del giorno {lastDate}",
                                        AvereDayTotal = avereDay,
                                        DareDayTotal = dareDay
                                    });
                                    spaceUsed += breakSpace;
                                }
                            }
                            if (row.Sign == "D")
                            {
                                dareDay = (row.Amount ?? 0);
                                avereDay = 0;
                            }
                            else
                            {
                                avereDay = (row.Amount ?? 0);
                                dareDay = 0;
                            }

                            AddJournalRow(displayRows, row, ref dareTop, ref avereTop, ref spaceUsed);
                            lastDate = row.RegistrationDate;
                        }
                        else
                        {
                            if (row.Sign == "D")
                                dareDay += (row.Amount ?? 0);
                            else
                                avereDay += (row.Amount ?? 0);

                            AddJournalRow(displayRows, row, ref dareTop, ref avereTop, ref spaceUsed);
                        }
                        if (row.Sign == "D")
                            dareTop += (row.Amount ?? 0);
                        else
                            avereTop += (row.Amount ?? 0);
                    }
                    // add last day break
                    displayRows.Add(new GeneralJournalReport.RowInfo()
                    {
                        IsDayTotal = true,
                        DayBreakText = $"Progressivi del giorno {(lastDate ?? DateTime.MinValue).ToString("dd/MM/yyyy")}",
                        AvereDayTotal = avereDay,
                        DareDayTotal = dareDay
                    });
                    // add last page break
                    displayRows.Add(new GeneralJournalReport.RowInfo()
                    {
                        IsPageBottom = true,
                        AvereBottom = avereTop,
                        DareBottom = dareTop
                    });
                }
                return new GeneralJournalReport()
                {
                    AccountingYear = AccountingYear,
                    StartingPage = esercizio.esepag ?? 0,
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    Rows = displayRows,
                    DareTotal = dareTop,
                    AvereTotal = avereTop,
                    PrintLimit = PrintUntil
                };

            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return null;
            }
        }

        public bool UpdateJournalDefinitives(GeneralJournalReport ReportData, int LastPagePrinted, string CompanyID, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    // update ESERCIZIO
                    connection.Execute("UPDATE ESERCIZIO SET esepag=@esepag, eseulr=@eseulr, eseusg=@eseusg, esedar=@esedar, eseave=@eseave, updatedUserID=@uid, updated=@eseusg  WHERE esesoc=@cid AND eseann=@yea",
                        new
                        {
                            esepag = LastPagePrinted,
                            eseulr = ((ReportData.Rows ?? new List<GeneralJournalReport.RowInfo>()).Where(w => !w.IsDayTotal && !w.IsPageTop && !w.IsPageBottom).Max(max => max.RowNumber) ?? 0),
                            eseusg = ReportData.PrintLimit,
                            esedar = ReportData.DareTotal,
                            eseave = ReportData.AvereTotal,
                            cid = CompanyID,
                            yea = ReportData.AccountingYear,
                            uid = UserID
                        }, transaction);
                    // update rows printed
                    Parallel.ForEach((ReportData.Rows ?? new List<GeneralJournalReport.RowInfo>()).Where(w => !w.IsDayTotal && !w.IsPageTop && !w.IsPageBottom), row =>
                    {
                        connection.Execute("UPDATE PNRIGHE SET N1STBO = '*', N1RIGB=@n1rigb, N1DABB=@now WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1REGI=@regi AND N1RIGA=@riga",
                            new { now = ReportData.PrintLimit.Date, cid = CompanyID, n1rigb = row.RowNumber, yea = ReportData.AccountingYear, regi = row.RegistrationNumber, riga = row.RegistrationRow }, transaction);
                    });
                    transaction.Commit();
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
                return false;
            }
        }

        public void AddJournalRow(List<GeneralJournalReport.RowInfo> displayRows, GeneralJournalReport.RowInfo row, ref decimal dareTop, ref decimal avereTop, ref decimal spaceUsed)
        {
            if (maxSpaceAvailable - spaceUsed >= (string.IsNullOrWhiteSpace(row.Note) ? minSpaceToAddRow : minSpaceToAddFullRow) + breakSpace)
            {
                displayRows.Add(row);
                if (!string.IsNullOrWhiteSpace(row.Note))
                    spaceUsed += rowFullSpace;
                else
                    spaceUsed += rowSpace;
            }
            else
            {
                displayRows.Add(new GeneralJournalReport.RowInfo()
                {
                    IsPageBottom = true,
                    AvereBottom = avereTop,
                    DareBottom = dareTop
                });
                spaceUsed += breakSpace;
                displayRows.Add(new GeneralJournalReport.RowInfo()
                {
                    IsPageTop = true,
                    AvereTop = avereTop,
                    DareTop = dareTop
                });
                spaceUsed = breakSpace;
                displayRows.Add(row);
                if (!string.IsNullOrWhiteSpace(row.Note))
                    spaceUsed += rowFullSpace;
                else
                    spaceUsed += rowSpace;
            }
        }
        #endregion
        #endregion
    }

    public class AccountingUfpRepository : RepositoryBase, IAccountingRepository
    {
        public AccountingUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<PNTESTATA>? GetHeadList(string CompanyID, int Year, string? FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount)
        {
            TotalCount = 0;
            try
            {
                using var connection = GetOpenConnection();


                var aliasList = new List<GenericIDDescriptionType>() {
                    new GenericIDDescriptionType(){ ID = "NotBalanced", Description="nb", Type = "#BF#sq.Amount <> sq.OtherAmount#sq.Amount = sq.OtherAmount#" },
                    new GenericIDDescriptionType(){ ID = "N1TmpPNBool", Description="N1TmpPN", Type = "#B#S#N#" },
                    new GenericIDDescriptionType(){ ID = "N1REGIText", Description="N1REGI" },
                    new GenericIDDescriptionType(){ ID = "N1CLFOText", Description="N1CLFO" },
                    new GenericIDDescriptionType(){ ID = "N1DAREText", Description="N1DARE", Type="D" },
                    new GenericIDDescriptionType(){ ID = "pncaus", Description="pncaus" },
                    new GenericIDDescriptionType(){ ID = "Amount", Description="Amount", Type = "#C#(SELECT SUM(N1IMEU) FROM PNRIGHE WHERE N1SEGN = 'D' AND N1SOCI = sq.N1SOCI AND N1ANNO = sq.N1ANNO AND N1REGI = sq.N1REGI)" },
                    new GenericIDDescriptionType(){ ID = "N1CLFO", Description="N1CLFO" },
                    new GenericIDDescriptionType(){ ID = "AccountingCausal.caudes", Description="caudes" },
                    new GenericIDDescriptionType(){ ID = "N1docn", Description="N1docn" },
                    new GenericIDDescriptionType(){ ID = "N1rifn", Description="N1rifn" },
                    new GenericIDDescriptionType(){ ID = "EntityFullDescription", Description = "EntityFullDescription", Type = "#C#CONCAT(TRIM(sq.abers1) , ' ', TRIM(sq.abers2))"} };

                #region Args
                FullTextSearch = FullTextSearch?.ToLower();
                var args = new DynamicParameters();
                args.Add("CompanyID", CompanyID);
                args.Add("Year", Year);
                args.Add("skip", RequestedPage * PageSize);
                args.Add("ps", PageSize);
                args.Add("ft", $"%{FullTextSearch}%");
                #endregion
                #region Where
                var whereFilter = new StringBuilder("sq.N1SOCI = @CompanyID AND sq.N1ANNO = @Year ");
                // grid filters
                TelerikGridService.ComputeFilter(whereFilter, FilterList, aliasList, args);
                // full-text search
                if (!string.IsNullOrWhiteSpace(FullTextSearch))
                {
                    whereFilter.Append($@"AND (LOWER(caudes) LIKE @ft OR LOWER(pncaus) LIKE @ft OR LOWER(CONVERT(nvarchar(20), N1REGI)) LIKE @ft OR
                                            LOWER(N1docn) LIKE @ft OR LOWER(N1rifn) LIKE @ft OR
                                            LOWER(abers1) LIKE @ft OR LOWER(abers2) LIKE @ft OR LOWER(CONVERT(nvarchar(10),N1CLFO)) LIKE @ft)");
                }
                #endregion
                #region Sort
                var sort = TelerikGridService.ComputeSort(SortList, aliasList);
                #endregion
                TotalCount = (int?)connection.ExecuteScalar($@"SELECT COUNT(*) FROM (SELECT t.*,
                                                        (SELECT SUM(N1IMEU) FROM PN_RIGHE WHERE N1SEGN = 'D' AND N1SOCI = t.N1SOCI AND N1ANNO = t.N1ANNO AND N1REGI = t.N1REGI) as Amount, 
                                                        (SELECT SUM(N1IMEU) FROM PN_RIGHE WHERE N1SEGN = 'A' AND N1SOCI = t.N1SOCI AND N1ANNO = t.N1ANNO AND N1REGI = t.N1REGI) as OtherAmount,
                                                        l.caudes, base.abers1, base.abers2, CONCAT(TRIM(base.abers1) , ' ', TRIM(base.abers2)) AS EntityFullDescription
                                                        FROM PN_TESTATA AS t 
                                                        INNER JOIN CAUCONT AS l ON t.PNCAUS = l.CAUCOD 
                                                        LEFT JOIN ANAG_BASE AS base ON base.abecod = t.N1CLFO) AS sq
                                                        WHERE {whereFilter.ToString()};", args) ?? 0;

                var list = connection.Query<PNTESTATA, CAUCONT, PNTESTATA>(
                    $@"SELECT *, ABS(Amount - OtherAmount) AS nb
                        FROM
                        (   
                            SELECT 
                                t.*,
                                CONCAT(TRIM(base.abers1), ' ', TRIM(base.abers2)) AS EntityFullDescription,

                                -- Aggregated sums instead of correlated subqueries
                                ISNULL(r.Amount, 0) AS Amount,
                                ISNULL(r.OtherAmount, 0) AS OtherAmount,

                                l.caudes,
                                l.caugen,
                                l.cauiva,
                                l.caucli,
                                l.caufor,
                                l.cauter,
                                l.cauint,

                                base.abers1,
                                base.abers2

                            FROM PN_TESTATA AS t
                            INNER JOIN CAUCONT AS l 
                                ON t.PNCAUS = l.CAUCOD
                            LEFT JOIN ANAG_BASE AS base 
                                ON base.abecod = t.N1CLFO

                            LEFT JOIN
                            (
                                SELECT 
                                    N1SOCI,
                                    N1ANNO,
                                    N1REGI,
                                    SUM(CASE WHEN N1SEGN = 'D' THEN N1IMEU ELSE 0 END) AS Amount,
                                    SUM(CASE WHEN N1SEGN = 'A' THEN N1IMEU ELSE 0 END) AS OtherAmount
                                FROM PN_RIGHE
                                GROUP BY N1SOCI, N1ANNO, N1REGI
                            ) r
                                ON r.N1SOCI = t.N1SOCI
                               AND r.N1ANNO = t.N1ANNO
                               AND r.N1REGI = t.N1REGI

                        ) AS sq
                        WHERE {whereFilter.ToString()}
                        {(!string.IsNullOrWhiteSpace(sort) ? sort : "ORDER BY N1DARE DESC, N1REGI DESC ")}
                        OFFSET @skip ROWS 
                        FETCH NEXT @ps ROWS ONLY",
                    (pn, caus) =>
                    {
                        pn.AccountingCausal = caus; return pn;
                    },
                    args,
                    splitOn: "caudes");

                return new ObservableCollection<PNTESTATA>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<PNRIGHE>? GetHeadTemporaryPeriodList(string CompanyID, int Year, string? MonthFlag, DateTime? From, DateTime? To, string GroupID, string AccountID, string SubaccountID)
        {
            try
            {
                using var connection = GetOpenConnection();


                // generate month flag where
                string? monthFlagWhere = null;
                if (!string.IsNullOrWhiteSpace(MonthFlag))
                {
                    monthFlagWhere = $"AND DATEFROMPARTS(YEAR(l.N1DARE),MONTH(l.N1DARE),1) {MonthFlag} " +
                        $"DATEFROMPARTS(YEAR(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'), MONTH(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'),1)";
                }

                string queryCommand = string.Empty;
                if (From.HasValue && To.HasValue)
                {
                    queryCommand = $@"SELECT r.*, l.*, c.caudes, base.abecod, base.abers1, base.abers2 FROM PNRIGHE AS r
                            INNER JOIN PN_TESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                            INNER JOIN ANAG_BASE AS c ON l.PNCAUS = c.CAUCOD
                            LEFT OUTER JOIN ABE AS base ON base.abecod = l.N1CLFO
                            WHERE r.n1soci = @cid AND r.N1ANNO = @yea {monthFlagWhere} AND l.N1DARE >= @from AND l.N1DARE <= @to AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND r.N1tmpPNR = 'S'
                            ORDER BY l.N1DARE ASC";
                }
                else
                {
                    if (From.HasValue && !To.HasValue)
                    {
                        queryCommand = $@"SELECT r.*, l.*, c.caudes, base.abecod, base.abers1, base.abers2 FROM PNRIGHE AS r
                                                INNER JOIN PN_TESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                                                INNER JOIN CAUCONT AS c ON l.PNCAUS = c.CAUCOD
                                                LEFT OUTER JOIN ANAG_BASE AS base ON base.abecod = l.N1CLFO
                                                WHERE r.n1soci = @cid AND r.N1ANNO = @yea {monthFlagWhere} AND l.N1DARE <= @from AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND r.N1tmpPNR = 'S'
                                                ORDER BY l.N1DARE ASC";
                    }
                    else
                    {
                        if (!From.HasValue && To.HasValue)
                        {
                            queryCommand = $@"SELECT r.*, l.*, c.caudes, base.abecod, base.abers1, base.abers2 FROM PNRIGHE AS r
                                                    INNER JOIN PN_TESTATA AS l ON r.N1SOCI = l.N1SOCI AND r.N1ANNO = l.N1ANNO AND r.N1REGI = l.N1REGI
                                                    INNER JOIN CAUCONT AS c ON l.PNCAUS = c.CAUCOD
                                                    LEFT OUTER JOIN ANAG_BASE AS base ON base.abecod = l.N1CLFO
                                                    WHERE r.n1soci = @cid AND r.N1ANNO = @yea {monthFlagWhere} AND l.N1DARE > @to AND r.pngrup = @group AND r.pncont = @account AND r.pnsott = @subaccount AND r.N1tmpPNR = 'S'
                                                    ORDER BY l.N1DARE ASC";
                        }
                    }
                }
                var list = connection.Query<PNRIGHE, PNTESTATA, CAUCONT, ABE, PNRIGHE>(
                    queryCommand,
                    (row, head, cau, bas) => { head.AccountingCausal = cau; head.BasicRegistry = bas; row.Testata = head; return row; },
                    new { cid = CompanyID, yea = Year, from = From, to = To, group = GroupID, account = AccountID, subaccount = SubaccountID },
                    splitOn: "N1SOCI,caudes,abecod");
                return new ObservableCollection<PNRIGHE>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool RunEqualization(string CompanyID, string EntityType, DateTime UntilDate)
        {
            try
            {
                using var connection = GetOpenConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@tipo", EntityType);
                parameters.Add("@societa", CompanyID);
                parameters.Add("@data_registrazione", UntilDate);

                connection.Execute(
                    "PareggioPartite",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        #region Receipts registration
        public string? AccountingReceipts(string CompanyID, DateTime RegistrationDate, ABE SelectedCustomer, CAUCONT SelectedCausal, string DocumentID, ObservableCollection<ReceiptInfo> Rows, decimal TotalTax, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    // get registration number
                    var newAccountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, RegistrationDate.Year, Constants.PN, true);
                    // CLIAMMI
                    var cliammi = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, SelectedCustomer.abecod);
                    // IVA book
                    var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(SelectedCausal.cauliv ?? string.Empty);
                    // CAUCONT_GROUPS
                    var grpcau = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetListNoCR(CompanyID, SelectedCausal.caucod);
                    var ricavo = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetFirstSign(CompanyID, SelectedCausal.caucod, "D");
                    // PAGCLI/TAB_ACC_TIPINC
                    var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(cliammi?.pclcod ?? string.Empty);
                    var incassi = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Get(pagcli?.pcltip ?? string.Empty);

                    #region PNTESTATA
                    PNTESTATA head = new PNTESTATA()
                    {
                        N1SOCI = CompanyID,
                        N1ANNO = RegistrationDate.Year,
                        N1REGI = newAccountingID,
                        pncaus = SelectedCausal.caucod,
                        N1DARE = RegistrationDate.Date,
                        N1docn = DocumentID,
                        N1docd = RegistrationDate.Date,
                        N1rifn = DocumentID,
                        N1rifd = RegistrationDate.Date,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1CLFO = SelectedCustomer.abecod,
                        N1FLCF = "C",
                        N1FL01 = string.Empty,
                        N1TmpPN = "N",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().INSERT_QUERY, head, transaction);
                    #endregion

                    int rowsCounter = 1;

                    #region Customer row
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
                        N1SEGN = "D",
                        pngrup = ricavo?.grpgrp,
                        pncont = ricavo?.grpcto,
                        pnsott = ricavo?.grpsct,
                        N1IMEU = Rows.Sum(sum => sum.Amount),
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = cliammi?.pclcod,
                        n1scad = head.N1DARE,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, customerRow, transaction);
                    #endregion

                    #region Details rows
                    foreach (var row in Rows)
                    {
                        var newRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = "A",
                            pngrup = row.SelectedGroup?.P1GRUP,
                            pncont = row.SelectedAccount?.P2CONT,
                            pnsott = row.SelectedSubaccount?.P3SOTC,
                            N1IMEU = row.Taxable,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            n1paga = cliammi?.pclcod,
                            n1scad = head.N1DARE,
                            N1DRri = head.N1docd,
                            N1STBO = string.Empty,
                            N1STMA = string.Empty,
                            N1STNO = string.Empty,
                            N1CCCC = row.CostCenter?.cecodc
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, newRow, transaction);
                    }
                    #endregion

                    #region IVA row
                    if (TotalTax > 0)
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
                            N1SEGN = grpcau?.grpseg,
                            pngrup = grpcau?.grpgrp,
                            pncont = grpcau?.grpcto,
                            pnsott = grpcau?.grpsct,
                            N1IMEU = Rows.Sum(sum => sum.Tax),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1STBO = string.Empty,
                            N1STMA = string.Empty,
                            N1STNO = string.Empty,
                            N1DRri = head.N1docd,
                            N1tmpPNR = "N",
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, ivaRow, transaction);
                    }
                    #endregion

                    #region PNIVA
                    int ivaRowsCounter = 1;
                    List<string> rates = new List<string>();
                    foreach (var row in Rows.OrderBy(o => o.Rate?.asscod).ThenBy(o => o.Rate?.assali))
                    {
                        if (!rates.Contains(row.Rate?.asscod + row.Rate?.assali.Trim()))
                        {
                            rates.Add(row.Rate?.asscod + row.Rate?.assali.Trim());
                        }
                    }
                    foreach (var rate in rates)
                    {
                        var imponibile = Rows
                            .Where(w => w.Rate?.asscod == rate.Substring(0, 1) && w.Rate?.assali == rate.Substring(1, rate.Length - 1))
                            .Sum(sum => sum.Taxable);
                        var imposta = Rows
                            .Where(w => w.Rate?.asscod == rate.Substring(0, 1) && w.Rate?.assali == rate.Substring(1, rate.Length - 1))
                            .Sum(sum => sum.Tax);
                        PNIVA pnivaRow = new PNIVA()
                        {
                            N4SOCI = CompanyID,
                            N4ANNO = head.N1ANNO,
                            N4REGI = newAccountingID,
                            N4RIGA = ivaRowsCounter++,
                            N4DOCU = head.N1docn,
                            N4RIFE = head.N1rifn,
                            N4DARE = head.N1DARE,
                            N4DADO = head.N1docd,
                            N4DARI = head.N1rifd,
                            N4CAUS = head.pncaus,
                            N4SEGN = SelectedCausal.causeg,
                            N4LIBR = ivaBook?.livcod,
                            N4SOTT = head.N1CLFO,
                            N4TCLF = "C",
                            N4ASSF = rate.Substring(0, 1),
                            n4assa = rate.Substring(1, rate.Length - 1),
                            N4TIDO = "E",
                            n4donu = int.Parse(head.N1docn),
                            N4DAST = Constants._GX_MIN_DATE,
                            N4FLGS = "",
                            n4tmppn = "N",
                            N4DTSCAD = head.N1DARE,
                            N4DTPGEF = null,
                            N4DTSCPG = head.N1DARE,
                            N4IMEU = imponibile,
                            N4IVEU = imposta,
                            N4IIEU = 0,
                            N4FLIVCA = "N",
                            N4FLSPESO = string.Empty,
                            N4IMPPAG = 0
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, pnivaRow, transaction);
                    }
                    #endregion

                    transaction.Commit();
                    return $"{head.N1ANNO}/{head.N1REGI}";
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public string? RegistrationAccounting(string CompanyID, int AccountingYear, DateTime RegistrationDate, ABE Entity, string EntityType, CAUCONT SelectedCausal, BANAZIEN SelectedBank, string? DocumentID, DateTime? DocumentDate, ObservableCollection<MastrinoECReportItem> Rows, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();

                using var transaction = connection.BeginTransaction();

                var pnRigheRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var pdcSottoRepo = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>();
                var pnClientiRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>();
                var pnFornitoriRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>();

                // get registration number
                var newAccountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, AccountingYear, Constants.PN, true);

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = CompanyID,
                    N1ANNO = RegistrationDate.Year,
                    N1REGI = newAccountingID,
                    pncaus = SelectedCausal.caucod,
                    N1DARE = RegistrationDate.Date,
                    N1docn = DocumentID,
                    N1docd = DocumentDate?.Date,
                    N1rifn = DocumentID,
                    N1rifd = DocumentDate?.Date,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Entity.abecod,
                    N1FLCF = EntityType,
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().INSERT_QUERY, head, transaction);
                #endregion

                int rowsCounter = 1;

                #region Rows
                foreach (var row in Rows)
                {
                    object? referenceRow = null;
                    PDCSOTTO? subaccount = null;

                    if (row.EntityType == "C")
                    {
                        referenceRow = connection.Query<PNCLIENTI>(
                                        "SELECT * FROM PN_CLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                                            new { n2soci = row.CompanyID, n2anno = row.Year, n2regi = row.Number, n2riga = row.RowID }, transaction).FirstOrDefault();

                        if (referenceRow != null && referenceRow is PNCLIENTI)
                            subaccount = connection.Query<PDCSOTTO>(
                                            @"SELECT TOP(1) * FROM PDC_SOTTOCONTI 
                                                WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3CLFO = @p3clfo AND (NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci)
                                                ORDER BY P1GRUP, P2CONT, P3SOTC",
                                                    new { p1grup = (referenceRow as PNCLIENTI)!.N2GRUP!, p2cont = (referenceRow as PNCLIENTI)!.N2CONT!, p3clfo = "C", p3soci = CompanyID }, transaction).FirstOrDefault();
                    }
                    else
                    {
                        referenceRow = connection.Query<PNFORNITORI>(
                                        "SELECT * FROM PN_FORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi AND N3RIGA = @n3riga",
                                             new { n3soci = row.CompanyID, n3anno = row.Year, n3regi = row.Number, n3riga = row.RowID }, transaction).FirstOrDefault();

                        if (referenceRow != null && referenceRow is PNFORNITORI)
                            subaccount = connection.Query<PDCSOTTO>(
                                  @"SELECT TOP(1) * FROM PDC_SOTTOCONTI 
                                                WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3CLFO = @p3clfo AND (NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci)
                                                ORDER BY P1GRUP, P2CONT, P3SOTC",
                                          new { p1grup = (referenceRow as PNFORNITORI)!.N3GRUP!, p2cont = (referenceRow as PNFORNITORI)!.N3CONT!, p3clfo = "F", p3soci = CompanyID }, transaction).FirstOrDefault();
                    }

                    PNRIGHE riga = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = row.ReferenceID,
                        N1DARI = row.ReferenceDate,
                        N1SEGN = (row.Segno == "D") ? "A" : "D",
                        pngrup = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2GRUP : (referenceRow as PNFORNITORI)?.N3GRUP,
                        pncont = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2CONT : (referenceRow as PNFORNITORI)?.N3CONT,
                        pnsott = subaccount?.P3SOTC,
                        N1IMEU = row.Valore,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1clie = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2SOTT : (referenceRow as PNFORNITORI)?.N3SOTT,
                        n1paga = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2PAGA : (referenceRow as PNFORNITORI)?.N3PAGA,
                        n1scad = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2SCAD : (referenceRow as PNFORNITORI)?.N3SCAD,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(pnRigheRepo.INSERT_QUERY, riga, transaction);

                    if (row.EntityType == "C")
                    {
                        PNCLIENTI customerRow = new PNCLIENTI()
                        {
                            N2SOCI = head.N1SOCI,
                            N2ANNO = head.N1ANNO,
                            N2REGI = head.N1REGI,
                            N2RIGA = (rowsCounter - 1),
                            n2rior = (rowsCounter - 1),
                            N2DARE = head.N1DARE,
                            N2DOCU = head.N1docn,
                            N2DADO = head.N1docd,
                            N2RIFE = row.ReferenceID,
                            N2DARI = row.ReferenceDate,
                            N2CAUS = head.pncaus,
                            N2SEGN = riga.N1SEGN,
                            N2GRUP = riga.pngrup,
                            N2CONT = riga.pncont,
                            N2SOTT = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2SOTT : (referenceRow as PNFORNITORI)?.N3SOTT,
                            N2IMEU = row.Valore,
                            N2TIDO = "E",
                            N2DIVI = "EUR",
                            N2DIDO = "EUR",
                            N2PAGA = riga.n1paga,
                            N2SCAD = riga.n1scad,
                            N2PARE = string.Empty,
                        };
                        connection.Execute(pnClientiRepo.INSERT_QUERY, customerRow, transaction);
                    }
                    else
                    {
                        PNFORNITORI supplierRow = new PNFORNITORI()
                        {
                            N3SOCI = head.N1SOCI,
                            N3ANNO = head.N1ANNO,
                            N3REGI = head.N1REGI,
                            N3RIGA = (rowsCounter - 1),
                            n3rior = (rowsCounter - 1),
                            N3DOCU = head.N1docn,
                            N3DARE = head.N1DARE,
                            N3DADO = head.N1docd,
                            N3RIFE = row.ReferenceID,
                            N3DARI = row.ReferenceDate,
                            N3CAUS = head.pncaus,
                            N3SEGN = riga.N1SEGN,
                            N3GRUP = riga.pngrup,
                            N3CONT = riga.pncont,
                            N3SOTT = (row.EntityType == "C") ? (referenceRow as PNCLIENTI)?.N2SOTT : (referenceRow as PNFORNITORI)?.N3SOTT,
                            N3IMEU = row.Valore,
                            N3TIDO = "E",
                            N3DIVI = "EUR",
                            N3DIDO = "EUR",
                            N3PAGA = riga.n1paga,
                            N3SCAD = riga.n1scad,
                            N3PARE = string.Empty,
                        };
                        connection.Execute(pnFornitoriRepo.INSERT_QUERY, supplierRow, transaction);
                    }
                }

                //BANK ROW
                PNRIGHE bankRiga = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = rowsCounter++,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1docn,
                    N1DARI = head.N1docd,
                    N1SEGN = (EntityType == "C") ? "D" : "A",
                    pngrup = SelectedBank.abigba,
                    pncont = SelectedBank.abicba,
                    pnsott = SelectedBank.abisba,
                    N1IMEU = (EntityType == "C") ? Rows.Where(o => o.Segno == "D").Sum(s => s.Valore) - Rows.Where(o => o.Segno == "A").Sum(s => s.Valore) : Rows.Where(o => o.Segno == "A").Sum(s => s.Valore) - Rows.Where(o => o.Segno == "D").Sum(s => s.Valore),
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "N",
                    n1paga = string.Empty,
                    n1scad = Constants._GX_MIN_DATE,
                    N1DRri = head.N1docd,
                    N1STBO = string.Empty,
                    N1STMA = string.Empty,
                    N1STNO = string.Empty,
                    N1DESC = $"{Entity.abecod}-{Entity.abers1?.TrimEnd()}"
                };
                connection.Execute(pnRigheRepo.INSERT_QUERY, bankRiga, transaction);
                #endregion

                transaction.Commit();
                return $"{head.N1ANNO}/{head.N1REGI}";
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region Accounting situation

        public AccountingSituationViewModel? GetAccountingSituation(string CompanyID, int Year, string? CostCenterID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new AccountingSituationViewModel()
                {
                    AttivitaGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>(),
                    PassivitaGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>(),
                    RicaviGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>(),
                    CostiGruppi = new ObservableCollection<AccountingSituationViewModel.ASItem>()
                };
                Parallel.ForEach(new string[] { "A", "P", "R", "C" }, groupType =>
                {
                    var groupsList = connection.Query<PDCGRUPPI>(@"SELECT P1GRUP, TRIM(P1DES1) AS P1DES1 FROM PDC_GRUPPI WHERE P1TICO = @groupType", new { groupType = groupType });
                    foreach (var grp in groupsList)
                    {
                        var multi = connection.QueryMultiple(
                            $@"SELECT SUM(N1IMEU) FROM PN_RIGHE
                                  WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND N1SEGN = 'D' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};
                                  SELECT SUM(N1IMEU) FROM PN_RIGHE
                                  WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND N1SEGN = 'A' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};",
                                new { cid = CompanyID, yea = Year, pngrup = grp.P1GRUP, cc = CostCenterID });
                        var newGroup = new AccountingSituationViewModel.ASItem()
                        {
                            GroupID = grp.P1GRUP,
                            Description = grp.FullDescriptionSearchable,
                            IsDare = groupType == "A" || groupType == "C",
                            Dare = multi.Read<decimal?>().Single() ?? 0m,
                            Avere = multi.Read<decimal?>().Single() ?? 0m,
                            Accounts = new ObservableCollection<AccountingSituationViewModel.ASItem>()
                        };

                        if (newGroup.Saldo != 0)
                        {
                            // accounts
                            foreach (var acc in VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList(grp.P1GRUP) ?? new ObservableCollection<PDCCONTI>())
                            {
                                var multiAccount = connection.QueryMultiple(
                                    $@"SELECT SUM(N1IMEU) FROM PN_RIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND N1SEGN = 'D' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};
                                      SELECT SUM(N1IMEU) FROM PN_RIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND N1SEGN = 'A' {(string.IsNullOrWhiteSpace(CostCenterID) ? null : " AND N1CCCC = @cc")};",
                                    new { cid = CompanyID, yea = Year, pngrup = grp.P1GRUP, pncont = acc.P2CONT, cc = CostCenterID });
                                var newAccount = new AccountingSituationViewModel.ASItem()
                                {
                                    GroupID = acc.P1GRUP,
                                    AccountID = acc.P2CONT,
                                    Description = acc.FullDescriptionSearchable,
                                    IsDare = groupType == "A" || groupType == "C",
                                    Dare = multiAccount.Read<decimal?>().Single() ?? 0m,
                                    Avere = multiAccount.Read<decimal?>().Single() ?? 0m
                                };
                                if (newAccount.Saldo != 0)
                                    newGroup.Accounts.Add(newAccount);
                            }
                            switch (groupType)
                            {
                                case "A":
                                    result.AttivitaGruppi.Add(newGroup);
                                    break;
                                case "P":
                                    result.PassivitaGruppi.Add(newGroup);
                                    break;
                                case "R":
                                    result.RicaviGruppi.Add(newGroup);
                                    break;
                                case "C":
                                    result.CostiGruppi.Add(newGroup);
                                    break;
                            }
                        }
                    }
                });
                return result;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ASItem>? GetAccountingSituationDetails(string CompanyID, int Year, string GroupID, bool IsDare, string AccountID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var result = new ObservableCollection<ASItem>();
                foreach (var sub in VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(GroupID, AccountID, CompanyID) ?? new ObservableCollection<PDCSOTTO>())
                {
                    var multi = connection.QueryMultiple(
                                @"SELECT SUM(N1IMEU) FROM PN_RIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND pnsott = @pnsott AND N1SEGN = 'D';
                                      SELECT SUM(N1IMEU) FROM PN_RIGHE
                                      WHERE N1ANNO = @yea AND N1SOCI = @cid AND pngrup = @pngrup AND pncont = @pncont AND pnsott = @pnsott AND N1SEGN = 'A';",
                                new { cid = CompanyID, yea = Year, pngrup = GroupID, pncont = AccountID, pnsott = sub.P3SOTC });
                    var newSub = new AccountingSituationViewModel.ASItem()
                    {
                        GroupID = GroupID,
                        Description = sub.FullDescriptionSearchable,
                        IsDare = IsDare,
                        Dare = multi.Read<decimal?>().Single() ?? 0m,
                        Avere = multi.Read<decimal?>().Single() ?? 0m
                    };
                    if (newSub.Saldo != 0)
                    {
                        result.Add(newSub);
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

        public string? ExportCEEBalance(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<PDCSOTTO, PDCGRUPPI, PDCSOTTO>(
                    @"SELECT s.*, g.P1TICO FROM PDC_SOTTOCONTI AS s
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP = s.P1GRUP
                            WHERE (s.p3soci = @CompanyID OR s.p3soci IS NULL)
                            ORDER BY s.P1GRUP, s.P2CONT, s.P3SOTC",
                    (sot, grp) => { sot.Group = grp; return sot; },
                    new { CompanyID = CompanyID }, splitOn: "P1TICO");
                ConcurrentQueue<GenericIntDecimal> codes = new ConcurrentQueue<GenericIntDecimal>();
                int unknownCount = -1;
                Parallel.ForEach(list.ToList(), sot =>
                {
                    var multi = connection.QueryMultiple(
                    @"SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1soci = r.N1soci AND t.n1anno = r.n1anno AND t.n1regi = r.n1regi
                            WHERE r.N1ANNO = @yea AND r.N1SOCI = @cid AND r.pngrup = @grp AND r.pncont = @cnt AND r.pnsott = @sot AND r.N1SEGN = 'D' AND t.canceled IS NULL;
                        SELECT SUM(r.N1IMEU) FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1soci = r.N1soci AND t.n1anno = r.n1anno AND t.n1regi = r.n1regi
                            WHERE r.N1ANNO = @yea AND r.N1SOCI = @cid AND r.pngrup = @grp AND r.pncont = @cnt AND r.pnsott = @sot AND r.N1SEGN = 'A' AND t.canceled IS NULL;",
                    new { cid = CompanyID, grp = sot.P1GRUP, cnt = sot.P2CONT, sot = sot.P3SOTC, yea = Year });
                    var dare = multi.Read<decimal?>().Single() ?? 0m;
                    var avere = multi.Read<decimal?>().Single() ?? 0m;
                    if (dare != avere)
                    {
                        if (sot.Group?.P1TICO == "A" || sot.Group?.P1TICO == "C")
                        {
                            if (dare > avere)
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3este ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += dare - avere;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3este ?? unknownCount--, Tag = (sot.p3este.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = dare - avere });
                            }
                            else
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3est2 ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += dare - avere;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3est2 ?? unknownCount--, Tag = (sot.p3est2.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = dare - avere });
                            }
                        }
                        else
                        {
                            if (dare > avere)
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3est2 ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += avere - dare;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3est2 ?? unknownCount--, Tag = (sot.p3est2.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = avere - dare });
                            }
                            else
                            {
                                var existing = codes.Where(w => w.IntItem == (sot.p3este ?? 99999999)).FirstOrDefault();
                                if (existing != null)
                                    existing.DecimalItem += avere - dare;
                                else
                                    codes.Enqueue(new GenericIntDecimal() { IntItem = sot.p3este ?? unknownCount--, Tag = (sot.p3este.HasValue ? null : $"{sot.P1GRUP}{sot.P2CONT}{sot.P3SOTC}"), DecimalItem = avere - dare });
                            }
                        }
                    }
                });
                StringBuilder result = new StringBuilder();
                foreach (var item in codes.OrderBy(o => o.IntItem))
                {
                    result.Append($"{(item.IntItem > 0 ? item.IntItem.ToString().PadLeft(8, '0') : $"#{item.Tag?.ToString()}")};{(item.DecimalItem > 0 ? "+" : "-")}{Math.Abs(item.DecimalItem).ToString("N2", new System.Globalization.CultureInfo("it-IT") { NumberFormat = new System.Globalization.NumberFormatInfo() { NumberGroupSeparator = string.Empty, NumberDecimalSeparator = "," } }).PadLeft(13, '0')}{Environment.NewLine}");
                }
                return result.ToString();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region Closing
        public bool PeriodClosing(string CompanyID, int Year, DateTime LimitDate)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // flag all rows
                        connection.Execute("UPDATE PN_RIGHE SET N1CHIU='C' WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1STBO='*' AND N1STMA='*';",
                            new { cid = CompanyID, yea = Year }, transaction);

                        // move period into esercizio (sum) on pdcanni
                        connection.Execute(@"UPDATE pdc
                                            SET pdc.P4DAES = ISNULL(pdc.P4DAES, 0) + ISNULL(pdc.P4DAPE, 0), pdc.P4AVES = ISNULL(pdc.P4AVES, 0) + ISNULL(pdc.P4AVPE, 0), pdc.P4DAPE = 0, pdc.P4AVPE = 0
                                            FROM PDC_ANNI AS pdc INNER JOIN
                                            (SELECT N1SOCI,N1ANNO,pngrup,pncont,pnsott FROM PN_RIGHE
                                            WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1STBO='*' AND N1STMA='*'
                                            GROUP BY N1SOCI,N1ANNO,pngrup,pncont,pnsott) AS grp
                                            ON grp.pngrup=P1GRUP AND grp.pncont=P2CONT AND grp.pnsott=P3SOTC AND grp.N1ANNO=P4ANNO AND grp.N1SOCI=P1SOCI",
                            new { cid = CompanyID, yea = Year }, transaction);

                        // update esercizio last closing
                        connection.Execute("UPDATE ESERCIZIO SET eseuch=@eseuch WHERE esesoc=@cid AND eseann=@yea",
                            new { cid = CompanyID, yea = Year, eseuch = LimitDate }, transaction);

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

        public bool YearClosing(string CompanyID, int Year, DateTime LimitDate, DateTime NewDate, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, Year)!;

                        var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                        var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                        var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                        var tabClosingRepositoty = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_CLOSINGRepository>();
                        var pdcAnniRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>();

                        LimitDate = LimitDate.Date;
                        NewDate = NewDate.Date;
                        // ECONOMICI
                        #region Registrazione chiusura conto economico (costi e ricavi)
                        #region PNTESTATA
                        var accountingID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                        var causals = tabClosingRepositoty.GetList();
                        var lossCausal = causals?.Where(w => w.cchppr == "S").First();
                        PNTESTATA head = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year,
                            N1REGI = accountingID,
                            pncaus = lossCausal?.cchchi,
                            N1DARE = LimitDate,
                            N1docn = accountingID.ToString(),
                            N1docd = LimitDate,
                            N1rifn = accountingID.ToString(),
                            N1rifd = LimitDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                        #endregion
                        int rowsCounter = 1;
                        #region Righe
                        decimal dareTotal = 0;
                        decimal avereTotal = 0;
                        foreach (var yea in pdcAnniRepository.GetListByYearTypes(CompanyID, Year, ["C", "R"]) ?? new ObservableCollection<PDCANNI>())
                        {
                            var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                            if (diff != 0)
                            {
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
                                    N1SEGN = diff > 0 ? "A" : "D",
                                    pngrup = yea.P1GRUP,
                                    pncont = yea.P2CONT,
                                    pnsott = yea.P3SOTC,
                                    N1IMEU = diff > 0 ? diff : diff * -1,
                                    N1CHIU = "A",
                                    N1TIDO = "E",
                                    N1DIVI = "EUR",
                                    N1tmpPNR = "N",
                                    N1DRri = head.N1docd
                                };
                                connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                if (newRow.N1SEGN == "D")
                                    dareTotal += newRow.N1IMEU ?? 0;
                                else
                                    avereTotal += newRow.N1IMEU ?? 0;
                            }
                        }
                        // totali conto profitti e perdite
                        var newDareRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = "A",
                            pngrup = lossCausal?.cchgrc,
                            pncont = lossCausal?.cchctc,
                            pnsott = lossCausal?.cchstc,
                            N1IMEU = dareTotal,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newDareRow, transaction);
                        var newAvereRow = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowsCounter++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = "D",
                            pngrup = lossCausal?.cchgrc,
                            pncont = lossCausal?.cchctc,
                            pnsott = lossCausal?.cchstc,
                            N1IMEU = avereTotal,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereRow, transaction);

                        #endregion
                        #endregion

                        #region Registrazione utile/perdita
                        var diffLG = dareTotal - avereTotal;
                        #region PNTESTATA
                        var accountingLGID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                        var headLG = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year,
                            N1REGI = accountingLGID,
                            pncaus = diffLG >= 0 ? lossCausal?.cchues : lossCausal?.cchpes,
                            N1DARE = LimitDate,
                            N1docn = accountingLGID.ToString(),
                            N1docd = LimitDate,
                            N1rifn = accountingLGID.ToString(),
                            N1rifd = LimitDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headLG, transaction);
                        #endregion
                        int rowsLGCounter = 1;
                        #region Righe
                        PNRIGHE newDiffRow = new PNRIGHE()
                        {
                            N1SOCI = headLG.N1SOCI,
                            N1ANNO = headLG.N1ANNO,
                            N1REGI = headLG.N1REGI,
                            N1RIGA = rowsLGCounter++,
                            N1DOCU = headLG.N1docn,
                            N1DADO = headLG.N1docd,
                            N1RIFE = headLG.N1rifn,
                            N1DARI = headLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "A" : "D",
                            pngrup = diffLG >= 0 ? lossCausal?.cchgru : lossCausal?.cchgrp,
                            pncont = diffLG >= 0 ? lossCausal?.cchctu : lossCausal?.cchctp,
                            pnsott = diffLG >= 0 ? lossCausal?.cchstu : lossCausal?.cchstp,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newDiffRow, transaction);
                        PNRIGHE newCPRow = new PNRIGHE()
                        {
                            N1SOCI = headLG.N1SOCI,
                            N1ANNO = headLG.N1ANNO,
                            N1REGI = headLG.N1REGI,
                            N1RIGA = rowsLGCounter++,
                            N1DOCU = headLG.N1docn,
                            N1DADO = headLG.N1docd,
                            N1RIFE = headLG.N1rifn,
                            N1DARI = headLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "D" : "A",
                            pngrup = lossCausal?.cchgrc,
                            pncont = lossCausal?.cchctc,
                            pnsott = lossCausal?.cchstc,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newCPRow, transaction);
                        #endregion
                        #endregion

                        #region Registrazione chiusura risultato esercizio
                        var esercizioCausal = (causals ?? new ObservableCollection<TAB_ACC_CLOSING>()).Where(w => w.cchppr == "N").First();

                        #region PNTESTATA
                        var accountingRisID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                        var headRis = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year,
                            N1REGI = accountingRisID,
                            pncaus = esercizioCausal.cchchi,
                            N1DARE = LimitDate,
                            N1docn = accountingRisID.ToString(),
                            N1docd = LimitDate,
                            N1rifn = accountingRisID.ToString(),
                            N1rifd = LimitDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headRis, transaction);
                        #endregion
                        int rowsRisCounter = 1;
                        #region Righe
                        PNRIGHE newRis1Row = new PNRIGHE()
                        {
                            N1SOCI = headRis.N1SOCI,
                            N1ANNO = headRis.N1ANNO,
                            N1REGI = headRis.N1REGI,
                            N1RIGA = rowsRisCounter++,
                            N1DOCU = headRis.N1docn,
                            N1DADO = headRis.N1docd,
                            N1RIFE = headRis.N1rifn,
                            N1DARI = headRis.N1rifd,
                            N1SEGN = newDiffRow.N1SEGN == "D" ? "A" : "D",
                            pngrup = newDiffRow.pngrup,
                            pncont = newDiffRow.pncont,
                            pnsott = newDiffRow.pnsott,
                            N1IMEU = newDiffRow.N1IMEU,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headRis.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRis1Row, transaction);
                        PNRIGHE newRis2Row = new PNRIGHE()
                        {
                            N1SOCI = headRis.N1SOCI,
                            N1ANNO = headRis.N1ANNO,
                            N1REGI = headRis.N1REGI,
                            N1RIGA = rowsRisCounter++,
                            N1DOCU = headRis.N1docn,
                            N1DADO = headRis.N1docd,
                            N1RIFE = headRis.N1rifn,
                            N1DARI = headRis.N1rifd,
                            N1SEGN = newRis1Row.N1SEGN == "D" ? "A" : "D",
                            pngrup = esercizioCausal.cchgrc,
                            pncont = esercizioCausal.cchctc,
                            pnsott = esercizioCausal.cchstc,
                            N1IMEU = newRis1Row.N1IMEU,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headRis.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRis2Row, transaction);
                        #endregion
                        #endregion

                        var azienda = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)!;

                        var apYears = pdcAnniRepository.GetListByYearTypes(CompanyID, Year, ["A", "P"]);

                        if (azienda.aziconchiuclifor ?? false)
                        {
                            // PATRIMONIALI
                            #region Registrazione chiusura conti patrimoniali
                            #region PNTESTATA
                            var accountingPatID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA headPat = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingPatID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headPat, transaction);
                            #endregion
                            int rowsPatCounter = 1;
                            #region Righe
                            decimal darePatTotal = 0;
                            decimal averePatTotal = 0;
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                if (yea.Account?.p2flcf != "C" && yea.Account?.p2flcf != "F")
                                {
                                    var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                    if (diff != 0)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = headPat.N1SOCI,
                                            N1ANNO = headPat.N1ANNO,
                                            N1REGI = headPat.N1REGI,
                                            N1RIGA = rowsPatCounter++,
                                            N1DOCU = headPat.N1docn,
                                            N1DADO = headPat.N1docd,
                                            N1RIFE = headPat.N1rifn,
                                            N1DARI = headPat.N1rifd,
                                            N1SEGN = diff > 0 ? "A" : "D",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = diff > 0 ? diff : diff * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            N1DRri = headPat.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            darePatTotal += newRow.N1IMEU ?? 0;
                                        else
                                            averePatTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                            }

                            var newDarePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = darePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDarePatRow, transaction);
                            var newAverePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = averePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAverePatRow, transaction);

                            #endregion
                            #endregion

                            #region Registrazione chiusura Clienti/Fornitori
                            #region PNTESTATA
                            var accountingCusID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA accountingCus = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingCusID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, accountingCus, transaction);

                            var accountingSupID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA accountingSup = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingSupID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, accountingSup, transaction);
                            #endregion

                            int rowsCusCounter = 1;
                            int rowsSupCounter = 1;

                            #region Righe
                            decimal dareCusTotal = 0;
                            decimal avereCusTotal = 0;
                            decimal dareSupTotal = 0;
                            decimal avereSupTotal = 0;

                            foreach (var yea in (apYears ?? new ObservableCollection<PDCANNI>()).Where(o => o.Account?.p2flcf == "C" || o.Account?.p2flcf == "F"))
                            {
                                if (yea.Account?.p2flcf == "C")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = accountingCus.N1SOCI,
                                            N1ANNO = accountingCus.N1ANNO,
                                            N1REGI = accountingCus.N1REGI,
                                            N1RIGA = rowsCusCounter++,
                                            N1DOCU = accountingCus.N1docn,
                                            N1DADO = accountingCus.N1docd,
                                            N1RIFE = accountingCus.N1rifn,
                                            N1DARI = accountingCus.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "A" : "D",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = accountingCus.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareCusTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereCusTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                                if (yea.Account?.p2flcf == "F")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = accountingSup.N1SOCI,
                                            N1ANNO = accountingSup.N1ANNO,
                                            N1REGI = accountingSup.N1REGI,
                                            N1RIGA = rowsSupCounter++,
                                            N1DOCU = accountingSup.N1docn,
                                            N1DADO = accountingSup.N1docd,
                                            N1RIFE = accountingSup.N1rifn,
                                            N1DARI = accountingSup.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "A" : "D",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = accountingSup.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareSupTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereSupTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                            }

                            var newDareCusRow = new PNRIGHE()
                            {
                                N1SOCI = accountingCus.N1SOCI,
                                N1ANNO = accountingCus.N1ANNO,
                                N1REGI = accountingCus.N1REGI,
                                N1RIGA = rowsCusCounter++,
                                N1DOCU = accountingCus.N1docn,
                                N1DADO = accountingCus.N1docd,
                                N1RIFE = accountingCus.N1rifn,
                                N1DARI = accountingCus.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = avereCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingCus.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareCusRow, transaction);
                            var newAvereCusRow = new PNRIGHE()
                            {
                                N1SOCI = accountingCus.N1SOCI,
                                N1ANNO = accountingCus.N1ANNO,
                                N1REGI = accountingCus.N1REGI,
                                N1RIGA = rowsCusCounter++,
                                N1DOCU = accountingCus.N1docn,
                                N1DADO = accountingCus.N1docd,
                                N1RIFE = accountingCus.N1rifn,
                                N1DARI = accountingCus.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = dareCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingCus.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereCusRow, transaction);

                            var newDareSupRow = new PNRIGHE()
                            {
                                N1SOCI = accountingSup.N1SOCI,
                                N1ANNO = accountingSup.N1ANNO,
                                N1REGI = accountingSup.N1REGI,
                                N1RIGA = rowsSupCounter++,
                                N1DOCU = accountingSup.N1docn,
                                N1DADO = accountingSup.N1docd,
                                N1RIFE = accountingSup.N1rifn,
                                N1DARI = accountingSup.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = avereSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingSup.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareSupRow, transaction);
                            var newAvereSupRow = new PNRIGHE()
                            {
                                N1SOCI = accountingSup.N1SOCI,
                                N1ANNO = accountingSup.N1ANNO,
                                N1REGI = accountingSup.N1REGI,
                                N1RIGA = rowsSupCounter++,
                                N1DOCU = accountingSup.N1docn,
                                N1DADO = accountingSup.N1docd,
                                N1RIFE = accountingSup.N1rifn,
                                N1DARI = accountingSup.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = dareSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = accountingSup.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereSupRow, transaction);
                            #endregion
                            #endregion

                            #region Registrazione riapertura conti patrimoniali
                            #region PNTESTATA
                            var accountingOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA headOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headOpen, transaction);
                            #endregion
                            int rowsOpenCounter = 1;
                            #region Righe
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                if (yea.Account?.p2flcf != "C" && yea.Account?.p2flcf != "F")
                                {
                                    var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                    if (diff != 0)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = headOpen.N1SOCI,
                                            N1ANNO = headOpen.N1ANNO,
                                            N1REGI = headOpen.N1REGI,
                                            N1RIGA = rowsOpenCounter++,
                                            N1DOCU = headOpen.N1docn,
                                            N1DADO = headOpen.N1docd,
                                            N1RIFE = headOpen.N1rifn,
                                            N1DARI = headOpen.N1rifd,
                                            N1SEGN = diff > 0 ? "D" : "A",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = diff > 0 ? diff : diff * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            N1DRri = headOpen.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                    }
                                }
                            }
                            var newDareOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = darePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareOpenRow, transaction);
                            var newAvereOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = averePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereOpenRow, transaction);

                            #endregion
                            #endregion

                            #region Registrazione riapertura Clienti/Fornitori
                            #region PNTESTATA
                            var accountingCusOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA cusOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingCusOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, cusOpen, transaction);

                            var accountingSupOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA supOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingSupOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, supOpen, transaction);
                            #endregion
                            int rowsCusOpenCounter = 1;
                            int rowsSupOpenCounter = 1;
                            #region Righe
                            decimal dareOpenCusTotal = 0;
                            decimal avereOpenCusTotal = 0;
                            decimal dareOpenSupTotal = 0;
                            decimal avereOpenSupTotal = 0;

                            foreach (var yea in (apYears ?? new ObservableCollection<PDCANNI>()).Where(o => o.Account?.p2flcf == "C" || o.Account?.p2flcf == "F"))
                            {
                                if (yea.Account?.p2flcf == "C")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = cusOpen.N1SOCI,
                                            N1ANNO = cusOpen.N1ANNO,
                                            N1REGI = cusOpen.N1REGI,
                                            N1RIGA = rowsCusOpenCounter++,
                                            N1DOCU = cusOpen.N1docn,
                                            N1DADO = cusOpen.N1docd,
                                            N1RIFE = cusOpen.N1rifn,
                                            N1DARI = cusOpen.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "D" : "A",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = cusOpen.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareOpenCusTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereOpenCusTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                                if (yea.Account?.p2flcf == "F")
                                {
                                    var list = pnRigheRepository.GetWithSaldo(CompanyID, yea.Group?.P1GRUP ?? string.Empty, yea.Account?.P2CONT ?? string.Empty, esercizio.Start, esercizio.End);
                                    foreach (var lst in list)
                                    {
                                        PNRIGHE newRow = new PNRIGHE()
                                        {
                                            N1SOCI = supOpen.N1SOCI,
                                            N1ANNO = supOpen.N1ANNO,
                                            N1REGI = supOpen.N1REGI,
                                            N1RIGA = rowsSupOpenCounter++,
                                            N1DOCU = supOpen.N1docn,
                                            N1DADO = supOpen.N1docd,
                                            N1RIFE = supOpen.N1rifn,
                                            N1DARI = supOpen.N1rifd,
                                            N1SEGN = (lst.Item2 - lst.Item3) > 0 ? "D" : "A",
                                            pngrup = yea.P1GRUP,
                                            pncont = yea.P2CONT,
                                            pnsott = yea.P3SOTC,
                                            N1IMEU = (lst.Item2 - lst.Item3) > 0 ? (lst.Item2 - lst.Item3) : (lst.Item2 - lst.Item3) * -1,
                                            N1CHIU = "A",
                                            N1TIDO = "E",
                                            N1DIVI = "EUR",
                                            N1tmpPNR = "N",
                                            n1clie = lst.Item1,
                                            N1DRri = supOpen.N1docd
                                        };
                                        connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                        if (newRow.N1SEGN == "D")
                                            dareOpenSupTotal += newRow.N1IMEU ?? 0;
                                        else
                                            avereOpenSupTotal += newRow.N1IMEU ?? 0;
                                    }
                                }
                            }
                            var newDareCusOpenRow = new PNRIGHE()
                            {
                                N1SOCI = cusOpen.N1SOCI,
                                N1ANNO = cusOpen.N1ANNO,
                                N1REGI = cusOpen.N1REGI,
                                N1RIGA = rowsCusOpenCounter++,
                                N1DOCU = cusOpen.N1docn,
                                N1DADO = cusOpen.N1docd,
                                N1RIFE = cusOpen.N1rifn,
                                N1DARI = cusOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = dareOpenCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = cusOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareCusOpenRow, transaction);
                            var newAvereCusOpenRow = new PNRIGHE()
                            {
                                N1SOCI = cusOpen.N1SOCI,
                                N1ANNO = cusOpen.N1ANNO,
                                N1REGI = cusOpen.N1REGI,
                                N1RIGA = rowsCusOpenCounter++,
                                N1DOCU = cusOpen.N1docn,
                                N1DADO = cusOpen.N1docd,
                                N1RIFE = cusOpen.N1rifn,
                                N1DARI = cusOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = avereOpenCusTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = cusOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereCusOpenRow, transaction);

                            var newDareSupOpenRow = new PNRIGHE()
                            {
                                N1SOCI = supOpen.N1SOCI,
                                N1ANNO = supOpen.N1ANNO,
                                N1REGI = supOpen.N1REGI,
                                N1RIGA = rowsSupOpenCounter++,
                                N1DOCU = supOpen.N1docn,
                                N1DADO = supOpen.N1docd,
                                N1RIFE = supOpen.N1rifn,
                                N1DARI = supOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = dareOpenSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = supOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareSupOpenRow, transaction);
                            var newAvereSupOpenRow = new PNRIGHE()
                            {
                                N1SOCI = supOpen.N1SOCI,
                                N1ANNO = supOpen.N1ANNO,
                                N1REGI = supOpen.N1REGI,
                                N1RIGA = rowsSupOpenCounter++,
                                N1DOCU = supOpen.N1docn,
                                N1DADO = supOpen.N1docd,
                                N1RIFE = supOpen.N1rifn,
                                N1DARI = supOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = avereOpenSupTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = supOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereSupOpenRow, transaction);
                            #endregion
                            #endregion
                        }
                        else
                        {
                            // PATRIMONIALI
                            #region Registrazione chiusura conti patrimoniali
                            #region PNTESTATA
                            var accountingPatID = numRegRepository.GetNumber(CompanyID, Year, Constants.PN, true);
                            PNTESTATA headPat = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year,
                                N1REGI = accountingPatID,
                                pncaus = esercizioCausal.cchchi,
                                N1DARE = LimitDate,
                                N1docn = accountingPatID.ToString(),
                                N1docd = LimitDate,
                                N1rifn = accountingPatID.ToString(),
                                N1rifd = LimitDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headPat, transaction);
                            #endregion
                            int rowsPatCounter = 1;
                            #region Righe
                            decimal darePatTotal = 0;
                            decimal averePatTotal = 0;
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                if (diff != 0)
                                {
                                    PNRIGHE newRow = new PNRIGHE()
                                    {
                                        N1SOCI = headPat.N1SOCI,
                                        N1ANNO = headPat.N1ANNO,
                                        N1REGI = headPat.N1REGI,
                                        N1RIGA = rowsPatCounter++,
                                        N1DOCU = headPat.N1docn,
                                        N1DADO = headPat.N1docd,
                                        N1RIFE = headPat.N1rifn,
                                        N1DARI = headPat.N1rifd,
                                        N1SEGN = diff > 0 ? "A" : "D",
                                        pngrup = yea.P1GRUP,
                                        pncont = yea.P2CONT,
                                        pnsott = yea.P3SOTC,
                                        N1IMEU = diff > 0 ? diff : diff * -1,
                                        N1CHIU = "A",
                                        N1TIDO = "E",
                                        N1DIVI = "EUR",
                                        N1tmpPNR = "N",
                                        N1DRri = headPat.N1docd
                                    };
                                    connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                    if (newRow.N1SEGN == "D")
                                        darePatTotal += newRow.N1IMEU ?? 0;
                                    else
                                        averePatTotal += newRow.N1IMEU ?? 0;
                                }
                            }
                            var newDarePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = dareTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDarePatRow, transaction);
                            var newAverePatRow = new PNRIGHE()
                            {
                                N1SOCI = headPat.N1SOCI,
                                N1ANNO = headPat.N1ANNO,
                                N1REGI = headPat.N1REGI,
                                N1RIGA = rowsPatCounter++,
                                N1DOCU = headPat.N1docn,
                                N1DADO = headPat.N1docd,
                                N1RIFE = headPat.N1rifn,
                                N1DARI = headPat.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrc,
                                pncont = esercizioCausal.cchctc,
                                pnsott = esercizioCausal.cchstc,
                                N1IMEU = avereTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headPat.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAverePatRow, transaction);

                            #endregion
                            #endregion

                            // RIAPERTURA
                            #region Registrazione riapertura
                            #region PNTESTATA
                            var accountingOpenID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                            PNTESTATA headOpen = new PNTESTATA()
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = Year + 1,
                                N1REGI = accountingOpenID,
                                pncaus = esercizioCausal.cchria,
                                N1DARE = NewDate,
                                N1docn = accountingOpenID.ToString(),
                                N1docd = NewDate,
                                N1rifn = accountingOpenID.ToString(),
                                N1rifd = NewDate,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = UserID
                            };
                            connection.Execute(pnTestataRepository.INSERT_QUERY, headOpen, transaction);
                            #endregion
                            int rowsOpenCounter = 1;
                            #region Righe
                            foreach (var yea in apYears ?? new ObservableCollection<PDCANNI>())
                            {
                                var diff = (yea.P4DAES ?? 0) - (yea.P4AVES ?? 0);
                                if (diff != 0)
                                {
                                    PNRIGHE newRow = new PNRIGHE()
                                    {
                                        N1SOCI = headOpen.N1SOCI,
                                        N1ANNO = headOpen.N1ANNO,
                                        N1REGI = headOpen.N1REGI,
                                        N1RIGA = rowsOpenCounter++,
                                        N1DOCU = headOpen.N1docn,
                                        N1DADO = headOpen.N1docd,
                                        N1RIFE = headOpen.N1rifn,
                                        N1DARI = headOpen.N1rifd,
                                        N1SEGN = diff > 0 ? "D" : "A",
                                        pngrup = yea.P1GRUP,
                                        pncont = yea.P2CONT,
                                        pnsott = yea.P3SOTC,
                                        N1IMEU = diff > 0 ? diff : diff * -1,
                                        N1CHIU = "A",
                                        N1TIDO = "E",
                                        N1DIVI = "EUR",
                                        N1tmpPNR = "N",
                                        N1DRri = headOpen.N1docd
                                    };
                                    connection.Execute(pnRigheRepository.INSERT_QUERY, newRow, transaction);
                                }
                            }
                            var newDareOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "D",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = darePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newDareOpenRow, transaction);
                            var newAvereOpenRow = new PNRIGHE()
                            {
                                N1SOCI = headOpen.N1SOCI,
                                N1ANNO = headOpen.N1ANNO,
                                N1REGI = headOpen.N1REGI,
                                N1RIGA = rowsOpenCounter++,
                                N1DOCU = headOpen.N1docn,
                                N1DADO = headOpen.N1docd,
                                N1RIFE = headOpen.N1rifn,
                                N1DARI = headOpen.N1rifd,
                                N1SEGN = "A",
                                pngrup = esercizioCausal.cchgrr,
                                pncont = esercizioCausal.cchctr,
                                pnsott = esercizioCausal.cchstr,
                                N1IMEU = averePatTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1tmpPNR = "N",
                                N1DRri = headOpen.N1docd
                            };
                            connection.Execute(pnRigheRepository.INSERT_QUERY, newAvereOpenRow, transaction);

                            #endregion
                            #endregion
                        }

                        #region Registrazione apertura utile/perdita
                        #region PNTESTATA
                        var accountingOpenLGID = numRegRepository.GetNumber(CompanyID, Year + 1, Constants.PN, true);
                        var headOpenLG = new PNTESTATA()
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = Year + 1,
                            N1REGI = accountingOpenLGID,
                            pncaus = esercizioCausal.cchria,
                            N1DARE = NewDate,
                            N1docn = accountingOpenLGID.ToString(),
                            N1docd = NewDate,
                            N1rifn = accountingOpenLGID.ToString(),
                            N1rifd = NewDate,
                            pnvcod = "UIC",
                            pnvdiv = "EUR",
                            N1TmpPN = "N",
                            n1mrii = 0,
                            addedUserID = UserID
                        };
                        connection.Execute(pnTestataRepository.INSERT_QUERY, headOpenLG, transaction);
                        #endregion
                        int rowsOpenLGCounter = 1;
                        #region Righe
                        PNRIGHE newDiffOpenRow = new PNRIGHE()
                        {
                            N1SOCI = headOpenLG.N1SOCI,
                            N1ANNO = headOpenLG.N1ANNO,
                            N1REGI = headOpenLG.N1REGI,
                            N1RIGA = rowsOpenLGCounter++,
                            N1DOCU = headOpenLG.N1docn,
                            N1DADO = headOpenLG.N1docd,
                            N1RIFE = headOpenLG.N1rifn,
                            N1DARI = headOpenLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "A" : "D",
                            pngrup = diffLG >= 0 ? esercizioCausal.cchgrp : esercizioCausal.cchgru,
                            pncont = diffLG >= 0 ? esercizioCausal.cchctp : esercizioCausal.cchctu,
                            pnsott = diffLG >= 0 ? esercizioCausal.cchstp : esercizioCausal.cchstu,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headOpenLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newDiffOpenRow, transaction);
                        PNRIGHE newCPOpenRow = new PNRIGHE()
                        {
                            N1SOCI = headOpenLG.N1SOCI,
                            N1ANNO = headOpenLG.N1ANNO,
                            N1REGI = headOpenLG.N1REGI,
                            N1RIGA = rowsOpenLGCounter++,
                            N1DOCU = headOpenLG.N1docn,
                            N1DADO = headOpenLG.N1docd,
                            N1RIFE = headOpenLG.N1rifn,
                            N1DARI = headOpenLG.N1rifd,
                            N1SEGN = diffLG >= 0 ? "D" : "A",
                            pngrup = esercizioCausal.cchgrr,
                            pncont = esercizioCausal.cchctr,
                            pnsott = esercizioCausal.cchstr,
                            N1IMEU = diffLG >= 0 ? diffLG : diffLG * -1,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = headOpenLG.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, newCPOpenRow, transaction);
                        #endregion
                        #endregion

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

        public decimal ComputeLossProfit(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();


                var multi = connection.QueryMultiple(
                    @"SELECT (SUM(ISNULL(a.P4AVES, 0)) - SUM(ISNULL(a.P4DAES, 0))) AS rbalance FROM PDC_ANNI AS a
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=a.P1GRUP
                            WHERE g.P1TICO = 'R' AND a.P4ANNO=@yea AND a.P1SOCI=@cid;
                            SELECT (SUM(ISNULL(a.P4DAES, 0)) - SUM(ISNULL(a.P4AVES, 0))) AS cbalance FROM PDC_ANNI AS a
                            INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP=a.P1GRUP
                            WHERE g.P1TICO = 'C' AND a.P4ANNO=@yea AND a.P1SOCI=@cid;",
                    new { cid = CompanyID, yea = Year });

                return multi.Read<decimal>().Single() - multi.Read<decimal>().Single();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return 0;
            }
        }
        #endregion

        #region Self invoicing
        public string? GenerateSelfInvoice(PNTESTATA Head, ESERCIZIO AccountingYear, ABE Customer, CAUFAT00F Causal, tab_articolo Product, DateTime SelectedDate, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    int sequence = 1;
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var newInvoiceID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Head.N1SOCI, AccountingYear.eseann, Constants.INVOICE_TEMP, true);
                    if (newInvoiceID <= 0)
                    {
                        ErrorHandler.Show("Impossibile recuperare un nuovo numero di fattura provvisiorio");
                        return null;
                    }
                    var customerData = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Head.N1SOCI, Customer.abecod);
                    var clientData = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(Customer.abecod);
                    if (customerData == null || clientData == null)
                    {
                        ErrorHandler.Show("Impossibile proseguire per la mancanza dei dati amministrativi del cliente selezionato");
                        return null;
                    }
                    var payment = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetFull(customerData?.pclcod ?? string.Empty);
                    if (payment == null)
                    {
                        ErrorHandler.Show("Impossibile recuperare il tipo pagamento dal cliente selezionato");
                        return null;
                    }

                    var incasso = VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().Get(payment.pcltip ?? string.Empty);
                    if (incasso == null)
                    {
                        ErrorHandler.Show("Impossibile recuperare il tipo incasso ");
                        return null;
                    }

                    var fullCustomer = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(Customer.abecod);
                    if (fullCustomer == null)
                    {
                        ErrorHandler.Show("Impossibile recuperare il cliente");
                        return null;
                    }

                    var fullProduct = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(Product.ID);
                    if (fullProduct == null)
                    {
                        ErrorHandler.Show("Impossibile recuperare l'articolo");
                        return null;
                    }

                    Customer = fullCustomer;
                    Product = fullProduct;

                    var sourceInvoice = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetByAccountingRegistration(Head.N1CLFO ?? 0, Head.N1rifn ?? string.Empty);

                    // generate invoice head
                    var newHead = new FATTT00F()
                    {
                        ftsoci = Head.N1SOCI,
                        FTANNO = AccountingYear.eseann,
                        FTNUOR = newInvoiceID,
                        FTTIPO = Causal.fatdoctip,
                        FTDAOR = SelectedDate,
                        FTCAUS = Causal.fatcod,
                        FTCODC = Customer.abecod,
                        FTPAGA = customerData?.pclcod,
                        ftdafa = SelectedDate,
                        FTABIB = payment.Incasso?.icssup == "R" ? customerData?.CLABI : customerData?.banabi,
                        FTCABB = payment.Incasso?.icssup == "R" ? customerData?.CLCAB : customerData?.bancab,
                        FTBCON = payment.Incasso?.icssup == "R" ? null : customerData?.bancoc,
                        FTCONS = customerData?.concod,
                        FTSPED = customerData?.specod,
                        FTCORR = customerData?.vetcod,
                        FTIMBL = customerData?.CLIMBA,
                        FTCIDI = Head.pnvcod,
                        ftciva = Head.pnvdiv,
                        addedUserID = UserID,
                        FTDE25 = clientData?.CLREAC,
                        FTSCCL = customerData?.CLSCON,
                        FTAREA = customerData?.arecod,
                        FTRIVE = customerData?.rivcod,
                        FTCONZ = customerData?.CLCCON,
                        FTFILI = customerData?.filcod,
                        FTZONA = customerData?.CLZONE,
                        FTSETM = customerData?.CLSETM,
                        FTREGI = customerData?.CLREGI,
                        fttdoc = Causal.fattido,
                        ftling = Customer.isocod,
                        FTCLSO = Head.N1SOCI,
                        fattmpag = incasso.icsfepacod,
                        FTFLA1 = string.Empty,
                        FTFLA2 = string.Empty,
                        FTFLA3 = string.Empty,
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().INSERT_QUERY, newHead, transaction);

                    // generate rows and update origins
                    foreach (var row in VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetList(Head.N1SOCI, Head.N1ANNO, Head.N1REGI) ?? new ObservableCollection<PNIVA>())
                    {
                        var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.N4ASSF ?? string.Empty, row.n4assa ?? string.Empty);
                        var newRow = new FATTD00F()
                        {
                            ftsoci = newHead.ftsoci,
                            FTANNO = newHead.FTANNO,
                            FTNUOR = newHead.FTNUOR,
                            FDRIGA = sequence++,
                            FDCODA = Product.ID,
                            fddaor = SelectedDate,
                            fdcodc = Customer.abecod,
                            FDQTAV = 1,
                            FDTQTA = "V",
                            FDPREZ = row.N4IMEU,
                            FDTPRE = "U",
                            FDASSF = row.N4ASSF,
                            FDALIV = row.n4assa,
                            FDGRUP = Causal.FATGRUp,
                            FDCONT = Causal.fatcont,
                            FDSCTO = Causal.fatsott,
                            FDUNIM = Product.UnitaID,
                            FDNOTE = $"Autofattura relativa a fatt. {Head.N1rifn} del {(Head.N1rifd ?? DateTime.Now).ToString("dd/MM/yyyy")} prot. {Head.N1docn}",
                            FDSHOW = true,
                            fdtiva = rate?.assnatufe,
                            fdartdise = Product.artdise,
                            FDTVEn = Product.artven,
                        };
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().INSERT_QUERY, newRow, transaction);

                        var supplier = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(row.N4SOTT ?? 0);
                        string? nota = $"{supplier?.FullDescriptionSearchable} - {row.N4RIFE?.TrimEnd()} - {row.N4DARI?.ToString("dd/MM/yyyy")}";

                        // NOTE RIGA
                        connection.Execute($"INSERT INTO FATTN001 (ftsoci,FNANNO,FNNUOR,FNRIGA,FNDES1, FNFLGA, FNDESC) VALUES(@ftsoci, @FNANNO, @FNNUOR, @FNRIGA, @FNDES1, @FNFLGA, @FNDESC)", new
                        {
                            ftsoci = newRow.ftsoci,
                            FNANNO = newRow.FTANNO,
                            FNNUOR = newRow.FTNUOR,
                            FNRIGA = newRow.FDRIGA,
                            FNDES1 = string.Empty,
                            FNFLGA = "S",
                            FNDESC = nota
                        }, transaction);
                    }

                    // generate self invoice data
                    var self = new FATTAUT()
                    {
                        FTAUSC = newHead.ftsoci,
                        FTAUAN = newHead.FTANNO,
                        FTAUNUM = newHead.FTNUOR,
                        FTAUCOF = Head.N1CLFO ?? 0,
                        FTAUDATRIC = sourceInvoice != null ? sourceInvoice.fattdataric : (Head.N1docd ?? DateTime.Now),
                        FTAUINDSDI = sourceInvoice?.fattidsdi,
                        FTAUDATFAT = sourceInvoice != null ? sourceInvoice.fattdata : Head.N1rifd,
                        FTAUNUMFAT = sourceInvoice != null ? sourceInvoice.fattnum : Head.N1rifn,
                        FTAPNAN = Head.N1ANNO,
                        FTAPNRE = Head.N1REGI
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IFATTAUTRepository>().INSERT_QUERY, self, transaction);

                    // flag accounting registration
                    Head.N1AUAN = AccountingYear.eseann;
                    Head.N1AUNU = newInvoiceID;
                    Head.N1AUGE = now;
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().UPDATE_QUERY, Head, transaction);

                    transaction.Commit();
                    return $"{Head.N1AUAN}/{Head.N1AUNU} del {SelectedDate.ToString("dd/MM/yyyy")}";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message);
                    return null;
                }

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        #endregion

        #region Printing
        public AccountingRecordReport? PrintAccountingRecord(PNTESTATA Head)
        {
            try
            {
                var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(Head.N1SOCI)!;

                Head.Rows = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().GetListPrint(Head.N1SOCI, Head.N1ANNO, Head.N1REGI);
                if (Head.N1FLCF == "C")
                {
                    Head.PrintDetailRows = new ObservableCollection<PrintRegViewModel>();
                    foreach (var item in VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().GetListByReg(Head.N1SOCI, Head.N1ANNO, Head.N1REGI) ?? new ObservableCollection<PNCLIENTI>())
                    {
                        Head.PrintDetailRows.Add(new PrintRegViewModel()
                        {
                            EntityTypeDescription = "Cliente",
                            Customer = item.CustomerDescription,
                            Expire = (item.N2SCAD ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            ReferenceID = item.N2RIFE,
                            ReferenceDate = (item.N2DARI ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            Amount = (item.N2IMEU ?? 0).ToString("N2"),
                            Sign = item.N2SEGN
                        });
                    }
                }
                else if (Head.N1FLCF == "F")
                {
                    Head.PrintDetailRows = new ObservableCollection<PrintRegViewModel>();
                    foreach (var item in VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().GetListByReg(Head.N1SOCI, Head.N1ANNO, Head.N1REGI) ?? new ObservableCollection<PNFORNITORI>())
                    {
                        Head.PrintDetailRows.Add(new PrintRegViewModel()
                        {
                            EntityTypeDescription = "Fornitore",
                            Customer = item.SupplierDescription,
                            Expire = (item.N3SCAD ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            ReferenceID = item.N3RIFE,
                            ReferenceDate = (item.N3DARI ?? DateTime.MinValue).ToString("dd/MM/yyyy"),
                            Amount = (item.N3IMEU ?? 0).ToString("N2"),
                            Sign = item.N3SEGN
                        });
                    }
                }

                Head.VATRows = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetListFull(Head.N1SOCI, Head.N1ANNO, Head.N1REGI);

                return new AccountingRecordReport()
                {
                    Head = Head,
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Head.N1SOCI),
                    LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png")
                };
            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return null;
            }
        }

        #region General journal
        // page break vars
        private static decimal rowSpace = .4m;
        private static decimal rowFullSpace = .6m;
        private static decimal breakSpace = .2m;
        private static decimal minSpaceToAddRow = .4m;
        private static decimal minSpaceToAddFullRow = .6m;
        private static decimal minSpaceToAddDayBreak = .2m;
        private static decimal maxSpaceAvailable = 25m; // is the space available for each page in cm
        public GeneralJournalReport? PrintGeneralJournal(string CompanyID, int AccountingYear, DateTime PrintUntil)
        {
            try
            {
                var esercizio = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Get(CompanyID, AccountingYear)!;

                using var connection = GetOpenConnection();

                var rows = connection.Query<GeneralJournalReport.RowInfo>(
                    @"SELECT (@start + ROW_NUMBER() over (order by (select NULL))) AS RowNumber, t.pncaus + ' ' + c.caudes AS CausalFullDescription, t.N1DARE AS RegistrationDate, t.N1REGI AS RegistrationNumber, r.N1RIGA AS RegistrationRow, TRIM(t.n1docn) AS DocumentNumber, t.n1docd AS DocumentDate, r.pngrup AS GroupID, r.pncont AS AccountID, r.pnsott AS SubaccountID, TRIM(r.N1DESC) AS Note, r.N1IMEU AS Amount, r.N1SEGN AS Sign, TRIM(p.P3DES1) AS PDCDescription, TRIM(abe.abers1) AS EntityDescription FROM PN_RIGHE AS r
                            INNER JOIN PN_TESTATA AS t ON t.N1SOCI=r.N1SOCI AND t.N1ANNO=r.N1ANNO AND t.N1REGI=r.N1REGI
                            INNER JOIN CAUCONT AS c ON c.caucod=t.pncaus
                            INNER JOIN PDC_SOTTOCONTI AS p ON p.P1GRUP=r.pngrup AND p.P2CONT=r.pncont AND p.P3SOTC=r.pnsott
                            LEFT JOIN ANAG_BASE AS abe ON abe.abecod = r.N1CLIE
                            WHERE r.N1SOCI=@cid AND r.N1ANNO=@yea AND t.N1DARE<=@until AND (r.N1STBO IS NULL OR r.N1STBO = '')
                            ORDER BY t.N1DARE, t.N1REGI, r.N1RIGA",
                    new { cid = CompanyID, yea = AccountingYear, until = PrintUntil, start = esercizio.eseulr ?? 0 });

                var displayRows = new List<GeneralJournalReport.RowInfo>();

                #region Vars
                decimal dareTop = 0;
                decimal avereTop = 0;
                decimal spaceUsed = 0;
                DateTime? lastDate = null;
                decimal avereDay = 0;
                decimal dareDay = 0;
                dareTop = esercizio.esedar ?? 0;
                avereTop = esercizio.eseave ?? 0;
                bool first = true;
                #endregion

                if (rows != null && rows.Count() > 0)
                {
                    foreach (var row in rows)
                    {
                        if (first)
                        {
                            displayRows.Add(new GeneralJournalReport.RowInfo()
                            {
                                IsPageTop = true,
                                AvereTop = avereTop,
                                DareTop = dareTop
                            });
                            spaceUsed += breakSpace;
                            first = false;
                        }

                        if (lastDate != row.RegistrationDate || !lastDate.HasValue)
                        {
                            if (lastDate.HasValue)
                            {
                                if (maxSpaceAvailable - spaceUsed >= minSpaceToAddDayBreak + breakSpace)
                                {
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsDayTotal = true,
                                        DayBreakText = $"Progressivi del giorno {lastDate.Value.ToString("dd/MM/yyyy")}",
                                        AvereDayTotal = avereDay,
                                        DareDayTotal = dareDay
                                    });
                                    spaceUsed += breakSpace;
                                }
                                else
                                {
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsPageBottom = true,
                                        AvereBottom = avereTop,
                                        DareBottom = dareTop
                                    });
                                    spaceUsed += breakSpace;
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsPageTop = true,
                                        AvereTop = avereTop,
                                        DareTop = dareTop
                                    });
                                    spaceUsed = breakSpace;
                                    displayRows.Add(new GeneralJournalReport.RowInfo()
                                    {
                                        IsDayTotal = true,
                                        DayBreakText = $"Progressivi del giorno {lastDate}",
                                        AvereDayTotal = avereDay,
                                        DareDayTotal = dareDay
                                    });
                                    spaceUsed += breakSpace;
                                }
                            }
                            if (row.Sign == "D")
                            {
                                dareDay = (row.Amount ?? 0);
                                avereDay = 0;
                            }
                            else
                            {
                                avereDay = (row.Amount ?? 0);
                                dareDay = 0;
                            }

                            AddJournalRow(displayRows, row, ref dareTop, ref avereTop, ref spaceUsed);
                            lastDate = row.RegistrationDate;
                        }
                        else
                        {
                            if (row.Sign == "D")
                                dareDay += (row.Amount ?? 0);
                            else
                                avereDay += (row.Amount ?? 0);

                            AddJournalRow(displayRows, row, ref dareTop, ref avereTop, ref spaceUsed);
                        }
                        if (row.Sign == "D")
                            dareTop += (row.Amount ?? 0);
                        else
                            avereTop += (row.Amount ?? 0);
                    }
                    // add last day break
                    displayRows.Add(new GeneralJournalReport.RowInfo()
                    {
                        IsDayTotal = true,
                        DayBreakText = $"Progressivi del giorno {(lastDate ?? DateTime.MinValue).ToString("dd/MM/yyyy")}",
                        AvereDayTotal = avereDay,
                        DareDayTotal = dareDay
                    });
                    // add last page break
                    displayRows.Add(new GeneralJournalReport.RowInfo()
                    {
                        IsPageBottom = true,
                        AvereBottom = avereTop,
                        DareBottom = dareTop
                    });
                }
                return new GeneralJournalReport()
                {
                    AccountingYear = AccountingYear,
                    StartingPage = esercizio.esepag ?? 0,
                    CompanyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID),
                    Rows = displayRows,
                    DareTotal = dareTop,
                    AvereTotal = avereTop,
                    PrintLimit = PrintUntil
                };

            }
            catch (Exception exc)
            {
                ErrorHandler.Show(exc.Message);
                return null;
            }
        }

        public bool UpdateJournalDefinitives(GeneralJournalReport ReportData, int LastPagePrinted, string CompanyID, string UserID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using var transaction = connection.BeginTransaction();
                try
                {
                    // update ESERCIZIO
                    connection.Execute("UPDATE ESERCIZIO SET esepag=@esepag, eseulr=@eseulr, eseusg=@eseusg, esedar=@esedar, eseave=@eseave, updatedUserID=@uid, updated=@eseusg  WHERE esesoc=@cid AND eseann=@yea",
                        new
                        {
                            esepag = LastPagePrinted,
                            eseulr = ((ReportData.Rows ?? new List<GeneralJournalReport.RowInfo>()).Where(w => !w.IsDayTotal && !w.IsPageTop && !w.IsPageBottom).Max(max => max.RowNumber) ?? 0),
                            eseusg = ReportData.PrintLimit,
                            esedar = ReportData.DareTotal,
                            eseave = ReportData.AvereTotal,
                            cid = CompanyID,
                            yea = ReportData.AccountingYear,
                            uid = UserID
                        }, transaction);
                    // update rows printed
                    Parallel.ForEach((ReportData.Rows ?? new List<GeneralJournalReport.RowInfo>()).Where(w => !w.IsDayTotal && !w.IsPageTop && !w.IsPageBottom), row =>
                    {
                        connection.Execute("UPDATE PN_RIGHE SET N1STBO = '*', N1RIGB=@n1rigb, N1DABB=@now WHERE N1SOCI=@cid AND N1ANNO=@yea AND N1REGI=@regi AND N1RIGA=@riga",
                            new { now = ReportData.PrintLimit.Date, cid = CompanyID, n1rigb = row.RowNumber, yea = ReportData.AccountingYear, regi = row.RegistrationNumber, riga = row.RegistrationRow }, transaction);
                    });
                    transaction.Commit();
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
                return false;
            }
        }

        public void AddJournalRow(List<GeneralJournalReport.RowInfo> displayRows, GeneralJournalReport.RowInfo row, ref decimal dareTop, ref decimal avereTop, ref decimal spaceUsed)
        {
            if (maxSpaceAvailable - spaceUsed >= (string.IsNullOrWhiteSpace(row.Note) ? minSpaceToAddRow : minSpaceToAddFullRow) + breakSpace)
            {
                displayRows.Add(row);
                if (!string.IsNullOrWhiteSpace(row.Note))
                    spaceUsed += rowFullSpace;
                else
                    spaceUsed += rowSpace;
            }
            else
            {
                displayRows.Add(new GeneralJournalReport.RowInfo()
                {
                    IsPageBottom = true,
                    AvereBottom = avereTop,
                    DareBottom = dareTop
                });
                spaceUsed += breakSpace;
                displayRows.Add(new GeneralJournalReport.RowInfo()
                {
                    IsPageTop = true,
                    AvereTop = avereTop,
                    DareTop = dareTop
                });
                spaceUsed = breakSpace;
                displayRows.Add(row);
                if (!string.IsNullOrWhiteSpace(row.Note))
                    spaceUsed += rowFullSpace;
                else
                    spaceUsed += rowSpace;
            }
        }
        #endregion
        #endregion
    }

}
