using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.Services.Accounting.Assets;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Accounting.Assets;

public interface IACC_ASSETS_CARDSRepository
{
    ObservableCollection<ACC_ASSETS_CARDS>? GetList(string CompanyID, int? Year);

    List<GenericIntIDDescription>? GetDistinctYears(string besoci);

    ACC_ASSETS_CARDS? Get(string besoci, int beann4, string begrup, string becont, string besotc, int benin2, int beinv);

    bool Exists(string besoci, int beann4, string begrup, string becont, string besotc, int benin2, int beinv);

    #region Depreciation
    bool ComputeDepreciation(string CompanyID, string UserID, int Year, DateTime DateTo, string ComputeType, ESERCIZIO Exercise);
    #endregion

    #region Accounting
    bool Accounting(string CompanyID, string UserID, ESERCIZIO AccountingYear, DateTime DateTo, CAUCONT Causal);
    #endregion

    #region Update history
    bool UpdateHistory(string CompanyID, string UserID, int Year, string ComputeType, DateTime StartDate, DateTime EndDate);
    #endregion

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(ACC_ASSETS_CARDS Model);

    bool Update(ACC_ASSETS_CARDS Model);

    bool Delete(ACC_ASSETS_CARDS Model);

    string? Validate(ACC_ASSETS_CARDS Model, bool IsInsert);
    #endregion
}

