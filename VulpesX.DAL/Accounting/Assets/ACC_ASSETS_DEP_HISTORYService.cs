using VulpesX.DAL;

namespace VulpesX.Services.Accounting.Assets;

public interface IACC_ASSETS_DEP_HISTORYRepository
{
    ObservableCollection<ACC_ASSETS_DEP_HISTORY>? GetList();

    ObservableCollection<ACC_ASSETS_DEP_HISTORY>? GetList(string bhsoci, int bhann4, string bhgrup, string bhcont, string bhsotc);

    ACC_ASSETS_DEP_HISTORY? Get(string bhsoci, int bhanco, int bhann4, string bhgrup, string bhcont, string bhsotc, int bhinv2, int bhinv);

    bool Exists(string bhsoci, int bhanco, int bhann4, string bhgrup, string bhcont, string bhsotc, int bhinv2, int bhinv);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_DEP_HISTORY Model);

    bool Update(ACC_ASSETS_DEP_HISTORY Model);

    bool Delete(ACC_ASSETS_DEP_HISTORY Model);

    string? Validate(ACC_ASSETS_DEP_HISTORY Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_DEP_HISTORYRepository : RepositoryBase, IACC_ASSETS_DEP_HISTORYRepository
{
    public ACC_ASSETS_DEP_HISTORYRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_DEP_HISTORY>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_ASSETS_DEP_HISTORY>(
                "SELECT * FROM ACC_ASSETS_DEP_HISTORY");

            return new ObservableCollection<ACC_ASSETS_DEP_HISTORY>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_ASSETS_DEP_HISTORY>? GetList(string bhsoci, int bhann4, string bhgrup, string bhcont, string bhsotc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_DEP_HISTORY>(
                @"SELECT bhanco, bhanne, bhpea, bhnnae 
                        FROM ACC_ASSETS_DEP_HISTORY 
                        WHERE bhsoci = @bhsoci AND bhann4 = @bhann4 AND bhgrup = @bhgrup AND bhcont = @bhcont AND bhsotc = @bhsotc
                        ORDER BY bhanco DESC",
                new { bhsoci = bhsoci, bhann4 = bhann4, bhgrup = bhgrup, bhcont = bhcont, bhsotc = bhsotc });

            return new ObservableCollection<ACC_ASSETS_DEP_HISTORY>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_DEP_HISTORY? Get(string bhsoci, int bhanco, int bhann4, string bhgrup, string bhcont, string bhsotc, int bhinv2, int bhinv)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_DEP_HISTORY>(
                "SELECT * FROM ACC_ASSETS_DEP_HISTORY WHERE bhsoci = @bhsoci AND bhanco = @bhanco AND bhann4 = @bhann4 AND bhgrup = @bhgrup AND bhcont = @bhcont AND bhsotc = @bhsotc AND bhinv2 = @bhinv2 AND bhinv = @bhinv",
                new { bhsoci = bhsoci, bhanco = bhanco, bhann4 = bhann4, bhgrup = bhgrup, bhcont = bhcont, bhsotc = bhsotc, bhinv2 = bhinv2, bhinv = bhinv })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string bhsoci, int bhanco, int bhann4, string bhgrup, string bhcont, string bhsotc, int bhinv2, int bhinv)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_DEP_HISTORY WHERE bhsoci = @bhsoci AND bhanco = @bhanco AND bhann4 = @bhann4 AND bhgrup = @bhgrup AND bhcont = @bhcont AND bhsotc = @bhsotc AND bhinv2 = @bhinv2 AND bhinv = @bhinv",
                new { bhsoci = bhsoci, bhanco = bhanco, bhann4 = bhann4, bhgrup = bhgrup, bhcont = bhcont, bhsotc = bhsotc, bhinv2 = bhinv2, bhinv = bhinv }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_DEP_HISTORY (bhsoci,bhanco,bhann4,bhgrup,bhcont,bhsotc,bhinv2,bhinv,bhval,bhpea,bhfoa,bhanqo,bhper,bhanna,bhpac,bhapp,bhpcc,bhdaip,bhdafp,bhanne,bhnnae,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@bhsoci,@bhanco,@bhann4,@bhgrup,@bhcont,@bhsotc,@bhinv2,@bhinv,@bhval,@bhpea,@bhfoa,@bhanqo,@bhper,@bhanna,@bhpac,@bhapp,@bhpcc,@bhdaip,@bhdafp,@bhanne,@bhnnae,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_DEP_HISTORY SET bhsoci = @bhsoci,bhanco = @bhanco,bhann4 = @bhann4,bhgrup = @bhgrup,bhcont = @bhcont,bhsotc = @bhsotc,bhinv2 = @bhinv2,bhinv = @bhinv,bhval = @bhval,bhpea = @bhpea,bhfoa = @bhfoa,bhanqo = @bhanqo,bhper = @bhper,bhanna = @bhanna,bhpac = @bhpac,bhapp = @bhapp,bhpcc = @bhpcc,bhdaip = @bhdaip,bhdafp = @bhdafp,bhanne = @bhanne,bhnnae = @bhnnae,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE bhsoci = @bhsoci AND bhanco = @bhanco AND bhann4 = @bhann4 AND bhgrup = @bhgrup AND bhcont = @bhcont AND bhsotc = @bhsotc AND bhinv2 = @bhinv2 AND bhinv = @bhinv AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_DEP_HISTORY OUTPUT DELETED.rv WHERE bhsoci = @bhsoci AND bhanco = @bhanco AND bhann4 = @bhann4 AND bhgrup = @bhgrup AND bhcont = @bhcont AND bhsotc = @bhsotc AND bhinv2 = @bhinv2 AND bhinv = @bhinv AND rv = @rv";
    public bool Insert(ACC_ASSETS_DEP_HISTORY Model)
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

    public bool Update(ACC_ASSETS_DEP_HISTORY Model)
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

    public bool Delete(ACC_ASSETS_DEP_HISTORY Model)
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

    public string? Validate(ACC_ASSETS_DEP_HISTORY Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.bhsoci) && IsInsert && !Exists(Model.bhsoci, Model.bhanco, Model.bhann4, Model.bhgrup, Model.bhcont, Model.bhsotc, Model.bhinv2, Model.bhinv)) || !IsInsert)
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