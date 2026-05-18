namespace VulpesX.Models.Default;
 
public partial class FE_PAGDOC : Base 
{
	private string _FEPACOD = null!;
	public required string FEPACOD { get => _FEPACOD; set { if (_FEPACOD != value) { _FEPACOD = value; NotifyPropertyChanged();} } }
	private string _FEPADES = null!;
	public required string FEPADES { get => _FEPADES; set { if (_FEPADES != value) { _FEPADES = value; NotifyPropertyChanged();} } }
	private string? _FEPATVAL;
	public string? FEPATVAL { get => _FEPATVAL; set { if (_FEPATVAL != value) { _FEPATVAL = value; NotifyPropertyChanged();} } }
	private DateTime? _FEPADAT;
	public DateTime? FEPADAT { get => _FEPADAT; set { if (_FEPADAT != value) { _FEPADAT = value; NotifyPropertyChanged();} } }
	private string? _FEPATIPP;
	public string? FEPATIPP { get => _FEPATIPP; set { if (_FEPATIPP != value) { _FEPATIPP = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}