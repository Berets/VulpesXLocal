using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Accounting;
using VulpesX.Services.Accounting;

namespace VulpesX.DAL.Treasury
{
    public interface ITreasuryRepository
    {
        ObservableCollection<RBCC01F0>? LoadMovements(string CompanyID, int Year, DateTime ExpireDate, bool OnlyAtBank);
    }

    public class TreasuryRepository : RepositoryBase, ITreasuryRepository
    {
        public TreasuryRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<RBCC01F0>? LoadMovements(string CompanyID, int Year, DateTime ExpireDate, bool OnlyAtBank)
        {
            try
            {
                using var connection = GetOpenConnection();

                var dateTimeService = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>();
                var pnPortafoglioRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();

                var list = connection.Query<RBCC01F0, PDCSOTTO, BANAZIEN, ABICAB, RBCC01F0>(
                    @"SELECT r.*, l2.*, b.*, a.* FROM RBCC01F0 AS r 
                            INNER JOIN PDCSOTTO AS l2 ON l2.P1GRUP = r.cnsl30 AND l2.P2CONT = r.cnsl31 AND l2.P3SOTC = r.cnsl32 
                            INNER JOIN BANAZIEN AS b ON b.abisoc=r.cnsl34 AND (b.abiabi= CASE WHEN r.cnslnewabi > 0 THEN r.cnslnewabi ELSE r.cnsl01 END) AND (b.abicab= CASE WHEN r.cnslnewcab > 0 THEN r.cnslnewcab ELSE r.cnsl02 END) AND b.abicon=r.cnsl05 AND b.abiatt = 1
                            INNER JOIN ABICAB AS a ON a.abiabi=b.abiabi AND a.abicab=b.abicab
                            WHERE r.cnsl34 =  @cid AND r.cnsl36 <> 'A'",
                    (cast, pdcsotto, bank, abc) => { bank.Bank = abc; cast.Bank = bank; cast.Subaccount = pdcsotto; return cast; },
                    new { cid = CompanyID },
                    splitOn: "p1grup,abisoc,abiabi")
                    .ToList();

                foreach (var item in list)
                {
                    item.Castelletto = pnPortafoglioRepository.GetCastelletto(CompanyID, ExpireDate, item.cnsl01, item.cnslnewabi ?? 0, item.cnsl02, item.cnslnewcab ?? 0);
                    var provv = pnRigheRepository.GetProvvisorioEScadenzaMeseCorrente(CompanyID, dateTimeService.GetDatabaseServerDateTime(), item.cnsl30 ??string.Empty, item.cnsl31 ?? string.Empty, item.cnsl32 ?? string.Empty, OnlyAtBank);
                    item.ImportoProvvisorio = provv.Item1;
                    item.ImportoScadenza = provv.Item2;
                    var provvSucc = pnRigheRepository.GetProvvisorioEScadenzaMeseSuccessivo(CompanyID, dateTimeService.GetDatabaseServerDateTime(), item.cnsl30 ?? string.Empty, item.cnsl31 ?? string.Empty, item.cnsl32 ?? string.Empty, OnlyAtBank);
                    item.ImportoProvvisorioSuccessivo = provvSucc.Item1;
                    item.ImportoScadenzaSuccessiva = provvSucc.Item2;
                }

                return new ObservableCollection<RBCC01F0>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }
    }
}
