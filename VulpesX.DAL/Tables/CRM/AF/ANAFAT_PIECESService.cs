using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models.Ufp;
using VulpesX.Shared.Generics;

namespace VulpesX.DAL.Tables.CRM.AF
{
    public interface IANAFAT_PIECESRepository
    {
        ObservableCollection<ANAFAT_PIECES>? GetList();

        ObservableCollection<ANAFAT_PIECES>? Get(string CompanyID, DateTime Date, int Version);

        ANAFAT_PIECES? GetFromQuantity(string CompanyID, int Quantity);

        bool Exists(string CompanyID, DateTime Date);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }
        bool Insert(ANAFAT_PIECES Model);

        bool Update(ANAFAT_PIECES Model);
        #endregion
    }

    public class ANAFAT_PIECESRepository : RepositoryBase, IANAFAT_PIECESRepository
    {
        public ANAFAT_PIECESRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<ANAFAT_PIECES>? GetList()
        {
            throw new NotImplementedException();
        }

        public ObservableCollection<ANAFAT_PIECES>? Get(string CompanyID, DateTime Date, int Version)
        {
            throw new NotImplementedException();
        }

        public ANAFAT_PIECES? GetFromQuantity(string CompanyID, int Quantity)
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
        public bool Insert(ANAFAT_PIECES Model)
        {
            throw new NotImplementedException();
        }

        public bool Update(ANAFAT_PIECES Model)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class ANAFAT_PIECESUfpRepository : RepositoryBase, IANAFAT_PIECESRepository
    {
        public ANAFAT_PIECESUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }


        public ObservableCollection<ANAFAT_PIECES>? GetList()
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<ANAFAT_PIECES>(
                    @"SELECT p.* FROM ANAFAT_PIECES AS p
                        ORDER BY afpdata desc");

                return new ObservableCollection<ANAFAT_PIECES>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ANAFAT_PIECES>? Get(string CompanyID, DateTime Date, int Version)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<ANAFAT_PIECES>(
                    @"SELECT * FROM ANAFAT_PIECES WHERE afpsoc = @afsoc AND afpdata = @afdata AND afpver = @afpver ORDER BY afppiecesfrom",
                    new { afsoc = CompanyID, afdata = Date, afpver = Version });

                return new ObservableCollection<ANAFAT_PIECES>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ANAFAT_PIECES? GetFromQuantity(string CompanyID, int Quantity)
        {
            try
            {
                using var connection = GetOpenConnection();

                return connection.Query<ANAFAT_PIECES>(
                    @"SELECT * FROM ANAFAT_PIECES WHERE afpsoc = @afsoc AND afpdata <= @afdata AND ( @quantity >= afppiecesfrom  AND (@quantity <= afppiecesto OR afppiecesto = 0)) ORDER BY afpdata desc",
                    new { afsoc = CompanyID, afdata = DateTime.Now, quantity = Quantity }).FirstOrDefault();
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
                    "SELECT COUNT(*) FROM ANAFAT_PIECES WHERE afpsoc = @afsoc AND afpdata = @afdata",
                     new { afsoc = CompanyID, afdata = Date }) > 0;

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return true;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO ANAFAT_PIECES (afpsoc,afpdata, afpver,afppiecesfrom,afppiecesto,afppercentage,afpproductiontype,afpproductionaut_enabled,added,updated,canceled,addedUserID,updateUserID,canceledUserID) OUTPUT INSERTED.rv VALUES(@afpsoc,@afpdata,@afpver,@afppiecesfrom,@afppiecesto,@afppercentage,@afpproductiontype,@afpproductionaut_enabled,SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',@updated,@canceled,@addedUserID,@updateUserID,@canceledUserID)";
        public string UPDATE_QUERY => "UPDATE ANAFAT_PIECES SET afpsoc=@afpsoc,afpdata=@afpdata,afpver=@afpver,afppiecesfrom=@afppiecesfrom,afppiecesto=@afppiecesto,afppercentage=@afppercentage,afpproductiontype=@afpproductiontype,afpproductionaut_enabled=@afpproductionaut_enabled,added=@added,updated=SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'Central Europe Standard Time',canceled=@canceled,addedUserID=@addedUserID,updateUserID=@updateUserID,canceledUserID=@canceledUserID OUTPUT INSERTED.rv WHERE afsoc = @afsoc AND afdata=@afdata AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM ANAFAT_PIECES OUTPUT DELETED.rv WHERE afpsoc = @afpsoc AND afpdata =@afpdata AND afpver=@afpver AND rv = @rv";
        public bool Insert(ANAFAT_PIECES Model)
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

        public bool Update(ANAFAT_PIECES Model)
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
        #endregion
    }
}
