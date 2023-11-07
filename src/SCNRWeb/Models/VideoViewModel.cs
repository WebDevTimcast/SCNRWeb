using ON.Fragments.Content;
using ON.Fragments.Settings;
using System;

namespace SCNRWeb.Models
{
    public class VideoViewModel
    {
        public ContentPublicRecord Record { get; set; }
        public ContentListRecord NextRecord { get; set; } = null;
        public ChannelRecord Channel { get; set; } = null;
    }
}
