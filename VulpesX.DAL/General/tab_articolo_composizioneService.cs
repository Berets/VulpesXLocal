
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using VulpesX.DAL.CRM;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.Tables.Productions;

namespace VulpesX.DAL.General;

public interface Itab_articolo_composizioneRepository
{
    ObservableCollection<tab_articolo>? GetRevisioni(string SocietaID);

    ObservableCollection<tab_articolo>? GetRevisioni(string SocietaID, string ArticoloID);

    ObservableCollection<string>? GetRevisioniSimpleList(string SocietaID, string ArticoloID);

    tab_articolo_composizione? Single(string SocietaID, string ArticoloID, string? RevisioneID, tab_articolo_composizione? Padre);

    bool HasCycle(string SocietaID, string ArticoloID);

    string? GetLastUpdatedRevisionID(string SocietaID, string ArticoloID);

    ObservableCollection<tab_articolo_composizione>? Get(string SocietaID, string ArticoloID, tab_articolo_composizione? Padre);

    ObservableCollection<tab_articolo_composizione>? Get(string SocietaID, string ArticoloID, string RevisioneID, tab_articolo_composizione? Padre);

    ObservableCollection<tab_articolo_composizione>? GetComponents(string SocietaID, string ArticoloID, string RevisioneID, tab_articolo_composizione Padre);

    ObservableCollection<tab_articolo_composizione>? GetComponentsByProduct(string SocietaID, string ArticoloID, string RevisioneID);

    ObservableCollection<tab_articolo_composizione>? GetStages(string SocietaID, string ArticoloID, string RevisioneID, long ComposizioneID, tab_articolo_composizione Padre);

    ObservableCollection<tab_articolo_composizione>? GetDipendenze(string SocietaID, string ArticoloID);

    ObservableCollection<tab_articolo_composizione>? GetDipendenze(string SocietaID, string ArticoloID, string RevisioneID);

    ObservableCollection<tab_articolo_composizione>? GetDipendenzeLevel(string SocietaID, string ArticoloID, tab_articolo_composizione? Padre);

    ObservableCollection<tab_articolo_composizione>? GetDipendenzeLevel(string SocietaID, string ArticoloID, string RevisioneID, tab_articolo_composizione? Padre);

    bool ExistRevision(string SocietaID, string ArticoloID, string RevisioneID);

    bool IsValid(tab_articolo_composizione Articolo, ref string Errors);

    bool IsValidRevisione(tab_articolo_composizione Articolo, ref string Error);

    bool DuplicateRevisione(tab_articolo Articolo);

    bool DeleteRevisione(tab_articolo Articolo);

    long SaveDuplicate(string SocietaID, string? ArticoloID, tab_articolo_composizione Articolo, long Posizione, string? RevisioneID);

    long Save(string SocietaID, string? ArticoloID, tab_articolo_composizione Articolo, long Posizione, string? RevisioneID);

    bool Exchange(string SocietaID, List<tab_articolo_composizione> Roots, string oArticoloID, string oRevisioneID, string nArticoloID, string nRevisioneID);

    bool ExchangeDependency(string SocietaID, IDbConnection Connection, IDbTransaction Transaction, tab_articolo_composizione Root);
}

