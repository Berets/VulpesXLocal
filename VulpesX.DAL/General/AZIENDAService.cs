
using Microsoft.Extensions.DependencyInjection;
using VulpesX.DAL.Store;

namespace VulpesX.DAL.General;

public interface IAZIENDARepository
{
    ObservableCollection<AZIENDA>? GetList();

    AZIENDA? Get(string AZCode);

    string? GetLotTemplate(string AZCode);

    bool Exists(string AZCode);

    bool Insert(AZIENDA Model);

    bool Update(AZIENDA Model);

    bool Delete(AZIENDA Model);

    string? Validate(AZIENDA Model, bool IsInsert);
}

public class AZIENDARepository : RepositoryBase, IAZIENDARepository
{
    public AZIENDARepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AZIENDA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AZIENDA>(
                "SELECT * FROM AZIENDA");

            return new ObservableCollection<AZIENDA>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AZIENDA? Get(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<AZIENDA>(
                "SELECT * FROM AZIENDA WHERE AZCode = @AZCode",
                new { AZCode = AZCode })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public string? GetLotTemplate(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<AZIENDA>(
                "SELECT lot_template FROM AZIENDA WHERE AZCode = @AZCode",
                new { AZCode = AZCode })
                .FirstOrDefault()?.lot_template;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AZIENDA WHERE AZCode = @AZCode",
                new { AZCode = AZCode }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public static readonly string INSERT_QUERY = "INSERT INTO AZIENDA (AZCode,azrssl,azsigl,azinsl,azcasl,azlosl,azprsl,azinsa,azcasa,azlosa,azprsa,azpaiv,azcofi,azufiv,azufim,azriin,azteap,azstat,aznatu,azsitu,aztipo,azcolr,aznolr,azselr,azdnlr,azcnlr,azpnlr,azcrlr,azprlr,azcalr,azirlr,azcoca,azdaca,azcflr,aztelr,azcanu,azcapr,azsubc,azaiat,azleat,azpeat,azceat,AZTel,AZFax,AZUrl,AZMail,azsmtp,azemi,azuser,azpsw,AZIndir,AZLocal,AZProv,azcuc,azcodrea,azdirez,added,updated,addedUserID,updatedUserID,AZATTINV,azsmtpport,azusetls,azusrcrm,azusrcrmpsw,azusrsrm,azusrsrmpsw,azusrname,azusrcrmname,azusrsrmname,azoffogg,azordogg,azddtogg,azinvogg,azofftex,azordtex,azddttex,azinvtex,AZCUSOFF,AZCCOFF,AZCCORD,AZCCDDT,AZCCINV,azbuyogg,azbuytex,AZCCBUY,azaststep,AZCUSORD,AZSUPORD,lot_template,azpnotafa,azpnotoff,azpnotord,azpnotddt,azpnotinv,azagedoff,azagedord,azagedddt,azagedfat,azddtgtex,azordgtex,azoffgtex,azinvgtex,AZCUSDDT,AZCUSINV,azacqgtex,lot_locked,prices_onmoves,ordered_as_available,azcapsoc,azuseei,azapikey,azregifatt,azcodcee,azcodextracee,azisoextracee,azeipath,azsocuni,azstaliq,azdisgrp,azdiscnt,azdissot,azimpgilat,azimpgilatid,azimpgilatcau,azimpgilatalc,azimpgilatala,azddtricl,azddtdefic,azinvricl,azinvshde,azfatfile,azimpbancolat,azimpbancolatid,azimpbancolatcau,aziconchiuclifor,azlinguadefault) OUTPUT INSERTED.rv VALUES(@AZCode,@azrssl,@azsigl,@azinsl,@azcasl,@azlosl,@azprsl,@azinsa,@azcasa,@azlosa,@azprsa,@azpaiv,@azcofi,@azufiv,@azufim,@azriin,@azteap,@azstat,@aznatu,@azsitu,@aztipo,@azcolr,@aznolr,@azselr,@azdnlr,@azcnlr,@azpnlr,@azcrlr,@azprlr,@azcalr,@azirlr,@azcoca,@azdaca,@azcflr,@aztelr,@azcanu,@azcapr,@azsubc,@azaiat,@azleat,@azpeat,@azceat,@AZTel,@AZFax,@AZUrl,@AZMail,@azsmtp,@azemi,@azuser,@azpsw,@AZIndir,@AZLocal,@AZProv,@azcuc,@azcodrea,@azdirez,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@addedUserID,@updatedUserID,@AZATTINV,@azsmtpport,@azusetls,@azusrcrm,@azusrcrmpsw,@azusrsrm,@azusrsrmpsw,@azusrname,@azusrcrmname,@azusrsrmname,@azoffogg,@azordogg,@azddtogg,@azinvogg,@azofftex,@azordtex,@azddttex,@azinvtex,@AZCUSOFF,@AZCCOFF,@AZCCORD,@AZCCDDT,@AZCCINV,@azbuyogg,@azbuytex,@AZCCBUY,@azaststep,@AZCUSORD,@AZSUPORD,@lot_template,@azpnotafa,@azpnotoff,@azpnotord,@azpnotddt,@azpnotinv,@azagedoff,@azagedord,@azagedddt,@azagedfat,@azddtgtex,@azordgtex,@azoffgtex,@azinvgtex,@AZCUSDDT,@AZCUSINV,@azacqgtex,@lot_locked,@prices_onmoves,@ordered_as_available,@azcapsoc,@azuseei,@azapikey,@azregifatt,@azcodcee,@azcodextracee,@azisoextracee,@azeipath,@azsocuni,@azstaliq,@azdisgrp,@azdiscnt,@azdissot,@azimpgilat,@azimpgilatid,@azimpgilatcau,@azimpgilatalc,@azimpgilatala,@azddtricl,@azddtdefic,@azinvricl,@azinvshde,@azfatfile,@azimpbancolat,@azimpbancolatid,@azimpbancolatcau,@aziconchiuclifor,@azlinguadefault)";
    public static readonly string UPDATE_QUERY = "UPDATE AZIENDA SET AZCode = @AZCode,azrssl = @azrssl,azsigl = @azsigl,azinsl = @azinsl,azcasl = @azcasl,azlosl = @azlosl,azprsl = @azprsl,azinsa = @azinsa,azcasa = @azcasa,azlosa = @azlosa,azprsa = @azprsa,azpaiv = @azpaiv,azcofi = @azcofi,azufiv = @azufiv,azufim = @azufim,azriin = @azriin,azteap = @azteap,azstat = @azstat,aznatu = @aznatu,azsitu = @azsitu,aztipo = @aztipo,azcolr = @azcolr,aznolr = @aznolr,azselr = @azselr,azdnlr = @azdnlr,azcnlr = @azcnlr,azpnlr = @azpnlr,azcrlr = @azcrlr,azprlr = @azprlr,azcalr = @azcalr,azirlr = @azirlr,azcoca = @azcoca,azdaca = @azdaca,azcflr = @azcflr,aztelr = @aztelr,azcanu = @azcanu,azcapr = @azcapr,azsubc = @azsubc,azaiat = @azaiat,azleat = @azleat,azpeat = @azpeat,azceat = @azceat,AZTel = @AZTel,AZFax = @AZFax,AZUrl = @AZUrl,AZMail = @AZMail,azsmtp = @azsmtp,azemi = @azemi,azuser = @azuser,azpsw = @azpsw,AZIndir = @AZIndir,AZLocal = @AZLocal,AZProv = @AZProv,azcuc = @azcuc,azcodrea = @azcodrea,azdirez = @azdirez,added = @added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',addedUserID = @addedUserID,updatedUserID = @updatedUserID,AZATTINV = @AZATTINV,azsmtpport = @azsmtpport,azusetls = @azusetls,azusrcrm = @azusrcrm,azusrcrmpsw = @azusrcrmpsw,azusrsrm = @azusrsrm,azusrsrmpsw = @azusrsrmpsw,azusrname = @azusrname,azusrcrmname = @azusrcrmname,azusrsrmname = @azusrsrmname,azoffogg = @azoffogg,azordogg = @azordogg,azddtogg = @azddtogg,azinvogg = @azinvogg,azofftex = @azofftex,azordtex = @azordtex,azddttex = @azddttex,azinvtex = @azinvtex,AZCUSOFF = @AZCUSOFF,AZCCOFF = @AZCCOFF,AZCCORD = @AZCCORD,AZCCDDT = @AZCCDDT,AZCCINV = @AZCCINV,azbuyogg = @azbuyogg,azbuytex = @azbuytex,AZCCBUY = @AZCCBUY,azaststep = @azaststep,AZCUSORD = @AZCUSORD,AZSUPORD = @AZSUPORD,lot_template = @lot_template,azpnotafa = @azpnotafa,azpnotoff = @azpnotoff,azpnotord = @azpnotord,azpnotddt = @azpnotddt,azpnotinv = @azpnotinv,azagedoff = @azagedoff,azagedord = @azagedord,azagedddt = @azagedddt,azagedfat = @azagedfat,azddtgtex = @azddtgtex,azordgtex = @azordgtex,azoffgtex = @azoffgtex,azinvgtex = @azinvgtex,AZCUSDDT = @AZCUSDDT,AZCUSINV = @AZCUSINV,azacqgtex = @azacqgtex,lot_locked = @lot_locked,prices_onmoves = @prices_onmoves,ordered_as_available = @ordered_as_available,azcapsoc = @azcapsoc,azuseei = @azuseei,azapikey = @azapikey,azregifatt = @azregifatt,azcodcee = @azcodcee,azcodextracee = @azcodextracee,azisoextracee = @azisoextracee,azeipath = @azeipath,azsocuni = @azsocuni,azstaliq = @azstaliq,azdisgrp = @azdisgrp,azdiscnt = @azdiscnt,azdissot = @azdissot,azimpgilat = @azimpgilat,azimpgilatid = @azimpgilatid,azimpgilatcau = @azimpgilatcau,azimpgilatalc = @azimpgilatalc,azimpgilatala = @azimpgilatala,azddtricl = @azddtricl,azddtdefic = @azddtdefic,azinvricl = @azinvricl,azinvshde = @azinvshde,azfatfile = @azfatfile,azimpbancolat = @azimpbancolat,azimpbancolatid = @azimpbancolatid,azimpbancolatcau = @azimpbancolatcau, aziconchiuclifor=@aziconchiuclifor,azlinguadefault=@azlinguadefault OUTPUT INSERTED.rv WHERE AZCode = @AZCode AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM AZIENDA OUTPUT DELETED.rv WHERE AZCode = @AZCode AND rv = @rv";
    public bool Insert(AZIENDA Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            // normalization
            Model.azimpgilatid = Model.azimpgilatid?.ToLower();
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

    public bool Update(AZIENDA Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            // normalization
            Model.azimpgilatid = Model.azimpgilatid?.ToLower();
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

    public bool Delete(AZIENDA Model)
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

    public string? Validate(AZIENDA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.AZCode) && IsInsert && !Exists(Model.AZCode)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.azrssl))
                {
                    var lotPreview = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GenerateLotID(Model.lot_template, new Random().Next(999999), new Random().Next(999999), 30);
                    if (string.IsNullOrWhiteSpace(Model.lot_template) || (!string.IsNullOrWhiteSpace(Model.lot_template) && lotPreview != null && lotPreview.Length <= 50))
                    {
                        if (string.IsNullOrWhiteSpace(Model.azeipath) || (!string.IsNullOrWhiteSpace(Model.azeipath) && Path.Exists(Model.azeipath)))
                        {
                            if ((Model.azimpgilat && !string.IsNullOrWhiteSpace(Model.azimpgilatid) && !string.IsNullOrWhiteSpace(Model.azimpgilatcau) && !string.IsNullOrWhiteSpace(Model.azimpgilatalc) && !string.IsNullOrWhiteSpace(Model.azimpgilatala)) ||
                                !Model.azimpgilat)
                            {
                                return null;
                            }
                            else
                            { return "Se si abilita l'importazione GILAT è necessario specificare il codice per la decodifica, la cuasale del DDT e l'aliquota per eventuali omaggi"; }
                        }
                        else
                        { return "Il percorso dove salvare i file XML non e' valido"; }
                    }
                    else
                    { return "Il template per il lotto produce un identificativo troppo lungo"; }
                }
                else
                { return "La descrizione è obbligatoria"; }
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

public class AZIENDAUfpRepository : RepositoryBase, IAZIENDARepository
{
    public AZIENDAUfpRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<AZIENDA>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();

            var list = connection.Query<AZIENDA>(
                "SELECT * FROM AZIENDA");

            return new ObservableCollection<AZIENDA>(list);
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public AZIENDA? Get(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();


            return connection.Query<AZIENDA>(
                "SELECT * FROM AZIENDA WHERE AZCode = @AZCode",
                new { AZCode = AZCode })
                .FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public string? GetLotTemplate(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return connection.Query<AZIENDA>(
                "SELECT lot_template FROM AZIENDA WHERE AZCode = @AZCode",
                new { AZCode = AZCode })
                .FirstOrDefault()?.lot_template;

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public bool Exists(string AZCode)
    {
        try
        {
            using var connection = GetOpenConnection();

            return (int?)connection.ExecuteScalar(
                "SELECT COUNT(*) FROM AZIENDA WHERE AZCode = @AZCode",
                new { AZCode = AZCode }) > 0;
        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return true;
        }
    }

    #region CRUD
    public static readonly string INSERT_QUERY = @"INSERT INTO AZIENDA (
    AZCode,
	azrssl,
	azsigl,
	azinsl,
	azcasl,
	azlosl,
	azprsl,
	azca2z,
	azinsa,
	azcasa,
	azlosa,
	azprsa,
	azsa2c,
	azpaiv,
	azcofi,
	azufiv,
	azufim,
	azriin,
	azteap,
	azstat,
	aznatu,
	azsitu,
	aztipo,
	azvalu,
	azcolr,
	aznolr,
	azselr,
	azdnlr,
	azcnlr,
	azpnlr,
	azp2lr,
	azcrlr,
	azprlr,
	azprl2,
	azcalr,
	azirlr,
	azcoca,
	azdaca,
	azcflr,
	aztelr,
	azcanu,
	azcapr,
	azsubc,
	azaiat,
	azleat,
	azpeat,
	azpec2,
	azceat,
	azieat,
	azdddr,
	azddf7,
	azcfdi,
	azcodi,
	aznodi,
	azindi,
	azcadi,
	azlodi,
	azprdi,
	azdi2c,
	aztedi,
	azlast,
	AZTel,
	AZFax,
	AZUrl,
	AZMail,
	azpth,
	azdoc,
	azsmtp,
	azemi,
	azuser,
	azpsw,
	AZRagso,
	AZIndir,
	AZIndir2,
	AZLocal,
	AZCap,
	AZProv,
	Azban,
	azpca,
	azpcp,
	azplc,
	azindsito,
	azcapsoc,
	azcapintver,
	azloccamcom,
	aznumcam,
	azlocreg,
	aznumreg,
	azcripto,
	azlinguadefault,
	addedUserID,
	updatedUserID,
	added,
	updated) OUTPUT INSERTED.rv VALUES(
        @AZCode,
	    @azrssl,
	    @azsigl,
	    @azinsl,
	    @azcasl,
	    @azlosl,
	    @azprsl,
	    @azca2z,
	    @azinsa,
	    @azcasa,
	    @azlosa,
	    @azprsa,
	    @azsa2c,
	    @azpaiv,
	    @azcofi,
	    @azufiv,
	    @azufim,
	    @azriin,
	    @azteap,
	    @azstat,
	    @aznatu,
	    @azsitu,
	    @aztipo,
	    @azvalu,
	    @azcolr,
	    @aznolr,
	    @azselr,
	    @azdnlr,
	    @azcnlr,
	    @azpnlr,
	    @azp2lr,
	    @azcrlr,
	    @azprlr,
	    @azprl2,
	    @azcalr,
	    @azirlr,
	    @azcoca,
	    @azdaca,
	    @azcflr,
	    @aztelr,
	    @azcanu,
	    @azcapr,
	    @azsubc,
	    @azaiat,
	    @azleat,
	    @azpeat,
	    @azpec2,
	    @azceat,
	    @azieat,
	    @azdddr,
	    @azddf7,
	    @azcfdi,
	    @azcodi,
	    @aznodi,
	    @azindi,
	    @azcadi,
	    @azlodi,
	    @azprdi,
	    @azdi2c,
	    @aztedi,
	    @azlast,
	    @AZTel,
	    @AZFax,
	    @AZUrl,
	    @AZMail,
	    @azpth,
	    @azdoc,
	    @azsmtp,
	    @azemi,
	    @azuser,
	    @azpsw,
	    @AZRagso,
	    @AZIndir,
	    @AZIndir2,
	    @AZLocal,
	    @AZCap,
	    @AZProv,
	    @Azban,
	    @azpca,
	    @azpcp,
	    @azplc,
	    @azindsito,
	    @azcapsoc,
	    @azcapintver,
	    @azloccamcom,
	    @aznumcam,
	    @azlocreg,
	    @aznumreg,
	    @azcripto,
	    @azlinguadefault,
	    @addedUserID,
	    @updatedUserID,
	    SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',
	    @updated
    )";
    public static readonly string UPDATE_QUERY = @"UPDATE AZIENDA SET 
    AZCode = @AZCode,
	azrssl = @azrssl,
	azsigl = @azsigl,
	azinsl = @azinsl,
	azcasl = @azcasl,
	azlosl = @azlosl,
	azprsl = @azprsl,
	azca2z = @azca2z,
	azinsa = @azinsa,
	azcasa = @azcasa,
	azlosa = @azlosa,
	azprsa = @azprsa,
	azsa2c = @azsa2c,
	azpaiv = @azpaiv,
	azcofi = @azcofi,
	azufiv = @azufiv,
	azufim = @azufim,
	azriin = @azriin,
	azteap = @azteap,
	azstat = @azstat,
	aznatu = @aznatu,
	azsitu = @azsitu,
	aztipo = @aztipo,
	azvalu = @azvalu,
	azcolr = @azcolr,
	aznolr = @aznolr,
	azselr = @azselr,
	azdnlr = @azdnlr,
	azcnlr = @azcnlr,
	azpnlr = @azpnlr,
	azp2lr = @azp2lr,
	azcrlr = @azcrlr,
	azprlr = @azprlr,
	azprl2 = @azprl2,
	azcalr = @azcalr,
	azirlr = @azirlr,
	azcoca = @azcoca,
	azdaca = @azdaca,
	azcflr = @azcflr,
	aztelr = @aztelr,
	azcanu = @azcanu,
	azcapr = @azcapr,
	azsubc = @azsubc,
	azaiat = @azaiat,
	azleat = @azleat,
	azpeat = @azpeat,
	azpec2 = @azpec2,
	azceat = @azceat,
	azieat = @azieat,
	azdddr = @azdddr,
	azddf7 = @azddf7,
	azcfdi = @azcfdi,
	azcodi = @azcodi,
	aznodi = @aznodi,
	azindi = @azindi,
	azcadi = @azcadi,
	azlodi = @azlodi,
	azprdi = @azprdi,
	azdi2c = @azdi2c,
	aztedi = @aztedi,
	azlast = @azlast,
	AZTel = @AZTel,
	AZFax = @AZFax,
	AZUrl = @AZUrl,
	AZMail = @AZMail,
	azpth = @azpth,
	azdoc = @azdoc,
	azsmtp = @azsmtp,
	azemi =@azemi,
	azuser=@azuser,
	azpsw=@azpsw,
	AZRagso=@AZRagso,
	AZIndir=@AZIndir,
	AZIndir2=@AZIndir2,
	AZLocal=@AZLocal,
	AZCap=@AZCap,
	AZProv=@AZProv,
	Azban=@Azban,
	azpca=@azpca,
	azpcp=@azpcp,
	azplc=@azplc,
	azindsito=@azindsito,
	azcapsoc=@azcapsoc,
	azcapintver=@azcapintver,
	azloccamcom=@azloccamcom,
	aznumcam=@aznumcam,
	azlocreg = @azlocreg,
	aznumreg = @aznumreg,
	azcripto = @azcripto,
	azlinguadefault=@azlinguadefault,
	addedUserID=@addedUserID,
	updatedUserID=@updatedUserID,
	added=@added,
	updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time'
    OUTPUT INSERTED.rv WHERE AZCode = @AZCode AND rv = @rv";
    public static readonly string DELETE_QUERY = "DELETE FROM AZIENDA OUTPUT DELETED.rv WHERE AZCode = @AZCode AND rv = @rv";
    public bool Insert(AZIENDA Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            // normalization
            Model.azimpgilatid = Model.azimpgilatid?.ToLower();
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

    public bool Update(AZIENDA Model)
    {
        try
        {
            using var connection = GetOpenConnection();

            // normalization
            Model.azimpgilatid = Model.azimpgilatid?.ToLower();
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

    public bool Delete(AZIENDA Model)
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

    public string? Validate(AZIENDA Model, bool IsInsert)
    {
        try
        {
            if ((!string.IsNullOrEmpty(Model.AZCode) && IsInsert && !Exists(Model.AZCode)) || !IsInsert)
            {
                if (!string.IsNullOrWhiteSpace(Model.azrssl))
                {
                    var lotPreview = VulpesServiceProvider.Provider.GetRequiredService<Istore_movementsRepository>().GenerateLotID(Model.lot_template, new Random().Next(999999), new Random().Next(999999), 30);
                    if (string.IsNullOrWhiteSpace(Model.lot_template) || (!string.IsNullOrWhiteSpace(Model.lot_template) && lotPreview != null && lotPreview.Length <= 50))
                    {
                        if (string.IsNullOrWhiteSpace(Model.azeipath) || (!string.IsNullOrWhiteSpace(Model.azeipath) && Path.Exists(Model.azeipath)))
                        {
                            if ((Model.azimpgilat && !string.IsNullOrWhiteSpace(Model.azimpgilatid) && !string.IsNullOrWhiteSpace(Model.azimpgilatcau) && !string.IsNullOrWhiteSpace(Model.azimpgilatalc) && !string.IsNullOrWhiteSpace(Model.azimpgilatala)) ||
                                !Model.azimpgilat)
                            {
                                return null;
                            }
                            else
                            { return "Se si abilita l'importazione GILAT è necessario specificare il codice per la decodifica, la cuasale del DDT e l'aliquota per eventuali omaggi"; }
                        }
                        else
                        { return "Il percorso dove salvare i file XML non e' valido"; }
                    }
                    else
                    { return "Il template per il lotto produce un identificativo troppo lungo"; }
                }
                else
                { return "La descrizione è obbligatoria"; }
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