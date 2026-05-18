namespace VulpesX.DAL.SRM;

public interface Iacq_orders_rows_rdasRepository
{
    ObservableCollection<acq_orders_rows_rdas>? GetList(string CompanyID, long PurchaseOrderID);

    ObservableCollection<acq_orders_rows_rdas>? GetList(string CompanyID, long PurchaseOrderID, int PurchaseOrderRow);

    acq_orders_rows_rdas? Get(string company_id, long order_id, int order_row_id, long rda_id);

    bool Exists(string company_id, long order_id, int order_row_id, long rda_id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(acq_orders_rows_rdas Model);

    bool Update(acq_orders_rows_rdas Model);

    bool Delete(acq_orders_rows_rdas Model);

    #endregion
}

public class acq_orders_rows_rdasRepository : RepositoryBase, Iacq_orders_rows_rdasRepository
{
    public acq_orders_rows_rdasRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<acq_orders_rows_rdas>? GetList(string CompanyID, long PurchaseOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<acq_orders_rows_rdas>(
                @"SELECT * FROM acq_orders_rows_rdas
                        WHERE company_id=@cid AND order_id=@poid",
                new { cid = CompanyID, poid = PurchaseOrderID });

            return new ObservableCollection<acq_orders_rows_rdas>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<acq_orders_rows_rdas>? GetList(string CompanyID, long PurchaseOrderID, int PurchaseOrderRow)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<acq_orders_rows_rdas>(
                @"SELECT * FROM acq_orders_rows_rdas
                        WHERE company_id=@cid AND order_id=@poid AND order_row_id=@por",
                new { cid = CompanyID, poid = PurchaseOrderID, por = PurchaseOrderRow });

            return new ObservableCollection<acq_orders_rows_rdas>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_rows_rdas? Get(string company_id, long order_id, int order_row_id, long rda_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<acq_orders_rows_rdas>(
                "SELECT * FROM acq_orders_rows_rdas WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND rda_id = @rda_id",
                new { company_id = company_id, order_id = order_id, order_row_id = order_row_id, rda_id = rda_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, long order_id, int order_row_id, long rda_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM acq_orders_rows_rdas WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND rda_id = @rda_id",
                new { company_id = company_id, order_id = order_id, order_row_id = order_row_id, rda_id = rda_id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO acq_orders_rows_rdas (company_id,order_id,order_row_id,rda_id) OUTPUT INSERTED.rv VALUES(@company_id,@order_id,@order_row_id,@rda_id)";
    public string UPDATE_QUERY => "UPDATE acq_orders_rows_rdas SET company_id = @company_id,order_id = @order_id,order_row_id = @order_row_id,rda_id = @rda_id OUTPUT INSERTED.rv WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND rda_id = @rda_id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_orders_rows_rdas OUTPUT DELETED.rv WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND rda_id = @rda_id AND rv = @rv";
    public bool Insert(acq_orders_rows_rdas Model)
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

    public bool Update(acq_orders_rows_rdas Model)
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

    public bool Delete(acq_orders_rows_rdas Model)
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