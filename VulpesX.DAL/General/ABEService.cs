using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Xml;
using VulpesX.DAL;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.General;

public interface IABERepository
{
    ObservableCollection<ABE>? GetCompanyList(string CompanyID);

    ObservableCollection<ABE>? GetList();

    ObservableCollection<ABE>? GetLightList();

    ObservableCollection<ABE>? GetLightListObsoleti();

    ObservableCollection<ABE>? GetLightList(string ItemType);

    ObservableCollection<ABE>? GetLightList(string CompanyID, string ItemType);

    ObservableCollection<ABE>? GetCustomersLightListActive(string ItemType, bool IncludeProspect = false);

    ObservableCollection<ABE>? GetCustomersLightListActiveForExternals(string ExternalID);

    ABE? Get(int ID);

    ABE? Get(string CompanyID, int ID);

    ABE? GetSupplierFromVAT(string VATID);

    bool Exists(int ID);

    ABE? ExistsVAT(string VATID, string? EntityType = null);

    ABE? ExistsVAT(int ID, string? VATID);

    ABE? ExistsCF(string CF, string? EntityType = null);

    ABE? ExistsCF(int ID, string? CF);

    ObservableCollection<int>? GetFreeIDList();

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(string CompanyID, ABE Model,
       ObservableCollection<DESTINATARI>? Recipients, ObservableCollection<DESFOR>? SupplierRecipients,
       ObservableCollection<NOTECLI1>? CustomerNotes, ObservableCollection<NOTEFOR>? SupplierNotes,
       CLIAMMI? CustomerData, FORNAMMI? SupplierData,
       CLIENTI? CustomerCommercialData, FORNITORI? SupplierCommercialData,
       ObservableCollection<RFFTB00F>? SupplierReferences, ObservableCollection<ANDEFRES>? CustomerReferences,
       ObservableCollection<SUPPLIER_GROUPS>? Counterparts, ObservableCollection<CUSTOMER_GROUPS>? CounterpartsCustomer,
       ObservableCollection<ABE_EXTERN>? ExternalCodes, ANACERT? Certificate, ObservableCollection<ANACERTLEVEL1>? Certificates, SCADCLI? ExpireCustomer);

    bool Update(string CompanyID, ABE Model,
       ObservableCollection<DESTINATARI>? Recipients, ObservableCollection<DESFOR>? SupplierRecipients,
       ObservableCollection<NOTECLI1>? CustomerNotes, ObservableCollection<NOTEFOR>? SupplierNotes,
       CLIAMMI? CustomerData, FORNAMMI? SupplierData,
       CLIENTI? CustomerCommercialData, FORNITORI? SupplierCommercialData,
       ObservableCollection<RFFTB00F>? SupplierReferences, ObservableCollection<ANDEFRES>? CustomerReferences,
       ObservableCollection<SUPPLIER_GROUPS>? Counterparts, ObservableCollection<CUSTOMER_GROUPS>? CounterpartsCustomer,
       ObservableCollection<ABE_EXTERN>? ExternalCodes, ANACERT? Certificate, ObservableCollection<ANACERTLEVEL1>? Certificates, SCADCLI? ExpireCustomer);


    string? CanDelete(int ID);

    bool Delete(ABE Model, string? ReasonText, string? UserID);

    string? Validate(ABE? Model, FORNAMMI? SupplierData, CLIAMMI? CustomerData, FORNITORI? SupplierCommercialData, CLIENTI? CustomerCommercialData, int VATLength, bool IsInsert);
    #endregion
}

