using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using VulpesX.DAL;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports;

namespace VulpesX.DAL.Store;


public interface Istore_stocks_lotsRepository
{
    ObservableCollection<store_stocks_lots>? GetList(string CompanyID, string StoreID, string ProductID, bool ShowZeroLots);

    ObservableCollection<store_stocks_lots>? GetListByProduct(string CompanyID, string ProductID);

    ObservableCollection<store_stocks_lots>? GetSimpleListByLot(string CompanyID, string Lot);

    store_stocks_lots? Get(string company_id, string store_id, string product_id, string lot);

    bool Exists(string company_id, string store_id, string product_id, string lot);

    #region Printing
    LotLabelReport? PrintLotLabel(string company_id, string store_id, string product_id, string lot);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(store_stocks_lots Model);

    bool Update(store_stocks_lots Model);

    bool Delete(store_stocks_lots Model);

    string? Validate(store_stocks_lots Model, bool IsInsert);
    #endregion
}

public class store_stocks_lotsRepository : RepositoryBase, Istore_stocks_lotsRepository
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAZIENDARepository _aziendaRepository;
    public store_stocks_lotsRepository(IConnectionFactory factory,  ICompanyRepository ICompanyRepository, IAZIENDARepository IAZIENDARepository) : base(factory)
    {
        _companyRepository = ICompanyRepository;
        _aziendaRepository = IAZIENDARepository;
    }


    public ObservableCollection<store_stocks_lots>? GetList(string CompanyID, string StoreID, string ProductID, bool ShowZeroLots)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<store_stocks_lots>(
                @"SELECT s.*, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.store_id = s.store_id AND eng.product_id = s.product_id AND eng.lot = s.lot AND eng.canceled IS NULL AND eng.date_unloaded IS NULL) AS EngagedQuantity FROM store_stocks_lots AS s
                        WHERE s.company_id = @cid AND s.store_id = @sid AND s.product_id = @pid
                        ORDER BY s.lot ASC, s.expire ASC",
                new { cid = CompanyID, sid = StoreID, pid = ProductID }).ToList();

            if (!ShowZeroLots)
            {
                list.RemoveAll(w => w.AvailableQuantity == 0);
            }

            return new ObservableCollection<store_stocks_lots>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_lots>? GetListByProduct(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = new ObservableCollection<store_stocks_lots>();

            var product = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSingle(CompanyID, ProductID);

            if (product != null)
            {
                if (product.HasLots)
                {
                    var list = connection.Query<store_stocks_lots, tab_articolo, store_stores, store_stocks_lots>(
                        @"SELECT s.*, 
                        (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.store_id = s.store_id AND eng.product_id = s.product_id AND eng.lot = s.lot AND eng.canceled IS NULL AND eng.date_unloaded IS NULL) AS EngagedQuantity, 
                        p.ID, p.UnitaID,
                        w.ID, w.Description
                        FROM store_stocks_lots AS s
                        INNER JOIN tab_articolo AS p ON p.societaID=s.company_id AND p.ID=s.product_id
                        INNER JOIN store_stores AS w ON w.company_id=s.company_id AND w.id=s.store_id
                        WHERE s.company_id = @cid AND s.product_id = @pid
                        ORDER BY s.lot ASC, s.expire ASC, s.store_id",
                        (lot, prd, sto) =>
                        {
                            lot.Store = sto;
                            lot.Product = prd;
                            return lot;
                        },
                        new { cid = CompanyID, pid = ProductID }, splitOn: "ID,ID");

                    foreach (var item in list)
                    {
                        if (item.AvailableQuantity > 0)
                            result.Add(item);
                    }
                }
                else
                {
                    var list = connection.Query<store_stocks, tab_articolo, store_stores, store_stocks>(
                        @"SELECT s.*,
                        (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.store_id = s.store_id AND eng.product_id = s.product_id AND eng.lot = @nolot AND eng.canceled IS NULL AND eng.date_unloaded IS NULL) AS EngagedQuantity, 
                        p.ID, p.UnitaID,
                        w.ID, w.Description 
                        FROM store_stocks AS s
                        INNER JOIN tab_articolo AS p ON p.societaID=s.company_id AND p.ID=s.product_id
                        INNER JOIN store_stores AS w ON w.company_id=s.company_id AND w.id=s.store_id
                        WHERE s.company_id = @cid AND s.product_id = @pid
                        ORDER BY s.store_id ASC",
                        (lot, prd, sto) =>
                        {
                            lot.Store = sto;
                            lot.Product = prd;
                            return lot;
                        },
                        new { cid = CompanyID, pid = ProductID, nolot = Constants.NO_LOT_ID }, splitOn: "ID,ID");
                    foreach (var item in list)
                    {
                        var newLot = new store_stocks_lots()
                        {
                            company_id = item.company_id,
                            store_id = item.store_id,
                            product_id = item.product_id,
                            lot = Constants.NO_LOT_ID,
                            quantity_stock = item.quantity_stock,
                            goods_location = item.goods_location,
                            EngagedQuantity = item.EngagedQuantity,
                            Store = item.Store,
                            Product = item.Product
                        };
                        if (newLot.AvailableQuantity > 0)
                            result.Add(newLot);
                    }
                    // add infinite materials
                    if (product.IsInfinite)
                    {
                        var defaultInfiniteStore = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetDefaultInfinite(CompanyID);
                        if (defaultInfiniteStore != null)
                        {
                            result.Add(new store_stocks_lots()
                            {
                                company_id = CompanyID,
                                store_id = defaultInfiniteStore.id,
                                product_id = ProductID,
                                lot = Constants.NO_LOT_ID,
                                quantity_stock = decimal.MaxValue,
                                Store = defaultInfiniteStore,
                                Product = product
                            });
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<store_stocks_lots>? GetSimpleListByLot(string CompanyID, string Lot)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = new ObservableCollection<store_stocks_lots>();
            var list = connection.Query<store_stocks_lots, store_stores, store_stocks_lots>(
                @"SELECT s.*, (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = s.company_id AND eng.store_id = s.store_id AND eng.product_id = s.product_id AND eng.lot = s.lot AND eng.canceled IS NULL AND eng.date_unloaded IS NULL) AS EngagedQuantity, w.* FROM store_stocks_lots AS s
                        INNER JOIN store_stores AS w ON w.company_id=s.company_id AND w.id=s.store_id
                        WHERE s.company_id = @cid AND s.lot = @lot
                        ORDER BY s.lot ASC, s.expire ASC, s.store_id",
                (lot, sto) =>
                {
                    lot.Store = sto;
                    return lot;
                },
                new { cid = CompanyID, lot = Lot }, splitOn: "company_id");

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public store_stocks_lots? Get(string company_id, string store_id, string product_id, string lot)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<store_stocks_lots>(
                "SELECT * FROM store_stocks_lots WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND lot = @lot",
                new { company_id = company_id, store_id = store_id, product_id = product_id, lot = lot })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string company_id, string store_id, string product_id, string lot)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM store_stocks_lots WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND lot = @lot",
                new { company_id = company_id, store_id = store_id, product_id = product_id, lot = lot }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Printing
    public LotLabelReport? PrintLotLabel(string company_id, string store_id, string product_id, string lot)
    {
        try
        {
            var socbase = _companyRepository.Get(company_id);

            if (socbase == null)
                return null;

            var report = new LotLabelReport
            {
                Lotto = Get(company_id, store_id, product_id, lot),
                Articolo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(company_id, product_id),
                CompanyInfo = _aziendaRepository.Get(company_id),
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
            };

            return report;
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO store_stocks_lots (company_id,store_id,product_id,lot,expire,quantity_stock,quantity_production,quantity_ordered,goods_location,supplier_lot) OUTPUT INSERTED.rv VALUES(@company_id,@store_id,@product_id,@lot,@expire,@quantity_stock,@quantity_production,@quantity_ordered,@goods_location,@supplier_lot)";
    public string UPDATE_QUERY => "UPDATE store_stocks_lots SET company_id = @company_id,store_id = @store_id,product_id = @product_id,lot = @lot,expire = @expire,quantity_stock = @quantity_stock,quantity_production = @quantity_production,quantity_ordered = @quantity_ordered,goods_location = @goods_location,supplier_lot = @supplier_lot OUTPUT INSERTED.rv WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND lot = @lot AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM store_stocks_lots OUTPUT DELETED.rv WHERE company_id = @company_id AND store_id = @store_id AND product_id = @product_id AND lot = @lot AND rv = @rv";
    public bool Insert(store_stocks_lots Model)
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

    public bool Update(store_stocks_lots Model)
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

    public bool Delete(store_stocks_lots Model)
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

    public string? Validate(store_stocks_lots Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.company_id) && IsInsert && !Exists(Model.company_id, Model.store_id, Model.product_id, Model.lot)) || !IsInsert)
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