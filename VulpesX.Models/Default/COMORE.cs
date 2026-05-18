namespace VulpesX.Models.Default;
 
public partial class COMORE : Base 
{
	private string _cosoc = null!;
	public required string cosoc { get => _cosoc; set { if (_cosoc != value) { _cosoc = value; NotifyPropertyChanged();} } }
	private int _coann;
	public int coann { get => _coann; set { if (_coann != value) { _coann = value; NotifyPropertyChanged();} } }
	private string _cocom = null!;
	public required string cocom { get => _cocom; set { if (_cocom != value) { _cocom = value; NotifyPropertyChanged();} } }
	private decimal? _coore;
	public decimal? coore { get => _coore; set { if (_coore != value) { _coore = value; NotifyPropertyChanged();} } }
}