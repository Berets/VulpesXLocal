using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Models.Models.Accounting
{
    public class AccountingSituationViewModel
    {
        public ObservableCollection<ASItem>? AttivitaGruppi { get; set; }
        public ObservableCollection<ASItem>? PassivitaGruppi { get; set; }
        public ObservableCollection<ASItem>? RicaviGruppi { get; set; }
        public ObservableCollection<ASItem>? CostiGruppi { get; set; }

        public decimal SaldoAP => (AttivitaGruppi?.Sum(sum => sum.Saldo) ?? 0) - (PassivitaGruppi?.Sum(sum => sum.Saldo) ?? 0);
        public string SaldoAPColore => SaldoAP >= 0 ? "W" : "O";
        public decimal SaldoRC => (RicaviGruppi?.Sum(sum => sum.Saldo) ?? 0) - (CostiGruppi?.Sum(sum => sum.Saldo) ?? 0);
        public string SaldoRCColore => SaldoRC >= 0 ? "W" : "O";

        public class ASItem : Base
        {
            public string? GroupID { get; set; }
            public string? AccountID { get; set; }
            public string? Description { get; set; }
            public bool IsDare { get; set; } = false;

            private decimal dare;
            private decimal avere;

            public decimal Dare { get => dare; set { dare = value; NotifyPropertyChanged("Saldo"); NotifyPropertyChanged("SaldoColore"); } }
            public decimal Avere { get => avere; set { avere = value; NotifyPropertyChanged("Saldo"); NotifyPropertyChanged("SaldoColore"); } }
            public decimal Saldo => IsDare ? Dare - Avere : Avere - Dare;

            public string SaldoColore => Saldo >= 0 ? "W" : "O";

            public ObservableCollection<ASItem>? Accounts { get; set; }
        }
    }
}
