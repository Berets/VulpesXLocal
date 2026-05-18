using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Models.Models.Production;

namespace VulpesX.DAL.Production;

public interface Ipro_ordine_composizioneRepository
{
    ObservableCollection<pro_ordine_composizione>? GetMaterialsListByOrder(string SocietaID, string OrdineID);

    ObservableCollection<pro_ordine_composizione>? GetHalfmadeListByOrder(string SocietaID, string OrdineID);

    ObservableCollection<pro_ordine_composizione>? GetHalfmadeListByOrderRecursive(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, ref int Position);

    ObservableCollection<pro_ordine_composizione>? GetListByOrderAndProduct(string SocietaID, string OrdineID, string ProductID);

    Tuple<ObservableCollection<pro_ordine_composizione>, ObservableCollection<pro_ordine_composizione>>? GetGantt(string SocietaID, pro_ordine_composizione? Padre, ObservableCollection<tab_produzione_calendario_chiusura> Stops);

    ObservableCollection<pro_ordine_composizione>? GetGanttComponents(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, pro_ordine_composizione Padre, ObservableCollection<tab_produzione_calendario_chiusura> Stops, ref ObservableCollection<pro_ordine_composizione> Resources);

    ObservableCollection<pro_ordine_composizione>? GetGanttStages(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, pro_ordine_composizione? Padre, ObservableCollection<tab_produzione_calendario_chiusura> Stops, ref ObservableCollection<pro_ordine_composizione> Resources);


    ObservableCollection<pro_ordine_composizione>? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, pro_ordine_composizione? Padre);

    pro_ordine_composizione? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID);

    ObservableCollection<pro_ordine_composizione>? GetComponents(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, pro_ordine_composizione Padre);

    ObservableCollection<pro_ordine_composizione>? GetStages(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, pro_ordine_composizione Padre);


    void GetTimes(pro_ordine_composizione Task);

    long Save(string SocietaID, string OrdineID, decimal Quantita, string OrderUM, string? PadreID, string? PadreRevisioneID, tab_articolo_composizione Articolo, long Posizione);

    bool UpdateNote(pro_ordine_composizione Model);

    bool UpdateHalfmade(string CompanyID, string OrdineID, string ArticoloID, string RevisioneID, decimal NewQuantity);

    bool Exists(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID);

    #region CRUD
    // rimuovere UniqueID dalle query perchè DA' ERRORE [github https://github.com/DapperLib/Dapper/issues/2024]!!!
    // aggiungere [] a UID
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(pro_ordine_composizione Model);

    bool Update(pro_ordine_composizione Model);

    bool Delete(pro_ordine_composizione Model);

    string? Validate(pro_ordine_composizione Model, bool IsInsert);
    #endregion
}

