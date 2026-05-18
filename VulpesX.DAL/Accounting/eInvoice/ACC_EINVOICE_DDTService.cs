namespace VulpesX.DAL.Accounting.eInvoice;


public interface IACC_EINVOICE_DDTRepository
{
    ObservableCollection<ACC_EINVOICE_DDT>? GetList();

    ObservableCollection<ACC_EINVOICE_DDT>? GetList(long id);

    ACC_EINVOICE_DDT? Get(long id, int ddtriga);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_DDT Model);

    bool Update(ACC_EINVOICE_DDT Model);

    bool Delete(ACC_EINVOICE_DDT Model);
    #endregion
}

public class ACC_EINVOICE_DDTRepository : RepositoryBase, IACC_EINVOICE_DDTRepository
{
    public ACC_EINVOICE_DDTRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_DDT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_DDT>(
                "SELECT * FROM ACC_EINVOICE_DDT");

            return new ObservableCollection<ACC_EINVOICE_DDT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_DDT>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_DDT>(
                @"SELECT * FROM ACC_EINVOICE_DDT
                        WHERE id = @id
                        ORDER BY ddtriga",
                new { id = id });

            return new ObservableCollection<ACC_EINVOICE_DDT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_DDT? Get(long id, int ddtriga)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_DDT>(
                "SELECT * FROM ACC_EINVOICE_DDT WHERE id = @id AND ddtriga = @ddtriga",
                new { id = id, ddtriga = ddtriga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_DDT (fattsoc,fattnum,fattdata,fattpiva,ddtriga,ddtdata,ddtnum,ddtriferiga,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@ddtriga,@ddtdata,@ddtnum,@ddtriferiga,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_DDT SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,ddtriga = @ddtriga,ddtdata = @ddtdata,ddtnum = @ddtnum,ddtriferiga = @ddtriferiga,id = @id OUTPUT INSERTED.rv WHERE id = @id AND ddtriga = @ddtriga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_DDT OUTPUT DELETED.rv WHERE id = @id AND ddtriga = @ddtriga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_DDT Model)
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

    public bool Update(ACC_EINVOICE_DDT Model)
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

    public bool Delete(ACC_EINVOICE_DDT Model)
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

public class ACC_EINVOICE_DDTUfpRepository : RepositoryBase, IACC_EINVOICE_DDTRepository
{
    public ACC_EINVOICE_DDTUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_DDT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_DDT>(
                "SELECT * FROM ACC_EINVOICE_DDT");

            return new ObservableCollection<ACC_EINVOICE_DDT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_DDT>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_DDT>(
                @"SELECT * FROM FATTIMPDDT
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY ddtriga",
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva });

                return new ObservableCollection<ACC_EINVOICE_DDT>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_DDT? Get(long id, int ddtriga)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_DDT>(
                "SELECT * FROM ACC_EINVOICE_DDT WHERE id = @id AND ddtriga = @ddtriga",
                new { id = id, ddtriga = ddtriga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPDDT (fattsoc,fattnum,fattdata,fattpiva,ddtriga,ddtdata,ddtnum,ddtriferiga) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@ddtriga,@ddtdata,@ddtnum,@ddtriferiga)";
    public string UPDATE_QUERY => "UPDATE FATTIMPDDT SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,ddtriga = @ddtriga,ddtdata = @ddtdata,ddtnum = @ddtnum,ddtriferiga = @ddtriferiga OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva  AND ddtriga = @ddtriga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPDDT OUTPUT DELETED.rv  WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva  AND ddtriga = @ddtriga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_DDT Model)
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

    public bool Update(ACC_EINVOICE_DDT Model)
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

    public bool Delete(ACC_EINVOICE_DDT Model)
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