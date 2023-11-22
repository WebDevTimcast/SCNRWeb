using ON.Fragments.Content;
using SCNRWeb.Helper;
using System.Linq;

namespace SCNRWeb.Models.CMS
{
    public class ExportModel
    {
        public ExportItemModel[] Items { get; set; }

        public ExportModel(GetAllContentResponse items, ContentUrlHelper cUrl)
        {
            Items = items.Records.Where(i => i != null).Select(i => new ExportItemModel(i, cUrl)).ToArray();
        }

        public class ExportItemModel
        {
            public string Id { get; }
            public string Title { get; }
            public string Author { get; }
            public string Url { get; }
            public string Image { get; }
            public string Time { get; }

            public ExportItemModel(ContentListRecord item, ContentUrlHelper cUrl)
            {
                Id = item.ContentID ?? "";
                Title = item.Title ?? "";
                Author = item.Author ?? "";
                Url = cUrl.GenerateFullArticleUrl(item);
                Image = cUrl.GenerateFullImageUrl(item);
                Time = item.PublishOnUTC.ToDateTime().ToLocalTime().ToString("MM.d.yy");

                if (Title.Length > 305)
                    Title = Title.Substring(0, 305);
            }
        }
    }
}
