using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Default.Partials;

namespace VulpesX.Models
{
    public class BankContext
    {
        private static BankContext? _instance;
        public static BankContext Instance => _instance ??= new BankContext();

        public ObservableCollection<BankItem>? OfflineBanks { get; set; }
    }
}
