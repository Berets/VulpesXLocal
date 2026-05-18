
using Dapper;

namespace VulpesX.DAL.CRM;

public interface IANFAT00FRepository
{
    ObservableCollection<ANFAT00F>? GetList(string CompanyID, int? Year, string Status);

    ANFAT00F? Get(string AFSOCI, int AFANNO, int AFNUOR);

    bool Exists(string AFSOCI, int AFANNO, int AFNUOR);

    bool Insert(ANFAT00F Model);

    bool Update(ANFAT00F Model);

    bool Delete(ANFAT00F Model);

    string? Validate(ANFAT00F Model, bool IsInsert);
}

public class ANFAT00FRepository : RepositoryBase, IANFAT00FRepository
{
    public ANFAT00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<ANFAT00F>? GetList(string CompanyID, int? Year, string Status)
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<ANFAT00F, int, ABE, DESTINATARI, ANFAT00F>(
                $@"SELECT a.*, (SELECT COUNT(*) FROM ANFAD00F AS r WHERE r.AFSOCI=a.AFSOCI AND r.AFANNO=a.AFANNO AND r.AFNUOR=a.AFNUOR) AS rc, c.abecod, c.abers1, c.abers2, d.codesti, d.ragisoc FROM ANFAT00F AS a
                        LEFT JOIN ABE AS c ON c.abecod=a.AFCOCL
                        LEFT JOIN DESTINATARI AS d ON d.cliecod=a.AFCOCL AND d.codesti=a.AFDEST
                        WHERE a.AFSOCI=@cid {(Year.HasValue && Year.Value > 0 ? "AND a.AFANNO = @yea" : null)} 
                            {(Status == "*" ? null : string.IsNullOrWhiteSpace(Status) ? " AND a.AFSTAT IS NULL " : Status == "X" ? " AND a.canceled IS NOT NULL " : " AND a.AFSTAT=@sta ")}
                        ORDER BY a.AFANNO DESC, a.AFNUOR DESC",
                (af, rc, abe, des) => { af.RowsCount = rc; af.Customer = abe; af.Recipient = des; return af; },
                new { cid = CompanyID, yea = Year, sta = Status }, splitOn: "rc,abecod,codesti");

            return new ObservableCollection<ANFAT00F>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ANFAT00F? Get(string AFSOCI, int AFANNO, int AFNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<ANFAT00F, ABE, ANFAT00F>(
                @"SELECT a.*, b.abecod, b.abers1, b.abers2 FROM ANFAT00F AS a
                        LEFT JOIN ABE AS b ON b.abecod=a.AFCOCL
                        WHERE a.AFSOCI = @AFSOCI AND a.AFANNO = @AFANNO AND a.AFNUOR = @AFNUOR",
                (anf, abe) => { anf.Customer = abe; return anf; },
                new { AFSOCI, AFANNO, AFNUOR }, splitOn: "abecod").FirstOrDefault();

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string AFSOCI, int AFANNO, int AFNUOR)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM ANFAT00F WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR",
                new { AFSOCI, AFANNO, AFNUOR }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public readonly string INSERT_QUERY = "INSERT INTO ANFAT00F (AFSOCI,AFANNO,AFNUOR,AFDAOR,AFCOCL,AFDEST,AFRASO,AFINDI,AFPIVA,AFCOFI,AFLOCA,AFPROV,AFTELE,AFNFAX,AFMAIL,AFCELL,AFCAPP,AFCIDI,AFDIVI,AFRIFE,AFOGGE,AFSTAT,added,updated,canceled,addedUserID,updatedUserID,canceledUserID,canceledNote,AFNOTET,AFNOTEP,AFSHOWT,AFSHOWP,AFLING) OUTPUT INSERTED.rv VALUES(@AFSOCI,@AFANNO,@AFNUOR,@AFDAOR,@AFCOCL,@AFDEST,@AFRASO,@AFINDI,@AFPIVA,@AFCOFI,@AFLOCA,@AFPROV,@AFTELE,@AFNFAX,@AFMAIL,@AFCELL,@AFCAPP,@AFCIDI,@AFDIVI,@AFRIFE,@AFOGGE,@AFSTAT,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updatedUserID,@canceledUserID,@canceledNote,@AFNOTET,@AFNOTEP,@AFSHOWT,@AFSHOWP,@AFLING)";
    public readonly string UPDATE_QUERY = "UPDATE ANFAT00F SET AFSOCI = @AFSOCI,AFANNO = @AFANNO,AFNUOR = @AFNUOR,AFDAOR = @AFDAOR,AFCOCL = @AFCOCL,AFDEST = @AFDEST,AFRASO = @AFRASO,AFINDI = @AFINDI,AFPIVA = @AFPIVA,AFCOFI = @AFCOFI,AFLOCA = @AFLOCA,AFPROV = @AFPROV,AFTELE = @AFTELE,AFNFAX = @AFNFAX,AFMAIL = @AFMAIL,AFCELL = @AFCELL,AFCAPP = @AFCAPP,AFCIDI = @AFCIDI,AFDIVI = @AFDIVI,AFRIFE = @AFRIFE,AFOGGE = @AFOGGE,AFSTAT = @AFSTAT,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled = @canceled,addedUserID = @addedUserID,updatedUserID = @updatedUserID,canceledUserID = @canceledUserID,canceledNote = @canceledNote,AFNOTET = @AFNOTET,AFNOTEP = @AFNOTEP,AFSHOWT = @AFSHOWT,AFSHOWP = @AFSHOWP,AFLING = @AFLING OUTPUT INSERTED.rv WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND rv = @rv";
    public readonly string DELETE_QUERY = "DELETE FROM ANFAT00F OUTPUT DELETED.rv WHERE AFSOCI = @AFSOCI AND AFANNO = @AFANNO AND AFNUOR = @AFNUOR AND rv = @rv";
    public bool Insert(ANFAT00F Model)
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

    public bool Update(ANFAT00F Model)
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

    public bool Delete(ANFAT00F Model)
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

    public string? Validate(ANFAT00F Model, bool IsInsert)
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.AFSOCI) && IsInsert && !Exists(Model.AFSOCI, Model.AFANNO, Model.AFNUOR) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.AFOGGE))
                {
                    return null;
                }
                else
                { return "L'oggetto dell'analisi di fattibilità è obbligatorio"; }
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