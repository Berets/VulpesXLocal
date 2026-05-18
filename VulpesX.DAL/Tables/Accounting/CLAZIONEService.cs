namespace VulpesX.DAL.Tables.Accounting;

public interface ICLAZIONERepository
{

    ObservableCollection<CLAZIONE>? GetList();

    CLAZIONE? Get(string csfcod);

    bool Exists(string csfcod);

    #region CRUD
    bool Insert(CLAZIONE Model);

    bool Update(CLAZIONE Model);

    bool Delete(CLAZIONE Model);

    string? Validate(CLAZIONE Model, bool IsInsert);
    #endregion
}

public class CLAZIONERepository : RepositoryBase, ICLAZIONERepository
{
    public CLAZIONERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CLAZIONE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CLAZIONE>(
                "SELECT * FROM CLAZIONE");

            return new ObservableCollection<CLAZIONE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CLAZIONE? Get(string csfcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CLAZIONE>(
                "SELECT * FROM CLAZIONE WHERE csfcod = @csfcod",
                new { csfcod = csfcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string csfcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CLAZIONE WHERE csfcod = @csfcod",
                new { csfcod = csfcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(CLAZIONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO CLAZIONE (csfcod,csfdes) OUTPUT INSERTED.rv VALUES(@csfcod,@csfdes)",
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

    public bool Update(CLAZIONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE CLAZIONE SET csfcod = @csfcod,csfdes = @csfdes OUTPUT INSERTED.rv WHERE csfcod = @csfcod AND rv = @rv",
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

    public bool Delete(CLAZIONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM CLAZIONE OUTPUT DELETED.rv WHERE csfcod = @csfcod AND rv = @rv",
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

    public string? Validate(CLAZIONE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.csfcod) && IsInsert && !Exists(Model.csfcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.csfdes))
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