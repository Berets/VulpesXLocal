using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class VETTORIViewModel : Base
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<VETTORI>? items;
        public ObservableCollection<VETTORI>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public abstract Task Load();
    }

    public class VETTORIViewModelDefault : VETTORIViewModel
    {
        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList());

            IsBusy = false;

        }
    }

    public class VETTORIViewModelUfp : VETTORIViewModel
    {
        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList());

            IsBusy = false;

        }
    }
}
