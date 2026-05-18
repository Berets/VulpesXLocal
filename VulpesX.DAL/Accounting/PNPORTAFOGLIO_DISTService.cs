

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;

namespace VulpesX.DAL.Accounting;

public interface IPNPORTAFOGLIO_DISTRepository
{
    ObservableCollection<PNPORTAFOGLIO_DIST>? GetList(string CompanyID, int Year);

    PNPORTAFOGLIO_DIST? Get(string company_id, long id);

    PNPORTAFOGLIO_DIST? GetFull(string company_id, long id);

    #region Functions
    bool GenerateFileCBI(PNPORTAFOGLIO_DIST Model, string CompanyName, string Path, bool OpenFile);

    bool Accounting(PNPORTAFOGLIO_DIST Model, DateTime AccountingDate, ESERCIZIO AccountingYear, string UserID);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(PNPORTAFOGLIO_DIST Model);

    bool InsertAll(PNPORTAFOGLIO_DIST Model);

    bool Update(PNPORTAFOGLIO_DIST Model);

    bool UpdateAll(PNPORTAFOGLIO_DIST Model);

    bool Delete(PNPORTAFOGLIO_DIST Model);

    bool DeleteAll(PNPORTAFOGLIO_DIST Model);

    string? Validate(PNPORTAFOGLIO_DIST Model, bool IsInsert);
    #endregion
}

public class PNPORTAFOGLIO_DISTRepository : RepositoryBase, IPNPORTAFOGLIO_DISTRepository
{
    public PNPORTAFOGLIO_DISTRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<PNPORTAFOGLIO_DIST>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNPORTAFOGLIO_DIST>(
                @"SELECT d.*, CONCAT(b.abiabi,'-',b.abicab,' ',a.abiban,' ',a.abiage) AS BankDescription FROM PNPORTAFOGLIO_DIST AS d
                        INNER JOIN BANAZIEN AS b ON b.abisoc=d.company_id AND b.abiabi=d.abi AND b.abicab=d.cab AND b.abicon=d.account
                        INNER JOIN ABICAB AS a ON a.abiabi=b.abiabi AND a.abicab=b.abicab
                        WHERE d.company_id=@cid AND CAST(d.id AS nvarchar(11)) like @yeac
                        ORDER BY id DESC",
                new { cid = CompanyID, yeac = Year.ToString() + "%" });

