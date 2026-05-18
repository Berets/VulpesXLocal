using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;
using VulpesX.Models.Models.Accounting;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PNImportWindow.xaml
    /// </summary>
    public partial class PNImportWindow : FluentDefaultWindow
    {
        private PNImportWindowViewModel _dataContext;
        public PNImportWindow(PNImportWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
        }

        private void btnAllegatoBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            var result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                txtAllegatoPath.Text = filename;
            }
        }

        private async void btnAllegatoUpload_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(txtAllegatoPath.Text))
            {
                if (!FileHelper.IsOpen(txtAllegatoPath.Text))
                {
                    _dataContext.FileName = txtAllegatoPath.Text;

                    await _dataContext.ImportFile();

                    var result = _dataContext.Validate();

                    if (result.Any())
                    {
                        string error = string.Join(Environment.NewLine, result.Select(s => s.Item2));

                        ErrorHandler.Validation(error);
                    }
                    else
                    {
                        var resultSave = await _dataContext.Import();

                        if (!string.IsNullOrEmpty(resultSave))
                        {
                            InfoHandler.Show(resultSave);
                            this.Close();
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation("Attenzione il file risulta aperto");
                }
            }
            else
            {
                ErrorHandler.Validation("Selezionare un file da importare");
            }
        }

        private void btnAllegatoDownload_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Save Excel File As",
                FileName = "Template.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string selectedPath = saveFileDialog.FileName;

                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "VulpesX.Templates.TEMPLATE_PN.xlsx";

                using (Stream? resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (resourceStream == null)
                    {
                        ErrorHandler.Validation("Impossibile trovare il file specificato");
                        return;
                    }

                    using (FileStream fileStream = new FileStream(selectedPath, FileMode.Create, FileAccess.Write))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }

                try
                {
                    Process.Start(new ProcessStartInfo(selectedPath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not open file: " + ex.Message);
                }
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
