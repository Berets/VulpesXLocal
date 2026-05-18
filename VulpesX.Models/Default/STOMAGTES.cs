namespace VulpesX.Models.Default;
 
public partial class STOMAGTES : Base 
{
	private string _smsoci = null!;
	public required string smsoci { get => _smsoci; set { if (_smsoci != value) { _smsoci = value; NotifyPropertyChanged();} } }
	private int _SMESCO;
	public int SMESCO { get => _SMESCO; set { if (_SMESCO != value) { _SMESCO = value; NotifyPropertyChanged();} } }
	private int _SMNURE;
	public int SMNURE { get => _SMNURE; set { if (_SMNURE != value) { _SMNURE = value; NotifyPropertyChanged();} } }
	private DateTime? _SMDARE;
	public DateTime? SMDARE { get => _SMDARE; set { if (_SMDARE != value) { _SMDARE = value; NotifyPropertyChanged();} } }
	private string? _SMNUDO;
	public string? SMNUDO { get => _SMNUDO; set { if (_SMNUDO != value) { _SMNUDO = value; NotifyPropertyChanged();} } }
	private DateTime? _SMDADO;
	public DateTime? SMDADO { get => _SMDADO; set { if (_SMDADO != value) { _SMDADO = value; NotifyPropertyChanged();} } }
	private string? _SMFLCF;
	public string? SMFLCF { get => _SMFLCF; set { if (_SMFLCF != value) { _SMFLCF = value; NotifyPropertyChanged();} } }
	private int? _SMCOCF;
	public int? SMCOCF { get => _SMCOCF; set { if (_SMCOCF != value) { _SMCOCF = value; NotifyPropertyChanged();} } }
	private string? _SMCAUS;
	public string? SMCAUS { get => _SMCAUS; set { if (_SMCAUS != value) { _SMCAUS = value; NotifyPropertyChanged();} } }
	private string? _SMMAGA;
	public string? SMMAGA { get => _SMMAGA; set { if (_SMMAGA != value) { _SMMAGA = value; NotifyPropertyChanged();} } }
	private string? _SMSEGN;
	public string? SMSEGN { get => _SMSEGN; set { if (_SMSEGN != value) { _SMSEGN = value; NotifyPropertyChanged();} } }
	private string? _SMCOPO;
	public string? SMCOPO { get => _SMCOPO; set { if (_SMCOPO != value) { _SMCOPO = value; NotifyPropertyChanged();} } }
	private string? _SMCIMB;
	public string? SMCIMB { get => _SMCIMB; set { if (_SMCIMB != value) { _SMCIMB = value; NotifyPropertyChanged();} } }
	private int? _SMNUCO;
	public int? SMNUCO { get => _SMNUCO; set { if (_SMNUCO != value) { _SMNUCO = value; NotifyPropertyChanged();} } }
	private decimal? _SMPESO;
	public decimal? SMPESO { get => _SMPESO; set { if (_SMPESO != value) { _SMPESO = value; NotifyPropertyChanged();} } }
	private decimal? _SMVOLU;
	public decimal? SMVOLU { get => _SMVOLU; set { if (_SMVOLU != value) { _SMVOLU = value; NotifyPropertyChanged();} } }
	private int? _SMCOCO;
	public int? SMCOCO { get => _SMCOCO; set { if (_SMCOCO != value) { _SMCOCO = value; NotifyPropertyChanged();} } }
	private string? _smseca;
	public string? smseca { get => _smseca; set { if (_smseca != value) { _smseca = value; NotifyPropertyChanged();} } }
	private string? _SMFLT1;
	public string? SMFLT1 { get => _SMFLT1; set { if (_SMFLT1 != value) { _SMFLT1 = value; NotifyPropertyChanged();} } }
	private int? _SMDEST;
	public int? SMDEST { get => _SMDEST; set { if (_SMDEST != value) { _SMDEST = value; NotifyPropertyChanged();} } }
	private int? _SMTPRO;
	public int? SMTPRO { get => _SMTPRO; set { if (_SMTPRO != value) { _SMTPRO = value; NotifyPropertyChanged();} } }
	private int? _SMTDAS;
	public int? SMTDAS { get => _SMTDAS; set { if (_SMTDAS != value) { _SMTDAS = value; NotifyPropertyChanged();} } }
}