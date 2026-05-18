namespace VulpesX.Models.Default;
 
public partial class AUTISTI : Base 
{
	private string _socaut = null!;
	public required string socaut { get => _socaut; set { if (_socaut != value) { _socaut = value; NotifyPropertyChanged();} } }
	private string _autcod = null!;
	public required string autcod { get => _autcod; set { if (_autcod != value) { _autcod = value; NotifyPropertyChanged();} } }
	private string? _autcon;
	public string? autcon { get => _autcon; set { if (_autcon != value) { _autcon = value; NotifyPropertyChanged();} } }
	private string? _autnom;
	public string? autnom { get => _autnom; set { if (_autnom != value) { _autnom = value; NotifyPropertyChanged();} } }
	private string? _autflg;
	public string? autflg { get => _autflg; set { if (_autflg != value) { _autflg = value; NotifyPropertyChanged();} } }
	private string? _autrag;
	public string? autrag { get => _autrag; set { if (_autrag != value) { _autrag = value; NotifyPropertyChanged();} } }
	private string? _autind;
	public string? autind { get => _autind; set { if (_autind != value) { _autind = value; NotifyPropertyChanged();} } }
	private int? _autcap;
	public int? autcap { get => _autcap; set { if (_autcap != value) { _autcap = value; NotifyPropertyChanged();} } }
	private string? _autloc;
	public string? autloc { get => _autloc; set { if (_autloc != value) { _autloc = value; NotifyPropertyChanged();} } }
	private string? _autpr1;
	public string? autpr1 { get => _autpr1; set { if (_autpr1 != value) { _autpr1 = value; NotifyPropertyChanged();} } }
	private int? _autpr2;
	public int? autpr2 { get => _autpr2; set { if (_autpr2 != value) { _autpr2 = value; NotifyPropertyChanged();} } }
}