using VulpesX.DAL;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.Accounting;

public interface IACC_PLAFONDRepository
{
    ObservableCollection<ACC_PLAFOND>? GetList(string CompanyID, int Year);

    ACC_PLAFOND? Get(string Cliasoc, int Cliacod, int cliannosol, int cliprog);

    ACC_PLAFOND? GetLast(string Cliasoc, int Cliacod, int cliannosol, DateTime InvoiceDate, bool IsRegenerate);

    int GetLastProgressive(string Cliasoc, int Cliacod, int cliannosol);

    bool Exists(string Cliasoc, int Cliacod, int cliannosol, int cliprog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_PLAFOND Model);

    bool Update(ACC_PLAFOND Model);

    bool Delete(ACC_PLAFOND Model);

    string? Validate(ACC_PLAFOND Model, bool IsInsert);
    #endregion
}

public class ACC_PLAFONDRepository : RepositoryBase, IACC_PLAFONDRepository
{
    public ACC_PLAFONDRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_PLAFOND>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();

    
                var list = connection.Query<ACC_PLAFOND, ABE, ACC_PLAFOND>(
                    @"SELECT p.*, a.abecod, a.abers1, a.abers2 FROM ACC_PLAFOND AS p
                        INNER JOIN ABE AS a ON a.abecod = p.Cliacod
                        WHERE p.Cliasoc = @cid AND p.Cliannosol = @yea AND p.canceled IS NULL",
                    (pla, abe) => { pla.Customer = abe; return pla; },
                    new { cid = CompanyID, yea = Year }, splitOn: "abecod");

