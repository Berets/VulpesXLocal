namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_risorsa_sorgentiRepository
{
    ObservableCollection<tab_produzione_risorsa_sorgenti>? GetFullList(string SocietaID);

    ObservableCollection<tab_produzione_risorsa_sorgenti>? GetList(string SocietaID, string RisorsaID);

    tab_produzione_risorsa_sorgenti? Get(string SocietaID, string RisorsaID, string ID);

    bool Exists(string SocietaID, string RisorsaID, string ID);

    #region CRUD
    bool Insert(tab_produzione_risorsa_sorgenti Model);

    bool Update(tab_produzione_risorsa_sorgenti Model);

    bool Delete(tab_produzione_risorsa_sorgenti Model);

    string? Validate(tab_produzione_risorsa_sorgenti Model, bool IsInsert);
    #endregion
}

public class tab_produzione_risorsa_sorgentiRepository : RepositoryBase, Itab_produzione_risorsa_sorgentiRepository
{
    public tab_produzione_risorsa_sorgentiRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_produzione_risorsa_sorgenti>? GetFullList(string SocietaID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_produzione_risorsa_sorgenti>(
                @"SELECT * FROM tab_produzione_risorsa_sorgenti
                        WHERE SocietaID = @cid",
                new { cid = SocietaID });

            return new ObservableCollection<tab_produzione_risorsa_sorgenti>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_risorsa_sorgenti>? GetList(string SocietaID, string RisorsaID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_risorsa_sorgenti>(
                @"SELECT * FROM tab_produzione_risorsa_sorgenti
                        WHERE SocietaID = @cid AND RisorsaID = @rid",
                new { cid = SocietaID, rid = RisorsaID });

            return new ObservableCollection<tab_produzione_risorsa_sorgenti>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_produzione_risorsa_sorgenti? Get(string SocietaID, string RisorsaID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_produzione_risorsa_sorgenti>(
                "SELECT * FROM tab_produzione_risorsa_sorgenti WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND ID = @ID",
                new { SocietaID = SocietaID, RisorsaID = RisorsaID, ID = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string RisorsaID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_produzione_risorsa_sorgenti WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND ID = @ID",
                new { SocietaID = SocietaID, RisorsaID = RisorsaID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_produzione_risorsa_sorgenti Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO tab_produzione_risorsa_sorgenti (SocietaID,RisorsaID,ID,Descrizione,Singolo) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@ID,@Descrizione,@Singolo)",
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

    public bool Update(tab_produzione_risorsa_sorgenti Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE tab_produzione_risorsa_sorgenti SET SocietaID = @SocietaID,RisorsaID = @RisorsaID,ID = @ID,Descrizione = @Descrizione,Singolo = @Singolo OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND ID = @ID AND rv = @rv",
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

    public bool Delete(tab_produzione_risorsa_sorgenti Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_produzione_risorsa_sorgenti OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND ID = @ID AND rv = @rv",
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

    public string? Validate(tab_produzione_risorsa_sorgenti Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && !string.IsNullOrEmpty(Model.RisorsaID) && !string.IsNullOrEmpty(Model.ID) &&
                IsInsert && !Exists(Model.SocietaID, Model.RisorsaID, Model.ID)) || !IsInsert)
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