using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace VulpesX.Shared.Controls.Helpers
{
    public static class BindingHelper
    {
        public static BindingExpression? GetInputElementBindingExpression(FrameworkElement source)
        {
            if (source is RadAutoCompleteBox)
            { return source.GetBindingExpression(RadAutoCompleteBox.SearchTextProperty); }
            else if (source is TextBox)
            { return source.GetBindingExpression(TextBox.TextProperty); }
            else if (source is RadNumericUpDown)
            { return source.GetBindingExpression(RadNumericUpDown.ValueProperty); }
            else if (source is RadComboBox)
            { return source.GetBindingExpression(RadComboBox.SelectedValueProperty); }
            else if (source is RadToggleSwitchButton)
            { return source.GetBindingExpression(RadToggleSwitchButton.IsCheckedProperty); }
            else if (source is RadDateTimePicker)
            { return source.GetBindingExpression(RadDateTimePicker.SelectedValueProperty); }
            else if (source is RadMaskedNumericInput)
            { return source.GetBindingExpression(RadMaskedNumericInput.ValueProperty); }
            return null;
        }
    }

}
