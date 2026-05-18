using VulpesX.DAL;

namespace VulpesX.DAL.General;

public interface IRFFTB00FRepository
{
    ObservableCollection<RFFTB00F>? GetList(int FOCLIF);

    ObservableCollection<string>? GetSRMEmailList(int ID, NotifierHelper.SendClasses Class);

    RFFTB00F? Get(int FOCLIF, int rffrig);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(RFFTB00F Model);

    bool Update(RFFTB00F Model);

    bool Delete(RFFTB00F Model);

    string? Validate(RFFTB00F Model, bool IsInsert);
    #endregion
}

public class RFFTB00FRepository : RepositoryBase, IRFFTB00FRepository
{
    public RFFTB00FRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<RFFTB00F>? GetList(int FOCLIF)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<RFFTB00F>(
                @"SELECT * FROM RFFTB00F 
                        WHERE FOCLIF = @foclif
                        ORDER BY rffcgn, rffnom",
                new { foclif = FOCLIF });

            return new ObservableCollection<RFFTB00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<string>? GetSRMEmailList(int ID, NotifierHelper.SendClasses Class)
    {
        try
        {
            using var connection = GetOpenConnection();


            // compose filter
            string? filter = null;
            switch (Class)
            {
                case NotifierHelper.SendClasses.SRM_Purchase_Orders:
                    filter = " AND rffsendbuy = 1";
                    break;
            }

            var list = connection.Query<string>(
                $"SELECT rffmai FROM RFFTB00F WHERE FOCLIF = @id {filter}",
                new { id = ID });

            return new ObservableCollection<string>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public RFFTB00F? Get(int FOCLIF, int rffrig)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<RFFTB00F>(
                "SELECT * FROM RFFTB00F WHERE FOCLIF = @FOCLIF AND rffrig = @rffrig",
                new { FOCLIF = FOCLIF, rffrig = rffrig })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO RFFTB00F (FOCLIF,rffrig,rffcgn,rffnom,rffqal,rfftel,rfffax,rffcel,rffmai,rffsendbuy) OUTPUT INSERTED.rv VALUES(@FOCLIF,@rffrig,@rffcgn,@rffnom,@rffqal,@rfftel,@rfffax,@rffcel,@rffmai,@rffsendbuy)";
    public string UPDATE_QUERY => "UPDATE RFFTB00F SET FOCLIF = @FOCLIF,rffrig = @rffrig,rffcgn = @rffcgn,rffnom = @rffnom,rffqal = @rffqal,rfftel = @rfftel,rfffax = @rfffax,rffcel = @rffcel,rffmai = @rffmai,rffsendbuy = @rffsendbuy OUTPUT INSERTED.rv WHERE FOCLIF = @FOCLIF AND rffrig = @rffrig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM RFFTB00F OUTPUT DELETED.rv WHERE FOCLIF = @FOCLIF AND rffrig = @rffrig AND rv = @rv";

    public bool Insert(RFFTB00F Model)
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

    public bool Update(RFFTB00F Model)
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

    public bool Delete(RFFTB00F Model)
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

    public string? Validate(RFFTB00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.rffcgn))
            {
                if (!string.IsNullOrWhiteSpace(Model.rffnom))
                {
                    if ((!string.IsNullOrWhiteSpace(Model.rffmai) && NotifierHelper.CheckEmailAddress(Model.rffmai)) ||
                        Model.rffmai == null)
                    {
                        return null;
                    }
                    else
                    { return "Se presente, l'indirizzo email deve essere un indirizzo email valido"; }
                }
                else
                { return "Il nome è obbligatorio"; }
            }
            else
            { return "Il cognome è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class RFFTB00FUfpRepository : RepositoryBase, IRFFTB00FRepository
{
    public RFFTB00FUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<RFFTB00F>? GetList(int FOCLIF)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<RFFTB00F>(
                @"SELECT * FROM RFFTB00F 
                        WHERE FOCLIF = @foclif
                        ORDER BY rffcgn, rffnom",
                new { foclif = FOCLIF });

            return new ObservableCollection<RFFTB00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<string>? GetSRMEmailList(int ID, NotifierHelper.SendClasses Class)
    {
        try
        {
            using var connection = GetOpenConnection();


            // compose filter
            string? filter = null;
            switch (Class)
            {
                case NotifierHelper.SendClasses.SRM_Purchase_Orders:
                    filter = " AND rffsendbuy = 1";
                    break;
            }

            var list = connection.Query<string>(
                $"SELECT rffmai FROM RFFTB00F WHERE FOCLIF = @id {filter}",
                new { id = ID });

            return new ObservableCollection<string>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public RFFTB00F? Get(int FOCLIF, int rffrig)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<RFFTB00F>(
                "SELECT * FROM RFFTB00F WHERE FOCLIF = @FOCLIF AND rffrig = @rffrig",
                new { FOCLIF = FOCLIF, rffrig = rffrig })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO RFFTB00F (FOCLIF,rffrig,rffcgn,rffnom,rffqal,rfftel,rfffax,rffcel,rffmai,rffspa,rffSpedoffacq,rffSpedOffVend,rffSpedInfo) OUTPUT INSERTED.rv VALUES(@FOCLIF,@rffrig,@rffcgn,@rffnom,@rffqal,@rfftel,@rfffax,@rffcel,@rffmai,@rffspa,@rffSpedoffacq,@rffSpedOffVend,@rffSpedInfo)";
    public string UPDATE_QUERY => "UPDATE RFFTB00F SET FOCLIF = @FOCLIF,rffrig = @rffrig,rffcgn = @rffcgn,rffnom = @rffnom,rffqal = @rffqal,rfftel = @rfftel,rfffax = @rfffax,rffcel = @rffcel,rffmai = @rffmai,rffspa=@rffspa,rffSpedoffacq=@rffSpedoffacq,rffSpedOffVend=@rffSpedOffVend,rffSpedInfo=@rffSpedInfo OUTPUT INSERTED.rv WHERE FOCLIF = @FOCLIF AND rffrig = @rffrig AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM RFFTB00F OUTPUT DELETED.rv WHERE FOCLIF = @FOCLIF AND rffrig = @rffrig AND rv = @rv";

    public bool Insert(RFFTB00F Model)
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

    public bool Update(RFFTB00F Model)
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

    public bool Delete(RFFTB00F Model)
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

    public string? Validate(RFFTB00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.rffcgn))
            {
                if (!string.IsNullOrWhiteSpace(Model.rffnom))
                {
                    if ((!string.IsNullOrWhiteSpace(Model.rffmai) && NotifierHelper.CheckEmailAddress(Model.rffmai)) ||
                        Model.rffmai == null)
                    {
                        return null;
                    }
                    else
                    { return "Se presente, l'indirizzo email deve essere un indirizzo email valido"; }
                }
                else
                { return "Il nome è obbligatorio"; }
            }
            else
            { return "Il cognome è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}