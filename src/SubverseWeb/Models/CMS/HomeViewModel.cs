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

            ShowLockStatus = !(user?.IsWriterOrHigher ?? false);
            UserSubscriptionLevel = user?.SubscriptionLevel ?? 0;
        }

        public bool ShowLockStatus { get; set; }

        public uint UserSubscriptionLevel { get; set; } = 0;

        public List<ContentListRecord> Records { get; } = new List<ContentListRecord>();

        public IEnumerable<ContentListRecord> EvenRecords => SkipEvenOrOdd(Records, true);
        public IEnumerable<ContentListRecord> OddRecords => SkipEvenOrOdd(Records, false);

        private IEnumerable<ContentListRecord> SkipEvenOrOdd(IEnumerable<ContentListRecord> records, bool wantEven)
        {
            bool flip = wantEven;

            foreach (var record in records)
            {
                if (flip)
                    yield return record;
                flip = !flip;
            }
        }
    }
}
