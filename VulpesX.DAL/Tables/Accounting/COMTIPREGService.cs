using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.Models.Ufp;

namespace VulpesX.DAL.Tables.Accounting
{

    public interface ICOMTIPREGUfpRepository
    {
        ObservableCollection<COMTIPREG>? GetSimpleList(string CompanyID);

        ObservableCollection<COMTIPREG>? GetSimpleList(string CompanyID, string CausalID);

        ObservableCollection<COMTIPREG>? GetList(string CompanyID);

        ObservableCollection<COMTIPREGLEVEL1>? GetDetails(string CompanyID, string CausalID, short Progressive);

        COMTIPREG? Get(string CompanyID, string CausalID, short ID);


        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        string INSERT_ROW_QUERY { get; }

        bool Insert(COMTIPREG Model, ObservableCollection<COMTIPREGLEVEL1>? Details, string CompanyID);

        bool Update(COMTIPREG Model, ObservableCollection<COMTIPREGLEVEL1>? Details, string CompanyID);

        bool Delete(COMTIPREG Model);

        string? Validate(COMTIPREG Model, ObservableCollection<COMTIPREGLEVEL1>? Details, bool IsInsert);

        string? ValidateRow(COMTIPREGLEVEL1 Model, bool IsInsert);
        #endregion

    }

    public class COMTIPREGUfpRepository : RepositoryBase, ICOMTIPREGUfpRepository
    {
        public COMTIPREGUfpRepository(IConnectionFactory factory) : base(factory)
        {
        }

