namespace VulpesX.DAL.Tables.Accounting.Assets;

public interface IACC_ASSETS_RATESRepository
{
    ObservableCollection<ACC_ASSETS_RATES>? GetList(string CompanyID, string GroupID, string AccountID, string Subaccount);

    ObservableCollection<PDCSOTTO>? GetPDCListWithRate(string CompanyID);

    ACC_ASSETS_RATES? Get(string ammsoc, string tgrupp, string gconto, string tsotco, int janno);

    bool DuplicateYear(string ammsoc, string UserID, int SourceYear, int TargetYear);

    bool ExistsYear(string ammsoc, int janno);

    bool Exists(string ammsoc, string tgrupp, string gconto, string tsotco, int janno);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_RATES Model);

    bool Update(ACC_ASSETS_RATES Model);

    bool Delete(ACC_ASSETS_RATES Model);

    string? Validate(ACC_ASSETS_RATES Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_RATESRepository : RepositoryBase, IACC_ASSETS_RATESRepository
{
    public ACC_ASSETS_RATESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_RATES>? GetList(string CompanyID, string GroupID, string AccountID, string Subaccount)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_RATES>(
                @"SELECT * FROM ACC_ASSETS_RATES
                        WHERE ammsoc = @cid AND tgrupp = @grp AND gconto = @acc AND tsotco = @sub
                        ORDER BY janno DESC",
                new { cid = CompanyID, grp = GroupID, acc = AccountID, sub = Subaccount });

            return new ObservableCollection<ACC_ASSETS_RATES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetPDCListWithRate(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCSOTTO, PDCGRUPPI, PDCCONTI, PDCSOTTO>(
                @"SELECT s.P3SOTC,s.P3DES1,g.P1GRUP,g.P1DES1,a.P2CONT,a.P2DES1 FROM pdcsotto AS s
                        INNER JOIN PDCGRUPPI AS g ON s.P1GRUP = g.P1GRUP
                        INNER JOIN PDCCONTI AS a ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT
                        WHERE EXISTS (SELECT *
                        FROM ACC_ASSETS_RATES
                            WHERE ACC_ASSETS_RATES.ammsoc = @cid
                                AND s.P1GRUP = ACC_ASSETS_RATES.tgrupp
                                AND s.P2CONT = ACC_ASSETS_RATES.gconto 
                                AND s.P3SOTC = ACC_ASSETS_RATES.tsotco) AND (p3soci IS NULL OR p3soci=@cid)",
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

    public ACC_ASSETS_RATES? Get(string ammsoc, string tgrupp, string gconto, string tsotco, int janno)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_RATES>(
                "SELECT * FROM ACC_ASSETS_RATES WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno",
                new { ammsoc = ammsoc, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco, janno = janno })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool DuplicateYear(string ammsoc, string UserID, int SourceYear, int TargetYear)
    {
        try
        {
            using var connection = GetOpenConnection();


            foreach (var item in connection.Query<ACC_ASSETS_RATES>(
                "SELECT * FROM ACC_ASSETS_RATES WHERE ammsoc = @ammsoc AND janno = @source",
                new { ammsoc = ammsoc, source = SourceYear }))
            {
                item.janno = TargetYear;
                item.addedUserID = UserID;
                Insert(item);
            }
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool ExistsYear(string ammsoc, int janno)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM ACC_ASSETS_RATES 
                        WHERE ammsoc = @ammsoc AND janno = @janno",
                new { ammsoc = ammsoc, janno = janno }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool Exists(string ammsoc, string tgrupp, string gconto, string tsotco, int janno)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_RATES WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno",
                new { ammsoc = ammsoc, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco, janno = janno }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_RATES (ammsoc,tgrupp,gconto,tsotco,janno,tpep1,tpep2,tmaxam,triv,trepai,tancb,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@ammsoc,@tgrupp,@gconto,@tsotco,@janno,@tpep1,@tpep2,@tmaxam,@triv,@trepai,@tancb,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_RATES SET ammsoc = @ammsoc,tgrupp = @tgrupp,gconto = @gconto,tsotco = @tsotco,janno = @janno,tpep1 = @tpep1,tpep2 = @tpep2,tmaxam = @tmaxam,triv = @triv,trepai = @trepai,tancb = @tancb,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_RATES OUTPUT DELETED.rv WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno AND rv = @rv";
    public bool Insert(ACC_ASSETS_RATES Model)
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

    public bool Update(ACC_ASSETS_RATES Model)
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

    public bool Delete(ACC_ASSETS_RATES Model)
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

    public string? Validate(ACC_ASSETS_RATES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ammsoc) && IsInsert && !Exists(Model.ammsoc, Model.tgrupp, Model.gconto, Model.tsotco, Model.janno)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.tgrupp) && !string.IsNullOrWhiteSpace(Model.gconto) && !string.IsNullOrWhiteSpace(Model.tsotco))
                {
                    if (Model.janno > 0)
                    {
                        if (Model.tpep1.HasValue)
                        {
                            return null;
                        }
                        else
                        { return "La percentuale di ammortamento ordinario è obbligatoria"; }
                    }
                    else
                    { return "L'anno è obbligatorio"; }
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

public class ACC_ASSETS_RATESUfpRepository : RepositoryBase, IACC_ASSETS_RATESRepository
{
    public ACC_ASSETS_RATESUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_RATES>? GetList(string CompanyID, string GroupID, string AccountID, string Subaccount)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_RATES>(
                @"SELECT * FROM ACC_ASSETS_RATES
                        WHERE ammsoc = @cid AND tgrupp = @grp AND gconto = @acc AND tsotco = @sub
                        ORDER BY janno DESC",
                new { cid = CompanyID, grp = GroupID, acc = AccountID, sub = Subaccount });

            return new ObservableCollection<ACC_ASSETS_RATES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetPDCListWithRate(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCSOTTO, PDCGRUPPI, PDCCONTI, PDCSOTTO>(
                @"SELECT s.P3SOTC,s.P3DES1,g.P1GRUP,g.P1DES1,a.P2CONT,a.P2DES1 FROM pdc_sottoconti AS s
                        INNER JOIN PDC_GRUPPI AS g ON s.P1GRUP = g.P1GRUP
                        INNER JOIN PDC_CONTI AS a ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT
                        WHERE EXISTS (SELECT *
                        FROM ACC_ASSETS_RATES
                            WHERE ACC_ASSETS_RATES.ammsoc = @cid
                                AND s.P1GRUP = ACC_ASSETS_RATES.tgrupp
                                AND s.P2CONT = ACC_ASSETS_RATES.gconto 
                                AND s.P3SOTC = ACC_ASSETS_RATES.tsotco) AND (p3soci IS NULL OR p3soci=@cid)",
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

    public ACC_ASSETS_RATES? Get(string ammsoc, string tgrupp, string gconto, string tsotco, int janno)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_RATES>(
                "SELECT * FROM ACC_ASSETS_RATES WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno",
                new { ammsoc = ammsoc, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco, janno = janno })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool DuplicateYear(string ammsoc, string UserID, int SourceYear, int TargetYear)
    {
        try
        {
            using var connection = GetOpenConnection();


            foreach (var item in connection.Query<ACC_ASSETS_RATES>(
                "SELECT * FROM ACC_ASSETS_RATES WHERE ammsoc = @ammsoc AND janno = @source",
                new { ammsoc = ammsoc, source = SourceYear }))
            {
                item.janno = TargetYear;
                item.addedUserID = UserID;
                Insert(item);
            }
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool ExistsYear(string ammsoc, int janno)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM ACC_ASSETS_RATES 
                        WHERE ammsoc = @ammsoc AND janno = @janno",
                new { ammsoc = ammsoc, janno = janno }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool Exists(string ammsoc, string tgrupp, string gconto, string tsotco, int janno)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_RATES WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno",
                new { ammsoc = ammsoc, tgrupp = tgrupp, gconto = gconto, tsotco = tsotco, janno = janno }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_RATES (ammsoc,tgrupp,gconto,tsotco,janno,tpep1,tpep2,tmaxam,triv,trepai,tancb,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@ammsoc,@tgrupp,@gconto,@tsotco,@janno,@tpep1,@tpep2,@tmaxam,@triv,@trepai,@tancb,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_RATES SET ammsoc = @ammsoc,tgrupp = @tgrupp,gconto = @gconto,tsotco = @tsotco,janno = @janno,tpep1 = @tpep1,tpep2 = @tpep2,tmaxam = @tmaxam,triv = @triv,trepai = @trepai,tancb = @tancb,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_RATES OUTPUT DELETED.rv WHERE ammsoc = @ammsoc AND tgrupp = @tgrupp AND gconto = @gconto AND tsotco = @tsotco AND janno = @janno AND rv = @rv";
    public bool Insert(ACC_ASSETS_RATES Model)
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

    public bool Update(ACC_ASSETS_RATES Model)
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

    public bool Delete(ACC_ASSETS_RATES Model)
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

    public string? Validate(ACC_ASSETS_RATES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ammsoc) && IsInsert && !Exists(Model.ammsoc, Model.tgrupp, Model.gconto, Model.tsotco, Model.janno)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.tgrupp) && !string.IsNullOrWhiteSpace(Model.gconto) && !string.IsNullOrWhiteSpace(Model.tsotco))
                {
                    if (Model.janno > 0)
                    {
                        if (Model.tpep1.HasValue)
                        {
                            return null;
                        }
                        else
                        { return "La percentuale di ammortamento ordinario è obbligatoria"; }
                    }
                    else
                    { return "L'anno è obbligatorio"; }
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