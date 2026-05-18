namespace VulpesX.DAL.Tables.Accounting;

public interface IIMBALLIRepository
{
    ObservableCollection<IMBALLI>? GetList();

    IMBALLI? Get(string imbcod);

    bool Exists(string imbcod);

    #region CRUD
    bool Insert(IMBALLI Model);

    bool Update(IMBALLI Model);

    bool Delete(IMBALLI Model);

    string? Validate(IMBALLI Model, bool IsInsert);
    #endregion
}

public class IMBALLIRepository : RepositoryBase, IIMBALLIRepository
{
    public IMBALLIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<IMBALLI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<IMBALLI>(
                "SELECT * FROM IMBALLI");

            return new ObservableCollection<IMBALLI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public IMBALLI? Get(string imbcod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<IMBALLI>(
                "SELECT * FROM IMBALLI WHERE imbcod = @imbcod",
                new { imbcod = imbcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string imbcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM IMBALLI WHERE imbcod = @imbcod",
                new { imbcod = imbcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(IMBALLI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO IMBALLI (imbcod,imbdes) OUTPUT INSERTED.rv VALUES(@imbcod,@imbdes)",
                Model);
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

    public bool Update(IMBALLI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE IMBALLI SET imbcod = @imbcod,imbdes = @imbdes OUTPUT INSERTED.rv WHERE imbcod = @imbcod AND rv = @rv",
                Model);
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

    public bool Delete(IMBALLI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM IMBALLI OUTPUT DELETED.rv WHERE imbcod = @imbcod AND rv = @rv",
                Model);
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

    public string? Validate(IMBALLI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.imbcod) && IsInsert && !Exists(Model.imbcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.imbdes))
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