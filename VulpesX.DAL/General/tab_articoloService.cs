using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VulpesX.DAL;
using VulpesX.DAL.Auth;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.Models.Models;
using VulpesX.Models.Models.Reports;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.DAL.General;

public interface Itab_articoloRepository
{
    ObservableCollection<tab_articolo>? GetSimpleList(string CompanyID, bool AddAllValuesItem = false);

    ObservableCollection<tab_articolo>? GetList(string CompanyID);

    ObservableCollection<tab_articolo>? GetListComponents(string CompanyID, string ArticleID);

    tab_articolo? Get(string CompanyID, string ID, IDbConnection? Connection = null);

    tab_articolo? GetSingle(string CompanyID, string ID);

    tab_articolo? GetCheckFields(string CompanyID, string ID);

    List<tab_articolo>? GetFromList(string CompanyID, string[] IDList);

    bool CheckIsInfinite(string CompanyID, string ID);

    string? GetDefaultStoreID(string CompanyID, string ID);

    int? GetDefaultSupplier(string CompanyID, string ID);

    string? GetDefaultRevision(string CompanyID, string ID);

    tab_articolo? GetCanceled(string CompanyID, string ID);

    bool Exists(string CompanyID, string ID);

    string? CheckIsCancellable(string CompanyID, string ProductID);

    ProductLabelReport? PrintProductLabel(string company_id, string product_id);

    #region Compute methods
    decimal ComputeQuantityToOrder(string CompanyID, string ProductID, decimal NeededQuantity);

    Tuple<decimal, decimal> ComputeWeight(string CompanyID, string ProductID, decimal Quantity, string UM);

    decimal ComputeRealQuantity(string CompanyID, string ProductID, string UM, string ProductMainUM, decimal? BoxQuantity, decimal OriginalQuantity);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    public bool Insert(tab_articolo Model, ObservableCollection<tab_articolo_produzione_sorgenti> PlantSources, ObservableCollection<tab_articolo_extern> ExternalCodes);

    public bool Update(tab_articolo Model, ObservableCollection<tab_articolo_produzione_sorgenti> PlantSources, ObservableCollection<tab_articolo_extern> ExternalCodes);

    public bool Delete(tab_articolo Model);

    public string? Validate(tab_articolo Model, bool IsInsert);
    #endregion

    #region IMMAGINI
    ObservableCollection<tab_articolo_immagine>? GetImmagini(string CompanyID, string ArticleID);

    tab_articolo_immagine? GetImmagine(string CompanyID, string ArticleID, long ID);

    bool InsertImmagine(tab_articolo_immagine Model);

    bool UpdateImmagine(tab_articolo_immagine Model);

    bool DeleteImmagine(tab_articolo_immagine Model);
    #endregion

    #region ALLEGATI
    ObservableCollection<tab_articolo_allegato>? GetAllegati(string CompanyID, string ArticleID);

    tab_articolo_allegato? GetAllegato(string CompanyID, string ArticleID, long ID);

    bool InsertAllegato(tab_articolo_allegato Model);

    bool UpdateAllegato(tab_articolo_allegato Model);

    bool DeleteAllegato(tab_articolo_allegato Model);
    #endregion

    #region Widget methods
    Tuple<int, int> GetMaterialAlarms(string CompanyID);

    ObservableCollection<UnderstockItem>? GetMaterialUnderstockList(string CompanyID);

    ObservableCollection<AlarmedItem>? GetMaterialAlarmedList(string CompanyID);
    #endregion

    #region UFP
    ObservableCollection<tab_articolo>? GetSimpleList(bool AddAllValuesItem = false);

    ObservableCollection<tab_articolo>? GetSimpleProductionList(string Type);

    ObservableCollection<tab_articolo>? GetList(string CompanyID, List<FilterEntry> FilterList, out int TotalCount);

    tab_articolo? Get(string ID, IDbConnection? Connection = null);

    List<Tuple<string, string>>? GetDraws(string ArticleID);
    #endregion
}

public class tab_articoloRepository : RepositoryBase, Itab_articoloRepository
{
    private readonly Istore_stocksRepository _store_stocksRepository;
    private readonly Itab_articolo_tipoRepository _tab_articolo_tipoRepository;
    private readonly Itab_articolo_externRepository _tab_articolo_externRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAZIENDARepository _aziendaRepository;
    private readonly ISRM_LISFORRepository _srm_lisforRepository;
    public tab_articoloRepository(IConnectionFactory factory, Istore_stocksRepository Istore_stocksRepository, Itab_articolo_tipoRepository Itab_articolo_tipoRepository, Itab_articolo_externRepository Itab_articolo_externRepository,
        ICompanyRepository ICompanyRepository, IAZIENDARepository IAZIENDARepository, ISRM_LISFORRepository ISRM_LISFORRepository) : base(factory)
    {
        _store_stocksRepository = Istore_stocksRepository;
        _tab_articolo_tipoRepository = Itab_articolo_tipoRepository;
        _tab_articolo_externRepository = Itab_articolo_externRepository;

        _companyRepository = ICompanyRepository;
        _aziendaRepository = IAZIENDARepository;

        _srm_lisforRepository = ISRM_LISFORRepository;
    }

    public ObservableCollection<tab_articolo>? GetSimpleList(string CompanyID, bool AddAllValuesItem = false)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo>(
                @"SELECT a.ID, a.Descrizione, a.UnitaID, a.UnitaIDAlt, a.GroupID, a.AccountID, a.SubaccountID, a.HasLots, a.store_warning_qty, a.store_min_stock, a.asscod, a.assali, a.TipoID, a.QuantitaDefault, a.ExpireDays, a.UnitaIDAlt
                    FROM tab_articolo AS a 
                    WHERE a.SocietaID = @SocietaID AND a.LogCanceled IS NULL"
                , new { SocietaID = CompanyID }).ToList();

            if (AddAllValuesItem)
                list.Add(new tab_articolo { SocietaID = CompanyID, TipoID = "A", ID = string.Empty, Descrizione = "Tutti gli articoli" });

