using Microsoft.Extensions.DependencyInjection;

namespace VulpesX.DAL.Shipping;

public interface IBOLLT00F_historyRepository
{

    ObservableCollection<BOLLT00F_history>? GetList();

    BOLLT00F_history? Get(string bolsoc, int BTANNO, int BTBOLL, int revision);

    #region CRUD
    // remove rv
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }

    bool Insert(BOLLT00F_history Model);

    bool Update(BOLLT00F_history Model);

    bool Delete(BOLLT00F_history Model);

    #endregion
}

public class BOLLT00F_historyRepository : RepositoryBase, IBOLLT00F_historyRepository
{
    public BOLLT00F_historyRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BOLLT00F_history>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BOLLT00F_history>(
                "SELECT * FROM BOLLT00F_history");

            return new ObservableCollection<BOLLT00F_history>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BOLLT00F_history? Get(string bolsoc, int BTANNO, int BTBOLL, int revision)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BOLLT00F_history>(
                "SELECT * FROM BOLLT00F_history WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL, revision = revision })
                .FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    // remove rv
    public string INSERT_QUERY => "INSERT INTO BOLLT00F_history (bolsoc,BTANNO,BTBOLL,revision,BTNUBD,BTDATP,BTDATA,BTCAUS,BTCODC,BTCODD,BTPAGA,BTCONS,BTSPED,BTCORR,BTDE25,BTPESO,BTDASP,BTCOLL,BTAREA,BTDEBE,BTPES2,BTIMBA,abiabi,abicab,BTNOTET,BTNOTEP,BTSHOWT,BTSHOWP,added,addedUserID,BTCORR2,BTBCON,BTCONZ,BTSTATO,BTDAFA,BTFILI,BTSCCL,BTZONA,BTSETM,BTREGI,BTRIVE,BTLING,updated,updatedUserID) VALUES(@bolsoc,@BTANNO,@BTBOLL,@revision,@BTNUBD,@BTDATP,@BTDATA,@BTCAUS,@BTCODC,@BTCODD,@BTPAGA,@BTCONS,@BTSPED,@BTCORR,@BTDE25,@BTPESO,@BTDASP,@BTCOLL,@BTAREA,@BTDEBE,@BTPES2,@BTIMBA,@abiabi,@abicab,@BTNOTET,@BTNOTEP,@BTSHOWT,@BTSHOWP,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@addedUserID,@BTCORR2,@BTBCON,@BTCONZ,@BTSTATO,@BTDAFA,@BTFILI,@BTSCCL,@BTZONA,@BTSETM,@BTREGI,@BTRIVE,@BTLING,@updated,@updatedUserID)";
    public string UPDATE_QUERY => "UPDATE BOLLT00F_history SET bolsoc = @bolsoc,BTANNO = @BTANNO,BTBOLL = @BTBOLL,revision = @revision,BTNUBD = @BTNUBD,BTDATP = @BTDATP,BTDATA = @BTDATA,BTCAUS = @BTCAUS,BTCODC = @BTCODC,BTCODD = @BTCODD,BTPAGA = @BTPAGA,BTCONS = @BTCONS,BTSPED = @BTSPED,BTCORR = @BTCORR,BTDE25 = @BTDE25,BTPESO = @BTPESO,BTDASP = @BTDASP,BTCOLL = @BTCOLL,BTAREA = @BTAREA,BTDEBE = @BTDEBE,BTPES2 = @BTPES2,BTIMBA = @BTIMBA,abiabi = @abiabi,abicab = @abicab,BTNOTET = @BTNOTET,BTNOTEP = @BTNOTEP,BTSHOWT = @BTSHOWT,BTSHOWP = @BTSHOWP,added = @added,addedUserID = @addedUserID,BTCORR2 = @BTCORR2,BTBCON = @BTBCON,BTCONZ = @BTCONZ,BTSTATO = @BTSTATO,BTDAFA = @BTDAFA,BTFILI = @BTFILI,BTSCCL = @BTSCCL,BTZONA = @BTZONA,BTSETM = @BTSETM,BTREGI = @BTREGI,BTRIVE = @BTRIVE,BTLING = @BTLING,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',updatedUserID = @updatedUserID WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision";
    public string DELETE_QUERY => "DELETE FROM BOLLT00F_history WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision";

    public bool Insert(BOLLT00F_history Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            using var transaction = connection.BeginTransaction();
            try
            {
                var bolldHistoryRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F_historyRepository>();
                var bolld1HistoryRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1_historyRepository>();

                var lastRevision = (int?)connection.ExecuteScalar(@"SELECT MAX(revision) FROM BOLLT00F_history
                                                                        WHERE bolsoc=@bolsoc AND BTANNO=@btanno AND BTBOLL=@btboll",
                    new { bolsoc = Model.bolsoc, btanno = Model.BTANNO, btboll = Model.BTBOLL }, transaction) ?? 0;
                lastRevision += 1;
                Model.revision = lastRevision;
                connection.Execute(INSERT_QUERY, Model, transaction);

                foreach (var row in Model.Rows ?? new List<BOLLD00F_history>())
                {
                    row.revision = lastRevision;
                    connection.Execute(bolldHistoryRepo.INSERT_QUERY, row, transaction);
                    foreach (var eng in row.Engages ?? new List<BOLLD00F1_history>())
                    {
                        eng.revision = lastRevision;
                        connection.Execute(bolld1HistoryRepo.INSERT_QUERY, eng, transaction);
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
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(BOLLT00F_history Model)
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

    public bool Delete(BOLLT00F_history Model)
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

    #endregion
}