using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VulpesX.DAL;
using VulpesX.DAL._ConnectionFactory;
using VulpesX.DAL.Accounting;

namespace VulpesX.DAL.Tables.Accounting;

public interface ISUPPLIER_GROUPSRepository
{
    ObservableCollection<SUPPLIER_GROUPS>? GetList();

    ObservableCollection<SUPPLIER_GROUPS>? GetList(string ccfsoc, int cfcode);

    ObservableCollection<SUPPLIER_GROUPS>? GetList(string ccfsoc, int cfcode, string cfcaus);

    ObservableCollection<SUPPLIER_GROUPS>? GetListFull(string ccfsoc, int cfcode, string cfcaus);

    SUPPLIER_GROUPS? Get(string ccfsoc, int cfcode, int cfprog);

    bool Exists(string ccfsoc, int cfcode, int cfprog);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(SUPPLIER_GROUPS Model);

    bool Update(SUPPLIER_GROUPS Model);

    bool Delete(SUPPLIER_GROUPS Model);

    string? Validate(SUPPLIER_GROUPS Model, bool IsInsert);
    #endregion
}

public class SUPPLIER_GROUPSRepository : RepositoryBase, ISUPPLIER_GROUPSRepository
{
    public SUPPLIER_GROUPSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SUPPLIER_GROUPS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<SUPPLIER_GROUPS>(
                @"SELECT * FROM SUPPLIER_GROUPS
                        ORDER BY cfprog");

            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<SUPPLIER_GROUPS>? GetList(string ccfsoc, int cfcode)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SUPPLIER_GROUPS>(
                @"SELECT * FROM SUPPLIER_GROUPS
                        WHERE ccfsoc=@ccfsoc AND cfcode=@cfcode
                        ORDER BY cfprog", new { ccfsoc = ccfsoc, cfcode = cfcode });

            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<SUPPLIER_GROUPS>? GetList(string ccfsoc, int cfcode, string cfcaus)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SUPPLIER_GROUPS>(
                @"SELECT * FROM SUPPLIER_GROUPS
                        WHERE ccfsoc=@ccfsoc AND cfcode=@cfcode AND cfcaus=@cfcaus
                        ORDER BY cfprog", new { ccfsoc = ccfsoc, cfcode = cfcode, cfcaus = cfcaus });

            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<SUPPLIER_GROUPS>? GetListFull(string ccfsoc, int cfcode, string cfcaus)
    {
        try
        {
            using var connection = GetOpenConnection();

            var accountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            var subaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(ccfsoc);

            var list = connection.Query<SUPPLIER_GROUPS, PDCGRUPPI, PDCCONTI, PDCSOTTO, SUPPLIER_GROUPS>(
                @"SELECT sg.*, g.*, c.*, s.* FROM SUPPLIER_GROUPS AS sg
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = sg.cfgrup
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = sg.cfgrup AND c.P2CONT = sg.cfcont
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = sg.cfgrup AND s.P2CONT = sg.cfcont AND s.P3SOTC = sg.cfsott
                        WHERE sg.ccfsoc=@ccfsoc AND sg.cfcode=@cfcode AND sg.cfcaus=@cfcaus
                        ORDER BY sg.cfprog",
                (sg, grp, cnt, sct) =>
                {
                    sg.AccountCache = accountCache;
                    sg.SubaccountCache = subaccountCache;
                    sg.SelectedGroup = grp;
                    sg.SelectedAccount = cnt;
                    sg.SelectedSubaccount = sct;
                    return sg;
                },
                new { ccfsoc = ccfsoc, cfcode = cfcode, cfcaus = cfcaus }, splitOn: "P1GRUP, P1GRUP, P1GRUP");
            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SUPPLIER_GROUPS? Get(string ccfsoc, int cfcode, int cfprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SUPPLIER_GROUPS>(
                "SELECT * FROM SUPPLIER_GROUPS WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfprog = @cfprog",
                new { ccfsoc = ccfsoc, cfcode = cfcode, cfprog = cfprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ccfsoc, int cfcode, int cfprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM SUPPLIER_GROUPS WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfprog = @cfprog",
                new { ccfsoc = ccfsoc, cfcode = cfcode, cfprog = cfprog }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO SUPPLIER_GROUPS (ccfsoc,cfcode,cfprog,cfgrup,cfcont,cfsott,cfsegn,cfcaus) OUTPUT INSERTED.rv VALUES(@ccfsoc,@cfcode,@cfprog,@cfgrup,@cfcont,@cfsott,@cfsegn,@cfcaus)";
    public string UPDATE_QUERY => "UPDATE SUPPLIER_GROUPS SET ccfsoc = @ccfsoc,cfcode = @cfcode,cfprog = @cfprog,cfgrup = @cfgrup,cfcont = @cfcont,cfsott = @cfsott,cfsegn = @cfsegn,cfcaus = @cfcaus OUTPUT INSERTED.rv WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfprog = @cfprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM SUPPLIER_GROUPS OUTPUT DELETED.rv WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfprog = @cfprog AND rv = @rv";
    public bool Insert(SUPPLIER_GROUPS Model)
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

    public bool Update(SUPPLIER_GROUPS Model)
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

    public bool Delete(SUPPLIER_GROUPS Model)
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

    public string? Validate(SUPPLIER_GROUPS Model, bool IsInsert)
    {
        try
        {
            if (Model.cfcode > 0)
            {
                if (Model.SelectedGroup != null && Model.SelectedAccount != null && Model.SelectedSubaccount != null)
                {
                    if (Model.SelectedCausal != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.cfsegn))
                        {
                            return null;
                        }
                        else
                        { return "Il segno è obbligatorio"; }
                    }
                    else
                    { return "La causale contabile è obbligatoria"; }
                }
                else
                { return "Gruppo, conto e sottoconto sono obbligatori"; }
            }
            else
            { return "Il codice fornitore è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class SUPPLIER_GROUPSUfpRepository : RepositoryBase, ISUPPLIER_GROUPSRepository
{
    public SUPPLIER_GROUPSUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<SUPPLIER_GROUPS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<SUPPLIER_GROUPS>(
                @"SELECT 
ccfsoc,
cfcode,
cfcaus,
cfriga as cfprog,
cfgrup,
cfcont,
cfsott,
cfsegn, 
cffase as cffase
FROM PNCLIFORLEVEL1
                        ORDER BY cfprog");

            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    
    public ObservableCollection<SUPPLIER_GROUPS>? GetList(string ccfsoc, int cfcode)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SUPPLIER_GROUPS>(
                @"SELECT 
ccfsoc,
cfcode,
cfcaus,
cfriga as cfprog,
cfgrup,
cfcont,
cfsott,
cfsegn, 
cffase as cffase
FROM PNCLIFORLEVEL1
                        WHERE ccfsoc=@ccfsoc AND cfcode=@cfcode
                        ORDER BY cfprog", new { ccfsoc = ccfsoc, cfcode = cfcode });

            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<SUPPLIER_GROUPS>? GetList(string ccfsoc, int cfcode, string cfcaus)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SUPPLIER_GROUPS>(
                @"SELECT ccfsoc,
cfcode,
cfcaus,
cfriga as cfprog,
cfgrup,
cfcont,
cfsott,
cfsegn, 
cffase as cffase
FROM PNCLIFORLEVEL1
                        WHERE ccfsoc=@ccfsoc AND cfcode=@cfcode AND cfcaus=@cfcaus
                        ORDER BY cfprog", new { ccfsoc = ccfsoc, cfcode = cfcode, cfcaus = cfcaus });

            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<SUPPLIER_GROUPS>? GetListFull(string ccfsoc, int cfcode, string cfcaus)
    {
        try
        {
            using var connection = GetOpenConnection();

            var accountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            var subaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(ccfsoc);

            var list = connection.Query<SUPPLIER_GROUPS, PDCGRUPPI, PDCCONTI, PDCSOTTO, SUPPLIER_GROUPS>(
                @"SELECT 
sg.ccfsoc,
sg.cfcode,
sg.cfcaus,
sg.cfriga as cfprog,
sg.cfgrup,
sg.cfcont,
sg.cfsott,
sg.cfsegn, 
sg.cffase as cffase, g.*, c.*, s.* FROM PNCLIFORLEVEL1 AS sg
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP = sg.cfgrup
                        INNER JOIN PDC_CONTI AS c ON c.P1GRUP = sg.cfgrup AND c.P2CONT = sg.cfcont
                        INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP = sg.cfgrup AND s.P2CONT = sg.cfcont AND s.P3SOTC = sg.cfsott
                        WHERE sg.ccfsoc=@ccfsoc AND sg.cfcode=@cfcode AND sg.cfcaus=@cfcaus
                        ORDER BY sg.cfriga",
                (sg, grp, cnt, sct) =>
                {
                    sg.AccountCache = accountCache;
                    sg.SubaccountCache = subaccountCache;
                    sg.SelectedGroup = grp;
                    sg.SelectedAccount = cnt;
                    sg.SelectedSubaccount = sct;
                    return sg;
                },
                new { ccfsoc = ccfsoc, cfcode = cfcode, cfcaus = cfcaus }, splitOn: "P1GRUP, P1GRUP, P1GRUP");
            return new ObservableCollection<SUPPLIER_GROUPS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SUPPLIER_GROUPS? Get(string ccfsoc, int cfcode, int cfprog)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SUPPLIER_GROUPS>(
                @$"SELECT 
ccfsoc,
cfcode,
cfcaus,
cfriga as cfprog,
cfgrup,
cfcont,
cfsott,
cfsegn, 
cffase as cffase
FROM PNCLIFORLEVEL1 
                WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfprog = @cfprog",
                new { ccfsoc = ccfsoc, cfcode = cfcode, cfprog = cfprog })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ccfsoc, int cfcode, int cfprog)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNCLIFORLEVEL1 WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfprog = @cfprog",
                new { ccfsoc = ccfsoc, cfcode = cfcode, cfprog = cfprog }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PNCLIFORLEVEL1 (ccfsoc,cfcode,cfriga,cfgrup,cfcont,cfsott,cfsegn,cfcaus,cffase) OUTPUT INSERTED.rv VALUES(@ccfsoc,@cfcode,@cfprog,@cfgrup,@cfcont,@cfsott,@cfsegn,@cfcaus,@cffase)";
    public string UPDATE_QUERY => "UPDATE PNCLIFORLEVEL1 SET ccfsoc = @ccfsoc,cfcode = @cfcode,cfprog = @cfprog,cfgrup = @cfgrup,cfcont = @cfcont,cfsott = @cfsott,cfsegn = @cfsegn,cfcaus = @cfcaus, cffase = @cffase OUTPUT INSERTED.rv WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfriga = @cfprog AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PNCLIFORLEVEL1 OUTPUT DELETED.rv WHERE ccfsoc = @ccfsoc AND cfcode = @cfcode AND cfriga = @cfprog AND rv = @rv";
    public bool Insert(SUPPLIER_GROUPS Model)
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

    public bool Update(SUPPLIER_GROUPS Model)
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

    public bool Delete(SUPPLIER_GROUPS Model)
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

    public string? Validate(SUPPLIER_GROUPS Model, bool IsInsert)
    {
        try
        {
            if (Model.cfcode > 0)
            {
                if (Model.SelectedGroup != null && Model.SelectedAccount != null && Model.SelectedSubaccount != null)
                {
                    if (Model.SelectedCausal != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.cfsegn))
                        {
                            return null;
                        }
                        else
                        { return "Il segno è obbligatorio"; }
                    }
                    else
                    { return "La causale contabile è obbligatoria"; }
                }
                else
                { return "Gruppo, conto e sottoconto sono obbligatori"; }
            }
            else
            { return "Il codice fornitore è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}