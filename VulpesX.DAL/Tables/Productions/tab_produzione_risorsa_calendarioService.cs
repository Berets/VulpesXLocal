namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_risorsa_calendarioRepository
{
    ObservableCollection<tab_produzione_risorsa_calendario>? GetPeriod(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al);

    ObservableCollection<tab_produzione_risorsa_calendario>? GetDay(string SocietaID, string RisorsaID, DateTime Giorno);

    #region CRUD
    bool Generate(ObservableCollection<tab_produzione_risorsa_calendario> Calendar, ObservableCollection<tab_produzione_calendario_chiusura> Stops);
    #endregion
}

public class tab_produzione_risorsa_calendarioRepository : RepositoryBase, Itab_produzione_risorsa_calendarioRepository
{
    public tab_produzione_risorsa_calendarioRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<tab_produzione_risorsa_calendario>? GetPeriod(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_risorsa_calendario>(
                @"SELECT * FROM tab_produzione_risorsa_calendario
                        WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND Giorno >= @Dal AND Giorno <= @Al
                        ORDER BY Dalle",
                new { SocietaID = SocietaID, RisorsaID = RisorsaID, Dal = Dal, Al = Al });

            return new ObservableCollection<tab_produzione_risorsa_calendario>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_risorsa_calendario>? GetDay(string SocietaID, string RisorsaID, DateTime Giorno)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_produzione_risorsa_calendario>(
                @"SELECT * FROM tab_produzione_risorsa_calendario
                        WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND Giorno = @Giorno
                        ORDER BY Dalle",
                new { SocietaID = SocietaID, RisorsaID = RisorsaID, Giorno = Giorno });

            return new ObservableCollection<tab_produzione_risorsa_calendario>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public bool Generate(ObservableCollection<tab_produzione_risorsa_calendario> Calendar, ObservableCollection<tab_produzione_calendario_chiusura> Stops)
    {
        try
        {
            using var connection = GetOpenConnection();



            var risorse = Calendar.GroupBy(g => g.RisorsaID).ToList();
            var dal = Calendar.Min(g => g.Giorno);
            var al = Calendar.Max(g => g.Giorno);

            using (var transaction = connection.BeginTransaction())
            {
                foreach (var ris in risorse)
                {
                    // PULISCE
                    var cleanCalendar = connection.Execute(@"DELETE tab_produzione_risorsa_calendario WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND Giorno >= @Dal AND Giorno <= @Al", new
                    {
                        SocietaID = Calendar.First().SocietaID,
                        RisorsaID = ris.Key,
                        Dal = dal,
                        Al = al
                    }, transaction);
                    var cleanStops = connection.Execute(@"DELETE tab_produzione_calendario_chiusura WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND Dalle >= @Dal AND Dalle <= @Al", new
                    {
                        SocietaID = Calendar.First().SocietaID,
                        RisorsaID = ris.Key,
                        Dal = dal.Date,
                        Al = al.Date.AddDays(1)
                    }, transaction);

                    // GENERA
                    foreach (var cal in ris)
                    {
                        var result = connection.Execute("INSERT INTO tab_produzione_risorsa_calendario (SocietaID,RisorsaID,Giorno,Dalle,Alle,Tipo,LogAdded,LogAddedUserID) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@Giorno,@Dalle,@Alle,@Tipo,@LogAdded,@LogAddedUserID)", cal, transaction);
                    }
                }
                foreach (var stp in Stops)
                {
                    var result = connection.Execute("INSERT INTO tab_produzione_calendario_chiusura (SocietaID,RisorsaID,Dalle,Alle,LogAdded,LogAddedUserID) OUTPUT INSERTED.rv VALUES(@SocietaID,@RisorsaID,@Dalle,@Alle,@LogAdded,@LogAddedUserID)", stp, transaction);
                }

                transaction.Commit();
                return true;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }
    #endregion
}