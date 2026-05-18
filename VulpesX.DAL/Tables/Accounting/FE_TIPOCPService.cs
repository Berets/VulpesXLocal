namespace VulpesX.DAL.Tables.Accounting;

public interface IFE_TIPOCPRepository
{

    ObservableCollection<FE_TIPOCP>? GetList();

    FE_TIPOCP? Get(string id);

    bool Exists(string id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FE_TIPOCP Model);

    bool Update(FE_TIPOCP Model);

    bool Delete(FE_TIPOCP Model);

    string? Validate(FE_TIPOCP Model, bool IsInsert);
    #endregion
}

public class FE_TIPOCPRepository : RepositoryBase, IFE_TIPOCPRepository
{
    public FE_TIPOCPRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_TIPOCP>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_TIPOCP>(
                "SELECT * FROM FE_TIPOCP");

            return new ObservableCollection<FE_TIPOCP>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_TIPOCP? Get(string id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FE_TIPOCP>(
                "SELECT * FROM FE_TIPOCP WHERE id = @id",
                new { id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_TIPOCP WHERE id = @id",
                new { id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FE_TIPOCP (id,description,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@id,@description,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE FE_TIPOCP SET id = @id,description = @description,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FE_TIPOCP OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public bool Insert(FE_TIPOCP Model)
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

    public bool Update(FE_TIPOCP Model)
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

    public bool Delete(FE_TIPOCP Model)
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

    public string? Validate(FE_TIPOCP Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.id) && IsInsert && !Exists(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
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