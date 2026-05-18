namespace VulpesX.Models.Default;
 
public partial class FE_TIPODOC : Base 
{
	private string _FETDCod = null!;
	public required string FETDCod { get => _FETDCod; set { if (_FETDCod != value) { _FETDCod = value; NotifyPropertyChanged();} } }
	private string? _FETDDes;
	public string? FETDDes { get => _FETDDes; set { if (_FETDDes != value) { _FETDDes = value; NotifyPropertyChanged();} } }
	private DateTime? _FETDDat;
	public DateTime? FETDDat { get => _FETDDat; set { if (_FETDDat != value) { _FETDDat = value; NotifyPropertyChanged();} } }
	private string? _FETDACQC;
	public string? FETDACQC { get => _FETDACQC; set { if (_FETDACQC != value) { _FETDACQC = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}