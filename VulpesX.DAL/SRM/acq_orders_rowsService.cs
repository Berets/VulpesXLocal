using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VulpesX.DAL._ConnectionFactory;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models.Models;

namespace VulpesX.DAL.SRM;


public interface Iacq_orders_rowsRepository
{
    ObservableCollection<acq_orders_rows>? GetListNotClosed(string CompanyID);

    decimal GetInOrder(string company_id, string product_id);

    ObservableCollection<acq_orders_rows>? GetList(string CompanyID, long PurchaseOrderID);

    ObservableCollection<acq_orders_rows>? GetListFull(string CompanyID, long PurchaseOrderID);

    ObservableCollection<acq_orders_rows>? GetToReceiptList(string CompanyID, int SupplierID);

    acq_orders_rows? Get(string company_id, long id, int sequence);

    acq_orders_rows? GetFull(string company_id, long id, int sequence);

    #region Prices
    GenericPriceInfo? GetLastPriceDifferentSupplier(string CompanyID, string ProductID, int SupplierID, long? CurrentOrderID);
    GenericPriceInfo? GetLastPriceSameSupplier(string CompanyID, string ProductID, int SupplierID, long? CurrentOrderID);
    #endregion

    bool Exists(string company_id, long id, int sequence);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(acq_orders_rows Model);

    bool Update(acq_orders_rows Model);

    bool UpdateAll(acq_orders_heads Head, ObservableCollection<acq_orders_rows>? Rows);

    bool Delete(acq_orders_rows Model);

    string? Validate(acq_orders_rows Model, bool IsInsert);

    string? ValidateModel(ObservableCollection<acq_orders_rows>? Rows);
    #endregion
}

