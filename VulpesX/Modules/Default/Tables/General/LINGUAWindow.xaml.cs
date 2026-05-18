using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Xml.Linq;
using VulpesX.Models.Models;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.General;

namespace VulpesX.Modules.Default.Tables.General
{
    /// <summary>
    /// Interaction logic for LINGUAWindow.xaml
    /// </summary>
    public partial class LINGUAWindow : FluentDefaultWindow
    {
        private LINGUAWindowViewModel _dataContext;
        public LINGUAWindow(LINGUAWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            LoadLanguageReport();
        }

        private void LoadLanguageReport()
        {
            XNamespace sys = "clr-namespace:System;assembly=mscorlib";
            XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";

            var storage = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, "Language_Reports.xaml");

            if (storage != null)
            {
                // Get keys from storage
                using var streamStorage = new MemoryStream(storage);
                XDocument docStorage = XDocument.Load(streamStorage);

                var keysStorage = docStorage
                   .Descendants(sys + "String")
                   .Where(e => e.Attribute(x + "Key") != null)
                   .ToDictionary(
                       e => (string)e.Attribute(x + "Key")!,
                       e => (string)e.Value
                );

                // Get keys from language
                Dictionary<string, string>? keyLanguage = null;

                if (_dataContext.Data.linreport != null)
                {
                    using var streamLanguage = new MemoryStream(_dataContext.Data.linreport);
                    XDocument docLanguage = XDocument.Load(streamLanguage);

                    keyLanguage = docLanguage
                       .Descendants(sys + "String")
                       .Where(e => e.Attribute(x + "Key") != null)
                       .ToDictionary(
                           e => (string)e.Attribute(x + "Key")!,
                           e => (string)e.Value
                    );
                }

                var keys = new ObservableCollection<LanguageModel>();

                foreach (var key in keysStorage)
                {
                    var value = (keyLanguage != null) ? (keyLanguage.Where(o => o.Key == key.Key).Select(s => s.Value).FirstOrDefault() ?? key.Value) : key.Value;

                    keys.Add(new LanguageModel { Key = key.Key, Value = key.Value, ValueTranslated = value });
                }

                _dataContext.LanguageReports = keys;
            }
        }

        private byte[] SaveLanguageReport()
        {
            // Define namespaces
            XNamespace xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XNamespace x = "http://schemas.microsoft.com/winfx/2006/xaml";
            XNamespace sys = "clr-namespace:System;assembly=mscorlib";

            // Create the root element
            var resourceDictionary = new XElement(xmlns + "ResourceDictionary",
                new XAttribute(XNamespace.Xmlns + "x", x.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "sys", sys.NamespaceName)
            );

            // Add <sys:String x:Key="...">...</sys:String> for each entry
            foreach (var kvp in _dataContext.LanguageReports ?? new ObservableCollection<LanguageModel>())
            {
                var element = new XElement(sys + "String",
                    new XAttribute(x + "Key", kvp.Key),
                    kvp.ValueTranslated ?? string.Empty
                );
                resourceDictionary.Add(element);
            }

            // Wrap it in an XDocument and save
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                resourceDictionary
            );

            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                _dataContext.Data.linreport = SaveLanguageReport();

                this.DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }
    }
}
