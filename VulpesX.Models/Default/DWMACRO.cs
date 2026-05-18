namespace VulpesX.Models.Default;
 
public partial class DWMACRO : Base 
{
	private string _SOMCOD = null!;
	public required string SOMCOD { get => _SOMCOD; set { if (_SOMCOD != value) { _SOMCOD = value; NotifyPropertyChanged();} } }
	private string _dwcoma = null!;
	public required string dwcoma { get => _dwcoma; set { if (_dwcoma != value) { _dwcoma = value; NotifyPropertyChanged();} } }
	private string? _dwmade;
	public string? dwmade { get => _dwmade; set { if (_dwmade != value) { _dwmade = value; NotifyPropertyChanged();} } }
}