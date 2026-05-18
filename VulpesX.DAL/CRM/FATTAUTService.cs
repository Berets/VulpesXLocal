namespace VulpesX.DAL.CRM;

public interface IFATTAUTRepository
{
    ObservableCollection<FATTAUT>? GetList();

    FATTAUT? Get(string FTAUSC, int FTAUAN, int FTAUNUM);

    bool Exists(string FTAUSC, int FTAUAN, int FTAUNUM);

    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FATTAUT Model);

    bool Update(FATTAUT Model);

    bool Delete(FATTAUT Model);

    string? Validate(FATTAUT Model, bool IsInsert);
}

public class FATTAUTRepository : RepositoryBase, IFATTAUTRepository
{
    public FATTAUTRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FATTAUT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FATTAUT>(
                "SELECT * FROM FATTAUT");

            return new ObservableCollection<FATTAUT>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FATTAUT? Get(string FTAUSC, int FTAUAN, int FTAUNUM)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FATTAUT>(
                "SELECT * FROM FATTAUT WHERE FTAUSC = @FTAUSC AND FTAUAN = @FTAUAN AND FTAUNUM = @FTAUNUM",
                new { FTAUSC, FTAUAN, FTAUNUM })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string FTAUSC, int FTAUAN, int FTAUNUM)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FATTAUT WHERE FTAUSC = @FTAUSC AND FTAUAN = @FTAUAN AND FTAUNUM = @FTAUNUM",
                new { FTAUSC, FTAUAN, FTAUNUM }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTAUT (FTAUSC,FTAUAN,FTAUNUM,FTAUCOF,FTAUDATRIC,FTAUINDSDI,FTAUDATFAT,FTAUNUMFAT,FTAPNAN,FTAPNRE) OUTPUT INSERTED.rv VALUES(@FTAUSC,@FTAUAN,@FTAUNUM,@FTAUCOF,@FTAUDATRIC,@FTAUINDSDI,@FTAUDATFAT,@FTAUNUMFAT,@FTAPNAN,@FTAPNRE)";
    public string UPDATE_QUERY => "UPDATE FATTAUT SET FTAUSC = @FTAUSC,FTAUAN = @FTAUAN,FTAUNUM = @FTAUNUM,FTAUCOF = @FTAUCOF,FTAUDATRIC = @FTAUDATRIC,FTAUINDSDI = @FTAUINDSDI,FTAUDATFAT = @FTAUDATFAT,FTAUNUMFAT = @FTAUNUMFAT,FTAPNAN = @FTAPNAN,FTAPNRE = @FTAPNRE OUTPUT INSERTED.rv WHERE FTAUSC = @FTAUSC AND FTAUAN = @FTAUAN AND FTAUNUM = @FTAUNUM AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTAUT OUTPUT DELETED.rv WHERE FTAUSC = @FTAUSC AND FTAUAN = @FTAUAN AND FTAUNUM = @FTAUNUM AND rv = @rv";

    public bool Insert(FATTAUT Model)
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

    public bool Update(FATTAUT Model)
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

    public bool Delete(FATTAUT Model)
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

    public string? Validate(FATTAUT Model, bool IsInsert)
    {
        try
        {
            if (IsInsert && !Exists(Model.FTAUSC, Model.FTAUAN, Model.FTAUNUM) || !IsInsert)
            {
                if (!string.IsNullOrEmpty(Model.FTAUNUMFAT) &&
                Model.FTAUCOF.HasValue && Model.FTAUDATRIC.HasValue && Model.FTAUDATRIC.HasValue)
                {
                    return null;
                }
                else
                { return "Tutti i dati sono obbligatori"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}