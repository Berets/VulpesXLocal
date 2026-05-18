using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface IFE_IVADOCRepository
{

    ObservableCollection<FE_IVADOC>? GetList();

    FE_IVADOC? Get(string ID);

    bool Exists(string FETICod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FE_IVADOC Model);

    bool Update(FE_IVADOC Model);

    bool Delete(FE_IVADOC Model);

    string? Validate(FE_IVADOC Model, bool IsInsert);
    #endregion
}

public class FE_IVADOCRepository : RepositoryBase, IFE_IVADOCRepository
{
    public FE_IVADOCRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_IVADOC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_IVADOC>(
                @"SELECT * FROM FE_IVADOC
                        WHERE canceled IS NULL");

            return new ObservableCollection<FE_IVADOC>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_IVADOC? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_IVADOC>(
                "SELECT * FROM FE_IVADOC WHERE FETICod = @feticod",
                new { feticod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string FETICod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_IVADOC WHERE FETICod = @FETICod",
                new { FETICod = FETICod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FE_IVADOC (FETICod,FETIDes,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@FETICod,@FETIDes,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE FE_IVADOC SET FETICod = @FETICod,FETIDes = @FETIDes,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE FETICod = @FETICod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FE_IVADOC OUTPUT DELETED.rv WHERE FETICod = @FETICod AND rv = @rv";
    public bool Insert(FE_IVADOC Model)
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

    public bool Update(FE_IVADOC Model)
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

    public bool Delete(FE_IVADOC Model)
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

    public string? Validate(FE_IVADOC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.FETICod) && IsInsert && !Exists(Model.FETICod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.FETIDes))
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

public class FE_IVADOCUfpRepository : RepositoryBase, IFE_IVADOCRepository
{
    public FE_IVADOCUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_IVADOC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_IVADOC>(
                @"SELECT * FROM FE_IVADOC");

            return new ObservableCollection<FE_IVADOC>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_IVADOC? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_IVADOC>(
                "SELECT * FROM FE_IVADOC WHERE FETICod = @feticod",
                new { feticod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string FETICod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_IVADOC WHERE FETICod = @FETICod",
                new { FETICod = FETICod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FE_IVADOC (FETICod,FETIDes) OUTPUT INSERTED.rv VALUES(@FETICod,@FETIDes)";
    public string UPDATE_QUERY => "UPDATE FE_IVADOC SET FETICod = @FETICod,FETIDes = @FETIDes OUTPUT INSERTED.rv WHERE FETICod = @FETICod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FE_IVADOC OUTPUT DELETED.rv WHERE FETICod = @FETICod AND rv = @rv";
    public bool Insert(FE_IVADOC Model)
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

    public bool Update(FE_IVADOC Model)
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

    public bool Delete(FE_IVADOC Model)
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

    public string? Validate(FE_IVADOC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.FETICod) && IsInsert && !Exists(Model.FETICod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.FETIDes))
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