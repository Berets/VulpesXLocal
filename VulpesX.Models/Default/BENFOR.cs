namespace VulpesX.Models.Default;
 
public partial class BENFOR : Base 
{
	private string _bensoc = null!;
	public required string bensoc { get => _bensoc; set { if (_bensoc != value) { _bensoc = value; NotifyPropertyChanged();} } }
	private int _benfor;
	public int benfor { get => _benfor; set { if (_benfor != value) { _benfor = value; NotifyPropertyChanged();} } }
	private int _benann;
	public int benann { get => _benann; set { if (_benann != value) { _benann = value; NotifyPropertyChanged();} } }
	private string _benrif = null!;
	public required string benrif { get => _benrif; set { if (_benrif != value) { _benrif = value; NotifyPropertyChanged();} } }
	private string? _bentip;
	public string? bentip { get => _bentip; set { if (_bentip != value) { _bentip = value; NotifyPropertyChanged();} } }
	private string? _bennom;
	public string? bennom { get => _bennom; set { if (_bennom != value) { _bennom = value; NotifyPropertyChanged();} } }
	private DateTime? _bendat;
	public DateTime? bendat { get => _bendat; set { if (_bendat != value) { _bendat = value; NotifyPropertyChanged();} } }
	private int? _benfac;
	public int? benfac { get => _benfac; set { if (_benfac != value) { _benfac = value; NotifyPropertyChanged();} } }
	private string? _benfss;
	public string? benfss { get => _benfss; set { if (_benfss != value) { _benfss = value; NotifyPropertyChanged();} } }
	private DateTime? _bendsc;
	public DateTime? bendsc { get => _bendsc; set { if (_bendsc != value) { _bendsc = value; NotifyPropertyChanged();} } }
}