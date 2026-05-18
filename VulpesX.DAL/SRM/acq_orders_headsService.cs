

using Microsoft.Extensions.DependencyInjection;
using System.Text;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.General;
using VulpesX.Models.Reports.SRM;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.SRM;

public interface Iacq_orders_headsRepository
{
    ObservableCollection<acq_orders_heads>? GetList(string CompanyID, string ShowType);

    acq_orders_heads? Get(string company_id, long id);

    acq_orders_heads? GetFull(string company_id, long id);

    bool Exists(string company_id, long id);


    #region Printing
    acq_orders_heads? GetPrintFull(string CompanyID, long ID);

    PurchaseOrderReport? PrintPurchaseOrder(acq_orders_heads PurchaseOrder);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(acq_orders_heads Model);

    bool InsertFull(acq_orders_heads Model);

    bool Update(acq_orders_heads Model);

    bool Delete(acq_orders_heads Model);

    string? Validate(acq_orders_heads Model, bool IsInsert);
    #endregion
}

public class acq_orders_headsRepository : RepositoryBase, Iacq_orders_headsRepository
{
    public acq_orders_headsRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public const string ERR_CANNOT_DELETE = "Impossibile eliminare un'ordine di acquisto chiuso, inviato o gia' annullato";

    public ObservableCollection<acq_orders_heads>? GetList(string CompanyID, string ShowType)
    {
        try
        {
            using var connection = GetOpenConnection();


            // compose filters
            var sbFilters = new StringBuilder();
            if (ShowType != "*")
            {
                switch (ShowType)
                {
                    case "D":
                        sbFilters.Append(" AND a.canceled IS NULL AND a.closed IS NULL AND a.commercial_signed IS NOT NULL AND a.management_signed IS NULL");
                        break;
                    case "M":
                        sbFilters.Append(" AND a.canceled IS NULL AND a.closed IS NULL AND a.commercial_signed IS NULL AND a.management_signed IS NULL");
                        break;
                    case "F":
                        sbFilters.Append(" AND a.canceled IS NULL AND a.sent IS NULL AND a.closed IS NULL AND a.commercial_signed IS NOT NULL AND a.management_signed IS NOT NULL");
                        break;
                    case "C":
                        sbFilters.Append(" AND a.canceled IS NULL AND a.closed IS NOT NULL");
                        break;
                    case "S":
                        sbFilters.Append(" AND a.canceled IS NULL AND a.closed IS NULL AND a.sent IS NOT NULL");
                        break;
                    case "X":
                        sbFilters.Append(" AND a.canceled IS NOT NULL");
                        break;
                }
            }
            var list = connection.Query<acq_orders_heads, ABE, PAGFOR, acq_orders_heads>(
                $@"SELECT a.*, b.abecod, b.abers1, b.abers2, g.pfocod, g.pfodes FROM acq_orders_heads AS a
                        LEFT JOIN ABE AS b ON b.abecod = a.supplier_id
                        LEFT JOIN PAGFOR AS g ON g.pfocod = a.payment_id
                        WHERE a.company_id = @cid {sbFilters.ToString()}
                        ORDER BY a.id DESC",
                (acq, abe, pay) => { acq.SupplierGrid = abe; acq.PaymentGrid = pay; return acq; },
                new { cid = CompanyID }, splitOn: "abecod,pfocod");
            // light rows
            var rows = new ObservableCollection<acq_orders_rows>(connection.Query<acq_orders_rows>(
                    $@"SELECT r.company_id,r.id,r.quantity,r.discount,r.price,r.discount_type,r.price_type FROM acq_orders_rows AS r
                            INNER JOIN acq_orders_heads AS a ON a.company_id=r.company_id AND a.id=r.id
                            WHERE r.company_id = @cid {sbFilters.ToString()}",
                    new { cid = CompanyID }).ToList());
            Parallel.ForEach(list, (item) =>
            {
                item.Rows = new ObservableCollection<acq_orders_rows>(rows.Where(w => w.id == item.id));
            });
            return new ObservableCollection<acq_orders_heads>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_heads? Get(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<acq_orders_heads, ABE, acq_orders_heads>(
                @"SELECT h.*, s.abecod, s.abers1, s.abers2 FROM acq_orders_heads AS h
                        LEFT JOIN ABE AS s ON s.abecod = h.supplier_id
                        WHERE h.company_id = @company_id AND h.id = @id",
                (acq, sup) => { acq.SupplierGrid = sup; acq.Supplier = sup; return acq; },
                new { company_id = company_id, id = id }, splitOn: "abecod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public acq_orders_heads? GetFull(string company_id, long id)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<acq_orders_heads, ABE, acq_orders_heads>(
                @"SELECT h.*, s.abecod, s.abers1, s.abers2 FROM acq_orders_heads AS h
                        LEFT JOIN ABE AS s ON s.abecod = h.supplier_id
                        WHERE h.company_id = @company_id AND h.id = @id",
                (acq, sup) => { acq.SupplierGrid = sup; return acq; },
                new { company_id = company_id, id = id }, splitOn: "abecod")
                .FirstOrDefault();

            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(company_id);
            var productTypes = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_tipoRepository>().GetList(company_id, false);

            if (result != null)
            {
                result.Rows = new ObservableCollection<acq_orders_rows>(connection.Query<acq_orders_rows, tab_articolo, acq_orders_rows>(
                        @"SELECT r.*, p.* FROM acq_orders_rows AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.company_id AND p.ID = r.product_id
                            WHERE r.company_id = @cid AND r.id = @id",
                        (row, prd) => { row.Product = prd; row.UMsCache = umsCache; row.ProductTypes = productTypes; return row; },
                        new { cid = company_id, id = id }, splitOn: "SocietaID").ToList());
            }
            return result;

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
                "SELECT COUNT(*) FROM acq_orders_heads WHERE company_id = @company_id AND id = @id",
                new { company_id = company_id, id = id }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }


    #region Printing
    public acq_orders_heads? GetPrintFull(string CompanyID, long ID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Query<acq_orders_heads>(
                @"SELECT i.*, c.*, s.*, co.*,iso.isolin,sl.spedes,cl.condes FROM acq_orders_heads AS i
                        LEFT JOIN ABE AS c ON c.abecod = i.supplier_id
                        LEFT OUTER JOIN SPEDIZIONE AS s ON s.specod = i.shipping_id
                        LEFT OUTER JOIN CONSEGNA AS co ON co.concod = i.delivery_id
                        LEFT OUTER JOIN ISO AS iso ON iso.isocod = c.isocod
                        LEFT OUTER JOIN SPEDIZIONE_LINGUA AS sl ON sl.specod = i.shipping_id AND sl.lincod = iso.isolin
                        LEFT OUTER JOIN CONSEGNA_LINGUA AS cl ON cl.concod = i.delivery_id AND cl.lincod = iso.isolin
                        WHERE i.company_id = @cid AND i.id = @id",
                new[] { typeof(acq_orders_heads), typeof(ABE), typeof(SPEDIZIONE), typeof(CONSEGNA), typeof(string), typeof(string), typeof(string) }, (objects) =>
                {
                    var acq = (acq_orders_heads)objects[0];

                    acq.Supplier = objects[1] as ABE;
                    acq.Shipment = objects[2] as SPEDIZIONE;
                    if (acq.Shipment != null)
                        acq.Shipment.spedes = (!string.IsNullOrEmpty(objects[5]?.ToString()) ? objects[5]!.ToString() : acq.Shipment!.spedes) ?? string.Empty;
                    acq.Delivery = objects[3] as CONSEGNA;
                    if (acq.Delivery != null)
                        acq.Delivery.condes = (!string.IsNullOrEmpty(objects[6]?.ToString()) ? objects[6]!.ToString() : acq.Delivery!.condes) ?? string.Empty;
                    acq.Language = objects[4] as string;


                    return acq;
                }, new { cid = CompanyID, id = ID }, splitOn: "abecod,specod,concod,isolin,spedes,condes").FirstOrDefault();

            var umsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            var productTypes = VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_tipoRepository>().GetList(CompanyID, false);

            if (result != null)
            {
                result.Rows = new ObservableCollection<acq_orders_rows>(connection.Query<acq_orders_rows, tab_articolo, tab_articolo_lingua, acq_orders_rows>(
                        @"SELECT r.*, p.*, l.* FROM acq_orders_rows AS r
                            INNER JOIN tab_articolo AS p ON p.SocietaID = r.company_id AND p.ID = r.product_id
                            LEFT OUTER JOIN tab_articolo_lingua AS l ON p.SocietaID = l.SocietaID AND p.ID = l.ArticoloID AND l.lincod = @Lingua
                            WHERE r.company_id = @cid AND r.id = @id",
                        (row, prd, lingua) =>
                        {
                            row.UMsCache = umsCache;
                            row.Product = prd;
                            row.Product.Descrizione = (lingua != null && !string.IsNullOrEmpty(lingua.Descrizione) ? lingua.Descrizione : prd.Descrizione);
                            row.Product.Note = (lingua != null && !string.IsNullOrEmpty(lingua.Note) ? lingua.Note : prd.Note);
                            row.ProductTypes = productTypes;
                            return row;
                        },
                        new { cid = CompanyID, id = ID, Lingua = result.Language }, splitOn: "SocietaID,SocietaID").ToList());

                result.Attachments = VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_heads_attachmentsRepository>().GetList(CompanyID, ID);

                // UMs recap and supplier codes
                result.UMsRecap = new ObservableCollection<GenericStringDecimal>();
                foreach (var row in result.Rows.OrderBy(o => o.Product?.UnitaID))
                {
                    #region Supplier code
                    var lisSup = VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>().GetCurrent(CompanyID, row.product_id ?? string.Empty, result.supplier_id, result.order_date ?? DateTime.Now, row.unit_id);
                    if (lisSup != null)
                    {
                        if (!result.use_supplier_codes)
                        {
                            if (!string.IsNullOrWhiteSpace(lisSup.SupplierProductID) || !string.IsNullOrWhiteSpace(lisSup.SupplierProductDescription))
                                row.SupplierCode = $"Codifica fornitore: {lisSup.SupplierProductID} {lisSup.SupplierProductDescription}";
                            else
                                row.SupplierCode = null;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(lisSup.SupplierProductID))
                                row.product_id = lisSup.SupplierProductID;
                            if (!string.IsNullOrWhiteSpace(lisSup.SupplierProductDescription) && row.Product != null)
                                row.Product.Descrizione = lisSup.SupplierProductDescription;
                        }
                    }
                    else
                    {
                        row.SupplierCode = null;
                    }
                    #endregion
                    #region UMs recap
                    var existingUM = result.UMsRecap.Where(w => w.Description == row.Product?.UnitaID).FirstOrDefault();
                    if (existingUM != null)
                    {
                        existingUM.Value += (row.quantity ?? 0);
                    }
                    else
                    {
                        result.UMsRecap.Add(new GenericStringDecimal()
                        {
                            Description = row.Product?.UnitaID,
                            Value = (row.quantity ?? 0)
                        });
                    }
                    #endregion
                }
                // Rates recap
                int i = 1;
                List<string> rates = new List<string>();
                var ratesList = new List<Tuple<string, string, string, string>>();
                var ratesList2 = new List<Tuple<string, string, string, string>>();
                foreach (var row in result.Rows.OrderBy(o => o.tax_code).ThenBy(o => o.tax_rate))
                {
                    if (!rates.Contains(row.tax_code + row.tax_rate.Trim()))
                    {
                        var rate = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(row.tax_code, row.tax_rate);
                        rates.Add(row.tax_code + row.tax_rate.Trim());
                        var imponibile = result.Rows
                        .Where(w => w.tax_code == row.tax_code && w.tax_rate == row.tax_rate)
                        .Sum(sum => sum.NetAmount);
                        int rateValue = 0;
                        int.TryParse(row.tax_rate, out rateValue);
                        var imposta = Math.Round((imponibile * rateValue / 100), 2);

                        if (rate != null)
                        {
                            if (i % 2 == 0)
                                ratesList2.Add(new Tuple<string, string, string, string>(row.tax_code + " " + row.tax_rate, rate.assdes, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                            else
                                ratesList.Add(new Tuple<string, string, string, string>(row.tax_code + " " + row.tax_rate, rate.assdes, $"{imponibile.ToString("N2")} €", $"{imposta.ToString("N2")} €"));
                        }

                        i++;
                    }
                }
                result.RatesRecap = ratesList;
                result.RatesRecap2 = ratesList2;
            }

            return result;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PurchaseOrderReport? PrintPurchaseOrder(acq_orders_heads PurchaseOrder)
    {
        try
        {
            PurchaseOrder.Supplier = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(PurchaseOrder.supplier_id);

            var aziendaRepository = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>();

            var azienda = aziendaRepository.Get(PurchaseOrder.company_id);

            var socbase = VulpesServiceProvider.Provider.GetRequiredService<ICompanyRepository>().Get(PurchaseOrder.company_id);

            var bank = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().Get(PurchaseOrder.bank_abi ?? 0, PurchaseOrder.bank_cab ?? 0);

            var companyBank = !string.IsNullOrEmpty(PurchaseOrder.bank_account) ? VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Get(PurchaseOrder.company_id, PurchaseOrder.bank_abi ?? 0, PurchaseOrder.bank_cab ?? 0, PurchaseOrder.bank_account) : null;

            var supplierData = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(PurchaseOrder.company_id, PurchaseOrder.supplier_id);

            var aziendaLingua = !string.IsNullOrEmpty(PurchaseOrder.Language) ? VulpesServiceProvider.Provider.GetRequiredService<IAZIENDA_LINGUARepository>().Get(PurchaseOrder.company_id, PurchaseOrder.Language) : null;

            // get customizations
            object? dictionary = null;

            if (!string.IsNullOrEmpty(PurchaseOrder.Language))
            {
                var languageDictionary = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetDictionary(PurchaseOrder.Language);

                if (languageDictionary != null)
                    dictionary = new LocalizationHelper().CreateClassFromDictionary(languageDictionary);
            }

            return new PurchaseOrderReport()
            {
                PurchaseOrder = PurchaseOrder,
                Payment = !string.IsNullOrEmpty(PurchaseOrder.payment_id) ? VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(PurchaseOrder.payment_id) : null,
                CompanyInfo = aziendaRepository.Get(PurchaseOrder.company_id),
                BankData = $"{bank?.FullDescriptionSearchable} c/c nr.{PurchaseOrder.bank_account} IBAN: {(companyBank != null ? companyBank.abibiba : supplierData?.FOIBAN)}",
                LogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase!.SOCUID}/{StorageHelper.CUSTOM_FOLDER}logo.png"),
                CertificationsLogoData = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{socbase.SOCUID}/{StorageHelper.CUSTOM_FOLDER}certs.png"),
                HeaderFootNote = PurchaseOrder.note,
                FixedText = (aziendaLingua != null) ? (!string.IsNullOrEmpty(aziendaLingua.azacqgtex) ? aziendaLingua.azacqgtex : azienda!.azacqgtex) : azienda!.azacqgtex,
                LinguaDictionary = dictionary,
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
    public string INSERT_QUERY => "INSERT INTO acq_orders_heads (company_id,id,order_date,supplier_id,note,sent,payment_id,delivery_id,shipping_id,send_user,closed,added,addedUserID,updated,updatedUserID,canceled,canceledUserID,canceledNote,commercial_signed,commercial_signer,management_signed,management_signer,bank_abi,bank_cab,bank_account,use_supplier_codes) OUTPUT INSERTED.rv VALUES(@company_id,@id,@order_date,@supplier_id,@note,@sent,@payment_id,@delivery_id,@shipping_id,@send_user,@closed,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@updated,@updatedUserID,@canceled,@canceledUserID,@canceledNote,@commercial_signed,@commercial_signer,@management_signed,@management_signer,@bank_abi,@bank_cab,@bank_account,@use_supplier_codes)";
    public string UPDATE_QUERY => "UPDATE acq_orders_heads SET company_id = @company_id,id = @id,order_date = @order_date,supplier_id = @supplier_id,note = @note,sent = @sent,payment_id = @payment_id,delivery_id = @delivery_id,shipping_id = @shipping_id,send_user = @send_user,closed = @closed,added = @added,addedUserID = @addedUserID,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID,canceled = @canceled,canceledUserID = @canceledUserID,canceledNote = @canceledNote,commercial_signed = @commercial_signed,commercial_signer = @commercial_signer,management_signed = @management_signed,management_signer = @management_signer,bank_abi = @bank_abi,bank_cab = @bank_cab,bank_account = @bank_account,use_supplier_codes = @use_supplier_codes OUTPUT INSERTED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM acq_orders_heads OUTPUT DELETED.rv WHERE company_id = @company_id AND id = @id AND rv = @rv";
    public bool Insert(acq_orders_heads Model)
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

    public bool InsertFull(acq_orders_heads Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                connection.Execute(INSERT_QUERY, Model, transaction);
                foreach (var row in Model.Rows ?? new ObservableCollection<acq_orders_rows>())
                {
                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rowsRepository>().INSERT_QUERY, row, transaction);
                    // RDAs
                    foreach (var rda in row.RDAs ?? new ObservableCollection<acq_orders_rows_rdas>())
                    {
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_rdasRepository>().INSERT_QUERY, rda, transaction);
                    }
                    // Jobs
                    foreach (var job in row.Jobs ?? new ObservableCollection<acq_orders_rows_jobs>())
                    {
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_jobsRepository>().INSERT_QUERY, job, transaction);
                    }
                    // Customer orders
                    foreach (var co in row.CustomerOrders ?? new ObservableCollection<acq_orders_rows_customer_orders>())
                    {
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<Iacq_orders_rows_customer_ordersRepository>().INSERT_QUERY, co, transaction);
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(acq_orders_heads Model)
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

    public bool Delete(acq_orders_heads Model)
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

    public string? Validate(acq_orders_heads Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.company_id))
            {
                if (Model.order_date.HasValue)
                {
                    if (Model.supplier_id > 0)
                    {
                        if (Model.bank_abi.HasValue && Model.bank_cab.HasValue && !string.IsNullOrWhiteSpace(Model.bank_account))
                        {
                            return null;
                        }
                        else
                        { return "La banca è obbligatoria"; }
                    }
                    else
                    { return "Il fornitore è obbligatorio"; }
                }
                else
                { return "La data dell'ordine è obbligatoria"; }
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