using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.DAL.Store;

public interface Istore_movementsRepository
{
    ObservableCollection<store_movements>? GetList(string CompanyID, int Year, bool ShowCancelled, string FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount);

    ObservableCollection<store_movements>? GetList(string CompanyID, string ProductID, bool ShowCancelled = false);

    ObservableCollection<store_movements>? GetList(string CompanyID, string ProductID, string StoreID, string Lot);

    List<store_movements>? GetList(string CompanyID, string ProductID, string StoreID, bool ShowCancelled = false, DateTime? PrintUntil = null);

    ObservableCollection<string>? GetGoodsLocationsList(string CompanyID);

    store_movements? Get(string company_id, long id);

    bool Exists(string company_id, long id);

    string? GenerateLotID(string? LotTemplate, int SupplierID, int Progressive, int? ExpireDays);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(store_movements Model, IDbConnection? Connection = null, IDbTransaction? Transaction = null);

    bool Update(store_movements Model);

    bool Delete(store_movements Model);

    string? Validate(store_movements Model, bool IsInsert);
    #endregion
}

public class store_movementsRepository : RepositoryBase, Istore_movementsRepository
{
    private readonly Istore_stocks_lotsRepository _store_stocks_lotsRepository;
    private readonly Istore_storesRepository _store_storesRepository;
    private readonly Istore_stocksRepository _store_stocksRepository;
    private readonly ISTORE_CAUSALSRepository _store_causalsRepository;
    private readonly Istore_movements_historyRepository _store_movementsHistoryRepository;
    private readonly Itab_articoloRepository _tab_articoloRepository;
    private readonly INUMREGRepository _numregRepository;

    public store_movementsRepository(IConnectionFactory factory, Istore_stocks_lotsRepository Istore_stocks_lotsRepository, Istore_storesRepository Istore_storesRepository, Istore_stocksRepository Istore_stocksRepository, ISTORE_CAUSALSRepository ISTORE_CAUSALSRepository
        , Istore_movements_historyRepository Istore_movements_historyRepository, Itab_articoloRepository Itab_articoloRepository, INUMREGRepository INUMREGRepository) : base(factory)
    {
        _store_stocks_lotsRepository = Istore_stocks_lotsRepository;
        _store_storesRepository = Istore_storesRepository;

        _store_stocksRepository = Istore_stocksRepository;
        _store_causalsRepository = ISTORE_CAUSALSRepository;
        _store_movementsHistoryRepository = Istore_movements_historyRepository;

        _tab_articoloRepository = Itab_articoloRepository;
        _numregRepository = INUMREGRepository;

    }

