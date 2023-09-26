using ON.Fragments.Content;
using ON.Fragments.Settings;
using System.Collections.Generic;

namespace SCNRWeb.Models.CMS
{
    public class CategoryViewModel
    {
        public CategoryRecord CategoryRecord { get; set; }
        public List<ContentListRecord> ContentRecords { get; set; } = new();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
