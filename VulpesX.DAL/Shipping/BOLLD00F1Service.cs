

using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.Shipping;

public interface IBOLLD00F1Repository
{
    ObservableCollection<BOLLD00F1>? GetListByDDTRow(string bolsoc, int btanno, int btboll, int borigb, tab_articolo Product, store_stores? InfiniteStore);

    BOLLD00F1? Get(string bolsoc, int BTANNO, int BTBOLL, int BORIGB, int boposc, string bolott);

    bool Exists(string bolsoc, int BTANNO, int BTBOLL, int BORIGB, int boposc, string bolott);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(BOLLD00F1 Model);

    bool Update(BOLLD00F1 Model);

    bool Delete(BOLLD00F1 Model);

    string? Validate(BOLLD00F1 Model, decimal QuantityAlreadySelected, decimal QuantityNeeded, decimal QuantitySameLotAlreadyUsed, bool IsInsert, string RowUM);
    #endregion
}

public class BOLLD00F1Repository : RepositoryBase, IBOLLD00F1Repository
{
    public BOLLD00F1Repository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BOLLD00F1>? GetListByDDTRow(string bolsoc, int btanno, int btboll, int borigb, tab_articolo Product, store_stores? InfiniteStore)
    {
        try
        {
            using var connection = GetOpenConnection();


            List<BOLLD00F1> list = new List<BOLLD00F1>();
            if (Product != null)
            {
                if (Product.HasLots)
                {
                    list = connection.Query<BOLLD00F1, store_stocks_lots, store_stores, decimal?, BOLLD00F1>(
                        @"SELECT b.bolsoc, b.btanno, b.btboll, b.borigb, b.boposc, b.bolott, b.boqtlo, b.store_id, b.product_id,
                                    l.company_id, l.store_id,l.product_id, l.lot,l.expire, l.quantity_stock, l.quantity_production, l.quantity_ordered,l.goods_location,l.supplier_lot,
                                    w.company_id,w.id,w.description,
                                    (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = b.bolsoc AND eng.store_id = b.store_id AND eng.product_id = b.product_id AND eng.lot = b.bolott AND eng.canceled IS NULL AND eng.date_unloaded IS NULL) AS EngagedQuantity 
                                    FROM BOLLD00F1 AS b
                                    LEFT JOIN store_stocks_lots AS l ON l.company_id=b.bolsoc AND l.store_id=b.store_id AND l.product_id=b.product_id AND l.lot=b.bolott
                                    LEFT JOIN store_stores AS w ON w.company_id=b.bolsoc AND w.id=l.store_id
                                    WHERE b.bolsoc=@bolsoc AND b.BTANNO=@btanno AND b.BTBOLL=@btboll AND b.BORIGB=@borigb",
                        (bd1, lot, sto, enq) =>
                        {
                            lot.EngagedQuantity = (enq ?? 0);
                            lot.Store = sto;
                            lot.Product = Product;
                            bd1.Lot = lot;
                            return bd1;
                        },
                        new { bolsoc = bolsoc, btanno = btanno, btboll = btboll, borigb = borigb, pid = Product.ID },
                        splitOn: "company_id,company_id,EngagedQuantity").ToList();
                }
                else
                {
                    list = connection.Query<BOLLD00F1, store_stocks_lots, store_stores, decimal?, BOLLD00F1>(
                        $@"SELECT '{Constants.NO_LOT_ID}' AS bolott, 
                                b.bolsoc, b.btanno, b.btboll, b.borigb, b.boposc, b.bolott, b.boqtlo, b.store_id, b.product_id,
                                l.company_id, l.store_id,l.product_id, l.quantity_stock, l.quantity_production, l.quantity_ordered,l.goods_location,
                                w.company_id,w.id,w.description,
                                (SELECT SUM(eng.quantity) FROM store_stocks_engage AS eng WHERE eng.company_id = b.bolsoc AND eng.store_id = b.store_id AND eng.product_id = b.product_id AND eng.lot = @nolot AND eng.canceled IS NULL AND eng.date_unloaded IS NULL) AS EngagedQuantity 
                                FROM BOLLD00F1 AS b
                                LEFT JOIN store_stocks AS l ON l.company_id=b.bolsoc AND l.store_id=b.store_id AND l.product_id=b.product_id
                                LEFT JOIN store_stores AS w ON w.company_id=b.bolsoc AND w.id=l.store_id
                                WHERE b.bolsoc=@bolsoc AND b.BTANNO=@btanno AND b.BTBOLL=@btboll AND b.BORIGB=@borigb",
                        (bd1, lot, sto, enq) =>
                        {
                            if (lot == null)
                                lot = new store_stocks_lots()
                                {
                                    lot = string.Empty,
                                    company_id = bolsoc,
                                    product_id = Product.ID,
                                    store_id = (InfiniteStore != null) ? InfiniteStore.id : "XX",
                                    quantity_stock = decimal.MaxValue
                                };
                            lot.EngagedQuantity = (enq ?? 0);
                            lot.Store = Product.IsInfinite ? InfiniteStore : sto;
                            lot.lot = Constants.NO_LOT_ID;
                            lot.Product = Product;
                            bd1.Lot = lot;
                            return bd1;
                        },
                        new { bolsoc = bolsoc, btanno = btanno, btboll = btboll, borigb = borigb, pid = Product.ID, nolot = Constants.NO_LOT_ID, },
                        splitOn: "company_id,company_id,EngagedQuantity").ToList();
                }
            }

            return new ObservableCollection<BOLLD00F1>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BOLLD00F1? Get(string bolsoc, int BTANNO, int BTBOLL, int BORIGB, int boposc, string bolott)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<BOLLD00F1>(
                "SELECT * FROM BOLLD00F1 WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL, BORIGB = BORIGB, boposc = boposc, bolott = bolott })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string bolsoc, int BTANNO, int BTBOLL, int BORIGB, int boposc, string bolott)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM BOLLD00F1 WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL, BORIGB = BORIGB, boposc = boposc, bolott = bolott }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO BOLLD00F1 (bolsoc,BTANNO,BTBOLL,BORIGB,boposc,bolott,boqtlo,store_id,product_id) OUTPUT INSERTED.rv VALUES(@bolsoc,@BTANNO,@BTBOLL,@BORIGB,@boposc,@bolott,@boqtlo,@store_id,@product_id)";
    public string UPDATE_QUERY => "UPDATE BOLLD00F1 SET bolsoc = @bolsoc,BTANNO = @BTANNO,BTBOLL = @BTBOLL,BORIGB = @BORIGB,boposc = @boposc,bolott = @bolott,boqtlo = @boqtlo,store_id = @store_id,product_id = @product_id OUTPUT INSERTED.rv WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM BOLLD00F1 OUTPUT DELETED.rv WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB AND boposc = @boposc AND bolott = @bolott AND rv = @rv";
    public bool Insert(BOLLD00F1 Model)
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

    public bool Update(BOLLD00F1 Model)
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

    public bool Delete(BOLLD00F1 Model)
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

    public string? Validate(BOLLD00F1 Model, decimal QuantityAlreadySelected, decimal QuantityNeeded, decimal QuantitySameLotAlreadyUsed, bool IsInsert, string RowUM)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.bolsoc))
            {
                if (Model.Lot != null)
                {
                    if (Model.boqtlo > 0)
                    {
                        var alreadyEngaged = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>().GetQuantityEngagedByDDT(Model.bolsoc, Model.Lot.product_id, Model.Lot.store_id, Model.Lot.lot, Model.BTANNO, Model.BTBOLL);

                        var realAvailability = Model.Lot.AvailableQuantity + (alreadyEngaged - QuantitySameLotAlreadyUsed);
                        if (Model.boqtlo <= realAvailability)
                        {
                            if (Model.Product?.UnitaID != RowUM || (Model.Product?.UnitaID == RowUM && Model.boqtlo + QuantityAlreadySelected <= QuantityNeeded))
                            {
                                return null;
                            }
                            else
                            { return $"La quantità totale selezionata [{(Model.boqtlo + QuantityAlreadySelected).ToString("N6")}] è superiore a quella necessaria [{QuantityNeeded.ToString("N6")}]"; }
                        }
                        else
                        { return $"La quantità selezionata {(Model.boqtlo + QuantitySameLotAlreadyUsed).ToString("N6")} non può essere superiore alla quantità disponibile {realAvailability.ToString("N6")} per il lotto [{Model.Lot.lot}] nel magazzino [{Model.Lot.Store?.FullDescriptionSearchable}]\nPotreste avere gia' selezionato questo lotto su altre righe di questo stesso DDT."; }
                    }
                    else
                    { return "La quantità è obbligatoria e deve essere maggiore di 0"; }
                }
                else
                { return "Il lotto è obbligatorio"; }
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