namespace VulpesX.Models.Default;
 
public partial class FATTES011 : Base 
{
	private string _ftesoc = null!;
	public required string ftesoc { get => _ftesoc; set { if (_ftesoc != value) { _ftesoc = value; NotifyPropertyChanged();} } }
	private int _fteann;
	public int fteann { get => _fteann; set { if (_fteann != value) { _fteann = value; NotifyPropertyChanged();} } }
	private int _ftecod;
	public int ftecod { get => _ftecod; set { if (_ftecod != value) { _ftecod = value; NotifyPropertyChanged();} } }
	private int _fteria;
	public int fteria { get => _fteria; set { if (_fteria != value) { _fteria = value; NotifyPropertyChanged();} } }
	private string? _ftetde;
	public string? ftetde { get => _ftetde; set { if (_ftetde != value) { _ftetde = value; NotifyPropertyChanged();} } }
	private string? _fteper;
	public string? fteper { get => _fteper; set { if (_fteper != value) { _fteper = value; NotifyPropertyChanged();} } }
}