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
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels;

namespace VulpesX
{
    public partial class LoginWindow : Window
    {
        private LoginWindowViewModel _dataContext;

        public LoginWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<LoginWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Username = _dataContext.GetLastLogin();

            //_dataContext.Username = "administrator@ufp.it";
            //pbPassword.Password = "mozart!8";

            //_dataContext.Username = "aldo.beretta@gxitalia.com";
            //pbPassword.Password = "faVamozart!8";

            //_dataContext.Username = "supportox@groupchemie.it";
            //pbPassword.Password = "seRaaLien,99";

            //_dataContext.Username = "demo@demo.it";
            //pbPassword.Password = "dEmoProva!99";

            //_dataContext.Username = "info@baruffaldisrl.it";
            //pbPassword.Password = "pOlimozart!8";

            //_dataContext.Username = "manuela.beretta@cerbertech.com";
            //pbPassword.Password = "Jumpmozart!8";
        }

        private void txtUserName_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserName.SelectAll();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.Password = pbPassword.Password;

            if (!string.IsNullOrEmpty(_dataContext.Username))
            {
                if (!string.IsNullOrEmpty(_dataContext.Password))
                {
                    var user = await _dataContext.Login();

                    if (user != null)
                    {
                        var mainWindow = new MainWindow();
                        mainWindow.Show();

                        App.Current.MainWindow = mainWindow;

                        this.Close();
                    }
                    else
                    {
                        ErrorHandler.Validation("Utente o password errati, riprovare prego");
                    }
                }
            }

        }
    }
}
