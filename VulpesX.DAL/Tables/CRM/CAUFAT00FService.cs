using VulpesX.DAL.CRM;

namespace VulpesX.DAL.Tables.CRM;


public interface ICAUFAT00FRepository
{
    ObservableCollection<CAUFAT00F>? GetList();

    CAUFAT00F? Get(string fatcod);

    bool Exists(string fatcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(CAUFAT00F Model);

    bool Update(CAUFAT00F Model);

    bool Delete(CAUFAT00F Model);

    string? Validate(CAUFAT00F Model, bool IsInsert);
    #endregion
}

public class CAUFAT00FRepository : RepositoryBase, ICAUFAT00FRepository
{
    public CAUFAT00FRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<CAUFAT00F>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CAUFAT00F>(
                "SELECT * FROM CAUFAT00F");

            return new ObservableCollection<CAUFAT00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CAUFAT00F? Get(string fatcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<CAUFAT00F>(
                "SELECT * FROM CAUFAT00F WHERE fatcod = @fatcod",
                new { fatcod = fatcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string fatcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM CAUFAT00F WHERE fatcod = @fatcod",
                new { fatcod = fatcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO CAUFAT00F (fatcod,fatdes,flgcon,fatcon,fataut,fattif,fatnmr,fattido,fatpre,fatcaut) OUTPUT INSERTED.rv VALUES(@fatcod,@fatdes,@flgcon,@fatcon,@fataut,@fattif,@fatnmr,@fattido,@fatpre,@fatcaut)";
    public string UPDATE_QUERY => "UPDATE CAUFAT00F SET fatcod = @fatcod,fatdes = @fatdes,flgcon = @flgcon,fatcon = @fatcon,fataut = @fataut,fattif = @fattif,fatnmr = @fatnmr,fattido = @fattido,fatpre = @fatpre,fatcaut = @fatcaut OUTPUT INSERTED.rv WHERE fatcod = @fatcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM CAUFAT00F OUTPUT DELETED.rv WHERE fatcod = @fatcod AND rv = @rv";
    public bool Insert(CAUFAT00F Model)
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

    public bool Update(CAUFAT00F Model)
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

    public bool Delete(CAUFAT00F Model)
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

    public string? Validate(CAUFAT00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.fatcod) && IsInsert && !Exists(Model.fatcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.fatdes))
                {
                    if (Model.fatautBool ||
                        (!Model.fatautBool && string.IsNullOrWhiteSpace(Model.fatcaut)))
                    {
                        return null;
                    }
                    else
                    { return "Se non è selezionato il flag di autofattura non deve essere specificata la causale contabile giroconto"; }
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