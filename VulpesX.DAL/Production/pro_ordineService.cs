
using MailKit.Search;
using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Auth;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Reports.Production;

namespace VulpesX.DAL.Production;

public interface Ipro_ordineRepository
{
    ObservableCollection<pro_ordine>? GetList(string SocietaID, long Year, string State);

    ObservableCollection<pro_ordine>? GetListByOrder(string SocietaID, int OrderYear, int OrderID);

    pro_ordine? Get(string SocietaID, string ID);

    pro_ordine? GetByOrder(string CompanyID, int Year, int Number, int RowID);

    bool Exists(string SocietaID, string ID);

    #region Production checks
    bool EngageAndClose(pro_ordine Order, ObservableCollection<StockInfo> Availabilities, string UserID);

    void UnlockOrders(string SocietaID, ObservableCollection<acq_orders_rows_jobs> Jobs, string UserID);

    #endregion

    #region Printing
    ProductionReport? PrintProductionOrder(pro_ordine Ordine);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(pro_ordine Model);

    bool Update(pro_ordine Model);

    bool Delete(pro_ordine Model, string UserID);

    string? Validate(pro_ordine Model, bool IsInsert);

    bool UpdateStatus(pro_ordine Model);
    #endregion

    #region Ufp
    ObservableCollection<pro_ordine>? GetPROD_ORDINIFromArticleID(string CompanyID, string ArticleID);
    #endregion
}

public class pro_ordineRepository : RepositoryBase, Ipro_ordineRepository
{
    public pro_ordineRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<pro_ordine>? GetList(string SocietaID, long Year, string State)
    {
        try
        {

            using var connection = GetOpenConnection();

            var orders = connection.Query<pro_ordine>(
                $@"SELECT o.*, cli.abers1 AS ClienteDescrizione , art.Descrizione AS ArticoloDescrizione, art.UnitaID AS UM, DurataPrevista, DurataProduzione, DurataSospensione, DurataPiazzamento FROM pro_ordine AS o 
                    LEFT OUTER JOIN ABE AS cli ON o.ClienteID = cli.abecod 
                    LEFT OUTER JOIN tab_articolo AS art ON art.SocietaID = o.SocietaID AND art.ID = o.ArticoloID 
                    OUTER APPLY(SELECT SUM(com.Tempo * com.Quantita) as DurataPrevista FROM pro_ordine_composizione as com  WHERE com.SocietaID = o.SocietaID AND com.OrdineID = o.ID) as DurataPrevista
                    OUTER APPLY(SELECT SUM(com.Durata) as DurataProduzione FROM pro_ordine_composizione_tempo as com  WHERE com.SocietaID = o.SocietaID AND com.OrdineID = o.ID) as DurataProduzione
                    OUTER APPLY(SELECT SUM(com.DurataSospensione) as DurataSospensione FROM pro_ordine_composizione_tempo as com  WHERE com.SocietaID = o.SocietaID AND com.OrdineID = o.ID) as DurataSospensione
                    OUTER APPLY(SELECT SUM(com.Durata) as DurataPiazzamento FROM pro_ordine_composizione_tempo as com  WHERE com.SocietaID = o.SocietaID AND com.OrdineID = o.ID AND com.TipoID = '7') as DurataPiazzamento
                    WHERE o.SocietaID = @SocietaID AND (o.ID BETWEEN (@Year * 10000000) AND ((@Year * 10000000) + 9999999)) {(State == "*" ? null : (State == "X" ? "AND o.LogCanceled IS NOT NULL" : "AND o.LogCanceled IS NULL AND o.Stato = @status"))}
                    ORDER BY o.ID DESC",
          new { SocietaID = SocietaID, status = State, Year = Year });

            return new ObservableCollection<pro_ordine>(orders);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new ObservableCollection<pro_ordine>();
        }
    }

