using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.Tables.Accounting;

public interface ICAUCONTRepository
{
    ObservableCollection<CAUCONT>? GetSimpleList();

    ObservableCollection<CAUCONT>? GetSimpleList(string StateID);

    ObservableCollection<CAUCONT>? GetList();

    CAUCONT? Get(string ID);

    CAUCONT? GetWalletAccountingCausal();

    bool Exists(string ID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(CAUCONT Model, ObservableCollection<CAUCONT_GROUPS>? CounterpartRows, string CompanyID);

    bool Update(CAUCONT Model, ObservableCollection<CAUCONT_GROUPS>? CounterpartRows, string CompanyID);

    bool Delete(CAUCONT Model);

    string? Validate(CAUCONT Model, bool IsInsert);
    #endregion
}

public class CAUCONTRepository : RepositoryBase, ICAUCONTRepository
{
    public CAUCONTRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CAUCONT>? GetSimpleList()
    {
        try
        {
            using var connection = GetOpenConnection();


            return new ObservableCollection<CAUCONT>(connection.Query<CAUCONT>(
                @"SELECT c.* FROM CAUCONT AS c
                        WHERE c.canceled IS NULL
                        ORDER BY c.caucod"));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CAUCONT>? GetSimpleList(string StateID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var sql = @"SELECT c.*
                        FROM CAUCONT AS c
                        WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (StateID != "*")
            {
                if (StateID == "S")
                    sql += " AND c.canceled IS NOT NULL";
                else
                    sql += " AND c.canceled IS NULL";
            }

            sql += " ORDER BY c.caucod";

            return new ObservableCollection<CAUCONT>(
                connection.Query<CAUCONT>(sql, parameters)
            );
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CAUCONT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CAUCONT, LIBRIIVA, CAUCONT>(
                @"SELECT c.*, l.* FROM CAUCONT AS c
                        LEFT JOIN LIBRIIVA AS l ON l.livcod = c.cauliv
                        WHERE c.canceled IS NULL
                        ORDER BY c.caucod",
                (cau, liv) => { cau.IVABook = liv; return cau; }, splitOn: "livcod");

            return new ObservableCollection<CAUCONT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT>(
                @"SELECT * FROM CAUCONT 
                        WHERE caucod = @id",
                new { id = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public CAUCONT? GetWalletAccountingCausal()
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT>(
                @"SELECT TOP(1) * FROM CAUCONT 
                        WHERE CAUCLI='S'")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CAUCONT WHERE caucod = @caucod",
                new { caucod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAUCONT (caucod,caudes,caugen,cauiva,caucli,caufor,cauass,cauali,cauliv,causeg,causol,caucol,cauzer,cauceco,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@caucod,@caudes,@caugen,@cauiva,@caucli,@caufor,@cauass,@cauali,@cauliv,@causeg,@causol,@caucol,@cauzer,@cauceco,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE CAUCONT SET caucod = @caucod,caudes = @caudes,caugen = @caugen,cauiva = @cauiva,caucli = @caucli,caufor = @caufor,cauass = @cauass,cauali = @cauali,cauliv = @cauliv,causeg = @causeg,causol = @causol,caucol = @caucol,cauzer = @cauzer,cauceco = @cauceco,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE caucod = @caucod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAUCONT OUTPUT DELETED.rv WHERE caucod = @caucod AND rv = @rv";
    public bool Insert(CAUCONT Model, ObservableCollection<CAUCONT_GROUPS>? CounterpartRows, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean groups
                    connection.Execute("DELETE FROM CAUCONT_GROUPS WHERE grpsoc=@grpsoc AND caucod=@caucod",
                        new { grpsoc = CompanyID, caucod = Model.caucod }, transaction);
                    // add groups
                    int rowid = 1;
                    foreach (var group in CounterpartRows ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        group.prog = rowid++;
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().INSERT_QUERY, group, transaction);
                    }
                    // add causal
                    connection.Execute(INSERT_QUERY, Model, transaction);
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

    public bool Update(CAUCONT Model, ObservableCollection<CAUCONT_GROUPS>? CounterpartRows, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean groups
                    connection.Execute("DELETE FROM CAUCONT_GROUPS WHERE grpsoc=@grpsoc AND caucod=@caucod",
                        new { grpsoc = CompanyID, caucod = Model.caucod }, transaction);
                    // add groups
                    int rowid = 1;
                    foreach (var group in CounterpartRows ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        group.prog = rowid++;
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().INSERT_QUERY, group, transaction);
                    }
                    // update causal
                    connection.Execute(UPDATE_QUERY, Model, transaction);
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

    public bool Delete(CAUCONT Model)
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

    public string? Validate(CAUCONT Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.caucod) && IsInsert && !Exists(Model.caucod)) || !IsInsert)
            {
                if (Model.caucod.Length > 0 && Model.caucod.Length <= 2)
                {
                    if (!string.IsNullOrWhiteSpace(Model.caudes))
                    {
                        if ((string.IsNullOrWhiteSpace(Model.cauliv) && !Model.cauivaBool) ||
                            (!string.IsNullOrWhiteSpace(Model.cauliv) && Model.cauivaBool))
                        {
                            if ((string.IsNullOrWhiteSpace(Model.cauliv) && string.IsNullOrWhiteSpace(Model.causeg)) ||
                            (!string.IsNullOrWhiteSpace(Model.cauliv) && !string.IsNullOrWhiteSpace(Model.causeg)))
                            {
                                return null;
                            }
                            else
                            { return "Se si seleziona un libro IVA è obbligatorio selezionarne anche il segno"; }
                        }
                        else
                        { return "Se si seleziona un libro IVA è obbligatorio il flag di gestione IVA e viceversa"; }
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice può contenere solo 2 caratteri"; }
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

public class CAUCONTUfpRepository : RepositoryBase, ICAUCONTRepository
{
    public CAUCONTUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CAUCONT>? GetSimpleList()
    {
        try
        {
            using var connection = GetOpenConnection();

            return new ObservableCollection<CAUCONT>(connection.Query<CAUCONT>(
                @"SELECT c.* FROM CAUCONT AS c
                        WHERE c.caucontann = 'N'
                        ORDER BY c.caucod"));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CAUCONT>? GetSimpleList(string StateID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var sql = @"SELECT c.*
                        FROM CAUCONT AS c
                        WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (StateID != "*")
            {
                sql += " AND c.caucontann = @StateID";
                parameters.Add("@StateID", StateID);
            }

            sql += " ORDER BY c.caucod";

            return new ObservableCollection<CAUCONT>(
                connection.Query<CAUCONT>(sql, parameters)
            );
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<CAUCONT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CAUCONT, LIBRIIVA, CAUCONT>(
                @"SELECT c.* , l.* FROM CAUCONT AS c
                        LEFT JOIN LIBRIIVA AS l ON l.livcod = c.cauliv
                        WHERE c.caucontann ='N'
                        ORDER BY c.caucod",
                (cau, liv) => { cau.IVABook = liv; return cau; }, splitOn: "livcod");

            return new ObservableCollection<CAUCONT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT>(
                @"SELECT * FROM CAUCONT 
                        WHERE caucod = @id",
                new { id = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUCONT? GetWalletAccountingCausal()
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUCONT>(
                @"SELECT TOP(1) * FROM CAUCONT 
                        WHERE CAUCLI='S'")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CAUCONT WHERE caucod = @caucod",
                new { caucod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAUCONT (caucod,caudes,caude2,caugen,cauiva,caucli,caufor,cauass,cauali,cauliv,causeg,causol,causos,cauaut) OUTPUT INSERTED.rv VALUES(@caucod,@caudes,@caude2,@caugen,@cauiva,@caucli,@caufor,@cauass,@cauali,@cauliv,@causeg,@causol,@causos,@cauaut)";
    public string UPDATE_QUERY => "UPDATE CAUCONT SET caucod = @caucod,caudes =@caudes,caude2 = @caude2,caugen = @caugen,cauiva = @cauiva,caucli = @caucli,caufor = @caufor,cauass = @cauass,cauali = @cauali,cauliv = @cauliv,causeg = @causeg,causol = @causol, causos = @causos, cauaut = @cauaut OUTPUT INSERTED.rv WHERE caucod = @caucod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAUCONT OUTPUT DELETED.rv WHERE caucod = @caucod AND rv = @rv";
    public bool Insert(CAUCONT Model, ObservableCollection<CAUCONT_GROUPS>? CounterpartRows, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean groups
                    connection.Execute("DELETE FROM GRPCAUCOD WHERE grpsoc=@grpsoc AND caucod=@caucod",
                        new { grpsoc = CompanyID, caucod = Model.caucod }, transaction);
                    // add groups
                    int rowid = 1;
                    foreach (var group in CounterpartRows ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        group.prog = rowid++;
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().INSERT_QUERY, group, transaction);
                    }
                    // add causal
                    connection.Execute(INSERT_QUERY, Model, transaction);
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

    public bool Update(CAUCONT Model, ObservableCollection<CAUCONT_GROUPS>? CounterpartRows, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean groups
                    connection.Execute("DELETE FROM GRPCAUCOD WHERE grpsoc=@grpsoc AND caucod=@caucod",
                        new { grpsoc = CompanyID, caucod = Model.caucod }, transaction);
                    // add groups
                    int rowid = 1;
                    foreach (var group in CounterpartRows ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        group.prog = rowid++;
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().INSERT_QUERY, group, transaction);
                    }
                    // update causal
                    connection.Execute(UPDATE_QUERY, Model, transaction);
                    transaction.Commit();
                    return true;
                }
                catch(Exception ex)
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

    public bool Delete(CAUCONT Model)
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

    public string? Validate(CAUCONT Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.caucod) && IsInsert && !Exists(Model.caucod)) || !IsInsert)
            {
                if (Model.caucod.Length > 0 && Model.caucod.Length <= 3)
                {
                    if (!string.IsNullOrWhiteSpace(Model.caudes))
                    {
                        if ((string.IsNullOrWhiteSpace(Model.cauliv) && !Model.cauivaBool) ||
                            (!string.IsNullOrWhiteSpace(Model.cauliv) && Model.cauivaBool))
                        {
                            if ((string.IsNullOrWhiteSpace(Model.cauliv) && string.IsNullOrWhiteSpace(Model.causeg)) ||
                            (!string.IsNullOrWhiteSpace(Model.cauliv) && !string.IsNullOrWhiteSpace(Model.causeg)))
                            {
                                return null;
                            }
                            else
                            { return "Se si seleziona un libro IVA è obbligatorio selezionarne anche il segno"; }
                        }
                        else
                        { return "Se si seleziona un libro IVA è obbligatorio il flag di gestione IVA e viceversa"; }
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice può contenere solo 2 caratteri"; }
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
