using ON.Fragments.Content;
using System.Collections.Generic;

namespace SCNRWeb.Models.CMS.Channels
{
    public class ChannelsIndexViewModel
    {
        public List<ContentListRecord> ContentRecords { get; set; } = new();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
