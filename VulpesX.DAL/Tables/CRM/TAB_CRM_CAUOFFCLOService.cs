using VulpesX.DAL;

namespace VulpesX.Services.Tables.CRM;

public interface ITAB_CRM_CAUOFFCLORepository
{
    ObservableCollection<TAB_CRM_CAUOFFCLO>? GetList(string CompanyID);

    TAB_CRM_CAUOFFCLO? Get(string company_id, string id);

    bool Exists(string company_id, string id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_CRM_CAUOFFCLO Model);

    bool Update(TAB_CRM_CAUOFFCLO Model);

    bool CanDelete(TAB_CRM_CAUOFFCLO Model);

    bool Delete(TAB_CRM_CAUOFFCLO Model);

    string? Validate(TAB_CRM_CAUOFFCLO Model, bool IsInsert);
    #endregion
}

public class TAB_CRM_CAUOFFCLORepository : RepositoryBase, ITAB_CRM_CAUOFFCLORepository
{
    public TAB_CRM_CAUOFFCLORepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<TAB_CRM_CAUOFFCLO>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_CRM_CAUOFFCLO>(
                @"SELECT * FROM TAB_CRM_CAUOFFCLO
                        WHERE company_id=@cid
                        ORDER BY description", new { cid = CompanyID });

            return new ObservableCollection<TAB_CRM_CAUOFFCLO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUOFFCLO? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<TAB_CRM_CAUOFFCLO>(
                "SELECT * FROM TAB_CRM_CAUOFFCLO WHERE company_id = @company_id AND id = @id",
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
                "SELECT COUNT(*) FROM TAB_CRM_CAUOFFCLO WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_CRM_CAUOFFCLO (company_id,id,description) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description)";
    public string UPDATE_QUERY => "UPDATE TAB_CRM_CAUOFFCLO SET company_id = @company_id,id = @id,description = @description OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_CRM_CAUOFFCLO OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(TAB_CRM_CAUOFFCLO Model)
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

    public bool Update(TAB_CRM_CAUOFFCLO Model)
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

    public bool CanDelete(TAB_CRM_CAUOFFCLO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM OFFET00F
                                                    WHERE company_id=@cid AND close_causal_id=@id",
                new { cid = Model.company_id, id = Model.id });
            return result == 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
    public bool Delete(TAB_CRM_CAUOFFCLO Model)
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

    public string? Validate(TAB_CRM_CAUOFFCLO Model, bool IsInsert)
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