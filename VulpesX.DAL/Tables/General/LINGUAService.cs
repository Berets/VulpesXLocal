using System.Xml.Linq;

namespace VulpesX.DAL.Tables.General;


public interface ILINGUARepository
{
    ObservableCollection<LINGUA>? GetList();

    ObservableCollection<LINGUA>? GetList(string AZCode);

    LINGUA? Get(string ID);

    bool Exists(string ID);

    Dictionary<string, string>? GetDictionary(string lincod);

    #region CRUD
    bool Insert(LINGUA Model);

    bool Update(LINGUA Model);

    bool Delete(LINGUA Model);

    string? Validate(LINGUA Model, bool IsInsert);
    #endregion
}

public class LINGUARepository : RepositoryBase, ILINGUARepository
{
    public LINGUARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<LINGUA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<LINGUA>(
                "SELECT * FROM LINGUA");

            return new ObservableCollection<LINGUA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<LINGUA>? GetList(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();



            var lingua = connection.Query<string>("SELECT azlinguadefault FROM AZIENDA WHERE AZCode = @AZCode", new { AZCode = AZCode }).FirstOrDefault();

            var list = connection.Query<LINGUA>(
                "SELECT * FROM LINGUA WHERE lincod <> @LinguaDefault", new { LinguaDefault = lingua });

            return new ObservableCollection<LINGUA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public LINGUA? Get(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<LINGUA>(
                "SELECT * FROM LINGUA WHERE lincod = @lincod",
                new { lincod = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM LINGUA WHERE lincod = @lincod",
                new { lincod = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public Dictionary<string, string>? GetDictionary(string lincod)
    {
        try
        {
            using var connection = GetOpenConnection();


            XNamespace sys = "clr-namespace:System;assembly=mscorlib";
            XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

            // Get from storage
            Dictionary<string, string>? keyStorage = null;
            var storage = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, "Language_Reports.xaml");

            if (storage != null)
            {
                // Get keys from storage
                using var streamStorage = new MemoryStream(storage);
                XDocument docStorage = XDocument.Load(streamStorage);

                keyStorage = docStorage
                   .Descendants(sys + "String")
                   .Where(e => e.Attribute(x + "Key") != null)
                   .ToDictionary(
                       e => (string)e.Attribute(x + "Key")!,
                       e => (string)e.Value
                );
            }

            var lingua = connection.Query<LINGUA>("SELECT * FROM LINGUA WHERE lincod = @lincod",
                                    new { lincod = lincod })
                                    .FirstOrDefault();

            if (lingua != null)
            {
                if (lingua.linreport != null)
                {
                    using var streamLanguage = new MemoryStream(lingua.linreport);
                    XDocument docLanguage = XDocument.Load(streamLanguage);

                    return docLanguage
                       .Descendants(sys + "String")
                       .Where(e => e.Attribute(x + "Key") != null)
                       .ToDictionary(
                           e => (string)e.Attribute(x + "Key")!,
                           e => (string)e.Value
                    );
                }
            }

            return keyStorage;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public bool Insert(LINGUA Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                "INSERT INTO LINGUA (lincod,lindes,linreport) OUTPUT INSERTED.rv VALUES(@lincod,@lindes,@linreport)",
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

    public bool Update(LINGUA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE LINGUA SET lincod = @lincod,lindes = @lindes,linreport=@linreport OUTPUT INSERTED.rv WHERE lincod = @lincod AND rv = @rv",
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

    public bool Delete(LINGUA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM LINGUA OUTPUT DELETED.rv WHERE lincod = @lincod AND rv = @rv",
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

    public string? Validate(LINGUA Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.lincod) && IsInsert && !Exists(Model.lincod) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.lindes))
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