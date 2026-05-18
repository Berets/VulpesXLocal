namespace VulpesX.Models.Default;
 
public partial class MGMOT00F : Base 
{
	private string _mmsoci = null!;
	public required string mmsoci { get => _mmsoci; set { if (_mmsoci != value) { _mmsoci = value; NotifyPropertyChanged();} } }
	private int _MMESCO;
	public int MMESCO { get => _MMESCO; set { if (_MMESCO != value) { _MMESCO = value; NotifyPropertyChanged();} } }
	private int _MMNURE;
	public int MMNURE { get => _MMNURE; set { if (_MMNURE != value) { _MMNURE = value; NotifyPropertyChanged();} } }
	private DateTime? _MMDARE;
	public DateTime? MMDARE { get => _MMDARE; set { if (_MMDARE != value) { _MMDARE = value; NotifyPropertyChanged();} } }
	private string? _MMNUDO;
	public string? MMNUDO { get => _MMNUDO; set { if (_MMNUDO != value) { _MMNUDO = value; NotifyPropertyChanged();} } }
	private DateTime? _MMDADO;
	public DateTime? MMDADO { get => _MMDADO; set { if (_MMDADO != value) { _MMDADO = value; NotifyPropertyChanged();} } }
	private string? _MMFLCF;
	public string? MMFLCF { get => _MMFLCF; set { if (_MMFLCF != value) { _MMFLCF = value; NotifyPropertyChanged();} } }
	private int? _MMCOCF;
	public int? MMCOCF { get => _MMCOCF; set { if (_MMCOCF != value) { _MMCOCF = value; NotifyPropertyChanged();} } }
	private string? _MMCAUS;
	public string? MMCAUS { get => _MMCAUS; set { if (_MMCAUS != value) { _MMCAUS = value; NotifyPropertyChanged();} } }
	private string? _MMMAGA;
	public string? MMMAGA { get => _MMMAGA; set { if (_MMMAGA != value) { _MMMAGA = value; NotifyPropertyChanged();} } }
	private string? _MMSEGN;
	public string? MMSEGN { get => _MMSEGN; set { if (_MMSEGN != value) { _MMSEGN = value; NotifyPropertyChanged();} } }
	private string? _MMCOPO;
	public string? MMCOPO { get => _MMCOPO; set { if (_MMCOPO != value) { _MMCOPO = value; NotifyPropertyChanged();} } }
	private string? _MMCIMB;
	public string? MMCIMB { get => _MMCIMB; set { if (_MMCIMB != value) { _MMCIMB = value; NotifyPropertyChanged();} } }
	private int? _MMNUCO;
	public int? MMNUCO { get => _MMNUCO; set { if (_MMNUCO != value) { _MMNUCO = value; NotifyPropertyChanged();} } }
	private decimal? _MMPESO;
	public decimal? MMPESO { get => _MMPESO; set { if (_MMPESO != value) { _MMPESO = value; NotifyPropertyChanged();} } }
	private decimal? _MMVOLU;
	public decimal? MMVOLU { get => _MMVOLU; set { if (_MMVOLU != value) { _MMVOLU = value; NotifyPropertyChanged();} } }
	private int? _MMCOCO;
	public int? MMCOCO { get => _MMCOCO; set { if (_MMCOCO != value) { _MMCOCO = value; NotifyPropertyChanged();} } }
	private string? _mmseca;
	public string? mmseca { get => _mmseca; set { if (_mmseca != value) { _mmseca = value; NotifyPropertyChanged();} } }
	private string? _MMFLT1;
	public string? MMFLT1 { get => _MMFLT1; set { if (_MMFLT1 != value) { _MMFLT1 = value; NotifyPropertyChanged();} } }
	private int? _MMDEST;
	public int? MMDEST { get => _MMDEST; set { if (_MMDEST != value) { _MMDEST = value; NotifyPropertyChanged();} } }
	private int? _MMTPRO;
	public int? MMTPRO { get => _MMTPRO; set { if (_MMTPRO != value) { _MMTPRO = value; NotifyPropertyChanged();} } }
	private int? _MMTDAS;
	public int? MMTDAS { get => _MMTDAS; set { if (_MMTDAS != value) { _MMTDAS = value; NotifyPropertyChanged();} } }
}