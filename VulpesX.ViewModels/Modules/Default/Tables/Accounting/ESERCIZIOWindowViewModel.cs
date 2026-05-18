using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using VulpesX.Shared.Utilities;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public class ESERCIZIOWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public ObservableCollection<GenericIntIDDescription> Months { get; set; }
        public ObservableCollection<GenericIDDescription> OCStatuses { get; set; }
        public ObservableCollection<GenericIDDescription> Liquidations { get; set; }

        public ESERCIZIOWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;

            Months = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetMonthsList();
            OCStatuses = new ObservableCollection<GenericIDDescription>() {
                new GenericIDDescription(){ ID = "A", Description = "Aperto" },
                new GenericIDDescription(){ ID = "C", Description = "Chiuso" },
                new GenericIDDescription(){ ID = "U", Description = "Ultimo" }
            };
            Liquidations = new ObservableCollection<GenericIDDescription>() {
                new GenericIDDescription(){ ID = "M", Description = "Mensile" },
                new GenericIDDescription(){ ID = "T", Description = "Trimestrale" }
            };
        }

        public required ESERCIZIO Data { get; set; }
        public bool IsInsert { get; set; }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;

                var inserResult = VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Insert(Data);

                if (inserResult)
                {
                    var done = VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Generate(CompanyID, Data.eseann);

                    if (done)
                        InfoHandler.Show("Creazione dei sottoconti contabili avvenuta con successo");
                    else
                        ErrorHandler.Validation("Errore durante la creazione dei sottoconti contabili");

                }
                return false;
            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().Update(Data);
            }
        }
    }
}
