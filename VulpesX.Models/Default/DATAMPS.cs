namespace VulpesX.Models.Default;
 
public partial class DATAMPS : Base 
{
	private string _DATSOC = null!;
	public required string DATSOC { get => _DATSOC; set { if (_DATSOC != value) { _DATSOC = value; NotifyPropertyChanged();} } }
	private DateTime _DATINI;
	public DateTime DATINI { get => _DATINI; set { if (_DATINI != value) { _DATINI = value; NotifyPropertyChanged();} } }
	private DateTime? _DATORI;
	public DateTime? DATORI { get => _DATORI; set { if (_DATORI != value) { _DATORI = value; NotifyPropertyChanged();} } }
	private DateTime? _DATFIN;
	public DateTime? DATFIN { get => _DATFIN; set { if (_DATFIN != value) { _DATFIN = value; NotifyPropertyChanged();} } }
	private DateTime? _DATSCA;
	public DateTime? DATSCA { get => _DATSCA; set { if (_DATSCA != value) { _DATSCA = value; NotifyPropertyChanged();} } }
	private int? _DATFOR;
	public int? DATFOR { get => _DATFOR; set { if (_DATFOR != value) { _DATFOR = value; NotifyPropertyChanged();} } }
}