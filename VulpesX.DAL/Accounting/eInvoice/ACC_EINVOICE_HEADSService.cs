

using CerberoRetrieveAPI;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Models.Accounting.eInvoice;
using VulpesX.Services.Tables.Accounting;
using SdIItemGX = VulpesX.Models.Models.Accounting.eInvoice.SdIItemGX;

namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_HEADSRepository
{
    ObservableCollection<ACC_EINVOICE_HEADS>? GetList(string CompanyID, DateTime Period, string Direction);

    ObservableCollection<ACC_EINVOICE_HEADS>? GetList(string CompanyID, int Year, int SupplierID);

    ACC_EINVOICE_HEADS? Get(long id);

    ACC_EINVOICE_HEADS? GetByAccountingRegistration(int SupplierID, string ReferenceID);

    ACC_EINVOICE_HEADS? GetFull(long id);

    bool Exists(string fattsoc, string fattnum, DateTime fattdata, string fattpiva, string fattdire);

    #region CRUD
    // ATTENZIONE! @fattdatasca va rimesso a NOW automatico in INSERT ad ogni modifica
    //             INSERTED.id invece di .rv in INSERT
    //             togliere id da INSERT ed UPDATE perchè è IDENTITY  
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    long? Insert(ACC_EINVOICE_HEADS Model);

    int Insert(SdIItemVulpesX Model, string CompanyID, string UserID, string Direction);

    int InsertLocal(SdIItemGX Model, short AttachmentsCount, string CompanyID, string UserID, string Direction);

    bool Update(ACC_EINVOICE_HEADS Model);

    bool Delete(ACC_EINVOICE_HEADS Model, Guid? CompanyGuid);

    #endregion
}

public class ACC_EINVOICE_HEADSRepository : RepositoryBase, IACC_EINVOICE_HEADSRepository
{
    public ACC_EINVOICE_HEADSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_HEADS>? GetList(string CompanyID, DateTime Period, string Direction)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_HEADS, ABE, ABE, CAUCONT, FE_RFIDOC, FE_TIPODOC, ISO, ACC_EINVOICE_HEADS>(
                @"SELECT h.*, a.abecod, a.abers1, a.abers2, ac.abecod, ac.abers1, ac.abers2, c.caucod, c.caudes, reg.regicod, reg.regides, td.FETDCod, td.FETDDes, iso.isocod, iso.isodes FROM ACC_EINVOICE_HEADS AS h
                        LEFT JOIN ABE AS a ON a.abecod=h.fattfor
                        LEFT JOIN ABE AS ac ON ac.abecod=h.fattcli
                        LEFT JOIN CAUCONT AS c ON c.caucod=h.fattcaus
                        LEFT JOIN FE_RFIDOC AS reg ON reg.regicod=h.fattregi
                        LEFT JOIN FE_TIPODOC AS td ON td.FETDCod=h.FATTTIPODOC
                        LEFT JOIN ISO AS iso ON iso.isocod=h.fattiso
                        WHERE h.fattsoc=@cid AND MONTH(fattdataric) = MONTH(@fattdata) AND YEAR(fattdataric) = YEAR(@fattdata) AND fattdire=@dire
                        ORDER BY h.fattdataric DESC",
                (hea, abe, abec, cau, rfi, tid, iso) => { hea.Supplier = abe; hea.Customer = abec; hea.Causal = cau; hea.TaxRegime = rfi; hea.DocumentType = tid; hea.ISO = iso; return hea; },
                new { cid = CompanyID, fattdata = Period, dire = Direction }, splitOn: "abecod,abecod,caucod,regicod,FETDCod, isocod");

            // check to sync with ABE
            if (Direction == "R")
            {
                foreach (var item in list.Where(w => !w.fattfor.HasValue))
                {
                    var supplierVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(item.fattpiva, "F");
                    if (supplierVAT != null)
                    {
                        item.fattfor = supplierVAT.abecod;
                        item.Supplier = supplierVAT;
                        Update(item);
                    }
                }
            }
            else
            {
                foreach (var item in list.Where(w => !w.fattcli.HasValue))
                {
                    var customerVAT = !string.IsNullOrEmpty(item.fattclipiva) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(item.fattclipiva, "C") : null;
                    if (customerVAT != null)
                    {
                        item.fattcli = customerVAT.abecod;
                        item.Customer = customerVAT;
                        Update(item);
                    }
                    else
                    {
                        var customerCF = !string.IsNullOrEmpty(item.fattclicf) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(item.fattclicf, "C") : null;
                        if (customerCF != null)
                        {
                            item.fattcli = customerCF.abecod;
                            item.Customer = customerCF;
                            Update(item);
                        }
                    }
                }
            }

