using VulpesX.DAL;

namespace VulpesX.Services.Tables.General;

public interface ITAB_GEN_TEXTSRepository
{

    ObservableCollection<TAB_GEN_TEXTS>? GetList(string CompanyID, string TextType);

    TAB_GEN_TEXTS? Get(string TTxsoc, string TTxcod, string TTXtip);

    bool Exists(string TTxsoc, string TTxcod, string TTXtip);

    #region CRUD
    bool Insert(TAB_GEN_TEXTS Model);

    bool Update(TAB_GEN_TEXTS Model);

    bool Delete(TAB_GEN_TEXTS Model);

    string? Validate(TAB_GEN_TEXTS Model, bool IsInsert);
    #endregion
}

public class TAB_GEN_TEXTSRepository : RepositoryBase, ITAB_GEN_TEXTSRepository
{
    public TAB_GEN_TEXTSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TAB_GEN_TEXTS>? GetList(string CompanyID, string TextType)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_GEN_TEXTS>(
                @"SELECT * FROM TAB_GEN_TEXTS
                        WHERE TTxsoc = @ttxsoc AND TTxtip = @ttxtip", new { ttxsoc = CompanyID, ttxtip = TextType });

            return new ObservableCollection<TAB_GEN_TEXTS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_GEN_TEXTS? Get(string TTxsoc, string TTxcod, string TTXtip)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<TAB_GEN_TEXTS>(
                "SELECT * FROM TAB_GEN_TEXTS WHERE TTxsoc = @TTxsoc AND TTxcod = @TTxcod AND TTXtip = @TTXtip",
                new { TTxsoc = TTxsoc, TTxcod = TTxcod, TTXtip = TTXtip })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string TTxsoc, string TTxcod, string TTXtip)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_GEN_TEXTS WHERE TTxsoc = @TTxsoc AND TTxcod = @TTxcod AND TTXtip = @TTXtip",
                new { TTxsoc = TTxsoc, TTxcod = TTxcod, TTXtip = TTXtip }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(TAB_GEN_TEXTS Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO TAB_GEN_TEXTS (TTxsoc,TTxcod,TTXtip,TTxdes,TTXNote,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@TTxsoc,@TTxcod,@TTXtip,@TTxdes,@TTXNote,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote)",
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

    public bool Update(TAB_GEN_TEXTS Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE TAB_GEN_TEXTS SET TTxsoc = @TTxsoc,TTxcod = @TTxcod,TTXtip = @TTXtip,TTxdes = @TTxdes,TTXNote = @TTXNote,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE TTxsoc = @TTxsoc AND TTxcod = @TTxcod AND TTXtip = @TTXtip AND rv = @rv",
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

    public bool Delete(TAB_GEN_TEXTS Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM TAB_GEN_TEXTS OUTPUT DELETED.rv WHERE TTxsoc = @TTxsoc AND TTxcod = @TTxcod AND TTXtip = @TTXtip AND rv = @rv",
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

    public string? Validate(TAB_GEN_TEXTS Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.TTxcod) && IsInsert && !Exists(Model.TTxsoc, Model.TTxcod, Model.TTXtip)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.TTxdes))
                {
                    if (!string.IsNullOrWhiteSpace(Model.TTXNote))
                    {
                        return null;
                    }
                    else
                    { return "Il testo è obbligatorio"; }
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