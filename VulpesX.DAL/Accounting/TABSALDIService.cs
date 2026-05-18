using VulpesX.DAL;

namespace VulpesX.Services.Accounting;

public interface ITABSALDIRepository
{
    ObservableCollection<TABSALDI>? GetList();

    TABSALDI? Get(string salsoc, int salann);

    bool Exists(string salsoc, int salann);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TABSALDI Model);

    bool Update(TABSALDI Model);

    bool Delete(TABSALDI Model);

    #endregion
}

public class TABSALDIRepository : RepositoryBase, ITABSALDIRepository
{
    public TABSALDIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TABSALDI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TABSALDI>(
                "SELECT * FROM TABSALDI");

            return new ObservableCollection<TABSALDI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TABSALDI? Get(string salsoc, int salann)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TABSALDI>(
                "SELECT * FROM TABSALDI WHERE salsoc = @salsoc AND salann = @salann",
                new { salsoc = salsoc, salann = salann })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string salsoc, int salann)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TABSALDI WHERE salsoc = @salsoc AND salann = @salann",
                new { salsoc = salsoc, salann = salann }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TABSALDI (salsoc,salann) VALUES(@salsoc,@salann)";
    public string UPDATE_QUERY => "UPDATE TABSALDI SET salsoc = @salsoc,salann = @salann WHERE salsoc = @salsoc AND salann = @salann";
    public string DELETE_QUERY => "DELETE FROM TABSALDI WHERE salsoc = @salsoc AND salann = @salann";
    public bool Insert(TABSALDI Model)
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

    public bool Update(TABSALDI Model)
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

    public bool Delete(TABSALDI Model)
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