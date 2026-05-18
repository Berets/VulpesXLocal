namespace VulpesX.DAL.Accounting.eInvoice;

public interface IACC_EINVOICE_RITRepository
{
    ObservableCollection<ACC_EINVOICE_RIT>? GetList();

    ObservableCollection<ACC_EINVOICE_RIT>? GetList(long id);

    ACC_EINVOICE_RIT? Get(long id, int progrit);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_EINVOICE_RIT Model);

    bool Update(ACC_EINVOICE_RIT Model);

    bool Delete(ACC_EINVOICE_RIT Model);

    #endregion
}

public class ACC_EINVOICE_RITRepository : RepositoryBase, IACC_EINVOICE_RITRepository
{
    public ACC_EINVOICE_RITRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_RIT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_RIT>(
                "SELECT * FROM ACC_EINVOICE_RIT");

            return new ObservableCollection<ACC_EINVOICE_RIT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_RIT>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_RIT, FE_TIPORIT, ACC_EINVOICE_RIT>(
                @"SELECT c.*, tc.id, tc.description FROM ACC_EINVOICE_RIT AS c
                        LEFT JOIN FE_TIPORIT AS tc ON tc.id=c.tiporit
                        WHERE c.id = @id
                        ORDER BY c.progrit",
                (rit, trit) => { rit.Ritenuta = trit; return rit; },
                new { id = id }, splitOn: "id");

            return new ObservableCollection<ACC_EINVOICE_RIT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_RIT? Get(long id, int progrit)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_RIT>(
                "SELECT * FROM ACC_EINVOICE_RIT WHERE id=@id AND progrit = @progrit",
                new { id = id, progrit = progrit })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_EINVOICE_RIT (fattsoc,fattnum,fattdata,fattpiva,progrit,tiporit,importo,aliquota,causalepagamento,id) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@progrit,@tiporit,@importo,@aliquota,@causalepagamento,@id)";
    public string UPDATE_QUERY => "UPDATE ACC_EINVOICE_RIT SET fattsoc = @fattsoc,fattnum = @fattnum,fattdata = @fattdata,fattpiva = @fattpiva,progrit = @progrit,tiporit = @tiporit,importo = @importo,aliquota = @aliquota,causalepagamento = @causalepagamento,id = @id OUTPUT INSERTED.rv WHERE id = @id AND progrit = @progrit AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_EINVOICE_RIT OUTPUT DELETED.rv WHERE id = @id AND progrit = @progrit AND rv = @rv";
    public bool Insert(ACC_EINVOICE_RIT Model)
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

    public bool Update(ACC_EINVOICE_RIT Model)
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

    public bool Delete(ACC_EINVOICE_RIT Model)
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

public class ACC_EINVOICE_RITUfpRepository : RepositoryBase, IACC_EINVOICE_RITRepository
{
    public ACC_EINVOICE_RITUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_EINVOICE_RIT>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_EINVOICE_RIT>(
                "SELECT * FROM ACC_EINVOICE_RIT");

            return new ObservableCollection<ACC_EINVOICE_RIT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ACC_EINVOICE_RIT>? GetList(long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var invoice = connection.Query<ACC_EINVOICE_HEADS>(@"SELECT * FROM FATTIMP WHERE id = @id", new { id = id }).FirstOrDefault();

            if (invoice != null)
            {
                var list = connection.Query<ACC_EINVOICE_RIT, FE_TIPORIT, ACC_EINVOICE_RIT>(
                $@"SELECT 
    c.fattprogr as progrit,
    c.tiporiten as tiporit,
    c.impriten as importo,
    c.aliqriten as aliquota,
    c.causriten as causalepagamento,
tc.FETRCod as id, tc.FETRDes as description FROM FATTIMPRITE AS c
                        LEFT JOIN FE_RITDOC AS tc ON tc.FETRCod=c.tiporiten
                        WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdari = @fattdata AND fattpiva = @fattpiva
                        ORDER BY c.fattprogr",
                (rit, trit) => { rit.Ritenuta = trit; return rit; },
                new { fattsoc = invoice.fattsoc, fattnum = invoice.fattnum, fattdata = invoice.fattdata, fattpiva = invoice.fattpiva }, splitOn: "id");

                return new ObservableCollection<ACC_EINVOICE_RIT>(list);
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_EINVOICE_RIT? Get(long id, int progrit)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_EINVOICE_RIT>(
                "SELECT * FROM ACC_EINVOICE_RIT WHERE id=@id AND progrit = @progrit",
                new { id = id, progrit = progrit })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO FATTIMPRITE (fattsoc,fattnum,fattdari,fattpiva,fattprogr,tiporiten,impriten,aliqriten,causriten) OUTPUT INSERTED.rv VALUES(@fattsoc,@fattnum,@fattdata,@fattpiva,@progrit,@tiporit,@importo,@aliquota,@causalepagamento)";
    public string UPDATE_QUERY => "UPDATE FATTIMPRITE SET fattsoc = @fattsoc,fattnum = @fattnum,fattdari = @fattdata,fattpiva = @fattpiva,fattprogr = @progrit,tiporiten = @tiporit,impriten = @importo,aliqriten = @aliquota,causriten = @causalepagament OUTPUT INSERTED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdari=@fattdata AND fattpiva=@fattpiva AND progrit = @progrit AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM FATTIMPRITE OUTPUT DELETED.rv WHERE fattsoc = @fattsoc AND fattnum = @fattnum AND fattdari=@fattdata AND fattpiva=@fattpiva AND progrit = @progrit AND rv = @rv";
    public bool Insert(ACC_EINVOICE_RIT Model)
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

    public bool Update(ACC_EINVOICE_RIT Model)
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

    public bool Delete(ACC_EINVOICE_RIT Model)
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