namespace VulpesX.Models.Default;
 
public partial class DWDOCUMENTI : Base 
{
	private string _SOMCOD = null!;
	public required string SOMCOD { get => _SOMCOD; set { if (_SOMCOD != value) { _SOMCOD = value; NotifyPropertyChanged();} } }
	private string _DocuNume = null!;
	public required string DocuNume { get => _DocuNume; set { if (_DocuNume != value) { _DocuNume = value; NotifyPropertyChanged();} } }
	private DateTime _DocumeDa;
	public DateTime DocumeDa { get => _DocumeDa; set { if (_DocumeDa != value) { _DocumeDa = value; NotifyPropertyChanged();} } }
	private string? _documeto;
	public string? documeto { get => _documeto; set { if (_documeto != value) { _documeto = value; NotifyPropertyChanged();} } }
}