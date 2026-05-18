namespace VulpesX.Models.Default;
 
public partial class FATINDW : Base 
{
	private string _ftesoc = null!;
	public required string ftesoc { get => _ftesoc; set { if (_ftesoc != value) { _ftesoc = value; NotifyPropertyChanged();} } }
	private int _fteann;
	public int fteann { get => _fteann; set { if (_fteann != value) { _fteann = value; NotifyPropertyChanged();} } }
	private int _ftecod;
	public int ftecod { get => _ftecod; set { if (_ftecod != value) { _ftecod = value; NotifyPropertyChanged();} } }
	private int _fterig;
	public int fterig { get => _fterig; set { if (_fterig != value) { _fterig = value; NotifyPropertyChanged();} } }
	private string? _fteute;
	public string? fteute { get => _fteute; set { if (_fteute != value) { _fteute = value; NotifyPropertyChanged();} } }
	private string? _fteinm;
	public string? fteinm { get => _fteinm; set { if (_fteinm != value) { _fteinm = value; NotifyPropertyChanged();} } }
	private string? _ftetem;
	public string? ftetem { get => _ftetem; set { if (_ftetem != value) { _ftetem = value; NotifyPropertyChanged();} } }
}