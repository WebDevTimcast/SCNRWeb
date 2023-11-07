using ON.Fragments.Authentication;
using ON.Fragments.Content;
using System.Collections.Generic;

namespace SCNRWeb.Models.CMS.News
{
    public class AuthorViewModel
    {
        public List<ContentListRecord> PagedRecords { get; set; } = new();
        public UserPublicRecord Author { get; set; } = null;
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
