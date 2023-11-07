using Microsoft.AspNetCore.Http;
using System;

namespace SCNRWeb.Models
{
    public class LeftNavModel
    {
        public LeftNavModel(IHttpContextAccessor context)
        {
            switch (context.HttpContext.Request.Path.Value.ToLower())
            {
                case "/":
                    IsHome = true;
                    break;
                case string s when s.StartsWith("/about-us"):
                    IsAbout = true;
                    break;
                case string s when s.StartsWith("/staff"):
                    IsStaff = true;
                    break;
                case string s when s.StartsWith("/contact"):
                    IsContact = true;
                    break;
                case string s when s.StartsWith("/videos"):
                    IsWatch = true;
                    break;
                case string s when s.StartsWith("/news"):
                    IsRead = true;
                    break;
            }

        }

        public bool IsHome { get; set; } = false;
        public bool IsAbout { get; set; } = false;
        public bool IsStaff { get; set; } = false;
        public bool IsContact { get; set; } = false;
        public bool IsRead { get; set; } = false;
        public bool IsWatch { get; set; } = false;

    }
}
