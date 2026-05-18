using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Accounting;

public interface ISTATE00FRepository
{
    ObservableCollection<STATE00F>? GetList();

    ObservableCollection<ObservableCollection<GenericShortDecimal>>? GetYearsInvoicesTrend(string CompanyID, int CustomerID);

    #region CRUD
    bool Insert(STATE00F Model);

    bool Update(STATE00F Model);

    bool Delete(STATE00F Model);

    string? Validate(STATE00F Model, bool IsInsert);
    #endregion
}

public class STATE00FRepository : RepositoryBase, ISTATE00FRepository
{
    public STATE00FRepository(IConnectionFactory factory) : base(factory)
    {
    }

    public ObservableCollection<STATE00F>? GetList()
    {
        try
        {
            using var connection = GetOpenConnection();


            var list = connection.Query<STATE00F>(
                "SELECT * FROM STATE00F");

            return new ObservableCollection<STATE00F>(list);

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    public ObservableCollection<ObservableCollection<GenericShortDecimal>>? GetYearsInvoicesTrend(string CompanyID, int CustomerID)
    {
        try
        {
            using var connection = GetOpenConnection();


            var listInvoices = connection.Query<GenericShortDecimal>(
                @"SELECT STANNO AS ShortItem, (SUM(STVALT) + SUM(STMAGG) - SUM(STSCO1) - SUM(STSCO2)) AS DecimalItem from STATE00F
                        WHERE stsoci = @cid AND STTIPD = 'F' AND STCOCL = @cust
                        GROUP BY STANNO;",
                new { cid = CompanyID, cust = CustomerID }).ToList();

            var listCredits = connection.Query<GenericShortDecimal>(
                @"SELECT STANNO AS ShortItem, (SUM(STVALT) + SUM(STMAGG) - SUM(STSCO1) - SUM(STSCO2)) AS DecimalItem from STATE00F
                        WHERE stsoci = @cid AND STTIPD = 'N' AND STCOCL = @cust
                        GROUP BY STANNO;",
                new { cid = CompanyID, cust = CustomerID }).ToList();

            foreach (var crd in listCredits)
            {
                var inv = listInvoices.Where(w => w.ShortItem == crd.ShortItem).FirstOrDefault();
                if (inv != null)
                {
                    inv.DecimalItem -= crd.DecimalItem;
                }
                else
                {
                    listInvoices.Add(new GenericShortDecimal() { ShortItem = crd.ShortItem, DecimalItem = crd.DecimalItem * -1 });
                }
            }
            foreach (var inv in listInvoices)
            {
                var crd = listCredits.Where(w => w.ShortItem == inv.ShortItem).FirstOrDefault();
                if (crd == null)
                {
                    listCredits.Add(new GenericShortDecimal() { ShortItem = inv.ShortItem, DecimalItem = 0 });
                }
            }

            return new ObservableCollection<ObservableCollection<GenericShortDecimal>>() { new ObservableCollection<GenericShortDecimal>(listInvoices.OrderBy(o => o.ShortItem)), new ObservableCollection<GenericShortDecimal>(listCredits.OrderBy(o => o.ShortItem)) };

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return null;
        }
    }

    #region CRUD
    public bool Insert(STATE00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "INSERT INTO STATE00F (stsoci,STANNO,STFAPR,STRIGA,STCOCL,STNUDO,STCODE,STCAPF,STCAPD,STDADO,STTIPD,STDABO,STNUBO,STCONS,STAREA,STZONA,STAGEN,STDEPO,STREGI,STREGG,STFILI,STCATM,STARTI,STCLAS,STFAMI,STPROO,STTPRO,STCONT,STPROF,STPROD,STCIST,STREDE,STQTAA,STVALT,STVALU,STSCO1,STSCO2,STMAGG,STPROV,STUNIM,STFLO1,STFL02,STFL03,STFL04,STFL05,STFL06,STPERC,STTIPO,STUNI2,STQTA2,STCAUS,STZON1,STAGE1,STSPEM,STCOC1,STGRUP,STSOTT,stcap1,stcap2,umide1,umide2,abedes,abede1,abede2,abede3,sttipa,stcopa,icscod,stsco3,STAMMI,STCODW,stsoc1,stTipDes,stArtdes,stRagSocC) OUTPUT INSERTED.rv VALUES(@stsoci,@STANNO,@STFAPR,@STRIGA,@STCOCL,@STNUDO,@STCODE,@STCAPF,@STCAPD,@STDADO,@STTIPD,@STDABO,@STNUBO,@STCONS,@STAREA,@STZONA,@STAGEN,@STDEPO,@STREGI,@STREGG,@STFILI,@STCATM,@STARTI,@STCLAS,@STFAMI,@STPROO,@STTPRO,@STCONT,@STPROF,@STPROD,@STCIST,@STREDE,@STQTAA,@STVALT,@STVALU,@STSCO1,@STSCO2,@STMAGG,@STPROV,@STUNIM,@STFLO1,@STFL02,@STFL03,@STFL04,@STFL05,@STFL06,@STPERC,@STTIPO,@STUNI2,@STQTA2,@STCAUS,@STZON1,@STAGE1,@STSPEM,@STCOC1,@STGRUP,@STSOTT,@stcap1,@stcap2,@umide1,@umide2,@abedes,@abede1,@abede2,@abede3,@sttipa,@stcopa,@icscod,@stsco3,@STAMMI,@STCODW,@stsoc1,@stTipDes,@stArtdes,@stRagSocC)",
                Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.INSERT_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Update(STATE00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.ExecuteScalar(
                "UPDATE STATE00F SET stsoci = @stsoci,STANNO = @STANNO,STFAPR = @STFAPR,STRIGA = @STRIGA,STCOCL = @STCOCL,STNUDO = @STNUDO,STCODE = @STCODE,STCAPF = @STCAPF,STCAPD = @STCAPD,STDADO = @STDADO,STTIPD = @STTIPD,STDABO = @STDABO,STNUBO = @STNUBO,STCONS = @STCONS,STAREA = @STAREA,STZONA = @STZONA,STAGEN = @STAGEN,STDEPO = @STDEPO,STREGI = @STREGI,STREGG = @STREGG,STFILI = @STFILI,STCATM = @STCATM,STARTI = @STARTI,STCLAS = @STCLAS,STFAMI = @STFAMI,STPROO = @STPROO,STTPRO = @STTPRO,STCONT = @STCONT,STPROF = @STPROF,STPROD = @STPROD,STCIST = @STCIST,STREDE = @STREDE,STQTAA = @STQTAA,STVALT = @STVALT,STVALU = @STVALU,STSCO1 = @STSCO1,STSCO2 = @STSCO2,STMAGG = @STMAGG,STPROV = @STPROV,STUNIM = @STUNIM,STFLO1 = @STFLO1,STFL02 = @STFL02,STFL03 = @STFL03,STFL04 = @STFL04,STFL05 = @STFL05,STFL06 = @STFL06,STPERC = @STPERC,STTIPO = @STTIPO,STUNI2 = @STUNI2,STQTA2 = @STQTA2,STCAUS = @STCAUS,STZON1 = @STZON1,STAGE1 = @STAGE1,STSPEM = @STSPEM,STCOC1 = @STCOC1,STGRUP = @STGRUP,STSOTT = @STSOTT,stcap1 = @stcap1,stcap2 = @stcap2,umide1 = @umide1,umide2 = @umide2,abedes = @abedes,abede1 = @abede1,abede2 = @abede2,abede3 = @abede3,sttipa = @sttipa,stcopa = @stcopa,icscod = @icscod,stsco3 = @stsco3,STAMMI = @STAMMI,STCODW = @STCODW,stsoc1 = @stsoc1,stTipDes = @stTipDes,stArtdes = @stArtdes,stRagSocC = @stRagSocC OUTPUT INSERTED.rv WHERE stsoci = @stsoci AND STANNO = @stanno AND STFAPR = @stfapr AND STRIGA = @striga AND rv = @rv",
                Model);
            if (result != null)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public bool Delete(STATE00F Model)
    {
        try
        {
            using var connection = GetOpenConnection();


            var result = connection.Execute(
                "DELETE FROM STATE00F OUTPUT DELETED.rv WHERE stsoci = @stsoci AND STANNO = @stanno AND STFAPR = @stfapr AND STRIGA = @striga AND rv = @rv",
                Model);
            if (result > 0)
            {
                return true;
            }
            else
            {
                ErrorHandler.Show(Constants.CONCURRENCY_VIOLATION);
                return false;
            }

        }
        catch (Exception ex)
        {
            ErrorHandler.Show(ex.Message);
            return false;
        }
    }

    public string? Validate(STATE00F Model, bool IsInsert)
    {
        try
        {
            if (true)
            {

                return null;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion
}