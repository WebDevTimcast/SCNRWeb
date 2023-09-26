using ON.Authentication;
using ON.Fragments.Content;
using System;
using System.Collections.Generic;

namespace SubverseWeb.Models.CMS
{
    public class ByMonthViewModel
    {
        public ByMonthViewModel(IEnumerable<ContentListRecord> records, ONUser user)
        {
            Records.AddRange(records);
        }

        public ByMonthViewModel(GetAllContentResponse contentResponse, ONUser user)
        {
            if (contentResponse?.Records == null)
                return;

            Records.AddRange(contentResponse.Records);
        }

        public DateTime Date { get; set; }
        public List<ContentListRecord> Records { get; } = new List<ContentListRecord>();
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
