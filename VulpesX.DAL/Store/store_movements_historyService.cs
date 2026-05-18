using VulpesX.DAL;

namespace VulpesX.DAL.Store;

public interface Istore_movements_historyRepository
{
    ObservableCollection<store_movements_history>? GetList(string CompanyID, long ID);

    store_movements_history? Get(string company_id, long id, int sequence);

    int? GetLastSequence(string company_id, long id);

    #region CRUD
    string INSERT_QUERY { get; }
    bool Insert(store_movements_history Model);
    #endregion
}

public class store_movements_historyRepository : RepositoryBase, Istore_movements_historyRepository
{
    public store_movements_historyRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<store_movements_history>? GetList(string CompanyID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_movements_history>(
                @"SELECT * FROM store_movements_history
                        WHERE company_id=@cid AND id=@id", new { cid = CompanyID, id = ID });

            return new ObservableCollection<store_movements_history>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_movements_history? Get(string company_id, long id, int sequence)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_movements_history>(
                "SELECT * FROM store_movements_history WHERE company_id = @company_id AND id = @id AND sequence = @sequence",
                new { company_id = company_id, id = id, sequence = sequence })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public int? GetLastSequence(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT MAX(sequence) FROM store_movements_history WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) ?? 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO store_movements_history (company_id,id,sequence,date,store_id,product_id,causal_id,quantity,document_id,document_date,document_row,note,added,add_user,order_id,engage_id,lot,expire,goods_receipt_id,goods_location,supplier_id,document_year,supplier_lot,price) OUTPUT INSERTED.rv VALUES(@company_id,@id,@sequence,@date,@store_id,@product_id,@causal_id,@quantity,@document_id,@document_date,@document_row,@note,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user,@order_id,@engage_id,@lot,@expire,@goods_receipt_id,@goods_location,@supplier_id,@document_year,@supplier_lot,@price)";
    public bool Insert(store_movements_history Model)
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
    #endregion
}