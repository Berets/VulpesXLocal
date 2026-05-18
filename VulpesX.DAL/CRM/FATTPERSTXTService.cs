using System.Globalization;
using System.Text.RegularExpressions;
using VulpesX.DAL;

namespace VulpesX.DAL.CRM;

public interface IFATTPERSTXTRepository
{

    ObservableCollection<FATTPERSTXT>? GetList(string CompanyID);

    bool GenerateFILCONAD(string CompanyID, int CustomerID, DateTime From, DateTime To, string Path);

    Tuple<string, string> GetIDAndDateFromRiferimento(string Riferimento);
}

public class FATTPERSTXTRepository : RepositoryBase, IFATTPERSTXTRepository
{
    public FATTPERSTXTRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<FATTPERSTXT>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FATTPERSTXT>(
                @"SELECT * FROM FATTPERSTXT
                        WHERE txtsoci = @cid 
                        ORDER BY txtid",
                new { cid = CompanyID });
            return new ObservableCollection<FATTPERSTXT>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool GenerateFILCONAD(string CompanyID, int CustomerID, DateTime From, DateTime To, string Path)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<FATTD00F, FATTT00F, BOLLD00F, BOLLT00F, tab_articolo, CAUFAT00F, FATTD00F>(
           @"SELECT fd.*, ft.*, bd.*, bt.*, ar.*,ca.* FROM FATTD00F AS fd
                        INNER JOIN FATTT00F AS ft ON fd.ftsoci = ft.ftsoci and fd.ftanno = ft.ftanno and fd.ftnuor = ft.ftnuor
                        LEFT OUTER JOIN BOLLD00F AS bd ON fd.ftsoci = bd.bolsoc and fd.fdbono = bd.btanno and fd.fdboll = bd.btboll and fd.fdbori = bd.borigb
                        LEFT OUTER JOIN BOLLT00F AS bt ON bd.bolsoc = bt.bolsoc and bd.btanno = bt.btanno and bd.btboll = bt.btboll
                        INNER JOIN tab_articolo AS ar ON ar.SocietaID=fd.ftsoci AND ar.ID=fd.FDCODA
                        INNER JOIN CAUFAT00F as ca ON ft.FTCAUS = ca.fatcod
                        WHERE fd.ftsoci = @cid AND ft.FTCODC = @aid and ft.FTDAOR >= @from and ft.FTDAOR <= @to
                        ORDER BY fd.FDRIGA",
           (fd, ft, bd, bt, ar, ca) =>
           {
               fd.LinkedInvoiceHead = ft;
               fd.LinkedInvoiceHead.Causal = ca;
               fd.LinkedDDT = bt;
               fd.Product = ar;
               return fd;
           },
           new { cid = CompanyID, aid = CustomerID, from = From, to = To }, splitOn: "ftsoci,bolsoc,bolsoc,SocietaID,fatcod");

