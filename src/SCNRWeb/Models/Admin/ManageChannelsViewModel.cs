using ON.Authentication;
using ON.Fragments.Authentication;
using ON.Fragments.Content;
using ON.Fragments.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCNRWeb.Models.Admin
{
    public class ManageChannelsViewModel
    {
        public ManageChannelsViewModel() { }

        public List<ChannelRecord> Records { get; set; } = new List<ChannelRecord>();
    }
}
