using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Telerik.Windows.Documents.Fixed.Model.Objects;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for FATTPERSTXTWindow.xaml
    /// </summary>
    public partial class FATTPERSTXTWindow : FluentDefaultWindow
    {
        private FATTPERSTXTWindowViewModel _dataContext;
        public FATTPERSTXTWindow(FATTPERSTXTWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
            _dataContext.From = DateTime.Now.Date.AddMonths(-1);
            _dataContext.To = DateTime.Now.Date;
            _dataContext.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }


        private void rcbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.Customers = _dataContext.GetCustomersLightListActiveForExternals();
        }

        private void acCustomer_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {

            if (_dataContext.SelectedType != null && _dataContext.SelectedCustomer != null)
            {
                if (_dataContext.From.HasValue && _dataContext.To.HasValue)
                {
                    if (_dataContext.From <= _dataContext.To)
                    {
                        if (Directory.Exists(_dataContext.Path))
                        {
                            _dataContext.Path = _dataContext.Path + $"\\{_dataContext.SelectedCustomer.abecod}_{_dataContext.From.Value.ToString("ddMMyyyy")}_{_dataContext.To.Value.ToString("ddMMyyyy")}.txt";

                            var result = await _dataContext.Generate();
                            if (result)
                            {
                                InfoHandler.Show("File generato correttamente");
                            }
                            else
                            {
                                ErrorHandler.Validation("Impossibile generare il file");

                            }
                        }
                        else
                        {
                            ErrorHandler.Validation("Controllare il percorso inserito");
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("Le date non coincidono");
                    }
                }
                else
                {
                    ErrorHandler.Validation("Le date non coincidono");
                }
            }
            else
            {
                ErrorHandler.Validation("Selezionare Tipo e Cliente");
            }
        }
    }
}
