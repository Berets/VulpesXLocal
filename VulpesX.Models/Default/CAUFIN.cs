namespace VulpesX.Models.Default;
 
public partial class CAUFIN : Base 
{
	private string _cafsoc = null!;
	public required string cafsoc { get => _cafsoc; set { if (_cafsoc != value) { _cafsoc = value; NotifyPropertyChanged();} } }
	private string _caftip = null!;
	public required string caftip { get => _caftip; set { if (_caftip != value) { _caftip = value; NotifyPropertyChanged();} } }
	private string _cafcod = null!;
	public required string cafcod { get => _cafcod; set { if (_cafcod != value) { _cafcod = value; NotifyPropertyChanged();} } }
	private string? _cafdes;
	public string? cafdes { get => _cafdes; set { if (_cafdes != value) { _cafdes = value; NotifyPropertyChanged();} } }
	private string? _cafseg;
	public string? cafseg { get => _cafseg; set { if (_cafseg != value) { _cafseg = value; NotifyPropertyChanged();} } }
	private string? _caffeu;
	public string? caffeu { get => _caffeu; set { if (_caffeu != value) { _caffeu = value; NotifyPropertyChanged();} } }
	private string? _cafrag;
	public string? cafrag { get => _cafrag; set { if (_cafrag != value) { _cafrag = value; NotifyPropertyChanged();} } }
	private string? _cafcog;
	public string? cafcog { get => _cafcog; set { if (_cafcog != value) { _cafcog = value; NotifyPropertyChanged();} } }
	private string? _cafgir;
	public string? cafgir { get => _cafgir; set { if (_cafgir != value) { _cafgir = value; NotifyPropertyChanged();} } }
	private string? _cafcgit;
	public string? cafcgit { get => _cafcgit; set { if (_cafcgit != value) { _cafcgit = value; NotifyPropertyChanged();} } }
	private string? _cafcgi;
	public string? cafcgi { get => _cafcgi; set { if (_cafcgi != value) { _cafcgi = value; NotifyPropertyChanged();} } }
	private string? _cafbon;
	public string? cafbon { get => _cafbon; set { if (_cafbon != value) { _cafbon = value; NotifyPropertyChanged();} } }
	private string? _rbtso1;
	public string? rbtso1 { get => _rbtso1; set { if (_rbtso1 != value) { _rbtso1 = value; NotifyPropertyChanged();} } }
}