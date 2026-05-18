using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Auth;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels
{
    public class LoginWindowViewModel : Base
    {
        private IAuthRepository? _authRepository;
        public LoginWindowViewModel(IAuthRepository authRepositoryFactory)
        {
            _authRepository = authRepositoryFactory;

        }

        private bool _isBusy;
        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        public string? Username { get; set; }
        public string? Password { get; set; }

        public string AppVersion => VersionHelper.GetVersionText();
        public string? ErrorMessage { get; set; }

        public string? GetLastLogin()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>().CheckLastUsername();
        }

        public async Task<ACCESS?> Login()
        {
            try
            {
                IsBusy = true;

                if (!string.IsNullOrEmpty(Username))
                {
                  
                    var userContext = UserContext.Instance;
                    userContext.UserName = Username;
                    userContext.Password = Password;

                    var authRepo = VulpesServiceProvider.Provider.GetRequiredService<IAuthRepository>();
                    await Task.Delay(500);

                    if (!string.IsNullOrEmpty(Password))
                    {
                        var access = await authRepo.Login(Username!, Password);

                        userContext.ACCESS = access;

                        if (userContext.ACCESS != null)
                        {
                            userContext.ACCESS.USRID = userContext.ACCESS.USRID.TrimEnd();

                            authRepo.WriteLastLogin(Username);

                            VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().RefreshOfflineBanksListAsync();

                            return access;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.ToString();
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

}
