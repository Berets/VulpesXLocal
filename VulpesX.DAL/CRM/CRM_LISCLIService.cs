using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Models.Models;

namespace VulpesX.DAL.CRM;

public interface ICRM_LISCLIRepository
{
    ObservableCollection<CRM_LISCLI>? GetList(string CompanyID, int? CustomerID, int? RecipientID, string StatusID);

    CRM_LISCLI? Get(long id, string CompanyID);

    GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int CustomerID, int? RecipientID, DateTime ReferenceDate, decimal Quantity, string UM);

    GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int CustomerID, DateTime ReferenceDate, string UM);

    bool CopyFrom(string CompanyID, int SourceCustomerID, int TargetCustomerID, string UserID);

    CRM_LISCLI? Exists(string CompanyID, string ProductID, int CustomerID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity);

    bool Insert(CRM_LISCLI Model);

    bool Update(CRM_LISCLI Model);

    bool Delete(CRM_LISCLI Model);

    string? Validate(CRM_LISCLI Model, bool IsInsert);
}

public class CRM_LISCLIRepository : RepositoryBase, ICRM_LISCLIRepository
{
    private readonly Itab_articolo_unitaRepository _tab_articolo_unitaRepository;
    public CRM_LISCLIRepository(IConnectionFactory factory, Itab_articolo_unitaRepository Itab_articolo_unitaRepository) : base(factory)
    {
        _tab_articolo_unitaRepository = Itab_articolo_unitaRepository;
    }

