namespace VulpesX.DAL.Production;

public interface Ipro_ordine_historyRepository
{

    #region Operations
    string PRODUCTION_START_LAUNCH { get; }
    string PRODUCTION_LAUNCH { get; }
    string PRODUCTION_LAUNCH_NOPROD { get; }
    string PRODUCTION_LAUNCH_WAIT { get; }
    string PRODUCTION_LAUNCH_OK { get; }
    string PRODUCTION_START_HALFWORK { get; }
    string PRODUCTION_START_RAWS { get; }
    string PRODUCTION_CANCELED_LAUNCH { get; }
    string PRODUCTION_CLEAR_ENGAGES { get; }
    string PRODUCTION_UNLOCK { get; }
    string PRODUCTION_MANUAL_CHANGE_STATUS { get; }
    #endregion

    ObservableCollection<pro_ordine_history>? GetList();

    ObservableCollection<pro_ordine_history>? GetListByOrder(string CompanyID, string OrderID);

    pro_ordine_history? Get(string company_id, string id, DateTime istant);

    #region CRUD
    // if changes to schema set istant always SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time' in INSERT and remove rv
    string INSERT_QUERY {get;}
    string UPDATE_QUERY {get;}
    string DELETE_QUERY { get; }
    bool Insert(pro_ordine_history Model);

    bool Update(pro_ordine_history Model);

    bool Delete(pro_ordine_history Model);
    #endregion
}

public class pro_ordine_historyRepository : RepositoryBase, Ipro_ordine_historyRepository
{
    public pro_ordine_historyRepository(IConnectionFactory factory) : base(factory)
    {
    }

    #region Operations
    public string PRODUCTION_START_LAUNCH => "Start launch";
    public string PRODUCTION_LAUNCH => "Launch";
    public string PRODUCTION_LAUNCH_NOPROD => "Launch without production";
    public string PRODUCTION_LAUNCH_WAIT => "Launch waited";
    public string PRODUCTION_LAUNCH_OK => "Launch OK";
    public string PRODUCTION_START_HALFWORK => "Started halfwork selection";
    public string PRODUCTION_START_RAWS => "Started raws selection";
    public string PRODUCTION_CANCELED_LAUNCH => "Launch canceled";
    public string PRODUCTION_CLEAR_ENGAGES => "Cleared all engages";
    public string PRODUCTION_UNLOCK => "Unlock";
    public string PRODUCTION_MANUAL_CHANGE_STATUS => "Manual status changed";
    #endregion

    public ObservableCollection<pro_ordine_history>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

         
                var list = connection.Query<pro_ordine_history>(
                    "SELECT * FROM pro_ordine_history");

                return new ObservableCollection<pro_ordine_history>(list);
           
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_history>? GetListByOrder(string CompanyID, string OrderID)
    {
        try
        {
            using var connection = GetOpenConnection();

         
                var list = connection.Query<pro_ordine_history>(
                    @"SELECT * FROM pro_ordine_history
                        WHERE company_id=@cid AND id=@id", new { cid = CompanyID, id = OrderID });

                return new ObservableCollection<pro_ordine_history>(list);
            
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public pro_ordine_history? Get(string company_id, string id, DateTime istant)
    {
        try
        {
            using var connection = GetOpenConnection();

                return connection.Query<pro_ordine_history>(
                    "SELECT * FROM pro_ordine_history WHERE company_id = @company_id AND id = @id AND istant = @istant",
                    new { company_id = company_id, id = id, istant = istant })
                    .FirstOrDefault();
           
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    // if changes to schema set istant always SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time' in INSERT and remove rv
    public string INSERT_QUERY => "INSERT INTO pro_ordine_history (company_id,id,istant,operation,cancelled_engages,production_quantity,engaged_quantity,previous_state,username,client_name) VALUES(@company_id,@id,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@operation,@cancelled_engages,@production_quantity,@engaged_quantity,@previous_state,@username,@client_name)";
    public string UPDATE_QUERY => "UPDATE pro_ordine_history SET company_id = @company_id,id = @id,istant = @istant,operation = @operation,cancelled_engages = @cancelled_engages,production_quantity = @production_quantity,engaged_quantity = @engaged_quantity,previous_state = @previous_state,username = @username,client_name = @client_name WHERE company_id = @company_id AND id = @id AND istant = @istant";
    public string DELETE_QUERY => "DELETE FROM pro_ordine_history WHERE company_id = @company_id AND id = @id AND istant = @istant";
    public bool Insert(pro_ordine_history Model)
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

    public bool Update(pro_ordine_history Model)
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

    public bool Delete(pro_ordine_history Model)
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