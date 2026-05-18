using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting;

public interface IPAGFORRepository
{
    ObservableCollection<PAGFOR>? GetList();

    ObservableCollection<GenericIDDescription>? GetGenericList();

    PAGFOR? Get(string ID);

    bool Exists(string ID);

    #region Functions

    List<DateTime>? ComputeExpires(string CompanyID, string PaymentID, DateTime StartDate, int SupplierID);

    #endregion

    #region CRUD
    public string INSERT_QUERY { get; }
    public string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(PAGFOR Model);

    bool Update(PAGFOR Model);

    string? CanDelete(PAGFOR Model);

    bool Delete(PAGFOR Model);

    string? Validate(PAGFOR Model, bool IsInsert);
    #endregion
}

public class PAGFORRepository : RepositoryBase, IPAGFORRepository
{
    public PAGFORRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PAGFOR>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PAGFOR, TAB_ACC_TIPPAG, PAGFOR>(
                @"SELECT p.*, i.* FROM PAGFOR AS p
                        INNER JOIN TAB_ACC_TIPPAG AS i ON i.inccod = p.pfotip
                        WHERE p.canceled IS NULL",
                (pag, tip) => { pag.PaymentType = tip; return pag; }, splitOn: "inccod");

            return new ObservableCollection<PAGFOR>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetGenericList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIDDescription>(
                "SELECT pfocod AS ID, TRIM(pfodes) AS Description FROM PAGFOR");

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PAGFOR? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PAGFOR>(
                "SELECT * FROM PAGFOR WHERE pfocod = @pfocod",
                new { pfocod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PAGFOR WHERE pfocod = @pfocod",
                new { pfocod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Functions

    public List<DateTime>? ComputeExpires(string CompanyID, string PaymentID, DateTime StartDate, int SupplierID)
    {
        try
        {
            List<DateTime> expires = new List<DateTime>();
            var payment = Get(PaymentID);

            if (payment != null)
            {
                if (payment.pfogs1.HasValue && payment.pfogs1.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs1.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs1.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }
                else
                {
                    expires.Add(StartDate);
                }

                if (payment.pfogs2.HasValue && payment.pfogs2.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs2.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs2.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs3.HasValue && payment.pfogs3.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs3.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs3.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs4.HasValue && payment.pfogs4.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs4.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs4.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs5.HasValue && payment.pfogs5.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs5.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs5.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs6.HasValue && payment.pfogs6.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs6.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs6.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs7.HasValue && payment.pfogs7.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs7.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs7.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs8.HasValue && payment.pfogs8.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs8.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs8.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs9.HasValue && payment.pfogs9.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs9.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs9.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                // check expires moves
                var supplier = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, SupplierID);

                if (supplier != null)
                {
                    if (!string.IsNullOrWhiteSpace(supplier.scacod))
                    {
                        var scadenzeRepository = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>();
                        var mover = scadenzeRepository.Get(supplier.scacod);
                        foreach (var exp in expires)
                        {
                            expires.Remove(exp);
                            expires.Add(scadenzeRepository.ComputeExpireMove(mover, exp));
                        }
                    }
                }
            }

            return expires;
        }
        catch (Exception)
        {
            return null;
        }
    }

    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PAGFOR (pfocod,pfodes,pfogs1,pfogs2,pfogs3,pfogs4,pfogs5,pfogs6,pfogs7,pfogs8,pfogs9,pfomed,pfoppa,pfosco,pfomag,pfotip,pfivacas,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@pfocod,@pfodes,@pfogs1,@pfogs2,@pfogs3,@pfogs4,@pfogs5,@pfogs6,@pfogs7,@pfogs8,@pfogs9,@pfomed,@pfoppa,@pfosco,@pfomag,@pfotip,@pfivacas,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE PAGFOR SET pfocod = @pfocod,pfodes = @pfodes,pfogs1 = @pfogs1,pfogs2 = @pfogs2,pfogs3 = @pfogs3,pfogs4 = @pfogs4,pfogs5 = @pfogs5,pfogs6 = @pfogs6,pfogs7 = @pfogs7,pfogs8 = @pfogs8,pfogs9 = @pfogs9,pfomed = @pfomed,pfoppa = @pfoppa,pfosco = @pfosco,pfomag = @pfomag,pfotip = @pfotip,pfivacas = @pfivacas,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE pfocod = @pfocod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PAGFOR OUTPUT DELETED.rv WHERE pfocod = @pfocod AND rv = @rv";
    public bool Insert(PAGFOR Model)
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

    public bool Update(PAGFOR Model)
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

    public string? CanDelete(PAGFOR Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            // acq_orders_heads
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM acq_orders_heads WHERE payment_id = @id AND canceled IS NULL",
                new { id = Model.pfocod }) > 0)
            {
                return "Impossibile eliminare, ci sono degli ordini di acquisto che utilizzano questo pagamento";
            }
            // FORNAMMI
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM FORNAMMI WHERE pfocod = @id ",
                new { id = Model.pfocod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle anagrafiche fornitore che utilizzano questo pagamento";
            }
            // PNRIGHE
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM PNRIGHE AS r
                                                    INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                                                    WHERE r.N1PAGA=@id AND s.P3CLFO='F'",
                new { id = Model.pfocod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle righe di prima nota contabile che utilizzano questo pagamento";
            }

            return null;

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(PAGFOR Model)
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

    public string? Validate(PAGFOR Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.pfocod) && IsInsert && !Exists(Model.pfocod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.pfodes))
                {
                    if (!string.IsNullOrWhiteSpace(Model.pfotip))
                    {
                        return null;
                    }
                    else
                    { return "Il tipo è obbligatorio"; }
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

public class PAGFORUfpRepository : RepositoryBase, IPAGFORRepository
{
    public PAGFORUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PAGFOR>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PAGFOR, TAB_ACC_TIPPAG, PAGFOR>(
                @"SELECT p.*, i.* FROM PAGFOR AS p
                        INNER JOIN PAGAMENTI AS i ON i.inccod = p.pfotip",
                (pag, tip) => { pag.PaymentType = tip; return pag; }, splitOn: "inccod");

            return new ObservableCollection<PAGFOR>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetGenericList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIDDescription>(
                "SELECT pfocod AS ID, TRIM(pfodes) AS Description FROM PAGFOR");

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PAGFOR? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PAGFOR>(
                "SELECT * FROM PAGFOR WHERE pfocod = @pfocod",
                new { pfocod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PAGFOR WHERE pfocod = @pfocod",
                new { pfocod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Functions

    public List<DateTime>? ComputeExpires(string CompanyID, string PaymentID, DateTime StartDate, int SupplierID)
    {
        try
        {
            List<DateTime> expires = new List<DateTime>();
            var payment = Get(PaymentID);

            if (payment != null)
            {
                if (payment.pfogs1.HasValue && payment.pfogs1.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs1.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs1.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }
                else
                {
                    expires.Add(StartDate);
                }

                if (payment.pfogs2.HasValue && payment.pfogs2.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs2.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs2.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs3.HasValue && payment.pfogs3.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs3.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs3.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs4.HasValue && payment.pfogs4.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs4.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs4.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs5.HasValue && payment.pfogs5.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs5.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs5.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs6.HasValue && payment.pfogs6.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs6.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs6.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs7.HasValue && payment.pfogs7.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs7.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs7.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs8.HasValue && payment.pfogs8.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs8.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs8.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pfogs9.HasValue && payment.pfogs9.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pfogs9.Value / 30);
                    if (payment.pfoppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pfogs9.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                // check expires moves
                var supplier = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, SupplierID);

                if (supplier != null)
                {
                    if (!string.IsNullOrWhiteSpace(supplier.scacod))
                    {
                        var scadenzeRepository = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>();
                        var mover = scadenzeRepository.Get(supplier.scacod);
                        foreach (var exp in expires)
                        {
                            expires.Remove(exp);
                            expires.Add(scadenzeRepository.ComputeExpireMove(mover, exp));
                        }
                    }
                }
            }
            return expires;
        }
        catch (Exception)
        {
            return null;
        }
    }

    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PAGFOR (pfocod,pfodes,pfogs1,pfogs2,pfogs3,pfogs4,pfogs5,pfogs6,pfogs7,pfogs8,pfogs9,pfomed,pfoppa,pfosco,pfomag,pfotip) OUTPUT INSERTED.rv VALUES(@pfocod,@pfodes,@pfogs1,@pfogs2,@pfogs3,@pfogs4,@pfogs5,@pfogs6,@pfogs7,@pfogs8,@pfogs9,@pfomed,@pfoppa,@pfosco,@pfomag,@pfotip)";
    public string UPDATE_QUERY => "UPDATE PAGFOR SET pfocod = @pfocod,pfodes = @pfodes,pfogs1 = @pfogs1,pfogs2 = @pfogs2,pfogs3 = @pfogs3,pfogs4 = @pfogs4,pfogs5 = @pfogs5,pfogs6 = @pfogs6,pfogs7 = @pfogs7,pfogs8 = @pfogs8,pfogs9 = @pfogs9,pfomed = @pfomed,pfoppa = @pfoppa,pfosco = @pfosco,pfomag = @pfomag,pfotip = @pfotip OUTPUT INSERTED.rv WHERE pfocod = @pfocod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PAGFOR OUTPUT DELETED.rv WHERE pfocod = @pfocod AND rv = @rv";
    public bool Insert(PAGFOR Model)
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

    public bool Update(PAGFOR Model)
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

    public string? CanDelete(PAGFOR Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            // acq_orders_heads
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM acq_orders_heads WHERE payment_id = @id AND canceled IS NULL",
                new { id = Model.pfocod }) > 0)
            {
                return "Impossibile eliminare, ci sono degli ordini di acquisto che utilizzano questo pagamento";
            }
            // FORNAMMI
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM FORNAMMI WHERE pfocod = @id ",
                new { id = Model.pfocod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle anagrafiche fornitore che utilizzano questo pagamento";
            }
            // PNRIGHE
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM PNRIGHE AS r
                                                    INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                                                    WHERE r.N1PAGA=@id AND s.P3CLFO='F'",
                new { id = Model.pfocod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle righe di prima nota contabile che utilizzano questo pagamento";
            }

            return null;

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(PAGFOR Model)
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

    public string? Validate(PAGFOR Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.pfocod) && IsInsert && !Exists(Model.pfocod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.pfodes))
                {
                    if (!string.IsNullOrWhiteSpace(Model.pfotip))
                    {
                        return null;
                    }
                    else
                    { return "Il tipo è obbligatorio"; }
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