using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;


public interface IFILIALIRepository
{


    ObservableCollection<FILIALI>? GetList(string filsoc);

    FILIALI? Get(string? filsoc, int filcod);

    bool Exists(string? filsoc, int filcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FILIALI Model);

    bool Update(FILIALI Model);

    bool Delete(FILIALI Model);

    string? Validate(FILIALI Model, bool IsInsert);
    #endregion
}

public class FILIALIRepository : RepositoryBase, IFILIALIRepository
{
    public FILIALIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FILIALI>? GetList(string filsoc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FILIALI>(
                @"SELECT * FROM FILIALI
                        WHERE filsoc = @filsoc", new { filsoc = @filsoc });

            return new ObservableCollection<FILIALI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FILIALI? Get(string? filsoc, int filcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FILIALI>(
                "SELECT * FROM FILIALI WHERE filsoc=@filsoc AND filcod = @filcod",
                new { filsoc = filsoc, filcod = filcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string? filsoc, int filcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FILIALI WHERE filsoc=@filsoc AND filcod = @filcod",
                new { filsoc = filsoc, filcod = filcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FILIALI (filcod,fildes,filsoc) OUTPUT INSERTED.rv VALUES(@filcod,@fildes,@filsoc)";
    public string UPDATE_QUERY => "UPDATE FILIALI SET filcod = @filcod,fildes = @fildes,filsoc = @filsoc OUTPUT INSERTED.rv WHERE filsoc = @filsoc AND filcod = @filcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FILIALI OUTPUT DELETED.rv WHERE filsoc = @filsoc AND filcod = @filcod AND rv = @rv";
    public bool Insert(FILIALI Model)
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

    public bool Update(FILIALI Model)
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

    public bool Delete(FILIALI Model)
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

    public string? Validate(FILIALI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.filsoc) && Model.filcod > 0 && IsInsert && !Exists(Model.filsoc, Model.filcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.fildes))
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

public class FILIALIUfpRepository : RepositoryBase, IFILIALIRepository
{
    public FILIALIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FILIALI>? GetList(string filsoc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FILIALI>(
                @"SELECT * FROM FILIALI");

            return new ObservableCollection<FILIALI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FILIALI? Get(string? filsoc, int filcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FILIALI>(
                "SELECT * FROM FILIALI WHERE  filcod = @filcod",
                new { filcod = filcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string? filsoc, int filcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FILIALI WHERE filcod = @filcod",
                new { filcod = filcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FILIALI (filcod,fildes,filbase) OUTPUT INSERTED.rv VALUES(@filcod,@fildes,@filbase)";
    public string UPDATE_QUERY => "UPDATE FILIALI SET filcod = @filcod,fildes = @fildes,filbase = @filbase OUTPUT INSERTED.rv WHERE filcod = @filcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FILIALI OUTPUT DELETED.rv WHERE  filcod = @filcod AND rv = @rv";
    public bool Insert(FILIALI Model)
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

    public bool Update(FILIALI Model)
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

    public bool Delete(FILIALI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(DELETE_QUERY, Model);
            if (result >0)
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

    public string? Validate(FILIALI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.filsoc) && Model.filcod > 0 && IsInsert && !Exists(Model.filsoc, Model.filcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.fildes))
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