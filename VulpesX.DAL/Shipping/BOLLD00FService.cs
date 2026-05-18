

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VulpesX.DAL;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Shipping;

namespace VulpesX.DAL.Shipping;

public interface IBOLLD00FRepository
{
    BOLLD00F? Get(string CompanyID, int Year, int Number, int RowID);

    Tuple<decimal, decimal, decimal>? GetRowsTotalQuantity(string CompanyID, int Year, int Number);

    bool HasDDTByOrder(string bolsoc, int BOANNO, int BONUOR, int BORIGA);

    bool Exists(string bolsoc, int BTANNO, int BTBOLL, int BORIGB);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(BOLLD00F Model);

    bool Update(BOLLD00F Model);

    bool UpdateAll(BOLLT00F Head, ObservableCollection<BOLLD00F> Rows, string UserID);

    bool UpdateAllDefinitive(BOLLT00F Head, ObservableCollection<BOLLD00F> Rows, string UserID);

    bool Delete(BOLLD00F Model);

    string? Validate(BOLLD00F Model, bool IsInsert);

    string? ValidateModel(ObservableCollection<BOLLD00F>? Rows);
    #endregion

}

public class BOLLD00FRepository : RepositoryBase, IBOLLD00FRepository
{
    public BOLLD00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public BOLLD00F? Get(string CompanyID, int Year, int Number, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BOLLD00F>(
                "SELECT * FROM BOLLD00F WHERE bolsoc = @bolsoc AND BTANNO = @btanno AND BTBOLL = @btboll AND BORIGB = @borigb",
                new { bolsoc = CompanyID, btanno = Year, btboll = Number, borigb = RowID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public Tuple<decimal, decimal, decimal>? GetRowsTotalQuantity(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();

            var multi = connection.QueryMultiple(@"SELECT SUM(BOQTAV) FROM BOLLD00F WHERE bolsoc = @cid AND BTANNO = @btanno AND BTBOLL = @btboll;
                                            SELECT f1.boqtlo, f1.BORIGB, p.UnitaID AS UM, r.BOUNIM AS RowUM, r.BOQTAV AS RowQuantity FROM BOLLD00F1 AS f1 
                                            INNER JOIN tab_articolo AS p ON p.SocietaID=f1.bolsoc AND p.ID=f1.product_id
                                            INNER JOIN BOLLD00F AS r ON r.bolsoc=f1.bolsoc AND r.BTANNO=f1.BTANNO AND r.BTBOLL=f1.BTBOLL AND r.BORIGB=f1.BORIGB
                                            WHERE f1.bolsoc = @cid AND f1.BTANNO = @btanno AND f1.BTBOLL = @btboll;
                                            SELECT e.quantity, e.ddt_row, p.UnitaID AS UM, r.BOUNIM AS RowUM, r.BOQTAV AS RowQuantity FROM store_stocks_engage AS e
                                            INNER JOIN tab_articolo AS p ON p.SocietaID=e.company_id AND p.ID=e.product_id
                                            INNER JOIN BOLLD00F AS r ON r.bolsoc=e.company_id AND r.BTANNO=e.ddt_year AND r.BTBOLL=e.ddt_number AND r.BORIGB=e.ddt_row
                                            WHERE e.company_id = @cid AND e.ddt_year = @btanno AND e.ddt_number = @btboll AND e.canceled IS NULL AND e.date_unloaded IS NULL;"
                                        , new { cid = CompanyID, btanno = Year, btboll = Number });
            var needed = multi.Read<decimal>().First();
            decimal engaged = 0;
            foreach (var item in multi.Read<BOLLD00F1>().GroupBy(g => new { g.BORIGB, g.UM }, (rKey, items) => new { rKey, items }))
            {
                if (item.items.First().RowUM == item.rKey.UM)
                {
                    engaged += item.items.Sum(sum => sum.boqtlo);
                }
                else
                {
                    if (item.items.Sum(sum => sum.boqtlo) > 0)
                        engaged += item.items.First().RowQuantity;
                }
            }
            decimal realEngaged = 0;
            foreach (var item in multi.Read<store_stocks_engage>().GroupBy(g => new { g.ddt_row, g.UM }, (rKey, items) => new { rKey, items }))
            {
                if (item.items.First().RowUM == item.rKey.UM)
                {
                    realEngaged += (item.items.Sum(sum => sum.quantity) ?? 0);
                }
                else
                {
                    if ((item.items.Sum(sum => sum.quantity) ?? 0) > 0)
                        realEngaged += item.items.First().RowQuantity;
                }
            }
            return new Tuple<decimal, decimal, decimal>(needed, engaged, realEngaged);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool HasDDTByOrder(string bolsoc, int BOANNO, int BONUOR, int BORIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM BOLLD00F WHERE bolsoc = @bolsoc AND BOANNO = @BOANNO AND BONUOR = @BONUOR AND BORIGA = @BORIGA",
                new { bolsoc = bolsoc, BOANNO = BOANNO, BONUOR = BONUOR, BORIGA = BORIGA }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public bool Exists(string bolsoc, int BTANNO, int BTBOLL, int BORIGB)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM BOLLD00F WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL, BORIGB = BORIGB }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO BOLLD00F (bolsoc,BTANNO,BTBOLL,BORIGB,BOANNO,BONUOR,BORIGA,BOMAGA,BOCODA,BOQTAV,BOTQTA,BOUNIM,BODACO,BOSERI,BORIFC,boprez,botpre,boaliq,boasso,bogrup,bocont,bosotc,bosco1,bosco2,bosco3,bomagg,botsc1,botsc2,botsc3,botmag,BONOTE,BOSHOW,BOCOA1,BOCOA2,BOCOA1P,BOCOA2P,BOCOA1PT,BOCOA2PT) OUTPUT INSERTED.rv VALUES(@bolsoc,@BTANNO,@BTBOLL,@BORIGB,@BOANNO,@BONUOR,@BORIGA,@BOMAGA,@BOCODA,@BOQTAV,@BOTQTA,@BOUNIM,@BODACO,@BOSERI,@BORIFC,@boprez,@botpre,@boaliq,@boasso,@bogrup,@bocont,@bosotc,@bosco1,@bosco2,@bosco3,@bomagg,@botsc1,@botsc2,@botsc3,@botmag,@BONOTE,@BOSHOW,@BOCOA1,@BOCOA2,@BOCOA1P,@BOCOA2P,@BOCOA1PT,@BOCOA2PT)";
    public string UPDATE_QUERY => "UPDATE BOLLD00F SET bolsoc = @bolsoc,BTANNO = @BTANNO,BTBOLL = @BTBOLL,BORIGB = @BORIGB,BOANNO = @BOANNO,BONUOR = @BONUOR,BORIGA = @BORIGA,BOMAGA = @BOMAGA,BOCODA = @BOCODA,BOQTAV = @BOQTAV,BOTQTA = @BOTQTA,BOUNIM = @BOUNIM,BODACO = @BODACO,BOSERI = @BOSERI,BORIFC = @BORIFC,boprez = @boprez,botpre = @botpre,boaliq = @boaliq,boasso = @boasso,bogrup = @bogrup,bocont = @bocont,bosotc = @bosotc,bosco1 = @bosco1,bosco2 = @bosco2,bosco3 = @bosco3,bomagg = @bomagg,botsc1 = @botsc1,botsc2 = @botsc2,botsc3 = @botsc3,botmag = @botmag,BONOTE = @BONOTE,BOSHOW = @BOSHOW,BOCOA1 = @BOCOA1,BOCOA2 = @BOCOA2,BOCOA1P = @BOCOA1P,BOCOA2P = @BOCOA2P,BOCOA1PT = @BOCOA1PT,BOCOA2PT = @BOCOA2PT OUTPUT INSERTED.rv WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM BOLLD00F OUTPUT DELETED.rv WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND BORIGB = @BORIGB AND rv = @rv";
    public bool Insert(BOLLD00F Model)
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

    public bool Update(BOLLD00F Model)
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

    public bool UpdateAll(BOLLT00F Head, ObservableCollection<BOLLD00F> Rows, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();



            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var storeMovementRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>();
                var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                var storeStockRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>();
                var storeStockLotRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>();
                var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();

                var bolld1Repo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1Repository>();
                var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();

                // delete current rows
                connection.Execute("DELETE FROM BOLLD00F WHERE bolsoc = @bolsoc AND BTANNO = @otanno AND BTBOLL = @btboll",
                    new { bolsoc = Head.bolsoc, otanno = Head.BTANNO, btboll = Head.BTBOLL },
                    transaction);

                decimal grossWeight = 0;
                decimal netWeight = 0;
                foreach (var row in Rows)
                {
                    connection.Execute(
                    INSERT_QUERY, row, transaction);
                    // compute weight
                    var weights = tabArticoloRepo.ComputeWeight(row.bolsoc, row.BOCODA ?? string.Empty, row.BOQTAV, row.BOUNIM ?? string.Empty);
                    grossWeight = weights.Item1;
                    netWeight = weights.Item2;
                }

                #region Engages
                // free current engages
                foreach (var cur in connection.Query<store_stocks_engage>(@"SELECT * FROM store_stocks_engage
                                                            WHERE canceled IS NULL AND date_unloaded IS NULL AND ddt_year=@BTANNO AND ddt_number=@BTBOLL", Head, transaction))
                {
                    cur.canceled = now;
                    cur.cancel_user = UserID;
                    connection.Execute(storeStockEngageRepo.UPDATE_QUERY, cur, transaction);
                }
                // delete current engages rows
                connection.Execute("DELETE FROM BOLLD00F1 WHERE bolsoc = @bolsoc AND BTANNO = @otanno AND BTBOLL = @btboll",
                    new { bolsoc = Head.bolsoc, otanno = Head.BTANNO, btboll = Head.BTBOLL },
                    transaction);
                // add fresh engages
                foreach (var row in Rows)
                {
                    if (row.EngagesRows != null)
                    {
                        foreach (var eng in row.EngagesRows)
                        {
                            connection.Execute(bolld1Repo.INSERT_QUERY, eng, transaction);
                            var engage = new store_stocks_engage()
                            {
                                company_id = eng.bolsoc,
                                id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(eng.bolsoc, now.Year, Constants.STORE_ENGAGES, true)),
                                store_id = eng.store_id,
                                product_id = eng.product_id,
                                quantity = eng.boqtlo,
                                date_engaged = now,
                                add_user = UserID,
                                lot = eng.bolott,
                                ddt_year = row.BTANNO,
                                ddt_number = row.BTBOLL,
                                ddt_row = row.BORIGB
                            };
                            connection.Execute(storeStockEngageRepo.INSERT_QUERY, engage, transaction);
                        }
                    }
                }
                #endregion

                // update head
                Head.BTPESO = grossWeight;
                Head.BTPES2 = netWeight;
                connection.ExecuteScalar(bolltRepo.UPDATE_QUERY, Head, transaction);

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
            return false;
        }
    }

    public bool UpdateAllDefinitive(BOLLT00F Head, ObservableCollection<BOLLD00F> Rows, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();

            using var transaction = connection.BeginTransaction();
            try
            {
                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
                var storeMovementRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>();
                var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
                var storeStockRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocksRepository>();
                var storeStockLotRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>();
                var tabArticoloRepo = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>();

                var bolld1Repo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1Repository>();
                var causBollRepo = VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>();
                var storeCausalRepo = VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>();
                var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();


                // cancel all store_movements and restore stocks
                foreach (var mov in connection.Query<store_movements>("SELECT * FROM store_movements WHERE company_id=@cid AND document_year=@dyear AND document_id=@did AND canceled IS NULL",
                                                                    new { cid = Head.bolsoc, dyear = Head.BTANNO, did = Head.BTBOLL }, transaction))
                {
                    mov.canceled = now;
                    mov.cancel_user = UserID;
                    mov.canceledNote = "Annullato per modifica DDT definitivo";
                    connection.Execute(storeMovementRepo.UPDATE_QUERY, mov, transaction);
                    var movProduct = tabArticoloRepo.GetSingle(mov.company_id, mov.product_id);

                    if (movProduct != null)
                    {
                        if (!movProduct.IsInfinite)
                        {
                            // update stock
                            var stock = connection.Query<store_stocks>("SELECT * FROM store_stocks WHERE company_id=@cid AND store_id=@sid AND product_id=@pid", new { cid = mov.company_id, sid = mov.store_id, pid = mov.product_id }, transaction).FirstOrDefault();
                            if (stock != null)
                            {
                                stock.quantity_stock += mov.quantity;
                                connection.Execute(storeStockRepo.UPDATE_QUERY, stock, transaction);
                            }
                            // update lot stock
                            var stockLot = connection.Query<store_stocks_lots>("SELECT * FROM store_stocks_lots WHERE company_id=@cid AND store_id=@sid AND product_id=@pid AND lot=@lot", new { cid = mov.company_id, sid = mov.store_id, pid = mov.product_id, lot = mov.lot }, transaction).FirstOrDefault();
                            if (stockLot != null)
                            {
                                stockLot.quantity_stock += mov.quantity;
                                connection.Execute(storeStockLotRepo.UPDATE_QUERY, stockLot, transaction);
                            }
                        }
                    }
                }
                // delete current rows
                connection.Execute("DELETE FROM BOLLD00F WHERE bolsoc = @bolsoc AND BTANNO = @otanno AND BTBOLL = @btboll",
                    new { bolsoc = Head.bolsoc, otanno = Head.BTANNO, btboll = Head.BTBOLL },
                    transaction);

                decimal grossWeight = 0;
                decimal netWeight = 0;
                foreach (var row in Rows)
                {
                    connection.Execute(INSERT_QUERY, row, transaction);
                    // compute weight
                    var weights = tabArticoloRepo.ComputeWeight(row.bolsoc, row.BOCODA ?? string.Empty, row.BOQTAV, row.BOUNIM ?? string.Empty);
                    grossWeight = weights.Item1;
                    netWeight = weights.Item2;
                }

                #region Engages
                // free current engages
                foreach (var cur in connection.Query<store_stocks_engage>(@"SELECT * FROM store_stocks_engage
                                                            WHERE canceled IS NULL AND ddt_year=@BTANNO AND ddt_number=@BTBOLL", Head, transaction))
                {
                    cur.canceled = now;
                    cur.cancel_user = UserID;
                    connection.Execute(storeStockEngageRepo.UPDATE_QUERY, cur, transaction);
                }
                // delete current engages rows
                connection.Execute("DELETE FROM BOLLD00F1 WHERE bolsoc = @bolsoc AND BTANNO = @otanno AND BTBOLL = @btboll",
                    new { bolsoc = Head.bolsoc, otanno = Head.BTANNO, btboll = Head.BTBOLL },
                    transaction);
                // add fresh engages already unloaded
                foreach (var row in Rows)
                {
                    if (row.EngagesRows != null)
                    {
                        foreach (var eng in row.EngagesRows)
                        {
                            connection.Execute(bolld1Repo.INSERT_QUERY, eng, transaction);
                            var engage = new store_stocks_engage()
                            {
                                company_id = eng.bolsoc,
                                id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(eng.bolsoc, now.Year, Constants.STORE_ENGAGES, true)),
                                store_id = eng.store_id,
                                product_id = eng.product_id,
                                quantity = eng.boqtlo,
                                date_engaged = now,
                                date_unloaded = now,
                                add_user = UserID,
                                lot = eng.bolott,
                                ddt_year = row.BTANNO,
                                ddt_number = row.BTBOLL,
                                ddt_row = row.BORIGB
                            };
                            connection.Execute(storeStockEngageRepo.INSERT_QUERY, engage, transaction);
                            // add store movement and update stocks
                            // update lot stock
                            DateTime? expire = null;
                            string? goodsLocation = null;
                            if (!string.IsNullOrWhiteSpace(engage.lot))
                            {
                                var stockLot = connection.Query<store_stocks_lots>("SELECT * FROM store_stocks_lots WHERE company_id=@cid AND store_id=@sid AND product_id=@pid AND lot=@lot", new { cid = engage.company_id, sid = engage.store_id, pid = engage.product_id, lot = engage.lot }, transaction).FirstOrDefault();
                                expire = stockLot?.expire;
                                goodsLocation = stockLot?.goods_location;
                            }
                            var causal = causBollRepo.Get(Head.BTCAUS ?? string.Empty);
                            var storeCausal = storeCausalRepo.Get(Head.bolsoc, causal?.BOLCAU ?? string.Empty);
                            var movement = new store_movements()
                            {
                                company_id = eng.bolsoc,
                                id = numRegRepo.GetFullLongID(now.Year, numRegRepo.GetNumber(eng.bolsoc, now.Year, Constants.STORE_MOVEMENTS, true)),
                                date = now,
                                causal_id = storeCausal?.id,
                                Causal = storeCausal,
                                document_date = Head.BTDATA,
                                document_year = Head.BTANNO,
                                document_id = Head.BTBOLL.ToString(),
                                document_row = row.BORIGB,
                                product_id = eng.product_id,
                                quantity = eng.boqtlo,
                                store_id = eng.store_id,
                                added = now,
                                engage_id = engage.id,
                                add_user = UserID,
                                lot = eng.bolott,
                                goods_location = goodsLocation,
                                expire = expire
                            };
                            storeMovementRepo.Insert(movement, connection, transaction);
                        }
                    }
                }
                #endregion

                // update head
                Head.BTPESO = grossWeight;
                Head.BTPES2 = netWeight;
                connection.ExecuteScalar(bolltRepo.UPDATE_QUERY, Head, transaction);

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
            return false;
        }
    }

    public bool Delete(BOLLD00F Model)
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

    public string? Validate(BOLLD00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.bolsoc) && IsInsert) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.Product?.ID))
                {
                    if (Model.BOTQTA != "O" || (Model.BOTQTA == "O" && Model.Rate?.assomaBool == true))
                    {
                        if (Model.BOQTAV > 0)
                        {
                            if (!Model.Product.QuantitaDefault.HasValue ||
                                (Model.Product.QuantitaDefault.HasValue && Model.BOUNIM == Model.Product.UnitaIDAlt && (Model.BOQTAV % 1 == 0 ? (Model.BOQTAV * (Model.Product.QuantitaDefault ?? 1)) % (Model.Product.QuantitaDefault ?? 1) == 0 : (Model.BOQTAV % (Model.Product.QuantitaDefault ?? 1) == 0))) ||
                                (Model.Product.QuantitaDefault.HasValue && Model.BOUNIM != Model.Product.UnitaIDAlt && (Model.BOQTAV % (Model.Product.QuantitaDefault ?? 1) == 0)))
                            {
                                if (Model.boprez > 0 || (Model.boprez == 0 && Model.BOTQTA == "N"))
                                {
                                    if (!string.IsNullOrWhiteSpace(Model.Rate?.assali) && !string.IsNullOrWhiteSpace(Model.Rate?.asscod))
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.Group?.P1GRUP) &&
                                            !string.IsNullOrWhiteSpace(Model.Account?.P2CONT) &&
                                            !string.IsNullOrWhiteSpace(Model.Subaccount?.P3SOTC))
                                        {
                                            if (((Model.bosco1.HasValue && !string.IsNullOrWhiteSpace(Model.botsc1)) ||
                                                    (!Model.bosco1.HasValue && string.IsNullOrWhiteSpace(Model.botsc1))) &&
                                                    ((Model.bosco2.HasValue && !string.IsNullOrWhiteSpace(Model.botsc2)) ||
                                                    (!Model.bosco2.HasValue && string.IsNullOrWhiteSpace(Model.botsc2))) &&
                                                    ((Model.bosco3.HasValue && !string.IsNullOrWhiteSpace(Model.botsc3)) ||
                                                    (!Model.bosco3.HasValue && string.IsNullOrWhiteSpace(Model.botsc3))))
                                            {
                                                return null;
                                            }
                                            else
                                            { return "Se si seleziona uno sconto e' necessario impostarne anche il tipo altrimenti ometterli entrambi"; }
                                        }
                                        else
                                        { return "Il conto contabile è obbligatorio"; }
                                    }
                                    else
                                    { return "L'aliquota è obbligatoria"; }
                                }
                                else
                                { return "Il prezzo è obbligatorio"; }
                            }
                            else
                            { return $"La quantità digitata ({Model.BOQTAV.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.Product.QuantitaDefault ?? 1).ToString("N6")})"; }
                        }
                        else
                        { return "La quantita' è obbligatoria"; }
                    }
                    else
                    { return "In caso di omaggio l'aliquota deve essere una di quelle abilitate agli omaggi"; }
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

