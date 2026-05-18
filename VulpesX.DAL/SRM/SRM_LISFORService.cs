
using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.Models.Models;
using VulpesX.Models.Ufp;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.SRM;


public interface ISRM_LISFORRepository
{
    ObservableCollection<SRM_LISFOR>? GetList(string CompanyID, int? SupplierID, string StatusID);

    SRM_LISFOR? Get(long id, string CompanyID);

    GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int SupplierID, DateTime ReferenceDate, decimal Quantity, string? UM);

    GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int SupplierID, DateTime ReferenceDate, string? UM);

    bool CopyFrom(string CompanyID, int SourceSupplierID, int TargetSupplierID, string UserID);

    SRM_LISFOR? Exists(string CompanyID, string ProductID, int SupplierID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity);

    #region CRUD
    // autonumber no id!!!
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(SRM_LISFOR Model);

    bool Update(SRM_LISFOR Model);

    bool Delete(SRM_LISFOR Model);

    string? Validate(SRM_LISFOR Model, bool IsInsert);
    #endregion

    ObservableCollection<LISFOR0F>? GetListLISFOR0FActive(string CompanyID, string ProductID, DateTime Date);
}

public class SRM_LISFORRepository : RepositoryBase, ISRM_LISFORRepository
{
    private readonly Itab_articolo_unitaRepository _tab_articolo_unitaRepository;
    public SRM_LISFORRepository(IConnectionFactory factory, Itab_articolo_unitaRepository Itab_articolo_unitaRepository) : base(factory)
    {
        _tab_articolo_unitaRepository = Itab_articolo_unitaRepository;
    }

