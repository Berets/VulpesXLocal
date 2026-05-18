using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.DWH;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.DWH
{
    public class DWHViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public DWHViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

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

        private ObservableCollection<DWH_Query>? queries;
        public ObservableCollection<DWH_Query>? Queries
        {
            get => queries; set
            {
                queries = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<DWH_Template>? templates;
        public ObservableCollection<DWH_Template>? Templates
        {
            get => templates; set
            {
                templates = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusyQueries;
        public bool IsBusyQueries
        {
            get { return _isBusyQueries; }
            set
            {
                _isBusyQueries = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusyTemplates;
        public bool IsBusyTemplates
        {
            get { return _isBusyTemplates; }
            set
            {
                _isBusyTemplates = value;
                NotifyPropertyChanged();
            }
        }

        public async Task LoadQueries()
        {
            IsBusyQueries = true;

            await Task.Run(() =>
                       Queries = VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().GetList(CompanyID));

            IsBusyQueries = false;
        }

        public async Task LoadTemplates()
        {
            IsBusyTemplates = true;

            await Task.Run(() =>
            {
                var dwhTemplateRepo = VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>();

                var list = new List<DWH_Template>();
                list.AddRange(dwhTemplateRepo.GetFoldersTemplates(CompanyID, UserID) ?? new List<DWH_Template>());
                list.AddRange(dwhTemplateRepo.GetRootTemplates(CompanyID, UserID) ?? new List<DWH_Template>());

                Templates = new ObservableCollection<DWH_Template>(list);
            });

            IsBusyTemplates = false;

        }


        public DWH_Query? GetQuery(Guid QueryID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Get(CompanyID, QueryID);
        }

        public DWH_Template? GetTemplate(Guid TemplateID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().Get(CompanyID, TemplateID);
        }

        public bool UpdateTemplate(DWH_Template Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().Update(Item);
        }

        public bool DeleteTemplate(DWH_Template Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().Delete(Item);
        }



        public async Task<DataTable?> Execute(DWH_Query Query)
        {
            IsBusy = true;

            DataTable? retValue = null;

            await Task.Run(() =>
            {
                retValue = VulpesServiceProvider.Provider.GetRequiredService<Idwh_queryRepository>().Execute(Query);
            });

            IsBusy = false;

            return retValue;
        }
    }
}
