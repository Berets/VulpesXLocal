namespace VulpesX.DAL.Tables.Accounting;

public interface IFE_PAGDOCRepository
{
    ObservableCollection<FE_PAGDOC>? GetList();

    FE_PAGDOC? Get(string FEPACOD);

    bool Exists(string FEPACOD);

    #region CRUD
    bool Insert(FE_PAGDOC Model);

    bool Update(FE_PAGDOC Model);

    bool Delete(FE_PAGDOC Model);

    string? Validate(FE_PAGDOC Model, bool IsInsert);
    #endregion
}

public class FE_PAGDOCRepository : RepositoryBase, IFE_PAGDOCRepository
{
    public FE_PAGDOCRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_PAGDOC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_PAGDOC>(
                "SELECT * FROM FE_PAGDOC");

            return new ObservableCollection<FE_PAGDOC>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_PAGDOC? Get(string FEPACOD)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_PAGDOC>(
                "SELECT * FROM FE_PAGDOC WHERE FEPACOD = @FEPACOD",
                new { FEPACOD = FEPACOD })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string FEPACOD)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_PAGDOC WHERE FEPACOD = @FEPACOD",
                new { FEPACOD = FEPACOD }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(FE_PAGDOC Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO FE_PAGDOC (FEPACOD,FEPADES,FEPATVAL,FEPADAT,FEPATIPP) OUTPUT INSERTED.rv VALUES(@FEPACOD,@FEPADES,@FEPATVAL,@FEPADAT,@FEPATIPP)",
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

    public bool Update(FE_PAGDOC Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE FE_PAGDOC SET FEPACOD = @FEPACOD,FEPADES = @FEPADES,FEPATVAL = @FEPATVAL,FEPADAT = @FEPADAT,FEPATIPP = @FEPATIPP OUTPUT INSERTED.rv WHERE FEPACOD = @FEPACOD AND rv = @rv",
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

    public bool Delete(FE_PAGDOC Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM FE_PAGDOC OUTPUT DELETED.rv WHERE FEPACOD = @FEPACOD AND rv = @rv",
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

    public string? Validate(FE_PAGDOC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.FEPACOD) && IsInsert && !Exists(Model.FEPACOD)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.FEPADES))
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