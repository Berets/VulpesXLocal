using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.WindowsFactory.Default.Accounting
{
    public interface IACC_PLAFONDWindowFactory
    {
        IWindowFactory Create(ACC_PLAFONDWindowViewModel dataContext);
    }

    public class IVACloseWindowFactory : IACC_PLAFONDWindowFactory
    {
        public IWindowFactory Create(ACC_PLAFONDWindowViewModel user)
        {
            switch (UserContext.Instance.Domain)
            {
                case Constants.UFP_DOMAIN:
                    return new Modules.Ufp.Accounting.ACC_PLAFONDWindow(user);
                default:
                    return new Modules.Default.Accounting.ACC_PLAFONDWindow(user);
            }
        }
    }
}
