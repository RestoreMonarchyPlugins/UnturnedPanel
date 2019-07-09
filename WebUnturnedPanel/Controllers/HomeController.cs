using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.WebUnturnedPanel.ActionFilters;
using RestoreMonarchy.WebUnturnedPanel.Helpers;
using RestoreMonarchy.WebUnturnedPanel.Managers;
using RestoreMonarchy.WebUnturnedPanel.Models;

namespace RestoreMonarchy.WebUnturnedPanel.Controllers
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

        [TypeFilter(typeof(RequireModuleAttribute), Arguments = new object[]{ "Moderation" })]
        [Route("/")]
        public IActionResult Bans()
        {
            return View(banManager.GetPlayersBans());            
        }

        [HttpPost("/bans")]
        public string PostBan(ulong playerId, ulong punisherId, string reason, int? duration)
        {
            if (playerManager.IsSigned && playerManager.Player.Role != 'D')
            {
                if (banManager.AddBan(playerId, punisherId, reason, duration))
                    return "Successfully banned " + playerId;
                else
                    return "Failed to find " + playerId;
            }
            
            return "No permissions";
        }

        [HttpDelete("/bans")]
        public string DeleteBan(int banId)
        {
            if (playerManager.IsSigned && playerManager.Player.Role != 'D')
            {
                if (banManager.DeleteBan(banId))
                    return "Successfully revoked ban " + banId;
                else
                    return "There wasn't any ban with ID " + banId;
            }

            return "No permissions";
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
