using ON.Authentication;
using ON.Fragments.Authentication;
using ON.Fragments.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCNRWeb.Models.CMS
{
    public class ManageViewModel
    {
        public ManageViewModel() { }

        public List<ContentListRecord> Records { get; set; } = new List<ContentListRecord>();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
