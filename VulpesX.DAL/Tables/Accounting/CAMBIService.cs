

namespace VulpesX.DAL.Tables.Accounting;

public interface ICAMBIRepository
{
    ObservableCollection<CAMBI>? GetList();

    CAMBI? Get(string UIC, string SWIFT, DateTime DT);

    bool Exists(string UIC, string SWIFT, DateTime DT);

    #region CRUD
    bool Insert(CAMBI Model);

    bool Update(CAMBI Model);

    bool Delete(CAMBI Model);

    string? Validate(CAMBI Model, bool IsInsert);
    #endregion
}

public class CAMBIRepository : RepositoryBase, ICAMBIRepository
{
    public CAMBIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CAMBI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

                var list = connection.Query<CAMBI>(
                    "SELECT * FROM CAMBI");

                return new ObservableCollection<CAMBI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAMBI? Get(string UIC, string SWIFT, DateTime DT)
    {
        try
        {
            using var connection = GetOpenConnection();

          
                return connection.Query<CAMBI>(
                    "SELECT * FROM CAMBI WHERE camlis = @uic AND camswi = @swift AND camdat = @dt",
                    new { uic = UIC, swift = SWIFT, dt = DT })
                    .FirstOrDefault();
        
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string UIC, string SWIFT, DateTime DT)
    {
        try
        {
            using var connection = GetOpenConnection();

        
                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM CAMBI WHERE camlis = @uic AND camswi = @swift AND camdat = @dt",
                    new { uic = UIC, swift = SWIFT, dt = DT }) > 0;
      
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(CAMBI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

      
                var result = connection.Execute(
                    "INSERT INTO CAMBI (CAMLIS,CAMSWI,CAMDAT,CAMDIV,CAMORA,CAMSCA,CAMACQ,CAMVEN,CAMDES) OUTPUT INSERTED.rv VALUES(@CAMLIS,@CAMSWI,@CAMDAT,@CAMDIV,@CAMORA,@CAMSCA,@CAMACQ,@CAMVEN,@CAMDES)",
                    Model);
                if (result >0)
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

    public bool Update(CAMBI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

         
                var result = connection.ExecuteScalar(
                    "UPDATE CAMBI SET CAMLIS = @CAMLIS,CAMSWI = @CAMSWI,CAMDAT = @CAMDAT,CAMDIV = @CAMDIV,CAMORA = @CAMORA,CAMSCA = @CAMSCA,CAMACQ = @CAMACQ,CAMVEN = @CAMVEN,CAMDES = @CAMDES OUTPUT INSERTED.rv WHERE camlis = @camlis AND camswi = @camswi AND camdat = @camdat AND rv = @rv",
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

    public bool Delete(CAMBI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

          
                var result = connection.Execute(
                    "DELETE FROM CAMBI OUTPUT DELETED.rv WHERE camlis = @camlis AND camswi = @camswi AND camdat = @camdat AND rv = @rv",
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

    public string? Validate(CAMBI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.CAMLIS) && !string.IsNullOrWhiteSpace(Model.CAMSWI) && IsInsert && !Exists(Model.CAMLIS, Model.CAMSWI, Model.CAMDAT)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.CAMDIV))
                {
                    return null;
                }
                else
                { return "La divisa e' obbligatoria"; }
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