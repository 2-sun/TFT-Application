using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace TFT
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {
        bool logining = false;

        public LoginWindow()
        {

            InitializeComponent();

            KeyGesture loginKeyGesture = new KeyGesture(Key.Enter);

            KeyBinding loginKeyBinding = new KeyBinding(ApplicationCommands.Save, loginKeyGesture);

            InputBindings.Add(loginKeyBinding);
        
        }

        private void ClickLoginButton(object sender, RoutedEventArgs e)
        {

            string id = txtBox_id.Text;
            string pw = txtBox_pw.Password;

            btn_login.IsEnabled = false;

            Login(id, pw);

            btn_login.IsEnabled = true;
            
        }

        private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

            if (logining)
            {

                e.CanExecute = false;

            }
            else
            {

                e.CanExecute = true;

            }


        }

        private void CommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

            string id = txtBox_id.Text;
            string pw = txtBox_pw.Password;

            Login(id, pw);

        }


        private void Login(string id, string pw)
        {

            logining = true;

            if (id == "" || pw == "")
            {

                MessageBox.Show("id/pw cannot be empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                logining = false;

                return;

            }

            try
            {

                Player player = new Player(id, pw);

                new MainWindow(player).Show();

                this.Close();

                logining = false;

            }
            catch (PlayerException e)
            {

                if (MessageBox.Show("invalid id/pw\ncheck it and try again ", "Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    Console.WriteLine(e.ToString());
                    logining = false;

                }
                else
                {

                    this.Close();

                }

            }

        }
    }
}
