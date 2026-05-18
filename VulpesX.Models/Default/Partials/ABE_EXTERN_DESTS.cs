using System.Collections.ObjectModel;

namespace VulpesX.Models.Default
{
    public partial class ABE_EXTERN_DESTS
    {
        #region Destination
        private ObservableCollection<DESTINATARI>? destinationsList;
        public ObservableCollection<DESTINATARI>? DestinationsList
        {
            get => destinationsList;
            set
            {
                destinationsList = value;
                if (abedestid > 0)
                    Destination = destinationsList?.Where(w => w.codesti == abedestid).FirstOrDefault();
                else
                    Destination = null;
                NotifyPropertyChanged("DestinationsList");
            }
        }

        private DESTINATARI? destination;
        public DESTINATARI? Destination
        {
            get => destination;
            set
            {
                if (value != null)
                {
                    abedestid = value.codesti;
                    destination = value;
                    NotifyPropertyChanged("Destination");
                }
            }
        }
        #endregion
    }
}