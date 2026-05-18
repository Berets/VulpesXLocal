
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models;
using VulpesX.Models.Models.Reports;

namespace VulpesX.DAL.Store;

public interface Istore_stocksRepository
{
    ObservableCollection<store_stocks>? GetList(string CompanyID, string StoreID, bool ShowInfinite, out string? Warnings);

    ObservableCollection<StockInfo>? GetListByProduct(string CompanyID, string ProductID);

    store_stocks? Get(string company_id, string store_id, string product_id);

    bool Exists(string company_id, string store_id, string product_id);

    StockInfo? CheckAvailabilityByStore(string company_id, string store_id, string product_id, string? lot);

    ObservableCollection<StockInfo>? CheckAvailabilityByProduct(string company_id, string product_id, string? lot);

    ObservableCollection<StockCheckExistance.Lots>? CheckExistanceLots(string company_id);

    ObservableCollection<StockCheckExistance.NoLots>? CheckExistanceNoLots(string company_id);

    bool Align(string CompanyID, string StoreID, string ProductID, string Lot);

    #region Reports
    StocksListReport? PrintStocks(string CompanyID, bool ShowInfinite, bool ShowZeroStocks, bool ShowLots, bool ShowMovements, bool ShowEngages, bool ShowMovementsCanceled, bool ShowEngagesUnloaded, string? SelectedStoreID = null, string? SelectedProductTypeID = null, string? SelectedProductID = null, DateTime? PrintUntil = null);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(store_stocks Model);

    bool Update(store_stocks Model);

    bool Delete(store_stocks Model);

    string? Validate(store_stocks Model, bool IsInsert);
    #endregion
}

public class store_stocksRepository : RepositoryBase, Istore_stocksRepository
{
    private readonly Istore_stocks_lotsRepository _store_stocks_lotsRepository;
    private readonly Istore_storesRepository _store_StoresRepository;

    private readonly ICompanyRepository _companyRepository;
    private readonly IAZIENDARepository _aziendaRepository;
    private readonly Iacq_orders_rowsRepository _acq_orders_rowsRepository;

    public store_stocksRepository(IConnectionFactory factory, Istore_stocks_lotsRepository Istore_stocks_lotsRepository, Istore_storesRepository Istore_storesRepository
        , ICompanyRepository ICompanyRepository, IAZIENDARepository IAZIENDARepository, Iacq_orders_rowsRepository Iacq_orders_rowsRepository) : base(factory)
    {
        _store_stocks_lotsRepository = Istore_stocks_lotsRepository;
        _store_StoresRepository = Istore_storesRepository;

        _companyRepository = ICompanyRepository;
        _aziendaRepository = IAZIENDARepository;
        _acq_orders_rowsRepository = Iacq_orders_rowsRepository;
    }

