using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VulpesX.DAL._ConnectionFactory;
using VulpesX.Shared.Utilities;

namespace VulpesX.DAL.Assets;
public interface IASSED00FRepository
{
    ObservableCollection<ASSED00F>? GetList(string CompanyID, string ID);

    ASSED00F? Get(string company_id, string id, int position);

    bool Exists(string company_id, string id, int position);

    bool Insert(ASSED00F Model);

    bool UpdateAll(ASSET00F Head, List<ASSED00F> Rows);

    bool Update(ASSED00F Model);

    bool Delete(ASSED00F Model);

    string? Validate(ASSED00F Model, bool IsInsert);
}

public class ASSED00FRepository : RepositoryBase, IASSED00FRepository
{
    public ASSED00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ASSED00F>? GetList(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ASSED00F>(
                @"SELECT * FROM ASSED00F
                        WHERE company_id=@cid AND id=@id
                        ORDER BY position", new { cid = CompanyID, id = ID });

            return new ObservableCollection<ASSED00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ASSED00F? Get(string company_id, string id, int position)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ASSED00F>(
                "SELECT * FROM ASSED00F WHERE company_id = @company_id AND id = @id AND position = @position",
                new { company_id, id, position })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, string id, int position)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ASSED00F WHERE company_id = @company_id AND id = @id AND position = @position",
                new { company_id, id, position }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ASSED00F (company_id,id,position,description,product_id,quantity,note,step) OUTPUT INSERTED.rv VALUES(@company_id,@id,@position,@description,@product_id,@quantity,@note,@step)";
    public readonly string UPDATE_QUERY = "UPDATE ASSED00F SET company_id = @company_id,id = @id,position = @position,description = @description,product_id = @product_id,quantity = @quantity,note = @note,step = @step OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND position = @position AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ASSED00F OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND position = @position AND rv = @rv";
    public bool Insert(ASSED00F Model)
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
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(ASSED00F Model)
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
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool UpdateAll(ASSET00F Head, List<ASSED00F> Rows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // update head
                var heads = connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IASSET00FRepository>().UPDATE_QUERY, Head, transaction);

                connection.Execute("DELETE FROM ASSED00F WHERE company_id = @cid AND id = @id",
                    new { cid = Head.company_id, id = Head.id },
                    transaction);

                foreach (var row in Rows)
                {
                    connection.Execute(INSERT_QUERY, row, transaction);
                }


                if (heads > 0)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Validation("Impossibile aggiornare la testata dell'asset");
                    return false;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message.ToString());
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message.ToString());
            return false;
        }
    }

    public bool Delete(ASSED00F Model)
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
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(ASSED00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.company_id) && !string.IsNullOrWhiteSpace(Model.id) && Model.position > 0)
            {
                if (!string.IsNullOrWhiteSpace(Model.description) || !string.IsNullOrWhiteSpace(Model.product_id))
                {
                    return null;
                }
                else
                { return "La descrizione o un articolo sono obbligatori"; }
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