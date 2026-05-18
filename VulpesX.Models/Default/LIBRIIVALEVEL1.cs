namespace VulpesX.Models.Default;
 
public partial class LIBRIIVALEVEL1 : Base 
{
	private string _livcod = null!;
	public required string livcod { get => _livcod; set { if (_livcod != value) { _livcod = value; NotifyPropertyChanged();} } }
	private int _LivAnno;
	public int LivAnno { get => _LivAnno; set { if (_LivAnno != value) { _LivAnno = value; NotifyPropertyChanged();} } }
	private int? _LivProtNum;
	public int? LivProtNum { get => _LivProtNum; set { if (_LivProtNum != value) { _LivProtNum = value; NotifyPropertyChanged();} } }
}