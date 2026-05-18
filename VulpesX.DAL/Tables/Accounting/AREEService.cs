using VulpesX.DAL.General;

namespace VulpesX.DAL.Tables.Accounting;

public interface IAREERepository
{

    ObservableCollection<AREE>? GetList();

    AREE? Get(string arecod);

    bool Exists(string arecod);

    #region CRUD
    bool Insert(AREE Model);

    bool Update(AREE Model);

    bool Delete(AREE Model);

    string? Validate(AREE Model, bool IsInsert);
    #endregion
}

public class AREERepository : RepositoryBase, IAREERepository
{
    public AREERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AREE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<AREE>(
                "SELECT * FROM AREE ORDER BY aredes");

            return new ObservableCollection<AREE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AREE? Get(string arecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<AREE>(
                "SELECT * FROM AREE WHERE arecod = @arecod",
                new { arecod = arecod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string arecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AREE WHERE arecod = @arecod",
                new { arecod = arecod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(AREE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO AREE (arecod,aredes,arecap) OUTPUT INSERTED.rv VALUES(@arecod,@aredes,@arecap)",
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

    public bool Update(AREE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE AREE SET arecod = @arecod,aredes = @aredes, arecap = @arecap OUTPUT INSERTED.rv WHERE arecod = @arecod AND rv = @rv",
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

    public bool Delete(AREE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM AREE OUTPUT DELETED.rv WHERE arecod = @arecod AND rv = @rv",
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

    public string? Validate(AREE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.arecod) && IsInsert && !Exists(Model.arecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.aredes))
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

public class AREEUfpRepository : RepositoryBase, IAREERepository
{
    public AREEUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AREE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<AREE>(
                "SELECT * FROM AREE ORDER BY aredes");

            return new ObservableCollection<AREE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AREE? Get(string arecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<AREE>(
                "SELECT * FROM AREE WHERE arecod = @arecod",
                new { arecod = arecod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string arecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AREE WHERE arecod = @arecod",
                new { arecod = arecod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(AREE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO AREE (arecod,aredes) OUTPUT INSERTED.rv VALUES(@arecod,@aredes)",
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

    public bool Update(AREE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE AREE SET arecod = @arecod,aredes = @aredes OUTPUT INSERTED.rv WHERE arecod = @arecod AND rv = @rv",
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

    public bool Delete(AREE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM AREE OUTPUT DELETED.rv WHERE arecod = @arecod AND rv = @rv",
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

    public string? Validate(AREE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.arecod) && IsInsert && !Exists(Model.arecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.aredes))
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