                return new ObservableCollection<ACC_PLAFOND>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND? Get(string Cliasoc, int Cliacod, int cliannosol, int cliprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND>(
                "SELECT * FROM ACC_PLAFOND WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND? GetLast(string Cliasoc, int Cliacod, int cliannosol, DateTime InvoiceDate, bool IsRegenerate)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND>(
                $@"SELECT * FROM ACC_PLAFOND WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol 
                        {(IsRegenerate ? null : " AND clidatchi IS NULL")}
                        ",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, invoicedate = InvoiceDate })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public int GetLastProgressive(string Cliasoc, int Cliacod, int cliannosol)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<int?>(
                @"SELECT MAX(cliprog) FROM ACC_PLAFOND 
                        WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol })
                .FirstOrDefault() ?? 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public bool Exists(string Cliasoc, int Cliacod, int cliannosol, int cliprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_PLAFOND WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_PLAFOND (Cliasoc,Cliacod,cliannosol,cliprog,clidatchi,cliimpfattprog,cliimpesefino,cliimpfattprovv,clinumprotuffiva,clidatuffiva,clistart,clinote,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@Cliasoc,@Cliacod,@cliannosol,@cliprog,@clidatchi,@cliimpfattprog,@cliimpesefino,@cliimpfattprovv,@clinumprotuffiva,@clidatuffiva,@clistart,@clinote,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ACC_PLAFOND SET Cliasoc = @Cliasoc,Cliacod = @Cliacod,cliannosol = @cliannosol,cliprog = @cliprog,clidatchi = @clidatchi,cliimpfattprog = @cliimpfattprog,cliimpesefino = @cliimpesefino,cliimpfattprovv = @cliimpfattprovv,clinumprotuffiva = @clinumprotuffiva,clidatuffiva = @clidatuffiva,clistart = @clistart,clinote = @clinote,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_PLAFOND OUTPUT DELETED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND rv = @rv";
    public bool Insert(ACC_PLAFOND Model)
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

    public bool Update(ACC_PLAFOND Model)
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

    public bool Delete(ACC_PLAFOND Model)
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

    public string? Validate(ACC_PLAFOND Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.Cliasoc) && IsInsert && !Exists(Model.Cliasoc, Model.Cliacod, Model.cliannosol, Model.cliprog)) || !IsInsert)
            {
                if (Model.Cliacod > 0)
                {
                    if (Model.cliannosol > 0)
                    {
                        return null;
                    }
                    else
                    { return "L'anno è obbligatorio"; }
                }
                else
                { return "Il codice cliente è obbligatorio"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class ACC_PLAFONDUfpRepository : RepositoryBase, IACC_PLAFONDRepository
{
    public ACC_PLAFONDUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_PLAFOND>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_PLAFOND, ABE, ACC_PLAFOND>(
                @"SELECT p.*, p.clianote as clinote, a.abecod, a.abers1, a.abers2 FROM CLIAMMIP1 AS p
                        INNER JOIN ANAG_BASE AS a ON a.abecod = p.Cliacod
                        WHERE p.Cliasoc = @cid AND p.Cliannosol = @yea AND p.canceled IS NULL",
                (pla, abe) => { pla.Customer = abe; return pla; },
                new { cid = CompanyID, yea = Year }, splitOn: "abecod");

            return new ObservableCollection<ACC_PLAFOND>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND? Get(string Cliasoc, int Cliacod, int cliannosol, int cliprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND>(
                "SELECT *, clianote as clinote, FROM CLIAMMIP1 WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND? GetLast(string Cliasoc, int Cliacod, int cliannosol, DateTime InvoiceDate, bool IsRegenerate)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND>(
                $@"SELECT *, clianote as clinote, FROM CLIAMMIP1 WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol 
                        {(IsRegenerate ? null : " AND clidatchi IS NULL")}
                        ",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, invoicedate = InvoiceDate })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public int GetLastProgressive(string Cliasoc, int Cliacod, int cliannosol)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<int?>(
                @"SELECT MAX(cliprog) FROM CLIAMMIP1 
                        WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol })
                .FirstOrDefault() ?? 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public bool Exists(string Cliasoc, int Cliacod, int cliannosol, int cliprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CLIAMMIP1 WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog",
                new { Cliasoc = Cliasoc, Cliacod = Cliacod, cliannosol = cliannosol, cliprog = cliprog }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CLIAMMIP1 (Cliasoc,Cliacod,cliannosol,cliprog,clidatchi,cliimpfattprog,cliimpesefino,cliimpfattprovv,clinumprotuffiva,clidatuffiva,clistart,clianote,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@Cliasoc,@Cliacod,@cliannosol,@cliprog,@clidatchi,@cliimpfattprog,@cliimpesefino,@cliimpfattprovv,@clinumprotuffiva,@clidatuffiva,@clistart,@clinote,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE CLIAMMIP1 SET Cliasoc = @Cliasoc,Cliacod = @Cliacod,cliannosol = @cliannosol,cliprog = @cliprog,clidatchi = @clidatchi,cliimpfattprog = @cliimpfattprog,cliimpesefino = @cliimpesefino,cliimpfattprovv = @cliimpfattprovv,clinumprotuffiva = @clinumprotuffiva,clidatuffiva = @clidatuffiva,clistart = @clistart,clianote = @clinote,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CLIAMMIP1 OUTPUT DELETED.rv WHERE Cliasoc = @Cliasoc AND Cliacod = @Cliacod AND cliannosol = @cliannosol AND cliprog = @cliprog AND rv = @rv";
    public bool Insert(ACC_PLAFOND Model)
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

    public bool Update(ACC_PLAFOND Model)
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

    public bool Delete(ACC_PLAFOND Model)
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

    public string? Validate(ACC_PLAFOND Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.Cliasoc) && IsInsert && !Exists(Model.Cliasoc, Model.Cliacod, Model.cliannosol, Model.cliprog)) || !IsInsert)
            {
                if (Model.Cliacod > 0)
                {
                    if (Model.cliannosol > 0)
                    {
                        return null;
                    }
                    else
                    { return "L'anno è obbligatorio"; }
                }
                else
                { return "Il codice cliente è obbligatorio"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}