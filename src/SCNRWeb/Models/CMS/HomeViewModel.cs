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

        public HomeViewModel(IEnumerable<ContentListRecord> records, ONUser user)
        {
            var list = records.ToList();

            PinnedRecords.AddRange(list.Where(r => r.PinnedOnUTC != null));
            Records.AddRange(list.Where(r => r.PinnedOnUTC == null));
        }

        public HomeViewModel(GetAllContentResponse contentResponse, ONUser user)
        {
            if (contentResponse?.Records == null)
                return;

            var list = contentResponse.Records.ToList();

            PinnedRecords.AddRange(list.Where(r => r.PinnedOnUTC != null));
            Records.AddRange(list.Where(r => r.PinnedOnUTC == null));
        }

        public HomeViewModel(SearchContentResponse contentResponse, ONUser user)
        {
            if (contentResponse?.Records == null)
                return;

            var list = contentResponse.Records.ToList();

            PinnedRecords.AddRange(list.Where(r => r.PinnedOnUTC != null));
            Records.AddRange(list.Where(r => r.PinnedOnUTC == null));
        }

        public List<ContentListRecord> PinnedRecords { get; } = new List<ContentListRecord>();
        public List<ContentListRecord> Records { get; } = new List<ContentListRecord>();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
