using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;


public interface ICOMUNIRepository
{

    ObservableCollection<COMUNI>? GetList();

    COMUNI? Get(string comdes);

    bool Exists(string comdes);

    #region CRUD
    bool Insert(COMUNI Model);

    bool Update(COMUNI Model);

    bool Delete(COMUNI Model);

    string? Validate(COMUNI Model, bool IsInsert);
    #endregion
}

public class COMUNIRepository : RepositoryBase, ICOMUNIRepository
{
    public COMUNIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<COMUNI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<COMUNI>(
                "SELECT * FROM COMUNI");

            return new ObservableCollection<COMUNI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public COMUNI? Get(string comdes)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<COMUNI>(
                "SELECT * FROM COMUNI WHERE comdes = @comdes",
                new { comdes = comdes })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string comdes)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM COMUNI WHERE comdes = @comdes",
                new { comdes = comdes }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(COMUNI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO COMUNI (comdes,comist,comcod) OUTPUT INSERTED.rv VALUES(@comdes,@comist,@comcod)",
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

    public bool Update(COMUNI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE COMUNI SET comdes = @comdes,comist = @comist,comcod = @comcod OUTPUT INSERTED.rv WHERE comdes = @comdes AND rv = @rv",
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

    public bool Delete(COMUNI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM COMUNI OUTPUT DELETED.rv WHERE comdes = @comdes AND rv = @rv",
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

    public string? Validate(COMUNI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.comdes) && IsInsert && !Exists(Model.comdes)) || !IsInsert)
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