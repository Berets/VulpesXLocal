namespace VulpesX.DAL.Accounting.eInvoice;


public interface IACC_EINVOICE_ROWS_PIDRepository
{
    ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? GetList();

    ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? GetList(long id);

    ACC_EINVOICE_ROWS_PIDS? Get(long id, int fattriga, int artprog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_ROWS_PIDS Model);

    bool Update(ACC_EINVOICE_ROWS_PIDS Model);

    bool Delete(ACC_EINVOICE_ROWS_PIDS Model);
    #endregion
}

public class ACC_EINVOICE_ROWS_PIDRepository : RepositoryBase, IACC_EINVOICE_ROWS_PIDRepository
{
    public ACC_EINVOICE_ROWS_PIDRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS_PIDS>(
                "SELECT * FROM ACC_EINVOICE_ROWS_PIDS");

            return new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS_PIDS>(
                @"SELECT * FROM ACC_EINVOICE_ROWS_PIDS
                        WHERE id = @id",
                new { id = id });

            return new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_ROWS_PIDS? Get(long id, int fattriga, int artprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_ROWS_PIDS>(
                "SELECT * FROM ACC_EINVOICE_ROWS_PIDS WHERE id = @id AND fattriga = @fattriga AND artprog = @artprog",
                new { id = id, fattriga = fattriga, artprog = artprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_ROWS_PIDS (fattsoc,fattnum,fattdata,fattpiva,fattriga,artprog,artvalcod,artipcod,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattriga,@artprog,@artvalcod,@artipcod,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_ROWS_PIDS SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattriga = @fattriga,artprog = @artprog,artvalcod = @artvalcod,artipcod = @artipcod,id = @id OUTPUT INSERTED.rv WHERE id = @id AND fattriga = @fattriga AND artprog = @artprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_ROWS_PIDS OUTPUT DELETED.rv WHERE id = @id AND fattriga = @fattriga AND artprog = @artprog AND rv = @rv";
    public bool Insert(ACC_EINVOICE_ROWS_PIDS Model)
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

    public bool Update(ACC_EINVOICE_ROWS_PIDS Model)
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

    public bool Delete(ACC_EINVOICE_ROWS_PIDS Model)
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

public class ACC_EINVOICE_ROWS_PIDUfpRepository : RepositoryBase, IACC_EINVOICE_ROWS_PIDRepository
{
    public ACC_EINVOICE_ROWS_PIDUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_ROWS_PIDS>(
                "SELECT * FROM ACC_EINVOICE_ROWS_PIDS");

            return new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_ROWS_PIDS>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_ROWS_PIDS>(
                @"SELECT * FROM FATTIMPLEVEL2ARTICOLI
                         WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva",
                    new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva });

                return new ObservableCollection<ACC_EINVOICE_ROWS_PIDS>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_ROWS_PIDS? Get(long id, int fattriga, int artprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_ROWS_PIDS>(
                "SELECT * FROM ACC_EINVOICE_ROWS_PIDS WHERE id = @id AND fattriga = @fattriga AND artprog = @artprog",
                new { id = id, fattriga = fattriga, artprog = artprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPLEVEL2ARTICOLI (fattsoc,fattnum,fattdata,fattpiva,fattriga,artprog,artvalcod,artipcod) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattriga,@artprog,@artvalcod,@artipcod)";
    public string UPDATE_QUERY => "UPDATE FATTIMPLEVEL2ARTICOLI SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattriga = @fattriga,artprog = @artprog,artvalcod = @artvalcod,artipcod = @artipcod OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum =@fattnum AND fattdata=@fattdata AND fattpiva =@fattpiva AND fattriga = @fattriga AND artprog = @artprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPLEVEL2ARTICOLI OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum =@fattnum AND fattdata=@fattdata AND fattpiva =@fattpiva AND fattriga = @fattriga AND artprog = @artprog";
    public bool Insert(ACC_EINVOICE_ROWS_PIDS Model)
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

    public bool Update(ACC_EINVOICE_ROWS_PIDS Model)
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

    public bool Delete(ACC_EINVOICE_ROWS_PIDS Model)
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