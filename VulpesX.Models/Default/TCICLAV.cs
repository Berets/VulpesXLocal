namespace VulpesX.Models.Default;
 
public partial class TCICLAV : Base 
{
	private string _cicsoc = null!;
	public required string cicsoc { get => _cicsoc; set { if (_cicsoc != value) { _cicsoc = value; NotifyPropertyChanged();} } }
	private string _cicart = null!;
	public required string cicart { get => _cicart; set { if (_cicart != value) { _cicart = value; NotifyPropertyChanged();} } }
	private string? _cicdec;
	public string? cicdec { get => _cicdec; set { if (_cicdec != value) { _cicdec = value; NotifyPropertyChanged();} } }
}