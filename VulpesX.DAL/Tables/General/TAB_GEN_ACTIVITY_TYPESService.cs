using VulpesX.DAL;

namespace VulpesX.DAL.Tables.General;

public interface ITAB_GEN_ACTIVITY_TYPESRepository
{
    ObservableCollection<TAB_GEN_ACTIVITY_TYPES>? GetList(string CompanyID);

    ObservableCollection<TAB_GEN_ACTIVITY_TYPES>? GetSimpleList(string CompanyID);

    TAB_GEN_ACTIVITY_TYPES? Get(string company_id, string id);

    bool Exists(string company_id, string id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_GEN_ACTIVITY_TYPES Model);

    bool Update(TAB_GEN_ACTIVITY_TYPES Model);

    bool Delete(TAB_GEN_ACTIVITY_TYPES Model);

    string? Validate(TAB_GEN_ACTIVITY_TYPES Model, bool IsInsert);
    #endregion
}

public class TAB_GEN_ACTIVITY_TYPESRepository : RepositoryBase, ITAB_GEN_ACTIVITY_TYPESRepository
{
    public TAB_GEN_ACTIVITY_TYPESRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<TAB_GEN_ACTIVITY_TYPES>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_GEN_ACTIVITY_TYPES>(
                "SELECT * FROM TAB_GEN_ACTIVITY_TYPES WHERE company_id=@cid",
                new { cid = CompanyID });

            return new ObservableCollection<TAB_GEN_ACTIVITY_TYPES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TAB_GEN_ACTIVITY_TYPES>? GetSimpleList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_GEN_ACTIVITY_TYPES>(
                "SELECT company_id, id, description FROM TAB_GEN_ACTIVITY_TYPES WHERE company_id=@cid",
                new { cid = CompanyID });

            return new ObservableCollection<TAB_GEN_ACTIVITY_TYPES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_GEN_ACTIVITY_TYPES? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_GEN_ACTIVITY_TYPES>(
                "SELECT * FROM TAB_GEN_ACTIVITY_TYPES WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_GEN_ACTIVITY_TYPES WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_GEN_ACTIVITY_TYPES (company_id,id,description,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE TAB_GEN_ACTIVITY_TYPES SET company_id = @company_id,id = @id,description = @description,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_GEN_ACTIVITY_TYPES OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(TAB_GEN_ACTIVITY_TYPES Model)
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

    public bool Update(TAB_GEN_ACTIVITY_TYPES Model)
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

    public bool Delete(TAB_GEN_ACTIVITY_TYPES Model)
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

    public string? Validate(TAB_GEN_ACTIVITY_TYPES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.id) && IsInsert && !Exists(Model.company_id, Model.id)) || !IsInsert)
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