            return new ObservableCollection<PNPORTAFOGLIO_DIST>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNPORTAFOGLIO_DIST? Get(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNPORTAFOGLIO_DIST>(
                "SELECT * FROM PNPORTAFOGLIO_DIST WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNPORTAFOGLIO_DIST? GetFull(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<PNPORTAFOGLIO_DIST>(
                "SELECT * FROM PNPORTAFOGLIO_DIST WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id })
                .FirstOrDefault();

            // rows
            if (result != null)
            {
                result.Items = new ObservableCollection<PNPORTAFOGLIO>(connection.Query<PNPORTAFOGLIO>(
                    @"SELECT p.*, CONCAT(a.abecod,' ',a.abers1) AS CustomerDescription, CONCAT(TRIM(abiban),' ' , TRIM(abiage)) AS CustomerBankDescription FROM PNPORTAFOGLIO AS p
                        INNER JOIN ABE AS a ON a.abecod = p.N6SOTT
                        INNER JOIN ABICAB AS b ON b.abiabi = p.N6CABI AND b.abicab = p.N6CCAB
                        WHERE p.N6SOCI=@company_id AND p.N6DISTI=@id
                        ORDER BY N6SCAD DESC, N6SOTT",
                    new { company_id = company_id, id = id }).ToList());
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Functions
    public bool GenerateFileCBI(PNPORTAFOGLIO_DIST Model, string CompanyName, string Path, bool OpenFile)
    {
        try
        {
            using var connection = GetOpenConnection();

         

                var bank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Model.company_id, Model.abi!.Value, Model.cab!.Value, Model.account!)!;

                var companyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Model.company_id);

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var fullPath = $"{Path}\\{(!string.IsNullOrWhiteSpace(bank.abisigla) ? bank.abisigla.Trim() : "CBI")}_{Model.id}.txt";
                if (!string.IsNullOrWhiteSpace(bank.abisia))
                {
                    var sb = new StringBuilder();
                    int prog = 1;
                    // header
                    sb.Append($" IB{bank.abisia.ToUpper()}{bank.abiabi.ToString().PadLeft(5, '0')}{now.ToString("ddMMyy")}{Model.id.ToString().PadRight(20, ' ')}{"".PadLeft(6, ' ')}{"".PadLeft(59, ' ')}{"".PadLeft(7, ' ')}{"".PadLeft(2, ' ')}E {"".PadLeft(5, ' ')}");
                    sb.Append("\r\n");
                    foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                    {
                        var customer = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(item.N6SOTT ?? 0)!;
                        // string sanitization
                        string? companyName = TextHelper.SanitizeFull(CompanyName.Trim());
                        string? customerName = TextHelper.SanitizeFull(customer.abers1?.Trim());
                        string? customerAddress = TextHelper.SanitizeFull(customer.abeind.Trim());
                        string? customerCity = TextHelper.SanitizeFull(customer.abeloc?.Trim() + " " + customer.abepro?.Trim());

                        if (customerCity?.Length > 25)
                            customerCity = TextHelper.SanitizeFull(customer.abeloc?.Trim().Substring(0, 22) + " " + customer.abepro?.Trim());

                        string? bankDescription = TextHelper.SanitizeFull(item.CustomerBankDescription?.Trim());

                        sb.Append($" 14{prog.ToString().PadLeft(7, '0')}{"".PadLeft(12, ' ')}{(item.N6SCAD ?? DateTime.MinValue).ToString("ddMMyy")}30000{(item.N6IMEU ?? 0).ToString("N2").Replace(",", string.Empty).Replace(".", string.Empty).PadLeft(13, '0')}-{bank.abiabi.ToString().PadLeft(5, '0')}{bank.abicab.ToString().PadLeft(5, '0')}{bank.abicon.ToString().PadLeft(12, '0')}{(item.N6CABI ?? 0).ToString().PadLeft(5, '0')}{(item.N6CCAB ?? 0).ToString().PadLeft(5, '0')}{"".PadLeft(12, ' ')}{bank.abisia.ToUpper()}4{(item.N6SOTT ?? 0).ToString().PadRight(16, ' ')}{"".PadLeft(6, ' ')}E");
                        sb.Append("\r\n");
                        sb.Append($" 20{prog.ToString().PadLeft(7, '0')}{(companyName?.Length <= 96 ? companyName.PadRight(96, ' ') : companyName?.Substring(0, 96))}{"".PadLeft(14, ' ')}");
                        sb.Append("\r\n");
                        sb.Append($" 30{prog.ToString().PadLeft(7, '0')}{(customerName?.Length <= 60 ? customerName.PadRight(60, ' ') : customerName?.Substring(0, 60))}{customer.abepiv?.PadRight(16, ' ')}{"".PadLeft(34, ' ')}");
                        sb.Append("\r\n");
                        sb.Append($" 40{prog.ToString().PadLeft(7, '0')}{(customerAddress?.Length <= 30 ? customerAddress.PadRight(30, ' ') : customerAddress?.Substring(0, 30))}{customer.abecap?.ToString()?.Trim()?.PadLeft(5, '0')}{customerCity}{(bankDescription?.Length <= 50 ? bankDescription.PadRight(50, ' ') : bankDescription?.Substring(0, 50))}");
                        sb.Append("\r\n");
                        sb.Append($" 50{prog.ToString().PadLeft(7, '0')}{($"FATT.{item.N6DOCU?.Trim()} del {(item.N6DADO ?? DateTime.MinValue).ToString("dd/MM/yyyy")}").PadRight(80, ' ')}{"".PadLeft(10, ' ')}{companyInfo?.azpaiv?.PadRight(16, ' ')}{"".PadLeft(4, ' ')}");
                        sb.Append("\r\n");
                        sb.Append($" 51{prog.ToString().PadLeft(7, '0')}{prog.ToString().PadLeft(10, '0')}{(companyName?.Length <= 20 ? companyName?.PadRight(20, ' ') : companyName?.Substring(0, 20))}{"".PadLeft(80, ' ')}");
                        sb.Append("\r\n");
                        sb.Append($" 70{prog.ToString().PadLeft(7, '0')}{"".PadLeft(110, ' ')}");
                        sb.Append("\r\n");
                        prog++;
                    }
                    // footer
                    sb.Append($" EF{bank.abisia.ToUpper()}{bank.abiabi.ToString().PadLeft(5, '0')}{now.ToString("ddMMyy")}{Model.id.ToString().PadRight(20, ' ')}{"".PadLeft(6, ' ')}{Model.Items?.Count.ToString().PadLeft(7, '0')}{Model.amount.ToString("N2").Replace(",", string.Empty).Replace(".", string.Empty).PadLeft(15, '0')}{"".PadLeft(15, '0')}{((7 * (prog - 1)) + 2).ToString().PadLeft(7, '0')}{"".PadLeft(24, ' ')}E{"".PadLeft(6, ' ')}\r\n");
                    var fs = new FileStream(fullPath, FileMode.Create);
                    var data = Encoding.UTF8.GetBytes(sb.ToString());
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                    fs.Close();
                    InfoHandler.Show($"Il file CBI e' stato correttamente creato nel seguente percorso:\n{fullPath}");


                    if (OpenFile)
                    {
                        var proc = new ProcessStartInfo(fullPath);
                        proc.UseShellExecute = true;
                        Process.Start(proc);
                    }
                    return true;
                }
                else
                {
                    ErrorHandler.Show("Codice SIA non presente sulla banca aziendale selezionata");
                    return false;
                }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Accounting(PNPORTAFOGLIO_DIST Model, DateTime AccountingDate, ESERCIZIO AccountingYear, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var accountingRepository = VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>();

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                // company bank
                var bank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Model.company_id, Model.abi!.Value, Model.cab!.Value, Model.account!)!;

                if (bank == null || string.IsNullOrWhiteSpace(bank.abigru) || string.IsNullOrWhiteSpace(bank.abicot) || string.IsNullOrWhiteSpace(bank.abisot))
                {
                    ErrorHandler.Show("La banca aziendale non è correttamente compilata, mancano alcune informazioni sui conti necessari per la contabilizzazione");
                    transaction.Rollback();
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirstByType("C", Model.company_id);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show("Non trovato sottoconto cliente per la società corrente");
                    transaction.Rollback();
                    return false;
                }
                // check second reg
                if (!string.IsNullOrWhiteSpace(bank.abicau3) &&
                    (string.IsNullOrWhiteSpace(bank.abigba) || string.IsNullOrWhiteSpace(bank.abicba) || string.IsNullOrWhiteSpace(bank.abisba)))
                {
                    ErrorHandler.Show("La banca aziendale non è correttamente compilata, manca il conto banca necessario per la contabilizzazione");
                    transaction.Rollback();
                    return false;
                }

                #region First registration
                // get first registration number
                var firstAccountingID = numRegRepository.GetNumber(Model.company_id, AccountingYear.eseann, Constants.PN, true);

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = Model.company_id,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = firstAccountingID,
                    pncaus = bank.abicau1,
                    N1DARE = AccountingDate.Date,
                    N1docn = firstAccountingID.ToString(),
                    N1docd = AccountingDate.Date,
                    N1rifn = firstAccountingID.ToString(),
                    N1rifd = AccountingDate.Date,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Model.Items?.First().N6SOTT,
                    N1FLCF = "C",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                #region Customer rows
                int rowsCounter = 1;
                PNRIGHE walletRow = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = rowsCounter++,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1rifn,
                    N1DARI = head.N1rifd,
                    n1clie = null,
                    N1SEGN = "D",
                    pngrup = bank.abigru,
                    pncont = bank.abicot,
                    pnsott = bank.abisot,
                    N1IMEU = Model.amount,
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "N",
                    n1paga = null,
                    n1scad = null,
                    N1DRri = head.N1docd,
                    N1STBO = string.Empty,
                    N1STMA = string.Empty,
                    N1STNO = string.Empty
                };
                connection.Execute(pnRigheRepository.INSERT_QUERY, walletRow, transaction);
                foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                {
                    // PNRIGHE
                    PNRIGHE customerRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = item.N6RIFE,
                        N1DARI = item.N6DARI,
                        n1clie = item.N6SOTT,
                        N1SEGN = "A",
                        pngrup = item.N6GRUP,
                        pncont = item.N6CONT,
                        pnsott = pdcSotto.P3SOTC,
                        N1IMEU = item.N6IMEU,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = null,
                        n1scad = item.N6SCAD,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    // PNCLIENTI
                    PNCLIENTI customer = new PNCLIENTI()
                    {
                        N2SOCI = head.N1SOCI,
                        N2ANNO = head.N1ANNO,
                        N2REGI = head.N1REGI,
                        N2RIGA = rowsCounter,
                        N2DARI = item.N6DARI,
                        N2RIFE = item.N6RIFE,
                        N2DOCU = head.N1docn,
                        N2DADO = head.N1docd,
                        N2DARE = AccountingDate.Date,
                        N2CAUS = head.pncaus,
                        N2GRUP = pdcSotto.P1GRUP,
                        N2CONT = pdcSotto.P2CONT,
                        N2SOTT = item.N6SOTT,
                        N2SSOC = head.N1SOCI,
                        N2SEGN = "A",
                        N2PAGA = null,
                        N2SCAD = item.N6SCAD,
                        N2DIVI = "EUR",
                        n2vcod = "UIC",
                        N2DIDO = "EUR",
                        N2VADO = "UIC",
                        N2TIDO = "E",
                        N2IMEU = item.N6IMEU,
                        n2rior = rowsCounter,
                        n2tipi = null
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, customer, transaction);
                    rowsCounter++;
                }
                #endregion
                #endregion

                #region Second registration
                int? secondAccountingID = null;
                if (!string.IsNullOrWhiteSpace(bank.abicau3))
                {
                    // get first registration number
                    secondAccountingID = numRegRepository.GetNumber(Model.company_id, AccountingYear.eseann, Constants.PN, true);

                    #region PNTESTATA
                    head = new PNTESTATA()
                    {
                        N1SOCI = Model.company_id,
                        N1ANNO = AccountingYear.eseann,
                        N1REGI = secondAccountingID.Value,
                        pncaus = bank.abicau3,
                        N1DARE = AccountingDate.Date,
                        N1docn = firstAccountingID.ToString(),
                        N1docd = AccountingDate.Date,
                        N1rifn = firstAccountingID.ToString(),
                        N1rifd = AccountingDate.Date,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1CLFO = null,
                        N1FLCF = null,
                        N1FL01 = string.Empty,
                        N1TmpPN = "S",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                    #endregion

                    #region Customer rows
                    rowsCounter = 1;
                    walletRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = head.N1rifn,
                        N1DARI = head.N1rifd,
                        n1clie = null,
                        N1SEGN = "A",
                        pngrup = bank.abigru,
                        pncont = bank.abicot,
                        pnsott = bank.abisot,
                        N1IMEU = Model.amount,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "S",
                        n1paga = null,
                        n1scad = null,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, walletRow, transaction);
                    foreach (var item in (Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>()).GroupBy(g => g.N6SCAD))
                    {
                        // PNRIGHE
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
                            n1clie = null,
                            N1SEGN = "D",
                            pngrup = bank.abigba,
                            pncont = bank.abicba,
                            pnsott = bank.abisba,
                            N1IMEU = Model.Items?.Where(w => w.N6SCAD == item.Key).Sum(sum => sum.N6IMEU),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "S",
                            n1paga = null,
                            n1scad = item.Key,
                            N1DRri = head.N1docd,
                            N1STBO = string.Empty,
                            N1STMA = string.Empty,
                            N1STNO = string.Empty,
                            N1DESC = (item.Key ?? DateTime.MinValue).ToString("dd/MM/yyyy")
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    }
                    #endregion
                }
                #endregion

                // flag invoice worked
                Model.accounting_date = now;
                connection.Execute(UPDATE_QUERY, Model, transaction);
                // update status on PNPORTAFOGLIO
                foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                {
                    item.N6STATO = "C";
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                }

                transaction.Commit();

                InfoHandler.Show($"Contabilizzazione completata correttamente, {(secondAccountingID.HasValue ? $"generate le registrazioni n.{firstAccountingID} e n.{secondAccountingID}" : $"generata la registrazione n.{firstAccountingID}")}");
                if (ConfirmHandler.Confirm("Si desidera stampare le registrazioni ?"))
                {
                    var item = pnTestataRepository.Get(Model.company_id, AccountingYear.eseann, firstAccountingID);

                    if (item != null)
                    {
                        var reportData = accountingRepository.PrintAccountingRecord(item);

                        if (reportData != null)
                        {
                            ReportingHandler.PrintPDF("", Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_RECORD, Model.company_id, reportData, $"Registrazione n.{item.PrintFullID}", item.PrintFilename, true);
                            if (secondAccountingID.HasValue)
                            {
                                item = pnTestataRepository.Get(Model.company_id, AccountingYear.eseann, secondAccountingID.Value);

                                if (item != null)
                                {
                                    reportData = accountingRepository.PrintAccountingRecord(item);

                                    if (reportData != null)
                                    {
                                        ReportingHandler.PrintPDF("", Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_RECORD, Model.company_id, reportData, $"Registrazione n.{item.PrintFullID}", item.PrintFilename, true);
                                    }
                                }
                            }
                        }
                    }
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
            return false;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PNPORTAFOGLIO_DIST (company_id,id,amount,abi,cab,account,extraction_date,accounting_date,added,addedUserID,updated,updatedUserID) OUTPUT INSERTED.rv VALUES(@company_id,@id,@amount,@abi,@cab,@account,@extraction_date,@accounting_date,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID)";
    public string UPDATE_QUERY => "UPDATE PNPORTAFOGLIO_DIST SET company_id = @company_id,id = @id,amount = @amount,abi = @abi,cab = @cab,account = @account,extraction_date = @extraction_date,accounting_date = @accounting_date,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PNPORTAFOGLIO_DIST OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(PNPORTAFOGLIO_DIST Model)
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

    public bool InsertAll(PNPORTAFOGLIO_DIST Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // add rows
                    foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                    {
                        item.N6DISTI = Model.id;
                        item.N6STATO = "E";
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                    }
                    // insert dist
                    connection.ExecuteScalar(INSERT_QUERY, Model, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
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

    public bool Update(PNPORTAFOGLIO_DIST Model)
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

    public bool UpdateAll(PNPORTAFOGLIO_DIST Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clear all selected and reselect
                    foreach (var item in connection.Query<PNPORTAFOGLIO>(@"SELECT * FROM PNPORTAFOGLIO WHERE N6SOCI=@cid AND N6DISTI=@id", new { cid = Model.company_id, id = Model.id }, transaction))
                    {
                        if ((Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>()).Where(w => w.N6SOCI == item.N6SOCI && w.N6ANNO == item.N6ANNO && w.N6REGI == item.N6REGI && w.N6RIGA == item.N6RIGA).FirstOrDefault() == null)
                        {
                            connection.Execute(@"UPDATE PNPORTAFOGLIO SET N6DISTI = NULL, N6STATO = 'N' WHERE N6SOCI = @n6soci AND N6ANNO = @n6anno AND N6REGI = @n6regi AND N6RIGA = @n6riga", new { n6soci = item.N6SOCI, n6anno = item.N6ANNO, n6regi = item.N6REGI, n6riga = item.N6RIGA }, transaction);
                        }
                    }
                    // update dist
                    connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
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

    public bool Delete(PNPORTAFOGLIO_DIST Model)
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

    public bool DeleteAll(PNPORTAFOGLIO_DIST Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // free rows
                    foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                    {
                        item.N6DISTI = null;
                        item.N6STATO = "N";
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                    }
                    // delete dist
                    connection.ExecuteScalar(DELETE_QUERY, Model, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
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
    public string? Validate(PNPORTAFOGLIO_DIST Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.account))
            {
                if (Model.Items != null && Model.Items.Count > 0)
                {
                    return null;
                }
                else
                { return "La distinta deve contenere almeno una disposizione"; }
            }
            else
            { return "La banca è un dato obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PNPORTAFOGLIO_DISTUfpRepository : RepositoryBase, IPNPORTAFOGLIO_DISTRepository
{
    public PNPORTAFOGLIO_DISTUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<PNPORTAFOGLIO_DIST>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNPORTAFOGLIO_DIST>(
    @"select 
                      d.n6soci as company_id,
                      d.N6NUDI as id,
                      YEAR(d.n6dadi) AS Year,
                      SUM(d.n6imeu) as amount,
                      d.N6AABI as abi,
                      d.N6ACAB as cab,
                      MIN(b.abicon) as account,
                      d.N6DADI as extraction_date,
                      CASE WHEN (MIN(d.N6COLL) = 'S') THEN GETDATE() ELSE null END as accounting_date,
                      CONCAT(MIN(b.abiabi),'-',MIN(b.abicab),' ',RTRIM(MIN(a.abiban)),' ',MIN(a.abiage)) AS BankDescription
                      from PN_PORTAFOGLIO as d
                      INNER JOIN BANAZIEN AS b ON b.abisoc = n6soci AND  b.abiabi=n6aabi AND b.abicab=n6acab 
                      INNER JOIN TAB_ABICAB AS a ON a.abiabi=b.abiabi AND a.abicab=b.abicab
                      where d.n6soci = @cid AND YEAR(n6dadi) = @yeac and n6nudi  is not null and n6nudi > 0
                      group by d.n6soci, d.n6nudi, d.N6DADI, n6aabi, n6acab",
    new { cid = CompanyID, yeac = Year });

            return new ObservableCollection<PNPORTAFOGLIO_DIST>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNPORTAFOGLIO_DIST? Get(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNPORTAFOGLIO_DIST>(
                "SELECT * FROM PNPORTAFOGLIO_DIST WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNPORTAFOGLIO_DIST? GetFull(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<PNPORTAFOGLIO_DIST>(
                @$"select 
d.n6soci as company_id,
YEAR(d.N6DADI) AS Year,
d.N6NUDI as id,
SUM(d.n6imeu) as amount,
d.N6AABI as abi,
d.N6ACAB as cab,
MIN(b.abicon) as account,
d.N6DADI as extraction_date,
CASE WHEN (MIN(d.N6COLL) = 'S') THEN GETDATE() ELSE null END as accounting_date,
CONCAT(MIN(b.abiabi),'-',MIN(b.abicab),' ',RTRIM(MIN(a.abiban)),' ',MIN(a.abiage)) AS BankDescription
from PN_PORTAFOGLIO as d
INNER JOIN BANAZIEN AS b ON b.abisoc = n6soci AND  b.abiabi=n6aabi AND b.abicab=n6acab 
INNER JOIN TAB_ABICAB AS a ON a.abiabi=b.abiabi AND a.abicab=b.abicab
where d.n6soci = @company_id and n6nudi = @id
group by d.n6soci, d.n6nudi, d.N6DADI, n6aabi, n6acab",
                new { company_id = company_id, id = id })
                .FirstOrDefault();

            // rows
            if (result != null)
            {
                result.Items = new ObservableCollection<PNPORTAFOGLIO>(connection.Query<PNPORTAFOGLIO>(
                    @"SELECT p.*, CONCAT(a.abecod,' ',a.abers1) AS CustomerDescription, CONCAT(TRIM(abiban),' ' , TRIM(abiage)) AS CustomerBankDescription FROM PN_PORTAFOGLIO AS p
                        LEFT OUTER JOIN ANAG_BASE AS a ON a.abecod = p.N6SOTT
                        LEFT OUTER JOIN TAB_ABICAB AS b ON b.abiabi = p.N6CABI AND b.abicab = p.N6CCAB
                        WHERE p.N6SOCI=@company_id AND p.n6nudi=@id
                        ORDER BY N6SCAD DESC, N6SOTT",
                    new { company_id = company_id, id = id }).ToList());
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Functions
    public bool GenerateFileCBI(PNPORTAFOGLIO_DIST Model, string CompanyName, string Path, bool OpenFile)
    {
        try
        {
            using var connection = GetOpenConnection();


            var bank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Model.company_id, Model.abi!.Value, Model.cab!.Value, Model.account!)!;

            var companyInfo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(Model.company_id);

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var fullPath = $"{Path}\\{(!string.IsNullOrWhiteSpace(bank.abisigla) ? bank.abisigla.Trim() : "CBI")}_{Model.id}.txt";
            if (!string.IsNullOrWhiteSpace(bank.abisia))
            {
                var sb = new StringBuilder();
                int prog = 1;
                // header
                sb.Append($" IB{bank.abisia.ToUpper()}{bank.abiabi.ToString().PadLeft(5, '0')}{now.ToString("ddMMyy")}{Model.id.ToString().PadRight(20, ' ')}{"".PadLeft(6, ' ')}{"".PadLeft(59, ' ')}{"".PadLeft(7, ' ')}{"".PadLeft(2, ' ')}E {"".PadLeft(5, ' ')}");
                sb.Append("\r\n");
                foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                {
                    var customer = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(item.N6SOTT ?? 0)!;
                    // string sanitization
                    string? companyName = TextHelper.SanitizeFull(CompanyName.Trim());
                    string? customerName = TextHelper.SanitizeFull(customer.abers1?.Trim());
                    string? customerAddress = TextHelper.SanitizeFull(customer.abeind.Trim());
                    string? customerCity = TextHelper.SanitizeFull(customer.abeloc?.Trim() + " " + customer.abepro?.Trim());

                    if (customerCity?.Length > 25)
                        customerCity = TextHelper.SanitizeFull(customer.abeloc?.Trim().Substring(0, 22) + " " + customer.abepro?.Trim());

                    string? bankDescription = TextHelper.SanitizeFull(item.CustomerBankDescription?.Trim());

                    sb.Append($" 14{prog.ToString().PadLeft(7, '0')}{"".PadLeft(12, ' ')}{(item.N6SCAD ?? DateTime.MinValue).ToString("ddMMyy")}30000{(item.N6IMEU ?? 0).ToString("N2").Replace(",", string.Empty).Replace(".", string.Empty).TrimEnd().PadLeft(13, '0')}-{bank.abiabi.ToString().PadLeft(5, '0')}{bank.abicab.ToString().PadLeft(5, '0')}{bank.abicon.TrimEnd().ToString().PadLeft(12, '0')}{(item.N6CABI ?? 0).ToString().PadLeft(5, '0')}{(item.N6CCAB ?? 0).ToString().PadLeft(5, '0')}{"".PadLeft(12, ' ')}{bank.abisia.ToUpper()}4{(item.N6SOTT ?? 0).ToString().PadRight(16, ' ')}{"".PadLeft(6, ' ')}E");
                    sb.Append("\r\n");
                    sb.Append($" 20{prog.ToString().PadLeft(7, '0')}{(companyName?.Length <= 96 ? companyName.PadRight(96, ' ') : companyName?.Substring(0, 96))}{"".PadLeft(14, ' ')}");
                    sb.Append("\r\n");
                    sb.Append($" 30{prog.ToString().PadLeft(7, '0')}{(customerName?.Length <= 60 ? customerName.PadRight(60, ' ') : customerName?.Substring(0, 60))}{customer.abepiv?.PadRight(16, ' ')}{"".PadLeft(34, ' ')}");
                    sb.Append("\r\n");
                    sb.Append($" 40{prog.ToString().PadLeft(7, '0')}{(customerAddress?.Length <= 30 ? customerAddress.PadRight(30, ' ') : customerAddress?.Substring(0, 30))}{customer.abecap?.ToString()?.Trim()?.PadLeft(5, '0')}{customerCity}{(bankDescription?.Length <= 50 ? bankDescription.PadRight(50, ' ') : bankDescription?.Substring(0, 50))}");
                    sb.Append("\r\n");
                    sb.Append($" 50{prog.ToString().PadLeft(7, '0')}{($"FATT.{item.N6DOCU?.Trim()} del {(item.N6DADO ?? DateTime.MinValue).ToString("dd/MM/yyyy")}").PadRight(80, ' ')}{"".PadLeft(10, ' ')}{companyInfo?.azpaiv?.PadRight(16, ' ')}{"".PadLeft(4, ' ')}");
                    sb.Append("\r\n");
                    sb.Append($" 51{prog.ToString().PadLeft(7, '0')}{prog.ToString().PadLeft(10, '0')}{(companyName?.Length <= 20 ? companyName?.PadRight(20, ' ') : companyName?.Substring(0, 20))}{"".PadLeft(80, ' ')}");
                    sb.Append("\r\n");
                    sb.Append($" 70{prog.ToString().PadLeft(7, '0')}{"".PadLeft(110, ' ')}");
                    sb.Append("\r\n");
                    prog++;
                }
                // footer
                sb.Append($" EF{bank.abisia.ToUpper()}{bank.abiabi.ToString().PadLeft(5, '0')}{now.ToString("ddMMyy")}{Model.id.ToString().PadRight(20, ' ')}{"".PadLeft(6, ' ')}{Model.Items?.Count.ToString().PadLeft(7, '0')}{Model.amount.ToString("N2").Replace(",", string.Empty).Replace(".", string.Empty).PadLeft(15, '0')}{"".PadLeft(15, '0')}{((7 * (prog - 1)) + 2).ToString().PadLeft(7, '0')}{"".PadLeft(24, ' ')}E{"".PadLeft(6, ' ')}\r\n");
                var fs = new FileStream(fullPath, FileMode.Create);
                var data = Encoding.UTF8.GetBytes(sb.ToString());
                fs.Write(data, 0, data.Length);
                fs.Flush();
                fs.Close();
                InfoHandler.Show($"Il file CBI e' stato correttamente creato nel seguente percorso:\n{fullPath}");


                if (OpenFile)
                {
                    var proc = new ProcessStartInfo(fullPath);
                    proc.UseShellExecute = true;
                    Process.Start(proc);
                }
                return true;
            }
            else
            {
                ErrorHandler.Show("Codice SIA non presente sulla banca aziendale selezionata");
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Accounting(PNPORTAFOGLIO_DIST Model, DateTime AccountingDate, ESERCIZIO AccountingYear, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var numRegRepository = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                var accountingRepository = VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>();

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                // company bank
                var bank = VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(Model.company_id, Model.abi!.Value, Model.cab!.Value, Model.account!)!;

                if (bank == null || string.IsNullOrWhiteSpace(bank.abigru) || string.IsNullOrWhiteSpace(bank.abicot) || string.IsNullOrWhiteSpace(bank.abisot))
                {
                    ErrorHandler.Show("La banca aziendale non è correttamente compilata, mancano alcune informazioni sui conti necessari per la contabilizzazione");
                    transaction.Rollback();
                    return false;
                }
                // PDC
                var pdcSotto = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirstByType("C", Model.company_id);
                if (pdcSotto == null)
                {
                    ErrorHandler.Show("Non trovato sottoconto cliente per la società corrente");
                    transaction.Rollback();
                    return false;
                }
                // check second reg
                if (!string.IsNullOrWhiteSpace(bank.abicau3) &&
                    (string.IsNullOrWhiteSpace(bank.abigba) || string.IsNullOrWhiteSpace(bank.abicba) || string.IsNullOrWhiteSpace(bank.abisba)))
                {
                    ErrorHandler.Show("La banca aziendale non è correttamente compilata, manca il conto banca necessario per la contabilizzazione");
                    transaction.Rollback();
                    return false;
                }

                #region First registration
                // get first registration number
                var firstAccountingID = numRegRepository.GetNumber(Model.company_id, AccountingYear.eseann, Constants.PN, true);

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = Model.company_id,
                    N1ANNO = AccountingYear.eseann,
                    N1REGI = firstAccountingID,
                    pncaus = bank.abicau1,
                    N1DARE = AccountingDate.Date,
                    N1docn = firstAccountingID.ToString(),
                    N1docd = AccountingDate.Date,
                    N1rifn = firstAccountingID.ToString(),
                    N1rifd = AccountingDate.Date,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1CLFO = Model.Items?.First().N6SOTT,
                    N1FLCF = "C",
                    N1FL01 = string.Empty,
                    N1TmpPN = "N",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                #region Customer rows
                int rowsCounter = 1;
                PNRIGHE walletRow = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = rowsCounter++,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1rifn,
                    N1DARI = head.N1rifd,
                    n1clie = null,
                    N1SEGN = "D",
                    pngrup = bank.abigru,
                    pncont = bank.abicot,
                    pnsott = bank.abisot,
                    N1IMEU = Model.amount,
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "N",
                    n1paga = null,
                    n1scad = null,
                    N1DRri = head.N1docd,
                    N1STBO = string.Empty,
                    N1STMA = string.Empty,
                    N1STNO = string.Empty
                };
                connection.Execute(pnRigheRepository.INSERT_QUERY, walletRow, transaction);
                foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                {
                    // PNRIGHE
                    PNRIGHE customerRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = item.N6RIFE,
                        N1DARI = item.N6DARI,
                        n1clie = item.N6SOTT,
                        N1SEGN = "A",
                        pngrup = item.N6GRUP,
                        pncont = item.N6CONT,
                        pnsott = pdcSotto.P3SOTC,
                        N1IMEU = item.N6IMEU,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "N",
                        n1paga = null,
                        n1scad = item.N6SCAD,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    // PNCLIENTI
                    PNCLIENTI customer = new PNCLIENTI()
                    {
                        N2SOCI = head.N1SOCI,
                        N2ANNO = head.N1ANNO,
                        N2REGI = head.N1REGI,
                        N2RIGA = rowsCounter,
                        N2DARI = item.N6DARI,
                        N2RIFE = item.N6RIFE,
                        N2DOCU = head.N1docn,
                        N2DADO = head.N1docd,
                        N2DARE = AccountingDate.Date,
                        N2CAUS = head.pncaus,
                        N2GRUP = pdcSotto.P1GRUP,
                        N2CONT = pdcSotto.P2CONT,
                        N2SOTT = item.N6SOTT,
                        N2SSOC = head.N1SOCI,
                        N2SEGN = "A",
                        N2PAGA = null,
                        N2SCAD = item.N6SCAD,
                        N2DIVI = "EUR",
                        n2vcod = "UIC",
                        N2DIDO = "EUR",
                        N2VADO = "UIC",
                        N2TIDO = "E",
                        N2IMEU = item.N6IMEU,
                        n2rior = rowsCounter,
                        n2tipi = null
                    };
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, customer, transaction);
                    rowsCounter++;
                }
                #endregion
                #endregion

                #region Second registration
                int? secondAccountingID = null;
                if (!string.IsNullOrWhiteSpace(bank.abicau3))
                {
                    // get first registration number
                    secondAccountingID = numRegRepository.GetNumber(Model.company_id, AccountingYear.eseann, Constants.PN, true);

                    #region PNTESTATA
                    head = new PNTESTATA()
                    {
                        N1SOCI = Model.company_id,
                        N1ANNO = AccountingYear.eseann,
                        N1REGI = secondAccountingID.Value,
                        pncaus = bank.abicau3,
                        N1DARE = AccountingDate.Date,
                        N1docn = firstAccountingID.ToString(),
                        N1docd = AccountingDate.Date,
                        N1rifn = firstAccountingID.ToString(),
                        N1rifd = AccountingDate.Date,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1CLFO = null,
                        N1FLCF = null,
                        N1FL01 = string.Empty,
                        N1TmpPN = "S",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                    #endregion

                    #region Customer rows
                    rowsCounter = 1;
                    walletRow = new PNRIGHE()
                    {
                        N1SOCI = head.N1SOCI,
                        N1ANNO = head.N1ANNO,
                        N1REGI = head.N1REGI,
                        N1RIGA = rowsCounter++,
                        N1DOCU = head.N1docn,
                        N1DADO = head.N1docd,
                        N1RIFE = head.N1rifn,
                        N1DARI = head.N1rifd,
                        n1clie = null,
                        N1SEGN = "A",
                        pngrup = bank.abigru,
                        pncont = bank.abicot,
                        pnsott = bank.abisot,
                        N1IMEU = Model.amount,
                        N1CHIU = "A",
                        N1TIDO = "E",
                        N1DIVI = "EUR",
                        N1tmpPNR = "S",
                        n1paga = null,
                        n1scad = null,
                        N1DRri = head.N1docd,
                        N1STBO = string.Empty,
                        N1STMA = string.Empty,
                        N1STNO = string.Empty
                    };
                    connection.Execute(pnRigheRepository.INSERT_QUERY, walletRow, transaction);
                    foreach (var item in (Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>()).GroupBy(g => g.N6SCAD))
                    {
                        // PNRIGHE
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
                            n1clie = null,
                            N1SEGN = "D",
                            pngrup = bank.abigba,
                            pncont = bank.abicba,
                            pnsott = bank.abisba,
                            N1IMEU = Model.Items?.Where(w => w.N6SCAD == item.Key).Sum(sum => sum.N6IMEU),
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "S",
                            n1paga = null,
                            n1scad = item.Key,
                            N1DRri = head.N1docd,
                            N1STBO = string.Empty,
                            N1STMA = string.Empty,
                            N1STNO = string.Empty,
                            N1DESC = (item.Key ?? DateTime.MinValue).ToString("dd/MM/yyyy")
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                    }
                    #endregion
                }
                #endregion

                // flag invoice worked
                //Model.accounting_date = now;
                //connection.Execute(UPDATE_QUERY, Model, transaction);

                // update status on PNPORTAFOGLIO
                foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                {
                    item.N6COLL = "S";
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                }

                transaction.Commit();

                InfoHandler.Show($"Contabilizzazione completata correttamente, {(secondAccountingID.HasValue ? $"generate le registrazioni n.{firstAccountingID} e n.{secondAccountingID}" : $"generata la registrazione n.{firstAccountingID}")}");
                if (ConfirmHandler.Confirm("Si desidera stampare le registrazioni ?"))
                {
                    var item = pnTestataRepository.Get(Model.company_id, AccountingYear.eseann, firstAccountingID);

                    if (item != null)
                    {
                        var reportData = accountingRepository.PrintAccountingRecord(item);

                        if (reportData != null)
                        {
                            ReportingHandler.PrintPDF("", Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_RECORD, Model.company_id, reportData, $"Registrazione n.{item.PrintFullID}", item.PrintFilename, true);
                            if (secondAccountingID.HasValue)
                            {
                                item = pnTestataRepository.Get(Model.company_id, AccountingYear.eseann, secondAccountingID.Value);

                                if (item != null)
                                {
                                    reportData = accountingRepository.PrintAccountingRecord(item);

                                    if (reportData != null)
                                    {
                                        ReportingHandler.PrintPDF("", Constants.MODULE_ACCOUNTING, Constants.REPORT_TYPE_ACCOUNTING_RECORD, Model.company_id, reportData, $"Registrazione n.{item.PrintFullID}", item.PrintFilename, true);
                                    }
                                }
                            }
                        }
                    }
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
            return false;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PNPORTAFOGLIO_DIST (company_id,id,amount,abi,cab,account,extraction_date,accounting_date,added,addedUserID,updated,updatedUserID) OUTPUT INSERTED.rv VALUES(@company_id,@id,@amount,@abi,@cab,@account,@extraction_date,@accounting_date,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID)";
    public string UPDATE_QUERY => "UPDATE PNPORTAFOGLIO_DIST SET company_id = @company_id,id = @id,amount = @amount,abi = @abi,cab = @cab,account = @account,extraction_date = @extraction_date,accounting_date = @accounting_date,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PNPORTAFOGLIO_DIST OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(PNPORTAFOGLIO_DIST Model)
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

    public bool InsertAll(PNPORTAFOGLIO_DIST Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // add rows
                    foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                    {
                        item.N6NUDI = Model.id;
                        item.N6DADI = Model.extraction_date;
                        item.N6ESTR = "S";
                        item.N6COLL = "N";
                        item.N6AABI = Model.abi;
                        item.N6ACAB = Model.cab;

                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                    }
                    // insert dist
                    //connection.ExecuteScalar(INSERT_QUERY, Model, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
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

    public bool Update(PNPORTAFOGLIO_DIST Model)
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

    public bool UpdateAll(PNPORTAFOGLIO_DIST Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clear all selected and reselect
                    foreach (var item in connection.Query<PNPORTAFOGLIO>(@"SELECT * FROM PN_PORTAFOGLIO WHERE N6SOCI=@cid AND N6ANNO = @year AND N6AABI =@abi AND N6ACAB=@cab AND N6NUDI=@id", new { cid = Model.company_id, year = Model.Year, abi = Model.abi, cab = Model.cab, id = Model.id }, transaction))
                    {
                        if ((Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>()).Where(w => w.N6SOCI == item.N6SOCI && w.N6ANNO == item.N6ANNO && w.N6REGI == item.N6REGI && w.N6RIGA == item.N6RIGA).FirstOrDefault() == null)
                        {
                            connection.Execute(@"UPDATE PN_PORTAFOGLIO SET N6NUDI = NULL, N6DADI = NULL, N6ESTR = 'N', N6AABI = null, n6ACAB = null WHERE N6SOCI = @n6soci AND N6ANNO = @n6anno AND N6REGI = @n6regi AND N6RIGA = @n6riga", new { n6soci = item.N6SOCI, n6anno = item.N6ANNO, n6regi = item.N6REGI, n6riga = item.N6RIGA }, transaction);
                        }
                        else
                        {
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                        }
                    }
                    // update dist
                    //connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
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

    public bool Delete(PNPORTAFOGLIO_DIST Model)
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

    public bool DeleteAll(PNPORTAFOGLIO_DIST Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // free rows
                    foreach (var item in Model.Items ?? new ObservableCollection<PNPORTAFOGLIO>())
                    {
                        item.N6NUDI = null;
                        item.N6ESTR = "N";
                        item.N6COLL = "N";
                        item.N6DADI = null;
                        item.N6AABI = null;
                        item.N6ACAB = null;

                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().UPDATE_QUERY, item, transaction);
                    }
                    // delete dist
                    //connection.ExecuteScalar(DELETE_QUERY, Model, transaction);

                    transaction.Commit();
                    return true;
                }
                catch
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

    public string? Validate(PNPORTAFOGLIO_DIST Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.account))
            {
                if (Model.Items != null && Model.Items.Count > 0)
                {
                    return null;
                }
                else
                { return "La distinta deve contenere almeno una disposizione"; }
            }
            else
            { return "La banca è un dato obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}