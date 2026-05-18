namespace VulpesX.Models.Default;
 
public partial class TAB_ACC_CLOSING : Base 
{
	private string _cchcod = null!;
	public required string cchcod { get => _cchcod; set { if (_cchcod != value) { _cchcod = value; NotifyPropertyChanged();} } }
	private string _cchdes = null!;
	public required string cchdes { get => _cchdes; set { if (_cchdes != value) { _cchdes = value; NotifyPropertyChanged();} } }
	private string? _cchchi;
	public string? cchchi { get => _cchchi; set { if (_cchchi != value) { _cchchi = value; NotifyPropertyChanged();} } }
	private string? _cchria;
	public string? cchria { get => _cchria; set { if (_cchria != value) { _cchria = value; NotifyPropertyChanged();} } }
	private string? _cchpes;
	public string? cchpes { get => _cchpes; set { if (_cchpes != value) { _cchpes = value; NotifyPropertyChanged();} } }
	private string? _cchues;
	public string? cchues { get => _cchues; set { if (_cchues != value) { _cchues = value; NotifyPropertyChanged();} } }
	private string? _cchppr;
	public string? cchppr { get => _cchppr; set { if (_cchppr != value) { _cchppr = value; NotifyPropertyChanged();} } }
	private string? _cchgrc;
	public string? cchgrc { get => _cchgrc; set { if (_cchgrc != value) { _cchgrc = value; NotifyPropertyChanged();} } }
	private string? _cchctc;
	public string? cchctc { get => _cchctc; set { if (_cchctc != value) { _cchctc = value; NotifyPropertyChanged();} } }
	private string? _cchstc;
	public string? cchstc { get => _cchstc; set { if (_cchstc != value) { _cchstc = value; NotifyPropertyChanged();} } }
	private string? _cchgrr;
	public string? cchgrr { get => _cchgrr; set { if (_cchgrr != value) { _cchgrr = value; NotifyPropertyChanged();} } }
	private string? _cchctr;
	public string? cchctr { get => _cchctr; set { if (_cchctr != value) { _cchctr = value; NotifyPropertyChanged();} } }
	private string? _cchstr;
	public string? cchstr { get => _cchstr; set { if (_cchstr != value) { _cchstr = value; NotifyPropertyChanged();} } }
	private string? _cchgrp;
	public string? cchgrp { get => _cchgrp; set { if (_cchgrp != value) { _cchgrp = value; NotifyPropertyChanged();} } }
	private string? _cchctp;
	public string? cchctp { get => _cchctp; set { if (_cchctp != value) { _cchctp = value; NotifyPropertyChanged();} } }
	private string? _cchstp;
	public string? cchstp { get => _cchstp; set { if (_cchstp != value) { _cchstp = value; NotifyPropertyChanged();} } }
	private string? _cchgru;
	public string? cchgru { get => _cchgru; set { if (_cchgru != value) { _cchgru = value; NotifyPropertyChanged();} } }
	private string? _cchctu;
	public string? cchctu { get => _cchctu; set { if (_cchctu != value) { _cchctu = value; NotifyPropertyChanged();} } }
	private string? _cchstu;
	public string? cchstu { get => _cchstu; set { if (_cchstu != value) { _cchstu = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
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
}