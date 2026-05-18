using VulpesX.DAL;

namespace VulpesX.Services.Tables.Accounting;

public interface IFE_TIPORITRepository
{

    ObservableCollection<FE_TIPORIT>? GetList();

    FE_TIPORIT? Get(string id);

    bool Exists(string id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FE_TIPORIT Model);

    bool Update(FE_TIPORIT Model);

    bool Delete(FE_TIPORIT Model);

    string? Validate(FE_TIPORIT Model, bool IsInsert);
    #endregion
}

public class FE_TIPORITRepository : RepositoryBase, IFE_TIPORITRepository
{
    public FE_TIPORITRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_TIPORIT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_TIPORIT>(
                "SELECT * FROM FE_TIPORIT");

            return new ObservableCollection<FE_TIPORIT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_TIPORIT? Get(string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_TIPORIT>(
                "SELECT * FROM FE_TIPORIT WHERE id = @id",
                new { id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_TIPORIT WHERE id = @id",
                new { id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FE_TIPORIT (id,description,FETREna,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@id,@description,@FETREna,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE FE_TIPORIT SET id = @id,description = @description,FETREna=@FETREna,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FE_TIPORIT OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public bool Insert(FE_TIPORIT Model)
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

    public bool Update(FE_TIPORIT Model)
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

    public bool Delete(FE_TIPORIT Model)
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

    public string? Validate(FE_TIPORIT Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.id) && IsInsert && !Exists(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
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

public class FE_TIPORITUfpRepository : RepositoryBase, IFE_TIPORITRepository
{
    public FE_TIPORITUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_TIPORIT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_TIPORIT>(
                @$"SELECT 
FETRCod as id,
FETRDes as description,
*
FROM FE_RITDOC");

            return new ObservableCollection<FE_TIPORIT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_TIPORIT? Get(string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_TIPORIT>(
                $@"SELECT 
FETRCod as id,
FETRDes as description,
*
FROM FE_RITDOC WHERE FETRCod = @id",
                new { id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_RITDOC WHERE FETRCod = @id",
                new { id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FE_RITDOC (FETRCod,FETRDes, FETREna) VALUES(@id,@description, @FETREna)";
    public string UPDATE_QUERY => "UPDATE FE_RITDOC SET FETRDes = @description,FETREna=@FETREna WHERE FETRCod = @id";
    public string DELETE_QUERY => "DELETE FROM FE_RITDOC WHERE FETRCod = @id";
    public bool Insert(FE_TIPORIT Model)
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

    public bool Update(FE_TIPORIT Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
      
                return true;
         

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(FE_TIPORIT Model)
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

    public string? Validate(FE_TIPORIT Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.id) && IsInsert && !Exists(Model.id)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.description))
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