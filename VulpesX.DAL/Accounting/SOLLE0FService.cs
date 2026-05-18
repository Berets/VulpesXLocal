
using VulpesX.Models.Default;

namespace VulpesX.DAL.Accounting;


public interface ISOLLE0FRepository
{

    ObservableCollection<SOLLE0F>? GetList(string CompanyID, int CustomerID, string ReferenceID);

    SOLLE0F? Get(string CompanyID, int CustomerID, DateTime DateAnn, DateTime DateDat, string ReferenceID, int SollType, DateTime ExpireDate, int RowID);

    bool Insert(SOLLE0F Model);

    bool Update(SOLLE0F Model);

    bool Delete(SOLLE0F Model);

    string? Validate(SOLLE0F Model, bool IsInsert);
}

public class SOLLE0FRepository : RepositoryBase, ISOLLE0FRepository
{
    public SOLLE0FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<SOLLE0F>? GetList(string CompanyID, int CustomerID, string ReferenceID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<SOLLE0F, SOLLECITI, SOLLE0F>(
                @"SELECT m.*, t.* FROM SOLLE0F AS m
                      INNER JOIN SOLLECITI AS t ON m.soltip = t.solcod
                      WHERE sollsoc = @sollsoc AND sollcli = @sollcli AND sollrif = @sollrif
                      ORDER BY solldat DESC",
                (soll, tip) => { soll.ReminderType = tip; return soll; },
                new { sollsoc = CompanyID, sollcli = CustomerID, sollrif = ReferenceID },
                splitOn: "solcod");

            return new ObservableCollection<SOLLE0F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public SOLLE0F? Get(string CompanyID, int CustomerID, DateTime DateAnn, DateTime DateDat, string ReferenceID, int SollType, DateTime ExpireDate, int RowID)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<SOLLE0F>(
                "SELECT * FROM SOLLE0F WHERE sollsoc = @sollsoc AND sollcli = @sollcli AND sollann = @sollann AND solldat = @solldat AND sollrif = @sollrif AND soltip = @soltip AND sollscad = @sollscad AND sollrig = @sollrig",
                new { sollsoc = CompanyID, sollcli = CustomerID, sollann = DateAnn, solldat = DateDat, sollrif = ReferenceID, soltip = SollType, sollscad = ExpireDate, sollrig = RowID })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Insert(SOLLE0F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO SOLLE0F (Sollsoc,Sollcli,sollann,solldat,sollrif,soltip,sollscad,sollrig,solsot,solcot,soltit,solltes,sollimpo,sollgra,sollflg,sollfile,sollsegn,sollcau,sollcaude) OUTPUT INSERTED.rv VALUES(@Sollsoc,@Sollcli,@sollann,@solldat,@sollrif,@soltip,@sollscad,@sollrig,@solsot,@solcot,@soltit,@solltes,@sollimpo,@sollgra,@sollflg,@sollfile,@sollsegn,@sollcau,@sollcaude)",
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

    public bool Update(SOLLE0F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE SOLLE0F SET Sollsoc = @Sollsoc,Sollcli = @Sollcli,sollann = @sollann,solldat = @solldat,sollrif = @sollrif,soltip = @soltip,sollscad = @sollscad,sollrig = @sollrig,solsot = @solsot,solcot = @solcot,soltit = @soltit,solltes = @solltes,sollimpo = @sollimpo,sollgra = @sollgra,sollflg = @sollflg,sollfile = @sollfile,sollsegn = @sollsegn,sollcau = @sollcau,sollcaude = @sollcaude OUTPUT INSERTED.rv WHERE sollsoc = @sollsoc AND sollcli = @sollcli AND sollann = @sollann AND solldat = @solldat AND sollrif = @sollrif AND soltip = @soltip AND sollscad = @sollscad AND sollrig = @sollrig AND rv = @rv",
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

    public bool Delete(SOLLE0F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM SOLLE0F OUTPUT DELETED.rv WHERE sollsoc = @sollsoc AND sollcli = @sollcli AND sollann = @sollann AND solldat = @solldat AND sollrif = @sollrif AND soltip = @soltip AND sollscad = @sollscad AND sollrig = @sollrig AND rv = @rv",
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

    public string? Validate(SOLLE0F Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}