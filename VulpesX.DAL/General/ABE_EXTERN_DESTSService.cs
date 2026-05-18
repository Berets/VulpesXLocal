using VulpesX.DAL;

namespace VulpesX.DAL.General;

public interface IABE_EXTERN_DESTRepository
{
    ObservableCollection<ABE_EXTERN_DESTS>? GetList(int CustomerID, string ExternCode, string ExternID);

    ObservableCollection<ABE_EXTERN_DESTS>? GeteInvoiceRecipientList(int CustomerID, int RecipientID);

    ABE_EXTERN_DESTS? Get(int abecod, string abeextcode, string abeextid, string abeextdid);

    ABE_EXTERN_DESTS? GetByExtern(string abeextcode, string abeextid, string abeextdid);

    bool Exists(int abecod, string abeextcode, string abeextid, string abeextdid);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(ABE_EXTERN_DESTS Model);

    bool Update(ABE_EXTERN_DESTS Model);

    bool Delete(ABE_EXTERN_DESTS Model);

    string? Validate(ABE_EXTERN_DESTS Model, bool IsInsert);
    #endregion
}

public class ABE_EXTERN_DESTRepository : RepositoryBase, IABE_EXTERN_DESTRepository
{
    public ABE_EXTERN_DESTRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<ABE_EXTERN_DESTS>? GetList(int CustomerID, string ExternCode, string ExternID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ABE_EXTERN_DESTS>(
                @"SELECT * FROM ABE_EXTERN_DESTS
                        WHERE abecod=@abecod AND abeextcode=@abeextcode AND abeextid=@abeextid",
                new { abecod = CustomerID, abeextcode = ExternCode, abeextid = ExternID });

            return new ObservableCollection<ABE_EXTERN_DESTS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ABE_EXTERN_DESTS>? GeteInvoiceRecipientList(int CustomerID, int RecipientID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ABE_EXTERN_DESTS>(
                @"SELECT * FROM ABE_EXTERN_DESTS
                        WHERE abecod=@abecod AND abedestid=@recid AND abeextcode LIKE 'FE.ADD#%'",
                new { abecod = CustomerID, recid = RecipientID });

            return new ObservableCollection<ABE_EXTERN_DESTS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE_EXTERN_DESTS? Get(int abecod, string abeextcode, string abeextid, string abeextdid)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ABE_EXTERN_DESTS>(
                "SELECT * FROM ABE_EXTERN_DESTS WHERE abecod = @abecod AND abeextcode = @abeextcode AND abeextid = @abeextid AND abeextdid = @abeextdid",
                new { abecod = abecod, abeextcode = abeextcode, abeextid = abeextid, abeextdid = abeextdid })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE_EXTERN_DESTS? GetByExtern(string abeextcode, string abeextid, string abeextdid)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE_EXTERN_DESTS>(
                "SELECT * FROM ABE_EXTERN_DESTS WHERE abeextcode = @abeextcode AND abeextid = @abeextid AND abeextdid = @abeextdid",
                new { abeextcode = abeextcode, abeextid = abeextid, abeextdid = abeextdid })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int abecod, string abeextcode, string abeextid, string abeextdid)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ABE_EXTERN_DESTS WHERE abecod = @abecod AND abeextcode = @abeextcode AND abeextid = @abeextid AND abeextdid = @abeextdid",
                new { abecod = abecod, abeextcode = abeextcode, abeextid = abeextid, abeextdid = abeextdid }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ABE_EXTERN_DESTS (abecod,abeextcode,abeextid,abeextdid,abedestid) OUTPUT INSERTED.rv VALUES(@abecod,@abeextcode,@abeextid,@abeextdid,@abedestid)";
    public string UPDATE_QUERY => "UPDATE ABE_EXTERN_DESTS SET abecod = @abecod,abeextcode = @abeextcode,abeextid = @abeextid,abeextdid = @abeextdid,abedestid = @abedestid OUTPUT INSERTED.rv WHERE abecod = @abecod AND abeextcode = @abeextcode AND abeextid = @abeextid AND abeextdid = @abeextdid AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ABE_EXTERN_DESTS OUTPUT DELETED.rv WHERE abecod = @abecod AND abeextcode = @abeextcode AND abeextid = @abeextid AND abeextdid = @abeextdid AND rv = @rv";

    public bool Insert(ABE_EXTERN_DESTS Model)
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

    public bool Update(ABE_EXTERN_DESTS Model)
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

    public bool Delete(ABE_EXTERN_DESTS Model)
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

    public string? Validate(ABE_EXTERN_DESTS Model, bool IsInsert)
    {
        try
        {
            if ((Model.abecod > 0 && !string.IsNullOrEmpty(Model.abeextcode) && IsInsert && !Exists(Model.abecod, Model.abeextcode, Model.abeextid, Model.abeextdid)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.abeextid))
                {
                    if (!string.IsNullOrWhiteSpace(Model.abeextdid))
                    {
                        if (Model.abedestid > 0)
                        {
                            return null;
                        }
                        else
                        { return "Il codice destinatario corrispondente è obbligatorio"; }
                    }
                    else
                    { return "Il codice destinatario esterno è obbligatorio"; }
                }
                else
                { return "Il codice cliente esterno è obbligatorio"; }
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