using System;
using System.Windows;
using System.Windows.Input;
using Food.ViewModels;

namespace Food
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void HideToast_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.IsToastVisible = false;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                // Ensure password is not empty
                if (string.IsNullOrEmpty(LoginPass.Password))
                {
                    vm.ShowToast("Please enter your password.");
                    return;
                }
                vm.Login(new object[] { LoginEmail.Text, LoginPass.Password });
            }
        }

        // --- Password Visibility Logic ---
        private void LoginEyeBtn_Checked(object sender, RoutedEventArgs e)
        {
            LoginPassText.Text = LoginPass.Password;
            LoginPass.Visibility = Visibility.Collapsed;
            LoginPassText.Visibility = Visibility.Visible;
        }

        private void LoginEyeBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            LoginPass.Password = LoginPassText.Text;
            LoginPassText.Visibility = Visibility.Collapsed;
            LoginPass.Visibility = Visibility.Visible;
        }

        private void LoginPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (LoginPass.Visibility == Visibility.Visible)
                LoginPassText.Text = LoginPass.Password;
        }

        private void LoginPassText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (LoginPassText.Visibility == Visibility.Visible)
                LoginPass.Password = LoginPassText.Text;
        }

        private void RegEyeBtn_Checked(object sender, RoutedEventArgs e)
        {
            RegPassText.Text = RegPass.Password;
            RegPass.Visibility = Visibility.Collapsed;
            RegPassText.Visibility = Visibility.Visible;
        }

        private void RegEyeBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            RegPass.Password = RegPassText.Text;
            RegPassText.Visibility = Visibility.Collapsed;
            RegPass.Visibility = Visibility.Visible;
        }

        private void RegPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (RegPass.Visibility == Visibility.Visible)
                RegPassText.Text = RegPass.Password;
        }

        private void RegPassText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (RegPassText.Visibility == Visibility.Visible)
                RegPass.Password = RegPassText.Text;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.Register(new object[] { RegName.Text, RegEmail.Text, RegPass.Password, RegPhone.Text, RegAddr.Text });
            }
        }

        private void ClearNewProduct_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.NewProduct = new Models.Product();
            }
        }
    }
}