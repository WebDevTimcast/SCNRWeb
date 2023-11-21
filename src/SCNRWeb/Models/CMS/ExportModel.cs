using ON.Fragments.Content;
using System.Linq;

namespace SCNRWeb.Models.CMS
{
    public class ExportModel
    {
        public ExportItemModel[] Items { get; set; }

        public ExportModel(GetAllContentResponse items)
        {
            Items = items.Records.Where(i => i != null).Select(i => new ExportItemModel(i)).ToArray();
        }

        public class ExportItemModel
        {
            public string Id { get; }
            public string Title { get; }
            public string Url { get; }
            public string Image { get; }
            public string Time { get; }

            public ExportItemModel(ContentListRecord item)
            {
                Id = item.ContentID;
                Title = item.Title;
                Url = $"https://scnr.com/content/{item.ContentID}/{item.URL}";
                Image = $"https://scnr.com/image/{item.FeaturedImageAssetID}";
                Time = item.PublishOnUTC.ToDateTime().ToLocalTime().ToString("MM.d.yy");
            }
        }
    }
}
