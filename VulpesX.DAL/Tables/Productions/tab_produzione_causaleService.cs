namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_causaleRepository
{

    ObservableCollection<tab_produzione_causale>? GetList(string CompanyID);

    tab_produzione_causale? Get(string CompanyID, string ID);

    bool Exists(string CompanyID, string ID);

    #region CRUD
    bool Insert(tab_produzione_causale Model);

    bool Update(tab_produzione_causale Model);

    bool Delete(tab_produzione_causale Model);

    string? Validate(tab_produzione_causale Model, bool IsInsert);
    #endregion
}

public class tab_produzione_causaleRepository : RepositoryBase, Itab_produzione_causaleRepository
{
    public tab_produzione_causaleRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_produzione_causale>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_causale>(
                "SELECT * FROM tab_produzione_causale WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

            return new ObservableCollection<tab_produzione_causale>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_produzione_causale? Get(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_causale>(
                "SELECT * FROM tab_produzione_causale WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_produzione_causale WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_produzione_causale Model)
    {
        using var connection = GetOpenConnection();

        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.Execute(
                    "INSERT INTO tab_produzione_causale (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Descrizione)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Descrizione)",
                    Model, transaction);

                //pulisce tab_produzione_reparto_risorsa
                result = connection.Execute(
                    "DELETE FROM tab_produzione_causale_risorsa WHERE SocietaID = @SocietaID AND CausaleID = @ID",
                    new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                foreach (var item in Model.CausaleRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                {

                    result = connection.Execute(
                    "INSERT INTO tab_produzione_causale_risorsa (SocietaID,CausaleID,RisorsaID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@CausaleID,@RisorsaID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                    new tab_produzione_causale_risorsa { SocietaID = Model.SocietaID, CausaleID = Model.ID, RisorsaID = item.ID, LogAdded = DateTime.Now }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

    }

    public bool Update(tab_produzione_causale Model)
    {
        using var connection = GetOpenConnection();

        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_produzione_causale SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Descrizione = @Descrizione" +
                    " OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND rv = @rv",
                    Model, transaction);

                //pulisce tab_produzione_reparto_risorsa
                result = connection.Execute(
                    "DELETE FROM tab_produzione_causale_risorsa WHERE SocietaID = @SocietaID AND CausaleID = @ID",
                    new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                foreach (var item in Model.CausaleRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                {

                    result = connection.Execute(
                    "INSERT INTO tab_produzione_causale_risorsa (SocietaID,CausaleID,RisorsaID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@CausaleID,@RisorsaID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                    new tab_produzione_causale_risorsa { SocietaID = Model.SocietaID, CausaleID = Model.ID, RisorsaID = item.ID, LogAdded = DateTime.Now }, transaction);
                }

                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

    }

    public bool Delete(tab_produzione_causale Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_produzione_causale OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv",
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

    public string? Validate(tab_produzione_causale Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.ID) && IsInsert && !Exists(Model.SocietaID, Model.ID) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Descrizione) && Model.Descrizione.Length <= 255)
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