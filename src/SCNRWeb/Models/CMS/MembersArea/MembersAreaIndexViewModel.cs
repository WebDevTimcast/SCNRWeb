using ON.Fragments.Content;
using ON.Fragments.Settings;
using System.Collections.Generic;

namespace SCNRWeb.Models.CMS.MembersArea
{
    public class MembersAreaIndexViewModel
    {
        public ContentListRecord MainRecord { get; set; } = new();
        public Dictionary<ChannelRecord, List<ContentListRecord>> Channels { get; set; } = new();
    }
}
