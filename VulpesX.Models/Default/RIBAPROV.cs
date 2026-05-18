namespace VulpesX.Models.Default;
 
public partial class RIBAPROV : Base 
{
	private string _socicau = null!;
	public required string socicau { get => _socicau; set { if (_socicau != value) { _socicau = value; NotifyPropertyChanged();} } }
	private string _Caurib = null!;
	public required string Caurib { get => _Caurib; set { if (_Caurib != value) { _Caurib = value; NotifyPropertyChanged();} } }
	private string? _causal1;
	public string? causal1 { get => _causal1; set { if (_causal1 != value) { _causal1 = value; NotifyPropertyChanged();} } }
	private string? _causal2;
	public string? causal2 { get => _causal2; set { if (_causal2 != value) { _causal2 = value; NotifyPropertyChanged();} } }
	private DateTime? _caudatu;
	public DateTime? caudatu { get => _caudatu; set { if (_caudatu != value) { _caudatu = value; NotifyPropertyChanged();} } }
	private int? _caudatc;
	public int? caudatc { get => _caudatc; set { if (_caudatc != value) { _caudatc = value; NotifyPropertyChanged();} } }
	private string? _caudes1;
	public string? caudes1 { get => _caudes1; set { if (_caudes1 != value) { _caudes1 = value; NotifyPropertyChanged();} } }
	private string? _caudes2;
	public string? caudes2 { get => _caudes2; set { if (_caudes2 != value) { _caudes2 = value; NotifyPropertyChanged();} } }
	private string? _caudes3;
	public string? caudes3 { get => _caudes3; set { if (_caudes3 != value) { _caudes3 = value; NotifyPropertyChanged();} } }
}