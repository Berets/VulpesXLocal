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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Auth;

namespace VulpesX.Modules.Default.Auth
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : FluentDefaultWindow
    {
        private UserWindowViewModel _dataContext;
        public UserWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<UserWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }

        private void cmdPasswordChange_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.ValidateChangePassword(pbOldPassword.Password, pbNewPassword.Password, pbConfirmPassword.Password);

            if (string.IsNullOrWhiteSpace(validated))
            {
                _dataContext.Data!.USRPWD = CryptoHelper.SHA512Hasher(pbNewPassword.Password);

                if (_dataContext.Update())
                {
                    InfoHandler.Show("La password e' stata aggiornata correttamente, ricordate che i primi 4 caratteri sono sempre gli stessi");
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
        #endregion
    }
}
