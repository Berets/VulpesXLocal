namespace VulpesX.DAL.CRM;

public interface IANFADD00FRepository
{
    ObservableCollection<ANFADD00F>? GetList(string CompanyID, int Year, int Number, int RowID);

    ANFADD00F? Get(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA, int AFDDPROG);

    bool Exists(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA, int AFDDPROG);

    bool Insert(ANFADD00F Model);

    bool Update(ANFADD00F Model);

    bool Delete(ANFADD00F Model);

    string? Validate(ANFADD00F Model, bool IsInsert);
}

public class ANFADD00FRepository : RepositoryBase, IANFADD00FRepository
{
    public ANFADD00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ANFADD00F>? GetList(string CompanyID, int Year, int Number, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ANFADD00F>(
                @"SELECT * FROM ANFADD00F
                        WHERE AFSOCI=@cid AND AFANNO=@yea AND AFNUOR=@num AND AFDRIGA = @roi
                        ORDER BY AFDDPROG",
                new { cid = CompanyID, yea = Year, num = Number, roi = RowID });

            return new ObservableCollection<ANFADD00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ANFADD00F? Get(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA, int AFDDPROG)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.QueryFirstOrDefault<ANFADD00F>(
                "SELECT * FROM ANFADD00F WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA AND AFDDPROG = @AFDDPROG",
                new { AFSOCI, AFANNO, AFNUOR, AFDRIGA, AFDDPROG });
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string AFSOCI, int AFANNO, int AFNUOR, int AFDRIGA, int AFDDPROG)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANFADD00F WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA AND AFDDPROG = @AFDDPROG",
                new { AFSOCI, AFANNO, AFNUOR, AFDRIGA, AFDDPROG }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ANFADD00F (AFSOCI,AFANNO,AFNUOR,AFDRIGA,AFDDPROG,AFDDTIPO,AFDDDESC,AFDDCODA,AFDDQTAV,AFDDCOST,AFDDNOTE) OUTPUT INSERTED.rv VALUES(@AFSOCI,@AFANNO,@AFNUOR,@AFDRIGA,@AFDDPROG,@AFDDTIPO,@AFDDDESC,@AFDDCODA,@AFDDQTAV,@AFDDCOST,@AFDDNOTE)";
    public readonly string UPDATE_QUERY = "UPDATE ANFADD00F SET AFSOCI = @AFSOCI,AFANNO = @AFANNO,AFNUOR = @AFNUOR,AFDRIGA = @AFDRIGA,AFDDPROG = @AFDDPROG,AFDDTIPO = @AFDDTIPO,AFDDDESC = @AFDDDESC,AFDDCODA = @AFDDCODA,AFDDQTAV = @AFDDQTAV,AFDDCOST = @AFDDCOST,AFDDNOTE = @AFDDNOTE OUTPUT INSERTED.rv WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA AND AFDDPROG = @AFDDPROG AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ANFADD00F OUTPUT DELETED.rv WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND AFDRIGA = @AFDRIGA AND AFDDPROG = @AFDDPROG AND rv = @rv";
    public bool Insert(ANFADD00F Model)
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

    public bool Update(ANFADD00F Model)
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

    public bool Delete(ANFADD00F Model)
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

    public string? Validate(ANFADD00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.AFSOCI))
            {
                if (!string.IsNullOrWhiteSpace(Model.AFDDTIPO))
                {
                    if (!string.IsNullOrWhiteSpace(Model.AFDDDESC) || !string.IsNullOrWhiteSpace(Model.AFDDCODA))
                    {
                        if (Model.AFDDCOST > 0)
                        {
                            if (Model.AFDDQTAV > 0)
                            {
                                return null;
                            }
                            else
                            {
                                return "La quantita' del componente e' obbligatoria";
                            }
                        }
                        else
                        {
                            return "Il costo del componente e' obbligatorio";
                        }
                    }
                    else
                    { return "La descrizione o un codice articolo sono obbligatori"; }
                }
                else
                { return "Il tipo del componente e' obbligatorio"; }
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