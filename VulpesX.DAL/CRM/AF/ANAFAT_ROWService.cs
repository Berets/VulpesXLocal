using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.CRM.AF
{
    public interface IANAFAT_ROWRepository
    {
        ObservableCollection<ANAFAT_ROW>? GetList(string CompanyID, int Year);

        ANAFAT_ROW? Get(string CompanyID, int Year, long ID);

        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(ANAFAT_ROW Model);

        bool Update(ANAFAT_ROW Model);

        bool Delete(ANAFAT_ROW Model);

        string? Validate(ANAFAT_ROW Model, bool IsInsert);

        void CalculateCosts(ref ANAFAT_CONST Const, ref ANAFAT_ROW Item);
    }

    public class ANAFAT_ROWRepository : RepositoryBase, IANAFAT_ROWRepository
    {
        public ANAFAT_ROWRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANAFAT_ROW>? GetList(string CompanyID, int Year)
        {
            throw new NotImplementedException();
        }

        public ANAFAT_ROW? Get(string CompanyID, int Year, long ID)
        {
            throw new NotImplementedException();
        }

        #region CRUD
        public string INSERT_QUERY => string.Empty;
        public string UPDATE_QUERY => string.Empty;
        public string DELETE_QUERY => string.Empty;

        public bool Insert(ANAFAT_ROW Model)
        {
            throw new NotImplementedException();
        }

        public bool Update(ANAFAT_ROW Model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ANAFAT_ROW Model)
        {
            throw new NotImplementedException();
        }

        public string? Validate(ANAFAT_ROW Model, bool IsInsert)
        {
            throw new NotImplementedException();
        }
        #endregion

        public void CalculateCosts(ref ANAFAT_CONST Const, ref ANAFAT_ROW Item)
        {
            throw new NotImplementedException();
        }

    }

    public class ANAFAT_ROWUfpRepository : RepositoryBase, IANAFAT_ROWRepository
    {
        public ANAFAT_ROWUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANAFAT_ROW>? GetList(string CompanyID, int Year)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<ANAFAT_ROW, tab_articolo, tab_articolo, tab_articolo, ANAFAT_ROW>(
                    @"SELECT r.*, 
                        a.artcod as ID, a.artdise,
                        m.artcod as ID, m.artdise,
                        c.artcod as ID, c.artdise
                        FROM ANAFAT_ROW as r
                        LEFT OUTER JOIN ANAG_ARTICOLI as a ON a.artcod = r.afartid
                        LEFT OUTER JOIN ANAG_ARTICOLI as m ON m.artcod = r.afmatid
                        LEFT OUTER JOIN ANAG_ARTICOLI as c ON c.artcod = r.afcicid
                        WHERE r.afsoc = @cid AND r.afyear = @yea
                        ORDER BY r.afid desc",
                (hea, art, mat, cyc) =>
                {
                    hea.Article = art;
                    hea.Material = mat;

                    return hea;
                },
                new { cid = CompanyID, yea = Year }, splitOn: "ID,ID,ID");

                return new ObservableCollection<ANAFAT_ROW>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ANAFAT_ROW? Get(string CompanyID, int Year, long ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<ANAFAT_ROW, tab_articolo, tab_articolo, tab_articolo, ANAFAT_ROW>(
                 @"SELECT r.*, 
                        a.artcod as ID, a.artdise,
                        m.artcod as ID, m.artdise,
                        c.artcod as ID, c.artdise
                        FROM ANAFAT_ROW as r
                        LEFT OUTER JOIN ANAG_ARTICOLI as a ON a.artcod = r.afartid
                        LEFT OUTER JOIN ANAG_ARTICOLI as m ON m.artcod = r.afmatid
                        LEFT OUTER JOIN ANAG_ARTICOLI as c ON c.artcod = r.afcicid
                        WHERE r.afsoc = @cid AND r.afyear = @yea AND r.afid = @ID
                        ORDER BY r.afid desc",
                         (hea, art, mat, cyc) =>
                         {
                             hea.Article = art;
                             hea.Material = mat;

                             return hea;
                         },
                         new { cid = CompanyID, yea = Year, ID }, splitOn: "ID,ID,ID").FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO ANAFAT_ROW (afsoc,afyear,afid,afdata,afcli,afclides,afconstdata,afconstver,afartid,afmatid,afmatforid,afmatfordata,afmatpre,afcicid,afcicseq,afcicmin,afcicpre,afextpre,afvarnote,afvarpre,afcustomertype,afproductiontype,afcomplexitytype,afmatsearch,afcicsearch,added,updated,canceled,addedUserID,updateUserID,canceledUserID) OUTPUT INSERTED.rv VALUES(@afsoc,@afyear,@afid,@afdata,@afcli,@afclides,@afconstdata,@afconstver,@afartid,@afmatid,@afmatforid,@afmatfordata,@afmatpre,@afcicid,@afcicseq,@afcicmin,@afcicpre,@afextpre,@afvarnote,@afvarpre,@afcustomertype,@afproductiontype,@afcomplexitytype,@afmatsearch,@afcicsearch,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updateUserID,@canceledUserID)";
        public string UPDATE_QUERY => "UPDATE ANAFAT_ROW SET afsoc=@afsoc,afyear=@afyear,afid=@afid,afdata=@afdata,afcli=@afcli,afclides=@afclides,afconstdata = @afconstdata,afconstver=@afconstver,afartid=@afartid,afmatid=@afmatid,afmatforid=@afmatforid,afmatfordata=@afmatfordata,afmatpre=@afmatpre,afcicid=@afcicid,afcicseq=@afcicseq,afcicmin=@afcicmin,afcicpre=@afcicpre,afextpre=@afextpre,afvarnote=@afvarnote,afvarpre=@afvarpre,afcustomertype=@afcustomertype,afproductiontype=@afproductiontype,afcomplexitytype=@afcomplexitytype,afmatsearch=@afmatsearch,afcicsearch=@afcicsearch,added=@added,updated = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled=@canceled,addedUserID=@addedUserID,updateUserID=@updateUserID,canceledUserID=@canceledUserID OUTPUT INSERTED.rv WHERE afsoc = @afsoc AND afyear = @afyear AND afid = @afid AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM ANAFAT_ROW OUTPUT DELETED.rv WHERE afsoc = @afsoc AND afyear = @afyear AND afid = @afid AND rv = @rv";

        public bool Insert(ANAFAT_ROW Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(INSERT_QUERY, Model);
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

        public bool Update(ANAFAT_ROW Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.ExecuteScalar(UPDATE_QUERY, Model);
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

        public bool Delete(ANAFAT_ROW Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                var result = connection.Execute(DELETE_QUERY, Model);
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

        public string? Validate(ANAFAT_ROW Model, bool IsInsert)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public void CalculateCosts(ref ANAFAT_CONST Const, ref ANAFAT_ROW Item)
        {
            Item.afcicmin = Item.afcicmin ?? 0;
            Item.afmatpre = Item.afmatpre ?? 0;
            Item.afvarpre = Item.afvarpre ?? 0;

            decimal costAut = (Const.afproductionaut ?? 0) / 60;
            decimal costPre = (Const.afproductionpre ?? 0) / 60;

            decimal complexityPercentage = Item.afcomplexitytype == "S" ? (Const.afcomplexitysta ?? 0) : Item.afcomplexitytype == "M" ? (Const.afcomplexitymed ?? 0) : (Const.afcomplexitycom ?? 0);
            decimal customerPercentage = Item.afcustomertype == "I" ? (Const.afcliico ?? 0) : (Const.afclidir ?? 0);
            decimal customerProvigion = Item.afcustomertype == "I" ? (Const.afproico ?? 0) : (Const.afprodir ?? 0);

            foreach (var pie in Const.Pieces)
            {
                decimal percentageProduction = (pie.afppercentage ?? 0) == 1 ? 1 : (pie.afppercentage ?? 1);

                pie.ProductionCostPresidiato = (Item.afcicmin * costPre) + (((Item.afcicmin * costPre) * percentageProduction) / 100);
                pie.NetCostPresidiato = pie.ProductionCostPresidiato + (Item.afmatpre ?? 0) + (Item.afvarpre ?? 0);
                pie.ComplexityPresidiato = ((pie.NetCostPresidiato * complexityPercentage) / 100);
                pie.AgentPresidiato = (((pie.NetCostPresidiato + pie.ComplexityPresidiato) * customerProvigion) / 100);
                pie.TotalPresidiato = pie.NetCostPresidiato + pie.ComplexityPresidiato + pie.AgentPresidiato;
                pie.CustomerPresidiato = ((pie.TotalPresidiato) / (1 - (customerPercentage / 100))) - (pie.TotalPresidiato);
                pie.TotalPricePresidiato = pie.TotalPresidiato + pie.CustomerPresidiato;

                // NON PRESIDIATO
                if (pie.afpproductionaut_enabled ?? false)
                {
                    pie.ProductionCostAuto = (Item.afcicmin * costAut);
                    pie.NetCostAuto = pie.ProductionCostAuto + (Item.afmatpre ?? 0) + (Item.afvarpre ?? 0);
                    pie.ComplexityAuto = ((pie.NetCostAuto * complexityPercentage) / 100);
                    pie.AgentAuto = (((pie.NetCostAuto + pie.ComplexityAuto) * customerProvigion) / 100);
                    pie.TotalAuto = pie.NetCostAuto + pie.ComplexityAuto + pie.AgentAuto;
                    pie.CustomerAuto = ((pie.TotalAuto) / (1 - (customerPercentage / 100))) - (pie.TotalAuto);
                    pie.TotalPriceAuto = pie.TotalAuto + pie.CustomerAuto;
                }
            }
        }
    }
}
