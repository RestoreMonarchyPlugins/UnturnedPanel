using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RestoreMonarchy.WebUnturnedPanel.Managers;
using Microsoft.AspNetCore.Http;

namespace RestoreMonarchy.WebUnturnedPanel.Controllers
{
    public class AccountController : Controller
    {
        private readonly PlayerManager playerManager;
        private readonly BanManager banManager;
        private readonly SteamManager steamManager;
        private readonly IHttpContextAccessor _accessor;
        public AccountController(PlayerManager playerManager, BanManager banManager, SteamManager steamManager, IHttpContextAccessor accessor)
        {
            this.playerManager = playerManager;
            this.banManager = banManager;
            this.steamManager = steamManager;
            this._accessor = accessor;
        }

        public IActionResult Index()
        {            
            if (playerManager.IsSigned)
            {
                return View();
            }
            else
            {
                return Redirect("/signin");
            }
        }        

        [Route("SignIn")]
        public IActionResult SignIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/account" }, "Steam");
        }

        [Route("SignOut")]
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
