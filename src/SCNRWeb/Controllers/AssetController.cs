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
using SubverseWeb.Models.Asset;
using SubverseWeb.Services;

namespace SubverseWeb.Controllers
{
    [Authorize(Roles = ONUser.ROLE_CAN_CREATE_CONTENT)]
    public class AssetController : Controller
    {
        private readonly ILogger logger;
        private readonly AssetService assetService;
        private readonly ONUserHelper userHelper;
        private readonly UserService userService;
        private const int ITEMS_PER_PAGE = 24;

        public AssetController(ILogger<AssetController> logger, AssetService assetService, UserService userService, ONUserHelper userHelper)
        {
            this.logger = logger;
            this.assetService = assetService;
            this.userHelper = userHelper;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpGet("/image/{id}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> Get(string id)
        {
            Guid assetId;
            if (!Guid.TryParse(id, out assetId))
                return NotFound();

            var res = await assetService.GetImage(assetId);
            if (res == null)
                return NotFound();

            return File(res.Data.Data.ToByteArray(), res.Data.MimeType);
        }

        [HttpGet("/admin/image/{id}/detail")]
        public async Task<IActionResult> ImageDetail(string id)
        {
            Guid assetId;
            if (!Guid.TryParse(id, out assetId))
                return NotFound();

            var res = await assetService.GetImage(assetId);
            if (res == null)
                return NotFound();

            return View(new ImageDetailViewModel(res));
        }

        [HttpGet("/admin/image/new")]
        [HttpPost("/admin/image/new")]
        public async Task<IActionResult> NewImage(NewImageViewModel vm)
        {
            if (Request.Method == "POST")
            {
                vm.ErrorMessage = vm.SuccessMessage = "";
                if (!ModelState.IsValid)
                {
                    vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                            ?.Errors?.FirstOrDefault()?.ErrorMessage;
                    return View(vm);
                }

                var res = await assetService.CreateImage(vm.File, vm.Title, vm.Caption);

                return RedirectToAction(nameof(ImageLibrary));
            }
            return View(vm);
        }

        [HttpGet("/admin/image")]
        [HttpPost("/admin/image")]
        public async Task<IActionResult> ImageLibrary(string s, int pageNum = 1)
        {
            var res = await assetService.SearchImages(new()
            {
                AssetType = AssetType.Image,
                PageSize = ITEMS_PER_PAGE,
                PageOffset = (uint)((pageNum - 1) * ITEMS_PER_PAGE),
                Query = s ?? "",
            });

            var model = new ImageSearchViewModel(res)
            {
                PageVM = new(pageNum, ((int)res.PageTotalItems + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE, $"/admin/image/?s={s}&pageNum="),
            };

            return View("ImageLibrary", model);
        }
    }
}
