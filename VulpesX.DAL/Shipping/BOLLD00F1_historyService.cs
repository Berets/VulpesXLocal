namespace VulpesX.DAL.Shipping;

public interface IBOLLD00F1_historyRepository
{
    ObservableCollection<BOLLD00F1_history>? GetList();

    BOLLD00F1_history? Get(string bolsoc, int BTANNO, int BTBOLL, int revision, int BORIGB, int boposc, string bolott);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(BOLLD00F1_history Model);

    bool Update(BOLLD00F1_history Model);

    bool Delete(BOLLD00F1_history Model);

    #endregion
}

public class BOLLD00F1_historyRepository : RepositoryBase, IBOLLD00F1_historyRepository
{
    public BOLLD00F1_historyRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BOLLD00F1_history>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BOLLD00F1_history>(
                "SELECT * FROM BOLLD00F1_history");

            return new ObservableCollection<BOLLD00F1_history>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BOLLD00F1_history? Get(string bolsoc, int BTANNO, int BTBOLL, int revision, int BORIGB, int boposc, string bolott)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BOLLD00F1_history>(
                "SELECT * FROM BOLLD00F1_history WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL, revision = revision, BORIGB = BORIGB, boposc = boposc, bolott = bolott })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO BOLLD00F1_history (bolsoc,BTANNO,BTBOLL,revision,BORIGB,boposc,bolott,boqtlo,store_id,product_id) VALUES(@bolsoc,@BTANNO,@BTBOLL,@revision,@BORIGB,@boposc,@bolott,@boqtlo,@store_id,@product_id)";
    public string UPDATE_QUERY => "UPDATE BOLLD00F1_history SET bolsoc = @bolsoc,BTANNO = @BTANNO,BTBOLL = @BTBOLL,revision = @revision,BORIGB = @BORIGB,boposc = @boposc,bolott = @bolott,boqtlo = @boqtlo,store_id = @store_id,product_id = @product_id WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott";
    public string DELETE_QUERY => "DELETE FROM BOLLD00F1_history WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott";
    public bool Insert(BOLLD00F1_history Model)
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

    public bool Update(BOLLD00F1_history Model)
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

    public bool Delete(BOLLD00F1_history Model)
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