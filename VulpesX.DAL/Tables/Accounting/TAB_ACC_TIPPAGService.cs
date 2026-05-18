using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting;

public interface ITAB_ACC_TIPPAGRepository
{

    ObservableCollection<TAB_ACC_TIPPAG>? GetList();

    ObservableCollection<GenericIDDescription>? GetGenericList();

    TAB_ACC_TIPPAG? Get(string inccod);

    bool Exists(string inccod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_ACC_TIPPAG Model);

    bool Update(TAB_ACC_TIPPAG Model);

    bool Delete(TAB_ACC_TIPPAG Model);

    string? Validate(TAB_ACC_TIPPAG Model, bool IsInsert);
    #endregion
}

public class TAB_ACC_TIPPAGRepository : RepositoryBase, ITAB_ACC_TIPPAGRepository
{
    public TAB_ACC_TIPPAGRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_ACC_TIPPAG>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_ACC_TIPPAG>(
                "SELECT * FROM TAB_ACC_TIPPAG");

            return new ObservableCollection<TAB_ACC_TIPPAG>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetGenericList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIDDescription>(
                "SELECT inccod AS ID, TRIM(incdes) AS Description FROM TAB_ACC_TIPPAG").ToList();
            list.Add(new GenericIDDescription() { ID = null, Description = "Tutti i tipi" });
            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_ACC_TIPPAG? Get(string inccod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_ACC_TIPPAG>(
                "SELECT * FROM TAB_ACC_TIPPAG WHERE inccod = @inccod",
                new { inccod = inccod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string inccod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_ACC_TIPPAG WHERE inccod = @inccod",
                new { inccod = inccod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_ACC_TIPPAG (inccod,incdes,incods,incsup,incpor) OUTPUT INSERTED.rv VALUES(@inccod,@incdes,@incods,@incsup,@incpor)";
    public string UPDATE_QUERY => "UPDATE TAB_ACC_TIPPAG SET inccod = @inccod,incdes = @incdes,incods = @incods,incsup = @incsup,incpor = @incpor OUTPUT INSERTED.rv WHERE inccod = @inccod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_ACC_TIPPAG OUTPUT DELETED.rv WHERE inccod = @inccod AND rv = @rv";
    public bool Insert(TAB_ACC_TIPPAG Model)
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

    public bool Update(TAB_ACC_TIPPAG Model)
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

    public bool Delete(TAB_ACC_TIPPAG Model)
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

    public string? Validate(TAB_ACC_TIPPAG Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.inccod) && IsInsert && !Exists(Model.inccod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.incdes))
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

public class TAB_ACC_TIPPAGUfpRepository : RepositoryBase, ITAB_ACC_TIPPAGRepository
{
    public TAB_ACC_TIPPAGUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_ACC_TIPPAG>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_ACC_TIPPAG>(
                "SELECT * FROM PAGAMENTI");

            return new ObservableCollection<TAB_ACC_TIPPAG>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetGenericList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIDDescription>(
                "SELECT inccod AS ID, TRIM(incdes) AS Description FROM PAGAMENTI").ToList();
            list.Add(new GenericIDDescription() { ID = null, Description = "Tutti i tipi" });
            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_ACC_TIPPAG? Get(string inccod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_ACC_TIPPAG>(
                "SELECT * FROM PAGAMENTI WHERE inccod = @inccod",
                new { inccod = inccod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string inccod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PAGAMENTI WHERE inccod = @inccod",
                new { inccod = inccod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PAGAMENTI (inccod,incdes,incods,incsup,incpor) OUTPUT INSERTED.rv VALUES(@inccod,@incdes,@incods,@incsup,@incpor)";
    public string UPDATE_QUERY => "UPDATE PAGAMENTI SET inccod = @inccod,incdes = @incdes,incods = @incods,incsup = @incsup,incpor = @incpor OUTPUT INSERTED.rv WHERE inccod = @inccod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PAGAMENTI OUTPUT DELETED.rv WHERE inccod = @inccod AND rv = @rv";
    public bool Insert(TAB_ACC_TIPPAG Model)
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

    public bool Update(TAB_ACC_TIPPAG Model)
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

    public bool Delete(TAB_ACC_TIPPAG Model)
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

    public string? Validate(TAB_ACC_TIPPAG Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.inccod) && IsInsert && !Exists(Model.inccod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.incdes))
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