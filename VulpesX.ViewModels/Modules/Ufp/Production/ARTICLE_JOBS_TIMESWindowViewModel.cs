using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Production;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Ufp.Production
{
    public class ARTICLE_JOBS_TIMESWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string ArticleID { get; set; }

        public ARTICLE_JOBS_TIMESWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public tab_articolo? Article { get; set; }

        private ObservableCollection<pro_ordine>? _orders;
        public ObservableCollection<pro_ordine>? Orders
        {
            get => _orders;
            set
            {
                _orders = value;

                NotifyPropertyChanged();
            }
        }

        public void GetArticle()
        {
            Article = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(ArticleID);
        }

        public async Task LoadOrders()
        {
            IsBusy = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    var orders = VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().GetPROD_ORDINIFromArticleID(CompanyID,ArticleID);

                    return new { orders };
                });

                Orders = result.orders;
            }
            finally
            {
                IsBusy = false;
            }
        }

    }
}
