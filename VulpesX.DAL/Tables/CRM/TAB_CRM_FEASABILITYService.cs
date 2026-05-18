using VulpesX.DAL;

namespace VulpesX.DAL.Tables.CRM;

public interface ITAB_CRM_FEASABILITYRepository
{
    ObservableCollection<TAB_CRM_FEASABILITY>? GetList();

    TAB_CRM_FEASABILITY? Get(string tdecod);

    bool Exists(string tdecod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TAB_CRM_FEASABILITY Model);

    bool Update(TAB_CRM_FEASABILITY Model);

    bool Delete(TAB_CRM_FEASABILITY Model);

    string? Validate(TAB_CRM_FEASABILITY Model, bool IsInsert);
    #endregion
}

public class TAB_CRM_FEASABILITYRepository : RepositoryBase, ITAB_CRM_FEASABILITYRepository
{
    public TAB_CRM_FEASABILITYRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<TAB_CRM_FEASABILITY>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TAB_CRM_FEASABILITY>(
                "SELECT * FROM TAB_CRM_FEASABILITY");

            return new ObservableCollection<TAB_CRM_FEASABILITY>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TAB_CRM_FEASABILITY? Get(string tdecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TAB_CRM_FEASABILITY>(
                "SELECT * FROM TAB_CRM_FEASABILITY WHERE tdecod = @tdecod",
                new { tdecod = tdecod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string tdecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_CRM_FEASABILITY WHERE tdecod = @tdecod",
                new { tdecod = tdecod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_CRM_FEASABILITY (tdecod,tdedes) OUTPUT INSERTED.rv VALUES(@tdecod,@tdedes)";
    public string UPDATE_QUERY => "UPDATE TAB_CRM_FEASABILITY SET tdecod = @tdecod,tdedes = @tdedes OUTPUT INSERTED.rv WHERE tdecod = @tdecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_CRM_FEASABILITY OUTPUT DELETED.rv WHERE tdecod = @tdecod AND rv = @rv";
    public bool Insert(TAB_CRM_FEASABILITY Model)
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

    public bool Update(TAB_CRM_FEASABILITY Model)
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

    public bool Delete(TAB_CRM_FEASABILITY Model)
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

    public string? Validate(TAB_CRM_FEASABILITY Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.tdecod) && IsInsert && !Exists(Model.tdecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.tdedes))
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