public class tab_articolo_composizioneRepository : RepositoryBase, Itab_articolo_composizioneRepository
{
    public tab_articolo_composizioneRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_articolo>? GetRevisioni(string SocietaID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_articolo, tab_articolo_composizione, tab_articolo_tipo, tab_articolo_categoria, tab_articolo_unita, int, tab_articolo>(
                "SELECT a.*, g.*, t.Descrizione, c.Descrizione, u.Descrizione, (SELECT COUNT(SocietaID) FROM tab_articolo_composizione as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Com, (SELECT COUNT(SocietaID) FROM tab_articolo_immagine as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Img, (SELECT COUNT(SocietaID) FROM tab_articolo_allegato as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Att FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY SocietaID,ArticoloID,RevisioneID ORDER BY ArticoloID DESC) AS [ROW NUMBER] FROM tab_articolo_composizione) as g " +
                "INNER JOIN tab_articolo as a ON a.SocietaID = g.SocietaID AND a.ID = g.ArticoloID " +
                "LEFT OUTER JOIN tab_articolo_tipo AS t ON a.SocietaID = t.SocietaID AND a.TipoID = t.ID " +
                "LEFT OUTER JOIN tab_articolo_categoria AS c ON a.SocietaID = c.SocietaID AND a.CategoriaID = c.ID " +
                "LEFT OUTER JOIN tab_articolo_unita AS u ON a.SocietaID = u.SocietaID AND a.UnitaID = u.ID " +
                "WHERE g.[ROW NUMBER] = 1 AND g.SocietaID = @SocietaID " +
                "ORDER BY g.ArticoloID DESC"
                , (articolo, revisione, tipo, categoria, unita, composizione) =>
                {
                    articolo.RevisioneID = revisione.RevisioneID;
                    articolo.TipoDescrizione = tipo?.Descrizione;
                    articolo.CategoriaDescrizione = categoria?.Descrizione;
                    articolo.UnitaDescrizione = unita?.Descrizione;
                    articolo.HaComposizione = composizione > 0;
                    //articolo.HaDipendenze = GetDipendenze(articolo.SocietaID, articolo.ID, articolo.RevisioneID).Any();
                    return articolo;
                }, new { SocietaID = SocietaID },
                    splitOn: "SocietaID,Descrizione,Descrizione,Descrizione,Com");

            return new ObservableCollection<tab_articolo>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }

    }

    public ObservableCollection<tab_articolo>? GetRevisioni(string SocietaID, string ArticoloID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_articolo, tab_articolo_composizione, tab_articolo_tipo, tab_articolo_categoria, tab_articolo_unita, int, tab_articolo>(
                "SELECT a.*, g.*, t.Descrizione, c.Descrizione, u.Descrizione, (SELECT COUNT(SocietaID) FROM tab_articolo_composizione as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Com, (SELECT COUNT(SocietaID) FROM tab_articolo_immagine as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Img, (SELECT COUNT(SocietaID) FROM tab_articolo_allegato as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Att FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY SocietaID,ArticoloID,RevisioneID ORDER BY ArticoloID DESC) AS [ROW NUMBER] FROM tab_articolo_composizione) as g " +
                "INNER JOIN tab_articolo as a ON a.SocietaID = g.SocietaID AND a.ID = g.ArticoloID " +
                "LEFT OUTER JOIN tab_articolo_tipo AS t ON a.SocietaID = t.SocietaID AND a.TipoID = t.ID " +
                "LEFT OUTER JOIN tab_articolo_categoria AS c ON a.SocietaID = c.SocietaID AND a.CategoriaID = c.ID " +
                "LEFT OUTER JOIN tab_articolo_unita AS u ON a.SocietaID = u.SocietaID AND a.UnitaID = u.ID " +
                "WHERE g.[ROW NUMBER] = 1 AND g.SocietaID = @SocietaID and g.ArticoloID = @ArticoloID " +
                "ORDER BY g.ArticoloID DESC"
                , (articolo, revisione, tipo, categoria, unita, composizione) =>
                {
                    articolo.RevisioneID = revisione.RevisioneID;
                    articolo.TipoDescrizione = tipo?.Descrizione;
                    articolo.CategoriaDescrizione = categoria?.Descrizione;
                    articolo.UnitaDescrizione = unita?.Descrizione;
                    articolo.HaComposizione = composizione > 0;

                    return articolo;
                }, new { SocietaID = SocietaID, ArticoloID = ArticoloID },
                    splitOn: "SocietaID,Descrizione,Descrizione,Descrizione,Com");

            return new ObservableCollection<tab_articolo>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }

    }

    public ObservableCollection<string>? GetRevisioniSimpleList(string SocietaID, string ArticoloID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var data = connection.Query<string>(
                @"SELECT DISTINCT RevisioneID FROM tab_articolo_composizione 
                      WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND LogCanceled IS NULL 
                      ORDER BY RevisioneID", new { SocietaID = SocietaID, ArticoloID = ArticoloID });
            return new ObservableCollection<string>(data);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }

    }

    public tab_articolo_composizione? Single(string SocietaID, string ArticoloID, string? RevisioneID, tab_articolo_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var article = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(SocietaID, ArticoloID);

            if (article == null)
                return null;

            var finished = new tab_articolo_composizione
            {
                SocietaID = article.SocietaID,
                ArticoloID = article.ID,
                RevisioneID = RevisioneID ?? string.Empty,
                RevisioneIDOld = RevisioneID,
                ComponenteRevisioneID = RevisioneID,
                ComponenteRevisioneIDOld = RevisioneID,
                Descrizione = article.Descrizione,
                Padre = Padre,
                EComponente = true,
                EMilestone = false,
                ESummary = false,
                ComponenteArticoloID = ArticoloID,
                IsRoot = true,
                Componenti = new ObservableCollection<tab_articolo_composizione>(),
            };

            var childrens = connection.Query<tab_articolo_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_composizione>(
               "SELECT c.*,rep.Descrizione,ris.*, com.* FROM tab_articolo_composizione AS c " +
               "LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID " +
               "LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID= ris.SocietaID AND c.RisorsaID = ris.ID " +
               "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID " +
               "WHERE c.SocietaID = @SocietaID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre is null"
               , (composizione, reparto, risorsa, componente) =>
               {
                   composizione.RevisioneIDOld = composizione.RevisioneID;
                   composizione.ComponenteRevisioneIDOld = composizione.ComponenteRevisioneID;
                   composizione.Padre = Padre;
                   composizione.Descrizione = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto.Descrizione : componente.Descrizione;
                   composizione.ETempoAlPezzo = (risorsa != null) ? risorsa.ETempoAlPezzo ?? false : false;
                   composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();

                   return composizione;
               },
               new { SocietaID = SocietaID, RevisioneID = RevisioneID, ArticoloID = ArticoloID },
                   splitOn: "Descrizione,SocietaID,SocietaID");

            foreach (var chl in childrens)
            {
                if (!chl.EReparto && !chl.ESummary && !chl.EMilestone)
                {
                    if (!string.IsNullOrEmpty(chl.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(chl.SocietaID, chl.ComponenteArticoloID ?? string.Empty, chl.ComponenteRevisioneID, chl) ?? new ObservableCollection<tab_articolo_composizione>())
                        {
                            chl.Componenti.Add(cmp);
                        }
                    }
                    else
                    {
                        chl.RevisioneID = String.Empty;
                    }
                }
                else
                {
                    foreach (var stg in GetStages(chl.SocietaID, chl.ArticoloID, chl.RevisioneID, chl.ComposizioneID, Padre) ?? new ObservableCollection<tab_articolo_composizione>())
                    {
                        chl.Componenti.Add(stg);
                    }
                }

                finished.Componenti.Add(chl);

            }

            return finished;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool HasCycle(string SocietaID, string ArticoloID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM tab_articolo_composizione
                                                    WHERE SocietaID = @cid AND ArticoloID = @pid AND LogCanceled IS NULL",
                new { cid = SocietaID, pid = ArticoloID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? GetLastUpdatedRevisionID(string SocietaID, string ArticoloID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return connection.ExecuteScalar<string>(@"SELECT TOP (1)RevisioneID FROM tab_articolo_composizione
                                                    WHERE SocietaID = @cid AND ArticoloID = @pid AND LogCanceled IS NULL
                                                    ORDER BY LogUpdated",
                new { cid = SocietaID, pid = ArticoloID });

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? Get(string SocietaID, string ArticoloID, tab_articolo_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var article = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(SocietaID, ArticoloID);

            if (article == null)
                return null;

            var finished = new tab_articolo_composizione
            {
                IsRoot = true,
                SocietaID = SocietaID,
                ArticoloID = article.ID,
                RevisioneID = String.Empty,
                RevisioneIDOld = String.Empty,
                Descrizione = article.Descrizione,
                Padre = Padre,
                EComponente = false,
                EMilestone = false,
                ESummary = false,
                EEspanso = true,
                ComponenteRevisioneID = String.Empty,
                ComponenteRevisioneIDOld = String.Empty,
                ComponenteArticoloID = ArticoloID,
                Componenti = new ObservableCollection<tab_articolo_composizione>(),
            };

            retValue.Add(finished);

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? Get(string SocietaID, string ArticoloID, string RevisioneID, tab_articolo_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();

            var risorsaRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>();
            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var article = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(SocietaID, ArticoloID, connection);

            if (article == null)
                return null;

            var finished = new tab_articolo_composizione
            {
                IsRoot = true,
                SocietaID = SocietaID,
                ArticoloID = article.ID,
                RevisioneID = RevisioneID,
                RevisioneIDOld = RevisioneID,
                Descrizione = article.Descrizione,
                Padre = Padre,
                EComponente = false,
                EMilestone = false,
                ESummary = false,
                EEspanso = true,
                Componenti = new ObservableCollection<tab_articolo_composizione>(),
            };

            var childrens = connection.Query<tab_articolo_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, tab_articolo_composizione>(
               $@"SELECT c.*,rep.Descrizione,ris.Descrizione, com.*, uni.Descrizione FROM tab_articolo_composizione AS c 
                   LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID 
                   LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                   LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID
                   LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID 
                   WHERE c.SocietaID = @SocietaID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre is null"
               , (composizione, reparto, risorsa, componente, unita) =>
               {
                   composizione.RevisioneIDOld = composizione.RevisioneID;
                   composizione.Padre = finished;
                   composizione.Descrizione = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto?.Descrizione : componente?.Descrizione;
                   composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                   composizione.EComponente = true;
                   composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                   composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                   composizione.ComponenteRevisioneIDOld = composizione.ComponenteRevisioneID;
                   composizione.ETempoAlPezzo = (risorsa != null) ? risorsa?.ETempoAlPezzo ?? false : false;
                   composizione.RisorsaDescrizione = (risorsa != null) ? risorsa?.Descrizione : "Non impostata";
                   composizione.UnitaDescrizione = (unita != null) ? unita?.Descrizione : String.Empty;

                   return composizione;
               },
               new { SocietaID = SocietaID, RevisioneID = RevisioneID, ArticoloID = ArticoloID },
                   splitOn: "Descrizione,Descrizione,SocietaID,Descrizione");

            foreach (var chl in childrens)
            {
                if (!chl.EReparto && !chl.ESummary && !chl.EMilestone)
                {
                    if (!string.IsNullOrEmpty(chl.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(chl.SocietaID, chl.ComponenteArticoloID ?? string.Empty, chl.ComponenteRevisioneID, chl) ?? new ObservableCollection<tab_articolo_composizione>())
                        {
                            chl.Componenti.Add(cmp);
                        }
                    }
                }
                else
                {

                    chl.Risorse = new ObservableCollection<tab_produzione_risorsa>( connection.Query<tab_produzione_risorsa>(
                @"SELECT r.* FROM tab_produzione_reparto_risorsa AS m
                        INNER JOIN tab_produzione_risorsa AS r ON r.SocietaID = m.SocietaID AND r.ID = m.RisorsaID
                        WHERE m.SocietaID = @SocietaID and m.RepartoID = @ID", new { SocietaID = chl.SocietaID, ID = chl.RepartoID }).ToList());
                    foreach (var stg in GetStages(chl.SocietaID, chl.ArticoloID, chl.RevisioneID, chl.ComposizioneID, finished) ?? new ObservableCollection<tab_articolo_composizione>())
                    {
                        chl.Componenti.Add(stg);
                    }
                }

                finished.Componenti.Add(chl);
            }

            retValue.Add(finished);

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetComponents(string SocietaID, string ArticoloID, string RevisioneID, tab_articolo_composizione Padre)
    {
        try
        {
            using var connection = GetOpenConnection();

            var risorsaRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>();


            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var list = connection.Query<tab_articolo_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, tab_articolo_composizione>(
                $@"SELECT c.*,rep.Descrizione,ris.Descrizione, com.*, uni.Descrizione FROM tab_articolo_composizione AS c 
                            LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID
                            LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID
                            LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID
                            LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID
                            WHERE c.SocietaID = @SocietaID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre is null"
                , (composizione, reparto, risorsa, componente, unita) =>
                {
                    composizione.RevisioneIDOld = composizione.RevisioneID;
                    composizione.Padre = Padre;
                    composizione.Descrizione = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto.Descrizione : componente.Descrizione;
                    composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                    composizione.EComponente = true;
                    composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                    composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                    composizione.ComponenteRevisioneIDOld = composizione.ComponenteRevisioneID;
                    composizione.ETempoAlPezzo = (risorsa != null) ? risorsa.ETempoAlPezzo ?? false : false;
                    composizione.RisorsaDescrizione = (risorsa != null) ? risorsa.Descrizione : "Non impostata";
                    composizione.UnitaDescrizione = (unita != null) ? unita.Descrizione : String.Empty;

                    return composizione;
                },
                new { SocietaID = SocietaID, RevisioneID = RevisioneID, ArticoloID = ArticoloID },
                    splitOn: "Descrizione,Descrizione,SocietaID,Descrizione");

            foreach (var component in list)
            {
                if (!component.EReparto && !component.ESummary && !component.EMilestone)
                {
                    if (!string.IsNullOrEmpty(component.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(component.SocietaID, component.ComponenteArticoloID ?? string.Empty, component.ComponenteRevisioneID, component) ?? new ObservableCollection<tab_articolo_composizione>())
                        {
                            component.Componenti.Add(cmp);
                        }
                    }
                }
                else
                {
                    component.Risorse = risorsaRepo.GetListFromReparto(component.SocietaID, component.RepartoID ?? string.Empty);
                    foreach (var stg in GetStages(component.SocietaID, component.ArticoloID, component.RevisioneID, component.ComposizioneID, Padre) ?? new ObservableCollection<tab_articolo_composizione>())
                    {
                        component.Componenti.Add(stg);
                    }
                }

                retValue.Add(component);

            }

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetComponentsByProduct(string SocietaID, string ArticoloID, string RevisioneID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return new ObservableCollection<tab_articolo_composizione>(connection.Query<tab_articolo_composizione>(
                @"SELECT c.* FROM tab_articolo_composizione AS c 
                    WHERE c.SocietaID = @SocietaID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID;",
                new { SocietaID = SocietaID, RevisioneID = RevisioneID, ArticoloID = ArticoloID }).ToList());

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetStages(string SocietaID, string ArticoloID, string RevisioneID, long ComposizioneID, tab_articolo_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();

            var risorsaRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_produzione_risorsaRepository>();

            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var list = connection.Query<tab_articolo_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_composizione>(
                $@"SELECT c.*,rep.Descrizione,ris.*,com.* FROM tab_articolo_composizione AS c 
                               LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID
                               LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID
                               LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID
                               WHERE c.SocietaID = @SocietaID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre = @ComposizioneID",
                (composizione, reparto, risorsa, componente) =>
                {
                    composizione.RevisioneIDOld = composizione.RevisioneID;
                    composizione.Padre = Padre;
                    composizione.Descrizione = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto.Descrizione : componente.Descrizione;
                    composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                    composizione.EComponente = true;
                    composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                    composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                    composizione.ComponenteRevisioneIDOld = composizione.ComponenteRevisioneID;
                    composizione.ETempoAlPezzo = (risorsa != null) ? risorsa.ETempoAlPezzo ?? false : false;
                    composizione.RisorsaDescrizione = (risorsa != null) ? risorsa.Descrizione : "Non impostata";

                    return composizione;
                },
                new { SocietaID = SocietaID, RevisioneID = RevisioneID, ArticoloID = ArticoloID, ComposizioneID = ComposizioneID },
                    splitOn: "Descrizione,SocietaID,SocietaID");

            foreach (var component in list)
            {
                if (!component.EReparto && !component.ESummary && !component.EMilestone)
                {
                    if (!string.IsNullOrEmpty(component.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(component.SocietaID, component.ComponenteArticoloID ?? string.Empty, component.ComponenteRevisioneID, component) ?? new ObservableCollection<tab_articolo_composizione>())
                        {
                            component.Componenti.Add(cmp);
                        }
                    }
                }
                else
                {
                    component.Risorse = risorsaRepo.GetListFromReparto(component.SocietaID, component.RepartoID ?? string.Empty);
                    foreach (var stg in GetStages(component.SocietaID, component.ArticoloID, component.RevisioneID, component.ComposizioneID, Padre) ?? new ObservableCollection<tab_articolo_composizione>())
                    {
                        component.Componenti.Add(stg);
                    }
                }

                retValue.Add(component);

            }

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetDipendenze(string SocietaID, string ArticoloID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var dependencies = connection.Query<tab_articolo_composizione, tab_articolo, tab_articolo_composizione>(
               "SELECT c.*, com.* FROM tab_articolo_composizione AS c " +
               "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ArticoloID = com.ID " +
               "WHERE c.SocietaID = @SocietaID AND c.ComponenteArticoloID = @ArticoloID"
               , (composizione, componente) =>
               {
                   composizione.RevisioneIDOld = composizione.RevisioneID;
                   composizione.Padre = null;
                   composizione.Descrizione = componente.Descrizione;
                   composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                   composizione.EComponente = true;
                   composizione.ComponenteArticoloID = composizione.ComponenteArticoloID;
                   composizione.HaDipendenze = true;
                   composizione.QuantitaNuova = composizione.Quantita;

                   return composizione;
               },
               new { SocietaID = SocietaID, ArticoloID = ArticoloID },
                   splitOn: "SocietaID");

            foreach (var dep in dependencies)
            {
                dep.Dipendenze = GetDipendenze(SocietaID, dep.ArticoloID, dep.RevisioneID);
            }

            retValue = new ObservableCollection<tab_articolo_composizione>(dependencies);

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetDipendenze(string SocietaID, string ArticoloID, string RevisioneID)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var dependencies = connection.Query<tab_articolo_composizione, tab_articolo, tab_articolo_composizione>(
               "SELECT c.*, com.* FROM tab_articolo_composizione AS c " +
               "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ArticoloID = com.ID " +
               "WHERE c.SocietaID = @SocietaID AND c.ComponenteArticoloID = @ArticoloID AND c.ComponenteRevisioneID = @RevisioneID"
               , (composizione, componente) =>
               {
                   composizione.RevisioneIDOld = composizione.RevisioneID;
                   composizione.Padre = null;
                   composizione.Descrizione = componente.Descrizione;
                   composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                   composizione.EComponente = true;
                   composizione.ComponenteArticoloID = composizione.ComponenteArticoloID;
                   composizione.HaDipendenze = true;
                   composizione.QuantitaNuova = composizione.Quantita;

                   return composizione;
               },
               new { SocietaID = SocietaID, ArticoloID = ArticoloID, RevisioneID = RevisioneID },
                   splitOn: "SocietaID");

            foreach (var dep in dependencies)
            {
                dep.Dipendenze = GetDipendenze(SocietaID, dep.ArticoloID, dep.RevisioneID);
            }

            retValue = new ObservableCollection<tab_articolo_composizione>(dependencies);

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetDipendenzeLevel(string SocietaID, string ArticoloID, tab_articolo_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var dependencies = connection.Query<tab_articolo_composizione, tab_articolo, tab_articolo_composizione>(
               "SELECT c.*, com.* FROM tab_articolo_composizione AS c " +
               "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ArticoloID = com.ID " +
               "WHERE c.SocietaID = @SocietaID AND c.ComponenteArticoloID = @ArticoloID"
               , (composizione, componente) =>
               {
                   composizione.RevisioneIDOld = composizione.RevisioneID;
                   composizione.Padre = Padre;
                   composizione.Descrizione = componente.Descrizione;
                   composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                   composizione.EComponente = true;
                   composizione.ComponenteArticoloID = composizione.ComponenteArticoloID;
                   composizione.HaDipendenze = true;
                   composizione.QuantitaNuova = composizione.Quantita;

                   return composizione;
               },
               new { SocietaID = SocietaID, ArticoloID = ArticoloID },
                   splitOn: "SocietaID");

            foreach (var dep in dependencies)
            {
                dep.Dipendenze = GetDipendenzeLevel(SocietaID, dep.ArticoloID, dep.RevisioneID, dep);
            }

            retValue = new ObservableCollection<tab_articolo_composizione>(dependencies);

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_composizione>? GetDipendenzeLevel(string SocietaID, string ArticoloID, string RevisioneID, tab_articolo_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<tab_articolo_composizione>();

            var dependencies = connection.Query<tab_articolo_composizione, tab_articolo, tab_articolo_composizione>(
               "SELECT c.*, com.* FROM tab_articolo_composizione AS c " +
               "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ArticoloID = com.ID " +
               "WHERE c.SocietaID = @SocietaID AND c.ComponenteArticoloID = @ArticoloID AND c.ComponenteRevisioneID = @RevisioneID"
               , (composizione, componente) =>
               {
                   composizione.RevisioneIDOld = composizione.RevisioneID;
                   composizione.Padre = Padre;
                   composizione.Descrizione = componente.Descrizione;
                   composizione.Componenti = new ObservableCollection<tab_articolo_composizione>();
                   composizione.EComponente = true;
                   composizione.ComponenteArticoloID = composizione.ComponenteArticoloID;
                   composizione.HaDipendenze = true;
                   composizione.QuantitaNuova = composizione.Quantita;

                   return composizione;
               },
               new { SocietaID = SocietaID, ArticoloID = ArticoloID, RevisioneID = RevisioneID },
                   splitOn: "SocietaID");

            foreach (var dep in dependencies)
            {
                dep.Dipendenze = GetDipendenzeLevel(SocietaID, dep.ArticoloID, dep.RevisioneID, dep);
            }

            retValue = new ObservableCollection<tab_articolo_composizione>(dependencies);

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool ExistRevision(string SocietaID, string ArticoloID, string RevisioneID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo_composizione WHERE SocietaID = @SocietaID and ArticoloID = @ArticoloID and RevisioneID = @RevisioneID",
                new { SocietaID = SocietaID, ArticoloID = ArticoloID, RevisioneID = RevisioneID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool IsValid(tab_articolo_composizione Articolo, ref string Errors)
    {
        return IsValidRevisione(Articolo, ref Errors);
    }

    public bool IsValidRevisione(tab_articolo_composizione Articolo, ref string Error)
    {
        if (Articolo.HaComposizione && !Articolo.EMilestone && !Articolo.ESummary)
        {
            if (Articolo.IsRoot)
            {
                if (!string.IsNullOrEmpty(Articolo.RevisioneID))
                {
                    if (Articolo.RevisioneID != Articolo.RevisioneIDOld)
                    {
                        Error = string.Empty;

                        bool cmpResult = false;
                        string outError = string.Empty;
                        foreach (var cmp in Articolo.Componenti)
                        {
                            cmpResult = IsValidRevisione(cmp, ref outError);

                            if (!cmpResult)
                                break;
                        }

                        Error = outError;
                        return cmpResult;
                    }
                    else
                    {
                        bool cmpResult = false;
                        string outError = string.Empty;
                        foreach (var cmp in Articolo.Componenti)
                        {
                            cmpResult = IsValidRevisione(cmp, ref outError);

                            if (!cmpResult)
                                break;
                        }

                        Error = outError;
                        return cmpResult;
                    }
                }
                else
                {
                    Error += string.Format("Inserire la revisione per l'articolo - {0}", Articolo.ArticoloID);
                    Error += Environment.NewLine;
                    return false;
                }
            }
            else
            {
                if (!Articolo.EReparto)
                {
                    if (!string.IsNullOrEmpty(Articolo.ComponenteRevisioneID))
                    {
                        if (Articolo.ComponenteRevisioneID != Articolo.ComponenteRevisioneIDOld)
                        {
                            Error = string.Empty;

                            bool cmpResult = false;
                            string outError = string.Empty;
                            foreach (var cmp in Articolo.Componenti)
                            {
                                cmpResult = IsValidRevisione(cmp, ref outError);

                                if (!cmpResult)
                                    break;
                            }

                            Error = outError;
                            return cmpResult;
                        }
                        else
                        {
                            bool cmpResult = false;
                            string outError = string.Empty;
                            foreach (var cmp in Articolo.Componenti)
                            {
                                cmpResult = IsValidRevisione(cmp, ref outError);

                                if (!cmpResult)
                                    break;
                            }

                            Error = outError;
                            return cmpResult;
                        }
                    }
                    else
                    {
                        Error += string.Format("Inserire la revisione per l'articolo - {0}", Articolo.ComponenteArticoloID);
                        Error += Environment.NewLine;
                        return false;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(Articolo.RisorsaID))
                    {
                        Error += string.Format("Inserire la risorsa per il reparto - {0}", Articolo.RepartoID);
                        Error += Environment.NewLine;

                        return false;
                    }
                    else
                    {
                        Error = string.Empty;

                        bool cmpResult = false;
                        string outError = string.Empty;
                        foreach (var cmp in Articolo.Componenti)
                        {
                            cmpResult = IsValidRevisione(cmp, ref outError);

                            if (!cmpResult)
                                break;
                        }

                        Error = outError;
                        return cmpResult;
                    }

                }
            }
        }
        else
        {
            if (Articolo.EReparto && (string.IsNullOrEmpty(Articolo.RisorsaID)))
            {
                Error += string.Format("Inserire la risorsa per il reparto - {0}", Articolo.RepartoID);
                Error += Environment.NewLine;

                return false;
            }
            else
            {
                Error = string.Empty;
                return true;
            }

        }
    }

    public bool DuplicateRevisione(tab_articolo Articolo)
    {
        var single = Get(Articolo.SocietaID, Articolo.ID, Articolo.RevisioneID ?? string.Empty, null);

        if (single == null)
            return false;

        var result = SaveDuplicate(Articolo.SocietaID, Articolo.ID, single.First(), 0, Articolo.RevisioneID + "_1");

        return true;
    }

    public bool DeleteRevisione(tab_articolo Articolo)
    {
        using var connection = GetOpenConnection();


        try
        {
            var result = connection.Execute(
                "DELETE FROM tab_articolo_composizione WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID",
                new { SocietaID = Articolo.SocietaID, ArticoloID = Articolo.ID, RevisioneID = Articolo.RevisioneID });

            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }

    }

    public long SaveDuplicate(string SocietaID, string? ArticoloID, tab_articolo_composizione Articolo, long Posizione, string? RevisioneID)
    {
        using var connection = GetOpenConnection();

        try
        {
            //clear
            if (Posizione == 0)
            {
                if (!Articolo.IsRoot)
                {
                    var result = connection.Execute(
                        "DELETE FROM tab_articolo_composizione WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID",
                        new { SocietaID = SocietaID, ArticoloID = ArticoloID, RevisioneID = Articolo.ComponenteRevisioneIDOld });
                }
            }

            //salva nuovo
            long? positionFather = (Posizione == 0) ? null : (long?)Posizione;

            foreach (var cmp in Articolo.Componenti)
            {
                //rinomina revisione
                if (cmp.ComponenteRevisioneID != cmp.ComponenteRevisioneIDOld)
                {
                    var update = connection.Execute(
                   "UPDATE tab_articolo_composizione SET ComponenteRevisioneID = @ComponenteRevisioneID WHERE SocietaID = @SocietaID AND ComponenteArticoloID = @ArticoloID AND ComponenteRevisioneID = @RevisioneID",
                   new { SocietaID = SocietaID, ArticoloID = cmp.ComponenteArticoloID, RevisioneID = cmp.ComponenteRevisioneIDOld, ComponenteRevisioneID = cmp.ComponenteRevisioneID });
                }

                var stageID = (cmp.EReparto) ? cmp.RepartoID : null;
                var componentID = (cmp.EReparto) ? null : cmp.ComponenteArticoloID;
                var componentRevisionID = (cmp.EReparto) ? null : ((cmp.HaComposizione) ? cmp.ComponenteRevisioneID : null);

                var result = connection.Execute(
               "INSERT INTO tab_articolo_composizione (SocietaID,ArticoloID,RevisioneID,ComposizioneID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID," +
               "ComposizioneIDPadre,RepartoID,ComponenteArticoloID,ComponenteRevisioneID,Posizione,Quantita,Tempo,RisorsaID,ESummary,EMilestone,DescrizioneMS,Note,Piazzamento)" +
               " OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RevisioneID,@ComposizioneID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID," +
             "@ComposizioneIDPadre,@RepartoID,@ComponenteArticoloID, @ComponenteRevisioneID,@Posizione,@Quantita,@Tempo,@RisorsaID,@ESummary,@EMilestone,@DescrizioneMS,@Note,@Piazzamento)",
               new tab_articolo_composizione
               {
                   SocietaID = SocietaID,
                   ArticoloID = ArticoloID!,
                   RevisioneID = RevisioneID!,
                   ComposizioneID = ++Posizione,
                   ComposizioneIDPadre = positionFather,
                   RepartoID = stageID,
                   ComponenteRevisioneID = componentRevisionID,
                   ComponenteArticoloID = componentID,
                   Quantita = cmp.Quantita,
                   Tempo = cmp.Tempo,
                   Piazzamento = cmp.Piazzamento,
                   RisorsaID = cmp.RisorsaID,
                   ESummary = cmp.ESummary,
                   EMilestone = cmp.EMilestone,
                   Note = cmp.Note,
                   DescrizioneMS = (cmp.ESummary || cmp.EMilestone) ? cmp.Descrizione : null,
                   LogUpdated = DateTime.Now,
               });

                long resultSave = SaveDuplicate(SocietaID, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? ArticoloID : componentID, cmp, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? Posizione : 0, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? RevisioneID : ((cmp.IsRoot) ? cmp.RevisioneID : cmp.ComponenteRevisioneID));

                if ((cmp.EReparto || cmp.EMilestone || cmp.ESummary))
                    Posizione = resultSave;
            }

            return (Articolo.Componenti.Any() ? Posizione : 1);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }


    }

    public long Save(string SocietaID, string? ArticoloID, tab_articolo_composizione Articolo, long Posizione, string? RevisioneID)
    {
        using var connection = GetOpenConnection();


        try
        {
            //clear
            if (Posizione == 0)
            {
                if (Articolo.IsRoot)
                {
                    var result = connection.Execute(
                        "DELETE FROM tab_articolo_composizione WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID",
                        new { SocietaID = SocietaID, ArticoloID = ArticoloID, RevisioneID = Articolo.RevisioneIDOld });
                }
                else
                {
                    var result = connection.Execute(
                        "DELETE FROM tab_articolo_composizione WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID",
                        new { SocietaID = SocietaID, ArticoloID = ArticoloID, RevisioneID = Articolo.ComponenteRevisioneIDOld });
                }
            }

            //salva nuovo
            long? positionFather = (Posizione == 0) ? null : (long?)Posizione;

            foreach (var cmp in Articolo.Componenti)
            {
                //rinomina revisione
                if (cmp.ComponenteRevisioneID != cmp.ComponenteRevisioneIDOld)
                {
                    var update = connection.Execute(
                   "UPDATE tab_articolo_composizione SET ComponenteRevisioneID = @ComponenteRevisioneID WHERE SocietaID = @SocietaID AND ComponenteArticoloID = @ArticoloID AND ComponenteRevisioneID = @RevisioneID",
                   new { SocietaID = SocietaID, ArticoloID = cmp.ComponenteArticoloID, RevisioneID = cmp.ComponenteRevisioneIDOld, ComponenteRevisioneID = cmp.ComponenteRevisioneID });
                }

                var stageID = (cmp.EReparto) ? cmp.RepartoID : null;
                var componentID = (cmp.EReparto) ? null : cmp.ComponenteArticoloID;
                var componentRevisionID = (cmp.EReparto) ? null : ((cmp.HaComposizione) ? cmp.ComponenteRevisioneID : null);

                ++Posizione;

                var result = connection.Execute(
               "INSERT INTO tab_articolo_composizione (SocietaID,ArticoloID,RevisioneID,ComposizioneID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID," +
               "ComposizioneIDPadre,RepartoID,ComponenteArticoloID,ComponenteRevisioneID,Posizione,Quantita,Tempo,RisorsaID,ESummary,EMilestone,DescrizioneMS,Note,Piazzamento)" +
               " OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RevisioneID,@ComposizioneID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID," +
             "@ComposizioneIDPadre,@RepartoID,@ComponenteArticoloID, @ComponenteRevisioneID,@Posizione,@Quantita,@Tempo,@RisorsaID,@ESummary,@EMilestone,@DescrizioneMS,@Note,@Piazzamento)",
               new tab_articolo_composizione
               {
                   SocietaID = SocietaID,
                   ArticoloID = ArticoloID ?? string.Empty,
                   RevisioneID = RevisioneID!,
                   ComposizioneID = Posizione,
                   ComposizioneIDPadre = positionFather,
                   RepartoID = stageID,
                   ComponenteRevisioneID = componentRevisionID,
                   ComponenteArticoloID = componentID,
                   Quantita = cmp.Quantita,
                   Tempo = cmp.Tempo,
                   Piazzamento = cmp.Piazzamento,
                   RisorsaID = cmp.RisorsaID,
                   ESummary = cmp.ESummary,
                   EMilestone = cmp.EMilestone,
                   Note = cmp.Note,
                   DescrizioneMS = (cmp.ESummary || cmp.EMilestone) ? cmp.Descrizione : null,
                   LogUpdated = DateTime.Now,
               });

                long resultSave = Save(SocietaID, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? ArticoloID : componentID, cmp, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? Posizione : 0, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? RevisioneID : ((cmp.IsRoot) ? cmp.RevisioneID : cmp.ComponenteRevisioneID));

                if ((cmp.EReparto || cmp.EMilestone || cmp.ESummary))
                    Posizione = resultSave;
            }

            return (Articolo.Componenti.Any() ? Posizione : 1);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }


    }

    public bool Exchange(string SocietaID, List<tab_articolo_composizione> Roots, string oArticoloID, string oRevisioneID, string nArticoloID, string? nRevisioneID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                bool retValue = true;

                foreach (var root in Roots)
                {
                    var rootComponents = connection.Query<tab_articolo_composizione>($@"SELECT c.* FROM tab_articolo_composizione AS c 
                            WHERE c.SocietaID = @SocietaID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID",
                        new { SocietaID = SocietaID, ArticoloID = root.ArticoloID, RevisioneID = root.RevisioneID }, transaction);

                    foreach (var component in rootComponents)
                    {
                        var result = connection.Execute(
                       "INSERT INTO tab_articolo_composizione (SocietaID,ArticoloID,RevisioneID,ComposizioneID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID," +
                       "ComposizioneIDPadre,RepartoID,ComponenteArticoloID,ComponenteRevisioneID,Posizione,Quantita,Tempo,RisorsaID,ESummary,EMilestone,DescrizioneMS,Note,Piazzamento)" +
                       " OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RevisioneID,@ComposizioneID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID," +
                       "@ComposizioneIDPadre,@RepartoID,@ComponenteArticoloID, @ComponenteRevisioneID,@Posizione,@Quantita,@Tempo,@RisorsaID,@ESummary,@EMilestone,@DescrizioneMS,@Note,@Piazzamento)",
                       new tab_articolo_composizione
                       {
                           SocietaID = SocietaID,
                           ArticoloID = component.ArticoloID,
                           RevisioneID = root.RevisioneNuova ?? "XX",
                           ComposizioneID = component.ComposizioneID,
                           ComposizioneIDPadre = component.ComposizioneIDPadre,
                           RepartoID = component.RepartoID,
                           ComponenteRevisioneID = (component.ComponenteArticoloID == oArticoloID && component.ComponenteRevisioneID == oRevisioneID) ? nRevisioneID : component.ComponenteRevisioneID,
                           ComponenteArticoloID = (component.ComponenteArticoloID == oArticoloID && component.ComponenteRevisioneID == oRevisioneID) ? nArticoloID : component.ComponenteArticoloID,
                           Quantita = (component.ComponenteArticoloID == oArticoloID && component.ComponenteRevisioneID == oRevisioneID) ? root.QuantitaNuova : component.Quantita,
                           Tempo = component.Tempo,
                           Piazzamento = component.Piazzamento,
                           RisorsaID = component.RisorsaID,
                           ESummary = component.ESummary,
                           EMilestone = component.EMilestone,
                           Note = component.Note,
                           DescrizioneMS = (component.ESummary || component.EMilestone) ? component.Descrizione : null,
                           LogUpdated = DateTime.Now,
                       }, transaction);
                    }

                    retValue = ExchangeDependency(SocietaID, connection, transaction, root);
                }

                transaction.Commit();
                return retValue;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool ExchangeDependency(string SocietaID, IDbConnection Connection, IDbTransaction Transaction, tab_articolo_composizione Root)
    {
        try
        {
            bool retValue = true;

            foreach (var dependency in Root.Dipendenze ?? new ObservableCollection<tab_articolo_composizione>())
            {
                var dependencyComponents = Connection.Query<tab_articolo_composizione>($@"SELECT c.* FROM tab_articolo_composizione AS c 
                            WHERE c.SocietaID = @SocietaID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID",
                                new { SocietaID = SocietaID, ArticoloID = dependency.ArticoloID, RevisioneID = dependency.RevisioneID }, Transaction);

                foreach (var component in dependencyComponents)
                {
                    var result = Connection.Execute(
                   "INSERT INTO tab_articolo_composizione (SocietaID,ArticoloID,RevisioneID,ComposizioneID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID," +
                   "ComposizioneIDPadre,RepartoID,ComponenteArticoloID,ComponenteRevisioneID,Posizione,Quantita,Tempo,RisorsaID,ESummary,EMilestone,DescrizioneMS,Note,Piazzamento)" +
                   " OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RevisioneID,@ComposizioneID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID," +
                 "@ComposizioneIDPadre,@RepartoID,@ComponenteArticoloID, @ComponenteRevisioneID,@Posizione,@Quantita,@Tempo,@RisorsaID,@ESummary,@EMilestone,@DescrizioneMS,@Note,@Piazzamento)",
                   new tab_articolo_composizione
                   {
                       SocietaID = SocietaID,
                       ArticoloID = component.ArticoloID,
                       RevisioneID = dependency.RevisioneNuova ?? "XX",
                       ComposizioneID = component.ComposizioneID,
                       ComposizioneIDPadre = component.ComposizioneIDPadre,
                       RepartoID = component.RepartoID,
                       ComponenteRevisioneID = (component.ComponenteArticoloID == Root.ArticoloID && component.ComponenteRevisioneID == Root.RevisioneID) ? Root.RevisioneNuova : component.ComponenteRevisioneID,
                       ComponenteArticoloID = (component.ComponenteArticoloID == Root.ArticoloID && component.ComponenteRevisioneID == Root.RevisioneID) ? Root.ArticoloID : component.ComponenteArticoloID,
                       Quantita = (component.ComponenteArticoloID == Root.ArticoloID && component.ComponenteRevisioneID == Root.RevisioneID) ? dependency.QuantitaNuova : component.Quantita,
                       Tempo = component.Tempo,
                       Piazzamento = component.Piazzamento,
                       RisorsaID = component.RisorsaID,
                       ESummary = component.ESummary,
                       EMilestone = component.EMilestone,
                       Note = component.Note,
                       DescrizioneMS = (component.ESummary || component.EMilestone) ? component.Descrizione : null,
                       LogUpdated = DateTime.Now,
                   }, Transaction);
                }

                retValue = ExchangeDependency(SocietaID, Connection, Transaction, dependency);
            }

            return retValue;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
}