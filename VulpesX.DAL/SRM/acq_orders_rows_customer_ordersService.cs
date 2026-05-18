namespace VulpesX.DAL.SRM;


public interface Iacq_orders_rows_customer_ordersRepository
{
    ObservableCollection<acq_orders_rows_customer_orders>? GetList(string CompanyID, long PurchaseOrderID);

    acq_orders_rows_customer_orders? Get(string company_id, long order_id, int order_row_id, int customer_order_year, int customer_order_number, int customer_order_row);

    bool Exists(string company_id, long order_id, int order_row_id, int customer_order_year, int customer_order_number, int customer_order_row);

    #region CRUD
    string INSERT_QUERY {get;}
    string UPDATE_QUERY {get;}
    string DELETE_QUERY { get; }
    bool Insert(acq_orders_rows_customer_orders Model);

    bool Update(acq_orders_rows_customer_orders Model);

    bool Delete(acq_orders_rows_customer_orders Model);

    #endregion
}

public class acq_orders_rows_customer_ordersRepository : RepositoryBase, Iacq_orders_rows_customer_ordersRepository
{
    public acq_orders_rows_customer_ordersRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<acq_orders_rows_customer_orders>? GetList(string CompanyID, long PurchaseOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<acq_orders_rows_customer_orders>(
                @"SELECT * FROM acq_orders_rows_customer_orders
                        WHERE company_id=@cid AND order_id=@poid",
                new { cid = CompanyID, poid = PurchaseOrderID });

            return new ObservableCollection<acq_orders_rows_customer_orders>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_rows_customer_orders? Get(string company_id, long order_id, int order_row_id, int customer_order_year, int customer_order_number, int customer_order_row)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<acq_orders_rows_customer_orders>(
                "SELECT * FROM acq_orders_rows_customer_orders WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND customer_order_year = @customer_order_year AND customer_order_number = @customer_order_number AND customer_order_row = @customer_order_row",
                new { company_id = company_id, order_id = order_id, order_row_id = order_row_id, customer_order_year = customer_order_year, customer_order_number = customer_order_number, customer_order_row = customer_order_row })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, long order_id, int order_row_id, int customer_order_year, int customer_order_number, int customer_order_row)
    {
        try
        {
            using var connection = GetOpenConnection();



            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM acq_orders_rows_customer_orders WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND customer_order_year = @customer_order_year AND customer_order_number = @customer_order_number AND customer_order_row = @customer_order_row",
                new { company_id = company_id, order_id = order_id, order_row_id = order_row_id, customer_order_year = customer_order_year, customer_order_number = customer_order_number, customer_order_row = customer_order_row }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO acq_orders_rows_customer_orders (company_id,order_id,order_row_id,customer_order_year,customer_order_number,customer_order_row,quantity_needed,quantity_received,quantity_original) OUTPUT INSERTED.rv VALUES(@company_id,@order_id,@order_row_id,@customer_order_year,@customer_order_number,@customer_order_row,@quantity_needed,@quantity_received,@quantity_original)";
    public string UPDATE_QUERY => "UPDATE acq_orders_rows_customer_orders SET company_id = @company_id,order_id = @order_id,order_row_id = @order_row_id,customer_order_year = @customer_order_year,customer_order_number = @customer_order_number,customer_order_row = @customer_order_row,quantity_needed = @quantity_needed,quantity_received = @quantity_received,quantity_original = @quantity_original OUTPUT INSERTED.rv WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND customer_order_year = @customer_order_year AND customer_order_number = @customer_order_number AND customer_order_row = @customer_order_row AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_orders_rows_customer_orders OUTPUT DELETED.rv WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND customer_order_year = @customer_order_year AND customer_order_number = @customer_order_number AND customer_order_row = @customer_order_row AND rv = @rv";
    public bool Insert(acq_orders_rows_customer_orders Model)
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

    public bool Update(acq_orders_rows_customer_orders Model)
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

    public bool Delete(acq_orders_rows_customer_orders Model)
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

    #endregion
}