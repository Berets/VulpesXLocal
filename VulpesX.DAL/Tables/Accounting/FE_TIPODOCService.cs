using VulpesX.DAL;

namespace VulpesX.Services.Tables.Accounting;

public interface IFE_TIPODOCRepository
{
    ObservableCollection<FE_TIPODOC>? GetList();

    FE_TIPODOC? Get(string FETDCod);

    bool Exists(string FETDCod);

    #region CRUD
    bool Insert(FE_TIPODOC Model);

    bool Update(FE_TIPODOC Model);

    bool Delete(FE_TIPODOC Model);

    string? Validate(FE_TIPODOC Model, bool IsInsert);
    #endregion
}

public class FE_TIPODOCRepository : RepositoryBase, IFE_TIPODOCRepository
{
    public FE_TIPODOCRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FE_TIPODOC>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FE_TIPODOC>(
                "SELECT * FROM FE_TIPODOC");

            return new ObservableCollection<FE_TIPODOC>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public FE_TIPODOC? Get(string FETDCod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<FE_TIPODOC>(
                "SELECT * FROM FE_TIPODOC WHERE FETDCod = @FETDCod",
                new { FETDCod = FETDCod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string FETDCod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM FE_TIPODOC WHERE FETDCod = @FETDCod",
                new { FETDCod = FETDCod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(FE_TIPODOC Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO FE_TIPODOC (FETDCod,FETDDes,FETDDat,FETDACQC) OUTPUT INSERTED.rv VALUES(@FETDCod,@FETDDes,@FETDDat,@FETDACQC)",
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

    public bool Update(FE_TIPODOC Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE FE_TIPODOC SET FETDCod = @FETDCod,FETDDes = @FETDDes,FETDDat = @FETDDat,FETDACQC = @FETDACQC OUTPUT INSERTED.rv WHERE FETDCod = @FETDCod AND rv = @rv",
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

    public bool Delete(FE_TIPODOC Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM FE_TIPODOC OUTPUT DELETED.rv WHERE FETDCod = @FETDCod AND rv = @rv",
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

    public string? Validate(FE_TIPODOC Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.FETDCod) && IsInsert && !Exists(Model.FETDCod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.FETDDes))
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