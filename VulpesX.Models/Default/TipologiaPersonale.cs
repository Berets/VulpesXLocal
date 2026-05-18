namespace VulpesX.Models.Default;
 
public partial class TipologiaPersonale : Base 
{
	private string _TIPECOD = null!;
	public required string TIPECOD { get => _TIPECOD; set { if (_TIPECOD != value) { _TIPECOD = value; NotifyPropertyChanged();} } }
	private string _TIPEDES = null!;
	public required string TIPEDES { get => _TIPEDES; set { if (_TIPEDES != value) { _TIPEDES = value; NotifyPropertyChanged();} } }
}