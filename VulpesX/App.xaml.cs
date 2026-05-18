using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Telerik.Reporting;
using VulpesX.DAL;
using VulpesX.DAL._CustomHandlers;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Modules.Default.Production;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Controls.Utilities;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Assets;
using VulpesX.ViewModels.Modules.Default.Accounting.Functions;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;
using VulpesX.ViewModels.Modules.Default.Accounting.Reports;
using VulpesX.ViewModels.Modules.Default.Assets;
using VulpesX.ViewModels.Modules.Default.Auth;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;
using VulpesX.ViewModels.Modules.Default.DWH;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.ViewModels.Modules.Default.Production;
using VulpesX.ViewModels.Modules.Default.SRM;
using VulpesX.ViewModels.Modules.Default.Stores;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;
using VulpesX.ViewModels.Modules.Default.Tables.Article;
using VulpesX.ViewModels.Modules.Default.Tables.Assets;
using VulpesX.ViewModels.Modules.Default.Tables.CRM;
using VulpesX.ViewModels.Modules.Default.Tables.CustomerRating;
using VulpesX.ViewModels.Modules.Default.Tables.EnergyMonitor;
using VulpesX.ViewModels.Modules.Default.Tables.General;
using VulpesX.ViewModels.Modules.Default.Tables.Production;
using VulpesX.ViewModels.Modules.Default.Tables.Store;
using VulpesX.ViewModels.Modules.Default.Treasury;
using VulpesX.ViewModels.Modules.Ufp.Accounting.Invoicing;
using VulpesX.ViewModels.Modules.Ufp.CRM;
using VulpesX.ViewModels.Modules.Ufp.CRM.AF;
using VulpesX.ViewModels.Modules.Ufp.General;
using VulpesX.ViewModels.Modules.Ufp.Production;
using VulpesX.ViewModels.Modules.Ufp.Tables.Accounting;
using VulpesX.ViewModels.Modules.Ufp.Tables.CRM.AF;
using VulpesX.WindowsFactory.Default.Accounting;
using VulpesX.WindowsFactory.Default.Auth;
using VulpesX.WindowsFactory.Default.General;
using VulpesX.WindowsFactory.Default.Tables.Accounting;
using VM = VulpesX.ViewModels;

namespace VulpesX
{
    public partial class App : Application
    {
        private RadDesktopAlertManager? _rdaManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            var culture = new CultureInfo("it-IT");
            var dtfInfo = new DateTimeFormatInfo
            {
                ShortDatePattern = "dd-MM-yyyy",
                ShortTimePattern = "HH:mm",
                DateSeparator = "/"
            };
            culture.DateTimeFormat = dtfInfo;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            Telerik.Windows.Controls.LocalizationManager.DefaultCulture = new System.Globalization.CultureInfo("it-IT");

            SqlMapper.AddTypeHandler(new SqlServerDateTimeHandler());

            Office2019Palette.LoadPreset(Office2019Palette.ColorVariation.Dark);
            Office2019Palette.Palette.FontSizeS = 14;
            Office2019Palette.Palette.FontSize = 14;
            Office2019Palette.Palette.FontSizeM = 16;
            Office2019Palette.Palette.FontSizeL = 18;

            InputManager.Current.PreProcessInput += (s, e) =>
            {
                if (e.StagingItem.Input is KeyEventArgs keyEvent && keyEvent.Key == Key.F12 && keyEvent.RoutedEvent == Keyboard.KeyDownEvent)
                {
                    Window? activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                    UserControl? activeUserControl = null;

                    var focusedElement = Keyboard.FocusedElement;

                    DependencyObject? current = focusedElement as DependencyObject;
                    while (current != null)
                    {
                        if (current is UserControl userControl)
                        {
                            activeUserControl = userControl;
                        }
                        current = VisualTreeHelper.GetParent(current);
                    }

                    if (activeWindow != null || activeUserControl != null)
                    {
                        string windowName = (activeWindow != null) ? $"{activeWindow.GetType().Namespace}.{activeWindow.GetType().Name}" : "Nessuna finestra attiva";
                        string controlName = (activeUserControl != null) ? $"{activeUserControl.GetType().Namespace}.{activeUserControl.GetType().Name}" : "Nessun user control attivo";

                        InfoHandler.Show($"{windowName}\n{controlName}");
                    }

                    keyEvent.Handled = true;
                }
            };

            _rdaManager = new RadDesktopAlertManager(AlertScreenPosition.BottomRight);

            base.OnStartup(e);

            EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.LoadedEvent, new RoutedEventHandler(OnWindowLoaded));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RegisterServices();