public class pro_ordine_composizioneRepository : RepositoryBase, Ipro_ordine_composizioneRepository
{
    public pro_ordine_composizioneRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<pro_ordine_composizione>? GetMaterialsListByOrder(string SocietaID, string OrdineID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return new ObservableCollection<pro_ordine_composizione>(connection.Query<pro_ordine_composizione>(
                $@"SELECT c.*, p.IsInfinite AS ProductIsInfinite FROM pro_ordine_composizione AS c
                        INNER JOIN tab_articolo AS p ON p.SocietaID = c.SocietaID AND p.ID = c.ComponenteArticoloID
                    WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ComponenteArticoloID IS NOT NULL AND c.ComponenteRevisioneID IS NULL;",
                new { SocietaID = SocietaID, OrdineID = OrdineID }));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione>? GetHalfmadeListByOrder(string SocietaID, string OrdineID)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new List<pro_ordine_composizione>();

            var order = connection.Query<pro_ordine>(
                $@"SELECT o.* FROM pro_ordine AS o 
                    WHERE o.SocietaID = @SocietaID AND o.ID = @oid",
          new { SocietaID = SocietaID, oid = OrdineID }).FirstOrDefault();

            if (order == null)
                return null;

            var hms = new ObservableCollection<pro_ordine_composizione>(connection.Query<pro_ordine_composizione>(
                $@"SELECT c.*, p.IsInfinite AS ProductIsInfinite FROM pro_ordine_composizione AS c
                        INNER JOIN tab_articolo AS p ON p.SocietaID = c.SocietaID AND p.ID = c.ComponenteArticoloID
                    WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID AND c.ComponenteArticoloID IS NOT NULL AND c.ComponenteRevisioneID IS NOT NULL;",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = order.ArticoloID, RevisioneID = order.RevisioneID }));

            // posizione 
            int posizione = 1;
            foreach (var hm in hms)
            {
                hm.Posizione = posizione;

                retValue.Add(hm);
            }

            foreach (var hm in hms)
            {
                retValue.AddRange(GetHalfmadeListByOrderRecursive(hm.SocietaID, hm.OrdineID, hm.ComponenteArticoloID ?? string.Empty, hm.ComponenteRevisioneID ?? string.Empty, ref posizione) ?? new ObservableCollection<pro_ordine_composizione>());
            }

            return new ObservableCollection<pro_ordine_composizione>(retValue);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione>? GetHalfmadeListByOrderRecursive(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, ref int Position)
    {
        try
        {
            using var connection = GetOpenConnection();


            var retValue = new List<pro_ordine_composizione>();

            var hms = new ObservableCollection<pro_ordine_composizione>(connection.Query<pro_ordine_composizione>(
                $@"SELECT c.*, p.IsInfinite AS ProductIsInfinite FROM pro_ordine_composizione AS c
                        INNER JOIN tab_articolo AS p ON p.SocietaID = c.SocietaID AND p.ID = c.ComponenteArticoloID
                    WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID AND c.ComponenteArticoloID IS NOT NULL AND c.ComponenteRevisioneID IS NOT NULL;",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID }));

            // posizione 
            Position = Position + 1;
            foreach (var hm in hms)
            {
                hm.Posizione = Position;

                retValue.Add(hm);
            }

            foreach (var hm in hms)
            {
                retValue.AddRange(GetHalfmadeListByOrderRecursive(hm.SocietaID, hm.OrdineID, hm.ComponenteArticoloID ?? string.Empty, hm.ComponenteRevisioneID ?? string.Empty, ref Position) ?? new ObservableCollection<pro_ordine_composizione>());
            }

            return new ObservableCollection<pro_ordine_composizione>(retValue);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione>? GetListByOrderAndProduct(string SocietaID, string OrdineID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return new ObservableCollection<pro_ordine_composizione>(connection.Query<pro_ordine_composizione>(
                $@"SELECT * FROM pro_ordine_composizione
                    WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID;",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ProductID }));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public Tuple<ObservableCollection<pro_ordine_composizione>, ObservableCollection<pro_ordine_composizione>>? GetGantt(string SocietaID, pro_ordine_composizione? Padre, ObservableCollection<tab_produzione_calendario_chiusura> Stops)
    {
        try
        {
            using var connection = GetOpenConnection();

            ObservableCollection<pro_ordine_composizione> tupleOrders = new ObservableCollection<pro_ordine_composizione>();
            ObservableCollection<pro_ordine_composizione> tupleResources = new ObservableCollection<pro_ordine_composizione>();



            var orders = connection.Query<pro_ordine, ABE, tab_articolo, string, pro_ordine_composizione>($@"SELECT o.*, cli.* , art.*, IsNull((SELECT TOP 1 tem.TipoID FROM pro_ordine_composizione_tempo as tem WHERE tem.OrdineID = o.ID) , '0') as ut FROM pro_ordine AS o 
                                                                                                                  LEFT OUTER JOIN ABE AS cli ON o.ClienteID = cli.abecod 
                                                                                                                  LEFT OUTER JOIN tab_articolo AS art ON o.ArticoloID = art.ID 
                                                                                                                  WHERE o.SocietaID = @SocietaID  AND (o.Stato = 'A' or o.Stato = 'O' or o.Stato = 'S')
                                                                                                                  ORDER BY o.DataConsegna"
          , (ordine, cliente, articolo, tempo) =>
          {
              var ordineComposizione = new pro_ordine_composizione()
              {
                  SocietaID = SocietaID,
                  RevisioneID = string.Empty,
                  UniqueId = ordine.ID.ToString(),
                  OrdineID = ordine.ID,
                  IsSummarize = true,
                  Description = string.Format("({0}) - {1} - {2} - {3}", ordine.Quantita, ordine.ID, cliente.abers1, articolo.Descrizione),
                  Start = DateTime.Now,
                  End = DateTime.Now.AddDays(1),
                  Duration = new TimeSpan(10, 0, 0),
                  PosizioneGantt = 0,
                  DataConsegna = ordine.DataConsegna,
                  UltimoTempoTipoID = tempo,
                  ArticoloID = ordine.ArticoloID ?? string.Empty,
              };
              return ordineComposizione;
          },
          new { SocietaID = SocietaID },
              splitOn: "abecod,SocietaID,ut");

            foreach (var ord in orders.Take(20))
            {
                var roots = connection.Query<pro_ordine_composizione, pro_ordine, tab_articolo, tab_produzione_risorsa, tab_produzione_reparto, pro_ordine_composizione>(
                                        $@"SELECT c.*, o.*, a.Descrizione, r.*, w.* FROM pro_ordine_composizione AS c 
                                                LEFT OUTER JOIN pro_ordine AS o ON c.SocietaID = o.SocietaID AND c.OrdineID = o.ID 
                                                LEFT OUTER JOIN tab_articolo AS a ON a.SocietaID = o.SocietaID AND a.ID = c.ComponenteArticoloID 
                                                LEFT OUTER JOIN tab_produzione_risorsa AS r ON r.SocietaID = o.SocietaID AND r.ID = c.RisorsaID 
                                                LEFT OUTER JOIN tab_produzione_reparto AS w ON w.SocietaID = o.SocietaID AND w.ID = c.RepartoID 
                                                WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre is NULL "
                , (composizione, ordine, componente, risorsa, reparto) =>
                {
                    composizione.UniqueId = composizione.UID.ToString();
                    composizione.IsSummarize = (string.IsNullOrEmpty(composizione.RepartoID) || composizione.ESummary) && !composizione.EMilestone;
                    composizione.IsMilestone = composizione.EMilestone;
                    composizione.Padre = ord;
                    composizione.Description = string.IsNullOrEmpty(composizione.RepartoID) ? ((composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : componente.Descrizione) : $"{reparto.Descrizione} - {risorsa?.Descrizione} - ({composizione.Quantita})";
                    composizione.Title = string.IsNullOrEmpty(composizione.RepartoID) ? ((composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : componente.Descrizione) : $"{reparto.Descrizione} - {risorsa?.Descrizione} - ({composizione.Quantita})";
                    composizione.Start = DateTime.Now;
                    composizione.End = DateTime.Now.Add(new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0)));
                    composizione.EffectiveDuration = new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0));
                    composizione.Duration = new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0));
                    composizione.ResourceID = composizione.RisorsaID;
                    composizione.DataConsegna = ordine.DataConsegna;
                    composizione.ETerminato = false;
                    composizione.EFisso = composizione.EFisso ?? false;
                    composizione.Stops = !string.IsNullOrEmpty(composizione.RisorsaID) ? new ObservableCollection<tab_produzione_calendario_chiusura>(Stops.Where(o => o.RisorsaID == composizione.RisorsaID)) : new ObservableCollection<tab_produzione_calendario_chiusura>();

                    return composizione;
                },
                new { SocietaID = SocietaID, OrdineID = ord.OrdineID, ArticoloID = ord.ArticoloID },
                    splitOn: "SocietaID,Descrizione,SocietaID,SocietaID");

                foreach (var chl in roots)
                {
                    bool isOnlyComponent = false;

                    var temps = new List<pro_ordine_composizione>();
                    tupleResources.Add(chl);

                    if (!chl.EReparto && !chl.ESummary && !chl.EMilestone)
                    {
                        if (!string.IsNullOrEmpty(chl.ComponenteRevisioneID))
                        {
                            foreach (var cmp in GetGanttComponents(chl.SocietaID, chl.OrdineID, chl.ComponenteArticoloID ?? string.Empty, chl.ComponenteRevisioneID, chl, Stops, ref tupleResources) ?? new ObservableCollection<pro_ordine_composizione>())
                            {
                                chl.Children.Add(cmp);
                            }

                            chl.Start = chl.Children.OrderBy(o => o.Start).Select(s => s.Start).FirstOrDefault();
                            chl.End = chl.Children.OrderByDescending(o => o.End).Select(s => s.End).FirstOrDefault();
                        }
                        else
                        {
                            isOnlyComponent = true;
                        }
                    }
                    else
                    {
                        foreach (var stg in GetGanttStages(chl.SocietaID, chl.OrdineID, chl.ArticoloID, chl.RevisioneID, chl.ComposizioneID, Padre, Stops, ref tupleResources) ?? new ObservableCollection<pro_ordine_composizione>())
                        {
                            if (chl.IsSummarize)
                                chl.Children.Add(stg);
                            else
                                temps.Add(stg);
                        }

                        if (chl.IsSummarize && chl.Children.Any())
                            chl.Duration = chl.Children.Min(o => o.End) - chl.Children.Min(o => o.Start);

                    }

                    if (!chl.IsSummarize && ord.Children.Cast<pro_ordine_composizione>().Where(o => o.IsSummarize).Any())
                    {
                        foreach (var item in ord.Children.Cast<pro_ordine_composizione>().Where(o => o.IsSummarize))
                        {
                            item.DependantTasks.Add(chl);
                            chl.Dependencies.Add(new pro_ordine_composizioneDependency { FromTask = item });
                        }
                    }

                    if (chl.IsSummarize && !chl.Children.Any())
                        chl.IsMilestone = true;

                    if (!isOnlyComponent)
                        ord.Children.Add(chl);

                    pro_ordine_composizione beforeTask = chl;
                    bool isFirst = true;
                    foreach (var item in temps)
                    {
                        if (beforeTask != null && isFirst)
                        {
                            beforeTask.DependantTasks.Add(item);
                            item.Dependencies.Add(new pro_ordine_composizioneDependency { FromTask = beforeTask });
                        }

                        ord.Children.Add(item);

                        beforeTask = item;

                        isFirst = false;
                    }
                }

                ord.Start = ord.Children.OrderBy(o => o.Start).Select(s => s.Start).FirstOrDefault();
                ord.End = ord.Children.OrderByDescending(o => o.End).Select(s => s.End).FirstOrDefault();
                ord.Duration = ord.End - ord.Start;

                tupleOrders.Add(ord);
            }

            return new Tuple<ObservableCollection<pro_ordine_composizione>, ObservableCollection<pro_ordine_composizione>>(tupleOrders, tupleResources);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<ObservableCollection<pro_ordine_composizione>, ObservableCollection<pro_ordine_composizione>>(new ObservableCollection<pro_ordine_composizione>(), new ObservableCollection<pro_ordine_composizione>());
        }
    }

    public ObservableCollection<pro_ordine_composizione>? GetGanttComponents(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, pro_ordine_composizione Padre, ObservableCollection<tab_produzione_calendario_chiusura> Stops, ref ObservableCollection<pro_ordine_composizione> Resources)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<pro_ordine_composizione>();

            var list = connection.Query<pro_ordine_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, pro_ordine, pro_ordine_composizione>(
                "SELECT c.*,rep.Descrizione,ris.Descrizione, com.*, uni.Descrizione, o.* FROM pro_ordine_composizione AS c " +
                "LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID " +
                "LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID " +
                "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID " +
                "LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID " +
                "LEFT OUTER JOIN pro_ordine AS o ON c.SocietaID = o.SocietaID AND c.OrdineID = o.ID " +
                "WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre is null"
                , (composizione, reparto, risorsa, componente, unita, ordine) =>
                {
                    composizione.UniqueId = composizione.UID.ToString();
                    composizione.Padre = Padre;
                    composizione.Description = string.IsNullOrEmpty(composizione.RepartoID) ? ((composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : componente.Descrizione) : $"{reparto.Descrizione} - {risorsa.Descrizione} - ({composizione.Quantita})";
                    composizione.Title = string.IsNullOrEmpty(composizione.RepartoID) ? ((composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : componente.Descrizione) : $"{reparto.Descrizione} - {risorsa.Descrizione} - ({composizione.Quantita})";
                    composizione.EComponente = true;
                    composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                    composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                    composizione.Start = DateTime.Now;
                    composizione.End = DateTime.Now.Add(new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0)));
                    composizione.EffectiveDuration = new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0));
                    composizione.Duration = new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0));
                    composizione.ResourceID = composizione.RisorsaID;
                    composizione.DataConsegna = ordine.DataConsegna;
                    composizione.Stops = !string.IsNullOrEmpty(composizione.RisorsaID) ? new ObservableCollection<tab_produzione_calendario_chiusura>(Stops.Where(o => o.RisorsaID == composizione.RisorsaID)) : new ObservableCollection<tab_produzione_calendario_chiusura>();

                    return composizione;
                },
                new { SocietaID = SocietaID, OrdineID = OrdineID, RevisioneID = RevisioneID, ArticoloID = ArticoloID },
                    splitOn: "Descrizione,Descrizione,SocietaID,Descrizione,SocietaID");

            foreach (var component in list)
            {
                bool isOnlyComponent = false;

                var temps = new List<pro_ordine_composizione>();
                Resources.Add(component);
                GetTimes(component);

                if (!component.EReparto && !component.ESummary && !component.EMilestone)
                {
                    if (!string.IsNullOrEmpty(component.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetGanttComponents(component.SocietaID, component.OrdineID, component.ComponenteArticoloID ?? string.Empty, component.ComponenteRevisioneID, component, Stops, ref Resources) ?? new ObservableCollection<pro_ordine_composizione>())
                        {
                            component.Children.Add(cmp);

                            component.Start = component.Children.OrderBy(o => o.Start).Select(s => s.Start).FirstOrDefault();
                            component.End = component.Children.OrderByDescending(o => o.End).Select(s => s.End).FirstOrDefault();
                        }
                    }
                    else
                    {
                        isOnlyComponent = true;
                    }
                }
                else
                {
                    foreach (var stg in GetGanttStages(component.SocietaID, component.OrdineID, component.ArticoloID, component.RevisioneID, component.ComposizioneID, Padre, Stops, ref Resources) ?? new ObservableCollection<pro_ordine_composizione>())
                    {
                        if (component.IsSummarize)
                            component.Children.Add(stg);
                        else
                            temps.Add(stg);
                    }
                }


                if (!component.IsSummarize && retValue.Where(o => o.IsSummarize).Any())
                {
                    foreach (var item in retValue.Where(o => o.IsSummarize))
                    {
                        item.DependantTasks.Add(component);
                        component.Dependencies.Add(new pro_ordine_composizioneDependency { FromTask = item });
                    }
                }

                if (component.IsSummarize && !component.Children.Any())
                    component.IsMilestone = true;

                if (!isOnlyComponent)
                    retValue.Add(component);

                pro_ordine_composizione beforeTask = component;
                foreach (var item in temps)
                {
                    if (beforeTask != null)
                    {
                        beforeTask.DependantTasks.Add(item);
                        item.Dependencies.Add(new pro_ordine_composizioneDependency { FromTask = beforeTask });
                    }

                    retValue.Add(item);

                    beforeTask = item;
                }
            }

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione>? GetGanttStages(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, pro_ordine_composizione? Padre, ObservableCollection<tab_produzione_calendario_chiusura> Stops, ref ObservableCollection<pro_ordine_composizione> Resources)
    {
        try
        {
            using var connection = GetOpenConnection();


            var retValue = new ObservableCollection<pro_ordine_composizione>();

            var list = connection.Query<pro_ordine_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, pro_ordine, pro_ordine_composizione>(
                "SELECT c.*,rep.Descrizione,ris.Descrizione,com.*,uni.Descrizione, o.* FROM pro_ordine_composizione AS c " +
                "LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID " +
                "LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID " +
                "LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID " +
                "LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID " +
                "LEFT OUTER JOIN pro_ordine AS o ON c.SocietaID = o.SocietaID AND c.OrdineID = o.ID " +
                "WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneIDPadre = @ComposizioneID",
                (composizione, reparto, risorsa, componente, unita, ordine) =>
                {
                    composizione.UniqueId = composizione.UID.ToString();
                    composizione.Padre = Padre;
                    composizione.Description = string.IsNullOrEmpty(composizione.RepartoID) ? ((composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : componente.Descrizione) : $"{reparto.Descrizione} - {risorsa.Descrizione} - ({composizione.Quantita})";
                    composizione.Title = string.IsNullOrEmpty(composizione.RepartoID) ? ((composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : componente.Descrizione) : $"{reparto.Descrizione} - {risorsa.Descrizione} - ({composizione.Quantita})";
                    composizione.EComponente = true;
                    composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                    composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                    composizione.Start = DateTime.Now;
                    composizione.End = DateTime.Now.Add(new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0)));
                    composizione.EffectiveDuration = new TimeSpan((composizione.Tempo ?? 0) + ((risorsa != null) ? ((risorsa?.EPiazzamento ?? false) ? composizione.Piazzamento ?? 0 : 0) : 0));
                    composizione.Duration = new TimeSpan((composizione.Tempo ?? 0) + (composizione.Piazzamento ?? 0));
                    composizione.ResourceID = composizione.RisorsaID;
                    composizione.UnitaDescrizione = unita?.Descrizione;
                    composizione.DataConsegna = ordine.DataConsegna;
                    composizione.Stops = new ObservableCollection<tab_produzione_calendario_chiusura>(Stops.Where(o => o.RisorsaID == composizione?.RisorsaID));

                    return composizione;
                },
                new { SocietaID = SocietaID, OrdineID = OrdineID, RevisioneID = RevisioneID, ArticoloID = ArticoloID, ComposizioneID = ComposizioneID },
                    splitOn: "Descrizione,Descrizione,SocietaID,Descrizione,SocietaID");

            foreach (var component in list)
            {
                bool isOnlyComponent = false;
                var temps = new List<pro_ordine_composizione>();

                Resources.Add(component);
                GetTimes(component);

                if (!component.EReparto && !component.ESummary && !component.EMilestone)
                {
                    if (!string.IsNullOrEmpty(component.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetGanttComponents(component.SocietaID, component.OrdineID, component.ComponenteArticoloID ?? string.Empty, component.ComponenteRevisioneID, component, Stops, ref Resources) ?? new ObservableCollection<pro_ordine_composizione>())
                        {
                            component.Children.Add(cmp);
                        }
                    }
                    else
                    {
                        isOnlyComponent = true;
                    }
                }
                else
                {
                    foreach (var stg in GetGanttStages(component.SocietaID, component.OrdineID, component.ArticoloID, component.RevisioneID, component.ComposizioneID, Padre, Stops, ref Resources) ?? new ObservableCollection<pro_ordine_composizione>())
                    {
                        if (component.IsSummarize)
                            component.Children.Add(stg);
                        else
                            temps.Add(stg);
                    }
                }

                if (!component.IsSummarize && retValue.Where(o => o.IsSummarize).Any())
                {
                    foreach (var item in retValue.Where(o => o.IsSummarize))
                    {
                        item.DependantTasks.Add(component);
                        component.Dependencies.Add(new pro_ordine_composizioneDependency { FromTask = item });
                    }
                }

                if (component.IsSummarize && !component.Children.Any())
                    component.IsMilestone = true;

                if (!isOnlyComponent)
                    retValue.Add(component);

                pro_ordine_composizione beforeTask = component;
                foreach (var item in temps)
                {
                    if (beforeTask != null)
                    {
                        beforeTask.DependantTasks.Add(item);
                        item.Dependencies.Add(new pro_ordine_composizioneDependency { FromTask = beforeTask });
                    }

                    retValue.Add(item);

                    beforeTask = item;
                }

            }

            return retValue;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }


    public ObservableCollection<pro_ordine_composizione>? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, pro_ordine_composizione? Padre)
    {
        try
        {
            using var connection = GetOpenConnection();

            var articoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();

            var retValue = new ObservableCollection<pro_ordine_composizione>();

            var article = articoloRepo.Get(SocietaID, ArticoloID);

            if (article == null)
                return null;

            var finished = new pro_ordine_composizione
            {
                SocietaID = SocietaID,
                OrdineID = string.Empty,
                ArticoloID = article.ID,
                RevisioneID = RevisioneID,
                DescrizioneMS = article.Descrizione,
                Padre = Padre,
                EComponente = false,
                EMilestone = false,
                ESummary = false,
                EEspanso = true,
            };

            var childrens = connection.Query<pro_ordine_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, string, pro_ordine_composizione>(
               $@"SELECT c.*,rep.Descrizione,ris.Descrizione, com.*, uni.Descrizione, COALESCE(utt.TipoID,'0') TipoID FROM pro_ordine_composizione AS c 
                    OUTER APPLY(SELECT TOP(1) ut.TipoID from pro_ordine_composizione_tempo as ut where ut.SocietaID = c.SocietaID AND ut.OrdineID = c.OrdineID AND ut.ArticoloID = c.ArticoloID AND ut.RevisioneID = c.RevisioneID AND ut.ComposizioneID = c.ComposizioneID order by ut.Data desc) utt 
                    LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID 
                    LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                    LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID 
                    LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID 
                   WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID AND c.ComposizioneIDPadre is null"
               , (composizione, reparto, risorsa, componente, unita, ultimo_tipo) =>
               {
                   composizione.Padre = finished;
                   composizione.DescrizioneMS = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto?.Descrizione : componente?.Descrizione;
                   composizione.EComponente = true;
                   composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                   composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                   composizione.UltimoTempoTipoID = ultimo_tipo;
                   composizione.RisorsaDescrizione = risorsa?.Descrizione;
                   composizione.UnitaDescrizione = unita?.Descrizione;

                   return composizione;
               },
               new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID },
                   splitOn: "Descrizione,Descrizione,SocietaID,Descrizione,TipoID");

            foreach (var chl in childrens)
            {
                if (!chl.EReparto && !chl.ESummary && !chl.EMilestone)
                {
                    if (!string.IsNullOrEmpty(chl.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(chl.SocietaID, chl.OrdineID, chl.ComponenteArticoloID ?? string.Empty, chl.ComponenteRevisioneID, chl) ?? new ObservableCollection<pro_ordine_composizione>())
                        {
                            chl.Children.Add(cmp);
                        }
                    }
                }
                else
                {
                    foreach (var stg in GetStages(chl.SocietaID, chl.OrdineID, chl.ArticoloID, chl.RevisioneID, chl.ComposizioneID, finished) ?? new ObservableCollection<pro_ordine_composizione>())
                    {
                        chl.Children.Add(stg);
                    }
                }

                finished.Children.Add(chl);
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

    public pro_ordine_composizione? Get(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<pro_ordine_composizione>(@"SELECT * FROM pro_ordine_composizione
                                                                WHERE SocietaID=@SocietaID AND OrdineID=@OrdineID AND ArticoloID=@ArticoloID AND RevisioneID=@RevisioneID AND ComposizioneID=@ComposizioneID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID, ComposizioneID = ComposizioneID }).FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<pro_ordine_composizione>? GetComponents(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, pro_ordine_composizione Padre)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<pro_ordine_composizione>();

            var list = connection.Query<pro_ordine_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, string, pro_ordine_composizione>(
                $@"SELECT c.*,rep.Descrizione,ris.Descrizione, com.*, uni.Descrizione, COALESCE(utt.TipoID,'0') TipoID  FROM pro_ordine_composizione AS c 
                    OUTER APPLY(SELECT TOP(1) ut.TipoID from pro_ordine_composizione_tempo as ut where ut.SocietaID = c.SocietaID AND ut.OrdineID = c.OrdineID AND ut.ArticoloID = c.ArticoloID AND ut.RevisioneID = c.RevisioneID AND ut.ComposizioneID = c.ComposizioneID order by ut.Data desc) utt
                    LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID 
                    LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                    LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID 
                    LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID 
                    WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID AND c.ComposizioneIDPadre is null"
                , (composizione, reparto, risorsa, componente, unita, ultimo_tipo) =>
                {
                    composizione.OrdineID = composizione.OrdineID;
                    composizione.Padre = Padre;
                    composizione.DescrizioneMS = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto.Descrizione : componente.Descrizione;
                    composizione.EComponente = true;
                    composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                    composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                    composizione.UltimoTempoTipoID = ultimo_tipo;
                    composizione.RisorsaDescrizione = risorsa?.Descrizione;
                    composizione.UnitaDescrizione = unita?.Descrizione;

                    return composizione;
                },
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID },
                    splitOn: "Descrizione,Descrizione,SocietaID,Descrizione, TipoID");

            foreach (var component in list)
            {
                if (!component.EReparto && !component.ESummary && !component.EMilestone)
                {
                    if (!string.IsNullOrEmpty(component.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(component.SocietaID, component.OrdineID, component.ComponenteArticoloID ?? string.Empty, component.ComponenteRevisioneID, component) ?? new ObservableCollection<pro_ordine_composizione>())
                        {
                            component.Children.Add(cmp);
                        }
                    }
                }
                else
                {
                    foreach (var stg in GetStages(component.SocietaID, component.OrdineID, component.ArticoloID, component.RevisioneID, component.ComposizioneID, Padre) ?? new ObservableCollection<pro_ordine_composizione>())
                    {
                        component.Children.Add(stg);
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

    public ObservableCollection<pro_ordine_composizione>? GetStages(string SocietaID, string OrdineID, string ArticoloID, string RevisioneID, long ComposizioneID, pro_ordine_composizione Padre)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<pro_ordine_composizione>();

            var list = connection.Query<pro_ordine_composizione, tab_produzione_reparto, tab_produzione_risorsa, tab_articolo, tab_articolo_unita, string, pro_ordine_composizione>(
                $@"SELECT c.*,rep.Descrizione,ris.Descrizione,com.*, uni.Descrizione, COALESCE(utt.TipoID,'0') TipoID  FROM pro_ordine_composizione AS c 
                    OUTER APPLY(SELECT TOP(1) ut.TipoID from pro_ordine_composizione_tempo as ut where ut.SocietaID = c.SocietaID AND ut.OrdineID = c.OrdineID AND ut.ArticoloID = c.ArticoloID AND ut.RevisioneID = c.RevisioneID AND ut.ComposizioneID = c.ComposizioneID  order by ut.Data desc) utt
                    LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID 
                    LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID 
                    LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID 
                    LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID 
                    WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.ArticoloID = @ArticoloID AND c.RevisioneID = @RevisioneID AND c.ComposizioneIDPadre = @ComposizioneID",
                (composizione, reparto, risorsa, componente, unita, ultimo_tipo) =>
                {
                    composizione.Padre = Padre;
                    composizione.DescrizioneMS = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? reparto.Descrizione : componente.Descrizione;
                    composizione.EComponente = true;
                    composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                    composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                    composizione.UltimoTempoTipoID = ultimo_tipo;
                    composizione.RisorsaDescrizione = risorsa?.Descrizione;
                    composizione.UnitaDescrizione = unita?.Descrizione;

                    return composizione;
                },
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID, ComposizioneID = ComposizioneID },
                    splitOn: "Descrizione,Descrizione,SocietaID,Descrizione, TipoID");

            foreach (var component in list)
            {
                if (!component.EReparto && !component.ESummary && !component.EMilestone)
                {
                    if (!string.IsNullOrEmpty(component.ComponenteRevisioneID))
                    {
                        foreach (var cmp in GetComponents(component.SocietaID, component.OrdineID, component.ComponenteArticoloID ?? string.Empty, component.ComponenteRevisioneID, component) ?? new ObservableCollection<pro_ordine_composizione>())
                        {
                            component.Children.Add(cmp);
                        }
                    }
                }
                else
                {
                    foreach (var stg in GetStages(component.SocietaID, component.OrdineID, component.ArticoloID, component.RevisioneID, component.ComposizioneID, Padre) ?? new ObservableCollection<pro_ordine_composizione>())
                    {
                        component.Children.Add(stg);
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


    public void GetTimes(pro_ordine_composizione Task)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<pro_ordine_composizione>();

            var times = connection.Query<pro_ordine_composizione_tempo>(
                "SELECT * FROM pro_ordine_composizione_tempo as c " +
                "WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID AND c.RevisioneID = @RevisioneID AND c.ArticoloID = @ArticoloID AND c.ComposizioneID =@ComposizioneID",
                new { SocietaID = Task.SocietaID, OrdineID = Task.OrdineID, RevisioneID = Task.RevisioneID, ArticoloID = Task.ArticoloID, ComposizioneID = Task.ComposizioneID });

            foreach (var time in times.Where(o => o.EProcessata == true))
            {
                var fix = new pro_ordine_composizione_tempo
                {
                    SocietaID = time.SocietaID,
                    RevisioneID = time.RevisioneID,
                    OrdineID = time.OrdineID,
                    ArticoloID = time.ArticoloID,
                    ComposizioneID = time.ComposizioneID,
                    ProgressivoID = time.ProgressivoID,
                    Start = time.Data.Add(-new TimeSpan(time.Durata ?? 0)),
                    End = time.Data,
                    Title = string.Format("{0} : {1} - {2}", time.Data.Add(-new TimeSpan(time.Durata ?? 0)).ToString("dd/MM/yyyy HH:mm:ss"), time.Data.ToString("dd/MM/yyyy HH:mm:ss"), new TimeSpan(time.Durata ?? 0).ToString(@"hh\:mm\:ss")),
                    Description = string.Format("{0} : {1} - {2}", time.Data.Add(-new TimeSpan(time.Durata ?? 0)).ToString("dd/MM/yyyy HH:mm:ss"), time.Data.ToString("dd/MM/yyyy HH:mm:ss"), new TimeSpan(time.Durata ?? 0).ToString(@"hh\:mm\:ss")),
                };

                Task.Times.Add(fix);
            }

            Task.UltimoTempoTipoID = times.OrderByDescending(o => o.Data).Select(s => s.TipoID).FirstOrDefault() ?? "0";

            if (Task.UltimoTempoTipoID == "5")
            {
                Task.ETerminato = true;
                Task.EffectiveDuration = new TimeSpan(0);
                Task.Start = DateTime.Now;
                Task.End = DateTime.Now.Add(new TimeSpan(Task.EffectiveDuration.Ticks));
            }

        }
        catch (Exception)
        {
        }
    }

    public long Save(string SocietaID, string OrdineID, decimal Quantita, string OrderUM, string? PadreID, string? PadreRevisioneID, tab_articolo_composizione Articolo, long Posizione)
    {
        using var connection = GetOpenConnection();



        try
        {
            var articoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();

            long? positionFather = (Posizione == 0) ? null : (long?)Posizione;

            foreach (var cmp in Articolo.Componenti ?? new ObservableCollection<tab_articolo_composizione>())
            {
                var stageID = (cmp.EReparto) ? cmp.RepartoID : null;
                var componentID = (cmp.EReparto) ? null : cmp.ComponenteArticoloID;
                var componentRevisionID = (cmp.EReparto) ? null : ((cmp.HaComposizione) ? cmp.ComponenteRevisioneID : null);
                var product = articoloRepo.GetSingle(SocietaID, PadreID ?? string.Empty);
                // check if halfmade recompute quantity
                decimal qty = 0;
                if ((cmp.ComponenteArticoloID != null && cmp.ComponenteRevisioneID != null) ||
                    (cmp.ComponenteArticoloID != null && cmp.ComponenteRevisioneID == null && cmp.ArticoloID == PadreID))
                {
                    qty = articoloRepo.ComputeRealQuantity(SocietaID, PadreID ?? string.Empty, OrderUM, product?.UnitaID ?? string.Empty, product?.QuantitaDefault, Quantita);
                }
                else
                {
                    qty = Quantita;
                }
                var result = connection.Execute(
               "INSERT INTO pro_ordine_composizione (SocietaID,OrdineID,ArticoloID,RevisioneID,ComposizioneID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID," +
               "ComposizioneIDPadre,RepartoID,ComponenteArticoloID,ComponenteRevisioneID,Posizione,Quantita,Tempo,RisorsaID,ESummary,EMilestone,DescrizioneMS,UID,Note,Piazzamento)" +
               " OUTPUT INSERTED.rv VALUES(@SocietaID,@OrdineID,@ArticoloID,@RevisioneID,@ComposizioneID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID," +
               "@ComposizioneIDPadre,@RepartoID,@ComponenteArticoloID, @ComponenteRevisioneID,@Posizione,@Quantita,@Tempo,@RisorsaID,@ESummary,@EMilestone,@DescrizioneMS,@UID, @Note,@Piazzamento)",
               new pro_ordine_composizione
               {
                   SocietaID = SocietaID,
                   OrdineID = OrdineID,
                   ArticoloID = PadreID ?? string.Empty,
                   RevisioneID = PadreRevisioneID ?? string.Empty,
                   ComposizioneID = ++Posizione,
                   ComposizioneIDPadre = positionFather,
                   RepartoID = stageID,
                   ComponenteRevisioneID = componentRevisionID,
                   ComponenteArticoloID = componentID,
                   Quantita = (cmp.Quantita ?? 1) * qty,
                   Tempo = (cmp.ETempoAlPezzo) ? (long?)(cmp.Tempo * qty) : cmp.Tempo,
                   Piazzamento = cmp.Piazzamento,
                   RisorsaID = cmp.RisorsaID,
                   ESummary = cmp.ESummary,
                   EMilestone = cmp.EMilestone,
                   Note = cmp.Note,
                   DescrizioneMS = (cmp.ESummary || cmp.EMilestone) ? cmp.Descrizione : null,
                   LogUpdated = DateTime.Now,
                   UID = Guid.NewGuid(),
               });

                long resultSave = Save(SocietaID, OrdineID, (cmp.Quantita ?? 1) * qty, OrderUM, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? PadreID : componentID, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? PadreRevisioneID : ((cmp.IsRoot) ? cmp.RevisioneID : cmp.ComponenteRevisioneID), cmp, (cmp.EReparto || cmp.EMilestone || cmp.ESummary) ? Posizione : 0);

                if ((cmp.EReparto || cmp.EMilestone || cmp.ESummary))
                    Posizione = resultSave;
            }

            return Posizione;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }


    }

    public bool UpdateNote(pro_ordine_composizione Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar("UPDATE pro_ordine_composizione SET Note = @Note OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND rv = @rv", Model);
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

    public bool UpdateHalfmade(string CompanyID, string OrdineID, string ArticoloID, string RevisioneID, decimal NewQuantity)
    {
        try
        {
            using var connection = GetOpenConnection();


            connection.Execute(@"UPDATE pro_ordine_composizione SET Quantita=@qta  
                                    WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComponenteArticoloID IS NULL",
                                new { SocietaID = CompanyID, OrdineID = OrdineID, ArticoloID = ArticoloID, RevisioneID = RevisioneID, qta = NewQuantity });
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Exists(string SocietaID, string OrdineID, string ArticoloID, long ComposizioneID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM pro_ordine_composizione WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND ComposizioneID = @ComposizioneID",
                new { SocietaID = SocietaID, OrdineID = OrdineID, ArticoloID = ArticoloID, ComposizioneID = ComposizioneID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    // rimuovere UniqueID dalle query perchè DA' ERRORE [github https://github.com/DapperLib/Dapper/issues/2024]!!!
    // aggiungere [] a UID
    public string INSERT_QUERY => "INSERT INTO pro_ordine_composizione (SocietaID,OrdineID,ArticoloID,RevisioneID,ComposizioneID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,ComponenteArticoloID,ComponenteRevisioneID,ComposizioneIDPadre,RepartoID,ComponenteID,Posizione,Quantita,Tempo,Inizio,Fine,RisorsaID,ESummary,EMilestone,DescrizioneMS,EFisso,TempoAlPezzo,Note,Piazzamento,[UID],QuantitaOriginale) OUTPUT INSERTED.rv VALUES(@SocietaID,@OrdineID,@ArticoloID,@RevisioneID,@ComposizioneID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@ComponenteArticoloID,@ComponenteRevisioneID,@ComposizioneIDPadre,@RepartoID,@ComponenteID,@Posizione,@Quantita,@Tempo,@Inizio,@Fine,@RisorsaID,@ESummary,@EMilestone,@DescrizioneMS,@EFisso,@TempoAlPezzo,@Note,@Piazzamento,@UID,@QuantitaOriginale)";
    public string UPDATE_QUERY => "UPDATE pro_ordine_composizione SET SocietaID = @SocietaID,OrdineID = @OrdineID,ArticoloID = @ArticoloID,RevisioneID = @RevisioneID,ComposizioneID = @ComposizioneID,LogAdded = @LogAdded,LogUpdated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,ComponenteArticoloID = @ComponenteArticoloID,ComponenteRevisioneID = @ComponenteRevisioneID,ComposizioneIDPadre = @ComposizioneIDPadre,RepartoID = @RepartoID,ComponenteID = @ComponenteID,Posizione = @Posizione,Quantita = @Quantita,Tempo = @Tempo,Inizio = @Inizio,Fine = @Fine,RisorsaID = @RisorsaID,ESummary = @ESummary,EMilestone = @EMilestone,DescrizioneMS = @DescrizioneMS,EFisso = @EFisso,TempoAlPezzo = @TempoAlPezzo,Note = @Note,Piazzamento = @Piazzamento,[UID] = @UID,QuantitaOriginale = @QuantitaOriginale OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine_composizione OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND OrdineID = @OrdineID AND ArticoloID = @ArticoloID AND RevisioneID = @RevisioneID AND ComposizioneID = @ComposizioneID AND rv = @rv";
    public bool Insert(pro_ordine_composizione Model)
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

    public bool Update(pro_ordine_composizione Model)
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

    public bool Delete(pro_ordine_composizione Model)
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

    public string? Validate(pro_ordine_composizione Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && IsInsert && !Exists(Model.SocietaID, Model.OrdineID, Model.ArticoloID, Model.ComposizioneID)) || !IsInsert)
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