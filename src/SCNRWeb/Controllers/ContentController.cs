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
using SCNRWeb.Helper;
using SCNRWeb.Models;
using SCNRWeb.Models.CMS;
using SCNRWeb.Services;
using Tim.Frontend.Web.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace SCNRWeb.Controllers
{
    [Authorize(Roles = ONUser.ROLE_CAN_CREATE_CONTENT)]
    public class ContentController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AssetService assetService;
        private readonly ContentService contentService;
        private readonly ContentUrlHelper cUrl;
        private readonly ONUserHelper userHelper;
        private readonly SettingsService settingsService;
        private const int ITEMS_PER_MANAGE_PAGE = 20;
        private static TimeZoneInfo estZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public const string CURRENT_CONTENT_ID = "CURRENT_CONTENT_ID";

        public ContentController(ILogger<HomeController> logger, AssetService assetService, ContentService contentService, ContentUrlHelper cUrl, ONUserHelper userHelper, SettingsService settingsService)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.contentService = contentService;
            this.cUrl = cUrl;
            this.userHelper = userHelper;
            this.settingsService = settingsService;
        }


        [AllowAnonymous]
        [HttpGet("/content/{id}")]
        [HttpGet("/content/{id}/{stub}")]
        public async Task<IActionResult> CanonicalGet(string id, string stub)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var record = await contentService.GetContent(contentId);
            if (record == null)
                return NotFound();

            if (record.Data.ContentDataOneofCase == ContentPublicData.ContentDataOneofOneofCase.Written)
                return RedirectPermanent(cUrl.GeneratePartialArticleUrl(record));

            if (record.Data.ContentDataOneofCase == ContentPublicData.ContentDataOneofOneofCase.Video)
                return RedirectPermanent(cUrl.GeneratePartialVideoUrl(record));

            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet("/article/{stub}_{id}")]
        public Task<IActionResult> GetArticle(string stub, string id)
        {
            return Get(id);
        }

        [AllowAnonymous]
        [HttpGet("/video/{id}")]
        public Task<IActionResult> GetVideo(string id)
        {
            return Get(id);
        }

        private async Task<IActionResult> Get(string id)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return NotFound();

            var res = await contentService.GetContent(contentId);
            if (res == null)
                return NotFound();

            HttpContext.Items[CURRENT_CONTENT_ID] = res.ContentID;

            if (res.Data.ContentDataOneofCase == ContentPublicData.ContentDataOneofOneofCase.Video)
            {
                return View("ViewVideo", res);
            }

            if (res.Data.ContentDataOneofCase == ContentPublicData.ContentDataOneofOneofCase.Written)
            {
                ContentListRecord next = null;
                var category = await settingsService.GetCategoryById(res.Data.CategoryIds.FirstOrDefault() ?? "");
                if (category != null)
                    next = (await contentService.GetAll(new() { CategoryId = category.CategoryId })).Records.AfterOrDefault(r => r.ContentID == res.ContentID);

                var model = new NewsViewModel()
                {
                    Record = res,
                    NextRecord = next,
                    Category = category,
                };
                return View("ViewWritten", model);
            }
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
            return View("NewWritten", new NewWrittenViewModel(userHelper.MyUser) { Categories = GetCategoryDropDowns() });
        }

        [HttpPost("/content/written/new")]
        public async Task<IActionResult> NewWrittenPost(NewWrittenViewModel vm)
        {
            vm.Categories = GetCategoryDropDowns();

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
                ID = res.Public.ContentID,
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

            if (vm.File?.Length > 0)
            {
                var resImage = await assetService.CreateImage(vm.File);
                if ((resImage?.Record?.AssetIDGuid ?? Guid.Empty) != Guid.Empty)
                    vm.FeaturedImageAssetID = resImage.Record.AssetIDGuid.ToString();
            }

            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("EditWritten", vm);
            }

            var res2 = await contentService.UpdateContent(contentId, vm);

            return Redirect("/content/" + res2.Public.ContentID);
        }

        [Authorize(Roles = ONUser.ROLE_CAN_CREATE_CONTENT)]
        [HttpGet("/content/{id}/pickImage")]
        [HttpPost("/content/{id}/pickImage")]
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
                PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/content/{id}/pickImage?s={s}&pageNum="),
            };

            return View("PickImage", model);
        }

        [Authorize(Roles = ONUser.ROLE_CAN_CREATE_CONTENT)]
        [HttpGet("/content/{id}/imagePicked/{aid}")]
        public async Task<IActionResult> ImagePicked(string id, string aid)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
                return RedirectToAction(nameof(Manage));

            Guid assetId;
            if (!Guid.TryParse(aid, out assetId))
                return RedirectToAction(nameof(EditWrittenGet), new { id });

            var res = await contentService.UpdateFeaturedItem(contentId, assetId);

            return RedirectToAction(nameof(EditWrittenGet), new { id });
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

            var res = await contentService.GetContentAdmin(contentId);
            if (res == null)
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction(nameof(Manage));
                return Redirect(returnUrl);
            }

            var vm = new PublishViewModel
            {
                ID = id,
                ReturnUrl = returnUrl ?? Url.Action(nameof(Manage)),
                Title = res.Public.Data.Title,
                PublishOnEST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, estZoneInfo),
            };

            return View(vm);
        }

        [Authorize(Roles = ONUser.ROLE_CAN_PUBLISH)]
        [HttpPost("/content/{id}/publish")]
        public async Task<IActionResult> PublishPost(PublishViewModel vm)
        {
            Guid contentId;
            if (!Guid.TryParse(vm.ID, out contentId))
            {
                if (string.IsNullOrEmpty(vm.ReturnUrl))
                    return RedirectToAction(nameof(Manage));
                return Redirect(vm.ReturnUrl);
            }

            var publishUTC = TimeZoneInfo.ConvertTimeToUtc(vm.PublishOnEST, estZoneInfo);

            await contentService.PublishContent(contentId, publishUTC);

            if (string.IsNullOrEmpty(vm.ReturnUrl))
                return RedirectToAction(nameof(Manage));
            return Redirect(vm.ReturnUrl);
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

        [Authorize(Roles = ONUser.ROLE_CAN_PUBLISH)]
        [HttpGet("/content/{id}/delete")]
        public async Task<IActionResult> Delete(string id, string returnUrl)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction(nameof(Manage));
                return Redirect(returnUrl);
            }

            await contentService.DeleteContent(contentId);

            if (string.IsNullOrEmpty(returnUrl))
                return RedirectToAction(nameof(Manage));
            return Redirect(returnUrl);
        }

        [Authorize(Roles = ONUser.ROLE_CAN_PUBLISH)]
        [HttpGet("/content/{id}/undelete")]
        public async Task<IActionResult> Undelete(string id, string returnUrl)
        {
            Guid contentId;
            if (!Guid.TryParse(id, out contentId))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return RedirectToAction(nameof(Manage));
                return Redirect(returnUrl);
            }

            await contentService.UndeleteContent(contentId);

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
