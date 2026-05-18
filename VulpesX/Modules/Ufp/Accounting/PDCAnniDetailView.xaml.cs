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
    /// Interaction logic for PDCAnniDetailView.xaml
    /// </summary>
    public partial class PDCAnniDetailView : UserControl
    {
        private PDCAnniDetailViewModel _dataContext;
        public event EventHandler? Saved;

        public PDCAnniDetailView(PDCAnniDetailViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
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
    }
}
