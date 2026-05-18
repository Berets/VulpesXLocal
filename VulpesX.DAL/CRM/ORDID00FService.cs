using Microsoft.Extensions.DependencyInjection;
using VulpesX.Models.Models;

namespace VulpesX.DAL.CRM;

public interface IORDID00FRepository
{

    ObservableCollection<ORDID00F>? GetList(string CompanyID, int Year, int Number);

    #region Prices
    GenericPriceInfo? GetLastPriceDifferentCustomer(string CompanyID, string ProductID, int CustomerID, int CurrentOrderYear, int? CurrentOrderID);
    GenericPriceInfo? GetLastPriceSameCustomer(string CompanyID, string ProductID, int CustomerID, int CurrentOrderYear, int? CurrentOrderID);
    #endregion

   ORDID00F? Get(string otsoci, int OTANNO, int OTNUOR, int ODRIGA);

   bool Exists(string otsoci, int OTANNO, int OTNUOR, int ODRIGA);

    #region CRUD
    string INSERT_QUERY {get;}
    string UPDATE_QUERY {get;}
    string DELETE_QUERY { get; }
    bool Insert(ORDID00F Model);

    bool Update(ORDID00F Model);

    bool UpdateAll(ORDIT00F Head, ObservableCollection<ORDID00F> Rows);

    bool Delete(ORDID00F Model);

    string? Validate(ORDID00F Model, bool IsInsert);

    string? ValidateModel(ObservableCollection<ORDID00F>? Rows);
    #endregion
}

