
namespace VulpesX.DAL.CRM;

public interface IANFAD00FRepository
{
    ObservableCollection<ANFAD00F>? GetList(string CompanyID, int Year, int Number);

    ANFAD00F? Get(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA);

    bool Exists(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA);

    bool Insert(ANFAD00F Model);

    bool Update(ANFAD00F Model);

    bool Delete(ANFAD00F Model);

    string? Validate(ANFAD00F Model, bool IsInsert);
}

public class ANFAD00FRepository : RepositoryBase, IANFAD00FRepository
{
    public ANFAD00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ANFAD00F>? GetList(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ANFAD00F>(
                @"SELECT * FROM ANFAD00F
                        WHERE AFSOCI=@cid AND AFANNO=@yea AND AFNUOR=@aid
                        ORDER BY AFDRIGA",
                new { cid = CompanyID, yea = Year, aid = Number });

            return new ObservableCollection<ANFAD00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ANFAD00F? Get(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.QueryFirstOrDefault<ANFAD00F>(
                "SELECT * FROM ANFAD00F WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA",
                new { AFSOCI, AFANNO, AFNUOR, AFDRIGA });
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANFAD00F WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA",
                new { AFSOCI, AFANNO, AFNUOR, AFDRIGA }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ANFAD00F (AFSOCI,AFANNO,AFNUOR,AFDRIGA,AFDCODA,AFDQTAV,AFDREDD,AFDTRED,AFDCOST,AFDPREZ,AFDNOTE,AFDSHOW,AFDUNIM,AFDTMAR) OUTPUT INSERTED.rv VALUES(@AFSOCI,@AFANNO,@AFNUOR,@AFDRIGA,@AFDCODA,@AFDQTAV,@AFDREDD,@AFDTRED,@AFDCOST,@AFDPREZ,@AFDNOTE,@AFDSHOW,@AFDUNIM,@AFDTMAR)";
    public readonly string UPDATE_QUERY = "UPDATE ANFAD00F SET AFSOCI = @AFSOCI,AFANNO = @AFANNO,AFNUOR = @AFNUOR,AFDRIGA = @AFDRIGA,AFDCODA = @AFDCODA,AFDQTAV = @AFDQTAV,AFDREDD = @AFDREDD,AFDTRED = @AFDTRED,AFDCOST = @AFDCOST,AFDPREZ = @AFDPREZ,AFDNOTE = @AFDNOTE,AFDSHOW = @AFDSHOW,AFDUNIM = @AFDUNIM,AFDTMAR = @AFDTMAR OUTPUT INSERTED.rv WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ANFAD00F OUTPUT DELETED.rv WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA AND rv = @rv";
    public bool Insert(ANFAD00F Model)
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

    public bool Update(ANFAD00F Model)
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

    public bool Delete(ANFAD00F Model)
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

    public string? Validate(ANFAD00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.AFSOCI))
            {
                if (!string.IsNullOrWhiteSpace(Model.AFDCODA))
                {
                    if (!string.IsNullOrWhiteSpace(Model.AFDUNIM))
                    {
                        if (Model.AFDQTAV > 0)
                        {
                            if (Model.AFDREDD.HasValue && Model.AFDREDD.Value > 0 && !string.IsNullOrWhiteSpace(Model.AFDTRED) ||
                                !Model.AFDREDD.HasValue && string.IsNullOrWhiteSpace(Model.AFDTRED))
                            {
                                return null;
                            }
                            else
                            { return "Se si specifica un margine è obbligatorio il suo tipo altrimenti devono essere entrambi vuoti"; }
                        }
                        else
                        { return "La quantita' è obbligatoria"; }
                    }
                    else
                    { return "L'unita' di misura è obbligatoria"; }
                }
                else
                { return "L'articolo è obbligatorio"; }
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