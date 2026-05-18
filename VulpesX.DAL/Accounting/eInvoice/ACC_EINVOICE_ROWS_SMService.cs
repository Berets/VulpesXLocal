using VulpesX.Models.Default;

namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_ROWS_SMRepository
{
    ObservableCollection<ACC_EINVOICE_ROWS_SM>? GetList();

    ObservableCollection<ACC_EINVOICE_ROWS_SM>? GetList(long id);

    ACC_EINVOICE_ROWS_SM? Get(long id, int fattriga, int Progsc);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_ROWS_SM Model);

    bool Update(ACC_EINVOICE_ROWS_SM Model);

    bool Delete(ACC_EINVOICE_ROWS_SM Model);
    #endregion
}

public class ACC_EINVOICE_ROWS_SMRepository : RepositoryBase, IACC_EINVOICE_ROWS_SMRepository
{
    public ACC_EINVOICE_ROWS_SMRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_ROWS_SM>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS_SM>(
                "SELECT * FROM ACC_EINVOICE_ROWS_SM");

            return new ObservableCollection<ACC_EINVOICE_ROWS_SM>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_ROWS_SM>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_ROWS_SM>(
                @"SELECT * FROM ACC_EINVOICE_ROWS_SM
                        WHERE id = @id",
                new { id = id });

            return new ObservableCollection<ACC_EINVOICE_ROWS_SM>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_ROWS_SM? Get(long id, int fattriga, int Progsc)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_EINVOICE_ROWS_SM>(
                "SELECT * FROM ACC_EINVOICE_ROWS_SM WHERE id=@id AND fattriga = @fattriga AND Progsc = @Progsc",
                new { id = id, fattriga = fattriga, Progsc = Progsc })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_ROWS_SM (fattsoc,fattnum,fattdata,fattpiva,fattriga,Progsc,sctipo,scimpo,scperc,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattriga,@Progsc,@sctipo,@scimpo,@scperc,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_ROWS_SM SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattriga = @fattriga,Progsc = @Progsc,sctipo = @sctipo,scimpo = @scimpo,scperc = @scperc,id = @id OUTPUT INSERTED.rv WHERE id = @id AND fattriga = @fattriga AND Progsc = @Progsc AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_ROWS_SM OUTPUT DELETED.rv WHERE id = @id AND fattriga = @fattriga AND Progsc = @Progsc AND rv = @rv";
    public bool Insert(ACC_EINVOICE_ROWS_SM Model)
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

    public bool Update(ACC_EINVOICE_ROWS_SM Model)
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

    public bool Delete(ACC_EINVOICE_ROWS_SM Model)
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

public class ACC_EINVOICE_ROWS_SMUfpRepository : RepositoryBase, IACC_EINVOICE_ROWS_SMRepository
{
    public ACC_EINVOICE_ROWS_SMUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_ROWS_SM>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS_SM>(
                "SELECT * FROM ACC_EINVOICE_ROWS_SM");

            return new ObservableCollection<ACC_EINVOICE_ROWS_SM>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_ROWS_SM>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_ROWS_SM>(
                    @"SELECT * FROM FATTIMPLEVEL2SCONTO
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva",
                    new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva });

                return new ObservableCollection<ACC_EINVOICE_ROWS_SM>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_ROWS_SM? Get(long id, int fattriga, int Progsc)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_EINVOICE_ROWS_SM>(
                "SELECT * FROM ACC_EINVOICE_ROWS_SM WHERE id=@id AND fattriga = @fattriga AND Progsc = @Progsc",
                new { id = id, fattriga = fattriga, Progsc = Progsc })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPLEVEL2SCONTO (fattsoc,fattnum,fattdata,fattpiva,fattriga,Progsc,sctipo,scimpo,scperc) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattriga,@Progsc,@sctipo,@scimpo,@scperc)";
    public string UPDATE_QUERY => "UPDATE FATTIMPLEVEL2SCONTO SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattriga = @fattriga,Progsc = @Progsc,sctipo = @sctipo,scimpo = @scimpo,scperc = @scperc OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattriga = @fattriga AND Progsc = @Progsc AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPLEVEL2SCONTO OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattriga = @fattriga AND Progsc = @Progsc AND rv = @rv";
    public bool Insert(ACC_EINVOICE_ROWS_SM Model)
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

    public bool Update(ACC_EINVOICE_ROWS_SM Model)
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

    public bool Delete(ACC_EINVOICE_ROWS_SM Model)
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