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
using VulpesX.Models;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Reports
{
    public class BatchReportWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public BatchReportWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public ObservableCollection<GenericIDDescription> CFFlags => CommonsService.EntityTypes;

        private string? entityType;
        public string? EntityType { get => entityType; set { entityType = value; NotifyPropertyChanged("EntityType"); } }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }


        private ObservableCollection<BatchReportModel.EntityModel>? items;
        public ObservableCollection<BatchReportModel.EntityModel>? Items { get => items; set { items = value; NotifyPropertyChanged("Items"); } }

        public async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var items = new ObservableCollection<BatchReportModel.EntityModel>();

                    if (EntityType == "C")
                        items = VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().GetBatch(CompanyID, From!.Value, To!.Value);
                    else
                        items = VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().GetBatch(CompanyID, From!.Value, To!.Value);

                    return new { items };

                });

                Items = result.items;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
