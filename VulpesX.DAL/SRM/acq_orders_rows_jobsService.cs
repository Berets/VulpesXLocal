namespace VulpesX.DAL.SRM;

public interface Iacq_orders_rows_jobsRepository
{
    ObservableCollection<acq_orders_rows_jobs>? GetList(string CompanyID, long PurchaseOrderID);

    ObservableCollection<acq_orders_rows_jobs>? GetList(string CompanyID, long PurchaseOrderID, int PurchaseOrderRow);

    acq_orders_rows_jobs? Get(string company_id, long order_id, int order_row_id, string job_id);

    bool Exists(string company_id, long order_id, int order_row_id, string job_id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(acq_orders_rows_jobs Model);

    bool Update(acq_orders_rows_jobs Model);

    bool Delete(acq_orders_rows_jobs Model);

    #endregion
}

public class acq_orders_rows_jobsRepository : RepositoryBase, Iacq_orders_rows_jobsRepository
{
    public acq_orders_rows_jobsRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<acq_orders_rows_jobs>? GetList(string CompanyID, long PurchaseOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<acq_orders_rows_jobs>(
                @"SELECT * FROM acq_orders_rows_jobs
                        WHERE company_id=@cid AND order_id=@poid",
                new { cid = CompanyID, poid = PurchaseOrderID });

            return new ObservableCollection<acq_orders_rows_jobs>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<acq_orders_rows_jobs>? GetList(string CompanyID, long PurchaseOrderID, int PurchaseOrderRow)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<acq_orders_rows_jobs>(
                @"SELECT * FROM acq_orders_rows_jobs
                        WHERE company_id=@cid AND order_id=@poid AND order_row_id=@por",
                new { cid = CompanyID, poid = PurchaseOrderID, por = PurchaseOrderRow });

            return new ObservableCollection<acq_orders_rows_jobs>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_rows_jobs? Get(string company_id, long order_id, int order_row_id, string job_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<acq_orders_rows_jobs>(
                "SELECT * FROM acq_orders_rows_jobs WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND job_id = @job_id",
                new { company_id = company_id, order_id = order_id, order_row_id = order_row_id, job_id = job_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, long order_id, int order_row_id, string job_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM acq_orders_rows_jobs WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND job_id = @job_id",
                new { company_id = company_id, order_id = order_id, order_row_id = order_row_id, job_id = job_id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO acq_orders_rows_jobs (company_id,order_id,order_row_id,job_id,quantity_needed,quantity_received,quantity_original) OUTPUT INSERTED.rv VALUES(@company_id,@order_id,@order_row_id,@job_id,@quantity_needed,@quantity_received,@quantity_original)";
    public string UPDATE_QUERY => "UPDATE acq_orders_rows_jobs SET company_id = @company_id,order_id = @order_id,order_row_id = @order_row_id,job_id = @job_id,quantity_needed = @quantity_needed,quantity_received = @quantity_received,quantity_original = @quantity_original OUTPUT INSERTED.rv WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND job_id = @job_id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_orders_rows_jobs OUTPUT DELETED.rv WHERE company_id = @company_id AND order_id = @order_id AND order_row_id = @order_row_id AND job_id = @job_id AND rv = @rv";
    public bool Insert(acq_orders_rows_jobs Model)
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

    public bool Update(acq_orders_rows_jobs Model)
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

    public bool Delete(acq_orders_rows_jobs Model)
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