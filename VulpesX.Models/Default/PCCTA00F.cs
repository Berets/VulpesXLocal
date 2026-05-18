namespace VulpesX.Models.Default;
 
public partial class PCCTA00F : Base 
{
	private string _pccsoc = null!;
	public required string pccsoc { get => _pccsoc; set { if (_pccsoc != value) { _pccsoc = value; NotifyPropertyChanged();} } }
	private int _pccann;
	public int pccann { get => _pccann; set { if (_pccann != value) { _pccann = value; NotifyPropertyChanged();} } }
	private string _pcccos = null!;
	public required string pcccos { get => _pcccos; set { if (_pcccos != value) { _pcccos = value; NotifyPropertyChanged();} } }
	private string _pcccoa = null!;
	public required string pcccoa { get => _pcccoa; set { if (_pcccoa != value) { _pcccoa = value; NotifyPropertyChanged();} } }
	private decimal? _pccafp;
	public decimal? pccafp { get => _pccafp; set { if (_pccafp != value) { _pccafp = value; NotifyPropertyChanged();} } }
	private decimal? _pccavp;
	public decimal? pccavp { get => _pccavp; set { if (_pccavp != value) { _pccavp = value; NotifyPropertyChanged();} } }
	private decimal? _pccavl;
	public decimal? pccavl { get => _pccavl; set { if (_pccavl != value) { _pccavl = value; NotifyPropertyChanged();} } }
	private decimal? _pccave;
	public decimal? pccave { get => _pccave; set { if (_pccave != value) { _pccave = value; NotifyPropertyChanged();} } }
	private string? _pccatt;
	public string? pccatt { get => _pccatt; set { if (_pccatt != value) { _pccatt = value; NotifyPropertyChanged();} } }
	private string? _pccori;
	public string? pccori { get => _pccori; set { if (_pccori != value) { _pccori = value; NotifyPropertyChanged();} } }
	private string? _pccorv;
	public string? pccorv { get => _pccorv; set { if (_pccorv != value) { _pccorv = value; NotifyPropertyChanged();} } }
	private string? _pccter;
	public string? pccter { get => _pccter; set { if (_pccter != value) { _pccter = value; NotifyPropertyChanged();} } }
	private string? _pccuti;
	public string? pccuti { get => _pccuti; set { if (_pccuti != value) { _pccuti = value; NotifyPropertyChanged();} } }
	private string? _pccutv;
	public string? pccutv { get => _pccutv; set { if (_pccutv != value) { _pccutv = value; NotifyPropertyChanged();} } }
}