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
    public class SearchViewModel
    {
        public SearchViewModel() { }

        public SearchViewModel(SearchContentResponse response, ONUser user)
        {
            Records.AddRange(response.Records);
        }

        public List<ContentListRecord> Records { get; } = new();

        public List<ContentListRecord> PagedRecords { get; set; } = new();
        public string Query { get; set; } = null;
        public PageNumViewModel PageVM { get; set; } = null;
    }
}
