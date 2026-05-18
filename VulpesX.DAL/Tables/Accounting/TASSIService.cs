using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface ITASSIRepository
{

    ObservableCollection<TASSI>? GetList(string CompanyID);

    TASSI? Get(string CompanyID, DateTime ID);

    bool Insert(TASSI Model);

    bool Update(TASSI Model);

    bool Delete(TASSI Model);
}

public class TASSIRepository : RepositoryBase, ITASSIRepository
{
    public TASSIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TASSI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TASSI>(
                "SELECT * FROM TASSI WHERE tassoc = @cid",
                new { cid = CompanyID });

            return new ObservableCollection<TASSI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TASSI? Get(string CompanyID, DateTime ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TASSI>(
                "SELECT * FROM TASSI WHERE tassoc = @cid AND tasdat = @tasdat",
                new { cid = CompanyID, tasdat = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Insert(TASSI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO TASSI (tassoc,tasdat,tasper) OUTPUT INSERTED.rv VALUES(@tassoc,@tasdat,@tasper)",
                Model);
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

    public bool Update(TASSI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE TASSI SET tassoc = @tassoc,tasdat = @tasdat,tasper = @tasper OUTPUT INSERTED.rv WHERE tassoc = @tassoc AND tasdat = @tasdat AND rv = @rv",
                Model);
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

    public bool Delete(TASSI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "DELETE FROM TASSI OUTPUT DELETED.rv WHERE tassoc = @tassoc AND tasdat = @tasdat AND rv = @rv",
                Model);
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
}