*//Check if username/password is valid

var utility = new LdapUtility("LDAP//hungdoan.local", "userName", "password");

var isAuthenticated = utility.Authenticate(); // return true if input password is correct

*//Get exception if username/password is not valid

Exception ex;

var isAuthenticated = utility.Authenticate(out ex);

