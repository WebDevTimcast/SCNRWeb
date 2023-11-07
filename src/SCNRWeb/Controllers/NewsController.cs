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
using SCNRWeb.Models.CMS.News;
using SCNRWeb.Services;

namespace SCNRWeb.Controllers
{
    public class NewsController : Controller
    {
        private const int ITEMS_PER_PAGE = 24;
        private readonly ILogger logger;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;

        public NewsController(ILogger<NewsController> logger, ContentService contentService, ONUserHelper userHelper)
        {
            this.logger = logger;
            this.contentService = contentService;
            this.userHelper = userHelper;
        }

        [HttpGet("/news")]
        [HttpGet("/news/page/{pageNum}")]
        public async Task<IActionResult> Index(int pageNum = 1)
        {
            if (pageNum < 1)
                return RedirectToAction(nameof(Index));

            var res = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                ContentType = ON.Fragments.Content.ContentType.Written,
            });
            if (res == null)
                return NotFound();

            var model = new NewsIndexViewModel()
            {
                Records = res.Records.ToList(),
                PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, "/news/page/"),
            };
            return View("Index", model);
        }
    }
}
