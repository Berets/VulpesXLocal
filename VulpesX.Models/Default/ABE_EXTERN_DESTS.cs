namespace VulpesX.Models.Default;
 
public partial class ABE_EXTERN_DESTS : Base 
{
	private int _abecod;
	public int abecod { get => _abecod; set { if (_abecod != value) { _abecod = value; NotifyPropertyChanged();} } }
	private string _abeextcode = null!;
	public required string abeextcode { get => _abeextcode; set { if (_abeextcode != value) { _abeextcode = value; NotifyPropertyChanged();} } }
	private string _abeextid = null!;
	public required string abeextid { get => _abeextid; set { if (_abeextid != value) { _abeextid = value; NotifyPropertyChanged();} } }
	private string _abeextdid = null!;
	public required string abeextdid { get => _abeextdid; set { if (_abeextdid != value) { _abeextdid = value; NotifyPropertyChanged();} } }
	private int _abedestid;
	public int abedestid { get => _abedestid; set { if (_abedestid != value) { _abedestid = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}