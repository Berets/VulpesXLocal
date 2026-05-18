
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using VulpesX.DAL.Tables.Accounting;

namespace VulpesX.DAL.Store;

public interface Istore_stocks_engageRepository
{
    ObservableCollection<store_stocks_engage>? GetList(string CompanyID, string ProductID, bool ShowCancelled = false);

    ObservableCollection<store_stocks_engage>? GetList(string CompanyID, string ProductID, long ProductionOrderID, bool ShowCancelled = false);

    ObservableCollection<store_stocks_engage>? GetSimpleListByProductionOrder(string CompanyID, long ProductionOrderID);

    List<store_stocks_engage>? GetList(string CompanyID, string ProductID, string StoreID, bool ShowCancelled = false);

    ObservableCollection<store_stocks_engage>? GetActiveList(string CompanyID, string ProductID);

    ObservableCollection<store_stocks_engage>? GetListByDDT(string CompanyID, int DDTYear, int DDTNumber);

    ObservableCollection<store_stocks_engage>? GetListByOrderID(string CompanyID, string ProductionOrderID);

    ObservableCollection<store_stocks_engage>? GetListByCustomerOrderID(string CompanyID, int Year, int Number, int RowID);

    decimal GetQuantityEngagedByDDT(string CompanyID, string ProductID, string StoreID, string Lot, int DDTYear, int DDTNumber);

    ObservableCollection<store_stocks_engage>? GetListByOrderIDForPrint(string CompanyID, string ProductionOrderID);

    store_stocks_engage? Get(string company_id, long id);

    store_stocks_engage? GetByCustomerOrderID(string company_id, int OrderYear, int OrderNumber, int OrderRow, string ProductID, string Lot);

    store_stocks_engage? GetByOrderID(string company_id, string ProductionOrderID, string ProductID, string Lot);

    store_stocks_engage? GetByOrderIDEveryLot(string company_id, string ProductionOrderID, string ProductID);

    bool FreeByProductionOrder(string CompanyID, string ProductionOrderID, string UserID);

    bool Exists(string company_id, long id);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(store_stocks_engage Model);

    bool Update(store_stocks_engage Model);

    bool Unload(store_stocks_engage Model, store_causals Causal, string CompanyID, string UserID, IDbConnection? Connection = null, IDbTransaction? Transaction = null);

    bool Delete(store_stocks_engage Model);

    string? Validate(store_stocks_engage Model, bool IsInsert, bool IsUnloading, decimal OlderQuantity = 0);
    #endregion
}

public class store_stocks_engageRepository : RepositoryBase, Istore_stocks_engageRepository
{
    private readonly Istore_stocks_lotsRepository _store_stocks_lotsRepository;
    private readonly Istore_movementsRepository _store_movementsRepository;
    private readonly Istore_stocksRepository _store_stocksRepository;
    private readonly INUMREGRepository _numregRepository;
    public store_stocks_engageRepository(IConnectionFactory factory, Istore_stocks_lotsRepository Istore_stocks_lotsRepository, Istore_movementsRepository Istore_movementsRepository, Istore_stocksRepository Istore_stocksRepository,
        INUMREGRepository INUMREGRepository) : base(factory)
    {
        _store_stocks_lotsRepository = Istore_stocks_lotsRepository;
        _store_movementsRepository = Istore_movementsRepository;
        _store_stocksRepository = Istore_stocksRepository;
        _numregRepository = INUMREGRepository;
    }

