namespace VulpesX.Models.Default;
 
public partial class TF24CONTIREGIONI : Base 
{
	private string _tf24rsoc = null!;
	public required string tf24rsoc { get => _tf24rsoc; set { if (_tf24rsoc != value) { _tf24rsoc = value; NotifyPropertyChanged();} } }
	private string _tf24rcod = null!;
	public required string tf24rcod { get => _tf24rcod; set { if (_tf24rcod != value) { _tf24rcod = value; NotifyPropertyChanged();} } }
	private string? _tf24rdesc;
	public string? tf24rdesc { get => _tf24rdesc; set { if (_tf24rdesc != value) { _tf24rdesc = value; NotifyPropertyChanged();} } }
	private string? _tf24rgrp;
	public string? tf24rgrp { get => _tf24rgrp; set { if (_tf24rgrp != value) { _tf24rgrp = value; NotifyPropertyChanged();} } }
	private string? _tf24rcto;
	public string? tf24rcto { get => _tf24rcto; set { if (_tf24rcto != value) { _tf24rcto = value; NotifyPropertyChanged();} } }
	private string? _tf24rsotc;
	public string? tf24rsotc { get => _tf24rsotc; set { if (_tf24rsotc != value) { _tf24rsotc = value; NotifyPropertyChanged();} } }
	private string? _tf24rdescagg;
	public string? tf24rdescagg { get => _tf24rdescagg; set { if (_tf24rdescagg != value) { _tf24rdescagg = value; NotifyPropertyChanged();} } }
	private string? _tf24rsodes;
	public string? tf24rsodes { get => _tf24rsodes; set { if (_tf24rsodes != value) { _tf24rsodes = value; NotifyPropertyChanged();} } }
}