    public string? ValidateModel(ObservableCollection<BOLLD00F>? Rows)
    {
        var storeStockEngageRepo = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_engageRepository>();
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
                // check engages
                foreach (var row in Rows)
                {
                    if (row.EngagesRows != null)
                    {
                        foreach (var eng in row.EngagesRows)
                        {
                            decimal totalUsedSameLot = 0;
                            foreach (var innRow in Rows)
                            {
                                totalUsedSameLot += (innRow.EngagesRows ?? new ObservableCollection<BOLLD00F1>()).Where(w => w.product_id == eng.product_id && w.bolott == eng.bolott && w.store_id == eng.store_id && w.boposc != eng.boposc).Sum(sum => sum.boqtlo);
                            }
                            var realAvailability = eng.Lot?.AvailableQuantity == decimal.MaxValue ? decimal.MaxValue : eng.Lot?.AvailableQuantity + (storeStockEngageRepo.GetQuantityEngagedByDDT(eng.bolsoc, eng.product_id, eng.store_id, eng.Lot?.lot ?? string.Empty, eng.BTANNO, eng.BTBOLL) - totalUsedSameLot);
                            if (eng.boqtlo > realAvailability)
                                validation = $"La quantità [{eng.boqtlo.ToString("N6")}] selezionata per l'articolo {eng.product_id}, riga [{row.BORIGB}], supera la disponibilità effettiva [{realAvailability?.ToString("N6")}]";
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(validation))
                {
                    return null;
                }
                else
                {
                    return validation;
                }
            }
            else
            { return validation; }
        }
        else
        {
            return "E' necessario che siano presenti delle righe per confermare il DDT";
        }

    }
    #endregion
}