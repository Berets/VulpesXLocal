using VulpesX.DAL;

namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_EXPIRESRepository
{
    ObservableCollection<ACC_EINVOICE_EXPIRES>? GetList();

    ObservableCollection<ACC_EINVOICE_EXPIRES>? GetList(long id);

    ACC_EINVOICE_EXPIRES? Get(long id, DateTime fattdatascad);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_EXPIRES Model);

    bool Update(ACC_EINVOICE_EXPIRES Model);

    bool Delete(ACC_EINVOICE_EXPIRES Model);
    #endregion
}

public class ACC_EINVOICE_EXPIRESRepository : RepositoryBase, IACC_EINVOICE_EXPIRESRepository
{
    public ACC_EINVOICE_EXPIRESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_EXPIRES>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_EXPIRES>(
                "SELECT * FROM ACC_EINVOICE_EXPIRES");

            return new ObservableCollection<ACC_EINVOICE_EXPIRES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_EXPIRES>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_EXPIRES, FE_PAGDOC, TAB_ACC_TIPPAG, ACC_EINVOICE_EXPIRES>(
                @"SELECT e.*, fet.FEPACOD, fet.FEPADES, tp.inccod, tp.incdes FROM ACC_EINVOICE_EXPIRES AS e
                        LEFT JOIN FE_PAGDOC AS fet ON fet.FEPACOD=e.fattipopag
                        LEFT JOIN TAB_ACC_TIPPAG AS tp ON tp.inccod=fet.FEPATIPP
                        WHERE e.id = @id
                        ORDER BY fattdatascad",
                (exp, fet, tp) => { exp.PaymentType = fet; exp.PaymentTypeInternal = tp; return exp; },
                new { id = id }, splitOn: "FEPACOD,inccod");

            return new ObservableCollection<ACC_EINVOICE_EXPIRES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_EXPIRES? Get(long id, DateTime fattdatascad)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_EXPIRES>(
                "SELECT * FROM ACC_EINVOICE_EXPIRES WHERE id = @id AND fattdatascad = @fattdatascad",
                new { id = id, fattdatascad = fattdatascad })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_EXPIRES (fattsoc,fattnum,fattdata,fattpiva,fattdatascad,fattimpscad,fattipopag,fattiban,fattistu,fattcab,fattabi,fattcond,FATTTIPOLPAG,FATTCODPAGV,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattdatascad,@fattimpscad,@fattipopag,@fattiban,@fattistu,@fattcab,@fattabi,@fattcond,@FATTTIPOLPAG,@FATTCODPAGV,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_EXPIRES SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattdatascad = @fattdatascad,fattimpscad = @fattimpscad,fattipopag = @fattipopag,fattiban = @fattiban,fattistu = @fattistu,fattcab = @fattcab,fattabi = @fattabi,fattcond = @fattcond,FATTTIPOLPAG = @FATTTIPOLPAG,FATTCODPAGV = @FATTCODPAGV,id = @id OUTPUT INSERTED.rv WHERE id = @id AND fattdatascad = @fattdatascad AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_EXPIRES OUTPUT DELETED.rv WHERE id = @id AND fattdatascad = @fattdatascad AND rv = @rv";
    public bool Insert(ACC_EINVOICE_EXPIRES Model)
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

    public bool Update(ACC_EINVOICE_EXPIRES Model)
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

    public bool Delete(ACC_EINVOICE_EXPIRES Model)
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

public class ACC_EINVOICE_EXPIRESUfpRepository : RepositoryBase, IACC_EINVOICE_EXPIRESRepository
{
    public ACC_EINVOICE_EXPIRESUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_EXPIRES>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_EINVOICE_EXPIRES>(
                "SELECT * FROM ACC_EINVOICE_EXPIRES");

            return new ObservableCollection<ACC_EINVOICE_EXPIRES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_EXPIRES>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_EXPIRES, FE_PAGDOC, TAB_ACC_TIPPAG, ACC_EINVOICE_EXPIRES>(
                @"SELECT 
e.fattsoc,
e.fattnum,
e.fattdata,
e.fattpiva,
e.fattimpscad,
e.fattipopag,
e.fattiban,
e.fattcond,
e.fattabi,
e.fattcab,
e.fattistu,
e.fattcodpagv,
e.fatttipolPag,
CASE 
    WHEN e.fattdatascad = '1753-01-01' THEN NULL
    ELSE e.fattdatascad
END AS fattdatascad,
fet.FEPACOD,
fet.FEPADES, 
tp.inccod, 
tp.incdes 
FROM FATTIMPLEVEL3 AS e
                        LEFT JOIN FE_PAGDOC AS fet ON fet.FEPACOD=e.fattipopag
                        LEFT JOIN PAGAMENTI AS tp ON tp.inccod=fet.FEPATIPP
                         WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva
                        ORDER BY fattdatascad",
                (exp, fet, tp) => { exp.PaymentType = fet; exp.PaymentTypeInternal = tp; return exp; },
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva }, splitOn: "FEPACOD,inccod");

                return new ObservableCollection<ACC_EINVOICE_EXPIRES>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_EXPIRES? Get(long id, DateTime fattdatascad)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_EXPIRES>(
                "SELECT * FROM ACC_EINVOICE_EXPIRES WHERE id = @id AND fattdatascad = @fattdatascad",
                new { id = id, fattdatascad = fattdatascad })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPLEVEL3 (fattsoc,fattnum,fattdata,fattpiva,fattdatascad,fattimpscad,fattipopag,fattiban,fattistu,fattcab,fattabi,fattcond,FATTTIPOLPAG,FATTCODPAGV) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@fattdatascad,@fattimpscad,@fattipopag,@fattiban,@fattistu,@fattcab,@fattabi,@fattcond,@FATTTIPOLPAG,@FATTCODPAGV)";
    public string UPDATE_QUERY => "UPDATE FATTIMPLEVEL3 SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,fattdatascad = @fattdatascad,fattimpscad = @fattimpscad,fattipopag = @fattipopag,fattiban = @fattiban,fattistu = @fattistu,fattcab = @fattcab,fattabi = @fattabi,fattcond = @fattcond,FATTTIPOLPAG = @FATTTIPOLPAG,FATTCODPAGV = @FATTCODPAGV OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattdatascad = @fattdatascad AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPLEVEL3 OUTPUT DELETED.rv  WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdata = @fattdata AND fattpiva = @fattpiva AND fattdatascad = @fattdatascad AND rv = @rv";
    public bool Insert(ACC_EINVOICE_EXPIRES Model)
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

    public bool Update(ACC_EINVOICE_EXPIRES Model)
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

    public bool Delete(ACC_EINVOICE_EXPIRES Model)
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