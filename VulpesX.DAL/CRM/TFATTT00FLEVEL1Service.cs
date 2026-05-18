namespace VulpesX.DAL.CRM;

public interface ITFATTT00FLEVEL1Repository
{
    ObservableCollection<TFATTT00FLEVEL1>? GetList();

    TFATTT00FLEVEL1? Get(string ftsoci, int FTANNO, int FTNUOR, DateTime ftdataora);

    bool Exists(string ftsoci, int FTANNO, int FTNUOR, DateTime ftdataora);

    #region CRUD
    bool Insert(TFATTT00FLEVEL1 Model);

    bool Update(TFATTT00FLEVEL1 Model);

    bool Delete(TFATTT00FLEVEL1 Model);

    string? Validate(TFATTT00FLEVEL1 Model, bool IsInsert);
    #endregion
}

public class TFATTT00FLEVEL1Repository : RepositoryBase, ITFATTT00FLEVEL1Repository
{
    public TFATTT00FLEVEL1Repository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TFATTT00FLEVEL1>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TFATTT00FLEVEL1>(
                "SELECT * FROM TFATTT00FLEVEL1");

            return new ObservableCollection<TFATTT00FLEVEL1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TFATTT00FLEVEL1? Get(string ftsoci, int FTANNO, int FTNUOR, DateTime ftdataora)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TFATTT00FLEVEL1>(
                "SELECT * FROM TFATTT00FLEVEL1 WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND ftdataora = @ftdataora",
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR, ftdataora = ftdataora })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ftsoci, int FTANNO, int FTNUOR, DateTime ftdataora)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TFATTT00FLEVEL1 WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND ftdataora = @ftdataora",
                new { ftsoci = ftsoci, FTANNO = FTANNO, FTNUOR = FTNUOR, ftdataora = ftdataora }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(TFATTT00FLEVEL1 Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO TFATTT00FLEVEL1 (ftsoci,FTANNO,FTNUOR,ftdataora,ftsdid,ftfilename,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) VALUES(@ftsoci,@FTANNO,@FTNUOR,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@ftsdid,@ftfilename,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)",
                Model);
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

    public bool Update(TFATTT00FLEVEL1 Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE TFATTT00FLEVEL1 SET ftsoci = @ftsoci,FTANNO = @FTANNO,FTNUOR = @FTNUOR,ftdataora = @ftdataora,ftsdid = @ftsdid,ftfilename = @ftfilename,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND ftdataora = @ftdataora AND rv = @rv",
                Model);
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

    public bool Delete(TFATTT00FLEVEL1 Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM TFATTT00FLEVEL1 OUTPUT DELETED.rv WHERE ftsoci = @ftsoci AND FTANNO = @FTANNO AND FTNUOR = @FTNUOR AND ftdataora = @ftdataora AND rv = @rv",
                Model);
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

    public string? Validate(TFATTT00FLEVEL1 Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.ftsoci) && IsInsert && !Exists(Model.ftsoci, Model.FTANNO, Model.FTNUOR, Model.ftdataora)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.ftfilename))
                {
                    if (!string.IsNullOrWhiteSpace(Model.ftsdid))
                    {
                        return null;
                    }
                    else
                    { return "Il codice SDI è obbligatorio"; }
                }
                else
                { return "Il nome del file è obbligatorio"; }
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