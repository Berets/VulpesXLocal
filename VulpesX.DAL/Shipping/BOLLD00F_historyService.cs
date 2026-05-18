namespace VulpesX.DAL.Shipping;

public interface IBOLLD00F_historyRepository
{
    ObservableCollection<BOLLD00F_history>? GetList();

    BOLLD00F_history? Get(string bolsoc, int BTANNO, int BTBOLL, int revision, int BORIGB);


    #region CRUD
    // remove rv
    string INSERT_QUERY { get; }
    string UPDATE_QUERY { get; }
    string DELETE_QUERY { get; }
    bool Insert(BOLLD00F_history Model);

    bool Update(BOLLD00F_history Model);

    bool Delete(BOLLD00F_history Model);
    #endregion
}

public class BOLLD00F_historyRepository : RepositoryBase, IBOLLD00F_historyRepository
{
    public BOLLD00F_historyRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<BOLLD00F_history>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<BOLLD00F_history>(
                "SELECT * FROM BOLLD00F_history");

            return new ObservableCollection<BOLLD00F_history>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public BOLLD00F_history? Get(string bolsoc, int BTANNO, int BTBOLL, int revision, int BORIGB)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<BOLLD00F_history>(
                "SELECT * FROM BOLLD00F_history WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision AND BORIGB = @BORIGB",
                new { bolsoc = bolsoc, BTANNO = BTANNO, BTBOLL = BTBOLL, revision = revision, BORIGB = BORIGB })
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
    public string INSERT_QUERY => "INSERT INTO BOLLD00F_history (bolsoc,BTANNO,BTBOLL,revision,BORIGB,BOANNO,BONUOR,BORIGA,BOMAGA,BOCODA,BOQTAV,BOTQTA,BOUNIM,BODACO,BOSERI,BORIFC,boprez,botpre,boaliq,boasso,bogrup,bocont,bosotc,bosco1,bosco2,bosco3,bomagg,botsc1,botsc2,botsc3,botmag,BONOTE,BOSHOW,BOCOA1,BOCOA2,BOCOA1P,BOCOA2P,BOCOA1PT,BOCOA2PT) VALUES(@bolsoc,@BTANNO,@BTBOLL,@revision,@BORIGB,@BOANNO,@BONUOR,@BORIGA,@BOMAGA,@BOCODA,@BOQTAV,@BOTQTA,@BOUNIM,@BODACO,@BOSERI,@BORIFC,@boprez,@botpre,@boaliq,@boasso,@bogrup,@bocont,@bosotc,@bosco1,@bosco2,@bosco3,@bomagg,@botsc1,@botsc2,@botsc3,@botmag,@BONOTE,@BOSHOW,@BOCOA1,@BOCOA2,@BOCOA1P,@BOCOA2P,@BOCOA1PT,@BOCOA2PT)";
    public string UPDATE_QUERY => "UPDATE BOLLD00F_history SET bolsoc = @bolsoc,BTANNO = @BTANNO,BTBOLL = @BTBOLL,revision = @revision,BORIGB = @BORIGB,BOANNO = @BOANNO,BONUOR = @BONUOR,BORIGA = @BORIGA,BOMAGA = @BOMAGA,BOCODA = @BOCODA,BOQTAV = @BOQTAV,BOTQTA = @BOTQTA,BOUNIM = @BOUNIM,BODACO = @BODACO,BOSERI = @BOSERI,BORIFC = @BORIFC,boprez = @boprez,botpre = @botpre,boaliq = @boaliq,boasso = @boasso,bogrup = @bogrup,bocont = @bocont,bosotc = @bosotc,bosco1 = @bosco1,bosco2 = @bosco2,bosco3 = @bosco3,bomagg = @bomagg,botsc1 = @botsc1,botsc2 = @botsc2,botsc3 = @botsc3,botmag = @botmag,BONOTE = @BONOTE,BOSHOW = @BOSHOW,BOCOA1 = @BOCOA1,BOCOA2 = @BOCOA2,BOCOA1P = @BOCOA1P,BOCOA2P = @BOCOA2P,BOCOA1PT = @BOCOA1PT,BOCOA2PT = @BOCOA2PT WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision AND BORIGB = @BORIGB";
    public string DELETE_QUERY => "DELETE FROM BOLLD00F_history WHERE bolsoc = @bolsoc AND BTANNO = @BTANNO AND BTBOLL = @BTBOLL AND revision = @revision AND BORIGB = @BORIGB";
    public bool Insert(BOLLD00F_history Model)
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

    public bool Update(BOLLD00F_history Model)
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

    public bool Delete(BOLLD00F_history Model)
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