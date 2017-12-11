using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Bunt.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        // GET: /Home/ClaimsTest
        public ActionResult ClaimsTest()
        {
            IEnumerable<Claim> claims = null;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.Claims != null && identity.Claims.Any())
            {
                claims = identity.Claims;
            }
            return View(claims);
        }
    }
}
