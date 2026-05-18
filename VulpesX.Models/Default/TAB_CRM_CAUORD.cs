namespace VulpesX.Models.Default;
 
public partial class TAB_CRM_CAUORD : Base 
{
	private string _cauacq = null!;
	public required string cauacq { get => _cauacq; set { if (_cauacq != value) { _cauacq = value; NotifyPropertyChanged();} } }
	private string? _caudec;
	public string? caudec { get => _caudec; set { if (_caudec != value) { _caudec = value; NotifyPropertyChanged();} } }
	private string? _caufla;
	public string? caufla { get => _caufla; set { if (_caufla != value) { _caufla = value; NotifyPropertyChanged();} } }
	private string? _cauflb;
	public string? cauflb { get => _cauflb; set { if (_cauflb != value) { _cauflb = value; NotifyPropertyChanged();} } }
	private string? _caubol;
	public string? caubol { get => _caubol; set { if (_caubol != value) { _caubol = value; NotifyPropertyChanged();} } }
	private string? _caugrp;
	public string? caugrp { get => _caugrp; set { if (_caugrp != value) { _caugrp = value; NotifyPropertyChanged();} } }
	private string? _caucon;
	public string? caucon { get => _caucon; set { if (_caucon != value) { _caucon = value; NotifyPropertyChanged();} } }
	private string? _causoc;
	public string? causoc { get => _causoc; set { if (_causoc != value) { _causoc = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string _cauacqsoc = null!;
	public required string cauacqsoc { get => _cauacqsoc; set { if (_cauacqsoc != value) { _cauacqsoc = value; NotifyPropertyChanged();} } }
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
	private string? _cauacqtxc;
	public string? cauacqtxc { get => _cauacqtxc; set { if (_cauacqtxc != value) { _cauacqtxc = value; NotifyPropertyChanged();} } }
	private string? _cauacqtxt;
	public string? cauacqtxt { get => _cauacqtxt; set { if (_cauacqtxt != value) { _cauacqtxt = value; NotifyPropertyChanged();} } }
	private string? _caufat;
	public string? caufat { get => _caufat; set { if (_caufat != value) { _caufat = value; NotifyPropertyChanged();} } }
}