    public ObservableCollection<CRM_LISCLI>? GetList(string CompanyID, int? CustomerID, int? RecipientID, string StatusID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<CRM_LISCLI, tab_articolo, ABE, CRM_LISCLI>(
                $@"SELECT l.*, p.SocietaID, p.ID, p.Descrizione, a.abecod, a.abers1, a.abers2 FROM CRM_LISCLI AS l
                        INNER JOIN ABE AS a ON a.abecod = l.customerID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = l.companyID AND p.ID = l.productID
                        WHERE l.companyID = @cid AND l.canceled IS NULL {(CustomerID.HasValue ? " AND customerID = @cusid " : null)}
                                {(RecipientID.HasValue ? " AND recipientID = @recid " : null)}
                                {(StatusID == "*" ? null : StatusID == "V" ? " AND CONVERT(date, toDate) >= CONVERT(date, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time')" : " AND CONVERT(date, toDate) < CONVERT(date, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time')")}
                        ORDER BY added DESC",
                (lis, prd, abe) => { lis.ProductDescription = prd.FullDescriptionSearchable; lis.Customer = abe; return lis; },
                new { cid = CompanyID, cusid = CustomerID, recid = RecipientID }, splitOn: "SocietaID,abecod");

            return new ObservableCollection<CRM_LISCLI>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public CRM_LISCLI? Get(long id, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var umsCache = _tab_articolo_unitaRepository.GetSimpleList(CompanyID);
            return connection.Query<CRM_LISCLI, tab_articolo, tab_articolo_unita, ABE, CRM_LISCLI>(
                @"SELECT l.*, p.SocietaID, p.ID, p.Descrizione, p.UnitaID, u.SocietaID, u.ID, u.Descrizione, a.abecod, a.abers1, a.abers2 FROM CRM_LISCLI AS l
                        INNER JOIN ABE AS a ON a.abecod = l.customerID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = l.companyID AND p.ID = l.productID
                        INNER JOIN tab_articolo_unita AS u ON u.SocietaID = l.companyID AND u.ID = p.UnitaID
                        WHERE l.id = @id AND l.canceled IS NULL",
                (lis, prd, um, abe) =>
                {
                    lis.UMsCache = umsCache;
                    lis.Product = prd;
                    lis.Customer = abe;
                    return lis;
                },
                new { id }, splitOn: "SocietaID,SocietaID,abecod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int CustomerID, int? RecipientID, DateTime ReferenceDate, decimal Quantity, string UM)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<GenericPriceInfo>(
                $@"SELECT TOP(1) l.price AS Price, l.discount1 AS Discount1, l.discount2 AS Discount2, l.discount3 AS Discount3, l.discountType1 AS DiscountType1, l.discountType2 AS DiscountType2, l.discountType3 AS DiscountType3, l.surcharge AS Surcharge, l.surchargeType AS SurchargeType, l.description AS Note, (TRIM(customerProductID) + ' ' + TRIM(customerProductDescription)) AS LastCustomerDescription FROM CRM_LISCLI AS l
                        WHERE l.companyID = @companyID AND l.productID = @productID AND l.customerID = @customerID AND unit_id=@um AND {(RecipientID.HasValue && RecipientID.Value > 0 ? "(l.recipientID = @recipientID OR l.recipientID IS NULL) AND " : null)}
                        CAST(l.fromDate AS date) <= @refDate AND CAST(l.toDate AS date) >= @refDate AND l.fromQuantity <= @refQuantity AND l.toQuantity >= @refQuantity AND canceled IS NULL 
                        ORDER BY l.fromDate DESC",
                new { companyID = CompanyID, productID = ProductID, customerID = CustomerID, refDate = ReferenceDate.Date, recipientID = RecipientID, refQuantity = Quantity, um = UM })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int CustomerID, DateTime ReferenceDate, string UM)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<GenericPriceInfo>(
                @"SELECT TOP(1) l.price AS Price, l.discount1 AS Discount1, l.discount2 AS Discount2, l.discount3 AS Discount3, l.discountType1 AS DiscountType1, l.discountType2 AS DiscountType2, l.discountType3 AS DiscountType3, l.surcharge AS Surcharge, l.surchargeType AS SurchargeType, l.description AS Note, TRIM(customerProductID) AS CustomerProductID , TRIM(customerProductDescription) AS CustomerProductDescription FROM CRM_LISCLI AS l
                        WHERE l.companyID = @companyID AND l.productID = @productID AND l.customerID = @customerID AND unit_id=@um AND
                        CAST(l.fromDate AS date) <= @refDate AND CAST(l.toDate AS date) >= @refDate AND canceled IS NULL
                        ORDER BY l.fromDate DESC",
                new { companyID = CompanyID, productID = ProductID, customerID = CustomerID, refDate = ReferenceDate.Date, um = UM })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CopyFrom(string CompanyID, int SourceCustomerID, int TargetCustomerID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            foreach (var item in connection.Query<CRM_LISCLI>(
                @"SELECT * FROM CRM_LISCLI
                        WHERE companyID = @cid AND customerID = @cusid AND canceled IS NULL AND recipientID IS NULL",
                new { cid = CompanyID, cusid = SourceCustomerID }))
            {
                Insert(new CRM_LISCLI()
                {
                    companyID = item.companyID,
                    productID = item.productID,
                    customerID = TargetCustomerID,
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

    public CRM_LISCLI? Exists(string CompanyID, string ProductID, int CustomerID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<CRM_LISCLI>(
                @"SELECT * FROM CRM_LISCLI 
                        WHERE companyID = @cid AND productID = @pid AND customerID = @cusid AND 
		                ((fromDate <= @fromDate AND toDate >= @toDate) OR (fromDate >= @fromDate AND toDate <= @toDate) OR 
			            (fromDate >= @fromDate AND fromDate <= @toDate) OR (toDate >= @fromDate AND toDate <= @toDate)) AND
			            ((fromQuantity <= @fromQuantity AND toQuantity >= @toQuantity) OR (fromQuantity >= @fromQuantity AND toQuantity <= @toQuantity) OR 
            			(fromQuantity >= @fromQuantity AND fromQuantity <= @toQuantity) OR (toQuantity >= @fromQuantity AND toQuantity <= @toQuantity)) AND canceled IS NULL
                        ORDER BY added DESC",
                new { cid = CompanyID, pid = ProductID, cusid = CustomerID, fromDate = FromDate, toDate = ToDate, fromQuantity = FromQuantity, toQuantity = ToQuantity }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new CRM_LISCLI { companyID = CompanyID, productID = ProductID };
        }
    }

    #region CRUD
    // autonumber no id!!!
    public readonly string INSERT_QUERY = "INSERT INTO CRM_LISCLI (customerID,companyID,productID,fromDate,toDate,fromQuantity,toQuantity,price,description,discount1,discount2,discount3,discountType1,discountType2,discountType3,surcharge,surchargeType,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,customerProductID,customerProductDescription,recipientID,unit_id) OUTPUT INSERTED.rv VALUES(@customerID,@companyID,@productID,@fromDate,@toDate,@fromQuantity,@toQuantity,@price,@description,@discount1,@discount2,@discount3,@discountType1,@discountType2,@discountType3,@surcharge,@surchargeType,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@customerProductID,@customerProductDescription,@recipientID,@unit_id)";
    public readonly string UPDATE_QUERY = "UPDATE CRM_LISCLI SET customerID = @customerID,companyID = @companyID,productID = @productID,fromDate = @fromDate,toDate = @toDate,fromQuantity = @fromQuantity,toQuantity = @toQuantity,price = @price,description = @description,discount1 = @discount1,discount2 = @discount2,discount3 = @discount3,discountType1 = @discountType1,discountType2 = @discountType2,discountType3 = @discountType3,surcharge = @surcharge,surchargeType = @surchargeType,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,customerProductID = @customerProductID,customerProductDescription = @customerProductDescription,recipientID = @recipientID,unit_id = @unit_id OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM CRM_LISCLI OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";

    public bool Insert(CRM_LISCLI Model)
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

    public bool Update(CRM_LISCLI Model)
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

    public bool Delete(CRM_LISCLI Model)
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

    public string? Validate(CRM_LISCLI model, bool isInsert)
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


            bool isDiscountValid =
                AccountingHelper.IsDiscountPairValid(model.discount1, model.discountType1) &&
                AccountingHelper.IsDiscountPairValid(model.discount2, model.discountType2) &&
                AccountingHelper.IsDiscountPairValid(model.discount3, model.discountType3) &&
                AccountingHelper.IsDiscountPairValid(model.surcharge, model.surchargeType);

            if (!isDiscountValid)
                return "Se si specifica un valore per uno sconto o per la maggiorazione è necessario specificarne anche il tipo, viceversa il tipo non deve essere valorizzato";


            var existing = Exists(
                model.companyID, model.productID, model.customerID,
                model.fromDate, model.toDate, model.fromQuantity, model.toQuantity
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