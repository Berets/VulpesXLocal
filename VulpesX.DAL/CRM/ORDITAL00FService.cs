using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.CRM;

public interface IORDITAL00FRepository
{
    ObservableCollection<ORDITAL00F>? GetList(string OTASOCI, int OTAANNO, int OTANUOR);

    ORDITAL00F? Get(string otasoci, int OTAANNO, int OTANUOR, Guid OTAUID);

    bool Exists(string otasoci, int OTAANNO, int OTANUOR, Guid OTAUID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ORDITAL00F Model);

    bool Update(ORDITAL00F Model);

    bool UpdateAll(ORDIT00F Model, Guid CompanyUID);

    bool Delete(ORDITAL00F Model);

    string? Validate(ORDITAL00F Model, bool IsInsert);
    #endregion
}

public class ORDITAL00FRepository : RepositoryBase, IORDITAL00FRepository
{
    public ORDITAL00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ORDITAL00F>? GetList(string OTASOCI, int OTAANNO, int OTANUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ORDITAL00F>(
                @"SELECT * FROM ORDITAL00F
                        WHERE otasoci = @otasoci AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR
                        ORDER BY OTANAME",
                new { otasoci = OTASOCI, OTAANNO = OTAANNO, OTANUOR = OTANUOR });

            return new ObservableCollection<ORDITAL00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ORDITAL00F? Get(string otasoci, int OTAANNO, int OTANUOR, Guid OTAUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ORDITAL00F>(
                "SELECT * FROM ORDITAL00F WHERE otasoci = @otasoci AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR AND OTAUID = @OTAUID",
                new { otasoci = otasoci, OTAANNO = OTAANNO, OTANUOR = OTANUOR, OTAUID = OTAUID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string otasoci, int OTAANNO, int OTANUOR, Guid OTAUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ORDITAL00F WHERE otasoci = @otasoci AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR AND OTAUID = @OTAUID",
                new { otasoci = otasoci, OTAANNO = OTAANNO, OTANUOR = OTANUOR, OTAUID = OTAUID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ORDITAL00F (otasoci,OTAANNO,OTANUOR,OTAUID,OTANAME,OTASIZE,added,add_user) OUTPUT INSERTED.rv VALUES(@otasoci,@OTAANNO,@OTANUOR,@OTAUID,@OTANAME,@OTASIZE,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user)";
    public string UPDATE_QUERY => "UPDATE ORDITAL00F SET otasoci = @otasoci,OTAANNO = @OTAANNO,OTANUOR = @OTANUOR,OTAUID = @OTAUID,OTANAME = @OTANAME,OTASIZE = @OTASIZE,added = @added,add_user = @add_user OUTPUT INSERTED.rv WHERE otasoci = @otasoci AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR AND OTAUID = @OTAUID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ORDITAL00F OUTPUT DELETED.rv WHERE otasoci = @otasoci AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR AND OTAUID = @OTAUID AND rv = @rv";
    public bool Insert(ORDITAL00F Model)
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

    public bool Update(ORDITAL00F Model)
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

    public bool UpdateAll(ORDIT00F Model, Guid CompanyUID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // check for deleted
                foreach (var att in GetList(Model.otsoci, Model.OTANNO, Model.OTNUOR) ?? new ObservableCollection<ORDITAL00F>())
                {
                    if (Model.Attachments?.Where(w => w.OTAUID == att.OTAUID).FirstOrDefault() == null)
                    {
                        // delete old blob
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{att.OTAUID}");
                        // delete data
                        connection.Execute(
                        "DELETE FROM ORDITAL00F OUTPUT DELETED.rv WHERE otasoci = @otasoci AND OTAANNO = @OTAANNO AND OTANUOR = @OTANUOR AND OTAUID = @OTAUID AND rv = @rv",
                        att, transaction);
                    }
                }
                // update and insert attachments
                foreach (var att in Model.Attachments ?? new ObservableCollection<ORDITAL00F>())
                {
                    if (!string.IsNullOrWhiteSpace(att.FullPath))
                    {
                        var existing = Get(att.otasoci, att.OTAANNO, att.OTANUOR, att.OTAUID);
                        if (existing != null)
                        {
                            // delete old blob
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{att.OTAUID}");
                            // upload new one
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{att.OTAUID}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                connection.ExecuteScalar(UPDATE_QUERY, att, transaction);
                            }
                        }
                        else
                        {
                            // upload blob
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.ORDERS_ATTACHMENTS_FOLDER}{att.OTAUID}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
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

    public bool Delete(ORDITAL00F Model)
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

    public string? Validate(ORDITAL00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.otasoci) && IsInsert && !Exists(Model.otasoci, Model.OTAANNO, Model.OTANUOR, Model.OTAUID)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.OTANAME))
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