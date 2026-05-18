using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Accounting.Assets;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.Assets;
using VulpesX.DAL.Auth;
using VulpesX.DAL.CRM;
using VulpesX.DAL.CRM.AF;
using VulpesX.DAL.DWH;
using VulpesX.DAL.General;
using VulpesX.DAL.Logs;
using VulpesX.DAL.Production;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Accounting.Assets;
using VulpesX.DAL.Tables.Article;
using VulpesX.DAL.Tables.Assets;
using VulpesX.DAL.Tables.CRM;
using VulpesX.DAL.Tables.CRM.AF;
using VulpesX.DAL.Tables.CustomerRating;
using VulpesX.DAL.Tables.EnergyMonitor;
using VulpesX.DAL.Tables.General;
using VulpesX.DAL.Tables.Productions;
using VulpesX.DAL.Tables.Shipping;
using VulpesX.DAL.Treasury;
using VulpesX.Models;
using VulpesX.Services.Accounting;
using VulpesX.Services.Accounting.Assets;
using VulpesX.Services.Logs;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Services.Tables.CRM;
using VulpesX.Services.Tables.General;


namespace VulpesX.DAL
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddDALServices(this IServiceCollection services)
        {
            services.AddTransient<IConnectionFactory, SqlConnectionFactory>();
            services.AddTransient<DateTimeService>();

            #region Accounting
            #region Assets
            services.AddTransient<IACC_ASSETS_CARDSRepository, ACC_ASSETS_CARDSRepository>();
            services.AddTransient<IACC_ASSETS_DEP_CIV_HISTORYRepository, ACC_ASSETS_DEP_CIV_HISTORYRepository>();
            services.AddTransient<IACC_ASSETS_DEP_HISTORYRepository, ACC_ASSETS_DEP_HISTORYRepository>();
            #endregion

            #region eInvoice
            services.AddTransient<IACC_EINVOICE_CPRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_CPUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_CPRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_CPRepository>();
            services.AddTransient<ACC_EINVOICE_CPUfpRepository>();

            services.AddTransient<IACC_EINVOICE_DDTRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_DDTUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_DDTRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_DDTRepository>();
            services.AddTransient<ACC_EINVOICE_DDTUfpRepository>();

            services.AddTransient<IACC_EINVOICE_EXPIRESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_EXPIRESUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_EXPIRESRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_EXPIRESRepository>();
            services.AddTransient<ACC_EINVOICE_EXPIRESUfpRepository>();

            services.AddTransient<IACC_EINVOICE_HEADSRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_HEADSUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_HEADSRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_HEADSRepository>();
            services.AddTransient<ACC_EINVOICE_HEADSUfpRepository>();

            services.AddTransient<IACC_EINVOICE_LINKEDRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_LINKEDUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_LINKEDRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_LINKEDRepository>();
            services.AddTransient<ACC_EINVOICE_LINKEDUfpRepository>();

            services.AddTransient<IACC_EINVOICE_PORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_POUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_PORepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_PORepository>();
            services.AddTransient<ACC_EINVOICE_POUfpRepository>();

            services.AddTransient<IACC_EINVOICE_RITRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_RITUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_RITRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_RITRepository>();
            services.AddTransient<ACC_EINVOICE_RITUfpRepository>();

            services.AddTransient<IACC_EINVOICE_ROWS_PIDRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_ROWS_PIDUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_ROWS_PIDRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_ROWS_PIDRepository>();
            services.AddTransient<ACC_EINVOICE_ROWS_PIDUfpRepository>();

            services.AddTransient<IACC_EINVOICE_ROWS_SMRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_ROWS_SMUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_ROWS_SMRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_ROWS_SMRepository>();
            services.AddTransient<ACC_EINVOICE_ROWS_SMUfpRepository>();

            services.AddTransient<IACC_EINVOICE_ROWSRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_ROWSUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_ROWSRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_ROWSRepository>();
            services.AddTransient<ACC_EINVOICE_ROWSUfpRepository>();

            services.AddTransient<IACC_EINVOICE_SMRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_SMUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_SMRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_SMRepository>();
            services.AddTransient<ACC_EINVOICE_SMUfpRepository>();

            services.AddTransient<IACC_EINVOICE_VATRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_EINVOICE_VATUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_EINVOICE_VATRepository>();
                }
            });
            services.AddTransient<ACC_EINVOICE_VATRepository>();
            services.AddTransient<ACC_EINVOICE_VATUfpRepository>();
            #endregion

            services.AddTransient<IACC_PLAFOND_PARMSRepository, ACC_PLAFOND_PARMSRepository>();

            services.AddTransient<IACC_PLAFOND_ROWSRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_PLAFOND_ROWSUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_PLAFOND_ROWSRepository>();
                }
            });
            services.AddTransient<ACC_PLAFOND_ROWSRepository>();
            services.AddTransient<ACC_PLAFOND_ROWSUfpRepository>();

            services.AddTransient<IACC_PLAFONDRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_PLAFONDUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_PLAFONDRepository>();
                }
            });
            services.AddTransient<ACC_PLAFONDRepository>();
            services.AddTransient<ACC_PLAFONDUfpRepository>();

            services.AddTransient<IAccountingRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AccountingUfpRepository>();
                    default:
                        return sp.GetRequiredService<AccountingRepository>();
                }
            });
            services.AddTransient<AccountingRepository>();
            services.AddTransient<AccountingUfpRepository>();

            services.AddTransient<IMPDETTAGLIORepository, MPDETTAGLIORepository>();
            services.AddTransient<IMPTESTATARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<MPTESTATAUfpRepository>();
                    default:
                        return sp.GetRequiredService<MPTESTATARepository>();
                }
            });
            services.AddTransient<MPTESTATARepository>();
            services.AddTransient<MPTESTATAUfpRepository>();

            services.AddTransient<IPDCANNIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCANNIUfpRepository>();
                    default:
                        return sp.GetRequiredService<PDCANNIRepository>();
                }
            });
            services.AddTransient<PDCANNIRepository>();
            services.AddTransient<PDCANNIUfpRepository>();

            services.AddTransient<IPDCCONTIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCCONTIUfpRepository>();
                    default:
                        return sp.GetRequiredService<PDCCONTIRepository>();
                }
            });
            services.AddTransient<PDCCONTIRepository>();
            services.AddTransient<PDCCONTIUfpRepository>();

            services.AddTransient<IPDCGRUPPIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCGRUPPIUfpRepository>();
                    default:
                        return sp.GetRequiredService<PDCGRUPPIRepository>();
                }
            });
            services.AddTransient<PDCGRUPPIRepository>();
            services.AddTransient<PDCGRUPPIUfpRepository>();

            services.AddTransient<IPDCSOTTORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PDCSOTTOUfpRepository>();
                    default:
                        return sp.GetRequiredService<PDCSOTTORepository>();
                }
            });
            services.AddTransient<PDCSOTTORepository>();
            services.AddTransient<PDCSOTTOUfpRepository>();

            services.AddTransient<IPNCLIENTIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNCLIENTIUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNCLIENTIRepository>();
                }
            });
            services.AddTransient<PNCLIENTIRepository>();
            services.AddTransient<PNCLIENTIUfpRepository>();

            services.AddTransient<IPNFORNITORIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNFORNITORIUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNFORNITORIRepository>();
                }
            });
            services.AddTransient<PNFORNITORIRepository>();
            services.AddTransient<PNFORNITORIUfpRepository>();

            services.AddTransient<IPNIVARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNIVAUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNIVARepository>();
                }
            });
            services.AddTransient<PNIVARepository>();
            services.AddTransient<PNIVAUfpRepository>();

            services.AddTransient<IPNPORTAFOGLIO_DISTRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNPORTAFOGLIO_DISTUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNPORTAFOGLIO_DISTRepository>();
                }
            });
            services.AddTransient<PNPORTAFOGLIO_DISTRepository>();
            services.AddTransient<PNPORTAFOGLIO_DISTUfpRepository>();

            services.AddTransient<IPNPORTAFOGLIORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNPORTAFOGLIOUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNPORTAFOGLIORepository>();
                }
            });
            services.AddTransient<PNPORTAFOGLIORepository>();
            services.AddTransient<PNPORTAFOGLIOUfpRepository>();

            services.AddTransient<IPNRIGHERepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNRIGHEUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNRIGHERepository>();
                }
            });
            services.AddTransient<PNRIGHERepository>();
            services.AddTransient<PNRIGHEUfpRepository>();

            services.AddTransient<IPNTESTATARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PNTESTATAUfpRepository>();
                    default:
                        return sp.GetRequiredService<PNTESTATARepository>();
                }
            });
            services.AddTransient<PNTESTATARepository>();
            services.AddTransient<PNTESTATAUfpRepository>();

            services.AddTransient<ISOLLE0FRepository, SOLLE0FRepository>();
            services.AddTransient<ISTATE00FRepository, STATE00FRepository>();
            services.AddTransient<ITABSALDI1Repository, TABSALDI1Repository>();
            services.AddTransient<ITABSALDIRepository, TABSALDIRepository>();

            services.AddTransient<ITCOMLIQIVARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TCOMLIQIVAUfpRepository>();
                    default:
                        return sp.GetRequiredService<TCOMLIQIVARepository>();
                }
            });
            services.AddTransient<TCOMLIQIVARepository>();
            services.AddTransient<TCOMLIQIVAUfpRepository>();

            #endregion

            #region Assets
            services.AddTransient<IASSED00FRepository, ASSED00FRepository>();
            services.AddTransient<IASSET00FRepository, ASSET00FRepository>();
            services.AddTransient<IASSETAL00FRepository, ASSETAL00FRepository>();
            services.AddTransient<IASSETCO00FRepository, ASSETCO00FRepository>();
            services.AddTransient<IASSETCODET00FRepository, ASSETCODET00FRepository>();
            #endregion

            #region Auth
            services.AddTransient<IAUTH_ACCESS_ROLESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AUTH_ACCESS_ROLESUfpRepository>();
                    default:
                        return sp.GetRequiredService<AUTH_ACCESS_ROLESRepository>();
                }
            });
            services.AddTransient<AUTH_ACCESS_ROLESRepository>();
            services.AddTransient<AUTH_ACCESS_ROLESUfpRepository>();

            services.AddTransient<IAuthRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AuthUfpRepository>();
                    default:
                        return sp.GetRequiredService<AuthDefaultRepository>();
                }
            });
            services.AddTransient<AuthDefaultRepository>();
            services.AddTransient<AuthUfpRepository>();

            services.AddTransient<ICompanyRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CompanyUfpRepository>();
                    default:
                        return sp.GetRequiredService<CompanyRepository>();
                }
            });
            services.AddTransient<CompanyRepository>();
            services.AddTransient<CompanyUfpRepository>();

            services.AddTransient<IMenuRepository>(sp  =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<MenuUfpRepository>();
                    default:
                        return sp.GetRequiredService<MenuRepository>();
                }
            });
            services.AddTransient<MenuRepository>();
            services.AddTransient<MenuUfpRepository>();
            #endregion

            #region CRM
            #region AF
            services.AddTransient<IANAFAT_ROWRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ANAFAT_ROWUfpRepository>();
                    default:
                        return sp.GetRequiredService<ANAFAT_ROWRepository>();
                }
            });
            services.AddTransient<ANAFAT_ROWRepository>();
            services.AddTransient<ANAFAT_ROWUfpRepository>();
            #endregion
            services.AddTransient<IANFAD00FRepository, ANFAD00FRepository>();
            services.AddTransient<IANFADD00FRepository, ANFADD00FRepository>();
            services.AddTransient<IANFAT00FRepository, ANFAT00FRepository>();

            services.AddTransient<ICRM_LISCLIRepository, CRM_LISCLIRepository>();
            services.AddTransient<ICRM_LISGENRepository, CRM_LISGENRepository>();

            services.AddTransient<IFATTAL00FRepository, FATTAL00FRepository>();
            services.AddTransient<IFATTAUTRepository, FATTAUTRepository>();
            services.AddTransient<IFATTD00FRepository, FATTD00FRepository>();

            services.AddTransient<IFATTD00FRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FATTD00FUfpRepository>();
                    default:
                        return sp.GetRequiredService<FATTD00FRepository>();
                }
            });
            services.AddTransient<FATTD00FRepository>();
            services.AddTransient<FATTD00FUfpRepository>();

            services.AddTransient<IFATTPERSTXTRepository, FATTPERSTXTRepository>();

            services.AddTransient<IFATTT00FRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FATTT00FUfpRepository>();
                    default:
                        return sp.GetRequiredService<FATTT00FRepository>();
                }
            });
            services.AddTransient<FATTT00FRepository>();
            services.AddTransient<FATTT00FUfpRepository>();

            services.AddTransient<IOFFED00FRepository, OFFED00FRepository>();
            services.AddTransient<IOFFET00FRepository, OFFET00FRepository>();
            services.AddTransient<IOFFETAL00FRepository, OFFETAL00FRepository>();

            services.AddTransient<IORDID00FRepository, ORDID00FRepository>();
            services.AddTransient<IORDIT00FRepository, ORDIT00FRepository>();
            services.AddTransient<IORDITAL00FRepository, ORDITAL00FRepository>();
            services.AddTransient<IORDITALERepository, ORDITALERepository>();

            services.AddTransient<ITEMPI_MEDI_VISTARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TEMPI_MEDI_VISTAUfpRepository>();
                    default:
                        return sp.GetRequiredService<TEMPI_MEDI_VISTARepository>();
                }
            });
            services.AddTransient<TEMPI_MEDI_VISTARepository>();
            services.AddTransient<TEMPI_MEDI_VISTAUfpRepository>();

            services.AddTransient<ITFATTT00FLEVEL1Repository, TFATTT00FLEVEL1Repository>();
            #endregion

            #region DWH
            services.AddTransient<Idwh_folderRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<dwh_folderUfpRepository>();
                    default:
                        return sp.GetRequiredService<dwh_folderRepository>();
                }
            });
            services.AddTransient<dwh_folderRepository>();
            services.AddTransient<dwh_folderUfpRepository>();

            services.AddTransient<Idwh_queryRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<dwh_queryUfpRepository>();
                    default:
                        return sp.GetRequiredService<dwh_queryRepository>();
                }
            });
            services.AddTransient<dwh_queryRepository>();
            services.AddTransient<dwh_queryUfpRepository>();

            services.AddTransient<Idwh_templateRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<dwh_templateUfpRepository>();
                    default:
                        return sp.GetRequiredService<dwh_templateRepository>();
                }
            });
            services.AddTransient<dwh_templateRepository>();
            services.AddTransient<dwh_templateUfpRepository>();
            #endregion

            #region General
            services.AddTransient<IABE_EXTERN_DESTRepository, ABE_EXTERN_DESTRepository>();
            services.AddTransient<IABE_EXTERNRepository, ABE_EXTERNRepository>();

            services.AddTransient<IABERepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ABEUfpRepository>();
                    default:
                        return sp.GetRequiredService<ABERepository>();
                }
            });
            services.AddTransient<ABERepository>();
            services.AddTransient<ABEUfpRepository>();

            services.AddTransient<IANACERTLEVEL1Repository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ANACERTLEVEL1UfpRepository>();
                    default:
                        return sp.GetRequiredService<ANACERTLEVEL1Repository>();
                }
            });
            services.AddTransient<ANACERTLEVEL1Repository>();
            services.AddTransient<ANACERTLEVEL1UfpRepository>();

            services.AddTransient<IANACERTRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ANACERTUfpRepository>();
                    default:
                        return sp.GetRequiredService<ANACERTRepository>();
                }
            });
            services.AddTransient<ANACERTRepository>();
            services.AddTransient<ANACERTUfpRepository>();

            services.AddTransient<IANDEFRESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ANDEFRESUfpRepository>();
                    default:
                        return sp.GetRequiredService<ANDEFRESRepository>();
                }
            });
            services.AddTransient<ANDEFRESRepository>();
            services.AddTransient<ANDEFRESUfpRepository>();

            services.AddTransient<IAZIENDA_LINGUARepository, AZIENDA_LINGUARepository>();

            services.AddTransient<IAZIENDARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AZIENDAUfpRepository>();
                    default:
                        return sp.GetRequiredService<AZIENDARepository>();
                }
            });
            services.AddTransient<AZIENDARepository>();
            services.AddTransient<AZIENDAUfpRepository>();

            services.AddTransient<ICLIAMMIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CLIAMMIUfpRepository>();
                    default:
                        return sp.GetRequiredService<CLIAMMIRepository>();
                }
            });
            services.AddTransient<CLIAMMIRepository>();
            services.AddTransient<CLIAMMIUfpRepository>();

            services.AddTransient<ICLIENTIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CLIENTIUfpRepository>();
                    default:
                        return sp.GetRequiredService<CLIENTIRepository>();
                }
            });
            services.AddTransient<CLIENTIRepository>();
            services.AddTransient<CLIENTIUfpRepository>();

            services.AddTransient<IDESFORRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<DESFORUfpRepository>();
                    default:
                        return sp.GetRequiredService<DESFORRepository>();
                }
            });
            services.AddTransient<DESFORRepository>();
            services.AddTransient<DESFORUfpRepository>();

            services.AddTransient<IDESTINATARIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<DESTINATARIUfpRepository>();
                    default:
                        return sp.GetRequiredService<DESTINATARIRepository>();
                }
            });
            services.AddTransient<DESTINATARIRepository>();
            services.AddTransient<DESTINATARIUfpRepository>();

            services.AddTransient<IFORNAMMIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FORNAMMIUfpRepository>();
                    default:
                        return sp.GetRequiredService<FORNAMMIRepository>();
                }
            });
            services.AddTransient<FORNAMMIRepository>();
            services.AddTransient<FORNAMMIUfpRepository>();

            services.AddTransient<IFORNITORIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FORNITORIUfpRepository>();
                    default:
                        return sp.GetRequiredService<FORNITORIRepository>();
                }
            });
            services.AddTransient<FORNITORIRepository>();
            services.AddTransient<FORNITORIUfpRepository>();

            services.AddTransient<INOTECLI1Repository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<NOTECLI1UfpRepository>();
                    default:
                        return sp.GetRequiredService<NOTECLI1Repository>();
                }
            });
            services.AddTransient<NOTECLI1Repository>();
            services.AddTransient<NOTECLI1UfpRepository>();

            services.AddTransient<INOTEFORRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<NOTEFORUfpRepository>();
                    default:
                        return sp.GetRequiredService<NOTEFORRepository>();
                }
            });
            services.AddTransient<NOTEFORRepository>();
            services.AddTransient<NOTEFORUfpRepository>();

            services.AddTransient<IPERSBOLLRepository, PERSBOLLUfpRepository>();

            services.AddTransient<IRFFTB00FRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<RFFTB00FUfpRepository>();
                    default:
                        return sp.GetRequiredService<RFFTB00FRepository>();
                }
            });
            services.AddTransient<RFFTB00FRepository>();
            services.AddTransient<RFFTB00FUfpRepository>();

            services.AddTransient<ISCADCLIService, SCADCLIUfpService>();

            services.AddTransient<Itab_articolo_composizioneRepository, tab_articolo_composizioneRepository>();
            services.AddTransient<Itab_articolo_externRepository, tab_articolo_externRepository>();
            services.AddTransient<Itab_articolo_linguaRepository, tab_articolo_linguaRepository>();
            services.AddTransient<Itab_articolo_tipoRepository, tab_articolo_tipoRepository>();
            services.AddTransient<Itab_articolo_unitaRepository, tab_articolo_unitaRepository>();

            services.AddTransient<Itab_articoloRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<tab_articoloUfpRepository>();
                    default:
                        return sp.GetRequiredService<tab_articoloRepository>();
                }
            });
            services.AddTransient<tab_articoloRepository>();
            services.AddTransient<tab_articoloUfpRepository>();
            #endregion

            #region Logs
            services.AddTransient<Ilog_crashRepository, log_crashRepository>();
            services.AddTransient<Ilog_crm_sendRepository, log_crm_sendRepository>();
            services.AddTransient<Ilog_gen_sendRepository, log_gen_sendRepository>();
            services.AddTransient<Ilog_srm_sendRepository, log_srm_sendRepository>();
            #endregion

            #region Production
            services.AddTransient<Ipro_ordine_composizione_lottoRepository, pro_ordine_composizione_lottoRepository>();

            services.AddTransient<Ipro_ordine_composizione_tempoRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<pro_ordine_composizione_tempoUfpRepository>();
                    default:
                        return sp.GetRequiredService<pro_ordine_composizione_tempoRepository>();
                }
            });
            services.AddTransient<pro_ordine_composizione_tempoRepository>();
            services.AddTransient<pro_ordine_composizione_tempoUfpRepository>();

            services.AddTransient<Ipro_ordine_composizioneRepository, pro_ordine_composizioneRepository>();
            services.AddTransient<Ipro_ordine_historyRepository, pro_ordine_historyRepository>();
            services.AddTransient<Ipro_ordine_lottiRepository, pro_ordine_lottiRepository>();
            services.AddTransient<Ipro_ordineRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<pro_ordineUfpRepository>();
                    default:
                        return sp.GetRequiredService<pro_ordineRepository>();
                }
            });
            services.AddTransient<pro_ordineRepository>();
            services.AddTransient<pro_ordineUfpRepository>();
            #endregion

            #region Shipping
            services.AddTransient<IBOLLD00F_historyRepository, BOLLD00F_historyRepository>();
            services.AddTransient<IBOLLD00F1_historyRepository, BOLLD00F1_historyRepository>();
            services.AddTransient<IBOLLD00F1Repository, BOLLD00F1Repository>();
            services.AddTransient<IBOLLD00FRepository, BOLLD00FRepository>();
            services.AddTransient<IBOLLT00F_historyRepository, BOLLT00F_historyRepository>();
            services.AddTransient<IBOLLT00FRepository, BOLLT00FRepository>();
            #endregion

            #region SRM
            services.AddTransient<Iacq_goods_receiptsRepository, acq_goods_receiptsRepository>();
            services.AddTransient<Iacq_orders_heads_attachmentsRepository, acq_orders_heads_attachmentsRepository>();
            services.AddTransient<Iacq_orders_headsRepository, acq_orders_headsRepository>();
            services.AddTransient<Iacq_orders_rows_customer_ordersRepository, acq_orders_rows_customer_ordersRepository>();
            services.AddTransient<Iacq_orders_rows_jobsRepository, acq_orders_rows_jobsRepository>();
            services.AddTransient<Iacq_orders_rows_rdasRepository, acq_orders_rows_rdasRepository>();
            services.AddTransient<Iacq_orders_rowsRepository, acq_orders_rowsRepository>();
            services.AddTransient<ICAUSVENService, CAUSVENService>();

            services.AddTransient<ISRM_LISFORRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<SRM_LISFORUfpRepository>();
                    default:
                        return sp.GetRequiredService<SRM_LISFORRepository>();
                }
            });
            services.AddTransient<SRM_LISFORRepository>();
            services.AddTransient<SRM_LISFORUfpRepository>();

            services.AddTransient<ISRM_RDARepository, SRM_RDARepository>();
            services.AddTransient<Itab_articolo_costiRepository, tab_articolo_costiRepository>();
            #endregion

            #region Store
            services.AddTransient<ISTORE_CAUSALSRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<STORE_CAUSALSUfpRepository>();
                    default:
                        return sp.GetRequiredService<STORE_CAUSALSRepository>();
                }
            });
            services.AddTransient<STORE_CAUSALSRepository>();
            services.AddTransient<STORE_CAUSALSUfpRepository>();

            services.AddTransient<Istore_movements_historyRepository, store_movements_historyRepository>();
            services.AddTransient<Istore_movementsRepository, store_movementsRepository>();
            services.AddTransient<Istore_stocks_engageRepository, store_stocks_engageRepository>();
            services.AddTransient<Istore_stocks_lotsRepository, store_stocks_lotsRepository>();
            services.AddTransient<Istore_stocksRepository, store_stocksRepository>();

            services.AddTransient<Istore_storesRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<store_storesUfpRepository>();
                    default:
                        return sp.GetRequiredService<store_storesRepository>();
                }
            });
            services.AddTransient<store_storesRepository>();
            services.AddTransient<store_storesUfpRepository>();

            #endregion

            #region Tables
            #region Accounting
            #region Assets
            services.AddTransient<IACC_ASSETS_CATEGORIESRepository, ACC_ASSETS_CATEGORIESRepository>();

            services.AddTransient<IACC_ASSETS_RATESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_ASSETS_RATESUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_ASSETS_RATESRepository>();
                }
            });
            services.AddTransient<ACC_ASSETS_RATESRepository>();
            services.AddTransient<ACC_ASSETS_RATESUfpRepository>();

            services.AddTransient<IACC_ASSETS_TYPESRepository, ACC_ASSETS_TYPESRepository>();

            services.AddTransient<IACC_ASSETS_TYPOLOGIESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ACC_ASSETS_TYPOLOGIESUfpRepository>();
                    default:
                        return sp.GetRequiredService<ACC_ASSETS_TYPOLOGIESRepository>();
                }
            });
            services.AddTransient<ACC_ASSETS_TYPOLOGIESRepository>();
            services.AddTransient<ACC_ASSETS_TYPOLOGIESUfpRepository>();
            #endregion

            services.AddTransient<IABICABRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ABICABUfpRepository>();
                    default:
                        return sp.GetRequiredService<ABICABRepository>();
                }
            });
            services.AddTransient<ABICABRepository>();
            services.AddTransient<ABICABUfpRepository>();

            services.AddTransient<IAFFIDABILITARepository, AFFIDABILITARepository>();

            services.AddTransient<IAGENTIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AGENTIUfpRepository>();
                    default:
                        return sp.GetRequiredService<AGENTIRepository>();
                }
            });
            services.AddTransient<AGENTIRepository>();
            services.AddTransient<AGENTIUfpRepository>();

            services.AddTransient<IAliquoteRepository, AliquoteRepository>();

            services.AddTransient<IAREERepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<AREEUfpRepository>();
                    default:
                        return sp.GetRequiredService<AREERepository>();
                }
            });
            services.AddTransient<AREERepository>();
            services.AddTransient<AREEUfpRepository>();

            services.AddTransient<IBANAZIENRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<BANAZIENUfpRepository>();
                    default:
                        return sp.GetRequiredService<BANAZIENRepository>();
                }
            });
            services.AddTransient<BANAZIENRepository>();
            services.AddTransient<BANAZIENUfpRepository>();

            services.AddTransient<ICAMBIRepository, CAMBIRepository>();
            services.AddTransient<ICATEGORIARepository, CATEGORIARepository>();

            services.AddTransient<ICAUCONT_GROUPSRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CAUCONT_GROUPSUfpRepository>();
                    default:
                        return sp.GetRequiredService<CAUCONT_GROUPSRepository>();
                }
            });
            services.AddTransient<CAUCONT_GROUPSRepository>();
            services.AddTransient<CAUCONT_GROUPSUfpRepository>();
            services.AddTransient<ICAUCONTRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CAUCONTUfpRepository>();
                    default:
                        return sp.GetRequiredService<CAUCONTRepository>();
                }
            });
            services.AddTransient<CAUCONTRepository>();
            services.AddTransient<CAUCONTUfpRepository>();

            services.AddTransient<ICLAZIONERepository, CLAZIONERepository>();

            services.AddTransient<ICODCHIUSURARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<CODCHIUSURAUfpRepository>();
                    default:
                        return sp.GetRequiredService<CODCHIUSURARepository>();
                }
            });
            services.AddTransient<CODCHIUSURARepository>();
            services.AddTransient<CODCHIUSURAUfpRepository>();

            services.AddTransient<ICOMTIPREGUfpRepository, COMTIPREGUfpRepository>();

            services.AddTransient<ICOMUNIRepository, COMUNIRepository>();
            services.AddTransient<ICONSEGNA_LINGUARepository, CONSEGNA_LINGUARepository>();
            services.AddTransient<ICONSEGNARepository, CONSEGNARepository>();
            services.AddTransient<ICUSTOMER_GROUPSRepository, CUSTOMER_GROUPSRepository>();
            services.AddTransient<IDEPOSITIRepository, DEPOSITIRepository>();

            services.AddTransient<IESERCIZIORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ESERCIZIOUfpRepository>();
                    default:
                        return sp.GetRequiredService<ESERCIZIORepository>();
                }
            });
            services.AddTransient<ESERCIZIORepository>();
            services.AddTransient<ESERCIZIOUfpRepository>();

            services.AddTransient<IFE_IVADOCRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FE_IVADOCUfpRepository>();
                    default:
                        return sp.GetRequiredService<FE_IVADOCRepository>();
                }
            });
            services.AddTransient<FE_IVADOCRepository>();
            services.AddTransient<FE_IVADOCUfpRepository>();

            services.AddTransient<IFE_PAGDOCRepository, FE_PAGDOCRepository>();
            services.AddTransient<IFE_RFIDOCRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FE_RFIDOCUfpRepository>();
                    default:
                        return sp.GetRequiredService<FE_RFIDOCRepository>();
                }
            });
            services.AddTransient<FE_RFIDOCRepository>();
            services.AddTransient<FE_RFIDOCUfpRepository>();

            services.AddTransient<IFE_TIPOCPRepository, FE_TIPOCPRepository>();
            services.AddTransient<IFE_TIPODOCRepository, FE_TIPODOCRepository>();

            services.AddTransient<IFE_TIPORITRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FE_TIPORITUfpRepository>();
                    default:
                        return sp.GetRequiredService<FE_TIPORITRepository>();
                }
            });
            services.AddTransient<FE_TIPORITRepository>();
            services.AddTransient<FE_TIPORITUfpRepository>();


            services.AddTransient<IFILIALIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<FILIALIUfpRepository>();
                    default:
                        return sp.GetRequiredService<FILIALIRepository>();
                }
            });
            services.AddTransient<FILIALIRepository>();
            services.AddTransient<FILIALIUfpRepository>();

            services.AddTransient<IIMBALLIRepository, IMBALLIRepository>();

            services.AddTransient<IINCASSORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<INCASSOUfpRepository>();
                    default:
                        return sp.GetRequiredService<INCASSORepository>();
                }
            });
            services.AddTransient<INCASSORepository>();
            services.AddTransient<INCASSOUfpRepository>();

            services.AddTransient<IISORepository, ISORepository>();

            services.AddTransient<ILIBRIIVARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<LIBRIIVAUfpRepository>();
                    default:
                        return sp.GetRequiredService<LIBRIIVARepository>();
                }
            });
            services.AddTransient<LIBRIIVARepository>();
            services.AddTransient<LIBRIIVAUfpRepository>();

            services.AddTransient<IMANDATORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<MANDATOUfpRepository>();
                    default:
                        return sp.GetRequiredService<MANDATORepository>();
                }
            });
            services.AddTransient<MANDATORepository>();
            services.AddTransient<MANDATOUfpRepository>();

            services.AddTransient<IMERCEOLOGICORepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<MERCEOLOGICOUfpRepository>();
                    default:
                        return sp.GetRequiredService<MERCEOLOGICORepository>();
                }
            });
            services.AddTransient<MERCEOLOGICORepository>();
            services.AddTransient<MERCEOLOGICOUfpRepository>();

            services.AddTransient<INAZIONIRepository, NAZIONIRepository>();
            services.AddTransient<INUMREGRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<NUMREGUfpRepository>();
                    default:
                        return sp.GetRequiredService<NUMREGRepository>();
                }
            });
            services.AddTransient<NUMREGRepository>();
            services.AddTransient<NUMREGUfpRepository>();

            services.AddTransient<IPAGCLI_LINGUARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PAGCLI_LINGUAUfpRepository>();
                    default:
                        return sp.GetRequiredService<PAGCLI_LINGUARepository>();
                }
            });
            services.AddTransient<PAGCLI_LINGUARepository>();
            services.AddTransient<PAGCLI_LINGUAUfpRepository>();

            services.AddTransient<IPAGCLIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PAGCLIUfpRepository>();
                    default:
                        return sp.GetRequiredService<PAGCLIRepository>();
                }
            });
            services.AddTransient<PAGCLIRepository>();
            services.AddTransient<PAGCLIUfpRepository>();

            services.AddTransient<IPAGFORRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<PAGFORUfpRepository>();
                    default:
                        return sp.GetRequiredService<PAGFORRepository>();
                }
            });
            services.AddTransient<PAGFORRepository>();
            services.AddTransient<PAGFORUfpRepository>();

            services.AddTransient<IPNCEEBILRepository, PNCEEBILRepository>();
            services.AddTransient<IREGIONIRepository, REGIONIRepository>();
            services.AddTransient<IRITENUTERepository, RITENUTERepository>();
            services.AddTransient<IRIVENDITORIRepository, RIVENDITORIRepository>();
            services.AddTransient<ISCADENZERepository, SCADENZERepository>();
            services.AddTransient<ISOCIETARepository, SOCIETARepository>();
            services.AddTransient<ISOLLECITIRepository, SOLLECITIRepository>();
            services.AddTransient<ISPEDIZIONE_LINGUARepository, SPEDIZIONE_LINGUARepository>();
            services.AddTransient<ISPEDIZIONERepository, SPEDIZIONERepository>();

            services.AddTransient<ISUPPLIER_GROUPSRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<SUPPLIER_GROUPSUfpRepository>();
                    default:
                        return sp.GetRequiredService<SUPPLIER_GROUPSRepository>();
                }
            });
            services.AddTransient<SUPPLIER_GROUPSRepository>();
            services.AddTransient<SUPPLIER_GROUPSUfpRepository>();

            services.AddTransient<ITAB_ACC_CLOSINGRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_ACC_CLOSINGUfpRepository>();
                    default:
                        return sp.GetRequiredService<TAB_ACC_CLOSINGRepository>();
                }
            });
            services.AddTransient<TAB_ACC_CLOSINGRepository>();
            services.AddTransient<TAB_ACC_CLOSINGUfpRepository>();

            services.AddTransient<ITAB_ACC_TIPINCRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_ACC_TIPINCUfpRepository>();
                    default:
                        return sp.GetRequiredService<TAB_ACC_TIPINCRepository>();
                }
            });
            services.AddTransient<TAB_ACC_TIPINCRepository>();
            services.AddTransient<TAB_ACC_TIPINCUfpRepository>();

            services.AddTransient<ITAB_ACC_TIPPAGRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_ACC_TIPPAGUfpRepository>();
                    default:
                        return sp.GetRequiredService<TAB_ACC_TIPPAGRepository>();
                }
            });
            services.AddTransient<TAB_ACC_TIPPAGRepository>();
            services.AddTransient<TAB_ACC_TIPPAGUfpRepository>();

            services.AddTransient<ITAB_STATESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_STATESUfpRepository>();
                    default:
                        return sp.GetRequiredService<TAB_STATESRepository>();
                }
            });
            services.AddTransient<TAB_STATESRepository>();
            services.AddTransient<TAB_STATESUfpRepository>();

            services.AddTransient<ITASSIRepository, TASSIRepository>();

            services.AddTransient<ITCECO00FRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TCECO00FUfpRepository>();
                    default:
                        return sp.GetRequiredService<TCECO00FRepository>();
                }
            });
            services.AddTransient<TCECO00FRepository>();
            services.AddTransient<TCECO00FUfpRepository>();

            services.AddTransient<ITCODLIQIVARepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TCODLIQIVAUfpRepository>();
                    default:
                        return sp.GetRequiredService<TCODLIQIVARepository>();
                }
            });
            services.AddTransient<TCODLIQIVARepository>();
            services.AddTransient<TCODLIQIVAUfpRepository>();

            services.AddTransient<IVALUTERepository, VALUTERepository>();

            services.AddTransient<IVETTORIRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<VETTORIUfpRepository>();
                    default:
                        return sp.GetRequiredService<VETTORIRepository>();
                }
            });
            services.AddTransient<VETTORIRepository>();
            services.AddTransient<VETTORIUfpRepository>();

            services.AddTransient<IZONERepository, ZONERepository>();
            #endregion

            #region Article
            services.AddTransient<ICategoriaRepository, CategoriaRepository>();
            services.AddTransient<IUnitaRepository, UnitaRepository>();
            #endregion

            #region Assets
            services.AddTransient<ITAB_AST_LOCATIONRepository, TAB_AST_LOCATIONRepository>();
            #endregion

            #region CRM
            #region AF
            services.AddTransient<IANAFAT_CONSTRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ANAFAT_CONSTUfpRepository>();
                    default:
                        return sp.GetRequiredService<ANAFAT_CONSTRepository>();
                }
            });
            services.AddTransient<ANAFAT_CONSTRepository>();
            services.AddTransient<ANAFAT_CONSTUfpRepository>();

            services.AddTransient<IANAFAT_PIECESRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<ANAFAT_PIECESUfpRepository>();
                    default:
                        return sp.GetRequiredService<ANAFAT_PIECESRepository>();
                }
            });
            services.AddTransient<ANAFAT_PIECESRepository>();
            services.AddTransient<ANAFAT_PIECESUfpRepository>();
            #endregion
            services.AddTransient<ICAUFAT00FRepository, CAUFAT00FRepository>();
            services.AddTransient<ITAB_CRM_CAUOFFCLORepository, TAB_CRM_CAUOFFCLORepository>();
            services.AddTransient<ITAB_CRM_CAUOFFRepository, TAB_CRM_CAUOFFRepository>();

            services.AddTransient<ITAB_CRM_CAUORDRepository>(sp =>
            {
                switch (UserContext.Instance.Domain)
                {
                    case (Constants.UFP_DOMAIN):
                        return sp.GetRequiredService<TAB_CRM_CAUORDUfpRepository>();
                    default:
                        return sp.GetRequiredService<TAB_CRM_CAUORDRepository>();
                }
            });
            services.AddTransient<TAB_CRM_CAUORDRepository>();
            services.AddTransient<TAB_CRM_CAUORDUfpRepository>();

            services.AddTransient<ITAB_CRM_FEASABILITYRepository, TAB_CRM_FEASABILITYRepository>();
            #endregion

            #region CustomerRating
            services.AddTransient<Icr_tab_elementsRepository, cr_tab_elementsRepository>();
            services.AddTransient<Icr_tab_points_financialRepository, cr_tab_points_financialRepository>();
            services.AddTransient<Icr_tab_ratingsRepository, cr_tab_ratingsRepository>();
            #endregion

            #region EnergyMonitor
            services.AddTransient<IEM_DEVICERepository, EM_DEVICERepository>();
            services.AddTransient<IEM_DEVICE_PERIODRepository, EM_DEVICE_PERIODRepository>();
            #endregion

            #region General
            services.AddTransient<ILINGUARepository, LINGUARepository>();
            services.AddTransient<ITAB_GEN_ACTIVITY_TYPESRepository, TAB_GEN_ACTIVITY_TYPESRepository>();
            services.AddTransient<ITAB_GEN_CONTACTS_ROLESRepository, TAB_GEN_CONTACTS_ROLESRepository>();
            services.AddTransient<ITAB_GEN_CONTACTS_TYPESRepository, TAB_GEN_CONTACTS_TYPESRepository>();
            services.AddTransient<ITAB_GEN_TEXTSRepository, TAB_GEN_TEXTSRepository>();
            #endregion

            #region Productions
            services.AddTransient<IANALOGIERepository, ANALOGIERepository>();
            services.AddTransient<IATTACCORepository, ATTACCORepository>();
            services.AddTransient<IDENTIRepository, DENTIRepository>();
            services.AddTransient<IDIAMETRORepository, DIAMETRORepository>();
            services.AddTransient<IFORILUBRIFICATIRepository, FORILUBRIFICATIRepository>();
            services.AddTransient<ILDRepository, LDRepository>();
            services.AddTransient<IMATERIEPRIMERepository, MATERIEPRIMERepository>();
            services.AddTransient<IRIVESTIMENTIRepository, RIVESTIMENTIRepository>();
            services.AddTransient<Itab_articolo_produzione_sorgentiRepository, tab_articolo_produzione_sorgentiRepository>();
            services.AddTransient<Itab_produzione_calendario_chiusuraRepository, tab_produzione_calendario_chiusuraRepository>();
            services.AddTransient<Itab_produzione_causaleRepository, tab_produzione_causaleRepository>();
            services.AddTransient<Itab_produzione_operatore_costoRepository, tab_produzione_operatore_costoRepository>();
            services.AddTransient<Itab_produzione_operatoreRepository, tab_produzione_operatoreRepository>();
            services.AddTransient<Itab_produzione_repartoRepository, tab_produzione_repartoRepository>();
            services.AddTransient<Itab_produzione_risorsa_calendarioRepository, tab_produzione_risorsa_calendarioRepository>();
            services.AddTransient<Itab_produzione_risorsa_costoRepository, tab_produzione_risorsa_costoRepository>();
            services.AddTransient<Itab_produzione_risorsa_sorgentiRepository, tab_produzione_risorsa_sorgentiRepository>();
            services.AddTransient<Itab_produzione_risorsaRepository, tab_produzione_risorsaRepository>();
            services.AddTransient<ITIPMATPRIRepository, TIPMATPRIRepository>();
            services.AddTransient<ITIPTA00FRepository, TIPTA00FRepository>();
            #endregion

            #region Shipping
            services.AddTransient<ICAUSBOLLRepository, CAUSBOLLRepository>();
            #endregion

            #endregion

            #region Treasure
            services.AddTransient<IRBCC01F0Repository, RBCC01F0Repository>();
            services.AddTransient<ITES_IMFIRepository, TES_IMFIRepository>();
            services.AddTransient<ITreasuryRepository, TreasuryRepository>();
            #endregion

            return services;
        }

    }
}