public class ACC_ASSETS_CARDSRepository : RepositoryBase, IACC_ASSETS_CARDSRepository
{
    public ACC_ASSETS_CARDSRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ACC_ASSETS_CARDS>? GetList(string CompanyID, int? Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<ACC_ASSETS_CARDS>(
                $@"SELECT c.*, TRIM(t.TCDES) AS TypeDescription,TRIM(cat.jdescr) AS CategoryDescription,
                            (SELECT COUNT(*) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinv > 0) AS DetailsCount,
                            (SELECT ISNULL(SUM(ISNULL(i.bevaleu,0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '+') AS StartValuePlus,
                            (SELECT ISNULL(SUM(ISNULL(i.bevaleu,0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '-') AS StartValueMinus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoae, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '+') AS FiscalValuePlus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoae, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '-') AS FiscalValueMinus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoai, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '+') AS CivilValuePlus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoai, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '-') AS CivilValueMinus,
                            (SELECT ISNULL(SUM(ISNULL(i.bhanne, 0) + ISNULL(i.bhnnae,0)), 0) FROM ACC_ASSETS_DEP_HISTORY AS i WHERE i.bhsoci=c.besoci AND i.bhgrup=c.begrup AND i.bhcont=c.becont AND i.bhsotc=c.besotc AND i.bhinv2=c.benin2) AS FiscalHistoryValue,
                            (SELECT ISNULL(SUM(ISNULL(i.bcanne, 0)), 0) FROM ACC_ASSETS_DEP_CIV_HISTORY AS i WHERE i.bcsoci=c.besoci AND i.bcgrup=c.begrup AND i.bccont=c.becont AND i.bcsotc=c.besotc AND i.bcinv2=c.benin2) AS CivilHistoryValue
                        FROM ACC_ASSETS_CARDS AS c
                        LEFT JOIN ACC_ASSETS_TYPES AS t ON t.socij=c.besoci AND t.JTICE=c.betice
                        LEFT JOIN ACC_ASSETS_CATEGORIES AS cat ON cat.jcateg=c.becat
                        WHERE c.besoci=@cid AND c.beinv = 0 {(Year.HasValue ? " AND c.beann4 = @yea" : null)}
                        ORDER BY c.beann4 DESC,c.begrup,c.becont,c.besotc,c.benot",
                new { cid = CompanyID, yea = Year });

            return new ObservableCollection<ACC_ASSETS_CARDS>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public List<GenericIntIDDescription>? GetDistinctYears(string besoci)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<GenericIntIDDescription>(
                "SELECT DISTINCT beann4 AS ID, beann4 AS Description FROM ACC_ASSETS_CARDS WHERE besoci = @besoci",
                new { besoci = besoci }).ToList();
            list.Add(new GenericIntIDDescription() { ID = null, Description = "Tutti gli anni" });
            return list.OrderBy(o => o.ID).ToList();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ACC_ASSETS_CARDS? Get(string besoci, int beann4, string begrup, string becont, string besotc, int benin2, int beinv)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<ACC_ASSETS_CARDS>(
                @"SELECT c.*, CONCAT(begrup, ' ', becont, ' ', besotc, ' > ', s.P3DES1 ) AS FullAccountDescription,
                            (SELECT COUNT(*) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinv > 0) AS DetailsCount,
                            (SELECT ISNULL(SUM(ISNULL(i.bevaleu,0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '+') AS StartValuePlus,
                            (SELECT ISNULL(SUM(ISNULL(i.bevaleu,0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '-') AS StartValueMinus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoae, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '+') AS FiscalValuePlus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoae, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '-') AS FiscalValueMinus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoai, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '+') AS CivilValuePlus,
                            (SELECT ISNULL(SUM(ISNULL(i.befoai, 0)), 0) FROM ACC_ASSETS_CARDS AS i WHERE i.besoci=c.besoci AND i.beann4=c.beann4 AND i.begrup=c.begrup AND i.becont=c.becont AND i.besotc=c.besotc AND i.benin2=c.benin2 AND beinc = '-') AS CivilValueMinus,
                            (SELECT ISNULL(SUM(ISNULL(i.bhanne, 0) + ISNULL(i.bhnnae,0)), 0) FROM ACC_ASSETS_DEP_HISTORY AS i WHERE i.bhsoci=c.besoci AND i.bhgrup=c.begrup AND i.bhcont=c.becont AND i.bhsotc=c.besotc AND i.bhinv2=c.benin2) AS FiscalHistoryValue,
                            (SELECT ISNULL(SUM(ISNULL(i.bcanne, 0)), 0) FROM ACC_ASSETS_DEP_CIV_HISTORY AS i WHERE i.bcsoci=c.besoci AND i.bcgrup=c.begrup AND i.bccont=c.becont AND i.bcsotc=c.besotc AND i.bcinv2=c.benin2) AS CivilHistoryValue
                        FROM ACC_ASSETS_CARDS AS c
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP=c.begrup AND s.P2CONT=c.becont AND s.P3SOTC=c.besotc
                        WHERE c.besoci = @besoci AND c.beann4 = @beann4 AND c.begrup = @begrup AND c.becont = @becont AND c.besotc = @besotc AND c.benin2 = @benin2 AND c.beinv = @beinv",
                new { besoci = besoci, beann4 = beann4, begrup = begrup, becont = becont, besotc = besotc, benin2 = benin2, beinv = beinv })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string besoci, int beann4, string begrup, string becont, string besotc, int benin2, int beinv)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ACC_ASSETS_CARDS WHERE besoci = @besoci AND beann4 = @beann4 AND begrup = @begrup AND becont = @becont AND besotc = @besotc AND benin2 = @benin2 AND beinv = @beinv",
                new { besoci = besoci, beann4 = beann4, begrup = begrup, becont = becont, besotc = besotc, benin2 = benin2, beinv = beinv }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Depreciation
    public bool ComputeDepreciation(string CompanyID, string UserID, int Year, DateTime DateTo, string ComputeType, ESERCIZIO Exercise)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    // compute full days
                    if (Exercise.eseini.HasValue)
                    {
                        var startDate = new DateTime(Year, Exercise.eseini.Value, 1);

                        int days = (DateTo - startDate).Days;
                        // clear BEQUOI/BEANNOE
                        if (ComputeType == "C")
                            connection.Execute(@"UPDATE ACC_ASSETS_CARDS SET bequoi = 0 WHERE besoci=@cid AND beann4 <= @yea AND beda4 IS NULL AND beinc='+'", new { cid = CompanyID, yea = Year }, transaction);
                        else
                            connection.Execute(@"UPDATE ACC_ASSETS_CARDS SET beannoe = 0 WHERE besoci=@cid AND beann4 <= @yea AND beda4 IS NULL AND beinc='+'", new { cid = CompanyID, yea = Year }, transaction);
                        foreach (var cesp in connection.Query<ACC_ASSETS_CARDS>(@"
                            SELECT c.* ,
                                (SELECT ISNULL(SUM(ISNULL(i.bcanne, 0)), 0) FROM ACC_ASSETS_DEP_CIV_HISTORY AS i WHERE i.bcsoci=c.besoci AND i.bcgrup=c.begrup AND i.bccont=c.becont AND i.bcsotc=c.besotc AND i.bcinv2=c.benin2 AND i.bcinv=c.beinv) AS CivilHistoryValue,
                                (SELECT ISNULL(SUM(ISNULL(i.bhanne, 0) + ISNULL(i.bhnnae,0)), 0) FROM ACC_ASSETS_DEP_HISTORY AS i WHERE i.bhsoci=c.besoci AND i.bhgrup=c.begrup AND i.bhcont=c.becont AND i.bhsotc=c.besotc AND i.bhinv2=c.benin2 AND i.bhinv=c.beinv) AS FiscalHistoryValue
                                FROM ACC_ASSETS_CARDS AS c
                                WHERE c.besoci=@cid AND c.beann4 <= @yea AND c.beda4 IS NULL AND c.beinc='+'", new { cid = CompanyID, yea = Year }, transaction))
                        {
                            // compute specific days
                            int cespDays = 0;
                            if (cesp.bzdaco.HasValue && ((Exercise.eseini == 1 && cesp.bzdaco.Value.Year == Year) ||
                                (Exercise.eseini != 1 && cesp.bzdaco.Value >= startDate && cesp.bzdaco.Value <= DateTo.Date)))
                                cespDays = (DateTo - cesp.bzdaco.Value).Days;
                            // get cespite rate
                            var rate = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_RATESRepository>().Get(cesp.besoci, cesp.begrup, cesp.becont, cesp.besotc, Year);
                            if (rate == null)
                            {
                                transaction.Rollback();
                                ErrorHandler.Show($"Non trovata l'aliquota per il cespite {cesp.begrup} {cesp.becont} {cesp.besotc} anno {Year}, calcolo annullato integralmente");
                                return false;
                            }
                            if (ComputeType == "C")
                            {
                                #region Civilistic
                                // need residual
                                decimal residual = ((cesp.bevaleu ?? 0) - ((cesp.befoai ?? 0) + cesp.CivilHistoryValue));
                                // compute %
                                var depTotalValue = Math.Round(((cesp.bevaleu ?? 0) * (rate.trepai ?? 0) / 100), 2, MidpointRounding.AwayFromZero);
                                var depDaily = Math.Round(depTotalValue / days, 2, MidpointRounding.AwayFromZero);
                                decimal depRealValue = 0;
                                if (cespDays == 0)
                                    depRealValue = depTotalValue;
                                else
                                    depRealValue = Math.Round(depDaily * cespDays, 2, MidpointRounding.AwayFromZero);

                                var finalValue = (depRealValue < residual) ? depRealValue : residual;
                                connection.Execute(@"UPDATE ACC_ASSETS_CARDS SET bequoi = @tot , updatedUserID=@uid, updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                WHERE besoci=@besoci AND beann4=@beann4 AND begrup=@begrup AND becont=@becont AND besotc=@besotc AND benin2=@benin2 AND beinv=@beinv",
                                                    new { besoci = cesp.besoci, beann4 = cesp.beann4, begrup = cesp.begrup, becont = cesp.becont, besotc = cesp.besotc, benin2 = cesp.benin2, beinv = cesp.beinv, tot = finalValue, uid = UserID },
                                                    transaction);
                                #endregion
                            }
                            else
                            {
                                #region Fiscal
                                // need residual
                                decimal residual = (cesp.bevaleu ?? 0) - ((cesp.befoae ?? 0) + cesp.FiscalHistoryValue);
                                // compute %
                                var depTotalValue = Math.Round(((cesp.bevaleu ?? 0) * (rate.tpep1 ?? 0) / 100), 2, MidpointRounding.AwayFromZero);
                                var depDaily = Math.Round(depTotalValue / days, 2, MidpointRounding.AwayFromZero);
                                decimal depRealValue = 0;
                                if (cespDays == 0)
                                    depRealValue = depTotalValue;
                                else
                                    depRealValue = Math.Round(depDaily * cespDays, 2, MidpointRounding.AwayFromZero);

                                var finalValue = (depRealValue < residual) ? depRealValue : residual;
                                connection.Execute(@"UPDATE ACC_ASSETS_CARDS SET beannoe = @tot , updatedUserID=@uid, updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                WHERE besoci=@besoci AND beann4=@beann4 AND begrup=@begrup AND becont=@becont AND besotc=@besotc AND benin2=@benin2 AND beinv=@beinv",
                                                    new { besoci = cesp.besoci, beann4 = cesp.beann4, begrup = cesp.begrup, becont = cesp.becont, besotc = cesp.besotc, benin2 = cesp.benin2, beinv = cesp.beinv, tot = finalValue, uid = UserID },
                                                    transaction);
                                #endregion
                            }
                        }
                        transaction.Commit();
                        return true;
                    }

                    return false;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(exc.Message);
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
    #endregion

    #region Accounting
    public bool Accounting(string CompanyID, string UserID, ESERCIZIO AccountingYear, DateTime DateTo, CAUCONT Causal)
    {
        try
        {
            using var connection = GetOpenConnection();


            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                    var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();
                    // get registration number
                    var accountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, AccountingYear.eseann, Constants.PN, true);
                    if (accountingID <= 0)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show("Impossibile recuperare un nuovo numero di registrazione, contabilizzazione annullata integralmente");
                        return false;
                    }
                    #region PNTESTATA
                    PNTESTATA head = new PNTESTATA()
                    {
                        N1SOCI = CompanyID,
                        N1ANNO = AccountingYear.eseann,
                        N1REGI = accountingID,
                        pncaus = Causal.caucod,
                        N1DARE = DateTo,
                        N1docn = "1",
                        N1docd = DateTo,
                        N1rifn = "1",
                        N1rifd = DateTo,
                        pnvcod = "UIC",
                        pnvdiv = "EUR",
                        N1FL01 = string.Empty,
                        N1TmpPN = "N",
                        n1mrii = 0,
                        addedUserID = UserID
                    };
                    connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                    #endregion
                    int rowID = 1;
                    foreach (var cesp in connection.Query<ACC_ASSETS_CARDS>(@"
                            SELECT c.*
                                FROM ACC_ASSETS_CARDS AS c
                                WHERE c.besoci=@cid AND c.beda4 IS NULL AND c.bequoi > 0 AND c.beann4 <= @yea", new { cid = CompanyID, yea = AccountingYear.eseann }, transaction))
                    {

                        // get cespite TYPOLOGY
                        var typo = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_TYPOLOGIESRepository>().Get(CompanyID, cesp.begrup, cesp.becont, cesp.besotc);
                        if (typo == null)
                        {
                            transaction.Rollback();
                            ErrorHandler.Show($"Impossibile recuperare la tipologia per il cespite {cesp.begrup} {cesp.becont} {cesp.besotc}, contabilizzazione annullata integralmente");
                            return false;
                        }

                        #region PNRIGHE
                        var row1 = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowID++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = typo.segno2,
                            pngrup = typo.grupp2,
                            pncont = typo.conto2,
                            pnsott = typo.sotto2,
                            N1IMEU = cesp.bequoi,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, row1, transaction);

                        var row2 = new PNRIGHE()
                        {
                            N1SOCI = head.N1SOCI,
                            N1ANNO = head.N1ANNO,
                            N1REGI = head.N1REGI,
                            N1RIGA = rowID++,
                            N1DOCU = head.N1docn,
                            N1DADO = head.N1docd,
                            N1RIFE = head.N1rifn,
                            N1DARI = head.N1rifd,
                            N1SEGN = typo.segno1,
                            pngrup = typo.grupp1,
                            pncont = typo.cont1,
                            pnsott = typo.sotto1,
                            N1IMEU = cesp.bequoi,
                            N1CHIU = "A",
                            N1TIDO = "E",
                            N1DIVI = "EUR",
                            N1tmpPNR = "N",
                            N1DRri = head.N1docd
                        };
                        connection.Execute(pnRigheRepository.INSERT_QUERY, row2, transaction);
                        #endregion
                    }

                    transaction.Commit();
                    InfoHandler.Show($"Contabilizzazione eseguita correttamente, registrazione {accountingID}");
                    return true;
                }
                catch (Exception exc)
                {
                    transaction.Rollback();
                    ErrorHandler.Show(exc.Message);
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
    #endregion

    #region Update history
    public bool UpdateHistory(string CompanyID, string UserID, int Year, string ComputeType, DateTime StartDate, DateTime EndDate)
    {
        try
        {
            using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var cesp in connection.Query<ACC_ASSETS_CARDS>($@"
                            SELECT c.*
                                FROM ACC_ASSETS_CARDS AS c
                                WHERE c.besoci=@cid AND c.beda4 IS NULL AND c.beann4 <= @yea {(ComputeType == "C" ? " AND c.bequoi > 0 " : " AND c.beannoe > 0 ")}", new { cid = CompanyID, yea = Year }, transaction))
                        {
                            // get cespite rate
                            var rate = VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_RATESRepository>().Get(cesp.besoci, cesp.begrup, cesp.becont, cesp.besotc, Year);
                            if (rate == null)
                            {
                                transaction.Rollback();
                                ErrorHandler.Show($"Non trovata l'aliquota per il cespite {cesp.begrup} {cesp.becont} {cesp.besotc} anno {Year}, aggiornamento dei cespiti annullato integralmente");
                                return false;
                            }
                            if (ComputeType == "C")
                            {
                                var newHistory = new ACC_ASSETS_DEP_CIV_HISTORY()
                                {
                                    bcsoci = cesp.besoci,
                                    bcanco = Year,
                                    bcann4 = cesp.beann4,
                                    bcgrup = cesp.begrup,
                                    bccont = cesp.becont,
                                    bcsotc = cesp.besotc,
                                    bcinv2 = cesp.benin2,
                                    bcinv = cesp.beinv,
                                    bcval = cesp.bevaleu,
                                    bcpea = rate.trepai,
                                    bcanne = cesp.bequoi,
                                    bcdaip = StartDate,
                                    bcdafp = EndDate,
                                    addedUserID = UserID
                                };
                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_DEP_CIV_HISTORYRepository>().INSERT_QUERY, newHistory, transaction);
                                // update source
                                connection.Execute(@"UPDATE ACC_ASSETS_CARDS SET bequoi = 0 , befoai = @tot, updatedUserID=@uid, updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                WHERE besoci=@besoci AND beann4=@beann4 AND begrup=@begrup AND becont=@becont AND besotc=@besotc AND benin2=@benin2 AND beinv=@beinv",
                                                    new { besoci = cesp.besoci, beann4 = cesp.beann4, begrup = cesp.begrup, becont = cesp.becont, besotc = cesp.besotc, benin2 = cesp.benin2, beinv = cesp.beinv, tot = cesp.bequoi, uid = UserID },
                                                    transaction);
                            }
                            else
                            {
                                var newHistory = new ACC_ASSETS_DEP_HISTORY()
                                {
                                    bhsoci = cesp.besoci,
                                    bhanco = Year,
                                    bhann4 = cesp.beann4,
                                    bhgrup = cesp.begrup,
                                    bhcont = cesp.becont,
                                    bhsotc = cesp.besotc,
                                    bhinv2 = cesp.benin2,
                                    bhinv = cesp.beinv,
                                    bhval = cesp.bevaleu,
                                    bhpea = rate.tpep1,
                                    bhdaip = StartDate,
                                    bhdafp = EndDate,
                                    bhanne = cesp.beannoe,
                                    addedUserID = UserID
                                };
                                connection.Execute(VulpesServiceProvider.Provider.GetRequiredService<IACC_ASSETS_DEP_HISTORYRepository>().INSERT_QUERY, newHistory, transaction);
                                // update source
                                connection.Execute(@"UPDATE ACC_ASSETS_CARDS SET beannoe = 0 , befoae = @tot, updatedUserID=@uid, updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
                                                WHERE besoci=@besoci AND beann4=@beann4 AND begrup=@begrup AND becont=@becont AND besotc=@besotc AND benin2=@benin2 AND beinv=@beinv",
                                                    new { besoci = cesp.besoci, beann4 = cesp.beann4, begrup = cesp.begrup, becont = cesp.becont, besotc = cesp.besotc, benin2 = cesp.benin2, beinv = cesp.beinv, tot = cesp.beannoe, uid = UserID },
                                                    transaction);
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception exc)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(exc.Message);
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
    #endregion

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO ACC_ASSETS_CARDS (besoci,beann4,begrup,becont,besotc,benin2,beinv,befil1,beinc,beubi,becdf,beda2,benfa,besosp,beda3,befon,bepea,befoa,bevar,beanno,benot,beper,beanna,bepac,beapp,bepcc,betice,beqta,beplus,bepein,befoai,bevari,bequoi,bequan,beda4,berad,bequen,bequqt,benrad,beprfo,bztras,bzggus,bzdaco,bzdote,bevalf,bzvalc,bzdivi,befoan,becape,beanpe,beusat,bemaxan,bequane,beannoe,beannae,bevaleu,befoae,fondoa,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,becat) OUTPUT INSERTED.rv VALUES(@besoci,@beann4,@begrup,@becont,@besotc,@benin2,@beinv,@befil1,@beinc,@beubi,@becdf,@beda2,@benfa,@besosp,@beda3,@befon,@bepea,@befoa,@bevar,@beanno,@benot,@beper,@beanna,@bepac,@beapp,@bepcc,@betice,@beqta,@beplus,@bepein,@befoai,@bevari,@bequoi,@bequan,@beda4,@berad,@bequen,@bequqt,@benrad,@beprfo,@bztras,@bzggus,@bzdaco,@bzdote,@bevalf,@bzvalc,@bzdivi,@befoan,@becape,@beanpe,@beusat,@bemaxan,@bequane,@beannoe,@beannae,@bevaleu,@befoae,@fondoa,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@becat)";
    public string UPDATE_QUERY => "UPDATE ACC_ASSETS_CARDS SET besoci = @besoci,beann4 = @beann4,begrup = @begrup,becont = @becont,besotc = @besotc,benin2 = @benin2,beinv = @beinv,befil1 = @befil1,beinc = @beinc,beubi = @beubi,becdf = @becdf,beda2 = @beda2,benfa = @benfa,besosp = @besosp,beda3 = @beda3,befon = @befon,bepea = @bepea,befoa = @befoa,bevar = @bevar,beanno = @beanno,benot = @benot,beper = @beper,beanna = @beanna,bepac = @bepac,beapp = @beapp,bepcc = @bepcc,betice = @betice,beqta = @beqta,beplus = @beplus,bepein = @bepein,befoai = @befoai,bevari = @bevari,bequoi = @bequoi,bequan = @bequan,beda4 = @beda4,berad = @berad,bequen = @bequen,bequqt = @bequqt,benrad = @benrad,beprfo = @beprfo,bztras = @bztras,bzggus = @bzggus,bzdaco = @bzdaco,bzdote = @bzdote,bevalf = @bevalf,bzvalc = @bzvalc,bzdivi = @bzdivi,befoan = @befoan,becape = @becape,beanpe = @beanpe,beusat = @beusat,bemaxan = @bemaxan,bequane = @bequane,beannoe = @beannoe,beannae = @beannae,bevaleu = @bevaleu,befoae = @befoae,fondoa = @fondoa,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,becat = @becat OUTPUT INSERTED.rv WHERE besoci = @besoci AND beann4 = @beann4 AND begrup = @begrup AND becont = @becont AND besotc = @besotc AND benin2 = @benin2 AND beinv = @beinv AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM ACC_ASSETS_CARDS OUTPUT DELETED.rv WHERE besoci = @besoci AND beann4 = @beann4 AND begrup = @begrup AND becont = @becont AND besotc = @besotc AND benin2 = @benin2 AND beinv = @beinv AND rv = @rv";
    public bool Insert(ACC_ASSETS_CARDS Model)
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

    public bool Update(ACC_ASSETS_CARDS Model)
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

    public bool Delete(ACC_ASSETS_CARDS Model)
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

    public string? Validate(ACC_ASSETS_CARDS Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.besoci) && IsInsert && !Exists(Model.besoci, Model.beann4, Model.begrup, Model.becont, Model.besotc, Model.benin2, Model.beinv)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.betice))
                {
                    return null;
                }
                else
                { return "Il tipo cespite è obbligatorio"; }
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