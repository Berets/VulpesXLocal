using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL;

namespace VulpesX.DAL.Assets;

public interface IASSETAL00FRepository
{
    ObservableCollection<ASSETAL00F>? GetList(string CompanyID, string ID);

    ASSETAL00F? Get(string company_id, string id, Guid document_id);

    bool Exists(string company_id, string id, Guid document_id);

    bool Insert(ASSETAL00F Model);

    bool Update(ASSETAL00F Model);

    bool UpdateAll(ASSET00F Model, Guid CompanyUID);

    bool Delete(ASSETAL00F Model);

    string? Validate(ASSETAL00F Model, bool IsInsert);
}

public class ASSETAL00FRepository : RepositoryBase, IASSETAL00FRepository
{
    public ASSETAL00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ASSETAL00F>? GetList(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ASSETAL00F>(
                @"SELECT * FROM ASSETAL00F
                        WHERE company_id=@cid AND id=@aid
                        ORDER BY added DESC", new { cid = CompanyID, aid = ID });

            return new ObservableCollection<ASSETAL00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSETAL00F? Get(string company_id, string id, Guid document_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.QueryFirstOrDefault<ASSETAL00F>(
                "SELECT * FROM ASSETAL00F WHERE company_id = @company_id AND id = @id AND document_id = @document_id",
                new { company_id, id, document_id });
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, string id, Guid document_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM ASSETAL00F WHERE company_id = @company_id AND id = @id AND document_id = @document_id",
                    new { company_id, id, document_id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ASSETAL00F (company_id,id,document_id,document_name,document_size,added,add_user,note) OUTPUT INSERTED.rv VALUES(@company_id,@id,@document_id,@document_name,@document_size,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user,@note)";
    public readonly string UPDATE_QUERY = "UPDATE ASSETAL00F SET company_id = @company_id,id = @id,document_id = @document_id,document_name = @document_name,document_size = @document_size,added = @added,add_user = @add_user,note = @note OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND document_id = @document_id AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ASSETAL00F OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND document_id = @document_id AND rv = @rv";
    public bool Insert(ASSETAL00F Model)
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

    public bool Update(ASSETAL00F Model)
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

    public bool UpdateAll(ASSET00F Model, Guid CompanyUID)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();

            try
            {
                var list = GetList(Model.company_id, Model.id);

                // check for deleted
                foreach (var att in list ?? new ObservableCollection<ASSETAL00F>())
                {
                    if (Model.Attachments?.Where(w => w.document_id == att.document_id).FirstOrDefault() == null)
                    {
                        // delete old blob
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ASSETS_ATTACHMENTS_FOLDER}{att.document_id}");
                        // delete data
                        connection.Execute(
                        "DELETE FROM ASSETAL00F OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND document_id = @document_id AND rv = @rv",
                        att, transaction);
                    }
                }

                // update and insert attachments
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                foreach (var att in Model.Attachments ?? new ObservableCollection<ASSETAL00F>())
                {
                    if (!string.IsNullOrWhiteSpace(att.FullPath))
                    {
                        var existing = Get(att.company_id, att.id, att.document_id);
                        if (existing != null)
                        {
                            // delete old blob
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ASSETS_ATTACHMENTS_FOLDER}{att.document_id}");
                            // upload new one
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ASSETS_ATTACHMENTS_FOLDER}{att.document_id}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = now;
                                connection.ExecuteScalar(UPDATE_QUERY, att, transaction);
                            }
                        }
                        else
                        {
                            // upload blob
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ASSETS_ATTACHMENTS_FOLDER}{att.document_id}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = now;
                                connection.Execute(INSERT_QUERY, att, transaction);
                            }
                        }
                    }
                    else
                    {
                        // update notes no new bytes
                        connection.ExecuteScalar(UPDATE_QUERY, att, transaction);
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

    public bool Delete(ASSETAL00F Model)
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

    public string? Validate(ASSETAL00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.company_id) && !string.IsNullOrWhiteSpace(Model.id) && Model.document_id != Guid.Empty)
            {
                if (!string.IsNullOrWhiteSpace(Model.document_name))
                {
                    return null;
                }
                else
                { return "Il nome del documento è obbligatorio"; }
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