using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace VulpesX.DAL.Accounting;

public interface IPDCANNIRepository
{

    ObservableCollection<PDCANNI>? GetList(string CompanyID);

    ObservableCollection<PDCANNI>? GetListByYear(string CompanyID, int Year);

    ObservableCollection<PDCANNI>? GetListByYearTypes(string CompanyID, int Year, string[] GroupTypes);

    ObservableCollection<PDCANNI>? GetListByYearTypesNoCustomerNoSupplier(string CompanyID, int Year, string[] GroupTypes);

    bool Generate(string CompanyID, int Year);

    PDCANNI? Get(string CompanyID, string GroupID, string AccountID, string SubaccountID, int ID);

    PDCANNI? GetFull(string CompanyID, string GroupID, string AccountID, string SubaccountID, int ID);

    bool Exists(string CompanyID, string GroupID, string AccountID, string SubaccountID, int Year);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(PDCANNI Model);

    bool Update(PDCANNI Model);

    bool CanDelete(PDCANNI Model);

    bool Delete(PDCANNI Model);

    string? Validate(PDCANNI Model, bool IsInsert);
    #endregion
}

public class PDCANNIRepository : RepositoryBase, IPDCANNIRepository
{
    public PDCANNIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PDCANNI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCANNI>(
                "SELECT * FROM PDCANNI WHERE P1SOCI = @cid ORDER BY P4ANNO DESC",
                new { cid = CompanyID });

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCANNI>? GetListByYear(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1 , TRIM(g.P1DES2) AS P1DES2, c.P2CONT AS P2CONT, TRIM(c.P2DES1) AS P2DES1 , TRIM(c.P2DES2) AS P2DES2, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDCANNI AS a
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P4ANNO = @yea",
                (ann, group, account, sott) =>
                {
                    ann.Group = group;
                    ann.Account = account;
                    ann.Subaccount = sott;
                    ann.SubaccountDescription = sott.P3DES1;
                    return ann;
                },
                new { cid = CompanyID, yea = Year },
                splitOn: "P1GRUP,P2CONT,P3SOTC");

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCANNI>? GetListByYearTypes(string CompanyID, int Year, string[] GroupTypes)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1, g.P1TICO , TRIM(g.P1DES2) AS P1DES2, c.*, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDCANNI AS a
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P4ANNO = @yea AND g.P1TICO IN @typs",
                (ann, group, account, sott) =>
                {
                    ann.Group = group;
                    ann.Account = account;
                    ann.Subaccount = sott;
                    ann.SubaccountDescription = sott.P3DES1;

                    return ann;
                },
                new { cid = CompanyID, yea = Year, typs = GroupTypes },
                splitOn: "P1GRUP,P2CONT,P3SOTC");

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCANNI>? GetListByYearTypesNoCustomerNoSupplier(string CompanyID, int Year, string[] GroupTypes)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1, g.P1TICO , TRIM(g.P1DES2) AS P1DES2, c.P2CONT AS P2CONT, TRIM(c.P2DES1) AS P2DES1 , TRIM(c.P2DES2) AS P2DES2, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDCANNI AS a
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P4ANNO = @yea AND g.P1TICO IN @typs AND c.p2flcf <> 'C' AND c.p2flcf <> 'F'",
                (ann, group, account, sott) =>
                {
                    ann.Group = group;
                    ann.Account = account;
                    ann.Subaccount = sott;
                    ann.SubaccountDescription = sott.P3DES1;

                    return ann;
                },
                new { cid = CompanyID, yea = Year, typs = GroupTypes },
                splitOn: "P1GRUP,P2CONT,P3SOTC");

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Generate(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();

            var groups = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Parallel.ForEach(VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID) ?? new ObservableCollection<PDCSOTTO>(), (sott) =>
            {
                Insert(new PDCANNI()
                {
                    P1SOCI = CompanyID,
                    P1GRUP = sott.P1GRUP,
                    P2CONT = sott.P2CONT,
                    P3SOTC = sott.P3SOTC,
                    P4ANNO = Year,
                    P1CCHI = groups?.Where(w => w.P1GRUP == sott.P1GRUP).FirstOrDefault()?.p1chco
                });
            });
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public PDCANNI? Get(string CompanyID, string GroupID, string AccountID, string SubaccountID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PDCANNI>(
                @"SELECT * FROM PDCANNI WHERE P1SOCI = @cid AND P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND P4ANNO = @p4anno",
                new { cid = CompanyID, p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID, p4anno = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCANNI? GetFull(string CompanyID, string GroupID, string AccountID, string SubaccountID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1 , TRIM(g.P1DES2) AS P1DES2, c.P2CONT AS P2CONT, TRIM(c.P2DES1) AS P2DES1 , TRIM(c.P2DES2) AS P2DES2, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDCANNI AS a
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P1GRUP=@p1grup AND a.P2CONT=@p2cont AND a.P3SOTC=@p3sotc AND a.P4ANNO = @p4anno",
                (ann, group, account, sott) => { ann.Group = group; ann.Account = account; ann.Subaccount = sott; return ann; },
                new { cid = CompanyID, p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID, p4anno = ID }, splitOn: "P1GRUP,P2CONT,P3SOTC")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string GroupID, string AccountID, string SubaccountID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PDCANNI WHERE P1SOCI=@p1soci AND P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND P4ANNO = @p4anno",
                new { p1soci = CompanyID, p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID, p4anno = Year }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PDCANNI (P1SOCI,P1GRUP,P2CONT,P3SOTC,P4ANNO,P1CCHI,P4DAPE,P4AVPE,P4DAES,P4AVES,P4PAM2,P4CORA,P4DATA,p4dpe,p4ape,p4dee,p4aee) OUTPUT INSERTED.rv VALUES(@P1SOCI,@P1GRUP,@P2CONT,@P3SOTC,@P4ANNO,@P1CCHI,@P4DAPE,@P4AVPE,@P4DAES,@P4AVES,@P4PAM2,@P4CORA,@P4DATA,@p4dpe,@p4ape,@p4dee,@p4aee)";
    public string UPDATE_QUERY => "UPDATE PDCANNI SET P1SOCI = @P1SOCI,P1GRUP = @P1GRUP,P2CONT = @P2CONT,P3SOTC = @P3SOTC,P4ANNO = @P4ANNO,P1CCHI = @P1CCHI,P4DAPE = @P4DAPE,P4AVPE = @P4AVPE,P4DAES = @P4DAES,P4AVES = @P4AVES,P4PAM2 = @P4PAM2,P4CORA = @P4CORA,P4DATA = @P4DATA,p4dpe = @p4dpe,p4ape = @p4ape,p4dee = @p4dee,p4aee = @p4aee OUTPUT INSERTED.rv WHERE P1SOCI = @P1SOCI AND P1GRUP = @P1GRUP AND P2CONT = @P2CONT AND P3SOTC = @P3SOTC AND P4ANNO = @P4ANNO AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PDCANNI OUTPUT DELETED.rv WHERE P1SOCI = @P1SOCI AND P1GRUP = @P1GRUP AND P2CONT = @P2CONT AND P3SOTC = @P3SOTC AND P4ANNO = @P4ANNO AND rv = @rv";
    public bool Insert(PDCANNI Model)
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

    public bool Update(PDCANNI Model)
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

    public bool CanDelete(PDCANNI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP AND pncont = @P2CONT AND pnsott = @P3SOTC AND N1ANNO = @P4ANNO AND N1SOCI = @P1SOCI",
                Model);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(PDCANNI Model)
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

    public string? Validate(PDCANNI Model, bool IsInsert)
    {
        try
        {
            if (Model.P4ANNO > 0)
            {
                if (!IsInsert || (IsInsert && !Exists(Model.P1SOCI, Model.P1GRUP, Model.P2CONT, Model.P3SOTC, Model.P4ANNO)))
                {
                    return null;
                }
                else
                { return "L'anno inserito è già in uso"; }
            }
            else
            { return "L'anno è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PDCANNIUfpRepository : RepositoryBase, IPDCANNIRepository
{
    public PDCANNIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PDCANNI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCANNI>(
                "SELECT * FROM PDC_ANNI WHERE P1SOCI = @cid ORDER BY P4ANNO DESC",
                new { cid = CompanyID });

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCANNI>? GetListByYear(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1 , TRIM(g.P1DES2) AS P1DES2, c.P2CONT AS P2CONT, TRIM(c.P2DES1) AS P2DES1 , TRIM(c.P2DES2) AS P2DES2, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDC_ANNI AS a
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDC_CONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P4ANNO = @yea",
                (ann, group, account, sott) =>
                {
                    ann.Group = group;
                    ann.Account = account;
                    ann.Subaccount = sott;
                    ann.SubaccountDescription = sott.P3DES1;
                    return ann;
                },
                new { cid = CompanyID, yea = Year },
                splitOn: "P1GRUP,P2CONT,P3SOTC");

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCANNI>? GetListByYearTypes(string CompanyID, int Year, string[] GroupTypes)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1, g.P1TICO , TRIM(g.P1DES2) AS P1DES2, c.*, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDC_ANNI AS a
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDC_CONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDC_SOTTO AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P4ANNO = @yea AND g.P1TICO IN @typs",
                (ann, group, account, sott) =>
                {
                    ann.Group = group;
                    ann.Account = account;
                    ann.Subaccount = sott;
                    ann.SubaccountDescription = sott.P3DES1;

                    return ann;
                },
                new { cid = CompanyID, yea = Year, typs = GroupTypes },
                splitOn: "P1GRUP,P2CONT,P3SOTC");

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCANNI>? GetListByYearTypesNoCustomerNoSupplier(string CompanyID, int Year, string[] GroupTypes)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1, g.P1TICO , TRIM(g.P1DES2) AS P1DES2, c.P2CONT AS P2CONT, TRIM(c.P2DES1) AS P2DES1 , TRIM(c.P2DES2) AS P2DES2, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDC_ANNI AS a
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDC_CONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDC_SOTTO AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P4ANNO = @yea AND g.P1TICO IN @typs AND c.p2flcf <> 'C' AND c.p2flcf <> 'F'",
                (ann, group, account, sott) =>
                {
                    ann.Group = group;
                    ann.Account = account;
                    ann.Subaccount = sott;
                    ann.SubaccountDescription = sott.P3DES1;

                    return ann;
                },
                new { cid = CompanyID, yea = Year, typs = GroupTypes },
                splitOn: "P1GRUP,P2CONT,P3SOTC");

            return new ObservableCollection<PDCANNI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Generate(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();

            var groups = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Parallel.ForEach(VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID) ?? new ObservableCollection<PDCSOTTO>(), (sott) =>
            {
                Insert(new PDCANNI()
                {
                    P1SOCI = CompanyID,
                    P1GRUP = sott.P1GRUP,
                    P2CONT = sott.P2CONT,
                    P3SOTC = sott.P3SOTC,
                    P4ANNO = Year,
                    P1CCHI = groups?.Where(w => w.P1GRUP == sott.P1GRUP).FirstOrDefault()?.p1chco
                });
            });
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public PDCANNI? Get(string CompanyID, string GroupID, string AccountID, string SubaccountID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PDCANNI>(
                @"SELECT * FROM PDC_ANNI WHERE P1SOCI = @cid AND P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND P4ANNO = @p4anno",
                new { cid = CompanyID, p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID, p4anno = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCANNI? GetFull(string CompanyID, string GroupID, string AccountID, string SubaccountID, int ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PDCANNI, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCANNI>(
                @"SELECT a.*, g.P1GRUP AS P1GRUP, TRIM(g.P1DES1) AS P1DES1 , TRIM(g.P1DES2) AS P1DES2, c.P2CONT AS P2CONT, TRIM(c.P2DES1) AS P2DES1 , TRIM(c.P2DES2) AS P2DES2, s.P3SOTC AS P3SOTC, TRIM(s.P3DES1) AS P3DES1 , TRIM(s.P3DES2) AS P3DES2 FROM PDC_ANNI AS a
                        INNER JOIN PDC_GRUPPI AS g ON g.P1GRUP = a.P1GRUP
                        INNER JOIN PDC_CONTI AS c ON c.P1GRUP = a.P1GRUP AND c.P2CONT = a.P2CONT
                        INNER JOIN PDC_SOTTOCONTI AS s ON s.P1GRUP = a.P1GRUP AND s.P2CONT = a.P2CONT AND s.P3SOTC = a.P3SOTC
                        WHERE a.P1SOCI = @cid AND a.P1GRUP=@p1grup AND a.P2CONT=@p2cont AND a.P3SOTC=@p3sotc AND a.P4ANNO = @p4anno",
                (ann, group, account, sott) => { ann.Group = group; ann.Account = account; ann.Subaccount = sott; return ann; },
                new { cid = CompanyID, p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID, p4anno = ID }, splitOn: "P1GRUP,P2CONT,P3SOTC")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string GroupID, string AccountID, string SubaccountID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PDC_ANNI WHERE P1SOCI=@p1soci AND P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND P4ANNO = @p4anno",
                new { p1soci = CompanyID, p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID, p4anno = Year }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PDC_ANNI (P1SOCI,P1GRUP,P2CONT,P3SOTC,P4ANNO,P1CCHI,P4DAPE,P4AVPE,P4DAES,P4AVES,P4PAM2,P4CORA,P4DATA,p4dpe,p4ape,p4dee,p4aee,p4deva,p4aeva) OUTPUT INSERTED.rv VALUES(@P1SOCI,@P1GRUP,@P2CONT,@P3SOTC,@P4ANNO,@P1CCHI,@P4DAPE,@P4AVPE,@P4DAES,@P4AVES,@P4PAM2,@P4CORA,@P4DATA,@p4dpe,@p4ape,@p4dee,@p4aee,@p4deva,@p4aeva)";
    public string UPDATE_QUERY => "UPDATE PDC_ANNI SET P1SOCI = @P1SOCI,P1GRUP = @P1GRUP,P2CONT = @P2CONT,P3SOTC = @P3SOTC,P4ANNO = @P4ANNO,P1CCHI = @P1CCHI,P4DAPE = @P4DAPE,P4AVPE = @P4AVPE,P4DAES = @P4DAES,P4AVES = @P4AVES,P4PAM2 = @P4PAM2,P4CORA = @P4CORA,P4DATA = @P4DATA,p4dpe = @p4dpe,p4ape = @p4ape,p4dee = @p4dee,p4aee = @p4aee,p4deva = @p4deva,p4aeva = @p4aeva OUTPUT INSERTED.rv WHERE P1SOCI = @P1SOCI AND P1GRUP = @P1GRUP AND P2CONT = @P2CONT AND P3SOTC = @P3SOTC AND P4ANNO = @P4ANNO AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PDC_ANNI OUTPUT DELETED.rv WHERE P1SOCI = @P1SOCI AND P1GRUP = @P1GRUP AND P2CONT = @P2CONT AND P3SOTC = @P3SOTC AND P4ANNO = @P4ANNO AND rv = @rv";
    public bool Insert(PDCANNI Model)
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

    public bool Update(PDCANNI Model)
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

    public bool CanDelete(PDCANNI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP AND pncont = @P2CONT AND pnsott = @P3SOTC AND N1ANNO = @P4ANNO AND N1SOCI = @P1SOCI",
                Model);
            if (result == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(PDCANNI Model)
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

    public string? Validate(PDCANNI Model, bool IsInsert)
    {
        try
        {
            if (Model.P4ANNO > 0)
            {
                if (!IsInsert || (IsInsert && !Exists(Model.P1SOCI, Model.P1GRUP, Model.P2CONT, Model.P3SOTC, Model.P4ANNO)))
                {
                    return null;
                }
                else
                { return "L'anno inserito è già in uso"; }
            }
            else
            { return "L'anno è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}