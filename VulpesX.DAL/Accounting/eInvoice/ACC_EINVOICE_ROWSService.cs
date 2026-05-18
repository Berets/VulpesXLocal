using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting;
using VulpesX.Models.Ufp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static VulpesX.Models.Models.Accounting.CheckInvoiceEntranceModel;

namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_ROWSRepository
{
    ObservableCollection<ACC_EINVOICE_ROWS>? GetList();

    ObservableCollection<ACC_EINVOICE_ROWS>? GetList(long id);

    ACC_EINVOICE_ROWS? Get(long id, int fattriga);

    #region Check invoice entrance
    Tuple<List<CheckInvoiceEntranceModel.DettaglioModel>, List<CheckInvoiceEntranceModel.EntrataModel>> GetRows(string CompanyID, int SupplierID, string InvoiceID, DateTime Date, string VATNumber);

    List<CheckInvoiceEntranceModel.EntrataModel> GetEntrances(string CompanyID, int SupplierID, string DDTID, List<CheckInvoiceEntranceModel.EntrataModel> EntranceLinked);

    bool UpdateEntrance(CheckInvoiceEntranceModel.EntrataModel Entrance, CheckInvoiceEntranceModel.DettaglioModel Row);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_ROWS Model);

    bool Update(ACC_EINVOICE_ROWS Model);

    bool Delete(ACC_EINVOICE_ROWS Model);
    #endregion
}

public class ACC_EINVOICE_ROWSRepository : RepositoryBase, IACC_EINVOICE_ROWSRepository
{
    public ACC_EINVOICE_ROWSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_ROWS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS>(
                "SELECT * FROM ACC_EINVOICE_ROWS");

            return new ObservableCollection<ACC_EINVOICE_ROWS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_ROWS>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS, FE_IVADOC, ACC_EINVOICE_ROWS>(
                @"SELECT r.*, fei.FETICod, fei.FETIDes FROM ACC_EINVOICE_ROWS AS r
                        LEFT JOIN FE_IVADOC AS fei ON fei.FETICod=r.fattnatu
                        WHERE r.id = @id
                        ORDER BY r.fattriga",
                (row, fei) => { row.Nature = fei; return row; },
                new { id = id }, splitOn: "FETICod");

            var pids = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_PIDRepository>().GetList(id);
            var sms = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_SMRepository>().GetList(id);

            foreach (var row in list)
            {
                row.PIDs = new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>(pids?.Where(w => w.fattriga == row.fattriga).ToList() ?? new List<ACC_EINVOICE_ROWS_PIDS>());
                row.SMs = new ObservableCollection<ACC_EINVOICE_ROWS_SM>(sms?.Where(w => w.fattriga == row.fattriga).ToList() ?? new List<ACC_EINVOICE_ROWS_SM>());
            }

