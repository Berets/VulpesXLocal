using VulpesX.DAL;

namespace VulpesX.DAL.General;


public interface IFORNITORIRepository
{
    ObservableCollection<FORNITORI>? GetList();

    FORNITORI? Get(int FOCLIF);

    bool Exists(int FOCLIF);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FORNITORI Model);

    bool Update(FORNITORI Model);

    bool Delete(FORNITORI Model);

    string? Validate(FORNITORI Model, bool IsInsert);
    #endregion
}

public class FORNITORIRepository : RepositoryBase, IFORNITORIRepository
{
    public FORNITORIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FORNITORI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FORNITORI>(
                "SELECT * FROM FORNITORI");

            return new ObservableCollection<FORNITORI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FORNITORI? Get(int FOCLIF)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FORNITORI>(
                "SELECT * FROM FORNITORI WHERE FOCLIF = @FOCLIF",
                new { FOCLIF = FOCLIF })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int FOCLIF)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FORNITORI WHERE FOCLIF = @FOCLIF",
                new { FOCLIF = FOCLIF }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FORNITORI (FOCLIF,FOSPOF,FOLEFF,FOFAXF,FOGNOF,FOMEF,FONASF,FOSSOF,FONASC,FOONAF,FOCAPN,FOINDR,FOLOCR,FOPROR,FOCAPR,FORAGS,FOINDD,FOLOCD,FOPROD,FOCAPD,Formail,ForSit,FOSOSP,focel,foperi,forpec,focoddest) OUTPUT INSERTED.rv VALUES(@FOCLIF,@FOSPOF,@FOLEFF,@FOFAXF,@FOGNOF,@FOMEF,@FONASF,@FOSSOF,@FONASC,@FOONAF,@FOCAPN,@FOINDR,@FOLOCR,@FOPROR,@FOCAPR,@FORAGS,@FOINDD,@FOLOCD,@FOPROD,@FOCAPD,@Formail,@ForSit,@FOSOSP,@focel,@foperi,@forpec,@focoddest)";
    public string UPDATE_QUERY => "UPDATE FORNITORI SET FOCLIF = @FOCLIF,FOSPOF = @FOSPOF,FOLEFF = @FOLEFF,FOFAXF = @FOFAXF,FOGNOF = @FOGNOF,FOMEF = @FOMEF,FONASF = @FONASF,FOSSOF = @FOSSOF,FONASC = @FONASC,FOONAF = @FOONAF,FOCAPN = @FOCAPN,FOINDR = @FOINDR,FOLOCR = @FOLOCR,FOPROR = @FOPROR,FOCAPR = @FOCAPR,FORAGS = @FORAGS,FOINDD = @FOINDD,FOLOCD = @FOLOCD,FOPROD = @FOPROD,FOCAPD = @FOCAPD,Formail = @Formail,ForSit = @ForSit,FOSOSP = @FOSOSP,focel = @focel,foperi = @foperi,forpec = @forpec,focoddest = @focoddest OUTPUT INSERTED.rv WHERE FOCLIF = @FOCLIF AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FORNITORI OUTPUT DELETED.rv WHERE FOCLIF = @FOCLIF AND rv = @rv";
    public bool Insert(FORNITORI Model)
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

    public bool Update(FORNITORI Model)
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

    public bool Delete(FORNITORI Model)
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

    public string? Validate(FORNITORI Model, bool IsInsert)
    {
        try
        {
            if ((Model.FOCLIF > 0 && IsInsert && !Exists(Model.FOCLIF)) || !IsInsert)
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

public class FORNITORIUfpRepository : RepositoryBase, IFORNITORIRepository
{
    public FORNITORIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FORNITORI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<FORNITORI>(
                "SELECT * FROM ANAG_FORNITORI");

            return new ObservableCollection<FORNITORI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FORNITORI? Get(int FOCLIF)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<FORNITORI>(
                "SELECT * FROM ANAG_FORNITORI WHERE FOCLIF = @FOCLIF",
                new { FOCLIF = FOCLIF })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int FOCLIF)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANAG_FORNITORI WHERE FOCLIF = @FOCLIF",
                new { FOCLIF = FOCLIF }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ANAG_FORNITORI (FOCLIF,FOSPOF,FOLEFF,FOFAXF,FOGNOF,FOMEF,FONASF,FOSSOF,FONASC,FOONAF,FOCAPN,FOINDR,FOLOCR,FOPROR,FOCAPR,FORAGS,FOINDD,FOLOCD,FOPROD,FOCAPD,Formail,ForSit,FOSOSP,focel,foperi,forpec,focoddest) OUTPUT INSERTED.rv VALUES(@FOCLIF,@FOSPOF,@FOLEFF,@FOFAXF,@FOGNOF,@FOMEF,@FONASF,@FOSSOF,@FONASC,@FOONAF,@FOCAPN,@FOINDR,@FOLOCR,@FOPROR,@FOCAPR,@FORAGS,@FOINDD,@FOLOCD,@FOPROD,@FOCAPD,@Formail,@ForSit,@FOSOSP,@focel,@foperi,@forpec,@focoddest)";
    public string UPDATE_QUERY => "UPDATE ANAG_FORNITORI SET FOCLIF = @FOCLIF,FOSPOF = @FOSPOF,FOLEFF = @FOLEFF,FOFAXF = @FOFAXF,FOGNOF = @FOGNOF,FOMEF = @FOMEF,FONASF = @FONASF,FOSSOF = @FOSSOF,FONASC = @FONASC,FOONAF = @FOONAF,FOCAPN = @FOCAPN,FOINDR = @FOINDR,FOLOCR = @FOLOCR,FOPROR = @FOPROR,FOCAPR = @FOCAPR,FORAGS = @FORAGS,FOINDD = @FOINDD,FOLOCD = @FOLOCD,FOPROD = @FOPROD,FOCAPD = @FOCAPD,Formail = @Formail,ForSit = @ForSit,FOSOSP = @FOSOSP,focel = @focel,foperi = @foperi,forpec = @forpec,focoddest = @focoddest OUTPUT INSERTED.rv WHERE FOCLIF = @FOCLIF AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ANAG_FORNITORI OUTPUT DELETED.rv WHERE FOCLIF = @FOCLIF AND rv = @rv";
    public bool Insert(FORNITORI Model)
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

    public bool Update(FORNITORI Model)
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

    public bool Delete(FORNITORI Model)
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

    public string? Validate(FORNITORI Model, bool IsInsert)
    {
        try
        {
            if ((Model.FOCLIF > 0 && IsInsert && !Exists(Model.FOCLIF)) || !IsInsert)
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