using VulpesX.DAL;

namespace VulpesX.Services.Tables.Accounting;

public interface ILIBRIIVARepository
{
    ObservableCollection<LIBRIIVA>? GetList();

    LIBRIIVA? Get(string livcod);

    LIBRIIVA? GetDefaultIVARecap();

    bool Exists(string livcod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(LIBRIIVA Model);

    bool Update(LIBRIIVA Model);

    bool Delete(LIBRIIVA Model);

    string? Validate(LIBRIIVA Model, bool IsInsert);
    #endregion
}

public class LIBRIIVARepository : RepositoryBase, ILIBRIIVARepository
{
    public LIBRIIVARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<LIBRIIVA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<LIBRIIVA, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCGRUPPI, PDCCONTI, PDCSOTTO, LIBRIIVA>(
                @"SELECT l.*, gi.*, ci.*, si.*, ge.*, ce.*, se.* FROM LIBRIIVA AS l
                        LEFT OUTER JOIN PDCGRUPPI AS gi ON gi.P1GRUP = l.livgci
                        LEFT OUTER JOIN PDCCONTI AS ci ON ci.P1GRUP = l.livgci AND ci.P2CONT = l.livcci
                        LEFT OUTER JOIN PDCSOTTO AS si ON si.P1GRUP = l.livgci AND si.P2CONT = l.livcci AND si.P3SOTC = l.livsci
                        LEFT OUTER JOIN PDCGRUPPI AS ge ON ge.P1GRUP = l.livgce
                        LEFT OUTER JOIN PDCCONTI AS ce ON ce.P1GRUP = l.livgce AND ce.P2CONT = l.livcce
                        LEFT OUTER JOIN PDCSOTTO AS se ON se.P1GRUP = l.livgce AND se.P2CONT = l.livcce AND se.P3SOTC = l.livsce",
                (liv, grpi, cnti, soti, grpe, cnte, sote) =>
                {
                    liv.SelectedIVAGroup = grpi;
                    liv.SelectedIVAAccount = cnti;
                    liv.SelectedIVASubaccount = soti;
                    liv.SelectedErarioGroup = grpe;
                    liv.SelectedErarioAccount = cnte;
                    liv.SelectedErarioSubaccount = sote;
                    return liv;
                },
                splitOn: "P1GRUP,P1GRUP,P1GRUP,P1GRUP,P1GRUP,P1GRUP");

