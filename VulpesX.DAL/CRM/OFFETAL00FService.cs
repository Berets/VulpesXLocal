using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.CRM;

public interface IOFFETAL00FRepository
{
    ObservableCollection<OFFETAL00F>? GetList(string OFTASOCI, int OFTAANNO, int OFTANUOR);

    OFFETAL00F? Get(string oftasoci, int OFTAANNO, int OFTANUOR, Guid OFTAUID);

    bool Exists(string oftasoci, int OFTAANNO, int OFTANUOR, Guid OFTAUID);

    #region CRUD
    string INSERT_QUERY {get;}
    string UPDATE_QUERY {get;}
    string DELETE_QUERY { get; }
    bool Insert(OFFETAL00F Model);

    bool Update(OFFETAL00F Model);

    bool UpdateAll(OFFET00F Model, Guid CompanyUID);

    bool Delete(OFFETAL00F Model);

    string? Validate(OFFETAL00F Model, bool IsInsert);
    #endregion
}

public class OFFETAL00FRepository : RepositoryBase, IOFFETAL00FRepository
{
    public OFFETAL00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<OFFETAL00F>? GetList(string OFTASOCI, int OFTAANNO, int OFTANUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<OFFETAL00F>(
                @"SELECT * FROM OFFETAL00F
                        WHERE oftasoci = @oftasoci AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR
                        ORDER BY OFTANAME",
                new { oftasoci = OFTASOCI, OFTAANNO = OFTAANNO, OFTANUOR = OFTANUOR });

            return new ObservableCollection<OFFETAL00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public OFFETAL00F? Get(string oftasoci, int OFTAANNO, int OFTANUOR, Guid OFTAUID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<OFFETAL00F>(
                "SELECT * FROM OFFETAL00F WHERE oftasoci = @oftasoci AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR AND OFTAUID = @OFTAUID",
                new { oftasoci = oftasoci, OFTAANNO = OFTAANNO, OFTANUOR = OFTANUOR, OFTAUID = OFTAUID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string oftasoci, int OFTAANNO, int OFTANUOR, Guid OFTAUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM OFFETAL00F WHERE oftasoci = @oftasoci AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR AND OFTAUID = @OFTAUID",
                new { oftasoci = oftasoci, OFTAANNO = OFTAANNO, OFTANUOR = OFTANUOR, OFTAUID = OFTAUID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO OFFETAL00F (oftasoci,OFTAANNO,OFTANUOR,OFTAUID,OFTANAME,OFTASIZE,added,add_user) OUTPUT INSERTED.rv VALUES(@oftasoci,@OFTAANNO,@OFTANUOR,@OFTAUID,@OFTANAME,@OFTASIZE,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user)";
    public string UPDATE_QUERY => "UPDATE OFFETAL00F SET oftasoci = @oftasoci,OFTAANNO = @OFTAANNO,OFTANUOR = @OFTANUOR,OFTAUID = @OFTAUID,OFTANAME = @OFTANAME,OFTASIZE = @OFTASIZE,added = @added,add_user = @add_user OUTPUT INSERTED.rv WHERE oftasoci = @oftasoci AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR AND OFTAUID = @OFTAUID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM OFFETAL00F OUTPUT DELETED.rv WHERE oftasoci = @oftasoci AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR AND OFTAUID = @OFTAUID AND rv = @rv";
    public bool Insert(OFFETAL00F Model)
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

    public bool Update(OFFETAL00F Model)
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

    public bool UpdateAll(OFFET00F Model, Guid CompanyUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // check for deleted
                foreach (var att in GetList(Model.oftsoci, Model.OFTANNO, Model.OFTNUOR) ?? new ObservableCollection<OFFETAL00F>())
                {
                    if (Model.Attachments?.Where(w => w.OFTAUID == att.OFTAUID).FirstOrDefault() == null)
                    {
                        // delete old blob
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.OFFERS_ATTACHMENTS_FOLDER}{att.OFTAUID}");
                        // delete data
                        connection.Execute(
                        "DELETE FROM OFFETAL00F OUTPUT DELETED.rv WHERE oftasoci = @oftasoci AND OFTAANNO = @OFTAANNO AND OFTANUOR = @OFTANUOR AND OFTAUID = @OFTAUID AND rv = @rv",
                        att, transaction);
                    }
                }
                // update and insert attachments
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                foreach (var att in Model.Attachments ?? new ObservableCollection<OFFETAL00F>())
                {
                    if (!string.IsNullOrWhiteSpace(att.FullPath))
                    {
                        var existing = Get(att.oftasoci, att.OFTAANNO, att.OFTANUOR, att.OFTAUID);
                        if (existing != null)
                        {
                            // delete old blob
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.OFFERS_ATTACHMENTS_FOLDER}{att.OFTAUID}");
                            // upload new one
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.OFFERS_ATTACHMENTS_FOLDER}{att.OFTAUID}", File.ReadAllBytes(att.FullPath));
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
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.OFFERS_ATTACHMENTS_FOLDER}{att.OFTAUID}", File.ReadAllBytes(att.FullPath));
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

    public bool Delete(OFFETAL00F Model)
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

    public string? Validate(OFFETAL00F Model, bool IsInsert)
    {
        try
        {
            if ((IsInsert && !Exists(Model.oftasoci, Model.OFTAANNO, Model.OFTANUOR, Model.OFTAUID)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.OFTANAME))
                {
                    return null;
                }
                else
                { return "Il nome del file è obbligatorio"; }
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