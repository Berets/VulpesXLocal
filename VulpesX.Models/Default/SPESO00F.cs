namespace VulpesX.Models.Default;
 
public partial class SPESO00F : Base 
{
	private string _spestip = null!;
	public required string spestip { get => _spestip; set { if (_spestip != value) { _spestip = value; NotifyPropertyChanged();} } }
	private string _spescfi = null!;
	public required string spescfi { get => _spescfi; set { if (_spescfi != value) { _spescfi = value; NotifyPropertyChanged();} } }
	private int _spesprg;
	public int spesprg { get => _spesprg; set { if (_spesprg != value) { _spesprg = value; NotifyPropertyChanged();} } }
	private string? _spesrec;
	public string? spesrec { get => _spesrec; set { if (_spesrec != value) { _spesrec = value; NotifyPropertyChanged();} } }
}