using System.Data;
using VulpesX.DAL.General;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting;

public interface INUMREGRepository
{
    ObservableCollection<NUMREG>? GetList(string CompanyID);

    ObservableCollection<NUMREG>? GetDistinctCodeList(string CompanyID);

    NUMREG? Get(string CompanyID, int Year, string Code);

    int GetNumber(string CompanyID, int Year, GenericIDDescription Code, bool UpdateNumerator, int? InitializeValue = null);

    bool Exists(string PERSOC, int PERANN, string PERCOD);

    #region CRUD
    bool Insert(NUMREG Model);

    bool Update(NUMREG Model);

    bool Delete(NUMREG Model);

    string? Validate(NUMREG Model, bool IsInsert);

    long GetFullLongID(int Year, int Number);

    int GetFullIntID(int Year, int Number);
    #endregion

}

public class NUMREGRepository : RepositoryBase, INUMREGRepository
{
    public NUMREGRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NUMREG>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<NUMREG>(
                "SELECT * FROM NUMREG WHERE PERSOC = @persoc ORDER BY PERANN DESC, PERCOD",
                new { persoc = CompanyID });

            return new ObservableCollection<NUMREG>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<NUMREG>? GetDistinctCodeList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<NUMREG>(
                $@"SELECT DISTINCT PERCOD FROM NUMREG
                        WHERE PERSOC = @persoc AND PERCOD NOT IN ({string.Join(',', Constants.ReservedIDsTSQL)})
                        ORDER BY PERCOD",
                new { persoc = CompanyID }).ToList();

