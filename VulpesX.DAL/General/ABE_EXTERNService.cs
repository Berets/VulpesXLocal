namespace VulpesX.DAL.General;

public interface IABE_EXTERNRepository
{
    ObservableCollection<ABE_EXTERN>? GetList(int CustomerID);

    ABE_EXTERN? Get(int abecod, string abeextcode);

    ABE_EXTERN? GetByExtern(string abeextcode, string abeextid);

    bool Exists(int abecod, string abeextcode);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ABE_EXTERN Model);

    bool Update(ABE_EXTERN Model);

    bool Delete(ABE_EXTERN Model);

    string? Validate(ABE_EXTERN Model, bool IsInsert);
    #endregion
}

public class ABE_EXTERNRepository : RepositoryBase, IABE_EXTERNRepository
{
    public ABE_EXTERNRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ABE_EXTERN>? GetList(int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ABE_EXTERN>(
                @"SELECT * FROM ABE_EXTERN
                        WHERE abecod=@abecod",
                new { abecod = CustomerID });

            var listDest = connection.Query<ABE_EXTERN_DESTS>(
                @"SELECT * FROM ABE_EXTERN_DESTS
                        WHERE abecod=@abecod",
                new { abecod = CustomerID });

            foreach (var item in list)
            {
                item.Destinations = new ObservableCollection<ABE_EXTERN_DESTS>(listDest.Where(w => w.abecod == item.abecod && w.abeextcode == item.abeextcode && w.abeextid == item.abeextid).ToList());
            }

            return new ObservableCollection<ABE_EXTERN>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE_EXTERN? Get(int abecod, string abeextcode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE_EXTERN>(
                "SELECT * FROM ABE_EXTERN WHERE abecod = @abecod AND abeextcode = @abeextcode",
                new { abecod = abecod, abeextcode = abeextcode })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ABE_EXTERN? GetByExtern(string abeextcode, string abeextid)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ABE_EXTERN>(
                "SELECT * FROM ABE_EXTERN WHERE LOWER(abeextcode) = @abeextcode AND abeextid = @abeextid",
                new { abeextcode = abeextcode, abeextid = abeextid })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int abecod, string abeextcode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ABE_EXTERN WHERE abecod = @abecod AND abeextcode = @abeextcode",
                new { abecod = abecod, abeextcode = abeextcode }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ABE_EXTERN (abecod,abeextcode,abeextid) OUTPUT INSERTED.rv VALUES(@abecod,@abeextcode,@abeextid)";
    public string UPDATE_QUERY => "UPDATE ABE_EXTERN SET abecod = @abecod,abeextcode = @abeextcode,abeextid = @abeextid OUTPUT INSERTED.rv WHERE abecod = @abecod AND abeextcode = @abeextcode AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ABE_EXTERN OUTPUT DELETED.rv WHERE abecod = @abecod AND abeextcode = @abeextcode AND rv = @rv";
    public bool Insert(ABE_EXTERN Model)
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

    public bool Update(ABE_EXTERN Model)
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

    public bool Delete(ABE_EXTERN Model)
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

    public string? Validate(ABE_EXTERN Model, bool IsInsert)
    {
        try
        {
            if ((Model.abecod > 0 && !string.IsNullOrEmpty(Model.abeextcode) && IsInsert && !Exists(Model.abecod, Model.abeextcode)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.abeextid))
                {
                    return null;
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