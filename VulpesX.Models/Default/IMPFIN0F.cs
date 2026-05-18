namespace VulpesX.Models.Default;
 
public partial class IMPFIN0F : Base 
{
	private string _IMSOC = null!;
	public required string IMSOC { get => _IMSOC; set { if (_IMSOC != value) { _IMSOC = value; NotifyPropertyChanged();} } }
	private int _IMANNO;
	public int IMANNO { get => _IMANNO; set { if (_IMANNO != value) { _IMANNO = value; NotifyPropertyChanged();} } }
	private int _IMMESE;
	public int IMMESE { get => _IMMESE; set { if (_IMMESE != value) { _IMMESE = value; NotifyPropertyChanged();} } }
	private int _IMPROG;
	public int IMPROG { get => _IMPROG; set { if (_IMPROG != value) { _IMPROG = value; NotifyPropertyChanged();} } }
	private DateTime _IMDADO;
	public DateTime IMDADO { get => _IMDADO; set { if (_IMDADO != value) { _IMDADO = value; NotifyPropertyChanged();} } }
	private string _IMNUDO = null!;
	public required string IMNUDO { get => _IMNUDO; set { if (_IMNUDO != value) { _IMNUDO = value; NotifyPropertyChanged();} } }
	private int _IMNURE;
	public int IMNURE { get => _IMNURE; set { if (_IMNURE != value) { _IMNURE = value; NotifyPropertyChanged();} } }
	private DateTime _IMDARE;
	public DateTime IMDARE { get => _IMDARE; set { if (_IMDARE != value) { _IMDARE = value; NotifyPropertyChanged();} } }
	private decimal _IMIMPO;
	public decimal IMIMPO { get => _IMIMPO; set { if (_IMIMPO != value) { _IMIMPO = value; NotifyPropertyChanged();} } }
	private string _IMSALDO = null!;
	public required string IMSALDO { get => _IMSALDO; set { if (_IMSALDO != value) { _IMSALDO = value; NotifyPropertyChanged();} } }
	private string _IMTIPR = null!;
	public required string IMTIPR { get => _IMTIPR; set { if (_IMTIPR != value) { _IMTIPR = value; NotifyPropertyChanged();} } }
	private int _IMCOCF;
	public int IMCOCF { get => _IMCOCF; set { if (_IMCOCF != value) { _IMCOCF = value; NotifyPropertyChanged();} } }
	private DateTime _IMSCAD;
	public DateTime IMSCAD { get => _IMSCAD; set { if (_IMSCAD != value) { _IMSCAD = value; NotifyPropertyChanged();} } }
	private int _IMABI;
	public int IMABI { get => _IMABI; set { if (_IMABI != value) { _IMABI = value; NotifyPropertyChanged();} } }
	private int _IMCAB;
	public int IMCAB { get => _IMCAB; set { if (_IMCAB != value) { _IMCAB = value; NotifyPropertyChanged();} } }
	private string _IMCC = null!;
	public required string IMCC { get => _IMCC; set { if (_IMCC != value) { _IMCC = value; NotifyPropertyChanged();} } }
	private string _IMGRB = null!;
	public required string IMGRB { get => _IMGRB; set { if (_IMGRB != value) { _IMGRB = value; NotifyPropertyChanged();} } }
	private string _IMCOB = null!;
	public required string IMCOB { get => _IMCOB; set { if (_IMCOB != value) { _IMCOB = value; NotifyPropertyChanged();} } }
	private string _IMSOB = null!;
	public required string IMSOB { get => _IMSOB; set { if (_IMSOB != value) { _IMSOB = value; NotifyPropertyChanged();} } }
	private string _IMFILE = null!;
	public required string IMFILE { get => _IMFILE; set { if (_IMFILE != value) { _IMFILE = value; NotifyPropertyChanged();} } }
	private string? _IMRIF;
	public string? IMRIF { get => _IMRIF; set { if (_IMRIF != value) { _IMRIF = value; NotifyPropertyChanged();} } }
	private DateTime? _IMDARI;
	public DateTime? IMDARI { get => _IMDARI; set { if (_IMDARI != value) { _IMDARI = value; NotifyPropertyChanged();} } }
	private string? _IMNOTE;
	public string? IMNOTE { get => _IMNOTE; set { if (_IMNOTE != value) { _IMNOTE = value; NotifyPropertyChanged();} } }
	private string? _IMTIIM;
	public string? IMTIIM { get => _IMTIIM; set { if (_IMTIIM != value) { _IMTIIM = value; NotifyPropertyChanged();} } }
}