        public ObservableCollection<COMTIPREG>? GetSimpleList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return new ObservableCollection<COMTIPREG>(connection.Query<COMTIPREG>(
                    @"SELECT c.* FROM COMTIPREG AS c
                        WHERE c.Cocodso = @cid
                        ORDER BY c.causcon", new { cid = CompanyID }));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<COMTIPREG>? GetSimpleList(string CompanyID, string CausalID)
        {
            try
            {
                using var connection = GetOpenConnection();

                return new ObservableCollection<COMTIPREG>(connection.Query<COMTIPREG>(
                    @"SELECT c.* FROM COMTIPREG AS c
                        WHERE c.Cocodso = @cid AND c.causcon = @sid
                        ORDER BY c.cauprco", new { cid = CompanyID, sid = CausalID }));

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<COMTIPREG>? GetList(string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<COMTIPREG, CAUCONT, COMTIPREG>(
                    @"SELECT c.*, l.* FROM COMTIPREG AS c
                        LEFT JOIN CAUCONT AS l ON l.caucod = c.causcon
                        WHERE c.Cocodso = @cid
                        ORDER BY c.causcon",
                    (cau, liv) => { cau.Causal = liv; return cau; }, new { cid = CompanyID }, splitOn: "caucod");

                return new ObservableCollection<COMTIPREG>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<COMTIPREGLEVEL1>? GetDetails(string CompanyID, string CausalID, short Progressive)
        {
            try
            {
                using var connection = GetOpenConnection();


                var list = connection.Query<COMTIPREGLEVEL1>(
                    @"SELECT * FROM COMTIPREGLEVEL1
                        WHERE Cocodso = @cid and causcon = @sid and cauprco = @pid
                        ORDER BY Conriga",
                  new { cid = CompanyID, sid = CausalID, pid = Progressive });

                return new ObservableCollection<COMTIPREGLEVEL1>(list);

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public COMTIPREG? Get(string CompanyID, string CausalID, short ID)
        {
            try
            {
                using var connection = GetOpenConnection();


                return connection.Query<COMTIPREG>(
                    @"SELECT * FROM COMTIPREG 
                        WHERE Cocodso = @cid AND causcon = @sid AND cauprco = @id",
                    new { cid = CompanyID, sid = CausalID, id = ID })
                    .FirstOrDefault();

            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }


        #region CRUD
        public string INSERT_QUERY => "INSERT INTO COMTIPREG (Cocodso,causcon,cauprco,Corigtot,cocodes) VALUES(@Cocodso,@causcon,@cauprco,@Corigtot,@cocodes)";
        public string UPDATE_QUERY => "UPDATE COMTIPREG SET Cocodso=@Cocodso,causcon=@causcon,cauprco=@cauprco,Corigtot=@Corigtot,cocodes=@cocodes WHERE Cocodso = @Cocodso AND causcon = @causcon AND cauprco = @cauprco";
        public string DELETE_QUERY => "DELETE FROM COMTIPREG WHERE Cocodso = @Cocodso AND causcon = @causcon AND cauprco = @cauprco";

        public string INSERT_ROW_QUERY => "INSERT INTO COMTIPREGLEVEL1 (Cocodso,causcon,cauprco,Conriga,Cosegno,Cogrup,Cocont,CoSotc,condesc) VALUES(@Cocodso,@causcon,@cauprco,@Conriga,@Cosegno,@Cogrup,@Cocont,@CoSotc,@condesc)";

        public bool Insert(COMTIPREG Model, ObservableCollection<COMTIPREGLEVEL1>? Details, string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();


                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var progressive = connection.Query<short?>($@"SELECT cauprco FROM COMTIPREG WHERE Cocodso = @cid AND causcon = @sid ORDER BY cauprco desc", new { cid = CompanyID, sid = Model.causcon }, transaction).FirstOrDefault() ?? 0;
                        ++progressive;

                        // add groups
                        short rowid = 1;
                        foreach (var group in Details ?? new ObservableCollection<COMTIPREGLEVEL1>())
                        {
                            group.cauprco = progressive;
                            group.Conriga = rowid++;
                            connection.Execute(INSERT_ROW_QUERY, group, transaction);
                        }

                        // add causal
                        Model.cauprco = progressive;
                        Model.corigtot = rowid;
                        connection.Execute(INSERT_QUERY, Model, transaction);
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
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

        public bool Update(COMTIPREG Model, ObservableCollection<COMTIPREGLEVEL1>? Details, string CompanyID)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // clean groups
                        connection.Execute("DELETE FROM COMTIPREGLEVEL1 WHERE Cocodso=@grpsoc AND causcon=@caucod AND cauprco = @prog",
                            new { grpsoc = CompanyID, caucod = Model.causcon, prog = Model.cauprco }, transaction);

                        // add groups
                        short rowid = 1;
                        foreach (var group in Details ?? new ObservableCollection<COMTIPREGLEVEL1>())
                        {
                            group.Conriga = rowid++;
                            connection.Execute(INSERT_ROW_QUERY, group, transaction);
                        }

                        // update causal
                        Model.corigtot = rowid;
                        connection.Execute(UPDATE_QUERY, Model, transaction);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(ex.Message.ToString());
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

        public bool Delete(COMTIPREG Model)
        {
            try
            {
                using var connection = GetOpenConnection();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // clean groups
                        connection.Execute("DELETE FROM COMTIPREGLEVEL1 WHERE Cocodso=@grpsoc AND causcon=@caucod AND cauprco = @prog",
                            new { grpsoc = Model.Cocodso, caucod = Model.causcon, }, transaction);

                        // update causal
                        connection.Execute(DELETE_QUERY, Model, transaction);
                        transaction.Commit();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ErrorHandler.Show(ex.Message.ToString());
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

        public string? Validate(COMTIPREG Model, ObservableCollection<COMTIPREGLEVEL1>? Details, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.causcon) && IsInsert) || !IsInsert)
                {
                    if ((Details ?? new ObservableCollection<COMTIPREGLEVEL1>()).Any())
                    {
                        return null;
                    }
                    else
                    {
                        return "Inserire almeno una riga";
                    }
                }
                else
                { return "Il codice inserito è già in uso o non è valido"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string? ValidateRow(COMTIPREGLEVEL1 Model, bool IsInsert)
        {
            try
            {
                if ((!string.IsNullOrEmpty(Model.causcon)))
                {
                    if (Model.SelectedGroup != null && Model.SelectedAccount != null && Model.SelectedSubaccount != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Model.Cosegno))
                        {
                            return null;
                        }
                        else
                        { return "Il segno è obbligatorio"; }
                    }
                    else
                    { return "Gruppo, conto e sottoconto somo obbligatori"; }
                }
                else
                { return "Il codice della causale è obbligatorio"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
