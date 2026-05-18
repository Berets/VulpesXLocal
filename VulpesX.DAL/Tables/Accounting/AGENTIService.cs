using VulpesX.DAL;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Production;
using VulpesX.Models.Ufp;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Accounting;


public interface IAGENTIRepository
{
    ObservableCollection<AGENTI>? GetList();

    ObservableCollection<AGENTI>? GetListWithEmpty();

    ObservableCollection<AGENTI>? GetAreaManager();

    AGENTI? Get(string agecod);

    bool Exists(string agecod);

    ObservableCollection<AGEPROVPER>? GetAGEPROVPERs(string agecod);

    ObservableCollection<AGENTI_SOTTOLIVELLO>? GetAGENTI_SOTTOLIVELLOs(string agecod);

    ObservableCollection<TAB_AGENTI_ENASARCO>? GetTAB_AGENTI_ENASARCOs(string CompanyID, string agecod);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    string INSERT_QUERY_EXCEPTION_CAUSAL { get; }
    string INSERT_QUERY_EXCEPTION_ARTICLE { get; }
    string INSERT_QUERY_ENASARCO { get; }

    bool Insert(AGENTI Model);

    bool Insert(string CompanyID, AGENTI Model, ObservableCollection<AGEPROVPER>? ExceptionsCausal, ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle, ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarco);

    bool Update(AGENTI Model);

    bool Update(string CompanyID, AGENTI Model, ObservableCollection<AGEPROVPER>? ExceptionsCausal, ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle, ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarco);

    bool Cancel(AGENTI Model);

    string? Validate(AGENTI Model, bool IsInsert);

    string? ValidateCausals(List<AGEPROVPER> Causals, AGEPROVPER Causal);

    string? ValidateArticles(List<AGENTI_SOTTOLIVELLO> Articles, AGENTI_SOTTOLIVELLO Article);

    string? ValidateEnasarco(List<TAB_AGENTI_ENASARCO> Enasarcos, TAB_AGENTI_ENASARCO Enasarco);
    #endregion
}

public class AGENTIRepository : RepositoryBase, IAGENTIRepository
{
    public AGENTIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AGENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AGENTI>(
                "SELECT * FROM AGENTI ORDER BY agedes");

