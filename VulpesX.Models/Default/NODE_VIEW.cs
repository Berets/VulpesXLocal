namespace VulpesX.Models.Default;
 
public partial class NODE_VIEW : Base 
{
	private string _viewsoc = null!;
	public required string viewsoc { get => _viewsoc; set { if (_viewsoc != value) { _viewsoc = value; NotifyPropertyChanged();} } }
	private int _viewann;
	public int viewann { get => _viewann; set { if (_viewann != value) { _viewann = value; NotifyPropertyChanged();} } }
	private int _viewfil;
	public int viewfil { get => _viewfil; set { if (_viewfil != value) { _viewfil = value; NotifyPropertyChanged();} } }
	private string _viewtip = null!;
	public required string viewtip { get => _viewtip; set { if (_viewtip != value) { _viewtip = value; NotifyPropertyChanged();} } }
	private bool? _viewval;
	public bool? viewval { get => _viewval; set { if (_viewval != value) { _viewval = value; NotifyPropertyChanged();} } }
	private bool? _viewpre;
	public bool? viewpre { get => _viewpre; set { if (_viewpre != value) { _viewpre = value; NotifyPropertyChanged();} } }
	private bool? _viewprecal;
	public bool? viewprecal { get => _viewprecal; set { if (_viewprecal != value) { _viewprecal = value; NotifyPropertyChanged();} } }
	private bool? _viewbud;
	public bool? viewbud { get => _viewbud; set { if (_viewbud != value) { _viewbud = value; NotifyPropertyChanged();} } }
	private bool? _viewnode2;
	public bool? viewnode2 { get => _viewnode2; set { if (_viewnode2 != value) { _viewnode2 = value; NotifyPropertyChanged();} } }
	private bool? _viewnode3;
	public bool? viewnode3 { get => _viewnode3; set { if (_viewnode3 != value) { _viewnode3 = value; NotifyPropertyChanged();} } }
	private int? _viewper;
	public int? viewper { get => _viewper; set { if (_viewper != value) { _viewper = value; NotifyPropertyChanged();} } }
	private bool? _viewvalcal;
	public bool? viewvalcal { get => _viewvalcal; set { if (_viewvalcal != value) { _viewvalcal = value; NotifyPropertyChanged();} } }
}