            var lines = new List<string>();
            var progressive = 0;
            foreach (var inv in list.Where(o => o.LinkedInvoiceHead?.FTTIPO == "F").GroupBy(g => new { g.LinkedDDT?.bolsoc, g.LinkedDDT?.BTANNO, g.LinkedDDT?.BTNUBD }))
            {
                var first = inv.First();

                var external = connection.Query<ABE_EXTERN_DESTS>(@"SELECT * from ABE_EXTERN_DESTS as ex WHERE ex.abecod = @aid AND abeextcode = 'FILCONAD' AND abedestid = @did", new { aid = first.LinkedInvoiceHead!.FTCODC, did = first.LinkedDDT!.BTCODD }).FirstOrDefault();

                ++progressive;
                string invoiceID = !string.IsNullOrEmpty(first.LinkedInvoiceHead!.Causal?.fatpre) ? $"{first.LinkedInvoiceHead!.Causal?.fatpre}{first.LinkedInvoiceHead!.FTNUFD.ToString().PadLeft(6 - first.LinkedInvoiceHead!.Causal?.fatpre.Length ?? 0, '0')}" : first.LinkedInvoiceHead.FTNUFD.ToString().PadLeft(6, '0');
                string invoiceDate = first.LinkedInvoiceHead!.FTDAOR!.Value.ToString("yyMMdd");
                string ddtID = inv.Key.BTNUBD.HasValue ? inv.Key.BTNUBD.ToString()!.PadLeft(6, '0') : "0";
                string ddtDate = first.LinkedDDT!.BTDATA!.Value.ToString("yyMMdd");
                string supplierID = "               "; //15
                string supplierType = " "; //1
                string customerID = "               "; //15
                string customerCoopID = "               "; //15
                string meetID = (external != null) ? external.abeextdid.PadLeft(15, ' ') : "               "; //15
                string meetType = " "; //1
                string invoiceType = "F";
                string invoiceValue = "EUR";
                lines.Add($"01{progressive.ToString("D5")}{invoiceID}{invoiceDate}{ddtID}{ddtDate}{supplierID}{supplierType}{customerID}{customerCoopID}{meetID}{meetType}{invoiceType}{invoiceValue}");

                foreach (var det in inv)
                {
                    if (det != null && det.Product != null)
                    {
                        //recupera articolo da listino cliente
                        var customer = connection.Query<CRM_LISCLI>(@"SELECT * from CRM_LISCLI as ex WHERE ex.customerID = @aid AND ex.productID = @pid AND ex.companyID = @cid AND ex.fromDate >= @idate AND ex.toDate <= @idate", new { aid = first.LinkedInvoiceHead.FTCODC, pid = det.Product.ID, cid = first.LinkedInvoiceHead.ftsoci, idate = first.LinkedInvoiceHead.FTDAOR }).FirstOrDefault();

                        if (customer == null)
                        {
                            customer = connection.Query<CRM_LISCLI>(@"SELECT * from CRM_LISCLI as ex WHERE ex.customerID = @aid AND ex.productID = @pid AND ex.companyID = @cid order by ex.toDate desc", new { aid = first.LinkedInvoiceHead.FTCODC, pid = det.Product.ID, cid = first.LinkedInvoiceHead.ftsoci }).FirstOrDefault();
                        }

                        string productID = (det.Product.ID.Length > 15) ? det.Product.ID.Substring(0, 15) : det.Product.ID.PadLeft(15, ' ');
                        string productDescription = (det.Product.Descrizione.Length > 30) ? det.Product.Descrizione.Substring(0, 30) : det.Product.Descrizione.PadRight(30, ' ');
                        if (customer != null && !string.IsNullOrEmpty(customer.customerProductID))
                            productID = (customer.customerProductID.Length > 15) ? customer.customerProductID.Substring(0, 15) : customer.customerProductID.PadLeft(15, ' ');
                        if (customer != null && !string.IsNullOrEmpty(customer.customerProductDescription))
                            productDescription = (customer.customerProductDescription.Length > 30) ? customer.customerProductDescription.Substring(0, 30) : customer.customerProductDescription.PadRight(30, ' ');

                        string productUM = (det.Product.UnitaID != null) ? (det.Product.UnitaID.Length > 2) ? det.Product.UnitaID.Substring(0, 2) : det.Product.UnitaID.PadRight(2, ' ') : string.Empty;
                        string quantity = string.Join("", String.Format("{0:0.00}", det.FDQTAV).Where(char.IsDigit)).PadLeft(7, '0');
                        string price = string.Join("", String.Format("{0:0.000}", det.NetPriceUnit).Where(char.IsDigit)).PadLeft(9, '0');
                        string total = string.Join("", String.Format("{0:0.000}", det.NetPrice).Where(char.IsDigit)).PadLeft(9, '0');
                        string pieces = "0000";
                        int vatValueOut = 0;
                        string vatType = (int.TryParse(det.FDALIV, out vatValueOut)) ? " " : "1";
                        string vatValue = det.FDALIV != null ? (det.FDALIV == "NS") ? "01" : (det.FDALIV.Length > 2) ? det.FDALIV.Substring(0, 2) : det.FDALIV : string.Empty;
                        string movementType = " ";
                        string cessionType = (det.FDTQTA == "O") ? "6" : "1";
                        string conadID = "      "; //6
                        string listID = "  "; //2
                        string productType = " ";
                        string contractType = " ";
                        string treatmentType = " ";
                        string transportPrice = "     "; //5
                        string accountID = " ";
                        string wasteType = (det.FDTQTA == "T") ? "1" : "";
                        string listPrice = "       "; // 7
                        string filler = "   "; //3
                        string orderDate = "      "; //6
                        string reserved = "      "; //6

                        lines.Add($"02{progressive.ToString("D5")}{productID}{productDescription}{productUM}{quantity}{price}{total}{pieces}{vatType}{vatValue}{movementType}{cessionType}{conadID}{listID}{productType}{contractType}{treatmentType}{transportPrice}{accountID}{wasteType}{listPrice}{filler}{orderDate}{reserved}");
                    }
                }
            }

