using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace TFT
{
    public class Player
    {
        public string APIKEY { get; set; }

        private ChromeDriver? driver = null;
        private ChromeDriverService? service = null;
        private ChromeOptions? options = null;

        public Player(string id, string pw)
        {

            DriverInit();

            driver = new ChromeDriver(service, options);

            driver.Navigate().GoToUrl("https://auth.riotgames.com/login#client_id=riot-developer-portal&redirect_uri=https%3A%2F%2Fdeveloper.riotgames.com%2Foauth2-callback&response_type=code&scope=openid%20email%20summoner&ui_locales=en");
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/div[2]/div/div/div/div[1]/div/input")); // id
            element.SendKeys(id);

            element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/div[2]/div/div/div/div[2]/div/input")); // pw
            element.SendKeys(pw);


            element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/button")); // login button
            element.Click();

            try
            {

                element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/div[2]/div/div/div/span")); // login button

                driver.Dispose();

                throw new PlayerException("wrong id/pw");

            }
            catch (NoSuchElementException)
            {

                element = driver.FindElement(By.XPath("//*[@id='apikey']")); // api key

                Console.WriteLine("API KEY : " + element.GetDomProperty("value"));

                APIKEY = element.GetDomProperty("value");

                driver.Dispose();

            }
            

        }


        private void DriverInit()
        {

            service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            options = new ChromeOptions();
            options.AddArgument("disable-gpu");

        }


    }

    class PlayerException : Exception
    {

        public PlayerException()
        {
        }

        public PlayerException(string message) : base(message)
        {
        }

        public PlayerException(string message, Exception inner) : base(message, inner)
        {
        }

    }
}
