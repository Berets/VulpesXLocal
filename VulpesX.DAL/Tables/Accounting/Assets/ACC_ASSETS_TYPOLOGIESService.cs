namespace VulpesX.DAL.Tables.Accounting.Assets;

public interface IACC_ASSETS_TYPOLOGIESRepository
{
    ObservableCollection<ACC_ASSETS_TYPOLOGIES>? GetList(string tsoci, string tgrupp, string gconto, string tsotco);

    ObservableCollection<PDCSOTTO>? GetPDCListWithTypology(string CompanyID);

    ACC_ASSETS_TYPOLOGIES? Get(string tsoci, string tgrupp, string gconto, string tsotco);

    ACC_ASSETS_TYPOLOGIES? GetIncremented(string tsoci, string tgrupp, string gconto, string tsotco);

    bool Exists(string tsoci, string tgrupp, string gconto, string tsotco);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_TYPOLOGIES Model);

    bool Update(ACC_ASSETS_TYPOLOGIES Model);

    bool Delete(ACC_ASSETS_TYPOLOGIES Model);

    string? Validate(ACC_ASSETS_TYPOLOGIES Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_TYPOLOGIESRepository : RepositoryBase, IACC_ASSETS_TYPOLOGIESRepository
{
    public ACC_ASSETS_TYPOLOGIESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_TYPOLOGIES>? GetList(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_TYPOLOGIES, ACC_ASSETS_CATEGORIES, ACC_ASSETS_TYPOLOGIES>(
                @"SELECT typ.*, cat.* FROM ACC_ASSETS_TYPOLOGIES AS typ
                        INNER JOIN ACC_ASSETS_CATEGORIES AS cat ON typ.jcateg = cat.jcateg
                        WHERE typ.tsoci = @tsoci AND typ.tgrupp = @tgrupp AND typ.gconto = @gconto AND typ.tsotco = @tsotco",
                (typ, cat) => { typ.SelectedAssetCategory = cat; return typ; },
                new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco },
                splitOn: "jcateg");

            return new ObservableCollection<ACC_ASSETS_TYPOLOGIES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetPDCListWithTypology(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCSOTTO, PDCGRUPPI, PDCCONTI, PDCSOTTO>(
                @"SELECT s.P3SOTC,s.P3DES1,g.P1GRUP,g.P1DES1,a.P2CONT,a.P2DES1 FROM pdcsotto AS s
                        INNER JOIN PDCGRUPPI AS g ON s.P1GRUP = g.P1GRUP
                        INNER JOIN PDCCONTI AS a ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT
                        WHERE EXISTS (SELECT *
                        FROM ACC_ASSETS_TYPOLOGIES
                            WHERE ACC_ASSETS_TYPOLOGIES.tsoci = @cid
                                AND s.P1GRUP = ACC_ASSETS_TYPOLOGIES.tgrupp
                                AND s.P2CONT = ACC_ASSETS_TYPOLOGIES.gconto 
                                AND s.P3SOTC = ACC_ASSETS_TYPOLOGIES.tsotco) AND (p3soci IS NULL OR p3soci=@cid)",
                (pdcs, pdcg, pdcc) => { pdcs.Group = pdcg; pdcs.Account = pdcc; return pdcs; },
                new { cid = CompanyID }, splitOn: "P1GRUP,P2CONT");

            return new ObservableCollection<PDCSOTTO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_TYPOLOGIES? Get(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_TYPOLOGIES>(
                "SELECT * FROM ACC_ASSETS_TYPOLOGIES WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco",
                new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_TYPOLOGIES? GetIncremented(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Query<ACC_ASSETS_TYPOLOGIES>(
                    "SELECT * FROM ACC_ASSETS_TYPOLOGIES WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco",
                    new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco }, transaction)
                    .FirstOrDefault();
                    if (result != null)
                    {
                        result.tnupro += 1;
                        connection.Execute("UPDATE ACC_ASSETS_TYPOLOGIES SET tnupro = @nv WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND rv = @rv",
                            new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco, rv = result.rv, nv = result.tnupro }, transaction);
                        transaction.Commit();
                        return result;
                    }
                    else
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                    return null;
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_TYPOLOGIES WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco",
                new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_TYPOLOGIES (tsoci,tgrupp,gconto,tsotco,jcateg,tnupro,grupp2,conto2,sotto2,grupp1,cont1,sotto1,segno1,segno2,grupp3,conto3,sotto3,segno3,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@tsoci,@tgrupp,@gconto,@tsotco,@jcateg,@tnupro,@grupp2,@conto2,@sotto2,@grupp1,@cont1,@sotto1,@segno1,@segno2,@grupp3,@conto3,@sotto3,@segno3,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_TYPOLOGIES SET tsoci = @tsoci,tgrupp = @tgrupp,gconto = @gconto,tsotco = @tsotco,jcateg = @jcateg,tnupro = @tnupro,grupp2 = @grupp2,conto2 = @conto2,sotto2 = @sotto2,grupp1 = @grupp1,cont1 = @cont1,sotto1 = @sotto1,segno1 = @segno1,segno2 = @segno2,grupp3 = @grupp3,conto3 = @conto3,sotto3 = @sotto3,segno3 = @segno3,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_TYPOLOGIES OUTPUT DELETED.rv WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND rv = @rv";
    public bool Insert(ACC_ASSETS_TYPOLOGIES Model)
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

    public bool Update(ACC_ASSETS_TYPOLOGIES Model)
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

    public bool Delete(ACC_ASSETS_TYPOLOGIES Model)
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

    public string? Validate(ACC_ASSETS_TYPOLOGIES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.tsoci) && IsInsert && !Exists(Model.tsoci, Model.tgrupp, Model.gconto, Model.tsotco)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.tgrupp) && !string.IsNullOrWhiteSpace(Model.gconto) && !string.IsNullOrWhiteSpace(Model.tsotco))
                {
                    if (!string.IsNullOrWhiteSpace(Model.jcateg))
                    {
                        return null;
                    }
                    else
                    { return "La categoria del cespite è obbligatoria"; }
                }
                else
                { return "Il gruppo, il conto e il sottoconto sono obbligatori"; }
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

public class ACC_ASSETS_TYPOLOGIESUfpRepository : RepositoryBase, IACC_ASSETS_TYPOLOGIESRepository
{
    public ACC_ASSETS_TYPOLOGIESUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_TYPOLOGIES>? GetList(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_TYPOLOGIES, ACC_ASSETS_CATEGORIES, ACC_ASSETS_TYPOLOGIES>(
                @"SELECT typ.*, cat.* FROM ACC_ASSETS_TYPOLOGIES AS typ
                        INNER JOIN ACC_ASSETS_CATEGORIES AS cat ON typ.jcateg = cat.jcateg
                        WHERE typ.tsoci = @tsoci AND typ.tgrupp = @tgrupp AND typ.gconto = @gconto AND typ.tsotco = @tsotco",
                (typ, cat) => { typ.SelectedAssetCategory = cat; return typ; },
                new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco },
                splitOn: "jcateg");

            return new ObservableCollection<ACC_ASSETS_TYPOLOGIES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetPDCListWithTypology(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCSOTTO, PDCGRUPPI, PDCCONTI, PDCSOTTO>(
                @"SELECT s.P3SOTC,s.P3DES1,g.P1GRUP,g.P1DES1,a.P2CONT,a.P2DES1 FROM pdc_sottoconti AS s
                        INNER JOIN PDC_GRUPPI AS g ON s.P1GRUP = g.P1GRUP
                        INNER JOIN PDC_CONTI AS a ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT
                        WHERE EXISTS (SELECT *
                        FROM ACC_ASSETS_TYPOLOGIES
                            WHERE ACC_ASSETS_TYPOLOGIES.tsoci = @cid
                                AND s.P1GRUP = ACC_ASSETS_TYPOLOGIES.tgrupp
                                AND s.P2CONT = ACC_ASSETS_TYPOLOGIES.gconto 
                                AND s.P3SOTC = ACC_ASSETS_TYPOLOGIES.tsotco) AND (p3soci IS NULL OR p3soci = '' OR p3soci=@cid)",
                (pdcs, pdcg, pdcc) => { pdcs.Group = pdcg; pdcs.Account = pdcc; return pdcs; },
                new { cid = CompanyID }, splitOn: "P1GRUP,P2CONT");

            return new ObservableCollection<PDCSOTTO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_TYPOLOGIES? Get(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_TYPOLOGIES>(
                "SELECT * FROM ACC_ASSETS_TYPOLOGIES WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco",
                new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_TYPOLOGIES? GetIncremented(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Query<ACC_ASSETS_TYPOLOGIES>(
                    "SELECT * FROM ACC_ASSETS_TYPOLOGIES WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco",
                    new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco }, transaction)
                    .FirstOrDefault();
                    if (result != null)
                    {
                        result.tnupro += 1;
                        connection.Execute("UPDATE ACC_ASSETS_TYPOLOGIES SET tnupro = @nv WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND rv = @rv",
                            new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco, rv = result.rv, nv = result.tnupro }, transaction);
                        transaction.Commit();
                        return result;
                    }
                    else
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                    return null;
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string tsoci, string tgrupp, string gconto, string tsotco)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_TYPOLOGIES WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco",
                new { tsoci = tsoci, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_TYPOLOGIES (tsoci,tgrupp,gconto,tsotco,jcateg,tnupro,grupp2,conto2,sotto2,grupp1,cont1,sotto1,segno1,segno2,grupp3,conto3,sotto3,segno3,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@tsoci,@tgrupp,@gconto,@tsotco,@jcateg,@tnupro,@grupp2,@conto2,@sotto2,@grupp1,@cont1,@sotto1,@segno1,@segno2,@grupp3,@conto3,@sotto3,@segno3,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_TYPOLOGIES SET tsoci = @tsoci,tgrupp = @tgrupp,gconto = @gconto,tsotco = @tsotco,jcateg = @jcateg,tnupro = @tnupro,grupp2 = @grupp2,conto2 = @conto2,sotto2 = @sotto2,grupp1 = @grupp1,cont1 = @cont1,sotto1 = @sotto1,segno1 = @segno1,segno2 = @segno2,grupp3 = @grupp3,conto3 = @conto3,sotto3 = @sotto3,segno3 = @segno3,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_TYPOLOGIES OUTPUT DELETED.rv WHERE tsoci = @tsoci AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND rv = @rv";
    public bool Insert(ACC_ASSETS_TYPOLOGIES Model)
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

    public bool Update(ACC_ASSETS_TYPOLOGIES Model)
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

    public bool Delete(ACC_ASSETS_TYPOLOGIES Model)
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

    public string? Validate(ACC_ASSETS_TYPOLOGIES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.tsoci) && IsInsert && !Exists(Model.tsoci, Model.tgrupp, Model.gconto, Model.tsotco)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.tgrupp) && !string.IsNullOrWhiteSpace(Model.gconto) && !string.IsNullOrWhiteSpace(Model.tsotco))
                {
                    if (!string.IsNullOrWhiteSpace(Model.jcateg))
                    {
                        return null;
                    }
                    else
                    { return "La categoria del cespite è obbligatoria"; }
                }
                else
                { return "Il gruppo, il conto e il sottoconto sono obbligatori"; }
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