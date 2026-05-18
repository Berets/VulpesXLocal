using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL;

namespace VulpesX.DAL.Accounting;

public interface IPDCGRUPPIRepository
{
    ObservableCollection<PDCGRUPPI>? GetList();

    ObservableCollection<PDCGRUPPI>? GetTreeList(string CompanyID);

    PDCGRUPPI? Get(string ID);

    bool Exists(string ID);

    #region CRUD
    bool Insert(PDCGRUPPI Model);

    bool Update(PDCGRUPPI Model);

    bool CanDelete(PDCGRUPPI Model);

    bool Delete(PDCGRUPPI Model);

    string? Validate(PDCGRUPPI Model, bool IsInsert);
    #endregion
}

public class PDCGRUPPIRepository : RepositoryBase, IPDCGRUPPIRepository
{
    public PDCGRUPPIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PDCGRUPPI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCGRUPPI>(
                @"SELECT P1GRUP,P1DES1,P1TICO,P1chco FROM PDCGRUPPI");

            return new ObservableCollection<PDCGRUPPI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCGRUPPI>? GetTreeList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var groups = connection.Query<PDCGRUPPI>(
                "SELECT * FROM PDCGRUPPI");
            var accounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList() ?? new ObservableCollection<PDCCONTI>();
            var subaccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID) ?? new ObservableCollection<PDCSOTTO>();
            var years = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().GetList(CompanyID) ?? new ObservableCollection<PDCANNI>();
            ObservableCollection<PDCGRUPPI> list = new ObservableCollection<PDCGRUPPI>();
            foreach (var grp in groups)
            {
                grp.Accounts = new ObservableCollection<PDCCONTI>();
                foreach (var acc in accounts.Where(w => w.P1GRUP == grp.P1GRUP))
                {
                    acc.Subaccounts = new ObservableCollection<PDCSOTTO>();
                    foreach (var sub in subaccounts.Where(w => w.P1GRUP == grp.P1GRUP && w.P2CONT == acc.P2CONT))
                    {
                        sub.Years = new ObservableCollection<PDCANNI>();
                        foreach (var yea in years.Where(w => w.P1GRUP == grp.P1GRUP && w.P2CONT == acc.P2CONT && w.P3SOTC == sub.P3SOTC))
                        {
                            sub.Years.Add(yea);
                        }
                        acc.Subaccounts.Add(sub);
                    }
                    grp.Accounts.Add(acc);
                }
                list.Add(grp);
            }

            return list;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCGRUPPI? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCGRUPPI>(
                "SELECT * FROM PDCGRUPPI WHERE P1GRUP = @p1grup",
                new { p1grup = ID })
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
                "SELECT COUNT(*) FROM PDCGRUPPI WHERE P1GRUP = @p1grup",
                new { p1grup = ID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public bool Insert(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO PDCGRUPPI (P1GRUP,p1chco,P1TICO,P1DES1,P1DES2,P1OBCP) OUTPUT INSERTED.rv VALUES(@P1GRUP,@p1chco,@P1TICO,@P1DES1,@P1DES2,@P1OBCP)",
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

    public bool Update(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE PDCGRUPPI SET P1GRUP = @P1GRUP,p1chco = @p1chco,P1TICO = @P1TICO,P1DES1 = @P1DES1,P1DES2 = @P1DES2,P1OBCP = @P1OBCP OUTPUT INSERTED.rv WHERE P1GRUP = @p1grup AND rv = @rv",
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

    public bool CanDelete(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP",
                Model);
            if (result == 0)
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

    public bool Delete(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();
            var result = connection.Execute(
                "DELETE FROM PDCGRUPPI OUTPUT DELETED.rv WHERE P1GRUP = @p1grup AND rv = @rv",
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

    public string? Validate(PDCGRUPPI Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.P1GRUP))
            {
                if ((!Exists(Model.P1GRUP) && IsInsert) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.P1DES1))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.P1TICO))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.p1chco))
                            {
                                return null;
                            }
                            else
                            { return "Il codice chiusura è obbligatorio"; }
                        }
                        else
                        { return "Il tipo è obbligatorio"; }
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice inserito è già in uso"; }
            }
            else
            { return "Il codice del gruppo è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PDCGRUPPIUfpRepository : RepositoryBase, IPDCGRUPPIRepository
{
    public PDCGRUPPIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PDCGRUPPI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCGRUPPI>(
                @"SELECT P1GRUP,P1DES1,P1TICO,P1chco FROM PDC_GRUPPI");

            return new ObservableCollection<PDCGRUPPI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCGRUPPI>? GetTreeList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var groups = connection.Query<PDCGRUPPI>(
                "SELECT * FROM PDC_GRUPPI");
            var accounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList() ?? new ObservableCollection<PDCCONTI>();
            var subaccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID) ?? new ObservableCollection<PDCSOTTO>();
            var years = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().GetList(CompanyID) ?? new ObservableCollection<PDCANNI>();
            ObservableCollection<PDCGRUPPI> list = new ObservableCollection<PDCGRUPPI>();
            foreach (var grp in groups)
            {
                grp.P1DES1 = grp.P1DES1?.TrimEnd();

                grp.Accounts = new ObservableCollection<PDCCONTI>();
                foreach (var acc in accounts.Where(w => w.P1GRUP == grp.P1GRUP))
                {
                    acc.P2DES1 = acc.P2DES1?.TrimEnd();

                    acc.Subaccounts = new ObservableCollection<PDCSOTTO>();
                    foreach (var sub in subaccounts.Where(w => w.P1GRUP == grp.P1GRUP && w.P2CONT == acc.P2CONT))
                    {
                        sub.P3DES1 = sub.P3DES1?.TrimEnd();

                        sub.Years = new ObservableCollection<PDCANNI>();
                        foreach (var yea in years.Where(w => w.P1GRUP == grp.P1GRUP && w.P2CONT == acc.P2CONT && w.P3SOTC == sub.P3SOTC))
                        {
                            sub.Years.Add(yea);
                        }
                        acc.Subaccounts.Add(sub);
                    }
                    grp.Accounts.Add(acc);
                }
                list.Add(grp);
            }

            return list;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCGRUPPI? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCGRUPPI>(
                "SELECT * FROM PDC_GRUPPI WHERE P1GRUP = @p1grup",
                new { p1grup = ID })
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
                "SELECT COUNT(*) FROM PDC_GRUPPI WHERE P1GRUP = @p1grup",
                new { p1grup = ID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public bool Insert(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO PDC_GRUPPI (P1GRUP,p1chco,P1TICO,P1DES1,P1DES2,P1OBCP) OUTPUT INSERTED.rv VALUES(@P1GRUP,@p1chco,@P1TICO,@P1DES1,@P1DES2,@P1OBCP)",
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

    public bool Update(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE PDC_GRUPPI SET P1GRUP = @P1GRUP,p1chco = @p1chco,P1TICO = @P1TICO,P1DES1 = @P1DES1,P1DES2 = @P1DES2,P1OBCP = @P1OBCP OUTPUT INSERTED.rv WHERE P1GRUP = @p1grup AND rv = @rv",
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

    public bool CanDelete(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP",
                Model);
            if (result == 0)
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

    public bool Delete(PDCGRUPPI Model)
    {
        try
        {
            using var connection = GetOpenConnection();
            var result = connection.Execute(
                "DELETE FROM PDC_GRUPPI OUTPUT DELETED.rv WHERE P1GRUP = @p1grup AND rv = @rv",
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

    public string? Validate(PDCGRUPPI Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.P1GRUP))
            {
                if ((!Exists(Model.P1GRUP) && IsInsert) || !IsInsert)
                {
                    if (!string.IsNullOrWhiteSpace(Model.P1DES1))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.P1TICO))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.p1chco))
                            {
                                return null;
                            }
                            else
                            { return "Il codice chiusura è obbligatorio"; }
                        }
                        else
                        { return "Il tipo è obbligatorio"; }
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice inserito è già in uso"; }
            }
            else
            { return "Il codice del gruppo è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}