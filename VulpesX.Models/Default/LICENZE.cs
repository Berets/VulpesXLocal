namespace VulpesX.Models.Default;
 
public partial class LICENZE : Base 
{
	private string _licesociet = null!;
	public required string licesociet { get => _licesociet; set { if (_licesociet != value) { _licesociet = value; NotifyPropertyChanged();} } }
	private string _licetipopr = null!;
	public required string licetipopr { get => _licetipopr; set { if (_licetipopr != value) { _licetipopr = value; NotifyPropertyChanged();} } }
	private int _liceprogre;
	public int liceprogre { get => _liceprogre; set { if (_liceprogre != value) { _liceprogre = value; NotifyPropertyChanged();} } }
	private string? _liceartico;
	public string? liceartico { get => _liceartico; set { if (_liceartico != value) { _liceartico = value; NotifyPropertyChanged();} } }
	private string? _licematric;
	public string? licematric { get => _licematric; set { if (_licematric != value) { _licematric = value; NotifyPropertyChanged();} } }
	private string? _licemarca;
	public string? licemarca { get => _licemarca; set { if (_licemarca != value) { _licemarca = value; NotifyPropertyChanged();} } }
	private string? _licemodell;
	public string? licemodell { get => _licemodell; set { if (_licemodell != value) { _licemodell = value; NotifyPropertyChanged();} } }
	private DateTime? _licedatafa;
	public DateTime? licedatafa { get => _licedatafa; set { if (_licedatafa != value) { _licedatafa = value; NotifyPropertyChanged();} } }
	private string? _licefornit;
	public string? licefornit { get => _licefornit; set { if (_licefornit != value) { _licefornit = value; NotifyPropertyChanged();} } }
	private DateTime? _licedecgar;
	public DateTime? licedecgar { get => _licedecgar; set { if (_licedecgar != value) { _licedecgar = value; NotifyPropertyChanged();} } }
	private DateTime? _licescadga;
	public DateTime? licescadga { get => _licescadga; set { if (_licescadga != value) { _licescadga = value; NotifyPropertyChanged();} } }
	private string? _licenotepr;
	public string? licenotepr { get => _licenotepr; set { if (_licenotepr != value) { _licenotepr = value; NotifyPropertyChanged();} } }
	private string? _licemailte;
	public string? licemailte { get => _licemailte; set { if (_licemailte != value) { _licemailte = value; NotifyPropertyChanged();} } }
	private string? _licemailco;
	public string? licemailco { get => _licemailco; set { if (_licemailco != value) { _licemailco = value; NotifyPropertyChanged();} } }
	private int? _liceclient;
	public int? liceclient { get => _liceclient; set { if (_liceclient != value) { _liceclient = value; NotifyPropertyChanged();} } }
	private int? _licequanti;
	public int? licequanti { get => _licequanti; set { if (_licequanti != value) { _licequanti = value; NotifyPropertyChanged();} } }
	private string? _liceweb;
	public string? liceweb { get => _liceweb; set { if (_liceweb != value) { _liceweb = value; NotifyPropertyChanged();} } }
	private string? _liceserien;
	public string? liceserien { get => _liceserien; set { if (_liceserien != value) { _liceserien = value; NotifyPropertyChanged();} } }
	private int? _licefattur;
	public int? licefattur { get => _licefattur; set { if (_licefattur != value) { _licefattur = value; NotifyPropertyChanged();} } }
	private string? _licedescti;
	public string? licedescti { get => _licedescti; set { if (_licedescti != value) { _licedescti = value; NotifyPropertyChanged();} } }
}