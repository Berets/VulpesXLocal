

using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Xml.Linq;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Models.Accounting;

namespace VulpesX.DAL.Accounting;

public interface ITCOMLIQIVARepository
{

    ObservableCollection<TCOMLIQIVA>? GetList(string CompanyID, int Year);

    ObservableCollection<TCOMLIQIVA>? GetListDetails(string CompanyID, int Year, int Month);

    ObservableCollection<TCOMLIQIVA>? GetListDetailsPeriod(string CompanyID, int Year, int Month);

    TCOMLIQIVA? Get(string CLICodSOC, int CLIAnnLiq, int CLIPerLiq, string CLIVocLiq);

    bool ComputeOrdinary(string CompanyID, int Year, string QuarterID, string ComputeType, string UserID);

    bool ComputeIva(string CompanyID, int Year, string QuarterID, string ComputeType, string UserID);

    string? GenerateXML(LIPEXMLViewModel Options);

    bool Exists(string CLICodSOC, int CLIAnnLiq, int CLIPerLiq);

    Tuple<int, int>? ComputeFirstLastPeriod(int Month);

    #region CRUD
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(TCOMLIQIVA Model);

    bool Update(ObservableCollection<TCOMLIQIVA> Model);

    bool Delete(TCOMLIQIVA Model);

    string? Validate(TCOMLIQIVA Model, bool IsInsert);
    #endregion
}

public class TCOMLIQIVARepository : RepositoryBase, ITCOMLIQIVARepository
{
    public TCOMLIQIVARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TCOMLIQIVA>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCOMLIQIVA>(
                @"SELECT * FROM TCOMLIQIVA
                        WHERE CLICodSOC = @cid AND CLIAnnLiq = @yea AND TRIM(CLIVocLiq) = 'VP14'
                        ORDER BY CLIPerLiq DESC",
                new { cid = CompanyID, yea = Year });

            return new ObservableCollection<TCOMLIQIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TCOMLIQIVA>? GetListDetails(string CompanyID, int Year, int Month)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCOMLIQIVA, TCODLIQIVA, TCOMLIQIVA>(
                @"select li.*, co.* from TCOMLIQIVA as li
                        inner join TCODLIQIVA as co on co.CVISoc=li.CLICodSOC and co.CVICod = li.CLIVocLiq
                        where li.CLICodSOC=@cid and li.cliannliq = @yea and CLIPerLiq = @mon
                        order by co.CVISeq",
                (liq, cod) => { liq.ItemInfo = cod; return liq; },
                new { cid = CompanyID, yea = Year, mon = Month }, splitOn: "CVISoc");

