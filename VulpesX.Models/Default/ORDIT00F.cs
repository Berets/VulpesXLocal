namespace VulpesX.Models.Default;
 
public partial class ORDIT00F : Base 
{
	private string _otsoci = null!;
	public required string otsoci { get => _otsoci; set { if (_otsoci != value) { _otsoci = value; NotifyPropertyChanged();} } }
	private int _OTANNO;
	public int OTANNO { get => _OTANNO; set { if (_OTANNO != value) { _OTANNO = value; NotifyPropertyChanged();} } }
	private int _OTNUOR;
	public int OTNUOR { get => _OTNUOR; set { if (_OTNUOR != value) { _OTNUOR = value; NotifyPropertyChanged();} } }
	private DateTime? _OTDAOR;
	public DateTime? OTDAOR { get => _OTDAOR; set { if (_OTDAOR != value) { _OTDAOR = value; NotifyPropertyChanged();} } }
	private string? _OTCAUS;
	public string? OTCAUS { get => _OTCAUS; set { if (_OTCAUS != value) { _OTCAUS = value; NotifyPropertyChanged();} } }
	private int? _OTCLIE;
	public int? OTCLIE { get => _OTCLIE; set { if (_OTCLIE != value) { _OTCLIE = value; NotifyPropertyChanged();} } }
	private int? _DESTIN;
	public int? DESTIN { get => _DESTIN; set { if (_DESTIN != value) { _DESTIN = value; NotifyPropertyChanged();} } }
	private int? _OTCONZ;
	public int? OTCONZ { get => _OTCONZ; set { if (_OTCONZ != value) { _OTCONZ = value; NotifyPropertyChanged();} } }
	private string? _OTPAGA;
	public string? OTPAGA { get => _OTPAGA; set { if (_OTPAGA != value) { _OTPAGA = value; NotifyPropertyChanged();} } }
	private int? _abicab;
	public int? abicab { get => _abicab; set { if (_abicab != value) { _abicab = value; NotifyPropertyChanged();} } }
	private string? _OTCONS;
	public string? OTCONS { get => _OTCONS; set { if (_OTCONS != value) { _OTCONS = value; NotifyPropertyChanged();} } }
	private string? _OTSPED;
	public string? OTSPED { get => _OTSPED; set { if (_OTSPED != value) { _OTSPED = value; NotifyPropertyChanged();} } }
	private int? _OTCORR;
	public int? OTCORR { get => _OTCORR; set { if (_OTCORR != value) { _OTCORR = value; NotifyPropertyChanged();} } }
	private DateTime? _OTDATC;
	public DateTime? OTDATC { get => _OTDATC; set { if (_OTDATC != value) { _OTDATC = value; NotifyPropertyChanged();} } }
	private string? _OTAREA;
	public string? OTAREA { get => _OTAREA; set { if (_OTAREA != value) { _OTAREA = value; NotifyPropertyChanged();} } }
	private int? _OTFILI;
	public int? OTFILI { get => _OTFILI; set { if (_OTFILI != value) { _OTFILI = value; NotifyPropertyChanged();} } }
	private string? _OTZONA;
	public string? OTZONA { get => _OTZONA; set { if (_OTZONA != value) { _OTZONA = value; NotifyPropertyChanged();} } }
	private string? _OTSETM;
	public string? OTSETM { get => _OTSETM; set { if (_OTSETM != value) { _OTSETM = value; NotifyPropertyChanged();} } }
	private string? _OTCIDI;
	public string? OTCIDI { get => _OTCIDI; set { if (_OTCIDI != value) { _OTCIDI = value; NotifyPropertyChanged();} } }
	private string? _OTDE25;
	public string? OTDE25 { get => _OTDE25; set { if (_OTDE25 != value) { _OTDE25 = value; NotifyPropertyChanged();} } }
	private string? _OTIMBA;
	public string? OTIMBA { get => _OTIMBA; set { if (_OTIMBA != value) { _OTIMBA = value; NotifyPropertyChanged();} } }
	private int? _abiabi;
	public int? abiabi { get => _abiabi; set { if (_abiabi != value) { _abiabi = value; NotifyPropertyChanged();} } }
	private string? _otdivi;
	public string? otdivi { get => _otdivi; set { if (_otdivi != value) { _otdivi = value; NotifyPropertyChanged();} } }
	private string? _flgchi;
	public string? flgchi { get => _flgchi; set { if (_flgchi != value) { _flgchi = value; NotifyPropertyChanged();} } }
	private string? _otling;
	public string? otling { get => _otling; set { if (_otling != value) { _otling = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private string? _OTNOTET;
	public string? OTNOTET { get => _OTNOTET; set { if (_OTNOTET != value) { _OTNOTET = value; NotifyPropertyChanged();} } }
	private string? _OTNOTEP;
	public string? OTNOTEP { get => _OTNOTEP; set { if (_OTNOTEP != value) { _OTNOTEP = value; NotifyPropertyChanged();} } }
	private bool _OTSHOWT;
	public bool OTSHOWT { get => _OTSHOWT; set { if (_OTSHOWT != value) { _OTSHOWT = value; NotifyPropertyChanged();} } }
	private bool _OTSHOWP;
	public bool OTSHOWP { get => _OTSHOWP; set { if (_OTSHOWP != value) { _OTSHOWP = value; NotifyPropertyChanged();} } }
	private string? _OTBCON;
	public string? OTBCON { get => _OTBCON; set { if (_OTBCON != value) { _OTBCON = value; NotifyPropertyChanged();} } }
	private DateTime? _OTINFI;
	public DateTime? OTINFI { get => _OTINFI; set { if (_OTINFI != value) { _OTINFI = value; NotifyPropertyChanged();} } }
	private string? _OTINFIUSR;
	public string? OTINFIUSR { get => _OTINFIUSR; set { if (_OTINFIUSR != value) { _OTINFIUSR = value; NotifyPropertyChanged();} } }
	private DateTime? _OTFICO;
	public DateTime? OTFICO { get => _OTFICO; set { if (_OTFICO != value) { _OTFICO = value; NotifyPropertyChanged();} } }
	private string? _OTFICOUSR;
	public string? OTFICOUSR { get => _OTFICOUSR; set { if (_OTFICOUSR != value) { _OTFICOUSR = value; NotifyPropertyChanged();} } }
	private DateTime? _OTFITE;
	public DateTime? OTFITE { get => _OTFITE; set { if (_OTFITE != value) { _OTFITE = value; NotifyPropertyChanged();} } }
	private string? _OTFITEUSR;
	public string? OTFITEUSR { get => _OTFITEUSR; set { if (_OTFITEUSR != value) { _OTFITEUSR = value; NotifyPropertyChanged();} } }
	private DateTime? _OTDARI;
	public DateTime? OTDARI { get => _OTDARI; set { if (_OTDARI != value) { _OTDARI = value; NotifyPropertyChanged();} } }
	private int? _OTCORR2;
	public int? OTCORR2 { get => _OTCORR2; set { if (_OTCORR2 != value) { _OTCORR2 = value; NotifyPropertyChanged();} } }
	private string? _OTREGI;
	public string? OTREGI { get => _OTREGI; set { if (_OTREGI != value) { _OTREGI = value; NotifyPropertyChanged();} } }
	private string? _OTRIVE;
	public string? OTRIVE { get => _OTRIVE; set { if (_OTRIVE != value) { _OTRIVE = value; NotifyPropertyChanged();} } }
	private decimal? _OTSCCL;
	public decimal? OTSCCL { get => _OTSCCL; set { if (_OTSCCL != value) { _OTSCCL = value; NotifyPropertyChanged();} } }
	private DateTime? _OTDAPA;
	public DateTime? OTDAPA { get => _OTDAPA; set { if (_OTDAPA != value) { _OTDAPA = value; NotifyPropertyChanged();} } }
	private string? _OTCUNO;
	public string? OTCUNO { get => _OTCUNO; set { if (_OTCUNO != value) { _OTCUNO = value; NotifyPropertyChanged();} } }
	private DateTime? _OTCUDO;
	public DateTime? OTCUDO { get => _OTCUDO; set { if (_OTCUDO != value) { _OTCUDO = value; NotifyPropertyChanged();} } }
}