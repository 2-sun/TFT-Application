using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFT
{
    public class PSession
    {

        public string playerName { get; set; }
        public string id { get; set; }
        public string accountID { get; set; }
        public string pUUID { get; set; }
        public int iconID { get; set; }
        public long revisionDate { get; set; }
        public int level { get; set; }

        public void PrintPlayerInfo()
        {

            Console.WriteLine($"\nname : {playerName}\nid : {id}\naccountId : {accountID}\n puuid : {pUUID}\nprofileIconId : {iconID}\n revisionDate : {revisionDate}\nsummonerLevel : {level}");

        }
    }
}
