namespace VulpesX.DAL.Tables.Productions;

public interface Itab_produzione_calendario_chiusuraRepository
{
    ObservableCollection<tab_produzione_calendario_chiusura>? GetPeriodo(string SocietaID, DateTime Dal, DateTime Al);

    ObservableCollection<tab_produzione_calendario_chiusura>? GetRisorsaPeriodo(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al);

}

public class tab_produzione_calendario_chiusuraRepository : RepositoryBase, Itab_produzione_calendario_chiusuraRepository
{
    public tab_produzione_calendario_chiusuraRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_produzione_calendario_chiusura>? GetPeriodo(string SocietaID, DateTime Dal, DateTime Al)
    {
        try
        {
            using var connection = GetOpenConnection();

       
                var list = connection.Query<tab_produzione_calendario_chiusura>(
                    @"SELECT * FROM tab_produzione_calendario_chiusura
                        WHERE SocietaID = @SocietaID AND Dalle >= @Dal AND Alle <= @Al
                        ORDER BY Dalle",
                    new { SocietaID = SocietaID, Dal = Dal, Al = Al });

                return new ObservableCollection<tab_produzione_calendario_chiusura>(list);
            
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_produzione_calendario_chiusura>? GetRisorsaPeriodo(string SocietaID, string RisorsaID, DateTime Dal, DateTime Al)
    {
        try
        {
            using var connection = GetOpenConnection();

                var list = connection.Query<tab_produzione_calendario_chiusura>(
                    @"SELECT * FROM tab_produzione_calendario_chiusura
                        WHERE SocietaID = @SocietaID AND RisorsaID = @RisorsaID AND Alle >= @Dal AND Alle <= @Al
                        ORDER BY Dalle",
                    new { SocietaID = SocietaID, RisorsaID = RisorsaID, Dal = Dal, Al = Al });

                return new ObservableCollection<tab_produzione_calendario_chiusura>(list);
            
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

}