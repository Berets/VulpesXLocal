namespace VulpesX.DAL.Tables.Accounting;

public interface ICONSEGNARepository
{
    ObservableCollection<CONSEGNA>? GetList();

    CONSEGNA? Get(string concod);

    bool Exists(string concod);

    #region CRUD
    bool Insert(CONSEGNA Model);

    bool Update(CONSEGNA Model);

    bool Delete(CONSEGNA Model);

    string? Validate(CONSEGNA Model, bool IsInsert);
    #endregion
}

public class CONSEGNARepository : RepositoryBase, ICONSEGNARepository
{
    public CONSEGNARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CONSEGNA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CONSEGNA>(
                "SELECT * FROM CONSEGNA");

            return new ObservableCollection<CONSEGNA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CONSEGNA? Get(string concod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CONSEGNA>(
                "SELECT * FROM CONSEGNA WHERE concod = @concod",
                new { concod = concod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string concod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CONSEGNA WHERE concod = @concod",
                new { concod = concod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(CONSEGNA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO CONSEGNA (concod,condes,conint) OUTPUT INSERTED.rv VALUES(@concod,@condes,@conint)",
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

    public bool Update(CONSEGNA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE CONSEGNA SET concod = @concod,condes = @condes,conint = @conint OUTPUT INSERTED.rv WHERE concod = @concod AND rv = @rv",
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

    public bool Delete(CONSEGNA Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "DELETE FROM CONSEGNA OUTPUT DELETED.rv WHERE concod = @concod AND rv = @rv",
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

    public string? Validate(CONSEGNA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.concod) && IsInsert && !Exists(Model.concod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.condes))
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