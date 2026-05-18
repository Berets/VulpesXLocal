using System.Collections.ObjectModel;
 

namespace VulpesX.Models.Default
{
    public partial class DESTINATARI
    {
        public string FullDescriptionSearchable => $"{codesti} {ragisoc?.Trim()}";
        public string FullRecipientText => $"{ragisoc} {DEINDI},{deloc} ({depro})";
        public string DECAPText => DECAP.HasValue ? $"{DECAP.Value.ToString().PadLeft(5, '0')}" : "00000";

        #region Isos
        private ObservableCollection<ISO>? isos;
        public ObservableCollection<ISO>? Isos
        {
            get => isos;
            set
            {
                isos = value;
                // city
          
                    Iso = isos?.Where(w => w.isocod== isocod).FirstOrDefault();
          
                NotifyPropertyChanged("Isos");
            }
        }
        #endregion

        #region Iso
        private ISO? iso;
        public ISO? Iso
        {
            get => iso;
            set
            {
                isocod = value?.isocod;
                iso = value;
                NotifyPropertyChanged("Iso");
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
                if (!string.IsNullOrWhiteSpace(deloc))
                    City = cities?.Where(w => w.comdes == deloc).FirstOrDefault();
                else
                    City = null;
                NotifyPropertyChanged("Cities");
            }
        }
        #endregion

        #region City
        private COMUNI? city;
        public COMUNI? City
        {
            get => city;
            set
            {
                city = value;
                NotifyPropertyChanged("City");
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
                if (!string.IsNullOrWhiteSpace(depro))
                    State = states?.Where(w => w.cappro == depro).FirstOrDefault();
                else
                    State = null;
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
                depro = value?.cappro;
                state = value;
                NotifyPropertyChanged("State");
            }
        }
        #endregion


        private AGENTI? firstAgent;
        public AGENTI? FirstAgent
        {
            get => firstAgent;
            set
            {
                if (firstAgent?.agecod != value?.agecod)
                {
                    if (value != null)
                    {
                        decoag1 = value.agecod;
                        deage1p = firstAgent != null || !deage1p.HasValue ? value?.agepvg : deage1p;
                        deage1pt = firstAgent != null || string.IsNullOrWhiteSpace(deage1pt) ? value?.agepvgt : deage1pt;
                    }
                    else
                    {
                        decoag1 = null;
                        deage1p = null;
                        deage1pt = null;
                    }
                    firstAgent = value;
                    NotifyPropertyChanged("FirstAgent");
                    NotifyPropertyChanged("deage1p");
                    NotifyPropertyChanged("deage1pt");
                }
            }
        }

        private AGENTI? secondAgent;
        public AGENTI? SecondAgent
        {
            get => secondAgent;
            set
            {
                if (secondAgent?.agecod != value?.agecod)
                {
                    if (value != null)
                    {
                        decoag2 = value.agecod;
                        deage2p = secondAgent != null || !deage2p.HasValue ? value?.agepvg : deage2p;
                        deage2pt = secondAgent != null || string.IsNullOrWhiteSpace(deage2pt) ? value?.agepvgt : deage2pt;
                    }
                    else
                    {
                        decoag2 = null;
                        deage2p = null;
                        deage2pt = null;
                    }
                    secondAgent = value;
                    NotifyPropertyChanged("SecondAgent");
                    NotifyPropertyChanged("deage2p");
                    NotifyPropertyChanged("deage2pt");
                }
            }
        }

        private ObservableCollection<AGENTI>? agentsList;
        public ObservableCollection<AGENTI>? AgentsList
        {
            get => agentsList;
            set
            {
                agentsList = value;
                if (!string.IsNullOrWhiteSpace(decoag1))
                    FirstAgent = agentsList?.Where(w => w.agecod == decoag1).FirstOrDefault();
                else
                    FirstAgent = null;
                if (!string.IsNullOrWhiteSpace(decoag2))
                    SecondAgent = agentsList?.Where(w => w.agecod == decoag2).FirstOrDefault();
                else
                    SecondAgent = null;
                NotifyPropertyChanged("FirstAgent");
                NotifyPropertyChanged("SecondAgent");
            }
        }

        public ObservableCollection<GenericIDDescription>? CommissionTypes => CommonsService.StandardValueTypes;

        public ObservableCollection<GenericIDDescription>? RecipientTypes => CommonsService.RecipientTypes;

        public string? depro1 { get; set; }
        public string? depro2 { get; set; }
        public string? climaga { get; set; }
        public string? clicaumag { get; set; }
        public string? depri { get; set; }

        private store_stores? store;
        public store_stores? Store
        {
            get => store;
            set
            {
                store = value;
                if (value != null)
                {
                    climaga = value.id;
                }
                else
                {
                    climaga = null;
                }

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<store_stores>? stores;
        public ObservableCollection<store_stores>? Stores
        {
            get => stores;
            set
            {
                stores = value;

                if (!string.IsNullOrWhiteSpace(climaga))
                    Store = stores?.Where(w => w.id == climaga).FirstOrDefault();
                else
                    Store = null;

                NotifyPropertyChanged();
                NotifyPropertyChanged("Store");
            }
        }

        private store_causals? storeCausal;
        public store_causals? StoreCausal
        {
            get => storeCausal;
            set
            {
                storeCausal = value;

                if (value != null)
                {
                    clicaumag = value.id;
                }
                else
                {
                    clicaumag = null;
                }

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<store_causals>? storeCausals;
        public ObservableCollection<store_causals>? StoreCausals
        {
            get => storeCausals;
            set
            {
                storeCausals = value;

                if (!string.IsNullOrWhiteSpace(clicaumag))
                    StoreCausal = storeCausals?.Where(w => w.id == clicaumag).FirstOrDefault();
                else
                    StoreCausal = null;

                NotifyPropertyChanged();
                NotifyPropertyChanged("StoreCausal");
            }
        }
    }
}
