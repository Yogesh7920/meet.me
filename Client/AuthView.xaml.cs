using System;
using System.Collections.Generic;
using System.Linq;
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
using Client.ViewModels;

namespace Client
{
    public partial class LoginWindow : Window
    {
        private MainWindow obj = new MainWindow();
        public LoginWindow()
        {
            InitializeComponent();
            //
            AuthViewModel viewmodel = new AuthViewModel();
            this.DataContext = viewmodel;
        }
        
       /* private void OnJoinClick(object sender, RoutedEventArgs e)
        {
            obj.Show();
            Close();
            // Page mainPage = new MainPage();
            //MainFrame.Navigate(mainPage);
        }*/
       private void OnJoinClick(object sender, RoutedEventArgs e)
       {
           var ip = this.IpBox.Text;
           var port = this.PortNumberBox.Text;
           var username = this.UsernameBox.Text;

           if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port) ||
               string.IsNullOrWhiteSpace((username)))
           {
               ErrorMsg.Visibility = System.Windows.Visibility.Visible;
           }
           else
           {
               AuthViewModel viewmodel = this.DataContext as AuthViewModel;
               var result = viewmodel.SendForAuth(ip, Convert.ToInt32(port), username);
               if (result == true)
               {
                   obj.Show();
                   Close();
               }
               else
               {
                   ErrorMsg.Visibility = System.Windows.Visibility.Visible;
                  // Close();
               }
           }


       }
       
    }
}