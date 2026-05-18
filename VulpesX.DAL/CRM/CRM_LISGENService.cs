using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Models.Models;

namespace VulpesX.DAL.CRM;

public interface ICRM_LISGENRepository
{
    ObservableCollection<CRM_LISGEN>? GetList(string CompanyID, string StatusID);

    CRM_LISGEN? Get(long id, string CompanyID);

    GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, DateTime ReferenceDate, decimal Quantity, string UM);

    bool CopyFrom(string CompanyID, string SourceProductID, string TargetProductID, string UserID);

    CRM_LISGEN? Exists(string CompanyID, string ProductID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity);

    bool Insert(CRM_LISGEN Model);

    bool Update(CRM_LISGEN Model);

    bool Delete(CRM_LISGEN Model);

    string? Validate(CRM_LISGEN Model, bool IsInsert);
}

public class CRM_LISGENRepository : RepositoryBase, ICRM_LISGENRepository
{
    private readonly Itab_articolo_unitaRepository _tab_articolo_unitaRepository;
    public CRM_LISGENRepository(IConnectionFactory factory, Itab_articolo_unitaRepository Itab_articolo_unitaRepository) : base(factory)
    {
        _tab_articolo_unitaRepository = Itab_articolo_unitaRepository;
    }
    public ObservableCollection<CRM_LISGEN>? GetList(string CompanyID, string StatusID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<CRM_LISGEN, tab_articolo, CRM_LISGEN>(
                $@"SELECT l.*, p.SocietaID, p.ID, p.Descrizione FROM CRM_LISGEN AS l
                        INNER JOIN tab_articolo AS p ON p.SocietaID = l.companyID AND p.ID = l.productID
                        WHERE l.companyID = @cid AND l.canceled IS NULL
                        {(StatusID == "*" ? null : StatusID == "V" ? " AND CONVERT(date, toDate) >= CONVERT(date, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time')" : " AND CONVERT(date, toDate) < CONVERT(date, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time')")}
                        ORDER BY added DESC",
                (lis, prd) => { lis.ProductDescription = prd.FullDescriptionSearchable; return lis; },
                new { cid = CompanyID },
                splitOn: "SocietaID");

            return new ObservableCollection<CRM_LISGEN>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CRM_LISGEN? Get(long id, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var umsCache = _tab_articolo_unitaRepository.GetSimpleList(CompanyID);
            return connection.Query<CRM_LISGEN, tab_articolo, tab_articolo_unita, CRM_LISGEN>(
                @"SELECT l.*, p.SocietaID, p.ID, p.Descrizione, p.UnitaID, u.SocietaID, u.ID, u.Descrizione FROM CRM_LISGEN AS l
                        INNER JOIN tab_articolo AS p ON p.SocietaID = l.companyID AND p.ID = l.productID
                        INNER JOIN tab_articolo_unita AS u ON u.SocietaID = l.companyID AND u.ID = p.UnitaID
                        WHERE l.id = @id AND l.canceled IS NULL",
                (lis, prd, um) =>
                {
                    lis.UMsCache = umsCache;
                    lis.Product = prd;
                    return lis;
                },
                new { id }, splitOn: "SocietaID,SocietaID")
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, DateTime ReferenceDate, decimal Quantity, string UM)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<GenericPriceInfo>(
                @"SELECT TOP(1) l.price AS Price, l.discount1 AS Discount1, l.discount2 AS Discount2, l.discount3 AS Discount3, l.discountType1 AS DiscountType1, l.discountType2 AS DiscountType2, l.discountType3 AS DiscountType3, l.surcharge AS Surcharge, l.surchargeType AS SurchargeType, l.description AS Note FROM CRM_LISGEN AS l
                        WHERE l.companyID = @companyID AND l.productID = @productID AND unit_id=@um AND
                        CAST(l.fromDate AS date) <= @refDate AND CAST(l.toDate AS date) >= @refDate AND l.fromQuantity <= @refQuantity AND l.toQuantity >= @refQuantity
                        ORDER BY l.fromDate DESC",
                new { companyID = CompanyID, productID = ProductID, refDate = ReferenceDate, refQuantity = Quantity, um = UM })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CopyFrom(string CompanyID, string SourceProductID, string TargetProductID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            foreach (var item in connection.Query<CRM_LISGEN>(
                @"SELECT * FROM CRM_LISGEN
                        WHERE companyID = @cid AND productID = @pid AND canceled IS NULL",
                new { cid = CompanyID, pid = SourceProductID }))
            {
                Insert(new CRM_LISGEN()
                {
                    companyID = item.companyID,
                    productID = TargetProductID,
                    fromDate = item.fromDate,
                    toDate = item.toDate,
                    fromQuantity = item.fromQuantity,
                    toQuantity = item.toQuantity,
                    price = item.price,
                    discount1 = item.discount1,
                    discount2 = item.discount2,
                    discount3 = item.discount3,
                    discountType1 = item.discountType1,
                    discountType2 = item.discountType2,
                    discountType3 = item.discountType3,
                    description = item.description,
                    surcharge = item.surcharge,
                    surchargeType = item.surchargeType,
                    addedUserID = UserID
                });
            }

            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public CRM_LISGEN? Exists(string CompanyID, string ProductID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                connection.Open();
                return connection.Query<CRM_LISGEN>(
                    @"SELECT * FROM CRM_LISGEN 
                        WHERE companyID = @cid AND productID = @pid AND 
		                ((fromDate <= @fromDate AND toDate >= @toDate) OR (fromDate >= @fromDate AND toDate <= @toDate) OR 
			            (fromDate >= @fromDate AND fromDate <= @toDate) OR (toDate >= @fromDate AND toDate <= @toDate)) AND
			            ((fromQuantity <= @fromQuantity AND toQuantity >= @toQuantity) OR (fromQuantity >= @fromQuantity AND toQuantity <= @toQuantity) OR 
            			(fromQuantity >= @fromQuantity AND fromQuantity <= @toQuantity) OR (toQuantity >= @fromQuantity AND toQuantity <= @toQuantity)) AND canceled IS NULL
                        ORDER BY added DESC",
                    new { cid = CompanyID, pid = ProductID, fromDate = FromDate, toDate = ToDate, fromQuantity = FromQuantity, toQuantity = ToQuantity }).FirstOrDefault();

            }
            else
            {
                ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                return new CRM_LISGEN { companyID = CompanyID, productID = ProductID };
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new CRM_LISGEN { companyID = CompanyID, productID = ProductID };
        }
    }

    #region CRUD
    // autonumber no id!!!
    public readonly string INSERT_QUERY = "INSERT INTO CRM_LISGEN (companyID,productID,fromDate,toDate,fromQuantity,toQuantity,price,description,discount1,discount2,discount3,discountType1,discountType2,discountType3,surcharge,surchargeType,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,unit_id) OUTPUT INSERTED.rv VALUES(@companyID,@productID,@fromDate,@toDate,@fromQuantity,@toQuantity,@price,@description,@discount1,@discount2,@discount3,@discountType1,@discountType2,@discountType3,@surcharge,@surchargeType,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@unit_id)";
    public readonly string UPDATE_QUERY = "UPDATE CRM_LISGEN SET companyID = @companyID,productID = @productID,fromDate = @fromDate,toDate = @toDate,fromQuantity = @fromQuantity,toQuantity = @toQuantity,price = @price,description = @description,discount1 = @discount1,discount2 = @discount2,discount3 = @discount3,discountType1 = @discountType1,discountType2 = @discountType2,discountType3 = @discountType3,surcharge = @surcharge,surchargeType = @surchargeType,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,unit_id = @unit_id OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM CRM_LISGEN OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public bool Insert(CRM_LISGEN Model)
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

    public bool Update(CRM_LISGEN Model)
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

    public bool Delete(CRM_LISGEN Model)
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

    public string? Validate(CRM_LISGEN model, bool isInsert)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.productID))
                return "Il prodotto è obbligatorio";

            if (model.fromDate >= model.toDate)
                return "La data di fine validità deve essere maggiore della data di inizio";

            if (model.fromQuantity <= 0 || model.toQuantity <= 0)
                return "Le quantità di riferimento sono obbligatorie";

            if (model.fromQuantity >= model.toQuantity)
                return "La quantità di fine validità deve essere maggiore di quella di inizio";

            if (model.price <= 0)
                return "Il prezzo è obbligatorio";


            bool validDiscounts =
                AccountingHelper.IsDiscountPairValid(model.discount1, model.discountType1) &&
                AccountingHelper.IsDiscountPairValid(model.discount2, model.discountType2) &&
                AccountingHelper.IsDiscountPairValid(model.discount3, model.discountType3) &&
                AccountingHelper.IsDiscountPairValid(model.surcharge, model.surchargeType);

            if (!validDiscounts)
                return "Se si specifica un valore per uno sconto o per la maggiorazione è necessario specificarne anche il tipo; viceversa, il tipo non deve essere valorizzato";


            var existing = Exists(
                model.companyID,
                model.productID,
                model.fromDate,
                model.toDate,
                model.fromQuantity,
                model.toQuantity
            );

            if (isInsert)
            {
                if (existing == null)
                    return null;

                bool proceed = ConfirmHandler.Confirm(
                    $"Attenzione esiste già un listino che si sovrappone a questo:\n\n" +
                    $"Prodotto: {model.productID} dal {model.fromDate:dd/MM/yyyy} al {model.toDate:dd/MM/yyyy}\n" +
                    $"Intervallo {model.fromQuantity:N6}-{model.toQuantity:N6}\n\n" +
                    "Proseguire comunque ?"
                );

                return proceed ? null : "Il codice inserito è già in uso o non è valido";
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return ex.Message;
        }
    }
    #endregion
}