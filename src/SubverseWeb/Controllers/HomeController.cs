using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ON.Authentication;
using SubverseWeb.Models;
using SubverseWeb.Models.Auth;
using SubverseWeb.Models.CMS;
using SubverseWeb.Services;

namespace SubverseWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
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
                PageSize = 30,
                ContentType = ON.Fragments.Content.ContentType.Written,
            })), userHelper.MyUser));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            return View("Home", new HomeViewModel((await contentService.Search(query ?? "")), userHelper.MyUser));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
