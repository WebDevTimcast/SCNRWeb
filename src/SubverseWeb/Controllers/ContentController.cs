using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ON.Authentication;
using ON.Fragments.Content;
using ON.Settings;
using SubverseWeb.Models;
using SubverseWeb.Models.CMS;
using SubverseWeb.Services;

namespace SubverseWeb.Controllers
{
    [Authorize(Roles = ONUser.ROLE_CAN_CREATE_CONTENT)]
    public class ContentController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AssetService assetService;
        private readonly ContentService contentService;
        private readonly ONUserHelper userHelper;
        private readonly SettingsService settingsService;
        private const int ITEMS_PER_MANAGE_PAGE = 20;

        public ContentController(ILogger<HomeController> logger, AssetService assetService, ContentService contentService, ONUserHelper userHelper, SettingsService settingsService)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.contentService = contentService;
            this.userHelper = userHelper;
            this.settingsService = settingsService;
        }

        [AllowAnonymous]
        [HttpGet("/content/{id}")]
        [HttpGet("/content/{id}/{stub}")]
        public async Task<IActionResult> Get(string id, string stub)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContent(contentId);
            if (res == null)
                return NotFound();

            if (res.Data.ContentDataOneofCase == ON.Fragments.Content.ContentPublicData.ContentDataOneofOneofCase.Video)
                return View("ViewVideo", res);
            if (res.Data.ContentDataOneofCase == ON.Fragments.Content.ContentPublicData.ContentDataOneofOneofCase.Written)
                return View("ViewWritten", res);
            return NotFound();
        }

        [HttpGet("/admin/content")]
        [HttpGet("/admin/content/page/{pageNum}")]
        public async Task<IActionResult> Manage(int pageNum = 1)
        {
            var res = await contentService.GetAllAdmin(new()
            {
                PageSize = ITEMS_PER_MANAGE_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_MANAGE_PAGE),
            });
            var model = new ManageViewModel();
            model.Records = res.Records.ToList();
            model.PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_MANAGE_PAGE - 1) / ITEMS_PER_MANAGE_PAGE, $"/admin/content/page/");

            return View("Manage", model);
        }

        [HttpGet("/content/video/new")]
        public IActionResult NewVideoGet()
        {
            return View("NewVideo", new NewVideoViewModel(userHelper.MyUser));
        }

        [HttpPost("/content/video/new")]
        public async Task<IActionResult> NewVideoPost(NewVideoViewModel vm)
        {
            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("NewVideo", vm);
            }

            var res = await contentService.CreateContent(vm);

            return Redirect("/content/" + res.Public.ContentID);
        }

        [HttpGet("/content/written/new")]
        public IActionResult NewWrittenGet()
        {
            return View("NewWritten", new NewWrittenViewModel(userHelper.MyUser));
        }

        [HttpPost("/content/written/new")]
        public async Task<IActionResult> NewWrittenPost(NewWrittenViewModel vm)
        {
            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("NewWritten", vm);
            }

            var res = await contentService.CreateContent(vm);

            return Redirect("/content/" + res.Public.ContentID);
        }

        [HttpGet("/content/{id}/edit")]
        public async Task<IActionResult> EditGet(string id)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContentAdmin(contentId);
            if (res == null)
                return NotFound();

            if (res.Public.Data.ContentDataOneofCase == ON.Fragments.Content.ContentPublicData.ContentDataOneofOneofCase.Video)
                return RedirectToAction(nameof(EditVideoGet), new { id });

            if (res.Public.Data.ContentDataOneofCase == ON.Fragments.Content.ContentPublicData.ContentDataOneofOneofCase.Written)
                return RedirectToAction(nameof(EditWrittenGet), new { id });

            return NotFound();
        }

        [HttpGet("/content/{id}/video/edit")]
        public async Task<IActionResult> EditVideoGet(string id)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContentAdmin(contentId);
            if (res == null)
                return NotFound();

            EditVideoViewModel vm = new()
            {
                Title = res.Public.Data.Title,
                Subtitle = res.Public.Data.Description,
                Author = res.Public.Data.Author,
                RumbleVideoId = res.Public.Data.Video.RumbleVideoId,
                YoutubeVideoId = res.Public.Data.Video.YoutubeVideoId,
                Body = res.Public.Data.Video.HtmlBody,
                Level = res.Public.Data.SubscriptionLevel,
            };

            return View("EditVideo", vm);
        }

        [HttpPost("/content/{id}/video/edit")]
        public async Task<IActionResult> EditVideoPost(string id, EditVideoViewModel vm)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContentAdmin(contentId);
            if (res == null)
                return NotFound();

            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("EditVideo", vm);
            }

            var res2 = await contentService.UpdateContent(contentId, vm);

            return Redirect("/content/" + res2.Public.ContentID);
        }

        [HttpGet("/content/{id}/written/edit")]
        public async Task<IActionResult> EditWrittenGet(string id)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContentAdmin(contentId);
            if (res == null)
                return NotFound();

            EditWrittenViewModel vm = new()
            {
                Title = res.Public.Data.Title,
                Subtitle = res.Public.Data.Description,
                Author = res.Public.Data.Author,
                Tags = string.Join(',', res.Public.Data.Tags),
                CategoryID = res.Public.Data.CategoryIds.FirstOrDefault(),
                Body = res.Public.Data.Written.HtmlBody,
                Level = res.Public.Data.SubscriptionLevel,
                FeaturedImageAssetID = res.Public.Data.FeaturedImageAssetID,
            };

            vm.Categories = GetCategoryDropDowns();

            return View("EditWritten", vm);
        }

        [HttpPost("/content/{id}/written/edit")]
        public async Task<IActionResult> EditWrittenPost(string id, EditWrittenViewModel vm)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContentAdmin(contentId);
            if (res == null)
                return NotFound();

            vm.Categories = GetCategoryDropDowns();

            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("EditWritten", vm);
            }

            if (vm.File?.Length > 0)
            {
                var resImage = await assetService.CreateImage(vm.File);
                if ((resImage?.Record?.AssetIDGuid ?? Guid.Empty) != Guid.Empty)
                    vm.FeaturedImageAssetID = resImage.Record.AssetIDGuid.ToString();
            }

            var res2 = await contentService.UpdateContent(contentId, vm);

            return Redirect("/content/" + res2.Public.ContentID);
        }

        [Authorize(Roles = ONUser.ROLE_CAN_PUBLISH)]
        [HttpGet("/content/{id}/publish")]
        public async Task<IActionResult> Publish(string id, string returnUrl)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction(nameof(Manage));
                return Redirect(returnUrl);
            }

            await contentService.PublishContent(contentId);

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction(nameof(Manage));
            return Redirect(returnUrl);
        }

        [Authorize(Roles = ONUser.ROLE_CAN_PUBLISH)]
        [HttpGet("/content/{id}/unpublish")]
        public async Task<IActionResult> Unpublish(string id, string returnUrl)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction(nameof(Manage));
                return Redirect(returnUrl);
            }

            await contentService.UnpublishContent(contentId);

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction(nameof(Manage));
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<SelectListItem> GetCategoryDropDowns()
        {
            List<SelectListItem> categories = new() { new SelectListItem("", "") };

            categories.AddRange(settingsService.GetCategories().Result.OrderBy(c => c.DisplayName).Select(c => new SelectListItem(c.DisplayName, c.CategoryId)));

            return categories;
        }
    }
}
