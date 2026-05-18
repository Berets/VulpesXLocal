namespace VulpesX.DAL.Tables.Accounting;

public interface ICATEGORIARepository
{
    ObservableCollection<CATEGORIA>? GetList();

    CATEGORIA? Get(string catcod);

    bool Exists(string catcod);

    #region CRUD
    bool Insert(CATEGORIA Model);

    bool Update(CATEGORIA Model);

    bool Delete(CATEGORIA Model);

    string? Validate(CATEGORIA Model, bool IsInsert);
    #endregion
}

public class CATEGORIARepository : RepositoryBase, ICATEGORIARepository
{
    public CATEGORIARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CATEGORIA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<CATEGORIA>(
                "SELECT * FROM CATEGORIA");

            return new ObservableCollection<CATEGORIA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CATEGORIA? Get(string catcod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CATEGORIA>(
                "SELECT * FROM CATEGORIA WHERE catcod = @catcod",
                new { catcod = catcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string catcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CATEGORIA WHERE catcod = @catcod",
                new { catcod = catcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(CATEGORIA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO CATEGORIA (catcod,catdes) OUTPUT INSERTED.rv VALUES(@catcod,@catdes)",
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

    public bool Update(CATEGORIA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE CATEGORIA SET catcod = @catcod,catdes = @catdes OUTPUT INSERTED.rv WHERE catcod = @catcod AND rv = @rv",
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

    public bool Delete(CATEGORIA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM CATEGORIA OUTPUT DELETED.rv WHERE catcod = @catcod AND rv = @rv",
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

    public string? Validate(CATEGORIA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.catcod) && IsInsert && !Exists(Model.catcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.catdes))
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