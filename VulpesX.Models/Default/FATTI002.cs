namespace VulpesX.Models.Default;
 
public partial class FATTI002 : Base 
{
	private string _ftesoc = null!;
	public required string ftesoc { get => _ftesoc; set { if (_ftesoc != value) { _ftesoc = value; NotifyPropertyChanged();} } }
	private int _fteann;
	public int fteann { get => _fteann; set { if (_fteann != value) { _fteann = value; NotifyPropertyChanged();} } }
	private int _ftecod;
	public int ftecod { get => _ftecod; set { if (_ftecod != value) { _ftecod = value; NotifyPropertyChanged();} } }
	private string _hntipo = null!;
	public required string hntipo { get => _hntipo; set { if (_hntipo != value) { _hntipo = value; NotifyPropertyChanged();} } }
	private decimal? _hnqtta;
	public decimal? hnqtta { get => _hnqtta; set { if (_hnqtta != value) { _hnqtta = value; NotifyPropertyChanged();} } }
	private decimal? _hnccst;
	public decimal? hnccst { get => _hnccst; set { if (_hnccst != value) { _hnccst = value; NotifyPropertyChanged();} } }
	private decimal? _hnpcre;
	public decimal? hnpcre { get => _hnpcre; set { if (_hnpcre != value) { _hnpcre = value; NotifyPropertyChanged();} } }
	private decimal? _hnmmar;
	public decimal? hnmmar { get => _hnmmar; set { if (_hnmmar != value) { _hnmmar = value; NotifyPropertyChanged();} } }
}