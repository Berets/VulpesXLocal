using VulpesX.Models.Default.Partials;

namespace VulpesX.DAL.Tables.Accounting.Assets;

public interface IACC_ASSETS_CATEGORIESRepository
{
    ObservableCollection<ACC_ASSETS_CATEGORIES>? GetList();

    ACC_ASSETS_CATEGORIES? Get(string jcateg);

    bool Exists(string jcateg);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_CATEGORIES Model);

    bool Update(ACC_ASSETS_CATEGORIES Model);

    bool Delete(ACC_ASSETS_CATEGORIES Model);

    string? Validate(ACC_ASSETS_CATEGORIES Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_CATEGORIESRepository : RepositoryBase, IACC_ASSETS_CATEGORIESRepository
{
    public ACC_ASSETS_CATEGORIESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_CATEGORIES>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_ASSETS_CATEGORIES>(
                "SELECT * FROM ACC_ASSETS_CATEGORIES WHERE canceled IS NULL");

            return new ObservableCollection<ACC_ASSETS_CATEGORIES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_CATEGORIES? Get(string jcateg)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_CATEGORIES>(
                "SELECT * FROM ACC_ASSETS_CATEGORIES WHERE jcateg = @jcateg",
                new { jcateg = jcateg })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string jcateg)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_CATEGORIES WHERE jcateg = @jcateg",
                new { jcateg = jcateg }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_CATEGORIES (jcateg,jdescr,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@jcateg,@jdescr,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_CATEGORIES SET jcateg = @jcateg,jdescr = @jdescr,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE jcateg = @jcateg AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_CATEGORIES OUTPUT DELETED.rv WHERE jcateg = @jcateg AND rv = @rv";
    public bool Insert(ACC_ASSETS_CATEGORIES Model)
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

    public bool Update(ACC_ASSETS_CATEGORIES Model)
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

    public bool Delete(ACC_ASSETS_CATEGORIES Model)
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

    public string? Validate(ACC_ASSETS_CATEGORIES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.jcateg) && IsInsert && !Exists(Model.jcateg)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.jdescr))
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