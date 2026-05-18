using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.WindowsFactory.Default.General
{
    public interface IABEWindowFactory
    {
        IWindowFactory Create(ABEWindowViewModel dataContext);
    }

    public class ABEWindowFactory : IABEWindowFactory
    {
        public IWindowFactory Create(ABEWindowViewModel user)
        {
            switch (UserContext.Instance.Domain)
            {
                case Constants.UFP_DOMAIN:
                    return new Modules.Ufp.General.ABEWindow(user);
                default:
                    return new Modules.Default.General.ABEWindow(user);
            }
        }
    }
}