            return new ObservableCollection<ACC_EINVOICE_HEADS>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_HEADS>? GetList(string CompanyID, int Year, int SupplierID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_HEADS, ABE, ABE, CAUCONT, FE_RFIDOC, FE_TIPODOC, ISO, ACC_EINVOICE_HEADS>(
                @"SELECT h.*, a.abecod, a.abers1, a.abers2, ac.abecod, ac.abers1, ac.abers2, c.caucod, c.caudes, reg.regicod, reg.regides, td.FETDCod, td.FETDDes, iso.isocod, iso.isodes FROM ACC_EINVOICE_HEADS AS h
                        LEFT JOIN ABE AS a ON a.abecod=h.fattfor
                        LEFT JOIN ABE AS ac ON ac.abecod=h.fattcli
                        LEFT JOIN CAUCONT AS c ON c.caucod=h.fattcaus
                        LEFT JOIN FE_RFIDOC AS reg ON reg.regicod=h.fattregi
                        LEFT JOIN FE_TIPODOC AS td ON td.FETDCod=h.FATTTIPODOC
                        LEFT JOIN ISO AS iso ON iso.isocod=h.fattiso
                        WHERE h.fattsoc=@cid AND YEAR(fattdata) = @Year AND h.fattfor =@SupplierID
                        ORDER BY h.fattdataric DESC",
                (hea, abe, abec, cau, rfi, tid, iso) => { hea.Supplier = abe; hea.Customer = abec; hea.Causal = cau; hea.TaxRegime = rfi; hea.DocumentType = tid; hea.ISO = iso; return hea; },
                new { cid = CompanyID, Year,SupplierID }, splitOn: "abecod,abecod,caucod,regicod,FETDCod, isocod");

            return new ObservableCollection<ACC_EINVOICE_HEADS>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_HEADS? Get(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_HEADS>(
                @"SELECT * FROM ACC_EINVOICE_HEADS 
                    WHERE id = @id",
                new { id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_HEADS? GetByAccountingRegistration(int SupplierID, string ReferenceID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_HEADS>(
                @"SELECT * FROM ACC_EINVOICE_HEADS 
                    WHERE fattnum = @fattnum AND fattfor = @fattfor",
                new { fattnum = ReferenceID, fattfor = SupplierID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_HEADS? GetFull(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();

            var result = connection.Query<ACC_EINVOICE_HEADS, ABE, CAUCONT, FE_TIPODOC, FE_RFIDOC, ISO, ACC_EINVOICE_HEADS>(
                @"SELECT h.*, a.abecod, a.abers1, a.abers2, c.caucod, c.caudes, fet.FETDCod, fet.FETDDes, fer.regicod, fer.regides, i.isocod, i.isodes FROM ACC_EINVOICE_HEADS AS h
                        LEFT JOIN ABE AS a ON a.abecod=h.fattfor
                        LEFT JOIN CAUCONT AS c ON c.caucod=h.fattcaus
                        LEFT JOIN FE_TIPODOC AS fet ON fet.FETDCod=h.FATTTIPODOC
                        LEFT JOIN FE_RFIDOC AS fer ON fer.regicod=h.fattregi
                        LEFT JOIN ISO AS i ON i.isocod=h.fattiso
                        WHERE h.id=@id",
                (hea, abe, cau, fet, fer, iso) =>
                {
                    hea.Supplier = abe;
                    hea.Causal = cau;
                    hea.DocumentType = fet;
                    hea.TaxRegime = fer;
                    hea.ISO = iso;
                    return hea;
                },
                new { id = id }, splitOn: "abecod, caucod, FETDCod, regicod, isocod")
                .FirstOrDefault();

            // Rows

            if (result != null)
            {
                result.Rows = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().GetList(id);
                // Rows - load rate taken by product
                foreach (var row in result.Rows ?? new ObservableCollection<ACC_EINVOICE_ROWS>())
                {
                    tab_articolo? product = null;
                    foreach (var pid in row.PIDs ?? new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>())
                    {
                        product = !string.IsNullOrEmpty(pid.artvalcod) ? tabArticoloRepo.GetSingle(row.fattsoc, pid.artvalcod) : null;
                        if (product != null)
                            break;
                    }

                    row.Product = product;
                    row.Rate = (product != null && !string.IsNullOrEmpty(product.asscod) && !string.IsNullOrEmpty(product.assali)) ? VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(product.asscod, product.assali) : null;
                }
                // SMs
                result.SMs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_SMRepository>().GetList(id);
                // VATs
                result.VATs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_VATRepository>().GetList(id);
                // Expires
                result.Expires = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_EXPIRESRepository>().GetList(id);
                // DDTs
                result.DDTs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().GetList(id);
                // POs
                result.POs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().GetList(id);
                // Linkeds
                result.Linkeds = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().GetList(id);
                // CPs
                result.CPs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_CPRepository>().GetList(id);
                // RITs
                result.RITs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_RITRepository>().GetList(id);
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string fattsoc, string fattnum, DateTime fattdata, string fattpiva, string fattdire)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM ACC_EINVOICE_HEADS 
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattdire = @fattdire",
                new { fattsoc = fattsoc, fattnum = fattnum, fattdata = fattdata, fattpiva = fattpiva, fattdire = fattdire }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    // ATTENZIONE! @fattdatasca va rimesso a NOW automatico in INSERT ad ogni modifica
    //             INSERTED.id invece di .rv in INSERT
    //             togliere id da INSERT ed UPDATE perchè è IDENTITY  
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_HEADS (fattsoc,fattnum,fattdata,fattpiva,fattabers1,fattfor,fattimpo,fattimposta,fatttot,fattannoreg,fattnumreg,fattdataric,fattnomefileric,fattcaus,fattcausaledes,fattregi,fattpec,fattcoddest,fattiso,fattimpobol,fattfornazi,fattforcap,fattforloca,fattforinde,fattforprov,fattclicap,fattclinaz,fattclipro,fattcliloca,fattcliinde,fattclipiva,fattstampa,fattarrotondamento,FATTTIPODOC,fattidsdi,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,fattclirags,fattdire,fattdatasca,accounted,accounted_UserID,fattcli,fattclicf,fattalle) OUTPUT INSERTED.id VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattabers1,@fattfor,@fattimpo,@fattimposta,@fatttot,@fattannoreg,@fattnumreg,@fattdataric,@fattnomefileric,@fattcaus,@fattcausaledes,@fattregi,@fattpec,@fattcoddest,@fattiso,@fattimpobol,@fattfornazi,@fattforcap,@fattforloca,@fattforinde,@fattforprov,@fattclicap,@fattclinaz,@fattclipro,@fattcliloca,@fattcliinde,@fattclipiva,@fattstampa,@fattarrotondamento,@FATTTIPODOC,@fattidsdi,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@fattclirags,@fattdire,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@accounted,@accounted_UserID,@fattcli,@fattclicf,@fattalle)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_HEADS SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattabers1 = @fattabers1,fattfor = @fattfor,fattimpo = @fattimpo,fattimposta = @fattimposta,fatttot = @fatttot,fattannoreg = @fattannoreg,fattnumreg = @fattnumreg,fattdataric = @fattdataric,fattnomefileric = @fattnomefileric,fattcaus = @fattcaus,fattcausaledes = @fattcausaledes,fattregi = @fattregi,fattpec = @fattpec,fattcoddest = @fattcoddest,fattiso = @fattiso,fattimpobol = @fattimpobol,fattfornazi = @fattfornazi,fattforcap = @fattforcap,fattforloca = @fattforloca,fattforinde = @fattforinde,fattforprov = @fattforprov,fattclicap = @fattclicap,fattclinaz = @fattclinaz,fattclipro = @fattclipro,fattcliloca = @fattcliloca,fattcliinde = @fattcliinde,fattclipiva = @fattclipiva,fattstampa = @fattstampa,fattarrotondamento = @fattarrotondamento,FATTTIPODOC = @FATTTIPODOC,fattidsdi = @fattidsdi,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,fattclirags = @fattclirags,fattdire = @fattdire,fattdatasca = @fattdatasca,accounted = @accounted,accounted_UserID = @accounted_UserID,fattcli = @fattcli,fattclicf = @fattclicf,fattalle = @fattalle OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_HEADS OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public long? Insert(ACC_EINVOICE_HEADS Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (long?)connection.ExecuteScalar(INSERT_QUERY, Model);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public int Insert(SdIItemVulpesX Model, string CompanyID, string UserID, string Direction)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    int counter = 0;
                    var culture = CultureInfo.CreateSpecificCulture("it-IT");
                    foreach (var invoice in Model.FullDocumentDecoded.Fatture)
                    {
                        var fattdata = DateTime.Parse(invoice.Data, culture);
                        var fattpiva = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA) ? Model.FullDocumentDecoded.CedentePrestatorePartitaIVA : Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale;
                        // check already exists
                        if (Exists(CompanyID, invoice.Numero, fattdata, fattpiva, Direction))
                        {

                            ErrorHandler.Show($"Il documento {invoice.Numero} del {fattdata.ToString("dd/MM/yyyy")} cedente/prestatore {fattpiva} è già presente");

                            return 0;
                        }
                        // head
                        var supplierVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA, "F");
                        var supplierCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale, "F") : null;
                        var customerVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA, "C");
                        var customerCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale, "C") : null;
                        var tdoc = VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Get(invoice.Tipo);
                        var head = new ACC_EINVOICE_HEADS()
                        {
                            fattsoc = CompanyID,
                            fattnum = invoice.Numero,
                            fattdata = fattdata,
                            fattpiva = fattpiva,
                            fattabers1 = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatoreDenominazione) ? Model.FullDocumentDecoded.CedentePrestatoreDenominazione : $"{Model.FullDocumentDecoded.CedenteCognome} {Model.FullDocumentDecoded.CedenteNome}",
                            fattfor = supplierVAT != null ? supplierVAT.abecod : supplierCF?.abecod,
                            fattcli = customerVAT != null ? customerVAT.abecod : customerCF?.abecod,
                            fattimpo = invoice.DatiRiepilogo?.Sum(sum => sum.Imponibile),
                            fattimposta = invoice.DatiRiepilogo?.Sum(sum => sum.Imposta),
                            fatttot = invoice.ImportoTotaleDocumento,
                            fattdataric = DateTime.Parse(Model.DataRicezione, culture),
                            fattnomefileric = Model.FileName,
                            fattcaus = tdoc?.FETDACQC,
                            fattcausaledes = invoice.Causali?.FirstOrDefault(),
                            fattregi = Model.FullDocumentDecoded.CedentePrestatoreRegimeFiscale,
                            fattpec = Model.FullDocumentDecoded.PECDestinatario,
                            fattcoddest = Model.FullDocumentDecoded.CodiceDestinatario,
                            fattiso = Model.FullDocumentDecoded.CedentePrestatoreSedeNazione,
                            fattimpobol = invoice.DatiBolloImporto,
                            fattfornazi = Model.FullDocumentDecoded.CedentePrestatorePaese,
                            fattforcap = Model.FullDocumentDecoded.CedentePrestatoreSedeCap,
                            fattforloca = Model.FullDocumentDecoded.CedentePrestatoreSedeComune,
                            fattforinde = $"{Model.FullDocumentDecoded.CedentePrestatoreSedeIndirizzo}, {Model.FullDocumentDecoded.CedentePrestatoreSedeNumeroCivico}",
                            fattforprov = Model.FullDocumentDecoded.CedentePrestatoreSedeProvincia,
                            fattclicap = Model.FullDocumentDecoded.CessionarioCommittenteSedeCap,
                            fattclinaz = Model.FullDocumentDecoded.CessionarioCommittenteSedeNazione,
                            fattclipro = Model.FullDocumentDecoded.CessionarioCommittenteSedeProvincia,
                            fattcliloca = Model.FullDocumentDecoded.CessionarioCommittenteSedeComune,
                            fattcliinde = $"{Model.FullDocumentDecoded.CessionarioCommittenteSedeIndirizzo}, {Model.FullDocumentDecoded.CessionarioCommittenteSedeNumeroCivico}",
                            fattclipiva = Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA,
                            fattclirags = Model.FullDocumentDecoded.CessionarioCommittenteDenominazione,
                            fattarrotondamento = invoice.Arrotondamento,
                            FATTTIPODOC = invoice.Tipo,
                            fattidsdi = Model.SdIID,
                            fattdire = Direction,
                            addedUserID = UserID,
                            fattalle = Model.AttachmentsCount
                        };

                        var newID = (long?)connection.ExecuteScalar(INSERT_QUERY, head, transaction) ?? 0;

                        if (newID > 0)
                        {
                            head.id = newID;
                            // rows
                            int rowCounter = 1;
                            foreach (var item in invoice.RigheFattura)
                            {
                                var row = new ACC_EINVOICE_ROWS()
                                {
                                    id = newID,
                                    fattsoc = head.fattsoc,
                                    fattnum = head.fattnum,
                                    fattdata = head.fattdata,
                                    fattpiva = head.fattpiva,
                                    fattriga = rowCounter++,
                                    fattartdes = item.Descrizione,
                                    fattprz = item.PrezzoUnitario,
                                    fattqta = item.Quantita,
                                    fatttotriga = item.PrezzoTotale,
                                    fattumi = item.UnitaMisura,
                                    fattaliriga = item.AliquotaIVA.ToString(),
                                    fattnatu = item.Natura
                                };
                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().INSERT_QUERY, row, transaction);
                                // PIDS
                                if (item.CodiciArticolo != null)
                                {
                                    int pidsCounter = 1;
                                    foreach (var pid in item.CodiciArticolo)
                                    {
                                        var pi = new ACC_EINVOICE_ROWS_PIDS()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattriga = row.fattriga,
                                            artprog = pidsCounter++,
                                            artvalcod = pid.CodiceValore,
                                            artipcod = pid.CodiceTipo
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_PIDRepository>().INSERT_QUERY, pi, transaction);
                                    }
                                }
                                // SM
                                if (item.ScontiMaggiorazioni != null)
                                {
                                    int smsCounter = 1;
                                    foreach (var sms in item.ScontiMaggiorazioni)
                                    {
                                        var sm = new ACC_EINVOICE_ROWS_SM()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattriga = row.fattriga,
                                            Progsc = smsCounter++,
                                            sctipo = sms.Tipo,
                                            scimpo = sms.Importo,
                                            scperc = sms.Percentuale
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_SMRepository>().INSERT_QUERY, sm, transaction);
                                    }
                                }
                            }
                            // VAT
                            if (invoice.DatiRiepilogo != null)
                            {
                                int vatCounter = 1;
                                foreach (var item in invoice.DatiRiepilogo)
                                {
                                    var vat = new ACC_EINVOICE_VAT()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        fattprog = vatCounter++,
                                        Fattaliq = item.Aliquota.ToString().Length > 2 ? item.Aliquota.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.Aliquota.ToString().Trim(),
                                        fattimpodett = item.Imponibile,
                                        fattimpostadett = item.Imposta,
                                        fattesi = item.Esigibilita,
                                        fattrifenorm = item.RiferimentoNormativo,
                                        fattnatu = item.Natura,
                                        fattarrotondamento = item.Arrotondamento
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_VATRepository>().INSERT_QUERY, vat, transaction);
                                }
                            }
                            // purchase orders
                            if (invoice.DatiOrdiniAcquisto != null)
                            {
                                int poCounter = 1;
                                foreach (var item in invoice.DatiOrdiniAcquisto)
                                {
                                    if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                    {
                                        foreach (var rif in item.RiferimentoLinea)
                                        {
                                            var po = new ACC_EINVOICE_PO()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                riga = poCounter++,
                                                numitem = item.NumItem,
                                                iddocumento = item.IdDocumento,
                                                datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                rigarife = (int)rif
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                        }
                                    }
                                    else
                                    {
                                        var po = new ACC_EINVOICE_PO()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            riga = poCounter++,
                                            numitem = item.NumItem,
                                            iddocumento = item.IdDocumento,
                                            datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                            rigarife = null
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                    }
                                }
                            }
                            // SM
                            if (invoice.ScontiMaggiorazioni != null)
                            {
                                int smCounter = 1;
                                foreach (var item in invoice.ScontiMaggiorazioni)
                                {
                                    var sm = new ACC_EINVOICE_SM()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        sctprog = smCounter++,
                                        scttipo = item.Tipo,
                                        sctimpo = item.Importo,
                                        sctperc = item.Percentuale
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_SMRepository>().INSERT_QUERY, sm, transaction);
                                }
                            }
                            // CP
                            if (invoice.DatiCassaPrevidenziale != null)
                            {
                                int cpCounter = 1;
                                foreach (var item in invoice.DatiCassaPrevidenziale)
                                {
                                    var cp = new ACC_EINVOICE_CP()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        progcassa = cpCounter++,
                                        tipocassa = item.TipoCassa,
                                        alicassa = item.AlCassa.HasValue ? (item.AlCassa?.ToString().Length > 2 ? item.AlCassa?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AlCassa?.ToString().Trim()) : null,
                                        impcontricassa = item.ImportoContributoCassa,
                                        impocassa = item.ImponibileCassa,
                                        aliivacassa = item.AliquotaIVA.HasValue ? (item.AliquotaIVA?.ToString().Length > 2 ? item.AliquotaIVA?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AliquotaIVA?.ToString().Trim()) : null,
                                        ritecassa = item.Ritenuta,
                                        natcassa = item.Natura,
                                        rifammicassa = item.RiferimentoAmministrazione
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_CPRepository>().INSERT_QUERY, cp, transaction);
                                }
                            }
                            // Ritenuta
                            if (invoice.DatiRitenuta != null)
                            {
                                int ritCounter = 1;
                                foreach (var item in invoice.DatiRitenuta)
                                {
                                    var rit = new ACC_EINVOICE_RIT()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        progrit = ritCounter++,
                                        tiporit = item.Tipo,
                                        importo = item.Importo,
                                        aliquota = item.Aliquota,
                                        causalepagamento = item.CausalePagamento
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_RITRepository>().INSERT_QUERY, rit, transaction);
                                }
                            }
                            // DDT
                            if (invoice.DatiDDT != null)
                            {
                                int ddtCounter = 1;
                                foreach (var item in invoice.DatiDDT)
                                {
                                    if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                    {
                                        foreach (var rif in item.RiferimentoLinea)
                                        {
                                            var ddt = new ACC_EINVOICE_DDT()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                ddtriga = ddtCounter++,
                                                ddtdata = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                ddtnum = item.Numero,
                                                ddtriferiga = (int)rif
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                        }
                                    }
                                    else
                                    {
                                        var ddt = new ACC_EINVOICE_DDT()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            ddtriga = ddtCounter++,
                                            ddtdata = DateTime.Parse(item.Data, culture),
                                            ddtnum = item.Numero,
                                            ddtriferiga = null
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                    }
                                }
                            }
                            // expires
                            if (invoice.Pagamenti != null)
                            {
                                foreach (var item in invoice.Pagamenti)
                                {
                                    short seconds = 1;
                                    foreach (var exp in item.DettagliPagamento)
                                    {
                                        var pay = VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Get(exp.Modalita);
                                        var expireDate = !string.IsNullOrWhiteSpace(exp.DataScadenza) ? DateTime.Parse(exp.DataScadenza, culture) : head.fattdata;
                                        var expire = new ACC_EINVOICE_EXPIRES()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattdatascad = expireDate.AddSeconds(seconds++),
                                            fattimpscad = exp.Importo,
                                            fattipopag = exp.Modalita,
                                            fattiban = exp.IBAN,
                                            fattistu = exp.IstitutoFinanziario,
                                            fattabi = exp.ABI,
                                            fattcab = exp.CAB,
                                            fattcond = item.Condizioni,
                                            FATTTIPOLPAG = pay?.FEPATIPP,
                                            FATTCODPAGV = exp.CodicePagamento
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_EXPIRESRepository>().INSERT_QUERY, expire, transaction);
                                    }
                                }
                            }
                            // linked
                            if (invoice.DatiFattureCollegate != null)
                            {
                                int lnkCounter = 1;
                                foreach (var item in invoice.DatiFattureCollegate)
                                {
                                    if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                    {
                                        foreach (var rif in item.RiferimentoLinea)
                                        {
                                            var linked = new ACC_EINVOICE_LINKED()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattcollriga = lnkCounter++,
                                                fattcollnumitem = item.NumItem,
                                                fattcolliddocumento = item.IdDocumento,
                                                fattcolldatadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                fattcollriferiga = (int)rif
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                        }
                                    }
                                    else
                                    {
                                        var linked = new ACC_EINVOICE_LINKED()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattcollriga = lnkCounter++,
                                            fattcollnumitem = item.NumItem,
                                            fattcolliddocumento = item.IdDocumento,
                                            fattcolldatadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                            fattcollriferiga = null
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                    }
                                }
                            }
                            counter++;
                        }
                        else
                        {

                            ErrorHandler.Show($"Impossibile inserire il documento, errore XGE-001");

                            return 0;
                        }
                    }
                    transaction.Commit();
                    return counter;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();

                    ErrorHandler.Show(exc.Message);

                    return 0;
                }
            }

        }
        catch (Exception ex)
        {

            ErrorHandler.Show(ex.Message);

            return 0;
        }
    }

    public int InsertLocal(SdIItemGX Model, short AttachmentsCount, string CompanyID, string UserID, string Direction)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    int counter = 0;
                    var culture = CultureInfo.CreateSpecificCulture("it-IT");

                    if (Model.FullDocumentDecoded != null)
                    {
                        foreach (var invoice in Model.FullDocumentDecoded.Fatture ?? new List<Models.Models.Accounting.eInvoice.FullDocumentItem>())
                        {
                            var fattdata = DateTime.Parse(invoice.Data ?? string.Empty, culture);
                            var fattpiva = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA) ? Model.FullDocumentDecoded.CedentePrestatorePartitaIVA : Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale;
                            // check already exists
                            if (Exists(CompanyID, invoice.Numero ?? string.Empty, fattdata, fattpiva ?? string.Empty, Direction))
                            {
                                ErrorHandler.Show($"Il documento {invoice.Numero} del {fattdata.ToString("dd/MM/yyyy")} cedente/prestatore {fattpiva} è già presente");
                                return 0;
                            }
                            // head
                            var supplierVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA ?? string.Empty, "F");
                            var supplierCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale, "F") : null;
                            var customerVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA ?? string.Empty, "C");
                            var customerCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale, "C") : null;
                            var tdoc = VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Get(invoice.Tipo ?? string.Empty);
                            var head = new ACC_EINVOICE_HEADS()
                            {
                                fattsoc = CompanyID,
                                fattnum = invoice.Numero ?? string.Empty,
                                fattdata = fattdata,
                                fattpiva = fattpiva ?? string.Empty,
                                fattabers1 = Model.FullDocumentDecoded.CedentePrestatoreDenominazione,
                                fattfor = supplierVAT != null ? supplierVAT.abecod : supplierCF?.abecod,
                                fattcli = customerVAT != null ? customerVAT.abecod : customerCF?.abecod,
                                fattimpo = invoice.DatiRiepilogo?.Sum(sum => sum.Imponibile),
                                fattimposta = invoice.DatiRiepilogo?.Sum(sum => sum.Imposta),
                                fatttot = invoice.ImportoTotaleDocumento,
                                fattdataric = DateTime.Parse(Model.DataRicezione ?? string.Empty, culture),
                                fattnomefileric = Model.FileName,
                                fattcaus = tdoc?.FETDACQC,
                                fattcausaledes = invoice.Causali?.FirstOrDefault(),
                                fattregi = Model.FullDocumentDecoded.CedentePrestatoreRegimeFiscale,
                                fattpec = Model.FullDocumentDecoded.PECDestinatario,
                                fattcoddest = Model.FullDocumentDecoded.CodiceDestinatario,
                                fattiso = Model.FullDocumentDecoded.CedentePrestatoreSedeNazione,
                                fattimpobol = invoice.DatiBolloImporto,
                                fattfornazi = Model.FullDocumentDecoded.CedentePrestatorePaese,
                                fattforcap = Model.FullDocumentDecoded.CedentePrestatoreSedeCap,
                                fattforloca = Model.FullDocumentDecoded.CedentePrestatoreSedeComune,
                                fattforinde = $"{Model.FullDocumentDecoded.CedentePrestatoreSedeIndirizzo}, {Model.FullDocumentDecoded.CedentePrestatoreSedeNumeroCivico}",
                                fattforprov = Model.FullDocumentDecoded.CedentePrestatoreSedeProvincia,
                                fattclicap = Model.FullDocumentDecoded.CessionarioCommittenteSedeCap,
                                fattclinaz = Model.FullDocumentDecoded.CessionarioCommittenteSedeNazione,
                                fattclipro = Model.FullDocumentDecoded.CessionarioCommittenteSedeProvincia,
                                fattcliloca = Model.FullDocumentDecoded.CessionarioCommittenteSedeComune,
                                fattcliinde = $"{Model.FullDocumentDecoded.CessionarioCommittenteSedeIndirizzo}, {Model.FullDocumentDecoded.CessionarioCommittenteSedeNumeroCivico}",
                                fattclipiva = Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA,
                                fattclirags = Model.FullDocumentDecoded.CessionarioCommittenteDenominazione,
                                fattarrotondamento = invoice.Arrotondamento,
                                FATTTIPODOC = invoice.Tipo,
                                fattidsdi = Model.SdIID,
                                fattdire = Direction,
                                addedUserID = UserID,
                                fattalle = AttachmentsCount
                            };
                            var newID = (long?)connection.ExecuteScalar(INSERT_QUERY, head, transaction) ?? 0;
                            if (newID > 0)
                            {
                                // rows
                                int rowCounter = 1;
                                foreach (var item in invoice.RigheFattura ?? new List<Models.Models.Accounting.eInvoice.FullDocumentItemRow>())
                                {
                                    var row = new ACC_EINVOICE_ROWS()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        fattriga = rowCounter++,
                                        fattartdes = item.Descrizione,
                                        fattprz = item.PrezzoUnitario,
                                        fattqta = item.Quantita,
                                        fatttotriga = item.PrezzoTotale,
                                        fattumi = item.UnitaMisura,
                                        fattaliriga = item.AliquotaIVA.ToString(),
                                        fattnatu = item.Natura
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().INSERT_QUERY, row, transaction);
                                    // PIDS
                                    if (item.CodiciArticolo != null)
                                    {
                                        int pidsCounter = 1;
                                        foreach (var pid in item.CodiciArticolo)
                                        {
                                            var pi = new ACC_EINVOICE_ROWS_PIDS()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattriga = row.fattriga,
                                                artprog = pidsCounter++,
                                                artvalcod = pid.CodiceValore,
                                                artipcod = pid.CodiceTipo
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_PIDRepository>().INSERT_QUERY, pi, transaction);
                                        }
                                    }
                                    // SM
                                    if (item.ScontiMaggiorazioni != null)
                                    {
                                        int smsCounter = 1;
                                        foreach (var sms in item.ScontiMaggiorazioni)
                                        {
                                            var sm = new ACC_EINVOICE_ROWS_SM()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattriga = row.fattriga,
                                                Progsc = smsCounter++,
                                                sctipo = sms.Tipo,
                                                scimpo = sms.Importo,
                                                scperc = sms.Percentuale
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_SMRepository>().INSERT_QUERY, sm, transaction);
                                        }
                                    }
                                }
                                // VAT
                                if (invoice.DatiRiepilogo != null)
                                {
                                    int vatCounter = 1;
                                    foreach (var item in invoice.DatiRiepilogo)
                                    {
                                        var vat = new ACC_EINVOICE_VAT()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattprog = vatCounter++,
                                            Fattaliq = item.Aliquota.ToString().Length > 2 ? item.Aliquota.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.Aliquota.ToString().Trim(),
                                            fattimpodett = item.Imponibile,
                                            fattimpostadett = item.Imposta,
                                            fattesi = item.Esigibilita,
                                            fattrifenorm = item.RiferimentoNormativo,
                                            fattnatu = item.Natura
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_VATRepository>().INSERT_QUERY, vat, transaction);
                                    }
                                }
                                // purchase orders
                                if (invoice.DatiOrdiniAcquisto != null)
                                {
                                    int poCounter = 1;
                                    foreach (var item in invoice.DatiOrdiniAcquisto)
                                    {
                                        if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                        {
                                            foreach (var rif in item.RiferimentoLinea)
                                            {
                                                var po = new ACC_EINVOICE_PO()
                                                {
                                                    id = newID,
                                                    fattsoc = head.fattsoc,
                                                    fattnum = head.fattnum,
                                                    fattdata = head.fattdata,
                                                    fattpiva = head.fattpiva,
                                                    riga = poCounter++,
                                                    numitem = item.NumItem,
                                                    iddocumento = item.IdDocumento,
                                                    datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                    rigarife = (int)rif
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                            }
                                        }
                                        else
                                        {
                                            var po = new ACC_EINVOICE_PO()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                riga = poCounter++,
                                                numitem = item.NumItem,
                                                iddocumento = item.IdDocumento,
                                                datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                rigarife = null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                        }
                                    }
                                }
                                // SM
                                if (invoice.ScontiMaggiorazioni != null)
                                {
                                    int smCounter = 1;
                                    foreach (var item in invoice.ScontiMaggiorazioni)
                                    {
                                        var sm = new ACC_EINVOICE_SM()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            sctprog = smCounter++,
                                            scttipo = item.Tipo,
                                            sctimpo = item.Importo,
                                            sctperc = item.Percentuale
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_SMRepository>().INSERT_QUERY, sm, transaction);
                                    }
                                }
                                // CP
                                if (invoice.DatiCassaPrevidenziale != null)
                                {
                                    int cpCounter = 1;
                                    foreach (var item in invoice.DatiCassaPrevidenziale)
                                    {
                                        var cp = new ACC_EINVOICE_CP()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            progcassa = cpCounter++,
                                            tipocassa = item.TipoCassa,
                                            alicassa = item.AlCassa.HasValue ? (item.AlCassa?.ToString().Length > 2 ? item.AlCassa?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AlCassa?.ToString().Trim()) : null,
                                            impcontricassa = item.ImportoContributoCassa,
                                            impocassa = item.ImponibileCassa,
                                            aliivacassa = item.AliquotaIVA.HasValue ? (item.AliquotaIVA?.ToString().Length > 2 ? item.AliquotaIVA?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AliquotaIVA?.ToString().Trim()) : null,
                                            ritecassa = item.Ritenuta,
                                            natcassa = item.Natura,
                                            rifammicassa = item.RiferimentoAmministrazione
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_CPRepository>().INSERT_QUERY, cp, transaction);
                                    }
                                }
                                // Ritenuta
                                if (invoice.DatiRitenuta != null)
                                {
                                    int ritCounter = 1;
                                    foreach (var item in invoice.DatiRitenuta)
                                    {
                                        var rit = new ACC_EINVOICE_RIT()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            progrit = ritCounter++,
                                            tiporit = item.Tipo,
                                            importo = item.Importo,
                                            aliquota = item.Aliquota,
                                            causalepagamento = item.CausalePagamento
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_RITRepository>().INSERT_QUERY, rit, transaction);
                                    }
                                }
                                // DDT
                                if (invoice.DatiDDT != null)
                                {
                                    int ddtCounter = 1;
                                    foreach (var item in invoice.DatiDDT)
                                    {
                                        if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                        {
                                            foreach (var rif in item.RiferimentoLinea)
                                            {
                                                var ddt = new ACC_EINVOICE_DDT()
                                                {
                                                    id = newID,
                                                    fattsoc = head.fattsoc,
                                                    fattnum = head.fattnum,
                                                    fattdata = head.fattdata,
                                                    fattpiva = head.fattpiva,
                                                    ddtriga = ddtCounter++,
                                                    ddtdata = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                    ddtnum = item.Numero,
                                                    ddtriferiga = (int)rif
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                            }
                                        }
                                        else
                                        {
                                            var ddt = new ACC_EINVOICE_DDT()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                ddtriga = ddtCounter++,
                                                ddtdata = DateTime.Parse(item.Data ?? string.Empty, culture),
                                                ddtnum = item.Numero,
                                                ddtriferiga = null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                        }
                                    }
                                }
                                // expires
                                if (invoice.Pagamenti != null)
                                {
                                    foreach (var item in invoice.Pagamenti)
                                    {
                                        foreach (var exp in item.DettagliPagamento ?? new List<Models.Models.Accounting.eInvoice.FullDocumentItemPaymentDetail>())
                                        {
                                            var pay = VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Get(exp.Modalita ?? string.Empty);
                                            var expire = new ACC_EINVOICE_EXPIRES()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattdatascad = !string.IsNullOrWhiteSpace(exp.DataScadenza) ? DateTime.Parse(exp.DataScadenza, culture) : head.fattdata,
                                                fattimpscad = exp.Importo,
                                                fattipopag = exp.Modalita,
                                                fattiban = exp.IBAN,
                                                fattistu = exp.IstitutoFinanziario,
                                                fattabi = exp.ABI,
                                                fattcab = exp.CAB,
                                                fattcond = item.Condizioni,
                                                FATTTIPOLPAG = pay?.FEPATIPP,
                                                FATTCODPAGV = exp.CodicePagamento
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_EXPIRESRepository>().INSERT_QUERY, expire, transaction);
                                        }
                                    }
                                }
                                // linked
                                if (invoice.DatiFattureCollegate != null)
                                {
                                    int lnkCounter = 1;
                                    foreach (var item in invoice.DatiFattureCollegate)
                                    {
                                        if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                        {
                                            foreach (var rif in item.RiferimentoLinea)
                                            {
                                                var linked = new ACC_EINVOICE_LINKED()
                                                {
                                                    id = newID,
                                                    fattsoc = head.fattsoc,
                                                    fattnum = head.fattnum,
                                                    fattdata = head.fattdata,
                                                    fattpiva = head.fattpiva,
                                                    fattcollriga = lnkCounter++,
                                                    fattcollnumitem = item.NumItem,
                                                    fattcolliddocumento = item.IdDocumento,
                                                    fattcolldatadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                    fattcollriferiga = (int)rif
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                            }
                                        }
                                        else
                                        {
                                            var linked = new ACC_EINVOICE_LINKED()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattcollriga = lnkCounter++,
                                                fattcollnumitem = item.NumItem,
                                                fattcolliddocumento = item.IdDocumento,
                                                fattcolldatadoc = DateTime.Parse(item.Data ?? string.Empty, culture),
                                                fattcollriferiga = null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                        }
                                    }
                                }
                                counter++;
                            }
                            else
                            {

                                ErrorHandler.Show($"Impossibile inserire il documento, errore XGE-001");

                                return 0;
                            }
                        }
                    }
                    transaction.Commit();
                    return counter;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();

                    ErrorHandler.Show(exc.Message);

                    return 0;
                }
            }

        }
        catch (Exception ex)
        {

            ErrorHandler.Show(ex.Message);

            return 0;
        }
    }

    public bool Update(ACC_EINVOICE_HEADS Model)
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

    public bool Delete(ACC_EINVOICE_HEADS Model, Guid? CompanyGuid)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // heads
                    connection.Execute(DELETE_QUERY, Model, transaction);
                    // rows
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_ROWS
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // rows pids
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_ROWS_PIDS
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // rows sm
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_ROWS_SM
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // cp
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_CP
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // DDT
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_DDT
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // expires
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_EXPIRES
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // linked
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_LINKED
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // po
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_PO
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // sm
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_SM
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // VAT
                    connection.Execute(@"DELETE FROM ACC_EINVOICE_VAT
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);

                    // check if need to delete file on cloud
                    if (Model.fattdire == "S")
                    {
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyGuid}/{StorageHelper.INVOICE_EXTERNAL_SENT_FOLDER}{Model.fattnomefileric}");
                    }

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

    #endregion
}

