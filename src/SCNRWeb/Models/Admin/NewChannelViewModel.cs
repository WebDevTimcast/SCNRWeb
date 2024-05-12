using Microsoft.AspNetCore.Mvc.Rendering;
using ON.Authentication;
using ON.Fragments.Authentication;
using ON.Fragments.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCNRWeb.Models.Admin
{
    public class NewChannelViewModel
    {
        public NewChannelViewModel() { }

        [Required]
        [Display(Name = "Display Name")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Url Stub")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string UrlStub { get; set; }

        [Display(Name = "Youtube Url")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string YoutubeUrl { get; set; }

        [Display(Name = "Rumble Url")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string RumbleUrl { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        internal ChannelRecord ToChannel()
        {
            return new ChannelRecord()
            {
                ChannelId = Guid.NewGuid().ToString(),
                DisplayName = DisplayName ?? "Missing",
                UrlStub = UrlStub ?? "",
                ImageAssetId = "",
                YoutubeUrl = YoutubeUrl ?? "",
                RumbleUrl = RumbleUrl ?? "",
            };
        }
    }
}
