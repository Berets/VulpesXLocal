namespace VulpesX.DAL.General;

public interface Itab_articolo_tipoRepository
{
    ObservableCollection<tab_articolo_tipo>? GetList(string CompanyID, bool AddAllValuesItem = false);

    tab_articolo_tipo? Get(string SocietaID, string ID);

    bool Exists(string SocietaID, string ID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(tab_articolo_tipo Model);

    bool Update(tab_articolo_tipo Model);

    bool Delete(tab_articolo_tipo Model);

    string? Validate(tab_articolo_tipo Model, bool IsInsert);
    #endregion
}

public class tab_articolo_tipoRepository : RepositoryBase, Itab_articolo_tipoRepository
{
    public tab_articolo_tipoRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_articolo_tipo>? GetList(string CompanyID, bool AddAllValuesItem = false)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo_tipo, store_stores, tab_articolo_tipo>(
                @"SELECT t.*, s.* FROM tab_articolo_tipo AS t
                        LEFT JOIN store_stores AS s ON s.company_id=t.SocietaID AND s.id=t.DefaultMagazzinoID
                        WHERE t.SocietaID = @cid",
                (tip, sto) => { tip.Stores = new ObservableCollection<store_stores>() { sto }; return tip; },
                new { cid = CompanyID }, splitOn: "company_id").ToList();
            if (AddAllValuesItem)
                list.Add(new tab_articolo_tipo() { SocietaID = CompanyID, ID = string.Empty, Descrizione = "Tutti i tipi articolo" });
            return new ObservableCollection<tab_articolo_tipo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_tipo? Get(string SocietaID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo_tipo>(
                "SELECT * FROM tab_articolo_tipo WHERE SocietaID = @SocietaID AND ID = @ID",
                new { SocietaID = SocietaID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo_tipo WHERE SocietaID = @SocietaID AND ID = @ID",
                new { SocietaID = SocietaID, ID = ID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO tab_articolo_tipo (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Descrizione,EMateriaPrima,DefaultMagazzinoID) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Descrizione,@EMateriaPrima,@DefaultMagazzinoID)";
    public string UPDATE_QUERY => "UPDATE tab_articolo_tipo SET SocietaID = @SocietaID,ID = @ID,LogAdded = @LogAdded,LogUpdated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Descrizione = @Descrizione,EMateriaPrima = @EMateriaPrima,DefaultMagazzinoID = @DefaultMagazzinoID OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM tab_articolo_tipo OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public bool Insert(tab_articolo_tipo Model)
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

    public bool Update(tab_articolo_tipo Model)
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

    public bool Delete(tab_articolo_tipo Model)
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

    public string? Validate(tab_articolo_tipo Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && IsInsert && !Exists(Model.SocietaID, Model.ID)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Descrizione))
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