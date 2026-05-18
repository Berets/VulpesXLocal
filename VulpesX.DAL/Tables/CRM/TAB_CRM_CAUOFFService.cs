namespace VulpesX.DAL.Tables.CRM;

public interface ITAB_CRM_CAUOFFRepository
{
    ObservableCollection<TAB_CRM_CAUOFF>? GetList(string offsoc);

    ObservableCollection<TAB_CRM_CAUOFF>? GetListFull(string offsoc);

    TAB_CRM_CAUOFF? Get(string offsoc, string offcod);

    TAB_CRM_CAUOFF? GetFull(string offsoc, string offcod);

    bool Exists(string offsoc, string offcod);

    #region CRUD
    bool Insert(TAB_CRM_CAUOFF Model);

    bool Update(TAB_CRM_CAUOFF Model);

    bool Delete(TAB_CRM_CAUOFF Model);

    string? Validate(TAB_CRM_CAUOFF Model, bool IsInsert);
    #endregion
}

public class TAB_CRM_CAUOFFRepository : RepositoryBase, ITAB_CRM_CAUOFFRepository
{
    public TAB_CRM_CAUOFFRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<TAB_CRM_CAUOFF>? GetList(string offsoc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_CRM_CAUOFF>(
                @"SELECT * FROM TAB_CRM_CAUOFF
                        WHERE offsoc = @offsoc AND canceled IS NULL", new { offsoc = offsoc });

            return new ObservableCollection<TAB_CRM_CAUOFF>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TAB_CRM_CAUOFF>? GetListFull(string offsoc)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_CRM_CAUOFF, TAB_CRM_CAUORD, TAB_CRM_CAUOFF>(
                @"SELECT c.*, co.* FROM TAB_CRM_CAUOFF AS c
                        LEFT JOIN TAB_CRM_CAUORD AS co ON co.cauacqsoc = c.offsoc AND co.cauacq = c.offord
                        WHERE c.offsoc = @offsoc AND c.canceled IS NULL",
                (cau, cauor) => { cau.OrderCausal = cauor; return cau; },
                new { offsoc = offsoc }, splitOn: "cauacq");

            return new ObservableCollection<TAB_CRM_CAUOFF>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUOFF? Get(string offsoc, string offcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_CAUOFF>(
                "SELECT * FROM TAB_CRM_CAUOFF WHERE offsoc = @offsoc AND offcod = @offcod",
                new { offsoc = offsoc, offcod = offcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_CAUOFF? GetFull(string offsoc, string offcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_CAUOFF, TAB_GEN_TEXTS, TAB_CRM_CAUOFF>(
                @"SELECT c.*, t.* FROM TAB_CRM_CAUOFF AS c
                        LEFT OUTER JOIN TAB_GEN_TEXTS AS t ON t.TTxsoc = c.offsoc AND t.TTxcod = c.offte1 AND t.TTXtip = c.offtxt
                        WHERE c.offsoc = @offsoc AND c.offcod = @offcod",
                (cau, tex) => { cau.Text = tex; return cau; },
                new { offsoc = offsoc, offcod = offcod }, splitOn: "TTxsoc")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string offsoc, string offcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_CRM_CAUOFF WHERE offsoc = @offsoc AND offcod = @offcod",
                new { offsoc = offsoc, offcod = offcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(TAB_CRM_CAUOFF Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO TAB_CRM_CAUOFF (offcod,offede,offord,offtxt,offte1,offsca,offsoc,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@offcod,@offede,@offord,@offtxt,@offte1,@offsca,@offsoc,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)",
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

    public bool Update(TAB_CRM_CAUOFF Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE TAB_CRM_CAUOFF SET offcod = @offcod,offede = @offede,offord = @offord,offtxt = @offtxt,offte1 = @offte1,offsca = @offsca,offsoc = @offsoc,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE offsoc = @offsoc AND offcod = @offcod AND rv = @rv",
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

    public bool Delete(TAB_CRM_CAUOFF Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "DELETE FROM TAB_CRM_CAUOFF OUTPUT DELETED.rv WHERE offsoc = @offsoc AND offcod = @offcod AND rv = @rv",
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

    public string? Validate(TAB_CRM_CAUOFF Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.offcod) && IsInsert && !Exists(Model.offsoc, Model.offcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.offede))
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