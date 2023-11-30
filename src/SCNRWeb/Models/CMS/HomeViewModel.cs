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
    public class HomeViewModel
    {
        public HomeViewModel() { }

        public HomeViewModel(GetAllContentResponse news, GetAllContentResponse videos, ONUser user)
        {
            var allItems = news.Records.ToList();
            allItems.AddRange(videos.Records);
            FeaturedItem = allItems.OrderByDescending(r => r.PublishOnUTC).OrderByDescending(r => r.PinnedOnUTC).FirstOrDefault();

            News.AddRange(news.Records.Where(r => r != FeaturedItem));
            Videos.AddRange(videos.Records.Where(r => r != FeaturedItem));
        }

        public List<ContentListRecord> News { get; } = new();
        public List<ContentListRecord> Videos { get; } = new();

        public ContentListRecord FeaturedItem { get; } = new();
    }
}
