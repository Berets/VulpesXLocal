

using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;

namespace VulpesX.DAL.SRM;

public interface Iacq_goods_receiptsRepository
{

    ObservableCollection<acq_goods_receipts>? GetList(string CompanyID);

    ObservableCollection<acq_goods_receipts>? GetList(string CompanyID, DateTime FromDate, DateTime ToDate);

    acq_goods_receipts? Get(string company_id, long id);

    acq_goods_receipts? Get(string company_id, int supplier_id, DateTime document_date, string document_number, int document_row);

    bool Exists(string company_id, int supplier_id, DateTime document_date, string document_number, int document_row);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(acq_goods_receipts Model, acq_orders_rows OrderRow, bool CloseRow);

    bool Update(acq_goods_receipts Model);

    bool Delete(acq_goods_receipts Model);

    string? Validate(acq_goods_receipts Model, decimal RemainQuantity, bool IsInsert);

    string? EditValidate(acq_goods_receipts Model);
    #endregion
}

public class acq_goods_receiptsRepository : RepositoryBase, Iacq_goods_receiptsRepository
{
    public acq_goods_receiptsRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<acq_goods_receipts>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<acq_goods_receipts>(
                @"SELECT * FROM acq_goods_receipts
                        WHERE company_id = @cid
                        ORDER BY added DESC",
                new { cid = CompanyID });

