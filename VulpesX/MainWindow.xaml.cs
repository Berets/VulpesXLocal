using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Documents.Spreadsheet.Model;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Modules.Default.Auth;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.WindowsFactory.Default.Auth;
using VulpesX.WindowsFactory.Default.General;

namespace VulpesX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _dataContext;
        public ObservableCollection<RadPane> _panes = new ObservableCollection<RadPane>();
        private ObservableCollection<MenuModel> _menu = new ObservableCollection<MenuModel>();

        public MainWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<MainWindowViewModel>();
            _dataContext.UserContext = UserContext.Instance;
            _dataContext.UserContext.ACCESS!.SelectedCompany = _dataContext.UserContext.ACCESS!.EnabledCompanies!.Where(o => o.SOMCOD == _dataContext.UserContext.ACCESS!.USRPRECOM).FirstOrDefault() ?? _dataContext.UserContext.ACCESS!.EnabledCompanies!.FirstOrDefault();
            
            if (_dataContext.UserContext.ACCESS!.SelectedCompany != null)
            {
                _dataContext.SetRoles(_dataContext.UserContext.ACCESS!.SelectedCompany.SOMCOD);
                
                _dataContext.GetBookmarks();
                _dataContext.GetMenu(_dataContext.UserContext.ACCESS!.SelectedCompany.SOMCOD);

                _menu = _dataContext.MenuItems;
            }

            InitializeComponent();

            this.DataContext = _dataContext;

            //TEMA
            string theme = Constants.THEME_DARK;
            if (!string.IsNullOrEmpty(UserContext.Instance.ACCESS!.USRPRETEMA))
                theme = UserContext.Instance.ACCESS!.USRPRETEMA;

            ChangeTheme(theme);
        }

        #region TitleBar
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                SystemCommands.MinimizeWindow(window);
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;

            if (window.WindowState == WindowState.Maximized)
                SystemCommands.RestoreWindow(window);
            else
                SystemCommands.MaximizeWindow(window);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                SystemCommands.CloseWindow(window);
        }

        private void btnTheme_Click(object sender, RoutedEventArgs e)
        {
            ChangeTheme(string.Empty);
        }

        private void btnUserSetting_Click(object sender, RoutedEventArgs e)
        {
            var window = new UserWindow();
            window.ShowDialog();
        }

        private void btnCompanySettings_Click(object sender, RoutedEventArgs e)
        {
            var window = VulpesServiceProvider.Provider.GetRequiredService<ICompanyWindowFactory>().Create();
            window.ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Premere [SI] per chiudere la sessione corrente"))
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                App.Current.MainWindow = loginWindow;

                this.Close();
            }
        }
        #endregion

        #region Theme
        public void ChangeTheme(string ThemeID)
        {
            var light = new Uri("/VulpesX.Shared.Controls;component/Themes/ThemeLight.xaml", UriKind.Relative);
            var dark = new Uri("/VulpesX.Shared.Controls;component/Themes/ThemeDark.xaml", UriKind.Relative);

            var isLight = App.Current.Resources.MergedDictionaries.Where(o => o.Source == light).FirstOrDefault();
            var isDark = App.Current.Resources.MergedDictionaries.Where(o => o.Source == dark).FirstOrDefault();

            string settedTheme = Constants.THEME_DARK;

            if (string.IsNullOrEmpty(ThemeID))
            {
                if (isLight != null)
                {
                    App.Current.Resources.MergedDictionaries.Remove(isLight);
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = dark });

                    Office2019Palette.LoadPreset(Office2019Palette.ColorVariation.Dark);

                    settedTheme = Constants.THEME_DARK;
                }
                else
                {
                    App.Current.Resources.MergedDictionaries.Remove(isDark);
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = light });

                    Office2019Palette.LoadPreset(Office2019Palette.ColorVariation.Gray);

                    settedTheme = Constants.THEME_LIGHT;
                }

                _dataContext.UserContext.ACCESS!.USRPRETEMA = settedTheme;
                _dataContext.UpdateUser();
            }
            else
            {
                if (isLight != null)
                    App.Current.Resources.MergedDictionaries.Remove(isLight);
                else
                    App.Current.Resources.MergedDictionaries.Remove(isDark);

                if (ThemeID == Constants.THEME_LIGHT)
                {
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = light });

                    Office2019Palette.LoadPreset(Office2019Palette.ColorVariation.Gray);

                    settedTheme = Constants.THEME_LIGHT;
                }
                else
                {
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = dark });

                    Office2019Palette.LoadPreset(Office2019Palette.ColorVariation.Dark);

                    settedTheme = Constants.THEME_DARK;
                }
            }
        }
        #endregion

        private void cmbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                var socbase = e.AddedItems[0] as SOCBASE;

                if (socbase != null)
                {
                    _panes.Clear();
                    panelsHome.PanesSource = _panes;

                    _dataContext.SetRoles(socbase.SOMCOD);

                    _dataContext.GetMenu(socbase.SOMCOD);
                }
            }
        }

        private void twMenu_ItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            try
            {
                var clickedItem = e.OriginalSource as RadTreeViewItem;

                if (clickedItem != null)
                {
                    var menuModel = clickedItem.DataContext as MenuModel;

                    if (menuModel != null && menuModel.Uri != null)
                    {
                        var type = Type.GetType($"VulpesX.Modules.{menuModel.Uri}", true, true);

                        if (type != null)
                        {
                            if (type.IsSubclassOf(typeof(UserControl)))
                            {
                                var uc = (UserControl?)Activator.CreateInstance(type, menuModel.Parameters);

                                var newPane = new RadDocumentPane()
                                {
                                    Header = $"{menuModel.Name} - [ {UserContext.Instance.ACCESS!.SelectedCompany!.Description} ]",
                                    Tag = $"{menuModel.Name}{UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD}",
                                    CanFloat = true,
                                    CanDockInDocumentHost = true,
                                    CanUserPin = false,
                                    ContextMenuTemplate = null,
                                    Height = 40,
                                    Content = uc
                                };

                                if (panelsHome.Visibility == Visibility.Collapsed)
                                    panelsHome.Visibility = Visibility.Visible;

                                _panes.Add(newPane);
                                panelsHome.PanesSource = _panes;
                            }

                            if (type.IsSubclassOf(typeof(Window)))
                            {
                                var uc = (Window?)Activator.CreateInstance(type);

                                if (uc != null)
                                {
                                    uc.ShowDialog();
                                }
                            }

                            grdNavigation.Visibility = Visibility.Collapsed;

                            mainGrid.Margin = new Thickness(0, 0, 0, 0);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
            }
        }

        private void wtxtMenuSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => { wtxtMenuSearch.SelectAll(); });
        }

        private void wtxtMenuSearch_GotMouseCapture(object sender, MouseEventArgs e)
        {
            Dispatcher.Invoke(() => { wtxtMenuSearch.SelectAll(); });
        }

        private void wtxtMenuSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dataContext.MenuItems = _menu;

            if (!string.IsNullOrWhiteSpace(wtxtMenuSearch.Text))
            {
                var filteredMenu = new ObservableCollection<MenuModel>();

                foreach (var menu in _dataContext.MenuItems)
                {
                    if (menu.Name.ToLower().Contains(wtxtMenuSearch.Text.ToLower()) && !string.IsNullOrEmpty(menu.Uri))
                    {
                        filteredMenu.Add(menu);
                    }

                    filteredMenu.AddRange(MenuSearch(menu));
                }

                _dataContext.MenuItems = filteredMenu;
            }
        }

        private List<MenuModel> MenuSearch(MenuModel Item)
        {
            var filteredMenu = new List<MenuModel>();

            foreach (var menu in Item.SubItems ?? new List<MenuModel>())
            {
                if (menu.Name.ToLower().Contains(wtxtMenuSearch.Text.ToLower()) && !string.IsNullOrEmpty(menu.Uri))
                {
                    filteredMenu.Add(menu);
                }

                filteredMenu.AddRange(MenuSearch(menu));
            }

            return filteredMenu;
        }

        private void panelsHome_PreviewClose(object sender, Telerik.Windows.Controls.Docking.StateChangeEventArgs e)
        {

        }


        private void grdMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (grdNavigation.Visibility == Visibility.Visible)
            {
                grdNavigation.Visibility = Visibility.Collapsed;
                mainGrid.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                grdNavigation.Visibility = Visibility.Visible;
                mainGrid.Margin = new Thickness(305, 0, 0, 0);
            }
        }

        private void mainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            grdNavigation.Visibility = Visibility.Collapsed;

            mainGrid.Margin = new Thickness(0, 0, 0, 0);
        }

        private void radContextMenu_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var menu = (sender as RadContextMenu);

            var item = (sender as RadContextMenu)?.DataContext as MenuModel;
            
            if (item != null)
            {
                if(string.IsNullOrEmpty(item.Uri))
                {
                    (menu!.Items[0] as RadMenuItem)!.IsEnabled = false;
                }
            }
        }

        private void ctxAddBookmark_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = (sender as RadMenuItem)?.DataContext as MenuModel;

            if (item != null)
            {
                var exist = _dataContext.BookmarkItems[0].SubItems!.Where(o => o.Name == item.Name && o.Uri == item.Uri && o.Username == _dataContext.UserContext.UserName).Any();

                if (!exist)
                {
                    item.Username = _dataContext.UserContext.UserName;

                    _dataContext.BookmarkItems[0].SubItems!.Add(item);

                    _dataContext.UpdateBookmarks();

                    _dataContext.GetBookmarks();
                }
            }
        }

        private void ctxRemoveBookmark_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = (sender as RadMenuItem)?.DataContext as MenuModel;

            if (item != null)
            {
                _dataContext.BookmarkItems[0].SubItems!.Remove(item);

                _dataContext.UpdateBookmarks();

                _dataContext.GetBookmarks();
            }
        }
    }
}