            ErrorHandler.ShowErrorAction = msg =>
            {
                if (_rdaManager != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _rdaManager.ShowAlert(new RadDesktopAlert
                        {
                            Header = "Attenzione",
                            Content = msg,
                            Background = FindResource("VulpesXRedBrush") as System.Windows.Media.SolidColorBrush,
                            CanAutoClose = true,
                            ShowDuration = 3000,
                            Tag = "RadDesktopAlert"
                        }, true);
                    });
                }
            };
            ConfirmHandler.ConfirmAction = msg => MessageBox.Show(msg, "VulpesX", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
            InfoHandler.ShowInfoAction = msg =>
            {
                if (_rdaManager != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _rdaManager.ShowAlert(new RadDesktopAlert
                        {
                            Header = "Info",
                            Width = 600,
                            Content = msg,
                            Background = FindResource("VulpesXBrush") as System.Windows.Media.SolidColorBrush,
                            CanAutoClose = true,
                            ShowDuration = 5000,
                            Tag = "RadDesktopAlert"
                        }, true);
                    });
                }
            };

            ReportingHandler.PrintPDFAction = (domain, module, reportType, companyId, dataSource, documentTitle, filename, openReport, subreports) =>
            {
                return TelerikReportingHelper.PrintPDF(domain, module, reportType, companyId, dataSource, documentTitle, filename, openReport, subreports);
            };
            ReportingHandler.PrintBookPDFAction = (domain, module, reportType, companyId, dataSource, documentTitle, filename, openReport, printwatermark) =>
            {
                return TelerikReportingHelper.PrintBookPDF(domain, module, reportType, companyId, dataSource, documentTitle, filename, openReport, printwatermark);
            };
            ReportingHandler.PrintInvoiceXMLAction = (filename, xmldata, xsltype, output) =>
            {
                return TelerikReportingHelper.PrintInvoiceXML(filename, xmldata, xsltype, output);
            };


            this.StartupUri = new Uri(@"/LoginWindow.xaml", UriKind.Relative);
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Window window)
            {
                bool draggingGridColumns = false;
                if(window.Content is System.Windows.Controls.Grid)
                {
                    draggingGridColumns = (window.Content as System.Windows.Controls.Grid)!.Children.OfType<Telerik.Windows.Controls.DraggedElement>().Any();
                }

                if (!(window.Content.ToString()?.StartsWith("Telerik.Windows.Controls.DesktopAlertWindow") ?? false) && 
                    !(window.Content.ToString()?.StartsWith("Telerik.Windows.DragDrop.DragVisual") ?? false) &&
                    !draggingGridColumns &&
                    !(window.Content.ToString()?.StartsWith("Telerik.Windows.Controls.FieldList.FieldDragVisual") ?? false))
                {
                    var owner = GetEffectiveOwner(window);

                    if (owner != null && owner != window)
                        WindowDimmer.Dim(owner);

                    window.Closed += (s, e) =>
                    {
                        var owner = GetEffectiveOwner(window);

                        if (owner != null && owner != window)
                            WindowDimmer.Undim(owner);
                    };
                }
            }
        }

        private static Window? GetEffectiveOwner(Window window)
        {
            if (window.Owner != null)
                return window.Owner;

            var active = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive && w != window);

            if (active != null)
                return active;

            return Application.Current.MainWindow;
        }

        private void RegisterServices()
        {
            var service = new ServiceCollection();

            service.AddDALServices();

            RegisterViewModels(service);
            RegisterWindowsFactories(service);

            var provider = service.BuildServiceProvider();

            VulpesServiceProvider.Provider = provider;
        }

        private void RegisterViewModels(ServiceCollection service)
        {
            service.AddTransient<VM.LoginWindowViewModel>();
            service.AddTransient<VM.MainWindowViewModel>();

            #region Default
            #region Accounting
            #region Assets
            service.AddTransient<ACC_ASSETS_CARDSHistoryWindowViewModel>();
            service.AddTransient<ACC_ASSETS_CARDSInsertWindowViewModel>();
            service.AddTransient<ACC_ASSETS_CARDSViewModel>();
            service.AddTransient<ACC_ASSETS_CARDSWindowViewModel>();
            service.AddTransient<AssetAccountingWindowViewModel>();
            service.AddTransient<AssetComputeWindowViewModel>();
            service.AddTransient<AssetHistoryWindowViewModel>();
            #endregion

            #region Functions
            service.AddTransient<ClosePeriodWindowViewModel>();
            service.AddTransient<CloseYearWindowViewModel>();
            service.AddTransient<EqualizationWindowViewModel>();
            #endregion

            #region Invoicing
            service.AddTransient<ACC_EINVOICE_HEADSViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_HEADSViewModelUfp>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_HEADSViewModelDefault>();
                }
            });
            service.AddTransient<ACC_EINVOICE_HEADSViewModelDefault>();
            service.AddTransient<ACC_EINVOICE_HEADSViewModelUfp>();

            service.AddTransient<ACC_EINVOICE_HEADSWindowViewModel>();
            service.AddTransient<AskAccountingWindowViewModel>();

            service.AddTransient<AttachmentsWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AttachmentsWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<AttachmentsWindowViewModelDefault>();
                }
            });
            service.AddTransient<AttachmentsWindowViewModelDefault>();
            service.AddTransient<AttachmentsWindowViewModelUfp>();

            service.AddTransient<PairAccountsWindowViewModel>();
            service.AddTransient<SelectPrintTypeWindowViewModel>();
            service.AddTransient<WaitDownloadWindowViewModel>();
            service.AddTransient<WaitImportWindowViewModel>();

            #endregion

            #region IVA
            service.AddTransient<IVABookWindowViewModel>();

            service.AddTransient<IVACloseWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<IVACloseWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<IVACloseWindowViewModelDefault>();
                }
            });
            service.AddTransient<IVACloseWindowViewModelDefault>();
            service.AddTransient<IVACloseWindowViewModelUfp>();

            service.AddTransient<PNIVAViewModel>();
            service.AddTransient<TCOMLIQIVALipeWindowViewModel>();
            service.AddTransient<TCOMLIQIVALipeXMLWindowViewModel>();
            service.AddTransient<TCOMLIQIVAViewModel>();
            service.AddTransient<TCOMLIQIVAWindowViewModel>();
            #endregion

            #region Reports
            service.AddTransient<BalanceSimulationWindowViewModel>();
            service.AddTransient<ExpiresReportWindowViewModel>();
            service.AddTransient<GeneralJournalWindowViewModel>();
            service.AddTransient<MastrinoReportWindowViewModel>();
            service.AddTransient<BatchReportWindowViewModel>();
            #endregion

            service.AddTransient<ACC_PLAFOND_PARMSWindowViewModel>();
            service.AddTransient<ACC_PLAFONDViewModel>();

            service.AddTransient<ACC_PLAFONDWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_PLAFONDWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<ACC_PLAFONDWindowViewModelDefault>();
                }
            });
            service.AddTransient<ACC_PLAFONDWindowViewModelDefault>();
            service.AddTransient<ACC_PLAFONDWindowViewModelUfp>();

            service.AddTransient<AccountingDetailWindowViewModel>();

            service.AddTransient<AskSelfInvoiceWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AskSelfInvoiceWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<AskSelfInvoiceWindowViewModelDefault>();
                }
            });
            service.AddTransient<AskSelfInvoiceWindowViewModelDefault>();
            service.AddTransient<AskSelfInvoiceWindowViewModelUfp>();

            service.AddTransient<AskWalletAccountWindowViewModel>();

            service.AddTransient<ECAccountingRegistrationWindowViewModel>();
            service.AddTransient<ECChangeRefWindowViewModel>();
            service.AddTransient<ECViewModel>();

            service.AddTransient<INTRAWindowViewModel>();

            service.AddTransient<MastrinoViewModel>();
            service.AddTransient<MastrinoWindowViewModel>();

            service.AddTransient<MPTESTATAWindowViewModel>();
            service.AddTransient<MPTESTATAViewModel>();

            service.AddTransient<PDCAnniDetailViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCAnniDetailViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PDCAnniDetailViewModelDefault>();
                }
            });
            service.AddTransient<PDCAnniDetailViewModelDefault>();
            service.AddTransient<PDCAnniDetailViewModelUfp>();

            service.AddTransient<PDCContiDetailViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCContiDetailViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PDCContiDetailViewModelDefault>();
                }
            });
            service.AddTransient<PDCContiDetailViewModelDefault>();
            service.AddTransient<PDCContiDetailViewModelUfp>();

            service.AddTransient<PDCGruppiDetailViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCGruppiDetailViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PDCGruppiDetailViewModelDefault>();
                }
            });
            service.AddTransient<PDCGruppiDetailViewModelDefault>();
            service.AddTransient<PDCGruppiDetailViewModelUfp>();

            service.AddTransient<PDCSottoDetailViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCSottoDetailViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PDCSottoDetailViewModelDefault>();
                }
            });
            service.AddTransient<PDCSottoDetailViewModelDefault>();
            service.AddTransient<PDCSottoDetailViewModelUfp>();

            service.AddTransient<PDCViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PDCViewModelDefault>();
                }
            });
            service.AddTransient<PDCViewModelDefault>();
            service.AddTransient<PDCViewModelUfp>();

            service.AddTransient<PNDuplicateWindowViewModel>();
            service.AddTransient<PNImportWindowViewModel>();
            service.AddTransient<PNRIGHEWindowViewModel>();
            service.AddTransient<PNTESTATAWindowViewModel>();
            service.AddTransient<PNViewModel>();

            service.AddTransient<PortafoglioDistWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PortafoglioDistWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PortafoglioDistWindowViewModelDefault>();
                }
            });
            service.AddTransient<PortafoglioDistWindowViewModelDefault>();
            service.AddTransient<PortafoglioDistWindowViewModelUfp>();

            service.AddTransient<PortafoglioViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PortafoglioViewModelUfp>();
                    default:
                        return sp.GetRequiredService<PortafoglioViewModelDefault>();
                }
            });
            service.AddTransient<PortafoglioViewModelDefault>();
            service.AddTransient<PortafoglioViewModelUfp>();

            service.AddTransient<SelectECWindowViewModel>();
            #endregion

            #region Assets
            service.AddTransient<ASSED00FWindowViewModel>();
            service.AddTransient<ASSET00FViewModel>();
            service.AddTransient<ASSET00FWindowViewModel>();
            service.AddTransient<ASSETAL00FWindowViewModel>();
            service.AddTransient<ASSETCO00FWindowViewModel>();
            #endregion

            #region Auth
            service.AddTransient<CompanyWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CompanyWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<CompanyWindowViewModelDefault>();
                }
            });
            service.AddTransient<CompanyWindowViewModelDefault>();
            service.AddTransient<CompanyWindowViewModelUfp>();

            service.AddTransient<UserWindowViewModel>();
            #endregion

            #region Commons
            service.AddTransient<AccountingYearWindowViewModel>();
            service.AddTransient<CancelReasonWindowViewModel>();
            service.AddTransient<SendMailWindowViewModel>();
            service.AddTransient<SingleDateWindowViewModel>();
            service.AddTransient<YearWindowViewModel>();
            service.AddTransient<TextMagnifierWindowViewModel>();
            #endregion

            #region CRM
            service.AddTransient<AskDDTSendDateWindowViewModel>();
            service.AddTransient<AskInvoiceFinalDateWindowViewModel>();
            service.AddTransient<BOLLD00FMagnifierWindowViewModel>();
            service.AddTransient<BOLLD00FWindowViewModel>();
            service.AddTransient<BOLLT00FViewModel>();
            service.AddTransient<BOLLT00FWindowViewModel>();
            service.AddTransient<FATTAL00FWindowViewModel>();
            service.AddTransient<FATTAUTWindowViewModel>();
            service.AddTransient<FATTD00FMagnifierWindowViewModel>();
            service.AddTransient<FATTD00FWindowViewModel>();
            service.AddTransient<FATTPERSTXTWindowViewModel>();
            service.AddTransient<FATTT00FSentInfoWindowViewModel>();
            service.AddTransient<FATTT00FViewModel>();
            service.AddTransient<FATTT00FWindowViewModel>();
            service.AddTransient<LISCLICopyCustomerWindowViewModel>();
            service.AddTransient<LISCLIViewModel>();
            service.AddTransient<LISCLIWindowViewModel>();
            service.AddTransient<LISGENCopyProductWindowViewModel>();
            service.AddTransient<LISGENViewModel>();
            service.AddTransient<LISGENWindowViewModel>();
            service.AddTransient<OFFED00FMagnifierWindowViewModel>();
            service.AddTransient<OFFED00FSelectWindowViewModel>();
            service.AddTransient<OFFED00FWindowViewModel>();
            service.AddTransient<OFFET00FCloseReasonWindowViewModel>();
            service.AddTransient<OFFET00FViewModel>();
            service.AddTransient<OFFET00FWindowViewModel>();
            service.AddTransient<OFFETAL00FWindowViewModel>();

            service.AddTransient<ORDID00FMagnifierWindowViewModel>();
            service.AddTransient<ORDID00FSelectWindowViewModel>();
            service.AddTransient<ORDID00FWindowViewModel>();
            service.AddTransient<ORDIT00FDDTsWindowViewModel>();
            service.AddTransient<ORDIT00FInvoicesWindowViewModel>();
            service.AddTransient<ORDIT00FViewModel>();
            service.AddTransient<ORDIT00FWindowViewModel>();
            service.AddTransient<ORDITAL00FWindowViewModel>();
            service.AddTransient<ORDITALEWindowViewModel>();
            #endregion

            #region DWH
            service.AddTransient<DWHFolderWindowViewModel>();
            service.AddTransient<DWHQueryViewModel>();
            service.AddTransient<DWHQueryWindowViewModel>();
            service.AddTransient<DWHRunWindowViewModel>();
            service.AddTransient<DWHTemplateWindowViewModel>();
            service.AddTransient<DWHViewModel>();
            #endregion

            #region General
            service.AddTransient<ABEFreeWindowViewModel>();

            service.AddTransient<ABEViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ABEViewModelUfp>();
                    default:
                        return sp.GetRequiredService<ABEViewModelDefault>();
                }
            });
            service.AddTransient<ABEViewModelDefault>();
            service.AddTransient<ABEViewModelUfp>();

            service.AddTransient<ABEWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ABEWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<ABEWindowViewModelDefault>();
                }
            });
            service.AddTransient<ABEWindowViewModelDefault>();
            service.AddTransient<ABEWindowViewModelUfp>();

            service.AddTransient<ARTAttachWindowViewModel>();
            service.AddTransient<ARTCompositionReplaceWindowViewModel>();
            service.AddTransient<ARTCompositionWindowViewModel>();
            service.AddTransient<ARTDependenciesWindowViewModel>();
            service.AddTransient<ARTImageWindowViewModel>();
            service.AddTransient<ARTViewModel>();
            service.AddTransient<ARTWindowViewModel>();

            service.AddTransient<NOTECLI1WindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<NOTECLI1WindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<NOTECLI1WindowViewModelDefault>();
                }
            });
            service.AddTransient<NOTECLI1WindowViewModelDefault>();
            service.AddTransient<NOTECLI1WindowViewModelUfp>();

            service.AddTransient<NOTEFORWindowViewModel>();
            #endregion

            #region Production
            service.AddTransient<GanttSingleViewModel>();
            service.AddTransient<GanttViewModel>();

            service.AddTransient<LotManagerTrackingWindowViewModel>();
            service.AddTransient<LotManagerViewModel>();
            service.AddTransient<LotManagerWindowViewModel>();

            service.AddTransient<ProductionOrderConfirmHalfWindowViewModel>();
            service.AddTransient<ProductionOrderConfirmRawWindowViewModel>();
            service.AddTransient<ProductionOrderConfirmWindowViewModel>();
            service.AddTransient<ProductionOrderLookWindowViewModel>();
            service.AddTransient<ProductionOrderViewModel>();
            service.AddTransient<ProductionOrderWindowViewModel>();
            #endregion

            #region SRM
            service.AddTransient<ACQOrderDetailMagnifierWindowViewModel>();
            service.AddTransient<ACQOrderDetailWindowViewModel>();
            service.AddTransient<ACQOrderHeadWindowViewModel>();
            service.AddTransient<ACQOrderViewModel>();
            service.AddTransient<CostMaterialsViewModel>();
            service.AddTransient<CostMaterialsWindowViewModel>();
            service.AddTransient<GoodsReceiptReceiveWindowViewModel>();
            service.AddTransient<GoodsReceiptWindowViewModel>();
            service.AddTransient<GoodsReceiptViewModel>();
            service.AddTransient<RDAViewModel>();
            service.AddTransient<RDAWindowViewModel>();
            service.AddTransient<SRM_LISFORCopySupplierWindowViewModel>();
            service.AddTransient<SRM_LISFORViewModel>();
            service.AddTransient<SRM_LISFORWindowViewModel>();
            #endregion

            #region Stores
            service.AddTransient<StoreMovementsCheckWindowViewModel>();
            service.AddTransient<StoreMovementsViewModel>();
            service.AddTransient<StoreMovementsWindowViewModel>();
            service.AddTransient<StoreStocksEngageCausalWindowViewModel>();
            service.AddTransient<StoreStocksEngageWindowViewModel>();
            service.AddTransient<StoreStocksReportWindowViewModel>();
            service.AddTransient<StoreStocksViewModel>();
            #endregion

            #region Tables
            #region Accounting
            service.AddTransient<ABICABViewModel>();
            service.AddTransient<ABICABWindowViewModel>();
            service.AddTransient<ACC_ASSETS_CATEGORIESViewModel>();
            service.AddTransient<ACC_ASSETS_CATEGORIESWindowViewModel>();
            service.AddTransient<ACC_ASSETS_RATESViewModel>();
            service.AddTransient<ACC_ASSETS_RATESWindowViewModel>();
            service.AddTransient<ACC_ASSETS_TYPESViewModel>();
            service.AddTransient<ACC_ASSETS_TYPESWindowViewModel>();
            service.AddTransient<ACC_ASSETS_TYPOLOGIESViewModel>();
            service.AddTransient<ACC_ASSETS_TYPOLOGIESWindowViewModel>();
            service.AddTransient<AFFIDABILITAViewModel>();
            service.AddTransient<AFFIDABILITAWindowViewModel>();

            service.AddTransient<AGENTIViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AGENTIViewModelUfp>();
                    default:
                        return sp.GetRequiredService<AGENTIViewModelDefault>();
                }
            });
            service.AddTransient<AGENTIViewModelDefault>();
            service.AddTransient<AGENTIViewModelUfp>();

            service.AddTransient<AGENTIWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AGENTIWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<AGENTIWindowViewModelDefault>();
                }
            });
            service.AddTransient<AGENTIWindowViewModelDefault>();
            service.AddTransient<AGENTIWindowViewModelUfp>();

            service.AddTransient<AliquoteViewModel>();
            service.AddTransient<AliquoteWindowViewModel>();

            service.AddTransient<AREEViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AREEViewModelUfp>();
                    default:
                        return sp.GetRequiredService<AREEViewModelDefault>();
                }
            });
            service.AddTransient<AREEViewModelDefault>();
            service.AddTransient<AREEViewModelUfp>();

            service.AddTransient<AREEWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AREEWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<AREEWindowViewModelDefault>();
                }
            });
            service.AddTransient<AREEWindowViewModelDefault>();
            service.AddTransient<AREEWindowViewModelUfp>();

            service.AddTransient<BANAZIENViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<BANAZIENViewModelUfp>();
                    default:
                        return sp.GetRequiredService<BANAZIENViewModelDefault>();
                }
            });
            service.AddTransient<BANAZIENViewModelDefault>();
            service.AddTransient<BANAZIENViewModelUfp>();

            service.AddTransient<BANAZIENWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<BANAZIENWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<BANAZIENWindowViewModelDeafult>();
                }
            });
            service.AddTransient<BANAZIENWindowViewModelDeafult>();
            service.AddTransient<BANAZIENWindowViewModelUfp>();

            service.AddTransient<CAMBIViewModel>();
            service.AddTransient<CAMBIWindowViewModel>();
            service.AddTransient<CATEGORIAViewModel>();
            service.AddTransient<CATEGORIAWindowViewModel>();
            service.AddTransient<CAUCONTViewModel>();

            service.AddTransient<CAUCONTWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CAUCONTWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<CAUCONTWindowViewModelDefault>();
                }
            });
            service.AddTransient<CAUCONTWindowViewModelDefault>();
            service.AddTransient<CAUCONTWindowViewModelUfp>();

            service.AddTransient<CLAZIONEViewModel>();
            service.AddTransient<CLAZIONEWindowViewModel>();
            service.AddTransient<COMUNIViewModel>();
            service.AddTransient<COMUNIWindowViewModel>();
            service.AddTransient<CONSEGNAViewModel>();
            service.AddTransient<CONSEGNAWindowViewModel>();
            service.AddTransient<DEPOSITIViewModel>();
            service.AddTransient<DEPOSITIWindowViewModel>();
            service.AddTransient<ESERCIZIOViewModel>();
            service.AddTransient<ESERCIZIOWindowViewModel>();

            service.AddTransient<FE_IVADOCViewModel>();
            service.AddTransient<FE_IVADOCWindowViewModel>();
            service.AddTransient<FE_PAGDOCViewModel>();
            service.AddTransient<FE_PAGDOCWindowViewModel>();
            service.AddTransient<FE_RFIDOCViewModel>();
            service.AddTransient<FE_RFIDOCWindowViewModel>();
            service.AddTransient<FE_TIPOCPViewModel>();
            service.AddTransient<FE_TIPOCPWindowViewModel>();
            service.AddTransient<FE_TIPODOCViewModel>();
            service.AddTransient<FE_TIPODOCWindowViewModel>();
            service.AddTransient<FE_TIPORITViewModel>();
            service.AddTransient<FE_TIPORITWindowViewModel>();

            service.AddTransient<FILIALIViewModel>();
            service.AddTransient<FILIALIWindowViewModel>();
            service.AddTransient<IMBALLIViewModel>();
            service.AddTransient<IMBALLIWindowViewModel>();
            service.AddTransient<ISOViewModel>();
            service.AddTransient<ISOWindowViewModel>();

            service.AddTransient<LIBRIIVAViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<LIBRIIVAViewModelUfp>();
                    default:
                        return sp.GetRequiredService<LIBRIIVAViewModelDefault>();
                }
            });
            service.AddTransient<LIBRIIVAViewModelDefault>();
            service.AddTransient<LIBRIIVAViewModelUfp>();

            service.AddTransient<LIBRIIVAWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<LIBRIIVAWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<LIBRIIVAWindowViewModelDefault>();
                }
            });
            service.AddTransient<LIBRIIVAWindowViewModelDefault>();
            service.AddTransient<LIBRIIVAWindowViewModelUfp>();

            service.AddTransient<MERCEOLOGICOViewModel>();
            service.AddTransient<MERCEOLOGICOWindowViewModel>();
            service.AddTransient<NAZIONIViewModel>();
            service.AddTransient<NAZIONIWindowViewModel>();
            service.AddTransient<NUMREGViewModel>();
            service.AddTransient<NUMREGWindowViewModel>();
            service.AddTransient<PAGCLIViewModel>();
            service.AddTransient<PAGCLIWindowViewModel>();
            service.AddTransient<PAGFORViewModel>();
            service.AddTransient<PAGFORWindowViewModel>();
            service.AddTransient<REGIONIViewModel>();
            service.AddTransient<REGIONIWindowViewModel>();
            service.AddTransient<RITENUTEViewModel>();
            service.AddTransient<RITENUTEWindowViewModel>();
            service.AddTransient<RIVENDITORIViewModel>();
            service.AddTransient<RIVENDITORIWindowViewModel>();
            service.AddTransient<SCADENZEViewModel>();
            service.AddTransient<SCADENZEWindowViewModel>();
            service.AddTransient<SOLLECITIViewModel>();
            service.AddTransient<SOLLECITIWindowViewModel>();
            service.AddTransient<SPEDIZIONEViewModel>();
            service.AddTransient<SPEDIZIONEWindowViewModel>();
            service.AddTransient<TAB_ACC_CLOSINGViewModel>();
            service.AddTransient<TAB_ACC_CLOSINGWindowViewModel>();

            service.AddTransient<TAB_ACC_TIPINCViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_ACC_TIPINCViewModelUfp>();
                    default:
                        return sp.GetRequiredService<TAB_ACC_TIPINCViewModelDefault>();
                }
            });
            service.AddTransient<TAB_ACC_TIPINCViewModelDefault>();
            service.AddTransient<TAB_ACC_TIPINCViewModelUfp>();

            service.AddTransient<TAB_ACC_TIPINCWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_ACC_TIPINCWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<TAB_ACC_TIPINCWindowViewModelDefault>();
                }
            });
            service.AddTransient<TAB_ACC_TIPINCWindowViewModelDefault>();
            service.AddTransient<TAB_ACC_TIPINCWindowViewModelUfp>();

            service.AddTransient<TAB_ACC_TIPPAGViewModel>();
            service.AddTransient<TAB_ACC_TIPPAGWindowViewModel>();
            service.AddTransient<TAB_STATESViewModel>();
            service.AddTransient<TAB_STATESWindowViewModel>();
            service.AddTransient<TCECO00FViewModel>();
            service.AddTransient<TCECO00FWindowViewModel>();

            service.AddTransient<TCODLIQIVAViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TCODLIQIVAViewModelUfp>();
                    default:
                        return sp.GetRequiredService<TCODLIQIVAViewModelDefault>();
                }
            });
            service.AddTransient<TCODLIQIVAViewModelDefault>();
            service.AddTransient<TCODLIQIVAViewModelUfp>();

            service.AddTransient<TCODLIQIVAWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TCODLIQIVAWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<TCODLIQIVAWindowViewModelDefault>();
                }
            });
            service.AddTransient<TCODLIQIVAWindowViewModelDefault>();
            service.AddTransient<TCODLIQIVAWindowViewModelUfp>();

            service.AddTransient<VALUTEViewModel>();
            service.AddTransient<VALUTEWindowViewModel>();

            service.AddTransient<VETTORIViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<VETTORIViewModelUfp>();
                    default:
                        return sp.GetRequiredService<VETTORIViewModelDefault>();
                }
            });
            service.AddTransient<VETTORIViewModelDefault>();
            service.AddTransient<VETTORIViewModelUfp>();

            service.AddTransient<VETTORIWindowViewModel>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<VETTORIWindowViewModelUfp>();
                    default:
                        return sp.GetRequiredService<VETTORIWindowViewModelDefault>();
                }
            });
            service.AddTransient<VETTORIWindowViewModelDefault>();
            service.AddTransient<VETTORIWindowViewModelUfp>();

            service.AddTransient<ZONEViewModel>();
            service.AddTransient<ZONEWindowViewModel>();
            #endregion

            #region Article
            service.AddTransient<CategoriaViewModel>();
            service.AddTransient<CategoriaWindowViewModel>();
            service.AddTransient<TipoViewModel>();
            service.AddTransient<TipoWindowViewModel>();
            service.AddTransient<UnitaViewModel>();
            service.AddTransient<UnitaWindowViewModel>();
            #endregion

            #region Assets
            service.AddTransient<TAB_AST_LOCATIONSViewModel>();
            service.AddTransient<TAB_AST_LOCATIONSWindowViewModel>();
            #endregion

            #region CRM
            service.AddTransient<CAUFAT00FViewModel>();
            service.AddTransient<CAUFAT00FWindowViewModel>();
            service.AddTransient<CAUSBOLLViewModel>();
            service.AddTransient<CAUSBOLLWindowViewModel>();
            service.AddTransient<TAB_CRM_CAUOFFCLOViewModel>();
            service.AddTransient<TAB_CRM_CAUOFFCLOWindowViewModel>();
            service.AddTransient<TAB_CRM_CAUOFFViewModel>();
            service.AddTransient<TAB_CRM_CAUOFFWindowViewModel>();
            service.AddTransient<TAB_CRM_CAUORDViewModel>();
            service.AddTransient<TAB_CRM_CAUORDWindowViewModel>();
            service.AddTransient<TAB_CRM_FEASABILITYViewModel>();
            service.AddTransient<TAB_CRM_FEASABILITYWindowViewModel>();
            #endregion

            #region CustomerRating
            service.AddTransient<ElementViewModel>();
            service.AddTransient<ElementWindowViewModel>();
            service.AddTransient<PointViewModel>();
            service.AddTransient<PointWindowViewModel>();
            service.AddTransient<RatingViewModel>();
            service.AddTransient<RatingWindowViewModel>();
            #endregion

            #region EnergyMonitor
            service.AddTransient<DeviceViewModel>();
            service.AddTransient<DeviceWindowViewModel>();
            #endregion

            #region General
            service.AddTransient<LINGUAViewModel>();
            service.AddTransient<LINGUAWindowViewModel>();
            service.AddTransient<TAB_GEN_ACTIVITY_TYPESViewModel>();
            service.AddTransient<TAB_GEN_ACTIVITY_TYPESWindowViewModel>();
            service.AddTransient<TAB_GEN_CONTACTS_ROLESViewModel>();
            service.AddTransient<TAB_GEN_CONTACTS_ROLESWindowViewModel>();
            service.AddTransient<TAB_GEN_CONTACTS_TYPESViewModel>();
            service.AddTransient<TAB_GEN_CONTACTS_TYPESWindowViewModel>();
            service.AddTransient<TAB_GEN_TEXTSViewModel>();
            service.AddTransient<TAB_GEN_TEXTSWindowViewModel>();
            #endregion

            #region Production
            service.AddTransient<CalendarioViewModel>();
            service.AddTransient<CalendarioWindowViewModel>();
            service.AddTransient<CausaliViewModel>();
            service.AddTransient<CausaliWindowViewModel>();
            service.AddTransient<OperatoriViewModel>();
            service.AddTransient<OperatoriWindowViewModel>();
            service.AddTransient<RepartiViewModel>();
            service.AddTransient<RepartiWindowViewModel>();
            service.AddTransient<RisorseViewModel>();
            service.AddTransient<RisorseWindowViewModel>();
            #endregion

            #region Store
            service.AddTransient<STORE_CAUSALSViewModel>();
            service.AddTransient<STORE_CAUSALSWindowViewModel>();
            service.AddTransient<STORE_STORESViewModel>();
            service.AddTransient<STORE_STORESWindowViewModel>();
            #endregion
            #endregion

            #region Treasure
            service.AddTransient<AskAccountingCommitmentWindowViewModel>();
            service.AddTransient<BankCastellettoWindowViewModel>();
            service.AddTransient<BankFluxesDetailsWindowViewModel>();
            service.AddTransient<BankFluxesWindowViewModel>();
            service.AddTransient<CommitmentWindowViewModel>();
            service.AddTransient<CommitmentViewModel>();
            service.AddTransient<MovementsViewModel>();

            #endregion
            #endregion

            #region Ufp
            #region Accounting
            #region Invoicing
            service.AddTransient<CheckInvoiceEntranceWindowViewModel>();
            #endregion
            #endregion

            #region CRM
            #region AF
            service.AddTransient<ANAFAT_ROWViewModel>();
            service.AddTransient<ANAFAT_ROWWindowViewModel>();
            #endregion
        
            #endregion

            #region General
            service.AddTransient<ARTSelectViewModel>();
            #endregion

            #region Production
            service.AddTransient<ARTICLE_JOBS_TIMESWindowViewModel>();
            service.AddTransient<JOB_TIMESViewModel>();
            #endregion

            #region Tables
            #region Accounting
            service.AddTransient<COMTIPREGViewModel>();
            service.AddTransient<COMTIPREGWindowViewModel>();

            service.AddTransient<INCASSOViewModel>();
            service.AddTransient<INCASSOWindowViewModel>();

            service.AddTransient<MANDATOViewModel>();
            service.AddTransient<MANDATOWindowViewModel>();
            #endregion

            #region AF
            service.AddTransient<ANAFAT_CONSTViewModel>();
            service.AddTransient<ANAFAT_CONSTWindowViewModel>();
            #endregion
            #endregion
            #endregion
        }

        private void RegisterWindowsFactories(ServiceCollection service)
        {
            service.AddTransient<IACC_PLAFONDWindowFactory, IVACloseWindowFactory>();
            service.AddTransient<IABEWindowFactory, ABEWindowFactory>();
            service.AddTransient<ICAUCONTWindowFactory, CAUCONTWindowFactory>();

            service.AddTransient<ICompanyWindowFactory, CompanyWindowFactory>();
        }
    }
}
