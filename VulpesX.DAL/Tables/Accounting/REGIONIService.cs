namespace VulpesX.DAL.Tables.Accounting;

public interface IREGIONIRepository
{
    ObservableCollection<REGIONI>? GetList();

    REGIONI? Get(string regcod);

    bool Exists(string regcod);

    #region CRUD
    bool Insert(REGIONI Model);

    bool Update(REGIONI Model);

    bool Delete(REGIONI Model);

    string? Validate(REGIONI Model, bool IsInsert);
    #endregion
}

public class REGIONIRepository : RepositoryBase, IREGIONIRepository
{
    public REGIONIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<REGIONI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<REGIONI>(
                "SELECT * FROM REGIONI");

            return new ObservableCollection<REGIONI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public REGIONI? Get(string regcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<REGIONI>(
                "SELECT * FROM REGIONI WHERE regcod = @regcod",
                new { regcod = regcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string regcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM REGIONI WHERE regcod = @regcod",
                new { regcod = regcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(REGIONI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO REGIONI (regcod,regdes) OUTPUT INSERTED.rv VALUES(@regcod,@regdes)",
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

    public bool Update(REGIONI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE REGIONI SET regcod = @regcod,regdes = @regdes OUTPUT INSERTED.rv WHERE regcod = @regcod AND rv = @rv",
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

    public bool Delete(REGIONI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM REGIONI OUTPUT DELETED.rv WHERE regcod = @regcod AND rv = @rv",
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

    public string? Validate(REGIONI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.regcod) && IsInsert && !Exists(Model.regcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.regdes))
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