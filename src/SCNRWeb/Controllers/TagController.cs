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
using ON.Fragments.Content;
using SubverseWeb.Models.CMS;
using SubverseWeb.Services;

namespace SubverseWeb.Controllers
{
    [AllowAnonymous]
    [Route("tag")]
    public class TagController : Controller
    {
        private readonly ILogger logger;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;
        private readonly SettingsService settingsService;
        private const int ITEMS_PER_PAGE = 15;

        public TagController(ILogger<TagController> logger, ContentService contentService, ONUserHelper userHelper, SettingsService settingsService)
        {
            this.logger = logger;
            this.contentService = contentService;
            this.userHelper = userHelper;
            this.settingsService = settingsService;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var model = new TagIndexViewModel();
        //    model.Tags = (await contentService.GetAllTags()).OrderBy(c => c.DisplayName).ToList();

        //    return View(model);
        //}

        [AllowAnonymous]
        [HttpGet("{tag}")]
        [HttpGet("{tag}/page/{pageNum}")]
        public async Task<IActionResult> CategoryPage(string tag, int pageNum = 1)
        {
            if (pageNum < 1)
                return Redirect("/");

            if (string.IsNullOrEmpty(tag))
                return Redirect("/");

            var res = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                ContentType = ContentType.Written,
                Tag = tag,
            });
            if (res == null)
                return NotFound();

            var model = new TagViewModel();
            model.Tag = tag;
            model.ContentRecords = res.Records.ToList();
            model.PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/tag/{tag}/page/");

            return View("View", model);
        }
    }
}
