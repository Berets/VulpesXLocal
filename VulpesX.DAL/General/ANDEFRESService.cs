using VulpesX.DAL;

namespace VulpesX.DAL.General;


public interface IANDEFRESRepository
{
    ObservableCollection<ANDEFRES>? GetList(int ID);

    ObservableCollection<string>? GetCRMEmailList(int ID, NotifierHelper.SendClasses Class);

    ANDEFRES? Get(int CLIENT, int clirig);

    bool Exists(int CLIENT, int clirig);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ANDEFRES Model);

    bool Update(ANDEFRES Model);

    bool Delete(ANDEFRES Model);

    string? Validate(ANDEFRES Model, bool IsInsert);
    #endregion
}

public class ANDEFRESRepository : RepositoryBase, IANDEFRESRepository
{
    public ANDEFRESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ANDEFRES>? GetList(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ANDEFRES>(
                "SELECT * FROM ANDEFRES WHERE CLIENT = @id ORDER BY clirco, clirte",
                new { id = ID });

            return new ObservableCollection<ANDEFRES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<string>? GetCRMEmailList(int ID, NotifierHelper.SendClasses Class)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                // compose filter
                string? filter = null;
                switch (Class)
                {
                    case NotifierHelper.SendClasses.CRM_Offers:
                        filter = " AND clisendoff = 1";
                        break;
                    case NotifierHelper.SendClasses.CRM_Orders:
                        filter = " AND clisendord = 1";
                        break;
                    case NotifierHelper.SendClasses.CRM_DDT:
                        filter = " AND clisendddt = 1";
                        break;
                    case NotifierHelper.SendClasses.CRM_Invoices:
                        filter = " AND clisendinv = 1";
                        break;
                }


                var list = connection.Query<string>(
                    $"SELECT climco FROM ANDEFRES WHERE CLIENT = @id {filter}",
                    new { id = ID });

                return new ObservableCollection<string>(list);
            }
            else
            {
                ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                return null;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ANDEFRES? Get(int CLIENT, int clirig)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ANDEFRES>(
                "SELECT * FROM ANDEFRES WHERE CLIENT = @CLIENT AND clirig = @clirig",
                new { CLIENT = CLIENT, clirig = clirig })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int CLIENT, int clirig)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANDEFRES WHERE CLIENT = @CLIENT AND clirig = @clirig",
                new { CLIENT = CLIENT, clirig = clirig }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ANDEFRES (CLIENT,clirig,clirco,clirte,climco,clmte,rfctel,rfcfax,rfccel,clirifamm,clisendoff,clisendord,clisendddt,clisendinv) OUTPUT INSERTED.rv VALUES(@CLIENT,@clirig,@clirco,@clirte,@climco,@clmte,@rfctel,@rfcfax,@rfccel,@clirifamm,@clisendoff,@clisendord,@clisendddt,@clisendinv)";
    public string UPDATE_QUERY => "UPDATE ANDEFRES SET CLIENT = @CLIENT,clirig = @clirig,clirco = @clirco,clirte = @clirte,climco = @climco,clmte = @clmte,rfctel = @rfctel,rfcfax = @rfcfax,rfccel = @rfccel,clirifamm = @clirifamm,clisendoff = @clisendoff,clisendord = @clisendord,clisendddt = @clisendddt,clisendinv = @clisendinv OUTPUT INSERTED.rv WHERE CLIENT = @CLIENT AND clirig = @clirig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ANDEFRES OUTPUT DELETED.rv WHERE CLIENT = @CLIENT AND clirig = @clirig AND rv = @rv";
    public bool Insert(ANDEFRES Model)
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

    public bool Update(ANDEFRES Model)
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

    public bool Delete(ANDEFRES Model)
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

