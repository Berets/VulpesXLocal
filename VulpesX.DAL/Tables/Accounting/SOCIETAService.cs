using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface ISOCIETARepository
{
    ObservableCollection<SOCIETA>? GetList();

    SOCIETA? Get(string soctip);

    bool Exists(string soctip);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(SOCIETA Model);

    bool Update(SOCIETA Model);

    bool Delete(SOCIETA Model);

    string? Validate(SOCIETA Model, bool IsInsert);
    #endregion
}

public class SOCIETARepository : RepositoryBase, ISOCIETARepository
{
    public SOCIETARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SOCIETA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<SOCIETA>(
                "SELECT * FROM SOCIETA");

            return new ObservableCollection<SOCIETA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SOCIETA? Get(string soctip)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SOCIETA>(
                "SELECT * FROM SOCIETA WHERE soctip = @soctip",
                new { soctip = soctip })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string soctip)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM SOCIETA WHERE soctip = @soctip",
                new { soctip = soctip }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO SOCIETA (soctip,socdes,soctpg) OUTPUT INSERTED.rv VALUES(@soctip,@socdes,@soctpg)";
    public string UPDATE_QUERY => "UPDATE SOCIETA SET soctip = @soctip,socdes = @socdes,soctpg = @soctpg OUTPUT INSERTED.rv WHERE soctip = @soctip AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM SOCIETA OUTPUT DELETED.rv WHERE soctip = @soctip AND rv = @rv";
    public bool Insert(SOCIETA Model)
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

    public bool Update(SOCIETA Model)
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

    public bool Delete(SOCIETA Model)
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

    public string? Validate(SOCIETA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.soctip) && IsInsert && !Exists(Model.soctip)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.socdes))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
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