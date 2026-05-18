using VulpesX.DAL;

namespace VulpesX.DAL.General;

public interface INOTEFORRepository
{

    ObservableCollection<NOTEFOR>? GetList(int SupplierID);

    NOTEFOR? Get(int SupplierID, int RowID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(NOTEFOR Model);

    bool Update(NOTEFOR Model);

    bool Delete(NOTEFOR Model);

    string? Validate(NOTEFOR Model, bool IsInsert);
    #endregion
}

public class NOTEFORRepository : RepositoryBase, INOTEFORRepository
{
    public NOTEFORRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NOTEFOR>? GetList(int SupplierID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<NOTEFOR>(
                "SELECT * FROM NOTEFOR WHERE Nofcod = @nofcod ORDER BY nofrig",
                new { nofcod = SupplierID });

            return new ObservableCollection<NOTEFOR>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NOTEFOR? Get(int SupplierID, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NOTEFOR>(
                "SELECT * FROM NOTEFOR WHERE Nofcod = @nofcod AND nofrig = @nofrig",
                new { nofcod = SupplierID, nofrig = RowID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO NOTEFOR (Nofcod,nofrig,Nofnot) OUTPUT INSERTED.rv VALUES(@Nofcod,@nofrig,@Nofnot)";
    public string UPDATE_QUERY => "UPDATE NOTEFOR SET Nofcod = @Nofcod,nofrig = @nofrig,Nofnot = @Nofnot OUTPUT INSERTED.rv WHERE Nofcod = @Nofcod AND nofrig = @nofrig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM NOTEFOR OUTPUT DELETED.rv WHERE Nofcod = @Nofcod AND nofrig = @nofrig AND rv = @rv";
    public bool Insert(NOTEFOR Model)
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

    public bool Update(NOTEFOR Model)
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

    public bool Delete(NOTEFOR Model)
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

    public string? Validate(NOTEFOR Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class NOTEFORUfpRepository : RepositoryBase, INOTEFORRepository
{
    public NOTEFORUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NOTEFOR>? GetList(int SupplierID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<NOTEFOR>(
                "SELECT * FROM NOTEFOR WHERE Nofcod = @nofcod ORDER BY nofrig",
                new { nofcod = SupplierID });

            return new ObservableCollection<NOTEFOR>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NOTEFOR? Get(int SupplierID, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NOTEFOR>(
                "SELECT * FROM NOTEFOR WHERE Nofcod = @nofcod AND nofrig = @nofrig",
                new { nofcod = SupplierID, nofrig = RowID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO NOTEFOR (nofsoc,Nofcod,nofrig,Nofnot) OUTPUT INSERTED.rv VALUES(@nofsoc,@Nofcod,@nofrig,@Nofnot)";
    public string UPDATE_QUERY => "UPDATE NOTEFOR SET Nofcod = @Nofcod,nofrig = @nofrig,Nofnot = @Nofnot OUTPUT INSERTED.rv WHERE Nofcod = @Nofcod AND nofrig = @nofrig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM NOTEFOR OUTPUT DELETED.rv WHERE Nofcod = @Nofcod AND nofrig = @nofrig AND rv = @rv";
    public bool Insert(NOTEFOR Model)
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

    public bool Update(NOTEFOR Model)
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

    public bool Delete(NOTEFOR Model)
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

    public string? Validate(NOTEFOR Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}