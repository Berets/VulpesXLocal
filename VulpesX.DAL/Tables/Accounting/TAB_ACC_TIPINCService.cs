using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting;

public interface ITAB_ACC_TIPINCRepository
{

    ObservableCollection<TAB_ACC_TIPINC>? GetList();

    ObservableCollection<GenericIDDescription>? GetGenericList();

    TAB_ACC_TIPINC? Get(string icscod);

    bool Exists(string icscod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_ACC_TIPINC Model);

    bool Update(TAB_ACC_TIPINC Model);

    bool Delete(TAB_ACC_TIPINC Model);

    string? Validate(TAB_ACC_TIPINC Model, bool IsInsert);
    #endregion
}

public class TAB_ACC_TIPINCRepository : RepositoryBase, ITAB_ACC_TIPINCRepository
{
    public TAB_ACC_TIPINCRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_ACC_TIPINC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_ACC_TIPINC>(
                "SELECT * FROM TAB_ACC_TIPINC");

            return new ObservableCollection<TAB_ACC_TIPINC>(list);

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
                "SELECT icscod AS ID, TRIM(icsdes) AS Description FROM TAB_ACC_TIPINC").ToList();
            list.Add(new GenericIDDescription() { ID = null, Description = "Tutti i tipi" });
            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_ACC_TIPINC? Get(string icscod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_ACC_TIPINC>(
                "SELECT * FROM TAB_ACC_TIPINC WHERE icscod = @icscod",
                new { icscod = icscod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string icscod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_ACC_TIPINC WHERE icscod = @icscod",
                new { icscod = icscod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_ACC_TIPINC (icscod,icsdes,icssup,icsics,icscau,icsfepacod) OUTPUT INSERTED.rv VALUES(@icscod,@icsdes,@icssup,@icsics,@icscau,@icsfepacod)";
    public string UPDATE_QUERY => "UPDATE TAB_ACC_TIPINC SET icscod = @icscod,icsdes = @icsdes,icssup = @icssup,icsics = @icsics,icscau = @icscau,icsfepacod = @icsfepacod OUTPUT INSERTED.rv WHERE icscod = @icscod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_ACC_TIPINC OUTPUT DELETED.rv WHERE icscod = @icscod AND rv = @rv";
    public bool Insert(TAB_ACC_TIPINC Model)
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

    public bool Update(TAB_ACC_TIPINC Model)
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

    public bool Delete(TAB_ACC_TIPINC Model)
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

    public string? Validate(TAB_ACC_TIPINC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.icscod) && IsInsert && !Exists(Model.icscod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.icsdes))
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

public class TAB_ACC_TIPINCUfpRepository : RepositoryBase, ITAB_ACC_TIPINCRepository
{
    public TAB_ACC_TIPINCUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_ACC_TIPINC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_ACC_TIPINC>(
                "SELECT * FROM INCASSI1");

            return new ObservableCollection<TAB_ACC_TIPINC>(list);

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
                "SELECT icscod AS ID, TRIM(icsdes) AS Description FROM INCASSI1").ToList();
            list.Add(new GenericIDDescription() { ID = null, Description = "Tutti i tipi" });
            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_ACC_TIPINC? Get(string icscod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_ACC_TIPINC>(
                "SELECT * FROM INCASSI1 WHERE icscod = @icscod",
                new { icscod = icscod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string icscod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM INCASSI1 WHERE icscod = @icscod",
                new { icscod = icscod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO INCASSI1 (icscod,icsdes,icssup,icsods,icspor,icsfepacod) OUTPUT INSERTED.rv VALUES(@icscod,@icsdes,@icssup,@icsods,@icspor,@icsfepacod)";
    public string UPDATE_QUERY => "UPDATE INCASSI1 SET icscod = @icscod,icsdes = @icsdes,icssup = @icssup,icsods=@icsods,icspor=@icspor,icsfepacod = @icsfepacod OUTPUT INSERTED.rv WHERE icscod = @icscod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM INCASSI1 OUTPUT DELETED.rv WHERE icscod = @icscod AND rv = @rv";
    public bool Insert(TAB_ACC_TIPINC Model)
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

    public bool Update(TAB_ACC_TIPINC Model)
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

    public bool Delete(TAB_ACC_TIPINC Model)
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

    public string? Validate(TAB_ACC_TIPINC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.icscod) && IsInsert && !Exists(Model.icscod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.icsdes))
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