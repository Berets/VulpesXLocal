using Dapper;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL._ConnectionFactory;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared.Utilities;

namespace VulpesX.DAL.Auth
{
    public interface IAuthRepository
    {
        Task<ACCESS?> Login(string Username, string Password);

        ACCESS? Get(string ID);

        ACCESS? GetFull(string ID);

        ObservableCollection<ACCESS>? GetUsers(string SocietaID);

        #region CRUD
        string INSERT_QUERY { get; }
        string UPDATE_QUERY { get; }
        string DELETE_QUERY { get; }

        bool Insert(ACCESS Model);

        bool Update(ACCESS Model);

        bool Delete(ACCESS Model);

        string? Validate(ACCESS Model, bool IsInsert);

        string? ValidateChangePassword(string UserID, string OldPassword, string NewPassword, string ConfirmPassword);
        #endregion

        string? CheckLastUsername();

        void WriteLastLogin(string Username);

    }

    public class AuthDefaultRepository : IAuthRepository
    {
        private readonly IConnectionFactory _factory;

        public AuthDefaultRepository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<ACCESS?> Login(string Username, string Password)
        {
            using var connection = _factory.CreateConnection();

            if (connection != null)
            {
                connection.Open();

                var cleanPassword = Password!.Substring(4, Password!.Length - 4);

                var lookup = new Dictionary<string, ACCESS>();

                var user = await connection.QueryAsync<ACCESS, SOCBASE, ACCESS>(
                        @"SELECT    a.*,s.*
                                    FROM ACCESS as a
                                    INNER JOIN ACCESOCE as l ON acsute = USROLD 
                                    INNER JOIN SOCBASE as s ON l.acssoc = s.SOMCOD
                                    WHERE a.USRID = @uid AND a.USRPWD = @pwd",
                        (a, l) =>
                        {
                            ACCESS? user;
                            if (!lookup.TryGetValue(a.USRID, out user))
                                lookup.Add(a.USRID, user = a);

                            if (user != null)
                            {
                                if (user.EnabledCompanies == null)
                                    user.EnabledCompanies = new List<SOCBASE>();

                                user.EnabledCompanies.Add(l);
                            }
                            return user!;
                        },
                        new { uid = Username, pwd = CryptoHelper.SHA512Hasher(cleanPassword) },
                        splitOn: "SOMCOD");
                return user.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public ACCESS? Get(string ID)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
                    return connection.Query<ACCESS>(
                        "SELECT * FROM ACCESS WHERE USRID = @id",
                        new { id = ID })
                        .FirstOrDefault();
                }
                else
                {
                    ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ACCESS? GetFull(string ID)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
                    var lookup = new Dictionary<string, ACCESS>();

                    return connection.Query<ACCESS, SOCBASE, ACCESS>(
                        @"SELECT *, l.* FROM ACCESS 
                            INNER JOIN ACCESOCE as l ON acsute = USROLD 
                            INNER JOIN SOCBASE as s ON l.acssoc = s.SOMCOD
                            WHERE USRID = @uid",
                        (acc, soce) =>
                        {
                            ACCESS? acce;
                            if (!lookup.TryGetValue(acc.USRID, out acce))
                                lookup.Add(acc.USRID, acce = acc);

                            if (acce.EnabledCompanies == null)
                                acce.EnabledCompanies = new List<SOCBASE>();

                            acce.EnabledCompanies.Add(soce);

                            return acc;
                        },
                        new { uid = ID },
                        splitOn: "SOMCOD")
                        .FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ACCESS>? GetUsers(string SocietaID)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();

                    var user = connection.Query<ACCESS, SOCBASE, ACCESOCE, AZIENDA, AUTH_ACCESS_ROLES, ACCESS>(
                        @"SELECT *, s.*, l.*, z.* FROM ACCESS 
                            INNER JOIN ACCESOCE l ON acsute = USROLD 
                            INNER JOIN SOCBASE as s ON l.acssoc = s.SOMCOD
                            INNER JOIN AZIENDA z ON l.acssoc = z.AZCode
                            LEFT OUTER JOIN AUTH_ACCESS_ROLES r ON r.companyID = z.azcode AND r.userID = USRID
                            WHERE l.acssoc = @SocietaID",
                        (acc,bas, soce, azi, role) =>
                        {
                            if (acc != null)
                            {
                                if (acc.EnabledCompanies == null)
                                    acc.EnabledCompanies = new List<SOCBASE>();

                                acc.SelectedCompany = bas;
                                bas.AccessCompany = soce;

                                acc.EnabledCompanies.Add(bas);
                            }
                            
                            return acc!;
                        },
                        new { SocietaID = SocietaID },
                        splitOn: "acssoc,somcod,AZCode,companyID");

                    return new ObservableCollection<ACCESS>(user);
                }
                else
                {
                    return new ObservableCollection<ACCESS>();
                }
            }
            catch (Exception)
            {
                return new ObservableCollection<ACCESS>();
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO ACCESS (USRID,USRPWD,USROLD,USRPRECOM,USRPRETEMA,USRPREVIEW,usrwidqta,usrwidqtatim) OUTPUT INSERTED.rv VALUES(@USRID,@USRPWD,@USROLD,@USRPRECOM,@USRPRETEMA,@USRPREVIEW,@usrwidqta,@usrwidqtatim)";
        public string UPDATE_QUERY => "UPDATE ACCESS SET USRID = @USRID,USRPWD = @USRPWD,USROLD = @USROLD,USRPRECOM = @USRPRECOM,USRPRETEMA = @USRPRETEMA,USRPREVIEW = @USRPREVIEW,usrwidqta = @usrwidqta,usrwidqtatim = @usrwidqtatim OUTPUT INSERTED.rv WHERE USRID = @USRID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM ACCESS OUTPUT DELETED.rv WHERE USRID = @USRID AND rv = @rv";

        public bool Insert(ACCESS Model)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
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

        public bool Update(ACCESS Model)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
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

        public bool Delete(ACCESS Model)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
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

        public string? Validate(ACCESS Model, bool IsInsert)
        {
            try
            {
                if ((!Model.usrwidqta && !Model.usrwidqtatim.HasValue) || (Model.usrwidqta && Model.usrwidqtatim.HasValue && Model.usrwidqtatim > 0))
                {
                    return null;
                }
                else
                { return "Se attivato il widget [Allarmi quantità magazzino] è necessario specificare un tempo di refresh, altrimenti non va inserito nessun valore"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string? ValidateChangePassword(string UserID, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(OldPassword))
                {
                    if (!string.IsNullOrWhiteSpace(NewPassword))
                    {
                        if (!string.IsNullOrWhiteSpace(ConfirmPassword))
                        {
                            if (NewPassword == ConfirmPassword)
                            {
                                if (Get(UserID)?.USRPWD == CryptoHelper.SHA512Hasher(OldPassword))
                                {
                                    return null;
                                }
                                else
                                { return "La password attuale non e' corretta"; }
                            }
                            else
                            { return "La nuova password e la conferma della password non corrispondono"; }
                        }
                        else
                        { return "La conferma della password e' obbligatoria"; }
                    }
                    else
                    { return "La nuova password e' obbligatoria"; }
                }
                else
                { return "La vecchia password e' obbligatoria"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public string? CheckLastUsername()
        {
            try
            {
                var fullPath = Path.GetTempPath() + "lu.vx";
                if (File.Exists(fullPath))
                {
                    return CryptoHelper.SimpleDecrypt(File.ReadAllText(fullPath));
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteLastLogin(string Username)
        {
            try
            {
                var fullPath = Path.GetTempPath() + "lu.vx";
                File.WriteAllText(fullPath, CryptoHelper.SimpleEncrypt(Username));
            }
            catch (Exception)
            {
            }
        }


    }

    public class AuthUfpRepository : IAuthRepository
    {
        private readonly IConnectionFactory _factory;

        public AuthUfpRepository(IConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<ACCESS?> Login(string Username, string Password)
        {
            using var connection = _factory.CreateConnection();

            if (connection != null)
            {
                connection.Open();

                var lookup = new Dictionary<string, ACCESS>();

                var decipher = connection.QueryFirstOrDefault<string>("SELECT percripto FROM PERSON");

                string cleanUsername = Username.Replace("@ufp.it", string.Empty);

                if (decipher != null)
                {
                    var user = await connection.QueryAsync<ACCESS, SOCBASE, ACCESS>(
                            @"SELECT UTECOD as USRID, UTECOD as USROLD, UTEPWD as USRPWD, t.*,  s.* FROM TUTENTI t
                            INNER JOIN ACCESOCE l ON acsute = UTECOD 
                            INNER JOIN SOCI_BASE as s ON l.acssoc = s.SOMCOD
                            WHERE UTECOD = @uid AND UTEPWD = @pwd",
                            (a, l) =>
                            {
                                ACCESS? user;
                                if (!lookup.TryGetValue(a.USRID, out user))
                                    lookup.Add(a.USRID, user = a);

                                if (user != null)
                                {
                                    if (user.EnabledCompanies == null)
                                        user.EnabledCompanies = new List<SOCBASE>();

                                    user.EnabledCompanies.Add(l);
                                }
                                return user!;
                            },
                            new { uid = cleanUsername, pwd = CryptoHelper.Encrypt64(decipher, Password) },
                            splitOn: "SOMCOD");

                    return user.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public ACCESS? Get(string ID)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
                    return connection.Query<ACCESS>(
                        "SELECT * FROM TUTENTI WHERE UTECOD = @id",
                        new { id = ID })
                        .FirstOrDefault();
                }
                else
                {
                    ErrorHandler.Show(Constants.CONNECTION_CREATION_ERROR);
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ACCESS? GetFull(string ID)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
                    var lookup = new Dictionary<string, ACCESS>();

                    string cleanUsername = ID.Replace("@ufp.it", string.Empty);

                    var user = connection.Query<ACCESS, SOCBASE, ACCESS>(
                            @"SELECT UTECOD as USRID, UTECOD as USROLD, UTEPWD as USRPWD, t.*,  s.* FROM TUTENTI t
                            INNER JOIN ACCESOCE l ON acsute = UTECOD 
                            INNER JOIN SOCI_BASE as s ON l.acssoc = s.SOMCOD
                            WHERE UTECOD = @uid",
                            (a, l) =>
                            {
                                ACCESS? user;
                                if (!lookup.TryGetValue(a.USRID, out user))
                                    lookup.Add(a.USRID, user = a);

                                if (user != null)
                                {
                                    if (user.EnabledCompanies == null)
                                        user.EnabledCompanies = new List<SOCBASE>();

                                    user.EnabledCompanies.Add(l);
                                }
                                return user!;
                            },
                            new { uid = cleanUsername },
                            splitOn: "SOMCOD");

                    return user.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Show(ex.Message);
                return null;
            }
        }

        public ObservableCollection<ACCESS>? GetUsers(string SocietaID)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();

                    var user = connection.Query<ACCESS, SOCBASE,ACCESOCE, ACCESS>(
                        @"SELECT UTECOD as USRID, UTECOD as USROLD, UTEPWD as USRPWD, s.*, l.* FROM TUTENTI
                            INNER JOIN ACCESOCE l ON acsute = UTECOD 
                            INNER JOIN SOCI_BASE as s ON l.acssoc = s.SOMCOD
                            WHERE l.acssoc = @SocietaID",
                        (acc, bas, soce) =>
                        {
                            if (acc != null)
                            {
                                if (acc.EnabledCompanies == null)
                                    acc.EnabledCompanies = new List<SOCBASE>();

                                acc.SelectedCompany = bas;
                                bas.AccessCompany = soce;

                                acc.EnabledCompanies.Add(bas);
                            }

                            return acc!;
                        },
                        new { SocietaID = SocietaID },
                        splitOn: "SOMCOD,acssoc");

                    return new ObservableCollection<ACCESS>(user);
                }
                else
                {
                    return new ObservableCollection<ACCESS>();
                }
            }
            catch (Exception)
            {
                return new ObservableCollection<ACCESS>();
            }
        }

        #region CRUD
        public string INSERT_QUERY => "INSERT INTO ACCESS (USRID,USRPWD,USROLD,USRPRECOM,USRPRETEMA,USRPREVIEW,usrwidqta,usrwidqtatim) OUTPUT INSERTED.rv VALUES(@USRID,@USRPWD,@USROLD,@USRPRECOM,@USRPRETEMA,@USRPREVIEW,@usrwidqta,@usrwidqtatim)";
        public string UPDATE_QUERY => "UPDATE TUTENTI SET UTECOD = @USRID,UTEPWD = @USRPWD,USRPRETEMA = @USRPRETEMA OUTPUT INSERTED.rv WHERE UTECOD = @USRID AND rv = @rv";
        public string DELETE_QUERY => "DELETE FROM ACCESS OUTPUT DELETED.rv WHERE USRID = @USRID AND rv = @rv";

        public bool Insert(ACCESS Model)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
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

        public bool Update(ACCESS Model)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
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

        public bool Delete(ACCESS Model)
        {
            try
            {
                using var connection = _factory.CreateConnection();

                if (connection != null)
                {
                    connection.Open();
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

        public string? Validate(ACCESS Model, bool IsInsert)
        {
            try
            {
                if ((!Model.usrwidqta && !Model.usrwidqtatim.HasValue) || (Model.usrwidqta && Model.usrwidqtatim.HasValue && Model.usrwidqtatim > 0))
                {
                    return null;
                }
                else
                { return "Se attivato il widget [Allarmi quantità magazzino] è necessario specificare un tempo di refresh, altrimenti non va inserito nessun valore"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string? ValidateChangePassword(string UserID, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(OldPassword))
                {
                    if (!string.IsNullOrWhiteSpace(NewPassword))
                    {
                        if (!string.IsNullOrWhiteSpace(ConfirmPassword))
                        {
                            if (NewPassword == ConfirmPassword)
                            {
                                if (Get(UserID)?.USRPWD == CryptoHelper.SHA512Hasher(OldPassword))
                                {
                                    return null;
                                }
                                else
                                { return "La password attuale non e' corretta"; }
                            }
                            else
                            { return "La nuova password e la conferma della password non corrispondono"; }
                        }
                        else
                        { return "La conferma della password e' obbligatoria"; }
                    }
                    else
                    { return "La nuova password e' obbligatoria"; }
                }
                else
                { return "La vecchia password e' obbligatoria"; }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public string? CheckLastUsername()
        {
            try
            {
                var fullPath = Path.GetTempPath() + "lu.vx";
                if (File.Exists(fullPath))
                {
                    return CryptoHelper.SimpleDecrypt(File.ReadAllText(fullPath));
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void WriteLastLogin(string Username)
        {
            try
            {
                var fullPath = Path.GetTempPath() + "lu.vx";
                File.WriteAllText(fullPath, CryptoHelper.SimpleEncrypt(Username));
            }
            catch (Exception)
            {
            }
        }

    }
}
