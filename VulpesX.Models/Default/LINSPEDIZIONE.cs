namespace VulpesX.Models.Default;
 
public partial class LINSPEDIZIONE : Base 
{
	private string _Lspecod = null!;
	public required string Lspecod { get => _Lspecod; set { if (_Lspecod != value) { _Lspecod = value; NotifyPropertyChanged();} } }
	private string _lspelin = null!;
	public required string lspelin { get => _lspelin; set { if (_lspelin != value) { _lspelin = value; NotifyPropertyChanged();} } }
	private string? _Lspedes;
	public string? Lspedes { get => _Lspedes; set { if (_Lspedes != value) { _Lspedes = value; NotifyPropertyChanged();} } }
}