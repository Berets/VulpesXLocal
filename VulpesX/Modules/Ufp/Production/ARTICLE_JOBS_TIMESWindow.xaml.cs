using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.Production;

namespace VulpesX.Modules.Ufp.Production
{
    /// <summary>
    /// Interaction logic for JOBS_TIMESWindow.xaml
    /// </summary>
    public partial class ARTICLE_JOBS_TIMESWindow : FluentDefaultWindow
    {
        private ARTICLE_JOBS_TIMESWindowViewModel _dataContext;
        public ARTICLE_JOBS_TIMESWindow(ARTICLE_JOBS_TIMESWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            this.DataContext = _dataContext;

            _dataContext.GetArticle();

            if (_dataContext.Article != null)
            {
                this.Title = $"Tempi commesse - {_dataContext.Article.ID} - {_dataContext.Article.artdise}";
            }
            else
            {
                ErrorHandler.Show($"Impossibile trovare l'articolo - {_dataContext.ArticleID}");
                this.Close();
            }

            this.Loaded += async (s, e) =>
            {
                await _dataContext.LoadOrders();
            };
        }

        private void GridOrders_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (GridOrders.SelectedItem != null)
            {
                var order = GridOrders.SelectedItem as pro_ordine;

                if (order != null && !string.IsNullOrEmpty(order.Commessa))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<JOB_TIMESViewModel>();
                    windowViewModel.JobID = order.Commessa;

                    grdTimes.Children.Clear();
                    grdTimes.Children.Add(new JOB_TIMESView(windowViewModel));
                }
            }
        }
    }
}
