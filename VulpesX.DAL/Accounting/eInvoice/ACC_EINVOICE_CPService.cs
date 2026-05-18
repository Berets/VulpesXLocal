namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_CPRepository
{
    ObservableCollection<ACC_EINVOICE_CP>? GetList();

    ObservableCollection<ACC_EINVOICE_CP>? GetList(long id);

    ACC_EINVOICE_CP? Get(long id, int progcassa);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_CP Model);

    bool Update(ACC_EINVOICE_CP Model);

    bool Delete(ACC_EINVOICE_CP Model);
    #endregion
}

public class ACC_EINVOICE_CPRepository : RepositoryBase, IACC_EINVOICE_CPRepository
{
    public ACC_EINVOICE_CPRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_CP>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_CP>(
                "SELECT * FROM ACC_EINVOICE_CP");

            return new ObservableCollection<ACC_EINVOICE_CP>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_CP>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_CP, FE_IVADOC, FE_TIPOCP, ACC_EINVOICE_CP>(
                @"SELECT c.*, i.FETICod, i.FETIDes, tc.id, tc.description FROM ACC_EINVOICE_CP AS c
                        LEFT JOIN FE_IVADOC AS i ON i.FETICod=c.natcassa
                        LEFT JOIN FE_TIPOCP AS tc ON tc.id=c.tipocassa
                        WHERE c.id = @id
                        ORDER BY c.progcassa",
                (cp, fei, tcp) => { cp.Nature = fei; cp.CPType = tcp; return cp; },
                new { id = id }, splitOn: "FETICod,id");

            return new ObservableCollection<ACC_EINVOICE_CP>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_CP? Get(long id, int progcassa)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_CP>(
                "SELECT * FROM ACC_EINVOICE_CP WHERE id=@id AND progcassa = @progcassa",
                new { id = id, progcassa = progcassa })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_CP (fattsoc,fattnum,fattdata,fattpiva,progcassa,tipocassa,alicassa,impcontricassa,impocassa,aliivacassa,ritecassa,natcassa,rifammicassa,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@progcassa,@tipocassa,@alicassa,@impcontricassa,@impocassa,@aliivacassa,@ritecassa,@natcassa,@rifammicassa,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_CP SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,progcassa = @progcassa,tipocassa = @tipocassa,alicassa = @alicassa,impcontricassa = @impcontricassa,impocassa = @impocassa,aliivacassa = @aliivacassa,ritecassa = @ritecassa,natcassa = @natcassa,rifammicassa = @rifammicassa,id = @id OUTPUT INSERTED.rv WHERE id = @id AND progcassa = @progcassa AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_CP OUTPUT DELETED.rv WHERE id = @id AND progcassa = @progcassa AND rv = @rv";
    public bool Insert(ACC_EINVOICE_CP Model)
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

    public bool Update(ACC_EINVOICE_CP Model)
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

    public bool Delete(ACC_EINVOICE_CP Model)
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

public class ACC_EINVOICE_CPUfpRepository : RepositoryBase, IACC_EINVOICE_CPRepository
{
    public ACC_EINVOICE_CPUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_CP>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_CP>(
                "SELECT * FROM ACC_EINVOICE_CP");

            return new ObservableCollection<ACC_EINVOICE_CP>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_CP>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_CP, FE_IVADOC, FE_TIPOCP, ACC_EINVOICE_CP>(
                @"SELECT c.*, i.FETICod, i.FETIDes, tc.id, tc.description FROM FATTIMPDATICASSAPREVIDENZIALE AS c
                        LEFT JOIN FE_IVADOC AS i ON i.FETICod=c.natcassa
                        LEFT JOIN FE_TIPOCP AS tc ON tc.id=c.tipocassa
                         WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY c.progcassa",
                (cp, fei, tcp) => { cp.Nature = fei; cp.CPType = tcp; return cp; },
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva }, splitOn: "FETICod,id");

                return new ObservableCollection<ACC_EINVOICE_CP>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_CP? Get(long id, int progcassa)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_CP>(
                "SELECT * FROM ACC_EINVOICE_CP WHERE id=@id AND progcassa = @progcassa",
                new { id = id, progcassa = progcassa })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPDATICASSAPREVIDENZIALE (fattsoc,fattnum,fattdata,fattpiva,progcassa,tipocassa,alicassa,impcontricassa,impocassa,aliivacassa,ritecassa,natcassa,rifammicassa) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@progcassa,@tipocassa,@alicassa,@impcontricassa,@impocassa,@aliivacassa,@ritecassa,@natcassa,@rifammicassa)";
    public string UPDATE_QUERY => "UPDATE FATTIMPDATICASSAPREVIDENZIALE SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,progcassa = @progcassa,tipocassa = @tipocassa,alicassa = @alicassa,impcontricassa = @impcontricassa,impocassa = @impocassa,aliivacassa = @aliivacassa,ritecassa = @ritecassa,natcassa = @natcassa,rifammicassa = @rifammicassa OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata=@fattdata AND fattpiva =@fattpiva  AND progcassa = @progcassa AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPDATICASSAPREVIDENZIALE OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata=@fattdata AND fattpiva =@fattpiva  AND progcassa = @progcassa AND rv = @rv";
    public bool Insert(ACC_EINVOICE_CP Model)
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

    public bool Update(ACC_EINVOICE_CP Model)
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

    public bool Delete(ACC_EINVOICE_CP Model)
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