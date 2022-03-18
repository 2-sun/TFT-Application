using System;
using System.Windows;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TFT
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            new LoginWindow().Show();

        }
    }
}
