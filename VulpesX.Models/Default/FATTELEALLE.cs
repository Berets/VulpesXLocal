namespace VulpesX.Models.Default;
 
public partial class FATTELEALLE : Base 
{
	private string _ftsoci = null!;
	public required string ftsoci { get => _ftsoci; set { if (_ftsoci != value) { _ftsoci = value; NotifyPropertyChanged();} } }
	private int _FTANNO;
	public int FTANNO { get => _FTANNO; set { if (_FTANNO != value) { _FTANNO = value; NotifyPropertyChanged();} } }
	private int _FTNUOR;
	public int FTNUOR { get => _FTNUOR; set { if (_FTNUOR != value) { _FTNUOR = value; NotifyPropertyChanged();} } }
	private int _FTRIGALLE;
	public int FTRIGALLE { get => _FTRIGALLE; set { if (_FTRIGALLE != value) { _FTRIGALLE = value; NotifyPropertyChanged();} } }
	private string _FTPERALLE = null!;
	public required string FTPERALLE { get => _FTPERALLE; set { if (_FTPERALLE != value) { _FTPERALLE = value; NotifyPropertyChanged();} } }
}