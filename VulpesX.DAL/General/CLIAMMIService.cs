using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.General;

public interface ICLIAMMIRepository
{
    ObservableCollection<CLIAMMI>? GetList(string CompanyID);

    CLIAMMI? Get(string Cliasoc, int Cliacod);

    bool Exists(string Cliasoc, int Cliacod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(CLIAMMI Model);
    bool Update(CLIAMMI Model);
    bool Delete(CLIAMMI Model);
    string? Validate(CLIAMMI Model, bool IsInsert);
    #endregion
}

public class CLIAMMIRepository : RepositoryBase, ICLIAMMIRepository
{
    public CLIAMMIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CLIAMMI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CLIAMMI>(
                "SELECT * FROM CLIAMMI WHERE Cliasoc = @cliasoc",
                new { cliasoc = CompanyID });

            return new ObservableCollection<CLIAMMI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CLIAMMI? Get(string Cliasoc, int Cliacod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CLIAMMI>(
                "SELECT * FROM CLIAMMI WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string Cliasoc, int Cliacod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CLIAMMI WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CLIAMMI (Cliasoc,Cliacod,CLSPEB,CLASBO,specod,concod,CLSCON,CLRAGR,affcod,CLABI,CLCAB,CLNUCC,CLFIDO,rivcod,clGRUP,clcont,scacod,catcod,vetcod,filcod,csfcod,arecod,CLCCON,clprca,banabi,bancab,bancoc,pclcod,clbban,cliban,clcin,CLAGEN,CLREGI,CLZONE,CLSETM,CLDEPO,CLIMBA,CLAGEN2,CLAGENP,CLAGEN2P,CLAGENPT,CLAGEN2PT,CLRAGF,CLRAGFD) OUTPUT INSERTED.rv VALUES(@Cliasoc,@Cliacod,@CLSPEB,@CLASBO,@specod,@concod,@CLSCON,@CLRAGR,@affcod,@CLABI,@CLCAB,@CLNUCC,@CLFIDO,@rivcod,@clGRUP,@clcont,@scacod,@catcod,@vetcod,@filcod,@csfcod,@arecod,@CLCCON,@clprca,@banabi,@bancab,@bancoc,@pclcod,@clbban,@cliban,@clcin,@CLAGEN,@CLREGI,@CLZONE,@CLSETM,@CLDEPO,@CLIMBA,@CLAGEN2,@CLAGENP,@CLAGEN2P,@CLAGENPT,@CLAGEN2PT,@CLRAGF,@CLRAGFD)";
    public string UPDATE_QUERY => "UPDATE CLIAMMI SET Cliasoc = @Cliasoc,Cliacod = @Cliacod,CLSPEB = @CLSPEB,CLASBO = @CLASBO,specod = @specod,concod = @concod,CLSCON = @CLSCON,CLRAGR = @CLRAGR,affcod = @affcod,CLABI = @CLABI,CLCAB = @CLCAB,CLNUCC = @CLNUCC,CLFIDO = @CLFIDO,rivcod = @rivcod,clGRUP = @clGRUP,clcont = @clcont,scacod = @scacod,catcod = @catcod,vetcod = @vetcod,filcod = @filcod,csfcod = @csfcod,arecod = @arecod,CLCCON = @CLCCON,clprca = @clprca,banabi = @banabi,bancab = @bancab,bancoc = @bancoc,pclcod = @pclcod,clbban = @clbban,cliban = @cliban,clcin = @clcin,CLAGEN = @CLAGEN,CLREGI = @CLREGI,CLZONE = @CLZONE,CLSETM = @CLSETM,CLDEPO = @CLDEPO,CLIMBA = @CLIMBA,CLAGEN2 = @CLAGEN2,CLAGENP = @CLAGENP,CLAGEN2P = @CLAGEN2P,CLAGENPT = @CLAGENPT,CLAGEN2PT = @CLAGEN2PT,CLRAGF = @CLRAGF,CLRAGFD = @CLRAGFD OUTPUT INSERTED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CLIAMMI OUTPUT DELETED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND rv = @rv";
    public bool Insert(CLIAMMI Model)
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
    public bool Update(CLIAMMI Model)
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
    public bool Delete(CLIAMMI Model)
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
    public string? Validate(CLIAMMI Model, bool IsInsert)
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

public class CLIAMMIUfpRepository : RepositoryBase, ICLIAMMIRepository
{
    public CLIAMMIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<CLIAMMI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CLIAMMI>(
                @$"SELECT 
Cliasoc,
Cliacod,
CLSPEB,
CLASBO,
specod,
concod,
CLSCON,
CLRAGR,
affcod,
CLABI,
CLCAB,
CLNUCC,
CLFIDO,
rivcod,
clGRUP,
clcont,
scacod,
catcod,
vetcod,
filcod,
csfcod,
arecod,
CLCCON,
clprca,
banabi,
bancab,
bancoc,
pclcod,
clbban,
cliban,
clcin,
clitop,
cliimballo,
cletiqr,
clibloccodisegno,
clisister,
climailfatture,
cliaasso,
cliaaliq,
cliinv,
clipackingordine,
contqobb,
clobbcer,
cliinvbo,
clblospe,
clipezzicl,
clivalore,
agecod as CLAGEN,
cliapro as CLAGENP,
cliagecod2 as CLAGEN2,
cliagepro2 as CLAGEN2P,
clifattcau,
clicidi,
cliiva
                FROM CLIAMMI WHERE Cliasoc = @cliasoc",
                new { cliasoc = CompanyID });

            return new ObservableCollection<CLIAMMI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CLIAMMI? Get(string Cliasoc, int Cliacod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CLIAMMI>(
                @$"SELECT 
Cliasoc,
Cliacod,
CLSPEB,
CLASBO,
specod,
concod,
CLSCON,
CLRAGR,
affcod,
CLABI,
CLCAB,
CLNUCC,
CLFIDO,
rivcod,
clGRUP,
clcont,
scacod,
catcod,
vetcod,
filcod,
csfcod,
arecod,
CLCCON,
clprca,
banabi,
bancab,
bancoc,
pclcod,
clbban,
cliban,
clcin,
clitop,
cliimballo,
cletiqr,
clibloccodisegno,
clisister,
climailfatture,
cliaasso,
cliaaliq,
cliinv,
clipackingordine,
contqobb,
clobbcer,
cliinvbo,
clblospe,
clipezzicl,
clivalore,
agecod as CLAGEN,
cliapro as CLAGENP,
cliagecod2 as CLAGEN2,
cliagepro2 as CLAGEN2P,
clifattcau,
clicidi,
cliiva,
'P' as CLAGENPT,
'P' as CLAGEN2PT,
rv
FROM CLIAMMI WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string Cliasoc, int Cliacod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CLIAMMI WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CLIAMMI (Cliasoc,Cliacod,CLSPEB,CLASBO,specod,concod,CLSCON,CLRAGR,affcod,CLABI,CLCAB,CLNUCC,CLFIDO,rivcod,clGRUP,clcont,scacod,catcod,vetcod,filcod,csfcod,arecod,CLCCON,clprca,banabi,bancab,bancoc,pclcod,clbban,cliban,clcin,agecod,cliapro,cliagecod2,cliagepro2,clicidi,cliiva,clicamb,clitop,cliimballo,cletiqr,clibloccodisegno,clisister,climailfatture,cliaasso, cliaaliq, cliinv, clipackingordine,contqobb,clobbcer,cliinvbo,clblospe,clipezzicl,clivalore,clifattcau) OUTPUT INSERTED.rv VALUES(@Cliasoc,@Cliacod,@CLSPEB,@CLASBO,@specod,@concod,@CLSCON,@CLRAGR,@affcod,@CLABI,@CLCAB,@CLNUCC,@CLFIDO,@rivcod,@clGRUP,@clcont,@scacod,@catcod,@vetcod,@filcod,@csfcod,@arecod,@CLCCON,@clprca,@banabi,@bancab,@bancoc,@pclcod,@clbban,@cliban,@clcin,@CLAGEN,@CLAGENP,@CLAGEN2,@CLAGEN2P,@clicidi,@cliiva,0,@clitop,@cliimballo,@cletiqr,@clibloccodisegno,@clisister,@climailfatture,@cliaasso, @cliaaliq, @cliinv, @clipackingordine,@contqobb,@clobbcer,@cliinvbo,@clblospe,@clipezzicl,@clivalore,@clifattcau)";
    public string UPDATE_QUERY => "UPDATE CLIAMMI SET Cliasoc = @Cliasoc,Cliacod = @Cliacod,CLSPEB = @CLSPEB,CLASBO = @CLASBO,specod = @specod,concod = @concod,CLSCON = @CLSCON,CLRAGR = @CLRAGR,affcod = @affcod,CLABI = @CLABI,CLCAB = @CLCAB,CLNUCC = @CLNUCC,CLFIDO = @CLFIDO,rivcod = @rivcod,clGRUP = @clGRUP,clcont = @clcont,scacod = @scacod,catcod = @catcod,vetcod = @vetcod,filcod = @filcod,csfcod = @csfcod,arecod = @arecod,CLCCON = @CLCCON,clprca = @clprca,banabi = @banabi,bancab = @bancab,bancoc = @bancoc,pclcod = @pclcod,clbban = @clbban,cliban = @cliban,clcin = @clcin,agecod = @CLAGEN,cliapro = @CLAGENP,cliagecod2 = @CLAGEN2,cliagepro2=@CLAGEN2P,clicidi = @clicidi,cliiva = @cliiva,clicamb = 0,clitop=@clitop,cliimballo=@cliimballo,cletiqr=@cletiqr,clibloccodisegno=@clibloccodisegno,clisister=@clisister,climailfatture=@climailfatture,cliaasso =@cliaasso, cliaaliq = @cliaaliq, cliinv = @cliinv, clipackingordine = @clipackingordine, contqobb = @contqobb,clobbcer = @clobbcer,cliinvbo = @cliinvbo,clblospe =@clblospe,clipezzicl=@clipezzicl,clivalore=@clivalore,clifattcau=@clifattcau OUTPUT INSERTED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CLIAMMI OUTPUT DELETED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND rv = @rv";
    public bool Insert(CLIAMMI Model)
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

    public bool Update(CLIAMMI Model)
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

    public bool Delete(CLIAMMI Model)
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

    public string? Validate(CLIAMMI Model, bool IsInsert)
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