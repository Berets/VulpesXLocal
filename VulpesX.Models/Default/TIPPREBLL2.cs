namespace VulpesX.Models.Default;
 
public partial class TIPPREBLL2 : Base 
{
	private string _tipblsoc = null!;
	public required string tipblsoc { get => _tipblsoc; set { if (_tipblsoc != value) { _tipblsoc = value; NotifyPropertyChanged();} } }
	private string _tipblcod = null!;
	public required string tipblcod { get => _tipblcod; set { if (_tipblcod != value) { _tipblcod = value; NotifyPropertyChanged();} } }
	private string _tipgrup = null!;
	public required string tipgrup { get => _tipgrup; set { if (_tipgrup != value) { _tipgrup = value; NotifyPropertyChanged();} } }
	private string _tipcont = null!;
	public required string tipcont { get => _tipcont; set { if (_tipcont != value) { _tipcont = value; NotifyPropertyChanged();} } }
	private string _tipsotc = null!;
	public required string tipsotc { get => _tipsotc; set { if (_tipsotc != value) { _tipsotc = value; NotifyPropertyChanged();} } }
	private string? _tipbese;
	public string? tipbese { get => _tipbese; set { if (_tipbese != value) { _tipbese = value; NotifyPropertyChanged();} } }
}