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

        [AllowAnonymous]
        [HttpGet("/image/profile/{id}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            Guid userId;
            if (!Guid.TryParse(id, out userId))
                return NotFound();

            var res = await userService.GetUserPublic(userId.ToString());
            if (res == null)
                return NotFound();

            return File(res.Data.ProfileImagePNG.ToByteArray(), "image/png");
        }
    }
}
