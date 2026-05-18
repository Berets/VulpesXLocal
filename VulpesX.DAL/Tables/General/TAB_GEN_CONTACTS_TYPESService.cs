namespace VulpesX.DAL.Tables.General;

public interface ITAB_GEN_CONTACTS_TYPESRepository
{
    ObservableCollection<TAB_GEN_CONTACTS_TYPES>? GetList();

    TAB_GEN_CONTACTS_TYPES? Get(int id);

    bool Exists(int id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_GEN_CONTACTS_TYPES Model);

    bool Update(TAB_GEN_CONTACTS_TYPES Model);

    bool Delete(TAB_GEN_CONTACTS_TYPES Model);

    string? Validate(TAB_GEN_CONTACTS_TYPES Model, bool IsInsert);
    #endregion
}

public class TAB_GEN_CONTACTS_TYPESRepository : RepositoryBase, ITAB_GEN_CONTACTS_TYPESRepository
{
    public TAB_GEN_CONTACTS_TYPESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_GEN_CONTACTS_TYPES>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_GEN_CONTACTS_TYPES>(
                @"SELECT * FROM TAB_GEN_CONTACTS_TYPES
                        ORDER BY description");

            return new ObservableCollection<TAB_GEN_CONTACTS_TYPES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_GEN_CONTACTS_TYPES? Get(int id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_GEN_CONTACTS_TYPES>(
                "SELECT * FROM TAB_GEN_CONTACTS_TYPES WHERE id = @id",
                new { id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_GEN_CONTACTS_TYPES WHERE id = @id",
                new { id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_GEN_CONTACTS_TYPES (description,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@description,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE TAB_GEN_CONTACTS_TYPES SET description = @description,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_GEN_CONTACTS_TYPES OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public bool Insert(TAB_GEN_CONTACTS_TYPES Model)
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

    public bool Update(TAB_GEN_CONTACTS_TYPES Model)
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

    public bool Delete(TAB_GEN_CONTACTS_TYPES Model)
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

    public string? Validate(TAB_GEN_CONTACTS_TYPES Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.description))
            {
                return null;
            }
            else
            { return "La descrizione è obbligatoria"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}