using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Simple.LdapUtility
{
    public class LdapUtility
    {
        private readonly string _adPath;
        private readonly string _userName;
        private readonly string _password;
        private readonly AuthenticationTypes _authType;
        /// <summary>
        /// Authorize the user input by UserName and password with default AuthenticationTypes:
        /// AuthenticationTypes = Secure
        /// </summary>
        /// <param name="adPath">LDAP://HostName[:PortNumber][/DistinguishedName]</param>
        /// <param name="userName">User name</param>
        /// <param name="password">User password in plain text</param>
        /// <returns>true if user is authorized</returns>
        /// <example>
        /// var ldapUtility = new LdapUtility("LDAP://server01:390", "hungdoan", "pwd");
        /// </example>
        public LdapUtility(string adPath, string userName, string password)
        {
            _adPath = adPath;
            _userName = userName;
            _password = password;
            _authType = AuthenticationTypes.Secure;
        }
        
        public bool Authenticate()
        {
            DirectoryEntry entry = new DirectoryEntry(
                _adPath,
                _userName,
                _password,
                _authType);
            try
            {
                //Bind to the native AdsObject to force authentication.			
                var obj = entry.NativeObject;
                return true;
            }
            catch (COMException ex)
            {
                return false;
            }
            finally
            {
                entry.Dispose();
            }
        }
        public bool Authenticate(out COMException outPutException)
        {
            DirectoryEntry entry = new DirectoryEntry(
                 _adPath,
                 _userName,
                 _password,
                 _authType);
            try
            {
                //Bind to the native AdsObject to force authentication.			
                var obj = entry.NativeObject;

                outPutException = null;
                return true;
            }
            catch (COMException ex)
            {
                outPutException = ex;
                return false;
            }
            finally
            {
                entry.Dispose();
            }
        }
        public LdapUserInfo FindUser(string userName)
        {
            DirectoryEntry entry = new DirectoryEntry(
                this._adPath,
                this._userName,
                this._password,
                _authType);
            try
            {
                string filter = string.Format("(sAMAccountName={0})", EscapeLDAPQueries(userName));

                using (DirectorySearcher search = new DirectorySearcher(
                    entry,
                    filter,
                    new string[]
                        {
                            "cn",
                            "sAMAccountName"
                        }))
                {
                    System.DirectoryServices.SearchResult objResult = search.FindOne();
                    if (objResult == null)
                    {
                        return null;
                    }
                    LdapUserInfo objUser = new LdapUserInfo();
                    objUser.CommonName = objResult.Properties["cn"][0].ToString();
                    objUser.AccountName = objResult.Properties["sAMAccountName"][0].ToString();
                    return objUser;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                entry.Dispose();
            }
        }
        private static string EscapeLDAPQueries(string sInput)
        {
            StringBuilder str = new StringBuilder();
            if (string.IsNullOrEmpty(sInput))
            {
                return sInput;
            }
            for (int i = 0; i < sInput.Length; i++)
            {
                char currentChar = sInput[i];
                switch (currentChar)
                {
                    case '\\':
                        str.Append(@"\5C");
                        break;
                    case '*':
                        str.Append(@"\5C");
                        break;
                    case '(':
                        str.Append(@"\28");
                        break;
                    case ')':
                        str.Append(@"\29");
                        break;
                    case '\u0000':
                        str.Append(@"\00");
                        break;
                    case '/':
                        str.Append(@"\2f");
                        break;

                    case ',':
                        str.Append(@"\,");
                        break;
                    case '+':
                        str.Append(@"\+");
                        break;
                    case '"':
                        str.Append(@"\""");
                        break;
                    case '<':
                        str.Append(@"\<");
                        break;
                    case '>':
                        str.Append(@"\>");
                        break;
                    case ';':
                        str.Append(@"\;");
                        break;
                    case '#':
                        str.Append(@"\#");
                        break;
                    case '=':
                        str.Append(@"\=");
                        break;
                    default:
                        str.Append(currentChar);
                        break;
                }
            }
            return str.ToString();
        }

        
    }
}
