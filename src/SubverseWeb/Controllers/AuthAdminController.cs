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
using ON.Fragments.Generic;
using SubverseWeb.Models;
using SubverseWeb.Models.Auth;
using SubverseWeb.Models.Auth.Admin;
using SubverseWeb.Services;

namespace SubverseWeb.Controllers
{
    [Authorize(Roles = ONUser.ROLE_IS_ADMIN_OR_OWNER)]
    [Route("admin/auth")]
    public class AuthAdminController : Controller
    {
        private readonly ILogger<AuthAdminController> logger;
        private readonly UserService userService;

        public AuthAdminController(ILogger<AuthAdminController> logger, UserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> ListUsers()
        {
            var v = new ListUsersViewModel
            {
                UserRecords = (await userService.GetUserList()).OrderBy(u => u.Public.Data.UserName).ToArray(),
            };

            return View(v);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> EditUser(string id, string success, string error)
        {
            var userId = Guid.Parse(id);
            var r = await userService.GetOtherUser(userId);

            var vm = new EditUserViewModel(r);
            vm.ErrorMessage = error;
            vm.SuccessMessage = success;

            var totps = await userService.GetOtherTotp(userId);
            vm.TotpDevices = totps?.Devices?.ToList() ?? new();

            return View(vm);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> EditUserPost(string id, EditUserViewModel vm)
        {
            var userId = Guid.Parse(id);

            var totps = await userService.GetOtherTotp(userId);
            vm.TotpDevices = totps?.Devices?.ToList() ?? new();

            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("EditUser", vm);
            }

            var res = await userService.ModifyOtherUser(userId, vm);
            if (!string.IsNullOrEmpty(res.Error))
            {
                vm.ErrorMessage = res.Error;
                return View("EditUser", vm);
            }

            var user = await userService.GetOtherUser(userId);
            if (user == null)
                return RedirectToAction(nameof(Error));

            vm = new EditUserViewModel(user)
            {
                SuccessMessage = "Settings updated Successfully"
            };

            return View("EditUser", vm);
        }

        [HttpGet("{id}/changepassword")]
        public IActionResult ChangePasswordGet(string id)
        {
            var vm = new ChangePasswordOtherViewModel();

            return View("ChangePasswordOther", vm);
        }

        [HttpPost("{userId}/changepassword")]
        public async Task<IActionResult> ChangePasswordPost(ChangePasswordOtherViewModel vm)
        {
            vm.ErrorMessage = vm.SuccessMessage = "";
            if (!ModelState.IsValid)
            {
                vm.ErrorMessage = ModelState.Values.FirstOrDefault(v => v.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                                        ?.Errors?.FirstOrDefault()?.ErrorMessage;
                return View("ChangePasswordOther", vm);
            }

            var error = await userService.ChangePasswordOtherUser(vm);
            switch (error)
            {
                case ON.Fragments.Authentication.ChangeOtherPasswordResponse.Types.ChangeOtherPasswordResponseErrorType.NoError:
                    return RedirectToAction(nameof(EditUser), new { id = vm.UserId, success = "Password updated successfully" });
                case ON.Fragments.Authentication.ChangeOtherPasswordResponse.Types.ChangeOtherPasswordResponseErrorType.BadNewPassword:
                    return View("ChangePasswordOther", new ChangePasswordOtherViewModel { ErrorMessage = "New password is not valid" });
                case ON.Fragments.Authentication.ChangeOtherPasswordResponse.Types.ChangeOtherPasswordResponseErrorType.UnknownError:
                default:
                    return View("ChangePasswordOther", new ChangePasswordOtherViewModel { ErrorMessage = "An unknown error occurred" });
            }
        }

        [HttpPost("{id}/changepic")]
        public async Task<IActionResult> ChangeProfilePic(string id, IFormFile file)
        {
            if (file == null) return RedirectToAction(nameof(EditUser), new { id, error = "No file uploaded" });
            if (file.Length == 0) return RedirectToAction(nameof(EditUser), new { id, error = "No file uploaded" });

            using var stream = file.OpenReadStream();

            var res = await userService.ChangeOtherProfilePicture(id, stream);
            switch (res.Error)
            {
                case ON.Fragments.Authentication.ChangeOtherProfileImageResponse.Types.ChangeOtherProfileImageResponseErrorType.NoError:
                    return RedirectToAction(nameof(EditUser), new { id, success = "Profile pic updated" });
            }

            return RedirectToAction(nameof(EditUser), new { id, error = "Unknown error updating profile pic" });
        }

        [HttpGet("{id}/disable")]
        public async Task<IActionResult> DisableUser(string id)
        {
            var vm = new EditUserViewModel();

            var res = await userService.DisableUser(id);
            switch (res.Error)
            {
                case ON.Fragments.Authentication.DisableEnableOtherUserResponse.Types.DisableEnableOtherUserResponseErrorType.NoError:
                    return RedirectToAction(nameof(EditUser), new { id, success = "The user was successfully disabled" });
                case ON.Fragments.Authentication.DisableEnableOtherUserResponse.Types.DisableEnableOtherUserResponseErrorType.UnknownError:
                    return RedirectToAction(nameof(EditUser), new { id, error = "An error occured" });
            }

            return RedirectToAction(nameof(EditUser), new { id });
        }

        [HttpGet("{id}/enable")]
        public async Task<IActionResult> EnableUser(string id)
        {
            var vm = new EditUserViewModel();

            var res = await userService.EnableUser(id);
            switch (res.Error)
            {
                case ON.Fragments.Authentication.DisableEnableOtherUserResponse.Types.DisableEnableOtherUserResponseErrorType.NoError:
                    return RedirectToAction(nameof(EditUser), new { id, success = "The user was successfully enabled" });
                case ON.Fragments.Authentication.DisableEnableOtherUserResponse.Types.DisableEnableOtherUserResponseErrorType.UnknownError:
                    return RedirectToAction(nameof(EditUser), new { id, error = "An error occured" });
            }

            return RedirectToAction(nameof(EditUser), new { id });
        }

        [HttpGet("{id}/totp/{totpid}/disable")]
        public async Task<IActionResult> DisableTotp(string id, string totpid)
        {
            await userService.DisableOtherTotp(id.ToGuid(), totpid.ToGuid());

            return RedirectToAction(nameof(EditUser), new { id });
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
