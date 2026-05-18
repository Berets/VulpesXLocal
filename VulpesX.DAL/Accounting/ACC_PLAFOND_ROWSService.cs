namespace VulpesX.DAL.Accounting;

public interface IACC_PLAFOND_ROWSRepository
{
    ObservableCollection<ACC_PLAFOND_ROWS>? GetList(string CompanyID, int CustomerID, int Year, int ID);

    ACC_PLAFOND_ROWS? Get(string Cliasoc, int Cliacod, int cliannosol, int cliprog, int clinumfat);

    bool Exists(string Cliasoc, int Cliacod, int cliannosol, int cliprog, int clinumfat);

    string? GetInvoiceArchived(string CompanyID, int CustomerID, short Year, int DefinitiveID);

    #region CRUD
    // remove [rv]
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_PLAFOND_ROWS Model);

    bool Update(ACC_PLAFOND_ROWS Model);

    bool Delete(ACC_PLAFOND_ROWS Model);

    string? Validate(ACC_PLAFOND_ROWS Model, bool IsInsert);
    #endregion
}

public class ACC_PLAFOND_ROWSRepository : RepositoryBase, IACC_PLAFOND_ROWSRepository
{
    public ACC_PLAFOND_ROWSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_PLAFOND_ROWS>? GetList(string CompanyID, int CustomerID, int Year, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_PLAFOND_ROWS>(
                @"SELECT p.*, f.FTNUFD AS InvoiceDefinitiveNumber FROM ACC_PLAFOND_ROWS AS p
                        INNER JOIN FATTT00F AS f ON f.ftsoci=p.cliasoc AND f.FTANNO=p.cliannosol AND f.FTNUOR=p.clinumfat
                        WHERE p.cliasoc=@cid AND p.cliacod=@cuid AND p.cliannosol=@yea AND p.cliprog = @id
                        ORDER BY p.clidatfatt DESC, p.clinumfat DESC",
                new { cid = CompanyID, cuid = CustomerID, yea = Year, id = ID });

            return new ObservableCollection<ACC_PLAFOND_ROWS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND_ROWS? Get(string Cliasoc, int Cliacod, int cliannosol, int cliprog, int clinumfat)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND_ROWS>(
                "SELECT * FROM ACC_PLAFOND_ROWS WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog, clinumfat = clinumfat })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string Cliasoc, int Cliacod, int cliannosol, int cliprog, int clinumfat)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_PLAFOND_ROWS WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog, clinumfat = clinumfat }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public string? GetInvoiceArchived(string CompanyID, int CustomerID, short Year, int DefinitiveID)
    {
        throw new NotImplementedException();
    }

    #region CRUD
    // remove [rv]
    public string INSERT_QUERY => "INSERT INTO ACC_PLAFOND_ROWS (Cliasoc,Cliacod,cliannosol,cliprog,clinumfat,cliimpimpo,cliimpplaf,clidatfatt) VALUES(@Cliasoc,@Cliacod,@cliannosol,@cliprog,@clinumfat,@cliimpimpo,@cliimpplaf,@clidatfatt)";
    public string UPDATE_QUERY => "UPDATE ACC_PLAFOND_ROWS SET Cliasoc = @Cliasoc,Cliacod = @Cliacod,cliannosol = @cliannosol,cliprog = @cliprog,clinumfat = @clinumfat,cliimpimpo = @cliimpimpo,cliimpplaf = @cliimpplaf,clidatfatt = @clidatfatt WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat";
    public string DELETE_QUERY => "DELETE FROM ACC_PLAFOND_ROWS WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat";
    public bool Insert(ACC_PLAFOND_ROWS Model)
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

    public bool Update(ACC_PLAFOND_ROWS Model)
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

    public bool Delete(ACC_PLAFOND_ROWS Model)
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

    public string? Validate(ACC_PLAFOND_ROWS Model, bool IsInsert)
    {
        try
        {
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class ACC_PLAFOND_ROWSUfpRepository : RepositoryBase, IACC_PLAFOND_ROWSRepository
{
    public ACC_PLAFOND_ROWSUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_PLAFOND_ROWS>? GetList(string CompanyID, int CustomerID, int Year, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_PLAFOND_ROWS>(
                @"SELECT p.*, f.FTNUFD AS InvoiceDefinitiveNumber,
f.FTTIPO as InvoiceTypeID
FROM CLIAMMIP2 AS p
                        INNER JOIN FATTT00F AS f ON f.ftsoci=p.cliasoc AND f.FTANNO=p.cliannosol AND f.FTNUOR=p.clinumfat
                        WHERE p.cliasoc=@cid AND p.cliacod=@cuid AND p.cliannosol=@yea AND p.cliprog = @id
                        ORDER BY p.clidatfatt DESC, p.clinumfat DESC",
                new { cid = CompanyID, cuid = CustomerID, yea = Year, id = ID });

            return new ObservableCollection<ACC_PLAFOND_ROWS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND_ROWS? Get(string Cliasoc, int Cliacod, int cliannosol, int cliprog, int clinumfat)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND_ROWS>(
                "SELECT * FROM CLIAMMIP2 WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog, clinumfat = clinumfat })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string Cliasoc, int Cliacod, int cliannosol, int cliprog, int clinumfat)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CLIAMMIP2 WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog, clinumfat = clinumfat }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public string? GetInvoiceArchived(string CompanyID, int CustomerID, short Year, int DefinitiveID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (string?)connection.ExecuteScalar(
                "SELECT arcfile FROM ARCCLI WHERE Arcsoc = @companyID AND arccli = @customerID AND arcanno = @year AND arcrig = @definitiveID AND arctip = 'F'",
                new { companyID = CompanyID, customerID = CustomerID, year = Year, definitiveID = DefinitiveID });

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    // remove [rv]
    public string INSERT_QUERY => "INSERT INTO CLIAMMIP2 (Cliasoc,Cliacod,cliannosol,cliprog,clinumfat,cliimpimpo,cliimpplaf,clidatfatt) VALUES(@Cliasoc,@Cliacod,@cliannosol,@cliprog,@clinumfat,@cliimpimpo,@cliimpplaf,@clidatfatt)";
    public string UPDATE_QUERY => "UPDATE CLIAMMIP2 SET Cliasoc = @Cliasoc,Cliacod = @Cliacod,cliannosol = @cliannosol,cliprog = @cliprog,clinumfat = @clinumfat,cliimpimpo = @cliimpimpo,cliimpplaf = @cliimpplaf,clidatfatt = @clidatfatt WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat";
    public string DELETE_QUERY => "DELETE FROM CLIAMMIP2 WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND clinumfat = @clinumfat";
    public bool Insert(ACC_PLAFOND_ROWS Model)
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

    public bool Update(ACC_PLAFOND_ROWS Model)
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

    public bool Delete(ACC_PLAFOND_ROWS Model)
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

    public string? Validate(ACC_PLAFOND_ROWS Model, bool IsInsert)
    {
        try
        {
            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}