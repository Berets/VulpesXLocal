using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.DWH;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.DWH
{
    public class DWHTemplateWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public DWHTemplateWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required DWH_Template Data { get; set; }
        public bool IsInsert { get; set; }

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

        private ObservableCollection<DWH_Template>? folders;
        public ObservableCollection<DWH_Template>? Folders
        {
            get => folders; set
            {
                folders = value;
                NotifyPropertyChanged();
            }
        }

        private DWH_Template? selectedFolder;
        public DWH_Template? SelectedFolder
        {
            get => selectedFolder; set
            {
                selectedFolder = value;
                NotifyPropertyChanged();
            }
        }

        public async Task GetFolders()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Folders = new ObservableCollection<DWH_Template>( VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().GetFolders(CompanyID) ?? new List<DWH_Template>()));

            IsBusy = false;
        }

        public DWH_Folder? GetFolder(Guid FolderID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_folderRepository>().Get(CompanyID, FolderID);
        }

        public bool DeleteFolder(Guid FolderID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_folderRepository>().Delete(CompanyID, FolderID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.LogAdded = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                Data.LogAddedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().Insert(Data);
            }
            else
            {
                Data.LogUpdated = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                Data.LogUpdatedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<Idwh_templateRepository>().Update(Data);
            }
        }
    }
}
