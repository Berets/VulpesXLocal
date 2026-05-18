using VulpesX.DAL;

namespace VulpesX.DAL.General;

public interface INOTECLI1Repository
{

    ObservableCollection<NOTECLI1>? GetList(int CustomerID);

    ObservableCollection<NOTECLI1>? GetListForInvoices(int CustomerID);

    NOTECLI1? Get(int CustomerID, string ID, int RowID);

    bool Exists(int NOTCLI, string NOTETI, int notrig);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(NOTECLI1 Model);

    bool Update(NOTECLI1 Model);

    bool Delete(NOTECLI1 Model);

    string? Validate(NOTECLI1 Model, bool IsInsert);
    #endregion
}

public class NOTECLI1Repository : RepositoryBase, INOTECLI1Repository
{
    public NOTECLI1Repository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NOTECLI1>? GetList(int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<NOTECLI1>(
                "SELECT * FROM NOTECLI1 WHERE NOTCLI = @notcli ORDER BY NOTETI, notrig",
                new { notcli = CustomerID });

            return new ObservableCollection<NOTECLI1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<NOTECLI1>? GetListForInvoices(int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<NOTECLI1>(
                @"SELECT * FROM NOTECLI1 
                        WHERE NOTCLI = @notcli AND NOTETI = 'F' AND NOTDES IS NOT NULL AND NOTDES <> ''
                        ORDER BY notrig",
                new { notcli = CustomerID });

            return new ObservableCollection<NOTECLI1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NOTECLI1? Get(int CustomerID, string ID, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NOTECLI1>(
                "SELECT * FROM NOTECLI1 WHERE NOTCLI = @notcli AND NOTETI = @noteti AND notrig = @notrig",
                new { notcli = CustomerID, noteti = ID, notrig = RowID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int NOTCLI, string NOTETI, int notrig)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM NOTECLI1 WHERE NOTCLI = @NOTCLI AND NOTETI = @NOTETI AND notrig = @notrig",
                new { NOTCLI = NOTCLI, NOTETI = NOTETI, notrig = notrig }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO NOTECLI1 (NOTCLI,NOTETI,notrig,NOTRAG,notdes) OUTPUT INSERTED.rv VALUES(@NOTCLI,@NOTETI,@notrig,@NOTRAG,@notdes)";
    public string UPDATE_QUERY => "UPDATE NOTECLI1 SET NOTCLI = @NOTCLI,NOTETI = @NOTETI,notrig = @notrig,NOTRAG = @NOTRAG,notdes = @notdes OUTPUT INSERTED.rv WHERE NOTCLI = @NOTCLI AND NOTETI = @NOTETI AND notrig = @notrig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM NOTECLI1 OUTPUT DELETED.rv WHERE NOTCLI = @NOTCLI AND NOTETI = @NOTETI AND notrig = @notrig AND rv = @rv";

    public bool Insert(NOTECLI1 Model)
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

    public bool Update(NOTECLI1 Model)
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

    public bool Delete(NOTECLI1 Model)
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

    public string? Validate(NOTECLI1 Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.notdes))
            {
                return null;
            }
            else
            { return "La descrizione della nota e' obbligatoria"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class NOTECLI1UfpRepository : RepositoryBase, INOTECLI1Repository
{
    public NOTECLI1UfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NOTECLI1>? GetList(int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<NOTECLI1>(
                @$"SELECT *
                            FROM NOTECLI1 WHERE NOTCLI = @notcli ORDER BY NOTETI, notrig",
                new { notcli = CustomerID });

            return new ObservableCollection<NOTECLI1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<NOTECLI1>? GetListForInvoices(int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<NOTECLI1>(
                @"SELECT * FROM NOTECLI1 
                        WHERE NOTCLI = @notcli AND NOTETI = 'F' AND NOTDES IS NOT NULL AND NOTDES <> ''
                        ORDER BY notrig",
                new { notcli = CustomerID });

            return new ObservableCollection<NOTECLI1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NOTECLI1? Get(int CustomerID, string ID, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NOTECLI1>(
                "SELECT * FROM NOTECLI1 WHERE NOTCLI = @notcli AND NOTETI = @noteti AND notrig = @notrig",
                new { notcli = CustomerID, noteti = ID, notrig = RowID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int NOTCLI, string NOTETI, int notrig)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM NOTECLI1 WHERE NOTCLI = @NOTCLI AND NOTETI = @NOTETI AND notrig = @notrig",
                new { NOTCLI = NOTCLI, NOTETI = NOTETI, notrig = notrig }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO NOTECLI1 (NOTCLI,NOTETI,notrig,NOTRAG,notdes,nosoc,NOTTIP,NOTDIS,NOTRIF,NOTTESTO,NOTSUPP,NOTLOGO) OUTPUT INSERTED.rv VALUES(@NOTCLI,@NOTETI,@notrig,@NOTRAG,@notdes,@nosoc,@NOTTIP,@NOTDIS,@NOTRIF,@NOTTESTO,@NOTSUPP, @NOTLOGO)";
    public string UPDATE_QUERY => "UPDATE NOTECLI1 SET NOTCLI = @NOTCLI,NOTETI = @NOTETI,notrig = @notrig,NOTRAG = @NOTRAG,notdes = @notdes, ,nosoc = @nosoc,NOTTIP=@NOTTIP,NOTDIS=@NOTDIS,NOTRIF=@NOTRIF,NOTTESTO=@NOTTESTO,NOTSUPP=@NOTSUPP, NOTLOGO=@NOTLOGO OUTPUT INSERTED.rv WHERE nosoc = @nosoc AND NOTCLI = @NOTCLI AND NOTETI = @NOTETI AND notrig = @notrig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM NOTECLI1 OUTPUT DELETED.rv WHERE NOTCLI = @NOTCLI AND NOTETI = @NOTETI AND notrig = @notrig AND rv = @rv";

    public bool Insert(NOTECLI1 Model)
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

    public bool Update(NOTECLI1 Model)
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

    public bool Delete(NOTECLI1 Model)
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

    public string? Validate(NOTECLI1 Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.notdes))
            {
                return null;
            }
            else
            { return "La descrizione della nota e' obbligatoria"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}