    public ObservableCollection<pro_ordine>? GetListByOrder(string SocietaID, int OrderYear, int OrderID)
    {
        try
        {

            using var connection = GetOpenConnection();



            var orders = connection.Query<pro_ordine>(
                $@"SELECT o.* FROM pro_ordine AS o 
                    WHERE o.SocietaID = @SocietaID AND o.OrdineClienteAnno = @yea AND o.OrdineClienteID = @oid AND o.LogCanceled IS NULL",
          new { SocietaID = SocietaID, yea = OrderYear, oid = OrderID });

            return new ObservableCollection<pro_ordine>(orders);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new ObservableCollection<pro_ordine>();
        }
    }

    public pro_ordine? Get(string SocietaID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();



            return connection.Query<pro_ordine>(
                $@"SELECT o.*, cli.abers1 AS ClienteDescrizione , art.Descrizione AS ArticoloDescrizione, art.UnitaID AS UM FROM pro_ordine AS o 
                    LEFT OUTER JOIN ABE AS cli ON o.ClienteID = cli.abecod 
                    LEFT OUTER JOIN tab_articolo AS art ON art.SocietaID = o.SocietaID AND art.ID = o.ArticoloID 
                    WHERE o.SocietaID = @SocietaID AND o.LogCanceled IS NULL AND o.ID = @ID
                    ORDER BY o.DataConsegna",
          new { SocietaID = SocietaID, ID = ID }).FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public pro_ordine? GetByOrder(string CompanyID, int Year, int Number, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<pro_ordine>(
                $@"SELECT o.* FROM pro_ordine AS o 
                    WHERE o.SocietaID = @cid AND o.LogCanceled IS NULL AND o.OrdineClienteAnno = @yea AND o.OrdineClienteID=@id AND o.OrdineClienteRiga=@rid
                    ORDER BY o.DataConsegna",
          new { cid = CompanyID, yea = Year, id = Number, rid = RowID }).FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string SocietaID, string ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM pro_ordine WHERE SocietaID = @SocietaID AND ID = @ID",
                new { SocietaID = SocietaID, ID = ID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Production checks
    public bool EngageAndClose(pro_ordine Order, ObservableCollection<StockInfo> Availabilities, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var transaction = connection.BeginTransaction();
            try
            {
                // engages
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();

                foreach (var eng in Availabilities.Where(w => w.QuantityToEngage > 0))
                {
                    var newEngage = new store_stocks_engage()
                    {
                        company_id = Order.SocietaID,
                        id = numRegRepo.GetFullLongID(now.Year, (numRegRepo.GetNumber(Order.SocietaID, now.Year, Constants.STORE_ENGAGES, true))),
                        store_id = eng.StoreID ?? string.Empty,
                        product_id = Order.ArticoloID ?? string.Empty,
                        job_id = Order.ID,
                        order_id = Order.ID,
                        quantity = eng.QuantityToEngage,
                        lot = eng.Lot != Constants.NO_LOT_ID ? eng.Lot : null,
                        add_user = UserID,
                        date_engaged = now
                    };
                    connection.Execute(storeStockEngageRepo.INSERT_QUERY, newEngage, transaction);
                }
                // update order
                Order.LogUpdatedUserID = UserID;
                Order.Stato = "E";
                connection.Execute(UPDATE_QUERY, Order, transaction);

                transaction.Commit();
                return true;
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
            return true;
        }
    }

    public void UnlockOrders(string SocietaID, ObservableCollection<acq_orders_rows_jobs> Jobs, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
            var proOrdineComposizioneRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>();


            foreach (var job in Jobs.Where(w => !string.IsNullOrWhiteSpace(w.job_id)))
            {
                var order = Get(SocietaID, job.job_id);
                var componentsList = proOrdineComposizioneRepo.GetMaterialsListByOrder(SocietaID, job.job_id);
                var engages = storeStockEngageRepo.GetListByOrderID(job.company_id, job.job_id);
                bool ready = true;
                foreach (var comp in componentsList ?? new ObservableCollection<pro_ordine_composizione>())
                {
                    var engage = (engages ?? new ObservableCollection<store_stocks_engage>()).Where(w => w.product_id == comp.ComponenteArticoloID).Sum(sum => sum.quantity);
                    if (engage != null)
                    {
                        if ((comp.Quantita ?? 0) != engage)
                        {
                            ready = false;
                            break;
                        }
                    }
                    else
                    {
                        ready = false;
                        break;
                    }
                }
                if (ready)
                {
                    if (order != null)
                    {
                        order.LogUpdatedUserID = UserID;
                        order.Stato = "R";
                        Update(order);
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
        }
    }

    #endregion

    #region Printing
    public ProductionReport? PrintProductionOrder(pro_ordine Ordine)
    {
        try
        {
            var companyRepo = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>();
            var aziendaRepo = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();

            var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();

            var socbase = companyRepo.Get(Ordine.SocietaID)!;

            //PRODUCTIONS
            using var connection = GetOpenConnection();

            var retValue = new List<pro_ordine_composizione>();

            var productions = connection.Query<pro_ordine_composizione>(
               $@"SELECT c.*, po.*, rep.Descrizione, ris.Descrizione,art.*, com.*, uni.Descrizione, ot.* FROM pro_ordine_composizione AS c 
                    LEFT OUTER JOIN pro_ordine as po ON c.SocietaID = po.SocietaID AND c.OrdineID = po.ID
                    LEFT OUTER JOIN tab_produzione_reparto AS rep ON c.SocietaID = rep.SocietaID AND c.RepartoID = rep.ID 
                    LEFT OUTER JOIN tab_produzione_risorsa AS ris ON c.SocietaID = ris.SocietaID AND c.RisorsaID = ris.ID
                    LEFT OUTER JOIN tab_articolo AS art ON c.SocietaID = art.SocietaID AND c.ArticoloID = art.ID 
                    LEFT OUTER JOIN tab_articolo AS com ON c.SocietaID = com.SocietaID AND c.ComponenteArticoloID = com.ID 
                    LEFT OUTER JOIN tab_articolo_unita AS uni ON com.SocietaID = uni.SocietaID AND com.UnitaID = uni.ID 
                    LEFT OUTER JOIN ORDIT00F as ot ON po.SocietaID = ot.OTSOCI AND po.OrdineClienteAnno = ot.OTANNO AND po.OrdineClienteID = ot.OTNUOR
                    WHERE c.SocietaID = @SocietaID AND c.OrdineID = @OrdineID 
                    order by c.ComposizioneID",
                new[] { typeof(pro_ordine_composizione), typeof(pro_ordine), typeof(string), typeof(string), typeof(tab_articolo), typeof(tab_articolo), typeof(string), typeof(ORDIT00F) }
               , (objects) =>
               {
                   var composizione = (pro_ordine_composizione)objects[0];
                   composizione.DescrizioneMS = (composizione.ESummary || composizione.EMilestone) ? composizione.DescrizioneMS : !string.IsNullOrEmpty(composizione.RepartoID) ? (objects[2] as string) : (objects[5] as tab_articolo)?.Descrizione;
                   composizione.EComponente = true;
                   composizione.RepartoID = !string.IsNullOrEmpty(composizione.RepartoID) ? composizione.RepartoID : null;
                   composizione.ComponenteArticoloID = string.IsNullOrEmpty(composizione.RepartoID) ? composizione.ComponenteArticoloID : null;
                   composizione.RisorsaDescrizione = (objects[3] as string);
                   composizione.RepartoDescrizione = (objects[2] as string);
                   composizione.UnitaDescrizione = (objects[6] as string);
                   composizione.ArticoloDescrizione = (objects[4] as tab_articolo)?.FullDescriptionSearchable;

                   var orderRef = (objects[7] as ORDIT00F)?.OTDE25;
                   var orderNum = (objects[7] as ORDIT00F)?.OTCUNO;
                   var orderDat = (objects[7] as ORDIT00F)?.OTCUDO;

                   var orderCustomerRef = (!string.IsNullOrWhiteSpace(orderRef) ? orderRef : string.Empty) + " " + (!string.IsNullOrWhiteSpace(orderNum) ? orderNum : string.Empty) + " " + (orderDat != null ? orderDat.Value.ToString("dd/MM/yyyy") : string.Empty);

                   Ordine.RiferimentoOrdineCliente = !string.IsNullOrWhiteSpace(orderCustomerRef) ? orderCustomerRef : "NESSUN ORDINE CLIENTE";
                   return composizione;
               },
              new { SocietaID = Ordine.SocietaID, OrdineID = Ordine.ID },
               splitOn: "SocietaID,Descrizione,Descrizione,SocietaID,SocietaID,Descrizione,OTSOCI");


            var straight = productions.GroupBy(g => new { g.ArticoloID, g.RevisioneID }).Count() == 1;

            if (straight)
            {
                retValue = productions.Where(o => o.EReparto).ToList();
            }

            return new ProductionReport()
            {
                CompanyInfo = aziendaRepo.Get(Ordine.SocietaID),
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                Order = Ordine,
                Productions = retValue,
                Engages = storeStockEngageRepo.GetListByOrderIDForPrint(Ordine.SocietaID, Ordine.ID)?.ToList()
            };
        }
        catch (Exception exc)
        {
            ErrorHandler.Show(exc.Message);
            return null;
        }
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO pro_ordine (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,ClienteID,ArticoloID,RevisioneID,DataOrdine,DataConsegna,Quantita,Stato,Commessa,Note,OrdineClienteAnno,OrdineClienteID,OrdineClienteRiga) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@ClienteID,@ArticoloID,@RevisioneID,@DataOrdine,@DataConsegna,@Quantita,@Stato,@Commessa,@Note,@OrdineClienteAnno,@OrdineClienteID,@OrdineClienteRiga)";
    public string UPDATE_QUERY => "UPDATE pro_ordine SET SocietaID = @SocietaID,ID = @ID,LogAdded = @LogAdded,LogUpdated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,ClienteID = @ClienteID,ArticoloID = @ArticoloID, RevisioneID = @RevisioneID,DataOrdine = @DataOrdine,DataConsegna = @DataConsegna,Quantita = @Quantita,Stato = @Stato,Commessa = @Commessa,Note = @Note,OrdineClienteAnno = @OrdineClienteAnno,OrdineClienteID = @OrdineClienteID,OrdineClienteRiga = @OrdineClienteRiga OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public bool Insert(pro_ordine Model)
    {
        try
        {
            var articoloComposizioneRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>();
            var proOrdineComposizioneRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>();

            var articolo = articoloComposizioneRepo.Single(Model.SocietaID, Model.ArticoloID ?? string.Empty, Model.RevisioneID ?? string.Empty, null);

            if (articolo != null)
            {
                var hasCycle = articoloComposizioneRepo.HasCycle(articolo.SocietaID, articolo.ArticoloID);
                var resultComposizione = proOrdineComposizioneRepo.Save(Model.SocietaID, Model.ID, Model.Quantita ?? 0, Model.UM ?? string.Empty, Model.ArticoloID ?? string.Empty, Model.RevisioneID ?? string.Empty, articolo, 0);

                if (resultComposizione > 0 || !hasCycle)
                {
                    using var connection = GetOpenConnection();


                    if (!hasCycle)
                        Model.Stato = "E";
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
                else
                {
                    ErrorHandler.Show("Errore nella creazione del ciclo, verificare che il ciclo sia corretto e che l'articolo abbia una revisione di default");
                    return false;
                }
            }
            else
            {
                ErrorHandler.Show("Errore nella creazione del ciclo, articolo non trovato");
                return false;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.ToString());
            return false;
        }
    }

    public bool Update(pro_ordine Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
            if (result != null)
            {
                if (Model.Stato == "O")
                {
                    var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                    var storeStockLotRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>();
                    var ordineComposizioneRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>();
                    var proOrdineComposizioneRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>();
                    var proOrdineComposizioneLottoRepo = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizione_lottoRepository>();

                    // write pro_ordine_composizione_lotto
                    // clean before
                    connection.Execute(@"DELETE FROM pro_ordine_composizione_lotto
                                            WHERE SocietaID = @cid AND OrdineID = @oid", new { cid = Model.SocietaID, oid = Model.ID });
                    // add for each pro_ordine_composizione where
                    // - componentearticoloid not null and articoloid==product_id.pro_ordine
                    // - articoloid != product_is.pro_ordine
                    // and write a row for each lot engaged
                    var engagesForOrder = storeStockEngageRepo.GetListByOrderID(Model.SocietaID, Model.ID);
                    var components = proOrdineComposizioneRepo.GetMaterialsListByOrder(Model.SocietaID, Model.ID);
                    int prog = 1;
                    foreach (var comp in components ?? new ObservableCollection<pro_ordine_composizione>())
                    {
                        foreach (var eng in (engagesForOrder ?? new ObservableCollection<store_stocks_engage>()).Where(w => w.product_id == comp.ComponenteArticoloID && w.order_id == Model.ID))
                        {
                            var lotInfo = storeStockLotRepo.Get(eng.company_id, eng.store_id, eng.product_id, eng.lot ?? string.Empty);
                            var newLot = new pro_ordine_composizione_lotto()
                            {
                                SocietaID = Model.SocietaID,
                                OrdineID = Model.ID,
                                ArticoloID = comp.ArticoloID,
                                RevisioneID = Model.RevisioneID ?? string.Empty,
                                ComposizioneID = comp.ComposizioneID,
                                ID = prog++,
                                Lotto = eng.lot,
                                Quantita = eng.quantity,
                                product_id = comp.ComponenteArticoloID ?? string.Empty,
                                DataScadenza = lotInfo?.expire,
                                LogAddedUserID = Model.LogUpdatedUserID
                            };
                            connection.Execute(proOrdineComposizioneLottoRepo.INSERT_QUERY, newLot);
                        }
                    }
                }
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

    public bool Delete(pro_ordine Model, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();



            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                    var orditRepo = VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>();

                    connection.Execute(DELETE_QUERY, Model, transaction);

                    // pulisce pro_ordine_composizione
                    connection.Execute("DELETE FROM pro_ordine_composizione WHERE SocietaID = @SocietaID AND OrdineID = @ID", new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    // pulisce pro_ordine_composizione_tempo
                    connection.Execute("DELETE FROM pro_ordine_composizione_tempo WHERE SocietaID = @SocietaID AND OrdineID = @ID", new { SocietaID = Model.SocietaID, ID = Model.ID }, transaction);

                    // elimina tutti gli impegni
                    var clearedEngages = storeStockEngageRepo.FreeByProductionOrder(Model.SocietaID, Model.ID, UserID);

                    transaction.Commit();

                    // if all ok reset flgchi to F and ODSTA to null to allow generation only of linked row
                    if (Model.OrdineClienteAnno.HasValue && Model.OrdineClienteAnno.Value > 0)
                    {
                        var head = orditRepo.Get(Model.SocietaID, Model.OrdineClienteAnno ?? 0, (int)(Model.OrdineClienteID ?? 0));

                        if (head != null)
                        {
                            head.updatedUserID = UserID;
                            head.flgchi = "F";
                            orditRepo.Update(head);
                        }
                    }

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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(pro_ordine Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.SocietaID) && IsInsert && !Exists(Model.SocietaID, Model.ID)) || !IsInsert)
            {
                if ((Model.Quantita) > 0)
                {
                    if ((Model.ClienteID ?? 0) > 0)
                    {
                        if (!string.IsNullOrEmpty(Model.ArticoloID))
                        {
                            return null;
                        }
                        else
                        {
                            return "Selezionare un codice articolo e una revisione valida";
                        }
                    }
                    else
                    {
                        return "Selezionare un cliente";
                    }
                }
                else
                {
                    return "La quantità dell'ordine deve essere maggiore di 0";
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

    public bool UpdateStatus(pro_ordine Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(UPDATE_QUERY, Model);
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
            ErrorHandler.Show(ex.ToString());
            return false;
        }

    }
    #endregion

    #region Ufp
    public ObservableCollection<pro_ordine>? GetPROD_ORDINIFromArticleID(string CompanyID, string ArticleID)
    {
        throw new NotImplementedException();
    }
    #endregion
}

public class pro_ordineUfpRepository : RepositoryBase, Ipro_ordineRepository
{
    public pro_ordineUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<pro_ordine>? GetList(string SocietaID, long Year, string State)
    {
        throw new NotImplementedException();
    }

    public ObservableCollection<pro_ordine>? GetListByOrder(string SocietaID, int OrderYear, int OrderID)
    {
        throw new NotImplementedException();
    }

    public pro_ordine? Get(string SocietaID, string ID)
    {
        throw new NotImplementedException();
    }

    public pro_ordine? GetByOrder(string CompanyID, int Year, int Number, int RowID)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string SocietaID, string ID)
    {
        throw new NotImplementedException();
    }

    #region Production checks
    public bool EngageAndClose(pro_ordine Order, ObservableCollection<StockInfo> Availabilities, string UserID)
    {
        throw new NotImplementedException();
    }

    public void UnlockOrders(string SocietaID, ObservableCollection<acq_orders_rows_jobs> Jobs, string UserID)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Printing
    public ProductionReport? PrintProductionOrder(pro_ordine Ordine)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO pro_ordine (SocietaID,ID,LogAdded,LogUpdated,LogCanceled,LogAddedUserID,LogUpdatedUserID,LogCanceledUserID,ClienteID,ArticoloID,RevisioneID,DataOrdine,DataConsegna,Quantita,Stato,Commessa,Note,OrdineClienteAnno,OrdineClienteID,OrdineClienteRiga) OUTPUT INSERTED.rv VALUES(@SocietaID,@ID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@LogUpdated,@LogCanceled,@LogAddedUserID,@LogUpdatedUserID,@LogCanceledUserID,@ClienteID,@ArticoloID,@RevisioneID,@DataOrdine,@DataConsegna,@Quantita,@Stato,@Commessa,@Note,@OrdineClienteAnno,@OrdineClienteID,@OrdineClienteRiga)";
    public string UPDATE_QUERY => "UPDATE pro_ordine SET SocietaID = @SocietaID,ID = @ID,LogAdded = @LogAdded,LogUpdated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',LogCanceled = @LogCanceled,LogAddedUserID = @LogAddedUserID,LogUpdatedUserID = @LogUpdatedUserID,LogCanceledUserID = @LogCanceledUserID,ClienteID = @ClienteID,ArticoloID = @ArticoloID, RevisioneID = @RevisioneID,DataOrdine = @DataOrdine,DataConsegna = @DataConsegna,Quantita = @Quantita,Stato = @Stato,Commessa = @Commessa,Note = @Note,OrdineClienteAnno = @OrdineClienteAnno,OrdineClienteID = @OrdineClienteID,OrdineClienteRiga = @OrdineClienteRiga OUTPUT INSERTED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM pro_ordine OUTPUT DELETED.rv WHERE SocietaID = @SocietaID AND ID = @ID AND rv = @rv";
    public bool Insert(pro_ordine Model)
    {
        throw new NotImplementedException();
    }

    public bool Update(pro_ordine Model)
    {
        throw new NotImplementedException();
    }

    public bool Delete(pro_ordine Model, string UserID)
    {
        throw new NotImplementedException();
    }

    public string? Validate(pro_ordine Model, bool IsInsert)
    {
        throw new NotImplementedException();
    }

    public bool UpdateStatus(pro_ordine Model)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Ufp
    public ObservableCollection<pro_ordine>? GetPROD_ORDINIFromArticleID(string CompanyID, string ArticleID)
    {
        try
        {
            using var connection = GetOpenConnection();

            var orders = connection.Query<pro_ordine>(
                $@"SELECT 
                    o.opsoci as SocietaID,
                    o.opcomme as Commessa,
                    o.opclie as ClienteID
                    FROM PROD_ORDINI AS o 
                    WHERE o.opsoci = @CompanyID AND o.OPCOAR = @ArticleID",
          new { CompanyID, ArticleID });

            return new ObservableCollection<pro_ordine>(orders);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return new ObservableCollection<pro_ordine>();
        }
    }
    #endregion
}