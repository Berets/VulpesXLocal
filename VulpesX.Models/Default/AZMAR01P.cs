namespace VulpesX.Models.Default;
 
public partial class AZMAR01P : Base 
{
	private string _AZMCOD = null!;
	public required string AZMCOD { get => _AZMCOD; set { if (_AZMCOD != value) { _AZMCOD = value; NotifyPropertyChanged();} } }
	private string? _AZMDES;
	public string? AZMDES { get => _AZMDES; set { if (_AZMDES != value) { _AZMDES = value; NotifyPropertyChanged();} } }
	private string? _AZMNOT;
	public string? AZMNOT { get => _AZMNOT; set { if (_AZMNOT != value) { _AZMNOT = value; NotifyPropertyChanged();} } }
	private DateTime? _AZMDAI;
	public DateTime? AZMDAI { get => _AZMDAI; set { if (_AZMDAI != value) { _AZMDAI = value; NotifyPropertyChanged();} } }
	private DateTime? _AZMDAF;
	public DateTime? AZMDAF { get => _AZMDAF; set { if (_AZMDAF != value) { _AZMDAF = value; NotifyPropertyChanged();} } }
}