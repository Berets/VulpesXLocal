using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Services.Accounting;
using VulpesX.Services.Tables.Accounting;

namespace VulpesX.DAL.Treasury;

public interface ITES_IMFIRepository
{
    ObservableCollection<TES_IMFI>? GetList(string CompanyID);

    ObservableCollection<TES_IMFI>? GetListNear(string CompanyID);

    TES_IMFI? Get(string CompanyID, string GroupID, string AccountID, string SubaccountID, DateTime DateID);

    bool Exists(string CompanyID, string GroupID, string AccountID, string SubaccountID, DateTime DateID);

    #region Accounting
    bool Accounting(TES_IMFI Item, int AccountingYear, DateTime AccountingDate, CAUCONT Causal, string GroupID, string AccountID, string SubaccountID, string UserID);
    #endregion

    #region CRUD
    bool Insert(TES_IMFI Model);

    bool Update(TES_IMFI Model);

    bool Delete(TES_IMFI Model);

    string? Validate(TES_IMFI Model, bool IsInsert);
    #endregion
}

public class TES_IMFIRepository : RepositoryBase, ITES_IMFIRepository
{
    public TES_IMFIRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<TES_IMFI>? GetList(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<TES_IMFI, PDCGRUPPI, PDCCONTI, PDCSOTTO, TES_IMFI>(
                @"SELECT m.*, g.*, c.*, s.* FROM TES_IMFI AS m 
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = m.ifgrup
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = m.ifgrup AND c.P2CONT = m.ifcont
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = m.ifgrup AND s.P2CONT = m.ifcont AND s.P3SOTC = m.ifsott
                        WHERE m.ifsoci = @ifsoci
                        ORDER BY m.ifdata DESC, m.ifgrup, m.ifcont, m.ifsott",
                (imfi, group, account, subaccount) => { imfi.Group = group; imfi.Account = account; imfi.Subaccount = subaccount; return imfi; },
                new { ifsoci = CompanyID },
                splitOn: "P1GRUP,P1GRUP,P1GRUP");

            return new ObservableCollection<TES_IMFI>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<TES_IMFI>? GetListNear(string CompanyID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = new List<TES_IMFI>();
            var distinct = connection.Query<TES_IMFI>(
                @"SELECT DISTINCT c.ifgrup, c.ifcont, c.ifsott, c.ifdata FROM TES_IMFI AS c WHERE c.ifsoci = @ifsoci AND c.ifdaca IS NULL",
                new { ifsoci = CompanyID });

            foreach (var dist in distinct)
            {
                result.Add(connection.Query<TES_IMFI, PDCGRUPPI, PDCCONTI, PDCSOTTO, TES_IMFI>(
                    @"SELECT TOP(1) m.*, g.*, c.*, s.* FROM TES_IMFI AS m 
                        INNER JOIN PDCGRUPPI AS g ON g.P1GRUP = m.ifgrup
                        INNER JOIN PDCCONTI AS c ON c.P1GRUP = m.ifgrup AND c.P2CONT = m.ifcont
                        INNER JOIN PDCSOTTO AS s ON s.P1GRUP = m.ifgrup AND s.P2CONT = m.ifcont AND s.P3SOTC = m.ifsott
                        WHERE m.ifsoci = @ifsoci AND m.ifgrup = @ifgrup AND m.ifcont = @ifcont AND m.ifsott = @ifsott AND m.ifdata = @ifdata AND m.ifdaca IS NULL
                        ORDER BY m.ifdata",
                    (imfi, group, account, subaccount) => { imfi.Group = group; imfi.Account = account; imfi.Subaccount = subaccount; return imfi; },
                    new { ifsoci = CompanyID, ifgrup = dist.ifgrup, ifcont = dist.ifcont, ifsott = dist.ifsott, ifdata = dist.ifdata },
                    splitOn: "P1GRUP,P1GRUP,P1GRUP").First());
            }

            return new ObservableCollection<TES_IMFI>(result.OrderBy(o => o.ifdata));

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public TES_IMFI? Get(string CompanyID, string GroupID, string AccountID, string SubaccountID, DateTime DateID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<TES_IMFI>(
                "SELECT * FROM TES_IMFI WHERE ifsoci = @ifsoci AND ifgrup = @ifgrup AND ifcont = @ifcont AND ifsott = @ifsott AND ifdata = @ifdata",
                new { ifsoci = CompanyID, ifgrup = GroupID, ifcont = AccountID, ifsott = SubaccountID, ifdata = DateID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string CompanyID, string GroupID, string AccountID, string SubaccountID, DateTime DateID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM TES_IMFI WHERE ifsoci = @ifsoci AND ifgrup = @ifgrup AND ifcont = @ifcont AND ifsott = @ifsott AND ifdata = @ifdata",
                new { ifsoci = CompanyID, ifgrup = GroupID, ifcont = AccountID, ifsott = SubaccountID, ifdata = DateID }) > 0;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region Accounting
    public bool Accounting(TES_IMFI Item, int AccountingYear, DateTime AccountingDate, CAUCONT Causal, string GroupID, string AccountID, string SubaccountID, string UserID)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var pnTestataRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>();
                var pnRigheRepository = VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>();

                // get registration number
                var accountingID = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(Item.ifsoci, AccountingYear, Constants.PN, true);

                #region PNTESTATA
                PNTESTATA head = new PNTESTATA()
                {
                    N1SOCI = Item.ifsoci,
                    N1ANNO = AccountingYear,
                    N1REGI = accountingID,
                    pncaus = Causal.caucod,
                    N1DARE = AccountingDate.Date,
                    N1docn = "1",
                    N1docd = AccountingDate.Date,
                    N1rifn = "1",
                    N1rifd = AccountingDate.Date,
                    pnvcod = "UIC",
                    pnvdiv = "EUR",
                    N1FL01 = string.Empty,
                    N1TmpPN = "S",
                    n1mrii = 0,
                    addedUserID = UserID
                };
                connection.Execute(pnTestataRepository.INSERT_QUERY, head, transaction);
                #endregion

                #region Item row
                PNRIGHE customerRow = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = 1,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1rifn,
                    N1DARI = head.N1rifd,
                    N1SEGN = "D",
                    pngrup = Item.ifgrup,
                    pncont = Item.ifcont,
                    pnsott = Item.ifsott,
                    N1IMEU = Item.ifimpo,
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "S"
                };
                connection.Execute(pnRigheRepository.INSERT_QUERY, customerRow, transaction);
                #endregion

                #region Specified row
                PNRIGHE specifiedRow = new PNRIGHE()
                {
                    N1SOCI = head.N1SOCI,
                    N1ANNO = head.N1ANNO,
                    N1REGI = head.N1REGI,
                    N1RIGA = 2,
                    N1DOCU = head.N1docn,
                    N1DADO = head.N1docd,
                    N1RIFE = head.N1rifn,
                    N1DARI = head.N1rifd,
                    N1SEGN = "A",
                    pngrup = GroupID,
                    pncont = AccountID,
                    pnsott = SubaccountID,
                    N1IMEU = Item.ifimpo,
                    N1CHIU = "A",
                    N1TIDO = "E",
                    N1DIVI = "EUR",
                    N1tmpPNR = "S"
                };
                connection.Execute(pnRigheRepository.INSERT_QUERY, specifiedRow, transaction);
                #endregion

                // flag job worked
                Item.ifdaca = AccountingDate.Date;
                Item.ifregann = AccountingYear;
                Item.ifregnum = accountingID;
                connection.Execute(
                "UPDATE TES_IMFI SET ifsoci = @ifsoci,ifgrup = @ifgrup,ifcont = @ifcont,ifsott = @ifsott,ifdata = @ifdata,ifimpo = @ifimpo,ifrife = @ifrife,ifdaca = @ifdaca,ifnote = @ifnote,ifregann = @ifregann,ifregnum = @ifregnum OUTPUT INSERTED.rv WHERE ifsoci = @ifsoci AND ifgrup = @ifgrup AND ifcont = @ifcont AND ifsott = @ifsott AND ifdata = @ifdata AND rv = @rv",
                Item, transaction);

                transaction.Commit();
                InfoHandler.Show($"Contabilizzazione completata correttamente, generata la registrazione n.{accountingID}");
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
    #endregion

    #region CRUD
    public bool Insert(TES_IMFI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO TES_IMFI (ifsoci,ifgrup,ifcont,ifsott,ifdata,ifimpo,ifrife,ifdaca,ifnote,ifregann,ifregnum) OUTPUT INSERTED.rv VALUES(@ifsoci,@ifgrup,@ifcont,@ifsott,@ifdata,@ifimpo,@ifrife,@ifdaca,@ifnote,@ifregann,@ifregnum)",
                Model);
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

    public bool Update(TES_IMFI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE TES_IMFI SET ifsoci = @ifsoci,ifgrup = @ifgrup,ifcont = @ifcont,ifsott = @ifsott,ifdata = @ifdata,ifimpo = @ifimpo,ifrife = @ifrife,ifdaca = @ifdaca,ifnote = @ifnote,ifregann = @ifregann,ifregnum = @ifregnum OUTPUT INSERTED.rv WHERE ifsoci = @ifsoci AND ifgrup = @ifgrup AND ifcont = @ifcont AND ifsott = @ifsott AND ifdata = @ifdata AND rv = @rv",
                Model);
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

    public bool Delete(TES_IMFI Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM TES_IMFI OUTPUT DELETED.rv WHERE ifsoci = @ifsoci AND ifgrup = @ifgrup AND ifcont = @ifcont AND ifsott = @ifsott AND ifdata = @ifdata AND rv = @rv",
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

    public string? Validate(TES_IMFI Model, bool IsInsert)
    {
        try
        {
            if (!IsInsert || (IsInsert && !Exists(Model.ifsoci, Model.ifgrup, Model.ifcont, Model.ifsott, Model.ifdata)))
            {
                return null;
            }
            else
            { return "Esiste già un record con queste informazioni"; }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}