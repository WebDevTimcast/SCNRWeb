using ON.Authentication;
using ON.Fragments.Content;
using System.Collections.Generic;
using System.Linq;

namespace SCNRWeb.Models.CMS.News
{
    public class NewsIndexViewModel
    {
        public List<ContentListRecord> Records { get; set; } = new();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
