using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.DWH;

namespace VulpesX.Modules.Default.DWH
{
    /// <summary>
    /// Interaction logic for DWHRunWindow.xaml
    /// </summary>
    public partial class DWHRunWindow : FluentDefaultWindow
    {
        private DWHRunWindowViewModel _dataContext;
        public DWHRunWindow(DWHRunWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            LoadParameters();
        }

        private void LoadParameters()
        {
            foreach (var parameter in _dataContext.Parameters)
            {
                parameter.ParameterValue = parameter.Valore;
                var dockPanel = new DockPanel();

                var parmLabel = new System.Windows.Controls.Label { Content = parameter.Nome, Width = 150, Foreground = this.FindResource("VulpesXGreenBrush") as SolidColorBrush };
                DockPanel.SetDock(parmLabel, Dock.Left);

                CultureInfo culture = new System.Globalization.CultureInfo("it-IT");
                culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

                if (parameter.Tipo.HasValue)
                {
                    switch ((SqlDbType)parameter.Tipo)
                    {
                        case SqlDbType.BigInt:
                            var bindBigInt = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmBigInt = new RadNumericUpDown { Width = 200, NumberDecimalDigits = 0, NumberDecimalSeparator = "", NumberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = "" } };
                            BindingOperations.SetBinding(parmBigInt, RadNumericUpDown.ValueProperty, bindBigInt);

                            DockPanel.SetDock(parmBigInt, Dock.Right);
                            dockPanel.Children.Add(parmBigInt);
                            break;
                        case SqlDbType.Binary:
                            break;
                        case SqlDbType.Bit:
                            break;
                        case SqlDbType.Char:
                            var bindChar = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmChar = new TextBox { Width = 200 };
                            BindingOperations.SetBinding(parmChar, TextBox.TextProperty, bindChar);

                            DockPanel.SetDock(parmChar, Dock.Right);
                            dockPanel.Children.Add(parmChar);
                            break;
                        case SqlDbType.DateTime:
                            var bindDateTime = new Binding { Path = new PropertyPath("ParameterDate"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmDateTime = new RadDatePicker { Width = 200, DisplayFormat = DateTimePickerFormat.Short };
                            BindingOperations.SetBinding(parmDateTime, RadDateTimePicker.SelectedDateProperty, bindDateTime);

                            DockPanel.SetDock(parmDateTime, Dock.Right);
                            dockPanel.Children.Add(parmDateTime);
                            break;
                        case SqlDbType.Decimal:
                            var bindDecimal = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmDecimal = new RadNumericUpDown { Width = 200 };
                            BindingOperations.SetBinding(parmDecimal, RadNumericUpDown.ValueProperty, bindDecimal);

                            DockPanel.SetDock(parmDecimal, Dock.Right);
                            dockPanel.Children.Add(parmDecimal);
                            break;
                        case SqlDbType.Float:
                            var bindFloat = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmFloat = new RadNumericUpDown { Width = 200 };
                            BindingOperations.SetBinding(parmFloat, RadNumericUpDown.ValueProperty, bindFloat);

                            DockPanel.SetDock(parmFloat, Dock.Right);
                            dockPanel.Children.Add(parmFloat);
                            break;
                        case SqlDbType.Image:
                            break;
                        case SqlDbType.Int:
                            var bindInt = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmInt = new RadNumericUpDown { Width = 200, NumberDecimalDigits = 0, NumberDecimalSeparator = "", NumberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = "" } };
                            BindingOperations.SetBinding(parmInt, RadNumericUpDown.ValueProperty, bindInt);

                            DockPanel.SetDock(parmInt, Dock.Right);
                            dockPanel.Children.Add(parmInt);
                            break;
                        case SqlDbType.Money:
                            var bindMoney = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmMoney = new RadNumericUpDown { Width = 200 };
                            BindingOperations.SetBinding(parmMoney, RadNumericUpDown.ValueProperty, bindMoney);

                            DockPanel.SetDock(parmMoney, Dock.Right);
                            dockPanel.Children.Add(parmMoney);
                            break;
                        case SqlDbType.NChar:
                            var bindNChar = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmNChar = new TextBox { Width = 200 };
                            BindingOperations.SetBinding(parmNChar, TextBox.TextProperty, bindNChar);

                            DockPanel.SetDock(parmNChar, Dock.Right);
                            dockPanel.Children.Add(parmNChar);
                            break;
                        case SqlDbType.NText:
                            var bindNText = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmNText = new TextBox { Width = 200 };
                            BindingOperations.SetBinding(parmNText, TextBox.TextProperty, bindNText);

                            DockPanel.SetDock(parmNText, Dock.Right);
                            dockPanel.Children.Add(parmNText);
                            break;
                        case SqlDbType.NVarChar:
                            var bindNVarChar = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmNVarChar = new TextBox { Width = 200 };
                            BindingOperations.SetBinding(parmNVarChar, TextBox.TextProperty, bindNVarChar);

                            DockPanel.SetDock(parmNVarChar, Dock.Right);
                            dockPanel.Children.Add(parmNVarChar);
                            break;
                        case SqlDbType.Real:
                            var bindReal = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmReal = new RadNumericUpDown { Width = 200 };
                            BindingOperations.SetBinding(parmReal, RadNumericUpDown.ValueProperty, bindReal);

                            DockPanel.SetDock(parmReal, Dock.Right);
                            dockPanel.Children.Add(parmReal);
                            break;
                        case SqlDbType.UniqueIdentifier:
                            break;
                        case SqlDbType.SmallDateTime:
                            break;
                        case SqlDbType.SmallInt:
                            var bindSmallInt = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmSmallInt = new RadNumericUpDown { Width = 200, NumberDecimalDigits = 0, NumberDecimalSeparator = "", NumberFormatInfo = new NumberFormatInfo { NumberGroupSeparator = "" } };
                            BindingOperations.SetBinding(parmSmallInt, RadNumericUpDown.ValueProperty, bindSmallInt);

                            DockPanel.SetDock(parmSmallInt, Dock.Right);
                            dockPanel.Children.Add(parmSmallInt);
                            break;
                        case SqlDbType.SmallMoney:
                            var bindSmallMoney = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmSmallMoney = new RadNumericUpDown { Width = 200 };
                            BindingOperations.SetBinding(parmSmallMoney, RadNumericUpDown.ValueProperty, bindSmallMoney);

                            DockPanel.SetDock(parmSmallMoney, Dock.Right);
                            dockPanel.Children.Add(parmSmallMoney);
                            break;
                        case SqlDbType.Text:
                            var bindText = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmText = new TextBox { Width = 200 };
                            BindingOperations.SetBinding(parmText, TextBox.TextProperty, bindText);

                            DockPanel.SetDock(parmText, Dock.Right);
                            dockPanel.Children.Add(parmText);
                            break;
                        case SqlDbType.Timestamp:
                            break;
                        case SqlDbType.TinyInt:
                            var bindTinyInt = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmTinyInt = new RadNumericUpDown { Width = 200 };
                            BindingOperations.SetBinding(parmTinyInt, RadNumericUpDown.ValueProperty, bindTinyInt);

                            DockPanel.SetDock(parmTinyInt, Dock.Right);
                            dockPanel.Children.Add(parmTinyInt);
                            break;
                        case SqlDbType.VarBinary:
                            break;
                        case SqlDbType.VarChar:
                            var bindVarChar = new Binding { Path = new PropertyPath("ParameterValue"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmVarChar = new TextBox { Width = 200 };
                            BindingOperations.SetBinding(parmVarChar, TextBox.TextProperty, bindVarChar);

                            DockPanel.SetDock(parmVarChar, Dock.Right);
                            dockPanel.Children.Add(parmVarChar);
                            break;
                        case SqlDbType.Variant:
                            break;
                        case SqlDbType.Xml:
                            break;
                        case SqlDbType.Udt:
                            break;
                        case SqlDbType.Structured:
                            break;
                        case SqlDbType.Date:
                            var bindDate = new Binding { Path = new PropertyPath("ParameterDate"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmDate = new RadDatePicker { Width = 200, DisplayFormat = DateTimePickerFormat.Short };
                            BindingOperations.SetBinding(parmDate, RadDateTimePicker.SelectedValueProperty, bindDate);

                            DockPanel.SetDock(parmDate, Dock.Right);
                            dockPanel.Children.Add(parmDate);
                            break;
                        case SqlDbType.Time:
                            break;
                        case SqlDbType.DateTime2:
                            var bindDateTime2 = new Binding { Path = new PropertyPath("ParameterDate"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmDateTime2 = new RadDatePicker { Width = 200, DisplayFormat = DateTimePickerFormat.Short };
                            BindingOperations.SetBinding(parmDateTime2, RadDateTimePicker.SelectedValueProperty, bindDateTime2);

                            DockPanel.SetDock(parmDateTime2, Dock.Right);
                            dockPanel.Children.Add(parmDateTime2);
                            break;
                        case SqlDbType.DateTimeOffset:
                            var bindDateTimeOffset = new Binding { Path = new PropertyPath("ParameterDate"), Source = parameter, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay };
                            var parmDateTimeOffset = new RadDatePicker { Width = 200, DisplayFormat = DateTimePickerFormat.Short };
                            BindingOperations.SetBinding(parmDateTimeOffset, RadDateTimePicker.SelectedValueProperty, bindDateTimeOffset);

                            DockPanel.SetDock(parmDateTimeOffset, Dock.Right);
                            dockPanel.Children.Add(parmDateTimeOffset);
                            break;
                        default:
                            break;
                    }

                    dockPanel.Children.Add(parmLabel);

                    stkParameters.Children.Add(dockPanel);
                }
            }

        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        #endregion
    }
}
