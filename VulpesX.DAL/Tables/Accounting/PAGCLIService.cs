using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting;

public interface IPAGCLIRepository
{
    ObservableCollection<PAGCLI>? GetList();

    ObservableCollection<GenericIDDescription>? GetGenericList();

    PAGCLI? Get(string ID);

    PAGCLI? GetFull(string ID);

    bool Exists(string ID);

    #region Functions

    List<DateTime>? ComputeExpires(string CompanyID, string PaymentID, DateTime StartDate, int CustomerID);

    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(PAGCLI Model);

    bool Update(PAGCLI Model);

    string? CanDelete(PAGCLI Model);

    bool Delete(PAGCLI Model);

    string? Validate(PAGCLI Model, bool IsInsert);
    #endregion
}

public class PAGCLIRepository : RepositoryBase, IPAGCLIRepository
{
    public PAGCLIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PAGCLI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PAGCLI, TAB_ACC_TIPINC, PAGCLI>(
                @"SELECT p.*, i.* FROM PAGCLI AS p
                        INNER JOIN TAB_ACC_TIPINC AS i ON i.icscod = p.pcltip
                        WHERE p.canceled IS NULL",
                (pag, inc) => { pag.Incasso = inc; return pag; }, splitOn: "icscod");

            return new ObservableCollection<PAGCLI>(list);

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
                "SELECT pclcod AS ID, TRIM(pcldes) AS Description FROM PAGCLI");

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PAGCLI? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PAGCLI>(
                @"SELECT * FROM PAGCLI WHERE pclcod = @pclcod",
                new { pclcod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PAGCLI? GetFull(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PAGCLI, TAB_ACC_TIPINC, PAGCLI>(
                @"SELECT p.*, i.* FROM PAGCLI AS p
                        INNER JOIN TAB_ACC_TIPINC AS i ON p.pcltip = i.icscod
                        WHERE p.pclcod = @pclcod",
                (pag, inc) => { pag.Incasso = inc; return pag; },
                new { pclcod = ID }, splitOn: "icscod")
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
                "SELECT COUNT(*) FROM PAGCLI WHERE pclcod = @pclcod",
                new { pclcod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Functions

    public List<DateTime>? ComputeExpires(string CompanyID, string PaymentID, DateTime StartDate, int CustomerID)
    {
        try
        {
            List<DateTime> expires = new List<DateTime>();
            var payment = Get(PaymentID);
            if (payment != null)
            {
                if (payment.pclgs1.HasValue && payment.pclgs1.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs1.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs1.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }
                else
                {
                    expires.Add(StartDate);
                }

                if (payment.pclgs2.HasValue && payment.pclgs2.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs2.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs2.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs3.HasValue && payment.pclgs3.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs3.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs3.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs4.HasValue && payment.pclgs4.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs4.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs4.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs5.HasValue && payment.pclgs5.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs5.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs5.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs6.HasValue && payment.pclgs6.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs6.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs6.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs7.HasValue && payment.pclgs7.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs7.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs7.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs8.HasValue && payment.pclgs8.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs8.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs8.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs9.HasValue && payment.pclgs9.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs9.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs9.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                // check expires moves
                var customer = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, CustomerID);

                if (customer != null)
                {
                    List<DateTime> results = new List<DateTime>();
                    if (!string.IsNullOrWhiteSpace(customer.scacod))
                    {
                        var scadenzeRepository = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>();

                        var mover = scadenzeRepository.Get(customer.scacod);
                        foreach (var exp in expires)
                        {
                            results.Add(scadenzeRepository.ComputeExpireMove(mover, exp));
                        }
                        return results;
                    }
                    else
                    {
                        return expires;
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
    public string INSERT_QUERY => "INSERT INTO PAGCLI (pclcod,pcldes,pclgs1,pclgs2,pclgs3,pclgs4,pclgs5,pclgs6,pclgs7,pclgs8,pclgs9,pclmed,pclppa,pclsco,pclmag,pcltip,pclgio,pclold,pcivacas,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@pclcod,@pcldes,@pclgs1,@pclgs2,@pclgs3,@pclgs4,@pclgs5,@pclgs6,@pclgs7,@pclgs8,@pclgs9,@pclmed,@pclppa,@pclsco,@pclmag,@pcltip,@pclgio,@pclold,@pcivacas,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE PAGCLI SET pclcod = @pclcod,pcldes = @pcldes,pclgs1 = @pclgs1,pclgs2 = @pclgs2,pclgs3 = @pclgs3,pclgs4 = @pclgs4,pclgs5 = @pclgs5,pclgs6 = @pclgs6,pclgs7 = @pclgs7,pclgs8 = @pclgs8,pclgs9 = @pclgs9,pclmed = @pclmed,pclppa = @pclppa,pclsco = @pclsco,pclmag = @pclmag,pcltip = @pcltip,pclgio = @pclgio,pclold = @pclold,pcivacas = @pcivacas,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE pclcod = @pclcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PAGCLI OUTPUT DELETED.rv WHERE pclcod = @pclcod AND rv = @rv";
    public bool Insert(PAGCLI Model)
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

    public bool Update(PAGCLI Model)
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

    public string? CanDelete(PAGCLI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            // PNRIGHE
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM PNRIGHE AS r
                                                    INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                                                    WHERE r.N1PAGA=@id AND s.P3CLFO='C'",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle righe di prima nota contabile che utilizzano questo pagamento";
            }
            // OFFET00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM OFFET00F WHERE OFTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle offerte che utilizzano questo pagamento";
            }
            // ORDIT00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM ORDIT00F WHERE OTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono degli ordini cliente che utilizzano questo pagamento";
            }
            // BOLLT00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM BOLLT00F WHERE BTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono dei DDT che utilizzano questo pagamento";
            }
            // FATTT00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM FATTT00F WHERE FTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle fatture che utilizzano questo pagamento";
            }
            // CLIAMMI
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM CLIAMMI WHERE pclcod = @id ",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle anagrafiche cliente che utilizzano questo pagamento";
            }

            return null;

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(PAGCLI Model)
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

    public string? Validate(PAGCLI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.pclcod) && IsInsert && !Exists(Model.pclcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.pcldes))
                {
                    if (!string.IsNullOrWhiteSpace(Model.pcltip))
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

public class PAGCLIUfpRepository : RepositoryBase, IPAGCLIRepository
{
    public PAGCLIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PAGCLI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PAGCLI, TAB_ACC_TIPINC, PAGCLI>(
                @"SELECT p.*, i.* FROM PAGCLI AS p
                        INNER JOIN PAGAMENTI AS i ON i.inccod = p.pcltip",
                (pag, inc) => { pag.Incasso = inc; return pag; }, splitOn: "inccod");

            return new ObservableCollection<PAGCLI>(list);

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
                "SELECT pclcod AS ID, TRIM(pcldes) AS Description FROM PAGCLI");

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PAGCLI? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PAGCLI>(
                @"SELECT * FROM PAGCLI WHERE pclcod = @pclcod",
                new { pclcod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PAGCLI? GetFull(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PAGCLI, TAB_ACC_TIPINC, PAGCLI>(
                @"SELECT p.*, i.* FROM PAGCLI AS p
                        INNER JOIN INCASSI1 AS i ON p.pcltip = i.icscod
                        WHERE p.pclcod = @pclcod",
                (pag, inc) => { pag.Incasso = inc; return pag; },
                new { pclcod = ID }, splitOn: "icscod")
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
                "SELECT COUNT(*) FROM PAGCLI WHERE pclcod = @pclcod",
                new { pclcod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Functions

    public List<DateTime>? ComputeExpires(string CompanyID, string PaymentID, DateTime StartDate, int CustomerID)
    {
        try
        {
            List<DateTime> expires = new List<DateTime>();
            var payment = Get(PaymentID);

            if (payment != null)
            {
                if (payment.pclgs1.HasValue && payment.pclgs1.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs1.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs1.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }
                else
                {
                    expires.Add(StartDate);
                }

                if (payment.pclgs2.HasValue && payment.pclgs2.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs2.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs2.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs3.HasValue && payment.pclgs3.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs3.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs3.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs4.HasValue && payment.pclgs4.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs4.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs4.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs5.HasValue && payment.pclgs5.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs5.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs5.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs6.HasValue && payment.pclgs6.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs6.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs6.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs7.HasValue && payment.pclgs7.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs7.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs7.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs8.HasValue && payment.pclgs8.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs8.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs8.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                if (payment.pclgs9.HasValue && payment.pclgs9.Value > 0)
                {
                    DateTime tmp = StartDate.AddMonths(payment.pclgs9.Value / 30);
                    if (payment.pclppa == "1")
                    {
                        tmp = new DateTime(tmp.Year, tmp.Month, DateTime.DaysInMonth(tmp.Year, tmp.Month));
                    }
                    else
                    {
                        var diff = payment.pclgs9.Value - (tmp - StartDate).Days;
                        tmp = tmp.AddDays(diff);
                    }
                    expires.Add(tmp);
                }

                // check expires moves
                var customer = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, CustomerID);

                if (customer != null)
                {
                    List<DateTime> results = new List<DateTime>();
                    if (!string.IsNullOrWhiteSpace(customer.scacod))
                    {
                        var scadenzeRepository = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>();

                        var mover = scadenzeRepository.Get(customer.scacod);
                        foreach (var exp in expires)
                        {
                            results.Add(scadenzeRepository.ComputeExpireMove(mover, exp));
                        }
                        return results;
                    }
                    else
                    { return expires; }
                }
            }

            return expires;
        }
        catch (Exception)
        { return null; }
    }

    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PAGCLI (pclcod,pcldes,pclgs1,pclgs2,pclgs3,pclgs4,pclgs5,pclgs6,pclgs7,pclgs8,pclgs9,pclmed,pclppa,pclsco,pclmag,pcltip,pclold) OUTPUT INSERTED.rv VALUES(@pclcod,@pcldes,@pclgs1,@pclgs2,@pclgs3,@pclgs4,@pclgs5,@pclgs6,@pclgs7,@pclgs8,@pclgs9,@pclmed,@pclppa,@pclsco,@pclmag,@pcltip,@pclold)";
    public string UPDATE_QUERY => "UPDATE PAGCLI SET pclcod = @pclcod,pcldes = @pcldes,pclgs1 = @pclgs1,pclgs2 = @pclgs2,pclgs3 = @pclgs3,pclgs4 = @pclgs4,pclgs5 = @pclgs5,pclgs6 = @pclgs6,pclgs7 = @pclgs7,pclgs8 = @pclgs8,pclgs9 = @pclgs9,pclmed = @pclmed,pclppa = @pclppa,pclsco = @pclsco,pclmag = @pclmag,pcltip = @pcltip,pclold = @pclold OUTPUT INSERTED.rv WHERE pclcod = @pclcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PAGCLI OUTPUT DELETED.rv WHERE pclcod = @pclcod AND rv = @rv";
    public bool Insert(PAGCLI Model)
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

    public bool Update(PAGCLI Model)
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

    public string? CanDelete(PAGCLI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            // PNRIGHE
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM PNRIGHE AS r
                                                    INNER JOIN PDCSOTTO AS s ON s.P1GRUP=r.pngrup AND s.P2CONT=r.pncont AND s.P3SOTC=r.pnsott
                                                    WHERE r.N1PAGA=@id AND s.P3CLFO='C'",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle righe di prima nota contabile che utilizzano questo pagamento";
            }
            // OFFET00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM OFFET00F WHERE OFTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle offerte che utilizzano questo pagamento";
            }
            // ORDIT00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM ORDIT00F WHERE OTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono degli ordini cliente che utilizzano questo pagamento";
            }
            // BOLLT00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM BOLLT00F WHERE BTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono dei DDT che utilizzano questo pagamento";
            }
            // FATTT00F
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM FATTT00F WHERE FTPAGA = @id AND canceled IS NULL",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle fatture che utilizzano questo pagamento";
            }
            // CLIAMMI
            if ((int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM CLIAMMI WHERE pclcod = @id ",
                new { id = Model.pclcod }) > 0)
            {
                return "Impossibile eliminare, ci sono delle anagrafiche cliente che utilizzano questo pagamento";
            }

            return null;

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(PAGCLI Model)
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

    public string? Validate(PAGCLI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.pclcod) && IsInsert && !Exists(Model.pclcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.pcldes))
                {
                    if (!string.IsNullOrWhiteSpace(Model.pcltip))
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