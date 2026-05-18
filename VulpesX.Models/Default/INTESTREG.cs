namespace VulpesX.Models.Default;
 
public partial class INTESTREG : Base 
{
	private string _societ = null!;
	public required string societ { get => _societ; set { if (_societ != value) { _societ = value; NotifyPropertyChanged();} } }
	private int _annoin;
	public int annoin { get => _annoin; set { if (_annoin != value) { _annoin = value; NotifyPropertyChanged();} } }
	private string? _ragsoc;
	public string? ragsoc { get => _ragsoc; set { if (_ragsoc != value) { _ragsoc = value; NotifyPropertyChanged();} } }
	private string? _indiri;
	public string? indiri { get => _indiri; set { if (_indiri != value) { _indiri = value; NotifyPropertyChanged();} } }
	private string? _locali;
	public string? locali { get => _locali; set { if (_locali != value) { _locali = value; NotifyPropertyChanged();} } }
	private string? _autori;
	public string? autori { get => _autori; set { if (_autori != value) { _autori = value; NotifyPropertyChanged();} } }
	private string? _licenz;
	public string? licenz { get => _licenz; set { if (_licenz != value) { _licenz = value; NotifyPropertyChanged();} } }
	private decimal? _numpro;
	public decimal? numpro { get => _numpro; set { if (_numpro != value) { _numpro = value; NotifyPropertyChanged();} } }
	private decimal? _numreg;
	public decimal? numreg { get => _numreg; set { if (_numreg != value) { _numreg = value; NotifyPropertyChanged();} } }
	private decimal? _numpr2;
	public decimal? numpr2 { get => _numpr2; set { if (_numpr2 != value) { _numpr2 = value; NotifyPropertyChanged();} } }
}