    public ObservableCollection<store_stocks_engage>? GetList(string CompanyID, string ProductID, bool ShowCancelled = false)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_engage, store_stores, tab_articolo, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.* FROM store_stocks_engage AS m
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id {(ShowCancelled ? null : "AND m.canceled IS NULL")}
                        ORDER BY m.date_engaged DESC, m.store_id ASC",
                (eng, sto, prd) =>
                {
                    eng.StoreFullDescription = sto.FullDescriptionSearchable;
                    eng.ProductFullDescription = prd.FullDescriptionSearchable;
                    eng.UM = prd.UnitaID;
                    return eng;
                },
                new { company_id = CompanyID, product_id = ProductID }, splitOn: "company_id,SocietaID");

            return new ObservableCollection<store_stocks_engage>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetList(string CompanyID, string ProductID, long ProductionOrderID, bool ShowCancelled = false)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_engage, store_stores, tab_articolo, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.* FROM store_stocks_engage AS m
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id AND m.order_id = @order_id {(ShowCancelled ? null : "AND m.canceled IS NULL")}
                        ORDER BY m.date_engaged DESC, m.store_id ASC",
                (eng, sto, prd) =>
                {
                    eng.StoreFullDescription = sto.FullDescriptionSearchable;
                    eng.ProductFullDescription = prd.FullDescriptionSearchable;
                    eng.UM = prd.UnitaID;
                    return eng;
                },
                new { company_id = CompanyID, product_id = ProductID, order_id = ProductionOrderID }, splitOn: "company_id,SocietaID");

            return new ObservableCollection<store_stocks_engage>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetSimpleListByProductionOrder(string CompanyID, long ProductionOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_engage, store_stores, tab_articolo, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.* FROM store_stocks_engage AS m
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        WHERE m.company_id = @company_id AND m.order_id = @order_id AND m.canceled IS NULL AND m.date_unloaded IS NULL",
                (eng, sto, prd) =>
                {
                    eng.StoreFullDescription = sto.FullDescriptionSearchable;
                    eng.ProductFullDescription = prd.FullDescriptionSearchable;
                    eng.UM = prd.UnitaID;
                    return eng;
                },
                new { company_id = CompanyID, order_id = ProductionOrderID }, splitOn: "company_id,SocietaID");

            return new ObservableCollection<store_stocks_engage>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<store_stocks_engage>? GetList(string CompanyID, string ProductID, string StoreID, bool ShowCancelled = false)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_engage>(
                $@"SELECT m.*, p.UnitaID AS UM FROM store_stocks_engage AS m
                        INNER JOIN tab_articolo AS p ON p.SocietaID=m.company_id AND p.ID=m.product_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id AND m.store_id = @store_id {(ShowCancelled ? null : "AND m.canceled IS NULL AND m.date_unloaded IS NULL")}
                        ORDER BY m.date_engaged DESC",
                new { company_id = CompanyID, product_id = ProductID, store_id = StoreID });

            return list.ToList();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetActiveList(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<store_stocks_engage, store_stores, tab_articolo, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.* FROM store_stocks_engage AS m
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        WHERE m.company_id = @company_id AND m.product_id = @product_id AND m.canceled IS NULL AND date_unloaded IS NULL
                        ORDER BY m.date_engaged DESC, m.store_id ASC",
                (eng, sto, prd) =>
                {
                    eng.StoreFullDescription = sto.FullDescriptionSearchable;
                    eng.ProductFullDescription = prd.FullDescriptionSearchable;
                    eng.UM = prd.UnitaID;
                    return eng;
                },
                new { company_id = CompanyID, product_id = ProductID }, splitOn: "company_id,SocietaID");

            return new ObservableCollection<store_stocks_engage>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetListByDDT(string CompanyID, int DDTYear, int DDTNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_engage>(
                $@"SELECT m.* FROM store_stocks_engage AS m
                        WHERE m.company_id = @company_id AND m.ddt_year = @ddty AND m.ddt_number = @ddtn AND m.canceled IS NULL AND m.date_unloaded IS NULL",
                new { company_id = CompanyID, ddty = DDTYear, ddtn = DDTNumber });

            return new ObservableCollection<store_stocks_engage>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetListByOrderID(string CompanyID, string ProductionOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return new ObservableCollection<store_stocks_engage>(connection.Query<store_stocks_engage, store_stores, tab_articolo, string, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.*, lot.goods_location  FROM store_stocks_engage AS m 
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        LEFT OUTER JOIN store_stocks_lots as lot ON m.company_id = lot.company_id AND m.store_id = lot.store_id AND m.product_id = lot.product_id AND m.lot = lot.lot
                        WHERE m.company_id = @company_id AND m.order_id = @oid AND m.quantity > 0 AND m.canceled IS NULL AND m.date_unloaded IS NULL",
               (eng, sto, prd, lot) =>
               {
                   eng.StoreFullDescription = sto.FullDescriptionSearchable;
                   eng.ProductFullDescription = prd.FullDescriptionSearchable;
                   eng.UM = prd.UnitaID;
                   eng.GoodsLocation = lot;
                   return eng;
               },
                new { company_id = CompanyID, oid = ProductionOrderID }, splitOn: "company_id,SocietaID,goods_location"));
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetListByCustomerOrderID(string CompanyID, int Year, int Number, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return new ObservableCollection<store_stocks_engage>(connection.Query<store_stocks_engage, store_stores, tab_articolo, string, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.*, lot.goods_location  FROM store_stocks_engage AS m 
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        LEFT OUTER JOIN store_stocks_lots as lot ON m.company_id = lot.company_id AND m.store_id = lot.store_id AND m.product_id = lot.product_id AND m.lot = lot.lot
                        WHERE m.company_id = @company_id AND m.order_year = @oyea AND m.order_number = @onum AND m.order_row = @orow AND m.quantity > 0 AND m.canceled IS NULL AND m.date_unloaded IS NULL",
               (eng, sto, prd, lot) =>
               {
                   eng.StoreFullDescription = sto.FullDescriptionSearchable;
                   eng.ProductFullDescription = prd.FullDescriptionSearchable;
                   eng.UM = prd.UnitaID;
                   eng.GoodsLocation = lot;
                   return eng;
               },
                new { company_id = CompanyID, oyea = Year, onum = Number, orow = RowID }, splitOn: "company_id,SocietaID,goods_location"));
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public decimal GetQuantityEngagedByDDT(string CompanyID, string ProductID, string StoreID, string Lot, int DDTYear, int DDTNumber)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (decimal?)connection.ExecuteScalar(
                $@"SELECT SUM(quantity) FROM store_stocks_engage
                        WHERE company_id = @company_id AND product_id = @pid AND store_id = @sid AND lot = @lot AND ddt_year = @ddty AND ddt_number = @ddtn AND canceled IS NULL",
                new { company_id = CompanyID, pid = ProductID, sid = StoreID, lot = Lot, ddty = DDTYear, ddtn = DDTNumber }) ?? 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }

    public ObservableCollection<store_stocks_engage>? GetListByOrderIDForPrint(string CompanyID, string ProductionOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return new ObservableCollection<store_stocks_engage>(connection.Query<store_stocks_engage, store_stores, tab_articolo, string, store_stocks_engage>(
                $@"SELECT m.*, sto.*, prd.*, lot.goods_location  FROM store_stocks_engage AS m 
                        INNER JOIN store_stores AS sto ON sto.company_id = m.company_id AND sto.id = m.store_id
                        INNER JOIN tab_articolo AS prd ON prd.SocietaID = m.company_id AND prd.ID = m.product_id
                        LEFT OUTER JOIN store_stocks_lots as lot ON m.company_id = lot.company_id AND m.store_id = lot.store_id AND m.product_id = lot.product_id AND m.lot = lot.lot
                        WHERE m.company_id = @company_id AND m.order_id = @oid AND m.quantity > 0 AND m.canceled IS NULL",
               (eng, sto, prd, lot) =>
               {
                   eng.StoreFullDescription = sto.FullDescriptionSearchable;
                   eng.ProductFullDescription = prd.FullDescriptionSearchable;
                   eng.UM = prd.UnitaID;
                   eng.GoodsLocation = lot;
                   return eng;
               },
                new { company_id = CompanyID, oid = ProductionOrderID }, splitOn: "company_id,SocietaID,goods_location"));
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stocks_engage? Get(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_stocks_engage>(
                "SELECT * FROM store_stocks_engage WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stocks_engage? GetByCustomerOrderID(string company_id, int OrderYear, int OrderNumber, int OrderRow, string ProductID, string Lot)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_stocks_engage>(
                $@"SELECT * FROM store_stocks_engage 
                        WHERE company_id = @company_id AND order_year = @yea AND order_number = @num AND order_row = @row AND product_id = @pid AND canceled IS NULL AND {(!string.IsNullOrWhiteSpace(Lot) ? "lot = @lot" : "lot IS NULL ")}",
                new { company_id = company_id, yea = OrderYear, num = OrderNumber, row = OrderRow, pid = ProductID, lot = Lot })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stocks_engage? GetByOrderID(string company_id, string ProductionOrderID, string ProductID, string Lot)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<store_stocks_engage>(
                $@"SELECT * FROM store_stocks_engage 
                        WHERE company_id = @company_id AND order_id = @oid AND product_id = @pid AND canceled IS NULL AND {(!string.IsNullOrWhiteSpace(Lot) ? "lot = @lot" : "lot IS NULL ")}",
                new { company_id = company_id, oid = ProductionOrderID, pid = ProductID, lot = Lot })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stocks_engage? GetByOrderIDEveryLot(string company_id, string ProductionOrderID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();
            return connection.Query<store_stocks_engage>(
                $@"SELECT * FROM store_stocks_engage 
                        WHERE company_id = @company_id AND order_id = @oid AND product_id = @pid AND canceled IS NULL",
                new { company_id = company_id, oid = ProductionOrderID, pid = ProductID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool FreeByProductionOrder(string CompanyID, string ProductionOrderID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_engage>(
                $@"UPDATE store_stocks_engage 
                        SET canceled = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time', cancel_user=@userID
                        WHERE company_id = @company_id AND order_id = @order_id AND canceled IS NULL",
                new { company_id = CompanyID, order_id = ProductionOrderID, userID = UserID });

            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Exists(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM store_stocks_engage WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO store_stocks_engage (company_id,id,store_id,product_id,job_id,order_id,ward_id,document_id,quantity,date_engaged,date_unloaded,added,add_user,updated,update_user,canceled,cancel_user,lot,ddt_year,ddt_number,ddt_row,order_year,order_number,order_row) OUTPUT INSERTED.rv VALUES(@company_id,@id,@store_id,@product_id,@job_id,@order_id,@ward_id,@document_id,@quantity,@date_engaged,@date_unloaded,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@add_user,@updated,@update_user,@canceled,@cancel_user,@lot,@ddt_year,@ddt_number,@ddt_row,@order_year,@order_number,@order_row)";
    public string UPDATE_QUERY => "UPDATE store_stocks_engage SET company_id = @company_id,id = @id,store_id = @store_id,product_id = @product_id,job_id = @job_id,order_id = @order_id,ward_id = @ward_id,document_id = @document_id,quantity = @quantity,date_engaged = @date_engaged,date_unloaded = @date_unloaded,added = @added,add_user = @add_user,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',update_user = @update_user,canceled = @canceled,cancel_user = @cancel_user,lot = @lot,ddt_year = @ddt_year,ddt_number = @ddt_number,ddt_row = @ddt_row,order_year = @order_year,order_number = @order_number,order_row = @order_row OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM store_stocks_engage OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(store_stocks_engage Model)
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

    public bool Update(store_stocks_engage Model)
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

    public bool Unload(store_stocks_engage Model, store_causals Causal, string CompanyID, string UserID, IDbConnection? Connection = null, IDbTransaction? Transaction = null)
    {
        try
        {
            IDbConnection connection = Connection == null ? GetOpenConnection() : Connection;

            var transaction = (Transaction != null ? Transaction : connection.BeginTransaction());
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                // write movement and update stock
                // get lot infos
                var lot = _store_stocks_lotsRepository.Get(Model.company_id, Model.store_id, Model.product_id, Model.lot!);
                var ddt = connection.Query<BOLLT00F>(@"SELECT * FROM BOLLT00F
                                                            WHERE bolsoc=@cid AND BTANNO=@yea AND BTBOLL=@num", new { cid = Model.company_id, yea = (Model.ddt_year ?? 0), num = (Model.ddt_number ?? 0) }, transaction).FirstOrDefault();
                var movement = new store_movements()
                {
                    company_id = CompanyID,
                    id = _numregRepository.GetFullLongID(now.Year, _numregRepository.GetNumber(CompanyID, now.Year, Constants.STORE_MOVEMENTS, true)),
                    date = Model.date_unloaded,
                    causal_id = Causal.id,
                    Causal = Causal,
                    document_date = ddt != null ? ddt.BTDATA : null,
                    document_year = Model.ddt_year,
                    document_id = Model.ddt_number?.ToString(),
                    document_row = Model.ddt_row,
                    order_id = Model.order_id,
                    product_id = Model.product_id,
                    quantity = Model.quantity,
                    store_id = Model.store_id,
                    added = now,
                    engage_id = Model.id,
                    add_user = UserID,
                    lot = Model.lot,
                    goods_location = lot != null ? lot.goods_location : null,
                    expire = lot != null ? lot.expire : null
                };
                _store_movementsRepository.Insert(movement, connection, transaction);
                // update engage
                Model.update_user = UserID;
                var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                if (result != null)
                {
                    if (Transaction == null)
                        transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(store_stocks_engage Model)
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

    public string? Validate(store_stocks_engage Model, bool IsInsert, bool IsUnloading, decimal OlderQuantity = 0)
    {
        try
        {
            if (true)
            {
                if (Model.id > 0)
                {
                    if ((IsInsert && Model.date_engaged.HasValue) || !IsInsert)
                    {
                        if ((IsUnloading && Model.date_unloaded.HasValue) || !IsUnloading)
                        {
                            if (Model.quantity > 0)
                            {
                                var quantityAvailable = _store_stocksRepository.CheckAvailabilityByStore(Model.company_id, Model.store_id, Model.product_id, Model.lot)?.QuantityAvailable;
                                if ((IsInsert && Model.quantity <= quantityAvailable) || (!IsInsert && (OlderQuantity >= Model.quantity || quantityAvailable >= Model.quantity - OlderQuantity)))
                                {
                                    return null;
                                }
                                else
                                {
                                    return $"La quantita' attualmente disponibile non e' sufficiente per eseguire l'operazione.\nLa quantita' disponibile su questo magazzino e' {(IsInsert ? quantityAvailable?.ToString("N6") : (quantityAvailable + OlderQuantity)?.ToString("N6"))} {Model.UM}";
                                }
                            }
                            else
                            { return "La quantita' e' obbligatoria"; }
                        }
                        else
                        { return "La data di scarico e' obbligatoria"; }
                    }
                    else
                    { return "La data di impegno e' obbligatoria"; }
                }
                else
                { return "Il progressivo non e' valido"; }
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}