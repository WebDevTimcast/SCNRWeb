using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
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
    [Route("archive")]
    public class ArchiveController : Controller
    {
        private const int ITEMS_PER_PAGE = 15;
        private readonly ILogger logger;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;

        public ArchiveController(ILogger<ArchiveController> logger, ContentService contentService, ONUserHelper userHelper)
        {
            this.logger = logger;
            this.contentService = contentService;
            this.userHelper = userHelper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet("{year}")]
        public IActionResult ByYear(int year)
        {
            if (!IsValidYear(year))
                return RedirectPermanent("/");

            return View(year);
        }

        [HttpGet("{year}/{month}")]
        [HttpGet("{year}/{month}/page/{pageNum}")]
        public async Task<IActionResult> ByMonth(int year, int month, int pageNum = 1)
        {
            if (!IsValidYearMonth(year, month))
                return RedirectPermanent("/");

            var bom = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var eom = bom.AddMonths(1).AddSeconds(-1);
            var res = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                PublishedAfterUTC = Timestamp.FromDateTime(bom),
                PublishedBeforeUTC = Timestamp.FromDateTime(eom),
            });


            var model = new ByMonthViewModel(res, userHelper.MyUser)
            {
                Date = bom,
                PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/archive/{year}/{month}/page/"),
            };

            return View(model);
        }

        private bool IsValidYear(int year)
        {
            if (year < 2021)
                return false;

            if (year > DateTime.UtcNow.Year)
                return false;

            return true;
        }

        private bool IsValidYearMonth(int year, int month)
        {
            if (!IsValidYear(year))
                return false;

            if (month < 1)
                return false;

            if (month > 12)
                return false;

            var date = new DateTime(year, month, 1);

            if (date < new DateTime(2021, 6, 1, 0, 0, 0, DateTimeKind.Utc))
                return false;

            if (date > DateTime.UtcNow)
                return false;

            return true;
        }
    }
}
