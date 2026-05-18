using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class TAB_AST_LOCATIONS
    {
        public string FullDescriptionSearchable => $"{id} {description}";

        #region City
        private ObservableCollection<COMUNI>? cities;
        public ObservableCollection<COMUNI>? Cities
        {
            get => cities;
            set
            {
                cities = value;
                if (!string.IsNullOrWhiteSpace(city_id))
                    City = cities?.Where(w => w.comdes == city_id).FirstOrDefault();
                else
                    City = null;
                NotifyPropertyChanged("City");
                NotifyPropertyChanged("Cities");
            }
        }

        private COMUNI? city;
        public COMUNI? City
        {
            get => city;
            set
            {
                if (city?.comdes != value?.comdes)
                {
                    city_id = value?.comdes;
                    city = value;
                    NotifyPropertyChanged("City");
                }
            }
        }
        #endregion

        #region State
        private ObservableCollection<TAB_STATES>? states;
        public ObservableCollection<TAB_STATES>? States
        {
            get => states;
            set
            {
                states = value;
                if (!string.IsNullOrWhiteSpace(state_id))
                    State = states?.Where(w => w.cappro == state_id).FirstOrDefault();
                else
                    State = null;
                NotifyPropertyChanged("State");
                NotifyPropertyChanged("States");
            }
        }

        private TAB_STATES? state;
        public TAB_STATES? State
        {
            get => state;
            set
            {
                if (state?.cappro != value?.cappro)
                {
                    state_id = value?.cappro;
                    state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }
        #endregion
    }
}
