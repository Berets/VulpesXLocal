using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.SRM;

public interface Iacq_orders_heads_attachmentsRepository
{
    ObservableCollection<acq_orders_heads_attachments>? GetList(string CompanyID, long ID);

    acq_orders_heads_attachments? Get(string company_id, long id, Guid document_id);

    bool Exists(string company_id, long id, Guid document_id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(acq_orders_heads_attachments Model);

    bool Update(acq_orders_heads_attachments Model);

    bool UpdateAll(acq_orders_heads Model, Guid CompanyUID);

    bool Delete(acq_orders_heads_attachments Model);

    string? Validate(acq_orders_heads_attachments Model, bool IsInsert);
    #endregion
}

public class acq_orders_heads_attachmentsRepository : RepositoryBase, Iacq_orders_heads_attachmentsRepository
{
    public acq_orders_heads_attachmentsRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<acq_orders_heads_attachments>? GetList(string CompanyID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<acq_orders_heads_attachments>(
                @"SELECT * FROM acq_orders_heads_attachments
                        WHERE company_id = @id AND id = @id
                        ORDER BY document_name",
                new { cid = CompanyID, id = ID });

            return new ObservableCollection<acq_orders_heads_attachments>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_heads_attachments? Get(string company_id, long id, Guid document_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<acq_orders_heads_attachments>(
                "SELECT * FROM acq_orders_heads_attachments WHERE company_id = @company_id AND id = @id AND document_id = @document_id",
                new { company_id = company_id, id = id, document_id = document_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, long id, Guid document_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM acq_orders_heads_attachments WHERE company_id = @company_id AND id = @id AND document_id = @document_id",
                new { company_id = company_id, id = id, document_id = document_id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO acq_orders_heads_attachments (company_id,id,document_id,document_name,document_size,added,add_user) OUTPUT INSERTED.rv VALUES(@company_id,@id,@document_id,@document_name,@document_size,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user)";
    public string UPDATE_QUERY => "UPDATE acq_orders_heads_attachments SET company_id = @company_id,id = @id,document_id = @document_id,document_name = @document_name,document_size = @document_size,added = @added,add_user = @add_user OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND document_id = @document_id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_orders_heads_attachments OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND document_id = @document_id AND rv = @rv";
    public bool Insert(acq_orders_heads_attachments Model)
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

    public bool Update(acq_orders_heads_attachments Model)
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

    public bool UpdateAll(acq_orders_heads Model, Guid CompanyUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // check for deleted
                foreach (var att in GetList(Model.company_id, Model.id) ?? new ObservableCollection<acq_orders_heads_attachments>())
                {
                    if ((Model.Attachments ?? new ObservableCollection<acq_orders_heads_attachments>()).Where(w => w.document_id == att.document_id).FirstOrDefault() == null)
                    {
                        // delete old blob
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.BUY_ATTACHMENTS_FOLDER}{att.document_id}");
                        // delete data
                        connection.Execute(
                        "DELETE FROM acq_orders_heads_attachments OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND document_id = @document_id AND rv = @rv",
                        att, transaction);
                    }
                }
                // update and insert attachments
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                foreach (var att in Model.Attachments ?? new ObservableCollection<acq_orders_heads_attachments>())
                {
                    if (!string.IsNullOrWhiteSpace(att.FullPath))
                    {
                        var existing = Get(att.company_id, att.id, att.document_id);
                        if (existing != null)
                        {
                            // delete old blob
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.BUY_ATTACHMENTS_FOLDER}{att.document_id}");
                            // upload new one
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.BUY_ATTACHMENTS_FOLDER}{att.document_id}", File.ReadAllBytes(att.FullPath));
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
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.BUY_ATTACHMENTS_FOLDER}{att.document_id}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = now;
                                connection.Execute(INSERT_QUERY, att, transaction);
                            }
                        }
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

    public bool Delete(acq_orders_heads_attachments Model)
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

    public string? Validate(acq_orders_heads_attachments Model, bool IsInsert)
    {
        try
        {
            if ((IsInsert && !Exists(Model.company_id, Model.id, Model.document_id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.document_name))
                {
                    return null;
                }
                else
                { return "Il nome del fiel è obbligatorio"; }
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