namespace VulpesX.Models.Default;
 
public partial class TCAUBLOCCO : Base 
{
	private string _cblcod = null!;
	public required string cblcod { get => _cblcod; set { if (_cblcod != value) { _cblcod = value; NotifyPropertyChanged();} } }
	private string? _cbldes;
	public string? cbldes { get => _cbldes; set { if (_cbldes != value) { _cbldes = value; NotifyPropertyChanged();} } }
	private string? _cbltip;
	public string? cbltip { get => _cbltip; set { if (_cbltip != value) { _cbltip = value; NotifyPropertyChanged();} } }
}