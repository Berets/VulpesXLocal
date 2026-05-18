

using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Models.Models.SRM;

namespace VulpesX.DAL.SRM;

public interface Itab_articolo_costiRepository
{
    ObservableCollection<tab_articolo_costiListViewModel>? GetList(string CompanyID);

    ObservableCollection<tab_articolo_costi>? GetDetailsList(string CompanyID, string ProductID);

    tab_articolo_costi? Get(string cid, string product_id, int year, int month);

    tab_articolo_costi? GetLast(string cid, string product_id);

    tab_articolo_costi? GetLastUpTo(string cid, string product_id, DateTime LimitLast);

    decimal GetLastAverageCost(string cid, string product_id);

    decimal GetAverageCostSince(string cid, string product_id, DateTime LimitLastDate, DateTime LimitDate);

    bool Exists(string cid, string product_id, int year, int month);

    decimal ComputeBillLastPrice(string CompanyID, string ProductID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Add(tab_articolo_costi Model);

    bool Insert(tab_articolo_costi Model);

    bool Update(tab_articolo_costi Model);

    bool Delete(tab_articolo_costi Model);

    string? Validate(tab_articolo_costi Model, bool IsInsert);
    #endregion
}

public class tab_articolo_costiRepository : RepositoryBase, Itab_articolo_costiRepository
{
    public tab_articolo_costiRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_articolo_costiListViewModel>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo_costiListViewModel>(
                @"SELECT d.product_id AS ID, p.Descrizione AS Description, p.UnitaID AS UM, sum(d.total_load) AS GloballyLoad, sum(d.total_value) AS GloballyValue FROM tab_articolo_costi AS d
                        INNER JOIN tab_articolo AS p ON p.SocietaID = @cid AND p.ID = d.product_id
                        WHERE d.cid = @cid
                        GROUP BY d.product_id, p.Descrizione, p.UnitaID
                        ORDER BY d.product_id",
                new { cid = CompanyID });

