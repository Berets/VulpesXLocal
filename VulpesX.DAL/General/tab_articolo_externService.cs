using VulpesX.DAL;

namespace VulpesX.DAL.General;

public interface Itab_articolo_externRepository
{
    ObservableCollection<tab_articolo_extern>? GetList(string CompanyID, string ProductID);

    tab_articolo_extern? Get(string extsoc, string extpid, string extcode);

    tab_articolo_extern? GetByExtern(string extsoc, string extcode, string extid);

    bool Exists(string extsoc, string extpid, string extcode);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(tab_articolo_extern Model);

    bool Update(tab_articolo_extern Model);

    bool Delete(tab_articolo_extern Model);

    string? Validate(tab_articolo_extern Model, bool IsInsert);
    #endregion
}

public class tab_articolo_externRepository : RepositoryBase, Itab_articolo_externRepository
{
    public tab_articolo_externRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_articolo_extern>? GetList(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo_extern>(
                @"SELECT * FROM tab_articolo_extern
                        WHERE extsoc=@cid AND extpid=@pid",
                new { cid = CompanyID, pid = ProductID });

            return new ObservableCollection<tab_articolo_extern>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_extern? Get(string extsoc, string extpid, string extcode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo_extern>(
                "SELECT * FROM tab_articolo_extern WHERE extsoc = @extsoc AND extpid = @extpid AND extcode = @extcode",
                new { extsoc = extsoc, extpid = extpid, extcode = extcode })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_extern? GetByExtern(string extsoc, string extcode, string extid)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_articolo_extern>(
                "SELECT * FROM tab_articolo_extern WHERE extsoc = @extsoc AND extcode = @extcode AND extid = @extid",
                new { extsoc = extsoc, extcode = extcode, extid = extid })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string extsoc, string extpid, string extcode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo_extern WHERE extsoc = @extsoc AND extpid = @extpid AND extcode = @extcode",
                new { extsoc = extsoc, extpid = extpid, extcode = extcode }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO tab_articolo_extern (extsoc,extpid,extcode,extid) OUTPUT INSERTED.rv VALUES(@extsoc,@extpid,@extcode,@extid)";
    public string UPDATE_QUERY => "UPDATE tab_articolo_extern SET extsoc = @extsoc,extpid = @extpid,extcode = @extcode,extid = @extid OUTPUT INSERTED.rv WHERE extsoc = @extsoc AND extpid = @extpid AND extcode = @extcode AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM tab_articolo_extern OUTPUT DELETED.rv WHERE extsoc = @extsoc AND extpid = @extpid AND extcode = @extcode AND rv = @rv";
    public bool Insert(tab_articolo_extern Model)
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

    public bool Update(tab_articolo_extern Model)
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

    public bool Delete(tab_articolo_extern Model)
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

    public string? Validate(tab_articolo_extern Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.extsoc) && !string.IsNullOrWhiteSpace(Model.extpid) && IsInsert && !Exists(Model.extsoc, Model.extpid, Model.extcode)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.extcode))
                {
                    if (!string.IsNullOrWhiteSpace(Model.extid))
                    {
                        return null;
                    }
                    else
                    { return "Il codice articolo esterno è obbligatorio"; }
                }
                else
                { return "Il codice esterno è obbligatorio"; }
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