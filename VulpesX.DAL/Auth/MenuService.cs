using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;

namespace VulpesX.DAL.Auth
{
    public interface IMenuRepository
    {
        ObservableCollection<MenuModel> GetFull(string CompanyID, string UserID, bool IsAdministratorView = false);

        ObservableCollection<MenuModel> GetBookmarks();

        void UpdateBookmarks(ObservableCollection<MenuModel> Bookmarks);

        bool UpdateMenuRole(string CompanyID, ACCESS User, string MenuJSON, AUTH_ACCESS_ROLES Role);
    }

    public class MenuRepository : RepositoryBase, IMenuRepository
    {
        public MenuRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MenuModel> GetFull(string CompanyID, string UserID, bool IsAdministratorView = false)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                var menus = new List<MenuModel>();

                ACCESOCE? accessCompany = null;

                if (connection != null)
                {
                    connection.Open();

                    accessCompany = connection.QueryFirst<ACCESOCE>("SELECT * FROM ACCESOCE WHERE acssoc = @CompanyID AND acsute = @UserID", new { CompanyID, UserID });

                    if (accessCompany != null && !string.IsNullOrEmpty(accessCompany.acsmenux))
                    {
                        menus = JsonConvert.DeserializeObject<List<MenuModel>>(accessCompany.acsmenux);
                    }
                }

                var retValue = new List<MenuModel>();

