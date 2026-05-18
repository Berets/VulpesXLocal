using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VulpesX.Models.Default;

public partial class ANFAT00F
{
    public event EventHandler? CustomerChanged;
    protected void OnCustomerChanged(EventArgs e)
    {
        EventHandler? handler = CustomerChanged;
        if (handler != null)
            handler(this, e);
    }

    #region Class 
    public ANFAT00F()
    {
        PropertyChanged += ANFAT00F_PropertyChanged;
    }

    private void ANFAT00F_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
    }
    #endregion

    #region Properties
    public int RowsCount { get; set; }
    #endregion

    #region Getters
    public string PrintFullID => $"{AFANNO}/{AFNUOR}";
    public string PrintFilename => $"AnalisiFattibilita [{AFSOCI}] {AFANNO}-{AFNUOR}.pdf";
    public string EntityFullDescription => AFCOCL.HasValue && AFCOCL.Value > 0 && Customer != null ? $"{Customer.abecod} {Customer.abers1} {Customer.abers2}" : $"[P] {AFRASO}";
    #endregion

    #region Related entities
    private ABE? customer;
    public ABE? Customer
    {
        get => customer;
        set
        {
            if (customer?.abecod != value?.abecod)
            {
                AFCOCL = value?.abecod;
            }
            customer = value;
            NotifyPropertyChanged("Customer");
            OnCustomerChanged(EventArgs.Empty);
        }
    }
    private ObservableCollection<ABE>? customers;
    public ObservableCollection<ABE>? Customers
    {
        get => customers;
        set
        {
            customers = value;
            if (AFCOCL.HasValue && AFCOCL.Value > 0)
                Customer = customers?.Where(w => w.abecod == AFCOCL).FirstOrDefault();
            else
                Customer = null;
            NotifyPropertyChanged("Customers");
        }
    }
    private CLIENTI? customerContacts;
    public CLIENTI? CustomerContacts { get => customerContacts; set { customerContacts = value; NotifyPropertyChanged("CustomerContacts"); } }


    private DESTINATARI? recipient;
    public DESTINATARI? Recipient { get => recipient; set { recipient = value; AFDEST = value?.codesti; NotifyPropertyChanged("Recipient"); } }
    private ObservableCollection<DESTINATARI>? recipients;
    public ObservableCollection<DESTINATARI>? Recipients
    {
        get => recipients;
        set
        {
            recipients = value;
            if (AFDEST.HasValue && AFDEST.Value > 0)
                Recipient = recipients?.Where(w => w.codesti == AFDEST).FirstOrDefault();
            else
                Recipient = null;
            NotifyPropertyChanged("Recipients");
        }
    }

    private LINGUA? language;
    public LINGUA? Language { get => language; set { language = value; AFLING = value?.lincod; NotifyPropertyChanged("Language"); } }
    private ObservableCollection<LINGUA>? languages;
    public ObservableCollection<LINGUA>? Languages
    {
        get => languages;
        set
        {
            languages = value;
            if (!string.IsNullOrWhiteSpace(AFLING))
                Language = languages?.Where(w => w.lincod == AFLING).FirstOrDefault();
            else
                Language = null;
            NotifyPropertyChanged("Languages");
        }
    }

    private COMUNI? city;
    public COMUNI? City { get => city; set { city = value; AFLOCA = value?.comdes; NotifyPropertyChanged("City"); } }
    private ObservableCollection<COMUNI>? cities;
    public ObservableCollection<COMUNI>? Cities
    {
        get => cities;
        set
        {
            cities = value;
            if (!string.IsNullOrWhiteSpace(AFLING))
                City = cities?.Where(w => w.comdes == AFLOCA).FirstOrDefault();
            else
                City = null;
            NotifyPropertyChanged("Cities");
        }
    }

    private TAB_STATES? state;
    public TAB_STATES? State { get => state; set { state = value; AFPROV = value?.cappro; NotifyPropertyChanged("State"); } }
    private ObservableCollection<TAB_STATES>? states;
    public ObservableCollection<TAB_STATES>? States
    {
        get => states;
        set
        {
            states = value;
            if (!string.IsNullOrWhiteSpace(AFPROV))
                State = states?.Where(w => w.cappro == AFPROV).FirstOrDefault();
            else
                State = null;
            NotifyPropertyChanged("States");
        }
    }

    public ObservableCollection<TAB_GEN_TEXTS>? Texts { get; set; }
    #endregion

    #region Info
    public string AddedText => added.HasValue ? added.Value.ToString() : "---";
    public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
    public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
    public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
    public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
    public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
    #endregion
}
