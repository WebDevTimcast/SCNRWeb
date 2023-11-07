using ON.Fragments.Content;
using ON.Fragments.Settings;
using System.Collections.Generic;

namespace SCNRWeb.Models.CMS.Channels
{
    public class ChannelViewModel
    {
        public ChannelRecord ChannelRecord { get; set; }
        public List<ContentListRecord> ContentRecords { get; set; } = new();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
