namespace VulpesX.DAL.Store;

public interface ISTORE_CAUSALSRepository
{
    ObservableCollection<store_causals>? GetList(string CompanyID, string? OnlySign = null);

    store_causals? Get(string company_id, string id);

    bool Exists(string company_id, string id);

    #region Default gets
    store_causals? GetDefaultFinalLoad(string CompanyID);

    store_causals? GetDefaultFinalUnoad(string CompanyID);

    store_causals? GetDefaultRawUnload(string CompanyID);

    store_causals? GetDefaultRawLoad(string CompanyID);

    store_causals? GetDefaultExternalUnload(string CompanyID);

    store_causals? GetDefaultExternalLoad(string CompanyID);

    store_causals? GetDefaultHalfLoad(string CompanyID);

    store_causals? GetDefaultHalfUnload(string CompanyID);

    store_causals? GetDefaultInfiniteUnload(string CompanyID);

    #region Recursive linked causals
    List<Tuple<store_stores, store_causals>>? GetLinkedList(string CompanyID, store_causals MainCausal);
    #endregion

    bool DefaultsCheck(string company_id);
    #endregion

    #region CRUD
    bool Insert(store_causals Model);

    bool Update(store_causals Model);

    bool Delete(store_causals Model);

    string? Validate(store_causals Model, bool IsInsert);
    #endregion
}

public class STORE_CAUSALSRepository : RepositoryBase, ISTORE_CAUSALSRepository
{
    public STORE_CAUSALSRepository(IConnectionFactory factory) : base(factory)
    {
    }
    public ObservableCollection<store_causals>? GetList(string CompanyID, string? OnlySign = null)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_causals>(
                $@"SELECT * FROM store_causals 
                        WHERE company_id = @company_id{(string.IsNullOrWhiteSpace(OnlySign) ? null : " AND sign = @sign")}",
                new { company_id = CompanyID, sign = OnlySign });

