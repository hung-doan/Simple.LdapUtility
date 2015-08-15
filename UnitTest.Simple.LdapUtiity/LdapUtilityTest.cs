using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simple.LdapUtility;

namespace UnitTest.Simple.LdapUtiity
{
    [TestClass]
    public class LdapUtilityTest
    {
        public string validPathByIP = "LDAP://192.168.1.9";
        public string validPathByFullyDNS = "LDAP://rims.local";

        public string validUserName = "test";
        public string validPassword = "Abc123456";

        public string inValidPath = "LDAP://192.168.1._";
        public string inValidUserName = "test_";
        public string inValidPassword = "Abc123456_";
        [TestMethod]
        public void Authenticate_Authorized_Use_IP()
        {
            var utility = new LdapUtility(validPathByIP, validUserName, validPassword);
            var isAuthenticated = utility.Authenticate();
            Assert.IsTrue(isAuthenticated);
        }
        [TestMethod]
        public void Authenticate_Authorized_Use_FullyDNS()
        {
            var utility = new LdapUtility(validPathByFullyDNS, validUserName, validPassword);
            var isAuthenticated = utility.Authenticate();
            Assert.IsTrue(isAuthenticated);
        }
        
        [TestMethod]
        public void Authenticate_Unauthorized_BecauseOf_Wrong_Path()
        {
            var utility = new LdapUtility(inValidPath, validUserName, validPassword);
            var isAuthenticated = utility.Authenticate();
            Assert.IsFalse(isAuthenticated);
        }
        [TestMethod]
        public void Authenticate_Unauthorized_BecauseOf_Wrong_UserName()
        {
            var utility = new LdapUtility(validPathByIP, inValidUserName, validPassword);
            var isAuthenticated = utility.Authenticate();
            Assert.IsFalse(isAuthenticated);
        }
        [TestMethod]
        public void Authenticate_Unauthorized_BecauseOf_Wrong_Password()
        {
            var utility = new LdapUtility(validPathByIP, validUserName, inValidPassword);
            var isAuthenticated = utility.Authenticate();
            Assert.IsFalse(isAuthenticated);
        }
    }
}