public class ACC_EINVOICE_HEADSUfpRepository : RepositoryBase, IACC_EINVOICE_HEADSRepository
{
    public ACC_EINVOICE_HEADSUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_HEADS>? GetList(string CompanyID, DateTime Period, string Direction)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_HEADS, ABE, ABE, CAUCONT, FE_RFIDOC, FE_TIPODOC, ISO, ACC_EINVOICE_HEADS>(
                @"SELECT h.*, a.abecod, a.abers1, a.abers2, ac.abecod, ac.abers1, ac.abers2, c.caucod, c.caudes, reg.regicod, reg.regides, td.FETDCod, td.FETDDes, iso.isocod, iso.isodes FROM FATTIMP AS h
                        LEFT JOIN ANAG_BASE AS a ON a.abecod=h.fattfor
                        OUTER APPLY (
                            SELECT TOP 1 *
                            FROM ANAG_BASE
                            WHERE abepiv = h.fattclipiva
                            ORDER BY abecod  
                        ) ac
                        LEFT JOIN CAUCONT AS c ON c.caucod=h.fattcaus
                        LEFT JOIN REGIMIFISCALI AS reg ON reg.regicod=h.fattregi
                        LEFT JOIN FE_TIPODOC AS td ON td.FETDCod=h.FATTTIPODOC
                        LEFT JOIN ISO AS iso ON iso.isocod=h.fattiso
                        WHERE h.fattsoc=@cid AND MONTH(fattdataric) = MONTH(@fattdata) AND YEAR(fattdataric) = YEAR(@fattdata)
                        ORDER BY h.fattdataric DESC",
                (hea, abe, abec, cau, rfi, tid, iso) => { hea.Supplier = abe; hea.Customer = abec; hea.Causal = cau; hea.TaxRegime = rfi; hea.DocumentType = tid; hea.ISO = iso; return hea; },
                new { cid = CompanyID, fattdata = Period, dire = Direction }, splitOn: "abecod,abecod,caucod,regicod,FETDCod, isocod");

