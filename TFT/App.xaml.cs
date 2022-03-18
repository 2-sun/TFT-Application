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
        public string APIKEY { get; set; }

        private ChromeDriver driver = null;
        private ChromeDriverService service = null;
        private ChromeOptions options = null;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Init();

            InputID:

            Console.WriteLine("Riot ID : ");
            string? id = Console.ReadLine();

            if (id == null) goto InputID;

            InputPW:

            Console.WriteLine("Riot PW : ");
            string? pw = Console.ReadLine();

            if (pw == null) goto InputPW;

            driver = new ChromeDriver(service, options);

            driver.Navigate().GoToUrl("https://auth.riotgames.com/login#client_id=riot-developer-portal&redirect_uri=https%3A%2F%2Fdeveloper.riotgames.com%2Foauth2-callback&response_type=code&scope=openid%20email%20summoner&ui_locales=en");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/input")); // id
            element.SendKeys(id);

            element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/div[2]/div/div/div/div[2]/div/input")); // pw
            element.SendKeys(pw);

            element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/button")); // login button
            element.Click();

            element = driver.FindElement(By.XPath("//*[@id='apikey']")); // api key

            Console.WriteLine("API KEY : " + element.GetDomProperty("value"));

            APIKEY = element.GetDomProperty("value");


        }

        private void Init()
        {

            service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            options = new ChromeOptions();
            options.AddArgument("enable-gpu");

        }
    }
}