    public string? Validate(ANDEFRES Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.clirco) && !string.IsNullOrWhiteSpace(Model.clirte))
            {
                if ((!string.IsNullOrWhiteSpace(Model.climco) && NotifierHelper.CheckEmailAddress(Model.climco)) ||
                    Model.climco == null)
                {
                    return null;
                }
                else
                { return "Se presente, l'indirizzo email deve essere un indirizzo email valido"; }
            }
            else
            { return "Nome e cognome sono obbligatori"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class ANDEFRESUfpRepository : RepositoryBase, IANDEFRESRepository
{
    public ANDEFRESUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ANDEFRES>? GetList(int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ANDEFRES>(
                "SELECT * FROM ANDEFRES WHERE CLIENT = @id ORDER BY clirco, clirte",
                new { id = ID });

            return new ObservableCollection<ANDEFRES>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<string>? GetCRMEmailList(int ID, NotifierHelper.SendClasses Class)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                // compose filter
                string? filter = null;
                switch (Class)
                {
                    case NotifierHelper.SendClasses.CRM_Offers:
                        filter = " AND clisendoff = 1";
                        break;
                    case NotifierHelper.SendClasses.CRM_Orders:
                        filter = " AND clisendord = 1";
                        break;
                    case NotifierHelper.SendClasses.CRM_DDT:
                        filter = " AND clisendddt = 1";
                        break;
                    case NotifierHelper.SendClasses.CRM_Invoices:
                        filter = " AND clisendinv = 1";
                        break;
                }


                var list = connection.Query<string>(
                    $"SELECT climco FROM ANDEFRES WHERE CLIENT = @id {filter}",
                    new { id = ID });

                return new ObservableCollection<string>(list);
            }
            else
            {
                ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                return null;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ANDEFRES? Get(int CLIENT, int clirig)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ANDEFRES>(
                "SELECT * FROM ANDEFRES WHERE CLIENT = @CLIENT AND clirig = @clirig",
                new { CLIENT = CLIENT, clirig = clirig })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int CLIENT, int clirig)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANDEFRES WHERE CLIENT = @CLIENT AND clirig = @clirig",
                new { CLIENT = CLIENT, clirig = clirig }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ANDEFRES (CLIENT,clirig,clirco,clirte,climco,clmte,rfctel,rfcfax,rfccel,rfcspe,rfspf,SpedOffVend,SpedOrdAcq,SpedOffAcq,SpedInfo) OUTPUT INSERTED.rv VALUES(@CLIENT,@clirig,@clirco,@clirte,@climco,@clmte,@rfctel,@rfcfax,@rfccel,@rfcspe,@rfspf,@SpedOffVend,@SpedOrdAcq,@SpedOffAcq,@SpedInfo)";
    public string UPDATE_QUERY => "UPDATE ANDEFRES SET CLIENT = @CLIENT,clirig = @clirig,clirco = @clirco,clirte = @clirte,climco = @climco,clmte = @clmte,rfctel = @rfctel,rfcfax = @rfcfax,rfccel = @rfccel,rfcspe=@rfcspe,rfspf=@rfspf,SpedOffVend=@SpedOffVend,SpedOrdAcq =@SpedOrdAcq,SpedOffAcq=@SpedOffAcq,SpedInfo =@SpedInfo OUTPUT INSERTED.rv WHERE CLIENT = @CLIENT AND clirig = @clirig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ANDEFRES OUTPUT DELETED.rv WHERE CLIENT = @CLIENT AND clirig = @clirig AND rv = @rv";
    public bool Insert(ANDEFRES Model)
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

    public bool Update(ANDEFRES Model)
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

    public bool Delete(ANDEFRES Model)
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

    public string? Validate(ANDEFRES Model, bool IsInsert)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(Model.clirco) && string.IsNullOrWhiteSpace(Model.clirte))
            {
                return "Nome o cognome sono obbligatori";
            }
            if((!string.IsNullOrWhiteSpace(Model.climco) && !NotifierHelper.CheckEmailAddress(Model.climco)))
            {
                return "Se presente, l'indirizzo email deve essere un indirizzo email valido";
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}