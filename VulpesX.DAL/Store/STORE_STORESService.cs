using VulpesX.DAL;

namespace VulpesX.DAL.Store;
public interface Istore_storesRepository
{
    ObservableCollection<store_stores>? GetList(string CompanyID, bool AddNullValue = false);

    store_stores? Get(string company_id, string id);

    bool Exists(string company_id, string id);

    #region Default gets
    store_stores? GetDefaultFinalLoad(string company_id);
    store_stores? GetDefaultHalfLoad(string company_id);
    store_stores? GetDefaultRawLoad(string company_id);
    store_stores? GetDefaultInfinite(string company_id);
    bool DefaultsCheck(string company_id);
    #endregion

    #region CRUD
    bool Insert(store_stores Model);

    bool Update(store_stores Model);

    bool Delete(store_stores Model);

    string? Validate(store_stores Model, bool IsInsert);
    #endregion
}

public class store_storesRepository : RepositoryBase, Istore_storesRepository
{
    public store_storesRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<store_stores>? GetList(string CompanyID, bool AddNullValue = false)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_stores>(
                $@"SELECT * FROM store_stores 
                        WHERE company_id = @company_id",
                new { company_id = CompanyID }).ToList();

            if (AddNullValue)
                list.Add(new store_stores { company_id = CompanyID, id = string.Empty, description = "Tutti i magazzini" });

            return new ObservableCollection<store_stores>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stores? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND id = @id",
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
                "SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Default gets
    public store_stores? GetDefaultFinalLoad(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_final_load = 1",
                new { company_id = company_id })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public store_stores? GetDefaultHalfLoad(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_half = 1",
                new { company_id = company_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public store_stores? GetDefaultRawLoad(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_raw = 1",
                new { company_id = company_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public store_stores? GetDefaultInfinite(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_infinite = 1",
                new { company_id = company_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public bool DefaultsCheck(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var query = connection.QueryMultiple(
                @"SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_raw = 1 AND canceled IS NULL;
                      SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_half = 1 AND canceled IS NULL;
                      SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_final_load = 1 AND canceled IS NULL;
                      SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_infinite = 1 AND canceled IS NULL;",
                new { company_id = company_id });
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default delle materie prime, verificare i magazzini per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default dei semilavorati, verificare i magazzini per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default del prodotto finito, verificare i magazzini per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default dei sempre disponibili, verificare i magazzini per proseguire");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
    #endregion

    #region CRUD
    public static readonly string INSERT_QUERY = "INSERT INTO store_stores (company_id,id,description,type,is_default_final_load,is_default_half,is_default_raw,is_default_infinite,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description,@type,@is_default_final_load,@is_default_half,@is_default_raw,@is_default_infinite,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public static readonly string UPDATE_QUERY = "UPDATE store_stores SET company_id = @company_id,id = @id,description = @description,type = @type,is_default_final_load = @is_default_final_load,is_default_half = @is_default_half,is_default_raw = @is_default_raw,is_default_infinite = @is_default_infinite,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM store_stores OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(store_stores Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_half = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_raw = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_infinite = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
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

    public bool Update(store_stores Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_half = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_raw = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_infinite = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
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

    public bool Delete(store_stores Model)
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

    public string? Validate(store_stores Model, bool IsInsert)
    {
        try
        {
            if ((!Exists(Model.company_id, Model.id) && IsInsert && !string.IsNullOrWhiteSpace(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
                {
                    if (!string.IsNullOrWhiteSpace(Model.type))
                    {
                        return null;
                    }
                    else
                    { return "Il tipo magazzino e' obbligatorio"; }
                }
                else
                { return "La descrizione e' obbligatoria"; }
            }
            else
            { return "Questo codice e' gia' utilizzato o non e' valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class store_storesUfpRepository : RepositoryBase, Istore_storesRepository
{
    public store_storesUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<store_stores>? GetList(string CompanyID, bool AddNullValue = false)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_stores>(
                $@"SELECT 
@company_id as company_id,
c.codmag as id,
c.coddes as description,
c.codtma as type
FROM CODMAG as c",
                new { company_id = CompanyID }).ToList();

            if (AddNullValue)
                list.Add(new store_stores { company_id = CompanyID, id = string.Empty, description = "Tutti i magazzini" });

            return new ObservableCollection<store_stores>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stores? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND id = @id",
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
                "SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Default gets
    public store_stores? GetDefaultFinalLoad(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_final_load = 1",
                new { company_id = company_id })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public store_stores? GetDefaultHalfLoad(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_half = 1",
                new { company_id = company_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public store_stores? GetDefaultRawLoad(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_raw = 1",
                new { company_id = company_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public store_stores? GetDefaultInfinite(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stores>(
                "SELECT * FROM store_stores WHERE company_id = @company_id AND is_default_infinite = 1",
                new { company_id = company_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public bool DefaultsCheck(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var query = connection.QueryMultiple(
                @"SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_raw = 1 AND canceled IS NULL;
                      SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_half = 1 AND canceled IS NULL;
                      SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_final_load = 1 AND canceled IS NULL;
                      SELECT COUNT(*) FROM store_stores WHERE company_id = @company_id AND is_default_infinite = 1 AND canceled IS NULL;",
                new { company_id = company_id });
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default delle materie prime, verificare i magazzini per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default dei semilavorati, verificare i magazzini per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default del prodotto finito, verificare i magazzini per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca il magazzino di default dei sempre disponibili, verificare i magazzini per proseguire");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
    #endregion

    #region CRUD
    public static readonly string INSERT_QUERY = "INSERT INTO store_stores (company_id,id,description,type,is_default_final_load,is_default_half,is_default_raw,is_default_infinite,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description,@type,@is_default_final_load,@is_default_half,@is_default_raw,@is_default_infinite,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public static readonly string UPDATE_QUERY = "UPDATE store_stores SET company_id = @company_id,id = @id,description = @description,type = @type,is_default_final_load = @is_default_final_load,is_default_half = @is_default_half,is_default_raw = @is_default_raw,is_default_infinite = @is_default_infinite,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM store_stores OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(store_stores Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_half = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_raw = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_infinite = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
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

    public bool Update(store_stores Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_half = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_raw = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_stores 
                                        SET is_default_infinite = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
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

    public bool Delete(store_stores Model)
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

    public string? Validate(store_stores Model, bool IsInsert)
    {
        try
        {
            if ((!Exists(Model.company_id, Model.id) && IsInsert && !string.IsNullOrWhiteSpace(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
                {
                    if (!string.IsNullOrWhiteSpace(Model.type))
                    {
                        return null;
                    }
                    else
                    { return "Il tipo magazzino e' obbligatorio"; }
                }
                else
                { return "La descrizione e' obbligatoria"; }
            }
            else
            { return "Questo codice e' gia' utilizzato o non e' valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}