            return new ObservableCollection<LIBRIIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public LIBRIIVA? Get(string livcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<LIBRIIVA>(
                "SELECT * FROM LIBRIIVA WHERE livcod = @livcod",
                new { livcod = livcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public LIBRIIVA? GetDefaultIVARecap()
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<LIBRIIVA>(
                "SELECT * FROM LIBRIIVA WHERE livliq = 1")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string livcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM LIBRIIVA WHERE livcod = @livcod",
                new { livcod = livcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO LIBRIIVA (livcod,livdes,livgci,livcci,livsci,livgce,livcce,livsce,livcgi,livtip,livaut,livcii,livulst,added,addedUserID,updated,updatedUserID,livliq,livili,livven) OUTPUT INSERTED.rv VALUES(@livcod,@livdes,@livgci,@livcci,@livsci,@livgce,@livcce,@livsce,@livcgi,@livtip,@livaut,@livcii,@livulst,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID,@livliq,@livili,@livven)";
    public string UPDATE_QUERY => "UPDATE LIBRIIVA SET livcod = @livcod,livdes = @livdes,livgci = @livgci,livcci = @livcci,livsci = @livsci,livgce = @livgce,livcce = @livcce,livsce = @livsce,livcgi = @livcgi,livtip = @livtip,livaut = @livaut,livcii = @livcii,livulst = @livulst,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID,livliq = @livliq,livili = @livili,livven = @livven OUTPUT INSERTED.rv WHERE livcod = @livcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM LIBRIIVA OUTPUT DELETED.rv WHERE livcod = @livcod AND rv = @rv";

    public bool Insert(LIBRIIVA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    if (Model.livliq)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE LIBRIIVA 
                                        SET livliq = 0
                                        WHERE livcod != @livcod",
                                Model, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(LIBRIIVA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(UPDATE_QUERY, Model, transaction);
                    if (Model.livliq)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE LIBRIIVA 
                                        SET livliq = 0
                                        WHERE livcod != @livcod",
                                Model, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(LIBRIIVA Model)
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

    public string? Validate(LIBRIIVA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.livcod) && IsInsert && !Exists(Model.livcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.livdes))
                {
                    if ((!string.IsNullOrWhiteSpace(Model.livgci) && !string.IsNullOrWhiteSpace(Model.livcci) && !string.IsNullOrWhiteSpace(Model.livsci)) ||
                        (string.IsNullOrWhiteSpace(Model.livgci) && string.IsNullOrWhiteSpace(Model.livcci) && string.IsNullOrWhiteSpace(Model.livsci)))
                    {
                        if ((!string.IsNullOrWhiteSpace(Model.livgce) && !string.IsNullOrWhiteSpace(Model.livcce) && !string.IsNullOrWhiteSpace(Model.livsce)) ||
                        (string.IsNullOrWhiteSpace(Model.livgce) && string.IsNullOrWhiteSpace(Model.livcce) && string.IsNullOrWhiteSpace(Model.livsce)))
                        {
                            return null;
                        }
                        else
                        { return "Gruppo, conto e sottocnto ERARIO devono essere tutti valorizzati o tutti vuoti"; }
                    }
                    else
                    { return "Gruppo, conto e sottocnto IVA devono essere tutti valorizzati o tutti vuoti"; }
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

public class LIBRIIVAUfpRepository : RepositoryBase, ILIBRIIVARepository
{
    public LIBRIIVAUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<LIBRIIVA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<LIBRIIVA, PDCGRUPPI, PDCCONTI, PDCSOTTO, PDCGRUPPI, PDCCONTI, PDCSOTTO, LIBRIIVA>(
                @"SELECT l.*, gi.*, ci.*, si.*, ge.*, ce.*, se.* FROM LIBRIIVA AS l
                        LEFT OUTER JOIN PDC_GRUPPI AS gi ON gi.P1GRUP = l.livgci
                        LEFT OUTER JOIN PDC_CONTI AS ci ON ci.P1GRUP = l.livgci AND ci.P2CONT = l.livcci
                        LEFT OUTER JOIN PDC_SOTTOCONTI AS si ON si.P1GRUP = l.livgci AND si.P2CONT = l.livcci AND si.P3SOTC = l.livsci
                        LEFT OUTER JOIN PDC_GRUPPI AS ge ON ge.P1GRUP = l.livgce
                        LEFT OUTER JOIN PDC_CONTI AS ce ON ce.P1GRUP = l.livgce AND ce.P2CONT = l.livcce
                        LEFT OUTER JOIN PDC_SOTTOCONTI AS se ON se.P1GRUP = l.livgce AND se.P2CONT = l.livcce AND se.P3SOTC = l.livsce",
                (liv, grpi, cnti, soti, grpe, cnte, sote) =>
                {
                    liv.SelectedIVAGroup = grpi;
                    liv.SelectedIVAAccount = cnti;
                    liv.SelectedIVASubaccount = soti;
                    liv.SelectedErarioGroup = grpe;
                    liv.SelectedErarioAccount = cnte;
                    liv.SelectedErarioSubaccount = sote;
                    return liv;
                },
                splitOn: "P1GRUP,P1GRUP,P1GRUP,P1GRUP,P1GRUP,P1GRUP");

            return new ObservableCollection<LIBRIIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public LIBRIIVA? Get(string livcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<LIBRIIVA>(
                "SELECT * FROM LIBRIIVA WHERE livcod = @livcod",
                new { livcod = livcod })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public LIBRIIVA? GetDefaultIVARecap()
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<LIBRIIVA>(
                "SELECT * FROM LIBRIIVA WHERE livliq = 1")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string livcod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM LIBRIIVA WHERE livcod = @livcod",
                new { livcod = livcod }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO LIBRIIVA (livcod,livdes,livgci,livcci,livsci,livgce,livcce,livsce,livcgi,livtip,livaut,livcii, livpro,livagg) OUTPUT INSERTED.rv VALUES(@livcod,@livdes,@livgci,@livcci,@livsci,@livgce,@livcce,@livsce,@livcgi,@livtip,@livaut,@livcii,@livulst,@livliq,@livili,@livven, @livpro)";
    public string UPDATE_QUERY => "UPDATE LIBRIIVA SET livcod = @livcod,livdes = @livdes,livgci = @livgci,livcci = @livcci,livsci = @livsci,livgce = @livgce,livcce = @livcce,livsce = @livsce,livcgi = @livcgi,livtip = @livtip,livaut = @livaut,livcii = @livcii, livpro = @livpro, livagg = @livagg OUTPUT INSERTED.rv WHERE livcod = @livcod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM LIBRIIVA OUTPUT DELETED.rv WHERE livcod = @livcod AND rv = @rv";

    public bool Insert(LIBRIIVA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(INSERT_QUERY, Model, transaction);
                    if (Model.livliq)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE LIBRIIVA 
                                        SET livliq = 0
                                        WHERE livcod != @livcod",
                                Model, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(LIBRIIVA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    connection.Execute(UPDATE_QUERY, Model, transaction);
                    if (Model.livliq)
                    {
                        // reset others
                        connection.ExecuteScalar(
                                @"UPDATE LIBRIIVA 
                                        SET livliq = 0
                                        WHERE livcod != @livcod",
                                Model, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message.ToString());
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(LIBRIIVA Model)
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

    public string? Validate(LIBRIIVA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.livcod) && IsInsert && !Exists(Model.livcod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.livdes))
                {
                    if ((!string.IsNullOrWhiteSpace(Model.livgci) && !string.IsNullOrWhiteSpace(Model.livcci) && !string.IsNullOrWhiteSpace(Model.livsci)) ||
                        (string.IsNullOrWhiteSpace(Model.livgci) && string.IsNullOrWhiteSpace(Model.livcci) && string.IsNullOrWhiteSpace(Model.livsci)))
                    {
                        if ((!string.IsNullOrWhiteSpace(Model.livgce) && !string.IsNullOrWhiteSpace(Model.livcce) && !string.IsNullOrWhiteSpace(Model.livsce)) ||
                        (string.IsNullOrWhiteSpace(Model.livgce) && string.IsNullOrWhiteSpace(Model.livcce) && string.IsNullOrWhiteSpace(Model.livsce)))
                        {
                            return null;
                        }
                        else
                        { return "Gruppo, conto e sottocnto ERARIO devono essere tutti valorizzati o tutti vuoti"; }
                    }
                    else
                    { return "Gruppo, conto e sottocnto IVA devono essere tutti valorizzati o tutti vuoti"; }
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