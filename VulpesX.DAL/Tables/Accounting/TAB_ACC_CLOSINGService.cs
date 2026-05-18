using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface ITAB_ACC_CLOSINGRepository
{
    ObservableCollection<TAB_ACC_CLOSING>? GetList();

    TAB_ACC_CLOSING? Get(string cchcod);

    bool Exists(string cchcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_ACC_CLOSING Model);

    bool Update(TAB_ACC_CLOSING Model);

    bool Delete(TAB_ACC_CLOSING Model);

    string? Validate(TAB_ACC_CLOSING Model, bool IsInsert);
    #endregion
}

public class TAB_ACC_CLOSINGRepository : RepositoryBase, ITAB_ACC_CLOSINGRepository
{
    public TAB_ACC_CLOSINGRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_ACC_CLOSING>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_ACC_CLOSING>(
                "SELECT * FROM TAB_ACC_CLOSING");

            return new ObservableCollection<TAB_ACC_CLOSING>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_ACC_CLOSING? Get(string cchcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_ACC_CLOSING>(
                "SELECT * FROM TAB_ACC_CLOSING WHERE cchcod = @cchcod",
                new { cchcod = cchcod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cchcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_ACC_CLOSING WHERE cchcod = @cchcod",
                new { cchcod = cchcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_ACC_CLOSING (cchcod,cchdes,cchchi,cchria,cchpes,cchues,cchppr,cchgrc,cchctc,cchstc,cchgrr,cchctr,cchstr,cchgrp,cchctp,cchstp,cchgru,cchctu,cchstu,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@cchcod,@cchdes,@cchchi,@cchria,@cchpes,@cchues,@cchppr,@cchgrc,@cchctc,@cchstc,@cchgrr,@cchctr,@cchstr,@cchgrp,@cchctp,@cchstp,@cchgru,@cchctu,@cchstu,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE TAB_ACC_CLOSING SET cchcod = @cchcod,cchdes = @cchdes,cchchi = @cchchi,cchria = @cchria,cchpes = @cchpes,cchues = @cchues,cchppr = @cchppr,cchgrc = @cchgrc,cchctc = @cchctc,cchstc = @cchstc,cchgrr = @cchgrr,cchctr = @cchctr,cchstr = @cchstr,cchgrp = @cchgrp,cchctp = @cchctp,cchstp = @cchstp,cchgru = @cchgru,cchctu = @cchctu,cchstu = @cchstu,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE cchcod = @cchcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_ACC_CLOSING OUTPUT DELETED.rv WHERE cchcod = @cchcod AND rv = @rv";
    public bool Insert(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    // always set gain/loss flag inverted 
                    var flag = Model.cchppr == "S" ? "N" : "S";
                    connection.Execute(@"UPDATE TAB_ACC_CLOSING
                                                SET cchppr = @flag
                                                WHERE cchcod <> @id", new { id = Model.cchcod, flag = flag }, transaction);
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(UPDATE_QUERY, Model, transaction);
                    // always set gain/loss flag inverted 
                    var flag = Model.cchppr == "S" ? "N" : "S";
                    connection.Execute(@"UPDATE TAB_ACC_CLOSING
                                                SET cchppr = @flag
                                                WHERE cchcod <> @id", new { id = Model.cchcod, flag = flag }, transaction);
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(TAB_ACC_CLOSING Model)
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

    public string? Validate(TAB_ACC_CLOSING Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cchcod) && IsInsert && !Exists(Model.cchcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.cchdes))
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

public class TAB_ACC_CLOSINGUfpRepository : RepositoryBase, ITAB_ACC_CLOSINGRepository
{
    public TAB_ACC_CLOSINGUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_ACC_CLOSING>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_ACC_CLOSING>(
                "SELECT * FROM CODCHIUSURA");

            return new ObservableCollection<TAB_ACC_CLOSING>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_ACC_CLOSING? Get(string cchcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_ACC_CLOSING>(
                "SELECT * FROM CODCHIUSURA WHERE cchcod = @cchcod",
                new { cchcod = cchcod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cchcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CODCHIUSURA WHERE cchcod = @cchcod",
                new { cchcod = cchcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CODCHIUSURA (cchcod,cchdes,cchchi,cchria,cchpes,cchues,cchppr,cchgrc,cchctc,cchstc,cchgrr,cchctr,cchstr,cchgrp,cchctp,cchstp,cchgru,cchctu,cchstu) OUTPUT INSERTED.rv VALUES(@cchcod,@cchdes,@cchchi,@cchria,@cchpes,@cchues,@cchppr,@cchgrc,@cchctc,@cchstc,@cchgrr,@cchctr,@cchstr,@cchgrp,@cchctp,@cchstp,@cchgru,@cchctu,@cchstu)";
    public string UPDATE_QUERY => "UPDATE CODCHIUSURA SET cchcod = @cchcod,cchdes = @cchdes,cchchi = @cchchi,cchria = @cchria,cchpes = @cchpes,cchues = @cchues,cchppr = @cchppr,cchgrc = @cchgrc,cchctc = @cchctc,cchstc = @cchstc,cchgrr = @cchgrr,cchctr = @cchctr,cchstr = @cchstr,cchgrp = @cchgrp,cchctp = @cchctp,cchstp = @cchstp,cchgru = @cchgru,cchctu = @cchctu,cchstu = @cchstu OUTPUT INSERTED.rv WHERE cchcod = @cchcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CODCHIUSURA OUTPUT DELETED.rv WHERE cchcod = @cchcod AND rv = @rv";
    public bool Insert(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    // always set gain/loss flag inverted 
                    var flag = Model.cchppr == "S" ? "N" : "S";
                    connection.Execute(@"UPDATE TAB_ACC_CLOSING
                                                SET cchppr = @flag
                                                WHERE cchcod <> @id", new { id = Model.cchcod, flag = flag }, transaction);
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(UPDATE_QUERY, Model, transaction);
                    // always set gain/loss flag inverted 
                    var flag = Model.cchppr == "S" ? "N" : "S";
                    connection.Execute(@"UPDATE CODCHIUSURA
                                                SET cchppr = @flag
                                                WHERE cchcod <> @id", new { id = Model.cchcod, flag = flag }, transaction);
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message.ToString());
                    return false;
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(TAB_ACC_CLOSING Model)
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

    public string? Validate(TAB_ACC_CLOSING Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cchcod) && IsInsert && !Exists(Model.cchcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.cchdes))
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