            // check to sync with ABE
            if (Direction == "R")
            {
                foreach (var item in list.Where(w => !w.fattfor.HasValue))
                {
                    var supplierVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(item.fattpiva, "F");
                    if (supplierVAT != null)
                    {
                        item.fattfor = supplierVAT.abecod;
                        item.Supplier = supplierVAT;
                        Update(item);
                    }
                }
            }
            else
            {
                foreach (var item in list.Where(w => !w.fattcli.HasValue))
                {
                    var customerVAT = !string.IsNullOrEmpty(item.fattclipiva) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(item.fattclipiva, "C") : null;
                    if (customerVAT != null)
                    {
                        item.fattcli = customerVAT.abecod;
                        item.Customer = customerVAT;
                        Update(item);
                    }
                    else
                    {
                        var customerCF = !string.IsNullOrEmpty(item.fattclicf) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(item.fattclicf, "C") : null;
                        if (customerCF != null)
                        {
                            item.fattcli = customerCF.abecod;
                            item.Customer = customerCF;
                            Update(item);
                        }
                    }
                }
            }

            return new ObservableCollection<ACC_EINVOICE_HEADS>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_HEADS>? GetList(string CompanyID, int Year, int SupplierID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_HEADS, ABE, ABE, CAUCONT, FE_RFIDOC, FE_TIPODOC, ISO, ACC_EINVOICE_HEADS>(
                @"SELECT h.*, a.abecod, a.abers1, a.abers2, ac.abecod, ac.abers1, ac.abers2, c.caucod, c.caudes, reg.regicod, reg.regides, td.FETDCod, td.FETDDes, iso.isocod, iso.isodes FROM FATTIMP AS h
                        LEFT JOIN ANAG_BASE AS a ON a.abecod=h.fattfor
                        OUTER APPLY (
                            SELECT TOP 1 *
                            FROM ANAG_BASE
                            WHERE abepiv = h.fattclipiva
                            ORDER BY abecod  
                        ) ac
                        LEFT JOIN CAUCONT AS c ON c.caucod=h.fattcaus
                        LEFT JOIN REGIMIFISCALI AS reg ON reg.regicod=h.fattregi
                        LEFT JOIN FE_TIPODOC AS td ON td.FETDCod=h.FATTTIPODOC
                        LEFT JOIN ISO AS iso ON iso.isocod=h.fattiso
                        WHERE h.fattsoc=@cid AND YEAR(h.fattdata) = @Year AND h.fattfor = @SupplierID 
                        ORDER BY h.fattdataric DESC",
                (hea, abe, abec, cau, rfi, tid, iso) => { hea.Supplier = abe; hea.Customer = abec; hea.Causal = cau; hea.TaxRegime = rfi; hea.DocumentType = tid; hea.ISO = iso; return hea; },
                new { cid = CompanyID, Year, SupplierID }, splitOn: "abecod,abecod,caucod,regicod,FETDCod, isocod");

            return new ObservableCollection<ACC_EINVOICE_HEADS>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_HEADS? Get(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_HEADS>(
                @"SELECT * FROM FATTIMP 
                    WHERE id = @id",
                new { id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_HEADS? GetByAccountingRegistration(int SupplierID, string ReferenceID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_HEADS>(
                @"SELECT * FROM FATTIMP 
                    WHERE fattnum = @fattnum AND fattfor = @fattfor",
                new { fattnum = ReferenceID, fattfor = SupplierID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_HEADS? GetFull(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();

            var result = connection.Query<ACC_EINVOICE_HEADS, ABE, CAUCONT, FE_TIPODOC, FE_RFIDOC, ISO, ACC_EINVOICE_HEADS>(
                @"SELECT h.*, a.abecod, a.abers1, a.abers2, c.caucod, c.caudes, fet.FETDCod, fet.FETDDes, fer.regicod, fer.regides, i.isocod, i.isodes FROM FATTIMP AS h
                        LEFT JOIN ANAG_BASE AS a ON a.abecod=h.fattfor
                        LEFT JOIN CAUCONT AS c ON c.caucod=h.fattcaus
                        LEFT JOIN FE_TIPODOC AS fet ON fet.FETDCod=h.FATTTIPODOC
                        LEFT JOIN REGIMIFISCALI AS fer ON fer.regicod=h.fattregi
                        LEFT JOIN ISO AS i ON i.isocod=h.fattiso
                        WHERE h.id=@id",
                (hea, abe, cau, fet, fer, iso) =>
                {
                    hea.Supplier = abe;
                    hea.Causal = cau;
                    hea.DocumentType = fet;
                    hea.TaxRegime = fer;
                    hea.ISO = iso;
                    return hea;
                },
                new { id = id }, splitOn: "abecod, caucod, FETDCod, regicod, isocod")
                .FirstOrDefault();

            // Rows

            if (result != null)
            {
                result.Rows = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().GetList(id);
                // Rows - load rate taken by product
                foreach (var row in result.Rows ?? new ObservableCollection<ACC_EINVOICE_ROWS>())
                {
                    tab_articolo? product = null;
                    foreach (var pid in row.PIDs ?? new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>())
                    {
                        product = !string.IsNullOrEmpty(pid.artvalcod) ? tabArticoloRepo.GetSingle(row.fattsoc, pid.artvalcod) : null;
                        if (product != null)
                            break;
                    }

                    row.Product = product;
                    row.Rate = (product != null && !string.IsNullOrEmpty(product.asscod) && !string.IsNullOrEmpty(product.assali)) ? VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(product.asscod, product.assali) : null;
                }
                // SMs
                result.SMs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_SMRepository>().GetList(id);
                // VATs
                result.VATs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_VATRepository>().GetList(id);
                // Expires
                result.Expires = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_EXPIRESRepository>().GetList(id);
                // DDTs
                result.DDTs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().GetList(id);
                // POs
                result.POs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().GetList(id);
                // Linkeds
                result.Linkeds = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().GetList(id);
                // CPs
                result.CPs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_CPRepository>().GetList(id);
                // RITs
                result.RITs = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_RITRepository>().GetList(id);
            }
            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string fattsoc, string fattnum, DateTime fattdata, string fattpiva, string fattdire)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FATTIMP 
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattdire = @fattdire",
                new { fattsoc = fattsoc, fattnum = fattnum, fattdata = fattdata, fattpiva = fattpiva, fattdire = fattdire }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    // ATTENZIONE! @fattdatasca va rimesso a NOW automatico in INSERT ad ogni modifica
    //             INSERTED.id invece di .rv in INSERT
    //             togliere id da INSERT ed UPDATE perchè è IDENTITY  
    public string INSERT_QUERY => "INSERT INTO FATTIMP (fattsoc,fattnum,fattdata,fattpiva,fattabers1,fattfor,fattimpo,fattimposta,fatttot,fattannoreg,fattnumreg,fattdataric,fattnomefileric,fattcaus,fattcausaledes,fattregi,fattpec,fattcoddest,fattiso,fattimpobol,fattfornazi,fattforcap,fattforloca,fattforinde,fattforprov,fattclicap,fattclinaz,fattclipro,fattcliloca,fattcliinde,fattclipiva,fattstampa,fattarrotondamento,FATTTIPODOC,fattidsdi,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,fattclirags,fattdire,fattdatasca,accounted,accounted_UserID,fattcli,fattclicf,fattalle) OUTPUT INSERTED.id VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattabers1,@fattfor,@fattimpo,@fattimposta,@fatttot,@fattannoreg,@fattnumreg,@fattdataric,@fattnomefileric,@fattcaus,@fattcausaledes,@fattregi,@fattpec,@fattcoddest,@fattiso,@fattimpobol,@fattfornazi,@fattforcap,@fattforloca,@fattforinde,@fattforprov,@fattclicap,@fattclinaz,@fattclipro,@fattcliloca,@fattcliinde,@fattclipiva,@fattstampa,@fattarrotondamento,@FATTTIPODOC,@fattidsdi,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@fattclirags,@fattdire,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@accounted,@accounted_UserID,@fattcli,@fattclicf,@fattalle)";
    public string UPDATE_QUERY => "UPDATE FATTIMP SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattabers1 = @fattabers1,fattfor = @fattfor,fattimpo = @fattimpo,fattimposta = @fattimposta,fatttot = @fatttot,fattannoreg = @fattannoreg,fattnumreg = @fattnumreg,fattdataric = @fattdataric,fattnomefileric = @fattnomefileric,fattcaus = @fattcaus,fattcausaledes = @fattcausaledes,fattregi = @fattregi,fattpec = @fattpec,fattcoddest = @fattcoddest,fattiso = @fattiso,fattimpobol = @fattimpobol,fattfornazi = @fattfornazi,fattforcap = @fattforcap,fattforloca = @fattforloca,fattforinde = @fattforinde,fattforprov = @fattforprov,fattclicap = @fattclicap,fattclinaz = @fattclinaz,fattclipro = @fattclipro,fattcliloca = @fattcliloca,fattcliinde = @fattcliinde,fattclipiva = @fattclipiva,fattstampa = @fattstampa,fattarrotondamento = @fattarrotondamento,FATTTIPODOC = @FATTTIPODOC,fattidsdi = @fattidsdi,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,fattclirags = @fattclirags,fattdire = @fattdire,fattdatasca = @fattdatasca,accounted = @accounted,accounted_UserID = @accounted_UserID,fattcli = @fattcli,fattclicf = @fattclicf,fattalle = @fattalle OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMP OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public long? Insert(ACC_EINVOICE_HEADS Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (long?)connection.ExecuteScalar(INSERT_QUERY, Model);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public int Insert(SdIItemVulpesX Model, string CompanyID, string UserID, string Direction)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    int counter = 0;
                    var culture = CultureInfo.CreateSpecificCulture("it-IT");
                    foreach (var invoice in Model.FullDocumentDecoded.Fatture)
                    {
                        var fattdata = DateTime.Parse(invoice.Data, culture);
                        var fattpiva = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA) ? Model.FullDocumentDecoded.CedentePrestatorePartitaIVA : Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale;
                        // check already exists
                        if (Exists(CompanyID, invoice.Numero, fattdata, fattpiva, Direction))
                        {
                            ErrorHandler.Show($"Il documento {invoice.Numero} del {fattdata.ToString("dd/MM/yyyy")} cedente/prestatore {fattpiva} è già presente");

                            return 0;
                        }
                        // head
                        var supplierVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetSupplierFromVAT(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA);
                        var supplierCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale, "F") : null;
                        var customerVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA, "C");
                        var customerCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale, "C") : null;
                        var tdoc = VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Get(invoice.Tipo);
                        var head = new ACC_EINVOICE_HEADS()
                        {
                            fattsoc = CompanyID,
                            fattnum = invoice.Numero,
                            fattdata = fattdata,
                            fattpiva = fattpiva,
                            fattabers1 = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatoreDenominazione) ? Model.FullDocumentDecoded.CedentePrestatoreDenominazione : $"{Model.FullDocumentDecoded.CedenteCognome} {Model.FullDocumentDecoded.CedenteNome}",
                            fattfor = supplierVAT != null ? supplierVAT.abecod : supplierCF?.abecod,
                            fattcli = customerVAT != null ? customerVAT.abecod : customerCF?.abecod,
                            fattimpo = invoice.DatiRiepilogo?.Sum(sum => sum.Imponibile),
                            fattimposta = invoice.DatiRiepilogo?.Sum(sum => sum.Imposta),
                            fatttot = invoice.ImportoTotaleDocumento,
                            fattdataric = DateTime.Parse(Model.DataRicezione, culture),
                            fattnomefileric = Model.FileName,
                            fattcaus = tdoc?.FETDACQC,
                            fattcausaledes = invoice.Causali?.FirstOrDefault(),
                            fattregi = Model.FullDocumentDecoded.CedentePrestatoreRegimeFiscale,
                            fattpec = Model.FullDocumentDecoded.PECDestinatario,
                            fattcoddest = Model.FullDocumentDecoded.CodiceDestinatario,
                            fattiso = Model.FullDocumentDecoded.CedentePrestatoreSedeNazione,
                            fattimpobol = invoice.DatiBolloImporto,
                            fattfornazi = Model.FullDocumentDecoded.CedentePrestatorePaese,
                            fattforcap = Model.FullDocumentDecoded.CedentePrestatoreSedeCap,
                            fattforloca = Model.FullDocumentDecoded.CedentePrestatoreSedeComune,
                            fattforinde = $"{Model.FullDocumentDecoded.CedentePrestatoreSedeIndirizzo}, {Model.FullDocumentDecoded.CedentePrestatoreSedeNumeroCivico}",
                            fattforprov = Model.FullDocumentDecoded.CedentePrestatoreSedeProvincia,
                            fattclicap = Model.FullDocumentDecoded.CessionarioCommittenteSedeCap,
                            fattclinaz = Model.FullDocumentDecoded.CessionarioCommittenteSedeNazione,
                            fattclipro = Model.FullDocumentDecoded.CessionarioCommittenteSedeProvincia,
                            fattcliloca = Model.FullDocumentDecoded.CessionarioCommittenteSedeComune,
                            fattcliinde = $"{Model.FullDocumentDecoded.CessionarioCommittenteSedeIndirizzo}, {Model.FullDocumentDecoded.CessionarioCommittenteSedeNumeroCivico}",
                            fattclipiva = Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA,
                            fattclirags = Model.FullDocumentDecoded.CessionarioCommittenteDenominazione,
                            fattarrotondamento = invoice.Arrotondamento,
                            FATTTIPODOC = invoice.Tipo,
                            fattidsdi = Model.SdIID,
                            fattdire = Direction,
                            addedUserID = UserID,
                            fattalle = Model.AttachmentsCount
                        };

                        var newID = (long?)connection.ExecuteScalar(INSERT_QUERY, head, transaction) ?? 0;

                        if (newID > 0)
                        {
                            head.id = newID;
                            // rows
                            int rowCounter = 1;
                            foreach (var item in invoice.RigheFattura)
                            {
                                var row = new ACC_EINVOICE_ROWS()
                                {
                                    id = newID,
                                    fattsoc = head.fattsoc,
                                    fattnum = head.fattnum,
                                    fattdata = head.fattdata,
                                    fattpiva = head.fattpiva,
                                    fattriga = rowCounter++,
                                    fattartdes = item.Descrizione,
                                    fattprz = item.PrezzoUnitario,
                                    fattqta = item.Quantita,
                                    fatttotriga = item.PrezzoTotale,
                                    fattumi = item.UnitaMisura,
                                    fattaliriga = item.AliquotaIVA.ToString(),
                                    fattnatu = item.Natura
                                };
                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().INSERT_QUERY, row, transaction);
                                // PIDS
                                if (item.CodiciArticolo != null)
                                {
                                    int pidsCounter = 1;
                                    foreach (var pid in item.CodiciArticolo)
                                    {
                                        var pi = new ACC_EINVOICE_ROWS_PIDS()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattriga = row.fattriga,
                                            artprog = pidsCounter++,
                                            artvalcod = pid.CodiceValore,
                                            artipcod = pid.CodiceTipo
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_PIDRepository>().INSERT_QUERY, pi, transaction);
                                    }
                                }
                                // SM
                                if (item.ScontiMaggiorazioni != null)
                                {
                                    int smsCounter = 1;
                                    foreach (var sms in item.ScontiMaggiorazioni)
                                    {
                                        var sm = new ACC_EINVOICE_ROWS_SM()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattriga = row.fattriga,
                                            Progsc = smsCounter++,
                                            sctipo = sms.Tipo,
                                            scimpo = sms.Importo,
                                            scperc = sms.Percentuale
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_SMRepository>().INSERT_QUERY, sm, transaction);
                                    }
                                }
                            }
                            // VAT
                            if (invoice.DatiRiepilogo != null)
                            {
                                int vatCounter = 1;
                                foreach (var item in invoice.DatiRiepilogo)
                                {
                                    var vat = new ACC_EINVOICE_VAT()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        fattprog = vatCounter++,
                                        Fattaliq = item.Aliquota.ToString().Length > 2 ? item.Aliquota.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.Aliquota.ToString().Trim(),
                                        fattimpodett = item.Imponibile,
                                        fattimpostadett = item.Imposta,
                                        fattesi = item.Esigibilita,
                                        fattrifenorm = item.RiferimentoNormativo,
                                        fattnatu = item.Natura,
                                        fattarrotondamento = item.Arrotondamento
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_VATRepository>().INSERT_QUERY, vat, transaction);
                                }
                            }
                            // purchase orders
                            if (invoice.DatiOrdiniAcquisto != null)
                            {
                                int poCounter = 1;
                                foreach (var item in invoice.DatiOrdiniAcquisto)
                                {
                                    if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                    {
                                        foreach (var rif in item.RiferimentoLinea)
                                        {
                                            var po = new ACC_EINVOICE_PO()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                riga = poCounter++,
                                                numitem = item.NumItem,
                                                iddocumento = item.IdDocumento,
                                                datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                rigarife = (int)rif
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                        }
                                    }
                                    else
                                    {
                                        var po = new ACC_EINVOICE_PO()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            riga = poCounter++,
                                            numitem = item.NumItem,
                                            iddocumento = item.IdDocumento,
                                            datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                            rigarife = null
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                    }
                                }
                            }
                            // SM
                            if (invoice.ScontiMaggiorazioni != null)
                            {
                                int smCounter = 1;
                                foreach (var item in invoice.ScontiMaggiorazioni)
                                {
                                    var sm = new ACC_EINVOICE_SM()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        sctprog = smCounter++,
                                        scttipo = item.Tipo,
                                        sctimpo = item.Importo,
                                        sctperc = item.Percentuale
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_SMRepository>().INSERT_QUERY, sm, transaction);
                                }
                            }
                            // CP
                            if (invoice.DatiCassaPrevidenziale != null)
                            {
                                int cpCounter = 1;
                                foreach (var item in invoice.DatiCassaPrevidenziale)
                                {
                                    var cp = new ACC_EINVOICE_CP()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        progcassa = cpCounter++,
                                        tipocassa = item.TipoCassa,
                                        alicassa = item.AlCassa.HasValue ? (item.AlCassa?.ToString().Length > 2 ? item.AlCassa?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AlCassa?.ToString().Trim()) : null,
                                        impcontricassa = item.ImportoContributoCassa,
                                        impocassa = item.ImponibileCassa,
                                        aliivacassa = item.AliquotaIVA.HasValue ? (item.AliquotaIVA?.ToString().Length > 2 ? item.AliquotaIVA?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AliquotaIVA?.ToString().Trim()) : null,
                                        ritecassa = item.Ritenuta,
                                        natcassa = item.Natura,
                                        rifammicassa = item.RiferimentoAmministrazione
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_CPRepository>().INSERT_QUERY, cp, transaction);
                                }
                            }
                            // Ritenuta
                            if (invoice.DatiRitenuta != null)
                            {
                                int ritCounter = 1;
                                foreach (var item in invoice.DatiRitenuta)
                                {
                                    var rit = new ACC_EINVOICE_RIT()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        progrit = ritCounter++,
                                        tiporit = item.Tipo,
                                        importo = item.Importo,
                                        aliquota = item.Aliquota,
                                        causalepagamento = item.CausalePagamento
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_RITRepository>().INSERT_QUERY, rit, transaction);
                                }
                            }
                            // DDT
                            if (invoice.DatiDDT != null)
                            {
                                int ddtCounter = 1;
                                foreach (var item in invoice.DatiDDT)
                                {
                                    if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                    {
                                        foreach (var rif in item.RiferimentoLinea)
                                        {
                                            var ddt = new ACC_EINVOICE_DDT()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                ddtriga = ddtCounter++,
                                                ddtdata = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                ddtnum = item.Numero,
                                                ddtriferiga = (int)rif
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                        }
                                    }
                                    else
                                    {
                                        var ddt = new ACC_EINVOICE_DDT()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            ddtriga = ddtCounter++,
                                            ddtdata = DateTime.Parse(item.Data, culture),
                                            ddtnum = item.Numero,
                                            ddtriferiga = null
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                    }
                                }
                            }
                            // expires
                            if (invoice.Pagamenti != null)
                            {
                                short seconds = 1;

                                foreach (var item in invoice.Pagamenti)
                                {
                                    foreach (var exp in item.DettagliPagamento)
                                    {
                                        var pay = VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Get(exp.Modalita);
                                        var expireDate = !string.IsNullOrWhiteSpace(exp.DataScadenza) ? DateTime.Parse(exp.DataScadenza, culture) : head.fattdata;
                                        var expire = new ACC_EINVOICE_EXPIRES()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattdatascad = expireDate.AddSeconds(seconds++),
                                            fattimpscad = exp.Importo,
                                            fattipopag = exp.Modalita,
                                            fattiban = exp.IBAN,
                                            fattistu = exp.IstitutoFinanziario,
                                            fattabi = exp.ABI,
                                            fattcab = exp.CAB,
                                            fattcond = item.Condizioni,
                                            FATTTIPOLPAG = pay?.FEPATIPP,
                                            FATTCODPAGV = exp.CodicePagamento
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_EXPIRESRepository>().INSERT_QUERY, expire, transaction);
                                    }
                                }
                            }
                            // linked
                            if (invoice.DatiFattureCollegate != null)
                            {
                                int lnkCounter = 1;
                                foreach (var item in invoice.DatiFattureCollegate)
                                {
                                    if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                    {
                                        foreach (var rif in item.RiferimentoLinea)
                                        {
                                            var linked = new ACC_EINVOICE_LINKED()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattcollriga = lnkCounter++,
                                                fattcollnumitem = item.NumItem,
                                                fattcolliddocumento = item.IdDocumento,
                                                fattcolldatadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : new DateTime(1753, 1, 1),
                                                fattcollriferiga = (int)rif
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                        }
                                    }
                                    else
                                    {
                                        var linked = new ACC_EINVOICE_LINKED()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattcollriga = lnkCounter++,
                                            fattcollnumitem = item.NumItem,
                                            fattcolliddocumento = item.IdDocumento,
                                            fattcolldatadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : new DateTime(1753,1,1),
                                            fattcollriferiga = null
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                    }
                                }
                            }
                            counter++;
                        }
                        else
                        {

                            ErrorHandler.Show($"Impossibile inserire il documento, errore XGE-001");

                            return 0;
                        }
                    }
                    transaction.Commit();
                    return counter;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();

                    ErrorHandler.Show(exc.Message);

                    return 0;
                }
            }

        }
        catch (Exception ex)
        {

            ErrorHandler.Show(ex.Message);

            return 0;
        }
    }

    public int InsertLocal(SdIItemGX Model, short AttachmentsCount, string CompanyID, string UserID, string Direction)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    int counter = 0;
                    var culture = CultureInfo.CreateSpecificCulture("it-IT");

                    if (Model.FullDocumentDecoded != null)
                    {
                        foreach (var invoice in Model.FullDocumentDecoded.Fatture ?? new List<Models.Models.Accounting.eInvoice.FullDocumentItem>())
                        {
                            var fattdata = DateTime.Parse(invoice.Data ?? string.Empty, culture);
                            var fattpiva = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA) ? Model.FullDocumentDecoded.CedentePrestatorePartitaIVA : Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale;
                            // check already exists
                            if (Exists(CompanyID, invoice.Numero ?? string.Empty, fattdata, fattpiva ?? string.Empty, Direction))
                            {
                                ErrorHandler.Show($"Il documento {invoice.Numero} del {fattdata.ToString("dd/MM/yyyy")} cedente/prestatore {fattpiva} è già presente");
                                return 0;
                            }
                            // head
                            var supplierVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CedentePrestatorePartitaIVA ?? string.Empty, "F");
                            var supplierCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CedentePrestatoreCodiceFiscale, "F") : null;
                            var customerVAT = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsVAT(Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA ?? string.Empty, "C");
                            var customerCF = !string.IsNullOrWhiteSpace(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale) ? VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().ExistsCF(Model.FullDocumentDecoded.CessionarioCommittenteCodiceFiscale, "C") : null;
                            var tdoc = VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().Get(invoice.Tipo ?? string.Empty);
                            var head = new ACC_EINVOICE_HEADS()
                            {
                                fattsoc = CompanyID,
                                fattnum = invoice.Numero ?? string.Empty,
                                fattdata = fattdata,
                                fattpiva = fattpiva ?? string.Empty,
                                fattabers1 = Model.FullDocumentDecoded.CedentePrestatoreDenominazione,
                                fattfor = supplierVAT != null ? supplierVAT.abecod : supplierCF?.abecod,
                                fattcli = customerVAT != null ? customerVAT.abecod : customerCF?.abecod,
                                fattimpo = invoice.DatiRiepilogo?.Sum(sum => sum.Imponibile),
                                fattimposta = invoice.DatiRiepilogo?.Sum(sum => sum.Imposta),
                                fatttot = invoice.ImportoTotaleDocumento,
                                fattdataric = DateTime.Parse(Model.DataRicezione ?? string.Empty, culture),
                                fattnomefileric = Model.FileName,
                                fattcaus = tdoc?.FETDACQC,
                                fattcausaledes = invoice.Causali?.FirstOrDefault(),
                                fattregi = Model.FullDocumentDecoded.CedentePrestatoreRegimeFiscale,
                                fattpec = Model.FullDocumentDecoded.PECDestinatario,
                                fattcoddest = Model.FullDocumentDecoded.CodiceDestinatario,
                                fattiso = Model.FullDocumentDecoded.CedentePrestatoreSedeNazione,
                                fattimpobol = invoice.DatiBolloImporto,
                                fattfornazi = Model.FullDocumentDecoded.CedentePrestatorePaese,
                                fattforcap = Model.FullDocumentDecoded.CedentePrestatoreSedeCap,
                                fattforloca = Model.FullDocumentDecoded.CedentePrestatoreSedeComune,
                                fattforinde = $"{Model.FullDocumentDecoded.CedentePrestatoreSedeIndirizzo}, {Model.FullDocumentDecoded.CedentePrestatoreSedeNumeroCivico}",
                                fattforprov = Model.FullDocumentDecoded.CedentePrestatoreSedeProvincia,
                                fattclicap = Model.FullDocumentDecoded.CessionarioCommittenteSedeCap,
                                fattclinaz = Model.FullDocumentDecoded.CessionarioCommittenteSedeNazione,
                                fattclipro = Model.FullDocumentDecoded.CessionarioCommittenteSedeProvincia,
                                fattcliloca = Model.FullDocumentDecoded.CessionarioCommittenteSedeComune,
                                fattcliinde = $"{Model.FullDocumentDecoded.CessionarioCommittenteSedeIndirizzo}, {Model.FullDocumentDecoded.CessionarioCommittenteSedeNumeroCivico}",
                                fattclipiva = Model.FullDocumentDecoded.CessionarioCommittentePartitaIVA,
                                fattclirags = Model.FullDocumentDecoded.CessionarioCommittenteDenominazione,
                                fattarrotondamento = invoice.Arrotondamento,
                                FATTTIPODOC = invoice.Tipo,
                                fattidsdi = Model.SdIID,
                                fattdire = Direction,
                                addedUserID = UserID,
                                fattalle = AttachmentsCount
                            };
                            var newID = (long?)connection.ExecuteScalar(INSERT_QUERY, head, transaction) ?? 0;
                            if (newID > 0)
                            {
                                // rows
                                int rowCounter = 1;
                                foreach (var item in invoice.RigheFattura ?? new List<Models.Models.Accounting.eInvoice.FullDocumentItemRow>())
                                {
                                    var row = new ACC_EINVOICE_ROWS()
                                    {
                                        id = newID,
                                        fattsoc = head.fattsoc,
                                        fattnum = head.fattnum,
                                        fattdata = head.fattdata,
                                        fattpiva = head.fattpiva,
                                        fattriga = rowCounter++,
                                        fattartdes = item.Descrizione,
                                        fattprz = item.PrezzoUnitario,
                                        fattqta = item.Quantita,
                                        fatttotriga = item.PrezzoTotale,
                                        fattumi = item.UnitaMisura,
                                        fattaliriga = item.AliquotaIVA.ToString(),
                                        fattnatu = item.Natura
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWSRepository>().INSERT_QUERY, row, transaction);
                                    // PIDS
                                    if (item.CodiciArticolo != null)
                                    {
                                        int pidsCounter = 1;
                                        foreach (var pid in item.CodiciArticolo)
                                        {
                                            var pi = new ACC_EINVOICE_ROWS_PIDS()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattriga = row.fattriga,
                                                artprog = pidsCounter++,
                                                artvalcod = pid.CodiceValore,
                                                artipcod = pid.CodiceTipo
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_PIDRepository>().INSERT_QUERY, pi, transaction);
                                        }
                                    }
                                    // SM
                                    if (item.ScontiMaggiorazioni != null)
                                    {
                                        int smsCounter = 1;
                                        foreach (var sms in item.ScontiMaggiorazioni)
                                        {
                                            var sm = new ACC_EINVOICE_ROWS_SM()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattriga = row.fattriga,
                                                Progsc = smsCounter++,
                                                sctipo = sms.Tipo,
                                                scimpo = sms.Importo,
                                                scperc = sms.Percentuale
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_ROWS_SMRepository>().INSERT_QUERY, sm, transaction);
                                        }
                                    }
                                }
                                // VAT
                                if (invoice.DatiRiepilogo != null)
                                {
                                    int vatCounter = 1;
                                    foreach (var item in invoice.DatiRiepilogo)
                                    {
                                        var vat = new ACC_EINVOICE_VAT()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            fattprog = vatCounter++,
                                            Fattaliq = item.Aliquota.ToString().Length > 2 ? item.Aliquota.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.Aliquota.ToString().Trim(),
                                            fattimpodett = item.Imponibile,
                                            fattimpostadett = item.Imposta,
                                            fattesi = item.Esigibilita,
                                            fattrifenorm = item.RiferimentoNormativo,
                                            fattnatu = item.Natura
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_VATRepository>().INSERT_QUERY, vat, transaction);
                                    }
                                }
                                // purchase orders
                                if (invoice.DatiOrdiniAcquisto != null)
                                {
                                    int poCounter = 1;
                                    foreach (var item in invoice.DatiOrdiniAcquisto)
                                    {
                                        if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                        {
                                            foreach (var rif in item.RiferimentoLinea)
                                            {
                                                var po = new ACC_EINVOICE_PO()
                                                {
                                                    id = newID,
                                                    fattsoc = head.fattsoc,
                                                    fattnum = head.fattnum,
                                                    fattdata = head.fattdata,
                                                    fattpiva = head.fattpiva,
                                                    riga = poCounter++,
                                                    numitem = item.NumItem,
                                                    iddocumento = item.IdDocumento,
                                                    datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                    rigarife = (int)rif
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                            }
                                        }
                                        else
                                        {
                                            var po = new ACC_EINVOICE_PO()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                riga = poCounter++,
                                                numitem = item.NumItem,
                                                iddocumento = item.IdDocumento,
                                                datadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                rigarife = null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_PORepository>().INSERT_QUERY, po, transaction);
                                        }
                                    }
                                }
                                // SM
                                if (invoice.ScontiMaggiorazioni != null)
                                {
                                    int smCounter = 1;
                                    foreach (var item in invoice.ScontiMaggiorazioni)
                                    {
                                        var sm = new ACC_EINVOICE_SM()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            sctprog = smCounter++,
                                            scttipo = item.Tipo,
                                            sctimpo = item.Importo,
                                            sctperc = item.Percentuale
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_SMRepository>().INSERT_QUERY, sm, transaction);
                                    }
                                }
                                // CP
                                if (invoice.DatiCassaPrevidenziale != null)
                                {
                                    int cpCounter = 1;
                                    foreach (var item in invoice.DatiCassaPrevidenziale)
                                    {
                                        var cp = new ACC_EINVOICE_CP()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            progcassa = cpCounter++,
                                            tipocassa = item.TipoCassa,
                                            alicassa = item.AlCassa.HasValue ? (item.AlCassa?.ToString().Length > 2 ? item.AlCassa?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AlCassa?.ToString().Trim()) : null,
                                            impcontricassa = item.ImportoContributoCassa,
                                            impocassa = item.ImponibileCassa,
                                            aliivacassa = item.AliquotaIVA.HasValue ? (item.AliquotaIVA?.ToString().Length > 2 ? item.AliquotaIVA?.ToString().Substring(0, 2).Trim().Replace(",", null).Replace(".", null) : item.AliquotaIVA?.ToString().Trim()) : null,
                                            ritecassa = item.Ritenuta,
                                            natcassa = item.Natura,
                                            rifammicassa = item.RiferimentoAmministrazione
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_CPRepository>().INSERT_QUERY, cp, transaction);
                                    }
                                }
                                // Ritenuta
                                if (invoice.DatiRitenuta != null)
                                {
                                    int ritCounter = 1;
                                    foreach (var item in invoice.DatiRitenuta)
                                    {
                                        var rit = new ACC_EINVOICE_RIT()
                                        {
                                            id = newID,
                                            fattsoc = head.fattsoc,
                                            fattnum = head.fattnum,
                                            fattdata = head.fattdata,
                                            fattpiva = head.fattpiva,
                                            progrit = ritCounter++,
                                            tiporit = item.Tipo,
                                            importo = item.Importo,
                                            aliquota = item.Aliquota,
                                            causalepagamento = item.CausalePagamento
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_RITRepository>().INSERT_QUERY, rit, transaction);
                                    }
                                }
                                // DDT
                                if (invoice.DatiDDT != null)
                                {
                                    int ddtCounter = 1;
                                    foreach (var item in invoice.DatiDDT)
                                    {
                                        if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                        {
                                            foreach (var rif in item.RiferimentoLinea)
                                            {
                                                var ddt = new ACC_EINVOICE_DDT()
                                                {
                                                    id = newID,
                                                    fattsoc = head.fattsoc,
                                                    fattnum = head.fattnum,
                                                    fattdata = head.fattdata,
                                                    fattpiva = head.fattpiva,
                                                    ddtriga = ddtCounter++,
                                                    ddtdata = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                    ddtnum = item.Numero,
                                                    ddtriferiga = (int)rif
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                            }
                                        }
                                        else
                                        {
                                            var ddt = new ACC_EINVOICE_DDT()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                ddtriga = ddtCounter++,
                                                ddtdata = DateTime.Parse(item.Data ?? string.Empty, culture),
                                                ddtnum = item.Numero,
                                                ddtriferiga = null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_DDTRepository>().INSERT_QUERY, ddt, transaction);
                                        }
                                    }
                                }
                                // expires
                                if (invoice.Pagamenti != null)
                                {
                                    foreach (var item in invoice.Pagamenti)
                                    {
                                        foreach (var exp in item.DettagliPagamento ?? new List<Models.Models.Accounting.eInvoice.FullDocumentItemPaymentDetail>())
                                        {
                                            var pay = VulpesServiceProvider.Provider.GetRequiredService<IFE_PAGDOCRepository>().Get(exp.Modalita ?? string.Empty);
                                            var expire = new ACC_EINVOICE_EXPIRES()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattdatascad = !string.IsNullOrWhiteSpace(exp.DataScadenza) ? DateTime.Parse(exp.DataScadenza, culture) : head.fattdata,
                                                fattimpscad = exp.Importo,
                                                fattipopag = exp.Modalita,
                                                fattiban = exp.IBAN,
                                                fattistu = exp.IstitutoFinanziario,
                                                fattabi = exp.ABI,
                                                fattcab = exp.CAB,
                                                fattcond = item.Condizioni,
                                                FATTTIPOLPAG = pay?.FEPATIPP,
                                                FATTCODPAGV = exp.CodicePagamento
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_EXPIRESRepository>().INSERT_QUERY, expire, transaction);
                                        }
                                    }
                                }
                                // linked
                                if (invoice.DatiFattureCollegate != null)
                                {
                                    int lnkCounter = 1;
                                    foreach (var item in invoice.DatiFattureCollegate)
                                    {
                                        if (item.RiferimentoLinea != null && item.RiferimentoLinea.Count() > 0)
                                        {
                                            foreach (var rif in item.RiferimentoLinea)
                                            {
                                                var linked = new ACC_EINVOICE_LINKED()
                                                {
                                                    id = newID,
                                                    fattsoc = head.fattsoc,
                                                    fattnum = head.fattnum,
                                                    fattdata = head.fattdata,
                                                    fattpiva = head.fattpiva,
                                                    fattcollriga = lnkCounter++,
                                                    fattcollnumitem = item.NumItem,
                                                    fattcolliddocumento = item.IdDocumento,
                                                    fattcolldatadoc = !string.IsNullOrWhiteSpace(item.Data) ? DateTime.Parse(item.Data, culture) : null,
                                                    fattcollriferiga = (int)rif
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                            }
                                        }
                                        else
                                        {
                                            var linked = new ACC_EINVOICE_LINKED()
                                            {
                                                id = newID,
                                                fattsoc = head.fattsoc,
                                                fattnum = head.fattnum,
                                                fattdata = head.fattdata,
                                                fattpiva = head.fattpiva,
                                                fattcollriga = lnkCounter++,
                                                fattcollnumitem = item.NumItem,
                                                fattcolliddocumento = item.IdDocumento,
                                                fattcolldatadoc = DateTime.Parse(item.Data ?? string.Empty, culture),
                                                fattcollriferiga = null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_LINKEDRepository>().INSERT_QUERY, linked, transaction);
                                        }
                                    }
                                }
                                counter++;
                            }
                            else
                            {

                                ErrorHandler.Show($"Impossibile inserire il documento, errore XGE-001");

                                return 0;
                            }
                        }
                    }
                    transaction.Commit();
                    return counter;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();

                    ErrorHandler.Show(exc.Message);

                    return 0;
                }
            }

        }
        catch (Exception ex)
        {

            ErrorHandler.Show(ex.Message);

            return 0;
        }
    }

    public bool Update(ACC_EINVOICE_HEADS Model)
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

    public bool Delete(ACC_EINVOICE_HEADS Model, Guid? CompanyGuid)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // heads
                    connection.Execute(DELETE_QUERY, Model, transaction);
                    // rows
                    connection.Execute(@"DELETE FROM FATTIMPLEVEL2
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // rows pids
                    connection.Execute(@"DELETE FROM FATTIMPLEVEL2ARTICOLI
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // rows sm
                    connection.Execute(@"DELETE FROM FATTIMPLEVEL2SCONTO
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // cp
                    connection.Execute(@"DELETE FROM FATTIMPDATICASSAPREVIDENZIALE
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // DDT
                    connection.Execute(@"DELETE FROM FATTIMPDDT
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // expires
                    connection.Execute(@"DELETE FROM FATTIMPLEVEL3
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // linked
                    connection.Execute(@"DELETE FROM FATTIMPFATTCOLLEGATE
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // po
                    connection.Execute(@"DELETE FROM FATTIMPORDACQ
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // sm
                    connection.Execute(@"DELETE FROM FATTIMPSCONTOTESTA
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);
                    // VAT
                    connection.Execute(@"DELETE FROM FATTIMPLEVEL1
                                                WHERE fattsoc=@fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva=@fattpiva",
                        new { fattsoc = Model.fattsoc, fattnum = Model.fattnum, fattdata = Model.fattdata, fattpiva = Model.fattpiva }, transaction);

                    // check if need to delete file on cloud
                    if (Model.fattdire == "S")
                    {
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyGuid}/{StorageHelper.INVOICE_EXTERNAL_SENT_FOLDER}{Model.fattnomefileric}");
                    }

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

    #endregion
}