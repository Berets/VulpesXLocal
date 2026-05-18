using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;

namespace VulpesX.DAL.CRM;

public interface IFATTD00FRepository
{
    ObservableCollection<FATTD00F>? GetList(string CompanyID, int Year, int Number);

    bool CheckPrices(string CompanyID, int Year, int Number);

    ObservableCollection<FATTD00F>? GetListByOrder(string CompanyID, int OrderYear, int OrderNumber);

    List<FATTD00F>? GetListWithRate(string CompanyID, int Year, int Number);

    FATTD00F? Get(string ftsoci, int FTANNO, int FTNUOR, int FDRIGA);

    bool? Exists(string ftsoci, int FTANNO, int FTNUOR, int FDRIGA);

    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(FATTD00F Model);

    bool Update(FATTD00F Model);

    bool UpdateAll(FATTT00F Head, ObservableCollection<FATTD00F> Rows);

    bool Delete(FATTD00F Model);

    string? Validate(FATTD00F Model, bool IsInsert);

    string? ValidateModel(ObservableCollection<FATTD00F> Rows);
}

public class FATTD00FRepository : RepositoryBase, IFATTD00FRepository
{
    private readonly IAZIENDARepository _aziendaRepository;
    private readonly ICLIAMMIRepository _cliammiRepository;
    private readonly IACC_PLAFONDRepository _acc_plafondRepository;
    private readonly IACC_PLAFOND_PARMSRepository _acc_plafondPARMSRepository;

    public FATTD00FRepository(IConnectionFactory factory, IAZIENDARepository IAZIENDARepository, ICLIAMMIRepository ICLIAMMIRepository, IACC_PLAFONDRepository IACC_PLAFONDRepository, IACC_PLAFOND_PARMSRepository IACC_PLAFOND_PARMSRepository) : base(factory)
    {
        _cliammiRepository = ICLIAMMIRepository;
        _aziendaRepository = IAZIENDARepository;
        _acc_plafondRepository = IACC_PLAFONDRepository;
        _acc_plafondPARMSRepository = IACC_PLAFOND_PARMSRepository;
    }

