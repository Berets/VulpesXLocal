namespace VulpesX.DAL.Tables.Accounting.Assets;

public interface IACC_ASSETS_TYPESRepository
{
    ObservableCollection<ACC_ASSETS_TYPES>? GetList(string CompanyID);

    ACC_ASSETS_TYPES? Get(string socij, string JTICE);

    bool Exists(string socij, string JTICE);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_TYPES Model);

    bool Update(ACC_ASSETS_TYPES Model);

    bool Delete(ACC_ASSETS_TYPES Model);

    string? Validate(ACC_ASSETS_TYPES Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_TYPESRepository : RepositoryBase, IACC_ASSETS_TYPESRepository
{
    public ACC_ASSETS_TYPESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_TYPES>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_TYPES>(
                @"SELECT * FROM ACC_ASSETS_TYPES
                        WHERE socij = @cid",
                new { cid = CompanyID });

            return new ObservableCollection<ACC_ASSETS_TYPES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_TYPES? Get(string socij, string JTICE)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_TYPES>(
                "SELECT * FROM ACC_ASSETS_TYPES WHERE socij = @socij AND JTICE = @JTICE",
                new { socij = socij, JTICE = JTICE })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string socij, string JTICE)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_TYPES WHERE socij = @socij AND JTICE = @JTICE",
                new { socij = socij, JTICE = JTICE }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_TYPES (socij,JTICE,TCDES,TCLINE,TCCALC,tcalc,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@socij,@JTICE,@TCDES,@TCLINE,@TCCALC,@tcalc,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_TYPES SET socij = @socij,JTICE = @JTICE,TCDES = @TCDES,TCLINE = @TCLINE,TCCALC = @TCCALC,tcalc = @tcalc,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE socij = @socij AND JTICE = @JTICE AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_TYPES OUTPUT DELETED.rv WHERE socij = @socij AND JTICE = @JTICE AND rv = @rv";
    public bool Insert(ACC_ASSETS_TYPES Model)
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

    public bool Update(ACC_ASSETS_TYPES Model)
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

    public bool Delete(ACC_ASSETS_TYPES Model)
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

    public string? Validate(ACC_ASSETS_TYPES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.JTICE) && IsInsert && !Exists(Model.socij, Model.JTICE)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.TCDES))
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