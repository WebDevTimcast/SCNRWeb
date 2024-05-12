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
using SCNRWeb.Models.Admin;
using SCNRWeb.Models.CMS;
using SCNRWeb.Models.CMS.Categories;
using SCNRWeb.Models.CMS.Channels;
using SCNRWeb.Services;

namespace SCNRWeb.Controllers
{
    [Authorize(Roles = ONUser.ROLE_IS_ADMIN_OR_OWNER)]
    [Route("channel")]
    public class ChannelController : Controller
    {
        private readonly ILogger logger;
        private readonly AssetService assetService;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;
        private readonly SettingsService settingsService;
        private const int ITEMS_PER_PAGE = 24;

        public ChannelController(ILogger<ChannelController> logger, AssetService assetService, ContentService contentService, ONUserHelper userHelper, SettingsService settingsService)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.contentService = contentService;
            this.userHelper = userHelper;
            this.settingsService = settingsService;
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    var model = new CategoryIndexViewModel();
        //    model.Categories = (await settingsService.GetCategories()).OrderBy(c => c.DisplayName).ToList();

        //    return View(model);
        //}

        [AllowAnonymous]
        [HttpGet("{slug}")]
        [HttpGet("{slug}/page/{pageNum}")]
        public async Task<IActionResult> ChannelPage(string slug, int pageNum = 1)
        {
            if (pageNum < 1)
                return RedirectToAction(nameof(Index));

            var channel = await settingsService.GetChannelBySlug(slug);
            if (channel == null)
                return RedirectToAction(nameof(Index));

            var res = await contentService.GetAll(new()
            {
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                ContentType = ContentType.Video,
                ChannelId = channel.ChannelId,
            });
            if (res == null)
                return NotFound();

            var model = new ChannelViewModel();
            model.ChannelRecord = channel;
            model.ContentRecords = res.Records.ToList();
            model.PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/channel/{slug}/page/");

            return View("View", model);
        }

        [HttpGet("/admin/channel")]
        public async Task<IActionResult> Manage()
        {
            var list = await settingsService.GetChannels();

            var model = new ManageChannelsViewModel();
            model.Records = list;

            return View("ManageList", model);
        }

        [HttpGet("/admin/channel/new")]
        public IActionResult NewChannelGet()
        {
            return View("New", new NewChannelViewModel());
        }

        [HttpPost("/admin/channel/new")]
        public async Task<IActionResult> NewChannelPost(NewChannelViewModel vm)
        {
            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("New", vm);
            }

            var res = await settingsService.CreateChannel(vm, userHelper.MyUser);

            return Redirect("/admin/channel/");
        }

        [HttpGet("/admin/channel/{id}")]
        public async Task<IActionResult> EditChannelGet(string id)
        {
            var res = await settingsService.GetChannelById(id);
            if (res == null)
                return NotFound();

            EditChannelViewModel vm = new()
            {
                ID = id,
                DisplayName = res.DisplayName,
                UrlStub = res.UrlStub,
                ImageAssetID = res.ImageAssetId,
                YoutubeUrl = res.YoutubeUrl,
                RumbleUrl = res.RumbleUrl,
            };

            return View("Edit", vm);
        }

        [HttpPost("/admin/channel/{id}")]
        public async Task<IActionResult> EditChannelPost(string id, EditChannelViewModel vm)
        {
            var res = await settingsService.GetChannelById(id);
            if (res == null)
                return NotFound();

            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;

                vm.ID = id;
                return View("Edit", vm);
            }

            var res2 = await settingsService.ModifyChannel(id, vm, userHelper.MyUser);

            return Redirect("/admin/channel/");
        }

        [HttpGet("/admin/channel/{id}/pickImage")]
        public async Task<IActionResult> PickImage(string id, string s, int pageNum = 1)
        {
            int ITEMS_PER_PAGE = 24;

            var res = await assetService.SearchImages(new()
            {
                AssetType = AssetType.Image,
                PageSize = (uint)ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                Query = s ?? "",
            });

            var model = new PickImageViewModel(res)
            {
                Id = id,
                PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/admin/channel/{id}/pickImage?s={s}&pageNum="),
            };

            return View("PickImage", model);
        }

        [HttpGet("/admin/channel/{id}/imagePicked/{aid}")]
        public async Task<IActionResult> ImagePicked(string id, string aid)
        {
            Guid channelId;
            if (!Guid.TryParse(id, out channelId))
                return RedirectToAction(nameof(Manage));

            Guid assetId;
            if (!Guid.TryParse(aid, out assetId))
                return RedirectToAction(nameof(EditChannelGet), new { id });

            var res = await settingsService.UpdateChannelAssetID(channelId, assetId, userHelper.MyUser);

            return RedirectToAction(nameof(EditChannelGet), new { id });
        }
    }
}
