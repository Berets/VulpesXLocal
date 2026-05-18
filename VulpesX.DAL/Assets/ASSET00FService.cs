using VulpesX.DAL;

namespace VulpesX.DAL.Assets;

public interface IASSET00FRepository
{
    ObservableCollection<ASSET00F>? GetList(string CompanyID);

    ASSET00F? Get(string company_id, string id);

    bool Exists(string company_id, string id);

    #region CRUD
    public string INSERT_QUERY { get; }
    public string UPDATE_QUERY { get; }
    public string DELETE_QUERY { get; }

    bool Insert(ASSET00F Model);

    bool Update(ASSET00F Model);

    bool Delete(ASSET00F Model);

    string? Validate(ASSET00F Model, bool IsInsert);
    #endregion
}

public class ASSET00FRepository : RepositoryBase, IASSET00FRepository
{
    public ASSET00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ASSET00F>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ASSET00F, TAB_AST_LOCATIONS, ASSET00F>(
                @"SELECT t.*, (SELECT COUNT(*) FROM ASSED00F AS d WHERE d.company_id=t.company_id AND d.id=t.id ) AS DetailsCount, (SELECT COUNT(*) FROM ASSETCO00F AS d WHERE d.company_id=t.company_id AND d.id=t.id ) AS ContactsCount, (SELECT COUNT(*) FROM ASSETAL00F AS d WHERE d.company_id=t.company_id AND d.id=t.id ) AS AttachmentsCount, tab.* FROM ASSET00F AS t
                        LEFT JOIN TAB_AST_LOCATIONS AS tab ON tab.company_id=t.company_id AND tab.id=t.location_id
                        WHERE t.company_id=@cid AND t.canceled IS NULL",
                (det, tab) => { det.Locations = new ObservableCollection<TAB_AST_LOCATIONS>() { tab }; return det; },
                new { cid = CompanyID }, splitOn: "company_id");

            return new ObservableCollection<ASSET00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSET00F? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.QueryFirstOrDefault<ASSET00F>(
                "SELECT * FROM ASSET00F WHERE company_id = @company_id AND id = @id",
                new { company_id, id });
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
                "SELECT COUNT(*) FROM ASSET00F WHERE company_id = @company_id AND id = @id",
                new { company_id, id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ASSET00F (company_id,id,description,product_id,customer_id,resource_id,location_id,purchase_date,installation_date,first_use_date,warranty_expiry_date,note,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description,@product_id,@customer_id,@resource_id,@location_id,@purchase_date,@installation_date,@first_use_date,@warranty_expiry_date,@note,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ASSET00F SET company_id = @company_id,id = @id,description = @description,product_id = @product_id,customer_id = @customer_id,resource_id = @resource_id,location_id = @location_id,purchase_date = @purchase_date,installation_date = @installation_date,first_use_date = @first_use_date,warranty_expiry_date = @warranty_expiry_date,note = @note,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ASSET00F OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";

    public bool Insert(ASSET00F Model)
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

    public bool Update(ASSET00F Model)
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

    public bool Delete(ASSET00F Model)
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

    public string? Validate(ASSET00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.company_id) && IsInsert && !Exists(Model.company_id, Model.id) || !IsInsert)
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
            ErrorHandler.Show(ex.Message);
            return ex.Message;
        }
    }
    #endregion
}