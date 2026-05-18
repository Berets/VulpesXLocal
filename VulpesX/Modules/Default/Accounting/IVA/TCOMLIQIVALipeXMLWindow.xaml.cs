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
using VulpesX.Models.Models.Accounting;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for TCOMLIQIVALipeXMLWindow.xaml
    /// </summary>
    public partial class TCOMLIQIVALipeXMLWindow : FluentDefaultWindow
    {
        private TCOMLIQIVALipeXMLWindowViewModel _dataContext;

        public TCOMLIQIVALipeXMLWindow(TCOMLIQIVALipeXMLWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            var company = _dataContext.GetAZIENDA();

            if (company != null)
            {
                string? senderVAT = !string.IsNullOrWhiteSpace(company.azpaiv) ? company.azpaiv?.Trim() : company.azcofi?.Trim();
                string? senderFISCALID = !string.IsNullOrWhiteSpace(company.azcofi) ? company.azcofi?.Trim() : company.azpaiv?.Trim();

                _dataContext.VATID = senderVAT;
                _dataContext.FISCALID = senderFISCALID;
                _dataContext.FiscalIDSender = company.azcflr;
                _dataContext.Titles = CommonsService.LIPETitles;
                _dataContext.TitleID = _dataContext.Titles.First().ID;
                _dataContext.Presentations = CommonsService.LIPEPresentations;
                _dataContext.PresentationID = _dataContext.Presentations.First().ID;
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            _dataContext.GenerateXML();

            Mouse.OverrideCursor = null;
            this.DialogResult = true;
        }
    }
}
