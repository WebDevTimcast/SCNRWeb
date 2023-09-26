using ON.Authentication;
using ON.Fragments.Authentication;
using ON.Fragments.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SubverseWeb.Models.CMS
{
    public class HomeViewModel
    {
        public HomeViewModel() { }

        public HomeViewModel(IEnumerable<ContentListRecord> records, ONUser user)
        {
            Records.AddRange(records);
        }

        public HomeViewModel(GetAllContentResponse contentResponse, ONUser user)
        {
            if (contentResponse?.Records == null)
                return;
            Records.AddRange(contentResponse.Records);
        }

        public HomeViewModel(SearchContentResponse contentResponse, ONUser user)
        {
            if (contentResponse?.Records == null)
                return;
            Records.AddRange(contentResponse.Records);
        }

        public List<ContentListRecord> Records { get; } = new List<ContentListRecord>();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
