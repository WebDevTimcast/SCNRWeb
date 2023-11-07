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

        public HomeViewModel(GetAllContentResponse response, ONUser user)
        {
            MainVideo = response.Records.FirstOrDefault(r => r.PinnedOnUTC != null);

            Records.AddRange(response.Records.Where(r => r != MainVideo));
        }

        public HomeViewModel(SearchContentResponse response, ONUser user)
        {
            Records.AddRange(response.Records);

            MainVideo = response.Records.FirstOrDefault(r => r.IsLiveStream);
        }

        public List<ContentListRecord> Records { get; } = new();

        public ContentListRecord MainVideo { get; } = new();
    }
}
