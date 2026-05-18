using VulpesX.DAL;

namespace VulpesX.DAL.Accounting;

public interface IPDCSOTTORepository
{
    List<PDCSOTTO>? GetBasicList(string CompanyID);

    ObservableCollection<PDCSOTTO>? GetList(string CompanyID);

    ObservableCollection<PDCSOTTO>? GetList(string GroupID, string AccountID, string CompanyID);

    PDCSOTTO? Get(string GroupID, string AccountID, string ID, string CompanyID);

    PDCSOTTO? GetFirst(string GroupID, string AccountID, string EntityType, string CompanyID);

    PDCSOTTO? GetFirstByType(string EntityType, string CompanyID);

    bool Exists(string GroupID, string AccountID, string SubaccountID);

    #region CRUD
    bool Insert(PDCSOTTO Model);

    bool Update(PDCSOTTO Model);

    bool CanDelete(PDCSOTTO Model);

    bool Delete(PDCSOTTO Model);

    string? Validate(PDCSOTTO Model, bool IsInsert);
    #endregion
}

public class PDCSOTTORepository : RepositoryBase, IPDCSOTTORepository
{
    public PDCSOTTORepository(IConnectionFactory factory) : base(factory)
    {
    }

    public List<PDCSOTTO>? GetBasicList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PDCSOTTO>(
                "SELECT P1GRUP,P2CONT,P3SOTC,P3DES1,P3DES2, P3CLFO, p3coni FROM PDCSOTTO WHERE P3SOCI IS NULL OR P3SOCI = @p3soci",
                new { p3soci = CompanyID }).ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCSOTTO>(
                "SELECT * FROM PDCSOTTO WHERE P3SOCI IS NULL OR P3SOCI = @p3soci",
                new { p3soci = CompanyID });

