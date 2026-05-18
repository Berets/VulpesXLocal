using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface IESERCIZIORepository
{
    ObservableCollection<ESERCIZIO>? GetList(string CompanyID);

    ObservableCollection<ESERCIZIO>? GetListOpen(string CompanyID);

    ESERCIZIO? Get(string CompanyID, int Year);

    bool Exists(string CompanyID, int Year);

    #region CRUD
    public string INSERT_QUERY { get; }
    public string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ESERCIZIO Model);

    bool Update(ESERCIZIO Model);

    bool Delete(ESERCIZIO Model);

    string? Validate(ESERCIZIO Model, bool IsInsert);
    #endregion
}

public class ESERCIZIORepository : RepositoryBase, IESERCIZIORepository
{
    public ESERCIZIORepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ESERCIZIO>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ESERCIZIO>(
                "SELECT * FROM ESERCIZIO WHERE esesoc = @cid ORDER BY eseann DESC",
                new { cid = CompanyID });

            return new ObservableCollection<ESERCIZIO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ESERCIZIO>? GetListOpen(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ESERCIZIO>(
                @"SELECT * FROM ESERCIZIO 
                        WHERE esesoc = @cid AND (eseest = 'A' OR eseest = 'U') 
                        ORDER BY eseest DESC",
                new { cid = CompanyID });

            return new ObservableCollection<ESERCIZIO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ESERCIZIO? Get(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ESERCIZIO>(
                "SELECT * FROM ESERCIZIO WHERE esesoc = @cid AND eseann = @eseann",
                new { cid = CompanyID, eseann = Year })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ESERCIZIO WHERE esesoc = @cid AND eseann = @eseann",
                new { cid = CompanyID, eseann = Year }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ESERCIZIO (esesoc,eseann,eseulr,esedar,eseave,eseest,eseist,esereg,eserpr,eseusg,eseusm,eselvi,eselai,eselso,eselve,eselae,eseuch,eseflg,eseini,esepag,eseagg,esecom,esesco,esepli,esepre,esedap,eseliq,eseivaven,esedtfniva,esedtiniva,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@esesoc,@eseann,@eseulr,@esedar,@eseave,@eseest,@eseist,@esereg,@eserpr,@eseusg,@eseusm,@eselvi,@eselai,@eselso,@eselve,@eselae,@eseuch,@eseflg,@eseini,@esepag,@eseagg,@esecom,@esesco,@esepli,@esepre,@esedap,@eseliq,@eseivaven,@esedtfniva,@esedtiniva,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE ESERCIZIO SET esesoc = @esesoc,eseann = @eseann,eseulr = @eseulr,esedar = @esedar,eseave = @eseave,eseest = @eseest,eseist = @eseist,esereg = @esereg,eserpr = @eserpr,eseusg = @eseusg,eseusm = @eseusm,eselvi = @eselvi,eselai = @eselai,eselso = @eselso,eselve = @eselve,eselae = @eselae,eseuch = @eseuch,eseflg = @eseflg,eseini = @eseini,esepag = @esepag,eseagg = @eseagg,esecom = @esecom,esesco = @esesco,esepli = @esepli,esepre = @esepre,esedap = @esedap,eseliq = @eseliq,eseivaven = @eseivaven,esedtfniva = @esedtfniva,esedtiniva = @esedtiniva,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE esesoc = @esesoc AND eseann = @eseann AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ESERCIZIO OUTPUT DELETED.rv WHERE esesoc = @esesoc AND eseann = @eseann AND rv = @rv";
    public bool Insert(ESERCIZIO Model)
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

    public bool Update(ESERCIZIO Model)
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

    public bool Delete(ESERCIZIO Model)
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

    public string? Validate(ESERCIZIO Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.esesoc) && Model.eseann > 0 && IsInsert && !Exists(Model.esesoc, Model.eseann)) || !IsInsert)
            {
                return null;
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

public class ESERCIZIOUfpRepository : RepositoryBase, IESERCIZIORepository
{
    public ESERCIZIOUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ESERCIZIO>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ESERCIZIO>(
                "SELECT * FROM ESERCIZIO WHERE esesoc = @cid ORDER BY eseann DESC",
                new { cid = CompanyID });

            return new ObservableCollection<ESERCIZIO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ESERCIZIO>? GetListOpen(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ESERCIZIO>(
                @"SELECT * FROM ESERCIZIO 
                        WHERE esesoc = @cid AND (eseest = 'A' OR eseest = 'U') 
                        ORDER BY eseest DESC",
                new { cid = CompanyID });

            return new ObservableCollection<ESERCIZIO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ESERCIZIO? Get(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ESERCIZIO>(
                "SELECT * FROM ESERCIZIO WHERE esesoc = @cid AND eseann = @eseann",
                new { cid = CompanyID, eseann = Year })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ESERCIZIO WHERE esesoc = @cid AND eseann = @eseann",
                new { cid = CompanyID, eseann = Year }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ESERCIZIO (esesoc,eseann,eseulr,esedar,eseave,eseest,eseist,esereg,eserpr,eseusg,eseusm,eselvi,eselai,eselso,eselve,eselae,eseuch,eseflg,eseini,esepag,eseagg,esecom,esesco,esepli,esepre,esedap,eseivaven,eseliq) OUTPUT INSERTED.rv VALUES(@esesoc,@eseann,@eseulr,@esedar,@eseave,@eseest,@eseist,@esereg,@eserpr,@eseusg,@eseusm,@eselvi,@eselai,@eselso,@eselve,@eselae,@eseuch,@eseflg,@eseini,@esepag,@eseagg,@esecom,@esesco,@esepli,@esepre,@esedap,@eseivaven,@eseliq)";
    public string UPDATE_QUERY => "UPDATE ESERCIZIO SET esesoc = @esesoc,eseann = @eseann,eseulr = @eseulr,esedar = @esedar,eseave = @eseave,eseest = @eseest,eseist = @eseist,esereg = @esereg,eserpr = @eserpr,eseusg = @eseusg,eseusm = @eseusm,eselvi = @eselvi,eselai = @eselai,eselso = @eselso,eselve = @eselve,eselae = @eselae,eseuch = @eseuch,eseflg = @eseflg,eseini = @eseini,esepag = @esepag,eseagg = @eseagg,esecom = @esecom,esesco = @esesco,esepli = @esepli,esepre = @esepre,esedap = @esedap,eseivaven = @eseivaven,eseliq = @eseliq OUTPUT INSERTED.rv WHERE esesoc = @esesoc AND eseann = @eseann AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ESERCIZIO OUTPUT DELETED.rv WHERE esesoc = @esesoc AND eseann = @eseann AND rv = @rv";
    public bool Insert(ESERCIZIO Model)
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

    public bool Update(ESERCIZIO Model)
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

    public bool Delete(ESERCIZIO Model)
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

    public string? Validate(ESERCIZIO Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.esesoc) && Model.eseann > 0 && IsInsert && !Exists(Model.esesoc, Model.eseann)) || !IsInsert)
            {
                return null;
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