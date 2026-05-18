using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VulpesX.Models;
using VulpesX.Models.Reports.Production;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for ProductionOrderLookWindow.xaml
    /// </summary>
    public partial class ProductionOrderLookWindow : FluentDefaultWindow
    {
        private ProductionOrderLookWindowViewModel _dataContext;
        public ProductionOrderLookWindow(ProductionOrderLookWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Width = System.Windows.SystemParameters.WorkArea.Width;
            this.Height = System.Windows.SystemParameters.WorkArea.Height - 200;

            this.DataContext = _dataContext;

            this.Closing += (s, e) =>
            {
                UpdateNote(_dataContext.Items?.FirstOrDefault());
            };

            LoadDistinta();
        }

        private async void LoadDistinta()
        {
            await _dataContext.LoadDistinta();

            rtvDistinct.ExpandAll();
        }

        private async void LoadTempi()
        {
            await _dataContext.LoadTempi();

        }

        private void UpdateNote(pro_ordine_composizione? Model)
        {
            if (Model != null)
            {
                foreach (var chl in Model.Children.Cast<pro_ordine_composizione>())
                {
                    _dataContext.UpdateNote(chl);

                    UpdateNote(chl);
                }
            }
        }

        private void rtvDistinct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                LoadTempi();
        }

        private void cmbBox_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (sender as Button)?.DataContext as pro_ordine_composizione_tempo;

            if (dataContext != null)
                ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_PRODUCTION, Constants.REPORT_TYPE_PRODUCTION_BOX, _dataContext.CompanyID, new BoxReport { Barcode = $"B_{dataContext.SocietaID}_{dataContext.OrdineID}_{dataContext.Box}" }, $"BOX n.{dataContext.OrdineID}_{dataContext.Box}", $"BOX [{dataContext.SocietaID}] {dataContext.OrdineID}_{dataContext.Box}.pdf", true);
        }
    }
}