            return new ObservableCollection<store_causals>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND id = @id",
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
                "SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Default gets
    public store_causals? GetDefaultFinalLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_final_load = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultFinalUnoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_final_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultRawUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_raw_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultRawLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_raw_load = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultExternalUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_external_discharge = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultExternalLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_external_charge = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultHalfLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_half_load = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultHalfUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_half_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultInfiniteUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_infinite_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Recursive linked causals
    public List<Tuple<store_stores, store_causals>>? GetLinkedList(string CompanyID, store_causals MainCausal)
    {
        try
        {
            var result = new List<Tuple<store_stores, store_causals>>();
            using var connection = GetOpenConnection();

            var multi = connection.QueryMultiple(
                @"SELECT id, sign, link_causal_id, link_store_id FROM store_causals WHERE company_id = @company_id AND id = @cauid;
                      SELECT id FROM store_stores WHERE company_id = @company_id AND id = @stoid;",
                new { company_id = CompanyID, cauid = MainCausal.link_causal_id, stoid = MainCausal.link_store_id });
            var causal = multi.Read<store_causals>().First();
            var store = multi.Read<store_stores>().First();
            result.Add(new Tuple<store_stores, store_causals>(store, causal));

            while (!string.IsNullOrWhiteSpace(causal.link_causal_id))
            {
                multi = connection.QueryMultiple(
                @"SELECT id, sign, link_causal_id, link_store_id FROM store_causals WHERE company_id = @company_id AND id = @cauid;
                      SELECT id FROM store_stores WHERE company_id = @company_id AND id = @stoid;",
                new { company_id = CompanyID, cauid = causal.link_causal_id, stoid = causal.link_store_id });
                causal = multi.Read<store_causals>().First();
                store = multi.Read<store_stores>().First();
                result.Add(new Tuple<store_stores, store_causals>(store, causal));
            }
            return result;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    public bool DefaultsCheck(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var query = connection.QueryMultiple(
                @"SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_raw_load = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_raw_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_final_load = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_final_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_half_load = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_half_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_infinite_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_external_charge = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_external_discharge = 1;",
                new { company_id = company_id });
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico delle materie prime, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico delle materie prime, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico del prodotto finito, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico del prodotto finito, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico del semilavorato, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico del semilavorato, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico dei sempre disponibili, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico del conto lavoro, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico del conto lavoro, verificare le causali di magazzino per proseguire");
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
    public static readonly string INSERT_QUERY = "INSERT INTO store_causals (company_id,id,description,sign,is_default_raw_unload,is_default_final_unload,is_default_final_load,is_default_external_discharge,is_default_external_charge,is_default_raw_load,is_default_half_load,is_default_half_unload,is_default_infinite_unload,cost_center_id,link_causal_id,link_store_id) OUTPUT INSERTED.rv VALUES(@company_id,@id,@description,@sign,@is_default_raw_unload,@is_default_final_unload,@is_default_final_load,@is_default_external_discharge,@is_default_external_charge,@is_default_raw_load,@is_default_half_load,@is_default_half_unload,@is_default_infinite_unload,@cost_center_id,@link_causal_id,@link_store_id)";
    public static readonly string UPDATE_QUERY = "UPDATE store_causals SET company_id = @company_id,id = @id,description = @description,sign = @sign,is_default_raw_unload = @is_default_raw_unload,is_default_final_unload = @is_default_final_unload,is_default_final_load = @is_default_final_load,is_default_external_discharge = @is_default_external_discharge,is_default_external_charge = @is_default_external_charge,is_default_raw_load = @is_default_raw_load,is_default_half_load = @is_default_half_load,is_default_half_unload = @is_default_half_unload,is_default_infinite_unload = @is_default_infinite_unload,cost_center_id = @cost_center_id,link_causal_id = @link_causal_id,link_store_id = @link_store_id OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM store_causals OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";

    public bool Insert(store_causals Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    if (Model.is_default_external_charge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_charge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_external_discharge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_discharge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_infinite_unload = 0
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

    public bool Update(store_causals Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                    if (Model.is_default_external_charge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_charge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_external_discharge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_discharge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_infinite_unload = 0
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

    public bool Delete(store_causals Model)
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

    public string? Validate(store_causals Model, bool IsInsert)
    {
        try
        {
            if ((!Exists(Model.company_id, Model.id) && IsInsert && !string.IsNullOrWhiteSpace(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
                {
                    if (!string.IsNullOrWhiteSpace(Model.sign))
                    {
                        if ((string.IsNullOrWhiteSpace(Model.link_causal_id) && string.IsNullOrWhiteSpace(Model.link_store_id)) ||
                            (!string.IsNullOrWhiteSpace(Model.link_causal_id) && !string.IsNullOrWhiteSpace(Model.link_store_id)))
                        {
                            if (Model.SelectedLinkedCausal == null || (Model.SelectedLinkedCausal != null && Model.sign != Model.SelectedLinkedCausal.sign))
                            {
                                return null;
                            }
                            else
                            { return "Il segno della causale collegata non può essere uguale al segno della causale alla quale la si sta collegando"; }
                        }
                        else
                        { return "I parametri per i magazzini collegati devono essere entrambi valorizzati o entrambi vuoti"; }
                    }
                    else
                    { return "Il tipo causale e' obbligatorio"; }
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

public class STORE_CAUSALSUfpRepository : RepositoryBase, ISTORE_CAUSALSRepository
{
    public STORE_CAUSALSUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<store_causals>? GetList(string CompanyID, string? OnlySign = null)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_causals>(
                $@"SELECT 
codcau as id,
descau as description,
segcau as sign
FROM CAUSMAG");

            if (!string.IsNullOrEmpty(OnlySign))
                return new ObservableCollection<store_causals>(list.Where(o => o.sign == OnlySign));

            return new ObservableCollection<store_causals>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? Get(string company_id, string id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                @$"SELECT 
                codcau as id,
descau as description,
segcau as sign
                FROM CAUSMAG WHERE codcau = @id")
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
                "SELECT COUNT(*) FROM CAUSMAG WHERE AND codcau = @id",
                new { id = id }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Default gets
    public store_causals? GetDefaultFinalLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_final_load = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultFinalUnoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_final_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultRawUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_raw_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultRawLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_raw_load = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultExternalUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_external_discharge = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultExternalLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_external_charge = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultHalfLoad(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_half_load = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultHalfUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_half_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_causals? GetDefaultInfiniteUnload(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_causals>(
                "SELECT * FROM store_causals WHERE company_id = @company_id AND is_default_infinite_unload = 1",
                new { company_id = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Recursive linked causals
    public List<Tuple<store_stores, store_causals>>? GetLinkedList(string CompanyID, store_causals MainCausal)
    {
        try
        {
            var result = new List<Tuple<store_stores, store_causals>>();
            using var connection = GetOpenConnection();

            var multi = connection.QueryMultiple(
                @"SELECT id, sign, link_causal_id, link_store_id FROM store_causals WHERE company_id = @company_id AND id = @cauid;
                      SELECT id FROM store_stores WHERE company_id = @company_id AND id = @stoid;",
                new { company_id = CompanyID, cauid = MainCausal.link_causal_id, stoid = MainCausal.link_store_id });
            var causal = multi.Read<store_causals>().First();
            var store = multi.Read<store_stores>().First();
            result.Add(new Tuple<store_stores, store_causals>(store, causal));

            while (!string.IsNullOrWhiteSpace(causal.link_causal_id))
            {
                multi = connection.QueryMultiple(
                @"SELECT id, sign, link_causal_id, link_store_id FROM store_causals WHERE company_id = @company_id AND id = @cauid;
                      SELECT id FROM store_stores WHERE company_id = @company_id AND id = @stoid;",
                new { company_id = CompanyID, cauid = causal.link_causal_id, stoid = causal.link_store_id });
                causal = multi.Read<store_causals>().First();
                store = multi.Read<store_stores>().First();
                result.Add(new Tuple<store_stores, store_causals>(store, causal));
            }
            return result;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    public bool DefaultsCheck(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var query = connection.QueryMultiple(
                @"SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_raw_load = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_raw_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_final_load = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_final_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_half_load = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_half_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_infinite_unload = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_external_charge = 1;
                      SELECT COUNT(*) FROM store_causals WHERE company_id = @company_id AND is_default_external_discharge = 1;",
                new { company_id = company_id });
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico delle materie prime, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico delle materie prime, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico del prodotto finito, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico del prodotto finito, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico del semilavorato, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico del semilavorato, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico dei sempre disponibili, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di carico del conto lavoro, verificare le causali di magazzino per proseguire");
                return false;
            }
            if (query.Read<int>().Single() <= 0)
            {
                ErrorHandler.Show("Manca la causale di default di scarico del conto lavoro, verificare le causali di magazzino per proseguire");
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
    public static readonly string INSERT_QUERY = "INSERT INTO CAUSMAG (codcau,descau,segcau,flgcau,flgsca,codsca,tipcau,codtip,cageco) OUTPUT INSERTED.rv VALUES(@codcau,@descau,@segcau,@flgcau,@flgsca,@codsca,@tipcau,@codtip,@cageco)";
    public static readonly string UPDATE_QUERY = "UPDATE CAUSMAG SET codcau =@codcau,descau=@descau,segcau=@segcau,flgcau=@flgcau,flgsca=@flgsca,codsca=@codsca,tipcau=@tipcau,codtip=@codtip,cageco=@cageco OUTPUT INSERTED.rv WHERE  caucod = @caucod AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM CAUSMAG OUTPUT DELETED.rv WHERE  caucod = @caucod AND rv = @rv";

    public bool Insert(store_causals Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    if (Model.is_default_external_charge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_charge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_external_discharge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_discharge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_infinite_unload = 0
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

    public bool Update(store_causals Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                    if (Model.is_default_external_charge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_charge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_external_discharge)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_external_discharge = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_final_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_final_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_raw_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_raw_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_load)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_load = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_half_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_half_unload = 0
                                        WHERE company_id = @company_id AND id != @id",
                                Model, transaction);
                    }
                    if (Model.is_default_infinite_unload)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE store_causals 
                                        SET is_default_infinite_unload = 0
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

    public bool Delete(store_causals Model)
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

    public string? Validate(store_causals Model, bool IsInsert)
    {
        try
        {
            if ((!Exists(Model.company_id, Model.id) && IsInsert && !string.IsNullOrWhiteSpace(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
                {
                    if (!string.IsNullOrWhiteSpace(Model.sign))
                    {
                        if ((string.IsNullOrWhiteSpace(Model.link_causal_id) && string.IsNullOrWhiteSpace(Model.link_store_id)) ||
                            (!string.IsNullOrWhiteSpace(Model.link_causal_id) && !string.IsNullOrWhiteSpace(Model.link_store_id)))
                        {
                            if (Model.SelectedLinkedCausal == null || (Model.SelectedLinkedCausal != null && Model.sign != Model.SelectedLinkedCausal.sign))
                            {
                                return null;
                            }
                            else
                            { return "Il segno della causale collegata non può essere uguale al segno della causale alla quale la si sta collegando"; }
                        }
                        else
                        { return "I parametri per i magazzini collegati devono essere entrambi valorizzati o entrambi vuoti"; }
                    }
                    else
                    { return "Il tipo causale e' obbligatorio"; }
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