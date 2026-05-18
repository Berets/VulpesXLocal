using VulpesX.DAL;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.General;


public interface IDESFORRepository
{
    ObservableCollection<DESFOR>? GetList(int EntityID);

    ObservableCollection<GenericIntIDDescription>? GetSimpleIntList(int EntityID, bool AddAllItem);

    DESFOR? Get(int EntityID, int Sequence);

    bool Exists(int EntityID, int Sequence);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(DESFOR Model);

    bool Update(DESFOR Model);

    bool Delete(DESFOR Model);

    string? Validate(DESFOR Model, bool IsInsert);
    #endregion
}

public class DESFORRepository : RepositoryBase, IDESFORRepository
{
    public DESFORRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<DESFOR>? GetList(int EntityID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DESFOR>(
                @"SELECT * FROM DESFOR 
                        WHERE fornicod = @fornicod
                        ORDER BY fodesti",
                new { fornicod = EntityID });

            return new ObservableCollection<DESFOR>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIntIDDescription>? GetSimpleIntList(int EntityID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIntIDDescription>(
                @"SELECT d.fodesti AS ID, d.foragso AS Description FROM DESFOR AS d
                        WHERE d.fornicod = @fornicod
                        ORDER BY d.fodesti",
                new { fornicod = EntityID }).ToList();

            if (AddAllItem)
            {
                list.Add(new GenericIntIDDescription()
                {
                    ID = null,
                    Description = "Tutti i destinatari"
                });
            }

            return new ObservableCollection<GenericIntIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DESFOR? Get(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<DESFOR>(
                "SELECT * FROM DESFOR WHERE fornicod = @fornicod AND fodesti = @fodesti",
                new { fornicod = EntityID, fodesti = Sequence })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM DESFOR WHERE fornicod = @fornicod AND fodesti = @fodesti ",
                new { fornicod = EntityID, fodesti = Sequence }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO DESFOR (fornicod,fodesti,foragso,fodein,fodecap,fodeloc,fodepro,foperco) OUTPUT INSERTED.rv VALUES(@fornicod,@fodesti,@foragso,@fodein,@fodecap,@fodeloc,@fodepro,@foperco)";
    public string UPDATE_QUERY => "UPDATE DESFOR SET fornicod = @fornicod,fodesti = @fodesti,foragso = @foragso,fodein = @fodein,fodecap = @fodecap,fodeloc = @fodeloc,fodepro = @fodepro,foperco = @foperco OUTPUT INSERTED.rv WHERE fornicod = @fornicod AND fodesti = @fodesti AND rv => @rv";
    public string DELETE_QUERY => "DELETE FROM DESFOR OUTPUT DELETED.rv WHERE fornicod = @fornicod AND fodesti = @fodesti AND rv = @rv";
    public bool Insert(DESFOR Model)
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

    public bool Update(DESFOR Model)
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

    public bool Delete(DESFOR Model)
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

    public string? Validate(DESFOR Model, bool IsInsert)
    {
        try
        {
            if (Model.fornicod > 0)
            {
                if (Model.fodesti > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.foragso))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.fodein))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.fodeloc))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.fodepro))
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.foperco))
                                    {
                                        return null;
                                    }
                                    else
                                    { return "Il riferimento è obbligatorio"; }
                                }
                                else
                                { return "La provincia è obbligatoria"; }
                            }
                            else
                            { return "Il comune è obbligatorio"; }
                        }
                        else
                        { return "L'indirizzo è obbligatorio"; }
                    }
                    else
                    { return "La ragione sociale è obbligatoria"; }
                }
                else
                { return "Il progressivo è obbligatorio"; }
            }
            else
            { return "Il fornitore di riferimento è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class DESFORUfpRepository : RepositoryBase, IDESFORRepository
{
    public DESFORUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }


    public ObservableCollection<DESFOR>? GetList(int EntityID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<DESFOR>(
                @"SELECT *, fodepro1 as fodepro FROM DESFOR 
                        WHERE fornicod = @fornicod
                        ORDER BY fodesti",
                new { fornicod = EntityID });

            return new ObservableCollection<DESFOR>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIntIDDescription>? GetSimpleIntList(int EntityID, bool AddAllItem)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIntIDDescription>(
                @"SELECT d.fodesti AS ID, d.foragso AS Description FROM DESFOR AS d
                        WHERE d.fornicod = @fornicod
                        ORDER BY d.fodesti",
                new { fornicod = EntityID }).ToList();

            if (AddAllItem)
            {
                list.Add(new GenericIntIDDescription()
                {
                    ID = null,
                    Description = "Tutti i destinatari"
                });
            }

            return new ObservableCollection<GenericIntIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public DESFOR? Get(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<DESFOR>(
                "SELECT *, fodepro1 as fodepro FROM DESFOR WHERE fornicod = @fornicod AND fodesti = @fodesti",
                new { fornicod = EntityID, fodesti = Sequence })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(int EntityID, int Sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM DESFOR WHERE fornicod = @fornicod AND fodesti = @fodesti ",
                new { fornicod = EntityID, fodesti = Sequence }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO DESFOR (fornicod,fodesti,foragso,fodein,fodecap,fodeloc,fodepro1,foperco) OUTPUT INSERTED.rv VALUES(@fornicod,@fodesti,@foragso,@fodein,@fodecap,@fodeloc,@fodepro,@foperco)";
    public string UPDATE_QUERY => "UPDATE DESFOR SET fornicod = @fornicod,fodesti = @fodesti,foragso = @foragso,fodein = @fodein,fodecap = @fodecap,fodeloc = @fodeloc,fodepro1 = @fodepro,foperco = @foperco OUTPUT INSERTED.rv WHERE fornicod = @fornicod AND fodesti = @fodesti AND rv => @rv";
    public string DELETE_QUERY => "DELETE FROM DESFOR OUTPUT DELETED.rv WHERE fornicod = @fornicod AND fodesti = @fodesti AND rv = @rv";
    public bool Insert(DESFOR Model)
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

    public bool Update(DESFOR Model)
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

    public bool Delete(DESFOR Model)
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

    public string? Validate(DESFOR Model, bool IsInsert)
    {
        try
        {
            if (Model.fornicod > 0)
            {
                if (Model.fodesti > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.foragso))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.fodein))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.fodeloc))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.fodepro))
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.foperco))
                                    {
                                        return null;
                                    }
                                    else
                                    { return "Il riferimento è obbligatorio"; }
                                }
                                else
                                { return "La provincia è obbligatoria"; }
                            }
                            else
                            { return "Il comune è obbligatorio"; }
                        }
                        else
                        { return "L'indirizzo è obbligatorio"; }
                    }
                    else
                    { return "La ragione sociale è obbligatoria"; }
                }
                else
                { return "Il progressivo è obbligatorio"; }
            }
            else
            { return "Il fornitore di riferimento è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}