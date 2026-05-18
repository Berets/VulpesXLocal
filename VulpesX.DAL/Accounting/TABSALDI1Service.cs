namespace VulpesX.DAL.Accounting;

public interface ITABSALDI1Repository
{
    ObservableCollection<TABSALDI1>? GetList();

    ObservableCollection<TABSALDI1>? GetListByYear(string CompanyID, int Year);

    TABSALDI1? Get(string salsoc, int salann, int salmes, DateTime salpag);

    TABSALDI1? GetLastByMonth(string salsoc, int salann, int salmes);

    TABSALDI1? GetLastByPreviousMonth(string salsoc, int salann, int salmes);

    bool Exists(string salsoc, int salann, int salmes, DateTime salpag);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TABSALDI1 Model);

    bool Update(TABSALDI1 Model);

    bool Delete(TABSALDI1 Model);

    #endregion
}

public class TABSALDI1Repository : RepositoryBase, ITABSALDI1Repository
{
    public TABSALDI1Repository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TABSALDI1>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TABSALDI1>(
                "SELECT * FROM TABSALDI1");

            return new ObservableCollection<TABSALDI1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TABSALDI1>? GetListByYear(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TABSALDI1>(
                @"SELECT * FROM TABSALDI1
                        WHERE salsoc=@cid AND salann=@yea
                        ORDER BY salmes, salpag",
                new { cid = CompanyID, yea = Year });

            return new ObservableCollection<TABSALDI1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TABSALDI1? Get(string salsoc, int salann, int salmes, DateTime salpag)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TABSALDI1>(
                "SELECT * FROM TABSALDI1 WHERE salsoc = @salsoc AND salann = @salann AND salmes = @salmes AND salpag = @salpag",
                new { salsoc = salsoc, salann = salann, salmes = salmes, salpag = salpag })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TABSALDI1? GetLastByMonth(string salsoc, int salann, int salmes)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TABSALDI1>(
                @"SELECT TOP(1) * FROM TABSALDI1 
                        WHERE salsoc = @salsoc AND salann = @salann AND salmes = @salmes
                        ORDER BY salpag DESC",
                new { salsoc = salsoc, salann = salann, salmes = salmes })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TABSALDI1? GetLastByPreviousMonth(string salsoc, int salann, int salmes)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TABSALDI1>(
                @"SELECT TOP(1) * FROM TABSALDI1 
                        WHERE salsoc = @salsoc AND salann = @salann AND salmes < @salmes
                        ORDER BY salpag DESC",
                new { salsoc = salsoc, salann = salann, salmes = salmes })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string salsoc, int salann, int salmes, DateTime salpag)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TABSALDI1 WHERE salsoc = @salsoc AND salann = @salann AND salmes = @salmes AND salpag = @salpag",
                new { salsoc = salsoc, salann = salann, salmes = salmes, salpag = salpag }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TABSALDI1 (salsoc,salann,salmes,salpag,salsal,saldeb,salcom) VALUES(@salsoc,@salann,@salmes,@salpag,@salsal,@saldeb,@salcom)";
    public string UPDATE_QUERY => "UPDATE TABSALDI1 SET salsoc = @salsoc,salann = @salann,salmes = @salmes,salpag = @salpag,salsal = @salsal,saldeb = @saldeb, salcom = @salcom WHERE salsoc = @salsoc AND salann = @salann AND salmes = @salmes AND salpag = @salpag";
    public string DELETE_QUERY => "DELETE FROM TABSALDI1 WHERE salsoc = @salsoc AND salann = @salann AND salmes = @salmes AND salpag = @salpag";
    public bool Insert(TABSALDI1 Model)
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

    public bool Update(TABSALDI1 Model)
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

    public bool Delete(TABSALDI1 Model)
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