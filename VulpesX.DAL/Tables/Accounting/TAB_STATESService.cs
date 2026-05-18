using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface ITAB_STATESRepository
{
    ObservableCollection<TAB_STATES>? GetList();

    TAB_STATES? Get(string cappro);

    bool Exists(string cappro);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_STATES Model);

    bool Update(TAB_STATES Model);

    bool Delete(TAB_STATES Model);

    string? Validate(TAB_STATES Model, bool IsInsert);
    #endregion
}

public class TAB_STATESRepository : RepositoryBase, ITAB_STATESRepository
{
    public TAB_STATESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_STATES>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_STATES>(
                "SELECT * FROM TAB_STATES");

            return new ObservableCollection<TAB_STATES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_STATES? Get(string cappro)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_STATES>(
                "SELECT * FROM TAB_STATES WHERE cappro = @cappro",
                new { cappro = cappro })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cappro)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_STATES WHERE cappro = @cappro",
                new { cappro = cappro }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_STATES (cappro,capdpr) OUTPUT INSERTED.rv VALUES(@cappro,@capdpr)";
    public string UPDATE_QUERY => "UPDATE TAB_STATES SET cappro = @cappro,capdpr = @capdpr OUTPUT INSERTED.rv WHERE cappro = @cappro AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_STATES OUTPUT DELETED.rv WHERE cappro = @cappro AND rv = @rv";
    public bool Insert(TAB_STATES Model)
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

    public bool Update(TAB_STATES Model)
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

    public bool Delete(TAB_STATES Model)
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

    public string? Validate(TAB_STATES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cappro) && IsInsert && !Exists(Model.cappro)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.capdpr))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
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

public class TAB_STATESUfpRepository : RepositoryBase, ITAB_STATESRepository
{
    public TAB_STATESUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_STATES>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_STATES>(
                "SELECT * FROM CAP");

            return new ObservableCollection<TAB_STATES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_STATES? Get(string cappro)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_STATES>(
                "SELECT * FROM CAP WHERE cappro = @cappro",
                new { cappro = cappro })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cappro)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CAP WHERE cappro = @cappro",
                new { cappro = cappro }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAP (cappro,capdpr) OUTPUT INSERTED.rv VALUES(@cappro,@capdpr)";
    public string UPDATE_QUERY => "UPDATE CAP SET cappro = @cappro,capdpr = @capdpr OUTPUT INSERTED.rv WHERE cappro = @cappro AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAP OUTPUT DELETED.rv WHERE cappro = @cappro AND rv = @rv";
    public bool Insert(TAB_STATES Model)
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

    public bool Update(TAB_STATES Model)
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

    public bool Delete(TAB_STATES Model)
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

    public string? Validate(TAB_STATES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cappro) && IsInsert && !Exists(Model.cappro)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.capdpr))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
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