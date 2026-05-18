namespace VulpesX.DAL.Tables.Accounting;

public interface IDEPOSITIRepository
{
    ObservableCollection<DEPOSITI>? GetList();

    DEPOSITI? Get(string depcod);

    bool Exists(string depcod);

    #region CRUD
    bool Insert(DEPOSITI Model);

    bool Update(DEPOSITI Model);

    bool Delete(DEPOSITI Model);

    string? Validate(DEPOSITI Model, bool IsInsert);
    #endregion
}

public class DEPOSITIRepository : RepositoryBase, IDEPOSITIRepository
{
    public DEPOSITIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<DEPOSITI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DEPOSITI>(
                "SELECT * FROM DEPOSITI");

            return new ObservableCollection<DEPOSITI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DEPOSITI? Get(string depcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<DEPOSITI>(
                "SELECT * FROM DEPOSITI WHERE depcod = @depcod",
                new { depcod = depcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string depcod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM DEPOSITI WHERE depcod = @depcod",
                new { depcod = depcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(DEPOSITI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO DEPOSITI (depcod,depdes,deppvg) OUTPUT INSERTED.rv VALUES(@depcod,@depdes,@deppvg)",
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

    public bool Update(DEPOSITI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE DEPOSITI SET depcod = @depcod,depdes = @depdes,deppvg = @deppvg OUTPUT INSERTED.rv WHERE depcod = @depcod AND rv = @rv",
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

    public bool Delete(DEPOSITI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM DEPOSITI OUTPUT DELETED.rv WHERE depcod = @depcod AND rv = @rv",
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

    public string? Validate(DEPOSITI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.depcod) && IsInsert && !Exists(Model.depcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.depdes))
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