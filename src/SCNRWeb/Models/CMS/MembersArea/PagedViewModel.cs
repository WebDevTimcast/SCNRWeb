using ON.Fragments.Content;
using System.Collections.Generic;

namespace SCNRWeb.Models.CMS.MembersArea
{
    public class PagedViewModel
    {
        public List<ContentListRecord> ContentRecords { get; set; } = new();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
