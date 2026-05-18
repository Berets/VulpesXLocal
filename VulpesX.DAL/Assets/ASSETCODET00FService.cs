using VulpesX.DAL;

namespace VulpesX.DAL.Assets;

public interface IASSETCODET00FRepository
{
    ObservableCollection<ASSETCODET00F>? GetList(string CompanyID, string AssetID, int Sequence);

    ASSETCODET00F? Get(string company_id, string id, int sequence, int progressive);

    bool Insert(ASSETCODET00F Model);

    bool Update(ASSETCODET00F Model);

    bool Delete(ASSETCODET00F Model);

    string? Validate(ASSETCODET00F Model, bool IsInsert);
}

public class ASSETCODET00FRepository : RepositoryBase, IASSETCODET00FRepository
{
    public ASSETCODET00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ASSETCODET00F>? GetList(string CompanyID, string AssetID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ASSETCODET00F, TAB_GEN_CONTACTS_TYPES, ASSETCODET00F>(
                @"SELECT a.*,t.id,t.description FROM ASSETCODET00F AS a
                        INNER JOIN TAB_GEN_CONTACTS_TYPES AS t ON t.id = a.contact_type_id
                        WHERE a.company_id=@cid AND a.id=@aid AND a.sequence=@seq",
                (asc, typ) => { asc.ContactType = typ; return asc; },
                new { cid = CompanyID, aid = AssetID, seq = Sequence }, splitOn: "id");

            return new ObservableCollection<ASSETCODET00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSETCODET00F? Get(string company_id, string id, int sequence, int progressive)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ASSETCODET00F>(
                "SELECT * FROM ASSETCODET00F WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND progressive = @progressive",
                new { company_id, id, sequence, progressive })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ASSETCODET00F (company_id,id,sequence,progressive,contact_type_id,contact,note,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@sequence,@progressive,@contact_type_id,@contact,@note,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public readonly string UPDATE_QUERY = "UPDATE ASSETCODET00F SET company_id = @company_id,id = @id,sequence = @sequence,progressive = @progressive,contact_type_id = @contact_type_id,contact = @contact,note = @note,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND progressive = @progressive AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ASSETCODET00F OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND progressive = @progressive AND rv = @rv";
    public bool Insert(ASSETCODET00F Model)
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

    public bool Update(ASSETCODET00F Model)
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

    public bool Delete(ASSETCODET00F Model)
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

    public string? Validate(ASSETCODET00F Model, bool IsInsert)
    {
        try
        {
            if (Model.progressive > 0)
            {
                if (!string.IsNullOrWhiteSpace(Model.contact))
                {
                    if (Model.contact_type_id > 0)
                    {
                        return null;
                    }
                    else
                    { return "Il tipo contatto è obbligatorio"; }
                }
                else
                { return "Il contatto è obbligatorio"; }
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