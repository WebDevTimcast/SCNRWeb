using ON.Fragments.Content;
using ON.Fragments.Settings;
using SCNRWeb.Helper;
using System;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace SCNRWeb.Models
{
    public class NewsViewModel
    {
        public ContentPublicRecord Record { get; set; }
        public ContentListRecord NextRecord { get; set; } = null;
        public CategoryRecord Category { get; set; } = null;

        public string FixedRecordBody => FixRecordBody(Record?.Data?.Written?.HtmlBody);

        private string FixRecordBody(string str)
        {
            return EmbedHelper.Process(str);
        }
    }
}
