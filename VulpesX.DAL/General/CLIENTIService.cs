using VulpesX.DAL;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.General;

public interface ICLIENTIRepository
{
    ObservableCollection<CLIENTI>? GetList();

    CLIENTI? Get(int CLIENT);

    bool Exists(int CLIENT);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(CLIENTI Model);

    bool Update(CLIENTI Model);

    bool Delete(CLIENTI Model);

    string? Validate(CLIENTI Model, bool IsInsert);
    #endregion
}

public class CLIENTIRepository : RepositoryBase, ICLIENTIRepository
{
    public CLIENTIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CLIENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<CLIENTI>(
                "SELECT * FROM CLIENTI");

            return new ObservableCollection<CLIENTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CLIENTI? Get(int CLIENT)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CLIENTI>(
                "SELECT * FROM CLIENTI WHERE CLIENT = @CLIENT",
                new { CLIENT = CLIENT })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int CLIENT)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CLIENTI WHERE CLIENT = @CLIENT",
                new { CLIENT = CLIENT }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CLIENTI (CLIENT,CLNOME,CLNUTE,CLNUFA,classo,classa,solcod,CLREDI,CLRECO,CLREAC,CLCCOL,ammcod,climail,Clisit,CLSOSP,clicel,cliperi,clcouff,clpaym,clcoddest,clpec) OUTPUT INSERTED.rv VALUES(@CLIENT,@CLNOME,@CLNUTE,@CLNUFA,@classo,@classa,@solcod,@CLREDI,@CLRECO,@CLREAC,@CLCCOL,@ammcod,@climail,@Clisit,@CLSOSP,@clicel,@cliperi,@clcouff,@clpaym,@clcoddest,@clpec)";
    public string UPDATE_QUERY => "UPDATE CLIENTI SET CLIENT = @CLIENT,CLNOME = @CLNOME,CLNUTE = @CLNUTE,CLNUFA = @CLNUFA,classo = @classo,classa = @classa,solcod = @solcod,CLREDI = @CLREDI,CLRECO = @CLRECO,CLREAC = @CLREAC,CLCCOL = @CLCCOL,ammcod = @ammcod,climail = @climail,Clisit = @Clisit,CLSOSP = @CLSOSP,clicel = @clicel,cliperi = @cliperi,clcouff = @clcouff,clpaym = @clpaym,clcoddest = @clcoddest,clpec = @clpec OUTPUT INSERTED.rv WHERE CLIENT = @CLIENT AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CLIENTI OUTPUT DELETED.rv WHERE CLIENT = @CLIENT AND rv = @rv";

    public bool Insert(CLIENTI Model)
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

    public bool Update(CLIENTI Model)
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

    public bool Delete(CLIENTI Model)
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

    public string? Validate(CLIENTI Model, bool IsInsert)
    {
        try
        {
            if ((Model.CLIENT > 0 && IsInsert && !Exists(Model.CLIENT)) || !IsInsert)
            {
                return null;
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

public class CLIENTIUfpRepository : RepositoryBase, ICLIENTIRepository
{
    public CLIENTIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CLIENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<CLIENTI>(
                @$"SELECT 
CLIENT,
CLNOME,
CLNUTE,
CLNUFA,
classo,
classa,
solcod,
CLREDI,
CLRECO,
CLREAC,
CLCCOL,
ammcod,
climail,
Clisit,
CLSOSP,
clicel,
cliperi,
clcouf as clcouff,
clpaym,
clcoddest,
clpec,
zoncod,
regcod,
smecod,
climkup
FROM ANAG_CLIENTI");

            return new ObservableCollection<CLIENTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CLIENTI? Get(int CLIENT)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CLIENTI>(
                @$"SELECT 
CLIENT,
CLNOME,
CLNUTE,
CLNUFA,
classo,
classa,
solcod,
CLREDI,
CLRECO,
CLREAC,
CLCCOL,
ammcod,
climail,
Clisit,
CLSOSP,
clicel,
cliperi,
clcouf as clcouff,
clpaym,
clcoddest,
clpec,
zoncod,
regcod,
smecod,
climkup,
rv
FROM ANAG_CLIENTI WHERE CLIENT = @CLIENT",
                new { CLIENT = CLIENT })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int CLIENT)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANAG_CLIENTI WHERE CLIENT = @CLIENT",
                new { CLIENT = CLIENT }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ANAG_CLIENTI (CLIENT,CLNOME,CLNUTE,CLNUFA,classo,classa,solcod,CLREDI,CLRECO,CLREAC,CLCCOL,ammcod,climail,Clisit,CLSOSP,clicel,cliperi,clcouf,clpaym,clcoddest,clpec, zoncod,regcod, smecod,climkup) OUTPUT INSERTED.rv VALUES(@CLIENT,@CLNOME,@CLNUTE,@CLNUFA,@classo,@classa,@solcod,@CLREDI,@CLRECO,@CLREAC,@CLCCOL,@ammcod,@climail,@Clisit,@CLSOSP,@clicel,@cliperi,@clcouff,@clpaym,@clcoddest,@clpec, @zoncod,@regcod, @smecod, @climkup)";
    public string UPDATE_QUERY => "UPDATE ANAG_CLIENTI SET CLIENT = @CLIENT,CLNOME = @CLNOME,CLNUTE = @CLNUTE,CLNUFA = @CLNUFA,classo = @classo,classa = @classa,solcod = @solcod,CLREDI = @CLREDI,CLRECO = @CLRECO,CLREAC = @CLREAC,CLCCOL = @CLCCOL,ammcod = @ammcod,climail = @climail,Clisit = @Clisit,CLSOSP = @CLSOSP,clicel = @clicel,cliperi = @cliperi,clcouf = @clcouff,clpaym = @clpaym,clcoddest = @clcoddest,clpec = @clpec, zoncod = @zoncod,regcod = @regcod, smecod = @smecod, climkup = @climkup OUTPUT INSERTED.rv WHERE CLIENT = @CLIENT AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ANAG_CLIENTI OUTPUT DELETED.rv WHERE CLIENT = @CLIENT AND rv = @rv";

    public bool Insert(CLIENTI Model)
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

    public bool Update(CLIENTI Model)
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

    public bool Delete(CLIENTI Model)
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

    public string? Validate(CLIENTI Model, bool IsInsert)
    {
        try
        {
            if ((Model.CLIENT > 0 && IsInsert && !Exists(Model.CLIENT)) || !IsInsert)
            {
                return null;
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