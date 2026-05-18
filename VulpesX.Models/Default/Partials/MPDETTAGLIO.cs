using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Default
{
    public partial class MPDETTAGLIO
    {
        public bool SelectECVisibility => selectedEntity != null ? true : false;

        #region Entity
        private ObservableCollection<ABE>? entitiesList;
        public ObservableCollection<ABE>? EntitiesList
        {
            get { return entitiesList; }
            set
            {
                entitiesList = value;
                if (M3SOTT.HasValue)
                    SelectedEntity = entitiesList?.Where(w => w.abecod == M3SOTT.Value).FirstOrDefault();
                else
                    SelectedEntity = null;
                NotifyPropertyChanged("EntitiesList");
            }
        }

        public string? EntityDescription { get; set; }

        private ABE? selectedEntity;
        public ABE? SelectedEntity
        {
            get => selectedEntity;
            set
            {
                selectedEntity = value;
                EntityDescription = selectedEntity?.FullDescriptionSearchable;
                NotifyPropertyChanged("SelectedEntity");
                NotifyPropertyChanged("EntityDescription");
                NotifyPropertyChanged("SelectECVisibility");
            }
        }
        public bool IsEntityReadonly { get; set; }
        #endregion
    }
}
