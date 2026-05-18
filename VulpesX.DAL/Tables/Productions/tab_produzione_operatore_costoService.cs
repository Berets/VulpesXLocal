namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_operatore_costoRepository
{
    ObservableCollection<tab_produzione_operatore_costo>? GetList(string SocietaID, string OperatoreID);

    tab_produzione_operatore_costo? Get(string SocietaID, string OperatoreID, DateTime Periodo);

    bool Exists(string SocietaID, string OperatoreID, DateTime Periodo);

    #region CRUD
    bool Insert(tab_produzione_operatore_costo Model);

    bool Update(tab_produzione_operatore_costo Model);

    bool Delete(tab_produzione_operatore_costo Model);

    string? Validate(tab_produzione_operatore_costo Model, bool IsInsert);
    #endregion
}

public class tab_produzione_operatore_costoRepository : RepositoryBase, Itab_produzione_operatore_costoRepository
{
    public tab_produzione_operatore_costoRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<tab_produzione_operatore_costo>? GetList(string SocietaID, string OperatoreID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_operatore_costo>(
                @"SELECT * FROM tab_produzione_operatore_costo
                        WHERE SocietaID = @SocietaID AND OperatoreID = @OperatoreID",
                new { SocietaID = SocietaID, OperatoreID = OperatoreID });

            return new ObservableCollection<tab_produzione_operatore_costo>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_produzione_operatore_costo? Get(string SocietaID, string OperatoreID, DateTime Periodo)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_operatore_costo>(
                "SELECT * FROM tab_produzione_operatore_costo WHERE SocietaID = @SocietaID AND OperatoreID = @OperatoreID AND Periodo = @Periodo",
                new { SocietaID = SocietaID, OperatoreID = OperatoreID, Periodo = Periodo })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string OperatoreID, DateTime Periodo)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_produzione_operatore_costo WHERE SocietaID = @SocietaID AND OperatoreID = @OperatoreID AND Periodo = @Periodo",
                new { SocietaID = SocietaID, OperatoreID = OperatoreID, Periodo = Periodo }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_produzione_operatore_costo Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO tab_produzione_operatore_costo (SocietaID,OperatoreID,Periodo,CostoOrario) OUTPUT INSERTED.rv VALUES(@SocietaID,@OperatoreID,@Periodo,@CostoOrario)",
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

    public bool Update(tab_produzione_operatore_costo Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE tab_produzione_operatore_costo SET SocietaID = @SocietaID,OperatoreID = @OperatoreID,Periodo = @Periodo,CostoOrario = @CostoOrario OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OperatoreID = @OperatoreID AND Periodo = @Periodo AND rv = @rv",
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

    public bool Delete(tab_produzione_operatore_costo Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_produzione_operatore_costo OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND OperatoreID = @OperatoreID AND Periodo = @Periodo AND rv = @rv",
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

    public string? Validate(tab_produzione_operatore_costo Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && !string.IsNullOrEmpty(Model.OperatoreID) &&
                IsInsert && !Exists(Model.SocietaID, Model.OperatoreID, Model.Periodo)) || !IsInsert)
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