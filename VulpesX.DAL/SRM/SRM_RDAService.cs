
using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.CRM;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.SRM;

public interface ISRM_RDARepository
{
    ObservableCollection<SRM_RDA>? GetList(string CompanyID, string Status);

    SRM_RDA? Get(string companyID, long id);

    bool Exists(string companyID, long id);

    #region Production
    bool UnlockCustomerOrder(ORDIT00F Order, ORDID00F OrderRow, ObservableCollection<store_stocks_engage> EngagesList, string UserID);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(SRM_RDA Model);

    bool Update(SRM_RDA Model);

    bool Delete(SRM_RDA Model);

    string? Validate(SRM_RDA Model, bool IsInsert);
    #endregion
}

public class SRM_RDARepository : RepositoryBase, ISRM_RDARepository
{
    public SRM_RDARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SRM_RDA>? GetList(string CompanyID, string Status)
    {
        try
        {
            using var connection = GetOpenConnection();


            string whereQuery = string.Empty;
            switch (Status)
            {
                case "P":
                    whereQuery = " AND r.approval_date IS NULL AND r.canceled IS NULL";
                    break;
                case "A":
                    whereQuery = " AND r.approval_date IS NOT NULL AND r.transformed IS NULL AND r.canceled IS NULL";
                    break;
                case "T":
                    whereQuery = " AND r.transformed IS NOT NULL AND r.canceled IS NULL";
                    break;
                case "X":
                    whereQuery = " AND r.canceled IS NOT NULL";
                    break;
            }

            var list = connection.Query<SRM_RDA, tab_articolo, ABE, SRM_RDA>(
                $@"SELECT r.*, p.ID, p.Descrizione, p.UnitaID, a.abecod, a.abers1, a.abers2 FROM SRM_RDA AS r
                        LEFT JOIN ABE AS a ON a.abecod=r.supplier_id
                        INNER JOIN tab_articolo AS p ON p.SocietaID = r.companyID AND p.ID = r.product_id
                        WHERE r.companyID = @cid {(Status == "*" ? null : whereQuery)}
                        ORDER BY r.id DESC",
                (rda, prd, abe) => { rda.UM = prd.UnitaID; rda.Product = prd; rda.Supplier = abe; return rda; },
                new { cid = CompanyID }, splitOn: "ID,abecod");

            return new ObservableCollection<SRM_RDA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SRM_RDA? Get(string companyID, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SRM_RDA>(
                "SELECT * FROM SRM_RDA WHERE companyID = @companyID AND id = @id",
                new { companyID = companyID, id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string companyID, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM SRM_RDA WHERE companyID = @companyID AND id = @id",
                new { companyID = companyID, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Production
    public bool UnlockCustomerOrder(ORDIT00F Order, ORDID00F OrderRow, ObservableCollection<store_stocks_engage> EngagesList, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                    // cancel angages
                    foreach (var eng in EngagesList)
                    {
                        eng.canceled = now;
                        eng.cancel_user = UserID;
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().UPDATE_QUERY, eng, transaction);
                    }
                    // reset row status
                    OrderRow.ODSTA = null;
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().UPDATE_QUERY, OrderRow, transaction);
                    // check if need to change order status
                    VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>().FlagFulfillment(OrderRow.otsoci, Order.OTANNO, Order.OTNUOR, UserID);
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO SRM_RDA (companyID,id,product_id,quantity,requested_delivery,note,approval_date,approval_user,purchase_order_id,production_order_id,transform_user,transformed,customer_order,original_needed,is_customer_material,customer_received_quantity,added,addedUserID,updated,updatedUserID,canceled,canceledUserID,canceledNote,supplier_id,order_year,order_number,order_row) OUTPUT INSERTED.rv VALUES(@companyID,@id,@product_id,@quantity,@requested_delivery,@note,@approval_date,@approval_user,@purchase_order_id,@production_order_id,@transform_user,@transformed,@customer_order,@original_needed,@is_customer_material,@customer_received_quantity,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID,@canceled,@canceledUserID,@canceledNote,@supplier_id,@order_year,@order_number,@order_row)";
    public string UPDATE_QUERY => "UPDATE SRM_RDA SET companyID = @companyID,id = @id,product_id = @product_id,quantity = @quantity,requested_delivery = @requested_delivery,note = @note,approval_date = @approval_date,approval_user = @approval_user,purchase_order_id = @purchase_order_id,production_order_id = @production_order_id,transform_user = @transform_user,transformed = @transformed,customer_order = @customer_order,original_needed = @original_needed,is_customer_material = @is_customer_material,customer_received_quantity = @customer_received_quantity,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID,canceled = @canceled,canceledUserID = @canceledUserID,canceledNote = @canceledNote,supplier_id = @supplier_id,order_year = @order_year,order_number = @order_number,order_row = @order_row OUTPUT INSERTED.rv WHERE companyID = @companyID AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM SRM_RDA OUTPUT DELETED.rv WHERE companyID = @companyID AND id = @id AND rv = @rv";
    public bool Insert(SRM_RDA Model)
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

    public bool Update(SRM_RDA Model)
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

    public bool Delete(SRM_RDA Model)
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

    public string? Validate(SRM_RDA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.companyID) && IsInsert && !Exists(Model.companyID, Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.product_id))
                {
                    if (Model.quantity > 0)
                    {
                        if (!Model.requested_delivery.HasValue || Model.requested_delivery.Value.Date >= VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime())
                        {
                            return null;
                        }
                        else
                        { return "La data di consegna richiesta deve essere maggiore o uguale ad oggi o vuota"; }
                    }
                    else
                    { return "La quantita' è obbligatoria"; }
                }
                else
                { return "L'articolo è obbligatorio"; }
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