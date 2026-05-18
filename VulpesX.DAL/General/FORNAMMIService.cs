namespace VulpesX.DAL.General;

public interface IFORNAMMIRepository
{
    ObservableCollection<FORNAMMI>? GetList(string CompanyID);

    FORNAMMI? Get(string CompanyID, int ID);

    bool Exists(string CompanyID, int ID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FORNAMMI Model);

    bool Update(FORNAMMI Model);

    bool Delete(FORNAMMI Model);

    string? Validate(FORNAMMI Model, bool IsInsert);
    #endregion
}

public class FORNAMMIRepository : RepositoryBase, IFORNAMMIRepository
{
    public FORNAMMIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FORNAMMI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FORNAMMI>(
                "SELECT * FROM FORNAMMI WHERE foraso = @foraso",
                new { foraso = CompanyID });

            return new ObservableCollection<FORNAMMI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FORNAMMI? Get(string CompanyID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FORNAMMI>(
                "SELECT * FROM FORNAMMI WHERE foraso = @foraso AND foraco = @foraco",
                new { foraso = CompanyID, foraco = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FORNAMMI WHERE foraso = @foraso AND foraco = @foraco",
                new { foraso = CompanyID, foraco = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FORNAMMI (Foraso,Foraco,FOASSF,FORTIV,zoncod,pfocod,specod,concod,foaass,foaali,FOABIF,FOCABF,FOCOCO,rivcod,foGRUP,foCONT,vetcod,FOFACT,scacod,codmag,FOIBAN,FOBBAN,FOBIC,FOCIN,FOAABI,FOACAB,FOACCN,FOIMBA,FOREGI) OUTPUT INSERTED.rv VALUES(@Foraso,@Foraco,@FOASSF,@FORTIV,@zoncod,@pfocod,@specod,@concod,@foaass,@foaali,@FOABIF,@FOCABF,@FOCOCO,@rivcod,@foGRUP,@foCONT,@vetcod,@FOFACT,@scacod,@codmag,@FOIBAN,@FOBBAN,@FOBIC,@FOCIN,@FOAABI,@FOACAB,@FOACCN,@FOIMBA,@FOREGI)";
    public string UPDATE_QUERY => "UPDATE FORNAMMI SET Foraso = @Foraso,Foraco = @Foraco,FOASSF = @FOASSF,FORTIV = @FORTIV,zoncod = @zoncod,pfocod = @pfocod,specod = @specod,concod = @concod,foaass = @foaass,foaali = @foaali,FOABIF = @FOABIF,FOCABF = @FOCABF,FOCOCO = @FOCOCO,rivcod = @rivcod,foGRUP = @foGRUP,foCONT = @foCONT,vetcod = @vetcod,FOFACT = @FOFACT,scacod = @scacod,codmag = @codmag,FOIBAN = @FOIBAN,FOBBAN = @FOBBAN,FOBIC = @FOBIC,FOCIN = @FOCIN,FOAABI = @FOAABI,FOACAB = @FOACAB,FOACCN = @FOACCN,FOIMBA = @FOIMBA,FOREGI = @FOREGI OUTPUT INSERTED.rv WHERE Foraso = @Foraso AND Foraco = @Foraco AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FORNAMMI OUTPUT DELETED.rv WHERE Foraso = @Foraso AND Foraco = @Foraco AND rv = @rv";
    public bool Insert(FORNAMMI Model)
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

    public bool Update(FORNAMMI Model)
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

    public bool Delete(FORNAMMI Model)
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

    public string? Validate(FORNAMMI Model, bool IsInsert)
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

public class FORNAMMIUfpRepository : RepositoryBase, IFORNAMMIRepository
{
    public FORNAMMIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FORNAMMI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FORNAMMI>(
                "SELECT * FROM FORNAMMI WHERE foraso = @foraso",
                new { foraso = CompanyID });

            return new ObservableCollection<FORNAMMI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FORNAMMI? Get(string CompanyID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FORNAMMI>(
                "SELECT * FROM FORNAMMI WHERE foraso = @foraso AND foraco = @foraco",
                new { foraso = CompanyID, foraco = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FORNAMMI WHERE foraso = @foraso AND foraco = @foraco",
                new { foraso = CompanyID, foraco = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FORNAMMI (Foraso,Foraco,FOASSF,FORTIV,zoncod,pfocod,specod,concod,foaass,foaali,FOABIF,FOCABF,FOCOCO,rivcod,foGRUP,foCONT,vetcod,FOFACT,scacod,codmag,FOIBAN,FOBBAN,FOBIC,FOCIN,FOAABI,FOACAB,FOACCN, focidi,foiva, focamb,focodcliedi,focodconsedi,foaccobb,focauacq) OUTPUT INSERTED.rv VALUES(@Foraso,@Foraco,@FOASSF,@FORTIV,@zoncod,@pfocod,@specod,@concod,@foaass,@foaali,@FOABIF,@FOCABF,@FOCOCO,@rivcod,@foGRUP,@foCONT,@vetcod,@FOFACT,@scacod,@codmag,@FOIBAN,@FOBBAN,@FOBIC,@FOCIN,@FOAABI,@FOACAB,@FOACCN,@focidi,@foiva, 0,@focodcliedi,@focodconsedi,@foaccobb,@focauacq)";
    public string UPDATE_QUERY => "UPDATE FORNAMMI SET Foraso = @Foraso,Foraco = @Foraco,FOASSF = @FOASSF,FORTIV = @FORTIV,zoncod = @zoncod,pfocod = @pfocod,specod = @specod,concod = @concod,foaass = @foaass,foaali = @foaali,FOABIF = @FOABIF,FOCABF = @FOCABF,FOCOCO = @FOCOCO,rivcod = @rivcod,foGRUP = @foGRUP,foCONT = @foCONT,vetcod = @vetcod,FOFACT = @FOFACT,scacod = @scacod,codmag = @codmag,FOIBAN = @FOIBAN,FOBBAN = @FOBBAN,FOBIC = @FOBIC,FOCIN = @FOCIN,FOAABI = @FOAABI,FOACAB = @FOACAB,FOACCN = @FOACCN,focidi=@focidi,foiva=@foiva, focamb = 0,focodcliedi=@focodcliedi,focodconsedi=@focodconsedi,foaccobb=@foaccobb,focauacq=@focauacq OUTPUT INSERTED.rv WHERE Foraso = @Foraso AND Foraco = @Foraco AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FORNAMMI OUTPUT DELETED.rv WHERE Foraso = @Foraso AND Foraco = @Foraco AND rv = @rv";
    public bool Insert(FORNAMMI Model)
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

    public bool Update(FORNAMMI Model)
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

    public bool Delete(FORNAMMI Model)
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

    public string? Validate(FORNAMMI Model, bool IsInsert)
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