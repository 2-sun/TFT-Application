using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

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
            element.SendKeys(Keys.Enter);

            try
            {

                element = driver.FindElement(By.XPath("/html/body/div[2]/div/div/div[2]/div/div/div[2]/div/div/div/span")); // login error

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
        public Player(string key)
        {

            APIKEY = key;

        }

        private void DriverInit()
        {

            service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = false;

            options = new ChromeOptions();
            options.AddArgument("--incognito");
            options.AddArgument("--headless");
            options.AddArgument("start-maximized");
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36");

        }
        //connection.setRequestProperty("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36");
        //connection.setRequestProperty("Accept-Language", "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
        //connection.setRequestProperty("Accept-Charset", "application/x-www-form-urlencoded; charset=UTF-8");
        //connection.setRequestProperty("Origin", "https://developer.riotgames.com");
        //connection.setRequestProperty("X-Riot-Token", riotApi);

        /// <summary>
        /// send request to riot
        /// </summary>
        /// <param name="url"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentException">input error</exception>
        /// <exception cref="System.Security.SecurityException">security error</exception>
        /// <exception cref="UriFormatException">url input error</exception>
        /// <exception cref="InvalidOperationException">wrong request error</exception>
        /// <exception cref="ProtocolViolationException"></exception>
        /// <exception cref="WebException">internal error</exception>
        /// <returns>received http web response</returns>
        private HttpWebResponse RequestRiot(string url)
        {

            HttpWebRequest request = WebRequest.CreateHttp(url);

            request.Method = "GET";
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.74 Safari/537.36");
            request.Headers.Add("Accept-Language", "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
            request.Headers.Add("Accept-Charset", "application/x-www-form-urlencoded; charset=UTF-8");
            request.Headers.Add("Origin", "https://developer.riotgames.com");
            request.Headers.Add("X-Riot-Token", APIKEY);

            return (HttpWebResponse) request.GetResponse();
        }

        public PSession RequestSummoner(string name)
        {
            PSession result = new PSession();

            JObject json = JObject.Parse(ResponseToString(RequestRiot("https://kr.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + HttpUtility.UrlEncode(name).Replace("+", "%20"))));


            result.playerName = json["name"].ToString();
            result.id = json["id"].ToString();
            result.accountID = json["accountId"].ToString();
            result.pUUID = json["puuid"].ToString();
            result.iconID = Int32.Parse(json["profileIconId"].ToString());
            result.revisionDate = Int64.Parse(json["revisionDate"].ToString());
            result.level = Int32.Parse(json["summonerLevel"].ToString());

            return result;

        }

        public Dictionary<string, int> RequestPlayerHistory(PSession session, int count)
        {

            if (!session.CheckEmpty())
            {

                throw new PlayerException("player session is not set");

            }

            Dictionary<string, int> result = new Dictionary<string, int>();

            String matchIds = ResponseToString(RequestRiot("https://asia.api.riotgames.com/tft/match/v1/matches/by-puuid/" + session.pUUID + "/ids?count=" + count));

            foreach (var matchId in matchIds.Replace("[", "").Replace("]", "").Replace("\"", "").Split(","))
            {

                JObject jobject = JObject.Parse(ResponseToString(RequestRiot("https://asia.api.riotgames.com/tft/match/v1/matches/" + matchId)));

                foreach (var participant in jobject["info"]["participants"].Children())
                {

                    JObject participantJson = JObject.Parse(participant.ToString());

                    if (session.pUUID != participantJson["puuid"].ToString()) continue;

                    if (participantJson["units"] == null) continue;

                    foreach (var unit in participantJson["units"].Children())
                    {

                        string unitName = unit["character_id"].ToString();

                        if (result.ContainsKey(unitName))
                        {

                            result[unitName] += UnitCount(unit.ToString());
                            continue;

                        }

                        result.Add(unitName, UnitCount(unit.ToString()));

                    }

                }



            }

            return result;
        }

        private string ResponseToString(HttpWebResponse response)
        {

            Stream? stream = null;

            StreamReader? reader = null;

            string? responseString = null;

            try
            {

                stream = response.GetResponseStream();

                reader = new StreamReader(stream);

                responseString = reader.ReadToEnd();

            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());

            }
            finally
            {

                if (response != null) response.Dispose();
                if (stream != null) stream.Dispose();
                if (reader != null) reader.Dispose();

            }

            return responseString;

        }

        private int UnitCount(string unit)
        {
            int count = 0;

            JObject unitJson = JObject.Parse(unit);

            int tier = Int32.Parse(unitJson["tier"].ToString());

            switch (tier) 
            {

                case 1:

                    count += 1;

                    break;

                case 2:

                    count += 3;

                    break;

                case 3:

                    count += 9;

                    break;

            }

            foreach (var i in unitJson["items"].Children())
            {

                count++;

            }

            return count;

            
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
