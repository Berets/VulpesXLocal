namespace VulpesX.DAL.CRM;

public interface IORDITALERepository
{
    ObservableCollection<ORDITALE>? GetList(string otsoci, int OTANNO, int OTNUOR);

    ORDITALE? Get(string otsoci, int OTANNO, int OTNUOR, int OTAID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ORDITALE Model);

    bool Update(ORDITALE Model);

    bool UpdateAll(ORDIT00F Head, ObservableCollection<ORDITALE> ModelRows);

    bool Delete(ORDITALE Model);

    string? Validate(ORDITALE Model, bool IsInsert);
    #endregion
}

public class ORDITALERepository : RepositoryBase, IORDITALERepository
{
    public ORDITALERepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ORDITALE>? GetList(string otsoci, int OTANNO, int OTNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ORDITALE>(
                @"SELECT * FROM ORDITALE
                        WHERE otsoci=@otsoci AND OTANNO = @otanno AND OTNUOR = @otnuor",
                new { otsoci = otsoci, otanno = OTANNO, otnuor = OTNUOR });

            return new ObservableCollection<ORDITALE>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ORDITALE? Get(string otsoci, int OTANNO, int OTNUOR, int OTAID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ORDITALE>(
                "SELECT * FROM ORDITALE WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND OTAID = @OTAID",
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR, OTAID = OTAID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ORDITALE (otsoci,OTANNO,OTNUOR,OTAID,OTADESC,OTAATTI,added,updated,addedUserID,updatedUserID) OUTPUT INSERTED.rv VALUES(@otsoci,@OTANNO,@OTNUOR,@OTAID,@OTADESC,@OTAATTI,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@addedUserID,@updatedUserID)";
    public string UPDATE_QUERY => "UPDATE ORDITALE SET otsoci = @otsoci,OTANNO = @OTANNO,OTNUOR = @OTNUOR,OTAID = @OTAID,OTADESC = @OTADESC,OTAATTI = @OTAATTI,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',addedUserID = @addedUserID,updatedUserID = @updatedUserID OUTPUT INSERTED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND OTAID = @OTAID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ORDITALE OUTPUT DELETED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND OTAID = @OTAID AND rv = @rv";
    public bool Insert(ORDITALE Model)
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

    public bool Update(ORDITALE Model)
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

    public bool UpdateAll(ORDIT00F Head, ObservableCollection<ORDITALE> ModelRows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // clear all
                connection.Execute("DELETE FROM ORDITALE WHERE otsoci = @otsoci AND OTANNO = @otanno AND OTNUOR = @otnuor",
                    new { otsoci = Head.otsoci, OTANNO = Head.OTANNO, otnuor = Head.OTNUOR }, transaction);
                // add all
                foreach (var item in ModelRows)
                {
                    connection.Execute(INSERT_QUERY, item, transaction);
                }
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
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

    public bool Delete(ORDITALE Model)
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

    public string? Validate(ORDITALE Model, bool IsInsert)
    {
        try
        {
            if (Model.OTAID > 0 && IsInsert || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.OTADESC))
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria"; }
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