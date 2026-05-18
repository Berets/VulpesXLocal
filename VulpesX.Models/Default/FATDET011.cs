namespace VulpesX.Models.Default;
 
public partial class FATDET011 : Base 
{
	private string _ftesoc = null!;
	public required string ftesoc { get => _ftesoc; set { if (_ftesoc != value) { _ftesoc = value; NotifyPropertyChanged();} } }
	private int _fteann;
	public int fteann { get => _fteann; set { if (_fteann != value) { _fteann = value; NotifyPropertyChanged();} } }
	private int _ftecod;
	public int ftecod { get => _ftecod; set { if (_ftecod != value) { _ftecod = value; NotifyPropertyChanged();} } }
	private int _fderig;
	public int fderig { get => _fderig; set { if (_fderig != value) { _fderig = value; NotifyPropertyChanged();} } }
	private int _fderia;
	public int fderia { get => _fderia; set { if (_fderia != value) { _fderia = value; NotifyPropertyChanged();} } }
	private string? _fdetdo;
	public string? fdetdo { get => _fdetdo; set { if (_fdetdo != value) { _fdetdo = value; NotifyPropertyChanged();} } }
	private string? _fdeper;
	public string? fdeper { get => _fdeper; set { if (_fdeper != value) { _fdeper = value; NotifyPropertyChanged();} } }
}