                retValue.Add(new MenuModel
                {
                    Name = "Anagrafiche",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Articoli", Uri= "Default.General.ARTView"},
                        new MenuModel{ Name = "Unica C/F", Uri = "Default.General.ABEView" }
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Contabilita'",
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Piano dei conti" ,  Uri="Default.Accounting.PDCView"},
                        new MenuModel{ Name= "Dichiarazioni d'intento",Uri="Default.Accounting.ACC_PLAFONDView"},
                        new MenuModel{ Name= "Prima nota", Uri="Default.Accounting.PNView"},
                        new MenuModel{ Name= "Mandati di pagamento", Uri="Default.Accounting.MPTESTATAView"},
                        new MenuModel{ Name= "Registrazione corrispettivi",},
                        new MenuModel{ Name= "Portafoglio", Uri="Default.Accounting.PortafoglioView"},
                        new MenuModel{ Name= "Fatture elettroniche",Uri="Default.Accounting.Invoicing.ACC_EINVOICE_HEADSView"},
                        new MenuModel{ Name= "Funzioni",
                            SubItems = new List<MenuModel>
                            {
                                   new MenuModel{ Name="Chiusura contabile periodica", Uri="Default.Accounting.Functions.ClosePeriodWindow"},
                                   new MenuModel{ Name="Chiusura contabile annuale", Uri="Default.Accounting.Functions.CloseYearWindow"},
                                   new MenuModel{ Name="Pareggiamento partite", Uri="Default.Accounting.Functions.EqualizationWindow"},
                            }
                        },
                       new MenuModel{ Name= "IVA",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name="Prima nota IVA",  Uri="Default.Accounting.IVA.PNIVAView"},
                                new MenuModel{ Name="Stampa libri IVA", Uri="Default.Accounting.IVA.IVABookWindow"},
                                new MenuModel{ Name="Liquidazione IVA",  Uri="Default.Accounting.IVA.IVACloseWindow"},
                                new MenuModel{ Name="Gestione LIPE",  Uri="Default.Accounting.IVA.TCOMLIQIVAView"},
                            }
                        },
                       new MenuModel{ Name= "Interrogazioni",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name="Mastrini",  Uri="Default.Accounting.MastrinoView"},
                                new MenuModel{ Name="E/C clienti|fornitori", Uri="Default.Accounting.ECView"},
                            }
                        },
                        new MenuModel{ Name= "Stampe",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name="Bilancino di verifica",  Uri="Default.Accounting.Reports.BalanceSimulationWindow"},
                                new MenuModel{ Name="Giornale generale", Uri="Default.Accounting.Reports.GeneralJournalWindow"},
                                new MenuModel{ Name="Mastrini",  Uri="Default.Accounting.Reports.MastrinoReportWindow"},
                                new MenuModel{ Name="Scadenziari clienti/fornitori",  Uri="Default.Accounting.Reports.ExpiresReportWindow"},
                                new MenuModel{ Name="Partitario clienti/fornitori",  Uri="Default.Accounting.Reports.BatchReportWindow"},
                            }
                        }
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Cespiti",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Schede cespiti", Uri= "Default.Accounting.Assets.ACC_ASSETS_CARDSView"},
                        new MenuModel{ Name= "Calcolo ammortamento", Uri= "Default.Accounting.Assets.AssetComputeWindow"},
                        new MenuModel{ Name= "Contabilizzazione cespiti", Uri= "Default.Accounting.Assets.AssetAccountingWindow"},
                        new MenuModel{ Name= "Aggiornamento cespiti", Uri= "Default.Accounting.Assets.AssetHistoryWindow"},

                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Magazzino",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Giacenze", Uri= "Default.Stores.StoreStocksView"},
                        new MenuModel{ Name= "Movimenti", Uri= "Default.Stores.StoreMovementsView"},
                         new MenuModel{ Name= "Reports",
                            SubItems = new List<MenuModel>
                            {
                                   new MenuModel{ Name="Stampa giacenze", Uri="Default.Stores.StoreStocksReportWindow"},
                            }
                        },
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "CRM",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Offerte", Uri= "Default.CRM.OFFET00FView"},
                        new MenuModel{ Name= "Ordini clienti", Uri= "Default.CRM.ORDIT00FView"},
                        new MenuModel{ Name= "DDT clienti", Uri= "Default.CRM.BOLLT00FView",  Parameters=new object[]{ "C" } },
                        new MenuModel{ Name= "Fatture", Uri= "Default.CRM.FATTT00FView" },
                        new MenuModel
                        {
                            Name = "Listini",
                            Uri = null,
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name= "Listini generali", Uri= "Default.CRM.LISGENView"},
                                new MenuModel{ Name= "Listini clienti", Uri= "Default.CRM.LISCLIView"},
                            }
                        }
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "SRM",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "RDA", Uri= "Default.SRM.RDAView"},
                        new MenuModel{ Name= "Ordini di acquisto", Uri= "Default.SRM.ACQOrderView"},
                        new MenuModel{ Name= "Entrata merci", Uri= "Default.SRM.GoodsReceiptView"},
                        new MenuModel{ Name= "DDT fornitori", Uri= "Default.CRM.BOLLT00FView", Parameters=new object[]{ "F" } },
                        new MenuModel{ Name= "Costi materiale", Uri= "Default.SRM.CostMaterialsView"},
                        new MenuModel
                        {
                            Name = "Listini",
                            Uri = null,
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name= "Listini fornitore", Uri= "Default.SRM.SRM_LISFORView"},
                            }
                        }
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Assets",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Assets", Uri= "Default.Assets.ASSET00FView"},
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Produzione",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Ordini di produzione", Uri= "Default.Production.ProductionOrderView"},
                        new MenuModel{ Name= "Gestione lotti", Uri= "Default.Production.LotManagerView"},
                        new MenuModel{ Name= "Gantt", Uri= "Default.Production.GanttView"},
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Tesoreria",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Movimenti e saldi", Uri= "Default.Treasury.MovementsView"},
                        new MenuModel{ Name= "Impegni fiscali", Uri= "Default.Treasury.CommitmentView"},
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "DWH",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Query", Uri= "Default.DWH.DWHQueryView"},
                        new MenuModel{ Name= "Visualizza", Uri= "Default.DWH.DWHView"},
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Tabelle",
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel
                        {
                            Name = "Generiche",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Lingue" , Uri = "Default.Tables.General.LINGUAView"},
                                new MenuModel{ Name = "Testi" , Uri = "Default.Tables.General.TAB_GEN_TEXTSView"},
                                new MenuModel{ Name = "Ruoli contatto" , Uri = "Default.Tables.General.TAB_GEN_CONTACTS_ROLESView" },
                                new MenuModel{ Name = "Tipi contatto" , Uri = "Default.Tables.General.TAB_GEN_CONTACTS_TYPESView" },
                                new MenuModel{ Name = "Tipi attività" , Uri = "Default.Tables.General.TAB_GEN_ACTIVITY_TYPESView" },
                            }
                        },
                        new MenuModel
                        {
                            Name = "Contabilità",
                            SubItems = new List<MenuModel>
                            {
                                 new MenuModel
                                 {
                                    Name = "Base",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "ABI/CAB" , Uri = "Default.Tables.Accounting.ABICABView" },
                                        new MenuModel{ Name = "Aliquote" , Uri = "Default.Tables.Accounting.AliquoteView" },
                                        new MenuModel{ Name = "Comuni" , Uri = "Default.Tables.Accounting.COMUNIView" },
                                        new MenuModel{ Name = "ISO" , Uri = "Default.Tables.Accounting.ISOView" },
                                        new MenuModel{ Name = "Nazioni" , Uri = "Default.Tables.Accounting.NAZIONIView"},
                                        new MenuModel{ Name = "Provincie" , Uri = "Default.Tables.Accounting.TAB_STATESView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Gestionali",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Affidabilità" , Uri = "Default.Tables.Accounting.AFFIDABILITAView" },
                                        new MenuModel{ Name = "Banche aziendali" , Uri = "Default.Tables.Accounting.BANAZIENView" },
                                        new MenuModel{ Name = "Cambi" , Uri = "Default.Tables.Accounting.CAMBIView" },
                                        new MenuModel{ Name = "Causali contabili" , Uri = "Default.Tables.Accounting.CAUCONTView"},
                                        new MenuModel{ Name = "Centri di costo" , Uri = "Default.Tables.Accounting.TCECO00FView" },
                                        new MenuModel{ Name = "Codici di chiusura" , Uri = "Default.Tables.Accounting.TAB_ACC_CLOSINGView" },
                                        new MenuModel{ Name = "Esercizi contabili" , Uri = "Default.Tables.Accounting.ESERCIZIOView"},
                                        new MenuModel{ Name = "Incassi clienti" , Uri = "Default.Tables.Accounting.PAGCLIView"},
                                        new MenuModel{ Name = "Libri IVA" , Uri = "Default.Tables.Accounting.LIBRIIVAView" },
                                        new MenuModel{ Name = "Numeratori" , Uri = "Default.Tables.Accounting.NUMREGView" },
                                        new MenuModel{ Name = "Pagamenti fornitori" , Uri = "Default.Tables.Accounting.PAGFORView" },
                                        new MenuModel{ Name = "Ritenute" , Uri = "Default.Tables.Accounting.RITENUTEView"},
                                        new MenuModel{ Name = "Spostamento scadenze" , Uri = "Default.Tables.Accounting.SCADENZEView"},
                                        new MenuModel{ Name = "Tipi incasso" , Uri = "Default.Tables.Accounting.TAB_ACC_TIPINCView" },
                                        new MenuModel{ Name = "Tipi pagamento" , Uri = "Default.Tables.Accounting.TAB_ACC_TIPPAGView" },
                                        new MenuModel{ Name = "Tipi solleciti" , Uri = "Default.Tables.Accounting.SOLLECITIView"},
                                        new MenuModel{ Name = "Tipi codici LIPE" , Uri = "Default.Tables.Accounting.TCODLIQIVAView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Cespiti",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Aliquote cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_RATESView"},
                                        new MenuModel{ Name = "Categorie fiscali cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_CATEGORIESView" },
                                        new MenuModel{ Name = "Tipi cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_TYPESView"},
                                        new MenuModel{ Name = "Tipologie cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_TYPOLOGIESView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Fatturazione elettronica",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Modalità di pagamento" , Uri = "Default.Tables.Accounting.FE_PAGDOCView" },
                                        new MenuModel{ Name = "Regimi fiscali" , Uri = "Default.Tables.Accounting.FE_RFIDOCView" },
                                        new MenuModel{ Name = "Tipi di cassa previdenziale" , Uri = "Default.Tables.Accounting.FE_TIPOCPView" },
                                        new MenuModel{ Name = "Tipi di documento" , Uri = "Default.Tables.Accounting.FE_TIPODOCView" },
                                        new MenuModel{ Name = "Tipi di natura IVA" , Uri = "Default.Tables.Accounting.FE_IVADOCView" },
                                        new MenuModel{ Name = "Tipi di ritenute" , Uri = "Default.Tables.Accounting.FE_TIPORITView"},
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Classificazione geografica",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Aree" , Uri = "Default.Tables.Accounting.AREEView" },
                                        new MenuModel{ Name = "Regioni" , Uri = "Default.Tables.Accounting.REGIONIView" },
                                        new MenuModel{ Name = "Zone" , Uri = "Default.Tables.Accounting.ZONEView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Classificazione statistica",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Categorie clienti" , Uri = "Default.Tables.Accounting.CATEGORIAView"},
                                        new MenuModel{ Name = "Classificazione clienti" , Uri = "Default.Tables.Accounting.CLAZIONEView"},
                                        new MenuModel{ Name = "Filiali" , Uri = "Default.Tables.Accounting.FILIALIView" },
                                        new MenuModel{ Name = "Rivenditori" , Uri = "Default.Tables.Accounting.RIVENDITORIView" },
                                        new MenuModel{ Name = "Settori merceologici" , Uri = "Default.Tables.Accounting.MERCEOLOGICOView"},
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Gestione commerciale",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Agenti" , Uri = "Default.Tables.Accounting.AGENTIView" },
                                        new MenuModel{ Name = "Consegna" , Uri = "Default.Tables.Accounting.CONSEGNAView" },
                                        new MenuModel{ Name = "Depositi" , Uri = "Default.Tables.Accounting.DEPOSITIView"},
                                        new MenuModel{ Name = "Imballi" , Uri = "Default.Tables.Accounting.IMBALLIView"},
                                        new MenuModel{ Name = "Spedizioni" , Uri = "Default.Tables.Accounting.SPEDIZIONEView" },
                                        new MenuModel{ Name = "Vettori" , Uri = "Default.Tables.Accounting.VETTORIView"},
                                    }
                                 }
                            }
                        },
                        new MenuModel
                        {
                            Name = "CRM",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Tipi dettaglio analisi fattibilità" , Uri = "Default.Tables.CRM.TAB_CRM_FEASABILITYView" },
                                new MenuModel{ Name = "Causali offerte" , Uri = "Default.Tables.CRM.TAB_CRM_CAUOFFView"},
                                new MenuModel{ Name = "Causali chiusura offerte" , Uri = "Default.Tables.CRM.TAB_CRM_CAUOFFCLOView" },
                                new MenuModel{ Name = "Causali ordini clienti" , Uri = "Default.Tables.CRM.TAB_CRM_CAUORDView"},
                                new MenuModel{ Name = "Causali DDT" , Uri = "Default.Tables.CRM.CAUSBOLLView" },
                                new MenuModel{ Name = "Causali fatture" , Uri = "Default.Tables.CRM.CAUFAT00FView" },
                            }
                        },
                        new MenuModel
                        {
                            Name = "Assets",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Ubicazioni" , Uri = "Default.Tables.Assets.TAB_AST_LOCATIONSView" },
                            }
                        },
                        new MenuModel
                        {
                            Name = "Magazzino",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Magazzini" , Uri = "Default.Tables.Store.STORE_STORESView"},
                                new MenuModel{ Name = "Causali di magazzino" , Uri = "Default.Tables.Store.STORE_CAUSALSView" },
                            }
                        },
                        new MenuModel
                        {
                            Name = "Articolo",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Unità di misura" , Uri = "Default.Tables.Article.UnitaView" },
                                new MenuModel{ Name = "Categoria" , Uri = "Default.Tables.Article.CategoriaView"},
                                new MenuModel{ Name = "Tipi articolo" , Uri = "Default.Tables.Article.TipoView" },
                            }
                        },
                        new MenuModel
                        {
                            Name = "Produzione",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Reparti" , Uri = "Default.Tables.Production.RepartiView" },
                                new MenuModel{ Name = "Risorse" , Uri = "Default.Tables.Production.RisorseView"},
                                new MenuModel{ Name = "Operatori" , Uri = "Default.Tables.Production.OperatoriView" },
                                new MenuModel{ Name = "Causali fermo" , Uri = "Default.Tables.Production.CausaliView"},
                                new MenuModel{ Name = "Calendario" , Uri = "Default.Tables.Production.CalendarioView"},
                            }
                        },
                        new MenuModel
                        {
                            Name = "Rating cliente",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Elementi" , Uri = "Default.Tables.CustomerRating.ElementView" },
                                new MenuModel{ Name = "Rating" , Uri = "Default.Tables.CustomerRating.RatingView"},
                                new MenuModel{ Name = "Punti finanziaria" , Uri = "Default.Tables.CustomerRating.PointView"},
                            }
                        },
                        new MenuModel
                        {
                            Name = "Energy monitor",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Devices" , Uri = "Default.Tables.EnergyMonitor.DeviceView"},
                            }
                        },
                    }
                });

                ExtractAndUpdateMenu(retValue, menus ?? new List<MenuModel>(), (accessCompany?.acsadmin ?? false), IsAdministratorView);

                return new ObservableCollection<MenuModel>(retValue);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new ObservableCollection<MenuModel>();
            }
        }

        private void ExtractAndUpdateMenu(List<MenuModel> Menu, List<MenuModel> Abilitations, bool IsAdministrator, bool IsAdministratorView)
        {
            foreach (var menu in Menu)
            {
                if (!string.IsNullOrEmpty(menu.Uri))
                {
                    menu.IsEnabled = (IsAdministrator) ? true : (Abilitations.Where(o => o.Name == menu.Name && o.Uri == menu.Uri).Any() ? Abilitations.Where(o => o.Name == menu.Name && o.Uri == menu.Uri).Select(s => s.IsEnabled).FirstOrDefault() : false);
                }
                else
                {
                    if (!IsAdministratorView)
                        menu.IsEnabled = true;
                }

                if (menu.SubItems != null && menu.SubItems.Any())
                {
                    ExtractAndUpdateMenu(menu.SubItems, Abilitations, IsAdministrator, IsAdministratorView);
                }
            }
        }

        public ObservableCollection<MenuModel> GetBookmarks()
        {
            var fullPath = @$"{Path.GetTempPath()}.vulpesx\bookmarks.json";

            if (File.Exists(fullPath))
            {
                return JsonConvert.DeserializeObject<ObservableCollection<MenuModel>>(File.ReadAllText(fullPath)) ?? new();
            }
            return new ObservableCollection<MenuModel>();
        }

        public void UpdateBookmarks(ObservableCollection<MenuModel> Bookmarks)
        {
            string jsonString = JsonConvert.SerializeObject(Bookmarks[0].SubItems, Formatting.Indented);

            string folderPath = Path.Combine(Path.GetTempPath(), ".vulpesx");
            string filePath = Path.Combine(folderPath, "bookmarks.json");

            Directory.CreateDirectory(folderPath);

            File.WriteAllText(@$"{Path.GetTempPath()}.vulpesx\bookmarks.json", jsonString);
        }

        public bool UpdateMenuRole(string CompanyID, ACCESS User, string MenuJSON, AUTH_ACCESS_ROLES Role)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        var result = connection.ExecuteScalar("UPDATE ACCESOCE SET acsadmin = @IsAdmin, acsmenux = @MenuJSON OUTPUT INSERTED.rv WHERE acssoc = @CompanyID AND acsute = @UserID", new { CompanyID, UserID = User.USROLD, MenuJSON, IsAdmin = User.SelectedCompany?.AccessCompany?.acsadmin ?? false }, transaction);

                        if (result != null)
                        {
                            var authAccessRoleRepo = VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>();

                            bool roleResult = false;
                            if (authAccessRoleRepo.Exists(CompanyID, User.USRID))
                                roleResult = authAccessRoleRepo.Update(Role);
                            else
                                roleResult = authAccessRoleRepo.Insert(Role);

                            if (roleResult)
                            {
                                transaction.Commit();

                                return true;
                            }
                            else
                            {
                                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                                return false;
                            }
                        }
                        else
                        {
                            ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                            return false;
                        }
                    }
                }
                else
                {
                    ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
    }

    public class MenuUfpRepository : RepositoryBase, IMenuRepository
    {
        public MenuUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MenuModel> GetFull(string CompanyID, string UserID, bool IsAdministratorView = false)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                var menus = new List<MenuModel>();

                ACCESOCE? accessCompany = null;

                if (connection != null)
                {
                    connection.Open();

                    accessCompany = connection.QueryFirst<ACCESOCE>("SELECT * FROM ACCESOCE WHERE acssoc = @CompanyID AND acsute = @UserID", new { CompanyID, UserID });

                    if (accessCompany != null && !string.IsNullOrEmpty(accessCompany.acsmenux))
                    {
                        menus = JsonConvert.DeserializeObject<List<MenuModel>>(accessCompany.acsmenux);
                    }
                }

                var retValue = new List<MenuModel>();

                retValue.Add(new MenuModel
                {
                    Name = "Anagrafiche",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name = "Unica C/F", Uri = "Ufp.General.ABEView" }
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Contabilita'",
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Piano dei conti" , Uri="Ufp.Accounting.PDCView"},
                        new MenuModel{ Name= "Dichiarazioni d'intento",Uri="Default.Accounting.ACC_PLAFONDView"},
                        new MenuModel{ Name= "Prima nota",Uri="Default.Accounting.PNView"},
                        new MenuModel{ Name= "Mandati di pagamento", Uri="Default.Accounting.MPTESTATAView"},
                        new MenuModel{ Name= "Portafoglio", Uri="Ufp.Accounting.PortafoglioView"},
                        new MenuModel{ Name= "Fatture elettroniche",Uri="Ufp.Accounting.Invoicing.ACC_EINVOICE_HEADSView"},
                        new MenuModel{ Name= "Funzioni",
                            SubItems = new List<MenuModel>
                            {
                                   new MenuModel{ Name="Chiusura contabile periodica", Uri="Default.Accounting.Functions.ClosePeriodWindow"},
                                   new MenuModel{ Name="Chiusura contabile annuale", Uri="Default.Accounting.Functions.CloseYearWindow"},
                                   new MenuModel{ Name="Pareggiamento partite", Uri="Default.Accounting.Functions.EqualizationWindow"},
                            }
                        },
                        new MenuModel{ Name= "IVA",
                            SubItems = new List<MenuModel>
                            {
                                    new MenuModel{ Name="Prima nota IVA",  Uri="Default.Accounting.IVA.PNIVAView"},
                                    new MenuModel{ Name="Stampa libri IVA", Uri="Default.Accounting.IVA.IVABookWindow"},
                                    new MenuModel{ Name="Liquidazione IVA",  Uri="Ufp.Accounting.IVA.IVACloseWindow"},
                                    new MenuModel{ Name="Gestione LIPE",  Uri="Default.Accounting.IVA.TCOMLIQIVAView"},
                            }
                        },
                        new MenuModel{ Name= "Interrogazioni",
                            SubItems = new List<MenuModel>
                            {
                                    new MenuModel{ Name="Mastrini",  Uri="Default.Accounting.MastrinoView"},
                                    new MenuModel{ Name="E/C clienti|fornitori", Uri="Default.Accounting.ECView"},
                            }
                        },
                        new MenuModel{ Name= "Stampe",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name="Bilancino di verifica",  Uri="Default.Accounting.Reports.BalanceSimulationWindow"},
                                new MenuModel{ Name="Giornale generale", Uri="Default.Accounting.Reports.GeneralJournalWindow"},
                                new MenuModel{ Name="Mastrini",  Uri="Default.Accounting.Reports.MastrinoReportWindow"},
                                new MenuModel{ Name="Scadenziari clienti/fornitori",  Uri="Default.Accounting.Reports.ExpiresReportWindow"},
                                new MenuModel{ Name="Partitario clienti/fornitori",  Uri="Default.Accounting.Reports.BatchReportWindow"},
                                new MenuModel{ Name="Estrazione INTRA",  Uri="Default.Accounting.INTRAWindow"},
                            }
                        }
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "CRM",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Analisi di fattibilità", Uri= "Ufp.CRM.AF.ANAFAT_ROWView"},
                        new MenuModel{ Name= "Fatture", Uri= "Default.CRM.FATTT00FView" },
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Cespiti",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Schede cespiti", Uri= "Default.Accounting.Assets.ACC_ASSETS_CARDSView"},
                        new MenuModel{ Name= "Calcolo ammortamento", Uri= "Default.Accounting.Assets.AssetComputeWindow"},
                        new MenuModel{ Name= "Contabilizzazione cespiti", Uri= "Default.Accounting.Assets.AssetAccountingWindow"},
                        new MenuModel{ Name= "Aggiornamento cespiti", Uri= "Default.Accounting.Assets.AssetHistoryWindow"},

                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "DWH",
                    Uri = null,
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel{ Name= "Query", Uri= "Default.DWH.DWHQueryView"},
                        new MenuModel{ Name= "Visualizza", Uri= "Default.DWH.DWHView"},
                    }
                });
                retValue.Add(new MenuModel
                {
                    Name = "Tabelle",
                    SubItems = new List<MenuModel>
                    {
                        new MenuModel
                        {
                            Name = "Generiche",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel{ Name = "Lingue" , Uri = "Default.Tables.General.LINGUAView" },
                                new MenuModel{ Name = "Testi" , Uri = "Default.Tables.General.TAB_GEN_TEXTSView"},
                                new MenuModel{ Name = "Ruoli contatto" , Uri = "Default.Tables.General.TAB_GEN_CONTACTS_ROLESView" },
                                new MenuModel{ Name = "Tipi contatto" , Uri = "Default.Tables.General.TAB_GEN_CONTACTS_TYPESView"},
                                new MenuModel{ Name = "Tipi attività" , Uri = "Default.Tables.General.TAB_GEN_ACTIVITY_TYPESView"},
                            }
                        },
                        new MenuModel
                        {
                            Name = "Contabilità",
                            SubItems = new List<MenuModel>
                            {
                                 new MenuModel
                                 {
                                    Name = "Base",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "ABI/CAB" , Uri = "Default.Tables.Accounting.ABICABView" },
                                        new MenuModel{ Name = "Aliquote" , Uri = "Default.Tables.Accounting.AliquoteView" },
                                        new MenuModel{ Name = "Comuni" , Uri = "Default.Tables.Accounting.COMUNIView" },
                                        new MenuModel{ Name = "ISO" , Uri = "Default.Tables.Accounting.ISOView"},
                                        new MenuModel{ Name = "Nazioni" , Uri = "Default.Tables.Accounting.NAZIONIView"},
                                        new MenuModel{ Name = "Provincie" , Uri = "Default.Tables.Accounting.TAB_STATESView" },
                                        new MenuModel{ Name = "Valute" , Uri = "Default.Tables.Accounting.VALUTEView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Gestionali",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Affidabilità" , Uri = "Default.Tables.Accounting.AFFIDABILITAView" },
                                        new MenuModel{ Name = "Banche aziendali" , Uri = "Ufp.Tables.Accounting.BANAZIENView" },
                                        new MenuModel{ Name = "Cambi" , Uri = "Default.Tables.Accounting.CAMBIView" },
                                        new MenuModel{ Name = "Causali contabili" , Uri = "Default.Tables.Accounting.CAUCONTView" },
                                        new MenuModel{ Name = "Composizione prima nota" , Uri = "Ufp.Tables.Accounting.COMTIPREGView" },
                                        new MenuModel{ Name = "Centri di costo" , Uri = "Default.Tables.Accounting.TCECO00FView" },
                                        new MenuModel{ Name = "Codici di chiusura" , Uri = "Default.Tables.Accounting.TAB_ACC_CLOSINGView" },
                                        new MenuModel{ Name = "Esercizi contabili" , Uri = "Default.Tables.Accounting.ESERCIZIOView" },
                                        new MenuModel{ Name = "Incassi clienti" , Uri = "Default.Tables.Accounting.PAGCLIView" },
                                        new MenuModel{ Name = "Libri IVA" , Uri = "Ufp.Tables.Accounting.LIBRIIVAView" },
                                        new MenuModel{ Name = "Numeratori" , Uri = "Default.Tables.Accounting.NUMREGView" },
                                        new MenuModel{ Name = "Pagamenti fornitori" , Uri = "Default.Tables.Accounting.PAGFORView" },
                                        new MenuModel{ Name = "Ritenute" , Uri = "Default.Tables.Accounting.RITENUTEView" },
                                        new MenuModel{ Name = "Spostamento scadenze" , Uri = "Default.Tables.Accounting.SCADENZEView" },
                                        new MenuModel{ Name = "Tipi incasso" , Uri = "Ufp.Tables.Accounting.TAB_ACC_TIPINCView"},
                                        new MenuModel{ Name = "Tipi pagamento" , Uri = "Default.Tables.Accounting.TAB_ACC_TIPPAGView" },
                                        new MenuModel{ Name = "Tipi solleciti" , Uri = "Default.Tables.Accounting.SOLLECITIView"},
                                        new MenuModel{ Name = "Tipi codici LIPE" , Uri = "Ufp.Tables.Accounting.TCODLIQIVAView" },
                                        new MenuModel{ Name = "Tipi incassi" , Uri = "Ufp.Tables.Accounting.INCASSOView"},
                                        new MenuModel{ Name = "Tipi mandato" , Uri = "Ufp.Tables.Accounting.MANDATOView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Cespiti",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Aliquote cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_RATESView"},
                                        new MenuModel{ Name = "Categorie fiscali cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_CATEGORIESView" },
                                        new MenuModel{ Name = "Tipi cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_TYPESView" },
                                        new MenuModel{ Name = "Tipologie cespite" , Uri = "Default.Tables.Accounting.ACC_ASSETS_TYPOLOGIESView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Fatturazione elettronica",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Modalità di pagamento" , Uri = "Default.Tables.Accounting.FE_PAGDOCView"},
                                        new MenuModel{ Name = "Regimi fiscali" , Uri = "Default.Tables.Accounting.FE_RFIDOCView" },
                                        new MenuModel{ Name = "Tipi di cassa previdenziale" , Uri = "Default.Tables.Accounting.FE_TIPOCPView" },
                                        new MenuModel{ Name = "Tipi di documento" , Uri = "Default.Tables.Accounting.FE_TIPODOCView"},
                                        new MenuModel{ Name = "Tipi di natura IVA" , Uri = "Default.Tables.Accounting.FE_IVADOCView" },
                                        new MenuModel{ Name = "Tipi di ritenute" , Uri = "Default.Tables.Accounting.FE_TIPORITView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Classificazione geografica",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Aree" , Uri = "Ufp.Tables.Accounting.AREEView" },
                                        new MenuModel{ Name = "Regioni" , Uri = "Default.Tables.Accounting.REGIONIView" },
                                        new MenuModel{ Name = "Zone" , Uri = "Default.Tables.Accounting.ZONEView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Classificazione statistica",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Categorie clienti" , Uri = "Default.Tables.Accounting.CATEGORIAView" },
                                        new MenuModel{ Name = "Classificazione clienti" , Uri = "Default.Tables.Accounting.CLAZIONEView" },
                                        new MenuModel{ Name = "Filiali" , Uri = "Default.Tables.Accounting.FILIALIView"},
                                        new MenuModel{ Name = "Rivenditori" , Uri = "Default.Tables.Accounting.RIVENDITORIView"},
                                        new MenuModel{ Name = "Settori merceologici" , Uri = "Default.Tables.Accounting.MERCEOLOGICOView" },
                                    }
                                 },
                                 new MenuModel
                                 {
                                    Name = "Gestione commerciale",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Agenti" , Uri = "Ufp.Tables.Accounting.AGENTIView" },
                                        new MenuModel{ Name = "Consegna" , Uri = "Default.Tables.Accounting.CONSEGNAView"},
                                        new MenuModel{ Name = "Depositi" , Uri = "Default.Tables.Accounting.DEPOSITIView" },
                                        new MenuModel{ Name = "Imballi" , Uri = "Default.Tables.Accounting.IMBALLIView" },
                                        new MenuModel{ Name = "Spedizioni" , Uri = "Default.Tables.Accounting.SPEDIZIONEView"},
                                        new MenuModel{ Name = "Vettori" , Uri = "Ufp.Tables.Accounting.VETTORIView" },
                                    }
                                 }
                            }
                        },
                        new MenuModel
                        {
                            Name = "CRM",
                            SubItems = new List<MenuModel>
                            {
                                new MenuModel
                                 {
                                    Name = "Analisi fattibilità",
                                    SubItems = new List<MenuModel>
                                    {
                                        new MenuModel{ Name = "Parametri" , Uri = "Ufp.Tables.CRM.AF.ANAFAT_CONSTView"},
                                    }
                                 },
                            }
                        },
                    }
                });

                ExtractAndUpdateMenu(retValue, menus ?? new List<MenuModel>(), (accessCompany?.acsadmin ?? false), IsAdministratorView);

                return new ObservableCollection<MenuModel>(retValue);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return new ObservableCollection<MenuModel>();
            }
        }

        private void ExtractAndUpdateMenu(List<MenuModel> Menu, List<MenuModel> Abilitations, bool IsAdministrator, bool IsAdministratorView)
        {
            foreach (var menu in Menu)
            {
                if (!string.IsNullOrEmpty(menu.Uri))
                {
                    menu.IsEnabled = (IsAdministrator) ? true : (Abilitations.Where(o => o.Name == menu.Name && o.Uri == menu.Uri).Any() ? Abilitations.Where(o => o.Name == menu.Name && o.Uri == menu.Uri).Select(s => s.IsEnabled).FirstOrDefault() : false);
                }
                else
                {
                    if (!IsAdministratorView)
                        menu.IsEnabled = true;
                }


                if (menu.SubItems != null && menu.SubItems.Any())
                {
                    ExtractAndUpdateMenu(menu.SubItems, Abilitations, IsAdministrator, IsAdministratorView);
                }
            }
        }

        public ObservableCollection<MenuModel> GetBookmarks()
        {
            var fullPath = @$"{Path.GetTempPath()}.vulpesx\bookmarks.json";

            if (File.Exists(fullPath))
            {
                return JsonConvert.DeserializeObject<ObservableCollection<MenuModel>>(File.ReadAllText(fullPath)) ?? new();
            }
            return new ObservableCollection<MenuModel>();
        }

        public void UpdateBookmarks(ObservableCollection<MenuModel> Bookmarks)
        {
            string jsonString = JsonConvert.SerializeObject(Bookmarks[0].SubItems, Formatting.Indented);

            string folderPath = Path.Combine(Path.GetTempPath(), ".vulpesx");
            string filePath = Path.Combine(folderPath, "bookmarks.json");

            Directory.CreateDirectory(folderPath);

            File.WriteAllText(@$"{Path.GetTempPath()}.vulpesx\bookmarks.json", jsonString);
        }

        public bool UpdateMenuRole(string CompanyID, ACCESS User, string MenuJSON, AUTH_ACCESS_ROLES Role)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        var result = connection.ExecuteScalar("UPDATE ACCESOCE SET acsadmin = @IsAdmin, acsmenux = @MenuJSON OUTPUT INSERTED.rv  WHERE acssoc = @CompanyID AND acsute = @UserID", new { CompanyID, UserID = User.USRID, MenuJSON, IsAdmin = User.SelectedCompany?.AccessCompany?.acsadmin ?? false }, transaction);

                        if (result != null)
                        {
                            var authAccessRoleRepo = VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>();

                            if (authAccessRoleRepo.Exists(CompanyID, User.USRID))
                                result = connection.ExecuteScalar(authAccessRoleRepo.UPDATE_QUERY, Role, transaction);
                            else
                                result = connection.ExecuteScalar(authAccessRoleRepo.INSERT_QUERY, Role, transaction);

                            if (result != null)
                            {
                                transaction.Commit();

                                return true;
                            }
                            else
                            {
                                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                                return false;
                            }
                        }
                        else
                        {
                            ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                            return false;
                        }
                    }
                }
                else
                {
                    ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }
    }
}
