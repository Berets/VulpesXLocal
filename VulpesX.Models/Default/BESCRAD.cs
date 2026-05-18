namespace VulpesX.Models.Default;
 
public partial class BESCRAD : Base 
{
	private string _crsoci = null!;
	public required string crsoci { get => _crsoci; set { if (_crsoci != value) { _crsoci = value; NotifyPropertyChanged();} } }
	private int _cranco;
	public int cranco { get => _cranco; set { if (_cranco != value) { _cranco = value; NotifyPropertyChanged();} } }
	private int _crann4;
	public int crann4 { get => _crann4; set { if (_crann4 != value) { _crann4 = value; NotifyPropertyChanged();} } }
	private string _crgrup = null!;
	public required string crgrup { get => _crgrup; set { if (_crgrup != value) { _crgrup = value; NotifyPropertyChanged();} } }
	private string _crcont = null!;
	public required string crcont { get => _crcont; set { if (_crcont != value) { _crcont = value; NotifyPropertyChanged();} } }
	private string _crsotc = null!;
	public required string crsotc { get => _crsotc; set { if (_crsotc != value) { _crsotc = value; NotifyPropertyChanged();} } }
	private int _crann2;
	public int crann2 { get => _crann2; set { if (_crann2 != value) { _crann2 = value; NotifyPropertyChanged();} } }
	private int _crinv2;
	public int crinv2 { get => _crinv2; set { if (_crinv2 != value) { _crinv2 = value; NotifyPropertyChanged();} } }
	private int _crinv;
	public int crinv { get => _crinv; set { if (_crinv != value) { _crinv = value; NotifyPropertyChanged();} } }
	private string? _crcat;
	public string? crcat { get => _crcat; set { if (_crcat != value) { _crcat = value; NotifyPropertyChanged();} } }
	private decimal? _crval;
	public decimal? crval { get => _crval; set { if (_crval != value) { _crval = value; NotifyPropertyChanged();} } }
	private decimal? _crpea;
	public decimal? crpea { get => _crpea; set { if (_crpea != value) { _crpea = value; NotifyPropertyChanged();} } }
	private decimal? _cranne;
	public decimal? cranne { get => _cranne; set { if (_cranne != value) { _cranne = value; NotifyPropertyChanged();} } }
	private DateTime? _crdaip;
	public DateTime? crdaip { get => _crdaip; set { if (_crdaip != value) { _crdaip = value; NotifyPropertyChanged();} } }
	private DateTime? _crdafp;
	public DateTime? crdafp { get => _crdafp; set { if (_crdafp != value) { _crdafp = value; NotifyPropertyChanged();} } }
}