public class ORDID00FRepository : RepositoryBase, IORDID00FRepository
{
    public ORDID00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ORDID00F>? GetList(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ORDID00F>(
                @"SELECT *, (SELECT COUNT(*) FROM BOLLD00F WHERE bolsoc = otsoci AND BOANNO = OTANNO AND BONUOR = OTNUOR AND BORIGA = ODRIGA) AS DDTCount FROM ORDID00F
                        WHERE otsoci = @cid AND OTANNO = @yea AND OTNUOR = @num
                        ORDER BY ODRIGA", new { cid = CompanyID, yea = Year, num = Number });

            return new ObservableCollection<ORDID00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region Prices
    public GenericPriceInfo? GetLastPriceDifferentCustomer(string CompanyID, string ProductID, int CustomerID, int CurrentOrderYear, int? CurrentOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();

                return connection.Query<GenericPriceInfo>(
                    $@"SELECT TOP(1) d.ODPREZ AS Price, d.ODSCO1 AS Discount1, d.ODSCO2 AS Discount2, d.ODSCO3 AS Discount3, d.ODTSC1 AS DiscountType1, d.ODTSC2 AS DiscountType2, d.ODTSC3 AS DiscountType3, d.ODMAGG AS Surcharge, d.ODTMAG AS SurchargeType, d.ODNOTE AS Note, CONCAT(TRIM(CONVERT(nvarchar(15), a.abecod)), ' ', a.abers1 , ' ' ,  a.abers2) AS CustomerDescription FROM ORDID00F AS d
                        INNER JOIN ORDIT00F AS m ON m.otsoci = d.otsoci AND m.OTANNO = d.OTANNO AND m.OTNUOR = d.OTNUOR
                        INNER JOIN ABE AS a ON a.abecod = m.OTCLIE
                        WHERE m.canceled IS NULL AND d.ODCODA = @pid AND d.otsoci = @cid AND m.OTCLIE <> @cusid {(CurrentOrderID.HasValue && CurrentOrderID.Value > 0 ? " AND CONCAT(d.OTANNO , d.OTNUOR) <> @curr" : null)}
                        ORDER BY m.OTDAOR DESC",
                    new { cid = CompanyID, pid = ProductID, cusid = CustomerID, curr = CurrentOrderYear.ToString() + (CurrentOrderID ?? 0).ToString() })
                    .FirstOrDefault();
           
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    public GenericPriceInfo? GetLastPriceSameCustomer(string CompanyID, string ProductID, int CustomerID, int CurrentOrderYear, int? CurrentOrderID)
    {
        try
        {
            using var connection = GetOpenConnection();

           
                return connection.Query<GenericPriceInfo>(
                    $@"SELECT TOP(1) d.ODPREZ AS Price, d.ODSCO1 AS Discount1, d.ODSCO2 AS Discount2, d.ODSCO3 AS Discount3, d.ODTSC1 AS DiscountType1, d.ODTSC2 AS DiscountType2, d.ODTSC3 AS DiscountType3, d.ODMAGG AS Surcharge, d.ODTMAG AS SurchargeType, d.ODNOTE AS Note, CONCAT(TRIM(CONVERT(nvarchar(15), a.abecod)), ' ', a.abers1 , ' ' ,  a.abers2) AS CustomerDescription FROM ORDID00F AS d
                        INNER JOIN ORDIT00F AS m ON m.otsoci = d.otsoci AND m.OTANNO = d.OTANNO AND m.OTNUOR = d.OTNUOR
                        INNER JOIN ABE AS a ON a.abecod = m.OTCLIE
                        WHERE m.canceled IS NULL AND d.ODCODA = @pid AND d.otsoci = @cid AND m.OTCLIE = @cusID {(CurrentOrderID.HasValue && CurrentOrderID.Value > 0 ? " AND CONCAT(d.OTANNO , d.OTNUOR) <> @curr" : null)}
                        ORDER BY m.OTDAOR DESC",
                    new { cid = CompanyID, pid = ProductID, cusID = CustomerID, curr = CurrentOrderYear.ToString() + (CurrentOrderID ?? 0).ToString() })
                    .FirstOrDefault();
           
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }
    #endregion

    public ORDID00F? Get(string otsoci, int OTANNO, int OTNUOR, int ODRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ORDID00F>(
                "SELECT * FROM ORDID00F WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND ODRIGA = @ODRIGA",
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR, ODRIGA = ODRIGA })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string otsoci, int OTANNO, int OTNUOR, int ODRIGA)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ORDID00F WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND ODRIGA = @ODRIGA",
                new { otsoci = otsoci, OTANNO = OTANNO, OTNUOR = OTNUOR, ODRIGA = ODRIGA }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ORDID00F (otsoci,OTANNO,OTNUOR,ODRIGA,ODCODA,ODQTAV,ODTQTA,ODPREZ,ODSCO1,ODSCO2,ODMAGG,ODTPRE,ODTSC1,ODTSC2,ODTMAG,ODALIV,ODASSF,ODDACO,ODSERI,ODRIFC,ODGRUP,ODCONT,ODSCTO,ODSCO3,ODTSC3,odunit,ODOFTAN,ODOFTNUM,ODOFDRIG,ODNOTE,ODSHOW,ODSTA,ODCOA1,ODCOA2,ODCOA1P,ODCOA2P,ODCOA1PT,ODCOA2PT,ODDARI,ODQTAEV,ODSTATO,ODDAPA) OUTPUT INSERTED.rv VALUES(@otsoci,@OTANNO,@OTNUOR,@ODRIGA,@ODCODA,@ODQTAV,@ODTQTA,@ODPREZ,@ODSCO1,@ODSCO2,@ODMAGG,@ODTPRE,@ODTSC1,@ODTSC2,@ODTMAG,@ODALIV,@ODASSF,@ODDACO,@ODSERI,@ODRIFC,@ODGRUP,@ODCONT,@ODSCTO,@ODSCO3,@ODTSC3,@odunit,@ODOFTAN,@ODOFTNUM,@ODOFDRIG,@ODNOTE,@ODSHOW,@ODSTA,@ODCOA1,@ODCOA2,@ODCOA1P,@ODCOA2P,@ODCOA1PT,@ODCOA2PT,@ODDARI,@ODQTAEV,@ODSTATO,@ODDAPA)";
    public string UPDATE_QUERY => "UPDATE ORDID00F SET otsoci = @otsoci,OTANNO = @OTANNO,OTNUOR = @OTNUOR,ODRIGA = @ODRIGA,ODCODA = @ODCODA,ODQTAV = @ODQTAV,ODTQTA = @ODTQTA,ODPREZ = @ODPREZ,ODSCO1 = @ODSCO1,ODSCO2 = @ODSCO2,ODMAGG = @ODMAGG,ODTPRE = @ODTPRE,ODTSC1 = @ODTSC1,ODTSC2 = @ODTSC2,ODTMAG = @ODTMAG,ODALIV = @ODALIV,ODASSF = @ODASSF,ODDACO = @ODDACO,ODSERI = @ODSERI,ODRIFC = @ODRIFC,ODGRUP = @ODGRUP,ODCONT = @ODCONT,ODSCTO = @ODSCTO,ODSCO3 = @ODSCO3,ODTSC3 = @ODTSC3,odunit = @odunit,ODOFTAN = @ODOFTAN,ODOFTNUM = @ODOFTNUM,ODOFDRIG = @ODOFDRIG,ODNOTE = @ODNOTE,ODSHOW = @ODSHOW,ODSTA = @ODSTA,ODCOA1 = @ODCOA1,ODCOA2 = @ODCOA2,ODCOA1P = @ODCOA1P,ODCOA2P = @ODCOA2P,ODCOA1PT = @ODCOA1PT,ODCOA2PT = @ODCOA2PT,ODDARI = @ODDARI,ODQTAEV = @ODQTAEV,ODSTATO = @ODSTATO,ODDAPA = @ODDAPA OUTPUT INSERTED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND ODRIGA = @ODRIGA AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ORDID00F OUTPUT DELETED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND ODRIGA = @ODRIGA AND rv = @rv";
    public bool Insert(ORDID00F Model)
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

    public bool Update(ORDID00F Model)
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

    public bool UpdateAll(ORDIT00F Head, ObservableCollection<ORDID00F> Rows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var orditRepository = VulpesServiceProvider.Provider.GetRequiredService<IORDIT00FRepository>();

                connection.Execute("DELETE FROM ORDID00F WHERE otsoci = @otsoci AND OTANNO = @otanno AND OTNUOR = @otnuor",
                    new { otsoci = Head.otsoci, otanno = Head.OTANNO, otnuor = Head.OTNUOR },
                    transaction);

                foreach (var row in Rows)
                {
                    connection.Execute(INSERT_QUERY, row, transaction);
                }

                // update head
                connection.Execute(orditRepository.UPDATE_QUERY, Head,transaction);
                transaction.Commit();

                // check fulfillment
                orditRepository.FlagFulfillment(Head.otsoci, Head.OTANNO, Head.OTNUOR, Head.updatedUserID ?? string.Empty);

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

    public bool Delete(ORDID00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM ORDID00F OUTPUT DELETED.rv WHERE otsoci = @otsoci AND OTANNO = @OTANNO AND OTNUOR = @OTNUOR AND ODRIGA = @ODRIGA AND rv = @rv",
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

    public string? Validate(ORDID00F Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.otsoci) && IsInsert) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.SelectedProduct?.ID))
                {
                    if (Model.ODTQTA != "O" || (Model.ODTQTA == "O" && Model.SelectedRate?.assomaBool == true))
                    {
                        if (Model.ODQTAV > 0)
                        {
                            if (!Model.SelectedProduct.QuantitaDefault.HasValue ||
                                (Model.SelectedProduct.QuantitaDefault.HasValue && Model.odunit == Model.SelectedProduct.UnitaIDAlt && (Model.ODQTAV % 1 == 0 ? (Model.ODQTAV * (Model.SelectedProduct.QuantitaDefault ?? 1)) % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0 : (Model.ODQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0))) ||
                                (Model.SelectedProduct.QuantitaDefault.HasValue && Model.odunit != Model.SelectedProduct.UnitaIDAlt && (Model.ODQTAV % (Model.SelectedProduct.QuantitaDefault ?? 1) == 0)))
                            {
                                if (!Model.ODQTAEV.HasValue || (Model.ODQTAEV.HasValue && Model.ODQTAEV.Value <= Model.QuantityValue))
                                {
                                    if (Model.ODPREZ > 0 || (Model.ODPREZ == 0 && Model.ODTQTA == "N"))
                                    {
                                        if (!string.IsNullOrWhiteSpace(Model.SelectedRate?.assali) && !string.IsNullOrWhiteSpace(Model.SelectedRate?.asscod))
                                        {
                                            if (!string.IsNullOrWhiteSpace(Model.SelectedGroup?.P1GRUP) &&
                                                !string.IsNullOrWhiteSpace(Model.SelectedAccount?.P2CONT) &&
                                                !string.IsNullOrWhiteSpace(Model.SelectedSubaccount?.P3SOTC))
                                            {
                                                if (((Model.ODSCO1.HasValue && !string.IsNullOrWhiteSpace(Model.ODTSC1)) ||
                                                    (!Model.ODSCO1.HasValue && string.IsNullOrWhiteSpace(Model.ODTSC1))) &&
                                                    ((Model.ODSCO2.HasValue && !string.IsNullOrWhiteSpace(Model.ODTSC2)) ||
                                                    (!Model.ODSCO2.HasValue && string.IsNullOrWhiteSpace(Model.ODTSC2))) &&
                                                    ((Model.ODSCO3.HasValue && !string.IsNullOrWhiteSpace(Model.ODTSC3)) ||
                                                    (!Model.ODSCO3.HasValue && string.IsNullOrWhiteSpace(Model.ODTSC3))))
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
                                { return "C'è già una quantità evasa, la nuova quantità non può essere inferiore a quella già evasa"; }
                            }
                            else
                            { return $"La quantità digitata ({Model.ODQTAV.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(Model.SelectedProduct.QuantitaDefault ?? 1).ToString("N6")})"; }
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

    public string? ValidateModel(ObservableCollection<ORDID00F>? Rows)
    {

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
                return null;
            }
            else
            { return validation; }
        }
        else
        {
            return "E' necessario che siano presenti delle righe per confermare l'ordine cliente";
        }

    }
    #endregion
}