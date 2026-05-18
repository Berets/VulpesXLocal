namespace VulpesX.Models.Default;
 
public partial class COMPRATORI : Base 
{
	private string _compsoc = null!;
	public required string compsoc { get => _compsoc; set { if (_compsoc != value) { _compsoc = value; NotifyPropertyChanged();} } }
	private string _compcod = null!;
	public required string compcod { get => _compcod; set { if (_compcod != value) { _compcod = value; NotifyPropertyChanged();} } }
	private string? _compnom;
	public string? compnom { get => _compnom; set { if (_compnom != value) { _compnom = value; NotifyPropertyChanged();} } }
	private string? _comptel;
	public string? comptel { get => _comptel; set { if (_comptel != value) { _comptel = value; NotifyPropertyChanged();} } }
	private string? _compfax;
	public string? compfax { get => _compfax; set { if (_compfax != value) { _compfax = value; NotifyPropertyChanged();} } }
}