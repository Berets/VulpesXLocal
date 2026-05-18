namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_repartoRepository
{
    ObservableCollection<tab_produzione_reparto>? GetList(string CompanyID);

    tab_produzione_reparto? Get(string CompanyID, string ID);

    bool Exists(string CompanyID, string ID);

    #region CRUD
    bool Insert(tab_produzione_reparto Model);

    bool Update(tab_produzione_reparto Model);

    bool Delete(tab_produzione_reparto Model);

    string? Validate(tab_produzione_reparto Model, bool IsInsert);
    #endregion
}

public class tab_produzione_repartoRepository : RepositoryBase, Itab_produzione_repartoRepository
{
    public tab_produzione_repartoRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_produzione_reparto>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_produzione_reparto>(
                "SELECT * FROM tab_produzione_reparto  WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

            return new ObservableCollection<tab_produzione_reparto>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_produzione_reparto? Get(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_reparto>(
                "SELECT * FROM tab_produzione_reparto WHERE SocietaID = @SocietaID and ID = @ID",
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
                "SELECT COUNT(*) FROM tab_produzione_reparto WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_produzione_reparto Model)
    {
        using var connection = GetOpenConnection();


        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.Execute(
                    "INSERT INTO tab_produzione_reparto (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Descrizione,TempoDefault,EPrelievo,EAvanzamentoParziale,FineCiclo)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Descrizione,@TempoDefault,@EPrelievo,@EAvanzamentoParziale,@FineCiclo)",
                    Model, transaction);

                //pulisce tab_produzione_reparto_risorsa
                result = connection.Execute(
                    "DELETE FROM tab_produzione_reparto_risorsa WHERE SocietaID = @SocietaID AND RepartoID = @ID",
                    new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                foreach (var item in Model.RepartoRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                {
                    result = connection.Execute(
                    "INSERT INTO tab_produzione_reparto_risorsa (SocietaID,RepartoID,RisorsaID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@RepartoID,@RisorsaID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                    new tab_produzione_reparto_risorsa { SocietaID = Model.SocietaID, RepartoID = Model.ID, RisorsaID = item.ID, LogAdded = DateTime.Now }, transaction);
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

    public bool Update(tab_produzione_reparto Model)
    {
        using var connection = GetOpenConnection();
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_produzione_reparto SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Descrizione = @Descrizione, TempoDefault = @TempoDefault, EPrelievo = @EPrelievo, EAvanzamentoParziale = @EAvanzamentoParziale, FineCiclo=@FineCiclo" +
                    " OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND rv = @rv",
                    Model, transaction);

                //pulisce tab_produzione_reparto_risorsa
                result = connection.Execute(
                    "DELETE FROM tab_produzione_reparto_risorsa WHERE SocietaID = @SocietaID AND RepartoID = @ID",
                    new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                foreach (var item in Model.RepartoRisorse ?? new ObservableCollection<tab_produzione_risorsa>())
                {

                    result = connection.Execute(
                    "INSERT INTO tab_produzione_reparto_risorsa (SocietaID,RepartoID,RisorsaID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@RepartoID,@RisorsaID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID)",
                    new tab_produzione_reparto_risorsa { SocietaID = Model.SocietaID, RepartoID = Model.ID, RisorsaID = item.ID, LogAdded = DateTime.Now }, transaction);
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

    public bool Delete(tab_produzione_reparto Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_produzione_reparto OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv",
                Model);

            //pulisce tab_produzione_reparto_risorsa
            result = connection.Execute(
                "DELETE FROM tab_produzione_reparto_risorsa OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND RepartoID = @ID AND rv = @rv",
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

    public string? Validate(tab_produzione_reparto Model, bool IsInsert)
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