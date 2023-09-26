using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ON.Authentication;
using SCNRWeb.Models;
using SCNRWeb.Models.Auth;
using SCNRWeb.Models.CMS;
using SCNRWeb.Services;

namespace SCNRWeb.Controllers
{
    public class HomeController : Controller
    {
        private const int ITEMS_PER_PAGE = 15;
        private readonly ILogger logger;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;

        public HomeController(ILogger<HomeController> logger, ContentService contentService, ONUserHelper userHelper)
        {
            this.logger = logger;
            this.contentService = contentService;
            this.userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View("Home", new HomeViewModel((await contentService.GetAll(new()
            {
                PageOffset = 0,
                PageSize = 24,
            })), userHelper.MyUser));
        }

        [Authorize]
        [HttpGet("/admin")]
        public IActionResult AdminIndex()
        {
            if (userHelper.MyUser.CanCreateContent)
                return RedirectToAction("Manage", "Content");
            return RedirectToAction("SettingsGet", "Auth");
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string s, int pageNum = 1)
        {
            var res = await contentService.Search(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                Query = s,
            });

            var model = new HomeViewModel(res, userHelper.MyUser)
            {
                PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/search/?s={s}&pageNum="),
            };

            return View("Home", model);
        }

        [HttpGet("about-us")]
        public IActionResult AboutUs()
        {
            return View("AboutUs");
        }

        [HttpGet("staff")]
        public IActionResult Staff()
        {
            return View("Staff");
        }

        [HttpGet("privacy")]
        public IActionResult PrivacyPolicy()
        {
            return View("Privacy");
        }

        [HttpGet("terms")]
        public IActionResult TermsAndConditions()
        {
            return View("Terms");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
