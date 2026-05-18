namespace VulpesX.Models.Default;
 
public partial class TipoPagaNC : Base 
{
	private string _TPCOD = null!;
	public required string TPCOD { get => _TPCOD; set { if (_TPCOD != value) { _TPCOD = value; NotifyPropertyChanged();} } }
	private string _TPDES = null!;
	public required string TPDES { get => _TPDES; set { if (_TPDES != value) { _TPDES = value; NotifyPropertyChanged();} } }
	private string _TPFLP = null!;
	public required string TPFLP { get => _TPFLP; set { if (_TPFLP != value) { _TPFLP = value; NotifyPropertyChanged();} } }
}