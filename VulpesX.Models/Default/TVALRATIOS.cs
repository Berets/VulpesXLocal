namespace VulpesX.Models.Default;
 
public partial class TVALRATIOS : Base 
{
	private string _socrat = null!;
	public required string socrat { get => _socrat; set { if (_socrat != value) { _socrat = value; NotifyPropertyChanged();} } }
	private string _codrat = null!;
	public required string codrat { get => _codrat; set { if (_codrat != value) { _codrat = value; NotifyPropertyChanged();} } }
	private int _annorat;
	public int annorat { get => _annorat; set { if (_annorat != value) { _annorat = value; NotifyPropertyChanged();} } }
	private decimal? _Valrat;
	public decimal? Valrat { get => _Valrat; set { if (_Valrat != value) { _Valrat = value; NotifyPropertyChanged();} } }
	private DateTime? _Datrat;
	public DateTime? Datrat { get => _Datrat; set { if (_Datrat != value) { _Datrat = value; NotifyPropertyChanged();} } }
	private int? _percrat;
	public int? percrat { get => _percrat; set { if (_percrat != value) { _percrat = value; NotifyPropertyChanged();} } }
}