            return new ObservableCollection<AGENTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<AGENTI>? GetListWithEmpty()
    {
        try
        {
            using var connection = GetOpenConnection();

            var retValue = new ObservableCollection<AGENTI>();
            retValue.Add(new AGENTI { agecod = string.Empty, agedes = "--NESSUNO--", ageflag = string.Empty });

            var list = connection.Query<AGENTI>(
                "SELECT * FROM AGENTI ORDER BY agedes");

            foreach (var age in list)
            {
                retValue.Add(age);
            }
            return retValue;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<AGENTI>? GetAreaManager()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AGENTI>(
                "SELECT * FROM AGENTI WHERE agecap = 1 ORDER BY agedes");

            return new ObservableCollection<AGENTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AGENTI? Get(string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<AGENTI>(
                "SELECT * FROM AGENTI WHERE agecod = @agecod",
                new { agecod = agecod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AGENTI WHERE agecod = @agecod",
                new { agecod = agecod }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ObservableCollection<AGEPROVPER>? GetAGEPROVPERs(string agecod)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<AGENTI_SOTTOLIVELLO>? GetAGENTI_SOTTOLIVELLOs(string agecod)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<TAB_AGENTI_ENASARCO>? GetTAB_AGENTI_ENASARCOs(string CompanyID, string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_AGENTI_ENASARCO>(
                "SELECT * FROM TAB_AGENTI_ENASARCO WHERE agecod = @agecod ORDER BY enaann desc", new { agecod = agecod });

            return new ObservableCollection<TAB_AGENTI_ENASARCO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO AGENTI (agecod,agedes,agepvg,agecal,ageliq,ageflag,agecap,agefor,agepvgt,LogCanceled,LogCanceledUserID) OUTPUT INSERTED.rv VALUES(@agecod,@agedes,@agepvg,@agecal,@ageliq,@ageflag,@agecap,@agefor,@agepvgt,@LogCanceled,@LogCanceledUserID)";
    public string UPDATE_QUERY => "UPDATE AGENTI SET agecod = @agecod,agedes = @agedes,agepvg = @agepvg,agecal = @agecal,ageliq = @ageliq,ageflag = @ageflag,agecap = @agecap,agefor = @agefor,agepvgt = @agepvgt, LogCanceled=@LogCanceled,LogCanceledUserID = @LogCanceledUserID OUTPUT INSERTED.rv WHERE agecod = @agecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM AGENTI OUTPUT DELETED.rv WHERE agecod = @agecod AND rv = @rv";
    public string INSERT_QUERY_EXCEPTION_CAUSAL => string.Empty;
    public string INSERT_QUERY_EXCEPTION_ARTICLE => string.Empty;
    public string INSERT_QUERY_ENASARCO => "INSERT INTO TAB_AGENTI_ENASARCO (agecod,enaann,enaperazi,enaannmax) VALUES(@agecod,@enaann,@enaperazi,@enaannmax)";


    public bool Insert(AGENTI Model)
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

    public bool Insert(string CompanyID, AGENTI Model, ObservableCollection<AGEPROVPER>? ExceptionsCausal, ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle, ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarco)
    {
        throw new NotImplementedException();
    }

    public bool Update(AGENTI Model)
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

    public bool Update(string CompanyID, AGENTI Model, ObservableCollection<AGEPROVPER>? ExceptionsCausal, ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle, ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarco)
    {
        throw new NotImplementedException();
    }

    public bool Cancel(AGENTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(UPDATE_QUERY, Model);
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

    public string? Validate(AGENTI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.agecod) && IsInsert && !Exists(Model.agecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.agedes))
                {
                    if ((Model.agepvg.HasValue && !string.IsNullOrWhiteSpace(Model.agepvgt)) ||
                        (!Model.agepvg.HasValue && string.IsNullOrWhiteSpace(Model.agepvgt)))
                    {
                        return null;
                    }
                    else
                    { return "Provvigione e tipo provvigione vanno specificati entrambi o lasciati entrambi vuoti"; }
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

    public string? ValidateCausals(List<AGEPROVPER> Causals, AGEPROVPER Causal)
    {
        throw new NotImplementedException();
    }

    public string? ValidateArticles(List<AGENTI_SOTTOLIVELLO> Articles, AGENTI_SOTTOLIVELLO Article)
    {
        throw new NotImplementedException();
    }

    public string? ValidateEnasarco(List<TAB_AGENTI_ENASARCO> Enasarcos, TAB_AGENTI_ENASARCO Enasarco)
    {
        throw new NotImplementedException();
    }
    #endregion
}

public class AGENTIUfpRepository : RepositoryBase, IAGENTIRepository
{
    public AGENTIUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AGENTI>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AGENTI>(
                "SELECT * FROM TAB_AGENTI ORDER BY agedes");

            return new ObservableCollection<AGENTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<AGENTI>? GetListWithEmpty()
    {
        try
        {
            using var connection = GetOpenConnection();

            var retValue = new ObservableCollection<AGENTI>();
            retValue.Add(new AGENTI { agecod = string.Empty, agedes = "--NESSUNO--", ageflag = string.Empty });

            var list = connection.Query<AGENTI>(
                "SELECT * FROM TAB_AGENTI ORDER BY agedes");

            foreach (var age in list)
            {
                retValue.Add(age);
            }
            return retValue;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<AGENTI>? GetAreaManager()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AGENTI>(
                "SELECT * FROM TAB_AGENTI WHERE agecap = 1 ORDER BY agedes");

            return new ObservableCollection<AGENTI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AGENTI? Get(string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<AGENTI>(
                "SELECT * FROM TAB_AGENTI WHERE agecod = @agecod",
                new { agecod = agecod })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TAB_AGENTI WHERE agecod = @agecod",
                new { agecod = agecod }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public ObservableCollection<AGEPROVPER>? GetAGEPROVPERs(string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AGEPROVPER>(
                "SELECT * FROM AGEPROVPER WHERE appcod = @agecod ORDER BY appclie", new { agecod = agecod });

            return new ObservableCollection<AGEPROVPER>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<AGENTI_SOTTOLIVELLO>? GetAGENTI_SOTTOLIVELLOs(string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AGENTI_SOTTOLIVELLO>(
                "SELECT * FROM AGENTI_SOTTOLIVELLO WHERE agecod = @agecod ORDER BY agecli", new { agecod = agecod });

            return new ObservableCollection<AGENTI_SOTTOLIVELLO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TAB_AGENTI_ENASARCO>? GetTAB_AGENTI_ENASARCOs(string CompanyID, string agecod)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<TAB_AGENTI_ENASARCO>(
                @$"SELECT en.*, ag.agefor as SupplierID FROM TAB_AGENTI_ENASARCO as en
                    LEFT OUTER JOIN TAB_AGENTI as ag ON en.agecod = ag.agecod
                    WHERE en.agesoc = @CompanyID AND en.agecod = @agecod 
                    ORDER BY en.enaann desc", new { CompanyID, agecod });

            foreach (var year in list.Where(o=>o.SupplierID != null))
            {
                var productions = connection.Query<ACC_EINVOICE_RIT>(
                    $@"SELECT 
                        rit.*,
                        rit.impriten as importo,
                        hea.*,
                        doc.FETREna
                        FROM FATTIMPRITE AS rit 
                        INNER JOIN FATTIMP as hea ON rit.fattsoc = hea.fattsoc AND rit.fattnum = hea.fattnum AND rit.fattdari = hea.fattdata AND rit.fattpiva = hea.fattpiva
                        LEFT OUTER JOIN FE_RITDOC as doc ON rit.tiporiten = doc.FETRCod
                        WHERE hea.fattsoc = @CompanyID AND YEAR(hea.fattdata) = @Year AND hea.fattfor = @SupplierID AND doc.FETREna = 1
                        order by  hea.fattdata",
                    new[] { typeof(ACC_EINVOICE_RIT), typeof(ACC_EINVOICE_HEADS), typeof(bool?) }
                , (objects) =>
                {
                    var ritenuta = (objects[0] as ACC_EINVOICE_RIT)!;
                    var head = (objects[1] as ACC_EINVOICE_HEADS)!;

                    ritenuta.EnasarcoCaricoAzienda = ((head.fattimpo * year.enaperazi) / 100) + ritenuta.importo;

                    return ritenuta;
                },
                new { CompanyID, Year = year.enaann, SupplierID = year.SupplierID },
                splitOn: "fattsoc,FETREna");

                year.Used = productions.Sum(s => s.EnasarcoCaricoAzienda ?? 0);
            }

            return new ObservableCollection<TAB_AGENTI_ENASARCO>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TAB_AGENTI (agecod,agedes,agepvg,agecal,ageliq,agefor,agetel,agefax,agemail,ageflg,ageflg2, ageinvmail) OUTPUT INSERTED.rv VALUES(@agecod,@agedes,@agepvg,@agecal,@ageliq,@agefor,@agetel,@agefax,@agemail,@ageflg,@ageflg2, @ageinvmail)";
    public string UPDATE_QUERY => "UPDATE TAB_AGENTI SET agecod = @agecod,agedes = @agedes,agepvg = @agepvg,agecal = @agecal,ageliq = @ageliq,agefor = @agefor,agetel=@agetel,agefax=@agefax,agemail=@agemail,ageflg=@ageflg,ageflg2=@ageflg2, ageinvmail=@ageinvmail OUTPUT INSERTED.rv WHERE agecod = @agecod AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TAB_AGENTI OUTPUT DELETED.rv WHERE agecod = @agecod AND rv = @rv";
    public string INSERT_QUERY_EXCEPTION_CAUSAL => "INSERT INTO AGEPROVPER (appcod,appclie,appcau,appfor,appsca1,appscaper1,appsca2,appscaper2,appsca3,appscaper3,appper1,appper2) VALUES(@appcod,@appclie,@appcau,@appfor,@appsca1,@appscaper1,@appsca2,@appscaper2,@appsca3,@appscaper3,@appper1,@appper2)";
    public string INSERT_QUERY_EXCEPTION_ARTICLE => "INSERT INTO AGENTI_SOTTOLIVELLO (agecod,agecli,ageart,ageperc,ageclid,ageartd) VALUES(@agecod,@agecli,@ageart,@ageperc,@ageclid,@ageartd)";
    public string INSERT_QUERY_ENASARCO => "INSERT INTO TAB_AGENTI_ENASARCO (agesoc,agecod,enaann,enaperazi,enaannmax) VALUES(@agesoc,@agecod,@enaann,@enaperazi,@enaannmax)";

    public bool Insert(AGENTI Model)
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

    public bool Insert(string CompanyID, AGENTI Model, ObservableCollection<AGEPROVPER>? ExceptionsCausal, ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle, ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarco)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean exceptions
                    connection.Execute("DELETE FROM AGEPROVPER WHERE appcod=@appcod",
                        new { appcod = Model.agecod }, transaction);

                    foreach (var group in ExceptionsCausal ?? new ObservableCollection<AGEPROVPER>())
                    {
                        connection.Execute(INSERT_QUERY_EXCEPTION_CAUSAL, group, transaction);
                    }

                    connection.Execute("DELETE FROM AGENTI_SOTTOLIVELLO WHERE agecod=@agecod",
                        new { agecod = Model.agecod }, transaction);

                    foreach (var group in ExceptionsArticle ?? new ObservableCollection<AGENTI_SOTTOLIVELLO>())
                    {
                        connection.Execute(INSERT_QUERY_EXCEPTION_ARTICLE, group, transaction);
                    }

                    connection.Execute("DELETE FROM TAB_AGENTI_ENASARCO WHERE agesoc = @agesoc AND agecod=@agecod",
                       new { agesoc = CompanyID, agecod = Model.agecod }, transaction);

                    foreach (var enasarco in Enasarco ?? new ObservableCollection<TAB_AGENTI_ENASARCO>())
                    {
                        connection.Execute(INSERT_QUERY_ENASARCO, enasarco, transaction);
                    }

                    var result = connection.Execute(INSERT_QUERY, Model, transaction);
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

    public bool Update(AGENTI Model)
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

    public bool Update(string CompanyID, AGENTI Model, ObservableCollection<AGEPROVPER>? ExceptionsCausal, ObservableCollection<AGENTI_SOTTOLIVELLO>? ExceptionsArticle, ObservableCollection<TAB_AGENTI_ENASARCO>? Enasarco)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // clean exceptions
                    connection.Execute("DELETE FROM AGEPROVPER WHERE appcod=@appcod",
                        new { appcod = Model.agecod }, transaction);

                    foreach (var group in ExceptionsCausal ?? new ObservableCollection<AGEPROVPER>())
                    {
                        connection.Execute(INSERT_QUERY_EXCEPTION_CAUSAL, group, transaction);
                    }

                    connection.Execute("DELETE FROM AGENTI_SOTTOLIVELLO WHERE agecod=@agecod",
                        new { agecod = Model.agecod }, transaction);

                    foreach (var group in ExceptionsArticle ?? new ObservableCollection<AGENTI_SOTTOLIVELLO>())
                    {
                        connection.Execute(INSERT_QUERY_EXCEPTION_ARTICLE, group, transaction);
                    }

                    connection.Execute("DELETE FROM TAB_AGENTI_ENASARCO WHERE agesoc = @agesoc AND agecod=@agecod",
                      new { agesoc = CompanyID, agecod = Model.agecod }, transaction);

                    foreach (var enasarco in Enasarco ?? new ObservableCollection<TAB_AGENTI_ENASARCO>())
                    {
                        connection.Execute(INSERT_QUERY_ENASARCO, enasarco, transaction);
                    }

                    var result = connection.Execute(UPDATE_QUERY, Model, transaction);
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

    public bool Cancel(AGENTI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(UPDATE_QUERY, Model);
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

    public string? Validate(AGENTI Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.agecod) && IsInsert && !Exists(Model.agecod)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.agedes))
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

    public string? ValidateCausals(List<AGEPROVPER> Causals, AGEPROVPER Causal)
    {
        try
        {
            if (Causal.SelectedCustomer == null)
            {
                return "Cliente obbligatorio";
            }
            if (Causal.SelectedSupplier == null)
            {
                return "Fornitore obbligatorio";
            }
            if (Causal.SelectedOrderCausal == null)
            {
                return "Causale obbligatoria";
            }

            var matches = (Causals ?? Enumerable.Empty<AGEPROVPER>()).Count(o =>
                o.appclie == Causal.SelectedCustomer?.abecod &&
                o.appfor == Causal.SelectedSupplier?.abecod &&
                o.appcau == Causal.SelectedOrderCausal?.cauacq
            );

            bool exist;

            if (Causal.IsInsert)
            {
                exist = matches >= 1;
            }
            else
            {
                bool keyUnchanged = Causal.appclie == Causal.SelectedCustomer?.abecod && Causal.appfor == Causal.SelectedSupplier?.abecod && Causal.appcau == Causal.SelectedOrderCausal?.cauacq;

                exist = keyUnchanged ? matches > 1 : matches >= 1;
            }

            if (exist)
            {
                return "Eccezione già esistente";
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string? ValidateArticles(List<AGENTI_SOTTOLIVELLO> Articles, AGENTI_SOTTOLIVELLO Article)
    {
        try
        {
            if (Article.SelectedCustomer == null)
            {
                return "Cliente obbligatorio";
            }
            if (Article.SelectedArticle == null)
            {
                return "Articolo obbligatorio";
            }
            if (Article.ageperc == null)
            {
                return "Percentuale obbligatoria";
            }

            var matches = (Articles ?? Enumerable.Empty<AGENTI_SOTTOLIVELLO>()).Count(o =>
                o.agecli == Article.SelectedCustomer?.abecod &&
                o.ageart == Article.SelectedArticle?.ID
            );

            bool exist;

            if (Article.IsInsert)
            {
                exist = matches >= 1;
            }
            else
            {
                bool keyUnchanged = Article.agecli == Article.SelectedCustomer?.abecod && Article.ageart == Article.SelectedArticle?.ID;

                exist = keyUnchanged ? matches > 1 : matches >= 1;
            }

            if (exist)
            {
                return "Eccezione già esistente";
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string? ValidateEnasarco(List<TAB_AGENTI_ENASARCO> Enasarcos, TAB_AGENTI_ENASARCO Enasarco)
    {
        try
        {
            if (Enasarco.enaann == 0)
            {
                return "Anno obbligatorio";
            }
            if (Enasarco.enaperazi == null)
            {
                return "% carico azienda obbligatoria";
            }
            if (Enasarco.enaannmax == null)
            {
                return "Massimale obbligatorio";
            }

            var matches = (Enasarcos ?? Enumerable.Empty<TAB_AGENTI_ENASARCO>()).Count(o =>
                o.enaann == Enasarco.enaann
            );

            bool exist;

            if (Enasarco.IsInsert)
            {
                exist = matches >= 2;
            }
            else
            {
                exist = matches >= 2;
            }

            if (exist)
            {
                return "Eccezione già esistente";
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}