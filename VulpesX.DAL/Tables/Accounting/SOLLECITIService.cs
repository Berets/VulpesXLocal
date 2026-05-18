namespace VulpesX.DAL.Tables.Accounting;

public interface ISOLLECITIRepository
{
    ObservableCollection<SOLLECITI>? GetList();

    SOLLECITI? Get(int solcod);

    bool Exists(int solcod);

    #region CRUD
    bool Insert(SOLLECITI Model);

    bool Update(SOLLECITI Model);

    bool Delete(SOLLECITI Model);

    string? Validate(SOLLECITI Model, bool IsInsert);
    #endregion
}

public class SOLLECITIRepository : RepositoryBase, ISOLLECITIRepository
{
    public SOLLECITIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SOLLECITI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SOLLECITI>(
                "SELECT * FROM SOLLECITI");

            return new ObservableCollection<SOLLECITI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SOLLECITI? Get(int solcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SOLLECITI>(
                "SELECT * FROM SOLLECITI WHERE solcod = @solcod",
                new { solcod = solcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int solcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM SOLLECITI WHERE solcod = @solcod",
                new { solcod = solcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(SOLLECITI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO SOLLECITI (solcod,soldes,tpflg,tptit,tptest,tpsoc,tpgra,tpleg) OUTPUT INSERTED.rv VALUES(@solcod,@soldes,@tpflg,@tptit,@tptest,@tpsoc,@tpgra,@tpleg)",
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

    public bool Update(SOLLECITI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE SOLLECITI SET solcod = @solcod,soldes = @soldes,tpflg = @tpflg,tptit = @tptit,tptest = @tptest,tpsoc = @tpsoc,tpgra = @tpgra,tpleg = @tpleg OUTPUT INSERTED.rv WHERE solcod = @solcod AND rv = @rv",
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

    public bool Delete(SOLLECITI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM SOLLECITI OUTPUT DELETED.rv WHERE solcod = @solcod AND rv = @rv",
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

    public string? Validate(SOLLECITI Model, bool IsInsert)
    {
        try
        {
            if ((Model.solcod > 0 && IsInsert && !Exists(Model.solcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.soldes))
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