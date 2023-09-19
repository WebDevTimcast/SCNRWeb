using Microsoft.AspNetCore.Http;
using ON.Authentication;
using System.Diagnostics;
using System.Threading;

namespace SubverseWeb.Models
{
    public class ManageLeftNavViewModel
    {
        public ManageLeftNavViewModel(IHttpContextAccessor context, ONUserHelper user)
        {
            switch (context.HttpContext.Request.Path.Value.ToLower())
            {
                case string s when s.StartsWith("/admin/content"):
                    IsContent = true;
                    break;
                case string s when s.StartsWith("/admin/image"):
                    IsImage = true;
                    break;
                case string s when s.StartsWith("/admin/auth"):
                    IsAuth = true;
                    break;
                case string s when s.StartsWith("/admin/settings"):
                    IsSettings = true;
                    break;
                case string s when s.StartsWith("/settings"):
                    IsProfile = true;
                    break;
            }

            CanSeeContent = user?.MyUser?.CanCreateContent ?? false;
            CanSeeImageLibrary = user?.MyUser?.CanCreateContent ?? false;
            CanSeeAuth = user?.MyUser?.IsAdminOrHigher ?? false;
            CanSeeSettings = user?.MyUser?.IsAdminOrHigher ?? false;
        }

        public bool IsContent { get; set; } = false;
        public bool IsImage { get; set; } = false;
        public bool IsAuth { get; set; } = false;
        public bool IsSettings { get; set; } = false;
        public bool IsProfile { get; set; } = false;

        public bool CanSeeContent { get; set; } = false;
        public bool CanSeeImageLibrary { get; set; } = false;
        public bool CanSeeAuth { get; set; } = false;
        public bool CanSeeSettings { get; set; } = false;
    }
}
