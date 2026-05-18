namespace VulpesX.DAL.Tables.Accounting;

public interface IAFFIDABILITARepository
{
    ObservableCollection<AFFIDABILITA>? GetList();

    AFFIDABILITA? Get(string ID);

    bool Exists(string ID);

    #region CRUD
    bool Insert(AFFIDABILITA Model);

    bool Update(AFFIDABILITA Model);

    bool Delete(AFFIDABILITA Model);

    string? Validate(AFFIDABILITA Model, bool IsInsert);
    #endregion
}

public class AFFIDABILITARepository : RepositoryBase, IAFFIDABILITARepository
{
    public AFFIDABILITARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AFFIDABILITA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<AFFIDABILITA>(
                "SELECT * FROM AFFIDABILITA");

            return new ObservableCollection<AFFIDABILITA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AFFIDABILITA? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<AFFIDABILITA>(
                "SELECT * FROM AFFIDABILITA WHERE affcod = @id",
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
                "SELECT COUNT(*) FROM AFFIDABILITA WHERE affcod = @id",
                new { id = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(AFFIDABILITA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO AFFIDABILITA (affcod,affdes,afford,afffat,affrib) OUTPUT INSERTED.rv VALUES(@affcod,@affdes,@afford,@afffat,@affrib)",
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

    public bool Update(AFFIDABILITA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE AFFIDABILITA SET affcod = @affcod,affdes = @affdes,afford = @afford,afffat = @afffat,affrib = @affrib OUTPUT INSERTED.rv WHERE affcod = @affcod AND rv = @rv",
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

    public bool Delete(AFFIDABILITA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM AFFIDABILITA OUTPUT DELETED.rv WHERE affcod = @affcod AND rv = @rv",
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

    public string? Validate(AFFIDABILITA Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.affcod) && IsInsert && !Exists(Model.affcod) || !IsInsert)
            {
                if (!String.IsNullOrWhiteSpace(Model.affdes) && Model.affdes.Length <= 255)
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria e può contenere al massimo 255 caratteri"; }
            }
            else
            { return "I codici inseriti sono già in uso o non sono validi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}