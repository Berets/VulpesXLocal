namespace VulpesX.Models.Default;
 
public partial class AQFAP00F : Base 
{
	private string _fapsoc = null!;
	public required string fapsoc { get => _fapsoc; set { if (_fapsoc != value) { _fapsoc = value; NotifyPropertyChanged();} } }
	private int _fapforn;
	public int fapforn { get => _fapforn; set { if (_fapforn != value) { _fapforn = value; NotifyPropertyChanged();} } }
	private int _fapann;
	public int fapann { get => _fapann; set { if (_fapann != value) { _fapann = value; NotifyPropertyChanged();} } }
	private int _fapnur;
	public int fapnur { get => _fapnur; set { if (_fapnur != value) { _fapnur = value; NotifyPropertyChanged();} } }
	private int _fatpos;
	public int fatpos { get => _fatpos; set { if (_fatpos != value) { _fatpos = value; NotifyPropertyChanged();} } }
	private int? _fatano;
	public int? fatano { get => _fatano; set { if (_fatano != value) { _fatano = value; NotifyPropertyChanged();} } }
	private int? _fatuor;
	public int? fatuor { get => _fatuor; set { if (_fatuor != value) { _fatuor = value; NotifyPropertyChanged();} } }
	private int? _fatpoo;
	public int? fatpoo { get => _fatpoo; set { if (_fatpoo != value) { _fatpoo = value; NotifyPropertyChanged();} } }
	private int? _fapane;
	public int? fapane { get => _fapane; set { if (_fapane != value) { _fapane = value; NotifyPropertyChanged();} } }
	private string? _fapboe;
	public string? fapboe { get => _fapboe; set { if (_fapboe != value) { _fapboe = value; NotifyPropertyChanged();} } }
	private DateTime? _fapdae;
	public DateTime? fapdae { get => _fapdae; set { if (_fapdae != value) { _fapdae = value; NotifyPropertyChanged();} } }
	private DateTime? _fapdab;
	public DateTime? fapdab { get => _fapdab; set { if (_fapdab != value) { _fapdab = value; NotifyPropertyChanged();} } }
	private decimal? _fapqta;
	public decimal? fapqta { get => _fapqta; set { if (_fapqta != value) { _fapqta = value; NotifyPropertyChanged();} } }
	private decimal? _fapqtf;
	public decimal? fapqtf { get => _fapqtf; set { if (_fapqtf != value) { _fapqtf = value; NotifyPropertyChanged();} } }
	private decimal? _fappre;
	public decimal? fappre { get => _fappre; set { if (_fappre != value) { _fappre = value; NotifyPropertyChanged();} } }
	private decimal? _fapsc1;
	public decimal? fapsc1 { get => _fapsc1; set { if (_fapsc1 != value) { _fapsc1 = value; NotifyPropertyChanged();} } }
	private string? _faptc1;
	public string? faptc1 { get => _faptc1; set { if (_faptc1 != value) { _faptc1 = value; NotifyPropertyChanged();} } }
	private decimal? _fapsc2;
	public decimal? fapsc2 { get => _fapsc2; set { if (_fapsc2 != value) { _fapsc2 = value; NotifyPropertyChanged();} } }
	private string? _fatpc2;
	public string? fatpc2 { get => _fatpc2; set { if (_fatpc2 != value) { _fatpc2 = value; NotifyPropertyChanged();} } }
	private decimal? _fapsc3;
	public decimal? fapsc3 { get => _fapsc3; set { if (_fapsc3 != value) { _fapsc3 = value; NotifyPropertyChanged();} } }
	private string? _fatpc3;
	public string? fatpc3 { get => _fatpc3; set { if (_fatpc3 != value) { _fatpc3 = value; NotifyPropertyChanged();} } }
	private decimal? _fatmag;
	public decimal? fatmag { get => _fatmag; set { if (_fatmag != value) { _fatmag = value; NotifyPropertyChanged();} } }
	private string? _fattim;
	public string? fattim { get => _fattim; set { if (_fattim != value) { _fattim = value; NotifyPropertyChanged();} } }
	private string? _fatval;
	public string? fatval { get => _fatval; set { if (_fatval != value) { _fatval = value; NotifyPropertyChanged();} } }
	private string? _fatdiv;
	public string? fatdiv { get => _fatdiv; set { if (_fatdiv != value) { _fatdiv = value; NotifyPropertyChanged();} } }
	private DateTime? _fatdav;
	public DateTime? fatdav { get => _fatdav; set { if (_fatdav != value) { _fatdav = value; NotifyPropertyChanged();} } }
	private decimal? _fatvap;
	public decimal? fatvap { get => _fatvap; set { if (_fatvap != value) { _fatvap = value; NotifyPropertyChanged();} } }
	private decimal? _fatvaf;
	public decimal? fatvaf { get => _fatvaf; set { if (_fatvaf != value) { _fatvaf = value; NotifyPropertyChanged();} } }
	private string? _fattier;
	public string? fattier { get => _fattier; set { if (_fattier != value) { _fattier = value; NotifyPropertyChanged();} } }
	private string? _FATALI;
	public string? FATALI { get => _FATALI; set { if (_FATALI != value) { _FATALI = value; NotifyPropertyChanged();} } }
	private string? _FATASS;
	public string? FATASS { get => _FATASS; set { if (_FATASS != value) { _FATASS = value; NotifyPropertyChanged();} } }
	private string? _FATGRU;
	public string? FATGRU { get => _FATGRU; set { if (_FATGRU != value) { _FATGRU = value; NotifyPropertyChanged();} } }
	private string? _fatcon3;
	public string? fatcon3 { get => _fatcon3; set { if (_fatcon3 != value) { _fatcon3 = value; NotifyPropertyChanged();} } }
	private string? _FATSOT;
	public string? FATSOT { get => _FATSOT; set { if (_FATSOT != value) { _FATSOT = value; NotifyPropertyChanged();} } }
	private string? _fatccc;
	public string? fatccc { get => _fatccc; set { if (_fatccc != value) { _fatccc = value; NotifyPropertyChanged();} } }
	private string? _fapart;
	public string? fapart { get => _fapart; set { if (_fapart != value) { _fapart = value; NotifyPropertyChanged();} } }
	private string? _fattpp;
	public string? fattpp { get => _fattpp; set { if (_fattpp != value) { _fattpp = value; NotifyPropertyChanged();} } }
	private string? _fatcome;
	public string? fatcome { get => _fatcome; set { if (_fatcome != value) { _fatcome = value; NotifyPropertyChanged();} } }
	private string? _fatcomsoc;
	public string? fatcomsoc { get => _fatcomsoc; set { if (_fatcomsoc != value) { _fatcomsoc = value; NotifyPropertyChanged();} } }
}