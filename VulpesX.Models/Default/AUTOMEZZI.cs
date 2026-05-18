namespace VulpesX.Models.Default;
 
public partial class AUTOMEZZI : Base 
{
	private string _autsoc = null!;
	public required string autsoc { get => _autsoc; set { if (_autsoc != value) { _autsoc = value; NotifyPropertyChanged();} } }
	private string _auttar = null!;
	public required string auttar { get => _auttar; set { if (_auttar != value) { _auttar = value; NotifyPropertyChanged();} } }
	private string? _autdes;
	public string? autdes { get => _autdes; set { if (_autdes != value) { _autdes = value; NotifyPropertyChanged();} } }
	private string? _autaut;
	public string? autaut { get => _autaut; set { if (_autaut != value) { _autaut = value; NotifyPropertyChanged();} } }
	private int? _autpes;
	public int? autpes { get => _autpes; set { if (_autpes != value) { _autpes = value; NotifyPropertyChanged();} } }
	private DateTime? _autdat;
	public DateTime? autdat { get => _autdat; set { if (_autdat != value) { _autdat = value; NotifyPropertyChanged();} } }
	private string? _sociau;
	public string? sociau { get => _sociau; set { if (_sociau != value) { _sociau = value; NotifyPropertyChanged();} } }
}