            return new ObservableCollection<acq_goods_receipts>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<acq_goods_receipts>? GetList(string CompanyID, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            using var connection = GetOpenConnection();


            FromDate = new DateTime(FromDate.Year, FromDate.Month, FromDate.Day, 0, 0, 0);
            ToDate = new DateTime(ToDate.Year, ToDate.Month, ToDate.Day, 23, 59, 59);
            var list = connection.Query<acq_goods_receipts>(
                @"SELECT g.*, 
                            CONCAT(p.ID, ' ', p.Descrizione) AS ProductDescription, 
                            CONCAT(a.abecod, ' ' , a.abers1) AS SupplierDescription, 
                            o.quantity AS OrderedQuantity, p.UnitaID AS UM,
                            CONCAT(s.id, ' ', s.description) AS StoreDescription,
                            CONCAT(c.id, ' ', c.description) AS CausalDescription
                            FROM acq_goods_receipts AS g
                        INNER JOIN tab_articolo AS p ON p.SocietaID=g.company_id AND p.ID = g.product_id
                        INNER JOIN ABE AS a ON a.abecod = g.supplier_id
                        INNER JOIN acq_orders_rows AS o ON o.company_id=g.company_id AND o.id = g.order_id AND o.sequence=g.order_row_id
                        INNER JOIN store_stores AS s ON s.company_id=g.company_id AND s.id=g.store_id
                        INNER JOIN store_causals AS c ON c.company_id=g.company_id AND c.id=g.causal_id
                        WHERE g.company_id = @cid AND g.added >= @fd AND g.added <= @td
                        ORDER BY g.added DESC",
                new { cid = CompanyID, fd = FromDate, td = ToDate });

            return new ObservableCollection<acq_goods_receipts>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_goods_receipts? Get(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<acq_goods_receipts>(
                @"SELECT g.*, 
                            CONCAT(p.ID, ' ', p.Descrizione) AS ProductDescription, 
                            CONCAT(a.abecod, ' ' , a.abers1) AS SupplierDescription, 
                            o.quantity AS OrderedQuantity, p.UnitaID AS UM,
                            CONCAT(s.id, ' ', s.description) AS StoreDescription,
                            CONCAT(c.id, ' ', c.description) AS CausalDescription
                            FROM acq_goods_receipts AS g
                        INNER JOIN tab_articolo AS p ON p.SocietaID=g.company_id AND p.ID = g.product_id
                        INNER JOIN ABE AS a ON a.abecod = g.supplier_id
                        INNER JOIN acq_orders_rows AS o ON o.company_id=g.company_id AND o.id = g.order_id AND o.sequence=g.order_row_id
                        INNER JOIN store_stores AS s ON s.company_id=g.company_id AND s.id=g.store_id
                        INNER JOIN store_causals AS c ON c.company_id=g.company_id AND c.id=g.causal_id
                        WHERE g.company_id = @cid AND g.id=@id",
                new { cid = company_id, id = id })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_goods_receipts? Get(string company_id, int supplier_id, DateTime document_date, string document_number, int document_row)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<acq_goods_receipts>(
                "SELECT * FROM acq_goods_receipts WHERE company_id = @company_id AND supplier_id = @supplier_id AND document_date = @document_date AND document_number = @document_number AND document_row = @document_row",
                new { company_id = company_id, supplier_id = supplier_id, document_date = document_date, document_number = document_number, document_row = document_row })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, int supplier_id, DateTime document_date, string document_number, int document_row)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM acq_goods_receipts WHERE company_id = @company_id AND supplier_id = @supplier_id AND document_date = @document_date AND document_number = @document_number AND document_row = @document_row",
                new { company_id = company_id, supplier_id = supplier_id, document_date = document_date, document_number = document_number, document_row = document_row }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO acq_goods_receipts (company_id,id,supplier_id,document_date,document_number,document_row,product_id,quantity,store_id,causal_id,lot,expire,order_id,order_row_id,note,added,addedUserID,updated,updatedUserID,canceled,canceledUserID,canceledNote,goods_location,unit_id,supplier_lot) OUTPUT INSERTED.rv VALUES(@company_id,@id,@supplier_id,@document_date,@document_number,@document_row,@product_id,@quantity,@store_id,@causal_id,@lot,@expire,@order_id,@order_row_id,@note,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID,@canceled,@canceledUserID,@canceledNote,@goods_location,@unit_id,@supplier_lot)";
    public string UPDATE_QUERY => "UPDATE acq_goods_receipts SET company_id = @company_id,id = @id,supplier_id = @supplier_id,document_date = @document_date,document_number = @document_number,document_row = @document_row,product_id = @product_id,quantity = @quantity,store_id = @store_id,causal_id = @causal_id,lot = @lot,expire = @expire,order_id = @order_id,order_row_id = @order_row_id,note = @note,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID,canceled = @canceled,canceledUserID = @canceledUserID,canceledNote = @canceledNote,goods_location = @goods_location,unit_id = @unit_id,supplier_lot = @supplier_lot OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_goods_receipts OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";

    public bool Insert(acq_goods_receipts Model, acq_orders_rows OrderRow, bool CloseRow)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                    var tabArticoloCostiRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_costiRepository>();
                    var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                    var storeMovementRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>();
                    var acqOrderHeadRepo = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_headsRepository>();
                    var acqOrderRowRepo = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>();
                    var acqOrderRowJobRepo = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_jobsRepository>();
                    var acqOrderRowCustomerOrderRepo = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_customer_ordersRepository>();

                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                    // compute standard unit price and quantity
                    decimal unitPrice = 0;
                    decimal realQuantity = 0;
                    if (Model.unit_id == OrderRow.Product?.UnitaID)
                    {
                        realQuantity = Model.quantity ?? 0;
                        unitPrice = OrderRow.price_type == "U" ? (OrderRow.price ?? 0) : (OrderRow.price ?? 0) / (OrderRow.quantity ?? 1);
                    }
                    else
                    {
                        realQuantity = (Model.quantity ?? 0) * (OrderRow.Product?.QuantitaDefault ?? 1);
                        unitPrice = OrderRow.price_type == "U" ? (OrderRow.price ?? 0) / (OrderRow.Product?.QuantitaDefault ?? 1) : (OrderRow.price ?? 0) / (OrderRow.quantity ?? 1 * (OrderRow.Product?.QuantitaDefault ?? 1));
                    }
                    // compute discount
                    if (OrderRow.discount.HasValue && OrderRow.discount.Value > 0)
                    {
                        if (OrderRow.discount_type == "P")
                            unitPrice = Math.Round((unitPrice - (unitPrice * OrderRow.discount.Value / 100)), 2, MidpointRounding.AwayFromZero);
                        else
                            unitPrice = Math.Round(unitPrice - OrderRow.discount.Value, 2, MidpointRounding.AwayFromZero);
                    }
                    #region Add movement
                    // movement
                    var mov = new store_movements()
                    {
                        company_id = Model.company_id,
                        id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(Model.company_id, now.Year, Constants.STORE_MOVEMENTS, true)),
                        date = now,
                        store_id = Model.store_id ?? "X",
                        product_id = Model.product_id ?? "X",
                        causal_id = Model.causal_id,
                        quantity = realQuantity,
                        document_date = Model.document_date,
                        document_id = Model.document_number,
                        document_row = Model.document_row,
                        note = Model.note,
                        added = now,
                        add_user = Model.addedUserID,
                        lot = Model.lot,
                        expire = Model.expire,
                        supplier_id = Model.supplier_id,
                        goods_location = Model.goods_location,
                        goods_receipt_id = Model.id,
                        supplier_lot = Model.supplier_lot,
                        Causal = Model.Causal,
                        price = (OrderRow.quantity ?? 0) > 0 ? OrderRow.NetAmount / OrderRow.quantity!.Value : 0
                    };
                    storeMovementRepo.Insert(mov, connection, transaction);
                    #endregion
                    // jobs
                    if (OrderRow.Jobs != null && OrderRow.Jobs.Count > 0)
                    {
                        #region Add engages and update received quantities on jobs
                        foreach (var job in OrderRow.Jobs.Where(w => !string.IsNullOrWhiteSpace(w.job_id)))
                        {
                            var existingEngage = (!string.IsNullOrEmpty(OrderRow.product_id) && !string.IsNullOrEmpty(Model.lot)) ? storeStockEngageRepo.GetByOrderID(Model.company_id, job.job_id, OrderRow.product_id, Model.lot) : null;
                            if (existingEngage != null)
                            {
                                existingEngage.quantity += job.QuantityAssigned;
                                connection.Execute(storeStockEngageRepo.UPDATE_QUERY, existingEngage, transaction);
                            }
                            else
                            {
                                var newEngage = new store_stocks_engage()
                                {
                                    company_id = Model.company_id,
                                    id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(Model.company_id, now.Year, Constants.STORE_ENGAGES, true)),
                                    store_id = Model.store_id ?? "X",
                                    product_id = Model.product_id ?? "X",
                                    job_id = job.job_id,
                                    order_id = job.job_id,
                                    document_id = Model.document_number,
                                    quantity = job.QuantityAssigned,
                                    date_engaged = now,
                                    added = now,
                                    add_user = Model.addedUserID,
                                    lot = !string.IsNullOrWhiteSpace(Model.lot) ? Model.lot : null
                                };
                                connection.Execute(storeStockEngageRepo.INSERT_QUERY, newEngage, transaction);
                            }
                            job.quantity_received += job.QuantityAssigned;
                            connection.Execute(acqOrderRowJobRepo.UPDATE_QUERY, job, transaction);
                        }
                        #endregion
                    }
                    // customer orders
                    if (OrderRow.CustomerOrders != null && OrderRow.CustomerOrders.Count > 0)
                    {
                        #region Add engages and update received quantities on customer order
                        foreach (var co in OrderRow.CustomerOrders)
                        {
                            var existingEngage = (!string.IsNullOrEmpty(OrderRow.product_id) && !string.IsNullOrEmpty(Model.lot)) ? storeStockEngageRepo.GetByCustomerOrderID(Model.company_id, co.customer_order_year, co.customer_order_number, co.customer_order_row, OrderRow.product_id, Model.lot) : null;
                            if (existingEngage != null)
                            {
                                existingEngage.quantity += co.QuantityAssigned;
                                connection.Execute(storeStockEngageRepo.UPDATE_QUERY, existingEngage, transaction);
                            }
                            else
                            {
                                var newEngage = new store_stocks_engage()
                                {
                                    company_id = Model.company_id,
                                    id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(Model.company_id, now.Year, Constants.STORE_ENGAGES, true)),
                                    store_id = Model.store_id ?? "X",
                                    product_id = Model.product_id ?? "X",
                                    job_id = co.FullID,
                                    order_year = co.customer_order_year,
                                    order_number = co.customer_order_number,
                                    order_row = co.customer_order_row,
                                    document_id = Model.document_number,
                                    quantity = co.QuantityAssigned,
                                    date_engaged = now,
                                    added = now,
                                    add_user = Model.addedUserID,
                                    lot = !string.IsNullOrWhiteSpace(Model.lot) ? Model.lot : null
                                };
                                connection.Execute(storeStockEngageRepo.INSERT_QUERY, newEngage, transaction);
                            }
                            co.quantity_received += co.QuantityAssigned;
                            connection.Execute(acqOrderRowCustomerOrderRepo.UPDATE_QUERY, co, transaction);
                        }
                        #endregion
                    }
                    // update order received quantity and check if order closed
                    if (OrderRow.quantity_received.HasValue)
                        OrderRow.quantity_received += Model.quantity;
                    else
                        OrderRow.quantity_received = Model.quantity;
                    if ((OrderRow.quantity_received ?? 0) == (OrderRow.quantity ?? 0) || CloseRow)
                    {
                        OrderRow.is_closed = true;
                        // check if all rows are closed
                        var rowsOpen = (int?)connection.ExecuteScalar(@"SELECT COUNT(*) FROM acq_orders_rows 
                                                                        WHERE company_id = @cid AND id = @oid AND sequence <> @seq AND is_closed = 0",
                                                                    new { cid = Model.company_id, oid = OrderRow.id, seq = OrderRow.sequence }, transaction);
                        if (rowsOpen == 0)
                        {
                            // close the entire order
                            var currentOrderHead = acqOrderHeadRepo.Get(OrderRow.company_id, OrderRow.id);

                            if (currentOrderHead != null)
                            {
                                currentOrderHead.closed = now;
                                connection.Execute(acqOrderHeadRepo.UPDATE_QUERY, currentOrderHead, transaction);
                            }
                        }
                    }
                    connection.Execute(acqOrderRowRepo.UPDATE_QUERY, OrderRow, transaction);
                    #region update product costs
                    var existingCost = (!string.IsNullOrEmpty(Model.product_id)) ? tabArticoloCostiRepo.Get(Model.company_id, Model.product_id, now.Year, now.Month) : null;

                    if (existingCost != null)
                    {
                        existingCost.total_load += realQuantity;
                        existingCost.last_cost = unitPrice;
                        existingCost.total_value += unitPrice * realQuantity;
                        existingCost.updatedUserID = Model.addedUserID;
                        connection.Execute(tabArticoloCostiRepo.UPDATE_QUERY, existingCost, transaction);
                    }
                    else
                    {
                        var cost = new tab_articolo_costi()
                        {
                            cid = Model.company_id,
                            product_id = Model.product_id ?? "X",
                            year = now.Year,
                            month = now.Month,
                            total_load = realQuantity,
                            last_cost = unitPrice,
                            total_value = unitPrice * realQuantity,
                            addedUserID = Model.addedUserID
                        };
                        connection.Execute(tabArticoloCostiRepo.INSERT_QUERY, cost, transaction);
                    }
                    #endregion
                    // insert goods receipt
                    connection.Execute(INSERT_QUERY, Model, transaction);
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

    public bool Update(acq_goods_receipts Model)
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

    public bool Delete(acq_goods_receipts Model)
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

    public string? Validate(acq_goods_receipts Model, decimal RemainQuantity, bool IsInsert)
    {
        try
        {
            if (!Exists(Model.company_id, Model.supplier_id, Model.document_date, Model.document_number, Model.document_row))
            {
                if (!string.IsNullOrWhiteSpace(Model.document_number) && Model.document_row > 0)
                {
                    if (!string.IsNullOrWhiteSpace(Model.product_id))
                    {
                        if (Model.quantity.HasValue && Model.quantity.Value > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(Model.store_id))
                            {
                                if (!string.IsNullOrWhiteSpace(Model.causal_id))
                                {
                                    if (!Model.expire.HasValue || (Model.expire.HasValue && Model.expire.Value.Date > VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime()))
                                    {
                                        // check if lot is needed
                                        var product = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(Model.company_id, Model.product_id);

                                        if (product != null)
                                        {
                                            if ((product.HasLots && !string.IsNullOrWhiteSpace(Model.lot)) || !product.HasLots)
                                            {
                                                return null;
                                            }
                                            else
                                            { return "Per questo articolo il lotto è obbligatorio"; }
                                        }
                                        else
                                        {
                                            return "Articolo non trovato";
                                        }
                                    }
                                    else
                                    {
                                        return "La data si scadenza, se presente, deve essere maggiore della data odierna";
                                    }
                                }
                                else
                                { return "La causale di magazzino è obbligatoria"; }
                            }
                            else
                            { return "Il magazzino è obbligatorio"; }
                        }
                        else
                        { return "La quantita' è obbligatoria e deve essere maggiore di 0"; }
                    }
                    else
                    { return "L'articolo è obbligatorio"; }
                }
                else
                { return "Tutti i dati del documento sono obbligatori"; }
            }
            else
            { return "Esiste gia' un'entrata merci per lo stesso fornitore con lo stesso documento di trasporto"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public string? EditValidate(acq_goods_receipts Model)
    {
        try
        {
            if (Model.quantity.HasValue && Model.quantity.Value > 0)
            {
                if (!string.IsNullOrWhiteSpace(Model.document_number) && Model.document_row > 0)
                {
                    return null;
                }
                else
                { return "Tutti i dati del documento sono obbligatori"; }
            }
            else
            { return "La quantità è obbligatoria e deve essere maggiore di 0"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}