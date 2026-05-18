namespace VulpesX.Models.Default;
 
public partial class TESLIN : Base 
{
	private string _txlcod = null!;
	public required string txlcod { get => _txlcod; set { if (_txlcod != value) { _txlcod = value; NotifyPropertyChanged();} } }
	private string _txldoc = null!;
	public required string txldoc { get => _txldoc; set { if (_txldoc != value) { _txldoc = value; NotifyPropertyChanged();} } }
	private int _txltex;
	public int txltex { get => _txltex; set { if (_txltex != value) { _txltex = value; NotifyPropertyChanged();} } }
	private string? _txldeI;
	public string? txldeI { get => _txldeI; set { if (_txldeI != value) { _txldeI = value; NotifyPropertyChanged();} } }
	private string? _txldeL;
	public string? txldeL { get => _txldeL; set { if (_txldeL != value) { _txldeL = value; NotifyPropertyChanged();} } }
}