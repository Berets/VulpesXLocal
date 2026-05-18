namespace VulpesX.DAL.Tables.Accounting;

public interface IVALUTERepository
{

    ObservableCollection<VALUTE>? GetList();

    VALUTE? Get(string Code, string ID);

    bool Insert(VALUTE Model);

    bool Update(VALUTE Model);

    bool Delete(VALUTE Model);

    string? Validate(VALUTE Model, bool IsInsert);
}

public class VALUTERepository : RepositoryBase, IVALUTERepository
{
    public VALUTERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<VALUTE>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<VALUTE>(
                "SELECT * FROM VALUTE");

            return new ObservableCollection<VALUTE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public VALUTE? Get(string Code, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<VALUTE>(
                "SELECT * FROM VALUTE WHERE VALCOD = @valcod AND VALDIV = @valdiv",
                new { valcod = Code, valdiv = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Insert(VALUTE Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO VALUTE (VALCOD,VALDIV,VALDES,VALTIP,VALCAM) OUTPUT INSERTED.rv VALUES(@VALCOD,@VALDIV,@VALDES,@VALTIP,@VALCAM)",
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

    public bool Update(VALUTE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE VALUTE SET VALCOD = @VALCOD,VALDIV = @VALDIV,VALDES = @VALDES,VALTIP = @VALTIP,VALCAM = @VALCAM OUTPUT INSERTED.rv WHERE VALCOD = @valcod AND VALDIV = @valdiv AND rv = @rv",
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

    public bool Delete(VALUTE Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM VALUTE OUTPUT DELETED.rv WHERE VALCOD = @valcod AND VALDIV = @valdiv AND rv = @rv",
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

    public string? Validate(VALUTE Model, bool IsInsert)
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
}