    public ObservableCollection<store_stocks>? GetList(string CompanyID, string StoreID, bool ShowInfinite, out string? Warnings)
    {
        Warnings = null;
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_stocks, store_stores, tab_articolo, decimal?, decimal?, store_stocks>(
                $@"SELECT s.*, sto.description, p.Descrizione, p.UnitaID, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.product_id = s.product_id AND eng.canceled IS NULL AND eng.date_unloaded IS NULL AND eng.store_id = s.store_id) AS engaged, (SELECT (SUM(r.quantity) - SUM(ISNULL(r.quantity_received, 0))) AS orde FROM acq_orders_rows AS r INNER JOIN acq_orders_heads AS t ON t.company_id=r.company_id AND t.id=r.id WHERE t.canceled IS NULL AND r.product_id = s.product_id AND t.sent IS NOT NULL AND t.closed IS NULL) AS ordered FROM store_stocks AS s
                        INNER JOIN store_stores AS sto ON sto.company_id = s.company_id AND sto.id = s.store_id
                        INNER JOIN tab_articolo AS p ON p.SocietaID = s.company_id AND p.ID = s.product_id
                        WHERE s.company_id = @company_id AND p.IsInfinite = 0 {(!string.IsNullOrWhiteSpace(StoreID) ? "AND s.store_id = @store_id" : null)}",
                (stk, sto, prd, eng, orde) =>
                {
                    stk.StoreDescription = sto.description;
                    stk.ProductDescription = prd.Descrizione;
                    stk.UM = prd.UnitaID;
                    stk.Info.QuantityStock = stk.quantity_stock;
                    stk.Info.QuantityProduction = stk.quantity_production;
                    stk.Info.QuantityEngaged = eng ?? 0;
                    stk.Info.QuantityOrdered = orde;
                    return stk;
                },
                new { company_id = CompanyID, store_id = StoreID }, splitOn: "description,Descrizione,engaged,ordered").ToList();

            if (ShowInfinite)
            {
                #region add always available
                // get always available default store
                var infiniteStore = _store_StoresRepository.GetDefaultInfinite(CompanyID);
                if (infiniteStore != null)
                {
                    foreach (var stock in connection.Query<store_stocks, tab_articolo, decimal?, store_stocks>(
                        @"SELECT 1, p.ID, p.Descrizione, p.UnitaID, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = @cid AND eng.product_id = p.ID AND eng.canceled IS NULL AND eng.date_unloaded IS NULL AND eng.store_id = @isid) AS engaged FROM tab_articolo AS p
                        WHERE p.SocietaID = @cid AND p.IsInfinite = 1 AND p.LogCanceled IS NULL",
                            (stk, prd, eng) =>
                            {
                                stk.company_id = CompanyID;
                                stk.store_id = infiniteStore.id;
                                stk.product_id = prd.ID;
                                stk.ProductDescription = prd.Descrizione;
                                stk.StoreDescription = infiniteStore?.description;
                                stk.UM = prd.UnitaID;
                                stk.Info.QuantityStock = int.MaxValue;
                                stk.Info.QuantityEngaged = eng ?? 0;
                                stk.IsInfinite = true;
                                return stk;
                            },
                            new { cid = CompanyID, isid = infiniteStore?.id }, splitOn: "ID,engaged"))
                    {
                        list.Add(stock);
                    }
                }
                else
                {
                    Warnings = "ATTENZIONE, non è stato impostato un magazzino di default per i materiali sempre disponibili, i quali non verranno mostrati";
                }
                #endregion
            }
            return new ObservableCollection<store_stocks>(list.OrderByDescending(o => o.IsInfinite).ThenBy(o => o.product_id));
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<StockInfo>? GetListByProduct(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<StockInfo, store_stores, decimal?, StockInfo>(
                $@"SELECT s.product_id AS ProductID, s.quantity_stock AS QuantityStock, s.quantity_production AS QuantityProduction, s.quantity_ordered AS QuantityOrdered, sto.id, sto.description, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.product_id = s.product_id AND eng.canceled IS NULL AND eng.date_unloaded IS NULL AND eng.store_id = s.store_id) AS QuantityEngaged FROM store_stocks AS s
                        INNER JOIN store_stores AS sto ON sto.company_id = s.company_id AND sto.id = s.store_id
                        WHERE s.company_id = @company_id AND s.product_id = @pid",
                (sti, sto, eng) =>
                {
                    sti.StoreID = sto.id;
                    sti.StoreDescription = sto.description;
                    sti.QuantityEngaged = eng ?? 0;
                    return sti;
                },
                new { company_id = CompanyID, pid = ProductID }, splitOn: "id,QuantityEngaged");

            foreach (var item in list)
            {
                item.Lots = _store_stocks_lotsRepository.GetList(CompanyID, item.StoreID!, ProductID, true);
            }
            return new ObservableCollection<StockInfo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stocks? Get(string company_id, string store_id, string product_id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stocks>(
                "SELECT * FROM store_stocks WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id",
                new { company_id = company_id, store_id = store_id, product_id = product_id })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, string store_id, string product_id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM store_stocks WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id",
                new { company_id = company_id, store_id = store_id, product_id = product_id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public StockInfo? CheckAvailabilityByStore(string company_id, string store_id, string product_id, string? lot)
    {
        try
        {
            using var connection = GetOpenConnection();


            StockInfo result = new StockInfo();

            if (string.IsNullOrWhiteSpace(lot))
            {
                // no lot
                var multi = connection.QueryMultiple(
                    @"SELECT * FROM store_stocks WHERE company_id = @company_id AND product_id = @product_id AND store_id = @store_id;
                      SELECT SUM(quantity) FROM store_stocks_engage WHERE company_id = @company_id AND product_id = @product_id AND store_id = @store_id AND canceled IS NULL AND date_unloaded IS NULL;",
                    new { company_id = company_id, store_id = store_id, product_id = product_id });
                var stock = multi.Read<store_stocks>().SingleOrDefault();
                result.ProductID = product_id;
                result.QuantityStock = stock != null ? stock.quantity_stock : 0;
                result.QuantityProduction = stock != null ? stock.quantity_production : 0;
                result.QuantityEngaged = multi.Read<decimal?>().Single() ?? 0m;
                result.QuantityOrdered = 0; // TODO
            }
            else
            {
                // check lot
                var multi = connection.QueryMultiple(
                    @"SELECT * FROM store_stocks_lots WHERE company_id = @company_id AND product_id = @product_id AND store_id = @store_id AND lot = @lot;
                      SELECT SUM(quantity) FROM store_stocks_engage WHERE company_id = @company_id AND product_id = @product_id AND store_id = @store_id AND canceled IS NULL AND date_unloaded IS NULL AND lot = @lot;",
                    new { company_id = company_id, store_id = store_id, product_id = product_id, lot = lot });
                var stock = multi.Read<store_stocks_lots>().SingleOrDefault();
                result.ProductID = product_id;
                result.Lot = lot;
                result.QuantityStock = stock != null ? stock.quantity_stock : 0;
                result.QuantityProduction = stock != null ? stock.quantity_production : 0;
                result.QuantityEngaged = multi.Read<decimal?>().Single() ?? 0m;
                result.QuantityOrdered = 0; // TODO
            }
            return result;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<StockInfo>? CheckAvailabilityByProduct(string company_id, string product_id, string? lot)
    {
        try
        {
            using var connection = GetOpenConnection();

            var customization = _aziendaRepository.Get(company_id);
            var result = new ObservableCollection<StockInfo>();
            if (string.IsNullOrWhiteSpace(lot))
            {
                var product = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(company_id, product_id);
                if (product != null)
                {
                    if (product.IsInfinite)
                    {
                        var defaultInfiniteStore = _store_StoresRepository.GetDefaultInfinite(company_id);
                        if (defaultInfiniteStore != null)
                        {
                            return new ObservableCollection<StockInfo>() { new StockInfo() { ProductID = product_id, Lot = Constants.NO_LOT_ID, QuantityStock = decimal.MaxValue, StoreID = defaultInfiniteStore.id, StoreDescription = defaultInfiniteStore.description } };
                        }
                        else
                        {
                            return new ObservableCollection<StockInfo>() { new StockInfo() { ProductID = product_id, Lot = Constants.NO_LOT_ID, QuantityStock = -1, StoreID = Constants.NO_LOT_ID, StoreDescription = "Manca il magazzino di default per i prodotti sempre disponibili" } };
                        }
                    }

                    if (product.HasLots)
                    {
                        // lot
                        result = new ObservableCollection<StockInfo>(connection.Query<StockInfo>(
                            @"SELECT s.product_id AS ProductID,s.lot AS Lot, s.expire AS Expire, s.store_id AS StoreID, s.goods_location as Ubication, d.description AS StoreDescription, s.quantity_stock AS QuantityStock, s.quantity_production AS QuantityProduction, (SELECT SUM(e.quantity) FROM store_stocks_engage AS e WHERE e.company_id = s.company_id AND e.product_id = s.product_id AND e.store_id = s.store_id AND e.lot = s.lot AND e.canceled IS NULL AND e.date_unloaded IS NULL) AS QuantityEngaged FROM store_stocks_lots AS s
                            INNER JOIN store_stores AS d ON d.company_id = s.company_id AND d.id = s.store_id
                            WHERE s.company_id = @company_id AND s.product_id = @product_id
                            ORDER BY s.expire ASC, s.lot ASC;",
                            new { company_id = company_id, product_id = product_id }).ToList());
                    }
                    else
                    {
                        // no lot
                        result = new ObservableCollection<StockInfo>(connection.Query<StockInfo>(
                            $@"SELECT s.product_id AS ProductID, '{Constants.NO_LOT_ID}' AS Lot, s.store_id AS StoreID, s.goods_location as Ubication, d.description AS StoreDescription, s.quantity_stock AS QuantityStock, s.quantity_production AS QuantityProduction, (SELECT SUM(e.quantity) FROM store_stocks_engage AS e WHERE e.company_id = s.company_id AND e.product_id = s.product_id AND e.store_id = s.store_id AND e.canceled IS NULL AND e.date_unloaded IS NULL) AS QuantityEngaged FROM store_stocks AS s
                                INNER JOIN store_stores AS d ON d.company_id = s.company_id AND d.id = s.store_id
                                WHERE s.company_id = @company_id AND s.product_id = @product_id;",
                            new { company_id = company_id, product_id = product_id }).ToList());
                    }
                }
            }
            else
            {
                // check lot
                result = new ObservableCollection<StockInfo>(connection.Query<StockInfo>(
                    @"SELECT s.product_id AS ProductID,s.lot AS Lot, s.expire AS Expire,s.store_id AS StoreID, s.goods_location as Ubication, d.description AS StoreDescription, s.quantity_stock AS QuantityStock, s.quantity_production AS QuantityProduction, (SELECT SUM(e.quantity) FROM store_stocks_engage AS e WHERE e.company_id = s.company_id AND e.product_id = s.product_id AND e.store_id = s.store_id AND e.lot = s.lot AND e.canceled IS NULL AND e.date_unloaded IS NULL) AS QuantityEngaged FROM store_stocks_lots AS s
                            INNER JOIN store_stores AS d ON d.company_id = s.company_id AND d.id = s.store_id
                            WHERE s.company_id = @company_id AND s.product_id = @product_id AND s.lot = @lot
                            ORDER BY s.expire ASC, s.lot ASC;",
                    new { company_id = company_id, product_id = product_id, lot = lot }));
            }
            // QuantityOrdered

            foreach (var item in result)
            {
                item.IncludeOrdered = (customization != null) ? customization.ordered_as_available : false;
                item.QuantityOrdered = _acq_orders_rowsRepository.GetInOrder(company_id, item.ProductID!);
            }
            return new ObservableCollection<StockInfo>(result.Where(w => w.QuantityAvailable > 0));
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<StockCheckExistance.Lots>? CheckExistanceLots(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();



            var list = connection.Query<StockCheckExistance.Lots>(
                $@"SELECT s.*, 
                                sto.description, 
                                p.Descrizione, 
                                p.UnitaID, 
                                load_lot.qty as LotLoad, 
                                unload_lot.qty as LotUnload,
                                load_lot.qty-unload_lot.qty as LotMovs, 
                                load.qty as  AllLoad,
                                unload.qty as AllUnload,
                                load.qty-unload.qty as AllMovs, 
                                stock.qty as StoreQty 
                                FROM store_stocks_lots AS s
                        INNER JOIN store_stores AS sto ON sto.company_id = s.company_id AND sto.id = s.store_id
                        INNER JOIN tab_articolo AS p ON p.SocietaID = s.company_id AND p.ID = s.product_id
						OUTER APPLY(SELECT COALESCE(SUM(m.quantity),0) as qty FROM store_movements AS m
                            INNER JOIN store_causals as cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                            WHERE m.company_id = s.company_id AND m.store_id = s.store_id AND m.product_id = s.product_id AND m.lot = s.lot AND cau.sign = '+' AND m.canceled IS NULL) as load_lot
						OUTER APPLY(SELECT COALESCE(SUM(m.quantity),0) as qty FROM store_movements AS m
                            INNER JOIN store_causals as cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                            WHERE m.company_id = s.company_id AND m.store_id = s.store_id AND m.product_id = s.product_id AND m.lot = s.lot AND cau.sign = '-' AND m.canceled IS NULL) as unload_lot
						OUTER APPLY(SELECT COALESCE(SUM(m.quantity),0) as qty FROM store_movements AS m
                            INNER JOIN store_causals as cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                            WHERE m.company_id = s.company_id AND m.store_id = s.store_id AND m.product_id = s.product_id AND cau.sign = '+' AND m.canceled IS NULL) as load
						OUTER APPLY(SELECT COALESCE(SUM(m.quantity),0) as qty FROM store_movements AS m
                            INNER JOIN store_causals as cau ON cau.company_id = m.company_id AND cau.id = m.causal_id
                            WHERE m.company_id = s.company_id AND m.store_id = s.store_id AND m.product_id = s.product_id  AND cau.sign = '-' AND m.canceled IS NULL) as unload
						OUTER APPLY(SELECT COALESCE(SUM(m.quantity_stock),0) as qty FROM store_stocks AS m
                            WHERE m.company_id = s.company_id AND m.store_id = s.store_id AND m.product_id = s.product_id) as stock
                        WHERE s.company_id = @company_id and p.HasLots = 1 and p.IsInfinite = 0"
                , new[] { typeof(store_stocks_lots), typeof(string), typeof(string), typeof(string), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal), typeof(decimal) }
                , (objects) =>
                {
                    var lot = objects[0] as store_stocks_lots;

                    var ret = new StockCheckExistance.Lots
                    {
                        CompanyID = company_id,
                        Lot = lot!.lot,
                        ProductID = lot!.product_id,
                        ProductDescription = objects[2] as string,
                        StoreID = lot!.store_id,
                        StoreDescription = objects[1] as string,
                        QuantityLot = lot!.quantity_stock ?? 0,
                        QuantityStore = (objects[10] as decimal?) ?? 0,
                        QuantityMovementsLot = (objects[6] as decimal?) ?? 0,
                        QuantityMovementsStore = (objects[9] as decimal?) ?? 0,
                        GapLot = (lot!.quantity_stock ?? 0) - ((objects[6] as decimal?) ?? 0),
                        GapStore = ((objects[10] as decimal?) ?? 0) - ((objects[9] as decimal?) ?? 0),
                        GapLotStore = (lot!.quantity_stock ?? 0) - ((objects[10] as decimal?) ?? 0)
                    };

                    return ret;
                },
                new { company_id = company_id }, splitOn: "description,Descrizione,UnitaID,LotLoad,LotUnload,LotMovs,AllLoad,AllUnload,AllMovs,StoreQty").ToList();

            return new ObservableCollection<StockCheckExistance.Lots>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<StockCheckExistance.NoLots>? CheckExistanceNoLots(string company_id)
    {
        try
        {
            using var connection = GetOpenConnection();



            var retValue = new ObservableCollection<StockCheckExistance.NoLots>();

            var list = connection.Query<store_stocks, store_stores, tab_articolo, store_stocks>(
                $@"SELECT s.*, sto.description, p.Descrizione, p.HasLots FROM store_stocks AS s
                        INNER JOIN store_stores AS sto ON sto.company_id = s.company_id AND sto.id = s.store_id
                        INNER JOIN tab_articolo AS p ON p.SocietaID = s.company_id AND p.ID = s.product_id
                        WHERE s.company_id = @company_id and p.HasLots = 0 and p.IsInfinite = 0",
                (stk, sto, prd) =>
                {
                    stk.StoreDescription = sto.description;
                    stk.ProductDescription = prd.Descrizione;
                    stk.product_haslots = prd.HasLots;

                    return stk;
                },
                new { company_id = company_id }, splitOn: "description,Descrizione").ToList();

            foreach (var ex in list)
            {

                var loads = connection.Query<decimal>(
                    @"SELECT COALESCE(SUM(s.quantity),0) FROM store_movements AS s
                            INNER JOIN store_causals as cau ON cau.company_id = s.company_id AND cau.id = s.causal_id
                            WHERE s.company_id = @company_id AND s.store_id = @store_id AND s.product_id = @product_id AND cau.sign = '+' AND s.canceled IS NULL",
                   new { company_id = company_id, store_id = ex.store_id, product_id = ex.product_id }).FirstOrDefault();

                var unloads = connection.Query<decimal>(
                    @"SELECT COALESCE(SUM(s.quantity),0) FROM store_movements AS s
                            INNER JOIN store_causals as cau ON cau.company_id = s.company_id AND cau.id = s.causal_id
                            WHERE s.company_id = @company_id AND s.store_id = @store_id AND s.product_id = @product_id AND cau.sign = '-' AND s.canceled IS NULL",
                    new { company_id = company_id, store_id = ex.store_id, product_id = ex.product_id }).FirstOrDefault();

                retValue.Add(new StockCheckExistance.NoLots
                {
                    CompanyID = company_id,
                    ProductID = ex.product_id,
                    ProductDescription = ex.ProductDescription,
                    StoreID = ex.store_id,
                    StoreDescription = ex.StoreDescription,
                    Quantity = ex.quantity_stock ?? 0,
                    QuantityMovements = loads - unloads,
                });
            }

            return retValue;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Align(string CompanyID, string StoreID, string ProductID, string Lot)
    {
        try
        {
            using var connection = GetOpenConnection();


            var loadsStore = connection.Query<decimal>(
                      @"SELECT COALESCE(SUM(s.quantity),0) FROM store_movements AS s
                                INNER JOIN store_causals as cau ON cau.company_id = s.company_id AND cau.id = s.causal_id
                                WHERE s.company_id = @company_id AND s.store_id = @store_id AND s.product_id = @product_id  AND cau.sign = '+' AND s.canceled IS NULL",
                     new { company_id = CompanyID, store_id = StoreID, product_id = ProductID }).FirstOrDefault();

            var unloadsStore = connection.Query<decimal>(
                @"SELECT COALESCE(SUM(s.quantity),0) FROM store_movements AS s
                                INNER JOIN store_causals as cau ON cau.company_id = s.company_id AND cau.id = s.causal_id
                                WHERE s.company_id = @company_id AND s.store_id = @store_id AND s.product_id = @product_id  AND cau.sign = '-' AND s.canceled IS NULL",
                new { company_id = CompanyID, store_id = StoreID, product_id = ProductID }).FirstOrDefault();

            //UPDATE STORE
            var totalMovementsStore = loadsStore - unloadsStore;

            var updateStore = connection.ExecuteScalar("UPDATE store_stocks SET quantity_stock = @quantity WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id",
                new { quantity = totalMovementsStore, company_id = CompanyID, store_id = StoreID, product_id = ProductID });

            if (!string.IsNullOrEmpty(Lot))
            {
                var loadsLot = connection.Query<decimal>(
                      @"SELECT COALESCE(SUM(s.quantity),0) FROM store_movements AS s
                                INNER JOIN store_causals as cau ON cau.company_id = s.company_id AND cau.id = s.causal_id
                                WHERE s.company_id = @company_id AND s.store_id = @store_id AND s.product_id = @product_id AND s.lot = @lot AND cau.sign = '+' AND s.canceled IS NULL",
                     new { company_id = CompanyID, store_id = StoreID, product_id = ProductID, lot = Lot }).FirstOrDefault();

                var unloadsLot = connection.Query<decimal>(
                    @"SELECT COALESCE(SUM(s.quantity),0) FROM store_movements AS s
                                INNER JOIN store_causals as cau ON cau.company_id = s.company_id AND cau.id = s.causal_id
                                WHERE s.company_id = @company_id AND s.store_id = @store_id AND s.product_id = @product_id AND s.lot = @lot AND cau.sign = '-' AND s.canceled IS NULL",
                    new { company_id = CompanyID, store_id = StoreID, product_id = ProductID, lot = Lot }).FirstOrDefault();

                //UPDATE LOTS
                var totalMovementsLot = loadsLot - unloadsLot;

                var updateLot = connection.ExecuteScalar("UPDATE store_stocks_lots SET quantity_stock = @quantity WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND lot = @lot",
                    new { quantity = totalMovementsLot, company_id = CompanyID, store_id = StoreID, product_id = ProductID, lot = Lot });
            }


            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    #region Reports
    public StocksListReport? PrintStocks(string CompanyID, bool ShowInfinite, bool ShowZeroStocks, bool ShowLots, bool ShowMovements, bool ShowEngages, bool ShowMovementsCanceled, bool ShowEngagesUnloaded, string? SelectedStoreID = null, string? SelectedProductTypeID = null, string? SelectedProductID = null, DateTime? PrintUntil = null)
    {
        try
        {
            using var connection = GetOpenConnection();

            var socbase = _companyRepository.Get(CompanyID);

            if (socbase != null)
            {
                var result = new StocksListReport()
                {
                    CompanyInfo = _aziendaRepository.Get(CompanyID),
                    LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                    Stores = new List<StoreInfo>()
                };

                var list = connection.Query<store_stocks, store_stores, tab_articolo, tab_articolo_tipo, decimal?, store_stocks>(
                    $@"SELECT s.*, sto.description, p.Descrizione, p.UnitaID, p.IsInfinite, pt.Descrizione, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.product_id = s.product_id AND eng.canceled IS NULL AND eng.date_unloaded IS NULL AND eng.store_id = s.store_id) AS engaged FROM store_stocks AS s
                        INNER JOIN store_stores AS sto ON sto.company_id = s.company_id AND sto.id = s.store_id
                        INNER JOIN tab_articolo AS p ON p.SocietaID = s.company_id AND p.ID = s.product_id
                        INNER JOIN tab_articolo_tipo AS pt ON pt.SocietaID = s.company_id AND pt.ID = p.TipoID
                        WHERE s.company_id = @company_id AND p.IsInfinite = 0
                        {(!string.IsNullOrWhiteSpace(SelectedStoreID) ? "AND s.store_id = @store_id" : null)}
                        {(!string.IsNullOrWhiteSpace(SelectedProductTypeID) ? "AND p.TipoID = @ptid" : null)}
                        {(!string.IsNullOrWhiteSpace(SelectedProductID) ? "AND p.ID = @pid" : null)}
                        ORDER BY s.store_id, s.product_id",
                    (stk, sto, prd, prdt, eng) =>
                    {
                        stk.StoreDescription = sto.description;
                        stk.ProductDescription = prd.Descrizione;
                        stk.ProductTypeDescription = prdt.Descrizione;
                        stk.UM = prd.UnitaID;
                        stk.IsInfinite = prd.IsInfinite;
                        stk.Info.QuantityStock = stk.quantity_stock;
                        stk.Info.QuantityProduction = stk.quantity_production;
                        stk.Info.QuantityEngaged = eng ?? 0;
                        return stk;
                    },
                    new { company_id = CompanyID, store_id = SelectedStoreID, ptid = SelectedProductTypeID, pid = SelectedProductID }, splitOn: "description,Descrizione,Descrizione,engaged").ToList();

                #region add always available
                if (ShowInfinite)
                {
                    // get always available default store
                    var infiniteStore = _store_StoresRepository.GetDefaultInfinite(CompanyID);
                    if (infiniteStore != null)
                    {
                        foreach (var stock in connection.Query<store_stocks, tab_articolo, decimal?, store_stocks>(
                            @"SELECT 1, p.ID, p.Descrizione, p.UnitaID, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = @cid AND eng.product_id = p.ID AND eng.canceled IS NULL AND eng.date_unloaded IS NULL AND eng.store_id = @isid) AS engaged FROM tab_articolo AS p
                        WHERE p.SocietaID = @cid AND p.IsInfinite = 1 AND p.LogCanceled IS NULL",
                                (stk, prd, eng) =>
                                {
                                    stk.company_id = CompanyID;
                                    stk.store_id = infiniteStore.id;
                                    stk.product_id = prd.ID;
                                    stk.ProductDescription = prd.Descrizione;
                                    stk.StoreDescription = infiniteStore?.description;
                                    stk.UM = prd.UnitaID;
                                    stk.Info.QuantityStock = int.MaxValue;
                                    stk.Info.QuantityEngaged = eng ?? 0;
                                    stk.IsInfinite = true;
                                    return stk;
                                },
                                new { cid = CompanyID, isid = infiniteStore?.id }, splitOn: "ID,engaged"))
                        {
                            list.Add(stock);
                        }
                    }
                }
                #endregion

                // get ordered
                var orderedRows = _acq_orders_rowsRepository.GetListNotClosed(CompanyID);

                foreach (var item in list.OrderBy(o => o.store_id).ThenBy(tb => tb.product_id).GroupBy(g => g.store_id, (key, items) => new { key, items }))
                {
                    var newStore = new StoreInfo()
                    {
                        StoreID = item.key,
                        StoreDescription = item.items.First().StoreDescription,
                        ProductInfos = new System.Collections.Generic.List<ProductInfo>()
                    };

                    foreach (var prod in item.items)
                    {
                        if (ShowZeroStocks || (!ShowZeroStocks && (prod.Info.QuantityStock ?? 0) > 0))
                        {
                            var ordered = orderedRows?.Where(w => w.store_id == item.key && w.product_id == prod.product_id).FirstOrDefault();
                            var article = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(CompanyID, prod.product_id);

                            var newProd = new ProductInfo()
                            {
                                ID = prod.product_id,
                                Description = prod.ProductDescription,
                                ProductTypeDescription = prod.ProductTypeDescription,
                                UM = prod.UM,
                                Available = prod.IsInfinite ? "∞" : prod.Info.QuantityAvailable.ToString("N6"),
                                Stock = prod.Info.QuantityStock.HasValue ? (prod.IsInfinite ? "∞" : prod.Info.QuantityStock.Value.ToString("N6")) : "0,000000",
                                Ordered = ordered != null ? ((ordered.quantity ?? 0) - (ordered.quantity_received ?? 0)) : 0,
                                Engaged = prod.Info.QuantityEngaged ?? 0,
                                Lots = ShowLots ? new List<LotInfo>() : null,
                                Movements = ShowMovements ? new List<store_movements>() : null,
                                Engages = ShowEngages ? new List<store_stocks_engage>() : null,
                                NetWeight = article?.NetWeight ?? 0,
                                GrossWeight = article?.GrossWeight ?? 0,
                            };

                            if (ShowLots)
                            {
                                foreach (var lot in _store_stocks_lotsRepository.GetList(CompanyID, item.key, prod.product_id, true) ?? new ObservableCollection<store_stocks_lots>())
                                {
                                    newProd.Lots!.Add(new LotInfo()
                                    {
                                        Lot = lot.lot,
                                        SupplierLot = lot.supplier_lot,
                                        UM = prod.UM,
                                        Location = lot.goods_location,
                                        Expire = lot.expire.HasValue ? lot.expire.Value.ToString("dd/MM/yyyy") : null,
                                        Available = prod.IsInfinite ? "∞" : lot.AvailableQuantity.ToString("N6"),
                                        Stock = lot.quantity_stock.HasValue ? (prod.IsInfinite ? "∞" : lot.quantity_stock.Value.ToString("N6")) : "0,000000",
                                        Engaged = lot.EngagedQuantity ?? 0
                                    });
                                }
                            }

                            List<store_movements>? movements = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GetList(CompanyID, prod.product_id, item.key, ShowMovementsCanceled, PrintUntil);

                            if (PrintUntil.HasValue && !prod.IsInfinite)
                            {
                                if (movements != null)
                                    newProd.Available = ((movements.Where(o => o.Causal?.sign == "+").Sum(s => s.quantity) ?? 0) - (movements.Where(o => o.Causal?.sign == ".").Sum(s => s.quantity) ?? 0)).ToString("N6");
                            }

                            if (ShowMovements)
                            {
                                newProd.Movements = movements;
                            }
                            if (ShowEngages)
                            {
                                newProd.Engages = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetList(CompanyID, prod.product_id, item.key, ShowEngagesUnloaded);
                            }
                            newStore.ProductInfos.Add(newProd);
                        }
                    }
                    result.Stores.Add(newStore);
                }
                return result;
            }

            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO store_stocks (company_id,store_id,product_id,quantity_stock,quantity_production,quantity_ordered,goods_location) OUTPUT INSERTED.rv VALUES(@company_id,@store_id,@product_id,@quantity_stock,@quantity_production,@quantity_ordered,@goods_location)";
    public string UPDATE_QUERY => "UPDATE store_stocks SET company_id = @company_id,store_id = @store_id,product_id = @product_id,quantity_stock = @quantity_stock,quantity_production = @quantity_production,quantity_ordered = @quantity_ordered,goods_location = @goods_location OUTPUT INSERTED.rv WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM store_stocks OUTPUT DELETED.rv WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND rv = @rv";
    public bool Insert(store_stocks Model)
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

    public bool Update(store_stocks Model)
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

    public bool Delete(store_stocks Model)
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

    public string? Validate(store_stocks Model, bool IsInsert)
    {
        try
        {
            if (true)
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}