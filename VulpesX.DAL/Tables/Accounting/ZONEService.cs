namespace VulpesX.DAL.Tables.Accounting;

public interface IZONERepository
{
    ObservableCollection<ZONE>? GetList();

    ZONE? Get(string zoncod);

    bool Exists(string zoncod);

    #region CRUD
    bool Insert(ZONE Model);

    bool Update(ZONE Model);

    bool Delete(ZONE Model);

    string? Validate(ZONE Model, bool IsInsert);
    #endregion
}

public class ZONERepository : RepositoryBase, IZONERepository
{
    public ZONERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ZONE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ZONE>(
                "SELECT * FROM ZONE");

            return new ObservableCollection<ZONE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ZONE? Get(string zoncod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ZONE>(
                "SELECT * FROM ZONE WHERE zoncod = @zoncod",
                new { zoncod = zoncod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string zoncod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ZONE WHERE zoncod = @zoncod",
                new { zoncod = zoncod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(ZONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO ZONE (zoncod,zondes) OUTPUT INSERTED.rv VALUES(@zoncod,@zondes)",
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

    public bool Update(ZONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE ZONE SET zoncod = @zoncod,zondes = @zondes OUTPUT INSERTED.rv WHERE zoncod = @zoncod AND rv = @rv",
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

    public bool Delete(ZONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "DELETE FROM ZONE OUTPUT DELETED.rv WHERE zoncod = @zoncod AND rv = @rv",
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

    public string? Validate(ZONE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.zoncod) && IsInsert && !Exists(Model.zoncod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.zondes))
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