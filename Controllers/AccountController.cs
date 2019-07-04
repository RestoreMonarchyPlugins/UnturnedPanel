using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebUnturnedPanel.Helpers;
using WebUnturnedPanel.Managers;
using WebUnturnedPanel.Models;

namespace WebUnturnedPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly PlayerManager playerManager;
        private readonly BanManager banManager;
        public AccountController(PlayerManager playerManager, BanManager banManager)
        {
            this.playerManager = playerManager;
            this.banManager = banManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                Player player = playerManager.GetPlayer(User.Claims.FirstOrDefault().Value.GetSteamId());
                banManager.GetPlayerBans(ref player);
                ViewData["Player"] = player;
                return View();
            }
            else
            {
                return Redirect("/account/signin");
            }
        }        

        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/account" }, "Steam");
        }

        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "../Account/Index" },
                                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
