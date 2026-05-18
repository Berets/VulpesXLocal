using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface IPNCEEBILRepository
{
    ObservableCollection<PNCEEBIL>? GetList();

    PNCEEBIL? Get(string ID1, string ID2, string ID3, string ID4, string ID5, string ID6, string ID7);

    #region CRUD
    bool Insert(PNCEEBIL Model);

    bool Update(PNCEEBIL Model);

    bool Delete(PNCEEBIL Model);

    string? Validate(PNCEEBIL Model, bool IsInsert);
    #endregion
}

public class PNCEEBILRepository : RepositoryBase, IPNCEEBILRepository
{
    public PNCEEBILRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNCEEBIL>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNCEEBIL>(
                "SELECT * FROM PNCEEBIL");

            return new ObservableCollection<PNCEEBIL>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNCEEBIL? Get(string ID1, string ID2, string ID3, string ID4, string ID5, string ID6, string ID7)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNCEEBIL>(
                "SELECT * FROM PNCEEBIL WHERE ceevo1 = @ceevo1 AND ceevo2 = @ceevo2 AND ceevo3 = @ceevo3 AND ceevo4 = @ceevo4 AND ceevo5 = @ceevo5 AND ceevo6 = @ceevo6 AND ceevo7 = @ceevo7",
                new { ceevo1 = ID1, ceevo2 = ID2, ceevo3 = ID3, ceevo4 = ID4, ceevo5 = ID5, ceevo6 = ID6, ceevo7 = ID7 })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public bool Insert(PNCEEBIL Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO PNCEEBIL (ceevo1,ceevo2,ceevo3,ceevo4,ceevo5,ceevo6,ceevo7,ceedes,ceedee,ceesal,ceesam,ceesas,ceespl,ceespm,ceesps,ceeord,ceevosom1,ceevosom2,ceevosom3,ceevosom4) OUTPUT INSERTED.rv VALUES(@ceevo1,@ceevo2,@ceevo3,@ceevo4,@ceevo5,@ceevo6,@ceevo7,@ceedes,@ceedee,@ceesal,@ceesam,@ceesas,@ceespl,@ceespm,@ceesps,@ceeord,@ceevosom1,@ceevosom2,@ceevosom3,@ceevosom4)",
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

    public bool Update(PNCEEBIL Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE PNCEEBIL SET ceevo1 = @ceevo1,ceevo2 = @ceevo2,ceevo3 = @ceevo3,ceevo4 = @ceevo4,ceevo5 = @ceevo5,ceevo6 = @ceevo6,ceevo7 = @ceevo7,ceedes = @ceedes,ceedee = @ceedee,ceesal = @ceesal,ceesam = @ceesam,ceesas = @ceesas,ceespl = @ceespl,ceespm = @ceespm,ceesps = @ceesps,ceeord = @ceeord,ceevosom1 = @ceevosom1,ceevosom2 = @ceevosom2,ceevosom3 = @ceevosom3,ceevosom4 = @ceevosom4 OUTPUT INSERTED.rv WHERE ceevo1 = @ceevo1 AND ceevo2 = @ceevo2 AND ceevo3 = @ceevo3 AND ceevo4 = @ceevo4 AND ceevo5 = @ceevo5 AND ceevo6 = @ceevo6 AND ceevo7 = @ceevo7 AND rv = @rv",
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

    public bool Delete(PNCEEBIL Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM PNCEEBIL OUTPUT DELETED.rv WHERE ceevo1 = @ceevo1 AND ceevo2 = @ceevo2 AND ceevo3 = @ceevo3 AND ceevo4 = @ceevo4 AND ceevo5 = @ceevo5 AND ceevo6 = @ceevo6 AND ceevo7 = @ceevo7 AND rv = @rv",
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

    public string? Validate(PNCEEBIL Model, bool IsInsert)
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
    #endregion
}