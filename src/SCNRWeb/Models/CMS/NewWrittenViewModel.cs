﻿using Microsoft.AspNetCore.Mvc.Rendering;
using ON.Authentication;
using ON.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCNRWeb.Models.CMS
{
    public class NewWrittenViewModel
    {
        public NewWrittenViewModel() { }

        public NewWrittenViewModel(ONUser user)
        {
            Author = user.DisplayName;
        }

        [Required]
        [Display(Name = "Title")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
        public string Title { get; set; }

        [Display(Name = "Subtitle")]
        [StringLength(100, ErrorMessage = "{0} length must be less than {1}.")]
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

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public List<SelectListItem> Categories { get; set; }
    }
}
