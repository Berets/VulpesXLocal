using Dapper;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Design;
using System.Net;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Accounting;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.DAL.Accounting;

public interface IPNTESTATARepository
{
    ObservableCollection<PNTESTATA>? GetList(string CompanyID, int Year);

    PNTESTATA? Get(string CompanyID, int Year, int Number);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(PNTESTATA Head, ObservableCollection<PNRIGHE> Rows, CAUCONT SelectedCausal, ObservableCollection<PNIVA> IVARows);

    bool Update(PNTESTATA Head, ObservableCollection<PNRIGHE> Rows, CAUCONT SelectedCausal, ObservableCollection<PNIVA> IVARows);

    bool Delete(PNTESTATA Model);

    string? Import(List<ImportPNModel> PNs);

    Tuple<string, int, int>? Duplicate(string CompanyID, int Year, int Number, int AccountingYear, DateTime Date, CAUCONT SelectedCausal, bool IsRemove);

    string? Validate(PNTESTATA Model, bool IsInsert);
    #endregion
}

public class PNTESTATARepository : RepositoryBase, IPNTESTATARepository
{
    public PNTESTATARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNTESTATA>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNTESTATA>(
                "SELECT * FROM PNTESTATA WHERE N1SOCI = @cid AND N1ANNO = @yea ORDER BY N1REGI DESC, N1DARE DESC",
                new { cid = CompanyID, yea = Year });

