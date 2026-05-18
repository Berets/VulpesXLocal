namespace VulpesX.Models.Default;
 
public partial class TCODLIQIVA : Base 
{
	private string _CVISoc = null!;
	public required string CVISoc { get => _CVISoc; set { if (_CVISoc != value) { _CVISoc = value; NotifyPropertyChanged();} } }
	private string _CVICod = null!;
	public required string CVICod { get => _CVICod; set { if (_CVICod != value) { _CVICod = value; NotifyPropertyChanged();} } }
	private string _CVIDes = null!;
	public required string CVIDes { get => _CVIDes; set { if (_CVIDes != value) { _CVIDes = value; NotifyPropertyChanged();} } }
	private string _CVITipo = null!;
	public required string CVITipo { get => _CVITipo; set { if (_CVITipo != value) { _CVITipo = value; NotifyPropertyChanged();} } }
	private int _CVISeq;
	public int CVISeq { get => _CVISeq; set { if (_CVISeq != value) { _CVISeq = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}