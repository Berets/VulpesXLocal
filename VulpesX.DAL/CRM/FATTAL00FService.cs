using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.CRM;

public interface IFATTAL00FRepository
{
    ObservableCollection<FATTAL00F>? GetList(string FTASOCI, int FTAANNO, int FTANUOR);

    FATTAL00F? Get(string FTASOCI, int FTAANNO, int FTANUOR, Guid FTAUID);

    bool Exists(string FTASOCI, int FTAANNO, int FTANUOR, Guid FTAUID);

    bool Insert(FATTAL00F Model);

    bool Update(FATTAL00F Model);

    bool UpdateAll(FATTT00F Model, Guid CompanyUID);

    bool Delete(FATTAL00F Model);

    string? Validate(FATTAL00F Model, bool IsInsert);
}

public class FATTAL00FRepository : RepositoryBase, IFATTAL00FRepository
{
    public FATTAL00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FATTAL00F>? GetList(string FTASOCI, int FTAANNO, int FTANUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FATTAL00F>(
                @"SELECT * FROM FATTAL00F
                        WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR
                        ORDER BY FTANAME",
                new { FTASOCI, FTAANNO, FTANUOR });

            return new ObservableCollection<FATTAL00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTAL00F? Get(string FTASOCI, int FTAANNO, int FTANUOR, Guid FTAUID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FATTAL00F>(
                "SELECT * FROM FATTAL00F WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR AND FTAUID = @FTAUID",
                new { FTASOCI, FTAANNO, FTANUOR, FTAUID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string FTASOCI, int FTAANNO, int FTANUOR, Guid FTAUID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FATTAL00F WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR AND FTAUID = @FTAUID",
                new { FTASOCI, FTAANNO, FTANUOR, FTAUID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(FATTAL00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO FATTAL00F (FTASOCI,FTAANNO,FTANUOR,FTAUID,FTANAME,FTASIZE,added,add_user) OUTPUT INSERTED.rv VALUES(@FTASOCI,@FTAANNO,@FTANUOR,@FTAUID,@FTANAME,@FTASIZE,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user)",
                Model);
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

    public bool Update(FATTAL00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE FATTAL00F SET FTASOCI = @FTASOCI,FTAANNO = @FTAANNO,FTANUOR = @FTANUOR,FTAUID = @FTAUID,FTANAME = @FTANAME,FTASIZE = @FTASIZE,added = @added,add_user = @add_user OUTPUT INSERTED.rv WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR AND FTAUID = @FTAUID AND rv = @rv",
                Model);
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

    public bool UpdateAll(FATTT00F Model, Guid CompanyUID)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                // check for deleted
                foreach (var att in GetList(Model.ftsoci, Model.FTANNO, Model.FTNUOR) ?? new ObservableCollection<FATTAL00F>())
                {
                    if (Model.Attachments?.Where(w => w.FTAUID == att.FTAUID).FirstOrDefault() == null)
                    {
                        // delete old blob
                        StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{att.FTAUID}");
                        // delete data
                        connection.Execute(
                        "DELETE FROM FATTAL00F OUTPUT DELETED.rv WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR AND FTAUID = @FTAUID AND rv = @rv",
                        att, transaction);
                    }
                }
                // update and insert attachments
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                foreach (var att in Model.Attachments ?? new ObservableCollection<FATTAL00F>())
                {
                    if (!string.IsNullOrWhiteSpace(att.FullPath))
                    {
                        var existing = Get(att.FTASOCI, att.FTAANNO, att.FTANUOR, att.FTAUID);
                        if (existing != null)
                        {
                            // delete old blob
                            StorageHelper.Delete(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{att.FTAUID}");
                            // upload new one
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{att.FTAUID}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = now;
                                connection.ExecuteScalar(
                                "UPDATE FATTAL00F SET FTASOCI = @FTASOCI,FTAANNO = @FTAANNO,FTANUOR = @FTANUOR,FTAUID = @FTAUID,FTANAME = @FTANAME,FTASIZE = @FTASIZE,added = @added,add_user = @add_user OUTPUT INSERTED.rv WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR AND FTAUID = @FTAUID AND rv = @rv",
                                att, transaction);
                            }
                        }
                        else
                        {
                            // upload blob
                            var uploadResult = StorageHelper.Upload(StorageHelper.VULPESX_DATA_CONTAINER, $"{CompanyUID}/{StorageHelper.INVOICE_ATTACHMENTS_FOLDER}{att.FTAUID}", File.ReadAllBytes(att.FullPath));
                            // save info
                            if (string.IsNullOrWhiteSpace(uploadResult))
                            {
                                att.added = now;
                                connection.Execute(
                                "INSERT INTO FATTAL00F (FTASOCI,FTAANNO,FTANUOR,FTAUID,FTANAME,FTASIZE,added,add_user) OUTPUT INSERTED.rv VALUES(@FTASOCI,@FTAANNO,@FTANUOR,@FTAUID,@FTANAME,@FTASIZE,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user)",
                                att, transaction);
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

    public bool Delete(FATTAL00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "DELETE FROM FATTAL00F OUTPUT DELETED.rv WHERE FTASOCI = @FTASOCI AND FTAANNO = @FTAANNO AND FTANUOR = @FTANUOR AND FTAUID = @FTAUID AND rv = @rv",
                Model);
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

    public string? Validate(FATTAL00F Model, bool IsInsert)
    {
        try
        {
            if (Model.FTAUID != Guid.Empty && IsInsert && !Exists(Model.FTASOCI, Model.FTAANNO, Model.FTANUOR, Model.FTAUID) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.FTANAME))
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