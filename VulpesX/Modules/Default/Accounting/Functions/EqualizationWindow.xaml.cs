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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Functions;

namespace VulpesX.Modules.Default.Accounting.Functions
{
    /// <summary>
    /// Interaction logic for EqualizationWindow.xaml
    /// </summary>
    public partial class EqualizationWindow : FluentDefaultWindow
    {
        private EqualizationWindowViewModel _dataContext;
        public EqualizationWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<EqualizationWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            dtpUntil.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            cmbEntityType.ItemsSource = CommonsService.EntityTypes;
            cmbEntityType.SelectedIndex = 0;
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var type = (cmbEntityType.SelectedItem as GenericIDDescription);

            if (type != null)
            {
                if (dtpUntil.SelectedValue.HasValue)
                {
                    if (ConfirmHandler.Confirm($"Confermate il lancio della funzione di pareggiamento delle partite [{type.Description}] fino al {dtpUntil.SelectedValue.Value.ToString("dd/MM/yyyy")} ?"))
                    {
                        var result = await _dataContext.RunEqualization(type.ID!, dtpUntil.SelectedValue.Value);

                        if (result)
                        {
                            InfoHandler.Show("Pareggiamento partite terminato correttamente");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation("La data fino alla quale pareggiare le partite è obbligatoria");
                }
            }
            else
            {
                ErrorHandler.Validation("Il tipo è obbligatorio");
            }
        }

    }
}
