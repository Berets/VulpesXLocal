using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.DAL.Accounting
{
    public interface IMPDETTAGLIORepository
    {
        ObservableCollection<MPDETTAGLIO>? GetList(string CompanyID, int Year, int ID);

        MPDETTAGLIO? Get(string CompanyID, int Year, int ID, int Position);

        bool Exists(string CompanyID, int Year, int ID, int Position);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(MPDETTAGLIO Model);

        bool Update(MPDETTAGLIO Model);

        bool Delete(MPDETTAGLIO Model);

        string? Validate(MPDETTAGLIO Model, bool IsInsert);
        #endregion
    }

    public class MPDETTAGLIORepository : RepositoryBase, IMPDETTAGLIORepository
    {
        public MPDETTAGLIORepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<MPDETTAGLIO>? GetList(string CompanyID, int Year, int ID)
        {
            try
            {
                using var connection = GetOpenConnection();

                var list = connection.Query<MPDETTAGLIO>(
                    "SELECT * FROM MPDETTAGLIO WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id ORDER BY MPPOSI DESC",
                    new { cid = CompanyID, year = Year, id = ID });

                return new ObservableCollection<MPDETTAGLIO>(list);
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public MPDETTAGLIO? Get(string CompanyID, int Year, int ID, int Position)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<MPDETTAGLIO>(
                    @"SELECT * FROM MPDETTAGLIO WHERE MPSOCI = @cid AND MPANNO = @year AND MPNUME = @id AND MPPOSI = @pos",
                    new { cid = CompanyID, year = Year, id = ID, pos = Position })
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public bool Exists(string CompanyID, int Year, int ID, int Position)
        {
            try
            {
                using var connection = GetOpenConnection();


                return (int?)connection.ExecuteScalar(
                    "SELECT COUNT(*) FROM MPDETTAGLIO WHERE MPSOCI=@cid AND MPANNO = @year AND MPNUME = @id AND MPPOSI = @pos",
                    new { cid = CompanyID, year = Year, id = ID, pos = Position }) > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return false;
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO MPDETTAGLIO (MPSOCI,MPANNO,MPNUME,MPPOSI,M3SOCI,M3ANNO,M3REGI,M3RIGA,M3DARI,M3RIFE,M3DOCU,M3DARE,M3DADO,M3CAUS,M3GRUP,M3CONT,M3SSOC,M3SOTT,M3IMPO,M3DESC,M3SEGN,M3RATA,M3SCAD,M3PAGA,M3PARE,M3PRAT,M3DEST,M3PAXI,M3INIZ,M3CAMB,M3VALU,M3DIVI,M3vcod,M3FLPA,M3DVAL,M3ABIF,M3CABF,M3IMEU,M3TIDO,M3RIOR,M3FL01,M3FLCO,M3FLES,M3RSOC,M3RANN,M3RREG,M3RRIG,M3IMAB,M3EUAB,M3SEGA,M3VAAB,m3numef) OUTPUT INSERTED.rv VALUES (@MPSOCI,@MPANNO,@MPNUME,@MPPOSI,@M3SOCI,@M3ANNO,@M3REGI,@M3RIGA,@M3DARI,@M3RIFE,@M3DOCU,@M3DARE,@M3DADO,@M3CAUS,@M3GRUP,@M3CONT,@M3SSOC,@M3SOTT,@M3IMPO,@M3DESC,@M3SEGN,@M3RATA,@M3SCAD,@M3PAGA,@M3PARE,@M3PRAT,@M3DEST,@M3PAXI,@M3INIZ,@M3CAMB,@M3VALU,@M3DIVI,@M3vcod,@M3FLPA,@M3DVAL,@M3ABIF,@M3CABF,@M3IMEU,@M3TIDO,@M3RIOR,@M3FL01,@M3FLCO,@M3FLES,@M3RSOC,@M3RANN,@M3RREG,@M3RRIG,@M3IMAB,@M3EUAB,@M3SEGA,@M3VAAB,@m3numef)";
        public string UPDATE_QUERY => "UPDATE MPDETTAGLIO SET M3SOCI=@M3SOCI,M3ANNO=@M3ANNO,M3REGI=@M3REGI,M3RIGA=@M3RIGA,M3DARI=@M3DARI,M3RIFE=@M3RIFE,M3DOCU=@M3DOCU,M3DARE=@M3DARE,M3DADO=@M3DADO,M3CAUS=@M3CAUS,M3GRUP=@M3GRUP,M3CONT=@M3CONT,M3SSOC=@M3SSOC,M3SOTT=@M3SOTT,M3IMPO=@M3IMPO,M3DESC=@M3DESC,M3SEGN=@M3SEGN,M3RATA=@M3RATA,M3SCAD=@M3SCAD,M3PAGA=@M3PAGA,M3PARE=@M3PARE,M3PRAT=@M3PRAT,M3DEST=@M3DEST,M3PAXI=@M3PAXI,M3INIZ=@M3INIZ,M3CAMB=@M3CAMB,M3VALU=@M3VALU,M3DIVI=@M3DIVI,M3vcod=@M3vcod,M3FLPA=@M3FLPA,M3DVAL=@M3DVAL,M3ABIF=@M3ABIF,M3CABF=@M3CABF,M3IMEU=@M3IMEU,M3TIDO=@M3TIDO,M3RIOR=@M3RIOR,M3FL01=@M3FL01,M3FLCO=@M3FLCO,M3FLES=@M3FLES,M3RSOC=@M3RSOC,M3RANN=@M3RANN,M3RREG=@M3RREG,M3RRIG=@M3RRIG,M3IMAB=@M3IMAB,M3EUAB=@M3EUAB,M3SEGA=@M3SEGA,M3VAAB=@M3VAAB,m3numef=@m3numef OUTPUT INSERTED.rv WHERE MPSOCI=@MPSOCI AND MPANNO=@MPANNO AND MPNUME=@MPNUME AND MPPOSI=@MPPOSI";
        public string DELETE_QUERY => "DELETE FROM MPDETTAGLIO OUTPUT DELETED.rv WHERE MPSOCI = @MPSOCI AND MPANNO = @MPANNO AND MPNUME = @MPNUME AND MPPOSI = @MPPOSI AND rv = @rv";

        public bool Insert(MPDETTAGLIO Model)
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

        public bool Update(MPDETTAGLIO Model)
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

        public bool Delete(MPDETTAGLIO Model)
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

        public string? Validate(MPDETTAGLIO Model, bool IsInsert)
        {
            try
            {
                if (Model.MPANNO > 0)
                {
                    if (!IsInsert || (IsInsert && !Exists(Model.MPSOCI, Model.MPANNO, Model.MPNUME, Model.MPPOSI)))
                    {
                        return null;
                    }
                    else
                    { return "L'anno inserito è già in uso"; }
                }
                else
                { return "L'anno è obbligatorio"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
