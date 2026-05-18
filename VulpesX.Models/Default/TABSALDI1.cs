namespace VulpesX.Models.Default;
 
public partial class TABSALDI1 : Base 
{
	private string _salsoc = null!;
	public required string salsoc { get => _salsoc; set { if (_salsoc != value) { _salsoc = value; NotifyPropertyChanged();} } }
	private int _salann;
	public int salann { get => _salann; set { if (_salann != value) { _salann = value; NotifyPropertyChanged();} } }
	private int _salmes;
	public int salmes { get => _salmes; set { if (_salmes != value) { _salmes = value; NotifyPropertyChanged();} } }
	private DateTime _salpag;
	public DateTime salpag { get => _salpag; set { if (_salpag != value) { _salpag = value; NotifyPropertyChanged();} } }
	private decimal? _salsal;
	public decimal? salsal { get => _salsal; set { if (_salsal != value) { _salsal = value; NotifyPropertyChanged();} } }
	private decimal? _saldeb;
	public decimal? saldeb { get => _saldeb; set { if (_saldeb != value) { _saldeb = value; NotifyPropertyChanged();} } }
}