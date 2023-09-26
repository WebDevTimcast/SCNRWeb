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
    [Route("category")]
    public class CategoryController : Controller
    {
        private readonly ILogger logger;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;
        private readonly SettingsService settingsService;
        private const int ITEMS_PER_PAGE = 15;

        public CategoryController(ILogger<CategoryController> logger, ContentService contentService, ONUserHelper userHelper, SettingsService settingsService)
        {
            this.logger = logger;
            this.contentService = contentService;
            this.userHelper = userHelper;
            this.settingsService = settingsService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new CategoryIndexViewModel();
            model.Categories = (await settingsService.GetCategories()).OrderBy(c => c.DisplayName).ToList();

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("{slug}")]
        [HttpGet("{slug}/page/{pageNum}")]
        public async Task<IActionResult> CategoryPage(string slug, int pageNum = 1)
        {
            if (pageNum < 1)
                return RedirectToAction(nameof(Index));

            var category = await settingsService.GetCategoryBySlug(slug);
            if (category == null)
                return RedirectToAction(nameof(Index));

            var res = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                ContentType = ContentType.Written,
                CategoryId = category.CategoryId,
            });
            if (res == null)
                return NotFound();

            var model = new CategoryViewModel();
            model.CategoryRecord = category;
            model.ContentRecords = res.Records.ToList();
            model.PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/category/{slug}/page/");

            return View("View", model);
        }
    }
}
