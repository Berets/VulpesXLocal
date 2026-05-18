namespace VulpesX.DAL.Tables.Accounting;

public interface IFE_RFIDOCRepository
{
    ObservableCollection<FE_RFIDOC>? GetList();

    FE_RFIDOC? Get(string regicod);

    bool Exists(string regicod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(FE_RFIDOC Model);

    bool Update(FE_RFIDOC Model);

    bool Delete(FE_RFIDOC Model);

    string? Validate(FE_RFIDOC Model, bool IsInsert);
    #endregion
}

public class FE_RFIDOCRepository : RepositoryBase, IFE_RFIDOCRepository
{
    public FE_RFIDOCRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_RFIDOC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_RFIDOC>(
                "SELECT * FROM FE_RFIDOC");

            return new ObservableCollection<FE_RFIDOC>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_RFIDOC? Get(string regicod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_RFIDOC>(
                "SELECT * FROM FE_RFIDOC WHERE regicod = @regicod",
                new { regicod = regicod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string regicod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_RFIDOC WHERE regicod = @regicod",
                new { regicod = regicod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FE_RFIDOC (regicod,regides,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@regicod,@regides,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE FE_RFIDOC SET regicod = @regicod,regides = @regides,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE regicod = @regicod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FE_RFIDOC OUTPUT DELETED.rv WHERE regicod = @regicod AND rv = @rv";
    public bool Insert(FE_RFIDOC Model)
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

    public bool Update(FE_RFIDOC Model)
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

    public bool Delete(FE_RFIDOC Model)
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

    public string? Validate(FE_RFIDOC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.regicod) && IsInsert && !Exists(Model.regicod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.regides))
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

public class FE_RFIDOCUfpRepository : RepositoryBase, IFE_RFIDOCRepository
{
    public FE_RFIDOCUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_RFIDOC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_RFIDOC>(
                "SELECT * FROM REGIMIFISCALI");

            return new ObservableCollection<FE_RFIDOC>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_RFIDOC? Get(string regicod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_RFIDOC>(
                "SELECT * FROM REGIMIFISCALI WHERE regicod = @regicod",
                new { regicod = regicod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string regicod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM REGIMIFISCALI WHERE regicod = @regicod",
                new { regicod = regicod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO REGIMIFISCALI (regicod,regides,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@regicod,@regides,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE REGIMIFISCALI SET regicod = @regicod,regides = @regides,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE regicod = @regicod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM REGIMIFISCALI OUTPUT DELETED.rv WHERE regicod = @regicod AND rv = @rv";
    public bool Insert(FE_RFIDOC Model)
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

    public bool Update(FE_RFIDOC Model)
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

    public bool Delete(FE_RFIDOC Model)
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

    public string? Validate(FE_RFIDOC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.regicod) && IsInsert && !Exists(Model.regicod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.regides))
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