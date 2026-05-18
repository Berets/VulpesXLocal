using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class DESFOR
    {
        #region Cities
        private ObservableCollection<COMUNI>? cities;
        public ObservableCollection<COMUNI>? Cities
        {
            get => cities;
            set
            {
                cities = value;
                // city
                if (!string.IsNullOrWhiteSpace(fodeloc))
                    City = cities?.Where(w => w.comdes == fodeloc).FirstOrDefault();
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
                fodeloc = value?.comdes;
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
                if (!string.IsNullOrWhiteSpace(fodepro))
                    State = states?.Where(w => w.cappro == fodepro).FirstOrDefault();
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
                fodepro = value?.cappro;
                state = value;
                NotifyPropertyChanged("State");
            }
        }
        #endregion
    }
}
