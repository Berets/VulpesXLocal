using VulpesX.DAL;
using VulpesX.Models.Default;

namespace VulpesX.DAL.Tables.Accounting;

public interface INAZIONIRepository
{
    ObservableCollection<NAZIONI>? GetList();

    NAZIONI? Get(string nazcod);

    bool Exists(string nazcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(NAZIONI Model);

    bool Update(NAZIONI Model);

    bool Delete(NAZIONI Model);

    string? Validate(NAZIONI Model, bool IsInsert);
    #endregion
}

public class NAZIONIRepository : RepositoryBase, INAZIONIRepository
{
    public NAZIONIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NAZIONI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<NAZIONI>(
                "SELECT * FROM NAZIONI");

            return new ObservableCollection<NAZIONI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NAZIONI? Get(string nazcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NAZIONI>(
                "SELECT * FROM NAZIONI WHERE nazcod = @nazcod",
                new { nazcod = nazcod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string nazcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM NAZIONI WHERE nazcod = @nazcod",
                new { nazcod = nazcod }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO NAZIONI (nazcod,nazdes,naztip,nazest) OUTPUT INSERTED.rv VALUES(@nazcod,@nazdes,@naztip,@nazest)";
    public string UPDATE_QUERY => "UPDATE NAZIONI SET nazcod = @nazcod,nazdes = @nazdes,naztip = @naztip,nazest = @nazest OUTPUT INSERTED.rv WHERE nazcod = @nazcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM NAZIONI OUTPUT DELETED.rv WHERE nazcod = @nazcod AND rv = @rv";
    public bool Insert(NAZIONI Model)
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

    public bool Update(NAZIONI Model)
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

    public bool Delete(NAZIONI Model)
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

    public string? Validate(NAZIONI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.nazcod) && IsInsert && !Exists(Model.nazcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.nazdes))
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