            return new ObservableCollection<tab_articolo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo>(
                "SELECT a.*, t.Descrizione,c.Descrizione,u.Descrizione," +
                "(SELECT COUNT(SocietaID) FROM tab_articolo_immagine as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Img," +
                "(SELECT COUNT(SocietaID) FROM tab_articolo_allegato as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID) as Att," +
                "(SELECT TOP(1) RevisioneID FROM tab_articolo_composizione as d WHERE a.SocietaID = d.SocietaID AND a.ID = d.ArticoloID order by d.LogUpdated desc) as Ur, " +
                "(SELECT COUNT(*) FROM tab_articolo_composizione AS ac WHERE ac.SocietaID = a.SocietaID AND ac.ComponenteArticoloID = a.ID) AS had " +
                "FROM tab_articolo AS a " +
                "LEFT JOIN tab_articolo_tipo AS t ON a.SocietaID = t.SocietaID AND a.TipoID = t.ID " +
                "LEFT JOIN tab_articolo_categoria AS c ON a.SocietaID = c.SocietaID AND a.CategoriaID = c.ID " +
                "LEFT JOIN tab_articolo_unita AS u ON a.SocietaID = u.SocietaID AND a.UnitaID = u.ID " +
                "WHERE a.SocietaID = @SocietaID AND a.LogCanceled IS NULL"
                , new[] { typeof(tab_articolo), typeof(tab_articolo_tipo), typeof(tab_articolo_categoria), typeof(tab_articolo_unita), typeof(int), typeof(int), typeof(string), typeof(int) }
                , (objects) =>
                    {
                        var articolo = objects[0] as tab_articolo;
                        articolo!.TipoDescrizione = (objects[1] as tab_articolo_tipo)?.Descrizione;
                        articolo!.CategoriaDescrizione = (objects[2] as tab_articolo_categoria)?.Descrizione;
                        articolo!.UnitaDescrizione = (objects[3] as tab_articolo_unita)?.Descrizione;
                        articolo!.HaComposizione = !string.IsNullOrEmpty((objects[6] as string));
                        articolo!.HaImmagine = ((int?)objects[4] ?? 0) > 0;
                        articolo!.HaAllegato = ((int?)objects[5] ?? 0) > 0;
                        articolo!.UltimaRevisioneID = objects[6] as string;
                        articolo!.HaDipendenze = ((int?)objects[7] ?? 0) > 0;

                        return articolo;
                    }, new { SocietaID = CompanyID },
                    splitOn: "Descrizione,Descrizione,Descrizione,Img,Att,Ur,had");

