namespace VulpesX.DAL.Tables.Accounting;

public interface IMERCEOLOGICORepository
{

    ObservableCollection<MERCEOLOGICO>? GetList();

    MERCEOLOGICO? Get(string smecod);

    bool Exists(string smecod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(MERCEOLOGICO Model);

    bool Update(MERCEOLOGICO Model);

    bool Delete(MERCEOLOGICO Model);

    string? Validate(MERCEOLOGICO Model, bool IsInsert);
    #endregion
}

public class MERCEOLOGICORepository : RepositoryBase, IMERCEOLOGICORepository
{
    public MERCEOLOGICORepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<MERCEOLOGICO>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<MERCEOLOGICO>(
                "SELECT * FROM MERCEOLOGICO");

            return new ObservableCollection<MERCEOLOGICO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public MERCEOLOGICO? Get(string smecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<MERCEOLOGICO>(
                "SELECT * FROM MERCEOLOGICO WHERE smecod = @smecod",
                new { smecod = smecod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string smecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM MERCEOLOGICO WHERE smecod = @smecod",
                     new { smecod = smecod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO MERCEOLOGICO (smecod,smedes) OUTPUT INSERTED.rv VALUES(@smecod,@smedes)";
    public string UPDATE_QUERY => "UPDATE MERCEOLOGICO SET smecod = @smecod,smedes = @smedes OUTPUT INSERTED.rv WHERE smecod = @smecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM MERCEOLOGICO OUTPUT DELETED.rv WHERE smecod = @smecod AND rv = @rv";
    public bool Insert(MERCEOLOGICO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(INSERT_QUERY, Model);
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

    public bool Update(MERCEOLOGICO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
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

    public bool Delete(MERCEOLOGICO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(DELETE_QUERY, Model);
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

    public string? Validate(MERCEOLOGICO Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.smecod) && IsInsert && !Exists(Model.smecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.smedes))
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

public class MERCEOLOGICOUfpRepository : RepositoryBase, IMERCEOLOGICORepository
{
    public MERCEOLOGICOUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<MERCEOLOGICO>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<MERCEOLOGICO>(
                "SELECT * FROM MERCEOLOGICO");

            return new ObservableCollection<MERCEOLOGICO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public MERCEOLOGICO? Get(string smecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<MERCEOLOGICO>(
                "SELECT * FROM MERCEOLOGICO WHERE smecod = @smecod",
                new { smecod = smecod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string smecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM MERCEOLOGICO WHERE smecod = @smecod",
                     new { smecod = smecod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO MERCEOLOGICO (smecod,smedes,smerie) OUTPUT INSERTED.rv VALUES(@smecod,@smedes,@smerie)";
    public string UPDATE_QUERY => "UPDATE MERCEOLOGICO SET smecod = @smecod,smedes = @smedes, smerie = @smerie OUTPUT INSERTED.rv WHERE smecod = @smecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM MERCEOLOGICO OUTPUT DELETED.rv WHERE smecod = @smecod AND rv = @rv";
    public bool Insert(MERCEOLOGICO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(INSERT_QUERY, Model);
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

    public bool Update(MERCEOLOGICO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
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

    public bool Delete(MERCEOLOGICO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(DELETE_QUERY, Model);
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

    public string? Validate(MERCEOLOGICO Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.smecod) && IsInsert && !Exists(Model.smecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.smedes))
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