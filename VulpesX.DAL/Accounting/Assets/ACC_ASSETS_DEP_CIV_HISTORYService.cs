namespace VulpesX.DAL.Accounting.Assets;


public interface IACC_ASSETS_DEP_CIV_HISTORYRepository
{
    ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>? GetList();

    ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>? GetList(string bcsoci, int bcann4, string bcgrup, string bccont, string bcsotc);

    ACC_ASSETS_DEP_CIV_HISTORY? Get(string bcsoci, int bcanco, int bcann4, string bcgrup, string bccont, string bcsotc, int bcinv2, int bcinv);

    bool Exists(string bcsoci, int bcanco, int bcann4, string bcgrup, string bccont, string bcsotc, int bcinv2, int bcinv);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_DEP_CIV_HISTORY Model);

    bool Update(ACC_ASSETS_DEP_CIV_HISTORY Model);

    bool Delete(ACC_ASSETS_DEP_CIV_HISTORY Model);

    string? Validate(ACC_ASSETS_DEP_CIV_HISTORY Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_DEP_CIV_HISTORYRepository : RepositoryBase, IACC_ASSETS_DEP_CIV_HISTORYRepository
{
    public ACC_ASSETS_DEP_CIV_HISTORYRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_DEP_CIV_HISTORY>(
                "SELECT * FROM ACC_ASSETS_DEP_CIV_HISTORY");

            return new ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>? GetList(string bcsoci, int bcann4, string bcgrup, string bccont, string bcsotc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_DEP_CIV_HISTORY>(
                @"SELECT bcanco, bcanne, bcpea 
                        FROM ACC_ASSETS_DEP_CIV_HISTORY 
                        WHERE bcsoci = @bcsoci AND bcann4 = @bcann4 AND bcgrup = @bcgrup AND bccont = @bccont AND bcsotc = @bcsotc
                        ORDER BY bcanco DESC",
                new { bcsoci = bcsoci, bcann4 = bcann4, bcgrup = bcgrup, bccont = bccont, bcsotc = bcsotc });

            return new ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_DEP_CIV_HISTORY? Get(string bcsoci, int bcanco, int bcann4, string bcgrup, string bccont, string bcsotc, int bcinv2, int bcinv)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_DEP_CIV_HISTORY>(
                "SELECT * FROM ACC_ASSETS_DEP_CIV_HISTORY WHERE bcsoci = @bcsoci AND bcanco = @bcanco AND bcann4 = @bcann4 AND bcgrup = @bcgrup AND bccont = @bccont AND bcsotc = @bcsotc AND bcinv2 = @bcinv2 AND bcinv = @bcinv",
                new { bcsoci = bcsoci, bcanco = bcanco, bcann4 = bcann4, bcgrup = bcgrup, bccont = bccont, bcsotc = bcsotc, bcinv2 = bcinv2, bcinv = bcinv })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string bcsoci, int bcanco, int bcann4, string bcgrup, string bccont, string bcsotc, int bcinv2, int bcinv)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_DEP_CIV_HISTORY WHERE bcsoci = @bcsoci AND bcanco = @bcanco AND bcann4 = @bcann4 AND bcgrup = @bcgrup AND bccont = @bccont AND bcsotc = @bcsotc AND bcinv2 = @bcinv2 AND bcinv = @bcinv",
                new { bcsoci = bcsoci, bcanco = bcanco, bcann4 = bcann4, bcgrup = bcgrup, bccont = bccont, bcsotc = bcsotc, bcinv2 = bcinv2, bcinv = bcinv }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_DEP_CIV_HISTORY (bcsoci,bcanco,bcann4,bcgrup,bccont,bcsotc,bcinv2,bcinv,bcval,bcpea,bcanne,bcdaip,bcdafp,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@bcsoci,@bcanco,@bcann4,@bcgrup,@bccont,@bcsotc,@bcinv2,@bcinv,@bcval,@bcpea,@bcanne,@bcdaip,@bcdafp,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_DEP_CIV_HISTORY SET bcsoci = @bcsoci,bcanco = @bcanco,bcann4 = @bcann4,bcgrup = @bcgrup,bccont = @bccont,bcsotc = @bcsotc,bcinv2 = @bcinv2,bcinv = @bcinv,bcval = @bcval,bcpea = @bcpea,bcanne = @bcanne,bcdaip = @bcdaip,bcdafp = @bcdafp,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE bcsoci = @bcsoci AND bcanco = @bcanco AND bcann4 = @bcann4 AND bcgrup = @bcgrup AND bccont = @bccont AND bcsotc = @bcsotc AND bcinv2 = @bcinv2 AND bcinv = @bcinv AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_DEP_CIV_HISTORY OUTPUT DELETED.rv WHERE bcsoci = @bcsoci AND bcanco = @bcanco AND bcann4 = @bcann4 AND bcgrup = @bcgrup AND bccont = @bccont AND bcsotc = @bcsotc AND bcinv2 = @bcinv2 AND bcinv = @bcinv AND rv = @rv";
    public bool Insert(ACC_ASSETS_DEP_CIV_HISTORY Model)
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

    public bool Update(ACC_ASSETS_DEP_CIV_HISTORY Model)
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

    public bool Delete(ACC_ASSETS_DEP_CIV_HISTORY Model)
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

    public string? Validate(ACC_ASSETS_DEP_CIV_HISTORY Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.bcsoci) && IsInsert && !Exists(Model.bcsoci, Model.bcanco, Model.bcann4, Model.bcgrup, Model.bccont, Model.bcsotc, Model.bcinv2, Model.bcinv)) || !IsInsert)
            {
                return null;
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