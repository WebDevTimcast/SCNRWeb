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
using ON.Fragments.Authentication;
using ON.Fragments.Generic;
using SCNRWeb.Models;
using SCNRWeb.Models.Auth;
using SCNRWeb.Models.CMS;
using SCNRWeb.Models.CMS.News;
using SCNRWeb.Services;
using static Google.Rpc.Context.AttributeContext.Types;

namespace SCNRWeb.Controllers
{
    public class HomeController : Controller
    {
        private const int ITEMS_PER_PAGE = 24;
        private readonly ILogger logger;
        private readonly ContentService contentService;
        private readonly UserService userService;
        private readonly ONUserHelper userHelper;

        public HomeController(ILogger<HomeController> logger, ContentService contentService, UserService userService, ONUserHelper userHelper)
        {
            this.logger = logger;
            this.contentService = contentService;
            this.userService = userService;
            this.userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            var news = await contentService.GetAll(new() { PageOffset = 0, PageSize = 13, ContentType = ON.Fragments.Content.ContentType.Written });
            var videos = await contentService.GetAll(new() { PageOffset = 0, PageSize = 3, ContentType = ON.Fragments.Content.ContentType.Video });
            var model = new HomeViewModel(news, videos, userHelper.MyUser);

            if (model.LiveId != Guid.Empty)
                model.LiveVideo = await contentService.GetContent(model.LiveId);

            return View("Home", model);
        }

        [Authorize]
        [HttpGet("/admin")]
        public IActionResult AdminIndex()
        {
            if (userHelper.MyUser.CanCreateContent)
                return RedirectToAction("Manage", "Content");
            return RedirectToAction("SettingsGet", "Auth");
        }

        [HttpGet("/search/{s}")]
        [HttpGet("/search/{s}/page/{pageNum}")]
        public async Task<IActionResult> Search(string s, int pageNum = 1)
        {
            if (string.IsNullOrWhiteSpace(s))
                return RedirectToAction("Index");

            var res = await contentService.Search(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                Query = s,
            });

            var model = new SearchViewModel(res, userHelper.MyUser);
            model.PagedRecords = res.Records.ToList();
            model.Query = s;
            model.PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/search/{s.ToString()}/page/");

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("/author/{authorId}")]
        [HttpGet("/author/{authorId}/page/{pageNum}")]
        public async Task<IActionResult> AuthorPage(string authorId, int pageNum = 1)
        {
            if (pageNum < 1)
                return RedirectToAction(nameof(Index));

            UserPublicRecord author;
            Guid authorGuid = authorId.ToGuid();
            author = await userService.GetUserPublic(authorGuid.ToString());

            if (author == null)
                return NotFound();

            var res = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                ContentType = ON.Fragments.Content.ContentType.Written,
                AuthorId = authorId.ToString(),
            });
            if (res == null)
                return NotFound();

            var model = new AuthorViewModel();
            model.PagedRecords = res.Records.ToList();
            model.Author = author;
            model.PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/author/{authorId.ToString()}/page/");

            return View("AuthorPaged", model);
        }

        [HttpGet("about-us")]
        public IActionResult AboutUs()
        {
            return View("AboutUs");
        }

        //[HttpGet("staff")]
        //public IActionResult Staff()
        //{
        //    return View("Staff");
        //}

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
