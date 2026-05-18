using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.DWH;

namespace VulpesX.Modules.Default.DWH
{
    /// <summary>
    /// Interaction logic for DWHQueryWindow.xaml
    /// </summary>
    public partial class DWHQueryWindow : FluentDefaultWindow
    {
        private DWHQueryWindowViewModel _dataContext;
        public DWHQueryWindow(DWHQueryWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        #region Various events
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            stkParameters.Children.Clear();

            var parameters = Regex.Matches((sender as TextBox)?.Text ?? string.Empty, @"@\b\S+?\ ");
            var added = new List<string>();

            foreach (var parameter in parameters)
            {
                if (parameter != null)
                {
                    string? parameterString = parameter.ToString();

                    if (!string.IsNullOrEmpty(parameterString))
                    {
                        if (!parameterString.Contains("__") && !parameterString.Contains("FETCH_STATUS"))
                        {
                            if (!added.Contains(parameterString))
                            {
                                var dockPanel = new DockPanel();

                                var parmLabel = new System.Windows.Controls.Label { Content = parameter.ToString(), Width = 150, Foreground = this.FindResource("VulpesXGreenBrush") as SolidColorBrush };
                                DockPanel.SetDock(parmLabel, Dock.Left);

                                var saved = (_dataContext.Data.Parametri ?? new ObservableCollection<DWH_QueryParameter>()).Where(o => o.Nome == parameter.ToString()).FirstOrDefault();
                                int dbType = (saved != null) ? saved.Tipo ?? 0 : 0;

                                var parmCombo = new RadComboBox { ItemsSource = Enum.GetValues(typeof(SqlDbType)), IsEnabled = parameterString.Trim() != "@SocietaID", SelectedValue = parameterString.Trim() != "@SocietaID" ? (SqlDbType)dbType : SqlDbType.NVarChar };
                                DockPanel.SetDock(parmCombo, Dock.Right);

                                dockPanel.Children.Add(parmLabel);
                                dockPanel.Children.Add(parmCombo);

                                stkParameters.Children.Add(dockPanel);

                                added.Add(parameterString);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.IsInsert)
                _dataContext.Data.ID = Guid.NewGuid();


            int posizione = 0;
            _dataContext.Data.Parametri = new ObservableCollection<DWH_QueryParameter>();
            foreach (var dock in stkParameters.Children.OfType<DockPanel>())
            {
                string? parmName = dock.Children.OfType<System.Windows.Controls.Label>().FirstOrDefault()?.Content.ToString();
                string? parmValue = dock.Children.OfType<RadComboBox>().FirstOrDefault()?.SelectedValue.ToString();

                if (!string.IsNullOrEmpty(parmName) && !string.IsNullOrEmpty(parmValue))
                {
                    var parmType = (int)(SqlDbType)Enum.Parse(typeof(SqlDbType), parmValue);

                    _dataContext.Data.Parametri.Add(new DWH_QueryParameter { SocietaID = _dataContext.CompanyID, ID = _dataContext.Data.ID, Nome = parmName, Tipo = parmType, Posizione = ++posizione });
                }
            }

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
        #endregion
    }
}
