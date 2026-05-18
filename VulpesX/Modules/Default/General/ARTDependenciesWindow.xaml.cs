using DocumentFormat.OpenXml.Bibliography;
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
using VulpesX.DAL.General;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ARTDependenciesWindow.xaml
    /// </summary>
    public partial class ARTDependenciesWindow : FluentDefaultWindow
    {
        private ARTDependenciesWindowViewModel _dataContext;

        private static Stack<GenericIDDescription> backHistory = new Stack<GenericIDDescription>();
        private static Stack<GenericIDDescription> forwardHistory = new Stack<GenericIDDescription>();

        private static GenericIDDescription? First = null;

        public ARTDependenciesWindow(ARTDependenciesWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            First = new GenericIDDescription() { ID = _dataContext.ProductID, Description = _dataContext.ProductDescription };

            backHistory.Clear();
            forwardHistory.Clear();
            CheckButtonsBackForward();

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        private void CheckButtonsBackForward()
        {
            cmdBack.IsEnabled = backHistory.Count > 0;
            cmdForward.IsEnabled = forwardHistory.Count > 0;
        }

        private void cmdComposition_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as tab_articolo_composizione;

            if (item != null)
            {
                var article = _dataContext.GetArticle(item.ArticoloID);

                if (article != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTCompositionWindowViewModel>();
                    windowViewModel.Data = article;

                    var wArticoloComposizione = new ARTCompositionWindow(windowViewModel);
                    wArticoloComposizione.Owner = Window.GetWindow(this);
                    wArticoloComposizione.ShowDialog();
                }
            }
        }

        private void cmdDrill_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as tab_articolo_composizione;

            backHistory.Push(new GenericIDDescription() { ID = _dataContext.ProductID, Description = _dataContext.ProductDescription });

            if (item != null)
            {
                _dataContext.ProductID = item.ArticoloID;
                _dataContext.ProductDescription = item.Descrizione ?? string.Empty;

                LoadData();
                CheckButtonsBackForward();
            }
        }

        private void cmdBack_Click(object sender, RoutedEventArgs e)
        {
            forwardHistory.Push(new GenericIDDescription() { ID = _dataContext.ProductID, Description = _dataContext.ProductDescription });

            var item = backHistory.Pop();

            if (item != null)
            {
                _dataContext.ProductID = item.ID!;
                _dataContext.ProductDescription = item.Description!;

                LoadData();
                CheckButtonsBackForward();
            }
        }

        private void cmdForward_Click(object sender, RoutedEventArgs e)
        {
            backHistory.Push(new GenericIDDescription() { ID = _dataContext.ProductID, Description = _dataContext.ProductDescription });

            var item = forwardHistory.Pop();

            if (item != null)
            {
                _dataContext.ProductID = item.ID!;
                _dataContext.ProductDescription = item.Description!;

                LoadData();
                CheckButtonsBackForward();
            }
        }

        private void cmdEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as tab_articolo_composizione;

            if (item != null)
            {
                var article = _dataContext.GetArticle(item.ArticoloID);

                if (article != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTWindowViewModel>();
                    windowViewModel.Data = article;
                    windowViewModel.IsInsert = false;

                    var wArticolo = new ARTWindow(windowViewModel);
                    wArticolo.Owner = Window.GetWindow(this);
                    if (wArticolo.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdFirst_Click(object sender, RoutedEventArgs e)
        {
            if (First != null)
            {
                _dataContext.ProductID = First.ID!;
                _dataContext.ProductDescription = First.Description!;

                LoadData();

                backHistory.Clear();
                forwardHistory.Clear();
                CheckButtonsBackForward();
            }
        }
    }
}
