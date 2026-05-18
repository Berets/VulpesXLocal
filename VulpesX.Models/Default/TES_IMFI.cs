namespace VulpesX.Models.Default;
 
public partial class TES_IMFI : Base 
{
	private string _ifsoci = null!;
	public required string ifsoci { get => _ifsoci; set { if (_ifsoci != value) { _ifsoci = value; NotifyPropertyChanged();} } }
	private string _ifgrup = null!;
	public required string ifgrup { get => _ifgrup; set { if (_ifgrup != value) { _ifgrup = value; NotifyPropertyChanged();} } }
	private string _ifcont = null!;
	public required string ifcont { get => _ifcont; set { if (_ifcont != value) { _ifcont = value; NotifyPropertyChanged();} } }
	private string _ifsott = null!;
	public required string ifsott { get => _ifsott; set { if (_ifsott != value) { _ifsott = value; NotifyPropertyChanged();} } }
	private DateTime _ifdata;
	public DateTime ifdata { get => _ifdata; set { if (_ifdata != value) { _ifdata = value; NotifyPropertyChanged();} } }
	private decimal _ifimpo;
	public decimal ifimpo { get => _ifimpo; set { if (_ifimpo != value) { _ifimpo = value; NotifyPropertyChanged();} } }
	private string? _ifrife;
	public string? ifrife { get => _ifrife; set { if (_ifrife != value) { _ifrife = value; NotifyPropertyChanged();} } }
	private DateTime? _ifdaca;
	public DateTime? ifdaca { get => _ifdaca; set { if (_ifdaca != value) { _ifdaca = value; NotifyPropertyChanged();} } }
	private string? _ifnote;
	public string? ifnote { get => _ifnote; set { if (_ifnote != value) { _ifnote = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private int? _ifregann;
	public int? ifregann { get => _ifregann; set { if (_ifregann != value) { _ifregann = value; NotifyPropertyChanged();} } }
	private int? _ifregnum;
	public int? ifregnum { get => _ifregnum; set { if (_ifregnum != value) { _ifregnum = value; NotifyPropertyChanged();} } }
}