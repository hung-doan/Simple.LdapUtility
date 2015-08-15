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
        }
        
        public bool Authenticate()
        {
            DirectoryEntry entry = new DirectoryEntry(
                _adPath,
                _userName,
                _password,
                AuthenticationTypes.Secure);
            try
            {
                //Bind to the native AdsObject to force authentication.			
                var obj = entry.NativeObject;

            }
            catch (COMException ex)
            {
                return false;
            }
            return true;
        }
        public bool Authenticate(out COMException outPutException)
        {
            DirectoryEntry entry = new DirectoryEntry(
                 _adPath,
                 _userName,
                 _password,
                 AuthenticationTypes.Secure);
            try
            {
                //Bind to the native AdsObject to force authentication.			
                var obj = entry.NativeObject;
            }
            catch (COMException ex)
            {
                outPutException = ex;
                return false;
            }

            outPutException = null;
            return true;
        }

        
    }
}
