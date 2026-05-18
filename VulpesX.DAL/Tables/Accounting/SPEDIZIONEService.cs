namespace VulpesX.DAL.Tables.Accounting;

public interface ISPEDIZIONERepository
{
    ObservableCollection<SPEDIZIONE>? GetList();

    SPEDIZIONE? Get(string specod);

    bool Exists(string specod);

    #region CRUD
    bool Insert(SPEDIZIONE Model);

    bool Update(SPEDIZIONE Model);

    bool Delete(SPEDIZIONE Model);

    string? Validate(SPEDIZIONE Model, bool IsInsert);
    #endregion
}

public class SPEDIZIONERepository : RepositoryBase, ISPEDIZIONERepository
{
    public SPEDIZIONERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SPEDIZIONE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SPEDIZIONE>(
                "SELECT * FROM SPEDIZIONE");

            return new ObservableCollection<SPEDIZIONE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SPEDIZIONE? Get(string specod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SPEDIZIONE>(
                "SELECT * FROM SPEDIZIONE WHERE specod = @specod",
                new { specod = specod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string specod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM SPEDIZIONE WHERE specod = @specod",
                new { specod = specod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(SPEDIZIONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO SPEDIZIONE (specod,spedes,spetip) OUTPUT INSERTED.rv VALUES(@specod,@spedes,@spetip)",
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

    public bool Update(SPEDIZIONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE SPEDIZIONE SET specod = @specod,spedes = @spedes,spetip = @spetip OUTPUT INSERTED.rv WHERE specod = @specod AND rv = @rv",
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

    public bool Delete(SPEDIZIONE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM SPEDIZIONE OUTPUT DELETED.rv WHERE specod = @specod AND rv = @rv",
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

    public string? Validate(SPEDIZIONE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.specod) && IsInsert && !Exists(Model.specod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.spedes))
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