namespace VulpesX.Models.Default;
 
public partial class FATTAUT : Base 
{
	private string _FTAUSC = null!;
	public required string FTAUSC { get => _FTAUSC; set { if (_FTAUSC != value) { _FTAUSC = value; NotifyPropertyChanged();} } }
	private int _FTAUAN;
	public int FTAUAN { get => _FTAUAN; set { if (_FTAUAN != value) { _FTAUAN = value; NotifyPropertyChanged();} } }
	private int _FTAUNUM;
	public int FTAUNUM { get => _FTAUNUM; set { if (_FTAUNUM != value) { _FTAUNUM = value; NotifyPropertyChanged();} } }
	private int? _FTAUCOF;
	public int? FTAUCOF { get => _FTAUCOF; set { if (_FTAUCOF != value) { _FTAUCOF = value; NotifyPropertyChanged();} } }
	private DateTime? _FTAUDATRIC;
	public DateTime? FTAUDATRIC { get => _FTAUDATRIC; set { if (_FTAUDATRIC != value) { _FTAUDATRIC = value; NotifyPropertyChanged();} } }
	private string? _FTAUINDSDI;
	public string? FTAUINDSDI { get => _FTAUINDSDI; set { if (_FTAUINDSDI != value) { _FTAUINDSDI = value; NotifyPropertyChanged();} } }
	private DateTime? _FTAUDATFAT;
	public DateTime? FTAUDATFAT { get => _FTAUDATFAT; set { if (_FTAUDATFAT != value) { _FTAUDATFAT = value; NotifyPropertyChanged();} } }
	private string? _FTAUNUMFAT;
	public string? FTAUNUMFAT { get => _FTAUNUMFAT; set { if (_FTAUNUMFAT != value) { _FTAUNUMFAT = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private int? _FTAPNAN;
	public int? FTAPNAN { get => _FTAPNAN; set { if (_FTAPNAN != value) { _FTAPNAN = value; NotifyPropertyChanged();} } }
	private int? _FTAPNRE;
	public int? FTAPNRE { get => _FTAPNRE; set { if (_FTAPNRE != value) { _FTAPNRE = value; NotifyPropertyChanged();} } }
}