            return new ObservableCollection<PDCSOTTO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetList(string GroupID, string AccountID, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCSOTTO>(
                @"SELECT P1GRUP,P2CONT,P3SOTC,P3DES1,P3CLFO,P3SOCI,P3ESTE,p3coni FROM PDCSOTTO 
                        WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND (P3SOCI IS NULL OR P3SOCI = @p3soci)",
                new { p1grup = GroupID, p2cont = AccountID, p3soci = CompanyID });

            return new ObservableCollection<PDCSOTTO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCSOTTO? Get(string GroupID, string AccountID, string ID, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();
            return connection.Query<PDCSOTTO>(
                @"SELECT * FROM PDCSOTTO 
                        WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND (P3SOCI IS NULL OR P3SOCI = @p3soci)",
                new { p1grup = GroupID, p2cont = AccountID, p3sotc = ID, p3soci = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCSOTTO? GetFirst(string GroupID, string AccountID, string EntityType, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCSOTTO>(
                @"SELECT TOP(1) * FROM PDCSOTTO 
                        WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3CLFO = @p3clfo AND (P3SOCI IS NULL OR P3SOCI = @p3soci)
                        ORDER BY P1GRUP, P2CONT, P3SOTC",
                new { p1grup = GroupID, p2cont = AccountID, p3clfo = EntityType, p3soci = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCSOTTO? GetFirstByType(string EntityType, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCSOTTO>(
                @"SELECT TOP(1) * FROM PDCSOTTO 
                        WHERE P3CLFO = @p3clfo AND (P3SOCI IS NULL OR P3SOCI = @p3soci)",
                new { p3clfo = EntityType, p3soci = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string GroupID, string AccountID, string SubaccountID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PDCSOTTO WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc",
                new { p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public bool Insert(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO PDCSOTTO (P1GRUP,P2CONT,P3SOTC,P3DES1,P3DES2,P3OBCP,P3CLFO,p3coni,p3este,p3soci,p3cee1,p3cee2,p3cee3,p3cee4,p3cee5,p3cove,p3ragr,p3racn,p3raso,p3sett,p3movc,p3est2,p3utgr,p3tcon,p3cos,p3filiale,p3tipbl,p3flSpeso,P3CONTO,P3cab,p3abi,p3cee6,p3cee7,p3cee1a,p3cee2a,p3cee3a,p3cee4a,p3cee5a,p3cee6a,p3cee7a) OUTPUT INSERTED.rv VALUES(@P1GRUP,@P2CONT,@P3SOTC,@P3DES1,@P3DES2,@P3OBCP,@P3CLFO,@p3coni,@p3este,@p3soci,@p3cee1,@p3cee2,@p3cee3,@p3cee4,@p3cee5,@p3cove,@p3ragr,@p3racn,@p3raso,@p3sett,@p3movc,@p3est2,@p3utgr,@p3tcon,@p3cos,@p3filiale,@p3tipbl,@p3flSpeso,@P3CONTO,@P3cab,@p3abi,@p3cee6,@p3cee7,@p3cee1a,@p3cee2a,@p3cee3a,@p3cee4a,@p3cee5a,@p3cee6a,@p3cee7a)",
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

    public bool Update(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE PDCSOTTO SET P1GRUP = @P1GRUP,P2CONT = @P2CONT,P3SOTC = @P3SOTC,P3DES1 = @P3DES1,P3DES2 = @P3DES2,P3OBCP = @P3OBCP,P3CLFO = @P3CLFO,p3coni = @p3coni,p3este = @p3este,p3soci = @p3soci,p3cee1 = @p3cee1,p3cee2 = @p3cee2,p3cee3 = @p3cee3,p3cee4 = @p3cee4,p3cee5 = @p3cee5,p3cove = @p3cove,p3ragr = @p3ragr,p3racn = @p3racn,p3raso = @p3raso,p3sett = @p3sett,p3movc = @p3movc,p3est2 = @p3est2,p3utgr = @p3utgr,p3tcon = @p3tcon,p3cos = @p3cos,p3filiale = @p3filiale,p3tipbl = @p3tipbl,p3flSpeso = @p3flSpeso,P3CONTO = @P3CONTO,P3cab = @P3cab,p3abi = @p3abi,p3cee6 = @p3cee6,p3cee7 = @p3cee7,p3cee1a = @p3cee1a,p3cee2a = @p3cee2a,p3cee3a = @p3cee3a,p3cee4a = @p3cee4a,p3cee5a = @p3cee5a,p3cee6a = @p3cee6a,p3cee7a = @p3cee7a OUTPUT INSERTED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND rv = @rv",
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

    public bool CanDelete(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP AND pncont = @P2CONT AND pnsott = @P3SOTC",
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

    public bool Delete(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();
            var result = connection.Execute(
                "DELETE FROM PDCSOTTO OUTPUT DELETED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND rv = @rv",
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

    public string? Validate(PDCSOTTO Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.P3SOTC))
            {
                if (!IsInsert || (IsInsert && !Exists(Model.P1GRUP, Model.P2CONT, Model.P3SOTC)))
                {
                    if (!string.IsNullOrWhiteSpace(Model.P3DES1))
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice inserito è già in uso"; }
            }
            else
            { return "Il codice è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PDCSOTTOUfpRepository : RepositoryBase, IPDCSOTTORepository
{
    public PDCSOTTOUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public List<PDCSOTTO>? GetBasicList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PDCSOTTO>(
                "SELECT P1GRUP,P2CONT,P3SOTC,P3DES1,P3DES2, P3CLFO, p3coni FROM PDC_SOTTOCONTI WHERE NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci",
                new { p3soci = CompanyID }).ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCSOTTO>(
                "SELECT * FROM PDC_SOTTOCONTI WHERE NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci",
                new { p3soci = CompanyID });

            return new ObservableCollection<PDCSOTTO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<PDCSOTTO>? GetList(string GroupID, string AccountID, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<PDCSOTTO>(
                @"SELECT P1GRUP,P2CONT,P3SOTC,P3DES1,P3CLFO,P3SOCI,P3ESTE,p3coni FROM PDC_SOTTOCONTI 
                        WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND (NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci)",
                new { p1grup = GroupID, p2cont = AccountID, p3soci = CompanyID });

            return new ObservableCollection<PDCSOTTO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCSOTTO? Get(string GroupID, string AccountID, string ID, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();
            return connection.Query<PDCSOTTO>(
                @"SELECT * FROM PDC_SOTTOCONTI 
                        WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND (NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci)",
                new { p1grup = GroupID, p2cont = AccountID, p3sotc = ID, p3soci = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCSOTTO? GetFirst(string GroupID, string AccountID, string EntityType, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCSOTTO>(
                @"SELECT TOP(1) * FROM PDC_SOTTOCONTI 
                        WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3CLFO = @p3clfo AND (NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci)
                        ORDER BY P1GRUP, P2CONT, P3SOTC",
                new { p1grup = GroupID, p2cont = AccountID, p3clfo = EntityType, p3soci = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PDCSOTTO? GetFirstByType(string EntityType, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<PDCSOTTO>(
                @"SELECT TOP(1) * FROM PDC_SOTTOCONTI 
                        WHERE P3CLFO = @p3clfo AND (NULLIF(LTRIM(RTRIM(P3SOCI)), '') IS NULL OR P3SOCI = @p3soci)",
                new { p3clfo = EntityType, p3soci = CompanyID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string GroupID, string AccountID, string SubaccountID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PDC_SOTTOCONTI WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc",
                new { p1grup = GroupID, p2cont = AccountID, p3sotc = SubaccountID }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region CRUD
    public bool Insert(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO PDC_SOTTOCONTI (P1GRUP,P2CONT,P3SOTC,P3DES1,P3DES2,P3OBCP,P3CLFO,p3coni,p3este,p3soci,p3cee1,p3cee2,p3cee3,p3cee4,p3cee5,p3cove,p3ragr,p3racn,p3raso,p3sett,p3movc,p3est2,p3utgr,p3tcon,p3cos,p3tipbl,p3flSpeso,p3gval,p3ragmas) OUTPUT INSERTED.rv VALUES(@P1GRUP,@P2CONT,@P3SOTC,@P3DES1,@P3DES2,@P3OBCP,@P3CLFO,@p3coni,@p3este,@p3soci,@p3cee1,@p3cee2,@p3cee3,@p3cee4,@p3cee5,@p3cove,@p3ragr,@p3racn,@p3raso,@p3sett,@p3movc,@p3est2,@p3utgr,@p3tcon,@p3cos,@p3tipbl,@p3flSpeso,@p3gval,@p3ragmas)",
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

    public bool Update(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.ExecuteScalar(
                "UPDATE PDC_SOTTOCONTI SET P1GRUP = @P1GRUP,P2CONT = @P2CONT,P3SOTC = @P3SOTC,P3DES1 = @P3DES1,P3DES2 = @P3DES2,P3OBCP = @P3OBCP,P3CLFO = @P3CLFO,p3coni = @p3coni,p3este = @p3este,p3soci = @p3soci,p3cee1 = @p3cee1,p3cee2 = @p3cee2,p3cee3 = @p3cee3,p3cee4 = @p3cee4,p3cee5 = @p3cee5,p3cove = @p3cove,p3ragr = @p3ragr,p3racn = @p3racn,p3raso = @p3raso,p3sett = @p3sett,p3movc = @p3movc,p3est2 = @p3est2,p3utgr = @p3utgr,p3tcon = @p3tcon,p3cos = @p3cos,p3tipbl = @p3tipbl,p3flSpeso = @p3flSpeso,p3gval=@p3gval,p3ragmas=@p3ragmas OUTPUT INSERTED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND rv = @rv",
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

    public bool CanDelete(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM PNRIGHE WHERE pngrup = @P1GRUP AND pncont = @P2CONT AND pnsott = @P3SOTC",
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

    public bool Delete(PDCSOTTO Model)
    {
        try
        {
            using var connection = GetOpenConnection();
            var result = connection.Execute(
                "DELETE FROM PDC_SOTTOCONTI OUTPUT DELETED.rv WHERE P1GRUP = @p1grup AND P2CONT = @p2cont AND P3SOTC = @p3sotc AND rv = @rv",
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

    public string? Validate(PDCSOTTO Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.P3SOTC))
            {
                if (!IsInsert || (IsInsert && !Exists(Model.P1GRUP, Model.P2CONT, Model.P3SOTC)))
                {
                    if (!string.IsNullOrWhiteSpace(Model.P3DES1))
                    {
                        return null;
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice inserito è già in uso"; }
            }
            else
            { return "Il codice è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}