            return new ObservableCollection<TCOMLIQIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TCOMLIQIVA>? GetListDetailsPeriod(string CompanyID, int Year, int Month)
    {
        try
        {
            using var connection = GetOpenConnection();



            var fl = ComputeFirstLastPeriod(Month);

            var list = connection.Query<TCOMLIQIVA>(
                @"select * FROM TCOMLIQIVA where CLICodSOC=@cid and cliannliq = @yea and CLIPerLiq >= @fmon and CLIPerLiq <= @lmon;",
                new { cid = CompanyID, yea = Year, fmon = fl?.Item1, lmon = fl?.Item2 });

            return new ObservableCollection<TCOMLIQIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TCOMLIQIVA? Get(string CLICodSOC, int CLIAnnLiq, int CLIPerLiq, string CLIVocLiq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TCOMLIQIVA>(
                "SELECT * FROM TCOMLIQIVA WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq AND CLIVocLiq = @CLIVocLiq",
                new { CLICodSOC = CLICodSOC, CLIAnnLiq = CLIAnnLiq, CLIPerLiq = CLIPerLiq, CLIVocLiq = CLIVocLiq })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool ComputeOrdinary(string CompanyID, int Year, string QuarterID, string ComputeType, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            decimal vp2 = 0;
            decimal vp3 = 0;
            decimal vp4 = 0;
            decimal vp5 = 0;
            decimal vp6 = 0;
            decimal vp7 = 0;
            decimal vp8 = 0;
            decimal vp9 = 0;
            decimal vp14 = 0;

            if (ComputeType == "M")
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                for (int i = startMonth; i < startMonth + 3; i++)
                {
                    // compute previous month
                    int previousYear = 0;
                    int previousMonth = 0;
                    if (i == 1)
                    {
                        previousYear = Year - 1;
                        previousMonth = 12;
                    }
                    else
                    {
                        previousYear = Year;
                        previousMonth = i - 1;
                    }

                    var multi = connection.QueryMultiple(
                        @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp2;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp4;
                              SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp3;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp5;
                              SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                        new { cid = CompanyID, yea = Year, mon = i, pyea = previousYear, pmon = previousMonth });

                    vp2 = multi.ReadFirst<decimal>();
                    vp4 = multi.ReadFirst<decimal>();
                    vp3 = multi.ReadFirst<decimal>();
                    vp5 = multi.ReadFirst<decimal>();
                    vp6 = vp4 - vp5;
                    var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                    if (vp7vp8 != null)
                    {
                        vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                        if (previousYear < Year)
                            vp9 = vp7vp8.cre;
                        else
                            vp8 = vp7vp8.cre;
                    }
                    vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                        {
                            var newVP = new TCOMLIQIVA()
                            {
                                CLICodSOC = CompanyID,
                                CLIAnnLiq = Year,
                                CLIPerLiq = i,
                                CLIVocLiq = vpl.CVICod,
                                CLITipLiq = "M",
                                CLIImpLiqD = 0,
                                CLIImpLiqC = 0,
                                addedUserID = UserID
                            };

                            switch (vpl.CVICod)
                            {
                                case "VP2":
                                    newVP.CLIImpLiqD = vp2;
                                    break;
                                case "VP3":
                                    newVP.CLIImpLiqC = vp3;
                                    break;
                                case "VP4":
                                    newVP.CLIImpLiqD = vp4;
                                    break;
                                case "VP5":
                                    newVP.CLIImpLiqC = vp5;
                                    break;
                                case "VP6":
                                    if (vp6 < 0)
                                        newVP.CLIImpLiqC = vp6 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp6;
                                    break;
                                case "VP7":
                                    newVP.CLIImpLiqD = vp7;
                                    break;
                                case "VP8":
                                    newVP.CLIImpLiqC = vp8;
                                    break;
                                case "VP9":
                                    newVP.CLIImpLiqC = vp9;
                                    break;
                                case "VP14":
                                    if (vp14 < 0)
                                        newVP.CLIImpLiqC = vp14 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp14;
                                    break;
                            }
                            connection.Execute(INSERT_QUERY, newVP, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    vp8 = 0;
                    vp9 = 0;
                }

                return true;
            }
            else
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                var startQuarter = QuarterID == "I" ? 1 : (QuarterID == "II" ? 2 : (QuarterID == "III" ? 3 : 4));
                // compute previous month
                int previousYear = 0;
                int previousMonth = 0;
                if (startMonth == 1)
                {
                    previousYear = Year - 1;
                    previousMonth = 12;
                }
                else
                {
                    previousYear = Year;
                    previousMonth = startMonth - 1;
                }

                var multi = connection.QueryMultiple(
                    @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp2;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp4;
                          SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp3;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp5;
                          SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                    new { cid = CompanyID, yea = Year, smon = startMonth, emon = startMonth + 2, pyea = previousYear, pmon = previousMonth });

                vp2 = multi.ReadFirst<decimal>();
                vp4 = multi.ReadFirst<decimal>();
                vp3 = multi.ReadFirst<decimal>();
                vp5 = multi.ReadFirst<decimal>();
                vp6 = vp4 - vp5;

                var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                if (vp7vp8 != null)
                {
                    vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                    if (previousYear < Year)
                        vp9 = vp7vp8.cre;
                    else
                        vp8 = vp7vp8.cre;
                }
                vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                    {
                        var newVP = new TCOMLIQIVA()
                        {
                            CLICodSOC = CompanyID,
                            CLIAnnLiq = Year,
                            CLIPerLiq = startQuarter,
                            CLIVocLiq = vpl.CVICod,
                            CLITipLiq = "T",
                            CLIImpLiqD = 0,
                            CLIImpLiqC = 0,
                            addedUserID = UserID
                        };

                        switch (vpl.CVICod)
                        {
                            case "VP2":
                                newVP.CLIImpLiqD = vp2;
                                break;
                            case "VP3":
                                newVP.CLIImpLiqC = vp3;
                                break;
                            case "VP4":
                                newVP.CLIImpLiqD = vp4;
                                break;
                            case "VP5":
                                newVP.CLIImpLiqC = vp5;
                                break;
                            case "VP6":
                                if (vp6 < 0)
                                    newVP.CLIImpLiqC = vp6 * -1;
                                else
                                    newVP.CLIImpLiqD = vp6;
                                break;
                            case "VP7":
                                newVP.CLIImpLiqD = vp7;
                                break;
                            case "VP8":
                                newVP.CLIImpLiqC = vp8;
                                break;
                            case "VP9":
                                newVP.CLIImpLiqC = vp9;
                                break;
                            case "VP14":
                                if (vp14 < 0)
                                    newVP.CLIImpLiqC = vp14 * -1;
                                else
                                    newVP.CLIImpLiqD = vp14;
                                break;
                        }
                        connection.Execute(INSERT_QUERY, newVP, transaction);
                    }

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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool ComputeIva(string CompanyID, int Year, string QuarterID, string ComputeType, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            decimal vp2 = 0;
            decimal vp3 = 0;
            decimal vp4 = 0;
            decimal vp5 = 0;
            decimal vp6 = 0;
            decimal vp7 = 0;
            decimal vp8 = 0;
            decimal vp9 = 0;
            decimal vp14 = 0;

            if (ComputeType == "M")
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                for (int i = startMonth; i < startMonth + 3; i++)
                {
                    // compute previous month
                    int previousYear = 0;
                    int previousMonth = 0;
                    if (i == 1)
                    {
                        previousYear = Year - 1;
                        previousMonth = 12;
                    }
                    else
                    {
                        previousYear = Year;
                        previousMonth = i - 1;
                    }

                    var multi = connection.QueryMultiple(
                        @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp2;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '-'), 0) AS vp4;
                              SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp3;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '-'), 0) AS vp5;
                              SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                        new { cid = CompanyID, yea = Year, mon = i, pyea = previousYear, pmon = previousMonth });

                    vp2 = multi.ReadFirst<decimal>();
                    vp4 = multi.ReadFirst<decimal>();
                    vp3 = multi.ReadFirst<decimal>();
                    vp5 = multi.ReadFirst<decimal>();
                    vp6 = vp4 - vp5;
                    var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                    if (vp7vp8 != null)
                    {
                        vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                        if (previousYear < Year)
                            vp9 = vp7vp8.cre;
                        else
                            vp8 = vp7vp8.cre;
                    }
                    vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                        {
                            var newVP = new TCOMLIQIVA()
                            {
                                CLICodSOC = CompanyID,
                                CLIAnnLiq = Year,
                                CLIPerLiq = i,
                                CLIVocLiq = vpl.CVICod,
                                CLITipLiq = "M",
                                CLIImpLiqD = 0,
                                CLIImpLiqC = 0,
                                addedUserID = UserID
                            };

                            switch (vpl.CVICod)
                            {
                                case "VP2":
                                    newVP.CLIImpLiqD = vp2;
                                    break;
                                case "VP3":
                                    newVP.CLIImpLiqC = vp3;
                                    break;
                                case "VP4":
                                    newVP.CLIImpLiqD = vp4;
                                    break;
                                case "VP5":
                                    newVP.CLIImpLiqC = vp5;
                                    break;
                                case "VP6":
                                    if (vp6 < 0)
                                        newVP.CLIImpLiqC = vp6 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp6;
                                    break;
                                case "VP7":
                                    newVP.CLIImpLiqD = vp7;
                                    break;
                                case "VP8":
                                    newVP.CLIImpLiqC = vp8;
                                    break;
                                case "VP9":
                                    newVP.CLIImpLiqC = vp9;
                                    break;
                                case "VP14":
                                    if (vp14 < 0)
                                        newVP.CLIImpLiqC = vp14 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp14;
                                    break;
                            }
                            connection.Execute(INSERT_QUERY, newVP, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                return true;
            }
            else
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                var startQuarter = QuarterID == "I" ? 1 : (QuarterID == "II" ? 2 : (QuarterID == "III" ? 3 : 4));
                // compute previous month
                int previousYear = 0;
                int previousMonth = 0;
                if (startMonth == 1)
                {
                    previousYear = Year - 1;
                    previousMonth = 12;
                }
                else
                {
                    previousYear = Year;
                    previousMonth = startMonth - 1;
                }

                var multi = connection.QueryMultiple(
                    @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp2;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '-'), 0) AS vp4;
                          SELECT ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp3;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PNIVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '-'), 0) AS vp5;
                          SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                    new { cid = CompanyID, yea = Year, smon = startMonth, emon = startMonth + 2, pyea = previousYear, pmon = previousMonth });

