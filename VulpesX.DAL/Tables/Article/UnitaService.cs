
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.Article;

public interface IUnitaRepository
{
    ObservableCollection<tab_articolo_unita>? GetList(string CompanyID);

    ObservableCollection<GenericIDDescription>? GetSimpleList(string CompanyID);

    ObservableCollection<GenericIDDescription>? GetSimpleListByProduct(string CompanyID, string ProductID);

    tab_articolo_unita? Get(string CompanyID, string ID);

    bool Exists(string CompanyID, string ID);

    #region CRUD
    bool Insert(tab_articolo_unita Model);

    bool Update(tab_articolo_unita Model);

    bool Delete(tab_articolo_unita Model);

    string? Validate(tab_articolo_unita Model, bool IsInsert);
    #endregion
}

public class UnitaRepository : RepositoryBase, IUnitaRepository
{
    public UnitaRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<tab_articolo_unita>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_articolo_unita>(
                "SELECT * FROM tab_articolo_unita WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

            return new ObservableCollection<tab_articolo_unita>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetSimpleList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIDDescription>(
                "SELECT ID, Descrizione AS Description FROM tab_articolo_unita WHERE SocietaID = @SocietaID", new { SocietaID = CompanyID });

            return new ObservableCollection<GenericIDDescription>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<GenericIDDescription>? GetSimpleListByProduct(string CompanyID, string ProductID)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(ProductID))
            {
                using var connection = GetOpenConnection();


                Tuple<string, string>? ums = connection.Query<Tuple<string, string>>(
                    "SELECT UnitaID AS Item1, UnitaIDAlt AS Item2 FROM tab_articolo WHERE SocietaID = @SocietaID AND ID = @ProductID", new { SocietaID = CompanyID, ProductID = ProductID }).FirstOrDefault();

                if (ums == null)
                    return null;

                var list = connection.Query<GenericIDDescription>(
                    "SELECT ID, Descrizione AS Description FROM tab_articolo_unita WHERE SocietaID = @SocietaID AND ID IN @ums", new { SocietaID = CompanyID, ums = new string[] { ums.Item1, ums.Item2 } });

                return new ObservableCollection<GenericIDDescription>(list);

            }
            else
            { return null; }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_unita? Get(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<tab_articolo_unita>(
                "SELECT * FROM tab_articolo_unita WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo_unita WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public bool Insert(tab_articolo_unita Model)
    {
        using var connection = GetOpenConnection();


        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.Execute(
                    "INSERT INTO tab_articolo_unita (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Descrizione)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Descrizione)",
                    Model, transaction);


                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

    }

    public bool Update(tab_articolo_unita Model)
    {
        using var connection = GetOpenConnection();


        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_articolo_unita SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Descrizione = @Descrizione" +
                    " OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND rv = @rv",
                    Model, transaction);

                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

    }

    public bool Delete(tab_articolo_unita Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_articolo_unita OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv",
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

    public string? Validate(tab_articolo_unita Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.ID) && IsInsert && !Exists(Model.SocietaID, Model.ID) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Descrizione) && Model.Descrizione.Length <= 255)
                {
                    return null;
                }
                else
                { return "La descrizione è obbligatoria e può contenere al massimo 255 caratteri"; }
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