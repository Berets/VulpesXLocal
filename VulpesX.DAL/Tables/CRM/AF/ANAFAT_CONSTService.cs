using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Ufp;
using VulpesX.Shared.Generics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.DAL.Tables.CRM.AF
{
    public interface IANAFAT_CONSTRepository
    {
        ObservableCollection<ANAFAT_CONST>? GetList();

        ObservableCollection<ANAFAT_CONST>? GetList(string CompanyID, DateTime Date);

        ANAFAT_CONST? Get(string CompanyID, DateTime Date, int Version);

        bool Exists(string CompanyID, DateTime Date);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(ANAFAT_CONST Model);

        bool Update(ANAFAT_CONST Model);

        bool Delete(ANAFAT_CONST Model);

        string? Validate(ANAFAT_CONST Model, bool IsInsert);
        #endregion
    }

    public class ANAFAT_CONSTRepository : RepositoryBase, IANAFAT_CONSTRepository
    {
        public ANAFAT_CONSTRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANAFAT_CONST>? GetList()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ANAFAT_CONST>? GetList(string CompanyID, DateTime Date)
        {
            throw new NotImplementedException();
        }

        public ANAFAT_CONST? Get(string CompanyID, DateTime Date, int Version)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string CompanyID, DateTime Date)
        {
            throw new NotImplementedException();
        }

        #region CRUD
        public string INSERT_QUERY => string.Empty;
        public string UPDATE_QUERY => string.Empty;
        public string DELETE_QUERY => string.Empty;
        public bool Insert(ANAFAT_CONST Model)
        {
            throw new NotImplementedException();
        }

        public bool Update(ANAFAT_CONST Model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ANAFAT_CONST Model)
        {
            throw new NotImplementedException();
        }

        public string? Validate(ANAFAT_CONST Model, bool IsInsert)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ANAFAT_CONSTUfpRepository : RepositoryBase, IANAFAT_CONSTRepository
    {
        public ANAFAT_CONSTUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANAFAT_CONST>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<ANAFAT_CONST>(
                    @"SELECT p.*, CR.CT as RowsCount FROM ANAFAT_CONST AS p
                        OUTER APPLY(SELECT COUNT(*) as CT FROM ANAFAT_ROW as ar
                                    WHERE ar.afsoc = p.afsoc AND ar.afconstdata = p.afdata AND ar.afconstver = p.afver) as CR                          
                        ORDER BY afdata desc");

                return new ObservableCollection<ANAFAT_CONST>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ANAFAT_CONST>? GetList(string CompanyID, DateTime Date)
        {
            try
            {
                using var connection = GetOpenConnection();

                var cnsts = connection.Query<ANAFAT_CONST>(
                    @"SELECT * FROM ANAFAT_CONST WHERE afsoc = @afsoc AND afdata <= @afdata ORDER BY afdata desc, afver desc",
                    new { afsoc = CompanyID, afdata = Date });

                foreach (var cnst in cnsts)
                {
                    cnst.Pieces = VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_PIECESRepository>().Get(cnst.afsoc, cnst.afdata, cnst.afver) ?? new ObservableCollection<ANAFAT_PIECES>();
                }

                return new ObservableCollection<ANAFAT_CONST>(cnsts);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ANAFAT_CONST? Get(string CompanyID, DateTime Date, int Version)
        {
            try
            {
                using var connection = GetOpenConnection();

                var cnst = connection.Query<ANAFAT_CONST>(
                    @"SELECT p.*,CR.CT as RowsCount FROM ANAFAT_CONST as p
OUTER APPLY(SELECT COUNT(*) as CT FROM ANAFAT_ROW as ar
                                    WHERE ar.afsoc = p.afsoc AND ar.afconstdata = p.afdata AND ar.afconstver = p.afver) as CR           
WHERE p.afsoc = @afsoc AND p.afdata = @afdata AND p.afver = @afver",
                    new { afsoc = CompanyID, afdata = Date, afver= Version })
                    .FirstOrDefault();

                if (cnst != null)
                    cnst.Pieces = VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_PIECESRepository>().Get(cnst.afsoc, cnst.afdata, cnst.afver) ?? new ObservableCollection<ANAFAT_PIECES>();

                return cnst;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, DateTime Date)
        {
            try
            {
                using var connection = GetOpenConnection();

                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM ANAFAT_CONST WHERE afsoc = @afsoc AND afdata = @afdata",
                     new { afsoc = CompanyID, afdata = Date }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO ANAFAT_CONST (afsoc,afdata, afver,afproductionpre,afproductionaut,afcomplexitysta,afcomplexitymed,afcomplexitycom,afcliico,afclidir,afproico,afprodir,added,updated,canceled,addedUserID,updateUserID,canceledUserID) OUTPUT INSERTED.rv VALUES(@afsoc,@afdata,@afver,@afproductionpre,@afproductionaut,@afcomplexitysta,@afcomplexitymed,@afcomplexitycom,@afcliico,@afclidir,@afproico,@afprodir,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updateUserID,@canceledUserID)";
        public string UPDATE_QUERY => "UPDATE ANAFAT_CONST SET afsoc=@afsoc,afdata=@afdata,afver=@afver,afproductionpre=@afproductionpre,afproductionaut=@afproductionaut,afcomplexitysta=@afcomplexitysta,afcomplexitymed=@afcomplexitymed,afcomplexitycom=@afcomplexitycom,afcliico=@afcliico,afclidir=@afclidir,afproico=@afproico,afprodir=@afprodir,added=@added,updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled=@canceled,addedUserID=@addedUserID,updateUserID=@updateUserID,canceledUserID=@canceledUserID OUTPUT INSERTED.rv WHERE afsoc = @afsoc AND afdata=@afdata AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM ANAFAT_CONST OUTPUT DELETED.rv WHERE afsoc = @afsoc AND afdata =@afdata AND afver=@afver AND rv = @rv";
        public bool Insert(ANAFAT_CONST Model)
        {
            try
            {
                var anafatPieces = VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_PIECESRepository>();

                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    //GET VERSION
                    int version = (connection.Query<int?>("SELECT afver FROM ANAFAT_CONST WHERE afsoc = @CompanyID AND afdata = @Date ORDER BY afver desc", new { CompanyID = Model.afsoc, Date = Model.afdata }, transaction).FirstOrDefault() ?? 0) + 1;
                    Model.afver = version;

                    //PIECES 
                    foreach (var pie in Model.Pieces)
                    {
                        pie.afpdata = Model.afdata;
                        pie.afpver = Model.afver;

                        connection.Execute(anafatPieces.INSERT_QUERY, pie, transaction);
                    }

                    var result = connection.Execute(INSERT_QUERY, Model, transaction);
                    transaction.Commit();

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
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Update(ANAFAT_CONST Model)
        {
            try
            {
                var anafatPieces = VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_PIECESRepository>();

                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    //REFRESH PIECES 
                    connection.Execute(@"DELETE FROM ANAFAT_PIECES WHERE afpsoc = @afpsoc AND afpdata =@afpdata AND afpver = @afpver",
                       new { afpsoc = Model.afsoc, afpdata = Model.afdata, afpver = Model.afver },
                       transaction);

                    foreach (var pie in Model.Pieces)
                    {
                        pie.afpdata = Model.afdata;
                        pie.afpver = Model.afver;
                        connection.Execute(anafatPieces.INSERT_QUERY, pie, transaction);
                    }

                    var result = connection.ExecuteScalar(UPDATE_QUERY, Model, transaction);
                    transaction.Commit();

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
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public bool Delete(ANAFAT_CONST Model)
        {
            try
            {
                var anafatPieces = VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_PIECESRepository>();

                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    connection.Execute(@"DELETE FROM ANAFAT_PIECES WHERE afpsoc = @afpsoc AND afpdata =@afpdata",
                      new { afpsoc = Model.afsoc, afpdata = Model.afdata },
                      transaction);

                    var result = connection.Execute(DELETE_QUERY, Model, transaction);
                    transaction.Commit();

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
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        public string? Validate(ANAFAT_CONST Model, bool IsInsert)
        {
            try
            {
                var sbError = new StringBuilder();

                if (!Model.afproductionaut.HasValue)
                    sbError.AppendLine("Inserire un valore per la produzione non presidiata");
                if (!Model.afproductionpre.HasValue)
                    sbError.AppendLine("Inserire un valore per la produzione presidiata");

                if (!Model.afcomplexitysta.HasValue)
                    sbError.AppendLine("Inserire un valore per la complessità standard");
                if (!Model.afcomplexitymed.HasValue)
                    sbError.AppendLine("Inserire un valore per la complessità media");
                if (!Model.afcomplexitycom.HasValue)
                    sbError.AppendLine("Inserire un valore per la complessità complessa");

                if (!Model.afclidir.HasValue)
                    sbError.AppendLine("Inserire un valore per i clienti diretti");
                if (!Model.afcliico.HasValue)
                    sbError.AppendLine("Inserire un valore per i clienti ICO");

                return sbError.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
