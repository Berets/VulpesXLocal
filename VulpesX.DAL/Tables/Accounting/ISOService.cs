namespace VulpesX.DAL.Tables.Accounting;


public interface IISORepository
{
    ObservableCollection<ISO>? GetList();

    ISO? Get(string ID);

    bool Exists(string ID);

    #region CRUD
    bool Insert(ISO Model);

    bool Update(ISO Model);

    bool Delete(ISO Model);

    string? Validate(ISO Model, bool IsInsert);
    #endregion
}

public class ISORepository : RepositoryBase, IISORepository
{
    public ISORepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ISO>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ISO>($@"SELECT i.*, l.lindes as isolinDescription FROM ISO as i
                 LEFT OUTER JOIN LINGUA as l ON i.isolin = l.lincod");

            return new ObservableCollection<ISO>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ISO? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ISO>(
                "SELECT * FROM ISO WHERE isocod = @id",
                new { id = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ISO WHERE isocod = @isocod",
                new { isocod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(ISO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO ISO (isocod,isodes,isopiv,isolin,isointr,isocod3166) OUTPUT INSERTED.rv VALUES(@isocod,@isodes,@isopiv,@isolin,@isointr,@isocod3166)",
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

    public bool Update(ISO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE ISO SET isocod = @isocod,isodes = @isodes,isopiv = @isopiv,isolin = @isolin,isointr = @isointr,isocod3166 = @isocod3166 OUTPUT INSERTED.rv WHERE isocod = @isocod AND rv = @rv",
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

    public bool Delete(ISO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM ISO OUTPUT DELETED.rv WHERE isocod = @isocod AND rv = @rv",
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

    public string? Validate(ISO Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrWhiteSpace(Model.isocod) && IsInsert && !Exists(Model.isocod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.isodes) && Model.isodes.Length <= 255)
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria e può contenere al massimo 255 caratteri"; }
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