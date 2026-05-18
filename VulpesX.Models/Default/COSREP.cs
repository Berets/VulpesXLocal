namespace VulpesX.Models.Default;
 
public partial class COSREP : Base 
{
	private string _cossoc = null!;
	public required string cossoc { get => _cossoc; set { if (_cossoc != value) { _cossoc = value; NotifyPropertyChanged();} } }
	private string _cosrep = null!;
	public required string cosrep { get => _cosrep; set { if (_cosrep != value) { _cosrep = value; NotifyPropertyChanged();} } }
	private int _cosann;
	public int cosann { get => _cosann; set { if (_cosann != value) { _cosann = value; NotifyPropertyChanged();} } }
	private int _cosmes;
	public int cosmes { get => _cosmes; set { if (_cosmes != value) { _cosmes = value; NotifyPropertyChanged();} } }
	private decimal? _cosore;
	public decimal? cosore { get => _cosore; set { if (_cosore != value) { _cosore = value; NotifyPropertyChanged();} } }
	private string? _cosso1;
	public string? cosso1 { get => _cosso1; set { if (_cosso1 != value) { _cosso1 = value; NotifyPropertyChanged();} } }
}