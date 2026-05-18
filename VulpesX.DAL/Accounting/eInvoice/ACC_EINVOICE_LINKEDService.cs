namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_LINKEDRepository
{
    ObservableCollection<ACC_EINVOICE_LINKED>? GetList();

    ObservableCollection<ACC_EINVOICE_LINKED>? GetList(long id);

    ACC_EINVOICE_LINKED? Get(long id, int fattcollriga);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_LINKED Model);

    bool Update(ACC_EINVOICE_LINKED Model);

    bool Delete(ACC_EINVOICE_LINKED Model);
    #endregion
}

public class ACC_EINVOICE_LINKEDRepository : RepositoryBase, IACC_EINVOICE_LINKEDRepository
{
    public ACC_EINVOICE_LINKEDRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_LINKED>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_LINKED>(
                "SELECT * FROM ACC_EINVOICE_LINKED");

            return new ObservableCollection<ACC_EINVOICE_LINKED>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_LINKED>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_LINKED>(
                @"SELECT * FROM ACC_EINVOICE_LINKED
                        WHERE id = @id
                        ORDER BY fattcollriga",
                new { id = id });

            return new ObservableCollection<ACC_EINVOICE_LINKED>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_LINKED? Get(long id, int fattcollriga)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_LINKED>(
                "SELECT * FROM ACC_EINVOICE_LINKED WHERE id = @id AND fattcollriga = @fattcollriga",
                new { id = id, fattcollriga = fattcollriga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_LINKED (fattsoc,fattnum,fattdata,fattpiva,fattcollriga,fattcollnumitem,fattcolliddocumento,fattcolldatadoc,fattcollriferiga,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattcollriga,@fattcollnumitem,@fattcolliddocumento,@fattcolldatadoc,@fattcollriferiga,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_LINKED SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattcollriga = @fattcollriga,fattcollnumitem = @fattcollnumitem,fattcolliddocumento = @fattcolliddocumento,fattcolldatadoc = @fattcolldatadoc,fattcollriferiga = @fattcollriferiga,id = @id OUTPUT INSERTED.rv WHERE id = @id AND fattcollriga = @fattcollriga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_LINKED OUTPUT DELETED.rv WHERE id = @id AND fattcollriga = @fattcollriga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_LINKED Model)
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

    public bool Update(ACC_EINVOICE_LINKED Model)
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

    public bool Delete(ACC_EINVOICE_LINKED Model)
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
    #endregion
}

public class ACC_EINVOICE_LINKEDUfpRepository : RepositoryBase, IACC_EINVOICE_LINKEDRepository
{
    public ACC_EINVOICE_LINKEDUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_LINKED>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_LINKED>(
                "SELECT * FROM ACC_EINVOICE_LINKED");

            return new ObservableCollection<ACC_EINVOICE_LINKED>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_LINKED>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_LINKED>(
                @"SELECT * FROM FATTIMPFATTCOLLEGATE
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY fattcollriga",
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva });

                return new ObservableCollection<ACC_EINVOICE_LINKED>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_LINKED? Get(long id, int fattcollriga)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_LINKED>(
                "SELECT * FROM ACC_EINVOICE_LINKED WHERE id = @id AND fattcollriga = @fattcollriga",
                new { id = id, fattcollriga = fattcollriga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPFATTCOLLEGATE (fattsoc,fattnum,fattdata,fattpiva,fattcollriga,fattcollnumitem,fattcolliddocumento,fattcolldatadoc,fattcollriferiga) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattcollriga,@fattcollnumitem,@fattcolliddocumento,@fattcolldatadoc,@fattcollriferiga)";
    public string UPDATE_QUERY => "UPDATE FATTIMPFATTCOLLEGATE SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattcollriga = @fattcollriga,fattcollnumitem = @fattcollnumitem,fattcolliddocumento = @fattcolliddocumento,fattcolldatadoc = @fattcolldatadoc,fattcollriferiga = @fattcollriferiga OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattcollriga = @fattcollriga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPFATTCOLLEGATE OUTPUT DELETED.rv  WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattcollriga = @fattcollriga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_LINKED Model)
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

    public bool Update(ACC_EINVOICE_LINKED Model)
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

    public bool Delete(ACC_EINVOICE_LINKED Model)
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
    #endregion
}
