namespace VulpesX.Models.Default;
 
public partial class TipoRicevuta : Base 
{
	private string _TRCOD = null!;
	public required string TRCOD { get => _TRCOD; set { if (_TRCOD != value) { _TRCOD = value; NotifyPropertyChanged();} } }
	private string _TRDES = null!;
	public required string TRDES { get => _TRDES; set { if (_TRDES != value) { _TRDES = value; NotifyPropertyChanged();} } }
	private string _TRFLG = null!;
	public required string TRFLG { get => _TRFLG; set { if (_TRFLG != value) { _TRFLG = value; NotifyPropertyChanged();} } }
}