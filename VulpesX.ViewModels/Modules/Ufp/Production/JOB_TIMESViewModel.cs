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
using VulpesX.Models.Models.Production;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Ufp.Production
{
    public class JOB_TIMESViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string JobID { get; set; }

        public JOB_TIMESViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }


        private ObservableCollection<ProductionModel.ProductionTimeUfpModel>? _times;
        public ObservableCollection<ProductionModel.ProductionTimeUfpModel>? Times
        {
            get => _times;
            set
            {
                _times = value;

                NotifyPropertyChanged();
            }
        }

        public async Task LoadProductionTimes()
        {
            IsBusy = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    var times = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordine_composizione_tempoRepository>().GetProductionTime(CompanyID, JobID);

                    return new { times };
                });

                Times = result.times;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
