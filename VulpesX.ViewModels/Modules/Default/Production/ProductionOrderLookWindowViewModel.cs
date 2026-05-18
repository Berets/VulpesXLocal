using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Production;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class ProductionOrderLookWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ProductionOrderLookWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required pro_ordine Data { get; set; }


        private ObservableCollection<pro_ordine_composizione>? _items;
        public ObservableCollection<pro_ordine_composizione>? Items
        {
            get { return _items; }
            set
            {
                _items = value;

                NotifyPropertyChanged("Items");
            }
        }

        private ObservableCollection<pro_ordine_composizione_tempo>? _times;
        public ObservableCollection<pro_ordine_composizione_tempo>? Times
        {
            get { return _times; }
            set
            {
                _times = value;

                NotifyPropertyChanged("Times");
            }
        }


        private pro_ordine_composizione? _SelectedDistinta;
        public pro_ordine_composizione? SelectedDistinta
        {
            get { return _SelectedDistinta; }
            set
            {
                _SelectedDistinta = value;

                NotifyPropertyChanged("SelectedDistinta");
            }
        }

        private bool _isBusyDistinta;
        public bool IsBusyDistinta
        {
            get { return _isBusyDistinta; }
            set { _isBusyDistinta = value; NotifyPropertyChanged(); }
        }

        private bool _isBusyTempi;
        public bool IsBusyTempi
        {
            get { return _isBusyTempi; }
            set { _isBusyTempi = value; NotifyPropertyChanged(); }
        }

        public async Task LoadDistinta()
        {
            IsBusyDistinta = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().Get(CompanyID, Data.ID, Data.ArticoloID ?? string.Empty, Data.RevisioneID ?? string.Empty, null);
                });

                Items = result;
            }
            finally
            {
                IsBusyDistinta = false;
            }
        }

        public async Task LoadTempi()
        {
            IsBusyTempi = true;

            try
            {
                if (SelectedDistinta != null)
                {
                    var result = await Task.Run(() =>
                    {
                        return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizione_tempoRepository>().Get(SelectedDistinta.SocietaID, SelectedDistinta.OrdineID, SelectedDistinta.ArticoloID, SelectedDistinta.RevisioneID, SelectedDistinta.ComposizioneID);
                    });

                    Times = result;
                }
            }
            finally
            {
                IsBusyTempi = false;
            }
        }

        public bool UpdateNote(pro_ordine_composizione Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizioneRepository>().UpdateNote(Item);
        }
    }
}