    public ObservableCollection<FATTD00F>? GetList(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FATTD00F>(
                @"SELECT * FROM FATTD00F
                        WHERE ftsoci = @cid AND FTANNO = @yea AND FTNUOR = @num
                        ORDER BY FDRIGA",
                new { cid = CompanyID, yea = Year, num = Number });
            return new ObservableCollection<FATTD00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CheckPrices(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FATTD00F
                        WHERE ftsoci = @cid AND FTANNO = @yea AND FTNUOR = @num AND (FDPREZ IS NULL OR FDPREZ = 0)",
                new { cid = CompanyID, yea = Year, num = Number }) == 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public ObservableCollection<FATTD00F>? GetListByOrder(string CompanyID, int OrderYear, int OrderNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FATTD00F>(
                @"SELECT * FROM FATTD00F
                        WHERE ftsoci = @cid AND OTANN1 = @yea AND OTNUO1 = @num
                        ORDER BY FDRIGA",
                new { cid = CompanyID, yea = OrderYear, num = OrderNumber });
            return new ObservableCollection<FATTD00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<FATTD00F>? GetListWithRate(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            var rows = connection.Query<FATTD00F>(
                        $@"SELECT r.*, 
                            (
                                SELECT TOP(1) TRIM(customerProductID) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductID,
                            (
                                SELECT TOP(1) TRIM(customerProductDescription) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductDescription,
                            p.*, u.*, al.*
                            FROM FATTD00F AS r
                            INNER JOIN FATTT00F AS h ON h.ftsoci=r.ftsoci AND h.FTANNO = r.FTANNO AND h.FTNUOR=r.FTNUOR
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR
                            ORDER BY r.FDBONO, r.FDBOLL, r.OTANN1, r.OTNUO1",
            new[] { typeof(FATTD00F), typeof(tab_articolo), typeof(tab_articolo_unita), typeof(ASSOGGETAMENTI) },
            (objs) =>
            {
                var obj = objs[0] as FATTD00F;

                obj!.Product = objs[1] as tab_articolo;
                obj!.UM = objs[2] as tab_articolo_unita;
                obj!.Rate = objs[3] as ASSOGGETAMENTI;

                return obj;
            },
            new { ftsoci = CompanyID, FTANNO = Year, FTNUOR = Number }, splitOn: "SocietaID,SocietaID,asscod").ToList();

            // check if custom code for each row
            foreach (var row in rows)
            {
                #region Customer code
                if (!string.IsNullOrWhiteSpace(row.CustomerProductID) && !string.IsNullOrWhiteSpace(row.CustomerProductDescription))
                {
                    var azienda = _aziendaRepository.Get(CompanyID);
                    if (azienda != null && azienda.AZCUSINV)
                    {
                        if (!string.IsNullOrWhiteSpace(row.CustomerProductID))
                            row.FDCODA = row.CustomerProductID;
                        if (!string.IsNullOrWhiteSpace(row.CustomerProductDescription) && row.Product != null)
                            row.Product.Descrizione = row.CustomerProductDescription;
                    }
                }
                #endregion
            }
            return rows;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTD00F? Get(string ftsoci, int FTANNO, int FTNUOR, int FDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FATTD00F>(
                "SELECT * FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA",
                new { ftsoci, FTANNO, FTNUOR, FDRIGA })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool? Exists(string ftsoci, int FTANNO, int FTNUOR, int FDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA",
                new { ftsoci, FTANNO, FTNUOR, FDRIGA }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTD00F (ftsoci,FTANNO,FTNUOR,FDRIGA,FDNUFD,FDCODA,FDQTAV,FDTQTA,FDPREZ,FDSCO1,FDSCO2,FDMAGG,FDTPRE,FDTSC1,FDTSC2,FDTMAG,FDUNIM,FDALIV,FDASSF,FDCOMM,FDDACO,FDSERI,FDRIFC,FDGRUP,FDCONT,FDSCTO,FDBONO,FDBOLL,FDBORI,OTANN1,OTNUO1,ODRIG1,FDSCO3,FDTSC3,FDFLA1,FDFLA2,FDFLA3,FDFLA4,FDPROV,fdpro2,fduni2,fdtiva,FDNOTE,FDSHOW,FDCOAG1,FDCOAG2,FDCOAG1PT,FDCOAG2PT,FDSTAMP,FDCECO) OUTPUT INSERTED.rv VALUES(@ftsoci,@FTANNO,@FTNUOR,@FDRIGA,@FDNUFD,@FDCODA,@FDQTAV,@FDTQTA,@FDPREZ,@FDSCO1,@FDSCO2,@FDMAGG,@FDTPRE,@FDTSC1,@FDTSC2,@FDTMAG,@FDUNIM,@FDALIV,@FDASSF,@FDCOMM,@FDDACO,@FDSERI,@FDRIFC,@FDGRUP,@FDCONT,@FDSCTO,@FDBONO,@FDBOLL,@FDBORI,@OTANN1,@OTNUO1,@ODRIG1,@FDSCO3,@FDTSC3,@FDFLA1,@FDFLA2,@FDFLA3,@FDFLA4,@FDPROV,@fdpro2,@fduni2,@fdtiva,@FDNOTE,@FDSHOW,@FDCOAG1,@FDCOAG2,@FDCOAG1PT,@FDCOAG2PT,@FDSTAMP,@FDCECO)";
    public string UPDATE_QUERY => "UPDATE FATTD00F SET ftsoci = @ftsoci,FTANNO = @FTANNO,FTNUOR = @FTNUOR,FDRIGA = @FDRIGA,FDNUFD = @FDNUFD,FDCODA = @FDCODA,FDQTAV = @FDQTAV,FDTQTA = @FDTQTA,FDPREZ = @FDPREZ,FDSCO1 = @FDSCO1,FDSCO2 = @FDSCO2,FDMAGG = @FDMAGG,FDTPRE = @FDTPRE,FDTSC1 = @FDTSC1,FDTSC2 = @FDTSC2,FDTMAG = @FDTMAG,FDUNIM = @FDUNIM,FDALIV = @FDALIV,FDASSF = @FDASSF,FDCOMM = @FDCOMM,FDDACO = @FDDACO,FDSERI = @FDSERI,FDRIFC = @FDRIFC,FDGRUP = @FDGRUP,FDCONT = @FDCONT,FDSCTO = @FDSCTO,FDBONO = @FDBONO,FDBOLL = @FDBOLL,FDBORI = @FDBORI,OTANN1 = @OTANN1,OTNUO1 = @OTNUO1,ODRIG1 = @ODRIG1,FDSCO3 = @FDSCO3,FDTSC3 = @FDTSC3,FDFLA1 = @FDFLA1,FDFLA2 = @FDFLA2,FDFLA3 = @FDFLA3,FDFLA4 = @FDFLA4,FDPROV = @FDPROV,fdpro2 = @fdpro2,fduni2 = @fduni2,fdtiva = @fdtiva,FDNOTE = @FDNOTE,FDSHOW = @FDSHOW,FDCOAG1 = @FDCOAG1,FDCOAG2 = @FDCOAG2,FDCOAG1PT = @FDCOAG1PT,FDCOAG2PT = @FDCOAG2PT,FDSTAMP = @FDSTAMP,FDCECO = @FDCECO OUTPUT INSERTED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTD00F OUTPUT DELETED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA AND rv = @rv";

    public bool Insert(FATTD00F Model)
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

    public bool Update(FATTD00F Model)
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

    public bool UpdateAll(FATTT00F Head, ObservableCollection<FATTD00F> Rows)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute("DELETE FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @ftanno AND FTNUOR = @ftnuor",
                    new { Head.ftsoci, ftanno = Head.FTANNO, ftnuor = Head.FTNUOR },
                    transaction);

                int rowID = 1;

                // 999999 is the stamp row, always last row
                foreach (var row in Rows.Where(w => w.FDRIGA != 999999).OrderBy(o => o.FDRIGA))
                {
                    row.FDRIGA = rowID++;
                    connection.Execute(INSERT_QUERY, row, transaction);
                }

                // check for plafond
                decimal plafondTotal = Rows.Where(w => w.SelectedRate != null && w.SelectedRate.assplaBool).Sum(sum => sum.Amount);
                if (plafondTotal > 0)
                {
                    var plafondSettings = _acc_plafondPARMSRepository.Get(Head.ftsoci);
                    var plafond = _acc_plafondRepository.GetLast(Head.ftsoci, Head.FTCODC!.Value, Head.FTANNO, Head.FTDAOR!.Value, false);

                    if (plafondSettings != null)
                    {
                        if (plafondTotal > plafondSettings.limit_amount && plafond != null)
                        {
                            // add stamp row
                            var cliammi = _cliammiRepository.Get(Head.ftsoci, Head.FTCODC.Value);
                            var existingStampRow = Rows.Where(w => w.FDRIGA == 999999).FirstOrDefault();

                            var stampRow = new FATTD00F()
                            {
                                ftsoci = Head.ftsoci,
                                FTANNO = Head.FTANNO,
                                FTNUOR = Head.FTNUOR,
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
                                FDSTAMP = existingStampRow != null ? existingStampRow.FDSTAMP : cliammi != null ? cliammi.CLASBOBool ? 1 : 2 : 1,
                                FDNOTE = existingStampRow != null ? existingStampRow.FDNOTE : plafond.clinote,
                                FDSHOW = existingStampRow != null ? existingStampRow.FDSHOW : true,
                                fdtiva = plafondSettings.IVANature
                            };
                            connection.Execute(INSERT_QUERY, stampRow, transaction);
                        }
                    }
                }
                else
                {
                    if (Rows.Where(w => w.FDRIGA == 999999).Count() > 0)
                    {
                        connection.Execute(INSERT_QUERY, Rows.Where(w => w.FDRIGA == 999999).First(), transaction);
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
            return false;
        }
    }

    public bool Delete(FATTD00F Model)
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

    public string? Validate(FATTD00F Model, bool IsInsert)
    {
        try
        {
            if (Model.FDTQTA != "O" || Model.FDTQTA == "O" && Model.SelectedRate?.assomaBool == true)
            {
                if (Model.FDQTAV.HasValue && Model.FDQTAV.Value > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.FDALIV))
                    {
                        if (Model.SelectedProduct != null)
                        {
                            if (!Model.SelectedProduct.QuantitaDefault.HasValue ||
                                        Model.SelectedProduct.QuantitaDefault.HasValue && Model.FDUNIM == Model.SelectedProduct.UnitaIDAlt && (Model.FDQTAV % 1 == 0 ? Model.FDQTAV * (Model.SelectedProduct.QuantitaDefault ?? 1) % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0 : Model.FDQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0) ||
                                        Model.SelectedProduct.QuantitaDefault.HasValue && Model.FDUNIM != Model.SelectedProduct.UnitaIDAlt && Model.FDQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0)
                            {
                                if (!string.IsNullOrEmpty(Model.ftsoci) && Model.FTANNO > 0 && Model.FTNUOR > 0 && Model.FDRIGA > 0)
                                {
                                    if (Model.FDPREZ > 0 || Model.FDPREZ == 0 && Model.FDTQTA == "N")
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.FDNOTE) || string.IsNullOrWhiteSpace(Model.FDNOTE) && !Model.FDSHOW)
                                        {
                                            if (!Model.FDSTAMP.HasValue && Model.FDRIGA != 999999 || Model.FDSTAMP.HasValue && Model.FDRIGA == 999999)
                                            {
                                                if ((Model.FDSCO1.HasValue && !string.IsNullOrWhiteSpace(Model.FDTSC1) ||
                                                !Model.FDSCO1.HasValue && string.IsNullOrWhiteSpace(Model.FDTSC1)) &&
                                                (Model.FDSCO2.HasValue && !string.IsNullOrWhiteSpace(Model.FDTSC2) ||
                                                !Model.FDSCO2.HasValue && string.IsNullOrWhiteSpace(Model.FDTSC2)) &&
                                                (Model.FDSCO3.HasValue && !string.IsNullOrWhiteSpace(Model.FDTSC3) ||
                                                !Model.FDSCO3.HasValue && string.IsNullOrWhiteSpace(Model.FDTSC3)))
                                                {
                                                    return null;
                                                }
                                                else
                                                { return "Se si seleziona uno sconto e' necessario impostarne anche il tipo altrimenti ometterli entrambi"; }
                                            }
                                            else
                                            { return "Il tipo bollo puo' essere modificato solo sulla riga del bollo"; }
                                        }
                                        else
                                        { return "Impossibile stampare una nota vuota"; }
                                    }
                                    else
                                    { return "Il prezzo e' obbligatorio"; }
                                }
                                else
                                { return "Il codice inserito è già in uso o non è valido"; }
                            }
                            else
                            { return $"La quantità digitata ({Model.FDQTAV.Value.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.SelectedProduct.QuantitaDefault ?? 1).ToString("N6")})"; }
                        }
                        else
                        {
                            return $"L'articolo è obbligatorio";
                        }
                    }
                    else
                    { return $"L'aliquota è obbligatoria"; }
                }
                else
                { return "La quantita' è obbligatoria"; }
            }
            else
            { return "In caso di omaggio l'aliquota deve essere una di quelle abilitate agli omaggi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string? ValidateModel(ObservableCollection<FATTD00F> Rows)
    {
        if (Rows != null && Rows.Count > 0)
        {
            if (Rows.Where(w => w.FDRIGA == 999999).Count() <= 1)
            {
                return null;
            }
            else
            {
                return "E' possibile inserire al massimo un bollo per fattura";
            }
        }
        else
        {
            return "E' necessario che siano presenti delle righe per confermare la fattura";
        }
    }

    #endregion
}

public class FATTD00FUfpRepository : RepositoryBase, IFATTD00FRepository
{
    private readonly IAZIENDARepository _aziendaRepository;
    private readonly ICLIAMMIRepository _cliammiRepository;
    private readonly IACC_PLAFONDRepository _acc_plafondRepository;
    private readonly IACC_PLAFOND_PARMSRepository _acc_plafondPARMSRepository;

    public FATTD00FUfpRepository(IConnectionFactory factory, IAZIENDARepository IAZIENDARepository, ICLIAMMIRepository ICLIAMMIRepository, IACC_PLAFONDRepository IACC_PLAFONDRepository, IACC_PLAFOND_PARMSRepository IACC_PLAFOND_PARMSRepository) : base(factory)
    {
        _cliammiRepository = ICLIAMMIRepository;
        _aziendaRepository = IAZIENDARepository;
        _acc_plafondRepository = IACC_PLAFONDRepository;
        _acc_plafondPARMSRepository = IACC_PLAFOND_PARMSRepository;
    }

    public ObservableCollection<FATTD00F>? GetList(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FATTD00F>(
                @"SELECT * FROM FATTD00F
                        WHERE ftsoci = @cid AND FTANNO = @yea AND FTNUOR = @num
                        ORDER BY FDRIGA",
                new { cid = CompanyID, yea = Year, num = Number });
            return new ObservableCollection<FATTD00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CheckPrices(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FATTD00F
                        WHERE ftsoci = @cid AND FTANNO = @yea AND FTNUOR = @num AND (FDPREZ IS NULL OR FDPREZ = 0)",
                new { cid = CompanyID, yea = Year, num = Number }) == 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public ObservableCollection<FATTD00F>? GetListByOrder(string CompanyID, int OrderYear, int OrderNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FATTD00F>(
                @"SELECT * FROM FATTD00F
                        WHERE ftsoci = @cid AND OTANN1 = @yea AND OTNUO1 = @num
                        ORDER BY FDRIGA",
                new { cid = CompanyID, yea = OrderYear, num = OrderNumber });
            return new ObservableCollection<FATTD00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<FATTD00F>? GetListWithRate(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            var rows = connection.Query<FATTD00F>(
                        $@"SELECT r.*, 
                            (
                                SELECT TOP(1) TRIM(customerProductID) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductID,
                            (
                                SELECT TOP(1) TRIM(customerProductDescription) FROM CRM_LISCLI AS l
                                WHERE l.companyID = r.ftsoci AND l.productID = r.FDCODA AND l.customerID = h.FTCODC AND unit_id=r.FDUNIM AND
                                CAST(l.fromDate AS date) <= h.FTDAOR AND CAST(l.toDate AS date) >= h.FTDAOR AND canceled IS NULL
                                ORDER BY l.fromDate DESC
                            ) AS CustomerProductDescription,
                            p.*, u.*, al.*
                            FROM FATTD00F AS r
                            INNER JOIN FATTT00F AS h ON h.ftsoci=r.ftsoci AND h.FTANNO = r.FTANNO AND h.FTNUOR=r.FTNUOR
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.ftsoci AND p.ID = r.FDCODA
                            LEFT OUTER JOIN tab_articolo_unita AS u ON u.SocietaID = r.ftsoci AND u.ID = r.FDUNIM
                            LEFT JOIN ASSOGGETAMENTI AS al ON al.asscod = r.FDASSF AND al.assali = r.FDALIV
                            WHERE r.ftsoci = @ftsoci AND r.FTANNO = @FTANNO AND r.FTNUOR = @FTNUOR
                            ORDER BY r.FDBONO, r.FDBOLL, r.OTANN1, r.OTNUO1",
            new[] { typeof(FATTD00F), typeof(tab_articolo), typeof(tab_articolo_unita), typeof(ASSOGGETAMENTI) },
            (objs) =>
            {
                var obj = objs[0] as FATTD00F;

                obj!.Product = objs[1] as tab_articolo;
                obj!.UM = objs[2] as tab_articolo_unita;
                obj!.Rate = objs[3] as ASSOGGETAMENTI;

                return obj;
            },
            new { ftsoci = CompanyID, FTANNO = Year, FTNUOR = Number }, splitOn: "SocietaID,SocietaID,asscod").ToList();

            // check if custom code for each row
            foreach (var row in rows)
            {
                #region Customer code
                if (!string.IsNullOrWhiteSpace(row.CustomerProductID) && !string.IsNullOrWhiteSpace(row.CustomerProductDescription))
                {
                    var azienda = _aziendaRepository.Get(CompanyID);
                    if (azienda != null && azienda.AZCUSINV)
                    {
                        if (!string.IsNullOrWhiteSpace(row.CustomerProductID))
                            row.FDCODA = row.CustomerProductID;
                        if (!string.IsNullOrWhiteSpace(row.CustomerProductDescription) && row.Product != null)
                            row.Product.Descrizione = row.CustomerProductDescription;
                    }
                }
                #endregion
            }
            return rows;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTD00F? Get(string ftsoci, int FTANNO, int FTNUOR, int FDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FATTD00F>(
                "SELECT * FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA",
                new { ftsoci, FTANNO, FTNUOR, FDRIGA })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool? Exists(string ftsoci, int FTANNO, int FTNUOR, int FDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA",
                new { ftsoci, FTANNO, FTNUOR, FDRIGA }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTD00F (ftsoci,FTANNO,FTNUOR,FDRIGA,FDNUFD,FDCODA,FDQTAV,FDTQTA,FDPREZ,FDSCO1,FDSCO2,FDMAGG,FDTPRE,FDTSC1,FDTSC2,FDTMAG,FDUNIM,FDALIV,FDASSF,FDCOMM,FDDACO,FDSERI,FDRIFC,FDGRUP,FDCONT,FDSCTO,FDBONO,FDBOLL,FDBORI,OTANN1,OTNUO1,ODRIG1,FDSCO3,FDTSC3,FDFLA1,FDFLA2,FDFLA3,FDFLA4,FDPROV,fdpro2,fduni2,fdtiva,fddaor,fdcodc,fdartdise,FDTVEn) OUTPUT INSERTED.rv VALUES(@ftsoci,@FTANNO,@FTNUOR,@FDRIGA,@FDNUFD,@FDCODA,@FDQTAV,@FDTQTA,@FDPREZ,@FDSCO1,@FDSCO2,@FDMAGG,@FDTPRE,@FDTSC1,@FDTSC2,@FDTMAG,@FDUNIM,@FDALIV,@FDASSF,@FDCOMM,@FDDACO,@FDSERI,@FDRIFC,@FDGRUP,@FDCONT,@FDSCTO,@FDBONO,@FDBOLL,@FDBORI,@OTANN1,@OTNUO1,@ODRIG1,@FDSCO3,@FDTSC3,@FDFLA1,@FDFLA2,@FDFLA3,@FDFLA4,@FDPROV,@fdpro2,@fduni2,@fdtiva,@fddaor,@fdcodc,@fdartdise,@FDTVEn)";
    public string UPDATE_QUERY => "UPDATE FATTD00F SET ftsoci = @ftsoci,FTANNO = @FTANNO,FTNUOR = @FTNUOR,FDRIGA = @FDRIGA,FDNUFD = @FDNUFD,FDCODA = @FDCODA,FDQTAV = @FDQTAV,FDTQTA = @FDTQTA,FDPREZ = @FDPREZ,FDSCO1 = @FDSCO1,FDSCO2 = @FDSCO2,FDMAGG = @FDMAGG,FDTPRE = @FDTPRE,FDTSC1 = @FDTSC1,FDTSC2 = @FDTSC2,FDTMAG = @FDTMAG,FDUNIM = @FDUNIM,FDALIV = @FDALIV,FDASSF = @FDASSF,FDCOMM = @FDCOMM,FDDACO = @FDDACO,FDSERI = @FDSERI,FDRIFC = @FDRIFC,FDGRUP = @FDGRUP,FDCONT = @FDCONT,FDSCTO = @FDSCTO,FDBONO = @FDBONO,FDBOLL = @FDBOLL,FDBORI = @FDBORI,OTANN1 = @OTANN1,OTNUO1 = @OTNUO1,ODRIG1 = @ODRIG1,FDSCO3 = @FDSCO3,FDTSC3 = @FDTSC3,FDFLA1 = @FDFLA1,FDFLA2 = @FDFLA2,FDFLA3 = @FDFLA3,FDFLA4 = @FDFLA4,FDPROV = @FDPROV,fdpro2 = @fdpro2,fduni2 = @fduni2,fdtiva = @fdtiva,fddaor=@fddaor,fdcodc=@fdcodc, fdartdise = @fdartdise,FDTVEn=@FDTVEn OUTPUT INSERTED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTD00F OUTPUT DELETED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND FDRIGA = @FDRIGA AND rv = @rv";

    public bool Insert(FATTD00F Model)
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

    public bool Update(FATTD00F Model)
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

    public bool UpdateAll(FATTT00F Head, ObservableCollection<FATTD00F> Rows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute("DELETE FROM FATTD00F WHERE ftsoci = @ftsoci AND FTANNO = @ftanno AND FTNUOR = @ftnuor",
                    new { Head.ftsoci, ftanno = Head.FTANNO, ftnuor = Head.FTNUOR },
                    transaction);

                int rowID = 1;

                // 999999 is the stamp row, always last row
                foreach (var row in Rows.Where(w => w.FDRIGA != 999999).OrderBy(o => o.FDRIGA))
                {
                    row.FDRIGA = rowID++;
                    connection.Execute(INSERT_QUERY, row, transaction);
                }

                // check for plafond
                decimal plafondTotal = Rows.Where(w => w.SelectedRate != null && w.SelectedRate.assplaBool).Sum(sum => sum.Amount);
                if (plafondTotal > 0)
                {
                    var plafondSettings = _acc_plafondPARMSRepository.Get(Head.ftsoci);
                    var plafond = _acc_plafondRepository.GetLast(Head.ftsoci, Head.FTCODC!.Value, Head.FTANNO, Head.FTDAOR!.Value, false);

                    if (plafondSettings != null)
                    {
                        if (plafondTotal > plafondSettings.limit_amount && plafond != null)
                        {
                            // add stamp row
                            var cliammi = _cliammiRepository.Get(Head.ftsoci, Head.FTCODC.Value);
                            var existingStampRow = Rows.Where(w => w.FDRIGA == 999999).FirstOrDefault();

                            var stampRow = new FATTD00F()
                            {
                                ftsoci = Head.ftsoci,
                                FTANNO = Head.FTANNO,
                                FTNUOR = Head.FTNUOR,
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
                                FDSTAMP = existingStampRow != null ? existingStampRow.FDSTAMP : cliammi != null ? cliammi.CLASBOBool ? 1 : 2 : 1,
                                FDNOTE = existingStampRow != null ? existingStampRow.FDNOTE : plafond.clinote,
                                FDSHOW = existingStampRow != null ? existingStampRow.FDSHOW : true,
                                fdtiva = plafondSettings.IVANature
                            };
                            connection.Execute(INSERT_QUERY, stampRow, transaction);
                        }
                    }
                }
                else
                {
                    if (Rows.Where(w => w.FDRIGA == 999999).Count() > 0)
                    {
                        connection.Execute(INSERT_QUERY, Rows.Where(w => w.FDRIGA == 999999).First(), transaction);
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
            return false;
        }
    }

    public bool Delete(FATTD00F Model)
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

    public string? Validate(FATTD00F Model, bool IsInsert)
    {
        try
        {
            if (Model.FDTQTA != "O" || Model.FDTQTA == "O" && Model.SelectedRate?.assomaBool == true)
            {
                if (Model.FDQTAV.HasValue && Model.FDQTAV.Value > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.FDALIV))
                    {
                        if (Model.SelectedProduct != null)
                        {
                            if (!Model.SelectedProduct.QuantitaDefault.HasValue ||
                                        Model.SelectedProduct.QuantitaDefault.HasValue && Model.FDUNIM == Model.SelectedProduct.UnitaIDAlt && (Model.FDQTAV % 1 == 0 ? Model.FDQTAV * (Model.SelectedProduct.QuantitaDefault ?? 1) % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0 : Model.FDQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0) ||
                                        Model.SelectedProduct.QuantitaDefault.HasValue && Model.FDUNIM != Model.SelectedProduct.UnitaIDAlt && Model.FDQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0)
                            {
                                if (!string.IsNullOrEmpty(Model.ftsoci) && Model.FTANNO > 0 && Model.FTNUOR > 0 && Model.FDRIGA > 0)
                                {
                                    if (Model.FDPREZ > 0 || Model.FDPREZ == 0 && Model.FDTQTA == "N")
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.FDNOTE) || string.IsNullOrWhiteSpace(Model.FDNOTE) && !Model.FDSHOW)
                                        {
                                            if (!Model.FDSTAMP.HasValue && Model.FDRIGA != 999999 || Model.FDSTAMP.HasValue && Model.FDRIGA == 999999)
                                            {
                                                if ((Model.FDSCO1.HasValue && !string.IsNullOrWhiteSpace(Model.FDTSC1) ||
                                                !Model.FDSCO1.HasValue && string.IsNullOrWhiteSpace(Model.FDTSC1)) &&
                                                (Model.FDSCO2.HasValue && !string.IsNullOrWhiteSpace(Model.FDTSC2) ||
                                                !Model.FDSCO2.HasValue && string.IsNullOrWhiteSpace(Model.FDTSC2)) &&
                                                (Model.FDSCO3.HasValue && !string.IsNullOrWhiteSpace(Model.FDTSC3) ||
                                                !Model.FDSCO3.HasValue && string.IsNullOrWhiteSpace(Model.FDTSC3)))
                                                {
                                                    return null;
                                                }
                                                else
                                                { return "Se si seleziona uno sconto e' necessario impostarne anche il tipo altrimenti ometterli entrambi"; }
                                            }
                                            else
                                            { return "Il tipo bollo puo' essere modificato solo sulla riga del bollo"; }
                                        }
                                        else
                                        { return "Impossibile stampare una nota vuota"; }
                                    }
                                    else
                                    { return "Il prezzo e' obbligatorio"; }
                                }
                                else
                                { return "Il codice inserito è già in uso o non è valido"; }
                            }
                            else
                            { return $"La quantità digitata ({Model.FDQTAV.Value.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.SelectedProduct.QuantitaDefault ?? 1).ToString("N6")})"; }
                        }
                        else
                        {
                            return $"L'articolo è obbligatorio";
                        }
                    }
                    else
                    { return $"L'aliquota è obbligatoria"; }
                }
                else
                { return "La quantita' è obbligatoria"; }
            }
            else
            { return "In caso di omaggio l'aliquota deve essere una di quelle abilitate agli omaggi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string? ValidateModel(ObservableCollection<FATTD00F> Rows)
    {
        if (Rows != null && Rows.Count > 0)
        {
            if (Rows.Where(w => w.FDRIGA == 999999).Count() <= 1)
            {
                return null;
            }
            else
            {
                return "E' possibile inserire al massimo un bollo per fattura";
            }
        }
        else
        {
            return "E' necessario che siano presenti delle righe per confermare la fattura";
        }
    }

    #endregion
}