            foreach (var inv in list.Where(o => o.LinkedInvoiceHead?.FTTIPO == "N").GroupBy(g => new { g.LinkedInvoiceHead?.ftsoci, g.LinkedInvoiceHead?.FTANNO, g.LinkedInvoiceHead?.FTNUFD }))
            {
                var first = inv.First();

                var external = connection.Query<ABE_EXTERN_DESTS>(@"SELECT * from ABE_EXTERN_DESTS as ex WHERE ex.abecod = @aid AND abeextcode = 'FILCONAD' AND abedestid = @did", new { aid = first.LinkedInvoiceHead!.FTCODC, did = first.LinkedInvoiceHead.FTCODD }).FirstOrDefault();
                var idDateFromRiferimento = GetIDAndDateFromRiferimento(first.LinkedInvoiceHead!.FTDE25);
                ++progressive;
                string invoiceID = !string.IsNullOrEmpty(first.LinkedInvoiceHead.Causal!.fatpre) ? $"{first.LinkedInvoiceHead.Causal.fatpre}{first.LinkedInvoiceHead.FTNUFD.ToString().PadLeft(6 - first.LinkedInvoiceHead.Causal.fatpre.Length, '0')}" : first.LinkedInvoiceHead.FTNUFD.ToString().PadLeft(6, '0');
                string invoiceDate = first.LinkedInvoiceHead.FTDAOR.HasValue ? first.LinkedInvoiceHead.FTDAOR.Value.ToString("yyMMdd") : string.Empty;
                string ddtID = idDateFromRiferimento.Item1; //6
                string ddtDate = idDateFromRiferimento.Item2; //6
                string supplierID = "               "; //15
                string supplierType = " "; //1
                string customerID = "               "; //15
                string customerCoopID = "               "; //15
                string meetID = (external != null) ? external.abeextdid.PadLeft(15, ' ') : "               "; //15
                string meetType = " "; //1
                string invoiceType = "N";
                string invoiceValue = "EUR";
                lines.Add($"01{progressive.ToString("D5")}{invoiceID}{invoiceDate}{ddtID}{ddtDate}{supplierID}{supplierType}{customerID}{customerCoopID}{meetID}{meetType}{invoiceType}{invoiceValue}");

                foreach (var det in inv)
                {
                    if (det != null && det.Product != null)
                    {
                        //recupera articolo da listino cliente
                        var customer = connection.Query<CRM_LISCLI>(@"SELECT * from CRM_LISCLI as ex WHERE ex.customerID = @aid AND ex.productID = @pid AND ex.companyID = @cid AND ex.fromDate >= @idate AND ex.toDate <= @idate", new { aid = first.LinkedInvoiceHead.FTCODC, pid = det.Product.ID, cid = first.LinkedInvoiceHead.ftsoci, idate = first.LinkedInvoiceHead.FTDAOR }).FirstOrDefault();

                        if (customer == null)
                        {
                            customer = connection.Query<CRM_LISCLI>(@"SELECT * from CRM_LISCLI as ex WHERE ex.customerID = @aid AND ex.productID = @pid AND ex.companyID = @cid order by ex.toDate desc", new { aid = first.LinkedInvoiceHead.FTCODC, pid = det.Product.ID, cid = first.LinkedInvoiceHead.ftsoci }).FirstOrDefault();
                        }

                        string productID = (det.Product.ID.Length > 15) ? det.Product.ID.Substring(0, 15) : det.Product.ID.PadLeft(15, ' ');
                        string productDescription = (det.Product.Descrizione.Length > 30) ? det.Product.Descrizione.Substring(0, 30) : det.Product.Descrizione.PadRight(30, ' ');
                        if (customer != null && !string.IsNullOrEmpty(customer.customerProductID))
                            productID = (customer.customerProductID.Length > 15) ? customer.customerProductID.Substring(0, 15) : customer.customerProductID.PadLeft(15, ' ');
                        if (customer != null && !string.IsNullOrEmpty(customer.customerProductDescription))
                            productDescription = (customer.customerProductDescription.Length > 30) ? customer.customerProductDescription.Substring(0, 30) : customer.customerProductDescription.PadRight(30, ' ');

                        string productUM = (det!.Product!.UnitaID?.Length > 2) ? det.Product.UnitaID.Substring(0, 2) : det.Product.UnitaID!.PadRight(2, ' ');
                        string quantity = string.Join("", String.Format("{0:0.00}", det.FDQTAV).Where(char.IsDigit)).PadLeft(7, '0');
                        string price = string.Join("", String.Format("{0:0.000}", det.FDPREZ).Where(char.IsDigit)).PadLeft(9, '0');
                        string total = string.Join("", String.Format("{0:0.000}", det.NetPrice).Where(char.IsDigit)).PadLeft(9, '0');
                        string pieces = "0000";
                        string vatType = " ";
                        string vatValue = (det.FDALIV != null) ? (det.FDALIV.Length > 2) ? det.FDALIV!.Substring(0, 2) : det.FDALIV : string.Empty;
                        string movementType = " ";
                        string cessionType = "1";
                        string conadID = "      "; //6
                        string listID = "  "; //2
                        string productType = " ";
                        string contractType = " ";
                        string treatmentType = " ";
                        string transportPrice = "     "; //5
                        string accountID = " ";
                        string wasteType = "1";
                        string listPrice = "       "; // 7
                        string filler = "   "; //3
                        string orderDate = "      "; //6
                        string reserved = "      "; //6

                        lines.Add($"02{progressive.ToString("D5")}{productID}{productDescription}{productUM}{quantity}{price}{total}{pieces}{vatType}{vatValue}{movementType}{cessionType}{conadID}{listID}{productType}{contractType}{treatmentType}{transportPrice}{accountID}{wasteType}{listPrice}{filler}{orderDate}{reserved}");
                    }
                }
            }

