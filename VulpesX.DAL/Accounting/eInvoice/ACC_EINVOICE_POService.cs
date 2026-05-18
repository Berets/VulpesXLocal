using VulpesX.DAL;
using VulpesX.DAL.Accounting.eInvoice;

namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_PORepository
{
    ObservableCollection<ACC_EINVOICE_PO>? GetList();

    ObservableCollection<ACC_EINVOICE_PO>? GetList(long id);

    ACC_EINVOICE_PO? Get(long id, int riga);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_PO Model);

    bool Update(ACC_EINVOICE_PO Model);

    bool Delete(ACC_EINVOICE_PO Model);
    #endregion
}

public class ACC_EINVOICE_PORepository : RepositoryBase, IACC_EINVOICE_PORepository
{
    public ACC_EINVOICE_PORepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_PO>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_PO>(
                "SELECT * FROM ACC_EINVOICE_PO");

            return new ObservableCollection<ACC_EINVOICE_PO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_PO>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_PO>(
                @"SELECT * FROM ACC_EINVOICE_PO
                        WHERE id = @id
                        ORDER BY riga",
                new { id = id });

            return new ObservableCollection<ACC_EINVOICE_PO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_PO? Get(long id, int riga)
    {
        try
        {
            using var connection = GetOpenConnection();



            return connection.Query<ACC_EINVOICE_PO>(
                "SELECT * FROM ACC_EINVOICE_PO WHERE id = @id AND riga = @riga",
                new { id = id, riga = riga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_PO (fattsoc,fattnum,fattdata,fattpiva,riga,numitem,iddocumento,datadoc,rigarife,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@riga,@numitem,@iddocumento,@datadoc,@rigarife,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_PO SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,riga = @riga,numitem = @numitem,iddocumento = @iddocumento,datadoc = @datadoc,rigarife = @rigarife,id = @id OUTPUT INSERTED.rv WHERE id = @id AND riga = @riga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_PO OUTPUT DELETED.rv WHERE id = @id AND riga = @riga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_PO Model)
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

    public bool Update(ACC_EINVOICE_PO Model)
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

    public bool Delete(ACC_EINVOICE_PO Model)
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

public class ACC_EINVOICE_POUfpRepository : RepositoryBase, IACC_EINVOICE_PORepository
{
    public ACC_EINVOICE_POUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_PO>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_PO>(
                "SELECT * FROM ACC_EINVOICE_PO");

            return new ObservableCollection<ACC_EINVOICE_PO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_PO>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_PO>(
                @"SELECT c.*, c.riga1 as riga FROM FATTIMPORDACQ as c
                         WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY riga1",
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva });

                return new ObservableCollection<ACC_EINVOICE_PO>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_PO? Get(long id, int riga)
    {
        try
        {
            using var connection = GetOpenConnection();



            return connection.Query<ACC_EINVOICE_PO>(
                "SELECT * FROM ACC_EINVOICE_PO WHERE id = @id AND riga = @riga",
                new { id = id, riga = riga })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPORDACQ (fattsoc,fattnum,fattdata,fattpiva,riga1,numitem,iddocumento,datadoc,rigarife) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@riga,@numitem,@iddocumento,@datadoc,@rigarife)";
    public string UPDATE_QUERY => "UPDATE FATTIMPORDACQ SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,riga1 = @riga,numitem = @numitem,iddocumento = @iddocumento,datadoc = @datadoc,rigarife = @rigarife OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva = @fattpiva AND riga = @riga AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPORDACQ OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum=@fattnum AND fattdata=@fattdata AND fattpiva = @fattpiva AND riga = @riga AND rv = @rv";
    public bool Insert(ACC_EINVOICE_PO Model)
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

    public bool Update(ACC_EINVOICE_PO Model)
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

    public bool Delete(ACC_EINVOICE_PO Model)
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