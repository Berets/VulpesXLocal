namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class ANAFAT_PIECES : Base 
{
	private string _afpsoc = null!;
	public required string afpsoc { get => _afpsoc; set { if (_afpsoc != value) { _afpsoc = value; NotifyPropertyChanged();} } }
	private DateTime _afpdata;
	public DateTime afpdata { get => _afpdata; set { if (_afpdata != value) { _afpdata = value; NotifyPropertyChanged();} } }
	private int _afpver;
	public int afpver { get => _afpver; set { if (_afpver != value) { _afpver = value; NotifyPropertyChanged();} } }
	private int _afppiecesfrom;
	public int afppiecesfrom { get => _afppiecesfrom; set { if (_afppiecesfrom != value) { _afppiecesfrom = value; NotifyPropertyChanged();} } }
	private int? _afppiecesto;
	public int? afppiecesto { get => _afppiecesto; set { if (_afppiecesto != value) { _afppiecesto = value; NotifyPropertyChanged();} } }
	private decimal? _afppercentage;
	public decimal? afppercentage { get => _afppercentage; set { if (_afppercentage != value) { _afppercentage = value; NotifyPropertyChanged();} } }
	private string? _afpproductiontype;
	public string? afpproductiontype { get => _afpproductiontype; set { if (_afpproductiontype != value) { _afpproductiontype = value; NotifyPropertyChanged();} } }
	private bool? _afpproductionaut_enabled;
	public bool? afpproductionaut_enabled { get => _afpproductionaut_enabled; set { if (_afpproductionaut_enabled != value) { _afpproductionaut_enabled = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updateUserID;
	public string? updateUserID { get => _updateUserID; set { if (_updateUserID != value) { _updateUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
}