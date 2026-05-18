namespace VulpesX.DAL.Tables.Assets;



public interface ITAB_AST_LOCATIONRepository
{
    ObservableCollection<TAB_AST_LOCATIONS>? GetList(string CompanyID);

    TAB_AST_LOCATIONS? Get(string company_id, string id);

    bool Exists(string company_id, string id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_AST_LOCATIONS Model);

    bool Update(TAB_AST_LOCATIONS Model);

    bool Delete(TAB_AST_LOCATIONS Model);

    string? Validate(TAB_AST_LOCATIONS Model, bool IsInsert);
    #endregion
}

public class TAB_AST_LOCATIONRepository : RepositoryBase, ITAB_AST_LOCATIONRepository
{
    public TAB_AST_LOCATIONRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_AST_LOCATIONS>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_AST_LOCATIONS>(
                @"SELECT * FROM TAB_AST_LOCATIONS
                        WHERE company_id=@cid
                        ORDER BY description", new { cid = CompanyID });

            return new ObservableCollection<TAB_AST_LOCATIONS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_AST_LOCATIONS? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_AST_LOCATIONS>(
                "SELECT * FROM TAB_AST_LOCATIONS WHERE company_id = @company_id AND id = @id",
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
                "SELECT COUNT(*) FROM TAB_AST_LOCATIONS WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_AST_LOCATIONS (company_id,id,description,address,city_id,state_id,zip,note,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description,@address,@city_id,@state_id,@zip,@note,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE TAB_AST_LOCATIONS SET company_id = @company_id,id = @id,description = @description,address = @address,city_id = @city_id,state_id = @state_id,zip = @zip,note = @note,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_AST_LOCATIONS OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(TAB_AST_LOCATIONS Model)
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

    public bool Update(TAB_AST_LOCATIONS Model)
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

    public bool Delete(TAB_AST_LOCATIONS Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var used = (int)connection.Execute(@"SELECT COUNT(*) FROM ASSET00F
                                                WHERE compèany_id=cid AND location_id=@lid",
                                            new { cid = Model.company_id, lid = Model.id });
            if (used > 0)
            {
                ErrorHandler.Show("Impossibile eliminare un'ubicazione utilizzata");
                return false;
            }
            else
            {
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(TAB_AST_LOCATIONS Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.company_id) && IsInsert && !Exists(Model.company_id, Model.id)) || !IsInsert)
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