            return new ObservableCollection<ACC_EINVOICE_ROWS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_ROWS? Get(long id, int fattriga)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_ROWS>(
                "SELECT * FROM ACC_EINVOICE_ROWS WHERE id = @id AND fattriga = @fattriga",
                new { id = id, fattriga = fattriga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Check invoice entrance
    public Tuple<List<CheckInvoiceEntranceModel.DettaglioModel>, List<CheckInvoiceEntranceModel.EntrataModel>> GetRows(string CompanyID, int SupplierID, string InvoiceID, DateTime Date, string VATNumber)
    {
        throw new NotImplementedException();
    }

    public List<CheckInvoiceEntranceModel.EntrataModel> GetEntrances(string CompanyID, int SupplierID, string DDTID, List<CheckInvoiceEntranceModel.EntrataModel> EntranceLinked)
    {
        throw new NotImplementedException();
    }

    public bool UpdateEntrance(CheckInvoiceEntranceModel.EntrataModel Entrance, CheckInvoiceEntranceModel.DettaglioModel Row)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_ROWS (fattsoc,fattnum,fattdata,fattpiva,fattriga,fattartdes,fattprz,fattqta,fatttotriga,fattgrup,fattcont,fattsott,fattumi,fattaliriga,fattnatu,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattriga,@fattartdes,@fattprz,@fattqta,@fatttotriga,@fattgrup,@fattcont,@fattsott,@fattumi,@fattaliriga,@fattnatu,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_ROWS SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattriga = @fattriga,fattartdes = @fattartdes,fattprz = @fattprz,fattqta = @fattqta,fatttotriga = @fatttotriga,fattgrup = @fattgrup,fattcont = @fattcont,fattsott = @fattsott,fattumi = @fattumi,fattaliriga = @fattaliriga,fattnatu = @fattnatu,id = @id OUTPUT INSERTED.rv WHERE id = @id AND fattriga = @fattriga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_ROWS OUTPUT DELETED.rv WHERE id = @id AND fattriga = @fattriga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_ROWS Model)
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

    public bool Update(ACC_EINVOICE_ROWS Model)
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

    public bool Delete(ACC_EINVOICE_ROWS Model)
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
    #endregion
}

public class ACC_EINVOICE_ROWSUfpRepository : RepositoryBase, IACC_EINVOICE_ROWSRepository
{
    public ACC_EINVOICE_ROWSUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_ROWS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS>(
                "SELECT * FROM ACC_EINVOICE_ROWS");

            return new ObservableCollection<ACC_EINVOICE_ROWS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_ROWS>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_ROWS, FE_IVADOC, ACC_EINVOICE_ROWS>(
                @"SELECT r.*, fei.FETICod, fei.FETIDes FROM FATTIMPLEVEL2 AS r
                        LEFT JOIN FE_IVADOC AS fei ON fei.FETICod=r.fattnatu
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY r.fattriga",
                (row, fei) => { row.Nature = fei; return row; },
                 new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva }, splitOn: "FETICod");

                var pids = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_PIDRepository>().GetList(id);
                var sms = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_SMRepository>().GetList(id);

                foreach (var row in list)
                {
                    row.PIDs = new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>(pids?.Where(w => w.fattriga == row.fattriga).ToList() ?? new List<ACC_EINVOICE_ROWS_PIDS>());
                    row.SMs = new ObservableCollection<ACC_EINVOICE_ROWS_SM>(sms?.Where(w => w.fattriga == row.fattriga).ToList() ?? new List<ACC_EINVOICE_ROWS_SM>());
                }

                return new ObservableCollection<ACC_EINVOICE_ROWS>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_ROWS? Get(long id, int fattriga)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_ROWS>(
                "SELECT * FROM ACC_EINVOICE_ROWS WHERE id = @id AND fattriga = @fattriga",
                new { id = id, fattriga = fattriga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Check invoice entrance
    public Tuple<List<CheckInvoiceEntranceModel.DettaglioModel>, List<CheckInvoiceEntranceModel.EntrataModel>> GetRows(string CompanyID, int SupplierID, string InvoiceID, DateTime Date, string VATNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            var rows = new List<CheckInvoiceEntranceModel.DettaglioModel>();
            var entranceLinked = new List<CheckInvoiceEntranceModel.EntrataModel>();

            var rowsQuery = @"SELECT
                        x.fattsoc       AS SocietaID,
                        x.fattnum       AS FatturaID,
                        x.fattdata      AS Data,
                        x.fattpiva      AS PartitaIVA,
                        x.ddtriga       AS Riga,
                        x.ddtdata       AS DDTData,
                        x.ddtnum        AS DDTID,
                        CASE
                            WHEN d.fattartcod IS NOT NULL AND d.fattartcod <> ''
                                THEN d.fattartcod + '-' + d.fattartdes
                            ELSE d.fattartdes
                        END             AS ArticoloFull,
                        ISNULL(d.fattprz, 0)       AS Prezzo,
                        ISNULL(d.fattqta, 0)       AS Quantita,
                        ISNULL(d.fatttotriga, 0)   AS Totale
                    FROM FATTIMPDDT x
                    INNER JOIN FATTIMPLEVEL2 d
                        ON  x.fattsoc      = d.fattsoc
                        AND x.fattnum      = d.fattnum
                        AND x.fattdata     = d.fattdata
                        AND x.fattpiva     = d.fattpiva
                        AND x.ddtriferiga  = CAST(d.fattriga AS INT)
                    WHERE
                        x.fattsoc = @SocietaID AND x.fattnum = @FatturaID AND x.fattdata = @Date AND x.fattpiva = @VATNumber
                    ORDER BY
                        x.ddtriga;
                    ";

            rows = connection.Query<DettaglioModel>(rowsQuery,
            new
            {
                SocietaID = CompanyID,
                FatturaID = InvoiceID,
                Date = Date,
                VATNumber = VATNumber.TrimEnd(),
            }
            ).ToList() ?? new List<DettaglioModel>();

            foreach (var det in rows)
            {
                var entranceQuery = @"SELECT
                                        e.emsoci        AS SocietaID,
                                        e.EMCDFO        AS FornitoreID,
                                        e.EMEDBF        AS Data,
                                        e.EMENBF        AS DDTID,
                                        e.EMEPEM        AS Riga,
                                        ISNULL(e.emepre, 0) AS Prezzo,
                                        ISNULL(e.EMEQTA, 0) AS Quantita,
                                        e.otsoco        AS OrdineSocietaID,
                                        ISNULL(e.OTANNO, 0) AS OrdineAnno,
                                        ISNULL(e.OTNORP, 0) AS OrdineID,
                                        ISNULL(e.ODNUPP, 0) AS OrdineRiga,
                                        e.EMECAM        AS ArticoloID,
                                        a.artdise       AS ArticoloDisegno
                                    FROM AQENT00F e
                                    LEFT JOIN ANAG_ARTICOLI a
                                        ON a.ARTCOD = e.EMECAR
                                    WHERE
                                        e.emsoci = @SocietaID
                                        AND e.EMCDFO = @FornitoreID
                                        AND e.EMEDBF = @DDTData
                                        AND e.EMENBF = @DDTID
                                        AND e.EMEQTA = @Quantita
                                        AND e.emepre = @Totale
                                    ORDER BY e.EMEPEM;
                                    ";

                var entrateDb = connection.Query<EntrataModel>(
                        entranceQuery,
                        new
                        {
                            SocietaID = det.SocietaID,
                            FornitoreID = SupplierID,
                            DDTData = det.DDTData,
                            DDTID = det.DDTID?.Trim(),
                            Quantita = det.Quantita,
                            Totale = det.Totale
                        }
                    ).ToList();

                var entrance = entrateDb.Where(e => !entranceLinked.Where(o => o.SocietaID == e.SocietaID && o.FornitoreID == e.FornitoreID && o.Data == e.Data && o.DDTID == e.DDTID && o.Riga == e.Riga).Any()).FirstOrDefault();

                if (entrance != null)
                {
                    det.Entrata = entrance;
                    entranceLinked.Add(entrance);
                }
            }

            return new Tuple<List<DettaglioModel>, List<EntrataModel>>(rows, entranceLinked);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<List<DettaglioModel>, List<EntrataModel>>(new List<DettaglioModel>(), new List<EntrataModel>());
        }
    }

    public List<CheckInvoiceEntranceModel.EntrataModel> GetEntrances(string CompanyID, int SupplierID, string DDTID, List<CheckInvoiceEntranceModel.EntrataModel> EntranceLinked)
    {
        try
        {
            using var connection = GetOpenConnection();

            var entranceQuery = @"SELECT
                                        e.emsoci        AS SocietaID,
                                        e.EMCDFO        AS FornitoreID,
                                        e.EMEDBF        AS Data,
                                        e.EMENBF        AS DDTID,
                                        e.EMEPEM        AS Riga,
                                        ISNULL(e.emepre, 0) AS Prezzo,
                                        ISNULL(e.EMEQTA, 0) AS Quantita,
                                        e.otsoco        AS OrdineSocietaID,
                                        ISNULL(e.OTANNO, 0) AS OrdineAnno,
                                        ISNULL(e.OTNORP, 0) AS OrdineID,
                                        ISNULL(e.ODNUPP, 0) AS OrdineRiga,
                                        a.artcod        AS ArticoloID,
                                        a.artdise       AS ArticoloDisegno
                                    FROM AQENT00F e
                                    LEFT JOIN ANAG_ARTICOLI a
                                        ON a.ARTCOD = e.EMECAR
                                    WHERE
                                        e.emsoci = @SocietaID
                                        AND e.EMCDFO = @FornitoreID
                                        AND e.EMENBF = @DDTID
                                    ORDER BY e.EMEPEM;
                                    ";

            var entrateDb = connection.Query<EntrataModel>(entranceQuery,
            new
            {
                SocietaID = CompanyID,
                FornitoreID = SupplierID,
                DDTID = DDTID,
            }
            ).ToList();

            return entrateDb.Where(e => !EntranceLinked.Where(o => o.SocietaID == e.SocietaID && o.FornitoreID == e.FornitoreID && o.Data == e.Data && o.DDTID == e.DDTID && o.Riga == e.Riga).Any()).ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new List<EntrataModel>();
        }
    }

    public bool UpdateEntrance(CheckInvoiceEntranceModel.EntrataModel Entrance, CheckInvoiceEntranceModel.DettaglioModel Row)
    {

        using var connection = GetOpenConnection();

        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                // TESTATA
                connection.ExecuteScalar(@$"UPDATE AQENT00F SET EMENBF = @DDT, EMEDBF = @DDTData, EMEQTA = @Quantita, EMEPRE = @Prezzo, emeflgcon = '2' WHERE emsoci = @CompanyID AND EMCDFO = @FornitoreID AND EMEDBF = @DDTDataOld AND EMENBF = @DDTOld AND EMEPEM = @Riga", new
                {
                    CompanyID = Entrance.SocietaID,
                    FornitoreID = Entrance.FornitoreID,
                    DDTDataOld = Entrance.Data,
                    DDTOld = Entrance.DDTID,
                    Riga = Entrance.Riga,
                    DDTData = Row.DDTData,
                    DDT = Row.DDTID,
                    Quantita = Row.Quantita,
                    Prezzo = Row.Totale
                }, transaction);

                //DETAIL
                connection.ExecuteScalar(@$"UPDATE AQENT00F1 SET EMENBF = @DDT, EMEDBF = @DDTData, EMEQTC = @Quantita WHERE emsoci = @CompanyID AND EMCDFO = @FornitoreID AND EMEDBF = @DDTDataOld AND EMENBF = @DDTOld AND EMEPEM = @Riga", new
                {
                    CompanyID = Entrance.SocietaID,
                    FornitoreID = Entrance.FornitoreID,
                    DDTDataOld = Entrance.Data,
                    DDTOld = Entrance.DDTID,
                    Riga = Entrance.Riga,
                    DDTData = Row.DDTData,
                    DDT = Row.DDTID,
                    Quantita = Row.Quantita,
                }, transaction);

                // ORDINE ACQUISTO
                var ordine = connection.QueryFirstOrDefault<AQORD00F>(@"SELECT * FROM AQORD00F WHERE otsoco = @Societa  AND OTANNO = @Anno   AND OTNORP = @OrdineID AND ODNUPP = @Riga",
                    new
                    {
                        Societa = Entrance.OrdineSocietaID,
                        Anno = Entrance.OrdineAnno,
                        OrdineID = Entrance.OrdineID,
                        Riga = Entrance.OrdineRiga
                    },
                    transaction
                );

                if (ordine != null)
                {
                    // Update ordine
                    connection.Execute(@"UPDATE AQORD00F SET ODQTOR = @Quantita,   ODIMP2 = @Prezzo  WHERE otsoco = @Societa  AND OTANNO = @Anno  AND OTNORP = @OrdineID    AND ODNUPP = @Riga",
                        new
                        {
                            Quantita = Row.Quantita,
                            Prezzo = Row.Prezzo,
                            Societa = ordine.otsoco,
                            Anno = ordine.OTANNO,
                            OrdineID = ordine.OTNORP,
                            Riga = ordine.ODNUPP
                        },
                        transaction
                    );

                    // RIPULISCE COSTI (LEVEL1)
                    var cosd = connection.QueryFirstOrDefault<COSTM00FLEVEL1>(@"SELECT * FROM COSTM00FLEVEL1 WHERE cmsoci = @Societa AND CMANMA = @Anno AND CMNUMA = @Numero  AND CMARTI = @Articolo AND CMTIPO = 'E'",
                        new
                        {
                            Societa = ordine.otsoco,
                            Anno = ordine.OTANNO,
                            Numero = ordine.ODNORD,
                            Articolo = ordine.ODCOAR
                        },
                        transaction
                    );

                    if (cosd != null)
                    {
                        // Delete level1
                        connection.Execute(@"DELETE FROM COSTM00FLEVEL1 WHERE cmsoci = @cmsoci AND CMARTI = @CMARTI AND CMANNO = @CMANNO AND CMMESE = @CMMESE AND CMPROG = @CMPROG",
                            cosd,
                            transaction
                        );

                        // Update COSTM00F
                        connection.Execute(@"UPDATE COSTM00F SET CMQENT = CMQENT - @Qta, CMVENT = CMVENT - @Prezzo  WHERE cmsoci = @Societa  AND CMARTI = @Articolo AND CMANNO = @Anno AND CMMESE = @Mese",
                            new
                            {
                                Qta = cosd.CMQTA,
                                Prezzo = cosd.CMPREZ,
                                Societa = cosd.cmsoci,
                                Articolo = cosd.CMARTI,
                                Anno = cosd.CMANNO,
                                Mese = cosd.CMMESE
                            },
                            transaction
                        );
                    }

                    // RIGENERA COSTI (COSTM00F)
                    var costn = connection.QueryFirstOrDefault<COSTM00F>(
                        @"SELECT *  FROM COSTM00F  WHERE cmsoci = @Societa  AND CMARTI = @Articolo AND CMANNO = @Anno AND CMMESE = @Mese",
                        new
                        {
                            Societa = Entrance.SocietaID,
                            Articolo = Entrance.ArticoloID,
                            Anno = Row.DDTData.Year,
                            Mese = Row.DDTData.Month
                        },
                        transaction
                    );

                    if (costn == null)
                    {
                        connection.Execute( @"INSERT INTO COSTM00F (cmsoci, CMARTI, CMANNO, CMMESE, CMQENT, CMVENT,
                     CMCOSM, CMCOSU, CMCOSS,
                     CMFL01, CMFL02, CMFL03, CMFL04, CMFL05,
                     CMACQ0, CMACQ2, CMACQ3, CMACQ4, CMACQ5,
                     CMACQ6, CMACQ7, CMACQ8, CMACQ9,
                     CMVAL3, CMVAL4, CMVAL6, CMVAL7,
                     CMCOME, CMCOSE, CMCOUE)
                    VALUES
                    (@Societa, @Articolo, @Anno, @Mese, @Qta, @Totale,
                     0,0,0,
                     '', '', '', '', '',
                     0,0,0,0,0,
                     0,0,0,0,
                     0,0,0,0,
                     0,0,0)",
                            new
                            {
                                Societa = Entrance.SocietaID,
                                Articolo = Entrance.ArticoloID,
                                Anno = Row.DDTData.Year,
                                Mese = Row.DDTData.Month,
                                Qta = Row.Quantita,
                                Totale = Row.Totale
                            },
                            transaction
                        );
                    }
                    else
                    {
                        connection.Execute(
                            @"UPDATE COSTM00F
                      SET CMQENT = CMQENT + @Qta,
                          CMVENT = CMVENT + @Totale
                      WHERE cmsoci = @Societa
                        AND CMARTI = @Articolo
                        AND CMANNO = @Anno
                        AND CMMESE = @Mese",
                            new
                            {
                                Qta = Row.Quantita,
                                Totale = Row.Totale,
                                Societa = Entrance.SocietaID,
                                Articolo = Entrance.ArticoloID,
                                Anno = Row.DDTData.Year,
                                Mese = Row.DDTData.Month
                            },
                            transaction
                        );
                    }

                    // ============================
                    // COSTM00FLEVEL1 (progressivo)
                    // ============================
                    var prog = connection.ExecuteScalar<short?>(
                        @"SELECT MAX(CMPROG)
                  FROM COSTM00FLEVEL1
                  WHERE cmsoci = @Societa
                    AND CMARTI = @Articolo
                    AND CMANNO = @Anno
                    AND CMMESE = @Mese",
                        new
                        {
                            Societa = Entrance.SocietaID,
                            Articolo = Entrance.ArticoloID,
                            Anno = Row.DDTData.Year,
                            Mese = Row.DDTData.Month
                        },
                        transaction
                    ) ?? 0;

                    prog++;

                    connection.Execute(
                        @"INSERT INTO COSTM00FLEVEL1
                (cmsoci, CMARTI, CMANNO, CMMESE, CMPROG,
                 CMANMA, CMNUMA, CMTIPO,
                 CMQTA, CMPREZ, CMSCO1, CMSCO2, CMSCO3)
                VALUES
                (@Societa, @Articolo, @Anno, @Mese, @Prog,
                 @OrdAnno, @OrdNum, 'E',
                 @Qta, @Prezzo, 0, 0, 0)",
                        new
                        {
                            Societa = Entrance.SocietaID,
                            Articolo = Entrance.ArticoloID,
                            Anno = Row.DDTData.Year,
                            Mese = Row.DDTData.Month,
                            Prog = prog,
                            OrdAnno = ordine.OTANNO,
                            OrdNum = ordine.ODNORD ?? 0,
                            Qta = Row.Quantita,
                            Prezzo = Row.Prezzo
                        },
                        transaction
                    );
                }
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        return true;

    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPLEVEL2 (fattsoc,fattnum,fattdata,fattpiva,fattriga,fattartdes,fattprz,fattqta,fatttotriga,fattgrup,fattcont,fattsott,fattumi,fattaliriga,fattnatu) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattriga,@fattartdes,@fattprz,@fattqta,@fatttotriga,@fattgrup,@fattcont,@fattsott,@fattumi,@fattaliriga,@fattnatu)";
    public string UPDATE_QUERY => "UPDATE FATTIMPLEVEL2 SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattriga = @fattriga,fattartdes = @fattartdes,fattprz = @fattprz,fattqta = @fattqta,fatttotriga = @fatttotriga,fattgrup = @fattgrup,fattcont = @fattcont,fattsott = @fattsott,fattumi = @fattumi,fattaliriga = @fattaliriga,fattnatu = @fattnatu,id = @id OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattriga = @fattriga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPLEVEL2 OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattriga = @fattriga AND rv = @rv";

    public bool Insert(ACC_EINVOICE_ROWS Model)
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

    public bool Update(ACC_EINVOICE_ROWS Model)
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

    public bool Delete(ACC_EINVOICE_ROWS Model)
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
    #endregion
}