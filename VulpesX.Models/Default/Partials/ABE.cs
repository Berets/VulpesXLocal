using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VulpesX.Models.Default
{
    public partial class ABE
    {
        public ABE()
        {
            PropertyChanged += ABE_PropertyChanged;
        }

        private void ABE_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "abecfe")
            {
                NotifyPropertyChanged("CanCustomer");
                NotifyPropertyChanged("CanSupplier");
                NotifyPropertyChanged("CanCustomerVisibility");
                NotifyPropertyChanged("CanSupplierVisibility");
            }
        }

        public int? IsObsolete { get; set; }

        public string FullDescription => IsObsolete == null ? $"{abers1?.Trim()} {abers2?.Trim()}" : $"NEW[{IsObsolete}] - {abers1?.Trim()} {abers2?.Trim()}";
        public string FullDescriptionSearchable => IsObsolete == null ? abecod > 0 ? $"{abecod} {abers1?.Trim()} {abers2?.Trim()}" : $"{abers1?.Trim()} {abers2?.Trim()}" : abecod > 0 ? $"NEW[{IsObsolete}] - {abecod} {abers1?.Trim()} {abers2?.Trim()}" : $"NEW[{IsObsolete}] - {abers1?.Trim()} {abers2?.Trim()}";
        public string FullDescriptionNotSearchable => abecod > 0 ? $"[{abecod}] {abers1?.Trim()} {abers2?.Trim()}" : $"{abers1?.Trim()} {abers2?.Trim()}";
        public string FullDescriptionSearchablePrint => $"{(abecod > 0 ? $"{abecod} " : null)}{abers1?.Trim()} {abers2?.Trim()}";
        public string ItemTypeDescription => abecfe == "P" ? "Prospect" : abecfe == "E" ? "Entrambi" : abecfe == "C" ? "Cliente" : "Fornitore";



        #region Tab visibility
        public bool CanCustomer => abecfe == "C" || abecfe == "E" || abecfe == "P";
        public bool CanSupplier => abecfe == "F" || abecfe == "E";
        public bool CanCustomerVisibility => abecfe == "C" || abecfe == "E" || abecfe == "P" ? true : false;
        public bool CanSupplierVisibility => abecfe == "F" || abecfe == "E" ? true : false;
        #endregion

        #region ISO
        private ObservableCollection<ISO>? isoList;
        public ObservableCollection<ISO>? ISOList
        {
            get => isoList;
            set
            {
                isoList = value;
                if (!string.IsNullOrWhiteSpace(isocod))
                    ISO = isoList?.Where(w => w.isocod == isocod).FirstOrDefault();
                else
                    ISO = null;
                NotifyPropertyChanged("ISOList");
            }
        }

        private ISO? iso;
        public ISO? ISO
        {
            get => iso;
            set
            {
                isocod = value?.isocod;
                iso = value;

                NotifyPropertyChanged("ISO");
            }
        }
        #endregion

        #region Country
        private ObservableCollection<NAZIONI>? countries;
        public ObservableCollection<NAZIONI>? Countries
        {
            get => countries;
            set
            {
                countries = value;
                if (!string.IsNullOrWhiteSpace(nazcod))
                    Country = countries?.Where(w => w.nazcod == nazcod).FirstOrDefault();
                else
                    Country = null;
                NotifyPropertyChanged("Countries");
            }
        }

        private NAZIONI? country;
        public NAZIONI? Country
        {
            get => country;
            set
            {
                nazcod = value?.nazcod;
                country = value;
                NotifyPropertyChanged("Country");
            }
        }
        #endregion

        #region CompanyType
        private ObservableCollection<SOCIETA>? companyTypes;
        public ObservableCollection<SOCIETA>? CompanyTypes
        {
            get => companyTypes;
            set
            {
                companyTypes = value;
                if (!string.IsNullOrWhiteSpace(soctip))
                    CompanyType = companyTypes?.Where(w => w.soctip == soctip).FirstOrDefault();
                else
                    CompanyType = null;
                NotifyPropertyChanged("CompanyTypes");
            }
        }

        private SOCIETA? companyType;
        public SOCIETA? CompanyType
        {
            get => companyType;
            set
            {
                soctip = value?.soctip;
                companyType = value;
                NotifyPropertyChanged("CompanyType");
            }
        }
        #endregion

        #region Cities
        private ObservableCollection<COMUNI>? cities;
        public ObservableCollection<COMUNI>? Cities
        {
            get => cities;
            set
            {
                cities = value;
                // city
                if (!string.IsNullOrWhiteSpace(abeloc))
                    City = abeloc;
                else
                    City = null;
                // legal city
                if (!string.IsNullOrWhiteSpace(abelol))
                    LegalCity = cities?.Where(w => w.comdes == abelol).FirstOrDefault();
                else
                    LegalCity = null;
                // birth city
                if (!string.IsNullOrWhiteSpace(abelna))
                    BirthCity = cities?.Where(w => w.comdes == abelna).FirstOrDefault();
                else
                    BirthCity = null;
                NotifyPropertyChanged("Cities");
            }
        }
        #endregion

        #region City
        private string? city;
        public string? City
        {
            get => city;
            set
            {
                abeloc = value;
                city = value;
                NotifyPropertyChanged("City");
            }
        }
        #endregion

        #region BirthCity
        private COMUNI? birthCity;
        public COMUNI? BirthCity
        {
            get => birthCity;
            set
            {
                abelna = value?.comdes;
                birthCity = value;
                NotifyPropertyChanged("BirthCity");
            }
        }
        #endregion

        #region LegalCity
        private COMUNI? legalCity;
        public COMUNI? LegalCity
        {
            get => legalCity;
            set
            {
                abelol = value?.comdes;
                legalCity = value;
                NotifyPropertyChanged("LegalCity");
            }
        }
        #endregion

        #region States
        private ObservableCollection<TAB_STATES>? states;
        public ObservableCollection<TAB_STATES>? States
        {
            get => states;
            set
            {
                states = value;
                // state
                if (!string.IsNullOrWhiteSpace(abepro))
                    State = states?.Where(w => w.cappro == abepro).FirstOrDefault();
                else
                    State = null;
                // legal state
                if (!string.IsNullOrWhiteSpace(abeprl))
                    LegalState = states?.Where(w => w.cappro == abeprl).FirstOrDefault();
                else
                    LegalState = null;
                NotifyPropertyChanged("States");
            }
        }
        #endregion

        #region State
        private TAB_STATES? state;
        public TAB_STATES? State
        {
            get => state;
            set
            {
                abepro = value?.cappro;
                state = value;
                NotifyPropertyChanged("State");
            }
        }
        #endregion

        #region LegalState
        private TAB_STATES? legalState;
        public TAB_STATES? LegalState
        {
            get => legalState;
            set
            {
                abeprl = value?.cappro;
                legalState = value;
                NotifyPropertyChanged("LegalState");
            }
        }
        #endregion

        #region Info
        public string AddedText => added.HasValue ? added.Value.ToString() : "---";
        public string AddedUserText => !string.IsNullOrWhiteSpace(addedUserID) ? addedUserID : "---";
        public string UpdatedText => updated.HasValue ? updated.Value.ToString() : "---";
        public string UpdatedUserText => !string.IsNullOrWhiteSpace(updatedUserID) ? updatedUserID : "---";
        public string CanceledText => canceled.HasValue ? canceled.Value.ToString() : "---";
        public string CanceledUserText => !string.IsNullOrWhiteSpace(canceledUserID) ? canceledUserID : "---";
        #endregion


        #region UFP
        public bool IsTop { get; set; }

        public string? abetipo { get; set; }
        public bool abetipoBool
        {
            get
            {
                return abetipo == "S";
            }
            set
            {
                if (value)
                    abetipo = "S";
                else
                    abetipo = "N";
            }
        }

        public string? CLSOSP { get; set; }
        public bool CLSOSPBool
        {
            get
            {
                return CLSOSP == "S";
            }
            set
            {
                if (value)
                    CLSOSP = "S";
                else
                    CLSOSP = "N";
            }

        }

        public string? FOSOSP { get; set; }
        public bool FOSOSPBool
        {
            get
            {
                return FOSOSP == "S";
            }
            set
            {
                if (value)
                    FOSOSP = "S";
                else
                    FOSOSP = "N";
            }
        }

        public bool? IsSister { get; set; }
        #endregion
    }
}
