using VulpesX.DAL;

namespace VulpesX.DAL.Accounting;

public interface IPDCCONTIRepository
{
    List<PDCCONTI>? GetBasicList();

    ObservableCollection<PDCCONTI>? GetList();

    ObservableCollection<PDCCONTI>? GetSimpleList();

    ObservableCollection<PDCCONTI>? GetList(string GroupID);

    ObservableCollection<PDCCONTI>? GetListForEntity(string GroupID, string EntityType);

    PDCCONTI? Get(string GroupID, string ID);

    bool Exists(string GroupID, string AccountID);

    #region CRUD
    bool Insert(PDCCONTI Model);

    bool Update(PDCCONTI Model);

    bool CanDelete(PDCCONTI Model);

    bool Delete(PDCCONTI Model);

    string? Validate(PDCCONTI Model, bool IsInsert);
    #endregion
}

public class PDCCONTIRepository : RepositoryBase, IPDCCONTIRepository
{
    public PDCCONTIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public List<PDCCONTI>? GetBasicList()
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCCONTI>(
                "SELECT P1GRUP,P2CONT,P2DES1,P2DES2,p2flcf FROM PDCCONTI").ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCCONTI>(
                "SELECT * FROM PDCCONTI");

            return new ObservableCollection<PDCCONTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetSimpleList()
    {
        try
        {
            using var connection = GetOpenConnection();
            var list = connection.Query<PDCCONTI>(
                "SELECT P1GRUP,P2CONT,P2DES1 FROM PDCCONTI");

            return new ObservableCollection<PDCCONTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetList(string GroupID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCCONTI>(
                @"SELECT P1GRUP,P2CONT,P2DES1,p2flcf 
                        FROM PDCCONTI WHERE P1GRUP = @p1grup",
                new { p1grup = GroupID });

            return new ObservableCollection<PDCCONTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetListForEntity(string GroupID, string EntityType)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCCONTI>(
                "SELECT * FROM PDCCONTI WHERE P1GRUP = @p1grup AND p2flcf=@et",
                new { p1grup = GroupID, et = EntityType });

            return new ObservableCollection<PDCCONTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCCONTI? Get(string GroupID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCCONTI>(
                "SELECT * FROM PDCCONTI WHERE P1GRUP = @p1grup AND P2CONT = @p2cont",
                new { p1grup = GroupID, p2cont = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string GroupID, string AccountID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PDCCONTI WHERE P1GRUP = @p1grup AND P2CONT = @p2cont",
                new { p1grup = GroupID, p2cont = AccountID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public bool Insert(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO PDCCONTI (P1GRUP,P2CONT,P2DES1,P2DES2,p2flcf,p2sett) OUTPUT INSERTED.rv VALUES(@P1GRUP,@P2CONT,@P2DES1,@P2DES2,@p2flcf,@p2sett)",
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

    public bool Update(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE PDCCONTI SET P1GRUP = @P1GRUP,P2CONT = @P2CONT,P2DES1 = @P2DES1,P2DES2 = @P2DES2,p2flcf = @p2flcf,p2sett = @p2sett OUTPUT INSERTED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND rv = @rv",
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

    public bool CanDelete(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP AND pncont = @P2CONT",
                Model);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            // TODO check not used
            var result = connection.Execute(
                "DELETE FROM PDCCONTI OUTPUT DELETED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND rv = @rv",
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

    public string? Validate(PDCCONTI Model, bool IsInsert)
    {
        try
        {
            if ((!Exists(Model.P1GRUP, Model.P2CONT) && IsInsert) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.P2DES1))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
            }
            else
            { return "Il codice inserito è già in uso"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PDCCONTIUfpRepository : RepositoryBase, IPDCCONTIRepository
{
    public PDCCONTIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public List<PDCCONTI>? GetBasicList()
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCCONTI>(
                "SELECT P1GRUP,P2CONT,P2DES1,P2DES2,p2flcf FROM PDC_CONTI").ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCCONTI>(
                "SELECT * FROM PDC_CONTI");

            return new ObservableCollection<PDCCONTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetSimpleList()
    {
        try
        {
            using var connection = GetOpenConnection();
            var list = connection.Query<PDCCONTI>(
                "SELECT P1GRUP,P2CONT,P2DES1 FROM PDC_CONTI");

            return new ObservableCollection<PDCCONTI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetList(string GroupID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCCONTI>(
                @"SELECT P1GRUP,P2CONT,P2DES1,p2flcf 
                        FROM PDC_CONTI WHERE P1GRUP = @p1grup",
                new { p1grup = GroupID });

            return new ObservableCollection<PDCCONTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCCONTI>? GetListForEntity(string GroupID, string EntityType)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCCONTI>(
                "SELECT * FROM PDC_CONTI WHERE P1GRUP = @p1grup AND p2flcf=@et",
                new { p1grup = GroupID, et = EntityType });

            return new ObservableCollection<PDCCONTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCCONTI? Get(string GroupID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCCONTI>(
                "SELECT * FROM PDC_CONTI WHERE P1GRUP = @p1grup AND P2CONT = @p2cont",
                new { p1grup = GroupID, p2cont = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string GroupID, string AccountID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PDC_CONTI WHERE P1GRUP = @p1grup AND P2CONT = @p2cont",
                new { p1grup = GroupID, p2cont = AccountID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public bool Insert(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO PDC_CONTI (P1GRUP,P2CONT,P2DES1,P2DES2,p2flcf,p2ragmas) OUTPUT INSERTED.rv VALUES(@P1GRUP,@P2CONT,@P2DES1,@P2DES2,@p2flcf,@p2ragmas)",
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

    public bool Update(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE PDC_CONTI SET P1GRUP = @P1GRUP,P2CONT = @P2CONT,P2DES1 = @P2DES1,P2DES2 = @P2DES2,p2flcf = @p2flcf,p2ragmas =@p2ragmas OUTPUT INSERTED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND rv = @rv",
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

    public bool CanDelete(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP AND pncont = @P2CONT",
                Model);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(PDCCONTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            // TODO check not used
            var result = connection.Execute(
                "DELETE FROM PDC_CONTI OUTPUT DELETED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND rv = @rv",
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

    public string? Validate(PDCCONTI Model, bool IsInsert)
    {
        try
        {
            if ((!Exists(Model.P1GRUP, Model.P2CONT) && IsInsert) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.P2DES1))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
            }
            else
            { return "Il codice inserito è già in uso"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}