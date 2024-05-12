using ON.Authentication;
using ON.Fragments.Authentication;
using ON.Fragments.Content;
using SCNRWeb.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCNRWeb.Models.CMS
{
    public class HomeViewModel
    {
        public HomeViewModel() { }

        public HomeViewModel(GetAllContentResponse news, GetAllContentResponse videos, ONUser user)
        {
            var live = videos.Records.Where(r => r.IsLiveStream && r.IsLive).OrderByDescending(r => r.PublishOnUTC).OrderByDescending(r => r.PinnedOnUTC).FirstOrDefault();
            if (live != null)
                LiveId = live.ContentIDGuid;

            //var allItems = news.Records.ToList();
            //allItems.AddRange(videos.Records);
            FeaturedItem = news.Records.OrderByDescending(r => r.PublishOnUTC).OrderByDescending(r => r.PinnedOnUTC).FirstOrDefault();

            News.AddRange(news.Records.Where(r => r != FeaturedItem));
            Videos.AddRange(videos.Records.Where(r => r != FeaturedItem));
        }

        public List<ContentListRecord> News { get; } = new();
        public List<ContentListRecord> Videos { get; } = new();

        public ContentListRecord FeaturedItem { get; } = new();

        public Guid LiveId { get; } = Guid.Empty;
        public ContentPublicRecord? LiveVideo { get; set; } = null;
    }
}
