namespace VulpesX.Models.Default;
 
public partial class TIPSPESO1 : Base 
{
	private string _tipssoc = null!;
	public required string tipssoc { get => _tipssoc; set { if (_tipssoc != value) { _tipssoc = value; NotifyPropertyChanged();} } }
	private string _tipscod = null!;
	public required string tipscod { get => _tipscod; set { if (_tipscod != value) { _tipscod = value; NotifyPropertyChanged();} } }
	private int _tipsanno;
	public int tipsanno { get => _tipsanno; set { if (_tipsanno != value) { _tipsanno = value; NotifyPropertyChanged();} } }
	private int _tipsmese;
	public int tipsmese { get => _tipsmese; set { if (_tipsmese != value) { _tipsmese = value; NotifyPropertyChanged();} } }
	private string _tipfor = null!;
	public required string tipfor { get => _tipfor; set { if (_tipfor != value) { _tipfor = value; NotifyPropertyChanged();} } }
	private string _tipscli = null!;
	public required string tipscli { get => _tipscli; set { if (_tipscli != value) { _tipscli = value; NotifyPropertyChanged();} } }
}