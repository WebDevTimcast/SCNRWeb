using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using ON.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCNRWeb.Models.CMS
{
    public class EditVideoViewModel
    {
        public EditVideoViewModel() { }

        public string ID { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string Title { get; set; }

        [Display(Name = "Subtitle")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string Subtitle { get; set; }

        [Display(Name = "Channel")]
        public string ChannelID { get; set; }

        [Display(Name = "Minimum subscription to view")]
        [DataType(DataType.Currency)]
        public uint Level { get; set; }

        [Display(Name = "Author")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string Author { get; set; }

        [Display(Name = "Rumble Video Id")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string RumbleVideoId { get; set; }

        [Display(Name = "Youtube Video Id")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string YoutubeVideoId { get; set; }

        [Display(Name = "Is livestream")]
        public bool IsLiveStream { get; set; }

        [Display(Name = "Is currently live")]
        public bool IsLive { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }

        public string FeaturedImageAssetID { get; set; }

        public IFormFile File { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public List<SelectListItem> Channels { get; set; }
    }
}