            return new ObservableCollection<tab_articolo_costiListViewModel>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo_costi>? GetDetailsList(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_articolo_costi, tab_articolo, tab_articolo_costi>(
                @"SELECT c.*, p.SocietaID, p.ID, p.Descrizione, p.UnitaID FROM tab_articolo_costi AS c
                        INNER JOIN tab_articolo AS p ON p.SocietaID = c.cid AND p.ID = c.product_id
                        WHERE c.cid = @cid AND c.product_id = @pid
                        ORDER BY c.product_id ASC, c.year DESC, c.month DESC",
                (cos, prd) => { cos.Product = prd; return cos; },
                new { cid = CompanyID, pid = ProductID }, splitOn: "SocietaID");

            return new ObservableCollection<tab_articolo_costi>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_costi? Get(string cid, string product_id, int year, int month)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_articolo_costi>(
                "SELECT * FROM tab_articolo_costi WHERE cid = @cid AND product_id = @product_id AND year = @year AND month = @month",
                new { cid = cid, product_id = product_id, year = year, month = month })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_costi? GetLast(string cid, string product_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_articolo_costi>(
                @"SELECT TOP(1) * FROM tab_articolo_costi 
                        WHERE cid = @cid AND product_id = @product_id
                        ORDER BY year DESC, month DESC",
                new { cid = cid, product_id = product_id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_costi? GetLastUpTo(string cid, string product_id, DateTime LimitLast)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_articolo_costi>(
                @"SELECT TOP(1) * FROM tab_articolo_costi 
                        WHERE cid = @cid AND product_id = @product_id AND (year < @yea OR (year = @yea AND month <= @mon))
                        ORDER BY year DESC, month DESC",
                new { cid = cid, product_id = product_id, yea = LimitLast.Year, mon = LimitLast.Month })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public decimal GetLastAverageCost(string cid, string product_id)
    {
        try
        {
            using var connection = GetOpenConnection();

       
                var result = connection.Query<tab_articolo_costi>(
                    @"SELECT TOP(1) * FROM tab_articolo_costi 
                        WHERE cid = @cid AND product_id = @product_id
                        ORDER BY year DESC, month DESC",
                    new { cid = cid, product_id = product_id })
                    .FirstOrDefault();
                if (result == null)
                    return 0;
                else
                    return result.AverageCost;
            
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public decimal GetAverageCostSince(string cid, string product_id, DateTime LimitLastDate, DateTime LimitDate)
    {
        try
        {
            using var connection = GetOpenConnection();

       
                var result = connection.Query<tab_articolo_costiListViewModel>(
                    @"SELECT d.product_id AS ID, p.Descrizione AS Description, p.UnitaID AS UM, sum(d.total_load) AS GloballyLoad, sum(d.total_value) AS GloballyValue FROM tab_articolo_costi AS d
                        INNER JOIN tab_articolo AS p ON p.SocietaID = @cid AND p.ID = d.product_id
                        WHERE d.cid = @cid AND d.product_id = @pid AND (year > @yea OR (year = @yea AND month >= @mon)) AND (year < @yeal OR (year = @yeal AND month <= @monl))
                        GROUP BY d.product_id, p.Descrizione, p.UnitaID",
                    new { cid = cid, pid = product_id, yea = LimitDate.Year, mon = LimitDate.Month, yeal = LimitLastDate.Year, monl = LimitLastDate.Month }).FirstOrDefault();
                if (result == null)
                    return 0;
                else
                    return result.GloballyAverageCost ?? 0;
            
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public bool Exists(string cid, string product_id, int year, int month)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo_costi WHERE cid = @cid AND product_id = @product_id AND year = @year AND month = @month",
                new { cid = cid, product_id = product_id, year = year, month = month }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public decimal ComputeBillLastPrice(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var tab_articolo_composizioneRepository = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>();

            decimal result = 0;

            // get last updated revision
            var lastUpdatedRevisionID = tab_articolo_composizioneRepository.GetLastUpdatedRevisionID(CompanyID, ProductID);
            // get product detailed composition
            var list = tab_articolo_composizioneRepository.Get(CompanyID, ProductID, lastUpdatedRevisionID ?? string.Empty, null);

            return result;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO tab_articolo_costi (cid,product_id,year,month,total_load,total_value,last_cost,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@cid,@product_id,@year,@month,@total_load,@total_value,@last_cost,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE tab_articolo_costi SET cid = @cid,product_id = @product_id,year = @year,month = @month,total_load = @total_load,total_value = @total_value,last_cost = @last_cost,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE cid = @cid AND product_id = @product_id AND year = @year AND month = @month AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM tab_articolo_costi OUTPUT DELETED.rv WHERE cid = @cid AND product_id = @product_id AND year = @year AND month = @month AND rv = @rv";

    public bool Add(tab_articolo_costi Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var existing = Get(Model.cid, Model.product_id, Model.year, Model.month);
            int result = 0;
            if (existing != null)
            {
                existing.total_load += Model.total_load;
                existing.last_cost = Model.last_cost;
                existing.total_value += Model.total_value;
                existing.updated = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                existing.updatedUserID = Model.addedUserID;
                result = connection.Execute(UPDATE_QUERY, existing);
            }
            else
            {
                result = connection.Execute(INSERT_QUERY, Model);
            }

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

    public bool Insert(tab_articolo_costi Model)
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

    public bool Update(tab_articolo_costi Model)
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

    public bool Delete(tab_articolo_costi Model)
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

    public string? Validate(tab_articolo_costi Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.cid))
            {
                if (!string.IsNullOrWhiteSpace(Model.product_id))
                {
                    if (Model.year > 0 && Model.month > 0)
                    {
                        if (Model.total_load > 0)
                        {
                            if (Model.last_cost > 0)
                            {
                                return null;
                            }
                            else
                            { return "Il prezzo unitario è obbligatorio"; }
                        }
                        else
                        { return "La quantità caricata è obbligatoria"; }
                    }
                    else
                    { return "La data è obbligatoria"; }
                }
                else
                { return "L'articolo è obbligatorio"; }
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