            return new ObservableCollection<tab_articolo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo>? GetListComponents(string CompanyID, string ArticleID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo, tab_articolo_tipo, tab_articolo_categoria, tab_articolo_unita, int, tab_articolo>(
                "SELECT a.*, t.Descrizione,c.Descrizione,u.Descrizione," +
                "(SELECT COUNT(c.SocietaID) FROM tab_articolo_composizione as c WHERE c.SocietaID = a.SocietaID AND c.ComponenteArticoloID = a.ID) as Dp " +
                "FROM tab_articolo AS a " +
                "INNER JOIN tab_articolo_tipo AS t ON a.SocietaID = t.SocietaID AND a.TipoID = t.ID " +
                "INNER JOIN tab_articolo_categoria AS c ON a.SocietaID = c.SocietaID AND a.CategoriaID = c.ID " +
                "INNER JOIN tab_articolo_unita AS u ON a.SocietaID = u.SocietaID AND a.UnitaID = u.ID " +
                "WHERE a.SocietaID = @SocietaID AND a.ID != @ArticoloID AND a.LogCanceled IS NULL"
                , (articolo, tipo, categoria, unita, dipendenze) =>
                {
                    articolo.TipoDescrizione = tipo.Descrizione;
                    articolo.CategoriaDescrizione = categoria.Descrizione;
                    articolo.UnitaDescrizione = unita.Descrizione;
                    articolo.HaComposizione = false;
                    articolo.UltimaRevisioneID = null;
                    articolo.HaDipendenze = dipendenze > 0;

                    return articolo;
                }, new { SocietaID = CompanyID, ArticoloID = ArticleID },
                    splitOn: "Descrizione,Descrizione,Descrizione,Dp");

            return new ObservableCollection<tab_articolo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo? Get(string CompanyID, string ID, IDbConnection? Connection = null)
    {
        try
        {
            if (Connection == null)
                Connection = GetOpenConnection();

            return Connection.Query<tab_articolo>(
                "SELECT * FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo? GetSingle(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo>(
                "SELECT * FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo? GetCheckFields(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo>(
                "SELECT SocietaID,ID,IsInfinite,HasLots FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<tab_articolo>? GetFromList(string CompanyID, string[] IDList)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo>(
                $"SELECT SocietaID, ID, Descrizione, Note FROM tab_articolo WHERE SocietaID = @SocietaID and ID IN @IDList AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, IDList = IDList })
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool CheckIsInfinite(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<bool>(
                "SELECT IsInfinite FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public string? GetDefaultStoreID(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var product = connection.Query<tab_articolo>(
                @"SELECT * FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL AND TipoID IS NOT NULL AND TipoID <> ''",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
            if (product != null)
            {
                var productType = _tab_articolo_tipoRepository.Get(product.SocietaID, product.TipoID);
                if (productType != null && !string.IsNullOrWhiteSpace(productType.DefaultMagazzinoID))
                    return productType.DefaultMagazzinoID;
            }
            return null;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public int? GetDefaultSupplier(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                @"SELECT default_supplier_id FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, ID = ID });
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public string? GetDefaultRevision(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<string>(
                @"SELECT DefaultRevisionID FROM tab_articolo 
                        WHERE SocietaID = @SocietaID and ID = @ID AND LogCanceled IS NULL",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo? GetCanceled(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo>(
                "SELECT * FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM tab_articolo WHERE SocietaID = @SocietaID and ID = @ID",
                new { SocietaID = CompanyID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public string? CheckIsCancellable(string CompanyID, string ProductID)
    {
        try
        {
            using var connection = GetOpenConnection();

            // stocks
            var stocks = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM store_stocks WHERE company_id = @cid and product_id = @pid",
                new { cid = CompanyID, pid = ProductID });
            if (stocks > 0)
                return "L'articolo ha delle giacenze nei magazzini, impossibile annullarlo";

            // engages
            var engages = (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM store_stocks_engage WHERE company_id = @cid and product_id = @pid and date_unloaded IS NULL and canceled IS NULL",
                new { cid = CompanyID, pid = ProductID });
            if (engages > 0)
                return "L'articolo ha degli impegni nei magazzini, impossibile annullarlo";

            // open offers
            var offers = (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM OFFED00F AS r
                        INNER JOIN OFFET00F AS t ON t.oftsoci = r.oftsoci AND t.oftanno = r.oftanno AND t.oftnuor = r.oftnuor
                        WHERE r.oftsoci = @cid AND r.ofdcoda = @pid AND t.oflgchi = 'A' AND t.canceled IS NULL",
                new { cid = CompanyID, pid = ProductID });
            if (offers > 0)
                return "L'articolo ha delle offerte in corso, impossibile annullarlo";

            // open orders
            var orders = (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM ORDID00F AS r
                        INNER JOIN ORDIT00F AS t ON t.otsoci = r.otsoci AND t.otanno = r.otanno AND t.otnuor = r.otnuor
                        WHERE r.otsoci = @cid AND r.odcoda = @pid AND t.flgchi = 'A' AND t.canceled IS NULL",
                new { cid = CompanyID, pid = ProductID });
            if (orders > 0)
                return "L'articolo ha degli ordini in corso, impossibile annullarlo";

            // DDT
            var ddt = (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM BOLLD00F AS r
                        INNER JOIN BOLLT00F AS t ON t.bolsoc = r.bolsoc AND t.btanno = r.btanno AND t.btboll = r.btboll
                        WHERE r.bolsoc = @cid AND r.bocoda = @pid AND t.canceled IS NULL",
                new { cid = CompanyID, pid = ProductID });
            if (ddt > 0)
                return "L'articolo ha dei DDT in corso, impossibile annullarlo";

            // invoices
            var invoices = (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FATTD00F AS r
                        INNER JOIN FATTT00F AS t ON t.ftsoci = r.ftsoci AND t.ftanno = r.ftanno AND t.ftnuor = r.ftnuor
                        WHERE r.ftsoci = @cid AND r.fdcoda = @pid AND t.ftfla2 <> '1' AND t.canceled IS NULL",
                new { cid = CompanyID, pid = ProductID });
            if (invoices > 0)
                return "L'articolo ha delle fatture non ancora contabilizzate, impossibile annullarlo";

            // cycles
            var cycles = (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM tab_articolo_composizione AS r
                        WHERE r.SocietaID = @cid AND r.ComponenteArticoloID = @pid AND r.LogCanceled IS NULL",
                new { cid = CompanyID, pid = ProductID });
            if (cycles > 0)
                return $"L'articolo è utilizzato in alcuni cicli di produzione, impossibile annullarlo";

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public ProductLabelReport? PrintProductLabel(string company_id, string product_id)
    {
        try
        {
            var socbase = _companyRepository.Get(company_id);

            if (socbase == null)
                return null;

            var report = new ProductLabelReport
            {
                Articolo = Get(company_id, product_id),
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

    #region Compute methods
    public decimal ComputeQuantityToOrder(string CompanyID, string ProductID, decimal NeededQuantity)
    {
        try
        {
            using var connection = GetOpenConnection();

            var product = Get(CompanyID, ProductID);
            if (product != null)
            {
                if ((product.store_restock_qty ?? 0) > NeededQuantity)
                {
                    if ((product.store_restock_qty ?? 0) - NeededQuantity >= (product.store_min_stock ?? 0))
                    {
                        return (product.store_restock_qty ?? 0);
                    }
                    else
                    {
                        return ((product.store_restock_qty ?? 0) + ((product.store_min_stock ?? 0) - ((product.store_restock_qty ?? 0) - NeededQuantity)));
                    }
                }
                else
                {
                    return (NeededQuantity + (product.store_min_stock ?? 0));
                }
            }
            return -1;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return -1;
        }
    }

    public Tuple<decimal, decimal> ComputeWeight(string CompanyID, string ProductID, decimal Quantity, string UM)
    {
        try
        {
            using var connection = GetOpenConnection();

            var product = Get(CompanyID, ProductID);
            if (product != null)
            {
                if (UM == product.UnitaID)
                {
                    var realQuantity = Quantity / (product.QuantitaDefault.HasValue && product.QuantitaDefault.Value > 0 ? product.QuantitaDefault.Value : 1m);
                    return new Tuple<decimal, decimal>((realQuantity * (product.GrossWeight ?? 0m)), (realQuantity * (product.NetWeight ?? 0m)));
                }
                else
                {
                    if (UM == product.UnitaIDAlt)
                        return new Tuple<decimal, decimal>((Quantity * (product.GrossWeight ?? 0m)), (Quantity * (product.NetWeight ?? 0m)));
                }
            }
            return new Tuple<decimal, decimal>(0m, 0m);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<decimal, decimal>(0m, 0m);
        }
    }

    public decimal ComputeRealQuantity(string CompanyID, string ProductID, string UM, string ProductMainUM, decimal? BoxQuantity, decimal OriginalQuantity)
    {
        try
        {
            decimal realQty = OriginalQuantity;
            if (UM != ProductMainUM)
                realQty = OriginalQuantity / (BoxQuantity.HasValue && BoxQuantity.Value > 0 ? BoxQuantity.Value : 1m);
            return realQty;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return 0;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO tab_articolo (SocietaID,ID,Descrizione,DescrizioneEstesa,TipoID,CategoriaID,UnitaID,UnitaIDAlt,QuantitaDefault,Note,GestionaleID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Barcode,conversion_factor,conversion_factor_alt,store_min_stock,store_restock_qty,default_supplier_id,asscod,assali,is_customer_material,ExpireDays,GroupID,AccountID,SubaccountID,LogCanceledReason,NetWeight,GrossWeight,HasLots,DefaultRevisionID,IsInfinite,store_warning_qty,packaging_qty,costcenter_id,cost_group_id,cost_account_id,cost_subaccount_id) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Descrizione,@DescrizioneEstesa,@TipoID,@CategoriaID,@UnitaID,@UnitaIDAlt,@QuantitaDefault,@Note,@GestionaleID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Barcode,@conversion_factor,@conversion_factor_alt,@store_min_stock,@store_restock_qty,@default_supplier_id,@asscod,@assali,@is_customer_material,@ExpireDays,@GroupID,@AccountID,@SubaccountID,@LogCanceledReason,@NetWeight,@GrossWeight,@HasLots,@DefaultRevisionID,@IsInfinite,@store_warning_qty,@packaging_qty,@costcenter_id,@cost_group_id,@cost_account_id,@cost_subaccount_id)";
    public string UPDATE_QUERY => "UPDATE tab_articolo SET SocietaID = @SocietaID,ID = @ID,Descrizione = @Descrizione,DescrizioneEstesa = @DescrizioneEstesa,TipoID = @TipoID,CategoriaID = @CategoriaID,UnitaID = @UnitaID,UnitaIDAlt = @UnitaIDAlt,QuantitaDefault = @QuantitaDefault,Note = @Note,GestionaleID = @GestionaleID,LogAdded = @LogAdded,LogUpdated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Barcode = @Barcode,conversion_factor = @conversion_factor,conversion_factor_alt = @conversion_factor_alt,store_min_stock = @store_min_stock,store_restock_qty = @store_restock_qty,default_supplier_id = @default_supplier_id,asscod = @asscod,assali = @assali,is_customer_material = @is_customer_material,ExpireDays = @ExpireDays,GroupID = @GroupID,AccountID = @AccountID,SubaccountID = @SubaccountID,LogCanceledReason = @LogCanceledReason,NetWeight = @NetWeight,GrossWeight = @GrossWeight,HasLots = @HasLots,DefaultRevisionID = @DefaultRevisionID,IsInfinite = @IsInfinite,store_warning_qty = @store_warning_qty,packaging_qty = @packaging_qty,costcenter_id = @costcenter_id,cost_group_id = @cost_group_id,cost_account_id = @cost_account_id,cost_subaccount_id = @cost_subaccount_id OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM tab_articolo OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public bool Insert(tab_articolo Model, ObservableCollection<tab_articolo_produzione_sorgenti> PlantSources, ObservableCollection<tab_articolo_extern> ExternalCodes)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.Execute(INSERT_QUERY, Model, transaction);

                    // Sources
                    // clear
                    connection.Execute(@"DELETE FROM tab_articolo_produzione_sorgenti WHERE SocietaID = @cid AND ArticoloID = @pid",
                        new { cid = Model.SocietaID, pid = Model.ID },
                        transaction);
                    if (PlantSources != null && PlantSources.Count > 0)
                    {
                        foreach (var item in PlantSources)
                        {
                            connection.Execute("INSERT INTO tab_articolo_produzione_sorgenti (SocietaID,ArticoloID,RisorsaID,SorgenteID) OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RisorsaID,@SorgenteID)",
                                item, transaction);
                        }
                    }
                    // Extern
                    // clear
                    connection.Execute(@"DELETE FROM tab_articolo_extern WHERE extsoc = @cid AND extpid = @pid",
                        new { cid = Model.SocietaID, pid = Model.ID }, transaction);
                    if (ExternalCodes != null && ExternalCodes.Count > 0)
                    {
                        foreach (var item in ExternalCodes)
                        {
                            connection.Execute(_tab_articolo_externRepository.INSERT_QUERY, item, transaction);
                        }
                    }

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

    public bool Update(tab_articolo Model, ObservableCollection<tab_articolo_produzione_sorgenti> PlantSources, ObservableCollection<tab_articolo_extern> ExternalCodes)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);

                    // Sources
                    // clear
                    connection.Execute(@"DELETE FROM tab_articolo_produzione_sorgenti WHERE SocietaID = @cid AND ArticoloID = @pid",
                        new { cid = Model.SocietaID, pid = Model.ID },
                        transaction);
                    if (PlantSources != null && PlantSources.Count > 0)
                    {
                        foreach (var item in PlantSources)
                        {
                            connection.Execute("INSERT INTO tab_articolo_produzione_sorgenti (SocietaID,ArticoloID,RisorsaID,SorgenteID) OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@RisorsaID,@SorgenteID)",
                                item, transaction);
                        }
                    }
                    // Extern
                    // clear
                    connection.Execute(@"DELETE FROM tab_articolo_extern WHERE extsoc = @cid AND extpid = @pid",
                        new { cid = Model.SocietaID, pid = Model.ID }, transaction);
                    if (ExternalCodes != null && ExternalCodes.Count > 0)
                    {
                        foreach (var item in ExternalCodes)
                        {
                            connection.Execute(_tab_articolo_externRepository.INSERT_QUERY, item, transaction);
                        }
                    }
                    // clear stocks and lot if is infinite
                    if (Model.IsInfinite)
                    {
                        // stocks
                        connection.Execute(@"DELETE FROM store_stocks WHERE company_id = @cid AND product_id = @pid",
                            new { cid = Model.SocietaID, pid = Model.ID }, transaction);
                        // stock_lots
                        connection.Execute(@"DELETE FROM store_stocks_lots WHERE company_id = @cid AND product_id = @pid",
                            new { cid = Model.SocietaID, pid = Model.ID }, transaction);
                    }
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

    public bool Delete(tab_articolo Model)
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

    public string? Validate(tab_articolo Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Model.ID) && IsInsert && !Exists(Model.SocietaID, Model.ID) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Descrizione) && Model.Descrizione.Length <= 255)
                {
                    if (!string.IsNullOrWhiteSpace(Model.UnitaID) && !string.IsNullOrWhiteSpace(Model.CategoriaID) && !string.IsNullOrWhiteSpace(Model.TipoID))
                    {
                        if ((!string.IsNullOrWhiteSpace(Model.GroupID) && !string.IsNullOrWhiteSpace(Model.AccountID) && !string.IsNullOrWhiteSpace(Model.SubaccountID)) ||
                            (string.IsNullOrWhiteSpace(Model.GroupID) && string.IsNullOrWhiteSpace(Model.AccountID) && string.IsNullOrWhiteSpace(Model.SubaccountID)))
                        {
                            if ((!string.IsNullOrWhiteSpace(Model.cost_group_id) && !string.IsNullOrWhiteSpace(Model.cost_account_id) && !string.IsNullOrWhiteSpace(Model.cost_subaccount_id)) ||
                            (string.IsNullOrWhiteSpace(Model.cost_group_id) && string.IsNullOrWhiteSpace(Model.cost_account_id) && string.IsNullOrWhiteSpace(Model.cost_subaccount_id)))
                            {
                                if ((string.IsNullOrWhiteSpace(Model.UnitaIDAlt) && !Model.QuantitaDefault.HasValue) ||
                                (!string.IsNullOrWhiteSpace(Model.UnitaIDAlt) && Model.QuantitaDefault.HasValue && Model.QuantitaDefault.Value > 0))
                                {
                                    if ((Model.IsInfinite && !Model.HasLots) || !Model.IsInfinite)
                                    {
                                        return null;
                                    }
                                    else
                                    { return "Un articolo definito come sempre disponibile non può essere gestito a lotti"; }
                                }
                                else
                                { return "Se si specifica una quantità per confezione (che deve essere maggiore di 0) è obbligatoria anche l'unità di misura alternativa e viceversa"; }
                            }
                            else
                            { return "Il conto contabile dei ricavi deve essere selezionato completo o lasciato tutto vuoto"; }
                        }
                        else
                        { return "Il conto contabile dei costi deve essere selezionato completo o lasciato tutto vuoto"; }
                    }
                    else
                    {
                        return "Unità | Categoria | Tipo sono obbligatori";
                    }
                }
                else
                {
                    return "La descrizione è obbligatoria e può contenere al massimo 255 caratteri";
                }
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

    #region IMMAGINI
    public ObservableCollection<tab_articolo_immagine>? GetImmagini(string CompanyID, string ArticleID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo_immagine>(
                "SELECT *  FROM tab_articolo_immagine " +
                "WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID"
                , new { SocietaID = CompanyID, ArticoloID = ArticleID });

            return new ObservableCollection<tab_articolo_immagine>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_immagine? GetImmagine(string CompanyID, string ArticleID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo_immagine>(
                "SELECT * FROM tab_articolo_immagine WHERE SocietaID = @SocietaID and ArticoloID = @ArticoloID and ID = @ID",
                new { SocietaID = CompanyID, ArticoloID = ArticleID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool InsertImmagine(tab_articolo_immagine Model)
    {
        using var connection = GetOpenConnection();

        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var numerator = connection.Query<long?>("SELECT ID FROM tab_articolo_immagine WHERE SocietaID = @SocietaID and ArticoloID = @ArticoloID ORDER BY ID DESC", new { SocietaID = Model.SocietaID, ArticoloID = Model.ArticoloID }, transaction).FirstOrDefault() ?? 0;
                Model.ID = ++numerator;

                var result = connection.Execute(
                    "INSERT INTO tab_articolo_immagine (SocietaID,ArticoloID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Url)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@ID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID, @Url)",
                    Model, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
    }

    public bool UpdateImmagine(tab_articolo_immagine Model)
    {
        using var connection = GetOpenConnection();

        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_articolo_immagine SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID, Url = @Url" +
                    " OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND ID = @ID AND rv = @rv",
                    Model, transaction);

                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
    }

    public bool DeleteImmagine(tab_articolo_immagine Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM tab_articolo_immagine OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ArticoloID= @ArticoloID AND ID = @ID AND rv = @rv",
                Model);
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
    #endregion

    #region ALLEGATI
    public ObservableCollection<tab_articolo_allegato>? GetAllegati(string CompanyID, string ArticleID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_articolo_allegato>(
                "SELECT *  FROM tab_articolo_allegato " +
                "WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID"
                , new { SocietaID = CompanyID, ArticoloID = ArticleID });

            return new ObservableCollection<tab_articolo_allegato>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo_allegato? GetAllegato(string CompanyID, string ArticleID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo_allegato>(
                "SELECT * FROM tab_articolo_allegato WHERE SocietaID = @SocietaID and ArticoloID = @ArticoloID and ID = @ID",
                new { SocietaID = CompanyID, ArticoloID = ArticleID, ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool InsertAllegato(tab_articolo_allegato Model)
    {
        using var connection = GetOpenConnection();


        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.Execute(
                    "INSERT INTO tab_articolo_allegato (SocietaID,ArticoloID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Uri, ID)" +
                    " OUTPUT INSERTED.rv VALUES(@SocietaID,@ArticoloID,@LogAdded,@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID, @Uri, @ID)",
                    Model, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
    }

    public bool UpdateAllegato(tab_articolo_allegato Model)
    {
        using var connection = GetOpenConnection();

        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                var result = connection.ExecuteScalar(
                    "UPDATE tab_articolo_allegato SET LogAdded = @LogAdded,LogUpdated = @LogUpdated,LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID, Uri = @Uri" +
                    " OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ArticoloID = @ArticoloID AND ID = @ID AND rv = @rv",
                    Model, transaction);

                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
    }

    public bool DeleteAllegato(tab_articolo_allegato Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            var result = connection.Execute(
                    "DELETE FROM tab_articolo_allegato OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ArticoloID= @ArticoloID AND ID = @ID AND rv = @rv",
                    Model);
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
    #endregion

    #region Widget methods
    public Tuple<int, int> GetMaterialAlarms(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<tab_articolo>(
                @"SELECT ID,store_warning_qty, store_min_stock FROM tab_articolo
                        WHERE SocietaID=@cid AND IsInfinite = 0 AND ((store_warning_qty IS NOT NULL AND store_warning_qty > 0) OR (store_min_stock IS NOT NULL AND store_min_stock > 0))",
                new { cid = CompanyID });
            int understock = 0;
            int alarmed = 0;
            foreach (var item in list)
            {
                var stock = _store_stocksRepository.CheckAvailabilityByProduct(CompanyID, item.ID, null)?.Sum(sum => sum.QuantityAvailable);
                if (item.store_min_stock > stock)
                    understock += 1;
                if (item.store_warning_qty > stock)
                    alarmed += 1;
            }
            return new Tuple<int, int>(understock, alarmed);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new Tuple<int, int>(0, 0);
        }
    }

    public ObservableCollection<UnderstockItem>? GetMaterialUnderstockList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            if (connection != null)
            {
                connection.Open();
                var list = connection.Query<UnderstockItem>(
                    @"SELECT ID AS ID, Descrizione AS Description, UnitaID AS UM, store_min_stock AS QuantityMinimum, store_restock_qty AS QuantityRestock, default_supplier_id AS DefaultSupplierID, a.abers1 AS DefaultSupplierDescription FROM tab_articolo
                        LEFT JOIN ABE AS a ON a.abecod = default_supplier_id
                        WHERE SocietaID=@cid AND IsInfinite = 0 AND (store_min_stock IS NOT NULL AND store_min_stock > 0)",
                    new { cid = CompanyID }).ToList();
                var result = new ObservableCollection<UnderstockItem>();
                foreach (var item in list)
                {
                    // quantity available
                    var availability = _store_stocksRepository.CheckAvailabilityByProduct(CompanyID, item.ID, null);
                    item.QuantityAvailable = availability?.Sum(sum => sum.QuantityAvailable) ?? 0;
                    item.QuantityOrdered = availability?.Sum(sum => sum.QuantityOrdered) ?? 0;
                    if (item.QuantityMinimum > item.QuantityAvailable)
                    {
                        if (item.DefaultSupplierID.HasValue && item.DefaultSupplierID > 0)
                        {
                            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            // default supplier product id
                            var supplierProduct = _srm_lisforRepository.GetCurrent(CompanyID, item.ID, item.DefaultSupplierID.Value, now, item.UM);
                            if (supplierProduct != null)
                            {
                                item.SupplierPID = supplierProduct?.SupplierProductID;
                                item.SupplierPDescription = supplierProduct?.SupplierProductDescription;
                            }
                        }
                        result.Add(item);
                    }
                }
                return result;
            }
            else
            {
                ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                return null;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<AlarmedItem>? GetMaterialAlarmedList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AlarmedItem>(
                @"SELECT ID AS ID, Descrizione AS Description, UnitaID AS UM, store_warning_qty AS QuantityAlarm, store_min_stock AS QuantityMinimum, store_restock_qty AS QuantityRestock, default_supplier_id AS DefaultSupplierID, a.abers1 AS DefaultSupplierDescription FROM tab_articolo
                        LEFT JOIN ABE AS a ON a.abecod = default_supplier_id
                        WHERE SocietaID=@cid AND IsInfinite = 0 AND (store_warning_qty IS NOT NULL AND store_warning_qty > 0)",
                new { cid = CompanyID });

            var result = new ObservableCollection<AlarmedItem>();

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            foreach (var item in list)
            {
                // quantity available
                var availability = _store_stocksRepository.CheckAvailabilityByProduct(CompanyID, item.ID, null);

                item.QuantityAvailable = availability?.Sum(sum => sum.QuantityAvailable) ?? 0;
                item.QuantityOrdered = availability?.Sum(sum => sum.QuantityOrdered) ?? 0;
                if (item.QuantityAlarm > item.QuantityAvailable)
                {
                    // default supplier product id
                    if (item.DefaultSupplierID.HasValue && item.DefaultSupplierID > 0)
                    {
                        var supplierProduct = _srm_lisforRepository.GetCurrent(CompanyID, item.ID, item.DefaultSupplierID.Value, now, item.UM);
                        if (supplierProduct != null)
                        {
                            item.SupplierPID = supplierProduct?.SupplierProductID;
                            item.SupplierPDescription = supplierProduct?.SupplierProductDescription;
                        }
                    }
                    result.Add(item);
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
    #endregion

    #region UFP
    public ObservableCollection<tab_articolo>? GetSimpleList(bool AddAllValuesItem = false)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<tab_articolo>? GetSimpleProductionList(string Type)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<tab_articolo>? GetList(string CompanyID, List<FilterEntry> FilterList, out int TotalCount)
    {
        TotalCount = 0;

        throw new NotImplementedException();
    }

    public tab_articolo? Get(string ID, IDbConnection? Connection = null)
    {
        throw new NotImplementedException();
    }

    public List<Tuple<string, string>>? GetDraws(string ArticleID)
    {
        throw new NotImplementedException();
    }
    #endregion
}

public class tab_articoloUfpRepository : RepositoryBase, Itab_articoloRepository
{
    private readonly Istore_stocksRepository _store_stocksRepository;
    private readonly Itab_articolo_tipoRepository _tab_articolo_tipoRepository;
    private readonly Itab_articolo_externRepository _tab_articolo_externRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IAZIENDARepository _aziendaRepository;
    private readonly ISRM_LISFORRepository _srm_lisforRepository;
    public tab_articoloUfpRepository(IConnectionFactory factory, Istore_stocksRepository Istore_stocksRepository, Itab_articolo_tipoRepository Itab_articolo_tipoRepository, Itab_articolo_externRepository Itab_articolo_externRepository,
        ICompanyRepository ICompanyRepository, IAZIENDARepository IAZIENDARepository, ISRM_LISFORRepository ISRM_LISFORRepository) : base(factory)
    {
        _store_stocksRepository = Istore_stocksRepository;
        _tab_articolo_tipoRepository = Itab_articolo_tipoRepository;
        _tab_articolo_externRepository = Itab_articolo_externRepository;

        _companyRepository = ICompanyRepository;
        _aziendaRepository = IAZIENDARepository;

        _srm_lisforRepository = ISRM_LISFORRepository;
    }

    public ObservableCollection<tab_articolo>? GetSimpleList(string CompanyID, bool AddAllValuesItem = false)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<tab_articolo>? GetList(string CompanyID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<tab_articolo>? GetListComponents(string CompanyID, string ArticleID)
    {
        throw new NotImplementedException();
    }

    public tab_articolo? Get(string CompanyID, string ID, IDbConnection? Connection = null)
    {
        throw new NotImplementedException();
    }

    public tab_articolo? GetSingle(string CompanyID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<tab_articolo>(
                @"SELECT 
a.artcod as ID, 
a.artdes as Descrizione,
a.artumi as UnitaID, 
a.artgr1 as GroupID,
a.artco1 as AccountID,
a.artso1 as SubaccountID,
a.artdise
                    FROM ANAG_ARTICOLI as a WHERE a.artcod = @ID",
                new { ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public tab_articolo? GetCheckFields(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public List<tab_articolo>? GetFromList(string CompanyID, string[] IDList)
    {
        throw new NotImplementedException();
    }

    public bool CheckIsInfinite(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public string? GetDefaultStoreID(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public int? GetDefaultSupplier(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public string? GetDefaultRevision(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public tab_articolo? GetCanceled(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string CompanyID, string ID)
    {
        throw new NotImplementedException();
    }

    public string? CheckIsCancellable(string CompanyID, string ProductID)
    {
        throw new NotImplementedException();
    }

    public ProductLabelReport? PrintProductLabel(string company_id, string product_id)
    {
        throw new NotImplementedException();
    }

    #region Compute methods
    public decimal ComputeQuantityToOrder(string CompanyID, string ProductID, decimal NeededQuantity)
    {
        throw new NotImplementedException();
    }

    public Tuple<decimal, decimal> ComputeWeight(string CompanyID, string ProductID, decimal Quantity, string UM)
    {
        throw new NotImplementedException();
    }

    public decimal ComputeRealQuantity(string CompanyID, string ProductID, string UM, string ProductMainUM, decimal? BoxQuantity, decimal OriginalQuantity)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO tab_articolo (SocietaID,ID,Descrizione,DescrizioneEstesa,TipoID,CategoriaID,UnitaID,UnitaIDAlt,QuantitaDefault,Note,GestionaleID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,Barcode,conversion_factor,conversion_factor_alt,store_min_stock,store_restock_qty,default_supplier_id,asscod,assali,is_customer_material,ExpireDays,GroupID,AccountID,SubaccountID,LogCanceledReason,NetWeight,GrossWeight,HasLots,DefaultRevisionID,IsInfinite,store_warning_qty,packaging_qty,costcenter_id,cost_group_id,cost_account_id,cost_subaccount_id) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,@Descrizione,@DescrizioneEstesa,@TipoID,@CategoriaID,@UnitaID,@UnitaIDAlt,@QuantitaDefault,@Note,@GestionaleID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@Barcode,@conversion_factor,@conversion_factor_alt,@store_min_stock,@store_restock_qty,@default_supplier_id,@asscod,@assali,@is_customer_material,@ExpireDays,@GroupID,@AccountID,@SubaccountID,@LogCanceledReason,@NetWeight,@GrossWeight,@HasLots,@DefaultRevisionID,@IsInfinite,@store_warning_qty,@packaging_qty,@costcenter_id,@cost_group_id,@cost_account_id,@cost_subaccount_id)";
    public string UPDATE_QUERY => "UPDATE tab_articolo SET SocietaID = @SocietaID,ID = @ID,Descrizione = @Descrizione,DescrizioneEstesa = @DescrizioneEstesa,TipoID = @TipoID,CategoriaID = @CategoriaID,UnitaID = @UnitaID,UnitaIDAlt = @UnitaIDAlt,QuantitaDefault = @QuantitaDefault,Note = @Note,GestionaleID = @GestionaleID,LogAdded = @LogAdded,LogUpdated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,Barcode = @Barcode,conversion_factor = @conversion_factor,conversion_factor_alt = @conversion_factor_alt,store_min_stock = @store_min_stock,store_restock_qty = @store_restock_qty,default_supplier_id = @default_supplier_id,asscod = @asscod,assali = @assali,is_customer_material = @is_customer_material,ExpireDays = @ExpireDays,GroupID = @GroupID,AccountID = @AccountID,SubaccountID = @SubaccountID,LogCanceledReason = @LogCanceledReason,NetWeight = @NetWeight,GrossWeight = @GrossWeight,HasLots = @HasLots,DefaultRevisionID = @DefaultRevisionID,IsInfinite = @IsInfinite,store_warning_qty = @store_warning_qty,packaging_qty = @packaging_qty,costcenter_id = @costcenter_id,cost_group_id = @cost_group_id,cost_account_id = @cost_account_id,cost_subaccount_id = @cost_subaccount_id OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM tab_articolo OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public bool Insert(tab_articolo Model, ObservableCollection<tab_articolo_produzione_sorgenti> PlantSources, ObservableCollection<tab_articolo_extern> ExternalCodes)
    {
        throw new NotImplementedException();
    }

    public bool Update(tab_articolo Model, ObservableCollection<tab_articolo_produzione_sorgenti> PlantSources, ObservableCollection<tab_articolo_extern> ExternalCodes)
    {
        throw new NotImplementedException();
    }

    public bool Delete(tab_articolo Model)
    {
        throw new NotImplementedException();
    }

    public string? Validate(tab_articolo Model, bool IsInsert)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region IMMAGINI
    public ObservableCollection<tab_articolo_immagine>? GetImmagini(string CompanyID, string ArticleID)
    {
        throw new NotImplementedException();
    }

    public tab_articolo_immagine? GetImmagine(string CompanyID, string ArticleID, long ID)
    {
        throw new NotImplementedException();
    }

    public bool InsertImmagine(tab_articolo_immagine Model)
    {
        throw new NotImplementedException();
    }

    public bool UpdateImmagine(tab_articolo_immagine Model)
    {
        throw new NotImplementedException();
    }

    public bool DeleteImmagine(tab_articolo_immagine Model)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region ALLEGATI
    public ObservableCollection<tab_articolo_allegato>? GetAllegati(string CompanyID, string ArticleID)
    {
        throw new NotImplementedException();
    }

    public tab_articolo_allegato? GetAllegato(string CompanyID, string ArticleID, long ID)
    {
        throw new NotImplementedException();
    }

    public bool InsertAllegato(tab_articolo_allegato Model)
    {
        throw new NotImplementedException();
    }

    public bool UpdateAllegato(tab_articolo_allegato Model)
    {
        throw new NotImplementedException();
    }

    public bool DeleteAllegato(tab_articolo_allegato Model)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Widget methods
    public Tuple<int, int> GetMaterialAlarms(string CompanyID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<UnderstockItem>? GetMaterialUnderstockList(string CompanyID)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<AlarmedItem>? GetMaterialAlarmedList(string CompanyID)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region UFP
    public ObservableCollection<tab_articolo>? GetSimpleList(bool AddAllValuesItem = false)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<tab_articolo>(
                @"SELECT 
a.artcod as ID, 
a.artdes as Descrizione,
a.artumi as UnitaID, 
a.artgr1 as GroupID,
a.artco1 as AccountID,
a.artso1 as SubaccountID,
a.artdise,
a.artmp1,
a.artmp2
                    FROM ANAG_ARTICOLI AS a 
                    WHERE  a.arannu <> 'S' AND (a.arttip = '1' OR a.arttip = '2')"
                ).ToList();

            if (AddAllValuesItem)
                list.Add(new tab_articolo { SocietaID = string.Empty, TipoID = "A", ID = string.Empty, Descrizione = "Tutti gli articoli" });

            return new ObservableCollection<tab_articolo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo>? GetSimpleProductionList(string Type)
    {
        try
        {
            using var connection = GetOpenConnection();

            string typeFilter = Type == "3" ? "AND a.arttip = '3'" : "AND a.arttip <> '3'";

            var list = connection.Query<tab_articolo>(
                @$"SELECT 
a.artcod as ID, 
a.artdes as Descrizione,
a.artumi as UnitaID, 
a.artdise,
a.artmp1,
a.artmp2,
a.artdi1,
a.artdi2,
a.artlun,
a.arttipmat
                    FROM ANAG_ARTICOLI AS a 
                    WHERE  a.arannu <> 'S' {typeFilter}"
                ).ToList();

            return new ObservableCollection<tab_articolo>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<tab_articolo>? GetList(string CompanyID, List<FilterEntry> FilterList, out int TotalCount)
    {
        TotalCount = 0;

        try
        {
            using var connection = GetOpenConnection();

            var aliasList = new List<GenericIDDescriptionType>() {
                    new GenericIDDescriptionType(){ ID = "arttipFull", Description="a.arttipFull",Type = "#COMPOSED#tipo.tipcod#tipo.tipdes" },
                    new GenericIDDescriptionType(){ ID = "ID", Description="a.artcod" },
                    new GenericIDDescriptionType(){ ID = "artdise", Description="a.artdise"},
                    new GenericIDDescriptionType(){ ID = "Descrizione", Description="a.artdes" },
                    new GenericIDDescriptionType(){ ID = "artfamFull", Description="a.artfamFull",Type = "#COMPOSED#analogia.angcod#analogia.angdesc"},
                    new GenericIDDescriptionType(){ ID = "artcorFull", Description="a.artcorFull",Type = "#COMPOSED#rivestimento.rivecod#rivestimento.rivedes"},
                    new GenericIDDescriptionType(){ ID = "arttipmatFull", Description="a.arttipmatFull", Type = "#COMPOSED#tipomp.tmpcod#tipomp.tmpdes"},
                    new GenericIDDescriptionType(){ ID = "artmapFull", Description="a.artmapFull", Type = "#COMPOSED#mp.matpcod#mp.matpdes"},
                    new GenericIDDescriptionType(){ ID = "artdenFull", Description="a.artdenFull", Type = "#COMPOSED#denti.Dencod#denti.Dendes"},
                    new GenericIDDescriptionType(){ ID = "artdiamFull", Description="a.artdiamFull", Type = "#COMPOSED#diametro.Diamcod#diametro.diamdes"},
                    new GenericIDDescriptionType(){ ID = "artldFull", Description="a.artldFull", Type = "#COMPOSED#ld.Ldcod#ld.Lddes"},
                    new GenericIDDescriptionType(){ ID = "artforiFull", Description="a.artforiFull", Type = "#COMPOSED#elica.FLcod#elica.FLdes"},
                    new GenericIDDescriptionType(){ ID = "artatcoFull", Description="a.artatcoFull", Type = "#COMPOSED#attacco.attacod#attacco.attades"},
                    new GenericIDDescriptionType(){ ID = "artmp1Full", Description="a.artmp1Full", Type = "#COMPOSED#mp_associata.artcod#mp_associata.artdise"},
                    new GenericIDDescriptionType(){ ID = "artfor1mFull", Description="a.artfor1mFull", Type = "#COMPOSED#for_associato.abecod#for_associato.abers1"},
                    new GenericIDDescriptionType(){ ID = "artdi3", Description="a.artdi3", Type="M"},
                    new GenericIDDescriptionType(){ ID = "artdi1", Description="a.artdi1", Type="M"},
                    new GenericIDDescriptionType(){ ID = "artlun", Description="a.artlun", Type="M"},
                    new GenericIDDescriptionType(){ ID = "artlar", Description="a.artlar", Type="M"},
                    new GenericIDDescriptionType(){ ID = "artalt", Description="a.artalt", Type="M"},
                    new GenericIDDescriptionType(){ ID = "artluncod", Description="a.artluncod", Type="M"},
                    new GenericIDDescriptionType(){ ID = "artlinuta", Description="a.artlinuta", Type="M"},
            };

            #region Args
            var args = new DynamicParameters();
            args.Add("CompanyID", CompanyID);
            #endregion

            #region Where
            var whereFilter = new StringBuilder($@"a.arannu <> 'S'");
            // grid filters
            TelerikGridService.ComputeFilter(whereFilter, FilterList, aliasList, args);
            #endregion

            TotalCount = ((int?)connection.ExecuteScalar($@"SELECT COUNT(a.artcod) FROM ANAG_ARTICOLI AS a
                                                        LEFT OUTER JOIN TIPTA00F as tipo ON a.arttip = tipo.tipcod
                                                        LEFT OUTER JOIN ANALOGIE as analogia ON a.artfam = analogia.angcod
                                                        LEFT OUTER JOIN RIVESTIMENTI as rivestimento ON a.artcor = rivestimento.rivecod
                                                        LEFT OUTER JOIN TIPMATPRI as tipomp ON a.arttipmat = tipomp.tmpcod
                                                        LEFT OUTER JOIN MATERIEPRIME as mp ON a.artmap = mp.matpcod
                                                        LEFT OUTER JOIN DENTI as denti ON a.artden = denti.Dencod
                                                        LEFT OUTER JOIN LD as ld ON a.artld = ld.Ldcod
                                                        LEFT OUTER JOIN FORILUBRIFICATI as elica ON a.artfori = elica.FLcod
                                                        LEFT OUTER JOIN DIAMETRO as diametro ON a.artdiam = diametro.Diamcod
                                                        LEFT OUTER JOIN ATTACCO as attacco ON a.artatco = attacco.attacod
                                                        LEFT OUTER JOIN ANAG_ARTICOLI as mp_associata ON a.artmp1 = mp_associata.ARTCOD
                                                        LEFT OUTER JOIN ANAG_BASE as for_associato ON a.artfor1m = for_associato.abecod
                                                        WHERE {whereFilter.ToString()};", args)) ?? 0;

            var list = connection.Query<tab_articolo>(
                $@"SELECT 
                        a.artcod as ID, 
                        a.artdes as Descrizione,
                        a.artumi as UnitaID, 
                        tipo.tipdes as arttipDescizione,
                        analogia.angdesc as artfamDescrizione,
                        rivestimento.rivedes as artcorDescrizione,
                        tipomp.tmpdes as arttipmatDescrizione,
                        mp.matpdes as artmapDescrizione,
                        denti.Dendes as artdenDescrizione,
                        ld.Lddes as artldDescrizione,
                        elica.FLdes as artforiDescrizione,
                        diametro.diamdes as artdiamDescrizione,
                        attacco.attades as artatcoDescrizione,
                        mp_associata.artdise as artmp1Descrizione,
                        for_associato.abers1 as artfor1mDescrizione,
                        tm.ct as HaTempiMedi,
                        tmcnc.ct as HaTempiMediCNC,
                        a.*
                        FROM ANAG_ARTICOLI AS a
                        LEFT OUTER JOIN TIPTA00F as tipo ON a.arttip = tipo.tipcod
                        LEFT OUTER JOIN ANALOGIE as analogia ON a.artfam = analogia.angcod
                        LEFT OUTER JOIN RIVESTIMENTI as rivestimento ON a.artcor = rivestimento.rivecod
                        LEFT OUTER JOIN TIPMATPRI as tipomp ON a.arttipmat = tipomp.tmpcod
                        LEFT OUTER JOIN MATERIEPRIME as mp ON a.artmap = mp.matpcod
                        LEFT OUTER JOIN DENTI as denti ON a.artden = denti.Dencod
                        LEFT OUTER JOIN LD as ld ON a.artld = ld.Ldcod
                        LEFT OUTER JOIN FORILUBRIFICATI as elica ON a.artfori = elica.FLcod
                        LEFT OUTER JOIN DIAMETRO as diametro ON a.artdiam = diametro.Diamcod
                        LEFT OUTER JOIN ATTACCO as attacco ON a.artatco = attacco.attacod
                        LEFT OUTER JOIN ANAG_ARTICOLI as mp_associata ON a.artmp1 = mp_associata.ARTCOD
                        LEFT OUTER JOIN ANAG_BASE as for_associato ON a.artfor1m = for_associato.abecod
                        OUTER APPLY(SELECT COUNT(DISTINCT tm.OrdineCommessa) as ct FROM TempiMedi as tm WHERE tm.Societa = @CompanyID AND tm.ArticoloID = a.artcod AND tm.TempoProduzionePezzo > 0 ) as tm
                        OUTER APPLY(SELECT COUNT(DISTINCT tm.OrdineCommessa) as ct FROM TempiMedi as tm
                                    LEFT OUTER JOIN PROD_FASI as fs ON tm.Societa = fs.lavsoc AND tm.FaseID = fs.lavcod
                                    WHERE tm.Societa = @CompanyID AND tm.ArticoloID = a.artcod AND fs.lavcnc = 'S'  AND tm.TempoProduzionePezzo > 0 ) as tmcnc
                        WHERE {whereFilter.ToString()}
                        ORDER BY a.artcod DESC",

                args);

            return new ObservableCollection<tab_articolo>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }


    public tab_articolo? Get(string ID, IDbConnection? Connection = null)
    {
        try
        {
            if (Connection == null)
                Connection = GetOpenConnection();

            return Connection.Query<tab_articolo>(
                @"SELECT 
                        a.artcod as ID, 
                        a.artdes as Descrizione,
                        a.artumi as UnitaID, 
                        a.artgr1 as GroupID,
                        a.artco1 as AccountID,
                        a.artso1 as SubaccountID,
                        analogia.angdesc as artfamDescrizione,
                        rivestimento.rivedes as artcorDescrizione,
                        tipomp.tmpdes as arttipmatDescrizione,
                        mp.matpdes as artmapDescrizione,
                        denti.Dendes as artdenDescrizione,
                        ld.Lddes as artldDescrizione,
                        elica.FLdes as artforiDescrizione,
                        diametro.diamdes as artdiamDescrizione,
                        attacco.attades as artatcoDescrizione,
                        mp_associata.artdise as artmp1Descrizione,
                        for_associato.abers1 as artfor1mDescrizione,
                        a.* FROM ANAG_ARTICOLI AS a
                        LEFT OUTER JOIN ANALOGIE as analogia ON a.artfam = analogia.angcod
                        LEFT OUTER JOIN RIVESTIMENTI as rivestimento ON a.artcor = rivestimento.rivecod
                        LEFT OUTER JOIN TIPMATPRI as tipomp ON a.arttipmat = tipomp.tmpcod
                        LEFT OUTER JOIN MATERIEPRIME as mp ON a.artmap = mp.matpcod
                        LEFT OUTER JOIN DENTI as denti ON a.artden = denti.Dencod
                        LEFT OUTER JOIN LD as ld ON a.artld = ld.Ldcod
                        LEFT OUTER JOIN FORILUBRIFICATI as elica ON a.artfori = elica.FLcod
                        LEFT OUTER JOIN DIAMETRO as diametro ON a.artdiam = diametro.Diamcod
                        LEFT OUTER JOIN ATTACCO as attacco ON a.artatco = attacco.attacod
                        LEFT OUTER JOIN ANAG_ARTICOLI as mp_associata ON a.artmp1 = mp_associata.ARTCOD
                        LEFT OUTER JOIN ANAG_BASE as for_associato ON a.artfor1m = for_associato.abecod
                        WHERE a.artcod = @ID",
                new { ID = ID })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<Tuple<string, string>>? GetDraws(string ArticleID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var attachs = connection.Query<(int CustomerID, string CustomerDescription, string File)>(@"SELECT d.aartcli, b.abers1, d.aafile FROM ANAG_ARTDISEGNI as d INNER JOIN ANAG_BASE as b ON d.aartcli = b.abecod WHERE d.Aartcod = @ArticleID AND d.aarttip = 'P'", new { ArticleID = ArticleID });

            return attachs.Select(s => new Tuple<string, string>($"{s.CustomerID} - {s.CustomerDescription.TrimEnd()}", s.File)).ToList();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion
}