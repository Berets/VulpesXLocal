namespace VulpesX.Models.Default;
 
public partial class ACCESOCE_TASKS_ATT : Base 
{
	private string _societa_id = null!;
	public required string societa_id { get => _societa_id; set { if (_societa_id != value) { _societa_id = value; NotifyPropertyChanged();} } }
	private long _task_id;
	public long task_id { get => _task_id; set { if (_task_id != value) { _task_id = value; NotifyPropertyChanged();} } }
	private Guid _id;
	public Guid id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string _fname = null!;
	public required string fname { get => _fname; set { if (_fname != value) { _fname = value; NotifyPropertyChanged();} } }
	private long? _fsize;
	public long? fsize { get => _fsize; set { if (_fsize != value) { _fsize = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}