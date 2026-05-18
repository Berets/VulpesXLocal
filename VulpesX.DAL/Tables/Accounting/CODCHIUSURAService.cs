using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface ICODCHIUSURARepository
{
    ObservableCollection<TAB_ACC_CLOSING>? GetList();

    TAB_ACC_CLOSING? Get(string ID);

    bool Insert(TAB_ACC_CLOSING Model);

    bool Update(TAB_ACC_CLOSING Model);

    bool Delete(TAB_ACC_CLOSING Model);

    string? Validate(TAB_ACC_CLOSING Model, bool IsInsert);
}

public class CODCHIUSURARepository : RepositoryBase, ICODCHIUSURARepository
{
    public CODCHIUSURARepository(IConnectionFactory factory) : base(factory)
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

    public TAB_ACC_CLOSING? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<TAB_ACC_CLOSING>(
                "SELECT * FROM TAB_ACC_CLOSING WHERE cchcod = @cchcod",
                new { cchcod = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Insert(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO TAB_ACC_CLOSING (cchcod,cchdes,cchchi,cchria,cchpes,cchues,cchppr,cchgrc,cchctc,cchstc,cchgrr,cchctr,cchstr,cchgrp,cchctp,cchstp,cchgru,cchctu,cchstu) OUTPUT INSERTED.rv VALUES(@cchcod,@cchdes,@cchchi,@cchria,@cchpes,@cchues,@cchppr,@cchgrc,@cchctc,@cchstc,@cchgrr,@cchctr,@cchstr,@cchgrp,@cchctp,@cchstp,@cchgru,@cchctu,@cchstu)",
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

    public bool Update(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE TAB_ACC_CLOSING SET cchcod = @cchcod,cchdes = @cchdes,cchchi = @cchchi,cchria = @cchria,cchpes = @cchpes,cchues = @cchues,cchppr = @cchppr,cchgrc = @cchgrc,cchctc = @cchctc,cchstc = @cchstc,cchgrr = @cchgrr,cchctr = @cchctr,cchstr = @cchstr,cchgrp = @cchgrp,cchctp = @cchctp,cchstp = @cchstp,cchgru = @cchgru,cchctu = @cchctu,cchstu = @cchstu OUTPUT INSERTED.rv WHERE ID = @cchcod AND rv = @rv",
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

    public bool Delete(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM TAB_ACC_CLOSING OUTPUT DELETED.rv WHERE ID = @cchcod AND rv = @rv",
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

    public string? Validate(TAB_ACC_CLOSING Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}

public class CODCHIUSURAUfpRepository : RepositoryBase, ICODCHIUSURARepository
{
    public CODCHIUSURAUfpRepository(IConnectionFactory factory) : base(factory)
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

    public TAB_ACC_CLOSING? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<TAB_ACC_CLOSING>(
                "SELECT * FROM CODCHIUSURA WHERE cchcod = @cchcod",
                new { cchcod = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Insert(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO CODCHIUSURA (cchcod,cchdes,cchchi,cchria,cchpes,cchues,cchppr,cchgrc,cchctc,cchstc,cchgrr,cchctr,cchstr,cchgrp,cchctp,cchstp,cchgru,cchctu,cchstu) OUTPUT INSERTED.rv VALUES(@cchcod,@cchdes,@cchchi,@cchria,@cchpes,@cchues,@cchppr,@cchgrc,@cchctc,@cchstc,@cchgrr,@cchctr,@cchstr,@cchgrp,@cchctp,@cchstp,@cchgru,@cchctu,@cchstu)",
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

    public bool Update(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE CODCHIUSURA SET cchcod = @cchcod,cchdes = @cchdes,cchchi = @cchchi,cchria = @cchria,cchpes = @cchpes,cchues = @cchues,cchppr = @cchppr,cchgrc = @cchgrc,cchctc = @cchctc,cchstc = @cchstc,cchgrr = @cchgrr,cchctr = @cchctr,cchstr = @cchstr,cchgrp = @cchgrp,cchctp = @cchctp,cchstp = @cchstp,cchgru = @cchgru,cchctu = @cchctu,cchstu = @cchstu OUTPUT INSERTED.rv WHERE ID = @cchcod AND rv = @rv",
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

    public bool Delete(TAB_ACC_CLOSING Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM CODCHIUSURA OUTPUT DELETED.rv WHERE ID = @cchcod AND rv = @rv",
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

    public string? Validate(TAB_ACC_CLOSING Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}