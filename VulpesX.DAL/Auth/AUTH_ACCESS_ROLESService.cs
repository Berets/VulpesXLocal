using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Auth;


public interface IAUTH_ACCESS_ROLESRepository
{
    AUTH_ACCESS_ROLES? Get(string CompanyID, string userID);

    bool Exists(string CompanyID, string userID);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(AUTH_ACCESS_ROLES Model);

    bool Update(AUTH_ACCESS_ROLES Model);

    bool Delete(AUTH_ACCESS_ROLES Model);

    string? Validate(AUTH_ACCESS_ROLES Model, bool IsInsert);
    #endregion
}

public class AUTH_ACCESS_ROLESRepository : RepositoryBase, IAUTH_ACCESS_ROLESRepository
{
    public AUTH_ACCESS_ROLESRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public AUTH_ACCESS_ROLES? Get(string CompanyID, string userID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Query<AUTH_ACCESS_ROLES>(
                "SELECT * FROM AUTH_ACCESS_ROLES WHERE companyID = @companyID AND userID = @userID",
                new { companyID = CompanyID, userID = userID })
                .FirstOrDefault();
            if (result != null)
            {
                result.Agents = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetListWithEmpty();
                return result;
            }
            else
                return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string userID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AUTH_ACCESS_ROLES WHERE companyID = @companyID AND userID = @userID",
                new { companyID = CompanyID, userID = userID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO AUTH_ACCESS_ROLES (companyID,userID,canOffers,canOffersSignCommercial,canOffersSignTech,canOrders,canOrdersSignCommercial,canOrdersSignTech,canDDT,canInvoices,canAccounting,canCompany,canRDA,canApproveRDA,canTransformRDA,canPurchaseOrders,canPOSignCommercial,canPOSignManagement,canPOSend,canTransformOffers,canFeasibility,canAssets,canStore,canTreasury,canProduction,canTables,canRegistries,canStats,canAccountingAssets,canCRMActivities, crmRole, canCustomerRating, agentID) OUTPUT INSERTED.rv VALUES(@companyID,@userID,@canOffers,@canOffersSignCommercial,@canOffersSignTech,@canOrders,@canOrdersSignCommercial,@canOrdersSignTech,@canDDT,@canInvoices,@canAccounting,@canCompany,@canRDA,@canApproveRDA,@canTransformRDA,@canPurchaseOrders,@canPOSignCommercial,@canPOSignManagement,@canPOSend,@canTransformOffers,@canFeasibility,@canAssets,@canStore,@canTreasury,@canProduction,@canTables,@canRegistries,@canStats,@canAccountingAssets,@canCRMActivities,@crmRole,@canCustomerRating, @agentID)";
    public string UPDATE_QUERY => "UPDATE AUTH_ACCESS_ROLES SET companyID = @companyID,userID = @userID,canOffers = @canOffers,canOffersSignCommercial = @canOffersSignCommercial,canOffersSignTech = @canOffersSignTech,canOrders = @canOrders,canOrdersSignCommercial = @canOrdersSignCommercial,canOrdersSignTech = @canOrdersSignTech,canDDT = @canDDT,canInvoices = @canInvoices,canAccounting = @canAccounting,canCompany = @canCompany,canRDA = @canRDA,canApproveRDA = @canApproveRDA,canTransformRDA = @canTransformRDA,canPurchaseOrders = @canPurchaseOrders,canPOSignCommercial = @canPOSignCommercial,canPOSignManagement = @canPOSignManagement,canPOSend = @canPOSend,canTransformOffers = @canTransformOffers,canFeasibility = @canFeasibility,canAssets = @canAssets,canStore = @canStore,canTreasury = @canTreasury,canProduction = @canProduction,canTables = @canTables,canRegistries = @canRegistries,canStats = @canStats,canAccountingAssets = @canAccountingAssets,canCRMActivities = @canCRMActivities, crmRole = @crmRole, canCustomerRating = @canCustomerRating, agentID = @agentID OUTPUT INSERTED.rv WHERE companyID = @companyID AND userID = @userID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM AUTH_ACCESS_ROLES OUTPUT DELETED.rv WHERE companyID = @companyID AND userID = @userID AND rv = @rv";
    public bool Insert(AUTH_ACCESS_ROLES Model)
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

    public bool Update(AUTH_ACCESS_ROLES Model)
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

    public bool Delete(AUTH_ACCESS_ROLES Model)
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

    public string? Validate(AUTH_ACCESS_ROLES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.userID) && IsInsert && !Exists(Model.companyID, Model.userID)) || !IsInsert)
            {
                return null;
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

public class AUTH_ACCESS_ROLESUfpRepository : RepositoryBase, IAUTH_ACCESS_ROLESRepository
{
    public AUTH_ACCESS_ROLESUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public AUTH_ACCESS_ROLES? Get(string CompanyID, string userID)
    {
        try
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Query<AUTH_ACCESS_ROLES>(
                    "SELECT * FROM AUTH_ACCESS_ROLES WHERE companyID = @companyID AND userID = @userID",
                    new { companyID = CompanyID, userID = userID })
                    .FirstOrDefault();
                if (result != null)
                {
                    result.Agents = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetListWithEmpty();
                    return result;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string userID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AUTH_ACCESS_ROLES WHERE companyID = @companyID AND userID = @userID",
                new { companyID = CompanyID, userID = userID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO AUTH_ACCESS_ROLES (companyID,userID,canOffers,canOffersSignCommercial,canOffersSignTech,canOrders,canOrdersSignCommercial,canOrdersSignTech,canDDT,canInvoices,canAccounting,canCompany,canRDA,canApproveRDA,canTransformRDA,canPurchaseOrders,canPOSignCommercial,canPOSignManagement,canPOSend,canTransformOffers,canFeasibility,canAssets,canStore,canTreasury,canProduction,canTables,canRegistries,canStats,canAccountingAssets,canCRMActivities, crmRole, canCustomerRating, agentID) OUTPUT INSERTED.rv VALUES(@companyID,@userID,@canOffers,@canOffersSignCommercial,@canOffersSignTech,@canOrders,@canOrdersSignCommercial,@canOrdersSignTech,@canDDT,@canInvoices,@canAccounting,@canCompany,@canRDA,@canApproveRDA,@canTransformRDA,@canPurchaseOrders,@canPOSignCommercial,@canPOSignManagement,@canPOSend,@canTransformOffers,@canFeasibility,@canAssets,@canStore,@canTreasury,@canProduction,@canTables,@canRegistries,@canStats,@canAccountingAssets,@canCRMActivities,@crmRole,@canCustomerRating, @agentID)";
    public string UPDATE_QUERY => "UPDATE AUTH_ACCESS_ROLES SET companyID = @companyID,userID = @userID,canOffers = @canOffers,canOffersSignCommercial = @canOffersSignCommercial,canOffersSignTech = @canOffersSignTech,canOrders = @canOrders,canOrdersSignCommercial = @canOrdersSignCommercial,canOrdersSignTech = @canOrdersSignTech,canDDT = @canDDT,canInvoices = @canInvoices,canAccounting = @canAccounting,canCompany = @canCompany,canRDA = @canRDA,canApproveRDA = @canApproveRDA,canTransformRDA = @canTransformRDA,canPurchaseOrders = @canPurchaseOrders,canPOSignCommercial = @canPOSignCommercial,canPOSignManagement = @canPOSignManagement,canPOSend = @canPOSend,canTransformOffers = @canTransformOffers,canFeasibility = @canFeasibility,canAssets = @canAssets,canStore = @canStore,canTreasury = @canTreasury,canProduction = @canProduction,canTables = @canTables,canRegistries = @canRegistries,canStats = @canStats,canAccountingAssets = @canAccountingAssets,canCRMActivities = @canCRMActivities, crmRole = @crmRole, canCustomerRating = @canCustomerRating, agentID = @agentID OUTPUT INSERTED.rv WHERE companyID = @companyID AND userID = @userID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM AUTH_ACCESS_ROLES OUTPUT DELETED.rv WHERE companyID = @companyID AND userID = @userID AND rv = @rv";
    public bool Insert(AUTH_ACCESS_ROLES Model)
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

    public bool Update(AUTH_ACCESS_ROLES Model)
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

    public bool Delete(AUTH_ACCESS_ROLES Model)
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

    public string? Validate(AUTH_ACCESS_ROLES Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.userID) && IsInsert && !Exists(Model.companyID, Model.userID)) || !IsInsert)
            {
                return null;
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