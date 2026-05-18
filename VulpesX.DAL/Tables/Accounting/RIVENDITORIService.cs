namespace VulpesX.DAL.Tables.Accounting;

public interface IRIVENDITORIRepository
{
    ObservableCollection<RIVENDITORI>? GetList();

    RIVENDITORI? Get(string rivcod);

    bool Exists(string rivcod);

    #region CRUD
    bool Insert(RIVENDITORI Model);

    bool Update(RIVENDITORI Model);

    bool Delete(RIVENDITORI Model);

    string? Validate(RIVENDITORI Model, bool IsInsert);
    #endregion
}

public class RIVENDITORIRepository : RepositoryBase, IRIVENDITORIRepository
{
    public RIVENDITORIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<RIVENDITORI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<RIVENDITORI>(
                "SELECT * FROM RIVENDITORI");

            return new ObservableCollection<RIVENDITORI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public RIVENDITORI? Get(string rivcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<RIVENDITORI>(
                "SELECT * FROM RIVENDITORI WHERE rivcod = @rivcod",
                new { rivcod = rivcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string rivcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM RIVENDITORI WHERE rivcod = @rivcod",
                new { rivcod = rivcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(RIVENDITORI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO RIVENDITORI (rivcod,rivdes) OUTPUT INSERTED.rv VALUES(@rivcod,@rivdes)",
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

    public bool Update(RIVENDITORI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE RIVENDITORI SET rivcod = @rivcod,rivdes = @rivdes OUTPUT INSERTED.rv WHERE rivcod = @rivcod AND rv = @rv",
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

    public bool Delete(RIVENDITORI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM RIVENDITORI OUTPUT DELETED.rv WHERE rivcod = @rivcod AND rv = @rv",
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

    public string? Validate(RIVENDITORI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.rivcod) && IsInsert && !Exists(Model.rivcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.rivdes))
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