            return new ObservableCollection<PNTESTATA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNTESTATA? Get(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNTESTATA, CAUCONT, ABE, PNTESTATA>(
                @"SELECT t.*, (SELECT SUM(N1IMEU) FROM PNRIGHE WHERE N1SEGN = 'D' AND N1SOCI = t.N1SOCI AND N1ANNO = t.N1ANNO AND N1REGI = t.N1REGI) as Amount, l.caucod, l.caudes, l.cauiva, a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2 FROM PNTESTATA AS t 
                        INNER JOIN CAUCONT AS l ON t.PNCAUS = l.CAUCOD
                        LEFT OUTER JOIN ABE AS a ON a.abecod = t.N1CLFO
                        WHERE t.N1SOCI = @cid AND t.N1ANNO = @yea AND t.N1REGI = @num",
                (tes, cau, bas) => { tes.AccountingCausal = cau; tes.BasicRegistry = bas; return tes; },
                new { cid = CompanyID, yea = Year, num = Number },
                splitOn: "caucod,abecod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PNTESTATA (N1SOCI,N1ANNO,N1REGI,pncaus,N1DARE,pnvcod,pnvdiv,N1FL01,N1docn,N1docd,N1rifn,N1rifd,N1FLCF,N1CLFO,N1VADO,N1DIDO,N1DADI,N1CADO,N1IMDO,n1mrii,N1TmpPN,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,N1AUAN,N1AUNU,N1AUGE) OUTPUT INSERTED.rv VALUES(@N1SOCI,@N1ANNO,@N1REGI,@pncaus,@N1DARE,@pnvcod,@pnvdiv,@N1FL01,@N1docn,@N1docd,@N1rifn,@N1rifd,@N1FLCF,@N1CLFO,@N1VADO,@N1DIDO,@N1DADI,@N1CADO,@N1IMDO,@n1mrii,@N1TmpPN,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@N1AUAN,@N1AUNU,@N1AUGE)";
    public string UPDATE_QUERY => "UPDATE PNTESTATA SET N1SOCI = @N1SOCI,N1ANNO = @N1ANNO,N1REGI = @N1REGI,pncaus = @pncaus,N1DARE = @N1DARE,pnvcod = @pnvcod,pnvdiv = @pnvdiv,N1FL01 = @N1FL01,N1docn = @N1docn,N1docd = @N1docd,N1rifn = @N1rifn,N1rifd = @N1rifd,N1FLCF = @N1FLCF,N1CLFO = @N1CLFO,N1VADO = @N1VADO,N1DIDO = @N1DIDO,N1DADI = @N1DADI,N1CADO = @N1CADO,N1IMDO = @N1IMDO,n1mrii = @n1mrii,N1TmpPN = @N1TmpPN,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,N1AUAN = @N1AUAN,N1AUNU = @N1AUNU,N1AUGE = @N1AUGE OUTPUT INSERTED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PNTESTATA OUTPUT DELETED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND rv = @rv";

    public bool Insert(PNTESTATA Head, ObservableCollection<PNRIGHE> Rows, CAUCONT SelectedCausal, ObservableCollection<PNIVA> IVARows)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // get ID
                    int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Head.N1SOCI, Head.N1ANNO, Constants.PN, true);
                    if (newID > 0)
                    {
                        Head.N1REGI = newID;
                        // add head
                        connection.Execute(INSERT_QUERY, Head, transaction);
                        // add rows
                        int sectionalRowID = 1;
                        foreach (var row in Rows)
                        {
                            row.N1REGI = newID;
                            row.N1DRri = Head.N1DARE;
                            row.n1mese = row.N1DADO?.Month;
                            row.N1DESC = row.N1DESC != null ? row.N1DESC : string.Empty;

                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, row, transaction);
                            // sectional
                            if (row.SelectedSubaccount?.P3CLFO == "C")
                            {
                                var exps = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().ComputeExpires(row.N1DADO ?? DateTime.MinValue, row.n1paga ?? string.Empty) ?? new List<DateTime>();

                                // Richiesta di Aldo : se cauiva = 'N' non splitta per scadenze
                                if (SelectedCausal.cauiva == "N" && row.n1scad.HasValue)
                                {
                                    exps.Clear();
                                    exps.Add(row.n1scad.Value);
                                }

                                var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty);
                                decimal splitAmount = Math.Round((row.N1IMEU ?? 0) / exps.Count, 2);
                                decimal splitRemains = (row.N1IMEU ?? 0) - (splitAmount * exps.Count);
                                var moveExpireID = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(row.N1SOCI, (row.n1clie ?? 0))?.scacod;

                                SCADENZE? mover = null;
                                if (!string.IsNullOrWhiteSpace(moveExpireID))
                                    mover = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().Get(moveExpireID);

                                foreach (var (value, i) in exps.Select((value, i) => (value, i)))
                                {
                                    var expItem = value;
                                    if (!string.IsNullOrWhiteSpace(moveExpireID))
                                    {
                                        expItem = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().ComputeExpireMove(mover, expItem);
                                    }
                                    else
                                    {
                                        if (pagcli != null)
                                            if (pagcli.pclgio.HasValue)
                                                expItem = expItem.AddDays(pagcli.pclgio.Value);
                                    }
                                    var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                    var newRow = new PNCLIENTI()
                                    {
                                        N2SOCI = Head.N1SOCI,
                                        N2ANNO = Head.N1ANNO,
                                        N2REGI = newID,
                                        N2RIGA = sectionalRowID++,
                                        N2DARI = row.N1DARI,
                                        N2RIFE = row.N1RIFE,
                                        N2DOCU = Head.N1docn,
                                        N2DARE = Head.N1DARE,
                                        N2DADO = Head.N1docd,
                                        N2CAUS = Head.pncaus,
                                        N2GRUP = row.pngrup,
                                        N2CONT = row.pncont,
                                        N2SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                        N2SOTT = row.n1clie,
                                        N2IMPO = row.N1IMPO ?? 0,
                                        N2DESC = row.N1DESC != null ? row.N1DESC : string.Empty,
                                        N2SEGN = row.N1SEGN,
                                        N2RATA = 0,
                                        N2SCAD = expItem,
                                        N2PAGA = row.n1paga,
                                        N2PRAT = string.Empty,
                                        N2DEST = string.Empty,
                                        N2PAXI = 0,
                                        N2CAMB = 0,
                                        N2VALU = row.N1IMVA ?? 0,
                                        N2DIVI = row.N1DIVI,
                                        n2vcod = "UIC",
                                        N2FLIN = string.Empty,
                                        N2IMEU = i + 1 == exps.Count ? splitAmount + splitRemains : splitAmount,
                                        N2TIDO = "E",
                                        n2rior = row.N1RIGA,
                                        N2FL01 = string.Empty,
                                        n2tipi = (VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty))?.pcltip,
                                        N2VADO = "UIC",
                                        N2DIDO = row.N1DIVI,
                                        n2comm = string.Empty,
                                        N2ANTI = 0,
                                        N2INIZ = SelectedCausal.cauceco == "S" ? row.N1DASM : null,
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, newRow, transaction);
                                }
                            }
                            else
                            {
                                if (row.SelectedSubaccount?.P3CLFO == "F")
                                {
                                    var exps = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().ComputeExpires(row.N1DADO ?? DateTime.MinValue, row.n1paga ?? string.Empty) ?? new List<DateTime>();

                                    // Richiesta di Aldo : se cauiva = 'N' non splitta per scadenze
                                    if (SelectedCausal.cauiva == "N" && row.n1scad.HasValue)
                                    {
                                        exps.Clear();
                                        exps.Add(row.n1scad.Value);
                                    }

                                    var pagfor = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty);
                                    decimal splitAmount = Math.Round((row.N1IMEU ?? 0) / exps.Count, 2);
                                    decimal splitRemains = (row.N1IMEU ?? 0) - (splitAmount * exps.Count);
                                    var moveExpireID = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(row.N1SOCI, row.n1clie ?? 0)?.scacod;
                                    SCADENZE? mover = null;
                                    if (!string.IsNullOrWhiteSpace(moveExpireID))
                                        mover = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().Get(moveExpireID);
                                    foreach (var (value, i) in exps.Select((value, i) => (value, i)))
                                    {
                                        var expItem = value;
                                        if (!string.IsNullOrWhiteSpace(moveExpireID))
                                            expItem = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().ComputeExpireMove(mover, expItem);
                                        var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                        var newRow = new PNFORNITORI()
                                        {
                                            N3SOCI = Head.N1SOCI,
                                            N3ANNO = Head.N1ANNO,
                                            N3REGI = newID,
                                            N3RIGA = sectionalRowID++,
                                            N3DARI = row.N1DARI,
                                            N3RIFE = row.N1RIFE,
                                            N3DOCU = Head.N1docn,
                                            N3DARE = Head.N1DARE,
                                            N3DADO = Head.N1docd,
                                            N3CAUS = Head.pncaus,
                                            N3GRUP = row.pngrup,
                                            N3CONT = row.pncont,
                                            N3SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                            N3SOTT = row.n1clie,
                                            N3IMPO = row.N1IMPO ?? 0,
                                            N3DESC = row.N1DESC,
                                            N3SEGN = row.N1SEGN,
                                            N3RATA = 0,
                                            N3SCAD = expItem,
                                            N3PAGA = row.n1paga,
                                            N3PRAT = string.Empty,
                                            N3DEST = string.Empty,
                                            N3PAXI = 0,
                                            N3CAMB = 0,
                                            N3VALU = row.N1IMVA ?? 0,
                                            N3DIVI = row.N1DIVI,
                                            n3vcod = "UIC",
                                            N3FLPA = string.Empty,
                                            N3ABIF = 0,
                                            N3CABF = 0,
                                            N3IMEU = i + 1 == exps.Count ? splitAmount + splitRemains : splitAmount,
                                            N3TIDO = "E",
                                            n3rior = row.N1RIGA,
                                            N3FL01 = string.Empty,
                                            n3tipp = (VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty))?.pfotip,
                                            N3VADO = "UIC",
                                            N3DIDO = "UIC",
                                            n3comm = string.Empty,
                                            N3CCNF = string.Empty,
                                            N3SOCA = row.N1SOCI,
                                            N3ABIA = 0,
                                            N3CABA = 0,
                                            N3CCNA = string.Empty
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, newRow, transaction);
                                    }
                                }
                            }
                            if (row.SelectedSubaccount?.P3CLFO == "C" || row.SelectedSubaccount?.P3CLFO == "F")
                            {
                                // add manual sectional rows
                                if (row.ExpireRows != null && row.ExpireRows.Count > 0)
                                {
                                    foreach (var exp in row.ExpireRows)
                                    {
                                        if (row.SelectedSubaccount.P3CLFO == "C")
                                        {
                                            var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                            var newRow = new PNCLIENTI()
                                            {
                                                N2SOCI = Head.N1SOCI,
                                                N2ANNO = Head.N1ANNO,
                                                N2REGI = newID,
                                                N2RIGA = sectionalRowID++,
                                                N2DARI = row.N1DARI,
                                                N2RIFE = row.N1RIFE,
                                                N2DOCU = Head.N1docn,
                                                N2DARE = Head.N1DARE,
                                                N2DADO = Head.N1docd,
                                                N2CAUS = Head.pncaus,
                                                N2GRUP = row.pngrup,
                                                N2CONT = row.pncont,
                                                N2SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                                N2SOTT = row.n1clie,
                                                N2IMPO = exp.Amount,
                                                N2DESC = exp.Note != null ? exp.Note : string.Empty,
                                                N2SEGN = exp.Sign,
                                                N2RATA = 0,
                                                N2SCAD = exp.ExpireDate,
                                                N2PAGA = row.n1paga,
                                                N2PRAT = string.Empty,
                                                N2DEST = string.Empty,
                                                N2PAXI = 0,
                                                N2CAMB = 0,
                                                N2VALU = exp.CurrencyAmount,
                                                N2DIVI = exp.CurrencyID,
                                                n2vcod = "UIC",
                                                N2FLIN = string.Empty,
                                                N2IMEU = exp.Amount,
                                                N2TIDO = "E",
                                                n2rior = exp.OriginalRowID,
                                                N2FL01 = string.Empty,
                                                n2tipi = (VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty))?.pcltip,
                                                N2VADO = "UIC",
                                                N2DIDO = exp.CurrencyID,
                                                n2comm = string.Empty,
                                                N2ANTI = 0,
                                                N2INIZ = SelectedCausal.cauceco == "S" ? row.N1DASM : null
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, newRow, transaction);
                                        }
                                        else
                                        {
                                            if (row.SelectedSubaccount.P3CLFO == "F")
                                            {
                                                var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                                var newRow = new PNFORNITORI()
                                                {
                                                    N3SOCI = Head.N1SOCI,
                                                    N3ANNO = Head.N1ANNO,
                                                    N3REGI = newID,
                                                    N3RIGA = sectionalRowID++,
                                                    N3DARI = row.N1DARI,
                                                    N3RIFE = row.N1RIFE,
                                                    N3DOCU = Head.N1docn,
                                                    N3DARE = Head.N1DARE,
                                                    N3DADO = Head.N1docd,
                                                    N3CAUS = Head.pncaus,
                                                    N3GRUP = row.pngrup,
                                                    N3CONT = row.pncont,
                                                    N3SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                                    N3SOTT = (row.n1clie ?? 0),
                                                    N3IMPO = exp.Amount,
                                                    N3DESC = exp.Note != null ? exp.Note : string.Empty,
                                                    N3SEGN = exp.Sign,
                                                    N3RATA = 0,
                                                    N3SCAD = exp.ExpireDate,
                                                    N3PAGA = row.n1paga,
                                                    N3PRAT = string.Empty,
                                                    N3DEST = string.Empty,
                                                    N3PAXI = 0,
                                                    N3CAMB = 0,
                                                    N3VALU = exp.CurrencyAmount,
                                                    N3DIVI = exp.CurrencyID,
                                                    n3vcod = "UIC",
                                                    N3FLPA = string.Empty,
                                                    N3ABIF = 0,
                                                    N3CABF = 0,
                                                    N3IMEU = exp.Amount,
                                                    N3TIDO = "E",
                                                    n3rior = exp.OriginalRowID,
                                                    N3FL01 = string.Empty,
                                                    n3tipp = (VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty))?.pfotip,
                                                    N3VADO = "UIC",
                                                    N3DIDO = "UIC",
                                                    n3comm = string.Empty,
                                                    N3CCNF = string.Empty,
                                                    N3SOCA = Head.N1SOCI,
                                                    N3ABIA = 0,
                                                    N3CABA = 0,
                                                    N3CCNA = string.Empty
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, newRow, transaction);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        // add IVA rows
                        bool hasIndetraibile = false;
                        decimal indetraibileTotal = 0;
                        foreach (var row in IVARows)
                        {
                            if (row.N4DTSCPG.HasValue)
                                row.N4DTSCAD = row.N4DTSCPG.Value.AddYears(1);
                            if (row.N4DTSCPG.HasValue && row.N4DTSCPG.Value > row.N4DADO)
                                row.N4FLIVCA = "S";
                            row.N4REGI = newID;
                            row.N4FLGS = null;

                            decimal indetraibileAmount = 0;
                            if (row.SelectedRate != null && row.SelectedRate.asspin.HasValue && row.SelectedRate.asspin.Value > 0)
                            {
                                indetraibileAmount = Math.Round(row.N4IIEU ?? 0, 2, MidpointRounding.AwayFromZero);
                                indetraibileTotal += indetraibileAmount;
                                hasIndetraibile = true;
                            }

                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, row, transaction);
                        }

                        #region Giroconto IVA indetraibile
                        if (hasIndetraibile)
                        {
                            // get registration number
                            var girocontoIndetraibileRegID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Head.N1SOCI, Head.N1ANNO, Constants.PN, true);
                            var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(SelectedCausal.cauliv ?? string.Empty);

                            var supplierGroups = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetListFull(Head.N1SOCI, Head.N1CLFO ?? 0, ivaBook?.livcii ?? string.Empty);

                            string? ivaSign = null;
                            if (SelectedCausal.causeg == "+")
                            {
                                ivaSign = "+";
                            }
                            else
                            {
                                ivaSign = "-";
                            }

                            #region PNTESTATA
                            PNTESTATA girocontoHead = new PNTESTATA()
                            {
                                N1SOCI = Head.N1SOCI,
                                N1ANNO = Head.N1ANNO,
                                N1REGI = girocontoIndetraibileRegID,
                                pncaus = ivaBook?.livcii,
                                N1DARE = Head.N1DARE,
                                N1docn = Head.N1docn,
                                N1docd = Head.N1docd,
                                N1rifn = Head.N1rifn,
                                N1rifd = Head.N1rifd,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1FL01 = string.Empty,
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = Head.addedUserID,
                            };
                            connection.Execute(INSERT_QUERY, girocontoHead, transaction);
                            #endregion

                            PNRIGHE firstRow = new PNRIGHE()
                            {
                                N1SOCI = girocontoHead.N1SOCI,
                                N1ANNO = girocontoHead.N1ANNO,
                                N1REGI = girocontoHead.N1REGI,
                                N1RIGA = 1,
                                N1DOCU = girocontoHead.N1docn,
                                N1DADO = girocontoHead.N1docd,
                                N1RIFE = girocontoHead.N1rifn,
                                N1DARI = girocontoHead.N1rifd,
                                N1SEGN = ivaSign == "+" ? "A" : "D",
                                pngrup = ivaBook?.livgci,
                                pncont = ivaBook?.livcci,
                                pnsott = ivaBook?.livsci,
                                N1IMEU = indetraibileTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DESC = Head.EntityFullDescription,
                                N1DRri = girocontoHead.N1docd,
                                N1tmpPNR = "N"
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, firstRow, transaction);

                            PNRIGHE secondRow = new PNRIGHE()
                            {
                                N1SOCI = girocontoHead.N1SOCI,
                                N1ANNO = girocontoHead.N1ANNO,
                                N1REGI = girocontoHead.N1REGI,
                                N1RIGA = 2,
                                N1DOCU = girocontoHead.N1docn,
                                N1DADO = girocontoHead.N1docd,
                                N1RIFE = girocontoHead.N1rifn,
                                N1DARI = girocontoHead.N1rifd,
                                N1SEGN = ivaSign == "+" ? "D" : "A",
                                pngrup = supplierGroups?.FirstOrDefault()?.cfgrup,
                                pncont = supplierGroups?.FirstOrDefault()?.cfcont,
                                pnsott = supplierGroups?.FirstOrDefault()?.cfsott,
                                N1IMEU = indetraibileTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DESC = Head.EntityFullDescription,
                                N1DRri = girocontoHead.N1docd,
                                N1tmpPNR = "N"
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, secondRow, transaction);

                            InfoHandler.Show($"Inserita correttamente la registrazione di giroconto n. {girocontoIndetraibileRegID}");
                        }
                        #endregion

                        transaction.Commit();

                        InfoHandler.Show($"Inserita correttamente la registrazione n. {newID}");
                        return true;
                    }
                    else
                    {
                        ErrorHandler.Show($"Impossibile recuperare un nuovo numero di registrazione");
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    ErrorHandler.Show($"{Constants.INSERT_VIOLATION}\n\n{exc.Message}");
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

    public bool Update(PNTESTATA Head, ObservableCollection<PNRIGHE> Rows, CAUCONT SelectedCausal, ObservableCollection<PNIVA> IVARows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // update head
                    connection.Execute(UPDATE_QUERY, Head, transaction);
                    // add rows
                    // delete current
                    connection.Execute(
                    "DELETE FROM PNRIGHE WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi",
                    new { n1soci = Head.N1SOCI, n1anno = Head.N1ANNO, n1regi = Head.N1REGI },
                    transaction);
                    int sectionalRowID = 1;
                    // delete current sectionals
                    connection.Execute(
                    "DELETE FROM PNCLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                    new { n2soci = Head.N1SOCI, n2anno = Head.N1ANNO, n2regi = Head.N1REGI },
                    transaction);
                    connection.Execute(
                    "DELETE FROM PNFORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi",
                    new { n3soci = Head.N1SOCI, n3anno = Head.N1ANNO, n3regi = Head.N1REGI },
                    transaction);
                    foreach (var row in Rows)
                    {
                        row.N1REGI = Head.N1REGI;
                        row.N1DRri = Head.N1DARE;
                        row.n1mese = row.N1DADO.HasValue ? row.N1DADO.Value.Month : Head.N1DARE!.Value.Month;
                        row.N1DESC = row.N1DESC != null ? row.N1DESC : string.Empty;
                        row.N1tmpPNR = Head.N1TmpPN;
                        // reset document date and number
                        row.N1DOCU = Head.N1docn;
                        row.N1DADO = Head.N1docd;

                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, row, transaction);
                        // add sectional rows
                        if (row.ExpireRows != null && row.ExpireRows.Count > 0)
                        {
                            foreach (var exp in row.ExpireRows)
                            {
                                if (row.SelectedSubaccount?.P3CLFO == "C")
                                {
                                    var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                    var newRow = new PNCLIENTI()
                                    {
                                        N2SOCI = Head.N1SOCI,
                                        N2ANNO = Head.N1ANNO,
                                        N2REGI = Head.N1REGI,
                                        N2RIGA = sectionalRowID++,
                                        N2DARI = row.N1DARI,
                                        N2RIFE = row.N1RIFE,
                                        N2DOCU = Head.N1docn,
                                        N2DARE = Head.N1DARE,
                                        N2DADO = Head.N1docd,
                                        N2CAUS = Head.pncaus,
                                        N2GRUP = row.pngrup,
                                        N2CONT = row.pncont,
                                        N2SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                        N2SOTT = row.n1clie,
                                        N2IMPO = exp.Amount,
                                        N2DESC = exp.Note != null ? exp.Note : string.Empty,
                                        N2SEGN = exp.Sign,
                                        N2RATA = 0,
                                        N2SCAD = exp.ExpireDate,
                                        N2PAGA = row.n1paga,
                                        N2PRAT = string.Empty,
                                        N2DEST = string.Empty,
                                        N2PAXI = 0,
                                        N2CAMB = 0,
                                        N2VALU = exp.CurrencyAmount,
                                        N2DIVI = exp.CurrencyID,
                                        n2vcod = "UIC",
                                        N2FLIN = string.Empty,
                                        N2IMEU = exp.Amount,
                                        N2TIDO = "E",
                                        n2rior = exp.OriginalRowID,
                                        N2FL01 = string.Empty,
                                        n2tipi = (VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty))?.pcltip,
                                        N2VADO = "UIC",
                                        N2DIDO = exp.CurrencyID,
                                        n2comm = string.Empty,
                                        N2ANTI = 0,
                                        N2INIZ = SelectedCausal.cauceco == "S" ? row.N1DASM : null,
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, newRow, transaction);
                                }
                                else
                                {
                                    if (row.SelectedSubaccount?.P3CLFO == "F")
                                    {
                                        var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                        var newRow = new PNFORNITORI()
                                        {
                                            N3SOCI = Head.N1SOCI,
                                            N3ANNO = Head.N1ANNO,
                                            N3REGI = Head.N1REGI,
                                            N3RIGA = sectionalRowID++,
                                            N3DARI = row.N1DARI,
                                            N3RIFE = row.N1RIFE,
                                            N3DOCU = Head.N1docn,
                                            N3DARE = Head.N1DARE,
                                            N3DADO = Head.N1docd,
                                            N3CAUS = Head.pncaus,
                                            N3GRUP = row.pngrup,
                                            N3CONT = row.pncont,
                                            N3SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                            N3SOTT = row.n1clie,
                                            N3IMPO = exp.Amount,
                                            N3DESC = exp.Note != null ? exp.Note : string.Empty,
                                            N3SEGN = exp.Sign,
                                            N3RATA = 0,
                                            N3SCAD = exp.ExpireDate,
                                            N3PAGA = row.n1paga,
                                            N3PRAT = string.Empty,
                                            N3DEST = string.Empty,
                                            N3PAXI = 0,
                                            N3CAMB = 0,
                                            N3VALU = exp.CurrencyAmount,
                                            N3DIVI = exp.CurrencyID,
                                            n3vcod = "UIC",
                                            N3FLPA = string.Empty,
                                            N3ABIF = 0,
                                            N3CABF = 0,
                                            N3IMEU = exp.Amount,
                                            N3TIDO = "E",
                                            n3rior = exp.OriginalRowID,
                                            N3FL01 = string.Empty,
                                            n3tipp = (VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty))?.pfotip,
                                            N3VADO = "UIC",
                                            N3DIDO = "UIC",
                                            n3comm = string.Empty,
                                            N3CCNF = string.Empty,
                                            N3SOCA = Head.N1SOCI,
                                            N3ABIA = 0,
                                            N3CABA = 0,
                                            N3CCNA = string.Empty
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, newRow, transaction);
                                    }
                                }
                            }
                        }
                    }
                    // add IVA rows
                    // delete current
                    connection.Execute(
                    "DELETE FROM PNIVA WHERE N4SOCI = @n4soci AND N4ANNO = @n4anno AND N4REGI = @n4regi",
                    new { n4soci = Head.N1SOCI, n4anno = Head.N1ANNO, n4regi = Head.N1REGI },
                    transaction);
                    foreach (var row in IVARows)
                    {
                        row.N4DARE = Head.N1DARE;
                        if (row.N4DTSCPG.HasValue)
                            row.N4DTSCAD = row.N4DTSCPG.Value.AddYears(1);
                        if (row.N4DTSCPG.HasValue && row.N4DTSCPG.Value > row.N4DADO)
                            row.N4FLIVCA = "S";
                        row.N4REGI = Head.N1REGI;
                        // reset document date and number
                        row.N4DOCU = Head.N1docn;
                        row.N4DADO = Head.N1docd;
                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, row, transaction);
                    }
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

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(PNTESTATA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().CheckPrinted(Model.N1SOCI, Model.N1ANNO, Model.N1REGI))
            {
                return false;
            }
            else
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // delete rows
                        connection.Execute(
                            "DELETE FROM PNRIGHE WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI",
                            Model, transaction);
                        // delete IVA rows
                        connection.Execute(
                            "DELETE FROM PNIVA WHERE N4SOCI = @N1SOCI AND N4ANNO = @N1ANNO AND N4REGI = @N1REGI",
                            Model, transaction);
                        // delete PNPORTAFOGLIO rows
                        connection.Execute(
                            "DELETE FROM PNPORTAFOGLIO WHERE N6SOCI = @N1SOCI AND N6ANNO = @N1ANNO AND N6REGI = @N1REGI",
                            Model, transaction);
                        // delete customer/supplier rows
                        connection.Execute(
                            "DELETE FROM PNCLIENTI WHERE N2SOCI = @N1SOCI AND N2ANNO = @N1ANNO AND N2REGI = @N1REGI",
                            Model, transaction);
                        connection.Execute(
                            "DELETE FROM PNFORNITORI WHERE N3SOCI = @N1SOCI AND N3ANNO = @N1ANNO AND N3REGI = @N1REGI",
                            Model, transaction);
                        // delete head
                        connection.Execute(
                            "DELETE FROM PNTESTATA WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI",
                            Model, transaction);
                        // check if need to unlock received invoices
                        connection.Execute(
                            "UPDATE ACC_EINVOICE_HEADS SET fattannoreg=NULL, fattnumreg=NULL, accounted=NULL, accounted_UserID=NULL WHERE fattsoc = @N1SOCI AND fattannoreg = @N1ANNO AND fattnumreg = @N1REGI",
                            Model, transaction);
                        transaction.Commit();
                        return true;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Import(List<ImportPNModel> PNs)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                string? retValue = null;

                foreach (var pns in PNs.GroupBy(g => new { g.SocietaID, g.Anno, g.ID }))
                {
                    int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(pns.Key.SocietaID, (int)pns.Key.Anno, Constants.PN, true);

                    if (newID > 0)
                    {
                        var first = pns.First();

                        if (first.Valuta?.ToUpper() == "EURO")
                            first.Valuta = "EUR";
                        if (first.Valuta?.ToUpper() == "DOLLAR")
                            first.Valuta = "USD";

                        var testata = new PNTESTATA
                        {
                            N1SOCI = pns.Key.SocietaID,
                            N1ANNO = pns.Key.Anno,
                            N1REGI = newID,
                            pncaus = first.CausaleID,
                            N1DARE = first.Data,
                            pnvcod = "UIC",
                            pnvdiv = first.Valuta?.ToUpper(),
                            N1FL01 = " ",
                            N1docn = first.DocumentoID,
                            N1docd = first.DocumentoData,
                            N1rifn = first.RiferimentoID,
                            N1rifd = first.RiferimentoData,
                            N1FLCF = " ",
                            N1CLFO = 0,
                            N1VADO = " ",
                            N1DIDO = " ",
                            N1DADI = new DateTime(1753, 1, 1),
                            N1CADO = 0,
                            N1IMDO = 0,
                            n1mrii = 0,
                            N1TmpPN = "N",
                        };
                        connection.Execute(INSERT_QUERY, testata, transaction);

                        int currentRow = 0;
                        foreach (var pn in PNs)
                        {
                            if (pn.Valuta?.ToUpper() == "EURO")
                                pn.Valuta = "EUR";
                            if (pn.Valuta?.ToUpper() == "DOLLAR")
                                pn.Valuta = "USD";

                            ++currentRow;

                            var riga = new PNRIGHE
                            {
                                N1SOCI = first.SocietaID,
                                N1ANNO = first.Anno,
                                N1REGI = newID,
                                N1RIGA = currentRow,
                                N1DOCU = pn.DocumentoID,
                                N1RIFE = pn.RiferimentoID,
                                N1DADO = pn.DocumentoData,
                                N1DARI = pn.RiferimentoData,
                                pngrup = pn.GruppoID,
                                pncont = pn.ContoID,
                                pnsott = pn.SottocontoID,
                                N1IMPO = 0,
                                N1DESC = pn.Note,
                                N1SEGN = pn.Segno == "+" ? "D" : "A",
                                n1clie = 0,
                                N1CCCC = "",
                                N1CCCS = "",
                                N1CONP = "",
                                N1COMM = "",
                                N1DEST = "",
                                N1GIRO = "",
                                N1STNO = "",
                                N1STBO = "",
                                N1STMA = "",
                                N1CHIU = "A",
                                N1RIGB = 0,
                                N1DABB = new DateTime(1753, 1, 1),
                                N1DASM = new DateTime(1753, 1, 1),
                                N1DACC = new DateTime(1753, 1, 1),
                                N1FIVA = "",
                                N1BC01 = "",
                                N1BC02 = 0,
                                N1BC03 = 0,
                                N1BC04 = "",
                                N1BC05 = "",
                                N1IMEU = pn.Importo,
                                N1TIDO = "E",
                                n1scad = new DateTime(1753, 1, 1),
                                n1paga = "",
                                N1DRri = testata.N1DARE,
                                N1SCON = "",
                                N1DIVI = pn.Valuta?.ToUpper(),
                                N1CAMB = 0,
                                N1IMVA = 0,
                                N1FLCO = "",
                                N1tmpPNR = "N",
                                n1mese = 0,
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, riga, transaction);
                        }

                        retValue += ($"{pns.Key.Anno}-{testata.N1REGI}");
                        retValue += Environment.NewLine;


                    }
                    else
                    {
                        transaction.Rollback();
                        ErrorHandler.Show($"Impossibile recuperare un nuovo numero di registrazione");
                        return null;
                    }
                }

                return retValue;

            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public Tuple<string, int, int>? Duplicate(string CompanyID, int Year, int Number, int AccountingYear, DateTime Date, CAUCONT SelectedCausal, bool IsRemove)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, AccountingYear, Constants.PN, true);

                Tuple<string, int, int>? retValue = null;

                if (newID > 0)
                {
                    var first = Get(CompanyID, Year, Number);

                    if (first != null)
                    {
                        var testata = new PNTESTATA
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = AccountingYear,
                            N1REGI = newID,
                            pncaus = SelectedCausal.caucod,
                            N1DARE = Date,
                            pnvcod = first.pnvcod,
                            pnvdiv = first.pnvdiv,
                            N1FL01 = first.N1FL01,
                            N1docn = first.N1docn,
                            N1docd = first.N1docd,
                            N1rifn = first.N1rifn,
                            N1rifd = first.N1rifd,
                            N1FLCF = first.N1FLCF,
                            N1CLFO = first.N1CLFO,
                            N1VADO = first.N1VADO,
                            N1DIDO = first.N1DIDO,
                            N1DADI = first.N1DADI,
                            N1CADO = first.N1CADO,
                            N1IMDO = first.N1IMDO,
                            n1mrii = first.n1mrii,
                            N1TmpPN = first.N1TmpPN,
                        };
                        connection.Execute(INSERT_QUERY, testata, transaction);

                        var rows = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().GetList(CompanyID, Year, Number);

                        int currentRow = 0;
                        foreach (var pn in rows ?? new ObservableCollection<PNRIGHE>())
                        {
                            string? sign = pn.N1SEGN;

                            if (IsRemove)
                            {
                                if (sign == "D")
                                    sign = "A";
                                else
                                    sign = "D";
                            }

                            ++currentRow;

                            var riga = new PNRIGHE
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = AccountingYear,
                                N1REGI = newID,
                                N1RIGA = pn.N1RIGA,
                                N1DOCU = pn.N1DOCU,
                                N1RIFE = pn.N1RIFE,
                                N1DADO = pn.N1DADO,
                                N1DARI = pn.N1DARI,
                                pngrup = pn.pngrup,
                                pncont = pn.pncont,
                                pnsott = pn.pnsott,
                                N1IMPO = pn.N1IMPO,
                                N1DESC = pn.N1DESC,
                                N1SEGN = sign,
                                n1clie = pn.n1clie,
                                N1CCCC = pn.N1CCCC,
                                N1CCCS = pn.N1CCCS,
                                N1CONP = pn.N1CONP,
                                N1COMM = pn.N1COMM,
                                N1DEST = pn.N1DEST,
                                N1GIRO = pn.N1GIRO,
                                N1STNO = pn.N1STNO,
                                N1STBO = pn.N1STBO,
                                N1STMA = pn.N1STMA,
                                N1CHIU = pn.N1CHIU,
                                N1RIGB = pn.N1RIGB,
                                N1DABB = pn.N1DABB,
                                N1DASM = pn.N1DASM,
                                N1DACC = pn.N1DACC,
                                N1FIVA = pn.N1FIVA,
                                N1BC01 = pn.N1BC01,
                                N1BC02 = pn.N1BC02,
                                N1BC03 = pn.N1BC03,
                                N1BC04 = pn.N1BC04,
                                N1BC05 = pn.N1BC05,
                                N1IMEU = pn.N1IMEU,
                                N1TIDO = pn.N1TIDO,
                                n1scad = pn.n1scad,
                                n1paga = pn.n1paga,
                                N1DRri = testata.N1DARE,
                                N1SCON = pn.N1SCON,
                                N1DIVI = pn.N1DIVI,
                                N1CAMB = pn.N1CAMB,
                                N1IMVA = pn.N1IMVA,
                                N1FLCO = pn.N1FLCO,
                                N1tmpPNR = pn.N1tmpPNR,
                                n1mese = pn.n1mese,
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, riga, transaction);
                        }

                        transaction.Commit();

                        retValue = new Tuple<string, int, int>(CompanyID, AccountingYear, newID);
                    }
                    else
                    {
                        ErrorHandler.Show($"Registrazione non trovata - {CompanyID}|{Year}|{Number}");
                    }
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show($"Impossibile recuperare un nuovo numero di registrazione");
                    return null;
                }

                return retValue;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public string? Validate(PNTESTATA Model, bool IsInsert)
    {
        try
        {
            if (!Model.N1DARE.HasValue)
            {
                return $"La data di registrazione è obbligatoria";
            }
            var validYears = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(Model.N1SOCI)!;

            if (!validYears.Where(o => o.eseann == Model.N1ANNO).Any())
            {
                return $"Anno contabile non aperto - {Model.N1ANNO}";
            }

            var validStart = new DateTime(validYears.Where(w => w.eseann == Model.N1ANNO).First().eseann, validYears.Where(w => w.eseann == Model.N1ANNO).First().eseini ?? 1, 1);
            var validEnd = validStart.AddMonths(12);

            if (Model.N1DARE.Value > validEnd ||
                Model.N1DARE.Value < validStart)
            {
                return $"La data di registrazione non è valida";
            }
            if (string.IsNullOrWhiteSpace(Model.pncaus))
            {
                return $"La causale contabile è obbligatoria";
            }
            var causal = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Get(Model.pncaus)!;

            if (!causal.causolBool && Model.Amount == 0 && !string.IsNullOrWhiteSpace(causal.cauliv) && !causal.cauzer)
            {
                return $"L'importo totale del documento è obbligatorio";
            }
            int testDoc = 0;
            if (causal.cauivaBool)
            {
                // check rate on customer/supplier
                if (IsInsert)
                {
                    if (Model.N1FLCF == "C")
                    {
                        var entityInfo = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(Model.N1CLFO ?? 0);
                        if (string.IsNullOrWhiteSpace(entityInfo?.classo) || string.IsNullOrWhiteSpace(entityInfo?.classa))
                            return $"Il cliente scelto non ha un'aliquota di default impostata";
                    }
                    else
                    {
                        var entityInfo = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(Model.N1SOCI, Model.N1CLFO ?? 0);
                        if (string.IsNullOrWhiteSpace(entityInfo?.foaass) || string.IsNullOrWhiteSpace(entityInfo?.foaali))
                            return $"Il fornitore scelto non ha un'aliquota di default impostata";
                    }
                }
                // check only numbers in document
                if (!int.TryParse(Model.N1docn, out testDoc))
                    return "La causale contabile prevede IVA quindi il numero documento deve contenere solo numeri e nessun carattere";
                // check protocol already used
                if (VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().CheckProtocolAlreadyUsed(Model.N1SOCI, Model.N1ANNO, Model.N1REGI, Model.N1docn, causal.cauliv ?? string.Empty))
                    return "Il numero di protocollo scelto è già utilizzato";
                if (!Model.ForceProtocol)
                {
                    // check valid protocol ID
                    var older = Get(Model.N1SOCI, Model.N1ANNO, Model.N1REGI);
                    if (older == null || older.N1docn != Model.N1docn || older.N1docd != Model.N1docd)
                    {
                        var lastProtocolInfo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetLastProtocolUsed(Model.N1SOCI, Model.N1DARE.Value, Model.N1REGI, causal.cauliv ?? string.Empty);
                        if (int.Parse(lastProtocolInfo?.Item1 ?? "0") > 0)
                        {
                            if ((int.Parse(lastProtocolInfo?.Item1 ?? "0") + 1) != int.Parse(Model.N1docn) || lastProtocolInfo?.Item2 > Model.N1docd)
                            { return $"Il numero protocollo deve essere consequenziale, senza salti e la data dello stesso non deve essere inferiore all'ultimo protocollo usato.\n\nUltimo protocollo: {lastProtocolInfo?.Item1} del {lastProtocolInfo?.Item2.ToString("dd/MM/yyyy")}"; }
                        }
                    }
                }
            }
            if ((string.IsNullOrWhiteSpace(Model.N1docn) || !Model.N1docd.HasValue) && !string.IsNullOrWhiteSpace(causal.cauliv))
            {
                return $"Gli estremi del documento sono obbligatori";
            }
            if ((string.IsNullOrWhiteSpace(Model.N1rifn) || !Model.N1rifd.HasValue) && !string.IsNullOrWhiteSpace(causal.cauliv))
            {
                return $"Gli estremi del riferimento sono obbligatori";
            }
            if (!string.IsNullOrWhiteSpace(Model.N1FLCF) && !Model.N1CLFO.HasValue && !Model.N1CLFO.HasValue)
            {
                return $"Se si seleziona un tipo codice occorre selezionare un cliente/fornitore altrimenti non va selezionato alcun codice";
            }
            if (!string.IsNullOrWhiteSpace(Model.N1FLCF) && !causal.caucliBool && !causal.cauforBool)
            {
                return "La causale non prevede nessun cliente ne' fornitore";
            }
            if (string.IsNullOrWhiteSpace(Model.N1FLCF) && causal.caucliBool)
            {
                return "La causale prevede un cliente";
            }
            if (string.IsNullOrWhiteSpace(Model.N1FLCF) && causal.cauforBool)
            {
                return "La causale prevede un fornitore";
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}

public class PNTESTATAUfpRepository : RepositoryBase, IPNTESTATARepository
{
    public PNTESTATAUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<PNTESTATA>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<PNTESTATA>(
                "SELECT * FROM PN_TESTATA WHERE N1SOCI = @cid AND N1ANNO = @yea ORDER BY N1REGI DESC, N1DARE DESC",
                new { cid = CompanyID, yea = Year });

            return new ObservableCollection<PNTESTATA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public PNTESTATA? Get(string CompanyID, int Year, int Number)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<PNTESTATA, CAUCONT, ABE, PNTESTATA>(
                @"SELECT t.*, (SELECT SUM(N1IMEU) FROM PN_RIGHE WHERE N1SEGN = 'D' AND N1SOCI = t.N1SOCI AND N1ANNO = t.N1ANNO AND N1REGI = t.N1REGI) as Amount, l.caucod, l.caudes, l.cauiva, a.abecod, TRIM(a.abers1) AS abers1, TRIM(a.abers2) AS abers2 FROM PN_TESTATA AS t 
                        INNER JOIN CAUCONT AS l ON t.PNCAUS = l.CAUCOD
                        LEFT OUTER JOIN ANAG_BASE AS a ON a.abecod = t.N1CLFO
                        WHERE t.N1SOCI = @cid AND t.N1ANNO = @yea AND t.N1REGI = @num",
                (tes, cau, bas) => { tes.AccountingCausal = cau; tes.BasicRegistry = bas; return tes; },
                new { cid = CompanyID, yea = Year, num = Number },
                splitOn: "caucod,abecod")
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO PN_TESTATA (N1SOCI,N1ANNO,N1REGI,pncaus,N1DARE,pnvcod,pnvdiv,N1FL01,N1docn,N1docd,N1rifn,N1rifd,N1FLCF,N1CLFO,N1VADO,N1DIDO,N1DADI,N1CADO,N1IMDO,n1mrii,N1TmpPN,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,N1AUAN,N1AUNU,N1AUGE) OUTPUT INSERTED.rv VALUES(@N1SOCI,@N1ANNO,@N1REGI,@pncaus,@N1DARE,@pnvcod,@pnvdiv,@N1FL01,@N1docn,@N1docd,@N1rifn,@N1rifd,@N1FLCF,@N1CLFO,@N1VADO,@N1DIDO,@N1DADI,@N1CADO,@N1IMDO,@n1mrii,@N1TmpPN,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@N1AUAN,@N1AUNU,@N1AUGE)";
    public string UPDATE_QUERY => "UPDATE PN_TESTATA SET N1SOCI = @N1SOCI,N1ANNO = @N1ANNO,N1REGI = @N1REGI,pncaus = @pncaus,N1DARE = @N1DARE,pnvcod = @pnvcod,pnvdiv = @pnvdiv,N1FL01 = @N1FL01,N1docn = @N1docn,N1docd = @N1docd,N1rifn = @N1rifn,N1rifd = @N1rifd,N1FLCF = @N1FLCF,N1CLFO = @N1CLFO,N1VADO = @N1VADO,N1DIDO = @N1DIDO,N1DADI = @N1DADI,N1CADO = @N1CADO,N1IMDO = @N1IMDO,n1mrii = @n1mrii,N1TmpPN = @N1TmpPN,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,N1AUAN = @N1AUAN,N1AUNU = @N1AUNU,N1AUGE = @N1AUGE OUTPUT INSERTED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM PN_TESTATA OUTPUT DELETED.rv WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI AND rv = @rv";

    public bool Insert(PNTESTATA Head, ObservableCollection<PNRIGHE> Rows, CAUCONT SelectedCausal, ObservableCollection<PNIVA> IVARows)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // get ID
                    int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Head.N1SOCI, Head.N1ANNO, Constants.PN, true);
                    if (newID > 0)
                    {
                        Head.N1REGI = newID;
                        // add head
                        connection.Execute(INSERT_QUERY, Head, transaction);

                        // add rows
                        int sectionalRowID = 1;
                        foreach (var row in Rows)
                        {
                            row.N1REGI = newID;
                            row.N1DRri = Head.N1DARE;
                            row.n1mese = row.N1DADO?.Month;
                            row.N1DESC = row.N1DESC != null ? row.N1DESC : string.Empty;

                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, row, transaction);
                            // sectional
                            if (row.SelectedSubaccount?.P3CLFO == "C")
                            {
                                var exps = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().ComputeExpires(row.N1DADO ?? DateTime.MinValue, row.n1paga ?? string.Empty) ?? new List<DateTime>();

                                // Richiesta di Aldo : se cauiva = 'N' non splitta per scadenze
                                if (SelectedCausal.cauiva == "N" && row.n1scad.HasValue)
                                {
                                    exps.Clear();
                                    exps.Add(row.n1scad.Value);
                                }

                                var pagcli = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty);
                                decimal splitAmount = Math.Round((row.N1IMEU ?? 0) / exps.Count, 2);
                                decimal splitRemains = (row.N1IMEU ?? 0) - (splitAmount * exps.Count);
                                var moveExpireID = VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(row.N1SOCI, (row.n1clie ?? 0))?.scacod;

                                SCADENZE? mover = null;
                                if (!string.IsNullOrWhiteSpace(moveExpireID))
                                    mover = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().Get(moveExpireID);

                                foreach (var (value, i) in exps.Select((value, i) => (value, i)))
                                {
                                    var expItem = value;
                                    if (!string.IsNullOrWhiteSpace(moveExpireID))
                                    {
                                        expItem = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().ComputeExpireMove(mover, expItem);
                                    }
                                    else
                                    {
                                        if (pagcli != null)
                                            if (pagcli.pclgio.HasValue)
                                                expItem = expItem.AddDays(pagcli.pclgio.Value);
                                    }
                                    var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                    var newRow = new PNCLIENTI()
                                    {
                                        N2SOCI = Head.N1SOCI,
                                        N2ANNO = Head.N1ANNO,
                                        N2REGI = newID,
                                        N2RIGA = sectionalRowID++,
                                        N2DARI = row.N1DARI,
                                        N2RIFE = row.N1RIFE,
                                        N2DOCU = Head.N1docn,
                                        N2DARE = Head.N1DARE,
                                        N2DADO = Head.N1docd,
                                        N2CAUS = Head.pncaus,
                                        N2GRUP = row.pngrup,
                                        N2CONT = row.pncont,
                                        N2SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                        N2SOTT = row.n1clie,
                                        N2IMPO = row.N1IMPO ?? 0,
                                        N2DESC = row.N1DESC != null ? row.N1DESC : string.Empty,
                                        N2SEGN = row.N1SEGN,
                                        N2RATA = 0,
                                        N2SCAD = expItem,
                                        N2PAGA = row.n1paga,
                                        N2PRAT = string.Empty,
                                        N2DEST = string.Empty,
                                        N2PAXI = 0,
                                        N2CAMB = 0,
                                        N2VALU = row.N1IMVA ?? 0,
                                        N2DIVI = row.N1DIVI,
                                        n2vcod = "UIC",
                                        N2FLIN = string.Empty,
                                        N2IMEU = i + 1 == exps.Count ? splitAmount + splitRemains : splitAmount,
                                        N2TIDO = "E",
                                        n2rior = row.N1RIGA,
                                        N2FL01 = string.Empty,
                                        n2tipi = (VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty))?.pcltip,
                                        N2VADO = "UIC",
                                        N2DIDO = row.N1DIVI,
                                        n2comm = string.Empty,
                                        N2ANTI = 0,
                                        N2INIZ = SelectedCausal.cauceco == "S" ? row.N1DASM : null,
                                        N2PARE = string.Empty,
                                    };
                                    connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, newRow, transaction);
                                }
                            }
                            else
                            {
                                if (row.SelectedSubaccount?.P3CLFO == "F")
                                {
                                    var exps = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().ComputeExpires(row.N1DADO ?? DateTime.MinValue, row.n1paga ?? string.Empty) ?? new List<DateTime>();

                                    // Richiesta di Aldo : se cauiva = 'N' non splitta per scadenze
                                    if (SelectedCausal.cauiva == "N" && row.n1scad.HasValue)
                                    {
                                        exps.Clear();
                                        exps.Add(row.n1scad.Value);
                                    }

                                    var pagfor = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty);
                                    decimal splitAmount = Math.Round((row.N1IMEU ?? 0) / exps.Count, 2);
                                    decimal splitRemains = (row.N1IMEU ?? 0) - (splitAmount * exps.Count);
                                    var moveExpireID = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(row.N1SOCI, row.n1clie ?? 0)?.scacod;
                                    SCADENZE? mover = null;
                                    if (!string.IsNullOrWhiteSpace(moveExpireID))
                                        mover = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().Get(moveExpireID);
                                    foreach (var (value, i) in exps.Select((value, i) => (value, i)))
                                    {
                                        var expItem = value;
                                        if (!string.IsNullOrWhiteSpace(moveExpireID))
                                            expItem = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().ComputeExpireMove(mover, expItem);
                                        var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                        var newRow = new PNFORNITORI()
                                        {
                                            N3SOCI = Head.N1SOCI,
                                            N3ANNO = Head.N1ANNO,
                                            N3REGI = newID,
                                            N3RIGA = sectionalRowID++,
                                            N3DARI = row.N1DARI,
                                            N3RIFE = row.N1RIFE,
                                            N3DOCU = Head.N1docn,
                                            N3DARE = Head.N1DARE,
                                            N3DADO = Head.N1docd,
                                            N3CAUS = Head.pncaus,
                                            N3GRUP = row.pngrup,
                                            N3CONT = row.pncont,
                                            N3SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                            N3SOTT = row.n1clie,
                                            N3IMPO = row.N1IMPO ?? 0,
                                            N3DESC = row.N1DESC,
                                            N3SEGN = row.N1SEGN,
                                            N3RATA = 0,
                                            N3SCAD = expItem,
                                            N3PAGA = row.n1paga,
                                            N3PRAT = string.Empty,
                                            N3DEST = string.Empty,
                                            N3PAXI = 0,
                                            N3CAMB = 0,
                                            N3VALU = row.N1IMVA ?? 0,
                                            N3DIVI = row.N1DIVI,
                                            n3vcod = "UIC",
                                            N3FLPA = string.Empty,
                                            N3ABIF = 0,
                                            N3CABF = 0,
                                            N3IMEU = i + 1 == exps.Count ? splitAmount + splitRemains : splitAmount,
                                            N3TIDO = "E",
                                            n3rior = row.N1RIGA,
                                            N3FL01 = string.Empty,
                                            n3tipp = (VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty))?.pfotip,
                                            N3VADO = "UIC",
                                            N3DIDO = "UIC",
                                            n3comm = string.Empty,
                                            N3CCNF = string.Empty,
                                            N3SOCA = row.N1SOCI,
                                            N3ABIA = 0,
                                            N3CABA = 0,
                                            N3CCNA = string.Empty,
                                            N3PARE = string.Empty,
                                        };
                                        connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, newRow, transaction);
                                    }
                                }
                            }
                            if (row.SelectedSubaccount?.P3CLFO == "C" || row.SelectedSubaccount?.P3CLFO == "F")
                            {
                                // add manual sectional rows
                                if (row.ExpireRows != null && row.ExpireRows.Count > 0)
                                {
                                    foreach (var exp in row.ExpireRows)
                                    {
                                        if (row.SelectedSubaccount.P3CLFO == "C")
                                        {
                                            var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                            var newRow = new PNCLIENTI()
                                            {
                                                N2SOCI = Head.N1SOCI,
                                                N2ANNO = Head.N1ANNO,
                                                N2REGI = newID,
                                                N2RIGA = sectionalRowID++,
                                                N2DARI = row.N1DARI,
                                                N2RIFE = row.N1RIFE,
                                                N2DOCU = Head.N1docn,
                                                N2DARE = Head.N1DARE,
                                                N2DADO = Head.N1docd,
                                                N2CAUS = Head.pncaus,
                                                N2GRUP = row.pngrup,
                                                N2CONT = row.pncont,
                                                N2SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                                N2SOTT = row.n1clie,
                                                N2IMPO = exp.Amount,
                                                N2DESC = exp.Note != null ? exp.Note : string.Empty,
                                                N2SEGN = exp.Sign,
                                                N2RATA = 0,
                                                N2SCAD = exp.ExpireDate,
                                                N2PAGA = row.n1paga,
                                                N2PRAT = string.Empty,
                                                N2DEST = string.Empty,
                                                N2PAXI = 0,
                                                N2CAMB = 0,
                                                N2VALU = exp.CurrencyAmount,
                                                N2DIVI = exp.CurrencyID,
                                                n2vcod = "UIC",
                                                N2FLIN = string.Empty,
                                                N2IMEU = exp.Amount,
                                                N2TIDO = "E",
                                                n2rior = exp.OriginalRowID,
                                                N2FL01 = string.Empty,
                                                n2tipi = (VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().Get(row.n1paga ?? string.Empty))?.pcltip,
                                                N2VADO = "UIC",
                                                N2DIDO = exp.CurrencyID,
                                                n2comm = string.Empty,
                                                N2ANTI = 0,
                                                N2INIZ = SelectedCausal.cauceco == "S" ? row.N1DASM : null,
                                                N2PARE = string.Empty,
                                            };
                                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().INSERT_QUERY, newRow, transaction);
                                        }
                                        else
                                        {
                                            if (row.SelectedSubaccount.P3CLFO == "F")
                                            {
                                                var pdcCompanyID = (VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                                var newRow = new PNFORNITORI()
                                                {
                                                    N3SOCI = Head.N1SOCI,
                                                    N3ANNO = Head.N1ANNO,
                                                    N3REGI = newID,
                                                    N3RIGA = sectionalRowID++,
                                                    N3DARI = row.N1DARI,
                                                    N3RIFE = row.N1RIFE,
                                                    N3DOCU = Head.N1docn,
                                                    N3DARE = Head.N1DARE,
                                                    N3DADO = Head.N1docd,
                                                    N3CAUS = Head.pncaus,
                                                    N3GRUP = row.pngrup,
                                                    N3CONT = row.pncont,
                                                    N3SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                                    N3SOTT = (row.n1clie ?? 0),
                                                    N3IMPO = exp.Amount,
                                                    N3DESC = exp.Note != null ? exp.Note : string.Empty,
                                                    N3SEGN = exp.Sign,
                                                    N3RATA = 0,
                                                    N3SCAD = exp.ExpireDate,
                                                    N3PAGA = row.n1paga,
                                                    N3PRAT = string.Empty,
                                                    N3DEST = string.Empty,
                                                    N3PAXI = 0,
                                                    N3CAMB = 0,
                                                    N3VALU = exp.CurrencyAmount,
                                                    N3DIVI = exp.CurrencyID,
                                                    n3vcod = "UIC",
                                                    N3FLPA = string.Empty,
                                                    N3ABIF = 0,
                                                    N3CABF = 0,
                                                    N3IMEU = exp.Amount,
                                                    N3TIDO = "E",
                                                    n3rior = exp.OriginalRowID,
                                                    N3FL01 = string.Empty,
                                                    n3tipp = (VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().Get(row.n1paga ?? string.Empty))?.pfotip,
                                                    N3VADO = "UIC",
                                                    N3DIDO = "UIC",
                                                    n3comm = string.Empty,
                                                    N3CCNF = string.Empty,
                                                    N3SOCA = Head.N1SOCI,
                                                    N3ABIA = 0,
                                                    N3CABA = 0,
                                                    N3CCNA = string.Empty,
                                                    N3PARE = string.Empty,
                                                };
                                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().INSERT_QUERY, newRow, transaction);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // add IVA rows
                        bool hasIndetraibile = false;
                        decimal indetraibileTotal = 0;

                        foreach (var row in IVARows)
                        {
                            if (row.N4DTSCPG.HasValue)
                                row.N4DTSCAD = row.N4DTSCPG.Value.AddYears(1);
                            if (row.N4DTSCPG.HasValue && row.N4DTSCPG.Value > row.N4DADO)
                                row.N4FLIVCA = "S";
                            row.N4REGI = newID;

                            row.N4FLGS = "";
                            row.N4DAST = Constants._GX_MIN_DATE;

                            decimal indetraibileAmount = 0;
                            if (row.SelectedRate != null && row.SelectedRate.asspin.HasValue && row.SelectedRate.asspin.Value > 0)
                            {
                                indetraibileAmount = Math.Round(row.N4IIEU ?? 0, 2, MidpointRounding.AwayFromZero);
                                indetraibileTotal += indetraibileAmount;
                                hasIndetraibile = true;
                            }

                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().INSERT_QUERY, row, transaction);
                        }

                        #region Giroconto IVA indetraibile
                        if (hasIndetraibile)
                        {
                            // get registration number
                            var girocontoIndetraibileRegID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Head.N1SOCI, Head.N1ANNO, Constants.PN, true);
                            var ivaBook = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(SelectedCausal.cauliv ?? string.Empty);

                            var supplierGroups = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetListFull(Head.N1SOCI, Head.N1CLFO ?? 0, ivaBook?.livcii ?? string.Empty);
                            
                            string? ivaSign = null;
                            if (SelectedCausal.causeg == "+")
                            {
                                ivaSign = "+";
                            }
                            else
                            {
                                ivaSign = "-";
                            }

                            #region PNTESTATA
                            PNTESTATA girocontoHead = new PNTESTATA()
                            {
                                N1SOCI = Head.N1SOCI,
                                N1ANNO =Head.N1ANNO,
                                N1REGI = girocontoIndetraibileRegID,
                                pncaus = ivaBook?.livcii,
                                N1DARE = Head.N1DARE,
                                N1docn = Head.N1docn,
                                N1docd = Head.N1docd,
                                N1rifn = Head.N1rifn,
                                N1rifd = Head.N1rifd,
                                pnvcod = "UIC",
                                pnvdiv = "EUR",
                                N1FL01 = string.Empty,
                                N1TmpPN = "N",
                                n1mrii = 0,
                                addedUserID = Head.addedUserID,
                            };
                            connection.Execute(INSERT_QUERY, girocontoHead, transaction);
                            #endregion

                            PNRIGHE firstRow = new PNRIGHE()
                            {
                                N1SOCI = girocontoHead.N1SOCI,
                                N1ANNO = girocontoHead.N1ANNO,
                                N1REGI = girocontoHead.N1REGI,
                                N1RIGA = 1,
                                N1DOCU = girocontoHead.N1docn,
                                N1DADO = girocontoHead.N1docd,
                                N1RIFE = girocontoHead.N1rifn,
                                N1DARI = girocontoHead.N1rifd,
                                N1SEGN = ivaSign == "+" ? "A" : "D",
                                pngrup = ivaBook?.livgci,
                                pncont = ivaBook?.livcci,
                                pnsott = ivaBook?.livsci,
                                N1IMEU = indetraibileTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DESC = Head.EntityFullDescription,
                                N1DRri = girocontoHead.N1docd,
                                N1tmpPNR = "N"
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, firstRow, transaction);
                   
                            PNRIGHE secondRow = new PNRIGHE()
                            {
                                N1SOCI = girocontoHead.N1SOCI,
                                N1ANNO = girocontoHead.N1ANNO,
                                N1REGI = girocontoHead.N1REGI,
                                N1RIGA = 2,
                                N1DOCU = girocontoHead.N1docn,
                                N1DADO = girocontoHead.N1docd,
                                N1RIFE = girocontoHead.N1rifn,
                                N1DARI = girocontoHead.N1rifd,
                                N1SEGN = ivaSign == "+" ? "D" : "A",
                                pngrup = supplierGroups?.FirstOrDefault()?.cfgrup,
                                pncont = supplierGroups?.FirstOrDefault()?.cfcont,
                                pnsott = supplierGroups?.FirstOrDefault()?.cfsott,
                                N1IMEU = indetraibileTotal,
                                N1CHIU = "A",
                                N1TIDO = "E",
                                N1DIVI = "EUR",
                                N1DESC = Head.EntityFullDescription,
                                N1DRri = girocontoHead.N1docd,
                                N1tmpPNR = "N"
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, secondRow, transaction);

                            InfoHandler.Show($"Inserita correttamente la registrazione di giroconto n. {girocontoIndetraibileRegID}");
                        }
                        #endregion

                        transaction.Commit();

                        InfoHandler.Show($"Inserita correttamente la registrazione n. {newID}");
                        return true;
                    }
                    else
                    {
                        ErrorHandler.Show($"Impossibile recuperare un nuovo numero di registrazione");
                        return false;
                    }
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    ErrorHandler.Show($"{Constants.INSERT_VIOLATION}\n\n{exc.Message}");
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

    public bool Update(PNTESTATA Head, ObservableCollection<PNRIGHE> Rows, CAUCONT SelectedCausal, ObservableCollection<PNIVA> IVARows)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // update head
                    connection.Execute(UPDATE_QUERY, Head, transaction);
                    // add rows
                    // delete current
                    connection.Execute(
                    "DELETE FROM PN_RIGHE WHERE N1SOCI = @n1soci AND N1ANNO = @n1anno AND N1REGI = @n1regi",
                    new { n1soci = Head.N1SOCI, n1anno = Head.N1ANNO, n1regi = Head.N1REGI },
                    transaction);
                    int sectionalRowID = 1;
                    // delete current sectionals
                    connection.Execute(
                    "DELETE FROM PN_CLIENTI WHERE N2SOCI = @n2soci AND N2ANNO = @n2anno AND N2REGI = @n2regi",
                    new { n2soci = Head.N1SOCI, n2anno = Head.N1ANNO, n2regi = Head.N1REGI },
                    transaction);
                    connection.Execute(
                    "DELETE FROM PN_FORNITORI WHERE N3SOCI = @n3soci AND N3ANNO = @n3anno AND N3REGI = @n3regi",
                    new { n3soci = Head.N1SOCI, n3anno = Head.N1ANNO, n3regi = Head.N1REGI },
                    transaction);

                    var pnRigheRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                    var pdcSottoRepo = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>();
                    var pagCliRepo = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>();
                    var pagForRepo = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>();
                    var pnClientiRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>();
                    var pnFornitoriRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>();
                    var pnIvaRepo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>();

                    foreach (var row in Rows)
                    {
                        row.N1REGI = Head.N1REGI;
                        row.N1DRri = Head.N1DARE;
                        row.n1mese = row.N1DADO.HasValue ? row.N1DADO.Value.Month : Head.N1DARE!.Value.Month;
                        row.N1DESC = row.N1DESC != null ? row.N1DESC : string.Empty;
                        row.N1tmpPNR = Head.N1TmpPN;
                        // reset document date and number
                        //row.N1DOCU = Head.N1docn;
                        //row.N1DADO = Head.N1docd;

                        connection.Execute(pnRigheRepo.INSERT_QUERY, row, transaction);

                        // add sectional rows
                        if (row.ExpireRows != null && row.ExpireRows.Count > 0)
                        {
                            foreach (var exp in row.ExpireRows)
                            {
                                if (row.SelectedSubaccount?.P3CLFO == "C")
                                {
                                    var pdcCompanyID = (pdcSottoRepo.Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                    var newRow = new PNCLIENTI()
                                    {
                                        N2SOCI = Head.N1SOCI,
                                        N2ANNO = Head.N1ANNO,
                                        N2REGI = Head.N1REGI,
                                        N2RIGA = sectionalRowID++,
                                        N2DARI = row.N1DARI,
                                        N2RIFE = row.N1RIFE,
                                        N2DOCU = Head.N1docn,
                                        N2DARE = Head.N1DARE,
                                        N2DADO = Head.N1docd,
                                        N2CAUS = Head.pncaus,
                                        N2GRUP = row.pngrup,
                                        N2CONT = row.pncont,
                                        N2SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                        N2SOTT = row.n1clie,
                                        N2IMPO = exp.Amount,
                                        N2DESC = exp.Note != null ? exp.Note : string.Empty,
                                        N2SEGN = exp.Sign,
                                        N2RATA = 0,
                                        N2SCAD = exp.ExpireDate,
                                        N2PAGA = row.n1paga,
                                        N2PRAT = string.Empty,
                                        N2DEST = string.Empty,
                                        N2PAXI = 0,
                                        N2CAMB = exp.CurrencyChange,
                                        N2VALU = exp.CurrencyValue,
                                        N2DIVI = exp.CurrencyID,
                                        n2vcod = "UIC",
                                        N2FLIN = string.Empty,
                                        N2IMEU = exp.Amount,
                                        N2TIDO = "E",
                                        n2rior = exp.OriginalRowID,
                                        N2FL01 = string.Empty,
                                        n2tipi = (pagCliRepo.Get(row.n1paga ?? string.Empty))?.pcltip,
                                        N2VADO = "UIC",
                                        N2DIDO = exp.CurrencyDoc,
                                        n2comm = string.Empty,
                                        N2ANTI = 0,
                                        N2INIZ = SelectedCausal.cauceco == "S" ? row.N1DASM : null,
                                        N2PARE = string.Empty,
                                    };
                                    connection.Execute(pnClientiRepo.INSERT_QUERY, newRow, transaction);
                                }
                                else
                                {
                                    if (row.SelectedSubaccount?.P3CLFO == "F")
                                    {
                                        var pdcCompanyID = (pdcSottoRepo.Get(row.pngrup ?? string.Empty, row.pncont ?? string.Empty, (row.n1clie ?? 0).ToString(), Head.N1SOCI))?.p3soci;
                                        var newRow = new PNFORNITORI()
                                        {
                                            N3SOCI = Head.N1SOCI,
                                            N3ANNO = Head.N1ANNO,
                                            N3REGI = Head.N1REGI,
                                            N3RIGA = sectionalRowID++,
                                            N3DARI = row.N1DARI,
                                            N3RIFE = row.N1RIFE,
                                            N3DOCU = Head.N1docn,
                                            N3DARE = Head.N1DARE,
                                            N3DADO = Head.N1docd,
                                            N3CAUS = Head.pncaus,
                                            N3GRUP = row.pngrup,
                                            N3CONT = row.pncont,
                                            N3SSOC = !string.IsNullOrWhiteSpace(pdcCompanyID) ? pdcCompanyID : row.N1SOCI,
                                            N3SOTT = row.n1clie,
                                            N3IMPO = exp.Amount,
                                            N3DESC = exp.Note != null ? exp.Note : string.Empty,
                                            N3SEGN = exp.Sign,
                                            N3RATA = 0,
                                            N3SCAD = exp.ExpireDate,
                                            N3PAGA = row.n1paga,
                                            N3PRAT = string.Empty,
                                            N3DEST = string.Empty,
                                            N3PAXI = 0,
                                            N3CAMB = 0,
                                            N3VALU = exp.CurrencyValue,
                                            N3DIVI = exp.CurrencyID,
                                            n3vcod = "UIC",
                                            N3FLPA = string.Empty,
                                            N3ABIF = 0,
                                            N3CABF = 0,
                                            N3IMEU = exp.Amount,
                                            N3TIDO = "E",
                                            n3rior = exp.OriginalRowID,
                                            N3FL01 = string.Empty,
                                            n3tipp = (pagForRepo.Get(row.n1paga ?? string.Empty))?.pfotip,
                                            N3VADO = "UIC",
                                            N3DIDO = exp.CurrencyDoc,
                                            n3comm = string.Empty,
                                            N3CCNF = string.Empty,
                                            N3SOCA = Head.N1SOCI,
                                            N3ABIA = 0,
                                            N3CABA = 0,
                                            N3CCNA = string.Empty,
                                            N3PARE = string.Empty,
                                        };
                                        connection.Execute(pnFornitoriRepo.INSERT_QUERY, newRow, transaction);
                                    }
                                }
                            }
                        }
                    }
                    // add IVA rows
                    // delete current
                    connection.Execute(
                    "DELETE FROM PN_IVA WHERE N4SOCI = @n4soci AND N4ANNO = @n4anno AND N4REGI = @n4regi",
                    new { n4soci = Head.N1SOCI, n4anno = Head.N1ANNO, n4regi = Head.N1REGI },
                    transaction);

                    foreach (var row in IVARows)
                    {
                        row.N4DARE = Head.N1DARE;
                        if (row.N4DTSCPG.HasValue)
                            row.N4DTSCAD = row.N4DTSCPG.Value.AddYears(1);
                        if (row.N4DTSCPG.HasValue && row.N4DTSCPG.Value > row.N4DADO)
                            row.N4FLIVCA = "S";
                        row.N4REGI = Head.N1REGI;
                        // reset document date and number
                        row.N4DOCU = Head.N1docn;
                        row.N4DADO = Head.N1docd;

                        if (row.N4DAST == null)
                            row.N4DAST = Constants._GX_MIN_DATE;

                        if (row.N4FLGS == null)
                            row.N4FLGS = " ";

                        connection.Execute(pnIvaRepo.INSERT_QUERY, row, transaction);
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(ex.Message.ToString());
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

    public bool Delete(PNTESTATA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            if (VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().CheckPrinted(Model.N1SOCI, Model.N1ANNO, Model.N1REGI))
            {
                return false;
            }
            else
            {
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // delete rows
                        connection.Execute(
                            "DELETE FROM PN_RIGHE WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI",
                            Model, transaction);
                        // delete IVA rows
                        connection.Execute(
                            "DELETE FROM PN_IVA WHERE N4SOCI = @N1SOCI AND N4ANNO = @N1ANNO AND N4REGI = @N1REGI",
                            Model, transaction);
                        // delete PNPORTAFOGLIO rows
                        connection.Execute(
                            "DELETE FROM PN_PORTAFOGLIO WHERE N6SOCI = @N1SOCI AND N6ANNO = @N1ANNO AND N6REGI = @N1REGI",
                            Model, transaction);
                        // delete customer/supplier rows
                        connection.Execute(
                            "DELETE FROM PN_CLIENTI WHERE N2SOCI = @N1SOCI AND N2ANNO = @N1ANNO AND N2REGI = @N1REGI",
                            Model, transaction);
                        connection.Execute(
                            "DELETE FROM PN_FORNITORI WHERE N3SOCI = @N1SOCI AND N3ANNO = @N1ANNO AND N3REGI = @N1REGI",
                            Model, transaction);
                        // delete head
                        connection.Execute(
                            "DELETE FROM PN_TESTATA WHERE N1SOCI = @N1SOCI AND N1ANNO = @N1ANNO AND N1REGI = @N1REGI",
                            Model, transaction);
                        // check if need to unlock received invoices
                        connection.Execute(
                            "UPDATE FATTIMP SET fattannoreg=NULL, fattnumreg=NULL, accounted=NULL, accounted_UserID=NULL WHERE fattsoc = @N1SOCI AND fattannoreg = @N1ANNO AND fattnumreg = @N1REGI",
                            Model, transaction);
                        transaction.Commit();
                        return true;

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Import(List<ImportPNModel> PNs)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                string? retValue = null;

                foreach (var pns in PNs.GroupBy(g => new { g.SocietaID, g.Anno, g.ID }))
                {
                    int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(pns.Key.SocietaID, (int)pns.Key.Anno, Constants.PN, true);

                    if (newID > 0)
                    {
                        var first = pns.First();

                        if (first.Valuta?.ToUpper() == "EURO")
                            first.Valuta = "EUR";
                        if (first.Valuta?.ToUpper() == "DOLLAR")
                            first.Valuta = "USD";

                        var testata = new PNTESTATA
                        {
                            N1SOCI = pns.Key.SocietaID,
                            N1ANNO = pns.Key.Anno,
                            N1REGI = newID,
                            pncaus = first.CausaleID,
                            N1DARE = first.Data,
                            pnvcod = "UIC",
                            pnvdiv = first.Valuta?.ToUpper(),
                            N1FL01 = " ",
                            N1docn = first.DocumentoID,
                            N1docd = first.DocumentoData,
                            N1rifn = first.RiferimentoID,
                            N1rifd = first.RiferimentoData,
                            N1FLCF = " ",
                            N1CLFO = 0,
                            N1VADO = " ",
                            N1DIDO = " ",
                            N1DADI = new DateTime(1753, 1, 1),
                            N1CADO = 0,
                            N1IMDO = 0,
                            n1mrii = 0,
                            N1TmpPN = "N",
                        };
                        connection.Execute(INSERT_QUERY, testata, transaction);

                        int currentRow = 0;
                        foreach (var pn in PNs)
                        {
                            if (pn.Valuta?.ToUpper() == "EURO")
                                pn.Valuta = "EUR";
                            if (pn.Valuta?.ToUpper() == "DOLLAR")
                                pn.Valuta = "USD";

                            ++currentRow;

                            var riga = new PNRIGHE
                            {
                                N1SOCI = first.SocietaID,
                                N1ANNO = first.Anno,
                                N1REGI = newID,
                                N1RIGA = currentRow,
                                N1DOCU = pn.DocumentoID,
                                N1RIFE = pn.RiferimentoID,
                                N1DADO = pn.DocumentoData,
                                N1DARI = pn.RiferimentoData,
                                pngrup = pn.GruppoID,
                                pncont = pn.ContoID,
                                pnsott = pn.SottocontoID,
                                N1IMPO = 0,
                                N1DESC = pn.Note,
                                N1SEGN = pn.Segno == "+" ? "D" : "A",
                                n1clie = 0,
                                N1CCCC = "",
                                N1CCCS = "",
                                N1CONP = "",
                                N1COMM = "",
                                N1DEST = "",
                                N1GIRO = "",
                                N1STNO = "",
                                N1STBO = "",
                                N1STMA = "",
                                N1CHIU = "A",
                                N1RIGB = 0,
                                N1DABB = new DateTime(1753, 1, 1),
                                N1DASM = new DateTime(1753, 1, 1),
                                N1DACC = new DateTime(1753, 1, 1),
                                N1FIVA = "",
                                N1BC01 = "",
                                N1BC02 = 0,
                                N1BC03 = 0,
                                N1BC04 = "",
                                N1BC05 = "",
                                N1IMEU = pn.Importo,
                                N1TIDO = "E",
                                n1scad = new DateTime(1753, 1, 1),
                                n1paga = "",
                                N1DRri = testata.N1DARE,
                                N1SCON = "",
                                N1DIVI = pn.Valuta?.ToUpper(),
                                N1CAMB = 0,
                                N1IMVA = 0,
                                N1FLCO = "",
                                N1tmpPNR = "N",
                                n1mese = 0,
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, riga, transaction);
                        }

                        retValue += ($"{pns.Key.Anno}-{testata.N1REGI}");
                        retValue += Environment.NewLine;
                    }
                    else
                    {
                        transaction.Rollback();
                        ErrorHandler.Show($"Impossibile recuperare un nuovo numero di registrazione");
                        return null;
                    }
                }

                transaction.Commit();

                return retValue;

            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public Tuple<string, int, int>? Duplicate(string CompanyID, int Year, int Number, int AccountingYear, DateTime Date, CAUCONT SelectedCausal, bool IsRemove)
    {
        try
        {
            using var connection = GetOpenConnection();

            using (var transaction = connection.BeginTransaction())
            {
                int newID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, AccountingYear, Constants.PN, true);

                Tuple<string, int, int>? retValue = null;

                if (newID > 0)
                {
                    var first = Get(CompanyID, Year, Number);

                    if (first != null)
                    {
                        var testata = new PNTESTATA
                        {
                            N1SOCI = CompanyID,
                            N1ANNO = AccountingYear,
                            N1REGI = newID,
                            pncaus = SelectedCausal.caucod,
                            N1DARE = Date.Date,
                            pnvcod = first.pnvcod,
                            pnvdiv = first.pnvdiv,
                            N1FL01 = first.N1FL01,
                            N1docn = first.N1docn,
                            N1docd = first.N1docd,
                            N1rifn = first.N1rifn,
                            N1rifd = first.N1rifd,
                            N1FLCF = first.N1FLCF,
                            N1CLFO = first.N1CLFO,
                            N1VADO = first.N1VADO,
                            N1DIDO = first.N1DIDO,
                            N1DADI = first.N1DADI,
                            N1CADO = first.N1CADO,
                            N1IMDO = first.N1IMDO,
                            n1mrii = first.n1mrii,
                            N1TmpPN = first.N1TmpPN,
                        };
                        connection.Execute(INSERT_QUERY, testata, transaction);

                        var rows = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().GetList(CompanyID, Year, Number);

                        int currentRow = 0;
                        foreach (var pn in rows ?? new ObservableCollection<PNRIGHE>())
                        {
                            string? sign = pn.N1SEGN;

                            if (IsRemove)
                            {
                                if (sign == "D")
                                    sign = "A";
                                else
                                    sign = "D";
                            }

                            ++currentRow;

                            var riga = new PNRIGHE
                            {
                                N1SOCI = CompanyID,
                                N1ANNO = AccountingYear,
                                N1REGI = newID,
                                N1RIGA = pn.N1RIGA,
                                N1DOCU = pn.N1DOCU,
                                N1RIFE = pn.N1RIFE,
                                N1DADO = pn.N1DADO,
                                N1DARI = pn.N1DARI,
                                pngrup = pn.pngrup,
                                pncont = pn.pncont,
                                pnsott = pn.pnsott,
                                N1IMPO = pn.N1IMPO,
                                N1DESC = pn.N1DESC,
                                N1SEGN = sign,
                                n1clie = pn.n1clie,
                                N1CCCC = pn.N1CCCC,
                                N1CCCS = pn.N1CCCS,
                                N1CONP = pn.N1CONP,
                                N1COMM = pn.N1COMM,
                                N1DEST = pn.N1DEST,
                                N1GIRO = pn.N1GIRO,
                                N1STNO = pn.N1STNO,
                                N1STBO = string.Empty,
                                N1STMA = string.Empty,
                                N1CHIU = "A",
                                N1RIGB = 0,
                                N1DABB = Constants._GX_MIN_DATE,
                                N1DASM = Constants._GX_MIN_DATE,
                                N1DACC = Constants._GX_MIN_DATE,
                                N1FIVA = pn.N1FIVA,
                                N1BC01 = pn.N1BC01,
                                N1BC02 = pn.N1BC02,
                                N1BC03 = pn.N1BC03,
                                N1BC04 = pn.N1BC04,
                                N1BC05 = pn.N1BC05,
                                N1IMEU = pn.N1IMEU,
                                N1TIDO = pn.N1TIDO,
                                n1scad = pn.n1scad,
                                n1paga = pn.n1paga,
                                N1DRri = testata.N1DARE,
                                N1SCON = pn.N1SCON,
                                N1DIVI = pn.N1DIVI,
                                N1CAMB = pn.N1CAMB,
                                N1IMVA = pn.N1IMVA,
                                N1FLCO = pn.N1FLCO,
                                N1tmpPNR = pn.N1tmpPNR,
                                n1mese = pn.n1mese,
                            };
                            connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().INSERT_QUERY, riga, transaction);
                        }

                        transaction.Commit();

                        retValue = new Tuple<string, int, int>(CompanyID, AccountingYear, newID);
                    }
                    else
                    {
                        ErrorHandler.Show($"Registrazione non trovata - {CompanyID}|{Year}|{Number}");
                    }
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show($"Impossibile recuperare un nuovo numero di registrazione");
                    return null;
                }

                return retValue;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public string? Validate(PNTESTATA Model, bool IsInsert)
    {
        try
        {
            if (!Model.N1DARE.HasValue)
            {
                return $"La data di registrazione è obbligatoria";
            }
            var validYears = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(Model.N1SOCI)!;

            if (!validYears.Where(o => o.eseann == Model.N1ANNO).Any())
            {
                return $"Anno contabile non aperto - {Model.N1ANNO}";
            }

            var validStart = new DateTime(validYears.Where(w => w.eseann == Model.N1ANNO).First().eseann, validYears.Where(w => w.eseann == Model.N1ANNO).First().eseini ?? 1, 1);
            var validEnd = validStart.AddMonths(12);

            if (Model.N1DARE.Value > validEnd ||
                Model.N1DARE.Value < validStart)
            {
                return $"La data di registrazione non è valida";
            }
            if (string.IsNullOrWhiteSpace(Model.pncaus))
            {
                return $"La causale contabile è obbligatoria";
            }
            var causal = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Get(Model.pncaus)!;

            if (!causal.causolBool && Model.Amount == 0 && !string.IsNullOrWhiteSpace(causal.cauliv) && !causal.cauzer)
            {
                return $"L'importo totale del documento è obbligatorio";
            }

            if((causal.caucliBool || causal.cauforBool) && (string.IsNullOrEmpty(Model.N1docn) || !Model.N1docd.HasValue))
            {
                return $"Documento e data documento obbligatori per questa causale";
            }

            int testDoc = 0;
            if (causal.cauivaBool)
            {
                // check rate on customer/supplier
                if (IsInsert)
                {
                    if (Model.N1FLCF == "C")
                    {
                        var entityInfo = VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(Model.N1CLFO ?? 0);
                        if (string.IsNullOrWhiteSpace(entityInfo?.classo) || string.IsNullOrWhiteSpace(entityInfo?.classa))
                            return $"Il cliente scelto non ha un'aliquota di default impostata";
                    }
                    else
                    {
                        var entityInfo = VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(Model.N1SOCI, Model.N1CLFO ?? 0);
                        if (string.IsNullOrWhiteSpace(entityInfo?.foaass) || string.IsNullOrWhiteSpace(entityInfo?.foaali))
                            return $"Il fornitore scelto non ha un'aliquota di default impostata";
                    }
                }
                // check only numbers in document
                if (!int.TryParse(Model.N1docn, out testDoc))
                    return "La causale contabile prevede IVA quindi il numero documento deve contenere solo numeri e nessun carattere";
                // check protocol already used
                if (VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().CheckProtocolAlreadyUsed(Model.N1SOCI, Model.N1ANNO, Model.N1REGI, Model.N1docn, causal.cauliv ?? string.Empty))
                    return "Il numero di protocollo scelto è già utilizzato";
                if (!Model.ForceProtocol)
                {
                    // check valid protocol ID
                    var older = Get(Model.N1SOCI, Model.N1ANNO, Model.N1REGI);
                    if (older == null || older.N1docn != Model.N1docn || older.N1docd != Model.N1docd)
                    {
                        var lastProtocolInfo = VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetLastProtocolUsed(Model.N1SOCI, Model.N1DARE.Value, Model.N1REGI, causal.cauliv ?? string.Empty);
                        if (int.Parse(lastProtocolInfo?.Item1 ?? "0") > 0)
                        {
                            if ((int.Parse(lastProtocolInfo?.Item1 ?? "0") + 1) != int.Parse(Model.N1docn) || lastProtocolInfo?.Item2 > Model.N1docd)
                            { return $"Il numero protocollo deve essere consequenziale, senza salti e la data dello stesso non deve essere inferiore all'ultimo protocollo usato.\n\nUltimo protocollo: {lastProtocolInfo?.Item1} del {lastProtocolInfo?.Item2.ToString("dd/MM/yyyy")}"; }
                        }
                    }
                }
            }
            if ((string.IsNullOrWhiteSpace(Model.N1docn) || !Model.N1docd.HasValue) && !string.IsNullOrWhiteSpace(causal.cauliv))
            {
                return $"Gli estremi del documento sono obbligatori";
            }
            if ((string.IsNullOrWhiteSpace(Model.N1rifn) || !Model.N1rifd.HasValue) && !string.IsNullOrWhiteSpace(causal.cauliv))
            {
                return $"Gli estremi del riferimento sono obbligatori";
            }
            if (!string.IsNullOrWhiteSpace(Model.N1FLCF) && !Model.N1CLFO.HasValue && !Model.N1CLFO.HasValue)
            {
                return $"Se si seleziona un tipo codice occorre selezionare un cliente/fornitore altrimenti non va selezionato alcun codice";
            }
            if (!string.IsNullOrWhiteSpace(Model.N1FLCF) && !causal.caucliBool && !causal.cauforBool)
            {
                return "La causale non prevede nessun cliente ne' fornitore";
            }
            if (string.IsNullOrWhiteSpace(Model.N1FLCF) && causal.caucliBool)
            {
                return "La causale prevede un cliente";
            }
            if (string.IsNullOrWhiteSpace(Model.N1FLCF) && causal.cauforBool)
            {
                return "La causale prevede un fornitore";
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}
