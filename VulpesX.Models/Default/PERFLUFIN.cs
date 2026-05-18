namespace VulpesX.Models.Default;
 
public partial class PERFLUFIN : Base 
{
	private string _Flusoc = null!;
	public required string Flusoc { get => _Flusoc; set { if (_Flusoc != value) { _Flusoc = value; NotifyPropertyChanged();} } }
	private string? _fluorcl;
	public string? fluorcl { get => _fluorcl; set { if (_fluorcl != value) { _fluorcl = value; NotifyPropertyChanged();} } }
	private string? _fluorfo;
	public string? fluorfo { get => _fluorfo; set { if (_fluorfo != value) { _fluorfo = value; NotifyPropertyChanged();} } }
	private string? _Flubocl;
	public string? Flubocl { get => _Flubocl; set { if (_Flubocl != value) { _Flubocl = value; NotifyPropertyChanged();} } }
	private string? _Flubofo;
	public string? Flubofo { get => _Flubofo; set { if (_Flubofo != value) { _Flubofo = value; NotifyPropertyChanged();} } }
	private string? _Flusacl;
	public string? Flusacl { get => _Flusacl; set { if (_Flusacl != value) { _Flusacl = value; NotifyPropertyChanged();} } }
	private string? _Flusafo;
	public string? Flusafo { get => _Flusafo; set { if (_Flusafo != value) { _Flusafo = value; NotifyPropertyChanged();} } }
	private int? _flumri;
	public int? flumri { get => _flumri; set { if (_flumri != value) { _flumri = value; NotifyPropertyChanged();} } }
	private string? _fluport;
	public string? fluport { get => _fluport; set { if (_fluport != value) { _fluport = value; NotifyPropertyChanged();} } }
	private string? _fluant;
	public string? fluant { get => _fluant; set { if (_fluant != value) { _fluant = value; NotifyPropertyChanged();} } }
}