            File.WriteAllLines(Path, lines.ToArray());

            return true;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public Tuple<string, string> GetIDAndDateFromRiferimento(string? Riferimento)
    {
        try
        {
            if (!string.IsNullOrEmpty(Riferimento))
            {
                string pattern = @"N<([^>]+)>|DEL<([^>]+)>";

                MatchCollection matches = Regex.Matches(Riferimento, pattern);

                string idDdt = "";
                DateTime dateDdt = DateTime.MinValue;

                foreach (Match match in matches)
                {
                    if (match.Groups[1].Success) // Group 1 corresponds to N<value>
                    {
                        idDdt = match.Groups[1].Value;
                    }

                    if (match.Groups[2].Success) // Group 2 corresponds to DEL<value>
                    {
                        var dateDdtString = match.Groups[2].Value;
                        DateTime.TryParseExact(dateDdtString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateDdt);
                    }
                }

                string retValue1 = "      "; // 6
                if (!string.IsNullOrEmpty(idDdt))
                    retValue1 = idDdt.PadLeft(6, '0');

                string retValue2 = "      "; // 6
                if (dateDdt != DateTime.MinValue)
                    retValue2 = dateDdt.ToString("yyMMdd");

                return new Tuple<string, string>(retValue1, retValue2);
            }

            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        catch (Exception)
        {
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
    }
}

