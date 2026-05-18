using VulpesX.DAL;

namespace VulpesX.Services.Tables.Accounting;

public interface ITCECO00FRepository
{

    ObservableCollection<TCECO00F>? GetList(string CompanyID, bool AddAllItem);

    TCECO00F? Get(string ID);

    bool Exists(string cecodc);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TCECO00F Model);

    bool Update(TCECO00F Model);

    string? CanDelete(string ID);

    bool Delete(TCECO00F Model);

    string? Validate(TCECO00F Model, bool IsInsert);
    #endregion
}

public class TCECO00FRepository : RepositoryBase, ITCECO00FRepository
{
    public TCECO00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TCECO00F>? GetList(string CompanyID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCECO00F>(
                @"SELECT * FROM TCECO00F
                        WHERE cesoci = @cid OR cesoci IS NULL",
                new { cid = CompanyID }).ToList();

            if (AddAllItem)
            {
                list.Add(new TCECO00F()
                {
                    cecodc = string.Empty,
                    cedesc = "Tutti"
                });
            }
            return new ObservableCollection<TCECO00F>(list.OrderBy(o => o.cecodc));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TCECO00F? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TCECO00F>(
                "SELECT * FROM TCECO00F WHERE cecodc = @id",
                new { id = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cecodc)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TCECO00F WHERE cecodc = @cecodc",
                new { cecodc = cecodc }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TCECO00F (cecodc,cedesc,cetipo,ceorla,cesoci) OUTPUT INSERTED.rv VALUES(@cecodc,@cedesc,@cetipo,@ceorla,@cesoci)";
    public string UPDATE_QUERY => "UPDATE TCECO00F SET cecodc = @cecodc,cedesc = @cedesc,cetipo = @cetipo,ceorla = @ceorla,cesoci = @cesoci OUTPUT INSERTED.rv WHERE cecodc = @cecodc AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TCECO00F OUTPUT DELETED.rv WHERE cecodc = @cecodc AND rv = @rv";
    public bool Insert(TCECO00F Model)
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

    public bool Update(TCECO00F Model)
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

    public string? CanDelete(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var pnrighe = (int?)connection.ExecuteScalar("SELECT COUNT(*) FROM PNRIGHE WHERE N1CCCC = @id", new { id = ID });
            if (pnrighe == 0)
                return null;
            else
                return $"Impossibile eliminare il centro di costo {ID} perchè in uso in prima nota";

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(TCECO00F Model)
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

    public string? Validate(TCECO00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cecodc) && IsInsert && !Exists(Model.cecodc)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.cedesc))
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

public class TCECO00FUfpRepository : RepositoryBase, ITCECO00FRepository
{
    public TCECO00FUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TCECO00F>? GetList(string CompanyID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCECO00F>(
                @"SELECT * FROM TCECO00F").ToList();

            if (AddAllItem)
            {
                list.Add(new TCECO00F()
                {
                    cecodc = string.Empty,
                    cedesc = "Tutti"
                });
            }
            return new ObservableCollection<TCECO00F>(list.OrderBy(o => o.cecodc));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TCECO00F? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TCECO00F>(
                "SELECT * FROM TCECO00F WHERE cecodc = @id",
                new { id = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string cecodc)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TCECO00F WHERE cecodc = @cecodc",
                new { cecodc = cecodc }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TCECO00F (cecodc,cedesc,cetipo) OUTPUT INSERTED.rv VALUES(@cecodc,@cedesc,@cetipo,@ceorla,@cesoci)";
    public string UPDATE_QUERY => "UPDATE TCECO00F SET cecodc = @cecodc,cedesc = @cedesc,cetipo = @cetipo OUTPUT INSERTED.rv WHERE cecodc = @cecodc AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TCECO00F OUTPUT DELETED.rv WHERE cecodc = @cecodc AND rv = @rv";
    public bool Insert(TCECO00F Model)
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

    public bool Update(TCECO00F Model)
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

    public string? CanDelete(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var pnrighe = (int?)connection.ExecuteScalar("SELECT COUNT(*) FROM PNRIGHE WHERE N1CCCC = @id", new { id = ID });
            if (pnrighe == 0)
                return null;
            else
                return $"Impossibile eliminare il centro di costo {ID} perchè in uso in prima nota";

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool Delete(TCECO00F Model)
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

    public string? Validate(TCECO00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.cecodc) && IsInsert && !Exists(Model.cecodc)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.cedesc))
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