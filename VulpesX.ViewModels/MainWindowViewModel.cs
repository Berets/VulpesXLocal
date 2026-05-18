using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VulpesX.DAL.Assets;
using VulpesX.DAL.Auth;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.ViewModels
{
    public class MainWindowViewModel : Base
    {
        private readonly IAUTH_ACCESS_ROLESRepository _iauth_access_rolesRepository;
        private readonly IMenuRepository _IMenuRepository;
        public required UserContext UserContext { get; set; }
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public MainWindowViewModel(IAUTH_ACCESS_ROLESRepository IAUTH_ACCESS_ROLESRepository)
        {
            _iauth_access_rolesRepository = IAUTH_ACCESS_ROLESRepository;
            _IMenuRepository = VulpesServiceProvider.Provider.GetRequiredService<IMenuRepository>();

            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public ObservableCollection<ABE> Customers { get; set; } = new();

        private ABE? _selectedCustomer;
        public ABE? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<MenuModel> _menuItems = new();
        public ObservableCollection<MenuModel> MenuItems
        {
            get { return _menuItems; }
            set
            {
                _menuItems = value;
                NotifyPropertyChanged();

            }
        }

        private ObservableCollection<MenuModel> _bookmarkItems = new();
        public ObservableCollection<MenuModel> BookmarkItems
        {
            get { return _bookmarkItems; }
            set
            {
                _bookmarkItems = value;
                NotifyPropertyChanged();

            }
        }

        public ObservableCollection<PaneModel> Panes { get; set; } = new ObservableCollection<PaneModel>();


        public void SetRoles(string CompanyID)
        {
            UserContext.Instance.ACCESS!.Roles = _iauth_access_rolesRepository.Get(CompanyID, UserContext.Instance.UserName!);
        }

        public void GetMenu(string CompanyID)
        {
            MenuItems = _IMenuRepository.GetFull(CompanyID, UserContext.Instance.ACCESS!.USROLD);
        }

        public void GetBookmarks()
        {
            var saved = _IMenuRepository.GetBookmarks().Where(o => o.Username == UserContext.UserName);

            var retValue = new List<MenuModel>();
            retValue.Add(new MenuModel { Name = "Preferiti", SubItems = saved.ToList() });

            BookmarkItems = new ObservableCollection<MenuModel>(retValue);
        }

        public void UpdateBookmarks()
        {
            _IMenuRepository.UpdateBookmarks(BookmarkItems);
        }

        public void UpdateUser()
        {
            try
            {
                var authRepo = VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>();

                authRepo.Update(UserContext.ACCESS!);

                UserContext.ACCESS!.rv = authRepo.Get(UserContext.ACCESS!.USRID)?.rv;
            }
            catch (Exception)
            {

            }
        }
    }
}
