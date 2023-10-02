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
    public class EditWrittenViewModel
    {
        public EditWrittenViewModel() { }

        public string ID { get; set; }

        [Required]
        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = "{0} length must be less than {1}.")]
        public string Title { get; set; }

        [Display(Name = "Subtitle")]
        [StringLength(1000, ErrorMessage = "{0} length must be less than {1}.")]
        public string Subtitle { get; set; }

        [Display(Name = "Category")]
        public string CategoryID { get; set; }

        [Display(Name = "Tags (comma separated)")]
        [RegularExpression(@"^[A-Za-z0-9,-]*$", ErrorMessage = "Tags can only be letters, numbers, and dashes.")]
        [StringLength(1000, ErrorMessage = "{0} length must be less than {1}.")]
        public string Tags { get; set; }

        [Display(Name = "Minimum subscription to view")]
        [DataType(DataType.Currency)]
        public uint Level { get; set; }

        [Required]
        [Display(Name = "Author")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string Author { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }

        public string FeaturedImageAssetID { get; set; }

        public IFormFile File { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