public class ABERepository : RepositoryBase, IABERepository
{
    public ABERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ABE> GetCompanyList(string CompanyID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<ABE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                $@"SELECT a.*,  b.abecod AS IsObsolete FROM ABE as a
                        OUTER APPLY (
                            SELECT TOP 1 abecod 
                            FROM dbo.ABE b 
                            WHERE b.abold = a.abecod
                        ) b WHERE a.canceled IS NULL");

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ABE>(
                "SELECT abecod, TRIM(abers1) AS abers1, TRIM(abers2) AS abers2, abecfe FROM ABE WHERE canceled IS NULL");

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightListObsoleti()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                "SELECT abecod, TRIM(abers1) AS abers1 FROM ABE");

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightList(string ItemType)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                $@"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2, b.abecod AS IsObsolete FROM ABE as a
                    OUTER APPLY (
                            SELECT TOP 1 abecod 
                            FROM dbo.ABE b 
                            WHERE b.abold = a.abecod
                    ) b
                    WHERE (a.ABECFE = 'E' OR a.ABECFE = @abecfe) AND a.canceled IS NULL",
                new { abecfe = ItemType }).ToList();
            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightList(string CompanyID, string ItemType)
    {
        throw new NotImplementedException();
    }


    public ObservableCollection<ABE>? GetCustomersLightListActive(string ItemType, bool IncludeProspect = false)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ABE>(
                $@"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2, abeind, abecap, abeloc, abepro, abepiv, abecfi, abecfe, b.abecod AS IsObsolete FROM ABE AS a
                        LEFT JOIN CLIENTI AS c ON c.CLIENT = a.abecod
                        OUTER APPLY (
                            SELECT TOP 1 abecod 
                            FROM dbo.ABE b 
                            WHERE b.abold = a.abecod
                        ) b
                        WHERE (a.ABECFE = 'E' OR a.ABECFE = @abecfe {(IncludeProspect ? " OR a.ABECFE = 'P' " : null)}) AND (c.CLSOSP <> 'S' OR CLSOSP IS NULL) AND canceled IS NULL",
                new { abecfe = ItemType });

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetCustomersLightListActiveForExternals(string ExternalID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                @"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2, abeind, abecap, abeloc, abepro, abepiv, abecfi FROM ABE AS a
                        INNER JOIN CLIENTI AS c ON c.CLIENT = a.abecod
                        INNER JOIN ABE_EXTERN AS e ON e.abecod = a.abecod
                        WHERE e.abeextcode = @externalid AND (c.CLSOSP <> 'S' OR CLSOSP IS NULL) AND canceled IS NULL",
                new { externalid = ExternalID });

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? Get(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                "SELECT * FROM ABE WHERE abecod = @id AND canceled IS NULL",
                new { id = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? Get(string CompanyID, int ID)
    {
        throw new NotImplementedException();
    }

    public ABE? GetSupplierFromVAT(string VATID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var abe = connection.Query<ABE>(
                $@"SELECT a.* FROM ABE as a WHERE a.abepiv =@VATID AND a.canceled IS NULL AND (abecfe = 'E' OR abecfe = 'F')", new { VATID = VATID }).FirstOrDefault();

            return abe;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ABE WHERE abecod = @id",
                new { id = ID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ABE? ExistsVAT(string VATID, string? EntityType = null)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                $"SELECT * FROM ABE WHERE abepiv = @abepiv AND canceled IS NULL {(string.IsNullOrWhiteSpace(EntityType) ? null : "AND (abecfe = 'E' OR abecfe = @abecfe)")}",
                new { abepiv = VATID, abecfe = EntityType }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? ExistsVAT(int ID, string? VATID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                "SELECT * FROM ABE WHERE abecod <> @id AND abepiv = @abepiv",
                new { id = ID, abepiv = VATID }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? ExistsCF(string CF, string? EntityType = null)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                $"SELECT * FROM ABE WHERE abecfi = @abecfi {(string.IsNullOrWhiteSpace(EntityType) ? null : "AND (abecfe = 'E' OR abecfe = @abecfe)")}",
                new { abecfi = CF, abecfe = EntityType }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? ExistsCF(int ID, string? CF)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                "SELECT * FROM ABE WHERE abecod <> @id AND abecfi = @abecfi",
                new { id = ID, abecfi = CF }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Recupera una lista di tutti i codici disponibili non utilizzati.
    /// ATTENZIONE : questo è l'unico modo per farlo facendo una sola chiamata al database, altrimenti bisognerebbe farne
    /// migliaia o utilizzare una tabella temporanea o una sequenza; questo metodo funziona ma ha un "bug", in caso di 
    /// diversi codici liberi consecutivi, mostra solo il primo, ad esempio se nell'intervallo tra 7000 e 7010 (esclusi)
    /// sono tutti liberi, questa query ritornerà solo 7001: per quello che ci serve per ora va bene così, metto un TODO 
    /// come promemoria
    /// </summary>
    /// <returns>ObservableCollection<int></returns>
    public ObservableCollection<int>? GetFreeIDList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Query<int>(
                "SELECT DISTINCT abecod+1 FROM ABE WHERE abecod + 1 NOT IN (SELECT DISTINCT abecod FROM ABE);");
            if (result != null && result.Count() > 0)
                return new ObservableCollection<int>(result.ToList());
            else
                return new ObservableCollection<int>(Enumerable.Range(1, 100).ToList());
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ABE (abecod,abers1,abers2,abeind,abecap,abeloc,abepro,nazcod,abetpo,isocod,abepiv,abecfi,soctip,abedna,abelna,abepna,abesex,abeais,abeccc,abetri,abeinl,abecal,abelol,abeprl,abecfe,abesomcod,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,abfatfile,abfatfileid, abold) OUTPUT INSERTED.rv VALUES(@abecod,@abers1,@abers2,@abeind,@abecap,@abeloc,@abepro,@nazcod,@abetpo,@isocod,@abepiv,@abecfi,@soctip,@abedna,@abelna,@abepna,@abesex,@abeais,@abeccc,@abetri,@abeinl,@abecal,@abelol,@abeprl,@abecfe,@abesomcod,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@abfatfile,@abfatfileid, @abold)";
    public string UPDATE_QUERY => "UPDATE ABE SET abecod = @abecod,abers1 = @abers1,abers2 = @abers2,abeind = @abeind,abecap = @abecap,abeloc = @abeloc,abepro = @abepro,nazcod = @nazcod,abetpo = @abetpo,isocod = @isocod,abepiv = @abepiv,abecfi = @abecfi,soctip = @soctip,abedna = @abedna,abelna = @abelna,abepna = @abepna,abesex = @abesex,abeais = @abeais,abeccc = @abeccc,abetri = @abetri,abeinl = @abeinl,abecal = @abecal,abelol = @abelol,abeprl = @abeprl,abecfe = @abecfe,abesomcod = @abesomcod,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,abfatfile = @abfatfile,abfatfileid = @abfatfileid, abold = @abold OUTPUT INSERTED.rv WHERE abecod = @abecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ABE OUTPUT DELETED.rv WHERE abecod = @abecod AND rv = @rv";
    public bool Insert(string CompanyID, ABE Model,
        ObservableCollection<DESTINATARI>? Recipients, ObservableCollection<DESFOR>? SupplierRecipients,
        ObservableCollection<NOTECLI1>? CustomerNotes, ObservableCollection<NOTEFOR>? SupplierNotes,
        CLIAMMI? CustomerData, FORNAMMI? SupplierData,
        CLIENTI? CustomerCommercialData, FORNITORI? SupplierCommercialData,
        ObservableCollection<RFFTB00F>? SupplierReferences, ObservableCollection<ANDEFRES>? CustomerReferences,
        ObservableCollection<SUPPLIER_GROUPS>? Counterparts, ObservableCollection<CUSTOMER_GROUPS>? CounterpartsCustomer,
        ObservableCollection<ABE_EXTERN>? ExternalCodes, ANACERT? Certificate, ObservableCollection<ANACERTLEVEL1>? Certificates, SCADCLI? ExpireCustomer)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var fornitoriRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>();
                    var fornammiRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>();
                    var clientiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>();
                    var cliammiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                    var abeExternRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>();
                    var abeExternDestRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();

                    var destinatariRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();

                    var notecli1Repository = VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>();

                    var irfftb00fRepository = VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>();
                    var andefresRepository = VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>();

                    var noteForRepository = VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>();
                    var desForRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>();

                    var supplierGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>();
                    var customerGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>();

                    object? resFORNAMMI = null;
                    object? resFORNITORI = null;
                    object? resCLIAMMI = null;
                    object? resCLIENTI = null;
                    var resInsertABE = connection.ExecuteScalar(INSERT_QUERY, Model, transaction);

                    #region Cancel Old
                    connection.Execute(@"UPDATE ABE SET canceledNote = @note WHERE abecod = @abold ",
                            new
                            {
                                note = $"Reso obsoleto dal codice [{Model.abecod}]",
                                abold = Model.abold
                            }, transaction);
                    #endregion

                    #region Clean possible supplier info
                    if (Model.abecfe == "C")
                    {
                        // DESFOR
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTEFOR
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod", new { nofcod = Model.abecod }, transaction);
                        // FORNAMMI
                        connection.Execute(@"DELETE FROM FORNAMMI WHERE Foraso = @cid AND Foraco = @foraco", new { cid = CompanyID, foraco = Model.abecod }, transaction);
                        // FORNITORI
                        connection.Execute(@"DELETE FROM FORNITORI WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // RFFTB00F
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // Countepart SUPPLIER_GROUPS
                        connection.Execute(@"DELETE FROM SUPPLIER_GROUPS WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode", new { ccfsoc = CompanyID, cfcode = Model.abecod }, transaction);
                        // set null objects
                        SupplierData = null;
                        SupplierCommercialData = null;
                    }
                    #endregion

                    #region Clean possible customer info
                    if (Model.abecfe == "F")
                    {
                        // DESTINATARI
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTECLI1
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli", new { notcli = Model.abecod }, transaction);
                        // CLIAMMI
                        connection.Execute(@"DELETE FROM CLIAMMI WHERE Cliasoc = @cid AND Cliacod = @client", new { cid = CompanyID, client = Model.abecod }, transaction);
                        // CLIENTI
                        connection.Execute(@"DELETE FROM CLIENTI WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // ANDEFRES
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // Countepart CUSTOMER_GROUPS
                        connection.Execute(@"DELETE FROM CUSTOMER_GROUPS WHERE cccsoc = @cccsoc AND cccode = @cccode", new { cccsoc = CompanyID, cccode = Model.abecod }, transaction);
                        // set null objects
                        CustomerData = null;
                        CustomerCommercialData = null;
                    }
                    #endregion

                    // EXTERN
                    // clear
                    connection.Execute(@"DELETE FROM ABE_EXTERN WHERE abecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                    connection.Execute(@"DELETE FROM ABE_EXTERN_DESTS WHERE abecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                    if (ExternalCodes != null && ExternalCodes.Count > 0)
                    {
                        foreach (var item in ExternalCodes)
                        {
                            connection.Execute(abeExternRepository.INSERT_QUERY, item, transaction);
                            foreach (var dest in item?.Destinations ?? new ObservableCollection<ABE_EXTERN_DESTS>())
                            {
                                connection.Execute(abeExternDestRepository.INSERT_QUERY, dest, transaction);
                            }
                        }
                    }

                    if (Model.abecfe == "C" || Model.abecfe == "E" || Model.abecfe == "P")
                    {
                        // DESTINATARI
                        // clear
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod",
                            new { cliecod = Model.abecod },
                            transaction);
                        if (Recipients != null && Recipients.Count > 0)
                        {
                            foreach (var item in Recipients)
                            {
                                connection.Execute(destinatariRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTECLI1
                        // clear
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli",
                            new { notcli = Model.abecod },
                            transaction);
                        if (CustomerNotes != null && CustomerNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerNotes)
                            {
                                item.NOTCLI = Model.abecod;
                                item.NOTRAG = Model.abers1;
                                item.notrig = rowID++;
                                connection.Execute(notecli1Repository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // CLIAMMI
                        if (CustomerData != null)
                        {
                            if (cliammiRepository.Exists(CustomerData.Cliasoc, CustomerData.Cliacod))
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.UPDATE_QUERY, CustomerData, transaction);
                            }
                            else
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.INSERT_QUERY, CustomerData, transaction);
                            }
                        }
                        // CLIENTI
                        if (CustomerCommercialData != null)
                        {
                            if (clientiRepository.Exists(CustomerCommercialData.CLIENT))
                            {
                                resCLIENTI = connection.ExecuteScalar(clientiRepository.UPDATE_QUERY, CustomerCommercialData, transaction);
                            }
                            else
                            {
                                resCLIENTI = connection.ExecuteScalar(clientiRepository.INSERT_QUERY, CustomerCommercialData, transaction);
                            }
                        }
                        // ANDEFRES
                        // clear
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client",
                            new { client = Model.abecod },
                            transaction);
                        if (CustomerReferences != null && CustomerReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerReferences)
                            {
                                item.CLIENT = Model.abecod;
                                item.clirig = rowID++;
                                connection.Execute(andefresRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // Countepart CUSTOMER_GROUPS
                        // clear
                        connection.Execute(@"DELETE FROM CUSTOMER_GROUPS WHERE cccsoc = @cccsoc AND cccode = @cccode",
                            new { cccsoc = CompanyID, cccode = Model.abecod }, transaction);
                        if (CounterpartsCustomer != null && CounterpartsCustomer.Count > 0)
                        {
                            int rowid = 1;
                            foreach (var item in CounterpartsCustomer.Where(w => !string.IsNullOrWhiteSpace(w.ccgrup) && !string.IsNullOrWhiteSpace(w.cccont) && !string.IsNullOrWhiteSpace(w.ccsott)))
                            {
                                item.ccprog = rowid++;
                                item.cccode = Model.abecod;
                                connection.Execute(customerGroupRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                    }

                    if (Model.abecfe == "F" || Model.abecfe == "E")
                    {
                        // DESFOR
                        // clear
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @fornicod",
                            new { fornicod = Model.abecod },
                            transaction);
                        if (SupplierRecipients != null && SupplierRecipients.Count > 0)
                        {
                            foreach (var item in SupplierRecipients)
                            {
                                connection.Execute(desForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTEFOR
                        // clear
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod",
                            new { nofcod = Model.abecod },
                            transaction);
                        if (SupplierNotes != null && SupplierNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierNotes)
                            {
                                item.Nofcod = Model.abecod;
                                item.nofrig = rowID++;
                                connection.Execute(noteForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // FORNAMMI
                        if (SupplierData != null)
                        {
                            if (fornammiRepository.Exists(SupplierData.Foraso, SupplierData.Foraco))
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.UPDATE_QUERY, SupplierData, transaction);
                            }
                            else
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.INSERT_QUERY, SupplierData, transaction);
                            }
                        }
                        // FORNITORI
                        if (SupplierCommercialData != null)
                        {
                            if (fornitoriRepository.Exists(SupplierCommercialData.FOCLIF))
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.UPDATE_QUERY, SupplierCommercialData, transaction);
                            }
                            else
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.INSERT_QUERY, SupplierCommercialData, transaction);
                            }
                        }
                        // RFFTB00F
                        // clear
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif",
                            new { foclif = Model.abecod },
                            transaction);
                        if (SupplierReferences != null && SupplierReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierReferences)
                            {
                                item.FOCLIF = Model.abecod;
                                item.rffrig = rowID++;
                                connection.Execute(irfftb00fRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // Countepart SUPPLIER_GROUPS
                        // clear
                        connection.Execute(@"DELETE FROM SUPPLIER_GROUPS WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode",
                            new { ccfsoc = CompanyID, cfcode = Model.abecod },
                            transaction);
                        if (Counterparts != null && Counterparts.Count > 0)
                        {
                            int rowid = 1;
                            foreach (var item in Counterparts.Where(w => !string.IsNullOrWhiteSpace(w.cfgrup) && !string.IsNullOrWhiteSpace(w.cfcont) && !string.IsNullOrWhiteSpace(w.cfsott)))
                            {
                                item.cfprog = rowid++;
                                item.cfcode = Model.abecod;
                                connection.Execute(supplierGroupRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                    }

                    if (resInsertABE != null &&
                        ((resCLIAMMI != null && CustomerData != null && CustomerData.Cliacod > 0) || (CustomerData == null || CustomerData.Cliacod == 0)) &&
                        ((resFORNAMMI != null && SupplierData != null && SupplierData.Foraco > 0) || (SupplierData == null || SupplierData.Foraco == 0)) &&
                        ((resCLIENTI != null && CustomerCommercialData != null && CustomerCommercialData.CLIENT > 0) || (CustomerCommercialData == null || CustomerCommercialData.CLIENT == 0)) &&
                        ((resFORNITORI != null && SupplierCommercialData != null && SupplierCommercialData.FOCLIF > 0) || (SupplierCommercialData == null || SupplierCommercialData.FOCLIF == 0)))
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

    public bool Update(string CompanyID, ABE Model,
        ObservableCollection<DESTINATARI>? Recipients, ObservableCollection<DESFOR>? SupplierRecipients,
        ObservableCollection<NOTECLI1>? CustomerNotes, ObservableCollection<NOTEFOR>? SupplierNotes,
        CLIAMMI? CustomerData, FORNAMMI? SupplierData,
        CLIENTI? CustomerCommercialData, FORNITORI? SupplierCommercialData,
        ObservableCollection<RFFTB00F>? SupplierReferences, ObservableCollection<ANDEFRES>? CustomerReferences,
        ObservableCollection<SUPPLIER_GROUPS>? Counterparts, ObservableCollection<CUSTOMER_GROUPS>? CounterpartsCustomer,
        ObservableCollection<ABE_EXTERN>? ExternalCodes, ANACERT? Certificate, ObservableCollection<ANACERTLEVEL1>? Certificates, SCADCLI? ExpireCustomer)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var fornitoriRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>();
                    var fornammiRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>();
                    var clientiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>();
                    var cliammiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                    var abeExternRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>();
                    var abeExternDestRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();

                    var destinatariRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();

                    var notecli1Repository = VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>();

                    var irfftb00fRepository = VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>();
                    var andefresRepository = VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>();

                    var noteForRepository = VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>();
                    var desForRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>();

                    var supplierGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>();
                    var customerGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>();

                    object? resFORNAMMI = null;
                    object? resFORNITORI = null;
                    object? resCLIAMMI = null;
                    object? resCLIENTI = null;
                    var resUpdateABE = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    #region Cancel Old
                    if (Model.abold != null)
                    {
                        connection.Execute(@"UPDATE ABE SET canceledNote = @note WHERE abecod = @abold",
                            new
                            {
                                note = $"Reso obsoleto dal codice [{Model.abecod}]",
                                abold = Model.abold
                            }, transaction);
                    }
                    #endregion

                    #region Clean possible supplier info
                    if (Model.abecfe == "C")
                    {
                        // DESFOR
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTEFOR
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod", new { nofcod = Model.abecod }, transaction);
                        // FORNAMMI
                        connection.Execute(@"DELETE FROM FORNAMMI WHERE Foraso = @cid AND Foraco = @foraco", new { cid = CompanyID, foraco = Model.abecod }, transaction);
                        // FORNITORI
                        connection.Execute(@"DELETE FROM FORNITORI WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // RFFTB00F
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // Countepart SUPPLIER_GROUPS
                        connection.Execute(@"DELETE FROM SUPPLIER_GROUPS WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode", new { ccfsoc = CompanyID, cfcode = Model.abecod }, transaction);
                        // set null objects
                        SupplierData = null;
                        SupplierCommercialData = null;
                    }
                    #endregion

                    #region Clean possible customer info
                    if (Model.abecfe == "F")
                    {
                        // DESTINATARI
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTECLI1
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli", new { notcli = Model.abecod }, transaction);
                        // CLIAMMI
                        connection.Execute(@"DELETE FROM CLIAMMI WHERE Cliasoc = @cid AND Cliacod = @client", new { cid = CompanyID, client = Model.abecod }, transaction);
                        // CLIENTI
                        connection.Execute(@"DELETE FROM CLIENTI WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // ANDEFRES
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // Countepart CUSTOMER_GROUPS
                        connection.Execute(@"DELETE FROM CUSTOMER_GROUPS WHERE cccsoc = @cccsoc AND cccode = @cccode", new { cccsoc = CompanyID, cccode = Model.abecod }, transaction);
                        // set null objects
                        CustomerData = null;
                        CustomerCommercialData = null;
                    }
                    #endregion

                    // EXTERN
                    // clear
                    connection.Execute(@"DELETE FROM ABE_EXTERN WHERE abecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                    connection.Execute(@"DELETE FROM ABE_EXTERN_DESTS WHERE abecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                    if (ExternalCodes != null && ExternalCodes.Count > 0)
                    {
                        foreach (var item in ExternalCodes)
                        {
                            connection.Execute(abeExternRepository.INSERT_QUERY, item, transaction);
                            foreach (var dest in item?.Destinations ?? new ObservableCollection<ABE_EXTERN_DESTS>())
                            {
                                connection.Execute(abeExternDestRepository.INSERT_QUERY, dest, transaction);
                            }
                        }
                    }

                    if (Model.abecfe == "C" || Model.abecfe == "E" || Model.abecfe == "P")
                    {
                        // DESTINATARI
                        // clear
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod",
                            new { cliecod = Model.abecod },
                            transaction);
                        if (Recipients != null && Recipients.Count > 0)
                        {
                            foreach (var item in Recipients)
                            {
                                connection.Execute(destinatariRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTECLI1
                        // clear
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli",
                            new { notcli = Model.abecod },
                            transaction);
                        if (CustomerNotes != null && CustomerNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerNotes)
                            {
                                item.NOTCLI = Model.abecod;
                                item.NOTRAG = Model.abers1;
                                item.notrig = rowID++;
                                connection.Execute(notecli1Repository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // CLIAMMI
                        if (CustomerData != null)
                        {
                            if (cliammiRepository.Exists(CustomerData.Cliasoc, CustomerData.Cliacod))
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.UPDATE_QUERY, CustomerData, transaction);
                            }
                            else
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.INSERT_QUERY, CustomerData, transaction);
                            }
                        }
                        // CLIENTI
                        if (CustomerCommercialData != null)
                        {
                            if (clientiRepository.Exists(CustomerCommercialData.CLIENT))
                            {
                                resCLIENTI = connection.ExecuteScalar(
                                    clientiRepository.UPDATE_QUERY, CustomerCommercialData, transaction);
                            }
                            else
                            {
                                resCLIENTI = connection.ExecuteScalar(
                                    clientiRepository.INSERT_QUERY, CustomerCommercialData, transaction);
                            }
                        }
                        // ANDEFRES
                        // clear
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client",
                            new { client = Model.abecod },
                            transaction);
                        if (CustomerReferences != null && CustomerReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerReferences)
                            {
                                item.CLIENT = Model.abecod;
                                item.clirig = rowID++;
                                connection.Execute(andefresRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // Countepart CUSTOMER_GROUPS
                        // clear
                        connection.Execute(@"DELETE FROM CUSTOMER_GROUPS WHERE cccsoc = @cccsoc AND cccode = @cccode",
                            new { cccsoc = CompanyID, cccode = Model.abecod },
                            transaction);
                        if (CounterpartsCustomer != null && CounterpartsCustomer.Count > 0)
                        {
                            int rowid = 1;
                            foreach (var item in CounterpartsCustomer.Where(w => !string.IsNullOrWhiteSpace(w.ccgrup) && !string.IsNullOrWhiteSpace(w.cccont) && !string.IsNullOrWhiteSpace(w.ccsott)).OrderBy(o => o.ccprog))
                            {
                                item.cccode = Model.abecod;
                                item.ccprog = rowid++;
                                connection.Execute(customerGroupRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                    }

                    if (Model.abecfe == "F" || Model.abecfe == "E")
                    {
                        // DESFOR
                        // clear
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @cliecod",
                            new { cliecod = Model.abecod },
                            transaction);
                        if (SupplierRecipients != null && SupplierRecipients.Count > 0)
                        {
                            foreach (var item in SupplierRecipients)
                            {
                                connection.Execute(desForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTEFOR
                        // clear
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod",
                            new { nofcod = Model.abecod },
                            transaction);
                        if (SupplierNotes != null && SupplierNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierNotes)
                            {
                                item.Nofcod = Model.abecod;
                                item.nofrig = rowID++;
                                connection.Execute(noteForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // FORNAMMI
                        if (SupplierData != null)
                        {
                            if (fornammiRepository.Exists(SupplierData.Foraso, SupplierData.Foraco))
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.UPDATE_QUERY, SupplierData, transaction);
                            }
                            else
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.INSERT_QUERY, SupplierData, transaction);
                            }
                        }
                        // FORNITORI
                        if (SupplierCommercialData != null)
                        {
                            if (fornitoriRepository.Exists(SupplierCommercialData.FOCLIF))
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.UPDATE_QUERY, SupplierCommercialData, transaction);
                            }
                            else
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.INSERT_QUERY, SupplierCommercialData, transaction);
                            }
                        }
                        // RFFTB00F
                        // clear
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif",
                            new { foclif = Model.abecod },
                            transaction);
                        if (SupplierReferences != null && SupplierReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierReferences)
                            {
                                item.FOCLIF = Model.abecod;
                                item.rffrig = rowID++;
                                connection.Execute(irfftb00fRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // Countepart SUPPLIER_GROUPS
                        // clear
                        connection.Execute(@"DELETE FROM SUPPLIER_GROUPS WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode",
                            new { ccfsoc = CompanyID, cfcode = Model.abecod },
                            transaction);
                        if (Counterparts != null && Counterparts.Count > 0)
                        {
                            int rowid = 1;
                            foreach (var item in Counterparts.Where(w => !string.IsNullOrWhiteSpace(w.cfgrup) && !string.IsNullOrWhiteSpace(w.cfcont) && !string.IsNullOrWhiteSpace(w.cfsott)).OrderBy(o => o.cfprog))
                            {
                                item.cfcode = Model.abecod;
                                item.cfprog = rowid++;
                                connection.Execute(supplierGroupRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                    }

                    if (resUpdateABE != null &&
                        ((resCLIAMMI != null && CustomerData != null && CustomerData.Cliacod > 0) || (CustomerData == null || CustomerData.Cliacod == 0) || Model.abecfe == "P") &&
                        ((resFORNAMMI != null && SupplierData != null && SupplierData.Foraco > 0) || (SupplierData == null || SupplierData.Foraco == 0) || Model.abecfe == "P") &&
                        ((resCLIENTI != null && CustomerCommercialData != null && CustomerCommercialData.CLIENT > 0) || (CustomerCommercialData == null || CustomerCommercialData.CLIENT == 0) || Model.abecfe == "P") &&
                        ((resFORNITORI != null && SupplierCommercialData != null && SupplierCommercialData.FOCLIF > 0) || (SupplierCommercialData == null || SupplierCommercialData.FOCLIF == 0)) || Model.abecfe == "P")
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


    public string? CanDelete(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multiAccount = connection.QueryMultiple(
                                    @"SELECT COUNT(*) FROM ANFAT00F WHERE AFCOCL = @id;
                                            SELECT COUNT(*) FROM OFFET00F WHERE OFTCOCL = @id;
                                            SELECT COUNT(*) FROM ORDIT00F WHERE OTCLIE = @id;
                                            SELECT COUNT(*) FROM FATTT00F WHERE FTCODC = @id;
                                            SELECT COUNT(*) FROM BOLLT00F WHERE BTCODC = @id;
                                            SELECT COUNT(*) FROM PNTESTATA WHERE N1CLFO = @id;
                                            SELECT COUNT(*) FROM PNRIGHE WHERE n1clie = @id;
                                            SELECT COUNT(*) FROM pro_ordine WHERE ClienteID = @id;
                                            SELECT COUNT(*) FROM ACC_PLAFOND WHERE Cliacod = @id;
                                            SELECT COUNT(*) FROM CRM_LISCLI WHERE customerID = @id;
                                            SELECT COUNT(*) FROM ASSET00F WHERE customer_id = @id;"
            , new { id = ID });
            var anfat = multiAccount.Read<int?>().Single() ?? 0;
            var offers = multiAccount.Read<int?>().Single() ?? 0;
            var orders = multiAccount.Read<int?>().Single() ?? 0;
            var invoices = multiAccount.Read<int?>().Single() ?? 0;
            var ddt = multiAccount.Read<int?>().Single() ?? 0;
            var pnt = multiAccount.Read<int?>().Single() ?? 0;
            var pnr = multiAccount.Read<int?>().Single() ?? 0;
            var prods = multiAccount.Read<int?>().Single() ?? 0;
            var plaf = multiAccount.Read<int?>().Single() ?? 0;
            var liscli = multiAccount.Read<int?>().Single() ?? 0;
            var assets = multiAccount.Read<int?>().Single() ?? 0;

            if (anfat == 0 && offers == 0 && orders == 0 && invoices == 0 && ddt == 0 && pnt == 0 && pnr == 0 && prods == 0 && plaf == 0 && liscli == 0 && assets == 0)
            {
                return null;
            }
            else
            {
                return $"Impossibile cancellare il codice anagrafico [{ID.ToString("N0")}] perchè in uso nelle seguenti gestioni:\n\n" +
                            $"{(anfat > 0 ? "- Analisi di fattibilità\n" : null)}" +
                            $"{(offers > 0 ? "- Offerte clienti\n" : null)}" +
                            $"{(orders > 0 ? "- Ordini clienti\n" : null)}" +
                            $"{(invoices > 0 ? "- Fatture clienti\n" : null)}" +
                            $"{(ddt > 0 ? "- DDT\n" : null)}" +
                            $"{(pnt > 0 ? "- Prima nota\n" : null)}" +
                            $"{(pnr > 0 ? "- Dettagli di prima nota\n" : null)}" +
                            $"{(prods > 0 ? "- Ordini di produzione\n" : null)}" +
                            $"{(plaf > 0 ? "- Dichiarazioni d'intento\n" : null)}" +
                            $"{(liscli > 0 ? "- Listini cliente\n" : null)}" +
                            $"{(assets > 0 ? "- Gestione assets\n" : null)}";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(ABE Model, string? ReasonText, string? UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var check = CanDelete(Model.abecod);
            if (string.IsNullOrWhiteSpace(check))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(ReasonText))
                        {
                            // delete
                            connection.Execute("DELETE FROM ABE_EXTERN WHERE abecod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ABE_EXTERN_DESTS WHERE abecod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM DESFOR WHERE fornicod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM NOTEFOR WHERE NOFCOD = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM FORNAMMI WHERE Foraco = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM FORNITORI WHERE FOCLIF = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM RFFTB00F WHERE FOCLIF = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM SUPPLIER_GROUPS WHERE cfcode = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM DESTINATARI WHERE cliecod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM NOTECLI1 WHERE NOTCLI = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM CLIAMMI WHERE Cliacod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM CLIENTI WHERE CLIENT = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ANDEFRES WHERE CLIENT = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ABE WHERE abecod = @id", new { id = Model.abecod }, transaction);
                        }
                        else
                        {
                            // cancel
                            Model.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            Model.canceledUserID = UserID;
                            Model.canceledNote = ReasonText;
                            connection.Execute(UPDATE_QUERY, Model, transaction);
                        }

                        transaction.Commit();
                        ErrorHandler.Show($"Operazione eseguita con successo, codice {(string.IsNullOrWhiteSpace(ReasonText) ? "eliminato definitivamente" : "annullato")}");
                        return true;
                    }
                    catch (Exception exc)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(exc.Message);
                        return false;
                    }
                }
            }
            else
            {
                ErrorHandler.Show(check);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(ABE? Model, FORNAMMI? SupplierData, CLIAMMI? CustomerData, FORNITORI? SupplierCommercialData, CLIENTI? CustomerCommercialData, int VATLength, bool IsInsert)
    {
        try
        {
            if (Model == null)
                return "Dati anagrafici mancanti";

            // Code validation
            if (IsInsert)
            {
                if (Model.abecod == 0 || Exists(Model.abecod))
                    return "Il codice scelto è già in uso o non valido";
            }

            if (string.IsNullOrWhiteSpace(Model.abecfe))
                return "Il tipo di utilizzo è obbligatorio";

            if (string.IsNullOrWhiteSpace(Model.abers1))
                return "La ragione sociale è obbligatoria";

            // Dispatch based on type
            return Model.abecfe == "P"
                ? ValidateProspect(Model)
                : ValidateCustomerOrSupplier(Model, SupplierData, CustomerData, SupplierCommercialData, CustomerCommercialData, VATLength);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string? ValidateProspect(ABE Model)
    {
        if (string.IsNullOrWhiteSpace(Model.abeind))
            return "L'indirizzo è obbligatorio per registrare un prospect";

        if (string.IsNullOrWhiteSpace(Model.abeloc))
            return "La città del prospect è obbligatoria";

        if (!Model.abecap.HasValue || Model.abecap.Value <= 0)
            return "Il C.A.P. del prospect è obbligatorio";

        if (string.IsNullOrWhiteSpace(Model.abepro))
            return "La provincia del prospect è obbligatoria";

        if (string.IsNullOrWhiteSpace(Model.abepiv) &&
            string.IsNullOrWhiteSpace(Model.abecfi))
            return "La partita IVA o il codice fiscale sono obbligatori per il prospect";

        return null;
    }

    private string? ValidateCustomerOrSupplier(ABE Model, FORNAMMI? SupplierData, CLIAMMI? CustomerData, FORNITORI? SupplierCommercialData, CLIENTI? CustomerCommercialData, int VATLength)
    {
        // ISO + Tipo rules
        bool isoOk =
            (!string.IsNullOrWhiteSpace(Model.isocod) &&
             (Model.abetpo == "1" || Model.abetpo == "2" || Model.abetpo == "4")) ||
            (string.IsNullOrWhiteSpace(Model.isocod) && string.IsNullOrWhiteSpace(Model.abetpo)) ||
            Model.abetpo == "3";

        if (!isoOk)
            return "Il codice ISO ed il Tipo sono obbligatori";

        // VAT required?
        if (Model.abetpo == "2" && string.IsNullOrWhiteSpace(Model.abepiv))
            return "La partita IVA non è necessaria per questo tipo di società";

        if ((Model.abetpo == "1" || Model.abetpo == "4") &&
            string.IsNullOrWhiteSpace(Model.abepiv))
            return "La partita IVA è obbligatoria per questo tipo di società";

        // CF required?
        if ((Model.abetpo == "1" || Model.abetpo == "3") &&
            string.IsNullOrWhiteSpace(Model.abecfi))
            return "Il codice fiscale è obbligatorio per questo tipo di società";

        if (Model.abetpo == "2" &&
            !string.IsNullOrWhiteSpace(Model.abecfi))
            return "Il codice fiscale non è necessario per questo tipo di società";

        // VAT length
        if ((Model.abetpo == "1" || Model.abetpo == "4") &&
            (Model.abepiv?.Trim().Length != VATLength))
            return "La lunghezza della partita IVA è discordante con il codice ISO";

        // Mandatory address
        if (string.IsNullOrWhiteSpace(Model.abeind))
            return "L'indirizzo è obbligatorio";

        // Duplicate checks
        var existingVAT = ExistsVAT(Model.abecod, Model.abepiv);
        var existingCF = ExistsCF(Model.abecod, Model.abecfi);

        // CF conflict
        if (existingCF != null &&
            !(Model.abetpo == "2" || Model.abetpo == "4"))
        {
            if (!ConfirmHandler.Confirm(
                    $"Esiste gia' un'anagrafica con lo stesso codice fiscale.\n\n" +
                    $"{existingCF.abecod} - {existingCF.abers1} [{existingCF.abecfi}]\n\n" +
                    "Si desidera continuare comunque ?"))
                return "Esiste gia' un'anagrafica con lo stesso codice fiscale";
        }

        // VAT conflict
        if (existingVAT != null &&
           (Model.abetpo == "1" || Model.abetpo == "3" || Model.abetpo == "4"))
        {
            if (!ConfirmHandler.Confirm(
                    $"Esiste gia' un'anagrafica con la stessa partita IVA.\n\n" +
                    $"{existingVAT.abecod} - {existingVAT.abers1} [{existingVAT.abepiv}]\n\n" +
                    "Si desidera continuare comunque ?"))
                return "Esiste gia' un'anagrafica con la stessa partita IVA";
        }

        // Payment type rules
        if (Model.abecfe == "F" && string.IsNullOrWhiteSpace(SupplierData?.pfocod))
            return "Il tipo pagamento cliente è obbligatorio";

        if (Model.abecfe == "C" && string.IsNullOrWhiteSpace(CustomerData?.pclcod))
            return "Il tipo pagamento fornitore è obbligatorio";

        // Invoice template
        if (Model.abfatfile && Model.abfatfileid == null)
            return "Il tipo di personalizazzione fattura è obbligatorio se flaggato";

        // Accounting constraints
        bool hasCL = !string.IsNullOrWhiteSpace(CustomerData?.clcont);
        bool hasFO = !string.IsNullOrWhiteSpace(SupplierData?.foCONT);

        if (Model.abecfe == "E" && !(hasCL && hasFO))
            return "I conti contabili cliente e/o fornitore sono obbligatori";

        if (Model.abecfe == "C" && !hasCL)
            return "I conti contabili cliente e/o fornitore sono obbligatori";

        if (Model.abecfe == "F" && !hasFO)
            return "I conti contabili cliente e/o fornitore sono obbligatori";

        return null;
    }
    #endregion
}

public class ABEUfpRepository : RepositoryBase, IABERepository
{
    public ABEUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ABE>? GetCompanyList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                $@"SELECT 
a.*,  
CASE WHEN COALESCE(x.clitop, 'N') = 'S' THEN 1 ELSE 0 END AS IsTop,
abetipo,
cliente.clsosp as CLSOSP,
fornitore.fososp as FOSOSP

                        FROM ANAG_BASE as a
                        OUTER APPLY (
                            SELECT TOP 1 clsosp
                            FROM ANAG_CLIENTI c
                            WHERE c.CLIENT = a.abecod) cliente
                        OUTER APPLY (
                            SELECT TOP 1 fososp
                            FROM ANAG_FORNITORI f
                            WHERE f.FOCLIF = a.abecod) fornitore
                        OUTER APPLY (
                            SELECT TOP 1 clitop 
                            FROM CLIAMMI b 
                            WHERE b.cliasoc = @cid AND b.cliacod = a.abecod) x", new { cid = CompanyID });

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                $@"SELECT a.* FROM ANAG_BASE as a");

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                "SELECT abecod, TRIM(abers1) AS abers1, TRIM(abers2) AS abers2, abecfe FROM ANAG_BASE");

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightListObsoleti()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                "SELECT abecod, TRIM(abers1) AS abers1 FROM ANAG_BASE");

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightList(string ItemType)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                $@"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2 FROM ANAG_BASE as a
                    WHERE (a.ABECFE = 'E' OR a.ABECFE = @abecfe)",
                new { abecfe = ItemType }).ToList();
            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetLightList(string CompanyID ,string ItemType)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                $@"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2, 
                    CASE 
                        WHEN c.clisister = 'S' THEN 'TRUE' 
                    ELSE 'FALSE' 
                        END AS IsSister
                    FROM ANAG_BASE as a
                    LEFT OUTER JOIN CLIAMMI as c ON c.cliasoc = @CompanyID AND c.cliacod = a.abecod
                    WHERE (a.ABECFE = 'E' OR a.ABECFE = @abecfe)",
                new { abecfe = ItemType, CompanyID }).ToList();
            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }


    public ObservableCollection<ABE>? GetCustomersLightListActive(string ItemType, bool IncludeProspect = false)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ABE>(
                $@"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2, abeind, abecap, abeloc, abepro, abepiv, abecfi, abecfe, b.abecod AS IsObsolete FROM ANAG_BASE AS a
                        LEFT JOIN ANAG_CLIENTI AS c ON c.CLIENT = a.abecod
                        OUTER APPLY (
                            SELECT TOP 1 abecod 
                            FROM dbo.ANAG_BASE b 
                            WHERE b.abold = a.abecod
                        ) b
                        WHERE (a.ABECFE = 'E' OR a.ABECFE = @abecfe {(IncludeProspect ? " OR a.ABECFE = 'P' " : null)}) AND (c.CLSOSP <> 'S' OR CLSOSP IS NULL) AND canceled IS NULL",
                new { abecfe = ItemType });

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE>? GetCustomersLightListActiveForExternals(string ExternalID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ABE>(
                @"SELECT a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2, abeind, abecap, abeloc, abepro, abepiv, abecfi FROM ANAG_BASE AS a
                        INNER JOIN ANAG_CLIENTI AS c ON c.CLIENT = a.abecod
                        INNER JOIN ABE_EXTERN AS e ON e.abecod = a.abecod
                        WHERE e.abeextcode = @externalid AND (c.CLSOSP <> 'S' OR CLSOSP IS NULL) AND canceled IS NULL",
                new { externalid = ExternalID });

            return new ObservableCollection<ABE>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? Get(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                "SELECT * FROM ANAG_BASE WHERE abecod = @id ",
                new { id = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? Get(string CompanyID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>($@"SELECT 
a.*,  
CASE WHEN COALESCE(x.clitop, 'N') = 'S' THEN 1 ELSE 0 END AS IsTop,
abetipo
FROM ANAG_BASE as a
                        OUTER APPLY (
                            SELECT TOP 1 clitop 
                            FROM CLIAMMI b 
                            WHERE b.cliasoc = @cid AND b.cliacod = a.abecod
                    ) x
WHERE a.abecod = @eid", new { cid = CompanyID, eid = ID }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? GetSupplierFromVAT(string VATID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var abe = connection.Query<ABE>(
                $@"SELECT a.* FROM ANAG_BASE as a 
OUTER APPLY (
    SELECT TOP 1 fososp
    FROM ANAG_FORNITORI f
    WHERE f.FOCLIF = a.abecod) fornitore
WHERE a.abepiv =@VATID AND fornitore.fososp <> 'S' AND (abecfe = 'E' OR abecfe = 'F')", new { VATID = VATID }).FirstOrDefault();

            return abe;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANAG_BASE WHERE abecod = @id",
                new { id = ID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ABE? ExistsVAT(string VATID, string? EntityType = null)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                $"SELECT * FROM ANAG_BASE WHERE abepiv = @abepiv {(string.IsNullOrWhiteSpace(EntityType) ? null : "AND (abecfe = 'E' OR abecfe = @abecfe)")}",
                new { abepiv = VATID, abecfe = EntityType }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? ExistsVAT(int ID, string? VATID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                "SELECT * FROM ANAG_BASE WHERE abecod <> @id AND abepiv = @abepiv",
                new { id = ID, abepiv = VATID }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? ExistsCF(string CF, string? EntityType = null)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                $"SELECT * FROM ANAG_BASE WHERE abecfi = @abecfi {(string.IsNullOrWhiteSpace(EntityType) ? null : "AND (abecfe = 'E' OR abecfe = @abecfe)")}",
                new { abecfi = CF, abecfe = EntityType }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE? ExistsCF(int ID, string? CF)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE>(
                "SELECT * FROM ANAG_BASE WHERE abecod <> @id AND abecfi = @abecfi",
                new { id = ID, abecfi = CF }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Recupera una lista di tutti i codici disponibili non utilizzati.
    /// ATTENZIONE : questo è l'unico modo per farlo facendo una sola chiamata al database, altrimenti bisognerebbe farne
    /// migliaia o utilizzare una tabella temporanea o una sequenza; questo metodo funziona ma ha un "bug", in caso di 
    /// diversi codici liberi consecutivi, mostra solo il primo, ad esempio se nell'intervallo tra 7000 e 7010 (esclusi)
    /// sono tutti liberi, questa query ritornerà solo 7001: per quello che ci serve per ora va bene così, metto un TODO 
    /// come promemoria
    /// </summary>
    /// <returns>ObservableCollection<int></returns>
    public ObservableCollection<int>? GetFreeIDList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Query<int>(
                "SELECT DISTINCT abecod+1 FROM ANAG_BASE WHERE abecod + 1 NOT IN (SELECT DISTINCT abecod FROM ANAG_BASE);");
            if (result != null && result.Count() > 0)
                return new ObservableCollection<int>(result.ToList());
            else
                return new ObservableCollection<int>(Enumerable.Range(1, 100).ToList());
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ANAG_BASE (abecod,abers1,abers2,abeind,abecap,abeloc,abepro,nazcod,abetpo,isocod,abepiv,abecfi,soctip,abedna,abelna,abepna,abesex,abeais,abeccc,abetri,abeinl,abecal,abelol,abeprl,abecfe,abetipo) OUTPUT INSERTED.rv VALUES(@abecod,@abers1,@abers2,@abeind,@abecap,@abeloc,@abepro,@nazcod,@abetpo,@isocod,@abepiv,@abecfi,@soctip,@abedna,@abelna,@abepna,@abesex,@abeais,@abeccc,@abetri,@abeinl,@abecal,@abelol,@abeprl,@abecfe,@abetipo)";
    public string UPDATE_QUERY => "UPDATE ANAG_BASE SET abecod = @abecod,abers1 = @abers1,abers2 = @abers2,abeind = @abeind,abecap = @abecap,abeloc = @abeloc,abepro = @abepro,nazcod = @nazcod,abetpo = @abetpo,isocod = @isocod,abepiv = @abepiv,abecfi = @abecfi,soctip = @soctip,abedna = @abedna,abelna = @abelna,abepna = @abepna,abesex = @abesex,abeais = @abeais,abeccc = @abeccc,abetri = @abetri,abeinl = @abeinl,abecal = @abecal,abelol = @abelol,abeprl = @abeprl,abecfe = @abecfe, abetipo = @abetipo OUTPUT INSERTED.rv WHERE abecod = @abecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ANAG_BASE OUTPUT DELETED.rv WHERE abecod = @abecod AND rv = @rv";
    public bool Insert(string CompanyID, ABE Model,
        ObservableCollection<DESTINATARI>? Recipients, ObservableCollection<DESFOR>? SupplierRecipients,
        ObservableCollection<NOTECLI1>? CustomerNotes, ObservableCollection<NOTEFOR>? SupplierNotes,
        CLIAMMI? CustomerData, FORNAMMI? SupplierData,
        CLIENTI? CustomerCommercialData, FORNITORI? SupplierCommercialData,
        ObservableCollection<RFFTB00F>? SupplierReferences, ObservableCollection<ANDEFRES>? CustomerReferences,
        ObservableCollection<SUPPLIER_GROUPS>? Counterparts, ObservableCollection<CUSTOMER_GROUPS>? CounterpartsCustomer,
        ObservableCollection<ABE_EXTERN>? ExternalCodes, ANACERT? Certificate, ObservableCollection<ANACERTLEVEL1>? Certificates, SCADCLI? ExpireCustomer)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var fornitoriRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>();
                    var fornammiRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>();
                    var clientiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>();
                    var cliammiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                    var abeExternRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>();
                    var abeExternDestRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();

                    var destinatariRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();

                    var notecli1Repository = VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>();

                    var irfftb00fRepository = VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>();
                    var andefresRepository = VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>();

                    var noteForRepository = VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>();
                    var desForRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>();

                    var supplierGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>();
                    var customerGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>();

                    var anacertRepository = VulpesServiceProvider.Provider.GetRequiredService<IANACERTRepository>();
                    var anacertlevel1Repository = VulpesServiceProvider.Provider.GetRequiredService<IANACERTLEVEL1Repository>();
                    var scadcliRepo = VulpesServiceProvider.Provider.GetRequiredService<ISCADCLIService>();

                    object? resFORNAMMI = null;
                    object? resFORNITORI = null;
                    object? resCLIAMMI = null;
                    object? resCLIENTI = null;
                    object? resCERTIFICATE = null;

                    Model.abetipo = Model.abetipo ?? "N";

                    var resInsertABE = connection.ExecuteScalar(INSERT_QUERY, Model, transaction);

                    #region Clean possible supplier info
                    if (Model.abecfe == "C")
                    {
                        // DESFOR
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTEFOR
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod", new { nofcod = Model.abecod }, transaction);
                        // FORNAMMI
                        connection.Execute(@"DELETE FROM FORNAMMI WHERE Foraso = @cid AND Foraco = @foraco", new { cid = CompanyID, foraco = Model.abecod }, transaction);
                        // FORNITORI
                        connection.Execute(@"DELETE FROM ANAG_FORNITORI WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // RFFTB00F
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // Countepart SUPPLIER_GROUPS
                        connection.Execute(@"DELETE FROM PNCLIFORLEVEL1 WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode", new { ccfsoc = CompanyID, cfcode = Model.abecod }, transaction);
                        // set null objects
                        SupplierData = null;
                        SupplierCommercialData = null;
                    }
                    #endregion

                    #region Clean possible customer info
                    if (Model.abecfe == "F")
                    {
                        // DESTINATARI
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTECLI1
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli", new { notcli = Model.abecod }, transaction);
                        // CLIAMMI
                        connection.Execute(@"DELETE FROM CLIAMMI WHERE Cliasoc = @cid AND Cliacod = @client", new { cid = CompanyID, client = Model.abecod }, transaction);
                        // CLIENTI
                        connection.Execute(@"DELETE FROM ANAG_CLIENTI WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // ANDEFRES
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // set null objects
                        CustomerData = null;
                        CustomerCommercialData = null;
                    }
                    #endregion

                    if (Model.abecfe == "C" || Model.abecfe == "E" || Model.abecfe == "P")
                    {
                        // DESTINATARI
                        // clear
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod",
                            new { cliecod = Model.abecod },
                            transaction);
                        if (Recipients != null && Recipients.Count > 0)
                        {
                            foreach (var item in Recipients)
                            {
                                connection.Execute(destinatariRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTECLI1
                        // clear
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli",
                            new { notcli = Model.abecod },
                            transaction);
                        if (CustomerNotes != null && CustomerNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerNotes)
                            {
                                item.NOTCLI = Model.abecod;
                                item.NOTRAG = Model.abers1;
                                item.notrig = rowID++;
                                connection.Execute(notecli1Repository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // CLIAMMI
                        if (CustomerData != null)
                        {
                            if (cliammiRepository.Exists(CustomerData.Cliasoc, CustomerData.Cliacod))
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.UPDATE_QUERY, CustomerData, transaction);
                            }
                            else
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.INSERT_QUERY, CustomerData, transaction);
                            }
                        }
                        // CLIENTI
                        if (CustomerCommercialData != null)
                        {
                            CustomerCommercialData.CLSOSP = CustomerCommercialData.CLSOSP ?? "N";

                            if (clientiRepository.Exists(CustomerCommercialData.CLIENT))
                            {
                                resCLIENTI = connection.ExecuteScalar(clientiRepository.UPDATE_QUERY, CustomerCommercialData, transaction);
                            }
                            else
                            {
                                resCLIENTI = connection.ExecuteScalar(clientiRepository.INSERT_QUERY, CustomerCommercialData, transaction);
                            }
                        }
                        // ANDEFRES
                        // clear
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client",
                            new { client = Model.abecod },
                            transaction);
                        if (CustomerReferences != null && CustomerReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerReferences)
                            {
                                item.CLIENT = Model.abecod;
                                item.clirig = rowID++;
                                connection.Execute(andefresRepository.INSERT_QUERY, item, transaction);
                            }
                        }

                        //ANACERTLEVEL1
                        connection.Execute(@"DELETE FROM ANACERTLEVEL1 WHERE anacli = @client",
                            new { client = Model.abecod },
                            transaction);

                        if (Certificates != null && Certificates.Count > 0)
                        {
                            short rowID = 1;
                            foreach (var item in Certificates)
                            {
                                item.anacli = Model.abecod;
                                item.anapro = rowID++;
                                connection.Execute(anacertlevel1Repository.INSERT_QUERY, item, transaction);
                            }

                            //ANACERT
                            if (Certificate != null)
                            {
                                Certificate.ananmax = rowID;

                                if (anacertRepository.Exists(Certificate.anacli))
                                {
                                    resCERTIFICATE = connection.ExecuteScalar(anacertRepository.UPDATE_QUERY, Certificate, transaction);
                                }
                                else
                                {
                                    resCERTIFICATE = connection.ExecuteScalar(anacertRepository.INSERT_QUERY, Certificate, transaction);
                                }
                            }
                        }
                    }

                    if (Model.abecfe == "F" || Model.abecfe == "E")
                    {
                        // DESFOR
                        // clear
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @fornicod",
                            new { fornicod = Model.abecod },
                            transaction);
                        if (SupplierRecipients != null && SupplierRecipients.Count > 0)
                        {
                            foreach (var item in SupplierRecipients)
                            {
                                connection.Execute(desForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTEFOR
                        // clear
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod",
                            new { nofcod = Model.abecod },
                            transaction);
                        if (SupplierNotes != null && SupplierNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierNotes)
                            {
                                item.Nofcod = Model.abecod;
                                item.nofrig = rowID++;
                                connection.Execute(noteForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // FORNAMMI
                        if (SupplierData != null)
                        {
                            if (fornammiRepository.Exists(SupplierData.Foraso, SupplierData.Foraco))
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.UPDATE_QUERY, SupplierData, transaction);
                            }
                            else
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.INSERT_QUERY, SupplierData, transaction);
                            }
                        }
                        // FORNITORI
                        if (SupplierCommercialData != null)
                        {
                            SupplierCommercialData.FOSOSP = SupplierCommercialData.FOSOSP ?? "N";

                            if (fornitoriRepository.Exists(SupplierCommercialData.FOCLIF))
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.UPDATE_QUERY, SupplierCommercialData, transaction);
                            }
                            else
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.INSERT_QUERY, SupplierCommercialData, transaction);
                            }
                        }
                        // RFFTB00F
                        // clear
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif",
                            new { foclif = Model.abecod },
                            transaction);
                        if (SupplierReferences != null && SupplierReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierReferences)
                            {
                                item.FOCLIF = Model.abecod;
                                item.rffrig = rowID++;
                                connection.Execute(irfftb00fRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // Countepart SUPPLIER_GROUPS
                        // clear
                        connection.Execute(@"DELETE FROM PNCLIFORLEVEL1 WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode",
                            new { ccfsoc = CompanyID, cfcode = Model.abecod },
                            transaction);
                        if (Counterparts != null && Counterparts.Count > 0)
                        {
                            int rowid = 1;
                            foreach (var item in Counterparts.Where(w => !string.IsNullOrWhiteSpace(w.cfgrup) && !string.IsNullOrWhiteSpace(w.cfcont) && !string.IsNullOrWhiteSpace(w.cfsott)))
                            {
                                item.cfprog = rowid++;
                                item.cfcode = Model.abecod;
                                connection.Execute(supplierGroupRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                    }

                    object? resExpireCustomer = null;
                    if (ExpireCustomer != null)
                    {
                        ExpireCustomer.scclicod = Model.abecod;
                        ExpireCustomer.descli = Model.abers1;

                        if (ExpireCustomer.Scagior == 0)
                        {
                            resExpireCustomer = connection.ExecuteScalar(scadcliRepo.DELETE_QUERY, ExpireCustomer, transaction);
                            ExpireCustomer = null;
                        }
                        else
                        {
                            if (scadcliRepo.Exists(Model.abecod))
                            {
                                resExpireCustomer = connection.ExecuteScalar(scadcliRepo.UPDATE_QUERY, ExpireCustomer, transaction);
                            }
                            else
                            {
                                resExpireCustomer = connection.ExecuteScalar(scadcliRepo.INSERT_QUERY, ExpireCustomer, transaction);
                            }
                        }
                    }


                    if (resInsertABE != null &&
                        ((resExpireCustomer != null && ExpireCustomer != null) || ExpireCustomer == null) &&
                        ((resCLIAMMI != null && CustomerData != null && CustomerData.Cliacod > 0) || (CustomerData == null || CustomerData.Cliacod == 0)) &&
                        ((resFORNAMMI != null && SupplierData != null && SupplierData.Foraco > 0) || (SupplierData == null || SupplierData.Foraco == 0)) &&
                        ((resCLIENTI != null && CustomerCommercialData != null && CustomerCommercialData.CLIENT > 0) || (CustomerCommercialData == null || CustomerCommercialData.CLIENT == 0)) &&
                        ((resFORNITORI != null && SupplierCommercialData != null && SupplierCommercialData.FOCLIF > 0) || (SupplierCommercialData == null || SupplierCommercialData.FOCLIF == 0)) &&
                        ((resCERTIFICATE != null) && Certificates != null && Certificates.Count > 0 && Certificate != null) || (Certificates == null || Certificates.Count == 0 || Certificate == null))
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
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message.ToString());
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

    public bool Update(string CompanyID, ABE Model,
        ObservableCollection<DESTINATARI>? Recipients, ObservableCollection<DESFOR>? SupplierRecipients,
        ObservableCollection<NOTECLI1>? CustomerNotes, ObservableCollection<NOTEFOR>? SupplierNotes,
        CLIAMMI? CustomerData, FORNAMMI? SupplierData,
        CLIENTI? CustomerCommercialData, FORNITORI? SupplierCommercialData,
        ObservableCollection<RFFTB00F>? SupplierReferences, ObservableCollection<ANDEFRES>? CustomerReferences,
        ObservableCollection<SUPPLIER_GROUPS>? Counterparts, ObservableCollection<CUSTOMER_GROUPS>? CounterpartsCustomer,
        ObservableCollection<ABE_EXTERN>? ExternalCodes, ANACERT? Certificate, ObservableCollection<ANACERTLEVEL1>? Certificates, SCADCLI? ExpireCustomer)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var fornitoriRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>();
                    var fornammiRepository = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>();
                    var clientiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>();
                    var cliammiRepository = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>();

                    var abeExternRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>();
                    var abeExternDestRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();

                    var destinatariRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();

                    var notecli1Repository = VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>();

                    var irfftb00fRepository = VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>();
                    var andefresRepository = VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>();

                    var noteForRepository = VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>();
                    var desForRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>();

                    var supplierGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>();
                    var customerGroupRepository = VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>();

                    var anacertRepository = VulpesServiceProvider.Provider.GetRequiredService<IANACERTRepository>();
                    var anacertlevel1Repository = VulpesServiceProvider.Provider.GetRequiredService<IANACERTLEVEL1Repository>();

                    var scadcliRepo = VulpesServiceProvider.Provider.GetRequiredService<ISCADCLIService>();

                    object? resFORNAMMI = null;
                    object? resFORNITORI = null;
                    object? resCLIAMMI = null;
                    object? resCLIENTI = null;
                    object? resCERTIFICATE = null;

                    Model.abetipo = Model.abetipo ?? "N";

                    var resUpdateABE = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    #region Cancel Old
                    if (Model.abold != null)
                    {
                        connection.Execute(@"UPDATE ABE SET canceledNote = @note WHERE abecod = @abold",
                            new
                            {
                                note = $"Reso obsoleto dal codice [{Model.abecod}]",
                                abold = Model.abold
                            }, transaction);
                    }
                    #endregion

                    #region Clean possible supplier info
                    if (Model.abecfe == "C")
                    {
                        // DESFOR
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTEFOR
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE NOFCOD = @nofcod", new { nofcod = Model.abecod }, transaction);
                        // FORNAMMI
                        connection.Execute(@"DELETE FROM FORNAMMI WHERE Foraso = @cid AND Foraco = @foraco", new { cid = CompanyID, foraco = Model.abecod }, transaction);
                        // FORNITORI
                        connection.Execute(@"DELETE FROM ANAG_FORNITORI WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // RFFTB00F
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif", new { foclif = Model.abecod }, transaction);
                        // Countepart SUPPLIER_GROUPS
                        connection.Execute(@"DELETE FROM PNCLIFORLEVEL1 WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode", new { ccfsoc = CompanyID, cfcode = Model.abecod }, transaction);
                        // set null objects
                        SupplierData = null;
                        SupplierCommercialData = null;
                    }
                    #endregion

                    #region Clean possible customer info
                    if (Model.abecfe == "F")
                    {
                        // DESTINATARI
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod", new { cliecod = Model.abecod }, transaction);
                        // NOTECLI1
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE NOTCLI = @notcli", new { notcli = Model.abecod }, transaction);
                        // CLIAMMI
                        connection.Execute(@"DELETE FROM CLIAMMI WHERE Cliasoc = @cid AND Cliacod = @client", new { cid = CompanyID, client = Model.abecod }, transaction);
                        // CLIENTI
                        connection.Execute(@"DELETE FROM ANAG_CLIENTI WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // ANDEFRES
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client", new { client = Model.abecod }, transaction);
                        // set null objects
                        CustomerData = null;
                        CustomerCommercialData = null;
                    }
                    #endregion

                    if (Model.abecfe == "C" || Model.abecfe == "E" || Model.abecfe == "P")
                    {
                        // DESTINATARI
                        // clear
                        connection.Execute(@"DELETE FROM DESTINATARI WHERE cliecod = @cliecod",
                            new { cliecod = Model.abecod },
                            transaction);
                        if (Recipients != null && Recipients.Count > 0)
                        {
                            foreach (var item in Recipients)
                            {
                                connection.Execute(destinatariRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTECLI1
                        // clear
                        connection.Execute(@"DELETE FROM NOTECLI1 WHERE nosoc = @nosoc AND NOTCLI = @notcli",
                            new { nosoc = CompanyID, notcli = Model.abecod },
                            transaction);
                        if (CustomerNotes != null && CustomerNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerNotes)
                            {
                                item.nosoc = CompanyID;
                                item.NOTCLI = Model.abecod;
                                item.NOTRAG = Model.abers1;
                                item.notrig = rowID++;
                                connection.Execute(notecli1Repository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // CLIAMMI
                        if (CustomerData != null)
                        {
                            if (cliammiRepository.Exists(CustomerData.Cliasoc, CustomerData.Cliacod))
                            {

                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.UPDATE_QUERY, CustomerData, transaction);
                            }
                            else
                            {
                                resCLIAMMI = connection.ExecuteScalar(cliammiRepository.INSERT_QUERY, CustomerData, transaction);
                            }
                        }
                        // CLIENTI
                        if (CustomerCommercialData != null)
                        {
                            if (clientiRepository.Exists(CustomerCommercialData.CLIENT))
                            {
                                resCLIENTI = connection.ExecuteScalar(
                                    clientiRepository.UPDATE_QUERY, CustomerCommercialData, transaction);
                            }
                            else
                            {
                                resCLIENTI = connection.ExecuteScalar(
                                    clientiRepository.INSERT_QUERY, CustomerCommercialData, transaction);
                            }
                        }
                        // ANDEFRES
                        // clear
                        connection.Execute(@"DELETE FROM ANDEFRES WHERE CLIENT = @client",
                            new { client = Model.abecod },
                            transaction);
                        if (CustomerReferences != null && CustomerReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in CustomerReferences)
                            {
                                item.CLIENT = Model.abecod;
                                item.clirig = rowID++;
                                connection.Execute(andefresRepository.INSERT_QUERY, item, transaction);
                            }
                        }

                        //ANACERTLEVEL1
                        connection.Execute(@"DELETE FROM ANACERTLEVEL1 WHERE anacli = @client",
                            new { client = Model.abecod },
                            transaction);

                        if (Certificates != null && Certificates.Count > 0)
                        {
                            short rowID = 1;
                            foreach (var item in Certificates)
                            {
                                item.anacli = Model.abecod;
                                item.anapro = rowID++;
                                connection.Execute(anacertlevel1Repository.INSERT_QUERY, item, transaction);
                            }

                            //ANACERT
                            if (Certificate != null)
                            {
                                Certificate.ananmax = rowID;

                                if (anacertRepository.Exists(Certificate.anacli))
                                {
                                    resCERTIFICATE = connection.ExecuteScalar(anacertRepository.UPDATE_QUERY, Certificate, transaction);
                                }
                                else
                                {
                                    resCERTIFICATE = connection.ExecuteScalar(anacertRepository.INSERT_QUERY, Certificate, transaction);
                                }
                            }
                        }
                    }

                    if (Model.abecfe == "F" || Model.abecfe == "E")
                    {
                        // DESFOR
                        // clear
                        connection.Execute(@"DELETE FROM DESFOR WHERE fornicod = @cliecod",
                            new { cliecod = Model.abecod },
                            transaction);
                        if (SupplierRecipients != null && SupplierRecipients.Count > 0)
                        {
                            foreach (var item in SupplierRecipients)
                            {
                                connection.Execute(desForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // NOTEFOR
                        // clear
                        connection.Execute(@"DELETE FROM NOTEFOR WHERE nofsoc = @nofsoc AND NOFCOD = @nofcod",
                            new { nofsoc = CompanyID, nofcod = Model.abecod },
                            transaction);
                        if (SupplierNotes != null && SupplierNotes.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierNotes)
                            {
                                item.nofsoc = CompanyID;
                                item.Nofcod = Model.abecod;
                                item.nofrig = rowID++;
                                connection.Execute(noteForRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // FORNAMMI
                        if (SupplierData != null)
                        {
                            if (fornammiRepository.Exists(SupplierData.Foraso, SupplierData.Foraco))
                            {

                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.UPDATE_QUERY, SupplierData, transaction);
                            }
                            else
                            {
                                resFORNAMMI = connection.ExecuteScalar(fornammiRepository.INSERT_QUERY, SupplierData, transaction);
                            }
                        }
                        // FORNITORI
                        if (SupplierCommercialData != null)
                        {
                            SupplierCommercialData.FOSOSP = SupplierCommercialData.FOSOSP ?? "N";

                            if (fornitoriRepository.Exists(SupplierCommercialData.FOCLIF))
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.UPDATE_QUERY, SupplierCommercialData, transaction);
                            }
                            else
                            {
                                resFORNITORI = connection.ExecuteScalar(fornitoriRepository.INSERT_QUERY, SupplierCommercialData, transaction);
                            }
                        }
                        // RFFTB00F
                        // clear
                        connection.Execute(@"DELETE FROM RFFTB00F WHERE FOCLIF = @foclif",
                            new { foclif = Model.abecod },
                            transaction);
                        if (SupplierReferences != null && SupplierReferences.Count > 0)
                        {
                            int rowID = 1;
                            foreach (var item in SupplierReferences)
                            {
                                item.FOCLIF = Model.abecod;
                                item.rffrig = rowID++;
                                connection.Execute(irfftb00fRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                        // Countepart SUPPLIER_GROUPS
                        // clear
                        connection.Execute(@"DELETE FROM PNCLIFORLEVEL1 WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode",
                            new { ccfsoc = CompanyID, cfcode = Model.abecod },
                            transaction);
                        if (Counterparts != null && Counterparts.Count > 0)
                        {
                            int rowid = 1;
                            foreach (var item in Counterparts.Where(w => !string.IsNullOrWhiteSpace(w.cfgrup) && !string.IsNullOrWhiteSpace(w.cfcont) && !string.IsNullOrWhiteSpace(w.cfsott)).OrderBy(o => o.cfprog))
                            {
                                item.cfcode = Model.abecod;
                                item.cfprog = rowid++;
                                connection.Execute(supplierGroupRepository.INSERT_QUERY, item, transaction);
                            }
                        }
                    }

                    object? resExpireCustomer = null;
                    if (ExpireCustomer != null)
                    {
                        ExpireCustomer.descli = Model.abers1;
                        if (ExpireCustomer.Scagior == 0)
                        {
                            resExpireCustomer = connection.ExecuteScalar(scadcliRepo.DELETE_QUERY, ExpireCustomer, transaction);
                            ExpireCustomer = null;
                        }
                        else
                        {
                            if (scadcliRepo.Exists(Model.abecod))
                            {
                                resExpireCustomer = connection.ExecuteScalar(scadcliRepo.UPDATE_QUERY, ExpireCustomer, transaction);
                            }
                            else
                            {
                                resExpireCustomer = connection.ExecuteScalar(scadcliRepo.INSERT_QUERY, ExpireCustomer, transaction);
                            }
                        }
                    }

                    if (resUpdateABE != null &&
                        ((resExpireCustomer != null && ExpireCustomer != null) || ExpireCustomer == null) &&
                        ((resCLIAMMI != null && CustomerData != null && CustomerData.Cliacod > 0) || (CustomerData == null || CustomerData.Cliacod == 0) || Model.abecfe == "P") &&
                        ((resFORNAMMI != null && SupplierData != null && SupplierData.Foraco > 0) || (SupplierData == null || SupplierData.Foraco == 0) || Model.abecfe == "P") &&
                        ((resCLIENTI != null && CustomerCommercialData != null && CustomerCommercialData.CLIENT > 0) || (CustomerCommercialData == null || CustomerCommercialData.CLIENT == 0) || Model.abecfe == "P") &&
                        ((resFORNITORI != null && SupplierCommercialData != null && SupplierCommercialData.FOCLIF > 0) || (SupplierCommercialData == null || SupplierCommercialData.FOCLIF == 0) || Model.abecfe == "P") &&
                        ((resCERTIFICATE != null) && Certificates != null && Certificates.Count > 0) || (Certificates == null || Certificates.Count == 0 || Certificate == null) || Model.abecfe == "P")
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
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message.ToString());
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


    public string? CanDelete(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var multiAccount = connection.QueryMultiple(
                                    @"  SELECT COUNT(*) FROM TESTATAOFFERTE WHERE OFTCOCL = @id;
                                            SELECT COUNT(*) FROM ORDI_TESTATA_ORDINI WHERE OTCLIE = @id;
                                            SELECT COUNT(*) FROM FATTT00F WHERE FTCODC = @id;
                                            SELECT COUNT(*) FROM BOLLT00F WHERE BTCODC = @id;
                                            SELECT COUNT(*) FROM PN_TESTATA WHERE N1CLFO = @id;
                                            SELECT COUNT(*) FROM PN_RIGHE WHERE n1clie = @id;
                                            SELECT COUNT(*) FROM PROD_ORDINI WHERE opclie = @id;
                                            SELECT COUNT(*) FROM CLIAMMIP1 WHERE Cliacod = @id;"
            , new { id = ID });
            var offers = multiAccount.Read<int?>().Single() ?? 0;
            var orders = multiAccount.Read<int?>().Single() ?? 0;
            var invoices = multiAccount.Read<int?>().Single() ?? 0;
            var ddt = multiAccount.Read<int?>().Single() ?? 0;
            var pnt = multiAccount.Read<int?>().Single() ?? 0;
            var pnr = multiAccount.Read<int?>().Single() ?? 0;
            var prods = multiAccount.Read<int?>().Single() ?? 0;
            var plaf = multiAccount.Read<int?>().Single() ?? 0;

            if (offers == 0 && orders == 0 && invoices == 0 && ddt == 0 && pnt == 0 && pnr == 0 && prods == 0 && plaf == 0)
            {
                return null;
            }
            else
            {
                return $"Impossibile cancellare il codice anagrafico [{ID.ToString("N0")}] perchè in uso nelle seguenti gestioni:\n\n" +
                            $"{(offers > 0 ? "- Offerte clienti\n" : null)}" +
                            $"{(orders > 0 ? "- Ordini clienti\n" : null)}" +
                            $"{(invoices > 0 ? "- Fatture clienti\n" : null)}" +
                            $"{(ddt > 0 ? "- DDT\n" : null)}" +
                            $"{(pnt > 0 ? "- Prima nota\n" : null)}" +
                            $"{(pnr > 0 ? "- Dettagli di prima nota\n" : null)}" +
                            $"{(prods > 0 ? "- Ordini di produzione\n" : null)}" +
                            $"{(plaf > 0 ? "- Dichiarazioni d'intento\n" : null)}";
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(ABE Model, string? ReasonText, string? UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var check = CanDelete(Model.abecod);
            if (string.IsNullOrWhiteSpace(check))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(ReasonText))
                        {
                            // delete
                            connection.Execute("DELETE FROM DESFOR WHERE fornicod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM NOTEFOR WHERE NOFCOD = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM FORNAMMI WHERE Foraco = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ANAG_FORNITORI WHERE FOCLIF = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM RFFTB00F WHERE FOCLIF = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM PNCLIFORLEVEL1 WHERE cfcode = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM DESTINATARI WHERE cliecod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM NOTECLI1 WHERE NOTCLI = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM CLIAMMI WHERE Cliacod = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ANAG_CLIENTI WHERE CLIENT = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ANDEFRES WHERE CLIENT = @id", new { id = Model.abecod }, transaction);
                            connection.Execute("DELETE FROM ANAG_BASE WHERE abecod = @id", new { id = Model.abecod }, transaction);
                        }
                        else
                        {
                            // cancel
                            Model.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            Model.canceledUserID = UserID;
                            Model.canceledNote = ReasonText;
                            connection.Execute(UPDATE_QUERY, Model, transaction);
                        }

                        transaction.Commit();
                        InfoHandler.Show($"Operazione eseguita con successo, codice {(string.IsNullOrWhiteSpace(ReasonText) ? "eliminato definitivamente" : "annullato")}");
                        return true;
                    }
                    catch (Exception exc)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(exc.Message);
                        return false;
                    }
                }
            }
            else
            {
                ErrorHandler.Show(check);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(ABE? Model, FORNAMMI? SupplierData, CLIAMMI? CustomerData, FORNITORI? SupplierCommercialData, CLIENTI? CustomerCommercialData, int VATLength, bool IsInsert)
    {
        try
        {
            if (Model == null)
                return "Dati anagrafici mancanti";

            // Code validation
            if (IsInsert)
            {
                if (Model.abecod == 0 || Exists(Model.abecod))
                    return "Il codice scelto è già in uso o non valido";
            }

            if (string.IsNullOrWhiteSpace(Model.abecfe))
                return "Il tipo di utilizzo è obbligatorio";

            if (string.IsNullOrWhiteSpace(Model.abers1))
                return "La ragione sociale è obbligatoria";

            // Dispatch based on type
            return Model.abecfe == "P"
                ? ValidateProspect(Model)
                : ValidateCustomerOrSupplier(Model, SupplierData, CustomerData, SupplierCommercialData, CustomerCommercialData, VATLength);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string? ValidateProspect(ABE Model)
    {
        if (string.IsNullOrWhiteSpace(Model.abeind))
            return "L'indirizzo è obbligatorio per registrare un prospect";

        if (string.IsNullOrWhiteSpace(Model.abeloc))
            return "La città del prospect è obbligatoria";

        if (!Model.abecap.HasValue || Model.abecap.Value <= 0)
            return "Il C.A.P. del prospect è obbligatorio";

        if (string.IsNullOrWhiteSpace(Model.abepro))
            return "La provincia del prospect è obbligatoria";

        if (string.IsNullOrWhiteSpace(Model.abepiv) &&
            string.IsNullOrWhiteSpace(Model.abecfi))
            return "La partita IVA o il codice fiscale sono obbligatori per il prospect";

        return null;
    }

    private string? ValidateCustomerOrSupplier(ABE Model, FORNAMMI? SupplierData, CLIAMMI? CustomerData, FORNITORI? SupplierCommercialData, CLIENTI? CustomerCommercialData, int VATLength)
    {
        // ISO + Tipo rules
        bool isoOk =
            (!string.IsNullOrWhiteSpace(Model.isocod) &&
             (Model.abetpo == "1" || Model.abetpo == "2" || Model.abetpo == "4")) ||
            (string.IsNullOrWhiteSpace(Model.isocod) && string.IsNullOrWhiteSpace(Model.abetpo)) ||
            Model.abetpo == "3";

        if (!isoOk)
            return "Il codice ISO ed il Tipo sono obbligatori";

        // VAT required?
        if (Model.abetpo == "2" && string.IsNullOrWhiteSpace(Model.abepiv))
            return "La partita IVA non è necessaria per questo tipo di società";

        if ((Model.abetpo == "1" || Model.abetpo == "4") &&
            string.IsNullOrWhiteSpace(Model.abepiv))
            return "La partita IVA è obbligatoria per questo tipo di società";

        // CF required?
        if ((Model.abetpo == "2" || Model.abetpo == "4") &&
            string.IsNullOrWhiteSpace(Model.abecfi))
            return "Il codice fiscale è obbligatorio per questo tipo di società";

        // VAT length
        if ((Model.abetpo == "1" || Model.abetpo == "4") &&
            (Model.abepiv?.Trim().Length != VATLength))
            return "La lunghezza della partita IVA è discordante con il codice ISO";

        // Mandatory address
        if (string.IsNullOrWhiteSpace(Model.abeind))
            return "L'indirizzo è obbligatorio";

        // Duplicate checks
        var existingVAT = ExistsVAT(Model.abecod, Model.abepiv);
        var existingCF = ExistsCF(Model.abecod, Model.abecfi);

        // CF conflict
        if (existingCF != null &&
            !(Model.abetpo == "2" || Model.abetpo == "4"))
        {
            if (!ConfirmHandler.Confirm(
                    $"Esiste gia' un'anagrafica con lo stesso codice fiscale.\n\n" +
                    $"{existingCF.abecod} - {existingCF.abers1} [{existingCF.abecfi}]\n\n" +
                    "Si desidera continuare comunque ?"))
                return "Esiste gia' un'anagrafica con lo stesso codice fiscale";
        }

        // VAT conflict
        if (existingVAT != null &&
           (Model.abetpo == "1" || Model.abetpo == "3" || Model.abetpo == "4"))
        {
            if (!ConfirmHandler.Confirm(
                    $"Esiste gia' un'anagrafica con la stessa partita IVA.\n\n" +
                    $"{existingVAT.abecod} - {existingVAT.abers1} [{existingVAT.abepiv}]\n\n" +
                    "Si desidera continuare comunque ?"))
                return "Esiste gia' un'anagrafica con la stessa partita IVA";
        }

        // Payment type rules
        if (Model.abecfe == "F" && string.IsNullOrWhiteSpace(SupplierData?.pfocod))
            return "Il tipo pagamento fornitore è obbligatorio";

        if (Model.abecfe == "C" && string.IsNullOrWhiteSpace(CustomerData?.pclcod))
            return "Il tipo pagamento cliente è obbligatorio";

        // Invoice template
        if (Model.abfatfile && Model.abfatfileid == null)
            return "Il tipo di personalizazzione fattura è obbligatorio se flaggato";

        // Accounting constraints
        bool hasCL = !string.IsNullOrWhiteSpace(CustomerData?.clcont);
        bool hasFO = !string.IsNullOrWhiteSpace(SupplierData?.foCONT);

        if (Model.abecfe == "E" && !(hasCL && hasFO))
            return "I conti contabili cliente e/o fornitore sono obbligatori";

        if (Model.abecfe == "C" && !hasCL)
            return "I conti contabili cliente e/o fornitore sono obbligatori";

        if (Model.abecfe == "F" && !hasFO)
            return "I conti contabili cliente e/o fornitore sono obbligatori";

        if (Model.abecfe == "C" && (string.IsNullOrEmpty(CustomerData?.clicidi) || string.IsNullOrEmpty(CustomerData?.cliiva)))
        {
            return "Il codice divisa è obbligaotorio";
        }

        if (Model.abecfe == "F" && (string.IsNullOrEmpty(SupplierData?.focidi) || string.IsNullOrEmpty(SupplierData?.foiva)))
        {
            return "Il codice divisa è obbligaotorio";
        }

        if (Model.abecfe == "C" && string.IsNullOrEmpty(CustomerCommercialData?.smecod))
        {
            return "Il settore merceologico è obbligatorio";
        }

        return null;
    }
    #endregion
}