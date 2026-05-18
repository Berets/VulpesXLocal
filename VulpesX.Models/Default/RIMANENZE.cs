namespace VulpesX.Models.Default;
 
public partial class RIMANENZE : Base 
{
	private string _rimsoc = null!;
	public required string rimsoc { get => _rimsoc; set { if (_rimsoc != value) { _rimsoc = value; NotifyPropertyChanged();} } }
	private int _rimann;
	public int rimann { get => _rimann; set { if (_rimann != value) { _rimann = value; NotifyPropertyChanged();} } }
	private decimal? _rimini;
	public decimal? rimini { get => _rimini; set { if (_rimini != value) { _rimini = value; NotifyPropertyChanged();} } }
	private decimal? _rimfin;
	public decimal? rimfin { get => _rimfin; set { if (_rimfin != value) { _rimfin = value; NotifyPropertyChanged();} } }
	private int? _rimcod;
	public int? rimcod { get => _rimcod; set { if (_rimcod != value) { _rimcod = value; NotifyPropertyChanged();} } }
	private string? _rimtso;
	public string? rimtso { get => _rimtso; set { if (_rimtso != value) { _rimtso = value; NotifyPropertyChanged();} } }
	private string? _Rimtab;
	public string? Rimtab { get => _Rimtab; set { if (_Rimtab != value) { _Rimtab = value; NotifyPropertyChanged();} } }
	private int? _rimfilcod;
	public int? rimfilcod { get => _rimfilcod; set { if (_rimfilcod != value) { _rimfilcod = value; NotifyPropertyChanged();} } }
}