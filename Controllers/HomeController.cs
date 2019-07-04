using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebUnturnedPanel.Helpers;
using WebUnturnedPanel.Managers;
using WebUnturnedPanel.Models;

namespace WebUnturnedPanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlayerManager playerManager;
        private readonly BanManager banManager;
        public HomeController(PlayerManager playerManager, BanManager banManager)
        {
            this.playerManager = playerManager;
            this.banManager = banManager;
        }

        [Route("/")]
        public IActionResult Bans()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
                ViewData["Player"] = playerManager.GetPlayer(User.Claims.FirstOrDefault().Value.GetSteamId());
            return View(banManager.GetPlayersBans());
        }

        [HttpPost("/bans")]
        public string PostBan(ulong playerId, ulong punisherId, string reason, int? duration)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                Player player = playerManager.GetPlayer(User.Claims.FirstOrDefault().Value.GetSteamId());
                if (player.Role != 'D')
                {
                    if (banManager.AddBan(playerId, punisherId, reason, duration))
                        return "Successfully banned " + playerId;
                    else
                        return "Failed to find " + playerId;
                }                
            }   
            
            return "No permission";
        }

        [HttpDelete("/bans")]
        public string DeleteBan(int banId)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                Player player = playerManager.GetPlayer(User.Claims.FirstOrDefault().Value.GetSteamId());
                if (player.Role != 'D')
                {
                    if (banManager.DeleteBan(banId))
                        return "Successfully revoked ban " + banId;
                    else
                        return "There wasn't any ban with ID " + banId;
                }
            }

            return "No permission";
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
