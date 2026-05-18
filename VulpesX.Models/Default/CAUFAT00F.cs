namespace VulpesX.Models.Default;
 
public partial class CAUFAT00F : Base 
{
	private string _fatcod = null!;
	public required string fatcod { get => _fatcod; set { if (_fatcod != value) { _fatcod = value; NotifyPropertyChanged();} } }
	private string _fatdes = null!;
	public required string fatdes { get => _fatdes; set { if (_fatdes != value) { _fatdes = value; NotifyPropertyChanged();} } }
	private string? _flgcon;
	public string? flgcon { get => _flgcon; set { if (_flgcon != value) { _flgcon = value; NotifyPropertyChanged();} } }
	private string? _fatcon;
	public string? fatcon { get => _fatcon; set { if (_fatcon != value) { _fatcon = value; NotifyPropertyChanged();} } }
	private string? _fataut;
	public string? fataut { get => _fataut; set { if (_fataut != value) { _fataut = value; NotifyPropertyChanged();} } }
	private string _fattif = null!;
	public required string fattif { get => _fattif; set { if (_fattif != value) { _fattif = value; NotifyPropertyChanged();} } }
	private string? _fatnmr;
	public string? fatnmr { get => _fatnmr; set { if (_fatnmr != value) { _fatnmr = value; NotifyPropertyChanged();} } }
	private string? _fattido;
	public string? fattido { get => _fattido; set { if (_fattido != value) { _fattido = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _fatpre;
	public string? fatpre { get => _fatpre; set { if (_fatpre != value) { _fatpre = value; NotifyPropertyChanged();} } }
	private string? _fatcaut;
	public string? fatcaut { get => _fatcaut; set { if (_fatcaut != value) { _fatcaut = value; NotifyPropertyChanged();} } }
}