    public ObservableCollection<SRM_LISFOR>? GetList(string CompanyID, int? SupplierID, string StatusID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<SRM_LISFOR, tab_articolo, ABE, SRM_LISFOR>(
                $@"SELECT l.*, p.SocietaID, p.ID, p.Descrizione, a.abecod, a.abers1, a.abers2 FROM SRM_LISFOR AS l
                        INNER JOIN ABE AS a ON a.abecod = l.supplierID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = l.companyID AND p.ID = l.productID
                        WHERE l.companyID = @cid AND l.canceled IS NULL {(SupplierID.HasValue ? " AND supplierID = @supid " : null)}
                        {(StatusID == "*" ? null : (StatusID == "V" ? " AND CONVERT(date, toDate) >= CONVERT(date, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time')" : " AND CONVERT(date, toDate) < CONVERT(date, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time')"))}
                        ORDER BY added DESC",
                (lis, prd, abe) => { lis.ProductDescription = prd.FullDescriptionSearchable; lis.Supplier = abe; return lis; },
                new { cid = CompanyID, supid = SupplierID }, splitOn: "SocietaID,abecod");

            return new ObservableCollection<SRM_LISFOR>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SRM_LISFOR? Get(long id, string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var umsCache = _tab_articolo_unitaRepository.GetSimpleList(CompanyID);
            return connection.Query<SRM_LISFOR, tab_articolo, tab_articolo_unita, ABE, SRM_LISFOR>(
                @"SELECT l.*, p.SocietaID, p.ID, p.Descrizione, p.UnitaID, p.UnitaIDAlt, u.SocietaID, u.ID, u.Descrizione, a.abecod, a.abers1, a.abers2 FROM SRM_LISFOR AS l
                        INNER JOIN ABE AS a ON a.abecod = l.supplierID
                        INNER JOIN tab_articolo AS p ON p.SocietaID = l.companyID AND p.ID = l.productID
                        INNER JOIN tab_articolo_unita AS u ON u.SocietaID = l.companyID AND u.ID = p.UnitaID
                        WHERE l.id = @id AND l.canceled IS NULL",
                (lis, prd, um, abe) => { lis.UMsCache = umsCache; lis.Product = prd; lis.Supplier = abe; return lis; },
                new { id = id }, splitOn: "SocietaID,SocietaID,abecod")
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int SupplierID, DateTime ReferenceDate, decimal Quantity, string? UM)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<GenericPriceInfo>(
                @"SELECT TOP(1) l.price AS Price, l.discount1 AS Discount1, l.discount2 AS Discount2, l.discount3 AS Discount3, l.discountType1 AS DiscountType1, l.discountType2 AS DiscountType2, l.discountType3 AS DiscountType3, l.surcharge AS Surcharge, l.surchargeType AS SurchargeType, l.description AS Note, (TRIM(supplierProductID) + ' ' + TRIM(supplierProductDescription)) AS LastSupplierDescription FROM SRM_LISFOR AS l
                        WHERE l.companyID = @companyID AND l.productID = @productID AND l.supplierID = @supplierID AND unit_id = @um AND
                        CAST(l.fromDate AS date) <= @refDate AND CAST(l.toDate AS date) >= @refDate AND l.fromQuantity <= @refQuantity AND l.toQuantity >= @refQuantity AND canceled IS NULL 
                        ORDER BY l.fromDate DESC",
                new { companyID = CompanyID, productID = ProductID, supplierID = SupplierID, refDate = ReferenceDate.Date, refQuantity = Quantity, um = UM })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int SupplierID, DateTime ReferenceDate, string? UM)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<GenericPriceInfo>(
                @"SELECT TOP(1) l.price AS Price, l.discount1 AS Discount1, l.discount2 AS Discount2, l.discount3 AS Discount3, l.discountType1 AS DiscountType1, l.discountType2 AS DiscountType2, l.discountType3 AS DiscountType3, l.surcharge AS Surcharge, l.surchargeType AS SurchargeType, l.description AS Note, TRIM(supplierProductID) AS SupplierProductID , TRIM(supplierProductDescription) AS SupplierProductDescription FROM SRM_LISFOR AS l
                        WHERE l.companyID = @companyID AND l.productID = @productID AND l.supplierID = @supplierID AND unit_id = @um AND
                        CAST(l.fromDate AS date) <= @refDate AND CAST(l.toDate AS date) >= @refDate AND canceled IS NULL
                        ORDER BY l.fromDate DESC",
                new { companyID = CompanyID, productID = ProductID, supplierID = SupplierID, refDate = ReferenceDate.Date, um = UM })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CopyFrom(string CompanyID, int SourceSupplierID, int TargetSupplierID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            foreach (var item in connection.Query<SRM_LISFOR>(
                @"SELECT * FROM SRM_LISFOR
                        WHERE companyID = @cid AND supplierID = @supid AND canceled IS NULL",
                new { cid = CompanyID, supid = SourceSupplierID }))
            {
                Insert(new SRM_LISFOR()
                {
                    companyID = item.companyID,
                    productID = item.productID,
                    supplierID = TargetSupplierID,
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

    public SRM_LISFOR? Exists(string CompanyID, string ProductID, int SupplierID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SRM_LISFOR>(
                @"SELECT * FROM SRM_LISFOR 
                        WHERE companyID = @cid AND productID = @pid AND supplierID = @supid AND 
		                ((fromDate <= @fromDate AND toDate >= @toDate) OR (fromDate >= @fromDate AND toDate <= @toDate) OR 
			            (fromDate >= @fromDate AND fromDate <= @toDate) OR (toDate >= @fromDate AND toDate <= @toDate)) AND
			            ((fromQuantity <= @fromQuantity AND toQuantity >= @toQuantity) OR (fromQuantity >= @fromQuantity AND toQuantity <= @toQuantity) OR 
            			(fromQuantity >= @fromQuantity AND fromQuantity <= @toQuantity) OR (toQuantity >= @fromQuantity AND toQuantity <= @toQuantity)) AND canceled IS NULL
                        ORDER BY added DESC",
                new { cid = CompanyID, pid = ProductID, supid = SupplierID, fromDate = FromDate, toDate = ToDate, fromQuantity = FromQuantity, toQuantity = ToQuantity }).FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new SRM_LISFOR { companyID = CompanyID, productID = ProductID };
        }
    }

    #region CRUD
    // autonumber no id!!!
    public string INSERT_QUERY => "INSERT INTO SRM_LISFOR (supplierID,companyID,productID,fromDate,toDate,fromQuantity,toQuantity,price,description,supplierProductID,supplierProductDescription,discount1,discount2,discount3,discountType1,discountType2,discountType3,surcharge,surchargeType,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,unit_id) OUTPUT INSERTED.rv VALUES(@supplierID,@companyID,@productID,@fromDate,@toDate,@fromQuantity,@toQuantity,@price,@description,@supplierProductID,@supplierProductDescription,@discount1,@discount2,@discount3,@discountType1,@discountType2,@discountType3,@surcharge,@surchargeType,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@unit_id)";
    public string UPDATE_QUERY => "UPDATE SRM_LISFOR SET supplierID = @supplierID,companyID = @companyID,productID = @productID,fromDate = @fromDate,toDate = @toDate,fromQuantity = @fromQuantity,toQuantity = @toQuantity,price = @price,description = @description,supplierProductID = @supplierProductID,supplierProductDescription = @supplierProductDescription,discount1 = @discount1,discount2 = @discount2,discount3 = @discount3,discountType1 = @discountType1,discountType2 = @discountType2,discountType3 = @discountType3,surcharge = @surcharge,surchargeType = @surchargeType,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,unit_id = @unit_id OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM SRM_LISFOR OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public bool Insert(SRM_LISFOR Model)
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

    public bool Update(SRM_LISFOR Model)
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

    public bool Delete(SRM_LISFOR Model)
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

    public string? Validate(SRM_LISFOR Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.productID))
            {

                if (Model.fromDate < Model.toDate)
                {
                    if (Model.fromQuantity > 0 && Model.toQuantity > 0)
                    {
                        if (Model.fromQuantity < Model.toQuantity)
                        {
                            if (Model.price > 0)
                            {
                                if (((Model.discount1.HasValue && Model.discount1.Value > 0 && !string.IsNullOrWhiteSpace(Model.discountType1)) ||
                                    (!Model.discount1.HasValue && string.IsNullOrWhiteSpace(Model.discountType1))) &&
                                    ((Model.discount2.HasValue && Model.discount2.Value > 0 && !string.IsNullOrWhiteSpace(Model.discountType2)) ||
                                    (!Model.discount2.HasValue && string.IsNullOrWhiteSpace(Model.discountType2))) &&
                                    ((Model.discount3.HasValue && Model.discount3.Value > 0 && !string.IsNullOrWhiteSpace(Model.discountType3)) ||
                                    (!Model.discount3.HasValue && string.IsNullOrWhiteSpace(Model.discountType3))) &&
                                    ((Model.surcharge.HasValue && Model.surcharge.Value > 0 && !string.IsNullOrWhiteSpace(Model.surchargeType)) ||
                                    (!Model.surcharge.HasValue && string.IsNullOrWhiteSpace(Model.surchargeType))))
                                {
                                    var existing = Exists(Model.companyID, Model.productID, Model.supplierID, Model.fromDate, Model.toDate, Model.fromQuantity, Model.toQuantity);
                                    if ((IsInsert && existing == null) ||
                                        (IsInsert && existing != null && ConfirmHandler.Confirm($"Attenzione esiste già un listino che si sovrappone a questo:\n\nProdotto: {Model.productID} dal {Model.fromDate.ToShortDateString()} al {Model.toDate.ToShortDateString()} Intervallo {Model.fromQuantity.ToString("N6")}-{Model.toQuantity.ToString("N6")} \n\nProseguire comunque ?")) ||
                                        !IsInsert)
                                    {
                                        return null;
                                    }
                                    else
                                    { return "Listino gia' presente per il periodo e le quantita' indicate"; }
                                }
                                else
                                { return "Se si specifica un valore per uno sconto o per la maggiorazione è necessario specificarne anche il tipo, viceversa il tipo non deve essere valorizzato"; }
                            }
                            else
                            { return "Il prezzo è obbligatorio"; }
                        }
                        else
                        { return "La quantità di fine validità deve essere maggiore di quella di inizio"; }
                    }
                    else
                    { return "Le quantità di riferimento sono obbligatorie"; }
                }
                else
                { return "La data di fine validità deve essere maggiore della data di inizio"; }

            }
            else
            { return "Il prodotto è obbligatorio"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

    public ObservableCollection<LISFOR0F>? GetListLISFOR0FActive(string CompanyID, string ProductID, DateTime Date)
    {
        throw new NotImplementedException();
    }
}

public class SRM_LISFORUfpRepository : RepositoryBase, ISRM_LISFORRepository
{
    private readonly Itab_articolo_unitaRepository _tab_articolo_unitaRepository;
    public SRM_LISFORUfpRepository(IConnectionFactory factory, Itab_articolo_unitaRepository Itab_articolo_unitaRepository) : base(factory)
    {
        _tab_articolo_unitaRepository = Itab_articolo_unitaRepository;
    }

    public ObservableCollection<SRM_LISFOR>? GetList(string CompanyID, int? SupplierID, string StatusID)
    {
        throw new NotImplementedException();
    }

    public SRM_LISFOR? Get(long id, string CompanyID)
    {
        throw new NotImplementedException();
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int SupplierID, DateTime ReferenceDate, decimal Quantity, string? UM)
    {
        throw new NotImplementedException();
    }

    public GenericPriceInfo? GetCurrent(string CompanyID, string ProductID, int SupplierID, DateTime ReferenceDate, string? UM)
    {
        throw new NotImplementedException();
    }

    public bool CopyFrom(string CompanyID, int SourceSupplierID, int TargetSupplierID, string UserID)
    {
        throw new NotImplementedException();
    }

    public SRM_LISFOR? Exists(string CompanyID, string ProductID, int SupplierID, DateTime FromDate, DateTime ToDate, decimal FromQuantity, decimal ToQuantity)
    {
        throw new NotImplementedException();
    }

    #region CRUD
    // autonumber no id!!!
    public string INSERT_QUERY => "INSERT INTO SRM_LISFOR (supplierID,companyID,productID,fromDate,toDate,fromQuantity,toQuantity,price,description,supplierProductID,supplierProductDescription,discount1,discount2,discount3,discountType1,discountType2,discountType3,surcharge,surchargeType,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,unit_id) OUTPUT INSERTED.rv VALUES(@supplierID,@companyID,@productID,@fromDate,@toDate,@fromQuantity,@toQuantity,@price,@description,@supplierProductID,@supplierProductDescription,@discount1,@discount2,@discount3,@discountType1,@discountType2,@discountType3,@surcharge,@surchargeType,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@unit_id)";
    public string UPDATE_QUERY => "UPDATE SRM_LISFOR SET supplierID = @supplierID,companyID = @companyID,productID = @productID,fromDate = @fromDate,toDate = @toDate,fromQuantity = @fromQuantity,toQuantity = @toQuantity,price = @price,description = @description,supplierProductID = @supplierProductID,supplierProductDescription = @supplierProductDescription,discount1 = @discount1,discount2 = @discount2,discount3 = @discount3,discountType1 = @discountType1,discountType2 = @discountType2,discountType3 = @discountType3,surcharge = @surcharge,surchargeType = @surchargeType,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,unit_id = @unit_id OUTPUT INSERTED.rv WHERE id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM SRM_LISFOR OUTPUT DELETED.rv WHERE id = @id AND rv = @rv";
    public bool Insert(SRM_LISFOR Model)
    {
        throw new NotImplementedException();
    }

    public bool Update(SRM_LISFOR Model)
    {
        throw new NotImplementedException();
    }

    public bool Delete(SRM_LISFOR Model)
    {
        throw new NotImplementedException();
    }

    public string? Validate(SRM_LISFOR Model, bool IsInsert)
    {
        throw new NotImplementedException();
    }
    #endregion

    public ObservableCollection<LISFOR0F>? GetListLISFOR0FActive(string CompanyID, string ProductID, DateTime Date)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<LISFOR0F, tab_articolo, ABE, LISFOR0F>(
                $@"SELECT l.*, p.artcod, p.artdes, p.artdise, a.abecod, a.abers1, a.abers2 FROM LISFOR0F AS l
                        INNER JOIN ANAG_BASE AS a ON a.abecod = l.LFFORN
                        INNER JOIN ANAG_ARTICOLI AS p ON p.artcod = l.LFCOAR
                        WHERE l.LFSOC = @cid AND l.LFCOAR = @pid AND l.LFLISATT = 'S' AND l.LFFDAT <= @Date
                        ORDER BY LFFDAT DESC",
                (lis, prd, abe) => { lis.ProductDescription = prd.FullDescriptionSearchableUfp; lis.Supplier = abe; return lis; },
                new { cid = CompanyID, pid = ProductID, Date = Date }, splitOn: "artcod,abecod");

            return new ObservableCollection<LISFOR0F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
}

