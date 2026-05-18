using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL;

namespace VulpesX.DAL.Assets;

public interface IASSETCO00FRepository
{
    ObservableCollection<ASSETCO00F>? GetList(string CompanyID, string AssetID);

    ASSETCO00F? Get(string company_id, string id, int sequence);

    bool Insert(ASSETCO00F Model);

    bool Update(ASSETCO00F Model);

    bool UpdateAll(ASSET00F Model);

    bool Delete(ASSETCO00F Model);

    string? Validate(ASSETCO00F Model, bool IsInsert);
}

public class ASSETCO00FRepository : RepositoryBase, IASSETCO00FRepository
{
    public ASSETCO00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ASSETCO00F>? GetList(string CompanyID, string AssetID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var contacts = connection.Query<ASSETCO00F, TAB_GEN_CONTACTS_ROLES, ASSETCO00F>(
                @"SELECT a.*, r.id, r.description FROM ASSETCO00F AS a
                        INNER JOIN TAB_GEN_CONTACTS_ROLES AS r ON r.id = a.role_id
                        WHERE a.company_id=@cid AND a.id=@aid AND a.canceled IS NULL",
                (asc, rol) => { asc.Role = rol; return asc; },
                new { cid = CompanyID, aid = AssetID }, splitOn: "id");
            var details = connection.Query<ASSETCODET00F, TAB_GEN_CONTACTS_TYPES, ASSETCODET00F>(
                @"SELECT a.*, r.id, r.description FROM ASSETCODET00F AS a
                        INNER JOIN TAB_GEN_CONTACTS_TYPES AS r ON r.id = a.contact_type_id
                        WHERE a.company_id=@cid AND a.id=@aid AND a.canceled IS NULL",
                (det, typ) => { det.ContactType = typ; return det; },
                new { cid = CompanyID, aid = AssetID }, splitOn: "id");
            foreach (var item in contacts)
            {
                item.ContactDetails = new ObservableCollection<ASSETCODET00F>(details.Where(w => w.sequence == item.sequence).ToList());
            }
            return new ObservableCollection<ASSETCO00F>(contacts);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSETCO00F? Get(string company_id, string id, int sequence)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ASSETCO00F>(
                "SELECT * FROM ASSETCO00F WHERE company_id = @company_id AND id = @id AND sequence = @sequence",
                new { company_id, id, sequence })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ASSETCO00F (company_id,id,sequence,role_id,note,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,description) OUTPUT INSERTED.rv VALUES(@company_id,@id,@sequence,@role_id,@note,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@description)";
    public readonly string UPDATE_QUERY = "UPDATE ASSETCO00F SET company_id = @company_id,id = @id,sequence = @sequence,role_id = @role_id,note = @note,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,description = @description OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ASSETCO00F OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND rv = @rv";
    public bool Insert(ASSETCO00F Model)
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

    public bool Update(ASSETCO00F Model)
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

    public bool UpdateAll(ASSET00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                // delete all contacts and contacts details
                connection.Execute(@"DELETE FROM ASSETCODET00F WHERE company_id=@cid AND id=@aid", new { cid = Model.company_id, aid = Model.id }, transaction);
                connection.Execute(@"DELETE FROM ASSETCO00F WHERE company_id=@cid AND id=@aid", new { cid = Model.company_id, aid = Model.id }, transaction);

                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var assetco00fRepository = VulpesServiceProvider.Provider.GetRequiredService<ASSETCO00FRepository>();
                // insert contacts
                foreach (var con in Model.Contacts ?? new ObservableCollection<ASSETCO00F>())
                {
                    connection.Execute(assetco00fRepository.INSERT_QUERY, con, transaction);
                    // insert contacts details
                    foreach (var det in con.ContactDetails ?? new ObservableCollection<ASSETCODET00F>())
                    {
                        connection.Execute(assetco00fRepository.INSERT_QUERY, det, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
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

    public bool Delete(ASSETCO00F Model)
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

    public string? Validate(ASSETCO00F Model, bool IsInsert)
    {
        try
        {
            if (Model.sequence > 0)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
                {
                    if (Model.role_id > 0)
                    {
                        return null;
                    }
                    else
                    { return "Il ruolo è obbligatorio"; }
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