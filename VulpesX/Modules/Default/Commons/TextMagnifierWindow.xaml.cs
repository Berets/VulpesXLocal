using DocumentFormat.OpenXml.Wordprocessing;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;

namespace VulpesX.Modules.Default.Commons
{
    /// <summary>
    /// Interaction logic for TextMagnifierWindow.xaml
    /// </summary>
    public partial class TextMagnifierWindow : FluentDefaultWindow
    {
        private TextMagnifierWindowViewModel _dataContext;
        private string _originalText;
        public TextMagnifierWindow(TextMagnifierWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _originalText = _dataContext.SourceText;
            txtText.Text = _dataContext.SourceText;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if ((_dataContext.MaxSize > 0 && txtText.Text.Trim().Length <= _dataContext.MaxSize) || _dataContext.MaxSize == 0)
            {
                _dataContext.SelectedText = txtText.Text.Trim();
                this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Validation($"Il testo digitato e' troppo lungo, pu' contenere al massimo {txtText.MaxLength} caratteri");
                e.Handled = true;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SelectedText = _originalText;
            this.DialogResult = false;
        }

        private void txtText_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dataContext.CurrentSize = txtText.Text.Trim().Length;
            e.Handled = true;
        }
    }
}
