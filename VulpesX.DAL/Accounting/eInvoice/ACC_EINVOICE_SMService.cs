
namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_SMRepository
{
    ObservableCollection<ACC_EINVOICE_SM>? GetList();

    ObservableCollection<ACC_EINVOICE_SM>? GetList(long id);

    ACC_EINVOICE_SM? Get(long id, int sctprog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_SM Model);

    bool Update(ACC_EINVOICE_SM Model);

    bool Delete(ACC_EINVOICE_SM Model);
    #endregion
}

public class ACC_EINVOICE_SMRepository : RepositoryBase, IACC_EINVOICE_SMRepository
{
    public ACC_EINVOICE_SMRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_SM>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_SM>(
                "SELECT * FROM ACC_EINVOICE_SM");

            return new ObservableCollection<ACC_EINVOICE_SM>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_SM>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_SM>(
                @"SELECT * FROM ACC_EINVOICE_SM
                        WHERE id = @id
                        ORDER BY sctprog",
                new { id = id });

            return new ObservableCollection<ACC_EINVOICE_SM>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_SM? Get(long id, int sctprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_EINVOICE_SM>(
                "SELECT * FROM ACC_EINVOICE_SM WHERE id=@id AND sctprog = @sctprog",
                new { id = id, sctprog = sctprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_SM (fattsoc,fattnum,fattdata,fattpiva,sctprog,scttipo,sctimpo,sctperc,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@sctprog,@scttipo,@sctimpo,@sctperc,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_SM SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,sctprog = @sctprog,scttipo = @scttipo,sctimpo = @sctimpo,sctperc = @sctperc,id = @id OUTPUT INSERTED.rv WHERE id = @id AND sctprog = @sctprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_SM OUTPUT DELETED.rv WHERE id = @id AND sctprog = @sctprog AND rv = @rv";
    public bool Insert(ACC_EINVOICE_SM Model)
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

    public bool Update(ACC_EINVOICE_SM Model)
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

    public bool Delete(ACC_EINVOICE_SM Model)
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

public class ACC_EINVOICE_SMUfpRepository : RepositoryBase, IACC_EINVOICE_SMRepository
{
    public ACC_EINVOICE_SMUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_SM>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_SM>(
                "SELECT * FROM ACC_EINVOICE_SM");

            return new ObservableCollection<ACC_EINVOICE_SM>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_SM>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_SM>(
                @"SELECT * FROM FATTIMPSCONTOTESTA
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY sctprog",
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva });

                return new ObservableCollection<ACC_EINVOICE_SM>(list);
            }
            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_SM? Get(long id, int sctprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_EINVOICE_SM>(
                "SELECT * FROM ACC_EINVOICE_SM WHERE id=@id AND sctprog = @sctprog",
                new { id = id, sctprog = sctprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPSCONTOTESTA (fattsoc,fattnum,fattdata,fattpiva,sctprog,scttipo,sctimpo,sctperc) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@sctprog,@scttipo,@sctimpo,@sctperc)";
    public string UPDATE_QUERY => "UPDATE FATTIMPSCONTOTESTA SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,sctprog = @sctprog,scttipo = @scttipo,sctimpo = @sctimpo,sctperc = @sctperc OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata=@fattdata AND fattpiva = @fattpiva AND sctprog = @sctprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPSCONTOTESTA OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata=@fattdata AND fattpiva = @fattpiva AND sctprog = @sctprog AND rv = @rv";
    public bool Insert(ACC_EINVOICE_SM Model)
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

    public bool Update(ACC_EINVOICE_SM Model)
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

    public bool Delete(ACC_EINVOICE_SM Model)
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