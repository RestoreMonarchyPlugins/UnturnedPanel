using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace RestoreMonarchy.WebUnturnedPanel.Managers
{
    public class SteamManager
    {
        private readonly IConfiguration configuration;
        public SteamManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public SteamPlayer GetSteamPlayer(ulong steamId)
        {
            string json = null;
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + 
                    configuration.GetValue<string>("SteamWebAPIKey") + "&steamids=" + steamId);
            }

            if (!string.IsNullOrEmpty(json))
            {
                string playerJson = JObject.Parse(json)["response"]["players"][0].ToString();
                return JsonConvert.DeserializeObject<SteamPlayer>(playerJson);
            } else
            {
                return null;
            }
        }
    }

    public class SteamPlayer
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public int lastlogoff { get; set; }
        public int commentpermission { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public int personastate { get; set; }
        public string realname { get; set; }
        public string primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
        public string loccountrycode { get; set; }
    }

}
