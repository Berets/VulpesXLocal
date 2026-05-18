namespace VulpesX.DAL.Tables.Accounting;

public interface IRITENUTERepository
{

    ObservableCollection<RITENUTE>? GetList();

    RITENUTE? Get(string ritcod);

    bool Exists(string ritcod);

    #region CRUD
    bool Insert(RITENUTE Model);

    bool Update(RITENUTE Model);

    bool Delete(RITENUTE Model);

    string? Validate(RITENUTE Model, bool IsInsert);
    #endregion
}

public class RITENUTERepository : RepositoryBase, IRITENUTERepository
{
    public RITENUTERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<RITENUTE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<RITENUTE>(
                "SELECT * FROM RITENUTE");

            return new ObservableCollection<RITENUTE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public RITENUTE? Get(string ritcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<RITENUTE>(
                "SELECT * FROM RITENUTE WHERE ritcod = @ritcod",
                new { ritcod = ritcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ritcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM RITENUTE WHERE ritcod = @ritcod",
                new { ritcod = ritcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(RITENUTE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO RITENUTE (ritcod,ritdes,ritgr1,ritco1,ritso1,ritca1,ritfl1,ritgr2,ritco2,ritso2,ritca2,ritfl2,ritpag,rtsez,rttipo,rtmese,rtTipRed) OUTPUT INSERTED.rv VALUES(@ritcod,@ritdes,@ritgr1,@ritco1,@ritso1,@ritca1,@ritfl1,@ritgr2,@ritco2,@ritso2,@ritca2,@ritfl2,@ritpag,@rtsez,@rttipo,@rtmese,@rtTipRed)",
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

    public bool Update(RITENUTE Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE RITENUTE SET ritcod = @ritcod,ritdes = @ritdes,ritgr1 = @ritgr1,ritco1 = @ritco1,ritso1 = @ritso1,ritca1 = @ritca1,ritfl1 = @ritfl1,ritgr2 = @ritgr2,ritco2 = @ritco2,ritso2 = @ritso2,ritca2 = @ritca2,ritfl2 = @ritfl2,ritpag = @ritpag,rtsez = @rtsez,rttipo = @rttipo,rtmese = @rtmese,rtTipRed = @rtTipRed OUTPUT INSERTED.rv WHERE ritcod = @ritcod AND rv = @rv",
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

    public bool Delete(RITENUTE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM RITENUTE OUTPUT DELETED.rv WHERE ritcod = @ritcod AND rv = @rv",
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

    public string? Validate(RITENUTE Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ritcod) && IsInsert && !Exists(Model.ritcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.ritdes))
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