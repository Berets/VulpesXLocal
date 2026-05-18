namespace VulpesX.Models.Default;
 
public partial class FATNOT01 : Base 
{
	private string _ftesoc = null!;
	public required string ftesoc { get => _ftesoc; set { if (_ftesoc != value) { _ftesoc = value; NotifyPropertyChanged();} } }
	private int _fteann;
	public int fteann { get => _fteann; set { if (_fteann != value) { _fteann = value; NotifyPropertyChanged();} } }
	private int _ftecod;
	public int ftecod { get => _ftecod; set { if (_ftecod != value) { _ftecod = value; NotifyPropertyChanged();} } }
	private int _fnorig;
	public int fnorig { get => _fnorig; set { if (_fnorig != value) { _fnorig = value; NotifyPropertyChanged();} } }
	private string? _fnonot;
	public string? fnonot { get => _fnonot; set { if (_fnonot != value) { _fnonot = value; NotifyPropertyChanged();} } }
}