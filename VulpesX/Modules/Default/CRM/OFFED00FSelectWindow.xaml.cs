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
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for OFFED00FSelectWindow.xaml
    /// </summary>
    public partial class OFFED00FSelectWindow : FluentDefaultWindow
    {
        private OFFED00FSelectWindowViewModel _dataContext;
        public OFFED00FSelectWindow(OFFED00FSelectWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Title = $"Seleziona le righe da trasformare in ordine cliente";
            this.DataContext = _dataContext;

            _dataContext.HasAllSigns = (UserContext.Instance!.ACCESS!.Roles?.canOrders ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignCommercial ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignTech ?? false);
        }

        #region Grid

        #endregion

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate la creazione di un ordine cliente con le righe selezionate ?"))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                // create order
                if (_dataContext.GenerateByOffer(rgvRows.SelectedItems.Cast<OFFED00F>().ToList(), rgvAttachments.SelectedItems.Cast<OFFETAL00F>().ToList()))
                {
                    if (_dataContext.AvailableRows?.Count != rgvRows.SelectedItems.Count)
                    {
                        // not all rows selected
                        if (ConfirmHandler.Confirm("Non sono state selezionate tutte le righe:\n- SI - desidera chiudere come completate le offerte parziali\n- NO - desidera lasciare aperte le offerte parziali"))
                        {
                            // close the offers
                            CloseOffers();
                        }
                    }
                    else
                    {
                        // close the offers
                        CloseOffers();
                    }
                }
            }
            Mouse.OverrideCursor = null;
            this.DialogResult = true;
        }

        private void CloseOffers()
        {
            foreach (var offer in _dataContext.OffersHeads)
            {
                var refreshed = _dataContext.GetFull(offer.OFTANNO, offer.OFTNUOR);

                if (refreshed != null)
                {
                    if (refreshed.Rows?.Any(any => any.transformed.HasValue) ?? false)
                    {
                        offer.oflgchi = "O";
                        offer.updated = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        offer.updatedUserID = _dataContext.UserID;

                        _dataContext.Update(offer);
                    }
                }
            }
        }
        #endregion
    }
}
