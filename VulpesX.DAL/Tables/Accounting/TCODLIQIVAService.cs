namespace VulpesX.DAL.Tables.Accounting;

public interface ITCODLIQIVARepository
{

    ObservableCollection<TCODLIQIVA>? GetList(string CVISoc);

    TCODLIQIVA? Get(string CVISoc, string CVICod);

    bool Exists(string CVISoc, string CVICod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TCODLIQIVA Model);

    bool Update(TCODLIQIVA Model);

    bool Delete(TCODLIQIVA Model);

    string? Validate(TCODLIQIVA Model, bool IsInsert);
    #endregion
}

public class TCODLIQIVARepository : RepositoryBase, ITCODLIQIVARepository
{
    public TCODLIQIVARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TCODLIQIVA>? GetList(string CVISoc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCODLIQIVA>(
                @"SELECT * FROM TCODLIQIVA
                        WHERE CVISoc = @cvisoc
                        ORDER BY CVISeq", new { cvisoc = CVISoc });

            return new ObservableCollection<TCODLIQIVA>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TCODLIQIVA? Get(string CVISoc, string CVICod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TCODLIQIVA>(
                "SELECT * FROM TCODLIQIVA WHERE CVISoc = @CVISoc AND CVICod = @CVICod",
                new { CVISoc = CVISoc, CVICod = CVICod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CVISoc, string CVICod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TCODLIQIVA WHERE CVISoc = @CVISoc AND CVICod = @CVICod",
                new { CVISoc = CVISoc, CVICod = CVICod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TCODLIQIVA (CVISoc,CVICod,CVIDes,CVITipo,CVISeq,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@CVISoc,@CVICod,@CVIDes,@CVITipo,@CVISeq,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE TCODLIQIVA SET CVISoc = @CVISoc,CVICod = @CVICod,CVIDes = @CVIDes,CVITipo = @CVITipo,CVISeq = @CVISeq,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE CVISoc = @CVISoc AND CVICod = @CVICod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TCODLIQIVA OUTPUT DELETED.rv WHERE CVISoc = @CVISoc AND CVICod = @CVICod AND rv = @rv";
    public bool Insert(TCODLIQIVA Model)
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

    public bool Update(TCODLIQIVA Model)
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

    public bool Delete(TCODLIQIVA Model)
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

    public string? Validate(TCODLIQIVA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.CVISoc) && !string.IsNullOrWhiteSpace(Model.CVICod) && IsInsert && !Exists(Model.CVISoc, Model.CVICod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.CVIDes))
                {
                    return null;
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

public class TCODLIQIVAUfpRepository : RepositoryBase, ITCODLIQIVARepository
{
    public TCODLIQIVAUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TCODLIQIVA>? GetList(string CVISoc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCODLIQIVA>(
                @"SELECT * FROM TCODLIQIVA
                        WHERE CVISoc = @cvisoc
                        ORDER BY CVISeq", new { cvisoc = CVISoc });

            return new ObservableCollection<TCODLIQIVA>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TCODLIQIVA? Get(string CVISoc, string CVICod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TCODLIQIVA>(
                "SELECT * FROM TCODLIQIVA WHERE CVISoc = @CVISoc AND CVICod = @CVICod",
                new { CVISoc = CVISoc, CVICod = CVICod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CVISoc, string CVICod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TCODLIQIVA WHERE CVISoc = @CVISoc AND CVICod = @CVICod",
                new { CVISoc = CVISoc, CVICod = CVICod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TCODLIQIVA (CVISoc,CVICod,CVIDes,CVITipo,CVISeq,CVIAttiva, CVITot1, CVITot2) OUTPUT INSERTED.rv VALUES(@CVISoc,@CVICod,@CVIDes,@CVITipo,@CVISeq,@CVIAttiva, @CVITot1, @CVITot2)";
    public string UPDATE_QUERY => "UPDATE TCODLIQIVA SET CVISoc = @CVISoc,CVICod = @CVICod,CVIDes = @CVIDes,CVITipo = @CVITipo,CVISeq = @CVISeq, CVIAttiva = @CVIAttiva, CVITot1 = @CVITot1, CVITot2 = @CVITot2 OUTPUT INSERTED.rv WHERE CVISoc = @CVISoc AND CVICod = @CVICod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TCODLIQIVA OUTPUT DELETED.rv WHERE CVISoc = @CVISoc AND CVICod = @CVICod AND rv = @rv";
    public bool Insert(TCODLIQIVA Model)
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

    public bool Update(TCODLIQIVA Model)
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

    public bool Delete(TCODLIQIVA Model)
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

    public string? Validate(TCODLIQIVA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.CVISoc) && !string.IsNullOrWhiteSpace(Model.CVICod) && IsInsert && !Exists(Model.CVISoc, Model.CVICod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.CVIDes))
                {
                    return null;
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