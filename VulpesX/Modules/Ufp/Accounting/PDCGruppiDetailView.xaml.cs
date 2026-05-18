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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Ufp.Accounting
{
    /// <summary>
    /// Interaction logic for PDCGruppiDetailView.xaml
    /// </summary>
    public partial class PDCGruppiDetailView : UserControl
    {
        private PDCGruppiDetailViewModel _dataContext;
        public event EventHandler? Saved;

        public PDCGruppiDetailView(PDCGruppiDetailViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
                acClosing.SelectedItem = _dataContext.Closings?.Where(w => w.cchcod == _dataContext.Data!.p1chco).FirstOrDefault();
        }

        private void acClosing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var closing = (e.AddedItems[0] as TAB_ACC_CLOSING);

                if (closing != null)
                {
                    _dataContext.Data!.p1chco = closing.cchcod;
                }
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate il salvataggio dei dati ?"))
            {
                var validated = _dataContext.Validate();

                if (string.IsNullOrWhiteSpace(validated))
                {
                    _dataContext.Save();

                    if (Saved != null)
                        Saved(this, EventArgs.Empty);
                }
                else
                {
                    ErrorHandler.Show(validated);
                }
            }
        }

        private void acClosing_LostFocus(object sender, RoutedEventArgs e)
        {
            if (acClosing.SelectedItem == null)
            {
                acClosing.SearchText = null;
            }
        }
    }
}
