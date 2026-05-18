using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ARTCompositionReplaceWindow.xaml
    /// </summary>
    public partial class ARTCompositionReplaceWindow : FluentDefaultWindow
    {
        private ARTCompositionReplaceWindowViewModel _dataContext;
        public ARTCompositionReplaceWindow(ARTCompositionReplaceWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadProducts();

            _dataContext.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedProduct")
                {
                    if (_dataContext.SelectedProduct != null)
                    {
                        _dataContext.Revisions = _dataContext.GetRevisions(_dataContext.SelectedProduct.ID);

                        if (_dataContext.Revisions != null && _dataContext.Revisions.Any())
                            _dataContext.SelectedRevision = dataContext.Revisions?.FirstOrDefault();
                        else
                            LoadDependencies();
                    }
                }
                if (e.PropertyName == "SelectedRevision")
                {
                    if (!string.IsNullOrEmpty(dataContext.SelectedRevision))
                        LoadDependencies();
                }

                if (e.PropertyName == "SelectedProductNew")
                {
                    if (_dataContext.SelectedProductNew != null)
                    {
                        _dataContext.RevisionsNew = _dataContext.GetRevisions(_dataContext.SelectedProductNew.ID);

                        if (_dataContext.RevisionsNew != null && _dataContext.RevisionsNew.Any())
                            dataContext.SelectedRevisionNew = dataContext.RevisionsNew?.FirstOrDefault();
                    }
                }
                if (e.PropertyName == "SelectedRevisionNew")
                {
                }
            };
        }

        private void LoadDependencies()
        {
            using (BackgroundWorker bgwOrders = new BackgroundWorker())
            {
                bgwOrders.DoWork += delegate (object? s, DoWorkEventArgs args)
                {
                    _dataContext.IsBusy = true;
                    if (!string.IsNullOrEmpty(_dataContext.SelectedRevision))
                        _dataContext.Dependencies = _dataContext.GetDependencies(_dataContext.SelectedProduct!.ID, _dataContext.SelectedRevision);
                    else
                        _dataContext.Dependencies = _dataContext.GetDependencies(_dataContext.SelectedProduct!.ID);
                };
                bgwOrders.RunWorkerAsync();
                bgwOrders.RunWorkerCompleted += (s, e) =>
                {
                    _dataContext.IsBusy = false;
                };
            }
        }

        private void CheckRoots()
        {
            var roots = rgvList.SelectedItems.Cast<tab_articolo_composizione>().Where(o => o.Padre == null);

            bool result = true;
            var resultMessage = new StringBuilder();

            using (BackgroundWorker bgwOrders = new BackgroundWorker())
            {
                bgwOrders.DoWork += delegate (object? s, DoWorkEventArgs args)
                {
                    _dataContext.IsBusy = true;

                    foreach (var root in roots)
                    {
                        if (string.IsNullOrEmpty(root.RevisioneNuova))
                        {
                            result = false;
                            resultMessage.AppendLine($"{root.ArticoloID}-{root.RevisioneID} | Revisione mancante");
                        }
                        else
                        {
                            if (_dataContext.ExistRevision(root.ArticoloID, root.RevisioneNuova))
                            {
                                result = false;
                                resultMessage.AppendLine($"{root.ArticoloID}-{root.RevisioneID} | Revisione {root.RevisioneNuova} esistente");
                            }
                        }

                        CheckDependencies(root, ref result, ref resultMessage);
                    }
                };
                bgwOrders.RunWorkerAsync();
                bgwOrders.RunWorkerCompleted += (s, e) =>
                {
                    if (result)
                    {
                        if (rgvList.SelectedItems.Cast<tab_articolo_composizione>().Where(o => o.Padre == null).Any())
                        {
                            if (_dataContext.SelectedProductNew != null)
                            {
                                result = _dataContext.Exchange(rgvList.SelectedItems.Cast<tab_articolo_composizione>().Where(o => o.Padre == null).ToList());

                                if (result)
                                    this.Close();
                                else
                                    ErrorHandler.Validation("Errore imprevisto, contattare GxItalia");
                            }
                            else
                            {
                                ErrorHandler.Validation("Selezionare un nuovo articolo");
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation("Selezionare almeno un elemento");
                        }
                    }
                    else
                    {
                        MessageBox.Show(resultMessage.ToString());
                    }
                    _dataContext.IsBusy = false;
                };
            }
        }

        private void CheckDependencies(tab_articolo_composizione Padre, ref bool Result, ref StringBuilder ResultMessage)
        {
            foreach (var root in Padre.Dipendenze ?? new System.Collections.ObjectModel.ObservableCollection<tab_articolo_composizione>())
            {
                if (string.IsNullOrEmpty(root.RevisioneNuova))
                {
                    Result = false;
                    ResultMessage.AppendLine($"{root.ArticoloID}-{root.RevisioneID} | Revisione mancante");
                }
                else
                {
                    if (_dataContext.ExistRevision(root.ArticoloID, root.RevisioneNuova))
                    {
                        Result = false;
                        ResultMessage.AppendLine($"{root.ArticoloID}-{root.RevisioneID} | Revisione {root.RevisioneNuova} esistente");
                    }
                }

                CheckDependencies(root, ref Result, ref ResultMessage);
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            CheckRoots();
        }

        private void btnSetRevision_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in rgvList.SelectedItems.Cast<tab_articolo_composizione>())
            {
                item.RevisioneNuova = txtRevisioneNuova.Text;
            }
        }

        private void btnSetQuantity_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in rgvList.SelectedItems.Cast<tab_articolo_composizione>())
            {
                item.QuantitaNuova = (decimal)(nuQuantitaNuova.Value ?? 0);
            }
        }
    }
}
