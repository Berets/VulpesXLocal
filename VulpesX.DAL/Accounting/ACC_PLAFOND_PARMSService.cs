using VulpesX.DAL;

namespace VulpesX.DAL.Accounting;

public interface IACC_PLAFOND_PARMSRepository
{
    ObservableCollection<ACC_PLAFOND_PARMS>? GetList();

    ACC_PLAFOND_PARMS? Get(string company_id);

    bool Exists(string company_id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_PLAFOND_PARMS Model);

    bool Update(ACC_PLAFOND_PARMS Model);

    bool Delete(ACC_PLAFOND_PARMS Model);

    string? Validate(ACC_PLAFOND_PARMS Model, bool IsInsert);
    #endregion
}

public class ACC_PLAFOND_PARMSRepository : RepositoryBase, IACC_PLAFOND_PARMSRepository
{
    public ACC_PLAFOND_PARMSRepository(IConnectionFactory factory) : base(factory)
    {
    }
    public ObservableCollection<ACC_PLAFOND_PARMS>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ACC_PLAFOND_PARMS>(
                "SELECT * FROM ACC_PLAFOND_PARMS");

            return new ObservableCollection<ACC_PLAFOND_PARMS>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_PLAFOND_PARMS? Get(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ACC_PLAFOND_PARMS>(
                @"SELECT ap.*, p.UnitaID AS UM, ass.assnatufe AS IVANature FROM ACC_PLAFOND_PARMS AS ap
                        INNER JOIN tab_articolo AS p ON p.SocietaID = ap.company_id AND p.ID = ap.product_id
                        INNER JOIN ASSOGGETAMENTI AS ass ON ass.asscod=ap.rate_code AND ass.assali=ap.rate_value
                        WHERE ap.company_id = @company_id",
                new { company_id = company_id })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_PLAFOND_PARMS WHERE company_id = @company_id",
                new { company_id = company_id }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_PLAFOND_PARMS (company_id,limit_amount,stamp_amount,rate_code,rate_value,group_id,account_id,subaccount_id,added,addedUserID,updated,updatedUserID,product_id) OUTPUT INSERTED.rv VALUES(@company_id,@limit_amount,@stamp_amount,@rate_code,@rate_value,@group_id,@account_id,@subaccount_id,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID,@product_id)";
    public string UPDATE_QUERY => "UPDATE ACC_PLAFOND_PARMS SET company_id = @company_id,limit_amount = @limit_amount,stamp_amount = @stamp_amount,rate_code = @rate_code,rate_value = @rate_value,group_id = @group_id,account_id = @account_id,subaccount_id = @subaccount_id,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID,product_id = @product_id OUTPUT INSERTED.rv WHERE company_id = @company_id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_PLAFOND_PARMS OUTPUT DELETED.rv WHERE company_id = @company_id AND rv = @rv";
    public bool Insert(ACC_PLAFOND_PARMS Model)
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

    public bool Update(ACC_PLAFOND_PARMS Model)
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

    public bool Delete(ACC_PLAFOND_PARMS Model)
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

    public string? Validate(ACC_PLAFOND_PARMS Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.company_id))
            {
                if (Model.limit_amount > 0)
                {
                    if (Model.stamp_amount > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.product_id))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.rate_code) && !string.IsNullOrWhiteSpace(Model.rate_value))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.group_id) && !string.IsNullOrWhiteSpace(Model.account_id) && !string.IsNullOrWhiteSpace(Model.subaccount_id))
                                {
                                    return null;
                                }
                                else
                                { return "Il conto contabile è un dato obbligatorio"; }
                            }
                            else
                            { return "L'aliquota IVA è un dato obbligatorio"; }
                        }
                        else
                        { return "L'articolo è un dato obbligatorio"; }
                    }
                    else
                    { return "L'importo del bollo deve essere maggiore di 0"; }
                }
                else
                { return "L'importo limite deve essere maggiore di 0"; }
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