using DocumentFormat.OpenXml.Office2010.Word;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class MPTESTATAWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public MPTESTATAWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required MPTESTATA Data { get; set; }
        public bool IsInsert { get; set; }


        private ObservableCollection<MPDETTAGLIO> rows = new();
        public ObservableCollection<MPDETTAGLIO> Rows
        {
            get { return rows; }
            set
            {
                rows = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<MANDATO>? Types { get; set; }

        private MANDATO? selectedType;
        public MANDATO? SelectedType { get => selectedType; set { selectedType = value; NotifyPropertyChanged("SelectedType"); } }

        public ObservableCollection<BANAZIEN>? Banks { get; set; }

        private BANAZIEN? selectedBank;
        public BANAZIEN? SelectedBank { get => selectedBank; set { selectedBank = value; NotifyPropertyChanged("SelectedBank"); } }

        public ObservableCollection<ABE>? SuppliersCache { get; set; }

        public ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public ObservableCollection<MANDATO>? GetMANDATOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMANDATORepository>().GetList();
        }

        public ObservableCollection<BANAZIEN>? GetBANAZIENs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().GetListActive(CompanyID);
        }

        public ObservableCollection<MPDETTAGLIO>? GetMPDETTAGLIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IMPDETTAGLIORepository>().GetList(CompanyID, Data.MPANNO, Data.MPNUME);
        }

        public ObservableCollection<ABE>? GetSuppliersCache()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public string? Validation()
        {
            if (Data.MPANNO == 0)
                return "Selezionare un anno";
            if (selectedType == null)
                return "Selezionare un tipo di mandato";
            if (SelectedBank == null)
                return "Selezionare una banca interna";

            var import = Rows.Where(o => o.M3SEGN == "A").Sum(s => s.M3IMEU) - Rows.Where(o => o.M3SEGN == "D").Sum(s => s.M3IMEU);
            if(import <= 0)
            {
                return "Controllare le partite inserite. Selezionate partite a credito";
            }

            return null;
        }

        public string? Save()
        {
            if (IsInsert)
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().Insert(Data, Rows);
            }
            else
            {
                return VulpesServiceProvider.Provider.GetRequiredService<IMPTESTATARepository>().Update(Data, Rows);
            }
        }
    }
}