                vp2 = multi.ReadFirst<decimal>();
                vp4 = multi.ReadFirst<decimal>();
                vp3 = multi.ReadFirst<decimal>();
                vp5 = multi.ReadFirst<decimal>();
                vp6 = vp4 - vp5;

                var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                if (vp7vp8 != null)
                {
                    vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                    if (previousYear < Year)
                        vp9 = vp7vp8.cre;
                    else
                        vp8 = vp7vp8.cre;
                }
                vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                    {
                        var newVP = new TCOMLIQIVA()
                        {
                            CLICodSOC = CompanyID,
                            CLIAnnLiq = Year,
                            CLIPerLiq = startQuarter,
                            CLIVocLiq = vpl.CVICod,
                            CLITipLiq = "T",
                            CLIImpLiqD = 0,
                            CLIImpLiqC = 0,
                            addedUserID = UserID
                        };

                        switch (vpl.CVICod)
                        {
                            case "VP2":
                                newVP.CLIImpLiqD = vp2;
                                break;
                            case "VP3":
                                newVP.CLIImpLiqC = vp3;
                                break;
                            case "VP4":
                                newVP.CLIImpLiqD = vp4;
                                break;
                            case "VP5":
                                newVP.CLIImpLiqC = vp5;
                                break;
                            case "VP6":
                                if (vp6 < 0)
                                    newVP.CLIImpLiqC = vp6 * -1;
                                else
                                    newVP.CLIImpLiqD = vp6;
                                break;
                            case "VP7":
                                newVP.CLIImpLiqD = vp7;
                                break;
                            case "VP8":
                                newVP.CLIImpLiqC = vp8;
                                break;
                            case "VP9":
                                newVP.CLIImpLiqC = vp9;
                                break;
                            case "VP14":
                                if (vp14 < 0)
                                    newVP.CLIImpLiqC = vp14 * -1;
                                else
                                    newVP.CLIImpLiqD = vp14;
                                break;
                        }
                        connection.Execute(INSERT_QUERY, newVP, transaction);
                    }

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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? GenerateXML(LIPEXMLViewModel Options)
    {
        try
        {
            CultureInfo culture = new CultureInfo("it-IT");
            XNamespace iv = "urn:www.agenziaentrate.gov.it:specificheTecniche:sco:ivp";
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";

            var fl = ComputeFirstLastPeriod(Options.Month);
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            #region Generate file name
            string fullPath = $"{Path.GetTempPath()}IT{Options.VATID}_LI_{Options.Year.ToString("F0")}{fl?.Item1.ToString("F0")}.xml";
            #endregion

            #region Compute needed LIPEs
            var items = new List<TCOMLIQIVA>();

            if (Options.LIPEType == "T")
            {
                items.AddRange(GetListDetails(Options.CompanyID, Options.Year, Options.Month) ?? new ObservableCollection<TCOMLIQIVA>());
            }
            else
            {
                items.AddRange(GetListDetailsPeriod(Options.CompanyID, Options.Year, Options.Month) ?? new ObservableCollection<TCOMLIQIVA>());
            }
            #endregion

            var root = new XElement(iv + "Fornitura",
                new XAttribute(XNamespace.Xmlns + "iv", iv.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "ds", ds.NamespaceName));

            var header = new XElement(iv + "Intestazione",
                new XElement(iv + "CodiceFornitura", "IVP18"),
                new XElement(iv + "CodiceFiscaleDichiarante", Options.FiscalIDSender?.Trim().ToUpper()),
                new XElement(iv + "CodiceCarica", Options.TitleID));
            root.Add(header);

            XElement[]? broker = null;
            if (!string.IsNullOrWhiteSpace(Options.BrokerFiscalID))
            {
                broker = new XElement[]{
                    new XElement(iv + "CFIntermediario", Options.BrokerFiscalID),
                    new XElement(iv + "ImpegnoPresentazione", Options.PresentationID),
                    new XElement(iv + "DataImpegno", now.ToString("ddMMyyyy")),
                    new XElement(iv + "FirmaIntermediario", "1")
                };
            }

            var title = new XElement(iv + "Frontespizio",
                new XElement(iv + "CodiceFiscale", Options.FISCALID?.Trim().ToUpper()),
                new XElement(iv + "AnnoImposta", $"{Options.Year.ToString("F0")}"),
                new XElement(iv + "PartitaIVA", Options.VATID?.Trim().ToUpper()),
                new XElement(iv + "CFDichiarante", Options.FiscalIDSender?.Trim().ToUpper()),
                new XElement(iv + "CodiceCaricaDichiarante", Options.TitleID),
                new XElement(iv + "FirmaDichiarazione", "1"),
                broker);

            var data = new XElement(iv + "DatiContabili");
            int modpro = 1;
            var isTri = items.GroupBy(g => g.CLIPerLiq).Count() == 1;
            foreach (var module in items.GroupBy(g => g.CLIPerLiq, (key, items) => new { key = key, items = items }).OrderBy(o => o.key))
            {
                var vp6 = module.items.Where(w => w.CLIVocLiq == "VP6").First();
                var vp7 = module.items.Where(w => w.CLIVocLiq == "VP7").First();
                var vp8 = module.items.Where(w => w.CLIVocLiq == "VP8").First();
                var vp9 = module.items.Where(w => w.CLIVocLiq == "VP9").First();
                var vp10 = module.items.Where(w => w.CLIVocLiq == "VP10").First();
                var vp11 = module.items.Where(w => w.CLIVocLiq == "VP11").First();
                var vp12 = module.items.Where(w => w.CLIVocLiq == "VP12").First();
                var vp14 = module.items.Where(w => w.CLIVocLiq == "VP14").First();

                data.Add(new XElement(iv + "Modulo",
                    new XElement(iv + "NumeroModulo", modpro++),
                    new XElement(iv + (isTri ? "Trimestre" : "Mese"), module.key),
                    new XElement(iv + "TotaleOperazioniAttive", module.items.Where(w => w.CLIVocLiq == "VP2").First().CLIImpLiqD.ToString("F2", culture)),
                    new XElement(iv + "TotaleOperazioniPassive", module.items.Where(w => w.CLIVocLiq == "VP3").First().CLIImpLiqC.ToString("F2", culture)),
                    new XElement(iv + "IvaEsigibile", module.items.Where(w => w.CLIVocLiq == "VP4").First().CLIImpLiqD.ToString("F2", culture)),
                    new XElement(iv + "IvaDetratta", module.items.Where(w => w.CLIVocLiq == "VP5").First().CLIImpLiqC.ToString("F2", culture)),
                    new XElement(iv + $"{(vp6.CLIImpLiqD == vp6.CLIImpLiqC || vp6.CLIImpLiqD > 0 ? "IvaDovuta" : "IvaCredito")}", $"{(vp6.CLIImpLiqD == vp6.CLIImpLiqC ? "0,00" : (vp6.CLIImpLiqD > 0 ? vp6.CLIImpLiqD.ToString("F2", culture) : vp6.CLIImpLiqC.ToString("F2", culture)))}"),
                    (vp7.CLIImpLiqD > 0 ? new XElement(iv + "DebitoPrecedente", vp7.CLIImpLiqD) : null),
                    (vp8.CLIImpLiqC > 0 ? new XElement(iv + "CreditoPeriodoPrecedente", vp8.CLIImpLiqC.ToString("F2", culture)) : null),
                    (vp9.CLIImpLiqC > 0 ? new XElement(iv + "CreditoAnnoPrecedente", vp9.CLIImpLiqC) : null),
                    (vp10.CLIImpLiqC > 0 ? new XElement(iv + "VersamentiAuto", vp10.CLIImpLiqC) : null),
                    (vp11.CLIImpLiqC > 0 ? new XElement(iv + "CreditiImposta", vp11.CLIImpLiqC) : null),
                    (vp12.CLIImpLiqD > 0 ? new XElement(iv + "InteressiDovuti", vp12.CLIImpLiqD) : null),
                    new XElement(iv + $"{(vp14.CLIImpLiqD == vp14.CLIImpLiqC || vp14.CLIImpLiqD > 0 ? "ImportoDaVersare" : "ImportoACredito")}", $"{(vp14.CLIImpLiqD == vp14.CLIImpLiqC ? "0,00" : (vp14.CLIImpLiqD > 0 ? vp14.CLIImpLiqD.ToString("F2", culture) : vp14.CLIImpLiqC.ToString("F2", culture)))}")));
            }

            var message = new XElement(iv + "Comunicazione",
                new XAttribute("identificativo", "00001"),
                title,
                data);
            root.Add(message);

            root.Save(fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CLICodSOC, int CLIAnnLiq, int CLIPerLiq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TCOMLIQIVA WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq",
                new { CLICodSOC = CLICodSOC, CLIAnnLiq = CLIAnnLiq, CLIPerLiq = CLIPerLiq }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public Tuple<int, int>? ComputeFirstLastPeriod(int Month)
    {
        if (Month >= 1 && Month <= 3)
        {
            return new Tuple<int, int>(1, 3);
        }
        else if (Month >= 4 && Month <= 6)
        {
            return new Tuple<int, int>(4, 6);
        }
        else if (Month >= 7 && Month <= 9)
        {
            return new Tuple<int, int>(7, 9);
        }
        else if (Month >= 10 && Month <= 12)
        {
            return new Tuple<int, int>(10, 12);
        }
        else
        { return null; }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TCOMLIQIVA (CLICodSOC,CLIAnnLiq,CLIPerLiq,CLIVocLiq,CLITipLiq,CLIImpLiqD,CLIImpLiqC,addedUserID,added,updated,canceled,updatedUserID,canceledUserID,canceledNote) OUTPUT INSERTED.rv VALUES(@CLICodSOC,@CLIAnnLiq,@CLIPerLiq,@CLIVocLiq,@CLITipLiq,@CLIImpLiqD,@CLIImpLiqC,@addedUserID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@updatedUserID,@canceledUserID,@canceledNote)";
    public string UPDATE_QUERY => "UPDATE TCOMLIQIVA SET CLICodSOC = @CLICodSOC,CLIAnnLiq = @CLIAnnLiq,CLIPerLiq = @CLIPerLiq,CLIVocLiq = @CLIVocLiq,CLITipLiq = @CLITipLiq,CLIImpLiqD = @CLIImpLiqD,CLIImpLiqC = @CLIImpLiqC,addedUserID = @addedUserID,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq AND CLIVocLiq = @CLIVocLiq AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TCOMLIQIVA WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq";
    public bool Insert(TCOMLIQIVA Model)
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

    public bool Update(ObservableCollection<TCOMLIQIVA> Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                int updated = 0;
                foreach (var item in Model)
                {
                    updated += (int)connection.Execute(UPDATE_QUERY, item, transaction);
                }

                if (updated == Model.Count)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
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

    public bool Delete(TCOMLIQIVA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            connection.Execute(DELETE_QUERY, Model);
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(TCOMLIQIVA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.CLICodSOC) && IsInsert && !Exists(Model.CLICodSOC, Model.CLIAnnLiq, Model.CLIPerLiq)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.CLITipLiq))
                {
                    return null;
                }
                else
                { return "Il tipo liquidazione è obbligatorio"; }
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

public class TCOMLIQIVAUfpRepository : RepositoryBase, ITCOMLIQIVARepository
{
    public TCOMLIQIVAUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TCOMLIQIVA>? GetList(string CompanyID, int Year)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCOMLIQIVA>(
                @"SELECT * FROM TCOMLIQIVA
                        WHERE CLICodSOC = @cid AND CLIAnnLiq = @yea AND TRIM(CLIVocLiq) = 'VP14'
                        ORDER BY CLIPerLiq DESC",
                new { cid = CompanyID, yea = Year });

            return new ObservableCollection<TCOMLIQIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TCOMLIQIVA>? GetListDetails(string CompanyID, int Year, int Month)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TCOMLIQIVA, TCODLIQIVA, TCOMLIQIVA>(
                @"select li.*, co.* from TCOMLIQIVA as li
                        inner join TCODLIQIVA as co on co.CVISoc=li.CLICodSOC and co.CVICod = li.CLIVocLiq
                        where li.CLICodSOC=@cid and li.cliannliq = @yea and CLIPerLiq = @mon
                        order by co.CVISeq",
                (liq, cod) => { liq.ItemInfo = cod; return liq; },
                new { cid = CompanyID, yea = Year, mon = Month }, splitOn: "CVISoc");

            return new ObservableCollection<TCOMLIQIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TCOMLIQIVA>? GetListDetailsPeriod(string CompanyID, int Year, int Month)
    {
        try
        {
            using var connection = GetOpenConnection();



            var fl = ComputeFirstLastPeriod(Month);

            var list = connection.Query<TCOMLIQIVA>(
                @"select * FROM TCOMLIQIVA where CLICodSOC=@cid and cliannliq = @yea and CLIPerLiq >= @fmon and CLIPerLiq <= @lmon;",
                new { cid = CompanyID, yea = Year, fmon = fl?.Item1, lmon = fl?.Item2 });

            return new ObservableCollection<TCOMLIQIVA>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TCOMLIQIVA? Get(string CLICodSOC, int CLIAnnLiq, int CLIPerLiq, string CLIVocLiq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TCOMLIQIVA>(
                "SELECT * FROM TCOMLIQIVA WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq AND CLIVocLiq = @CLIVocLiq",
                new { CLICodSOC = CLICodSOC, CLIAnnLiq = CLIAnnLiq, CLIPerLiq = CLIPerLiq, CLIVocLiq = CLIVocLiq })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool ComputeOrdinary(string CompanyID, int Year, string QuarterID, string ComputeType, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            decimal vp2 = 0;
            decimal vp3 = 0;
            decimal vp4 = 0;
            decimal vp5 = 0;
            decimal vp6 = 0;
            decimal vp7 = 0;
            decimal vp8 = 0;
            decimal vp9 = 0;
            decimal vp14 = 0;

            if (ComputeType == "M")
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                for (int i = startMonth; i < startMonth + 3; i++)
                {
                    // compute previous month
                    int previousYear = 0;
                    int previousMonth = 0;
                    if (i == 1)
                    {
                        previousYear = Year - 1;
                        previousMonth = 12;
                    }
                    else
                    {
                        previousYear = Year;
                        previousMonth = i - 1;
                    }

                    var multi = connection.QueryMultiple(
                        @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp2;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp4;
                              SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp3;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp5;
                              SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                        new { cid = CompanyID, yea = Year, mon = i, pyea = previousYear, pmon = previousMonth });

                    vp2 = multi.ReadFirst<decimal>();
                    vp4 = multi.ReadFirst<decimal>();
                    vp3 = multi.ReadFirst<decimal>();
                    vp5 = multi.ReadFirst<decimal>();
                    vp6 = vp4 - vp5;
                    var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                    if (vp7vp8 != null)
                    {
                        vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                        if (previousYear < Year)
                            vp9 = vp7vp8.cre;
                        else
                            vp8 = vp7vp8.cre;
                    }
                    vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                        {
                            var newVP = new TCOMLIQIVA()
                            {
                                CLICodSOC = CompanyID,
                                CLIAnnLiq = Year,
                                CLIPerLiq = i,
                                CLIVocLiq = vpl.CVICod,
                                CLITipLiq = "M",
                                CLIImpLiqD = 0,
                                CLIImpLiqC = 0,
                                addedUserID = UserID
                            };

                            switch (vpl.CVICod.Trim())
                            {
                                case "VP2":
                                    newVP.CLIImpLiqD = vp2;
                                    break;
                                case "VP3":
                                    newVP.CLIImpLiqC = vp3;
                                    break;
                                case "VP4":
                                    newVP.CLIImpLiqD = vp4;
                                    break;
                                case "VP5":
                                    newVP.CLIImpLiqC = vp5;
                                    break;
                                case "VP6":
                                    if (vp6 < 0)
                                        newVP.CLIImpLiqC = vp6 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp6;
                                    break;
                                case "VP7":
                                    newVP.CLIImpLiqD = vp7;
                                    break;
                                case "VP8":
                                    newVP.CLIImpLiqC = vp8;
                                    break;
                                case "VP9":
                                    newVP.CLIImpLiqC = vp9;
                                    break;
                                case "VP14":
                                    if (vp14 < 0)
                                        newVP.CLIImpLiqC = vp14 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp14;
                                    break;
                            }
                            connection.Execute(INSERT_QUERY, newVP, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    vp8 = 0;
                    vp9 = 0;
                }

                return true;
            }
            else
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                var startQuarter = QuarterID == "I" ? 1 : (QuarterID == "II" ? 2 : (QuarterID == "III" ? 3 : 4));
                // compute previous month
                int previousYear = 0;
                int previousMonth = 0;
                if (startMonth == 1)
                {
                    previousYear = Year - 1;
                    previousMonth = 12;
                }
                else
                {
                    previousYear = Year;
                    previousMonth = startMonth - 1;
                }

                var multi = connection.QueryMultiple(
                    @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp2;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp4;
                          SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp3;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp5;
                          SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                    new { cid = CompanyID, yea = Year, smon = startMonth, emon = startMonth + 2, pyea = previousYear, pmon = previousMonth });

                vp2 = multi.ReadFirst<decimal>();
                vp4 = multi.ReadFirst<decimal>();
                vp3 = multi.ReadFirst<decimal>();
                vp5 = multi.ReadFirst<decimal>();
                vp6 = vp4 - vp5;

                var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                if (vp7vp8 != null)
                {
                    vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                    if (previousYear < Year)
                        vp9 = vp7vp8.cre;
                    else
                        vp8 = vp7vp8.cre;
                }
                vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                    {
                        var newVP = new TCOMLIQIVA()
                        {
                            CLICodSOC = CompanyID,
                            CLIAnnLiq = Year,
                            CLIPerLiq = startQuarter,
                            CLIVocLiq = vpl.CVICod,
                            CLITipLiq = "T",
                            CLIImpLiqD = 0,
                            CLIImpLiqC = 0,
                            addedUserID = UserID
                        };

                        switch (vpl.CVICod.Trim())
                        {
                            case "VP2":
                                newVP.CLIImpLiqD = vp2;
                                break;
                            case "VP3":
                                newVP.CLIImpLiqC = vp3;
                                break;
                            case "VP4":
                                newVP.CLIImpLiqD = vp4;
                                break;
                            case "VP5":
                                newVP.CLIImpLiqC = vp5;
                                break;
                            case "VP6":
                                if (vp6 < 0)
                                    newVP.CLIImpLiqC = vp6 * -1;
                                else
                                    newVP.CLIImpLiqD = vp6;
                                break;
                            case "VP7":
                                newVP.CLIImpLiqD = vp7;
                                break;
                            case "VP8":
                                newVP.CLIImpLiqC = vp8;
                                break;
                            case "VP9":
                                newVP.CLIImpLiqC = vp9;
                                break;
                            case "VP14":
                                if (vp14 < 0)
                                    newVP.CLIImpLiqC = vp14 * -1;
                                else
                                    newVP.CLIImpLiqD = vp14;
                                break;
                        }
                        connection.Execute(INSERT_QUERY, newVP, transaction);
                    }

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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool ComputeIva(string CompanyID, int Year, string QuarterID, string ComputeType, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            decimal vp2 = 0;
            decimal vp3 = 0;
            decimal vp4 = 0;
            decimal vp5 = 0;
            decimal vp6 = 0;
            decimal vp7 = 0;
            decimal vp8 = 0;
            decimal vp9 = 0;
            decimal vp14 = 0;

            if (ComputeType == "M")
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                for (int i = startMonth; i < startMonth + 3; i++)
                {
                    // compute previous month
                    int previousYear = 0;
                    int previousMonth = 0;
                    if (i == 1)
                    {
                        previousYear = Year - 1;
                        previousMonth = 12;
                    }
                    else
                    {
                        previousYear = Year;
                        previousMonth = i - 1;
                    }

                    var multi = connection.QueryMultiple(
                        @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp2;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '-'), 0) AS vp4;
                              SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) = @mon AND N4SEGN = '-'), 0) AS vp3;
                              SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) = @mon AND N4SEGN = '-'), 0) AS vp5;
                              SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                        new { cid = CompanyID, yea = Year, mon = i, pyea = previousYear, pmon = previousMonth });

                    vp2 = multi.ReadFirst<decimal>();
                    vp4 = multi.ReadFirst<decimal>();
                    vp3 = multi.ReadFirst<decimal>();
                    vp5 = multi.ReadFirst<decimal>();
                    vp6 = vp4 - vp5;
                    var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                    if (vp7vp8 != null)
                    {
                        vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                        if (previousYear < Year)
                            vp9 = vp7vp8.cre;
                        else
                            vp8 = vp7vp8.cre;
                    }
                    vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                    using var transaction = connection.BeginTransaction();
                    try
                    {
                        foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                        {
                            var newVP = new TCOMLIQIVA()
                            {
                                CLICodSOC = CompanyID,
                                CLIAnnLiq = Year,
                                CLIPerLiq = i,
                                CLIVocLiq = vpl.CVICod,
                                CLITipLiq = "M",
                                CLIImpLiqD = 0,
                                CLIImpLiqC = 0,
                                addedUserID = UserID
                            };

                            switch (vpl.CVICod.Trim())
                            {
                                case "VP2":
                                    newVP.CLIImpLiqD = vp2;
                                    break;
                                case "VP3":
                                    newVP.CLIImpLiqC = vp3;
                                    break;
                                case "VP4":
                                    newVP.CLIImpLiqD = vp4;
                                    break;
                                case "VP5":
                                    newVP.CLIImpLiqC = vp5;
                                    break;
                                case "VP6":
                                    if (vp6 < 0)
                                        newVP.CLIImpLiqC = vp6 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp6;
                                    break;
                                case "VP7":
                                    newVP.CLIImpLiqD = vp7;
                                    break;
                                case "VP8":
                                    newVP.CLIImpLiqC = vp8;
                                    break;
                                case "VP9":
                                    newVP.CLIImpLiqC = vp9;
                                    break;
                                case "VP14":
                                    if (vp14 < 0)
                                        newVP.CLIImpLiqC = vp14 * -1;
                                    else
                                        newVP.CLIImpLiqD = vp14;
                                    break;
                            }
                            connection.Execute(INSERT_QUERY, newVP, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                return true;
            }
            else
            {
                var startMonth = QuarterID == "I" ? 1 : (QuarterID == "II" ? 4 : (QuarterID == "III" ? 7 : 10));
                var startQuarter = QuarterID == "I" ? 1 : (QuarterID == "II" ? 2 : (QuarterID == "III" ? 3 : 4));
                // compute previous month
                int previousYear = 0;
                int previousMonth = 0;
                if (startMonth == 1)
                {
                    previousYear = Year - 1;
                    previousMonth = 12;
                }
                else
                {
                    previousYear = Year;
                    previousMonth = startMonth - 1;
                }

                var multi = connection.QueryMultiple(
                    @"SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp2;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'C' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '-'), 0) AS vp4;
                          SELECT ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IMEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DARE) = @yea AND MONTH(N4DARE) >= @smon AND MONTH(N4DARE) <= @emon AND N4SEGN = '-'), 0) AS vp3;
                          SELECT ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '+'), 0) - ISNULL((SELECT SUM(N4IVEU-N4IIEU) FROM PN_IVA WHERE N4SOCI = @cid AND N4TCLF = 'F' AND YEAR(N4DTPGEF) = @yea AND MONTH(N4DTPGEF) >= @smon AND MONTH(N4DTPGEF) <= @emon AND N4SEGN = '-'), 0) AS vp5;
                          SELECT CLIImpLiqD AS deb, CLIImpLiqC AS cre FROM TCOMLIQIVA WHERE CLICodSoc=@cid AND CLIVocLiq='VP14' AND CLIAnnLiq=@pyea AND CLIPerLiq=@pmon;",
                    new { cid = CompanyID, yea = Year, smon = startMonth, emon = startMonth + 2, pyea = previousYear, pmon = previousMonth });

                vp2 = multi.ReadFirst<decimal>();
                vp4 = multi.ReadFirst<decimal>();
                vp3 = multi.ReadFirst<decimal>();
                vp5 = multi.ReadFirst<decimal>();
                vp6 = vp4 - vp5;

                var vp7vp8 = multi.ReadFirstOrDefault<dynamic?>();
                if (vp7vp8 != null)
                {
                    vp7 = vp7vp8.deb <= 25.82m ? vp7vp8.deb : 0;
                    if (previousYear < Year)
                        vp9 = vp7vp8.cre;
                    else
                        vp8 = vp7vp8.cre;
                }
                vp14 = ((vp6 >= 0 ? (vp6 + vp7) : (vp7 - (vp6 * -1)))) - (vp8 + vp9);
                using var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var vpl in VulpesServiceProvider.Provider.GetRequiredService<ITCODLIQIVARepository>().GetList(CompanyID) ?? new ObservableCollection<TCODLIQIVA>())
                    {
                        var newVP = new TCOMLIQIVA()
                        {
                            CLICodSOC = CompanyID,
                            CLIAnnLiq = Year,
                            CLIPerLiq = startQuarter,
                            CLIVocLiq = vpl.CVICod,
                            CLITipLiq = "T",
                            CLIImpLiqD = 0,
                            CLIImpLiqC = 0,
                            addedUserID = UserID
                        };

                        switch (vpl.CVICod.Trim())
                        {
                            case "VP2":
                                newVP.CLIImpLiqD = vp2;
                                break;
                            case "VP3":
                                newVP.CLIImpLiqC = vp3;
                                break;
                            case "VP4":
                                newVP.CLIImpLiqD = vp4;
                                break;
                            case "VP5":
                                newVP.CLIImpLiqC = vp5;
                                break;
                            case "VP6":
                                if (vp6 < 0)
                                    newVP.CLIImpLiqC = vp6 * -1;
                                else
                                    newVP.CLIImpLiqD = vp6;
                                break;
                            case "VP7":
                                newVP.CLIImpLiqD = vp7;
                                break;
                            case "VP8":
                                newVP.CLIImpLiqC = vp8;
                                break;
                            case "VP9":
                                newVP.CLIImpLiqC = vp9;
                                break;
                            case "VP14":
                                if (vp14 < 0)
                                    newVP.CLIImpLiqC = vp14 * -1;
                                else
                                    newVP.CLIImpLiqD = vp14;
                                break;
                        }
                        connection.Execute(INSERT_QUERY, newVP, transaction);
                    }

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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? GenerateXML(LIPEXMLViewModel Options)
    {
        try
        {
            CultureInfo culture = new CultureInfo("it-IT");
            XNamespace iv = "urn:www.agenziaentrate.gov.it:specificheTecniche:sco:ivp";
            XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";

            var fl = ComputeFirstLastPeriod(Options.Month);
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            #region Generate file name
            string fullPath = $"{Path.GetTempPath()}IT{Options.VATID}_{Options.Year.ToString("F0")}{fl?.Item1.ToString("F0")}.xml";
            #endregion

            #region Compute needed LIPEs
            var items = new List<TCOMLIQIVA>();

            if (Options.LIPEType == "T")
            {
                items.AddRange(GetListDetails(Options.CompanyID, Options.Year, Options.Month) ?? new ObservableCollection<TCOMLIQIVA>());
            }
            else
            {
                items.AddRange(GetListDetailsPeriod(Options.CompanyID, Options.Year, Options.Month) ?? new ObservableCollection<TCOMLIQIVA>());
            }
            #endregion

            var root = new XElement(iv + "Fornitura",
                new XAttribute(XNamespace.Xmlns + "iv", iv.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "ds", ds.NamespaceName));

            var header = new XElement(iv + "Intestazione",
                new XElement(iv + "CodiceFornitura", "IVP18"),
                new XElement(iv + "CodiceFiscaleDichiarante", Options.FiscalIDSender?.Trim().ToUpper()),
                new XElement(iv + "CodiceCarica", Options.TitleID));
            root.Add(header);

            XElement[]? broker = null;
            if (!string.IsNullOrWhiteSpace(Options.BrokerFiscalID))
            {
                broker = new XElement[]{
                    new XElement(iv + "CFIntermediario", Options.BrokerFiscalID),
                    new XElement(iv + "ImpegnoPresentazione", Options.PresentationID),
                    new XElement(iv + "DataImpegno", now.ToString("ddMMyyyy")),
                    new XElement(iv + "FirmaIntermediario", "1")
                };
            }

            var title = new XElement(iv + "Frontespizio",
                new XElement(iv + "CodiceFiscale", Options.FISCALID?.Trim().ToUpper()),
                new XElement(iv + "AnnoImposta", $"{Options.Year.ToString("F0")}"),
                new XElement(iv + "PartitaIVA", Options.VATID?.Trim().ToUpper()),
                new XElement(iv + "CFDichiarante", Options.FiscalIDSender?.Trim().ToUpper()),
                new XElement(iv + "CodiceCaricaDichiarante", Options.TitleID),
                new XElement(iv + "FirmaDichiarazione", "1"),
                broker);

            var data = new XElement(iv + "DatiContabili");
            int modpro = 1;
            var isTri = items.GroupBy(g => g.CLIPerLiq).Count() == 1;
            foreach (var module in items.GroupBy(g => g.CLIPerLiq, (key, items) => new { key = key, items = items }).OrderBy(o => o.key))
            {
                var vp6 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP6").First();
                var vp7 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP7").First();
                var vp8 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP8").First();
                var vp9 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP9").First();
                var vp10 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP10").First();
                var vp11 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP11").First();
                var vp12 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP12").First();
                var vp14 = module.items.Where(w => w.CLIVocLiq.Trim() == "VP14").First();

                data.Add(new XElement(iv + "Modulo",
                    new XElement(iv + "NumeroModulo", modpro++),
                    new XElement(iv + (isTri ? "Trimestre" : "Mese"), module.key),
                    new XElement(iv + "TotaleOperazioniAttive", module.items.Where(w => w.CLIVocLiq.Trim() == "VP2").First().CLIImpLiqD.ToString("F2", culture)),
                    new XElement(iv + "TotaleOperazioniPassive", module.items.Where(w => w.CLIVocLiq.Trim() == "VP3").First().CLIImpLiqC.ToString("F2", culture)),
                    new XElement(iv + "IvaEsigibile", module.items.Where(w => w.CLIVocLiq.Trim() == "VP4").First().CLIImpLiqD.ToString("F2", culture)),
                    new XElement(iv + "IvaDetratta", module.items.Where(w => w.CLIVocLiq.Trim() == "VP5").First().CLIImpLiqC.ToString("F2", culture)),
                    new XElement(iv + $"{(vp6.CLIImpLiqD == vp6.CLIImpLiqC || vp6.CLIImpLiqD > 0 ? "IvaDovuta" : "IvaCredito")}", $"{(vp6.CLIImpLiqD == vp6.CLIImpLiqC ? "0,00" : (vp6.CLIImpLiqD > 0 ? vp6.CLIImpLiqD.ToString("F2", culture) : vp6.CLIImpLiqC.ToString("F2", culture)))}"),
                    (vp7.CLIImpLiqD > 0 ? new XElement(iv + "DebitoPrecedente", vp7.CLIImpLiqD) : null),
                    (vp8.CLIImpLiqC > 0 ? new XElement(iv + "CreditoPeriodoPrecedente", vp8.CLIImpLiqC.ToString("F2", culture)) : null),
                    (vp9.CLIImpLiqC > 0 ? new XElement(iv + "CreditoAnnoPrecedente", vp9.CLIImpLiqC.ToString("F2", culture)) : null),
                    (vp10.CLIImpLiqC > 0 ? new XElement(iv + "VersamentiAuto", vp10.CLIImpLiqC.ToString("F2", culture)) : null),
                    (vp11.CLIImpLiqC > 0 ? new XElement(iv + "CreditiImposta", vp11.CLIImpLiqC.ToString("F2", culture)) : null),
                    (vp12.CLIImpLiqD > 0 ? new XElement(iv + "InteressiDovuti", vp12.CLIImpLiqD.ToString("F2", culture)) : null),
                    new XElement(iv + $"{(vp14.CLIImpLiqD == vp14.CLIImpLiqC || vp14.CLIImpLiqD > 0 ? "ImportoDaVersare" : "ImportoACredito")}", $"{(vp14.CLIImpLiqD == vp14.CLIImpLiqC ? "0,00" : (vp14.CLIImpLiqD > 0 ? vp14.CLIImpLiqD.ToString("F2", culture) : vp14.CLIImpLiqC.ToString("F2", culture)))}")));
            }

            var message = new XElement(iv + "Comunicazione",
                new XAttribute("identificativo", "00001"),
                title,
                data);
            root.Add(message);

            root.Save(fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CLICodSOC, int CLIAnnLiq, int CLIPerLiq)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TCOMLIQIVA WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq",
                new { CLICodSOC = CLICodSOC, CLIAnnLiq = CLIAnnLiq, CLIPerLiq = CLIPerLiq }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    public Tuple<int, int>? ComputeFirstLastPeriod(int Month)
    {
        if (Month >= 1 && Month <= 3)
        {
            return new Tuple<int, int>(1, 3);
        }
        else if (Month >= 4 && Month <= 6)
        {
            return new Tuple<int, int>(4, 6);
        }
        else if (Month >= 7 && Month <= 9)
        {
            return new Tuple<int, int>(7, 9);
        }
        else if (Month >= 10 && Month <= 12)
        {
            return new Tuple<int, int>(10, 12);
        }
        else
        { return null; }
    }

    #region CRUD
    public string INSERT_QUERY => "INSERT INTO TCOMLIQIVA (CLICodSOC,CLIAnnLiq,CLIPerLiq,CLIVocLiq,CLITipLiq,CLIImpLiqD,CLIImpLiqC,addedUserID,added,updated,canceled,updatedUserID,canceledUserID,canceledNote, CLISubfor, CliEventi,Clidatelab, CliUserelab, cligenxml) OUTPUT INSERTED.rv VALUES(@CLICodSOC,@CLIAnnLiq,@CLIPerLiq,@CLIVocLiq,@CLITipLiq,@CLIImpLiqD,@CLIImpLiqC,@addedUserID,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@updatedUserID,@canceledUserID,@canceledNote, '', '','1753-01-01', '', 'N')";
    public string UPDATE_QUERY => "UPDATE TCOMLIQIVA SET CLICodSOC = @CLICodSOC,CLIAnnLiq = @CLIAnnLiq,CLIPerLiq = @CLIPerLiq,CLIVocLiq = @CLIVocLiq,CLITipLiq = @CLITipLiq,CLIImpLiqD = @CLIImpLiqD,CLIImpLiqC = @CLIImpLiqC,addedUserID = @addedUserID,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote OUTPUT INSERTED.rv WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq AND CLIVocLiq = @CLIVocLiq AND rv = @rv";
    public string DELETE_QUERY => "DELETE FROM TCOMLIQIVA WHERE CLICodSOC = @CLICodSOC AND CLIAnnLiq = @CLIAnnLiq AND CLIPerLiq = @CLIPerLiq";
    public bool Insert(TCOMLIQIVA Model)
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

    public bool Update(ObservableCollection<TCOMLIQIVA> Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                int updated = 0;
                foreach (var item in Model)
                {
                    updated += (int)connection.Execute(UPDATE_QUERY, item, transaction);
                }

                if (updated == Model.Count)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                    return false;
                }
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

    public bool Delete(TCOMLIQIVA Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            connection.Execute(DELETE_QUERY, Model);
            return true;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(TCOMLIQIVA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.CLICodSOC) && IsInsert && !Exists(Model.CLICodSOC, Model.CLIAnnLiq, Model.CLIPerLiq)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.CLITipLiq))
                {
                    return null;
                }
                else
                { return "Il tipo liquidazione è obbligatorio"; }
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