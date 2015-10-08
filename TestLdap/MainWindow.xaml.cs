using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Simple.LdapUtility;

namespace TestLdap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModels _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModels();
            DataContext = _viewModel;
        }

        private void ribbonButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private bool validate(string ldapServer, string userName, string password)
        {
            if (string.IsNullOrEmpty(ldapServer))
            {
                MessageBoxResult msgBoxResult = MessageBox.Show("LDAP Server can't be null or empty");
                return false;
            }
            if (string.IsNullOrEmpty(userName))
            {
                MessageBoxResult msgBoxResult = MessageBox.Show("UserName can't be null or empty");
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBoxResult msgBoxResult = MessageBox.Show("Password can't be null or empty");
                return false;
            }
            //
            if (ldapServer.Substring(0, 7) != "LDAP://")
            {
                MessageBoxResult msgBoxResult = MessageBox.Show(@"LDAP Server have to be started with ""LDAP://""(this is case sensitive)");
                return false;
            }
            return true;
        }

        private bool TestAuthenticate(string ldapServer, string userName, string pwd)
        {
            _viewModel.AddToProgress(DateTime.Now,
                $@"Authenthicate",
                "BEGIN",
                true,
                $@">User:'{userName}'" + Environment.NewLine + $">LDAP:'{ldapServer}'");


            var utility = new LdapUtility(ldapServer, userName, pwd);

            COMException exception;
            var isAuthenticated = utility.Authenticate(out exception);

            var progress = _viewModel.AddToProgress(DateTime.Now,
                $@"Authenthicate",
                $"Authenticated: {isAuthenticated}",
                isAuthenticated,
                $@">User:'{userName}'" + Environment.NewLine + $">LDAP:'{ldapServer}'");
            //
            if (!isAuthenticated)
            {
                progress.Note += $"\n{exception.Message}";
                if (exception.InnerException != null)
                {
                    progress.Note += $"\n{exception.InnerException.Message}";
                }
                _viewModel.Refresh();
                MessageBoxResult msgBoxResult = MessageBox.Show("Are you typing the wrong UserName/Password?",
                    "Confirmation",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgBoxResult == MessageBoxResult.Yes)
                {
                    progress.Note += "[typing the wrong UserName/Password]";
                    _viewModel.Refresh();
                }
            }
            return isAuthenticated;
        }

        private bool CheckIfUserExist(string ldapServer, string userName, string pwd)
        {
            try
            {
                _viewModel.AddToProgress(DateTime.Now,
                $@"FindUser",
                "BEGIN",
                true,
                $@">User:'{userName}'"+Environment.NewLine+$">LDAP:'{ldapServer}'");
                var utility = new LdapUtility(ldapServer, userName, pwd);
                var userInfo = utility.FindUser(userName);
                _viewModel.AddToProgress(DateTime.Now,
                    $@"FindUser",
                    $"Found user: {userInfo != null}",
                    userInfo != null,
                    $@">User:'{userName}'" + Environment.NewLine + $">LDAP:'{ldapServer}'");
                return userInfo != null;
            }
            catch (Exception ex)
            {
                _viewModel.AddToProgress(DateTime.Now,
                    $@"FindUser",
                    "ERR",
                    true,
                    $@">User:'{userName}'" + Environment.NewLine + $">LDAP:'{ldapServer}'"+Environment.NewLine+$"{ex.Message}");
                return false;
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            btnExport.IsEnabled = false;
            var ldapServer = txtLdapServer.Text;

            var userName = txtUserName.Text;
            var pwd = txtPwd.Password;

            if (!validate(ldapServer, userName, pwd))
            {
                return;
            }
            var testPass = TestAuthenticate(ldapServer, userName, pwd);
            if (testPass)
            {
                testPass = CheckIfUserExist(ldapServer, userName, pwd);

            }
            if (testPass)
            {
                lblTestResult.Content = $"TEST RESULT: PASS";
                lblTestResult.Foreground = new SolidColorBrush(Colors.Green);
            }
            else
            {
                lblTestResult.Content = $"TEST RESULT: FAIL";
                lblTestResult.Foreground = new SolidColorBrush(Colors.Red);
            }

            MessageBox.Show("Complete!");
            btnExport.IsEnabled = true;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var  dlg = new SaveFileDialog();
            var date = DateTime.Now;
            dlg.FileName = $"LDAP_TESTING_{date.Month}_{date.Day}_{date.Year}"; // Default file name

            dlg.DefaultExt = ".csv"; // Default file extension
                                     // Show save file dialog box
            
            if (dlg.ShowDialog() == true)
            {
                StringBuilder str = new StringBuilder();
                string filename = dlg.FileName;
                str.AppendLine("Date,Title,Status,Note,Passed");
                foreach (var item in _viewModel._testProgress)
                {
                    str.AppendLine($@"""{item.Date}"",""{item.Title}"",""{item.Status}"",""{item.Note}"",""{item.Passed}""");
                }
                using (var sw = File.CreateText(filename))
                {
                   sw.Write(str.ToString());
                    sw.Close();
                }
                MessageBox.Show("Complete!");
            }
        }
    }
}
