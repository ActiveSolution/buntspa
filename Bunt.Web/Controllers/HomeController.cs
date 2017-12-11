using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Bunt.Core.Security;

namespace Bunt.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ICurrentUser _currentUser;
        public HomeController(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }

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
            var claims = _currentUser.Claims;
            return View(claims);
        }
    }
}