    public ObservableCollection<store_movements>? GetList(string CompanyID, int Year, bool ShowCancelled, string FullTextSearch, int PageSize, int RequestedPage, List<GenericIDDescription> SortList, List<FilterEntry> FilterList, out int TotalCount)
    {
        TotalCount = 0;
        try
        {
            using var connection = GetOpenConnection();


            var aliasList = new List<GenericIDDescriptionType>() {
                    new GenericIDDescriptionType(){ ID = "id", Description="m.id" },
                    new GenericIDDescriptionType(){ ID = "dateText", Description="m.date", Type = "DT" },
                    new GenericIDDescriptionType(){ ID = "Sign", Description="c.sign" },
                    new GenericIDDescriptionType(){ ID = "UM", Description="p.UnitaID" },
                    new GenericIDDescriptionType(){ ID = "product_id", Description="m.product_id" },
                    new GenericIDDescriptionType(){ ID = "ProductDescription", Description="p.Descrizione" },
                    new GenericIDDescriptionType(){ ID = "store_id", Description="m.store_id" },
                    new GenericIDDescriptionType(){ ID = "StoreDescription", Description="s.description" },
                    new GenericIDDescriptionType(){ ID = "causal_id", Description="m.causal_id" },
                    new GenericIDDescriptionType(){ ID = "CausalDescription", Description="c.description" },
                    new GenericIDDescriptionType(){ ID = "price", Description="m.price" },
                    new GenericIDDescriptionType(){ ID = "quantity", Description="m.quantity" },
                    new GenericIDDescriptionType(){ ID = "supplier_lot", Description="m.supplier_lot" },
                    new GenericIDDescriptionType(){ ID = "goods_location", Description="m.goods_location" },
                    new GenericIDDescriptionType(){ ID = "order_id", Description="m.order_id" },
                    new GenericIDDescriptionType(){ ID = "document_id", Description="m.document_id" },
                    new GenericIDDescriptionType(){ ID = "document_row", Description="m.document_row" },
                    new GenericIDDescriptionType(){ ID = "quantity", Description="m.quantity" },
                    new GenericIDDescriptionType(){ ID = "add_user", Description="m.add_user" },
                    new GenericIDDescriptionType(){ ID = "supplier_id", Description="m.supplier_id" },
                    new GenericIDDescriptionType(){ ID = "SupplierDescription", Description="SupplierDescription", Type="#C#CONCAT(TRIM(a.abers1) , ' ', TRIM(a.abers2))" },
                    new GenericIDDescriptionType(){ ID = "expireText", Description="m.expire", Type = "D" },
                    new GenericIDDescriptionType(){ ID = "document_dateText", Description="m.document_date", Type = "D" },
                    new GenericIDDescriptionType(){ ID = "addedText", Description="m.added", Type = "DT" },
                    new GenericIDDescriptionType(){ ID = "lot", Description="m.lot" }};

            #region Args
            FullTextSearch = FullTextSearch.ToLower();
            var args = new DynamicParameters();
            args.Add("company_id", CompanyID);
            args.Add("year", Year + "%");
            args.Add("skip", RequestedPage * PageSize);
            args.Add("ps", PageSize);
            args.Add("ft", $"%{FullTextSearch}%");
            #endregion
            #region Where
            var whereFilter = new StringBuilder($@"m.company_id = @company_id AND m.id LIKE @year {(ShowCancelled ? null : "AND m.canceled IS NULL ")} ");
            // grid filters
            TelerikGridService.ComputeFilter(whereFilter, FilterList, aliasList, args);
            // full-text search
            if (!string.IsNullOrWhiteSpace(FullTextSearch))
            {
                whereFilter.Append($@"AND (LOWER(CONVERT(nvarchar(20), LOWER(m.id))) LIKE @ft OR LOWER(c.sign) LIKE @ft OR LOWER(p.UnitaID) LIKE @ft OR LOWER(CONVERT(nvarchar(20), m.price)) LIKE @ft OR
                                            LOWER(CONVERT(nvarchar(20), m.date, 103)) LIKE @ft OR LOWER(m.store_id) LIKE @ft OR LOWER(s.description) LIKE @ft OR LOWER(m.causal_id) LIKE @ft OR
                                            LOWER(c.description) LIKE @ft OR LOWER(CONVERT(nvarchar(10), m.document_row)) LIKE @ft OR
                                            LOWER(m.add_user) LIKE @ft OR LOWER(m.product_id) LIKE @ft OR LOWER(p.Descrizione) LIKE @ft OR 
                                            LOWER(CONVERT(nvarchar(10), m.supplier_id)) LIKE @ft OR LOWER(a.abers1) LIKE @ft OR LOWER(a.abers2) LIKE @ft OR LOWER(m.supplier_lot) LIKE @ft OR
                                            LOWER(m.goods_location) LIKE @ft OR LOWER(m.document_id) LIKE @ft OR LOWER(CONVERT(nvarchar(20), m.quantity)) LIKE @ft OR
                                            LOWER(CONVERT(nvarchar(20), m.expire, 103)) LIKE @ft OR LOWER(CONVERT(nvarchar(20), m.document_date, 103)) LIKE @ft OR LOWER(CONVERT(nvarchar(20), m.added, 131)) LIKE @ft OR
                                            LOWER(m.lot) LIKE @ft)");
            }
            #endregion
            #region Sort
            var sort = TelerikGridService.ComputeSort(SortList, aliasList);
            #endregion
            TotalCount = (int?)connection.ExecuteScalar($@"SELECT count(*) FROM store_movements as m
                        INNER JOIN store_causals AS c ON m.company_id = c.company_id AND m.causal_id = c.id
                        INNER JOIN store_stores AS s ON m.company_id = s.company_id AND m.store_id = s.id
                        INNER JOIN tab_articolo AS p ON m.company_id = p.SocietaID AND m.product_id = p.ID
                        LEFT JOIN ABE AS a ON a.abecod=m.supplier_id
                        WHERE {whereFilter.ToString()};", args) ?? 0;

            var list = connection.Query<store_movements>(
                $@"SELECT m.*, c.sign AS Sign, p.UnitaID AS UM, c.description AS CausalDescription, s.description AS StoreDescription, p.Descrizione AS ProductDescription, CONCAT(TRIM(a.abers1) , ' ', TRIM(a.abers2)) AS SupplierDescription FROM store_movements as m
                        INNER JOIN store_causals AS c ON m.company_id = c.company_id AND m.causal_id = c.id
                        INNER JOIN store_stores AS s ON m.company_id = s.company_id AND m.store_id = s.id
                        INNER JOIN tab_articolo AS p ON m.company_id = p.SocietaID AND m.product_id = p.ID
                        LEFT JOIN ABE AS a ON a.abecod=m.supplier_id
                        WHERE {whereFilter.ToString()}
                        {(!string.IsNullOrWhiteSpace(sort) ? sort : "ORDER BY m.date DESC ")}
                        OFFSET @skip ROWS 
                        FETCH NEXT @ps ROWS ONLY", args);

            return new ObservableCollection<store_movements>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_movements>? GetList(string CompanyID, string ProductID, bool ShowCancelled = false)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_movements, store_causals, store_stores, tab_articolo, store_movements>(
                $@"SELECT m.*, cau.*, sto.*, prd.UnitaID FROM store_movements AS m
                        INNER JOIN store_causals AS cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id {(ShowCancelled ? null : "AND m.canceled IS NULL")}
                        ORDER BY m.date DESC",
                (mov, cau, sto, prd) =>
                {
                    mov.Causal = cau;
                    mov.StoreFullDescription = sto.FullDescriptionSearchable;
                    mov.UM = prd.UnitaID;
                    return mov;
                },
                new { company_id = CompanyID, product_id = ProductID }, splitOn: "company_id,company_id,UnitaID");

            return new ObservableCollection<store_movements>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_movements>? GetList(string CompanyID, string ProductID, string StoreID, string Lot)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_movements, store_causals, store_stores, tab_articolo, store_movements>(
                $@"SELECT m.*, cau.*, sto.*, prd.UnitaID FROM store_movements AS m
                        INNER JOIN store_causals AS cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id AND m.store_id = @store_id {(string.IsNullOrEmpty(Lot) ? null : "AND m.lot =@lot AND m.canceled IS NULL")}
                        ORDER BY m.date DESC",
                (mov, cau, sto, prd) =>
                {
                    mov.Causal = cau;
                    mov.StoreFullDescription = sto.FullDescriptionSearchable;
                    mov.UM = prd.UnitaID;
                    return mov;
                },
                new { company_id = CompanyID, product_id = ProductID, store_id = StoreID, lot = Lot }, splitOn: "company_id,company_id,UnitaID");

            return new ObservableCollection<store_movements>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<store_movements>? GetList(string CompanyID, string ProductID, string StoreID, bool ShowCancelled = false, DateTime? PrintUntil = null)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_movements, store_causals, store_movements>(
                $@"SELECT m.*, cau.* FROM store_movements AS m
                        INNER JOIN store_causals AS cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id AND m.store_id = @store_id {(ShowCancelled ? null : "AND m.canceled IS NULL")} {(!PrintUntil.HasValue ? null : "AND m.date <= @put")}
                        ORDER BY m.date DESC",
                (mov, cau) =>
                {
                    mov.Causal = cau;
                    return mov;
                },
                new { company_id = CompanyID, product_id = ProductID, store_id = StoreID, put = PrintUntil }, splitOn: "company_id");

            return list.ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<string>? GetGoodsLocationsList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<string>(
                $@"SELECT MIN(m.goods_location) FROM store_movements AS m
                        WHERE m.company_id = @company_id AND m.goods_location is not null
                        GROUP BY m.goods_location",
                new { company_id = CompanyID });

            return new ObservableCollection<string>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_movements? Get(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_movements>(
                "SELECT * FROM store_movements WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM store_movements WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public string? GenerateLotID(string? LotTemplate, int SupplierID, int Progressive, int? ExpireDays)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(LotTemplate))
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                var exp = now.AddDays(ExpireDays ?? 0);
                return LotTemplate.Replace("#Y2#", now.Year.ToString().Substring(2, 2))
                                .Replace("#Y4#", now.Year.ToString().Substring(0, 4))
                                .Replace("#M2#", now.Month.ToString().PadLeft(2, '0'))
                                .Replace("#D2#", now.Day.ToString().PadLeft(2, '0'))
                                .Replace("#XY2#", exp.Year.ToString().Substring(2, 2))
                                .Replace("#XY4#", exp.Year.ToString().Substring(0, 4))
                                .Replace("#XM2#", exp.Month.ToString().PadLeft(2, '0'))
                                .Replace("#XD2#", exp.Day.ToString().PadLeft(2, '0'))
                                .Replace("#P6#", Progressive.ToString().PadLeft(6, '0'))
                                .Replace("#F6#", SupplierID.ToString().PadLeft(6, '0'));
            }
            else
            {
                return null;
            }
        }
        catch
        { return null; }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO store_movements (company_id,id,date,store_id,product_id,causal_id,quantity,document_id,document_date,document_row,note,canceled,cancel_user,added,add_user,updated,update_user,order_id,engage_id,lot,expire,goods_receipt_id,goods_location,supplier_id,document_year,supplier_lot,price,canceledNote) OUTPUT INSERTED.rv VALUES(@company_id,@id,@date,@store_id,@product_id,@causal_id,@quantity,@document_id,@document_date,@document_row,@note,@canceled,@cancel_user,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user,@updated,@update_user,@order_id,@engage_id,@lot,@expire,@goods_receipt_id,@goods_location,@supplier_id,@document_year,@supplier_lot,@price,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE store_movements SET company_id = @company_id,id = @id,date = @date,store_id = @store_id,product_id = @product_id,causal_id = @causal_id,quantity = @quantity,document_id = @document_id,document_date = @document_date,document_row = @document_row,note = @note,canceled = @canceled,cancel_user = @cancel_user,added = @added,add_user = @add_user,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',update_user = @update_user,order_id = @order_id,engage_id = @engage_id,lot = @lot,expire = @expire,goods_receipt_id = @goods_receipt_id,goods_location = @goods_location,supplier_id = @supplier_id,document_year = @document_year,supplier_lot = @supplier_lot,price = @price,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM store_movements OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";

    public bool Insert(store_movements Model, IDbConnection? Connection = null, IDbTransaction? Transaction = null)
    {
        try
        {
            IDbConnection connection = Connection == null ? GetOpenConnection() : Connection;

            var transaction = (Transaction != null ? Transaction : connection.BeginTransaction());
            try
            {
                connection.Execute(INSERT_QUERY, Model, transaction);
                var isProductInfinite = _tab_articoloRepository.CheckIsInfinite(Model.company_id, Model.product_id);

                if (!isProductInfinite)
                {
                    var currentStock = connection.Query<store_stocks>(@"SELECT * FROM store_stocks 
                                                                        WHERE company_id=@cid AND store_id=@sid AND product_id=@pid",
                                                                        new { cid = Model.company_id, sid = Model.store_id, pid = Model.product_id }, transaction).FirstOrDefault();
                    if (currentStock == null)
                    {
                        // new stock
                        store_stocks newStock = new store_stocks()
                        {
                            company_id = Model.company_id,
                            store_id = Model.store_id,
                            product_id = Model.product_id,
                            quantity_production = 0,
                            quantity_stock = Model.quantity,
                            goods_location = Model.goods_location,
                            quantity_ordered = 0
                        };
                        connection.Execute(_store_stocksRepository.INSERT_QUERY, newStock, transaction);
                    }
                    else
                    {
                        // update current
                        if (Model.Causal != null && Model.Causal.sign == "+")
                            currentStock.quantity_stock += Model.quantity;
                        else
                            currentStock.quantity_stock -= Model.quantity;
                        // update stock info
                        currentStock.goods_location = Model.goods_location;
                        // update stock
                        connection.ExecuteScalar(_store_stocksRepository.UPDATE_QUERY, currentStock, transaction);
                    }

                    // lots
                    var lot = !string.IsNullOrWhiteSpace(Model.lot) ? Model.lot : Constants.NO_LOT_ID;
                    var existingLot = connection.Query<store_stocks_lots>(@"SELECT * FROM store_stocks_lots
                                                                            WHERE company_id=@cid AND store_id=@sid AND product_id=@pid AND lot=@lot",
                                                                            new { cid = Model.company_id, sid = Model.store_id, pid = Model.product_id, lot = lot }, transaction).FirstOrDefault();
                    if (existingLot == null)
                    {
                        // add new lot detail
                        var newLot = new store_stocks_lots()
                        {
                            company_id = Model.company_id,
                            store_id = Model.store_id,
                            product_id = Model.product_id,
                            lot = lot,
                            quantity_stock = Model.quantity,
                            goods_location = Model.goods_location,
                            expire = Model.expire,
                            supplier_lot = Model.supplier_lot
                        };
                        connection.Execute(_store_stocks_lotsRepository.INSERT_QUERY, newLot, transaction);
                    }
                    else
                    {
                        // update existing lot detail
                        if (Model.Causal != null && Model.Causal.sign == "+")
                            existingLot.quantity_stock += Model.quantity;
                        else
                            existingLot.quantity_stock -= Model.quantity;
                        // update stock info
                        existingLot.goods_location = Model.goods_location;
                        // update stock lot
                        connection.ExecuteScalar(_store_stocks_lotsRepository.UPDATE_QUERY, existingLot, transaction);
                    }
                }

                #region Recursive linked store causals
                if (Model.Causal != null && !string.IsNullOrWhiteSpace(Model.Causal.link_causal_id))
                {
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                    foreach (var linked in _store_causalsRepository.GetLinkedList(Model.company_id, Model.Causal) ?? new List<Tuple<store_stores, store_causals>>())
                    {
                        var linkedMovement = new store_movements()
                        {
                            company_id = Model.company_id,
                            id = _numregRepository.GetFullLongID(now.Year, _numregRepository.GetNumber(Model.company_id, now.Year, Constants.STORE_MOVEMENTS, true)),
                            date = Model.date,
                            causal_id = linked.Item2.id,
                            Causal = linked.Item2,
                            document_date = Model.document_date,
                            document_year = Model.document_year,
                            document_id = Model.document_id,
                            document_row = Model.document_row,
                            order_id = Model.order_id,
                            product_id = Model.product_id,
                            quantity = Model.quantity,
                            store_id = linked.Item1.id,
                            added = now,
                            engage_id = Model.engage_id,
                            add_user = Model.add_user,
                            lot = Model.lot,
                            goods_location = Model.goods_location,
                            expire = Model.expire,
                            note = $"#Collegato {Model.id}",
                            goods_receipt_id = Model.goods_receipt_id,
                            supplier_id = Model.supplier_id,
                            supplier_lot = Model.supplier_lot,
                            price = Model.price
                        };
                        connection.Execute(INSERT_QUERY, linkedMovement, transaction);
                        if (!isProductInfinite)
                        {
                            var currentStock = connection.Query<store_stocks>(@"SELECT * FROM store_stocks 
                                                                        WHERE company_id=@cid AND store_id=@sid AND product_id=@pid",
                                                                                new { cid = linkedMovement.company_id, sid = linkedMovement.store_id, pid = linkedMovement.product_id }, transaction).FirstOrDefault();
                            if (currentStock == null)
                            {
                                // new stock
                                store_stocks newStock = new store_stocks()
                                {
                                    company_id = linkedMovement.company_id,
                                    store_id = linkedMovement.store_id,
                                    product_id = linkedMovement.product_id,
                                    quantity_production = 0,
                                    quantity_stock = linkedMovement.quantity,
                                    goods_location = linkedMovement.goods_location,
                                    quantity_ordered = 0
                                };
                                connection.Execute(_store_stocksRepository.INSERT_QUERY, newStock, transaction);
                            }
                            else
                            {
                                // update current
                                if (linkedMovement.Causal.sign == "+")
                                    currentStock.quantity_stock += linkedMovement.quantity;
                                else
                                    currentStock.quantity_stock -= linkedMovement.quantity;
                                // update stock info
                                currentStock.goods_location = linkedMovement.goods_location;
                                // update stock
                                connection.ExecuteScalar(_store_stocksRepository.UPDATE_QUERY, currentStock, transaction);
                            }

                            // lots
                            var lot = !string.IsNullOrWhiteSpace(linkedMovement.lot) ? linkedMovement.lot : Constants.NO_LOT_ID;
                            var existingLot = connection.Query<store_stocks_lots>(@"SELECT * FROM store_stocks_lots
                                                                            WHERE company_id=@cid AND store_id=@sid AND product_id=@pid AND lot=@lot",
                                                                                    new { cid = linkedMovement.company_id, sid = linkedMovement.store_id, pid = linkedMovement.product_id, lot = lot }, transaction).FirstOrDefault();
                            if (existingLot == null)
                            {
                                // add new lot detail
                                var newLot = new store_stocks_lots()
                                {
                                    company_id = linkedMovement.company_id,
                                    store_id = linkedMovement.store_id,
                                    product_id = linkedMovement.product_id,
                                    lot = lot,
                                    quantity_stock = linkedMovement.quantity,
                                    goods_location = linkedMovement.goods_location,
                                    expire = linkedMovement.expire,
                                    supplier_lot = linkedMovement.supplier_lot
                                };
                                connection.Execute(_store_stocks_lotsRepository.INSERT_QUERY, newLot, transaction);
                            }
                            else
                            {
                                // update existing lot detail
                                if (linkedMovement.Causal.sign == "+")
                                    existingLot.quantity_stock += linkedMovement.quantity;
                                else
                                    existingLot.quantity_stock -= linkedMovement.quantity;
                                // update stock info
                                existingLot.goods_location = linkedMovement.goods_location;
                                // update stock lot
                                connection.ExecuteScalar(_store_stocks_lotsRepository.UPDATE_QUERY, existingLot, transaction);
                            }
                        }
                    }
                }
                #endregion

                if (Transaction == null)
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(store_movements Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // compute quantity diff
                        var diff = (Model.quantity ?? 0) - (Model.OldVersion?.quantity ?? 0);
                        // get all
                        var product = _tab_articoloRepository.GetCheckFields(Model.company_id, Model.product_id);

                        if (product != null)
                        {
                            if (product.IsInfinite)
                            {
                                var newSequence = _store_movementsHistoryRepository.GetLastSequence(Model.company_id, Model.id);
                                if (newSequence.HasValue)
                                {
                                    // save the older
                                    Model.OldVersion!.sequence = newSequence.Value + 1;
                                    connection.Execute(_store_movementsHistoryRepository.INSERT_QUERY, Model.OldVersion, transaction);

                                    // update the movement
                                    connection.Execute(UPDATE_QUERY, Model, transaction);

                                    transaction.Commit();
                                    return true;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ErrorHandler.Show("Impossibile salvare lo storico del movimento, modifica non riuscita");
                                    return false;
                                }
                            }
                            else
                            {
                                var stock = _store_stocksRepository.Get(Model.company_id, Model.store_id, Model.product_id);
                                var stockLot = _store_stocks_lotsRepository.Get(Model.company_id, Model.store_id, Model.product_id, !string.IsNullOrWhiteSpace(Model.lot) ? Model.lot : Constants.NO_LOT_ID);
                                var engages = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetActiveList(Model.company_id, Model.product_id);
                                decimal engagedQuantity = engages?.Where(w => w.lot == Model.lot).Sum(sum => sum.quantity) ?? 0;

                                // check stock negative
                                if (stock != null && stockLot != null)
                                {
                                    if (Model.Causal != null && ((Model.Causal.sign == "-" && diff > 0) ||
                                        (Model.Causal.sign == "+" && diff < 0)))
                                    {
                                        if (((product.HasLots || !string.IsNullOrWhiteSpace(Model.lot)) && (stockLot.quantity_stock + diff) < 0) ||
                                            ((!product.HasLots || string.IsNullOrWhiteSpace(Model.lot)) && (stock.quantity_stock + diff) < 0))
                                        {
                                            transaction.Rollback();
                                            ErrorHandler.Show("Impossibile modificare il movimento per mancanza di materiale");
                                            return false;
                                        }
                                        // check engages
                                        if (((product.HasLots || !string.IsNullOrWhiteSpace(Model.lot)) && (stockLot.quantity_stock - engagedQuantity + diff) < 0) ||
                                            ((!product.HasLots || string.IsNullOrWhiteSpace(Model.lot)) && (stock.quantity_stock - engagedQuantity + diff) < 0))
                                        {
                                            transaction.Rollback();
                                            ErrorHandler.Show("Impossibile modificare il movimento per la presenza di impegni");
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    ErrorHandler.Show("Giacenza o Lotto non trovati");
                                    return false;
                                }

                                var newSequence = _store_movementsHistoryRepository.GetLastSequence(Model.company_id, Model.id);
                                if (newSequence.HasValue)
                                {
                                    // save the older
                                    Model.OldVersion!.sequence = newSequence.Value + 1;
                                    connection.Execute(_store_movementsHistoryRepository.INSERT_QUERY, Model.OldVersion, transaction);

                                    // update stock
                                    stock.quantity_stock += Model.Causal?.sign == "-" ? diff * -1 : diff;
                                    stock.goods_location = Model.goods_location;
                                    connection.Execute(_store_stocksRepository.UPDATE_QUERY, stock, transaction);

                                    // update lot if needed
                                    if (stockLot != null)
                                    {
                                        stockLot.quantity_stock += Model.Causal?.sign == "-" ? diff * -1 : diff;
                                        stockLot.goods_location = Model.goods_location;
                                        stockLot.expire = Model.expire;
                                        stockLot.supplier_lot = Model.supplier_lot;
                                        connection.Execute(_store_stocks_lotsRepository.UPDATE_QUERY, stockLot, transaction);
                                    }

                                    // update the movement
                                    connection.Execute(UPDATE_QUERY, Model, transaction);

                                    transaction.Commit();
                                    return true;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ErrorHandler.Show("Impossibile salvare lo storico del movimento, modifica non riuscita");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            ErrorHandler.Show("Articolo non trovato");
                            return false;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                        return false;
                    }
                }
            }
            else
            {
                ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(store_movements Model)
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

    public string? Validate(store_movements Model, bool IsInsert)
    {
        try
        {
            if (Model.id > 0)
            {
                if (Model.date.HasValue)
                {
                    if (!string.IsNullOrWhiteSpace(Model.product_id))
                    {
                        if (!string.IsNullOrWhiteSpace(Model.store_id))
                        {
                            if (!string.IsNullOrWhiteSpace(Model.causal_id))
                            {
                                if (Model.quantity > 0)
                                {
                                    if (Model.Product != null)
                                    {
                                        if (!Model.Product.QuantitaDefault.HasValue ||
                                            (Model.Product.QuantitaDefault.HasValue && (Model.quantity % (Model.Product.QuantitaDefault ?? 1) == 0)))
                                        {
                                            var um = _tab_articoloRepository.Get(Model.company_id, Model.product_id)?.UnitaID;
                                            var quantityAvailable = _store_stocksRepository.CheckAvailabilityByStore(Model.company_id, Model.store_id, Model.product_id, Model.lot)?.QuantityAvailable ?? 0;
                                            if ((Model.Causal != null) && (Model.Causal.sign == "+" || (Model.Causal.sign == "-" && Model.quantity <= quantityAvailable)))
                                            {
                                                // check linked mov
                                                decimal linkedQuantityAvailable = 0;
                                                string linkedSign = string.Empty;
                                                string linkedStoreDescription = string.Empty;
                                                if (!string.IsNullOrWhiteSpace(Model.Causal.link_causal_id) && !string.IsNullOrWhiteSpace(Model.Causal.link_store_id))
                                                {
                                                    var linkedCausal = _store_causalsRepository.Get(Model.company_id, Model.Causal.link_causal_id);
                                                    if (linkedCausal != null && linkedCausal.sign == "-")
                                                    {
                                                        linkedSign = "-";
                                                        linkedQuantityAvailable = _store_stocksRepository.CheckAvailabilityByStore(Model.company_id, Model.Causal.link_store_id, Model.product_id, Model.lot)?.QuantityAvailable ?? 0;
                                                        linkedStoreDescription = _store_storesRepository.Get(Model.company_id, Model.Causal.link_store_id)?.FullDescriptionSearchable ?? string.Empty;
                                                    }
                                                }
                                                if (string.IsNullOrWhiteSpace(linkedSign) || (!string.IsNullOrWhiteSpace(linkedSign) && Model.quantity <= linkedQuantityAvailable))
                                                {
                                                    if ((Model.Product.HasLots && !string.IsNullOrWhiteSpace(Model.lot))
                                                        || !Model.Product.HasLots)
                                                    {
                                                        if ((Model.expire.HasValue && !string.IsNullOrWhiteSpace(Model.lot)) ||
                                                            !Model.expire.HasValue)
                                                        {
                                                            return null;
                                                        }
                                                        else
                                                        { return "Per selezionare una data di scadenza occorre indicare anche il lotto"; }
                                                    }
                                                    else
                                                    { return "Per questo articolo è prevista la gestione lotti, il numero lotto è obbligatorio"; }
                                                }
                                                else
                                                { return $"La quantità digitata ({Model.quantity.Value.ToString("N6")}) non è disponibile per lo scarico dal magazzino collegato [{linkedStoreDescription}]"; }
                                            }
                                            else
                                            {
                                                return $"La quantita' attualmente disponibile non e' sufficiente per eseguire l'operazione.\nLa quantita' disponibile su questo magazzino{(!string.IsNullOrWhiteSpace(Model.lot) ? $" per il lotto {Model.lot}" : null)} e' {quantityAvailable.ToString("N6")} {um}";
                                            }
                                        }
                                        else
                                        { return $"La quantità digitata ({Model.quantity.Value.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.Product.QuantitaDefault ?? 1).ToString("N6")})"; }
                                    }
                                    else
                                    {
                                        return "L'articolo è obbligatorio";
                                    }
                                }
                                else
                                { return "La quantita' e' obbligatoria"; }
                            }
                            else
                            { return "La causale di magazzino e' obbligatoria"; }
                        }
                        else
                        { return "Il magazzino da utilizzare e' obbligatorio"; }
                    }
                    else
                    { return "Il codice prodotto e' obbligatorio"; }
                }
                else
                { return "La data del movimento e' obbligatoria"; }
            }
            else
            { return "Il progressivo non e' valido"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}