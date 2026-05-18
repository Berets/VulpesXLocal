using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.Tables.Productions;

public interface Itab_articolo_produzione_sorgentiRepository
{
    ObservableCollection<tab_articolo_produzione_sorgenti>? GetList(string SocietaID, string ArticoloID);

    tab_articolo_produzione_sorgenti? Get(string SocietaID, string ArticoloID, string RisorsaID, string SorgenteID);

    bool Exists(string SocietaID, string ArticoloID, string RisorsaID, string SorgenteID);

    tab_articolo_produzione_sorgenti? CheckSingleAlreadyAssigned(string SocietaID, string RisorsaID, string SorgenteID);

    #region CRUD
    bool Insert(tab_articolo_produzione_sorgenti Model);

    bool Update(tab_articolo_produzione_sorgenti Model);

    bool Delete(tab_articolo_produzione_sorgenti Model);

    string? Validate(tab_articolo_produzione_sorgenti Model, bool IsInsert);
    #endregion
}

public class tab_articolo_produzione_sorgentiRepository : RepositoryBase, Itab_articolo_produzione_sorgentiRepository
{
    public tab_articolo_produzione_sorgentiRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_articolo_produzione_sorgenti>? GetList(string SocietaID, string ArticoloID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var allSources = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_sorgentiRepository>().GetFullList(SocietaID);
            var list = connection.Query<tab_articolo_produzione_sorgenti>(
                @"SELECT * FROM tab_articolo_produzione_sorgenti
                        WHERE SocietaID = @cid AND ArticoloID = @pid",
                new { cid = SocietaID, pid = ArticoloID });
            foreach (var row in list)
            {
                row.AllSources = allSources;
            }
            return new ObservableCollection<tab_articolo_produzione_sorgenti>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_produzione_sorgenti? Get(string SocietaID, string ArticoloID, string RisorsaID, string SorgenteID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_articolo_produzione_sorgenti>(
                "SELECT * FROM tab_articolo_produzione_sorgenti WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RisorsaID = @RisorsaID AND SorgenteID = @SorgenteID",
                new { SocietaID = SocietaID, ArticoloID = ArticoloID, RisorsaID = RisorsaID, SorgenteID = SorgenteID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string ArticoloID, string RisorsaID, string SorgenteID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo_produzione_sorgenti WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RisorsaID = @RisorsaID AND SorgenteID = @SorgenteID",
                new { SocietaID = SocietaID, ArticoloID = ArticoloID, RisorsaID = RisorsaID, SorgenteID = SorgenteID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public tab_articolo_produzione_sorgenti? CheckSingleAlreadyAssigned(string SocietaID, string RisorsaID, string SorgenteID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo_produzione_sorgenti, tab_articolo, tab_articolo_produzione_sorgenti>(
                @"SELECT s.*, p.* FROM tab_articolo_produzione_sorgenti AS s
                        INNER JOIN tab_articolo AS p ON s.SocietaID = p.SocietaID AND s.ArticoloID = p.ID
                        WHERE s.SocietaID = @SocietaID AND s.RisorsaID = @RisorsaID AND s.SorgenteID = @SorgenteID",
                (src, prd) => { src.Product = prd; return src; },
                new { SocietaID = SocietaID, RisorsaID = RisorsaID, SorgenteID = SorgenteID }, splitOn: "SocietaID").FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public bool Insert(tab_articolo_produzione_sorgenti Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO tab_articolo_produzione_sorgenti (SocietaID,ArticoloID,RisorsaID,SorgenteID) OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RisorsaID,@SorgenteID)",
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

    public bool Update(tab_articolo_produzione_sorgenti Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE tab_articolo_produzione_sorgenti SET SocietaID = @SocietaID,ArticoloID = @ArticoloID,RisorsaID = @RisorsaID,SorgenteID = @SorgenteID OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RisorsaID = @RisorsaID AND SorgenteID = @SorgenteID AND rv = @rv",
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

    public bool Delete(tab_articolo_produzione_sorgenti Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_articolo_produzione_sorgenti OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RisorsaID = @RisorsaID AND SorgenteID = @SorgenteID AND rv = @rv",
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

    public string? Validate(tab_articolo_produzione_sorgenti Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && !string.IsNullOrEmpty(Model.ArticoloID) &&
                !string.IsNullOrEmpty(Model.SelectedRisorsa?.ID) && !string.IsNullOrEmpty(Model.SelectedSorgente?.ID) && IsInsert &&
                !Exists(Model.SocietaID, Model.ArticoloID, Model.SelectedRisorsa?.ID ?? string.Empty, Model.SelectedSorgente?.ID ?? string.Empty)) || !IsInsert)
            {
                var source = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsa_sorgentiRepository>().Get(Model.SocietaID, Model.SelectedRisorsa?.ID ?? string.Empty, Model.SelectedSorgente?.ID ?? string.Empty);

                if (source == null)
                {
                    return "Sorgente non trovata";
                }
                var assigned = CheckSingleAlreadyAssigned(Model.SocietaID, Model.SelectedRisorsa?.ID ?? string.Empty, Model.SelectedSorgente?.ID ?? string.Empty);
                if (!source.Singolo || (source.Singolo && assigned == null))
                {
                    return null;
                }
                else
                {
                    return $"La sorgente selezionata puo' essere assegnata una sola volta, e risulta gia' utilizzata da [{assigned?.Product?.FullDescriptionSearchable}]";
                }
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