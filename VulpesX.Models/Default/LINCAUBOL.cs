namespace VulpesX.Models.Default;
 
public partial class LINCAUBOL : Base 
{
	private string _Lbolcau = null!;
	public required string Lbolcau { get => _Lbolcau; set { if (_Lbolcau != value) { _Lbolcau = value; NotifyPropertyChanged();} } }
	private string _LbolLin = null!;
	public required string LbolLin { get => _LbolLin; set { if (_LbolLin != value) { _LbolLin = value; NotifyPropertyChanged();} } }
	private string? _LbolDes;
	public string? LbolDes { get => _LbolDes; set { if (_LbolDes != value) { _LbolDes = value; NotifyPropertyChanged();} } }
}