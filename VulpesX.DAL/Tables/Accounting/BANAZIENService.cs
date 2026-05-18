using VulpesX.DAL;

namespace VulpesX.DAL.Tables.Accounting;

public interface IBANAZIENRepository
{

    ObservableCollection<BANAZIEN>? GetListAll(string CompanyID);

    ObservableCollection<BANAZIEN>? GetListActive(string CompanyID);

    BANAZIEN? Get(string CompanyID, int ABI, int CAB, string Account);

    bool Exists(string CompanyID, int ABI, int CAB, string Account);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(BANAZIEN Model);

    bool Update(BANAZIEN Model);

    bool Delete(BANAZIEN Model);

    string? Validate(BANAZIEN Model, bool IsInsert);
    #endregion
}

public class BANAZIENRepository : RepositoryBase, IBANAZIENRepository
{
    public BANAZIENRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BANAZIEN>? GetListAll(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BANAZIEN>(
                @"SELECT r.*, l.abiban AS BankName, l.abiage AS BankAgency FROM BANAZIEN AS r 
                        INNER JOIN ABICAB as l ON r.abiabi = l.abiabi AND r.abicab = l.abicab 
                        WHERE r.abisoc = @cid",
                new { cid = CompanyID });

            return new ObservableCollection<BANAZIEN>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BANAZIEN>? GetListActive(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BANAZIEN>(
                @"SELECT r.*, l.abiban AS BankName, l.abiage AS BankAgency FROM BANAZIEN AS r 
                        INNER JOIN ABICAB as l ON r.abiabi = l.abiabi AND r.abicab = l.abicab 
                        WHERE r.abisoc = @cid AND r.abiatt = 1",
                new { cid = CompanyID });

            return new ObservableCollection<BANAZIEN>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BANAZIEN? Get(string CompanyID, int ABI, int CAB, string Account)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BANAZIEN, ABICAB, BANAZIEN>(
                @"SELECT l.*, r.* FROM BANAZIEN AS l
                        INNER JOIN ABICAB AS r ON r.abiabi = l.abiabi AND r.abicab = l.abicab
                        WHERE l.abisoc = @cid AND l.abiabi = @abi AND l.abicab = @cab AND l.abicon = @con",
                (ban, abi) => { ban.Bank = abi; return ban; },
                new { cid = CompanyID, abi = ABI, cab = CAB, con = Account },
                splitOn: "abiabi")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, int ABI, int CAB, string Account)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM BANAZIEN
                        WHERE abisoc = @cid AND abiabi = @abi AND abicab = @cab AND abicon = @con",
                new { cid = CompanyID, abi = ABI, cab = CAB, con = Account }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO BANAZIEN (abisoc,abiabi,abicab,abicon,abigba,abicba,abisba,abisia,abigru,abicot,abisot,abitip,abigru1,abicon1,abisot1,abigru2,abicon2,abisot2,abigru3,abicon3,abisot3,abigru4,abicon4,abisot4,abigru5,abicon5,abisot5,abicau1,abicau2,abicau3,abicau4,abibiba,abibic,abicin,abibba,abiisocod,abigrb,abicob,abisob,abiantita,abiantestero,scrittdett,abivalport,abifidant,abispeban,abicuc,abicau5,abiatt,abisigla,abifidcas) OUTPUT INSERTED.rv VALUES(@abisoc,@abiabi,@abicab,@abicon,@abigba,@abicba,@abisba,@abisia,@abigru,@abicot,@abisot,@abitip,@abigru1,@abicon1,@abisot1,@abigru2,@abicon2,@abisot2,@abigru3,@abicon3,@abisot3,@abigru4,@abicon4,@abisot4,@abigru5,@abicon5,@abisot5,@abicau1,@abicau2,@abicau3,@abicau4,@abibiba,@abibic,@abicin,@abibba,@abiisocod,@abigrb,@abicob,@abisob,@abiantita,@abiantestero,@scrittdett,@abivalport,@abifidant,@abispeban,@abicuc,@abicau5,@abiatt,@abisigla,@abifidcas)";
    public string UPDATE_QUERY => "UPDATE BANAZIEN SET abisoc = @abisoc,abiabi = @abiabi,abicab = @abicab,abicon = @abicon,abigba = @abigba,abicba = @abicba,abisba = @abisba,abisia = @abisia,abigru = @abigru,abicot = @abicot,abisot = @abisot,abitip = @abitip,abigru1 = @abigru1,abicon1 = @abicon1,abisot1 = @abisot1,abigru2 = @abigru2,abicon2 = @abicon2,abisot2 = @abisot2,abigru3 = @abigru3,abicon3 = @abicon3,abisot3 = @abisot3,abigru4 = @abigru4,abicon4 = @abicon4,abisot4 = @abisot4,abigru5 = @abigru5,abicon5 = @abicon5,abisot5 = @abisot5,abicau1 = @abicau1,abicau2 = @abicau2,abicau3 = @abicau3,abicau4 = @abicau4,abibiba = @abibiba,abibic = @abibic,abicin = @abicin,abibba = @abibba,abiisocod = @abiisocod,abigrb = @abigrb,abicob = @abicob,abisob = @abisob,abiantita = @abiantita,abiantestero = @abiantestero,scrittdett = @scrittdett,abivalport = @abivalport,abifidant = @abifidant,abispeban = @abispeban,abicuc = @abicuc,abicau5 = @abicau5,abiatt = @abiatt,abisigla = @abisigla,abifidcas = @abifidcas OUTPUT INSERTED.rv WHERE abisoc = @abisoc AND abiabi = @abiabi AND abicab = @abicab AND abicon = @abicon AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM BANAZIEN OUTPUT DELETED.rv WHERE abisoc = @abisoc AND abiabi = @abiabi AND abicab = @abicab AND abicon = @abicon AND rv = @rv";
    public bool Insert(BANAZIEN Model)
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

    public bool Update(BANAZIEN Model)
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

    public bool Delete(BANAZIEN Model)
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

    public string? Validate(BANAZIEN Model, bool IsInsert)
    {
        try
        {
            if ((IsInsert && !string.IsNullOrWhiteSpace(Model.abisoc) && Model.abiabi > 0 && Model.abicab > 0 && !string.IsNullOrWhiteSpace(Model.abicon) && !Exists(Model.abisoc, Model.abiabi, Model.abicab, Model.abicon)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.abisia) && Model.abisia.Trim().Length == 5)
                {
                    return null;
                }
                else
                { return "Il codice SIA è obbligatorio e deve essere lungo 5 caratteri"; }
            }
            else
            { return "ABI, CAB e conto sono già presenti in questa società o non sono validi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class BANAZIENUfpRepository : RepositoryBase, IBANAZIENRepository
{
    public BANAZIENUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BANAZIEN>? GetListAll(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BANAZIEN>(
                @"SELECT r.*,  CASE r.abisos
        WHEN 'N' THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END AS abiatt,  l.abiban AS BankName, l.abiage AS BankAgency FROM BANAZIEN AS r 
                        INNER JOIN TAB_ABICAB as l ON r.abiabi = l.abiabi AND r.abicab = l.abicab 
                        WHERE r.abisoc = @cid",
                new { cid = CompanyID });

            return new ObservableCollection<BANAZIEN>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<BANAZIEN>? GetListActive(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BANAZIEN>(
                @"SELECT r.*,  CASE r.abisos
        WHEN 'N' THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END AS abiatt, l.abiban AS BankName, l.abiage AS BankAgency FROM BANAZIEN AS r 
                        INNER JOIN TAB_ABICAB as l ON r.abiabi = l.abiabi AND r.abicab = l.abicab 
                        WHERE r.abisoc = @cid AND r.abisos = 'N'",
                new { cid = CompanyID });

            return new ObservableCollection<BANAZIEN>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BANAZIEN? Get(string CompanyID, int ABI, int CAB, string Account)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BANAZIEN, ABICAB, BANAZIEN>(
                @"SELECT l.*,  CASE l.abisos
        WHEN 'N' THEN CAST(1 AS bit)
        ELSE CAST(0 AS bit)
    END AS abiatt, r.* FROM BANAZIEN AS l
                        INNER JOIN TAB_ABICAB AS r ON r.abiabi = l.abiabi AND r.abicab = l.abicab
                        WHERE l.abisoc = @cid AND l.abiabi = @abi AND l.abicab = @cab AND l.abicon = @con",
                (ban, abi) => { ban.Bank = abi; return ban; },
                new { cid = CompanyID, abi = ABI, cab = CAB, con = Account },
                splitOn: "abiabi")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, int ABI, int CAB, string Account)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM BANAZIEN
                        WHERE abisoc = @cid AND abiabi = @abi AND abicab = @cab AND abicon = @con",
                new { cid = CompanyID, abi = ABI, cab = CAB, con = Account }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO BANAZIEN (abisoc,abiabi,abicab,abicon,abigba,abicba,abisba,abisia,abigru,abicot,abisot,abitip,abigru1,abicon1,abisot1,abigru2,abicon2,abisot2,abigru3,abicon3,abisot3,abigru4,abicon4,abisot4,abigru5,abicon5,abisot5,abicau1,abicau2,abicau3,abicau4,abibiba,abibic,abicin,abibba,abiisocod,abigrb,abicob,abisob,abiantita,abiantestero,scrittdett,abivalport,abifidant,abicuc,abiest,abisepa,abidaa,abidad,abiswift,abinud) OUTPUT INSERTED.rv VALUES(@abisoc,@abiabi,@abicab,@abicon,@abigba,@abicba,@abisba,@abisia,@abigru,@abicot,@abisot,@abitip,@abigru1,@abicon1,@abisot1,@abigru2,@abicon2,@abisot2,@abigru3,@abicon3,@abisot3,@abigru4,@abicon4,@abisot4,@abigru5,@abicon5,@abisot5,@abicau1,@abicau2,@abicau3,@abicau4,@abibiba,@abibic,@abicin,@abibba,@abiisocod,@abigrb,@abicob,@abisob,@abiantita,@abiantestero,@scrittdett,@abivalport,@abifidant,@abicuc,@abiest,@abisepa,@abidaa,@abidad,@abiswift,@abinud)";
    public string UPDATE_QUERY => "UPDATE BANAZIEN SET abisoc = @abisoc,abiabi = @abiabi,abicab = @abicab,abicon = @abicon,abigba = @abigba,abicba = @abicba,abisba = @abisba,abisia = @abisia,abigru = @abigru,abicot = @abicot,abisot = @abisot,abitip = @abitip,abigru1 = @abigru1,abicon1 = @abicon1,abisot1 = @abisot1,abigru2 = @abigru2,abicon2 = @abicon2,abisot2 = @abisot2,abigru3 = @abigru3,abicon3 = @abicon3,abisot3 = @abisot3,abigru4 = @abigru4,abicon4 = @abicon4,abisot4 = @abisot4,abigru5 = @abigru5,abicon5 = @abicon5,abisot5 = @abisot5,abicau1 = @abicau1,abicau2 = @abicau2,abicau3 = @abicau3,abicau4 = @abicau4,abibiba = @abibiba,abibic = @abibic,abicin = @abicin,abibba = @abibba,abiisocod = @abiisocod,abigrb = @abigrb,abicob = @abicob,abisob = @abisob,abiantita = @abiantita,abiantestero = @abiantestero,scrittdett = @scrittdett,abivalport = @abivalport,abifidant = @abifidant,abicuc = @abicuc,abiest=@abiest,abisepa=@abisepa,abidaa=@abidaa,abidad=@abidad,abiswift=@abiswift,abinud=@abinud OUTPUT INSERTED.rv WHERE abisoc = @abisoc AND abiabi = @abiabi AND abicab = @abicab AND abicon = @abicon AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM BANAZIEN OUTPUT DELETED.rv WHERE abisoc = @abisoc AND abiabi = @abiabi AND abicab = @abicab AND abicon = @abicon AND rv = @rv";
    public bool Insert(BANAZIEN Model)
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

    public bool Update(BANAZIEN Model)
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

    public bool Delete(BANAZIEN Model)
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

    public string? Validate(BANAZIEN Model, bool IsInsert)
    {
        try
        {
            if ((IsInsert && !string.IsNullOrWhiteSpace(Model.abisoc) && Model.abiabi > 0 && Model.abicab > 0 && !string.IsNullOrWhiteSpace(Model.abicon) && !Exists(Model.abisoc, Model.abiabi, Model.abicab, Model.abicon)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.abisia) && Model.abisia.Trim().Length == 5)
                {
                    return null;
                }
                else
                { return "Il codice SIA è obbligatorio e deve essere lungo 5 caratteri"; }
            }
            else
            { return "ABI, CAB e conto sono già presenti in questa società o non sono validi"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}