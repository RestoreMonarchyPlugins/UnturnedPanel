using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;

namespace RestoreMonarchy.WebUnturnedPanel.Managers
{
    public class ImageManager
    {
        private readonly IHostingEnvironment environment;
        private readonly string directory;
        private readonly string avatarDir;


        public ImageManager(IHostingEnvironment environment)
        {
            this.environment = environment;
            this.directory = environment.ContentRootPath + "/wwwroot/img/";
            this.avatarDir = directory + "avatars/";
        }

        public void SaveAvatar(string url, ulong steamId)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(url, avatarDir + steamId + ".jpg");
            }
        }
    }
}