            foreach (var row in list)
            {
                row.PERDE1 = connection.ExecuteScalar(
                    @"SELECT TOP(1) PERDE1 FROM NUMREG 
                            WHERE PERSOC=@persoc AND PERCOD = @percod
                            ORDER BY PERANN DESC", new { persoc = CompanyID, percod = row.PERCOD })?.ToString();
            }

            return new ObservableCollection<NUMREG>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NUMREG? Get(string CompanyID, int Year, string Code)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NUMREG>(
                "SELECT * FROM NUMREG WHERE PERSOC = @cid AND PERANN = @yea AND PERCOD = @code",
                new { cid = CompanyID, yea = Year, code = Code })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public int GetNumber(string CompanyID, int Year, GenericIDDescription Code, bool UpdateNumerator, int? InitializeValue = null)
    {
        try
        {
            using var connection = GetOpenConnection();

       

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var item = connection.Query<NUMREG>(
                        "SELECT * FROM NUMREG WHERE PERSOC = @cid AND PERANN = @yea AND PERCOD = @Code",
                        new { cid = CompanyID, yea = Year, code = Code.ID }, transaction).FirstOrDefault();

                        if (item != null)
                        {
                            if (UpdateNumerator)
                            {
                                if (!item.PERNUM.HasValue)
                                    item.PERNUM = InitializeValue.HasValue ? InitializeValue.Value : 1;
                                else
                                    item.PERNUM += 1;
                                connection.ExecuteScalar(UPDATE_QUERY, item, transaction);
                                transaction.Commit();
                            }
                            return item.PERNUM!.Value;
                        }
                        else
                        {
                            var newItem = new NUMREG()
                            {
                                PERSOC = CompanyID,
                                PERANN = Year,
                                PERCOD = Code.ID!,
                                PERNUM = InitializeValue.HasValue ? InitializeValue.Value : 1,
                                PERDE1 = $"Creazione automatica - {Code.Description}"
                            };
                            connection.Execute(INSERT_QUERY, newItem, transaction);
                            transaction.Commit();
                            return newItem.PERNUM.Value;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return -1;
                    }
                }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public bool Exists(string PERSOC, int PERANN, string PERCOD)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM NUMREG
                        WHERE PERSOC = @cid AND PERANN = @yea AND PERCOD = @id",
                new { cid = PERSOC, yea = PERANN, id = PERCOD }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public static readonly string INSERT_QUERY = "INSERT INTO NUMREG (PERSOC,PERCOD,PERANN,PERNUM,PERDE1,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@PERSOC,@PERCOD,@PERANN,@PERNUM,@PERDE1,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public static readonly string UPDATE_QUERY = "UPDATE NUMREG SET PERSOC = @PERSOC,PERCOD = @PERCOD,PERANN = @PERANN,PERNUM = @PERNUM,PERDE1 = @PERDE1,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE PERSOC = @PERSOC AND PERCOD = @PERCOD AND PERANN = @PERANN AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM NUMREG OUTPUT DELETED.rv WHERE PERSOC = @PERSOC AND PERCOD = @PERCOD AND PERANN = @PERANN AND rv = @rv";

    public bool Insert(NUMREG Model)
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

    public bool Update(NUMREG Model)
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

    public bool Delete(NUMREG Model)
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

    public string? Validate(NUMREG Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.PERCOD) && IsInsert && !Exists(Model.PERSOC, Model.PERANN, Model.PERCOD)) || !IsInsert)
            {
                if (!Constants.ReservedIDs.Contains(Model.PERCOD))
                {
                    if (!string.IsNullOrWhiteSpace(Model.PERDE1))
                    {
                        if (Model.PERANN > 0)
                        {
                            return null;
                        }
                        else
                        { return "L'anno è obbligatorio"; }
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice scelto e' riservato e non puo' essere utilizzato"; }
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

    #region Static utils

    /// <summary>
    /// SYNC
    /// </summary>
    /// In caso di modifica sincronizzare la stessa procedura entityframework sul MES e/o altri tools
    public long GetFullLongID(int Year, int Number)
    {
        return long.Parse(Year.ToString() + Number.ToString().PadLeft(7, '0'));
    }

    public int GetFullIntID(int Year, int Number)
    {
        return int.Parse(Year.ToString() + Number.ToString().PadLeft(4, '0'));
    }
    #endregion
}

public class NUMREGUfpRepository : RepositoryBase, INUMREGRepository
{
    public NUMREGUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<NUMREG>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<NUMREG>(
                "SELECT * FROM TAB_NUMERATORI WHERE PERSOC = @persoc ORDER BY PERANN DESC, PERCOD",
                new { persoc = CompanyID });

            return new ObservableCollection<NUMREG>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<NUMREG>? GetDistinctCodeList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<NUMREG>(
                $@"SELECT DISTINCT PERCOD FROM TAB_NUMERATORI
                        WHERE PERSOC = @persoc AND PERCOD NOT IN ({string.Join(',', Constants.ReservedIDsTSQL)})
                        ORDER BY PERCOD",
                new { persoc = CompanyID }).ToList();

            foreach (var row in list)
            {
                row.PERDE1 = connection.ExecuteScalar(
                    @"SELECT TOP(1) PERDE1 FROM TAB_NUMERATORI 
                            WHERE PERSOC=@persoc AND PERCOD = @percod
                            ORDER BY PERANN DESC", new { persoc = CompanyID, percod = row.PERCOD })?.ToString();
            }

            return new ObservableCollection<NUMREG>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public NUMREG? Get(string CompanyID, int Year, string Code)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<NUMREG>(
                "SELECT * FROM TAB_NUMERATORI WHERE PERSOC = @cid AND PERANN = @yea AND PERCOD = @code",
                new { cid = CompanyID, yea = Year, code = Code })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public int GetNumber(string CompanyID, int Year, GenericIDDescription Code, bool UpdateNumerator, int? InitializeValue = null)
    {
        try
        {
            using var connection = GetOpenConnection();

     

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var item = connection.Query<NUMREG>(
                        "SELECT * FROM TAB_NUMERATORI WHERE PERSOC = @cid AND PERANN = @yea AND PERCOD = @Code",
                        new { cid = CompanyID, yea = Year, code = Code.ID }, transaction).FirstOrDefault();

                        if (item != null)
                        {
                            if (UpdateNumerator)
                            {
                                if (!item.PERNUM.HasValue)
                                    item.PERNUM = InitializeValue.HasValue ? InitializeValue.Value : 1;
                                else
                                    item.PERNUM += 1;
                                connection.ExecuteScalar(UPDATE_QUERY, item, transaction);
                                transaction.Commit();
                            }
                            return item.PERNUM!.Value;
                        }
                        else
                        {
                            var newItem = new NUMREG()
                            {
                                PERSOC = CompanyID,
                                PERANN = Year,
                                PERCOD = Code.ID!,
                                PERNUM = InitializeValue.HasValue ? InitializeValue.Value : 1,
                                PERDE1 = $"Automatica - {Code.Description}"
                            };
                            connection.Execute(INSERT_QUERY, newItem, transaction);
                            transaction.Commit();
                            return newItem.PERNUM.Value;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return -1;
                    }
                }
           
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public bool Exists(string PERSOC, int PERANN, string PERCOD)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM TAB_NUMERATORI
                        WHERE PERSOC = @cid AND PERANN = @yea AND PERCOD = @id",
                new { cid = PERSOC, yea = PERANN, id = PERCOD }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public static readonly string INSERT_QUERY = "INSERT INTO TAB_NUMERATORI (PERSOC,PERCOD,PERANN,PERNUM,PERDE1) OUTPUT INSERTED.rv VALUES(@PERSOC,@PERCOD,@PERANN,@PERNUM,@PERDE1)";
    public static readonly string UPDATE_QUERY = "UPDATE TAB_NUMERATORI SET PERSOC = @PERSOC,PERCOD = @PERCOD,PERANN = @PERANN,PERNUM = @PERNUM,PERDE1 = @PERDE1 OUTPUT INSERTED.rv WHERE PERSOC = @PERSOC AND PERCOD = @PERCOD AND PERANN = @PERANN AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM NUMREG OUTPUT DELETED.rv WHERE PERSOC = @PERSOC AND PERCOD = @PERCOD AND PERANN = @PERANN AND rv = @rv";

    public bool Insert(NUMREG Model)
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

    public bool Update(NUMREG Model)
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

    public bool Delete(NUMREG Model)
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

    public string? Validate(NUMREG Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.PERCOD) && IsInsert && !Exists(Model.PERSOC, Model.PERANN, Model.PERCOD)) || !IsInsert)
            {
                if (!Constants.ReservedIDs.Contains(Model.PERCOD))
                {
                    if (!string.IsNullOrWhiteSpace(Model.PERDE1))
                    {
                        if (Model.PERANN > 0)
                        {
                            return null;
                        }
                        else
                        { return "L'anno è obbligatorio"; }
                    }
                    else
                    { return "La descrizione è obbligatoria"; }
                }
                else
                { return "Il codice scelto e' riservato e non puo' essere utilizzato"; }
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

    #region Static utils

    /// <summary>
    /// SYNC
    /// </summary>
    /// In caso di modifica sincronizzare la stessa procedura entityframework sul MES e/o altri tools
    public long GetFullLongID(int Year, int Number)
    {
        return long.Parse(Year.ToString() + Number.ToString().PadLeft(7, '0'));
    }

    public int GetFullIntID(int Year, int Number)
    {
        return int.Parse(Year.ToString() + Number.ToString().PadLeft(4, '0'));
    }
    #endregion
}