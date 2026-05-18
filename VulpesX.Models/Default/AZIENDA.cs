namespace VulpesX.Models.Default;
 
public partial class AZIENDA : Base 
{
	private string _AZCode = null!;
	public required string AZCode { get => _AZCode; set { if (_AZCode != value) { _AZCode = value; NotifyPropertyChanged();} } }
	private string _azrssl = null!;
	public required string azrssl { get => _azrssl; set { if (_azrssl != value) { _azrssl = value; NotifyPropertyChanged();} } }
	private string? _azsigl;
	public string? azsigl { get => _azsigl; set { if (_azsigl != value) { _azsigl = value; NotifyPropertyChanged();} } }
	private string? _azinsl;
	public string? azinsl { get => _azinsl; set { if (_azinsl != value) { _azinsl = value; NotifyPropertyChanged();} } }
	private int? _azcasl;
	public int? azcasl { get => _azcasl; set { if (_azcasl != value) { _azcasl = value; NotifyPropertyChanged();} } }
	private string? _azlosl;
	public string? azlosl { get => _azlosl; set { if (_azlosl != value) { _azlosl = value; NotifyPropertyChanged();} } }
	private string? _azprsl;
	public string? azprsl { get => _azprsl; set { if (_azprsl != value) { _azprsl = value; NotifyPropertyChanged();} } }
	private string? _azinsa;
	public string? azinsa { get => _azinsa; set { if (_azinsa != value) { _azinsa = value; NotifyPropertyChanged();} } }
	private int? _azcasa;
	public int? azcasa { get => _azcasa; set { if (_azcasa != value) { _azcasa = value; NotifyPropertyChanged();} } }
	private string? _azlosa;
	public string? azlosa { get => _azlosa; set { if (_azlosa != value) { _azlosa = value; NotifyPropertyChanged();} } }
	private string? _azprsa;
	public string? azprsa { get => _azprsa; set { if (_azprsa != value) { _azprsa = value; NotifyPropertyChanged();} } }
	private string? _azpaiv;
	public string? azpaiv { get => _azpaiv; set { if (_azpaiv != value) { _azpaiv = value; NotifyPropertyChanged();} } }
	private string? _azcofi;
	public string? azcofi { get => _azcofi; set { if (_azcofi != value) { _azcofi = value; NotifyPropertyChanged();} } }
	private string? _azufiv;
	public string? azufiv { get => _azufiv; set { if (_azufiv != value) { _azufiv = value; NotifyPropertyChanged();} } }
	private string? _azufim;
	public string? azufim { get => _azufim; set { if (_azufim != value) { _azufim = value; NotifyPropertyChanged();} } }
	private string? _azriin;
	public string? azriin { get => _azriin; set { if (_azriin != value) { _azriin = value; NotifyPropertyChanged();} } }
	private DateTime? _azteap;
	public DateTime? azteap { get => _azteap; set { if (_azteap != value) { _azteap = value; NotifyPropertyChanged();} } }
	private string? _azstat;
	public string? azstat { get => _azstat; set { if (_azstat != value) { _azstat = value; NotifyPropertyChanged();} } }
	private int? _aznatu;
	public int? aznatu { get => _aznatu; set { if (_aznatu != value) { _aznatu = value; NotifyPropertyChanged();} } }
	private int? _azsitu;
	public int? azsitu { get => _azsitu; set { if (_azsitu != value) { _azsitu = value; NotifyPropertyChanged();} } }
	private string? _aztipo;
	public string? aztipo { get => _aztipo; set { if (_aztipo != value) { _aztipo = value; NotifyPropertyChanged();} } }
	private string? _azcolr;
	public string? azcolr { get => _azcolr; set { if (_azcolr != value) { _azcolr = value; NotifyPropertyChanged();} } }
	private string? _aznolr;
	public string? aznolr { get => _aznolr; set { if (_aznolr != value) { _aznolr = value; NotifyPropertyChanged();} } }
	private string? _azselr;
	public string? azselr { get => _azselr; set { if (_azselr != value) { _azselr = value; NotifyPropertyChanged();} } }
	private DateTime? _azdnlr;
	public DateTime? azdnlr { get => _azdnlr; set { if (_azdnlr != value) { _azdnlr = value; NotifyPropertyChanged();} } }
	private string? _azcnlr;
	public string? azcnlr { get => _azcnlr; set { if (_azcnlr != value) { _azcnlr = value; NotifyPropertyChanged();} } }
	private string? _azpnlr;
	public string? azpnlr { get => _azpnlr; set { if (_azpnlr != value) { _azpnlr = value; NotifyPropertyChanged();} } }
	private string? _azcrlr;
	public string? azcrlr { get => _azcrlr; set { if (_azcrlr != value) { _azcrlr = value; NotifyPropertyChanged();} } }
	private string? _azprlr;
	public string? azprlr { get => _azprlr; set { if (_azprlr != value) { _azprlr = value; NotifyPropertyChanged();} } }
	private int? _azcalr;
	public int? azcalr { get => _azcalr; set { if (_azcalr != value) { _azcalr = value; NotifyPropertyChanged();} } }
	private string? _azirlr;
	public string? azirlr { get => _azirlr; set { if (_azirlr != value) { _azirlr = value; NotifyPropertyChanged();} } }
	private string? _azcoca;
	public string? azcoca { get => _azcoca; set { if (_azcoca != value) { _azcoca = value; NotifyPropertyChanged();} } }
	private DateTime? _azdaca;
	public DateTime? azdaca { get => _azdaca; set { if (_azdaca != value) { _azdaca = value; NotifyPropertyChanged();} } }
	private string? _azcflr;
	public string? azcflr { get => _azcflr; set { if (_azcflr != value) { _azcflr = value; NotifyPropertyChanged();} } }
	private string? _aztelr;
	public string? aztelr { get => _aztelr; set { if (_aztelr != value) { _aztelr = value; NotifyPropertyChanged();} } }
	private int? _azcanu;
	public int? azcanu { get => _azcanu; set { if (_azcanu != value) { _azcanu = value; NotifyPropertyChanged();} } }
	private int? _azcapr;
	public int? azcapr { get => _azcapr; set { if (_azcapr != value) { _azcapr = value; NotifyPropertyChanged();} } }
	private int? _azsubc;
	public int? azsubc { get => _azsubc; set { if (_azsubc != value) { _azsubc = value; NotifyPropertyChanged();} } }
	private int? _azaiat;
	public int? azaiat { get => _azaiat; set { if (_azaiat != value) { _azaiat = value; NotifyPropertyChanged();} } }
	private string? _azleat;
	public string? azleat { get => _azleat; set { if (_azleat != value) { _azleat = value; NotifyPropertyChanged();} } }
	private string? _azpeat;
	public string? azpeat { get => _azpeat; set { if (_azpeat != value) { _azpeat = value; NotifyPropertyChanged();} } }
	private int? _azceat;
	public int? azceat { get => _azceat; set { if (_azceat != value) { _azceat = value; NotifyPropertyChanged();} } }
	private string? _AZTel;
	public string? AZTel { get => _AZTel; set { if (_AZTel != value) { _AZTel = value; NotifyPropertyChanged();} } }
	private string? _AZFax;
	public string? AZFax { get => _AZFax; set { if (_AZFax != value) { _AZFax = value; NotifyPropertyChanged();} } }
	private string? _AZUrl;
	public string? AZUrl { get => _AZUrl; set { if (_AZUrl != value) { _AZUrl = value; NotifyPropertyChanged();} } }
	private string? _AZMail;
	public string? AZMail { get => _AZMail; set { if (_AZMail != value) { _AZMail = value; NotifyPropertyChanged();} } }
	private string? _azsmtp;
	public string? azsmtp { get => _azsmtp; set { if (_azsmtp != value) { _azsmtp = value; NotifyPropertyChanged();} } }
	private string? _azemi;
	public string? azemi { get => _azemi; set { if (_azemi != value) { _azemi = value; NotifyPropertyChanged();} } }
	private string? _azuser;
	public string? azuser { get => _azuser; set { if (_azuser != value) { _azuser = value; NotifyPropertyChanged();} } }
	private string? _azpsw;
	public string? azpsw { get => _azpsw; set { if (_azpsw != value) { _azpsw = value; NotifyPropertyChanged();} } }
	private string? _AZIndir;
	public string? AZIndir { get => _AZIndir; set { if (_AZIndir != value) { _AZIndir = value; NotifyPropertyChanged();} } }
	private string? _AZLocal;
	public string? AZLocal { get => _AZLocal; set { if (_AZLocal != value) { _AZLocal = value; NotifyPropertyChanged();} } }
	private string? _AZProv;
	public string? AZProv { get => _AZProv; set { if (_AZProv != value) { _AZProv = value; NotifyPropertyChanged();} } }
	private string? _azcuc;
	public string? azcuc { get => _azcuc; set { if (_azcuc != value) { _azcuc = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _azcodrea;
	public string? azcodrea { get => _azcodrea; set { if (_azcodrea != value) { _azcodrea = value; NotifyPropertyChanged();} } }
	private string? _azdirez;
	public string? azdirez { get => _azdirez; set { if (_azdirez != value) { _azdirez = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private bool _AZATTINV;
	public bool AZATTINV { get => _AZATTINV; set { if (_AZATTINV != value) { _AZATTINV = value; NotifyPropertyChanged();} } }
	private int? _azsmtpport;
	public int? azsmtpport { get => _azsmtpport; set { if (_azsmtpport != value) { _azsmtpport = value; NotifyPropertyChanged();} } }
	private bool? _azusetls;
	public bool? azusetls { get => _azusetls; set { if (_azusetls != value) { _azusetls = value; NotifyPropertyChanged();} } }
	private string? _azusrcrm;
	public string? azusrcrm { get => _azusrcrm; set { if (_azusrcrm != value) { _azusrcrm = value; NotifyPropertyChanged();} } }
	private string? _azusrcrmpsw;
	public string? azusrcrmpsw { get => _azusrcrmpsw; set { if (_azusrcrmpsw != value) { _azusrcrmpsw = value; NotifyPropertyChanged();} } }
	private string? _azusrsrm;
	public string? azusrsrm { get => _azusrsrm; set { if (_azusrsrm != value) { _azusrsrm = value; NotifyPropertyChanged();} } }
	private string? _azusrsrmpsw;
	public string? azusrsrmpsw { get => _azusrsrmpsw; set { if (_azusrsrmpsw != value) { _azusrsrmpsw = value; NotifyPropertyChanged();} } }
	private string? _azusrname;
	public string? azusrname { get => _azusrname; set { if (_azusrname != value) { _azusrname = value; NotifyPropertyChanged();} } }
	private string? _azusrcrmname;
	public string? azusrcrmname { get => _azusrcrmname; set { if (_azusrcrmname != value) { _azusrcrmname = value; NotifyPropertyChanged();} } }
	private string? _azusrsrmname;
	public string? azusrsrmname { get => _azusrsrmname; set { if (_azusrsrmname != value) { _azusrsrmname = value; NotifyPropertyChanged();} } }
	private string? _azoffogg;
	public string? azoffogg { get => _azoffogg; set { if (_azoffogg != value) { _azoffogg = value; NotifyPropertyChanged();} } }
	private string? _azordogg;
	public string? azordogg { get => _azordogg; set { if (_azordogg != value) { _azordogg = value; NotifyPropertyChanged();} } }
	private string? _azddtogg;
	public string? azddtogg { get => _azddtogg; set { if (_azddtogg != value) { _azddtogg = value; NotifyPropertyChanged();} } }
	private string? _azinvogg;
	public string? azinvogg { get => _azinvogg; set { if (_azinvogg != value) { _azinvogg = value; NotifyPropertyChanged();} } }
	private string? _azofftex;
	public string? azofftex { get => _azofftex; set { if (_azofftex != value) { _azofftex = value; NotifyPropertyChanged();} } }
	private string? _azordtex;
	public string? azordtex { get => _azordtex; set { if (_azordtex != value) { _azordtex = value; NotifyPropertyChanged();} } }
	private string? _azddttex;
	public string? azddttex { get => _azddttex; set { if (_azddttex != value) { _azddttex = value; NotifyPropertyChanged();} } }
	private string? _azinvtex;
	public string? azinvtex { get => _azinvtex; set { if (_azinvtex != value) { _azinvtex = value; NotifyPropertyChanged();} } }
	private bool _AZCUSOFF;
	public bool AZCUSOFF { get => _AZCUSOFF; set { if (_AZCUSOFF != value) { _AZCUSOFF = value; NotifyPropertyChanged();} } }
	private bool _AZCCOFF;
	public bool AZCCOFF { get => _AZCCOFF; set { if (_AZCCOFF != value) { _AZCCOFF = value; NotifyPropertyChanged();} } }
	private bool _AZCCORD;
	public bool AZCCORD { get => _AZCCORD; set { if (_AZCCORD != value) { _AZCCORD = value; NotifyPropertyChanged();} } }
	private bool _AZCCDDT;
	public bool AZCCDDT { get => _AZCCDDT; set { if (_AZCCDDT != value) { _AZCCDDT = value; NotifyPropertyChanged();} } }
	private bool _AZCCINV;
	public bool AZCCINV { get => _AZCCINV; set { if (_AZCCINV != value) { _AZCCINV = value; NotifyPropertyChanged();} } }
	private string? _azbuyogg;
	public string? azbuyogg { get => _azbuyogg; set { if (_azbuyogg != value) { _azbuyogg = value; NotifyPropertyChanged();} } }
	private string? _azbuytex;
	public string? azbuytex { get => _azbuytex; set { if (_azbuytex != value) { _azbuytex = value; NotifyPropertyChanged();} } }
	private bool _AZCCBUY;
	public bool AZCCBUY { get => _AZCCBUY; set { if (_AZCCBUY != value) { _AZCCBUY = value; NotifyPropertyChanged();} } }
	private bool _azaststep;
	public bool azaststep { get => _azaststep; set { if (_azaststep != value) { _azaststep = value; NotifyPropertyChanged();} } }
	private bool _AZCUSORD;
	public bool AZCUSORD { get => _AZCUSORD; set { if (_AZCUSORD != value) { _AZCUSORD = value; NotifyPropertyChanged();} } }
	private bool _AZSUPORD;
	public bool AZSUPORD { get => _AZSUPORD; set { if (_AZSUPORD != value) { _AZSUPORD = value; NotifyPropertyChanged();} } }
	private string? _lot_template;
	public string? lot_template { get => _lot_template; set { if (_lot_template != value) { _lot_template = value; NotifyPropertyChanged();} } }
	private bool _azpnotafa;
	public bool azpnotafa { get => _azpnotafa; set { if (_azpnotafa != value) { _azpnotafa = value; NotifyPropertyChanged();} } }
	private bool _azpnotoff;
	public bool azpnotoff { get => _azpnotoff; set { if (_azpnotoff != value) { _azpnotoff = value; NotifyPropertyChanged();} } }
	private bool _azpnotord;
	public bool azpnotord { get => _azpnotord; set { if (_azpnotord != value) { _azpnotord = value; NotifyPropertyChanged();} } }
	private bool _azpnotddt;
	public bool azpnotddt { get => _azpnotddt; set { if (_azpnotddt != value) { _azpnotddt = value; NotifyPropertyChanged();} } }
	private bool _azpnotinv;
	public bool azpnotinv { get => _azpnotinv; set { if (_azpnotinv != value) { _azpnotinv = value; NotifyPropertyChanged();} } }
	private bool _azagedoff;
	public bool azagedoff { get => _azagedoff; set { if (_azagedoff != value) { _azagedoff = value; NotifyPropertyChanged();} } }
	private bool _azagedord;
	public bool azagedord { get => _azagedord; set { if (_azagedord != value) { _azagedord = value; NotifyPropertyChanged();} } }
	private bool _azagedddt;
	public bool azagedddt { get => _azagedddt; set { if (_azagedddt != value) { _azagedddt = value; NotifyPropertyChanged();} } }
	private bool _azagedfat;
	public bool azagedfat { get => _azagedfat; set { if (_azagedfat != value) { _azagedfat = value; NotifyPropertyChanged();} } }
	private string? _azddtgtex;
	public string? azddtgtex { get => _azddtgtex; set { if (_azddtgtex != value) { _azddtgtex = value; NotifyPropertyChanged();} } }
	private string? _azordgtex;
	public string? azordgtex { get => _azordgtex; set { if (_azordgtex != value) { _azordgtex = value; NotifyPropertyChanged();} } }
	private string? _azoffgtex;
	public string? azoffgtex { get => _azoffgtex; set { if (_azoffgtex != value) { _azoffgtex = value; NotifyPropertyChanged();} } }
	private string? _azinvgtex;
	public string? azinvgtex { get => _azinvgtex; set { if (_azinvgtex != value) { _azinvgtex = value; NotifyPropertyChanged();} } }
	private bool _AZCUSDDT;
	public bool AZCUSDDT { get => _AZCUSDDT; set { if (_AZCUSDDT != value) { _AZCUSDDT = value; NotifyPropertyChanged();} } }
	private bool _AZCUSINV;
	public bool AZCUSINV { get => _AZCUSINV; set { if (_AZCUSINV != value) { _AZCUSINV = value; NotifyPropertyChanged();} } }
	private string? _azacqgtex;
	public string? azacqgtex { get => _azacqgtex; set { if (_azacqgtex != value) { _azacqgtex = value; NotifyPropertyChanged();} } }
	private bool _lot_locked;
	public bool lot_locked { get => _lot_locked; set { if (_lot_locked != value) { _lot_locked = value; NotifyPropertyChanged();} } }
	private bool _prices_onmoves;
	public bool prices_onmoves { get => _prices_onmoves; set { if (_prices_onmoves != value) { _prices_onmoves = value; NotifyPropertyChanged();} } }
	private bool _ordered_as_available;
	public bool ordered_as_available { get => _ordered_as_available; set { if (_ordered_as_available != value) { _ordered_as_available = value; NotifyPropertyChanged();} } }
	private decimal? _azcapsoc;
	public decimal? azcapsoc { get => _azcapsoc; set { if (_azcapsoc != value) { _azcapsoc = value; NotifyPropertyChanged();} } }
	private bool _azuseei;
	public bool azuseei { get => _azuseei; set { if (_azuseei != value) { _azuseei = value; NotifyPropertyChanged();} } }
	private string? _azapikey;
	public string? azapikey { get => _azapikey; set { if (_azapikey != value) { _azapikey = value; NotifyPropertyChanged();} } }
	private string? _azregifatt;
	public string? azregifatt { get => _azregifatt; set { if (_azregifatt != value) { _azregifatt = value; NotifyPropertyChanged();} } }
	private string? _azcodcee;
	public string? azcodcee { get => _azcodcee; set { if (_azcodcee != value) { _azcodcee = value; NotifyPropertyChanged();} } }
	private string? _azcodextracee;
	public string? azcodextracee { get => _azcodextracee; set { if (_azcodextracee != value) { _azcodextracee = value; NotifyPropertyChanged();} } }
	private string? _azisoextracee;
	public string? azisoextracee { get => _azisoextracee; set { if (_azisoextracee != value) { _azisoextracee = value; NotifyPropertyChanged();} } }
	private string? _azeipath;
	public string? azeipath { get => _azeipath; set { if (_azeipath != value) { _azeipath = value; NotifyPropertyChanged();} } }
	private string? _azsocuni;
	public string? azsocuni { get => _azsocuni; set { if (_azsocuni != value) { _azsocuni = value; NotifyPropertyChanged();} } }
	private string? _azstaliq;
	public string? azstaliq { get => _azstaliq; set { if (_azstaliq != value) { _azstaliq = value; NotifyPropertyChanged();} } }
	private string? _azdisgrp;
	public string? azdisgrp { get => _azdisgrp; set { if (_azdisgrp != value) { _azdisgrp = value; NotifyPropertyChanged();} } }
	private string? _azdiscnt;
	public string? azdiscnt { get => _azdiscnt; set { if (_azdiscnt != value) { _azdiscnt = value; NotifyPropertyChanged();} } }
	private string? _azdissot;
	public string? azdissot { get => _azdissot; set { if (_azdissot != value) { _azdissot = value; NotifyPropertyChanged();} } }
	private bool _azimpgilat;
	public bool azimpgilat { get => _azimpgilat; set { if (_azimpgilat != value) { _azimpgilat = value; NotifyPropertyChanged();} } }
	private string? _azimpgilatid;
	public string? azimpgilatid { get => _azimpgilatid; set { if (_azimpgilatid != value) { _azimpgilatid = value; NotifyPropertyChanged();} } }
	private string? _azimpgilatcau;
	public string? azimpgilatcau { get => _azimpgilatcau; set { if (_azimpgilatcau != value) { _azimpgilatcau = value; NotifyPropertyChanged();} } }
	private string? _azimpgilatalc;
	public string? azimpgilatalc { get => _azimpgilatalc; set { if (_azimpgilatalc != value) { _azimpgilatalc = value; NotifyPropertyChanged();} } }
	private string? _azimpgilatala;
	public string? azimpgilatala { get => _azimpgilatala; set { if (_azimpgilatala != value) { _azimpgilatala = value; NotifyPropertyChanged();} } }
	private bool _azddtricl;
	public bool azddtricl { get => _azddtricl; set { if (_azddtricl != value) { _azddtricl = value; NotifyPropertyChanged();} } }
	private bool _azddtdefic;
	public bool azddtdefic { get => _azddtdefic; set { if (_azddtdefic != value) { _azddtdefic = value; NotifyPropertyChanged();} } }
	private bool _azinvricl;
	public bool azinvricl { get => _azinvricl; set { if (_azinvricl != value) { _azinvricl = value; NotifyPropertyChanged();} } }
	private bool _azinvshde;
	public bool azinvshde { get => _azinvshde; set { if (_azinvshde != value) { _azinvshde = value; NotifyPropertyChanged();} } }
	private bool _azfatfile;
	public bool azfatfile { get => _azfatfile; set { if (_azfatfile != value) { _azfatfile = value; NotifyPropertyChanged();} } }
	private bool _azimpbancolat;
	public bool azimpbancolat { get => _azimpbancolat; set { if (_azimpbancolat != value) { _azimpbancolat = value; NotifyPropertyChanged();} } }
	private string? _azimpbancolatid;
	public string? azimpbancolatid { get => _azimpbancolatid; set { if (_azimpbancolatid != value) { _azimpbancolatid = value; NotifyPropertyChanged();} } }
	private string? _azimpbancolatcau;
	public string? azimpbancolatcau { get => _azimpbancolatcau; set { if (_azimpbancolatcau != value) { _azimpbancolatcau = value; NotifyPropertyChanged();} } }
	private bool? _aziconchiuclifor;
	public bool? aziconchiuclifor { get => _aziconchiuclifor; set { if (_aziconchiuclifor != value) { _aziconchiuclifor = value; NotifyPropertyChanged();} } }
	private string? _azlinguadefault;
	public string? azlinguadefault { get => _azlinguadefault; set { if (_azlinguadefault != value) { _azlinguadefault = value; NotifyPropertyChanged();} } }
    private string? _azartautfat;
    public string? azartautfat { get => _azartautfat; set { if (_azartautfat != value) { _azartautfat = value; NotifyPropertyChanged(); } } }
}