public class acq_orders_rowsRepository : RepositoryBase, Iacq_orders_rowsRepository
{
    public acq_orders_rowsRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<acq_orders_rows>? GetListNotClosed(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<acq_orders_rows>(
                @"SELECT * FROM acq_orders_rows
                        WHERE company_id=@cid AND is_closed = 0
                        ORDER BY id,sequence",
                new { cid = CompanyID });

            return new ObservableCollection<acq_orders_rows>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public decimal GetInOrder(string company_id, string product_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            var rows = connection.Query<acq_orders_rows>(
                @"SELECT r.* FROM acq_orders_rows AS r
                        INNER JOIN acq_orders_heads AS h ON h.company_id=r.company_id AND h.id=r.id
                        WHERE r.company_id = @company_id AND r.product_id = @pid AND h.sent IS NOT NULL AND h.closed IS NULL AND h.canceled IS NULL AND r.is_closed = 0",
                new { company_id = company_id, pid = product_id });
            return rows.Sum(sum => ((sum.quantity ?? 0) - (sum.quantity_received ?? 0)));
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public ObservableCollection<acq_orders_rows>? GetList(string CompanyID, long PurchaseOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<acq_orders_rows>(
                @"SELECT * FROM acq_orders_rows
                        WHERE company_id=@cid AND id=@poid",
                new { cid = CompanyID, poid = PurchaseOrderID });

            return new ObservableCollection<acq_orders_rows>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<acq_orders_rows>? GetListFull(string CompanyID, long PurchaseOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            var productTypes = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_tipoRepository>().GetList(CompanyID, false);
            var list = connection.Query<acq_orders_rows>(
                @"SELECT * FROM acq_orders_rows
                        WHERE company_id=@cid AND id=@poid",
                new { cid = CompanyID, poid = PurchaseOrderID });
            var rdaList = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_rdasRepository>().GetList(CompanyID, PurchaseOrderID);
            var jobsList = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_jobsRepository>().GetList(CompanyID, PurchaseOrderID);
            var cuorList = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_customer_ordersRepository>().GetList(CompanyID, PurchaseOrderID);
            foreach (var row in list)
            {
                row.UMsCache = umsCache;
                row.ProductTypes = productTypes;
                row.RDAs = new ObservableCollection<acq_orders_rows_rdas>((rdaList ?? new ObservableCollection<acq_orders_rows_rdas>()).Where(w => w.order_row_id == row.sequence));
                row.Jobs = new ObservableCollection<acq_orders_rows_jobs>((jobsList ?? new ObservableCollection<acq_orders_rows_jobs>()).Where(w => w.order_row_id == row.sequence));
                row.CustomerOrders = new ObservableCollection<acq_orders_rows_customer_orders>((cuorList ?? new ObservableCollection<acq_orders_rows_customer_orders>()).Where(w => w.order_row_id == row.sequence));
            }
            return new ObservableCollection<acq_orders_rows>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<acq_orders_rows>? GetToReceiptList(string CompanyID, int SupplierID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            var list = connection.Query<acq_orders_rows, tab_articolo, acq_orders_rows>(
                $@"SELECT r.*, p.SocietaID, p.ID, p.Descrizione, p.UnitaID FROM acq_orders_rows AS r
                        INNER JOIN acq_orders_heads AS h ON h.company_id = r.company_id AND h.id = r.id
                        INNER JOIN tab_articolo AS p ON p.SocietaID = r.company_id AND p.ID = r.product_id
                        WHERE r.company_id = @cid AND h.supplier_id = @sid AND h.sent IS NOT NULL AND h.canceled IS NULL AND h.closed IS NULL AND r.is_closed = 0
                        ORDER BY r.id ASC, r.sequence ASC",
                (acq, prd) =>
                {
                    acq.UMsCache = umsCache;
                    acq.Product = prd;
                    return acq;
                },
                new { cid = CompanyID, sid = SupplierID }, splitOn: "SocietaID");
            return new ObservableCollection<acq_orders_rows>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_rows? Get(string company_id, long id, int sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(company_id);
            return connection.Query<acq_orders_rows, tab_articolo, acq_orders_rows>(
                @"SELECT r.*, p.* FROM acq_orders_rows AS r
                        INNER JOIN tab_articolo AS p ON p.SocietaID = r.company_id AND p.ID = r.product_id
                        WHERE r.company_id = @company_id AND r.id = @id AND r.sequence = @sequence",
                (acq, prd) =>
                {
                    acq.UMsCache = umsCache;
                    acq.Product = prd;
                    return acq;
                },
                new { company_id = company_id, id = id, sequence = sequence }, splitOn: "ID")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_rows? GetFull(string company_id, long id, int sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(company_id);
            var item = connection.Query<acq_orders_rows, tab_articolo, acq_orders_rows>(
                @"SELECT r.*, p.ID, p.Descrizione, p.UnitaID, p.QuantitaDefault, p.HasLots FROM acq_orders_rows AS r
                        INNER JOIN tab_articolo AS p ON p.SocietaID = r.company_id AND p.ID = r.product_id
                        WHERE r.company_id = @company_id AND r.id = @id AND r.sequence = @sequence",
                (acq, prd) =>
                {
                    acq.UMsCache = umsCache;
                    acq.Product = prd;
                    return acq;
                },
                new { company_id = company_id, id = id, sequence = sequence }, splitOn: "ID")
                .FirstOrDefault();

            if (item != null)
            {
                item.Jobs = new ObservableCollection<acq_orders_rows_jobs>((VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_jobsRepository>().GetList(company_id, id) ?? new ObservableCollection<acq_orders_rows_jobs>()).Where(w => w.order_row_id == sequence));
                item.CustomerOrders = new ObservableCollection<acq_orders_rows_customer_orders>((VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_customer_ordersRepository>().GetList(company_id, id) ?? new ObservableCollection<acq_orders_rows_customer_orders>()).Where(w => w.order_row_id == sequence));
            }

            return item;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Prices
    public GenericPriceInfo? GetLastPriceDifferentSupplier(string CompanyID, string ProductID, int SupplierID, long? CurrentOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<GenericPriceInfo>(
                $@"SELECT TOP(1) d.price AS Price, d.discount AS Discount1, d.discount_type AS DiscountType1, d.note AS Note, CONCAT(TRIM(CONVERT(nvarchar(15), a.abecod)), ' ', a.abers1 , ' ' ,  a.abers2) AS SupplierDescription FROM acq_orders_rows AS d
                        INNER JOIN acq_orders_heads AS m ON m.company_id = d.company_id AND m.id = d.id
                        INNER JOIN ABE AS a ON a.abecod = m.supplier_id
                        WHERE m.canceled IS NULL AND d.product_id = @pid AND d.company_id = @cid AND m.supplier_id <> @supid {(CurrentOrderID.HasValue && CurrentOrderID.Value > 0 ? " AND d.id <> @curr" : null)}
                        ORDER BY m.order_date DESC",
                new { cid = CompanyID, pid = ProductID, supid = SupplierID, curr = CurrentOrderID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public GenericPriceInfo? GetLastPriceSameSupplier(string CompanyID, string ProductID, int SupplierID, long? CurrentOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return connection.Query<GenericPriceInfo>(
                $@"SELECT TOP(1) d.price AS Price, d.discount AS Discount1, d.discount_type AS DiscountType1, d.note AS Note, CONCAT(TRIM(CONVERT(nvarchar(15), a.abecod)), ' ', a.abers1 , ' ' ,  a.abers2) AS SupplierDescription FROM acq_orders_rows AS d
                        INNER JOIN acq_orders_heads AS m ON m.company_id = d.company_id AND m.id = d.id
                        INNER JOIN ABE AS a ON a.abecod = m.supplier_id
                        WHERE m.canceled IS NULL AND d.product_id = @pid AND d.company_id = @cid AND m.supplier_id = @supid {(CurrentOrderID.HasValue && CurrentOrderID.Value > 0 ? " AND d.id <> @curr" : null)}
                        ORDER BY m.order_date DESC",
                new { cid = CompanyID, pid = ProductID, supid = SupplierID, curr = CurrentOrderID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    public bool Exists(string company_id, long id, int sequence)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM acq_orders_rows WHERE company_id = @company_id AND id = @id AND sequence = @sequence",
                new { company_id = company_id, id = id, sequence = sequence }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO acq_orders_rows (company_id,id,sequence,product_id,store_id,quantity,discount,price,delivery_requested,note,discount_type,quantity_received,price_second_um,tax_code,tax_rate,price_type,is_closed,unit_id) OUTPUT INSERTED.rv VALUES(@company_id,@id,@sequence,@product_id,@store_id,@quantity,@discount,@price,@delivery_requested,@note,@discount_type,@quantity_received,@price_second_um,@tax_code,@tax_rate,@price_type,@is_closed,@unit_id)";
    public string UPDATE_QUERY => "UPDATE acq_orders_rows SET company_id = @company_id,id = @id,sequence = @sequence,product_id = @product_id,store_id = @store_id,quantity = @quantity,discount = @discount,price = @price,delivery_requested = @delivery_requested,note = @note,discount_type = @discount_type,quantity_received = @quantity_received,price_second_um = @price_second_um,tax_code = @tax_code,tax_rate = @tax_rate,price_type = @price_type,is_closed = @is_closed,unit_id = @unit_id OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_orders_rows OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND sequence = @sequence AND rv = @rv";
    public bool Insert(acq_orders_rows Model)
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
    public bool Update(acq_orders_rows Model)
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
    public bool UpdateAll(acq_orders_heads Head, ObservableCollection<acq_orders_rows>? Rows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                // update head
                var heads = connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>().UPDATE_QUERY, Head, transaction);

                connection.Execute("DELETE FROM acq_orders_rows WHERE company_id = @cid AND id = @id",
                    new { cid = Head.company_id, id = Head.id },
                    transaction);

                foreach (var row in Rows ?? new ObservableCollection<acq_orders_rows>())
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
                    ErrorHandler.Show("Impossibile aggiornare la testata dell'ordine di acquisto");
                    return false;
                }
            }
            catch (Exception)
            {
                transaction.Rollback();
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

    public bool Delete(acq_orders_rows Model)
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
    public string? Validate(acq_orders_rows Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.company_id))
            {
                if (!string.IsNullOrWhiteSpace(Model.product_id))
                {
                    if (Model.price.HasValue && !string.IsNullOrWhiteSpace(Model.price_type))
                    {
                        if (!string.IsNullOrEmpty(Model.store_id))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.tax_code) && !string.IsNullOrWhiteSpace(Model.tax_rate))
                            {
                                if ((Model.discount.HasValue && !string.IsNullOrWhiteSpace(Model.discount_type)) ||
                                    (!Model.discount.HasValue && string.IsNullOrWhiteSpace(Model.discount_type)))
                                {
                                    if (Model.quantity.HasValue && Model.quantity.Value > 0)
                                    {
                                        if (Model.Product != null)
                                        {
                                            if (!Model.Product.QuantitaDefault.HasValue ||
                                            (Model.Product.QuantitaDefault.HasValue && Model.unit_id == Model.Product.UnitaIDAlt && (Model.quantity % 1 == 0 ? (Model.quantity * (Model.Product.QuantitaDefault ?? 1)) % (Model.Product.QuantitaDefault ?? 1) == 0 : (Model.quantity % (Model.Product.QuantitaDefault ?? 1) == 0))) ||
                                            (Model.Product.QuantitaDefault.HasValue && Model.unit_id != Model.Product.UnitaIDAlt && (Model.quantity % (Model.Product.QuantitaDefault ?? 1) == 0)))
                                            {
                                                return null;
                                            }
                                            else
                                            { return $"La quantità digitata ({Model.quantity.Value.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.Product.QuantitaDefault ?? 1).ToString("N6")})"; }
                                        }
                                        else
                                        {
                                            return "L'articolo e obbligatorio";
                                        }
                                    }
                                    else
                                    { return "La quantita' è obbligatoria"; }
                                }
                                else
                                { return "Sconto e tipo sconto devono essere entrambi presenti o entrambi vuoti"; }
                            }
                            else
                            { return "L'aliquota è obbligatoria"; }
                        }
                        else
                        { return "Il magazzino è obbligatorio"; }
                    }
                    else
                    { return "Prezzo e tipo prezzo sono obbligatori"; }
                }
                else
                { return "L'articolo è obbligatorio"; }
            }
            else
            { return "Il codice inserito è già in uso o non è valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    public string? ValidateModel(ObservableCollection<acq_orders_rows>? Rows)
    {
        if (Rows != null && Rows.Count > 0)
        {
            string? validation = null;
            foreach (var row in Rows)
            {
                validation = Validate(row, false);
                if (!string.IsNullOrWhiteSpace(validation))
                    break;
            }
            if (string.IsNullOrWhiteSpace(validation))
            {
                return null;
            }
            else
            { return validation; }
        }
        else
        {
            return "E' necessario che siano presenti delle righe per confermare l'ordine di acquisto";
        }

    }
    #endregion
}