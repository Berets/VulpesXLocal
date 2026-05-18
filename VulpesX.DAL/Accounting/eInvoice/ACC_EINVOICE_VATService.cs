namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_VATRepository
{
    ObservableCollection<ACC_EINVOICE_VAT>? GetList();

    ObservableCollection<ACC_EINVOICE_VAT>? GetList(long id);

    ACC_EINVOICE_VAT? Get(long id, int fattprog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_VAT Model);

    bool Update(ACC_EINVOICE_VAT Model);

    bool Delete(ACC_EINVOICE_VAT Model);
    #endregion
}

public class ACC_EINVOICE_VATRepository : RepositoryBase, IACC_EINVOICE_VATRepository
{
    public ACC_EINVOICE_VATRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_VAT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_VAT>(
                "SELECT * FROM ACC_EINVOICE_VAT");

            return new ObservableCollection<ACC_EINVOICE_VAT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_VAT>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_VAT, FE_IVADOC, ACC_EINVOICE_VAT>(
                @"SELECT v.*, fei.FETICod, fei.FETIDes FROM ACC_EINVOICE_VAT AS v
                        LEFT JOIN FE_IVADOC AS fei ON fei.FETICod=v.fattnatu
                        WHERE v.id=@id
                        ORDER BY v.fattprog",
                (vat, fei) => { vat.SelectedNature = fei; return vat; },
                new { id = id }, splitOn: "FETICod");

            return new ObservableCollection<ACC_EINVOICE_VAT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_VAT? Get(long id, int fattprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_VAT>(
                "SELECT * FROM ACC_EINVOICE_VAT WHERE id=@id AND fattprog = @fattprog",
                new { id = @id, fattprog = fattprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_VAT (fattsoc,fattnum,fattdata,fattpiva,fattprog,Fattaliq,fattimpodett,fattimpostadett,fattesi,fattrifenorm,fattnatu,id,fattarrotondamento) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattprog,@Fattaliq,@fattimpodett,@fattimpostadett,@fattesi,@fattrifenorm,@fattnatu,@id,@fattarrotondamento)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_VAT SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattprog = @fattprog,Fattaliq = @Fattaliq,fattimpodett = @fattimpodett,fattimpostadett = @fattimpostadett,fattesi = @fattesi,fattrifenorm = @fattrifenorm,fattnatu = @fattnatu,id = @id,fattarrotondamento = @fattarrotondamento OUTPUT INSERTED.rv WHERE id = @id AND fattprog = @fattprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_VAT OUTPUT DELETED.rv WHERE id = @id AND fattprog = @fattprog AND rv = @rv";
    public bool Insert(ACC_EINVOICE_VAT Model)
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

    public bool Update(ACC_EINVOICE_VAT Model)
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

    public bool Delete(ACC_EINVOICE_VAT Model)
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

public class ACC_EINVOICE_VATUfpRepository : RepositoryBase, IACC_EINVOICE_VATRepository
{
    public ACC_EINVOICE_VATUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_VAT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_VAT>(
                "SELECT * FROM ACC_EINVOICE_VAT");

            return new ObservableCollection<ACC_EINVOICE_VAT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_VAT>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_VAT, FE_IVADOC, ACC_EINVOICE_VAT>(
                @"SELECT v.*, fei.FETICod, fei.FETIDes FROM FATTIMPLEVEL1 AS v
                        LEFT JOIN FE_IVADOC AS fei ON fei.FETICod=v.fattnatu
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY v.fattprog",
                (vat, fei) => { vat.SelectedNature = fei; return vat; },
                 new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva }, splitOn: "FETICod");

                return new ObservableCollection<ACC_EINVOICE_VAT>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_VAT? Get(long id, int fattprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_VAT>(
                "SELECT * FROM ACC_EINVOICE_VAT WHERE id=@id AND fattprog = @fattprog",
                new { id = @id, fattprog = fattprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPLEVEL1 (fattsoc,fattnum,fattdata,fattpiva,fattprog,Fattaliq,fattimpodett,fattimpostadett,fattesi,fattrifenorm,fattnatu,fattarrotondamento) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattprog,@Fattaliq,@fattimpodett,@fattimpostadett,@fattesi,@fattrifenorm,@fattnatu,@fattarrotondamento)";
    public string UPDATE_QUERY => "UPDATE FATTIMPLEVEL1 SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattprog = @fattprog,Fattaliq = @Fattaliq,fattimpodett = @fattimpodett,fattimpostadett = @fattimpostadett,fattesi = @fattesi,fattrifenorm = @fattrifenorm,fattnatu = @fattnatu,fattarrotondamento = @fattarrotondamento OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata=@fattdata AND fattpiva = @fattpiva AND fattprog = @fattprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPLEVEL1 OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata=@fattdata AND fattpiva = @fattpiva AND fattprog = @fattprog AND rv = @rv";
    public bool Insert(ACC_EINVOICE_VAT Model)
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

    public bool Update(ACC_EINVOICE_VAT Model)
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

    public bool Delete(ACC_EINVOICE_VAT Model)
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